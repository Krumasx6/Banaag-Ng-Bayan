using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [Serializable]
    public class CharacterEntry
    {
        public string characterName;
        public GameObject prefab;
    }

    [Header("Character Selection")]
    [SerializeField] private string selectedCharacter = "Datu Sulo"; // set by the character select screen buttons

    [Header("Character Roster")]
    [SerializeField] private CharacterEntry[] characters; // fill this in the Inspector: name + prefab for each of the four

    [Header("Spawning")]
    [SerializeField] private string gameplaySceneName = "TrainingGround"; // auto-spawns when this scene loads
    [SerializeField] private string spawnPointTag = "SpawnPoint"; // tag the spawn point GameObject in TrainingGround with this

    private GameObject spawnedCharacter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // survives the switch from Character Select -> TrainingGround
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameplaySceneName)
        {
            SpawnSelectedCharacter();
        }
    }

    public string SelectedCharacter => selectedCharacter;

    // Wire this to your 4 character buttons in the Character Select scene
    public void SetCharacter(string characterName)
    {
        selectedCharacter = characterName;
    }

    // ─── Spawning ────────────────────────────────────────────────────────────

    private void SpawnSelectedCharacter()
    {
        GameObject prefab = GetPrefabFor(selectedCharacter);

        if (prefab == null)
        {
            Debug.LogError($"CharacterManager: no prefab found for '{selectedCharacter}'. " +
                            "Check the Character Roster list in the Inspector.", this);
            return;
        }

        GameObject spawnPointObj = GameObject.FindGameObjectWithTag(spawnPointTag);
        Vector3 position = spawnPointObj != null ? spawnPointObj.transform.position : Vector3.zero;

        if (spawnedCharacter != null)
            Destroy(spawnedCharacter);

        spawnedCharacter = Instantiate(prefab, position, Quaternion.identity);
    }

    private GameObject GetPrefabFor(string characterName)
    {
        if (characters == null) return null;

        foreach (CharacterEntry entry in characters)
        {
            if (entry.characterName == characterName)
                return entry.prefab;
        }

        return null;
    }
}