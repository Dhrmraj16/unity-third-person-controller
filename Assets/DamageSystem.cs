using System;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public static event Action<HitInfo> OnDamageAttempt;
    public static event Action<HitInfo> OnDamageApplied;

    public static void ApplyDamage(HitInfo hitinfo)
    {
        OnDamageApplied?.Invoke(hitinfo);
    }

    public static void DamageAttempt(HitInfo hitinfo)
    {
        OnDamageAttempt?.Invoke(hitinfo);
    }

    public static void ResetState()
    {
        OnDamageAttempt = null;
        OnDamageApplied = null;
    }

    public static void ApplyHit(GameObject target, HitInfo hitinfo)
    {
        IDamageable damagaable = target.GetComponent<IDamageable>();

        if (damagaable != null)
        {
            damagaable.TakeHit(hitinfo);
        }
    }
}
