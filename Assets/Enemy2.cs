using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    Rigidbody rb;

    private Renderer enemyRenderer;

    private Color originalColor;

    [Header("Enemy Health")]
    [SerializeField] private int health = 3;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();

        originalColor = enemyRenderer.material.color;
    }

    public void TakeHit(Vector3 hitDirection, float force)
    {
        // Health AI system
        health--;
        Debug.Log("Enemy Health " + health);

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        // To fresh store the hit direction
        rb.linearVelocity = Vector3.zero;

        rb.AddForce(hitDirection * force, ForceMode.Impulse);

        // Use to execute timed / paused methods 
        StartCoroutine(HitFlash());
    }

    // Timed / Paused Method 
    private IEnumerator HitFlash()
    {
        enemyRenderer.material.color = Color.red;

        yield return new WaitForSeconds(0.25f);

        enemyRenderer.material.color = originalColor;
    }
}
