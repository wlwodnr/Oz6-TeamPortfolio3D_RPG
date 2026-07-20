using System.Collections.Generic;
using UnityEngine;

public class MapRootController : MonoBehaviour
{
    [Header("맵 식별 정보")]
    [SerializeField] private string _mapId;

    [Header("플레이어 생성 위치")]
    [SerializeField] private List<MapSpawnPoint> _playerSpawnPointList;

    [Header("NPC 스폰 설정")]
    [SerializeField] private List<NPCSpawnSpot> _npcSpawnSpotList;

    public string MapId
    {
        get 
        { 
            return _mapId; 
        }
    }

    public void ActivateMap()
    {
        gameObject.SetActive(true);

        if (Floor_Dungeon != null)
        {
            Floor_Dungeon.ActivateFloor();
        }

        ActivateNpcSpawnSpots();
    }

    public void DeactivateMap()
    {
        if (Floor_Dungeon != null)
        {
            Floor_Dungeon.DeactivateFloor();
        }

        DeactivateNpcSpawnSpots();

        gameObject.SetActive(false);
    }

    public Transform GetPlayerSpawnPointCanBeNull(
        string spawnPointId)
    {

    }
}
