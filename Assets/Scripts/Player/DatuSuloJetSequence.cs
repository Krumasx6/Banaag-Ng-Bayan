using UnityEngine;
using System.Collections;

public class DatuSuloJetSequence : MonoBehaviour
{
    [Header("Jets")]
    [SerializeField] private GameObject jetPrefab;
    [SerializeField] private int jetCount = 3;
    [SerializeField] private float jetSpawnHeight = 6f;
    [SerializeField] private float delayBetweenJets = 0.4f; // real-time seconds, unaffected by the freeze

    [Header("Final Explosion")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private int explosionCount = 6; // how many explosion visuals to scatter across the screen
    [SerializeField] private int damage = 999; // guarantees a kill on anything hit
    [SerializeField] private LayerMask damageLayers; // set to whatIsEnemy
    [SerializeField] private float explosionEffectLifetime = 1f;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.3f;

    public void PlaySequence()
    {
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        Time.timeScale = 0f; // freeze gameplay — jets still move since JetStrike uses unscaled time

        for (int i = 0; i < jetCount; i++)
        {
            bool jetDone = false;

            GameObject jetGO = Instantiate(jetPrefab, new Vector3(transform.position.x, jetSpawnHeight, 0f), Quaternion.identity);
            JetStrike jet = jetGO.GetComponent<JetStrike>();
            if (jet != null) jet.onReachedEnd += () => jetDone = true;

            yield return new WaitUntil(() => jetDone);
            yield return new WaitForSecondsRealtime(delayBetweenJets);
        }

        TriggerFinalExplosion();

        if (cameraShake != null)
            cameraShake.Shake(shakeDuration, shakeMagnitude);

        yield return new WaitForSecondsRealtime(shakeDuration);

        Time.timeScale = 1f; // resume gameplay
    }

    private void TriggerFinalExplosion()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float halfWidth = cam.orthographicSize * cam.aspect;
        float halfHeight = cam.orthographicSize;
        Vector3 camPos = cam.transform.position;

        // Scatter explosion visuals across the visible screen
        for (int i = 0; i < explosionCount; i++)
        {
            if (explosionEffectPrefab == null) break;

            float rx = Random.Range(-halfWidth, halfWidth);
            float ry = Random.Range(-halfHeight, halfHeight);
            Vector3 pos = new Vector3(camPos.x + rx, camPos.y + ry, 0f);

            GameObject effect = Instantiate(explosionEffectPrefab, pos, Quaternion.identity);
            Destroy(effect, explosionEffectLifetime);
        }

        // Damage everything currently visible inside the camera view
        Vector2 boxCenter = new Vector2(camPos.x, camPos.y);
        Vector2 boxSize = new Vector2(halfWidth * 2f, halfHeight * 2f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, damageLayers);

        foreach (Collider2D hit in hits)
        {
            IDamageable dmg = hit.GetComponent<IDamageable>();
            if (dmg != null) dmg.TakeDamage(damage);
        }
    }
}