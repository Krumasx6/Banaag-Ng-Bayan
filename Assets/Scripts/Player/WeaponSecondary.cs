using UnityEngine;

public class WeaponSecondary : MonoBehaviour
{
    [Header("Grenade")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float throwForce = 8f;
    [SerializeField] private float throwUpwardForce = 4f; // gives it a small arc, like Metal Slug's toss
    [SerializeField] private float fireRate = 1f; // grenades per second

    [Header("Facing")]
    [SerializeField] private PlayerMovement movement; // used to get left/right facing — aim direction is ignored on purpose

    private Transform firePoint;
    private bool isEquipped = true; // no pickup system yet — always equipped for now
    private float nextFireTime;

    public bool IsEquipped() => isEquipped;

    public void SetFirePoint(Transform point)
    {
        firePoint = point;
    }

    // 'direction' is intentionally unused — grenades always throw based on which way the character is facing,
    // not the aim direction, matching Metal Slug's grenade behavior (no aiming up/down for it)
    public void TryFire(Vector2 direction)
    {
        if (!isEquipped) return;
        if (grenadePrefab == null || firePoint == null) return;
        if (movement == null)
        {
            Debug.LogWarning($"[{name}] WeaponSecondary: 'Movement' reference is not assigned — can't determine facing direction.", this);
            return;
        }
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + (1f / fireRate);

        float facing = movement.IsFacingRight() ? 1f : -1f;

        GameObject grenadeGO = Instantiate(grenadePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = grenadeGO.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(facing * throwForce, throwUpwardForce);
        }
        else
        {
            Debug.LogWarning($"[{name}] WeaponSecondary: grenadePrefab has no Rigidbody2D — it won't move or bounce.", this);
        }
    }
}