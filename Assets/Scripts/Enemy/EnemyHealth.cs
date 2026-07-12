using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 30;

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color hitFlashColor = Color.white;
    [SerializeField] private float hitFlashDuration = 0.08f;

    [Header("Death")]
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private float destroyDelay = 0f; // set above 0 if you add a death animation later and want it to play out first

    private int currentHealth;
    private Color originalColor;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        if (sprite != null) originalColor = sprite.color;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"[{name}] took {amount} damage — {currentHealth}/{maxHealth} HP left");

        FlashHit();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ─── Feedback ────────────────────────────────────────────────────────────

    private void FlashHit()
    {
        if (sprite == null) return;
        CancelInvoke(nameof(ResetColor));
        sprite.color = hitFlashColor;
        Invoke(nameof(ResetColor), hitFlashDuration);
    }

    private void ResetColor()
    {
        if (sprite != null) sprite.color = originalColor;
    }

    // ─── Death ───────────────────────────────────────────────────────────────

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"[{name}] destroyed");

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // TODO: hook up score/drops/objective tracking once those systems exist

        Destroy(gameObject, destroyDelay);
    }
}