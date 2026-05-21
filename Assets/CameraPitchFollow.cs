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
        

        /*
        Vector3 desiredWorldPos = transform.parent.position + Vector3.up * defaultOffset.y + backward * finalDistance;

        transform.position = Vector3.Lerp(transform.position, desiredWorldPos, smoothSpeed * Time.deltaTime);
        */
    }
}














//using UnityEngine;

//class CameraPitchFollow : MonoBehaviour
//{
//    public float sensitivity = 120f;
//    public float minPitch = -40f;
//    public float maxPitch = 70f;

//    public Vector3 defaultOffset = new Vector3(0f, 1.6f, -4f);

//    public float collisionRadius = 0.3f;
//    public float collisionPadding = 0.2f;
//    public LayerMask collisionMask;

//    public float smoothSpeed = 10f;

//    float pitch;
//    Vector3 currentOffset;

//    void Start()
//    {
//        currentOffset = defaultOffset;
//    }

//    void LateUpdate()
//    {
//        if (!GameStateManager.IsPlaying()) return;
//        HandlePitch();
//        HandleCollision();
//    }

//    void HandlePitch()
//    {
//        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

//        pitch -= mouseY;
//        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

//        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
//    }

//    void HandleCollision()
//    {

//        float desiredDistance = defaultOffset.magnitude;

//        // Pivot ka backward direction 
//        Vector3 backward = -transform.parent.forward;

//        RaycastHit hit;

//        float finalDistance = desiredDistance;

//        if (Physics.SphereCast(transform.parent.position, collisionRadius, backward, out hit, desiredDistance, collisionMask))
//        {
//            finalDistance = hit.distance - collisionPadding;
//            finalDistance = Mathf.Clamp(finalDistance, 0.5f, desiredDistance);

//            Vector3 desiredPosition = backward * finalDistance;

//            currentOffset = Vector3.Lerp(currentOffset, desiredPosition, smoothSpeed * Time.deltaTime);

//            transform.localPosition = currentOffset;
//        }
//        else
//        {
//            transform.localPosition = defaultOffset;

//        }


//    }

//}

























/*
using UnityEngine;

public class CameraPitchFollow : MonoBehaviour
{
    [Header("References")]
    public Transform pivot;        // CameraPivot
    public Transform player;       // Player root

    [Header("Camera Settings")]
    public float minPitch = -40f;
    public float maxPitch = 70f;
    public float mouseSensitivity = 120f;

    [Header("Follow")]
    public Vector3 offset = new Vector3(0f, 1.6f, -4f);
    public float followSpeed = 10f;

    [Header("Collision")]
    public float collisionRadius = 0.25f;
    public LayerMask collisionMask;
    public float collisionPadding = 0.2f;

    float pitch;

    void LateUpdate()
    {
        if (!GameStateManager.IsPlaying()) return;
        HandlePitch();
        HandleCollisionFollow();
    }

    // -------------------------
    void HandlePitch()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    // -------------------------
    void HandleCollisionFollow()
    {
        //0. Ideal camera Local position
        Vector3 desiredLocalPos = offset;

        // 1. Ideal camera world position
        Vector3 desiredWorldPos =
            pivot.TransformPoint(desiredLocalPos);

        // 2. Ray direction
        Vector3 dir = (desiredWorldPos - pivot.position).normalized;
        float distance = Vector3.Distance(pivot.position, desiredWorldPos);

        // 3. Collision check
        RaycastHit hit;
        Vector3 targetPos = desiredWorldPos;

        if (Physics.SphereCast(
            pivot.position,
            collisionRadius,
            dir,
            out hit,
            distance,
            collisionMask))
        {
            targetPos =
                hit.point - dir * collisionPadding;
        }

        // 4. Smooth movement
        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPos,
                followSpeed * Time.deltaTime);
    }
}
*/














//using UnityEngine;
//using UnityEngine.UIElements;

//public class CameraPitchFollow : MonoBehaviour
//{
//    public float senstivity = 120f;
//    public float minPitch = -40f;
//    public float maxPitch = 70f;

//    public Vector3 offset = new Vector3(0f, 1.6f, -4f);

//    public float collisionRadius = 0.2f;
//    public float collisionPadding = 0.1f;
//    public LayerMask collisionMask;
//    public float minDistanceFromPlayer = 0.8f;

//    float pitch;

//    void LateUpdate()
//    {
//        if (!GameStateManager.IsPlaying()) return;
//        float mouseY = Input.GetAxis("Mouse Y") * senstivity * Time.deltaTime;
//        pitch -= mouseY;
//        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

//        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
//        //transform.localPosition = offset;
//        HandleCollision();
//    }

//    void HandleCollision()
//    {

//        Transform pivot = transform.parent;

//        Vector3 desiredLocalPos = offset;

//        Vector3 disredWorldPos = pivot.TransformPoint(desiredLocalPos);

//        Vector3 direction = disredWorldPos - pivot.position;
//        //Vector3 direction = pivot.position-disredWorldPos;
//        direction.y = 0f;
//        float distance = direction.magnitude;

//        if (Physics.SphereCast(pivot.position, collisionRadius, direction.normalized, out RaycastHit hit, distance, collisionMask))
//        {
//            float safeDistance = hit.distance - collisionPadding;
//            safeDistance = Mathf.Max(safeDistance, minDistanceFromPlayer);
//            transform.localPosition = direction.normalized * safeDistance;
//        }
//        else
//        {
//            transform.localPosition = offset;
//        }
//    }





//        

//        // 1. Ideal camera world position
//        float followSpeed = 10f;
//        Transform pivot = transform.parent;
//        Vector3 desiredWorldPos =
//            pivot.TransformPoint(offset);

//        // 2. Ray direction
//        Vector3 dir = (desiredWorldPos - pivot.position).normalized;
//        float distance = Vector3.Distance(pivot.position, desiredWorldPos);

//        // 3. Collision check
//        RaycastHit hit;
//        Vector3 targetPos = desiredWorldPos;

//        if (Physics.SphereCast(
//            pivot.position,
//            collisionRadius,
//            dir,
//            out hit,
//            distance,
//            collisionMask))
//        {
//            targetPos =
//                hit.point - dir * collisionPadding;
//        }

//        // 4. Smooth movement
//        transform.position =
//            Vector3.Lerp(
//                transform.position,
//                targetPos,
//                followSpeed * Time.deltaTime);

//    }

//}

// -----------------------------------------------------------------------------------------

/*
 * // 1. Ideal camera world position
        Vector3 desiredWorldPos =
            pivot.TransformPoint(offset);

        // 2. Ray direction
        Vector3 dir = (desiredWorldPos - pivot.position).normalized;
        float distance = Vector3.Distance(pivot.position, desiredWorldPos);

        // 3. Collision check
        RaycastHit hit;
        Vector3 targetPos = desiredWorldPos;

        if (Physics.SphereCast(
            pivot.position,
            collisionRadius,
            dir,
            out hit,
            distance,
            collisionMask))
        {
            targetPos =
                hit.point - dir * collisionPadding;
        }

        // 4. Smooth movement
        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPos,
                followSpeed * Time.deltaTime);
 */
