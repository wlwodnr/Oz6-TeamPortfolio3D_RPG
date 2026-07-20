using System;
using UnityEngine;
using System.Collections.Generic;

public class DungeonRoomController : MonoBehaviour
{
    [Header("방 스폰 설정")]
    [SerializeField] private List<SpawnSpot> _spawnSpotList = new List<SpawnSpot>();

    [Header("방 시작 설정")]
    [SerializeField] private bool _activateOnStart;

    private DungeonRoomState _currentState = DungeonRoomState.None;

    public event Action<DungeonRoomController> OnRoomActivated;

    public event Action<DungeonRoomController> OnRoomDeactivated;

    public event Action<DungeonRoomController, SpawnSpot> OnRoomWaveSpawned;

    public event Action<DungeonRoomController, SpawnSpot> OnRoomWaveCleared;

    public DungeonRoomState CurrentState
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
            return _currentState == DungeonRoomState.Active;
        }
    }


    private void Awake()
    {
        ChangeState(DungeonRoomState.Inactive);
    }

    private void OnEnable()
    {
        BindSpawnSpotEvent();
    }

    private void Start()
    {
        if(_activateOnStart == false)
        {
            return;
        }
        ActivateRoom();
    }

    private void OnDisable()
    {
        DeactivateRoom();

        UnBindSpawnSpotEvent();
    }

    private void OnDestroy()
    {
        UnBindSpawnSpotEvent();

        OnRoomActivated = null;
        OnRoomDeactivated = null;
        OnRoomWaveSpawned = null;
        OnRoomWaveCleared = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(_spawnSpotList == null)
        {
            return;
        }

        for(int index = 0; index < _spawnSpotList.Count; index++)
        {
            if(_spawnSpotList[index] != null )
            {
                continue;
            }
            Debug.LogWarning($"DungeonRoomController: [{gameObject.name}] SpawnSpot List의 Element {index}가 비어 있습니다.", this);
        }
    }
#endif

    public void ActivateRoom()
    {
        if (_currentState == DungeonRoomState.Active)
        {
            Debug.LogWarning($"DungeonRoomController: [{gameObject.name}] 방이 이미 활성화되어 있습니다.");
            return;
        }
        if( _currentState == DungeonRoomState.Deactivating)
        {
            Debug.LogWarning($"DungeonRoomController: [{gameObject.name}] 방을 비활성화하는 중에는 다시 활성화할 수 없습니다.");
            return;
        }

        ChangeState(DungeonRoomState.Waiting);

        BindSpawnSpotEvent();

        int activatedSpawnSpotCount = 0;

        if( _spawnSpotList != null )
        {
            foreach(SpawnSpot spawnSpot in _spawnSpotList)
            {
                if(spawnSpot == null)
                {
                    continue;
                }

                if(spawnSpot.IsSpawnOperationActive == false)
                {
                    spawnSpot.ActivateSpawnSpot();
                }
                activatedSpawnSpotCount++;
            }
        }

        ChangeState(DungeonRoomState.Active);

        Debug.Log($"DungeonRoomController: [{gameObject.name}] 방 활성화 완료. SpawnSpot 수: {activatedSpawnSpotCount}", this);

        OnRoomActivated?.Invoke(this);
    }

    public void DeactivateRoom()
    {
        if (_currentState == DungeonRoomState.Inactive)
        {
            return;
        }

        if (_currentState == DungeonRoomState.Deactivating)
        {
            return;
        }

        ChangeState(DungeonRoomState.Deactivating);

        if(_spawnSpotList != null)
        {
            foreach(SpawnSpot spawnSpot in _spawnSpotList)
            {
                if(spawnSpot == null)
                {
                    continue;
                }

                spawnSpot.DeactivateSpawnSpot();
            }
        }
        ChangeState(DungeonRoomState.Inactive);

        Debug.Log($"DungeonRoomController: [{gameObject.name}] 방 비활성화 완료.", this);

        OnRoomDeactivated?.Invoke(this);
    }

    public int GetTotalAliveMonsterCount()
    {
        int totalAliveCount = 0;

        if(_spawnSpotList == null)
        {
            return totalAliveCount;
        }

        foreach(SpawnSpot spawnSpot in _spawnSpotList)
        {
            if( spawnSpot == null)
            {
                continue;
            }
            totalAliveCount += spawnSpot.AliveCount;
        }

        return totalAliveCount;
    }

    public bool HasNoAliveMonster()
    {
        return GetTotalAliveMonsterCount() == 0;
    }

    private void BindSpawnSpotEvent()
    {
        if (_spawnSpotList == null)
        {
            return;
        }

        foreach (SpawnSpot spawnSpot in _spawnSpotList)
        {
            if (spawnSpot == null)
            {
                continue;
            }

            spawnSpot.OnWaveSpawned -= HandleWaveSpawned;

            spawnSpot.OnWaveCleared -= HandleWaveCleared;

            spawnSpot.OnWaveSpawned += HandleWaveSpawned;

            spawnSpot.OnWaveCleared += HandleWaveCleared;
        }
        
    }

    private void UnBindSpawnSpotEvent()
    {
        if(_spawnSpotList == null)
        {
            return;
        }

        foreach(SpawnSpot spawnSpot in _spawnSpotList)
        {
            if( spawnSpot == null)
            {
                continue;
            }

            spawnSpot.OnWaveSpawned -= HandleWaveSpawned;

            spawnSpot.OnWaveCleared -= HandleWaveCleared;
        }
    }

    private void HandleWaveSpawned(SpawnSpot spawnSpot)
    {
        if(_currentState != DungeonRoomState.Active && _currentState != DungeonRoomState.Waiting)
        {
            return;
        }

        Debug.Log($"DungeonRoomController: [{gameObject.name}] 웨이브 생성 감지. SpawnSpot: {spawnSpot.gameObject.name}",this);

        OnRoomWaveSpawned?.Invoke(this, spawnSpot);
    }

    private void HandleWaveCleared(SpawnSpot spawnSpot)
    {
        if(_currentState != DungeonRoomState.Active)
        {
            return;
        }

        Debug.Log($"DungeonRoomController: [{gameObject.name}] 웨이브 전멸 감지. SpawnSpot: {spawnSpot.gameObject.name}",this);

        OnRoomWaveCleared?.Invoke(this, spawnSpot);
    }

    private void ChangeState(DungeonRoomState nextState)
    {
        if(_currentState == nextState)
        {
            return;
        }

        DungeonRoomState previousState = _currentState;
        
        _currentState = nextState;


        Debug.Log($"DungeonRoomController: [{gameObject.name}] 방 상태 변경. {previousState} → {_currentState}",this);
    }

#if UNITY_EDITOR
    [ContextMenu("TEST/방 활성화")]
    private void TestActivateRoom()
    {
        ActivateRoom();
    }

    [ContextMenu("TEST/방 비활성화")]
    private void TestDeactivateRoom()
    {
        DeactivateRoom();
    }
#endif

}
