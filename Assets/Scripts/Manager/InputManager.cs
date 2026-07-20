using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    private bool _isMoveActive = true;
    public Vector3 MoveInput {  get; private set; }
    public CameraController CameraController;
    public static event Action OnJumpPressed;
    public static event Action OnAttackPressed;

    private void Awake()
    {
        if(Instance == null)
        {
            LockCursor();
            CameraController = Camera.main.GetComponent<CameraController>();
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if(_isMoveActive == true)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            MoveInput = new Vector3(horizontal, 0, vertical).normalized;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnJumpPressed?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                OnAttackPressed?.Invoke();
            }
        }
        else
        {
            MoveInput = Vector3.zero;
        }

    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ActiveMove()
    {
        _isMoveActive = true;
    }
    public void DisableMove()
    {
        _isMoveActive = false;
    }

    public void ActiveCamera()
    {
        CameraController.ActiveCameraMove();
    }

    public void DisableCamera()
    {
        CameraController.DisableCameraMove();
    }
}
