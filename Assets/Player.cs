using UnityEngine;

public class Player : MonoBehaviour
{
    //private int currentHealth = 100;
    //private bool IsDead = false;
    

    //void OnEnable()
    //{
    //    currentHealth = 100;
    //    Debug.Log("[Player] Enable Called : ");
    //    GameEventManager.OnDamage += TookDamage;

    //}

    //void OnDisable()
    //{
    //    Debug.Log("[Player] Disable Called : ");
    //    GameEventManager.OnDamage -= TookDamage;
    //}
    
    //public void TookDamage(int amount)
    //{
    //    Debug.Log("[Player] TakeDamage Called : ");
    //    if (IsDead) return;
    //    currentHealth -= amount;

    //    if (currentHealth <=0)
    //    {
    //        IsDead = true;
    //        Debug.Log("Player is Dead : ");
    //        GameEventManager.RaisePlayerDeath();
    //        GameStateManager.Die();

    //        return;
    //    }
    //    Debug.Log($"[Player] CurrentHealth Reduced to {currentHealth}");
    //}

}