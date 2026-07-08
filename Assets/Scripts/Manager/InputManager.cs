using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public Vector3 MoveInput {  get; private set; }
    public static event Action OnJumpPressed;

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

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        MoveInput = new Vector3(horizontal, 0, vertical).normalized;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpPressed?.Invoke();
        }
    }
}
