using System;
using System.Collections.Generic;
using UnityEngine;
public class DungeonFloorController : MonoBehaviour
{
    [Header("층 식별 정보")]
    [SerializeField] private string _floorId;

    [Header("층 소속 방 설정")]
    [SerializeField] private List<DungeonRoomController> _roomList = new List<DungeonRoomController>();

    [Header("층 입장 위치")]
    [SerializeField] private Transform PointEntrance;

    [Header("층 시작 설정")]
    [SerializeField] private bool _activateOnStart;

    [Header("층 재진입 설정")]
    [SerializeField] private bool _resetRoomProgressOnActivate = true;

    private DungeonFloorState _currentState = DungeonFloorState.None;

    public event Action<DungeonFloorController> OnFloorActivated;

    public event Action<DungeonFloorController> OnFloorDeactivated;

    public event Action<DungeonFloorController, DungeonRoomController> OnFloorRoomActivated;

    public event Action<DungeonFloorController, DungeonRoomController> OnFloorRoomDeactivated;

    public event Action<DungeonFloorController, DungeonRoomController, SpawnSpot> OnFloorWaveSpawned;

    public event Action<DungeonFloorController, DungeonRoomController, SpawnSpot> OnFloorWaveCleared;

    public string FloorId
    {
        get
        {
            return _floorId;
        }
    }

    public DungeonFloorState CurrentState
    {
        get
        {
            return _currentState;
        }
    }

    public bool IsActive
    {
        get
        {
            return _currentState == DungeonFloorState.Active;
        }
    }

    public Transform EntrancePoint
    {
        get
        {
            return PointEntrance;
        }
    }

    private void Awake()
    {
    }

    private void OnEnable()
    {
    }

    private void Start()
    {
        if (_activateOnStart == false)
        {
            return;
        }
    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {

        OnFloorActivated = null;
        OnFloorDeactivated = null;
        OnFloorRoomActivated = null;
        OnFloorRoomDeactivated = null;
        OnFloorWaveSpawned = null;
        OnFloorWaveCleared = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_floorId))
        {
            Debug.LogWarning($"DungeonFloorController: [{gameObject.name}] FloorId가 비어 있습니다.", this);
        }
        if (_roomList == null)
        {
            return;
        }

        for (int index = 0; index < _roomList.Count; index++)
        {
            if (_roomList[index] != null)
            {
                continue;
            }
            Debug.LogWarning($"DungeonFloorController: [{gameObject.name}] Room List의 Element {index}가 비어 있습니다.", this);
        }
    }
#endif

}

