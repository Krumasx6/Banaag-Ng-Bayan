using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Character Names — must match CharacterManager's roster exactly, same order as the arrays below")]
    public string[] characterNames; // e.g. "Datu Sulo", "Lakan Yano", "Tala Lingayan", "Nara Alon"

    [Header("Card Slots — all arrays must be in the same order")]
    public Button[] cardButtons;
    public Image[] cardImages;
    public GameObject[] cardNameTexts;
    public GameObject[] cardRoleTexts;
    public GameObject[] cardDescriptionTexts;

    [Header("Colors")]
    public Color32 deselectedColor = new Color32(145, 145, 145, 255);
    public Color32 selectedColor = new Color32(255, 255, 255, 255);

    [Header("Proceed")]
    public string gameplaySceneName = "TrainingGround";

    [Header("Force Card Active State")]
    public bool cardsActiveAtStart = true;

    private int selectedIndex = -1;

    private void Start()
    {
        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (cardButtons[i] == null)
            {
                Debug.LogWarning($"[CharacterSelectUI] cardButtons[{i}] is not assigned.", this);
                continue;
            }

            cardButtons[i].gameObject.SetActive(cardsActiveAtStart);
            Debug.Log($"[CharacterSelectUI] cardButtons[{i}] ('{cardButtons[i].name}') gameObject.SetActive({cardsActiveAtStart})");

            int index = i; // captured per-button, so each listener gets its own correct index
            cardButtons[i].onClick.AddListener(() => OnCardClicked(index));
        }

        // Start fully deselected
        for (int i = 0; i < cardImages.Length; i++)
        {
            SetCardState(i, false);
        }
    }

    private void OnCardClicked(int index)
    {
        string charName = (index < characterNames.Length) ? characterNames[index] : "UNKNOWN";
        Debug.Log($"[CharacterSelectUI] Card {index} clicked: {charName}");

        selectedIndex = index;

        for (int i = 0; i < cardButtons.Length; i++)
        {
            SetCardState(i, i == index);
        }

        if (index < characterNames.Length && CharacterManager.Instance != null)
        {
            CharacterManager.Instance.SetCharacter(characterNames[index]);
        }
    }

    private void SetCardState(int index, bool selected)
    {
        Debug.Log($"[CharacterSelectUI] SetCardState({index}, selected={selected})");

        if (cardImages != null && index < cardImages.Length && cardImages[index] != null)
        {
            cardImages[index].color = selected ? selectedColor : deselectedColor;
            Debug.Log($"[CharacterSelectUI] cardImages[{index}] color set to {(selected ? "SELECTED" : "DESELECTED")}");
        }

        if (cardNameTexts != null && index < cardNameTexts.Length && cardNameTexts[index] != null)
        {
            cardNameTexts[index].SetActive(selected);
            Debug.Log($"[CharacterSelectUI] cardNameTexts[{index}] ('{cardNameTexts[index].name}') SetActive({selected})");
        }

        if (cardRoleTexts != null && index < cardRoleTexts.Length && cardRoleTexts[index] != null)
        {
            cardRoleTexts[index].SetActive(selected);
            Debug.Log($"[CharacterSelectUI] cardRoleTexts[{index}] ('{cardRoleTexts[index].name}') SetActive({selected})");
        }

        if (cardDescriptionTexts != null && index < cardDescriptionTexts.Length && cardDescriptionTexts[index] != null)
        {
            cardDescriptionTexts[index].SetActive(selected);
            Debug.Log($"[CharacterSelectUI] cardDescriptionTexts[{index}] ('{cardDescriptionTexts[index].name}') SetActive({selected})");
        }
    }

    // Wire the Proceed button's OnClick to this
    public void OnProceedPressed()
    {
        if (selectedIndex < 0)
        {
            Debug.LogWarning("[CharacterSelectUI] No character selected yet.");
            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }
}