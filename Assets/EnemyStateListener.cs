using Unity.VisualScripting;
using UnityEngine;

public class EnemyStateListener : MonoBehaviour
{
    [SerializeField] Enemy2 enemy;
    [SerializeField] EnemyAnimator enemyAnimator;

    void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChanged;
        enemy.OnStateChanged += HandleEnemyStateChanged;
    }

    void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChanged;
        enemy.OnStateChanged -= HandleEnemyStateChanged;
    }
    void HandleGameStateChanged(GameState state)
    {
        bool enableControls = state == GameState.Playing;

        enemy.CanThink = enableControls;
    }

    void HandleEnemyStateChanged(EnemyState state)
    {
        Debug.Log("EnemyStateListener received state " + state);
        enemyAnimator.SetState(state);
        
    }

}
