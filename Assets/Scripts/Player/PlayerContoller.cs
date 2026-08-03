using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GroundCheck _groundCheck;
    [SerializeField] public float MoveSpeed = 10;
    [SerializeField] public float JumpPower = 5;
    [SerializeField] private float InteractRange = 2;
    public int _jumpCount;
    private Camera _mainCamera;
    private Animator _anim;
    private Transform _currentInteractionTarget;


    //벽뚫 방지용 코드용 변수
    private CapsuleCollider _movementCollider;

    [SerializeField] private LayerMask Layer_wall;

    [SerializeField, Min(0.01f)] private float _wallSkinWidth = 0.02f;

    [SerializeField, Range(1, 4)] private int _wallSlideIterations = 2;

    private const int WallHitBufferSize = 16;
    private readonly RaycastHit[] _wallHitBuffer = new RaycastHit[WallHitBufferSize];
    //


    private Rigidbody _rb;

    private bool _hasLoggedMissingInputManager;

    //아래는 1차빌드용 공격기능 변수 (이후에 바뀔 수 있음)
    [Header("1차 빌드용 공격관련 변수")]
    public Vector3 boxSize = new Vector3(1, 1, 1); 
    public Vector3 offset = new Vector3(0, 1, 1); 
    public LayerMask enemyLayer;

    //추후 PlayerData에서 가져오도록 변경해야 합니다
    [Header("1차 빌드용 임시 공격력")]
    [SerializeField] private int _temporaryAttackDamage = 10;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _anim = GetComponent<Animator>();
        _movementCollider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        if (_groundCheck != null)
        {
            _groundCheck.OnGrounded -= HandleGrounded;
            _groundCheck.OnGrounded += HandleGrounded;
        }

        InputManager.OnJumpPressed -= HandleJumpPressed;
        InputManager.OnAttackPressed -= HandleAttackPressed;
        InputManager.OnInteractPressed -= HandleInteractPressed;
        InputManager.OnJumpPressed += HandleJumpPressed;
        InputManager.OnAttackPressed += HandleAttackPressed;
        InputManager.OnInteractPressed += HandleInteractPressed;
        NetworkManager.Inst.LocalPlayerModel.OnPlayerStatsChanged += MoveSpeedHandler;
    }

    private void OnDisable()
    {
        if (_groundCheck != null)
        {
            _groundCheck.OnGrounded -= HandleGrounded;
        }

        InputManager.OnJumpPressed -= HandleJumpPressed;
        InputManager.OnAttackPressed -= HandleAttackPressed;
        InputManager.OnInteractPressed -= HandleInteractPressed;
    }

    private void Start()
    {
        if(Layer_wall.value == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] PlayerController의 Layer_Wall이 설정되지 않았습니다. 벽 사전 검사를 사용하려면 Inspector에서 Wall 레이어를 지정해야 합니다.", this);
        }
    }

    public void ResetControllerForPool()
    {
        _jumpCount = 0;
        _currentInteractionTarget = null;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        if (_anim != null)
        {
            _anim.Rebind();
            _anim.Update(0f);
        }
    }

    private void Update()
    {
        if(_currentInteractionTarget == null)
        {
            return;
        }

        //float distance = Vector3.Distance(transform.position, _currentInteractionTarget.position);
       // if(distance > InteractRange)
       // {
       //     UIManager.Instance.CloseContentUI(UIType.DialogueUI);
       //     _currentInteractionTarget = null;
       // }
    }

    private void FixedUpdate()
    {
        if (InputManager.Instance == null)
        {
            if (_hasLoggedMissingInputManager == false)
            {
                Debug.LogWarning($"PlayerController: [{gameObject.name}] InputManager가 아직 준비되지 않아 이동 처리를 대기합니다.", this);
                _hasLoggedMissingInputManager = true;
            }
            return;
        }

        _hasLoggedMissingInputManager = false;
        MovePlayer(InputManager.Instance.MoveInput);
    }

    private void MovePlayer(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Vector3 moveDirection = _mainCamera.transform.TransformDirection(direction);
            moveDirection.y = 0f;

            Vector3 desiredDisplacement = moveDirection * MoveSpeed * Time.fixedDeltaTime;

            Vector3 resolvedDisplacement = ResolveWallDisplacement(desiredDisplacement);

            if(resolvedDisplacement.sqrMagnitude > 0.000001f)
            {
                _rb.MovePosition(_rb.position + resolvedDisplacement);
                Quaternion targetRotation = Quaternion.LookRotation(resolvedDisplacement.normalized);

                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, 0.2f));

                _anim.SetFloat("MoveSpeed", 1.0f, 0.15f, Time.fixedDeltaTime);
            }
            else
            {
                _anim.SetFloat("MoveSpeed", 0.0f, 0.02f, Time.fixedDeltaTime);
            }
        }
        else
        {
            _anim.SetFloat("MoveSpeed", 0.0f, 0.02f, Time.fixedDeltaTime);
        }

        _anim.SetFloat("yVelocity", _rb.linearVelocity.y);

    }

    private Vector3 ResolveWallDisplacement(Vector3 desiredDisplacement)
    {
        if(desiredDisplacement.sqrMagnitude <= 0.000001f)
        {
            return Vector3.zero;
        }
        if(Layer_wall.value == 0)
        {
            return desiredDisplacement;
        }

        GetMovementCapsuleWorldGeometry(out Vector3 capsulePointA, out Vector3 capsulePointB, out float capsuleRadius);

        float effectiveSkinWidth = Mathf.Min(_wallSkinWidth, capsuleRadius * 0.5f);

        float castRadius = Mathf.Max(capsuleRadius - effectiveSkinWidth, 0.001f);

        Vector3 resolvedDisplacement = Vector3.zero;
        Vector3 remainingDisplacement = desiredDisplacement;

        for(int iteration = 0; iteration < _wallSlideIterations; iteration++)
        {
            float remainingDistance = remainingDisplacement.magnitude;

            if(remainingDistance <= 0.001f)
            {
                break;
            }

            Vector3 moveDirection = remainingDisplacement / remainingDistance;

            Vector3 currentPointA = capsulePointA + resolvedDisplacement;

            Vector3 currentPointB = capsulePointB + resolvedDisplacement;

            bool hitWall = TryGetNearestWallHit(currentPointA, currentPointB, castRadius, moveDirection, remainingDistance + effectiveSkinWidth, out RaycastHit nearestHit);

            if(hitWall == false)
            {
                resolvedDisplacement += remainingDisplacement;
                break;
            }

            float allowedDistance = Mathf.Clamp(nearestHit.distance - effectiveSkinWidth, 0f, remainingDistance);

            Vector3 allowedDisplacement = moveDirection * allowedDistance;

            resolvedDisplacement += allowedDisplacement;

            Vector3 blockedRemainder = remainingDisplacement - allowedDisplacement;

            Vector3 wallNormal = nearestHit.normal;
            wallNormal.y = 0f;

            if(wallNormal.sqrMagnitude <= 0.000001f)
            {
                break;
            }

            wallNormal.Normalize();

            remainingDisplacement = Vector3.ProjectOnPlane(blockedRemainder, wallNormal);
            
            remainingDisplacement.y = 0f;

        }
        return resolvedDisplacement;
        
    }

    private bool TryGetNearestWallHit(Vector3 capsulePointA, Vector3 capsulePointB, float capsuleRadius, Vector3 direction, float distance, out RaycastHit nearestHit)
    {
        int hitCount = Physics.CapsuleCastNonAlloc(capsulePointA, capsulePointB, capsuleRadius, direction, _wallHitBuffer, distance, Layer_wall.value, QueryTriggerInteraction.Ignore);

        nearestHit = default;
        float nearestDistance = float.PositiveInfinity;
        bool foundWall = false;

        for(int i = 0; i < hitCount; i++)
        {
            RaycastHit currentHit = _wallHitBuffer[i];

            if (currentHit.collider == null)
            {
                continue;
            }
            if(currentHit.collider.transform == transform || currentHit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            if(currentHit.distance >= nearestDistance)
            {
                continue;
            }
            nearestDistance = currentHit.distance;
            nearestHit = currentHit;
            foundWall = true;

        }
        return foundWall;
    }

    private void GetMovementCapsuleWorldGeometry(out Vector3 pointA, out Vector3 pointB, out float radius)
    {
        Vector3 lossyScale = transform.lossyScale;

        Vector3 absoluteScale = new Vector3(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y), Mathf.Abs(lossyScale.z));

        Vector3 localAxis;
        float heightScale;
        float radiusScale;

        switch(_movementCollider.direction)
        {
            case 0:
                localAxis = Vector3.right;
                heightScale = absoluteScale.x;
                radiusScale = Mathf.Max(absoluteScale.y, absoluteScale.z);
                break;
            case 2:
                localAxis = Vector3.forward;
                heightScale = absoluteScale.z;
                radiusScale = Mathf.Max(absoluteScale.x, absoluteScale.y);
                break;
            default:
                localAxis = Vector3.up;
                heightScale = absoluteScale.y;
                radiusScale = Mathf.Max(absoluteScale.x, absoluteScale.z);
                break;
        }
        radius = _movementCollider.radius * radiusScale;

        float height = Mathf.Max(_movementCollider.height * heightScale, radius * 2f);

        float halfSegmentLength = Mathf.Max(0f, height * 0.5f - radius);

        Vector3 scaledLocalCenter = Vector3.Scale(_movementCollider.center, lossyScale);

        Vector3 worldCenter = _rb.position + (_rb.rotation * scaledLocalCenter);

        Vector3 worldAxis = (_rb.rotation * localAxis).normalized;

        pointA = worldCenter + worldAxis * halfSegmentLength;

        pointB = worldCenter - worldAxis * halfSegmentLength;
    }

    private void HandleGrounded()
    {
        _anim.SetBool("isGrounded", true);
        _jumpCount = 0;
    }

    private void HandleJumpPressed()
    {
        if (_jumpCount < 2)
        {
            Debug.Log($"JumpCount : {_jumpCount}");
            _anim.SetTrigger("Jump");
            _anim.SetBool("isGrounded", false);
            _rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            _jumpCount++;
        }
    }

    private void HandleAttackPressed()
    {
        //널체크 추가
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning(
                $"[{gameObject.name}] GameObjectManager가 없어 공격 요청을 전달할 수 없습니다."
            );

            return;
        }

        Vector3 center = transform.position + transform.TransformDirection(offset);
        Collider[] hitEnemies = Physics.OverlapBox(center, boxSize / 2, transform.rotation, enemyLayer);

        HashSet<int> attackedInstanceIdSet = new HashSet<int>();

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy == null)
            {
                continue;
            }
            
            IGameObjectEntity targetEntity = enemy.GetComponentInParent<IGameObjectEntity>();
            if (targetEntity == null)
            {
                Debug.LogWarning(
                    $"[{enemy.gameObject.name}] 공격 대상에서 " +
                    "IGameObjectEntity를 찾을 수 없습니다."
                );

                continue;
            }

            int targetInstanceId = targetEntity.InstanceId;

            if (targetInstanceId < 0)
            {
                Debug.LogWarning(
                    $"[{enemy.gameObject.name}] 공격 대상의 InstanceId가 유효하지 않습니다. " +
                    $"InstanceId: {targetInstanceId}"
                );

                continue;
            }
            if (attackedInstanceIdSet.Add(targetInstanceId) == false)
            {
                continue;
            }


            Vector3 direction = (enemy.transform.position - transform.position).normalized;
            direction.y = 0f;
            DamageInfo dmgInfo = new DamageInfo(_temporaryAttackDamage, false, Vector3.zero, direction, 0f, transform.gameObject);
            GameObjectManager.Instance.RequestTakeDamage(targetInstanceId, dmgInfo);
        }
    }

    private int GetAttackDamage()
    {
        if (NetworkManager.Inst == null || NetworkManager.Inst.LocalPlayerService == null)
        {
            return _temporaryAttackDamage;
        }

        PlayerModel playerModel = NetworkManager.Inst.LocalPlayerService.GetLocalPlayerModel();

        if (playerModel == null)
        {
            return _temporaryAttackDamage;
        }

        return Mathf.Max(1, Mathf.RoundToInt(playerModel.GetStatValue(StatType.AttackPower)));
    }

    private void HandleInteractPressed()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, InteractRange);

        foreach(Collider hitCollider in hitColliders)
        {
            IInteractable interactable = hitCollider.GetComponentInParent<IInteractable>();

            if(interactable == null || interactable.CanInteract == false)
            {
                continue;
            }

            interactable.Interact();
            _currentInteractionTarget = hitCollider.transform;
            break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + transform.TransformDirection(offset), transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }

    private void MoveSpeedHandler(string type)
    {
        if(type == "MoveSpeed")
        {
            float value = NetworkManager.Inst.LocalPlayerModel.GetStatValue(StatType.MoveSpeed);
            MoveSpeed = value;
        }
    }
}
