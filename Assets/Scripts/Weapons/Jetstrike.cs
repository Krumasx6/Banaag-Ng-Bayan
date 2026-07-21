using UnityEngine;
using System;

public class JetStrike : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private float flySpeed = 50f;
    [SerializeField] private float edgeBuffer = 2f; // extra distance past the camera edges before the jet despawns

    // Fired the moment this jet finishes crossing the screen — the sequence controller listens for this
    public Action onReachedEnd;

    private Camera cam;
    private float endX;

    private void Start()
    {
        cam = Camera.main;

        if (cam != null)
        {
            float halfWidth = cam.orthographicSize * cam.aspect;
            float startX = cam.transform.position.x - halfWidth - edgeBuffer;
            endX = cam.transform.position.x + halfWidth + edgeBuffer;

            transform.position = new Vector3(startX, transform.position.y, transform.position.z);
        }
    }

    private void Update()
    {
        // Uses UNSCALED time so the jet keeps flying even while Time.timeScale is 0
        // (gameplay is frozen for this special, but the jet still needs to visibly move)
        transform.Translate(Vector3.right * flySpeed * Time.unscaledDeltaTime, Space.World);

        if (cam != null && transform.position.x >= endX)
        {
            onReachedEnd?.Invoke();
            Destroy(gameObject);
        }
    }
}