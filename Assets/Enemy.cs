using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyMovement enemyPosition;
    [SerializeField] private int AttackAmount;
    public float attackInterval = 0.2f;

    private float attackTimer;

    private bool CanAttack;
    void OnEnable()
    {
        Debug.Log("[Enemy] Enable Called : ");
    }

    void OnDisable()
    {
        Debug.Log("[Enemy] Disable Called : ");
    }
    void Start()
    {
        attackTimer = 0f;
        CanAttack = false;
    }
    void Update()
    {
        if (!GameStateManager.IsPlaying()) return;
        UpdateCooldown();
    }

    void UpdateCooldown()
    {
        if (!CanAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                CanAttack = true;
            }
        }

    }

    public void TryAttack()
    {
        if (GameStateManager.IsDead()) return;
        if (!CanAttack) return;

        CanAttack = false;
        Debug.Log("[Enemy Attack triggered : ]");
        Attack();
        
    }
    void Attack()
    {
        if (GameStateManager.currentState != GameState.Playing) return;
        Debug.Log($"[Enemy] Attacked With amount {AttackAmount}");
        //GameEventManager.RaiseOnDamage(AttackAmount);
        DamageSystem.DamageAttempt(AttackAmount,transform.position);

    }
}
