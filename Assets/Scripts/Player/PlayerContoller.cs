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

    private Rigidbody _rb;

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
        _groundCheck.OnGrounded += HandleGrounded;
        InputManager.OnJumpPressed += HandleJumpPressed;
        InputManager.OnAttackPressed += HandleAttackPressed;
        InputManager.OnInteractPressed += HandleInteractPressed;
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _anim = GetComponent<Animator>();
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

    void FixedUpdate()
    {
        MovePlayer(InputManager.Instance.MoveInput);
    }

    private void MovePlayer(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Vector3 moveDirection = _mainCamera.transform.TransformDirection(direction);
            moveDirection.y = 0f;
            _rb.MovePosition(_rb.position + (moveDirection * MoveSpeed * Time.deltaTime));

            _anim.SetFloat("MoveSpeed", 1.0f, 0.15f, Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }
        else
        {
            _anim.SetFloat("MoveSpeed", 0.0f, 0.02f, Time.deltaTime);
        }

        _anim.SetFloat("yVelocity", _rb.linearVelocity.y);

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
            DamageInfo dmgInfo = new DamageInfo(_temporaryAttackDamage, false, Vector3.zero, direction, transform.gameObject);
            GameObjectManager.Instance.RequestTakeDamage(targetInstanceId, dmgInfo);
        }
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
}