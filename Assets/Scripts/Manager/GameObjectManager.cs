using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public static GameObjectManager Instance { get; private set; }
    [SerializeField] private Transform Root_DynamicObject;

    private int _objectInstanceKeyGenerator = 0;

    private Dictionary<int, GameObject> _createdGameObjectContainer = new Dictionary<int, GameObject>();

    private Dictionary<int, GameObject> _prefabContainer = new Dictionary<int, GameObject>();

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

    public int RequestSpawnGameObject(GameObject prefab, Vector3 spawnPosition, Quaternion spawnRotation, string dataId)
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

        if (TryReuseInactiveGameObject(prefab, spawnPosition, spawnRotation, dataId, out int reusedInstanceId))
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

        InitSpawnedObject(instanceId, createdObject, dataId);

        //JU 이 디버그는 테스트 후 삭제할 예정
        Debug.Log($"동적 오브젝트 생성 완료. InstanceId: {instanceId}, Name: {createdObject.name}");

        return instanceId;
    }

    private bool TryReuseInactiveGameObject(GameObject prefab, Vector3 spawnPosition, Quaternion spawnRotation, string dataId, out int reusedInstanceId)
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

            if (entity != null)
            {
                entity.ResetEntity();

                entity.InitEntity(instanceId, dataId);
            }
            else
            {
                Debug.LogWarning($"재사용할 오브젝트에 IGameObjectEntity가 없습니다. ObjectName: {pooledObject.name}");
                continue;
            }

            pooledObject.SetActive(true);

            reusedInstanceId = instanceId;

            Debug.Log($"비활성화된 오브젝트 재사용 완료. InstanceId: {instanceId}, Name: {pooledObject.name}");

            return true;
        }
        reusedInstanceId = -1;
        return false;
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

    private void InitSpawnedObject(int instanceId, GameObject createdObject, string dataId)
    {
        IGameObjectEntity entity = createdObject.GetComponent<IGameObjectEntity>();

        if (entity == null)
        {
            Debug.LogWarning($"생성된 오브젝트에 IGameObjectEntity가 없습니다. ObjectName: {createdObject.name}");
            return;
        }


        entity.InitEntity(instanceId, dataId);

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
        GameObject targetObject = GetGameObjectCanBeNull(instanceId);

        if (targetObject == null)
        {
            return;
        }

        if (targetObject.activeSelf == false)
        {
            return;
        }

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

    }
}