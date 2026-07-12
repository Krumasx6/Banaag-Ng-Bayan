using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public enum WeaponSlot { Primary, Secondary }

    [Header("Debug")]
    [SerializeField] private WeaponSlot currentWeapon = WeaponSlot.Primary; // Read-only display: shows Primary while J is held, Secondary right after U is pressed

    [Header("Components")]
    [SerializeField] private AimController aimController;
    [SerializeField] private PlayerMovement movement;

    [Header("Weapons")]
    [SerializeField] private WeaponPrimary primaryWeapon;
    [SerializeField] private WeaponSecondary secondaryWeapon;

    [Header("Fire Points")]
    [SerializeField] private Transform firePointStanding;
    [SerializeField] private Transform firePointCrouching;

    private void Start()
    {
        // Primary is always equipped from the start
        if (primaryWeapon != null) primaryWeapon.Equip();
    }

    private void Update()
    {
        HandlePrimary();
        HandleSecondary();
    }

    // ─── Primary Attack (J) ──────────────────────────────────────────────────

    private void HandlePrimary()
    {
        if (!InputManager.Instance.PrimaryFireHeld) return;
        if (primaryWeapon == null) return;

        currentWeapon = WeaponSlot.Primary;

        primaryWeapon.SetFirePoint(GetFirePoint());
        primaryWeapon.TryFire(ResolveFireDirection());
    }

    // ─── Secondary Attack (U) ────────────────────────────────────────────────

    private void HandleSecondary()
    {
        if (!InputManager.Instance.SecondaryFirePressed) return;
        if (secondaryWeapon == null) return;
        if (!secondaryWeapon.IsEquipped()) return;

        currentWeapon = WeaponSlot.Secondary;

        secondaryWeapon.SetFirePoint(GetFirePoint());
        secondaryWeapon.TryFire(ResolveFireDirection());
    }

    // ─── Fire Point Selection ────────────────────────────────────────────────

    private Transform GetFirePoint()
    {
        if (movement != null && movement.IsCrouching() && firePointCrouching != null)
            return firePointCrouching;

        if (firePointStanding != null)
            return firePointStanding;

        return transform;
    }

    // ─── Direction Resolution ────────────────────────────────────────────────

    private Vector2 ResolveFireDirection()
    {
        if (aimController == null)
            return movement.IsFacingRight() ? Vector2.right : Vector2.left;

        Vector2 aim = aimController.GetAimDirection();

        if (aimController.IsAimingDown() && movement.IsGrounded())
            aim = movement.IsFacingRight() ? Vector2.right : Vector2.left;

        return aim;
    }

    // ─── Public (called by WeaponPickup) — disabled until WeaponSecondary exists ─

    /*
    public void EquipSecondary(GameObject prefab, int ammo, float fireRate, int damage)
    {
        if (secondaryWeapon == null) return;
        secondaryWeapon.SetWeapon(prefab, ammo, fireRate, damage);
        secondaryWeapon.Equip();
    }

    public void ClearSecondary()
    {
        if (secondaryWeapon == null) return;
        secondaryWeapon.ClearWeapon();
    }
    */
}