using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
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

    }

    void OnDisable()
    {
        Debug.Log("[PlayerHealth] OnDisable Called :");
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

    public void TakeHit(HitInfo hitInfo)
    {
        Debug.Log($"[PlayerHealth] TakeHit called before condition Invincibility is {isInvincible} :");
        //if (isInvincible) return;
        if (isInvincible)
        {
            Debug.Log($"[PlayerHealth] Invincibility is {isInvincible} So cannot Attack Now");
            Debug.Log($"[PlayerHealth] Invincibility TImer Value is {invincibilityTimer}");
            return;

        }
        if (GameStateManager.IsDead()) return;
        Debug.Log("[PlayerHealth] TakeHit Validated :");

        int finalDamage = Mathf.Max((int)hitInfo.Damage - armor, 1);

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
            movement.ApplyKnockback(hitInfo.SourcePosition);
        }

    }

    void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }
    void HandleInvincibilityTimer()
    {
        if (!isInvincible) return;

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
