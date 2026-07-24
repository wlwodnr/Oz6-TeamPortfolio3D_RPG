using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public static GameObjectManager Instance { get; private set; }
    [SerializeField] private Transform Root_DynamicObject;
    [SerializeField] private TestMonsterHudController _hudController;

    private int _objectInstanceKeyGenerator = 0;

    private Dictionary<int, GameObject> _createdGameObjectContainer = new Dictionary<int, GameObject>();

    private Dictionary<int, GameObject> _prefabContainer = new Dictionary<int, GameObject>();

    private Dictionary<int, SpawnSpot> _spawnSpotContainer = new Dictionary<int, SpawnSpot>();

    private int _playerInstanceId = -1;

    public int PlayerInstanceId
    {
        get
        {
            return _playerInstanceId;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning($"중복된 GameObjectManager가 발견되었습니다: {gameObject.name}");
            Destroy(gameObject);
            return;
        }
    }

    public int RequestSpawnGameObject(GameObject prefab, Vector3 spawnPosition, Quaternion spawnRotation, string dataId, SpawnSpot ownerSpawnSpot = null)
    {
        if (prefab == null)
        {
            Debug.LogWarning("생성할 Prefab이 없습니다.");
            return -1;
        }
        if (Root_DynamicObject == null)
        {
            Debug.LogWarning("Root_DynamicObject가 등록되지 않았습니다.");
            return -1;
        }

        IGameObjectEntity prefabEntity = prefab.GetComponent<IGameObjectEntity>();

        if (prefabEntity == null)
        {
            Debug.LogWarning($"생성할 Prefab에 IGameObjectEntity가 없습니다. PrefabName: {prefab.name}");
            return -1;
        }

        if (TryReuseInactiveGameObject(prefab, spawnPosition, spawnRotation, dataId, ownerSpawnSpot, out int reusedInstanceId))
        {
            return reusedInstanceId;
        }

        GameObject createdObject = Instantiate(prefab, spawnPosition, spawnRotation, Root_DynamicObject);



        if (createdObject == null)
        {
            Debug.LogWarning("게임 오브젝트 생성에 실패했습니다.");
            return -1;
        }


        int instanceId = GenerateInstanceId();

        if (instanceId < 0)
        {
            Debug.LogWarning("InstanceId 생성에 실패했습니다.");
            createdObject.SetActive(false);
            return -1;
        }

        if (_createdGameObjectContainer.ContainsKey(instanceId) == true)
        {
            Debug.LogWarning($"이미 동일한 InstanceId가 존재합니다. InstanceId: {instanceId}");
            createdObject.SetActive(false);
            return -1;
        }

        IGameObjectEntity entity = createdObject.GetComponent<IGameObjectEntity>();

        if (entity == null)
        {
            Debug.LogWarning($"생성된 오브젝트에 IGameObjectEntity가 없습니다. ObjectName: {createdObject.name}");
            createdObject.SetActive(false);
            return -1;
        }


        _createdGameObjectContainer.Add(instanceId, createdObject);
        _prefabContainer[instanceId] = prefab;

        RegisterOwnerSpawnSpot(instanceId, ownerSpawnSpot);

        InitSpawnedObject(instanceId, createdObject, dataId, ownerSpawnSpot);

        // 신규 스폰시 HUD 생성
        EnemyStatus enemyStatus = createdObject.GetComponent<EnemyStatus>() ?? createdObject.GetComponentInChildren<EnemyStatus>(true);
        if (enemyStatus != null)
        {
            _hudController?.AddMonsterHud(instanceId, enemyStatus, createdObject.transform);
        }

        //JU 이 디버그는 테스트 후 삭제할 예정
        Debug.Log($"동적 오브젝트 생성 완료. InstanceId: {instanceId}, Name: {createdObject.name}");

        return instanceId;
    }

    private bool TryReuseInactiveGameObject(GameObject prefab, Vector3 spawnPosition, Quaternion spawnRotation, string dataId, SpawnSpot ownerSpawnSpot, out int reusedInstanceId)
    {
        foreach (var pair in _createdGameObjectContainer)
        {
            int instanceId = pair.Key;
            GameObject pooledObject = pair.Value;

            if (pooledObject == null)
            {
                continue;
            }
            if (pooledObject.activeSelf == true)
            {
                continue;
            }


            if (_prefabContainer.TryGetValue(instanceId, out GameObject originPrefab) == false)
            {
                continue;
            }


            if (originPrefab != prefab)
            {
                continue;
            }


            pooledObject.transform.position = spawnPosition;
            pooledObject.transform.rotation = spawnRotation;

            IGameObjectEntity entity = pooledObject.GetComponent<IGameObjectEntity>();

            if (entity == null)
            {
                Debug.LogWarning(
                    $"재사용할 오브젝트에 IGameObjectEntity가 없습니다. ObjectName: {pooledObject.name}");

                continue;
            }

            entity.ResetEntity();

            RegisterOwnerSpawnSpot(instanceId, ownerSpawnSpot);

            entity.InitEntity(instanceId, dataId);

            RegisterPlayerObject(instanceId, pooledObject);

            EnemyAI enemyAI = pooledObject.GetComponent<EnemyAI>();

            if (enemyAI == null)
            {
                enemyAI =
                    pooledObject.GetComponentInChildren<EnemyAI>(true);
            }

            if (enemyAI != null)
            {
                enemyAI.InitEnemyInfo(instanceId, dataId, ownerSpawnSpot);

                enemyAI.ResetEnemyAIForPool(ownerSpawnSpot);
            }

            if (pooledObject.activeSelf == false)
            {
                pooledObject.SetActive(true);
            }

            // 풀링 재사용 시 HUD 생성
            EnemyStatus enemyStatus = pooledObject.GetComponent<EnemyStatus>() ?? pooledObject.GetComponentInChildren<EnemyStatus>(true);
            if (enemyStatus != null)
            {
                _hudController?.AddMonsterHud(instanceId, enemyStatus, pooledObject.transform);
            }

            reusedInstanceId = instanceId;

            Debug.Log($"비활성화된 오브젝트 재사용 완료. InstanceId: {instanceId}, Name: {pooledObject.name}");

            return true;
        }
        reusedInstanceId = -1;
        return false;
    }

    private void RegisterPlayerObject(int instanceId, GameObject targetObject)
    {
        if (targetObject == null)
        {
            return;
        }

        PlayerEntity playerEntity = targetObject.GetComponent<PlayerEntity>();

        if (playerEntity == null)
        {
            return;
        }

        if (playerEntity.InstanceId != instanceId)
        {
            Debug.Log($"GameObjectManager: 플레이어 등록에 사용할 InstanceId와 PlayerEntity의 InstanceId가 다릅니다. " +
                $"등록할 ID: {instanceId}, PlayerEntity의 ID: {playerEntity.InstanceId}");
            return;
        }
        if (_playerInstanceId > 0 && _playerInstanceId != instanceId)
        {
            Debug.LogWarning($"GameObjectManager: 이미 다른 플레이어가 등록되어 있어 플레이어 InstanceId를 교체합니다. " +
                $"PreviousId: {_playerInstanceId}, NewId: {instanceId}");
            return;
        }
        _playerInstanceId = instanceId;
        Debug.Log($"GameObjectManager: 플레이어 오브젝트 등록 완료. InstanceId: {_playerInstanceId}, ObjectName: {targetObject.name}");
    }

    public bool TryGetPlayerObject(out int playerInstanceId, out GameObject playerObject)
    {
        playerInstanceId = -1;
        playerObject = null;

        if (_playerInstanceId <= 0)
        {
            Debug.LogWarning("GameObjectManager: 등록된 플레이어 InstanceId가 없습니다.");
            return false;
        }
        if (_createdGameObjectContainer.TryGetValue(_playerInstanceId, out GameObject registeredPlayerObject) == false)
        {
            Debug.LogWarning($"GameObjectManager: 플레이어 InstanceId에 해당하는 오브젝트가 없습니다. InstanceId: {_playerInstanceId}");
            return false;
        }
        if (registeredPlayerObject == null)
        {
            Debug.LogWarning($"GameObjectManager: 등록된 플레이어 오브젝트가 null입니다. InstanceId: {_playerInstanceId}");
            return false;
        }

        if (registeredPlayerObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"GameObjectManager: 등록된 플레이어 오브젝트가 비활성화 상태입니다. InstanceId: {_playerInstanceId}, ObjectName: {registeredPlayerObject.name}");
            return false;
        }

        PlayerEntity playerEntity = registeredPlayerObject.GetComponent<PlayerEntity>();

        if (playerEntity == null)
        {
            Debug.LogWarning($"GameObjectManager: 등록된 플레이어 오브젝트에 PlayerEntity가 없습니다. InstanceId: {_playerInstanceId}, ObjectName: {registeredPlayerObject.name}");
            return false;
        }

        if (playerEntity.InstanceId != _playerInstanceId)
        {
            Debug.LogWarning($"GameObjectManager: 등록된 플레이어 InstanceId와 PlayerEntity의 InstanceId가 다릅니다. RegisteredId: {_playerInstanceId}, EntityId: {playerEntity.InstanceId}");
            return false;
        }

        playerInstanceId = _playerInstanceId;
        playerObject = registeredPlayerObject;

        return true;

    }

    public bool IsPlayerInstanceId(int instanceId)
    {
        if (instanceId <= 0)
        {
            return false;
        }

        return _playerInstanceId == instanceId;
    }

    private void RegisterOwnerSpawnSpot(int instanceId, SpawnSpot ownerSpawnSpot)
    {
        if (ownerSpawnSpot == null)
        {
            _spawnSpotContainer.Remove(instanceId);
            return;
        }

        _spawnSpotContainer[instanceId] = ownerSpawnSpot;
    }


    private int GenerateInstanceId()
    {
        _objectInstanceKeyGenerator++;

        if (_objectInstanceKeyGenerator <= 0)
        {
            Debug.LogWarning("InstanceId가 int 범위를 초과했거나 비정상 값이 되었습니다.");
            return -1;
        }

        return _objectInstanceKeyGenerator;
    }

    private void InitSpawnedObject(int instanceId, GameObject createdObject, string dataId, SpawnSpot ownerSpawnSpot)
    {

        IGameObjectEntity entity = createdObject.GetComponent<IGameObjectEntity>();

        if (entity == null)
        {
            Debug.LogWarning($"생성된 오브젝트에 IGameObjectEntity가 없습니다. ObjectName: {createdObject.name}");
            return;
        }


        entity.InitEntity(instanceId, dataId);

        RegisterPlayerObject(instanceId, createdObject);

        EnemyAI enemyAI = createdObject.GetComponent<EnemyAI>();

        if (enemyAI == null)
        {
            enemyAI =
                createdObject.GetComponentInChildren<EnemyAI>(true);
        }

        if (enemyAI != null)
        {
            enemyAI.InitEnemyInfo(instanceId, dataId, ownerSpawnSpot);
        }

    }

    public GameObject GetGameObjectCanBeNull(int instanceId)
    {
        if (_createdGameObjectContainer.TryGetValue(instanceId, out GameObject targetObject) == false)
        {
            Debug.LogWarning($"해당 InstanceId의 오브젝트가 없습니다. InstanceId: {instanceId}");
            return null;
        }

        if (targetObject == null)
        {
            Debug.LogWarning($"해당 InstanceId의 오브젝트가 null입니다. InstanceId: {instanceId}");
            return null;
        }

        return targetObject;
    }

    public void RequestDisableGameObject(int instanceId)
    {
        GameObject targetObject =
            GetGameObjectCanBeNull(instanceId);

        if (targetObject == null)
        {
            return;
        }

        if (targetObject.activeSelf == false)
        {
            return;
        }

        if (_playerInstanceId == instanceId)
        {
            _playerInstanceId = -1;
            Debug.Log($"GameObjectManager: 플레이어 오브젝트 등록 해제. InstanceId: {instanceId}");
        }

        // 비활성화 시 HUD 제거 요청
        _hudController?.RemoveMonsterHud(instanceId);

        _spawnSpotContainer.TryGetValue(instanceId, out SpawnSpot ownerSpawnSpot);

        IGameObjectEntity entity = targetObject.GetComponent<IGameObjectEntity>();

        if (entity != null)
        {
            entity.ResetEntity();
        }
        else
        {
            Debug.LogWarning($"비활성화할 오브젝트에 IGameObjectEntity가 없습니다. ObjectName: {targetObject.name}");
        }

        targetObject.SetActive(false);

        Debug.Log($"동적 오브젝트 비활성화 완료. InstanceId: {instanceId}");

        if (ownerSpawnSpot != null)
        {
            ownerSpawnSpot.NotifySpawnedObjectDisabled(instanceId);
        }
    }

    public bool RequestTakeDamage(int instanceId, DamageInfo damageInfo)
    {
        if (instanceId <= 0)
        {
            Debug.LogWarning($"유효하지 않은 InstanceId로 데미지 요청이 들어왔습니다. InstanceId: {instanceId}");
            return false;
        }

        if (damageInfo == null)
        {
            Debug.LogWarning($"DamageInfo가 null이어서 데미지 요청을 처리할 수 없습니다.InstanceId: {instanceId}");

            return false;
        }

        GameObject targetObject = GetGameObjectCanBeNull(instanceId);

        if (targetObject == null)
        {
            return false;
        }

        if (targetObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"비활성화된 오브젝트에는 데미지를 줄 수 없습니다.InstanceId: {instanceId}, ObjectName: {targetObject.name}");
            return false;
        }

        IGameObjectEntity targetEntity = targetObject.GetComponent<IGameObjectEntity>();

        if (targetEntity == null)
        {
            Debug.LogWarning($"대상 오브젝트에 IGameObjectEntity가 없습니다. InstanceId: {instanceId}, ObjectName: {targetObject.name}");

            return false;
        }

        if (targetEntity.InstanceId != instanceId)
        {
            Debug.LogWarning($"요청된 InstanceId와 대상 Entity의 InstanceId가 다릅니다.RequestId: {instanceId}, EntityId: {targetEntity.InstanceId}");
            return false;
        }

        IDamageable damageable = targetObject.GetComponent<IDamageable>();

        if (damageable == null)
        {
            damageable = targetObject.GetComponentInChildren<IDamageable>(true);
        }
        if (damageable == null)
        {
            Debug.LogWarning($"대상 오브젝트에서 IDamageable을 찾을 수 없습니다. InstanceId: {instanceId}, ObjectName: {targetObject.name}");
            return false;
        }
        if (damageable.IsDead == true)
        {
            Debug.LogWarning($"이미 사망한 오브젝트에 데미지 요청이 들어왔습니다.InstanceId: {instanceId}, ObjectName: {targetObject.name}");
            return false;
        }

        damageable.TakeDamage(damageInfo);
        Debug.Log($"데미지 전달 완료.InstanceId: {instanceId}, Damage: {damageInfo.BaseDamage},ObjectName: {targetObject.name}");
        return true;
    }
}