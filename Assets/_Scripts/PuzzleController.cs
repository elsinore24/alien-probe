using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Button, Text
using System.Collections.Generic;
using System.Linq; // Added for string.Join in Debug.Log

public class PuzzleController : MonoBehaviour
{
    [Header("Puzzle Data")]
    [Tooltip("Assign the current PuzzleData ScriptableObject here.")]
    public RebusPuzzleData currentPuzzle;

    [Header("Dialogue System")]
    [Tooltip("Assign the CharacterDialogueManager (e.g., on Koharu).")]
    public CharacterDialogueManager characterDialogueManager;

    [Header("Puzzle UI Elements")]
    [Tooltip("Prefab for the letter tiles in the bank.")]
    public GameObject letterTilePrefab;
    [Tooltip("Prefab for the answer slot UI elements.")]
    public GameObject answerSlotPrefab;
    [Tooltip("Parent Transform for instantiating letter tiles.")]
    public Transform letterBankContainer;
    [Tooltip("Parent Transform for instantiating answer slots.")]
    public Transform answerSlotsContainer;

    private List<GameObject> spawnedLetterTiles = new List<GameObject>();
    private List<GameObject> spawnedAnswerSlots = new List<GameObject>();


    void Start()
    {
        if (currentPuzzle == null)
        {
            Debug.Log("No puzzle asset assigned, creating test puzzle in code...");
            CreateTestPuzzleInCode(); // This will now correctly use string for letterBank
        }

        if (letterBankContainer == null)
            letterBankContainer = GameObject.Find("LetterBankContainer")?.transform;
        if (answerSlotsContainer == null)
            answerSlotsContainer = GameObject.Find("AnswerSlotsContainer")?.transform;
        if (characterDialogueManager == null)
            characterDialogueManager = FindObjectOfType<CharacterDialogueManager>();

        if (currentPuzzle != null)
        {
            SetupPuzzleUI(currentPuzzle);
        }
        else
        {
            Debug.LogError("PuzzleController: Could not create or assign puzzle data!");
        }

        if (characterDialogueManager == null)
        {
            Debug.LogWarning("PuzzleController: CharacterDialogueManager is not assigned!");
        }

        if (letterBankContainer == null || answerSlotsContainer == null)
        {
            // Check again after potential Find attempts
            if (letterBankContainer == null || answerSlotsContainer == null)
            {
                 Debug.LogError("PuzzleController: One or more UI containers are missing even after attempting to find them!");
                 // Optionally, you could still call CreateContainersDirectly() here as a last resort
                 // Debug.Log("Creating containers directly as a fallback...");
                 // CreateContainersDirectly();
            }
        }
    }

    void CreateContainersDirectly() // This method is a fallback, ideally containers are set up in the scene
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene! Cannot create UI containers directly.");
            return;
        }

        if (letterBankContainer == null)
        {
            GameObject letterBankGO = new GameObject("LetterBankContainer_Generated");
            letterBankGO.transform.SetParent(canvas.transform, false);
            letterBankContainer = letterBankGO.transform;
            
            RectTransform rect = letterBankGO.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.1f, 0.1f);
            rect.anchorMax = new Vector2(0.9f, 0.2f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup layoutGroup = letterBankGO.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.spacing = 10;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;

            ContentSizeFitter sizeFitter = letterBankGO.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Often good for layout groups
        }

        if (answerSlotsContainer == null)
        {
            GameObject answerSlotsGO = new GameObject("AnswerSlotsContainer_Generated");
            answerSlotsGO.transform.SetParent(canvas.transform, false);
            answerSlotsContainer = answerSlotsGO.transform;
            
            RectTransform rect = answerSlotsGO.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.1f, 0.4f);
            rect.anchorMax = new Vector2(0.9f, 0.6f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup layoutGroup = answerSlotsGO.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.spacing = 10;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;

            ContentSizeFitter sizeFitter = answerSlotsGO.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Often good for layout groups
        }
    }

    void CreateTestPuzzleInCode()
    {
        RebusPuzzleData testPuzzle = ScriptableObject.CreateInstance<RebusPuzzleData>();
        
        testPuzzle.puzzleID = "test001";
        testPuzzle.solution = "CAT";
        // MODIFIED: Assign a string directly
        testPuzzle.letterBank = "CATXYZ"; 
        
        testPuzzle.CorrectDialogue = new List<DialogueLineData>();
        testPuzzle.IncorrectDialogue = new List<DialogueLineData>();
        
        testPuzzle.destructionMeterChangeOnCorrect = -0.1f;
        testPuzzle.destructionMeterChangeOnIncorrect = 0.05f;
        
        currentPuzzle = testPuzzle;
        // MODIFIED: Use .Length for string
        Debug.Log("Test puzzle created in code: " + testPuzzle.solution + " with " + testPuzzle.letterBank.Length + " letters");
    }

    GameObject CreateLetterTile(char letter, Transform parent)
    {
        if (letterTilePrefab == null)
        {
            Debug.LogError("LetterTilePrefab is not assigned in the Inspector!");
            return null; 
        }
        GameObject tileInstance = Instantiate(letterTilePrefab, parent);
        
        Text tileText = tileInstance.GetComponentInChildren<Text>(true); 
        if (tileText != null)
        {
            tileText.text = letter.ToString();
        }
        else
        {
            // Attempt to find TextMeshProUGUI if standard Text is not found
            TMPro.TextMeshProUGUI tmproText = tileInstance.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            if (tmproText != null)
            {
                tmproText.text = letter.ToString();
            }
            else
            {
                Debug.LogWarning($"Instantiated LetterTile for '{letter}' does not have a UnityEngine.UI.Text or TMPro.TextMeshProUGUI component in its children.");
            }
        }
        tileInstance.name = "LetterTile_Inst_" + letter; 
        return tileInstance;
    }

    GameObject CreateAnswerSlot(Transform parent)
    {
        if (answerSlotPrefab == null)
        {
            Debug.LogError("AnswerSlotPrefab is not assigned in the Inspector!");
            return null;
        }
        GameObject slotInstance = Instantiate(answerSlotPrefab, parent);
        slotInstance.name = "AnswerSlot_Inst"; 
        return slotInstance;
    }

    public void SetupPuzzleUI(RebusPuzzleData puzzle)
    {
        if (puzzle == null)
        {
            Debug.LogError("SetupPuzzleUI: Cannot setup UI for a null puzzle.");
            return;
        }
        currentPuzzle = puzzle; 

        ClearPuzzleUI();

        // Setup Letter Bank
        // MODIFIED: Use .Length for string and check for !string.IsNullOrEmpty
        if (!string.IsNullOrEmpty(puzzle.letterBank) && letterBankContainer != null)
        {
            // Using string.Join requires System.Linq, or you can just print puzzle.letterBank directly
            Debug.Log("Creating letter tiles for: " + puzzle.letterBank); 
            foreach (char letter in puzzle.letterBank) // This loop works correctly with a string
            {
                GameObject tile = CreateLetterTile(letter, letterBankContainer);
                if (tile != null) // Only add if tile creation was successful
                {
                    spawnedLetterTiles.Add(tile);
                }
            }
            Debug.Log($"Created {spawnedLetterTiles.Count} letter tiles");
        }
        else if (string.IsNullOrEmpty(puzzle.letterBank))
        {
            Debug.LogWarning("Puzzle letterBank is null or empty. No letter tiles will be created.");
        }
        else if (letterBankContainer == null)
        {
            Debug.LogError("LetterBankContainer is not assigned. Cannot create letter tiles.");
        }


        // Setup Answer Slots based on the length of the Solution string
        if (!string.IsNullOrEmpty(puzzle.solution) && answerSlotsContainer != null)
        {
            Debug.Log("Creating " + puzzle.solution.Length + " answer slots for solution: " + puzzle.solution);
            foreach (char _ in puzzle.solution) 
            {
                GameObject slot = CreateAnswerSlot(answerSlotsContainer);
                if (slot != null) // Only add if slot creation was successful
                {
                    spawnedAnswerSlots.Add(slot);
                }
            }
            Debug.Log($"Created {spawnedAnswerSlots.Count} answer slots");
        }
        else if (string.IsNullOrEmpty(puzzle.solution))
        {
            Debug.LogWarning("Puzzle solution is null or empty. No answer slots will be created.");
        }
        else if (answerSlotsContainer == null)
        {
            Debug.LogError("AnswerSlotsContainer is not assigned. Cannot create answer slots.");
        }
    }

    void ClearPuzzleUI()
    {
        // It's safer to iterate backwards when removing from a list or destroying objects
        for (int i = spawnedLetterTiles.Count - 1; i >= 0; i--)
        {
            if (spawnedLetterTiles[i] != null)
            {
                Destroy(spawnedLetterTiles[i]); // Use Destroy for runtime objects, DestroyImmediate is more for editor scripting
            }
        }
        spawnedLetterTiles.Clear();

        for (int i = spawnedAnswerSlots.Count - 1; i >= 0; i--)
        {
            if (spawnedAnswerSlots[i] != null)
            {
                Destroy(spawnedAnswerSlots[i]);
            }
        }
        spawnedAnswerSlots.Clear();
    }

    public void SubmitCorrectAnswer()
    {
        if (currentPuzzle == null || characterDialogueManager == null)
        {
            Debug.LogError("Cannot submit correct answer: PuzzleData or CharacterDialogueManager is missing.");
            return;
        }
        Debug.Log("Correct answer submitted for puzzle: " + currentPuzzle.puzzleID); // Use puzzleID for consistency
        if (currentPuzzle.CorrectDialogue != null && currentPuzzle.CorrectDialogue.Count > 0)
        {
            characterDialogueManager.StartConversation(currentPuzzle.CorrectDialogue);
        }
        else
        {
            Debug.LogWarning("No correct feedback dialogue assigned to the current puzzle: " + currentPuzzle.puzzleID);
        }
    }

    public void SubmitIncorrectAnswer()
    {
        if (currentPuzzle == null || characterDialogueManager == null)
        {
            Debug.LogError("Cannot submit incorrect answer: PuzzleData or CharacterDialogueManager is missing.");
            return;
        }
        Debug.Log("Incorrect answer submitted for puzzle: " + currentPuzzle.puzzleID); // Use puzzleID
        if (currentPuzzle.IncorrectDialogue != null && currentPuzzle.IncorrectDialogue.Count > 0)
        {
            characterDialogueManager.StartConversation(currentPuzzle.IncorrectDialogue);
        }
        else
        {
            Debug.LogWarning("No incorrect feedback dialogue assigned to the current puzzle: " + currentPuzzle.puzzleID);
        }
    }
}