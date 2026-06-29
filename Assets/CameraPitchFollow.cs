using UnityEngine;

class CameraPitchFollow : MonoBehaviour
{
    public float sensitivity = 120f;
    public float minPitch = -40f;
    public float maxPitch = 70f;

    public Vector3 defaultOffset = new Vector3(0f, 1.6f, -4f);

    public float collisionRadius = 0.3f;
    public float collisionPadding = 0.2f;
    public LayerMask collisionMask;

    public float smoothSpeed = 10f;

    float pitch;
    Vector3 currentOffset;

    void Start()
    {
        currentOffset = defaultOffset;
    }

    void LateUpdate()
    {
        if (!GameStateManager.IsPlaying()) return;
        HandlePitch();
        HandleCollision();
    }



    void HandlePitch()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

    }

    void HandleCollision()
    {
        float desiredDistance = Mathf.Abs(defaultOffset.z);

        Vector3 backward =- transform.parent.forward;

        RaycastHit hit;

        float finalDistance = desiredDistance;

        if (Physics.SphereCast(transform.parent.position, collisionRadius, backward, out hit, desiredDistance, collisionMask))
        {
            finalDistance = hit.distance - collisionPadding;
            finalDistance = Mathf.Clamp(finalDistance, 1f, desiredDistance);
        }
        
        Vector3 desiredLocalPos = new Vector3(defaultOffset.x, defaultOffset.y, -finalDistance);

        currentOffset = Vector3.Lerp(currentOffset, desiredLocalPos, smoothSpeed * Time.deltaTime);

        transform.localPosition = currentOffset;
    }
}