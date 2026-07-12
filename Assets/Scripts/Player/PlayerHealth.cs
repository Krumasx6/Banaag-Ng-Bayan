using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityDuration = 1f; // brief immunity after getting hit, so one overlap doesn't chain-hit every frame
    [SerializeField] private float flickerInterval = 0.1f; // how fast the sprite blinks while invulnerable

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer[] spriteRenderers; // assign both upper and lower body renderers here

    [Header("On Death")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerCombat combat;

    private int currentHealth;
    private bool isInvulnerable;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[{name}] took {amount} damage — {currentHealth}/{maxHealth} HP left");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityFlicker());
        }
    }

    // ─── Invulnerability ─────────────────────────────────────────────────────

    private IEnumerator InvulnerabilityFlicker()
    {
        isInvulnerable = true;
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invulnerabilityDuration)
        {
            visible = !visible;
            SetSpritesVisible(visible);

            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }

        SetSpritesVisible(true);
        isInvulnerable = false;
    }

    private void SetSpritesVisible(bool visible)
    {
        if (spriteRenderers == null) return;

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr != null) sr.enabled = visible;
        }
    }

    // ─── Death ───────────────────────────────────────────────────────────────

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"[{name}] died");

        if (movement != null) movement.enabled = false;
        if (combat != null) combat.enabled = false;

        // TODO: hook up respawn / game over screen once that flow exists
    }
}