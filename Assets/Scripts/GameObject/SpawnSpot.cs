using UnityEngine;

public class SpawnSpot : MonoBehaviour
{
    [SerializeField] private GameObject Prefab_SpawnTarget;

    [SerializeField] private string _spawnDataId;

    [SerializeField] private bool _spawnOnStart = true;

    private int _spawnedInstanceId = -1;

    public int SpawnedIstanceId
    {
        get {  return _spawnedInstanceId; }
    }

    private void Start()
    {
         if(_spawnOnStart == true)
        {
            RequestSpawn();
        }
    }

    public void RequestSpawn()
    {
        if(GameObjectManager.Instance == null)
        {
            Debug.LogWarning("GameObjectManager가 존재하지 않아 스폰할 수 없습니다.");
            return;
        }

        if(Prefab_SpawnTarget == null)
        {
            Debug.LogWarning($"SpawnSpot에 Prefab_SpawnTarget이 등록되지 않았습니다. ObjectName: {gameObject.name}");
            return;
        }
        _spawnedInstanceId = GameObjectManager.Instance.RequestSpawnGameObject(
            Prefab_SpawnTarget,
            transform.position,
            transform.rotation,
            _spawnDataId
        );
    }

    public void RequestDisableSpawnedObject()
    {
        if(_spawnedInstanceId < 0)
        {
            Debug.LogWarning("아직 생성된 오브젝트가 없습니다.");
            return;
        }
        GameObjectManager.Instance.RequestDisableGameObject(_spawnedInstanceId);
        _spawnedInstanceId = -1;
    }
}
