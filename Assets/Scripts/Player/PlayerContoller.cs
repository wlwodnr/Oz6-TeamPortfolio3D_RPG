using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GroundCheck _groundCheck;
    [SerializeField] public float MoveSpeed = 10;
    [SerializeField] public float JumpPower = 5;
    public int _jumpCount;
    private Camera _mainCamera;
    private Animator _anim;

    private Rigidbody _rb;

    //아래는 1차빌드용 공격기능 변수 (이후에 바뀔 수 있음)
    [Header("1차 빌드용 공격관련 변수")]
    public Vector3 boxSize = new Vector3(1, 1, 1); 
    public Vector3 offset = new Vector3(0, 1, 1); 
    public LayerMask enemyLayer;

    private void Awake()
    {
        _groundCheck.OnGrounded += HandleGrounded;
        InputManager.OnJumpPressed += HandleJumpPressed;
        InputManager.OnAttackPressed += HandleAttackPressed;
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _anim = GetComponent<Animator>();
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
        Vector3 center = transform.position + transform.TransformDirection(offset);
        Collider[] hitEnemies = Physics.OverlapBox(center, boxSize / 2, transform.rotation, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Vector3 direction = (enemy.transform.position - transform.position).normalized;
            DamageInfo dmgInfo = new DamageInfo(10, false, Vector3.zero, direction, transform.gameObject);
            enemy.GetComponent<IDamageable>().TakeDamage(dmgInfo);
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