using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SpawnSpot : MonoBehaviour
{
    [Header("웨이브 몬스터 설정")]
    [SerializeField] private List<SpawnEntry> _spawnEntryList = new List<SpawnEntry>();

    [Header("몬스터 생성 위치")]
    [SerializeField] private List<Transform> _spawnPointList = new List<Transform>();

    [Header("스폰 시작 설정")]
    [SerializeField] private bool _spawnOnStart = true;

    [Header("웨이브 리스폰 설정")]
    [SerializeField] private bool _enableWaveRespawn = true;


    [Min(0f)]
    [SerializeField] private float _respawnDelay = 5f;

    private readonly HashSet<int> _spawnedInstanceIdSet = new HashSet<int>();

    private CancellationTokenSource _respawnCancellationTokenSource;

    private bool _hasActiveWave;

    private bool _isSpawnOperationActive;

    public event Action<SpawnSpot> OnWaveSpawned;

    public event Action<SpawnSpot> OnWaveCleared;

    public event Action<SpawnSpot, int> OnSpawnedObjectDisabled;

    public int AliveCount
    {
        get
        {
            return _spawnedInstanceIdSet.Count;
        }
    }

    public bool HasAliveSpawnedObject
    {
        get
        {
            return _spawnedInstanceIdSet.Count > 0;
        }
    }

    public bool IsWaitingRespawn
    {
        get
        {
            return _respawnCancellationTokenSource != null;
        }
    }

    public bool IsWaveRespawnEnabled
    {
        get
        {
            return _enableWaveRespawn;
        }
    }

    public bool IsSpawnOperationActive
    {
        get
        {
            return _isSpawnOperationActive;
        }
    }

    private void Start()
    {
        if (_spawnOnStart == false)
        {
            return;
        }

        ActivateSpawnSpot();
    }

    private void OnDisable()
    {
        DeactivateSpawnSpot();
    }
    private void OnDestroy()
    {
        CancelRespawnTask();

        OnWaveSpawned = null;
        OnWaveCleared = null;
        OnSpawnedObjectDisabled = null;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_respawnDelay < 0f)
        {
            _respawnDelay = 0f;
        }
        if (_spawnEntryList == null)
        {
            return;
        }
        foreach (SpawnEntry spawnEntry in _spawnEntryList)
        {
            if (spawnEntry == null)
            {
                continue;
            }
            spawnEntry.Validate();
        }
    }
#endif

    public void ActivateSpawnSpot()
    {
        if (_isSpawnOperationActive == true)
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] SpawnSpot이 이미 작동 중입니다.");
            return;
        }

        _isSpawnOperationActive = true;
        RequestSpawnWave();
    }

    public void DeactivateSpawnSpot()
    {
        _isSpawnOperationActive = false;
        CancelRespawnTask();
        RequestDisableAllSpawnedObjects();

        _hasActiveWave = false;
    }

    public void RequestSpawn()
    {
        if (_isSpawnOperationActive == false)
        {
            ActivateSpawnSpot();
            return;
        }
        RequestSpawnWave();
    }

    public void RequestSpawnWave()
    {
        if (_isSpawnOperationActive == false)
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] SpawnSpot이 비활성 상태여서 웨이브를 생성할 수 없습니다.");
            return;
        }

        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] GameObjectManager가 없어 웨이브를 생성할 수 없습니다.");
            return;
        }
        if (_spawnEntryList == null || _spawnEntryList.Count == 0)
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] SpawnEntry가 등록되지 않았습니다.");
            return;
        }
        if (_spawnedInstanceIdSet.Count > 0)
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] 아직 살아 있는 몬스터가 있어 새 웨이브를 생성할 수 없습니다. " +
                $"AliveCount: {_spawnedInstanceIdSet.Count}");
            return;
        }
        if (IsWaitingRespawn == true)
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] 현재 리스폰 대기 중이므로 웨이브를 중복 생성할 수 없습니다.");
            return;
        }
        _hasActiveWave = false;

        int requestedSpawnCount = 0;
        int successfulSpawnCount = 0;
        int spawnPositionIndex = 0;

        foreach (SpawnEntry spawnEntry in _spawnEntryList)
        {
            if (spawnEntry == null)
            {
                Debug.LogWarning($"SpawnSpot: [{gameObject.name}] SpawnEntry 목록에 null 항목이 있습니다.");
                continue;
            }
            if (ValidateSpawnEntry(spawnEntry) == false)
            {
                continue;
            }
            for (int spawnIndex = 0; spawnIndex < spawnEntry.SpawnCount; spawnIndex++)
            {
                requestedSpawnCount++;

                Vector3 spawnPosition = GetSpawnPosition(spawnPositionIndex);

                Quaternion spawnRotation = GetSpawnRotation(spawnPositionIndex);

                int instanceId = GameObjectManager.Instance.RequestSpawnGameObject(spawnEntry.MonsterPrefab, spawnPosition, spawnRotation, spawnEntry.MonsterDataId, this);

                spawnPositionIndex++;

                if (instanceId < 0)
                {
                    Debug.LogWarning($"SpawnSpot: [{gameObject.name}] 몬스터 생성에 실패했습니다. MonsterDataId: {spawnEntry.MonsterDataId}");
                    continue;
                }

                bool wasAdded = _spawnedInstanceIdSet.Add(instanceId);

                if (wasAdded == false)
                {
                    Debug.LogWarning($"SpawnSpot: [{gameObject.name}] 동일한 InstanceId가 중복 등록되었습니다. InstanceId: {instanceId}");
                    continue;
                }
                successfulSpawnCount++;
            }
        }

        if (successfulSpawnCount <= 0)
        {
            _hasActiveWave = false;

            Debug.LogWarning($"[{gameObject.name}] 생성에 성공한 몬스터가 없습니다. 요청 수: {requestedSpawnCount}");
            return;
        }
        _hasActiveWave = true;

        Debug.Log($"SpawnSpot: [{gameObject.name}] 웨이브 생성 완료. 요청 수: {requestedSpawnCount}, 성공 수: {successfulSpawnCount}");

        OnWaveSpawned?.Invoke(this);
    }

    private bool ValidateSpawnEntry(SpawnEntry spawnEntry)
    {
        if (spawnEntry.MonsterPrefab == null)
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] SpawnEntry의 몬스터 프리팹이 등록되지 않았습니다.");
            return false;
        }
        if (string.IsNullOrWhiteSpace(spawnEntry.MonsterDataId))
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] SpawnEntry의 MonsterDataId가 비어 있습니다.");
            return false;
        }

        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameDataManager가 없어 MonsterData를 검증할 수 없습니다.");
            return false;
        }

        MonsterData monsterData = GameDataManager.Instance.GetMonsterData(spawnEntry.MonsterDataId);

        if (monsterData == null)
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] MonsterData를 찾을 수 없습니다. MonsterDataId: {spawnEntry.MonsterDataId}");

            return false;
        }
        return true;
    }

    public void NotifySpawnedObjectDisabled(int disabledInstanceId)
    {
        bool wasRemoved = _spawnedInstanceIdSet.Remove(disabledInstanceId);

        if (wasRemoved == false)
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] 이 SpawnSpot이 관리하지 않는 InstanceId가 전달되었습니다. InstanceId: {disabledInstanceId}", this);
            return;
        }
        Debug.Log($"SpawnSpot: [{gameObject.name}] 몬스터 비활성화.InstanceId: {disabledInstanceId},남은 몬스터 수: {_spawnedInstanceIdSet.Count}");

        OnSpawnedObjectDisabled?.Invoke(this, disabledInstanceId);

        if (_spawnedInstanceIdSet.Count > 0)
        {
            return;
        }
        if (_hasActiveWave == false)
        {
            return;
        }

        _hasActiveWave = false;

        if (_isSpawnOperationActive == false)
        {
            return;
        }

        Debug.Log($"SpawnSpot: [{gameObject.name}] 현재 웨이브의 모든 몬스터가 비활성화되었습니다.", this);

        OnWaveCleared?.Invoke(this);


        if (_enableWaveRespawn == false)
        {
            return;
        }

        StartRespawnTask();
    }

    public void RequestDisableAllSpawnedObjects()
    {
        if (_spawnedInstanceIdSet.Count == 0)
        {
            return;
        }
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning($"SpawnSpot: [{gameObject.name}] GameObjectManager가 없어 생성된 몬스터를 반환할 수 없습니다.");
            return;
        }

        List<int> instanceIdList = new List<int>(_spawnedInstanceIdSet);

        foreach (int instanceId in instanceIdList)
        {
            GameObjectManager.Instance.RequestDisableGameObject(instanceId);
        }
    }

    public void SetWaveRespawnEnabled(bool isEnabled)
    {
        _enableWaveRespawn = isEnabled;

        if (_enableWaveRespawn == false)
        {
            CancelRespawnTask();
        }
    }


    private void StartRespawnTask()
    {
        if (_isSpawnOperationActive == false)
        {
            return;
        }
        if (_enableWaveRespawn == false)
        {
            return;
        }

        if (_spawnedInstanceIdSet.Count > 0)
        {
            return;
        }

        CancelRespawnTask();

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        _respawnCancellationTokenSource = cancellationTokenSource;

        RespawnAfterDelayAsync(cancellationTokenSource).Forget();
    }

    private async UniTask RespawnAfterDelayAsync(CancellationTokenSource cancellationTokenSource)
    {
        float delay = Mathf.Max(0f, _respawnDelay);
        Debug.Log($"SpawnSpot: [{gameObject.name}] {delay}초 후 다시 스폰합니다.");

        bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationTokenSource.Token).SuppressCancellationThrow();

        if (isCanceled == true)
        {
            return;
        }

        if (_respawnCancellationTokenSource != cancellationTokenSource)
        {
            return;
        }

        cancellationTokenSource.Dispose();
        _respawnCancellationTokenSource = null;

        if (_isSpawnOperationActive == false)
        {
            return;
        }
        if (_enableWaveRespawn == false)
        {
            return;
        }
        if (_spawnedInstanceIdSet.Count > 0)
        {
            return;
        }

        RequestSpawnWave();
    }

    private void CancelRespawnTask()
    {
        if (_respawnCancellationTokenSource == null)
        {
            return;
        }

        _respawnCancellationTokenSource.Cancel();
        _respawnCancellationTokenSource.Dispose();
        _respawnCancellationTokenSource = null;
    }

    private void CleanInactiveSpawnObjectIds()
    {
        if (_spawnedInstanceIdSet.Count == 0)
        {
            return;
        }
        if (GameObjectManager.Instance == null)
        {
            return;
        }

        List<int> inactiveInstanceIdList = new List<int>();

        foreach (int instanceId in _spawnedInstanceIdSet)
        {
            GameObject spawnObject = GameObjectManager.Instance.GetGameObjectCanBeNull(instanceId);

            if (spawnObject == null)
            {
                inactiveInstanceIdList.Add(instanceId);
                continue;
            }
            if (spawnObject.activeSelf == false)
            {
                inactiveInstanceIdList.Add(instanceId);
            }
        }
        foreach (int inactiveInstanceId in inactiveInstanceIdList)
        {
            _spawnedInstanceIdSet.Remove(inactiveInstanceId);
            Debug.Log($"SpawnSpot: [{gameObject.name}] 비활성화된 몬스터의 추적 정보를 정리했습니다. InstanceId: {inactiveInstanceId}", this);
        }
    }

    private Transform GetSpawnPointCanBeNull(int spawnIndex)
    {
        if (_spawnPointList == null || _spawnPointList.Count == 0)
        {
            return null;
        }
        int ValidSpawnPointCount = 0;

        foreach (Transform spawnPoint in _spawnPointList)
        {
            if (spawnPoint != null)
            {
                ValidSpawnPointCount++;
            }
        }

        if (ValidSpawnPointCount == 0)
        {
            return null;
        }

        int selectedValidIndex = spawnIndex % ValidSpawnPointCount;

        int currentValidIndex = 0;

        foreach (Transform spawnPoint in _spawnPointList)
        {
            if (spawnPoint == null)
            {
                continue;
            }

            if (currentValidIndex == selectedValidIndex)
            {
                return spawnPoint;
            }

            currentValidIndex++;
        }
        return null;
    }

    private Vector3 GetSpawnPosition(int spawnIndex)
    {
        Transform spawnPoint = GetSpawnPointCanBeNull(spawnIndex);

        if (spawnPoint == null)
        {
            return transform.position;
        }
        return spawnPoint.position;
    }

    private Quaternion GetSpawnRotation(int spawnIndex)
    {
        Transform spawnPoint = GetSpawnPointCanBeNull(spawnIndex);

        if (spawnPoint == null)
        {
            return transform.rotation;
        }
        return spawnPoint.rotation;
    }


#if UNITY_EDITOR
    [ContextMenu("TEST/SpawnSpot 활성화")]
    private void TestActivateSpawnSpot()
    {
        ActivateSpawnSpot();
    }
    [ContextMenu("TEST/SpawnSpot 비활성화")]
    private void TestDeactivateSpawnSpot()
    {
        DeactivateSpawnSpot();
    }
    [ContextMenu("TEST/웨이브 전체 반환")]
    private void TestDisableAllSpawnedObjects()
    {
        RequestDisableAllSpawnedObjects();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (_spawnPointList == null || _spawnPointList.Count == 0)
        {
            Gizmos.DrawWireSphere(transform.position, 0.4f);
            return;
        }

        foreach (Transform spawnPoint in _spawnPointList)
        {
            if (spawnPoint == null)
            {
                continue;
            }

            Gizmos.DrawWireSphere(spawnPoint.position, 0.4f);

            Gizmos.DrawLine(transform.position, spawnPoint.position);
        }
    }
#endif
}
