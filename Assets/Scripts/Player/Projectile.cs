using UnityEngine;
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 3f;
    [Header("Layers")]
    [SerializeField] private LayerMask destroyOnLayers;  // set to Ground, Wall, Enemy etc.
    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    private Vector2 direction;
    private bool initialized;
    private void Start()
    {
        Destroy(gameObject, lifetime);
        if (initialized) AlignToDirection();
    }
    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        initialized = true;
        AlignToDirection();
    }
    private void Update()
    {
        if (!initialized) return;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    // ─── Collision ───────────────────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore other projectiles
        if (other.CompareTag("PlayerProjectile")) return;
        // Ignore the player
        if (other.CompareTag("Player")) return;
        // Check if this layer should destroy the projectile
        bool shouldDestroy = ((destroyOnLayers.value & (1 << other.gameObject.layer)) != 0);
        // Always try to damage regardless of layer
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }
        // No damageable found — only destroy if layer is in the destroy list
        if (shouldDestroy)
        {
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }
    // ─── Helpers ─────────────────────────────────────────────────────────────
    private void AlignToDirection()
    {
        if (direction == Vector2.zero) return;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    private void SpawnHitEffect()
    {
        if (hitEffectPrefab == null) return;
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
    }
}