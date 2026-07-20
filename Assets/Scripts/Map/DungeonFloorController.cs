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
        ChangeState(DungeonFloorState.Inactive);
    }

    private void OnEnable()
    {
        BindRoomEvents();
    }

    private void Start()
    {
        if (_activateOnStart == false)
        {
            return;
        }
        ActivateFloor();
    }

    private void OnDisable()
    {
        DeactivateFloor();

        UnbindRoomEvents();
    }

    private void OnDestroy()
    {
        UnbindRoomEvents();

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
        if (string.IsNullOrWhiteSpace(_floorId))
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

    public void ActivateFloor()
    {
        if(_currentState == DungeonFloorState.Active)
        {
            Debug.LogWarning($"DungeonFloorController: [{gameObject.name}] 층이 이미 활성화되어 있습니다.",this);
            return;
        }
        if(_currentState == DungeonFloorState.Activating)
        {
            return;
        }
        if(_currentState == DungeonFloorState.Deactivating)
        {
            Debug.LogWarning($"DungeonFloorController: [{gameObject.name}] 층을 비활성화하는 중에는 활성화할 수 없습니다.",this);
            return;
        }

        ChangeState(DungeonFloorState.Activating);

        BindRoomEvents();

        int activateRoomCount = 0;

        if(_roomList != null)
        {
            foreach(DungeonRoomController room in _roomList)
            {
                if(room == null)
                {
                    continue;
                }
                if(room.IsActive == false)
                {
                    room.ActivateRoom();
                }

                activateRoomCount++;
            }
        }

        ChangeState(DungeonFloorState.Active);

        Debug.Log($"DungeonFloorController: [{gameObject.name}] 층 활성화 완료. FloorId: {_floorId}, 방 수: {activateRoomCount}", this);

        OnFloorActivated?.Invoke(this);
    }

    public void DeactivateFloor()
    {
        if(_currentState == DungeonFloorState.Inactive)
        {
            return;
        }
        if(_currentState == DungeonFloorState.Deactivating)
        {
            return;
        }

        ChangeState(DungeonFloorState.Deactivating);

        if(_roomList != null)
        {
            foreach(DungeonRoomController room in _roomList)
            {
                if(room == null)
                {
                    continue;
                }

                room.DeactivateRoom();
            }
            
        }
        ChangeState(DungeonFloorState.Inactive);

        Debug.Log($"DungeonFloorController: [{gameObject.name}] 층 비활성화 완료. FloorId: {_floorId}",this);

        OnFloorDeactivated?.Invoke(this);
    }

    public int GetTotalAliveMonsterCount()
    {
        int totalAliveMonsterCount = 0;
        if(_roomList == null)
        {
            return totalAliveMonsterCount;
        }
        foreach(DungeonRoomController room in _roomList)
        {
            if(room == null)
            {
                continue;
            }

            totalAliveMonsterCount += room.GetTotalAliveMonsterCount();
        }

        return totalAliveMonsterCount;
    }

    public bool HasNoAliveMonster()
    {
        return GetTotalAliveMonsterCount()==0;
    }


    private void BindRoomEvents()
    {
        if(_roomList == null)
        {
            return;
        }

        foreach(DungeonRoomController room in _roomList)
        {
            if(room == null)
            {
                continue;
            }

            room.OnRoomActivated -= HandleRoomActivated;

            room.OnRoomDeactivated -= HandleRoomDeactivated;

            room.OnRoomWaveSpawned -= HandleRoomWaveSpawned;

            room.OnRoomWaveCleared -= HandleRoomWaveCleared;

            room.OnRoomActivated += HandleRoomActivated;

            room.OnRoomDeactivated += HandleRoomDeactivated;

            room.OnRoomWaveSpawned += HandleRoomWaveSpawned;

            room.OnRoomWaveCleared += HandleRoomWaveCleared;
        }
    }

    private void UnbindRoomEvents()
    {
        if(_roomList == null)
        {
            return;
        }

        foreach(DungeonRoomController room in _roomList)
        {
            if(room == null)
            {
                continue;
            }

            room.OnRoomActivated -= HandleRoomActivated;

            room.OnRoomDeactivated -= HandleRoomDeactivated;

            room.OnRoomWaveSpawned -= HandleRoomWaveSpawned;

            room.OnRoomWaveCleared -= HandleRoomWaveCleared;
        }
    }

    private void HandleRoomActivated(DungeonRoomController room)
    {
        if(_currentState != DungeonFloorState.Activating && _currentState != DungeonFloorState.Active)
        {
            return;
        }
        OnFloorRoomActivated?.Invoke(this, room);
    }

    private void HandleRoomDeactivated(DungeonRoomController room)
    {
        if(_currentState != DungeonFloorState.Deactivating)
        {
            return;
        }
        OnFloorRoomDeactivated?.Invoke(this, room);
    }

    private void HandleRoomWaveSpawned(DungeonRoomController room, SpawnSpot spawnSpot)
    {
        if(_currentState != DungeonFloorState.Active &&  _currentState != DungeonFloorState.Activating)
        {
            return;
        }
        OnFloorWaveSpawned?.Invoke(this, room, spawnSpot);
    }

    private void HandleRoomWaveCleared(DungeonRoomController room, SpawnSpot spawnSpot)
    {
        if(_currentState != DungeonFloorState.Active)
        {
            return;
        }

        OnFloorWaveCleared?.Invoke(this, room, spawnSpot);
    }

    private void ChangeState(DungeonFloorState nextState)
    {
        if(_currentState == nextState)
        {
            return;
        }

        DungeonFloorState previousState = _currentState;

        _currentState = nextState;

        Debug.Log($"DungeonFloorController: [{gameObject.name}] 층 상태 변경. {previousState} → {_currentState}", this);
    }
#if UNITY_EDITOR
    [ContextMenu("TEST/층 활성화")]
    private void TestActivateFloor()
    {
        ActivateFloor();
    }

    [ContextMenu("TEST/층 비활성화")]
    private void TestDeactivateFloor()
    {
        DeactivateFloor();
    }
#endif
}

