using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TakeHit(Vector3 hitDirection, float force)
    {
        rb.linearVelocity = Vector3.zero;

        rb.AddForce(hitDirection * force, ForceMode.Impulse);
    }
}
