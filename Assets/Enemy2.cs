using System.Collections;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

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
    [SerializeField] private float stopDistance = 1f;

    [Header("Enemy Attack")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackTimer;

    [Header("Patrol")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    private Transform currentTarget;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;

    private EnemyState currentState;
    private enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Dead
    }



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

        currentState = EnemyState.Patrol;
    }





    private void Update()
    {
        if (!GameStateManager.IsPlaying())
        {
            return;
        }

        // 1). To check the Detection while Patrolling
        CanDetectPlayer();

        // 2). To provide Switch between methods 
        switch (currentState) 
        {
            case EnemyState.Patrol:
                if (CanDetectPlayer()) 
                { 
                    StateChange(EnemyState.Chase);

                }
                else
                {
                    Patrol();
                }
                    break;

            case EnemyState.Chase:
                if (!CanDetectPlayer()) 
                {
                    StateChange(EnemyState.Patrol);

                } else
                {
                    ChasePlayer();
                }
                    break;

            case EnemyState.Attack:
                
                if (GetDistanceToPlayer() > attackRange)
                {
                    StateChange(EnemyState.Chase);


                }
                else
                {
                    AttackPlayer();
                }
                break;
        
        }

        // 3). Attack Timer Updatation
        UpdateAttackTimer();

        //Debug.Log("CurrentState -- " + currentState);
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
            Debug.Log("ChasePlayer method state " + currentState);
            StateChange(EnemyState.Attack);
            return;
        }
    }

    private void UpdateAttackTimer()
    {
        attackTimer -= Time.deltaTime;
    }

    private void AttackPlayer()
    {
        //Debug.Log("----------AttackPlayer Trigger----------");

        if (isDead) return;

        if (attackTimer > 0)
        {
            return;
        }

        if (isAttacking) return;

        isAttacking = true;

        //Debug.Log("---Animator Triggered-----");
        animator.SetTrigger("Attack");

        Invoke(nameof(DealDamage), 0.5f);

        attackTimer = attackCooldown;

        //Debug.Log("-- Enemy attacked player --");

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
    
    // Returns distance between Enemy and Player
    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    private void StateChange(EnemyState newState)
    {
        if (newState == currentState) return;

        currentState = newState;

        Debug.Log("Current state changed to : " + currentState);
    }
}
