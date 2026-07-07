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

    private void Awake()
    {
        _groundCheck.OnGrounded += HandleGrounded;
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        MovePlayer(InputManager.Instance.MoveInput, InputManager.Instance.IsJumpTriggered);
    }

    private void MovePlayer(Vector3 direction, bool jumpTrigger)
    {
        if (jumpTrigger && _jumpCount < 2)
        {
            Debug.Log($"JumpCount : {_jumpCount}");
            _anim.SetTrigger("Jump");
            _anim.SetBool("isGrounded", false);
            _rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            _jumpCount++;
        }


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
}