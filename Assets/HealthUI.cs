using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateHealth;
    }

    void UpdateHealth(int current, int max)
    {
        fillImage.fillAmount = (float)current / max;
    }
}
