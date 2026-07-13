using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; 
    public float sensitivity = 2.0f;
    public Vector3 offset = new Vector3(0, 2f, -5f);
    private float mouseX, mouseY;


    private void Start()
    {
        LockCursor();
    }
    private void OnDisable()
    {
        UnlockCursor();
    }

    void Update()
    {
        // 마우스 입력값 누적
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivity; 
        mouseY = Mathf.Clamp(mouseY, -30f, 60f); 
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        transform.position = target.position + (transform.rotation * offset);
    }
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
