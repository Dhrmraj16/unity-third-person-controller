using UnityEngine;

public class PlayerStateListener : MonoBehaviour
{
    [SerializeField] PlayerMovement movement;
    
    void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChanged;
    }
    void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    void Start()
    {
        Debug.Log("[PlayerStateListener] Start called :");
        HandleGameStateChanged(GameStateManager.currentState);
    }
    void HandleGameStateChanged(GameState state)
    {
        bool enableControls = state == GameState.Playing;

        movement.CanMove = enableControls;
       

        Debug.Log($"[PlayerStateListener] Movement enabling is {movement.enabled}");
        
    }
}
