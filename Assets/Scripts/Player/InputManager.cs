using UnityEngine;

// Single source of truth for input. PlayerMovement, PlayerCombat, and AimController
// all read from here instead of checking KeyCode directly. Keyboard works right now;
// touch UI scripts (built later) call the "Trigger/Set" methods below to feed the
// same values in from on-screen buttons. Both work at the same time — nothing about
// keyboard play changes.
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    // ─── Touch-fed backing values (set by on-screen UI scripts, built later) ───
    private float touchHorizontal;
    private float touchVertical;
    private bool touchPrimaryHeld;
    private bool touchJumpPressed;
    private bool touchDashPressed;
    private bool touchDashReleased;
    private bool touchSecondaryPressed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void LateUpdate()
    {
        // Clear one-frame touch flags after everyone's had a chance to read them this frame
        touchJumpPressed = false;
        touchDashPressed = false;
        touchDashReleased = false;
        touchSecondaryPressed = false;
    }

    // ─── Public read API — this is what gameplay scripts call ──────────────

    public float Horizontal
    {
        get
        {
            float kb = 0f;
            if (Input.GetKey(KeyCode.A)) kb = -1f;
            if (Input.GetKey(KeyCode.D)) kb = 1f;
            return kb != 0f ? kb : touchHorizontal;
        }
    }

    public float Vertical
    {
        get
        {
            float kb = 0f;
            if (Input.GetKey(KeyCode.W)) kb = 1f;
            if (Input.GetKey(KeyCode.S)) kb = -1f;
            return kb != 0f ? kb : touchVertical;
        }
    }

    public bool JumpPressed => Input.GetKeyDown(KeyCode.K) || touchJumpPressed;
    public bool DashPressed => Input.GetKeyDown(KeyCode.I) || touchDashPressed;
    public bool DashReleased => Input.GetKeyUp(KeyCode.I) || touchDashReleased;
    public bool PrimaryFireHeld => Input.GetKey(KeyCode.J) || touchPrimaryHeld;
    public bool SecondaryFirePressed => Input.GetKeyDown(KeyCode.U) || touchSecondaryPressed;

    // ─── Called by on-screen touch UI scripts (build these when adding mobile controls) ───

    public void SetMoveInput(Vector2 direction)
    {
        touchHorizontal = direction.x;
        touchVertical = direction.y;
    }

    public void SetPrimaryHeld(bool held) => touchPrimaryHeld = held;
    public void TriggerJump() => touchJumpPressed = true;
    public void TriggerDashPressed() => touchDashPressed = true;
    public void TriggerDashReleased() => touchDashReleased = true;
    public void TriggerSecondaryPressed() => touchSecondaryPressed = true;
}