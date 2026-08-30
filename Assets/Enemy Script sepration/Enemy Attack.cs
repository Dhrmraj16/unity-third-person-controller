using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    [Header("Enemy Attack")]

    [SerializeField] private float attackCooldown = 1.5f;

    private bool isAttacking;
    private float attackTimer;

    [SerializeField] private GameObject player;
    [SerializeField] private EnemyAnimator enemyAnimator;


    //private void Update()
    //{
    //    Debug.Log($"Enemy Attack Update Frame : {Time.frameCount} Instance ID : {GetInstanceID()}");

    //    UpdateAttackTimer();
    //}
    public void AttackPlayer()
    {
        Debug.Log("Attack player method called");
        //if (Isdead()) return;

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
        Debug.Log("EnemyAttack class DealDamage called :--------------------------");

        DamageSystem.ApplyHit(player, hitInfo);

        isAttacking = false;
        enemyAnimator.SetAttacking(isAttacking);
    }

    public void UpdateAttackTimer()
    {
        Debug.Log($"Enemy Attack Timer is {attackTimer}");
        attackTimer -= Time.deltaTime;
    }

}
