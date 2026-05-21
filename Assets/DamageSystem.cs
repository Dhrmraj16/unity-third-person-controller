using System;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public static event Action<int, Vector3> OnDamageAttempt;
    public static event Action<int , Vector3 > OnDamageApplied;

    public static void ApplyDamage(int amount, Vector3 sourcePosition)
    {
        OnDamageApplied?.Invoke(amount, sourcePosition);
    }

    public static void DamageAttempt(int amount, Vector3 sourcePosition)
    {
        OnDamageAttempt?.Invoke(amount, sourcePosition);
    }

    public static void ResetState()
    {
        OnDamageAttempt = null;
        OnDamageApplied = null;
    }
}
