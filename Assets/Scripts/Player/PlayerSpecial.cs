using UnityEngine;

public class PlayerSpecial : MonoBehaviour
{
    [Header("Datu Sulo — Jet Strike Sequence")]
    [Tooltip("Optional manual assignment. If left empty, it will be auto-found on this object or its children at runtime.")]
    [SerializeField] private DatuSuloJetSequence jetSequence;

    private void Awake()
    {
        // Auto-resolve so this works even inside a prefab, where you can't
        // drag in a reference from outside the prefab's own hierarchy.
        if (jetSequence == null)
            jetSequence = GetComponentInChildren<DatuSuloJetSequence>(true);
    }

    private void Update()
    {
        if (!InputManager.Instance.SpecialPressed) return;

        string character = CharacterManager.Instance != null ? CharacterManager.Instance.SelectedCharacter : "Datu Sulo"; // fallback for testing TrainingGround directly

        if (character == "Datu Sulo")
        {
            TriggerJetStrike();
        }
        else
        {
            Debug.Log($"[PlayerSpecial] No special ability implemented yet for '{character}'");
        }
    }

    private void TriggerJetStrike()
    {
        if (jetSequence == null)
        {
            Debug.LogWarning("[PlayerSpecial] Jet Sequence is not assigned.", this);
            return;
        }

        jetSequence.PlaySequence();
    }
}