using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public Vector3 MoveInput {  get; private set; }
    public static event Action OnJumpPressed;
    public static event Action OnAttackPressed;

    private int _activeUiCount = 0;
    public bool IsUIActive => _activeUiCount > 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        //UpdateCursorState();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        MoveInput = new Vector3(horizontal, 0, vertical).normalized;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpPressed?.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnAttackPressed?.Invoke();
        }
    }

    //public void SetCursorAndInputState(bool isOpen)
    //{
    //    if (isOpen)
    //    {
    //        _activeUiCount++;
    //    }
    //    else
    //    {
    //        _activeUiCount = Mathf.Max(0, _activeUiCount - 1);
    //    }

    //    UpdateCursorState();
    //}

    //private void UpdateCursorState()
    //{
    //    if (IsUIActive)
    //    {
    //        Cursor.lockState = CursorLockMode.None;
    //        Cursor.visible = true;
    //    }
    //    else
    //    {
    //        Cursor.lockState = CursorLockMode.Locked;
    //        Cursor.visible = false;
    //    }
    //}
}
