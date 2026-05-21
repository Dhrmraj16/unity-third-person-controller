using System;
using UnityEngine;

public static class GameEventManager
{
    public static event Action OnPlayerDeath;
    public static void RaisePlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }
    
    public static void ResetState()
    {       
        OnPlayerDeath = null;
    }


}