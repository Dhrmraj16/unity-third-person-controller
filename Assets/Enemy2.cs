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

    [Header("Player Chase")]
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1.5f;

    [Header("Enemy Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();

        originalColor = enemyRenderer.material.color;
    }

    private void Update()
    {
        if (!GameStateManager.IsPlaying())
        {
            return;
        }

        // 1). Chase player position
        ChasePlayer();

        // 2). Attack Timer Updatation
        UpdateAttackTimer();
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

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        // To stop the enemy to collapse at the exact player position
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {

          transform.position += direction * moveSpeed * Time.deltaTime;

        }

        if (distance <= attackRange)
        {
            AttackPlayer();
        }
    }

    private void UpdateAttackTimer()
    {
        attackTimer -= Time.deltaTime;
    }

    private void AttackPlayer()
    {
        if (attackTimer > 0)
        {
            return;
        }

        attackTimer = attackCooldown;

        Debug.Log("-- Enemy attacked player --");

    }
}
