using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Slider playerHealthSlider;
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
        playerHealthSlider.maxValue = max;
        playerHealthSlider.value = current;

        float healthParcentage = (float)current / max;

        if (healthParcentage > 0.6)
        {
            fillImage.color = Color.green;
        }
        else if (healthParcentage > 0.3)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }

    }
}
