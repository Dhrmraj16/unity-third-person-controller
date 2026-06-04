using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] private Renderer enemyRenderer;

    private Color originalColor;

    // Animator
    private Animator animator;
    private bool isAttacking;
    private bool isDead;

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

    [Header("Patrol")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    private Transform currentTarget;
    private bool isPatrollingToB;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();
    }

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();

        originalColor = enemyRenderer.material.color;

        currentTarget = pointA;
    }


    private void Update()
    {
        if (!GameStateManager.IsPlaying())
        {
            return;
        }

        // 1). Condition For in Range Detection 
       if (CanDetectPlayer())
        {
            ChasePlayer();
        } else
        {
            Patrol();
        }

        // 2). Attack Timer Updatation
        UpdateAttackTimer();
    }
    public void TakeHit(Vector3 hitDirection, float force)
    {
        if (isDead) return;

        // Health AI system
        health--;
        Debug.Log("Enemy Health " + health);

        if (health <= 0)
        {
            Die();
            return;
        }

        // To fresh store the hit direction
        rb.linearVelocity = Vector3.zero;

        // Enemy KnockBack method
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
        if (isDead) return;

        Vector3 direction = (player.position - transform.position).normalized;

        // To stop the enemy to collapse at the exact player position
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {

          transform.position += direction * moveSpeed * Time.deltaTime;

        }

        direction.y = 0;
        
        // To look in the direction of Player
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
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
        if (isDead) return;

        if (attackTimer > 0)
        {
            return;
        }

        if (isAttacking) return;

        isAttacking = true;

        animator.SetTrigger("Attack");

        Invoke(nameof(DealDamage), 0.5f);

        attackTimer = attackCooldown;

        Debug.Log("-- Enemy attacked player --");

    }

    private void DealDamage()
    {
        PlayerMovement playerMovement =
            player.GetComponent<PlayerMovement>();
    
        if (playerMovement != null)
        {
            playerMovement.TakeDamage(1, transform.position);
        }
    
        isAttacking = false;
    }

    private void Die()
    {
        isDead = true;
    
        rb.linearVelocity = Vector3.zero;
        animator.SetTrigger("Death");

        StartCoroutine(DeathRoutine());

    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1.49f);

        Destroy(gameObject);
    }

    private void Patrol()
    {
        Vector3 direction = (currentTarget.position - transform.position).normalized;   
        
        transform.position += direction * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance < 0.2f)
        {
            if (currentTarget == pointA)
            {
                currentTarget = pointB;
            } else
            {
                currentTarget = pointA;
            }
        }

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    private bool CanDetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        return distance <= detectionRange;

    }
}
