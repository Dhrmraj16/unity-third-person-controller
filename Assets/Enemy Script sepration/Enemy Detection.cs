using UnityEditor;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float visionAngle = 60f;

    private Vector3 lastKnownPlayerPosition;
    public Vector3 LastKnownPlayerPosition => lastKnownPlayerPosition;


    private float dot;
    private float visionThreshold;
    public bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position);
        directionToPlayer.y = 0f;
        directionToPlayer.Normalize();

        dot = Vector3.Dot(transform.forward, directionToPlayer);

        visionThreshold = Mathf.Cos(visionAngle * 0.5f * Mathf.Deg2Rad);

        // for Debuging eye
        Debug.DrawRay(transform.position,transform.forward * 3f ,Color.blue);
        Debug.DrawRay(transform.position,directionToPlayer * 3f,Color.red);

        return dot >= visionThreshold;
    }

    public bool CanDetectPlayer()
    {

        Vector3 offset = transform.position - player.position;
        offset.y = 0f;

        float distance = offset.magnitude;

        bool canSee = CanSeePlayer();
        bool detected = distance <= detectionRange && canSee;

        return detected;
    }



    public void UpdateLastKnownPlayerPosition()
    {
        if (CanDetectPlayer())
        {
            lastKnownPlayerPosition = player.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;

        // Forward ray
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(
            origin,
            transform.forward * detectionRange
        );

        // Vision cone boundary rays
        Gizmos.color = Color.green;

        Quaternion leftRotation =
            Quaternion.Euler(0f, -visionAngle * 0.5f, 0f);

        Quaternion rightRotation =
            Quaternion.Euler(0f, visionAngle * 0.5f, 0f);

        Vector3 leftDirection =
            leftRotation * transform.forward;

        Vector3 rightDirection =
            rightRotation * transform.forward;

        Gizmos.DrawRay(
            origin,
            leftDirection * detectionRange
        );

        Gizmos.DrawRay(
            origin,
            rightDirection * detectionRange
        );
    }
}