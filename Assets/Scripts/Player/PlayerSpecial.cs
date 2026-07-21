using UnityEngine;

public class PlayerSpecial : MonoBehaviour
{
    [Header("Datu Sulo — Jet Strike Sequence")]
    [SerializeField] private DatuSuloJetSequence jetSequence;

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