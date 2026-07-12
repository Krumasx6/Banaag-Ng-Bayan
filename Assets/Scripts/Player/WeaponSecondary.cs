using UnityEngine;

public class WeaponSecondary : MonoBehaviour
{
    private Transform firePoint;
    private bool isEquipped = true; // no pickup system yet — always equipped for now

    public bool IsEquipped() => isEquipped;

    public void SetFirePoint(Transform point)
    {
        firePoint = point;
    }

    public void TryFire(Vector2 direction)
    {
        string character = CharacterManager.Instance != null ? CharacterManager.Instance.SelectedCharacter : "Unknown";

        if (character == "Datu Sulo")
        {
            Debug.Log("Datu Sulo secondary ability");
        }
        else if (character == "Tala Lingayan")
        {
            Debug.Log("Tala Lingayan secondary ability");
        }
        else if (character == "Lakan Yano")
        {
            Debug.Log("Lakan Yano secondary ability");
        }
        else if (character == "Nara Alon")
        {
            Debug.Log("Nara Alon secondary ability");
        }
        else
        {
            Debug.Log("Unknown character — no secondary ability set up");
        }
    }
}