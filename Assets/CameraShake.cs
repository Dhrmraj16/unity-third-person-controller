using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 0.15f;

    private float shakeTimer;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }
    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            Vector3 randomOffset = Random.insideUnitSphere * shakeStrength;

            transform.localPosition = originalPosition + randomOffset;

        } else
        {
            transform.localPosition = originalPosition;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            ShakeTimerUpdate();
        }

    }

    public void ShakeTimerUpdate()
    {
        shakeTimer = shakeDuration;
    }


}