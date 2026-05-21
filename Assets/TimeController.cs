using UnityEngine;

public class TimeController : MonoBehaviour
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
        Debug.Log("[TimeController] Start called : ");
        HandleStateChanged(GameStateManager.currentState);
    }
    void HandleStateChanged(GameState state)
    {
        bool paused = (GameStateManager.currentState != GameState.Playing);
        Time.timeScale = paused ? 0f : 1f;
        Debug.Log($"[TimeController's HandleStateChanged] Time = {Time.timeScale} : state = {state}");
    }


}
