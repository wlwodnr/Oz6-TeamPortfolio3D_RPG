using UnityEngine;
using System;

public class GroundCheck : MonoBehaviour
{
    public event Action OnGrounded;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            OnGrounded?.Invoke();
        }
    }
}