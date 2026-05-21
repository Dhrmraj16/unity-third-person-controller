using UnityEngine;
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Player root
    public Transform cameraPivot; // Empty object (pivot)

    [Header("Camera Setting")]
    public float mouseSensitivity = 3f;
    public float minPatch = -40f;
    public float maxPitch = 70f;

    [Header("Follow Setting")]
    public Vector3 offset = new Vector3(0f, 1.6f, -3f);
    public float followSpeed = 10f;

    [Header("Camera Dead Zone")]
    [SerializeField] float deadZoneRadius = 0.2f;

    float yaw;
    float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yaw = player.eulerAngles.y;
        pitch = 0f;
    }
    void LateUpdate()
    {
        if (!GameStateManager.IsPlaying()) return;
        if (!player) return;
        HandleRotation();
        FollowPlayer();
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, minPatch, maxPitch);

    }

    void FollowPlayer()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 desiredPosition = player.position + rotation * offset;
        //transform.position = desiredPosition;
        float distance = Vector3.Distance(transform.position, desiredPosition);

        if (distance < deadZoneRadius) return;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        //transform.LookAt(player.position + Vector3.up * 1.6f); 
    }

     
    /*
    void Update()
    {
        if (!GameStateManager.IsPlaying()) return;

        if (!player) return;
        
        // 1) Mouse Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, minPatch, maxPitch);

    }
    void LateUpdate()
    {
        // Smooth camera rotation
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
        cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, targetRotation, followSmooth * Time.deltaTime);

    }
    */
    //void LateUpdate()
    //{
    //    if (!GameStateManager.IsPlaying()) return;

    //    if (!player) return;

    //    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
    //    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

    //    yaw += mouseX;
    //    pitch -= mouseY;

    //    pitch = Mathf.Clamp(pitch, minPatch, maxPitch);

    //    transform.position = player.position + Vector3.up * 2f;
    //    transform.rotation = Quaternion.Euler(0f, yaw, 0f);

    //    Camera.main.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

    //}


}
