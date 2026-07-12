using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "TrainingGround";

    // Wire each of the 4 character buttons' OnClick to one of these
    public void SelectDatuSulo() => Select("Datu Sulo");
    public void SelectTalaLingayan() => Select("Tala Lingayan");
    public void SelectLakanYano() => Select("Lakan Yano");
    public void SelectNaraAlon() => Select("Nara Alon");

    private void Select(string characterName)
    {
        CharacterManager.Instance.SetCharacter(characterName);
        Debug.Log($"Selected: {characterName}");
    }

    // Wire the Proceed button's OnClick to this
    public void OnProceedPressed()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }
}