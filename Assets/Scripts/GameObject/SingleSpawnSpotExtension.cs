using System;
using UnityEngine;

[Serializable]
public class SingleSpawnSpotExtension
{
    [Header("단일 생성 오브젝트")]
    [SerializeField] private GameObject Prefab_SpawnObject;

    [Header("단일 생성 데이터")]
    [SerializeField] private string _dataId;

    public int RequestSpawn(SpawnSpot ownerSpawnSpot, Transform spawnPoint)
    {
        if (ownerSpawnSpot == null)
        {
            Debug.LogWarning("SingleSpawnSpotExtension: Owner SpawnSpot이 없어 오브젝트를 생성할 수 없습니다.");
            return -1;
        }
        if (Prefab_SpawnObject == null)
        {
            Debug.LogWarning($"SingleSpawnSpotExtension: [{ownerSpawnSpot.gameObject.name}] 생성할 Prefab이 등록되지 않았습니다.", ownerSpawnSpot);
            return -1;
        }
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning($"SingleSpawnSpotExtension: [{ownerSpawnSpot.gameObject.name}] GameObjectManager가 없어 오브젝트를 생성할 수 없습니다.", ownerSpawnSpot);
            return -1;
        }
        if (Prefab_SpawnObject.GetComponent<Treasure>() != null && GameObjectManager.Instance.HasOpenedTreasureId(_dataId))
        {
            Debug.Log($"SingleSpawnSpotExtension: [{ownerSpawnSpot.gameObject.name}] 이미 열린 보물상자이므로 다시 생성하지 않습니다. TreasureDataId: {_dataId}", ownerSpawnSpot);
            return -1;
        }

        Transform targetSpawnPoint = spawnPoint == null ? ownerSpawnSpot.transform : spawnPoint;

        return GameObjectManager.Instance.RequestSpawnGameObject(Prefab_SpawnObject, targetSpawnPoint.position, targetSpawnPoint.rotation, _dataId, ownerSpawnSpot);
    }
}
