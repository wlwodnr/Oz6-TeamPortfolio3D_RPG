using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonFlowController : MonoBehaviour
{
    [Header("던전 층 설정")]
    [SerializeField] private List<DungeonFloorController> _floorList = new List<DungeonFloorController>();

    [SerializeField] private DungeonFloorController FloorStart;

    [Header("플레이어 설정")]
    [SerializeField] private Transform transformPlayer;

    [Header("포탈 중복 이동 방지")]
    [Min(0f)]
    [SerializeField] private float _transitionCooldown = 0.5f;

    private DungeonFloorController _currentFloor;

    private bool _isTransitioning;

    private float _nextTransitionAllowedTime;

    public event Action<DungeonFloorController, DungeonFloorController> OnFloorTransitionStarted;

    public event Action<DungeonFloorController, DungeonFloorController> OnFloorTransitionCompleted;

    public DungeonFloorController CurrentFloor
    {
        get
        {
            return _currentFloor;
        }
    }

    public bool IsTransitioning
    {
        get
        {
            return _isTransitioning;
        }
    }

    private void Awake()
    {
        ValidateReferences();
    }

    private void Start()
    {
        InitializeDungeon();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(_transitionCooldown < 0f)
        {
            _transitionCooldown = 0f;
        }

        if(_floorList == null)
        {
            return;
        }

        for(int index = 0; index < _floorList.Count; index++)
        {
            if(_floorList[index] != null)
            {
                continue;
            }
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] Floor List의 Element {index}가 비어 있습니다.", this);
        }
    }
#endif

    private void ValidateReferences()
    {
        if(_floorList == null || _floorList.Count == 0)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] Floor List가 비어 있습니다.", this);
        }
        if(FloorStart == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 시작 층이 연결되지 않았습니다.", this);
        }

        if(transformPlayer == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 플레이어 Transform이 연결되지 않았습니다.", this);
        }
    }

    private void InitializeDungeon()
    {
        if(FloorStart == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 시작 층이 없어 던전을 초기화할 수 없습니다.", this);
            return;
        }

        if(IsRegisteredFloor(FloorStart) == false)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 시작 층이 Floor List에 등록되지 않았습니다.", this);
            return;
        }

        if(_floorList != null)
        {
            foreach(DungeonFloorController floor in _floorList)
            {
                if(floor == null || floor == FloorStart)
                {
                    continue;
                }
                floor.DeactivateFloor();

                floor.gameObject.SetActive(false);
            }
        }

        FloorStart.gameObject.SetActive(true);

        MovePlayerToFloorEntrance(FloorStart);

        if(FloorStart.IsActive == false)
        {
            FloorStart.ActivateFloor();
        }

        _currentFloor = FloorStart;
        _isTransitioning = false;
        _nextTransitionAllowedTime = Time.unscaledTime + _transitionCooldown;

        Debug.Log($"DungeonFlowController: [{gameObject.name}] 던전 초기화 완료. 시작 층: {FloorStart.FloorId}", this);
    }

    public bool RequestMoveToFloor(DungeonFloorController targetFloor)
    {
        if(_isTransitioning == true)
        {
            return false;
        }

        if(Time.unscaledTime < _nextTransitionAllowedTime)
        {
            return false;
        }
        if(targetFloor == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 목적 층이 null입니다.", this);
            return false;
        }

        if(IsRegisteredFloor(targetFloor) == false)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 등록되지 않은 층으로 이동할 수 없습니다. Target: {targetFloor.gameObject.name}", this);

            return false;
        }

        if(targetFloor == _currentFloor)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 현재 층과 목적 층이 같습니다.", this);
            return false;
        }

        ExcuteFloorTransition(targetFloor);

        return true;
    }

    private void ExcuteFloorTransition(DungeonFloorController targetFloor)
    {
        _isTransitioning = true;

        DungeonFloorController previousFloor = _currentFloor;

        OnFloorTransitionStarted?.Invoke(previousFloor, targetFloor);

        if(previousFloor != null)
        {
            previousFloor.DeactivateFloor();

            previousFloor.gameObject.SetActive(false);
        }

        targetFloor.gameObject.SetActive(false);

        MovePlayerToFloorEntrance(targetFloor);

        if(targetFloor.IsActive == false)
        {
            targetFloor.ActivateFloor();
        }

        _currentFloor = targetFloor;

        _nextTransitionAllowedTime = Time.unscaledTime + _transitionCooldown;

        _isTransitioning = false;

        Debug.Log($"DungeonFlowController: [{gameObject.name}] 층 이동 완료. Previous: {GetFloorIdCanBeEmpty(previousFloor)}, Current: {_currentFloor.FloorId}", this);

        OnFloorTransitionCompleted?.Invoke(previousFloor, targetFloor);
    }

    private void MovePlayerToFloorEntrance(DungeonFloorController targetFloor)
    {
        if(transformPlayer == null)
        {
            Debug.Log($"DungeonFlowController: [{gameObject.name}] 플레이어 Transform이 없어 위치를 이동할 수 없습니다.", this);
            return;
        }

        if(targetFloor == null || targetFloor.EntrancePoint == null)
        {
            Debug.Log($"DungeonFlowController: [{gameObject.name}] 목적 층의 EntrancePoint가 없습니다.", this);
            return;
        }

        transformPlayer.SetPositionAndRotation(targetFloor.EntrancePoint.position, targetFloor.EntrancePoint.rotation);

    }
    private bool IsRegisteredFloor(DungeonFloorController targetFloor)
    {
        if(_floorList == null || targetFloor == null)
        {
            return false;
        }

        return _floorList.Contains(targetFloor);
    }

    private string GetFloorIdCanBeEmpty(DungeonFloorController floor)
    {
        if(floor == null)
        {
            return string.Empty;
        }
        return floor.FloorId;
    }
}
