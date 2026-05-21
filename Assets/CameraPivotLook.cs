using UnityEngine;

public class CameraPivotLook : MonoBehaviour
{
    public float senstivity = 120f;
    float yaw;

    void LateUpdate()
    {
        if (!GameStateManager.IsPlaying()) return;
        float mouseX = Input.GetAxis("Mouse X") * senstivity * Time.deltaTime;
        yaw += mouseX;

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }


}
