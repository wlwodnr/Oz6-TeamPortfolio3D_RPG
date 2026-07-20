using UnityEngine;

public class PortalController : MonoBehaviour
{
    [Header("포탈 충돌 설정")]
    [SerializeField] private Collider ColliderPortal;

    [Header("던전 이동 설정")]
    [SerializeField] private DungeonFlowController ControllerDungeonFlow;
    [SerializeField] private DungeonFloorController FloorTarget;

    [SerializeField] private Transform Transform_TargetPoint;

    [Header("플레이어 감지 설정")]
    [SerializeField] private LayerMask LayerPlayer;

    private bool _isTransitionRequested;

    public DungeonFloorController TargetFloor
    {
        get
        {
            return FloorTarget;
        }
    }

    public Transform TargetPoint
    {
        get
        {
            return Transform_TargetPoint;
        }
    }

    private void Awake()
    {
        ValidateReferences();
    }

    private void OnEnable()
    {
        _isTransitionRequested = false;

        if (ColliderPortal != null)
        {
            ColliderPortal.enabled = true;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (ColliderPortal != null && ColliderPortal.isTrigger == false)
        {
            Debug.LogWarning($"PortalController: [{gameObject.name}] Collider_Portal의 Is Trigger가 꺼져 있습니다.", this);
        }

        if (Transform_TargetPoint == null)
        {
            Debug.LogWarning($"PortalController: [{gameObject.name}] Transform_TargetPoint가 연결되지 않았습니다.", this);
        }
    }
#endif

    private void ValidateReferences()
    {
        if (ColliderPortal == null)
        {
            Debug.LogWarning($"PortalController: [{gameObject.name}] Collider_Portal이 연결되지 않았습니다.", this);
        }

        if (ControllerDungeonFlow == null)
        {
            Debug.LogWarning($"PortalController: [{gameObject.name}] DungeonFlowController가 연결되지 않았습니다.", this);
        }

        if (FloorTarget == null)
        {
            Debug.LogWarning($"PortalController: [{gameObject.name}] 목적 DungeonFloorController가 연결되지 않았습니다.", this);
        }

        if (Transform_TargetPoint == null)
        {
            Debug.LogWarning($"PortalController: [{gameObject.name}] 목적지 Transform이 연결되지 않았습니다.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isTransitionRequested == true)
        {
            return;
        }

        if (IsPlayerLayer(other.gameObject.layer) == false)
        {
            return;
        }

        if (ControllerDungeonFlow == null)
        {
            Debug.LogWarning($"PortalController: [{gameObject.name}] DungeonFlowController가 없어 이동할 수 없습니다.", this);
            return;
        }

        if (FloorTarget == null)
        {
            Debug.LogWarning($"PortalController: [{gameObject.name}] 목적 층이 없어 이동할 수 없습니다.", this);
            return;
        }

        if (Transform_TargetPoint == null)
        {
            Debug.LogWarning($"PortalController: [{gameObject.name}] 목적지 Transform이 없어 이동할 수 없습니다.", this);
            return;
        }

        _isTransitionRequested = true;

        bool wasAccepted = ControllerDungeonFlow.RequestMoveToFloor(FloorTarget, Transform_TargetPoint);

        if (wasAccepted == false)
        {
            _isTransitionRequested = false;
        }
    }

    private bool IsPlayerLayer(int targetLayer)
    {
        int targetLayerMask = 1 << targetLayer;

        return (LayerPlayer.value & targetLayerMask) != 0;
    }

#if UNITY_EDITOR
    [ContextMenu("TEST/목적 층 이동 요청")]
    private void TestRequestFloorTransition()
    {
        if (ControllerDungeonFlow == null || FloorTarget == null || Transform_TargetPoint == null)
        {
            Debug.LogWarning($"PortalController: [{gameObject.name}] 이동 설정이 완료되지 않았습니다.", this);
            return;
        }

        bool wasAccepted = ControllerDungeonFlow.RequestMoveToFloor(FloorTarget, Transform_TargetPoint);

        Debug.Log($"PortalController: [{gameObject.name}] 테스트 이동 요청 결과: {wasAccepted}", this);
    }
#endif
}