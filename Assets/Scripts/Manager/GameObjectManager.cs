using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public static GameObjectManager Instance { get; private set; }
    [SerializeField] private Transform Root_DynamicObject;

    private int _objectInstanceKeyGenerator = 0;

    private Dictionary<int, GameObject> _createdGameObjectContainer = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if(Instance == null)
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
        if(prefab == null)
        {
            Debug.LogWarning("생성할 Prefab이 없습니다.");
            return -1;
        }
        if (Root_DynamicObject == null)
        {
            Debug.LogWarning("Root_DynamicObject가 등록되지 않았습니다.");
            return -1;
        }
        //object pool 방식으로 변경할 예정
        GameObject createdObject = Instantiate(prefab, Root_DynamicObject);

        if (createdObject == null)
        {
            Debug.LogWarning("게임 오브젝트 생성에 실패했습니다.");
            return -1;
        }

        createdObject.transform.position = spawnPosition;
        createdObject.transform.rotation = spawnRotation;

        _objectInstanceKeyGenerator++;

        int instanceId = _objectInstanceKeyGenerator;

        if (_createdGameObjectContainer.ContainsKey(instanceId) == true)
        {
            Debug.LogWarning($"이미 동일한 InstanceId가 존재합니다. InstanceId: {instanceId}");
            createdObject.SetActive(false);
            return -1;
        }

        _createdGameObjectContainer.Add(instanceId, createdObject);
        InitSpawnedObject(instanceId, createdObject, dataId);

        //이 디버그는 테스트 후 삭제할 예정
        Debug.Log($"동적 오브젝트 생성 완료. InstanceId: {instanceId}, Name: {createdObject.name}");

        return instanceId;
    }

    private int GenerateInstanceId()
    {
        _objectInstanceKeyGenerator++;
        return _objectInstanceKeyGenerator;
    }

    private void InitSpawnedObject(int instanceId, GameObject createdObject, string dataId)
    {
        IGameObjectEntity entity = createdObject.GetComponent<IGameObjectEntity>();

        if(entity == null)
        {
            Debug.LogWarning($"생성된 오브젝트에 IGameObjectEntity가 없습니다. ObjectName: {createdObject.name}");
            return;
        }


        entity.InitEntity(instanceId, dataId);

    }

    public GameObject GetGameObjectCanBeNull(int instanceId)
    {
        if(_createdGameObjectContainer.ContainsKey(instanceId) == false)
        {
            Debug.LogWarning($"해당 InstanceId의 오브젝트가 없습니다. InstanceId: {instanceId}");
            return null;
        }

        return _createdGameObjectContainer[instanceId];
    }

    public void RequestDisableGameObject(int instanceId)
    {
        GameObject targetObject = GetGameObjectCanBeNull(instanceId);

        if(targetObject == null)
        {
            return;
        }

        IGameObjectEntity entity = targetObject.GetComponent<IGameObjectEntity>();

        if(entity != null)
        {
            entity.ResetEntity();
        }

        targetObject.SetActive(false);

        Debug.Log($"동적 오브젝트 비활성화 완료. InstanceId: {instanceId}");

    }
}