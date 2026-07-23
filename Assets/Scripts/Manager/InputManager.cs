using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public Vector3 MoveInput {  get; private set; }
    public static event Action OnJumpPressed;
    public static event Action OnAttackPressed;
    public static event Action OnInteractPressed;

    private bool _isGamePlayInputEnable = false;

    private int _activeUiCount = 0;

    public bool IsUIActive
    {   get
        {
            return _activeUiCount > 0;
        }
    }

    public bool CanProcessGameplayInput
    {
        get
        {
            return _isGamePlayInputEnable && IsUIActive == false;
        }
    }

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
        _isGamePlayInputEnable = GameManager.Instance != null && GameManager.Instance.IsPlaying();
        UpdateCursorState();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //UI가 열려있거나, Playing이 아니라면, 움직임 막기.
        if(CanProcessGameplayInput == false)
        {
            MoveInput = Vector3.zero;
            return;
        }


        MoveInput = new Vector3(horizontal, 0, vertical).normalized;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpPressed?.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnAttackPressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteractPressed?.Invoke();
        }
    }

    public void SetCursorAndInputState(bool isOpen)
    {
        if(isOpen)
        {
            _activeUiCount++;
        }
        else
        {
            _activeUiCount = Mathf.Max(0, _activeUiCount - 1);
        }

        UpdateCursorState();
    }

    public void SetGameplayInputState(bool isEnable)
    {
        _isGamePlayInputEnable = isEnable;

        if(_isGamePlayInputEnable == false)
        {
            MoveInput = Vector3.zero;
        }
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        if(CanProcessGameplayInput == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
    }
}
