using System.Collections;
using System.Data;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.UI;

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
    [SerializeField] private int health = 5;

    [Header("Player Chase")]

    public bool CanThink { get; set; } = true;

    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1f;

    [Header("Enemy Attack")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackTimer;

    [Header("Patrol")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    private Transform currentTarget;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float visionAngle = 60f;
    private Vector3 lastKnownPlayerPosition;

    private bool reachedSearchPosition;

    [SerializeField] private float SearchWaitTime = 2f;
    private float SearchWaitTimer;

    private bool isStunned;
    [SerializeField] private float stunDuration = 1f;

    private EnemyState currentState;
    private enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Search,
        Dead
    }

    [Header("Enemy HealthBar")]    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthFill;

    [SerializeField] private Transform healthBarCanvas;
    [SerializeField] private float healthBarSetActiveTime = 3f;
    private float healthBarTimer;

    


    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();
        Debug.Log($"Enemy Awake >>>>>>>>>>> {GetInstanceID()} : ");
    }

    void Start()
    {
        Debug.Log($"Enemy Start >>>>>>>>>>> {GetInstanceID()} : ");
        enemyRenderer = GetComponent<Renderer>();

        originalColor = enemyRenderer.material.color;

        currentTarget = pointA;

        currentState = EnemyState.Patrol;

        UpdateAnimatorState();

        healthSlider.maxValue = health;
        healthSlider.value = health;

        UpdateHealthBar();

        // To make Health Bar invisible at the start of the game
        healthBarCanvas.gameObject.SetActive(false);
    }


    void OnDestroy()
    {
        Debug.Log($"Enemy OnDestroy >>>>>>>>>>> {GetInstanceID()} : ");
    }


    private void Update()
    {
        //if (!GameStateManager.IsPlaying())
        //{
        //    animator.speed = 0f;
        //    return;
        //}
        //else
        //{
        //    animator.speed = 1f;
        //}

        if (!CanThink)
        {
            animator.speed = 0f;
            return;
        }

        
            animator.speed = 1f;
        

        if (isStunned) return;

        Debug.Log($"Enemy Update Frame : {Time.frameCount} Instance ID : {GetInstanceID()} | {currentState}");

        Debug.Log("Current State of Enenmy is -------------- " + currentState);
        Debug.Log("Current State of Game is ----------------" + GameStateManager.currentState);



        // 1). To give location memory to the enemy detection system 
        UpdateLastKnownPlayerPosition();



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

                    reachedSearchPosition = false;

                    SearchWaitTimer = SearchWaitTime;

                    StateChange(EnemyState.Search);
                }
                else
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

            case EnemyState.Search:

                if (CanDetectPlayer())
                {
                    StateChange(EnemyState.Chase);
                    break;
                }
                if (!reachedSearchPosition)
                {
                    Debug.Log("Searching..........................................");
                    Search();

                }
                else
                {

                    SearchWaitTimer -= Time.deltaTime;

                    if (SearchWaitTimer <= 0)
                    {
                        StateChange(EnemyState.Patrol);
                    }
                    else
                    {
                        transform.Rotate(0f, 60f * Time.deltaTime, 0f);
                    }

                }
                break;


            case EnemyState.Dead:
                break;


        }

        // 3). Attack Timer Updatation
        UpdateAttackTimer();


        // 4). HealthBarTimer Updtation 
        UpdateHealthBarVisibility();
    }

    private void LateUpdate()
    {
        healthBarCanvas.forward = Camera.main.transform.forward;
    }




    public void TakeHit(Vector3 hitDirection, float force)
    {
        if (isdead()) return;

        // Hit Aniamtion reaction triggered
        animator.SetTrigger("Hit");


        // Health AI system
        health--;
        Debug.Log("Enemy Health " + health);
        healthSlider.value = health;
        UpdateHealthBar();

        // This is to Show HealthBar on screen when enemy Got hit
        healthBarCanvas.gameObject.SetActive(true);
        healthBarTimer = healthBarSetActiveTime;



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

        isStunned = true;
        StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
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
        if (isdead()) return;

        Vector3 direction = (player.position - transform.position).normalized;

        // To stop the enemy to collapse at the exact player position
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Debug.Log("-----------------Enemy Chasing--------------");

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

        if (isdead()) return;

        if (attackTimer > 0)
        {
            return;
        }

        if (isAttacking)
        {
            animator.SetBool("isAttacking", isAttacking);

            return;
        }



        isAttacking = true;

        animator.SetTrigger("Attack");

        Invoke(nameof(DealDamage), 0.5f);

        attackTimer = attackCooldown;


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
        animator.SetBool("isAttacking", isAttacking);
    }



    private void Die()
    {

        StateChange(EnemyState.Dead);

        healthBarCanvas.gameObject.SetActive(false);
        rb.linearVelocity = Vector3.zero;
        animator.SetTrigger("Death");

        StartCoroutine(DeathRoutine());

    }

    private bool isdead()
    {
        return currentState == EnemyState.Dead;
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

        Debug.Log("-----------------Enemy Petrolling--------------");
        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance < 0.2f)
        {
            if (currentTarget == pointA)
            {
                currentTarget = pointB;
            }
            else
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

        return (distance <= detectionRange && CanSeePlayer());

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

        UpdateAnimatorState();
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        float dot = Vector3.Dot(transform.forward, directionToPlayer);

        float visionThreshold = Mathf.Cos(visionAngle * 0.5f * Mathf.Deg2Rad);

        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        Debug.DrawRay(transform.position, directionToPlayer * 3, Color.red);
        return dot >= visionThreshold;
    }

    private void UpdateLastKnownPlayerPosition()
    {
        if (CanDetectPlayer())
        {
            lastKnownPlayerPosition = player.position;

        }

    }

    private void Search()
    {

        Vector3 direction = lastKnownPlayerPosition - transform.position;

        direction.y = 0f;

        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;

        Vector3 FlatEnemyPos = transform.position;
        Vector3 FlatTargetPos = lastKnownPlayerPosition;

        FlatEnemyPos.y = 0f;
        FlatTargetPos.y = 0f;

        float distance = Vector3.Distance(FlatEnemyPos, FlatTargetPos);

        if (distance < 0.1f)
        {
            reachedSearchPosition = true;
            return;
        }


    }



    private void UpdateAnimatorState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:

                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);

                break;

            case EnemyState.Chase:

                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);

                break;

            case EnemyState.Search:

                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);

                break;

            case EnemyState.Attack:

                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);

                break;

            case EnemyState.Dead:

                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);

                break;
        }

    }


    void OnDrawGizmosSelected()
    {
        if (enemyRenderer == null) return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void UpdateHealthBar()
    {
        healthSlider.value = health;

        float healthPercantage = (float)health / healthSlider.maxValue;

        if (healthPercantage > 0.6f)
        {
            healthFill.color = Color.green;
        }
        else if (healthPercantage > 0.3f)
        {
            healthFill.color = Color.yellow;
        }
        else
        {
            healthFill.color = Color.red;
        }
    }


    // To set time or to hide the health bar
    private void UpdateHealthBarVisibility()
    {
        if (!healthBarCanvas.gameObject.activeSelf) return;

        healthBarTimer -= Time.deltaTime;

        if (healthBarTimer <= 0)
        {
            healthBarCanvas.gameObject.SetActive(false);

        }

    }

}
