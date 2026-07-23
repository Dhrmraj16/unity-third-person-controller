using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] EnemyHealth enemyHealth;
    [SerializeField] Transform healthBarCanvas;
    [SerializeField] Image healthImage;
    [SerializeField] Slider healthSlider;
    private float healthVisibleTime = 0.2f;

    void Start()
    {
        healthBarCanvas.gameObject.SetActive(false);
    }
    void OnEnable()
    {
        enemyHealth.OnHealthChanged += HandleHealthChanged;
    }

    void OnDisable()
    {
        enemyHealth.OnHealthChanged -= HandleHealthChanged;

    }

    void LateUpdate()
    {
        healthBarCanvas.forward = Camera.main.transform.position;
    }

    private void HideHealthCanvas()
    {
        healthBarCanvas.gameObject.SetActive(false);
    }
    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
       
        float healthParcentage = (float)currentHealth / maxHealth;

        if (healthParcentage > 0.6)
        {
            healthImage.color = Color.green;
        } else if (healthParcentage > 0.3)
        {
            healthImage.color = Color.yellow;
        } else
        {
            healthImage.color = Color.red;
        }

        healthBarCanvas.gameObject.SetActive(true);

        CancelInvoke(nameof(HideHealthCanvas));
        Invoke(nameof(HideHealthCanvas), healthVisibleTime);

    }

}
