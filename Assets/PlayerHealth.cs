 using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Health Variable's And Field
    [SerializeField] private int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    // CoolDown Timer Variables and Field
    [SerializeField] private float invincibilityDuration = 0.15f;
    private bool isInvincible = false;
    public bool Isinvincible => isInvincible;
    private float invincibilityTimer;

    // Health Check Static Events Field
    public static event Action<int, int> OnHealthChanged;

    // Flash While Taking Damage
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.2f;

    // Armor
    [SerializeField] private int armor = 8;

    // Player Flash While Taking Damage
    Color originalColor;

    void Awake()
    {
        Debug.Log("[PlayerHealth] Awake Called :");
        CurrentHealth = maxHealth;
        //isInvincible = false;
        invincibilityTimer = invincibilityDuration;
    }
    void OnEnable()
    {
        Debug.Log("[PlayerHealth] OnEnable Called :");
        
        CurrentHealth = maxHealth; 
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        //GameEventManager.OnDamage += TakeDamage;
        DamageSystem.OnDamageAttempt += AttemptedDamage;
        DamageSystem.OnDamageApplied += TakeDamage;

    }

    void OnDisbale()
    {
        Debug.Log("[PlayerHealth] OnDisable Called :");
        //GameEventManager.OnDamage -= TakeDamage;
        DamageSystem.OnDamageAttempt -= AttemptedDamage;
        DamageSystem.OnDamageApplied -= TakeDamage;
        OnHealthChanged = null;
    }

    void Start()
    {
        originalColor = playerRenderer.material.color;
    }

    void Update()
    {
        if (!GameStateManager.IsPlaying()) return;
        HandleInvincibilityTimer();
    }
    void AttemptedDamage(int amount, Vector3 hitSourcePosition)
    {
            Debug.Log($"[PlayerHealth] AttemptedDamage called before condition Invincibility is {isInvincible} :");
        //if (isInvincible) return;
        if (isInvincible)
        {
            Debug.Log($"[PlayerHealth] Invincibility is {isInvincible} So cannot Attack Now");
            Debug.Log($"[PlayerHealth] Invincibility TImer Value is {invincibilityTimer}");
            return;

        }
        if (GameStateManager.IsDead()) return;
        Debug.Log("[PlayerHealth] DamageApplied Event Invoked :");
        DamageSystem.ApplyDamage(amount, hitSourcePosition);
    }
    void TakeDamage(int amount, Vector3 hitSourcePosition)
    {
        Debug.Log("[PlayerHealth] TakeDamage Called :");
        Debug.Log($"[PlayerHealth] isInvincibility is {isInvincible}");
        
        int finalDamage = Mathf.Max(amount - armor, 1);

        CurrentHealth -= finalDamage;

        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        Debug.Log("[PlayerHealth] OnHealthChanged Event Invoked :");
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);


        if (CurrentHealth <= 0)
        {
            Debug.Log("Player is Dead : ");
            GameStateManager.Die();

            return;
        }

        FlashHit();

        StartInvincibility();

        Debug.Log($"[PlayerHealth's] CurrentHealth Reduced to {CurrentHealth}");
        
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ApplyKnockback(hitSourcePosition);
        }
        
    }

    void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }
    void HandleInvincibilityTimer()
    {
        //Debug.Log($"[PlayerHealth] HandleInvincibilityTimer Called Invicibility is {isInvincible}");
        if (!isInvincible) return;
        //Debug.Log($"[PlayerHealth] HandleInvincibilityTimer called invincibilityTimer value is {invincibilityTimer}");
        invincibilityTimer -= Time.deltaTime;

        if (invincibilityTimer <= 0)
        {
            Debug.Log($"[PlayerHealth] Invincibility TImer Reached to 0 :");
            isInvincible = false;
        }
    }

    void FlashHit()
    {
        playerRenderer.material.color = hitColor;
        Invoke(nameof(ResetColor), hitFlashDuration);
    }

    void ResetColor()
    {
        playerRenderer.material.color = originalColor;
    }
}
