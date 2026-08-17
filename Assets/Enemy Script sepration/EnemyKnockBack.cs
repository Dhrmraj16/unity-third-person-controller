using UnityEngine;

public class EnemyKnockBack : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyKnockBack(HitInfo hitInfo)
    {
        rb.linearVelocity = Vector3.zero;

        Vector3 Direction = transform.position - hitInfo.SourcePosition;
        Direction.y = 0f;

        Direction.Normalize();

        rb.AddForce(Direction * hitInfo.Force, ForceMode.Impulse);
    }

}
