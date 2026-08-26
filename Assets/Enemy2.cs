using System;
using System.Collections;
using System.Data;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.UI;

public class Enemy2 : MonoBehaviour, IDamageable
{
    Rigidbody rb;

    [SerializeField] private Renderer enemyRenderer;

    private Color originalColor;


    [Header("Player Chase")]

    public bool CanThink { get; set; } = true;

    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 1f;

    [Header("Enemy Attack")]
    private bool isAttacking;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackTimer;

    [Header("Patrol")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    private Transform currentTarget;

    [Header("Detection")]
    [SerializeField] EnemyDetection enemyDetection;

    private bool reachedSearchPosition;

    [SerializeField] private float SearchWaitTime = 2f;
    private float SearchWaitTimer;

    private bool isStunned;
    [SerializeField] private float stunDuration = 1f;

    private EnemyState currentState;

    [SerializeField] EnemyState enemyState;

    [Header("Enemy HealthBar")]

    // for Refactoring Enemyhealth
    [SerializeField] EnemyHealth enemyHealth;


    // State Event 
    public event Action<EnemyState> OnStateChanged;

    // Animator connection to enemy Object
    [SerializeField] EnemyAnimator enemyAnimator;

    [Header("KnockBack")]
    [SerializeField] EnemyKnockBack enemyKnockBack;






    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        Debug.Log($"Enemy Awake >>>>>>>>>>> {GetInstanceID()} : ");
    }

    void OnEnable()
    {
        enemyHealth.OnDied += Die;

    }

    void Start()
    {
        Debug.Log($"Enemy Start >>>>>>>>>>> {GetInstanceID()} : ");
        enemyRenderer = GetComponent<Renderer>();

        originalColor = enemyRenderer.material.color;

        currentTarget = pointA;

        currentState = EnemyState.Patrol;

    }

    void OnDisable()
    {
        enemyHealth.OnDied -= Die;
    }
    void OnDestroy()
    {
        Debug.Log($"Enemy OnDestroy >>>>>>>>>>> {GetInstanceID()} : ");
    }


    private void Update()
    {
        if (!CanThink)
        {
            enemyAnimator.SetSpeed(0f);
            return;
        }

        enemyAnimator.SetSpeed(1f);


        if (isStunned) return;

        Debug.Log($"Enemy Update Frame : {Time.frameCount} Instance ID : {GetInstanceID()} | {currentState}");

        Debug.Log("Current State of Enenmy is -------------- " + currentState);
        Debug.Log("Current State of Game is ----------------" + GameStateManager.currentState);



        // 1). To give location memory to the enemy detection system 
        enemyDetection.UpdateLastKnownPlayerPosition();



        // 2). To provide Switch between methods 
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (enemyDetection.CanDetectPlayer())
                {
                    StateChange(EnemyState.Chase);

                }
                else
                {
                    Patrol();
                }
                break;

            case EnemyState.Chase:

                float distanceToPlayer = GetDistanceToPlayer();
                Debug.Log($"In chase State distance to player is {distanceToPlayer} and attack range is {attackRange}");

                if (distanceToPlayer <= attackRange) 
                {
                    Debug.Log("Enemy state now chaged from chase to attack ");
                    StateChange(EnemyState.Attack);

                } else if (!enemyDetection.CanDetectPlayer() )
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

                if (enemyDetection.CanDetectPlayer())
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

    }

    // Enemy-level reaction orchestrator
    public void TakeHit(HitInfo hit)
    {
        if (Isdead()) return;

        // Hit Aniamtion reaction triggered
        enemyAnimator.PlayHit();


        // Inform's EnemyHealth for damage 
        enemyHealth.TakeDamage((int)hit.Damage);

        // To fresh store the hit direction
        rb.linearVelocity = Vector3.zero;

        //rb.AddForce(Direction * hit.Force, ForceMode.Impulse);
        enemyKnockBack.ApplyKnockBack(hit);

        // Use to execute timed / paused methods 
        StartCoroutine(HitFlash());


        isStunned = true;
        StartCoroutine(StunRoutine());
    }


    // Enemy KnockBack method
    public void ApplyKnockBack(HitInfo hit)
    {

        Vector3 Direction = transform.position - hit.SourcePosition;
        Direction.y = 0f;
        Direction.Normalize();

        rb.AddForce(Direction * hit.Force, ForceMode.Impulse);
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
        if (Isdead()) return;

        Vector3 direction = (player.position - transform.position);
        direction.y = 0f;

        direction.Normalize();

        // To stop the enemy to collapse at the exact player position
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Debug.Log("-----------------Enemy Chasing--------------");

            transform.position += direction * moveSpeed * Time.deltaTime;

        }

        // To look in the direction of Player
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }

    }

    private void UpdateAttackTimer()
    {
        attackTimer -= Time.deltaTime;
    }

    private void AttackPlayer()
    {
        Debug.Log("Attack player method called");
        if (Isdead()) return;

        if (attackTimer > 0)
        {
            return;
        }

        if (isAttacking)
        {
            enemyAnimator.SetAttacking(isAttacking);
            return;
        }



        isAttacking = true;

        enemyAnimator.PlayAttack();

        Invoke(nameof(DealDamage), 0.5f);

        attackTimer = attackCooldown;


    }

    private void DealDamage()
    {
        HitInfo hitInfo = new HitInfo()
        {
            SourcePosition = transform.position,
            Damage = 1
        };
        Debug.Log("Enemy2 DamageAttempted :--------------------------");
        //DamageSystem.DamageAttempt(hitInfo);
        DamageSystem.ApplyHit(player.gameObject, hitInfo);
        isAttacking = false;
        enemyAnimator.SetAttacking(isAttacking);
    }



    private void Die()
    {

        StateChange(EnemyState.Dead);

        rb.linearVelocity = Vector3.zero;
        //animator.SetTrigger("Death");
        enemyAnimator.PlayDeath();

        StartCoroutine(DeathRoutine());

    }

    private bool Isdead()
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
        Vector3 direction = (currentTarget.position - transform.position);
        direction.y = 0f;
        direction.Normalize();

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


    // Returns distance between Enemy and Player
    private float GetDistanceToPlayer()
    {
        Vector3 offset = transform.position - player.position;
        offset.y = 0f;

        float distanceToPlayer = offset.magnitude;
        return distanceToPlayer;
    }

    private void StateChange(EnemyState newState)
    {
        if (newState == currentState) return;

        currentState = newState;

        Debug.Log("Current state changed to : " + currentState);

        OnStateChanged?.Invoke(currentState);

    }

    private void Search()
    {

        Vector3 direction = enemyDetection.LastKnownPlayerPosition - transform.position;

        direction.y = 0f;

        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;

        Vector3 FlatEnemyPos = transform.position;
        Vector3 FlatTargetPos = enemyDetection.LastKnownPlayerPosition;

        FlatEnemyPos.y = 0f;
        FlatTargetPos.y = 0f;

        float distance = Vector3.Distance(FlatEnemyPos, FlatTargetPos);

        if (distance < 0.1f)
        {
            reachedSearchPosition = true;
            return;
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

}