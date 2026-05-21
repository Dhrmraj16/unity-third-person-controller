using UnityEngine;

public class PlayerStateListener : MonoBehaviour
{
    [SerializeField] PlayerMovement movement;
    
    void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandlePlayerStateChanged;
    }
    void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandlePlayerStateChanged;
    }

    void Start()
    {
        Debug.Log("[PlayerStateListener] Start called :");
        HandlePlayerStateChanged(GameStateManager.currentState);
    }
    void HandlePlayerStateChanged(GameState state)
    {
        bool enableControls = state == GameState.Playing;

        movement.CanMove = enableControls;
       

        Debug.Log($"[PlayerStateListener] Movement enabling is {movement.enabled}");
        
    }
}
