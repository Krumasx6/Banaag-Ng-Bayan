using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Smoothing")]
    [SerializeField] private float damping = 0.3f;

    [Header("Vertical Follow")]
    [SerializeField] private bool followVertical = false; // false = camera Y stays fixed, ignores jumping entirely
    [SerializeField] private float fixedY = 0f; // used when Follow Vertical is off

    [Header("Forward-Only Scroll (Metal Slug style)")]
    [SerializeField] private bool lockBackwardScroll = true;
    [SerializeField] private float leftBoundaryBuffer = 0.5f; // extra room the player can walk behind the camera edge before getting blocked

    [Header("Backtrack Wall")]
    [SerializeField] private Transform backtrackWall; // assign a GameObject with a BoxCollider2D (NOT trigger) — physically blocks the player

    [Header("Background")]
    [SerializeField] private Transform background; // assign your background Sprite's Transform — moves 1:1 with the camera, no parenting needed
    [SerializeField] private float backgroundZ = 10f; // world Z for the background sprite, keep positive so it renders in front of the camera

    private Vector3 velocity = Vector3.zero;
    private float farthestX;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        FindPlayer();
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        Vector3 targetPosition = target.position + offset;

        if (!followVertical)
        {
            targetPosition.y = fixedY;
        }

        if (lockBackwardScroll)
        {
            // Camera can only ever move further right — never scrolls back once it's advanced
            farthestX = Mathf.Max(farthestX, targetPosition.x);
            targetPosition.x = farthestX;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position, targetPosition, ref velocity, damping);

        UpdateBacktrackWall();
        UpdateBackground();
    }

    private void UpdateBacktrackWall()
    {
        if (backtrackWall == null) return;

        float leftEdge = GetLeftBoundaryX();
        backtrackWall.position = new Vector3(leftEdge, backtrackWall.position.y, backtrackWall.position.z);
    }

    private void UpdateBackground()
    {
        if (background == null) return;

        background.position = new Vector3(transform.position.x, transform.position.y, backgroundZ);
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            transform.position = target.position + offset;
            farthestX = transform.position.x;
        }
    }

    // ─── Public: current visible left edge, used by PlayerMovement to block backtracking ───

    public float GetLeftBoundaryX()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null || !cam.orthographic) return float.NegativeInfinity;

        float halfWidth = cam.orthographicSize * cam.aspect;
        return (transform.position.x - halfWidth) - leftBoundaryBuffer;
    }
}