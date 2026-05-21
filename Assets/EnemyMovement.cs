using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Speed")]
    public float moveSpeed = 2f;
    
    [Header("Attack Range")]
    public float attackRange = 2.5f;

    private Transform player;
    private CharacterController controller;

    [SerializeField] Enemy enemyObj;
    
    public bool isAttacking;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        
        player = GameObject.FindWithTag("Player").transform;
        if (player == null)
        {
            Debug.Log("[EnemyMovement] Player object not found! Make Sure Player Tag Exist");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        
        if (!GameStateManager.IsPlaying()) return;
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = player.position;
        enemyPos.y = 0f;
        playerPos.y = 0f;

        float distance = Vector3.Distance(enemyPos,playerPos);
        //Debug.Log($"[Enemy Movement] Dist ={distance}: Attack Range = {attackRange}");
        if (distance > attackRange)
        {
            isAttacking = false;
          MoveTowardsPlayer();
        } else
        {
            isAttacking = true;

            AttackPlayer();
            
            
        }

    }
    void MoveTowardsPlayer()
    {
        if (isAttacking) return;
        Vector3 direction = (player.position - transform.position);
        direction.y = 0f;
        direction = direction.normalized;
        controller.Move(direction * moveSpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        enemyObj.TryAttack();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player == null) return;

        Gizmos.color = Color.green;
        Vector3 direction = (player.position - transform.position).normalized;
        Gizmos.DrawLine(transform.position, transform.position + direction * 2f);
    }
}
