using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class SpawnSpot : MonoBehaviour
{
    [SerializeField] private GameObject Prefab_SpawnTarget;

    [SerializeField] private string _spawnDataId;

    [SerializeField] private bool _spawnOnStart = true;

    [SerializeField] private bool _enableRespawn = true;

    private int _spawnedInstanceId = -1;

    //JU ToDo 데이터 드리븐으로 주기 설정하기
    [SerializeField] private float _respawnDelay = 5f;

    private CancellationTokenSource _respawnCancellationTokenSource;

    public event Action<SpawnSpot, int> OnSpawnedObjectDisabled;


    public int SpawnedIstanceId
    {
        get { return _spawnedInstanceId; }
    }
    public string SpawnDataId
    {
        get { return _spawnDataId; }
    }

    public bool HasAliveSpawnedObject
    {
        get
        {
            if (_spawnedInstanceId < 0)
            {
                return false;
            }

            if (GameObjectManager.Instance == null)
            {
                return false;
            }

            GameObject spawnObject = GameObjectManager.Instance.GetGameObjectCanBeNull(_spawnedInstanceId);

            if (spawnObject == null)
            {
                return false;
            }

            return spawnObject.activeInHierarchy;
        }
    }


    private void Start()
    {
        if (_spawnOnStart == true)
        {
            RequestSpawn();
        }
    }

    private void OnDisable()
    {
        CancelRespawnTask();

        OnSpawnedObjectDisabled = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(_respawnDelay < 0f)
        {
            _respawnDelay = 0f;
        }
    }
#endif



    public void RequestSpawn()
    {
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning("GameObjectManager가 존재하지 않아 스폰할 수 없습니다.");
            return;
        }

        if (Prefab_SpawnTarget == null)
        {
            Debug.LogWarning($"SpawnSpot에 Prefab_SpawnTarget이 등록되지 않았습니다. ObjectName: {gameObject.name}");
            return;
        }

        if (_spawnedInstanceId >= 0)
        {
            GameObject spawnObject = GameObjectManager.Instance.GetGameObjectCanBeNull(_spawnedInstanceId);
            if (spawnObject != null && spawnObject.activeSelf == true)
            {
                Debug.LogWarning($"[{gameObject.name}] 이미 활성화된 오브젝트가 있습니다. InstanceId: {_spawnedInstanceId}");
                return;
            }
        }


        _spawnedInstanceId = GameObjectManager.Instance.RequestSpawnGameObject(
            Prefab_SpawnTarget,
            transform.position,
            transform.rotation,
            _spawnDataId,
            this
        );

        if (_spawnedInstanceId < 0)
        {
            Debug.LogWarning($"[{gameObject.name}] 오브젝트 스폰 요청에 실패했습니다.");

            return;
        }

        Debug.Log($"[{gameObject.name}] 오브젝트 스폰 완료.InstanceId: {_spawnedInstanceId}");

    }

    public void NotifySpawnedObjectDisabled(int disabledInstanceId)
    {
        if (_spawnedInstanceId != disabledInstanceId)
        {
            Debug.LogWarning($"[{gameObject.name}] 관리 중인 InstanceId와 비활성화된 InstanceId가 다릅니다. " +
                $"SpawnedId: {_spawnedInstanceId},DisabledId: {disabledInstanceId}");
            return;
        }
        _spawnedInstanceId = -1;

        OnSpawnedObjectDisabled?.Invoke(this, disabledInstanceId);

        if(_enableRespawn == false)
        {
            Debug.Log($"[{gameObject.name}] 자동 리스폰이 비활성화되어 있습니다.");
            return ;
        }

        StartRespawnTask();
    }

    private void StartRespawnTask()
    {
        CancelRespawnTask();

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        _respawnCancellationTokenSource = cancellationTokenSource;

        RespawnAfterDelayAsync(cancellationTokenSource).Forget();
    }

    private async UniTask RespawnAfterDelayAsync(CancellationTokenSource cancellationTokenSource)
    {
        float delay = Mathf.Max(0f, _respawnDelay);
        Debug.Log($"[{gameObject.name}] {delay}초 후 다시 스폰합니다.");

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

        RequestSpawn();
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



    public void RequestDisableSpawnedObject()
    {
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameObjectManager가 존재하지 않아 오브젝트를 비활성화할 수 없습니다.");

            return;
        }
        if (_spawnedInstanceId < 0)
        {
            Debug.LogWarning("아직 생성된 오브젝트가 없습니다.");
            return;
        }

        GameObjectManager.Instance.RequestDisableGameObject(_spawnedInstanceId);
    }
}
