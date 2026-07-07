using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public PlayerController controller;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            controller.anim.SetBool("isGrounded", true);
            controller.jumpCount = 0;
        }
    }
}