using UnityEngine;

public class CursorController : MonoBehaviour
{ 
    void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleStateChanged;
    }

    void Start()
    {
        Debug.Log("[CursorController] Start called :");
        HandleStateChanged(GameStateManager.currentState);
    }
    void HandleStateChanged(GameState state)
    {
        bool free = (state != GameState.Playing);
        Cursor.lockState = free ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = free;
        Debug.Log($"[CursorController's HandleStateChanged] free = {free} state = {state}");
    }
}
