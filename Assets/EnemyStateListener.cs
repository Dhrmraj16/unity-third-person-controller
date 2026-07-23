using Unity.VisualScripting;
using UnityEngine;

public class EnemyStateListener : MonoBehaviour
{
    [SerializeField] Enemy2 enemy;

    void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleEnemyStateChanged;
    }

    void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleEnemyStateChanged;
    }
    void HandleEnemyStateChanged(GameState state)
    {
        bool enableControls = state == GameState.Playing;

        enemy.CanThink = enableControls;
    }
}
