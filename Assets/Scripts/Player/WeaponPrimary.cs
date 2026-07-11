using UnityEngine;

public class WeaponPrimary : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 6f; // shots per second

    private Transform firePoint;
    private bool isEquipped;
    private float nextFireTime;

    // ─── Equip ───────────────────────────────────────────────────────────────

    public void Equip()
    {
        isEquipped = true;
    }

    public bool IsEquipped() => isEquipped;

    // ─── Called by PlayerCombat every frame before firing ───────────────────

    public void SetFirePoint(Transform point)
    {
        firePoint = point;
    }

    // ─── Fire ────────────────────────────────────────────────────────────────

    public void TryFire(Vector2 direction)
    {
        if (!isEquipped) return;
        if (projectilePrefab == null || firePoint == null) return;
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + (1f / fireRate);

        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Init(direction);
        }
        else
        {
            Debug.LogWarning($"[{name}] WeaponPrimary: projectilePrefab has no Projectile component attached.", this);
        }
    }
}