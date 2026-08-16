using System;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public static void ApplyHit(GameObject target, HitInfo hitinfo)
    {
        IDamageable damagaable = target.GetComponent<IDamageable>();

        if (damagaable != null)
        {
            damagaable.TakeHit(hitinfo);
        }
    }
}
