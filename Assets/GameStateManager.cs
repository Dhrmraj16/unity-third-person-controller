using JetBrains.Annotations;
using System;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Paused,
    Dead
}

public class GameStateManager 
{
    public static GameState currentState { get; private set; } = GameState.Playing;

    public static event Action<GameState> OnGameStateChanged;

    public static void SetState(GameState newState)
    {
        if (currentState == newState)
        { 
        Debug.Log($"[GameStateManager] Ignored : Already {newState}");
        return;
        }
        
        Debug.Log($"[GameStateManager] Changed {currentState} to {newState}");
        currentState = newState;

        OnGameStateChanged?.Invoke(currentState);

        if (OnGameStateChanged != null)
        {
            foreach(Delegate d in OnGameStateChanged.GetInvocationList())
            {
                Debug.Log("Delegated Listed method :- " + d.Method.Name);
            }

        } else
        {
            Debug.Log("----------Delegate List is NULL -------");
        }
    }

    public static void Paused() => GameStateManager.SetState(GameState.Paused);
    public static void Resume() => GameStateManager.SetState(GameState.Playing);
    public static void Die()    => GameStateManager.SetState(GameState.Dead);
    public static bool IsPlaying() => currentState == GameState.Playing;
    public static bool IsPaused()  => currentState == GameState.Paused;
    public static bool IsDead()    => currentState == GameState.Dead;
}
