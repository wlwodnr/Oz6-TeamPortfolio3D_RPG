using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonFlowController : MonoBehaviour
{
    [Header("던전 층 설정")]
    [SerializeField] private List<DungeonFloorController> _floorList = new List<DungeonFloorController>();
    [SerializeField] private DungeonFloorController FloorStart;

    [Header("플레이어 설정")]
    [SerializeField] private GameObject Prefab_Player;
    [SerializeField] private string _playerDataId = "player_main";
    [SerializeField] private CameraController Controller_Camera;

    [Header("플레이어 런타임 참조")]
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
        InitializePlayer();
        InitializeDungeon();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_transitionCooldown < 0f)
        {
            _transitionCooldown = 0f;
        }

        if (_floorList == null)
        {
            return;
        }

        for (int index = 0; index < _floorList.Count; index++)
        {
            if (_floorList[index] != null)
            {
                continue;
            }

            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] Floor List의 Element {index}가 비어 있습니다.", this);
        }
    }
#endif

    private void ValidateReferences()
    {
        if (_floorList == null || _floorList.Count == 0)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] Floor List가 비어 있습니다.", this);
        }

        if (FloorStart == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 시작 층이 연결되지 않았습니다.", this);
        }

        if (Prefab_Player == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] Player 프리팹이 연결되지 않았습니다.", this);
        }

        if (string.IsNullOrWhiteSpace(_playerDataId))
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] PlayerDataId가 비어 있습니다.", this);
        }

        if (Controller_Camera == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] CameraController가 연결되지 않았습니다.", this);
        }
    }

    private void InitializeDungeon()
    {
        if (FloorStart == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 시작 층이 없어 던전을 초기화할 수 없습니다.", this);
            return;
        }

        if (IsRegisteredFloor(FloorStart) == false)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 시작 층이 Floor List에 등록되지 않았습니다.", this);
            return;
        }

        if (_floorList != null)
        {
            foreach (DungeonFloorController floor in _floorList)
            {
                if (floor == null || floor == FloorStart)
                {
                    continue;
                }

                floor.DeactivateFloor();
                floor.gameObject.SetActive(false);
            }
        }

        FloorStart.gameObject.SetActive(true);

        MovePlayerToDestination(FloorStart, FloorStart.EntrancePoint);

        if (FloorStart.IsActive == false)
        {
            FloorStart.ActivateFloor();
        }

        _currentFloor = FloorStart;
        _isTransitioning = false;
        _nextTransitionAllowedTime = Time.unscaledTime + _transitionCooldown;

        Debug.Log($"DungeonFlowController: [{gameObject.name}] 던전 초기화 완료. 시작 층: {FloorStart.FloorId}", this);
    }

    private void InitializePlayer()
    {
        if (transformPlayer != null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 이미 Player Transform이 연결되어 있어 Player를 새로 생성하지 않습니다.", this);

            ConnectPlayerToCamera();
            return;
        }

        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] GameObjectManager가 없어 Player를 생성할 수 없습니다.", this);
            return;
        }

        if (Prefab_Player == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] Player 프리팹이 없어 Player를 생성할 수 없습니다.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(_playerDataId))
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] PlayerDataId가 비어 있어 Player를 생성할 수 없습니다.", this);
            return;
        }

        if (FloorStart == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 시작 층이 없어 Player 생성 위치를 결정할 수 없습니다.", this);
            return;
        }

        if (FloorStart.EntrancePoint == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 시작 층의 EntrancePoint가 없어 Player를 생성할 수 없습니다.", this);
            return;
        }

        Vector3 spawnPosition = FloorStart.EntrancePoint.position;
        Quaternion spawnRotation = FloorStart.EntrancePoint.rotation;

        int playerInstanceId = GameObjectManager.Instance.RequestSpawnGameObject(Prefab_Player, spawnPosition, spawnRotation, _playerDataId);

        if (playerInstanceId < 0)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] Player 생성 요청에 실패했습니다.", this);
            return;
        }

        GameObject playerObject = GameObjectManager.Instance.GetGameObjectCanBeNull(playerInstanceId);

        if (playerObject == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 생성된 Player를 가져올 수 없습니다. InstanceId: {playerInstanceId}", this);
            return;
        }

        transformPlayer = playerObject.transform;

        ConnectPlayerToCamera();

        Debug.Log($"DungeonFlowController: [{gameObject.name}] Player 동적 생성 완료. InstanceId: {playerInstanceId}, PlayerDataId: {_playerDataId}", this);
    }

    private void ConnectPlayerToCamera()
    {
        if (Controller_Camera == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] CameraController가 없어 Player를 카메라에 연결할 수 없습니다.", this);
            return;
        }

        if (transformPlayer == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] Player Transform이 없어 카메라 Target을 연결할 수 없습니다.", this);
            return;
        }

        Controller_Camera.target = transformPlayer;
    }

    public bool RequestMoveToFloor(DungeonFloorController targetFloor)
    {
        Transform destinationPoint = null;

        if (targetFloor != null)
        {
            destinationPoint = targetFloor.EntrancePoint;
        }

        return RequestMoveToFloor(targetFloor, destinationPoint);
    }

    public bool RequestMoveToFloor(DungeonFloorController targetFloor, Transform destinationPoint)
    {
        if (_isTransitioning == true)
        {
            return false;
        }

        if (Time.unscaledTime < _nextTransitionAllowedTime)
        {
            return false;
        }

        if (targetFloor == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 목적 층이 null입니다.", this);
            return false;
        }

        if (IsRegisteredFloor(targetFloor) == false)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 등록되지 않은 층으로 이동할 수 없습니다. Target: {targetFloor.gameObject.name}", this);
            return false;
        }

        if (targetFloor == _currentFloor)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 현재 층과 목적 층이 같습니다.", this);
            return false;
        }

        // 신규: 목적지 위치가 없으면 해당 층의 기본 EntrancePoint를 사용합니다.
        if (destinationPoint == null)
        {
            destinationPoint = targetFloor.EntrancePoint;
        }

        if (destinationPoint == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 목적지 Transform과 목적 층의 EntrancePoint가 모두 비어 있습니다. TargetFloor: {targetFloor.gameObject.name}", this);
            return false;
        }

        ExecuteFloorTransition(targetFloor, destinationPoint);

        return true;
    }

    private void ExecuteFloorTransition(DungeonFloorController targetFloor, Transform destinationPoint)
    {
        _isTransitioning = true;

        DungeonFloorController previousFloor = _currentFloor;

        OnFloorTransitionStarted?.Invoke(previousFloor, targetFloor);

        if (previousFloor != null)
        {
            previousFloor.DeactivateFloor();
            previousFloor.gameObject.SetActive(false);
        }

        targetFloor.gameObject.SetActive(true);

        MovePlayerToDestination(targetFloor, destinationPoint);

        if (targetFloor.IsActive == false)
        {
            targetFloor.ActivateFloor();
        }

        _currentFloor = targetFloor;
        _nextTransitionAllowedTime = Time.unscaledTime + _transitionCooldown;
        _isTransitioning = false;

        Debug.Log($"DungeonFlowController: [{gameObject.name}] 층 이동 완료. Previous: {GetFloorIdCanBeEmpty(previousFloor)}, Current: {_currentFloor.FloorId}, Destination: {destinationPoint.gameObject.name}", this);

        OnFloorTransitionCompleted?.Invoke(previousFloor, targetFloor);
    }

    private void MovePlayerToDestination(DungeonFloorController targetFloor, Transform destinationPoint)
    {
        if (transformPlayer == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 플레이어 Transform이 없어 위치를 이동할 수 없습니다.", this);
            return;
        }

        if (targetFloor == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 목적 층이 null이어서 플레이어를 이동할 수 없습니다.", this);
            return;
        }

        if (destinationPoint == null)
        {
            destinationPoint = targetFloor.EntrancePoint;
        }

        if (destinationPoint == null)
        {
            Debug.LogWarning($"DungeonFlowController: [{gameObject.name}] 플레이어가 이동할 목적 위치가 없습니다. TargetFloor: {targetFloor.gameObject.name}", this);
            return;
        }

        transformPlayer.SetPositionAndRotation(destinationPoint.position, destinationPoint.rotation);
    }

    private bool IsRegisteredFloor(DungeonFloorController targetFloor)
    {
        if (_floorList == null || targetFloor == null)
        {
            return false;
        }

        return _floorList.Contains(targetFloor);
    }

    private string GetFloorIdCanBeEmpty(DungeonFloorController floor)
    {
        if (floor == null)
        {
            return string.Empty;
        }

        return floor.FloorId;
    }
}