using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Grenade : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private LayerMask damageLayers; // set to whatIsEnemy
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float explosionEffectLifetime = 1f; // effect gets destroyed after this long, so its animation/particles stop instead of looping forever

    [Header("Fuse")]
    [SerializeField] private float fuseTime = 3f; // auto-explodes after this long even if it never hits an enemy — set to 0 to disable

    [Header("Spin")]
    [SerializeField] private float spinSpeed = 360f; // degrees per second — tumbles like a rolling wheel as it flies

    private Rigidbody2D rb;
    private bool hasExploded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Spin direction follows whichever way this grenade is actually moving,
        // so it works correctly regardless of who threw it or how
        float facing = Mathf.Sign(rb.linearVelocity.x);
        rb.angularVelocity = -facing * spinSpeed; // negative so moving right spins clockwise, like it's rolling forward

        if (fuseTime > 0f)
            Invoke(nameof(Explode), fuseTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bouncing off ground/walls is handled natively by a bouncy Physics Material 2D
        // on this object's Collider2D (set that up in the Inspector, not here).
        // This script only cares about the one case that should end the bounce early: hitting something damageable.
        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, GetEffectLifetime(effect));
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageLayers);
        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private float GetEffectLifetime(GameObject effect)
    {
        Animator anim = effect.GetComponent<Animator>();

        if (anim != null && anim.runtimeAnimatorController != null && anim.runtimeAnimatorController.animationClips.Length > 0)
        {
            // Read the actual clip length instead of guessing — matches the animation exactly
            return anim.runtimeAnimatorController.animationClips[0].length;
        }

        // No Animator found (e.g. a Particle System instead) — fall back to the manual duration
        return explosionEffectLifetime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}