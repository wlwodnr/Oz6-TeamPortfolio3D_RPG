using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; 
    public float sensitivity = 2.0f;
    public Vector3 offset = new Vector3(0, 2f, -5f);
    private float mouseX, mouseY;

    void Update()
    {
        // 마우스 입력값 누적
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivity; 
        mouseY = Mathf.Clamp(mouseY, -30f, 60f); 
    }

    void LateUpdate()
    {

        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        transform.position = target.position + (transform.rotation * offset);
    }
}
