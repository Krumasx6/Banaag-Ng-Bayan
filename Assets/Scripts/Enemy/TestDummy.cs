using UnityEngine;

public class TestDummy : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool respawnOnDeath = true;
    [SerializeField] private float respawnDelay = 1.5f;

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color hitFlashColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.1f;

    private int currentHealth;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;
        if (sprite != null) originalColor = sprite.color;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"[{name}] took {amount} damage — {currentHealth}/{maxHealth} HP left");

        FlashHit();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

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

    private void Die()
    {
        Debug.Log($"[{name}] destroyed");

        if (respawnOnDeath)
        {
            gameObject.SetActive(false);
            Invoke(nameof(Respawn), respawnDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Respawn()
    {
        currentHealth = maxHealth;
        ResetColor();
        gameObject.SetActive(true);
    }
}