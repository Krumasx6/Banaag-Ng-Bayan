using UnityEngine;
public class AimController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement movement;
    private Vector2 aimDirection = Vector2.right;
    private Vector2 lastFacingDirection = Vector2.right;
    private void Update()
    {
        ResolveAim();
    }
    // ─── Aim Resolution ──────────────────────────────────────────────────────
    private void ResolveAim()
    {
        float h = GetHorizontalInput(); 
        float v = GetVerticalInput(); 
        if (h != 0f)
            lastFacingDirection = new Vector2(h, 0f);
        bool isGrounded = movement != null && movement.IsGrounded();
        if (v < 0f && isGrounded)
            v = 0f; 
        Vector2 raw = new Vector2(h, v);
        if (raw == Vector2.zero)
        {
            aimDirection = lastFacingDirection;
        }
        else
        {
            aimDirection = SnapTo8(raw);
        }
    }
    // ─── 8-Direction Snap ────────────────────────────────────────────────────
    private Vector2 SnapTo8(Vector2 raw)
    {
        float x = Mathf.Abs(raw.x) > 0.3f ? Mathf.Sign(raw.x) : 0f;
        float y = Mathf.Abs(raw.y) > 0.3f ? Mathf.Sign(raw.y) : 0f;
        if (x == 0f && y == 0f)
            return aimDirection;
        return new Vector2(x, y).normalized;
    }
    // ─── Input Readers ───────────────────────────────────────────────────────
    private float GetHorizontalInput()
    {
        if (Input.GetKey(KeyCode.A)) return -1f;
        if (Input.GetKey(KeyCode.D)) return  1f;
        return 0f;
    }
    private float GetVerticalInput()
    {
        if (Input.GetKey(KeyCode.W)) return  1f;
        if (Input.GetKey(KeyCode.S)) return -1f;
        return 0f;
    }
    // ─── Public Accessors (read by PlayerCombat) ─────────────────────────────
    public Vector2 GetAimDirection() => aimDirection;
    public bool IsAimingDown() => aimDirection.y < 0f;
    public bool IsAimingUp() => aimDirection.y > 0f;
    public bool IsAimingHorizontal() => aimDirection.y == 0f;
}