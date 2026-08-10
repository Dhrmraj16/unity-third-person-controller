using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        SetState(EnemyState.Patrol);
    }


    public void SetState(EnemyState state)
    {
        switch (state)
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

    public void PlayHit()
    {
        animator.SetTrigger("Hit");
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void SetAttacking(bool value)
    {
        animator.SetBool("isAttacking", value);
    }

    public void PlayDeath()
    {
        animator.SetTrigger("Death");
    }

    public void SetSpeed(float speed)
    {
        animator.speed = speed;
    }

}