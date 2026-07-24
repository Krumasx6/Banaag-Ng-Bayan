using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string characterSelectScene = "CharacterSelection";

    [Header("Panels")]
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private GameObject settingsPanel;

    // Wire the Start button's OnClick to this
    public void OnStartPressed()
    {
        SceneManager.LoadScene(characterSelectScene);
    }

    // Wire the Settings button's OnClick to this
    public void OnSettingsPressed()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (buttonsPanel != null) buttonsPanel.SetActive(false);
    }

    // Wire the Exit button's OnClick to this
    public void OnExitPressed()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}