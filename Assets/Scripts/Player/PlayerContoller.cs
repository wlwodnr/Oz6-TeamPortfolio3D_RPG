using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10;
    public float jumpPower = 5;
    private float horizontalInput;
    private float verticalInput;
    public int jumpCount;
    private Camera mainCamera;
    public Animator anim;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            Debug.Log($"JumpCount : {jumpCount}");
            anim.SetTrigger("Jump");
            anim.SetBool("isGrounded", false);
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpCount++;
        }

        if (Input.GetKeyDown(KeyCode.A)) horizontalInput = -1f;
        if (Input.GetKeyDown(KeyCode.D)) horizontalInput = 1f;

        if (Input.GetKeyUp(KeyCode.A)) horizontalInput = Input.GetKey(KeyCode.D) ? 1f : 0f;
        if (Input.GetKeyUp(KeyCode.D)) horizontalInput = Input.GetKey(KeyCode.A) ? -1f : 0f;

        if (Input.GetKeyDown(KeyCode.W)) verticalInput = 1f;
        if (Input.GetKeyDown(KeyCode.S)) verticalInput = -1f;

        if (Input.GetKeyUp(KeyCode.W)) verticalInput = Input.GetKey(KeyCode.S) ? -1f : 0f;
        if (Input.GetKeyUp(KeyCode.S)) verticalInput = Input.GetKey(KeyCode.W) ? 1f : 0f;

        Vector3 moveVec = (camForward * verticalInput) + (camRight * horizontalInput);
        rb.MovePosition(rb.position + (moveVec.normalized * moveSpeed * Time.deltaTime));

        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        if (moveVec != Vector3.zero)
        {
            anim.SetBool("walk", true);
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }
        else
        {
            anim.SetBool("walk", false);
        }

    }





    
}