using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; // For Debug.Log if using string.Join

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
    // Removed: public Button submitButton; // We'll add submission logic later
    [Tooltip("Button to clear the current answer in the slots.")]
    public Button clearButton; // Assuming you have this assigned for ClearAnswer

    private List<GameObject> spawnedLetterTiles = new List<GameObject>();
    private List<GameObject> spawnedAnswerSlots = new List<GameObject>();
    private List<Text> spawnedAnswerSlotTexts = new List<Text>(); // To easily access Text components
    private Dictionary<Button, char> letterBankButtonChars = new Dictionary<Button, char>(); // To re-enable correct buttons

    private int currentAnswerIndex = 0; // ADDED: Tracks the next empty answer slot

    void Start()
    {
        if (currentPuzzle == null)
        {
            Debug.LogWarning("No puzzle asset assigned in Inspector. Attempting to create a test puzzle in code...");
            CreateTestPuzzleInCode();
        }

        // Attempt to find essential components if not assigned
        if (characterDialogueManager == null)
            characterDialogueManager = FindObjectOfType<CharacterDialogueManager>();
        if (letterBankContainer == null)
            letterBankContainer = GameObject.Find("LetterBankContainer")?.transform;
        if (answerSlotsContainer == null)
            answerSlotsContainer = GameObject.Find("AnswerSlotsContainer")?.transform;
        
        // Assign ClearButton listener if the button exists
        if (clearButton != null)
        {
            clearButton.onClick.RemoveAllListeners(); // Good practice
            clearButton.onClick.AddListener(ClearAnswer);
        }
        else
        {
            Debug.LogWarning("PuzzleController: Clear Button is not assigned in the Inspector. Clear functionality will be unavailable via UI.");
        }

        if (currentPuzzle != null)
        {
            SetupPuzzleUI(currentPuzzle);
        }
        else
        {
            Debug.LogError("PuzzleController: Critical error - No puzzle data available after Start().");
        }
    }
    
    // CreateTestPuzzleInCode and CreateContainersDirectly can remain as they were,
    // but ensure CreateTestPuzzleInCode sets a string for letterBank.

    void CreateTestPuzzleInCode() // Ensure this is consistent with RebusPuzzleData.letterBank being a string
    {
        RebusPuzzleData testPuzzle = ScriptableObject.CreateInstance<RebusPuzzleData>();
        testPuzzle.puzzleID = "test001_code";
        testPuzzle.solution = "CODE";
        testPuzzle.letterBank = "CODEABXY"; // String
        testPuzzle.CorrectDialogue = new List<DialogueLineData>();
        testPuzzle.IncorrectDialogue = new List<DialogueLineData>();
        currentPuzzle = testPuzzle;
        Debug.Log("Test puzzle created in code: " + testPuzzle.solution + " with " + testPuzzle.letterBank.Length + " letters");
    }


    GameObject CreateLetterTile(char letter, Transform parent)
    {
        if (letterTilePrefab == null) { Debug.LogError("LetterTilePrefab is not assigned!"); return null; }

        GameObject tileInstance = Instantiate(letterTilePrefab, parent);
        Text tileText = tileInstance.GetComponentInChildren<Text>(true); // For standard UI Text
        // TMPro.TextMeshProUGUI tmproText = tileInstance.GetComponentInChildren<TMPro.TextMeshProUGUI>(true); // For TextMeshPro

        if (tileText != null) { tileText.text = letter.ToString(); }
        // else if (tmproText != null) { tmproText.text = letter.ToString(); } // Uncomment if using TextMeshPro
        else { Debug.LogWarning($"LetterTile '{letter}' created without a Text/TMP_Text component in its children."); }

        tileInstance.name = "LetterTile_Inst_" + letter;
        
        Button tileButton = tileInstance.GetComponent<Button>();
        if (tileButton != null)
        {
            // Store the character with its button for re-enabling later
            if (!letterBankButtonChars.ContainsKey(tileButton)) // Avoid issues if letters repeat in bank
            {
                 letterBankButtonChars.Add(tileButton, letter);
            }
            // MODIFIED: Add listener to call OnLetterTileClicked
            tileButton.onClick.AddListener(() => OnLetterTileClicked(letter, tileButton));
        }
        else
        {
            Debug.LogError($"LetterTilePrefab '{letterTilePrefab.name}' is missing a Button component on its root.");
        }
        return tileInstance;
    }

    GameObject CreateAnswerSlot(Transform parent)
    {
        if (answerSlotPrefab == null) { Debug.LogError("AnswerSlotPrefab is not assigned!"); return null; }
        GameObject slotInstance = Instantiate(answerSlotPrefab, parent);
        slotInstance.name = "AnswerSlot_Inst_" + spawnedAnswerSlots.Count;

        // Get and store the Text component of the answer slot for easy access
        Text slotText = slotInstance.GetComponentInChildren<Text>(true); // Assuming answer slot prefab has a Text child for the letter
        // TMPro.TextMeshProUGUI slotTmproText = slotInstance.GetComponentInChildren<TMPro.TextMeshProUGUI>(true); // For TextMeshPro
        
        if (slotText != null)
        {
            spawnedAnswerSlotTexts.Add(slotText);
            slotText.text = ""; // Ensure it's initially empty
        }
        // else if (slotTmproText != null) { spawnedAnswerSlotTexts.Add(slotTmproText); slotTmproText.text = ""; } // For TextMeshPro
        else
        {
            Debug.LogWarning("AnswerSlotPrefab does not have a Text/TMP_Text child to display the letter.");
        }
        return slotInstance;
    }

    // NEW METHOD to handle letter tile clicks
    public void OnLetterTileClicked(char letter, Button clickedButton)
    {
        if (currentAnswerIndex < spawnedAnswerSlotTexts.Count)
        {
            spawnedAnswerSlotTexts[currentAnswerIndex].text = letter.ToString();
            clickedButton.interactable = false; // Disable the clicked button
            currentAnswerIndex++;

            // Optional: Check if all slots are filled to auto-submit or enable submit button
            // if (currentAnswerIndex == spawnedAnswerSlotTexts.Count) { /* Enable submit button or auto-submit */ }
        }
        else
        {
            Debug.Log("All answer slots are filled.");
        }
    }

    public void SetupPuzzleUI(RebusPuzzleData puzzle)
    {
        if (puzzle == null) { Debug.LogError("SetupPuzzleUI: Null puzzle data."); return; }
        currentPuzzle = puzzle;

        ClearPuzzleUI(); // This will also reset currentAnswerIndex

        if (!string.IsNullOrEmpty(puzzle.letterBank) && letterBankContainer != null)
        {
            foreach (char letter_char in puzzle.letterBank) // Renamed to avoid conflict with 'letter' in lambda
            {
                GameObject tile = CreateLetterTile(letter_char, letterBankContainer);
                if (tile != null) spawnedLetterTiles.Add(tile);
            }
        }
        // ... (rest of the error checks from previous version)

        if (!string.IsNullOrEmpty(puzzle.solution) && answerSlotsContainer != null)
        {
            for (int i = 0; i < puzzle.solution.Length; i++)
            {
                GameObject slot = CreateAnswerSlot(answerSlotsContainer);
                if (slot != null) spawnedAnswerSlots.Add(slot);
            }
        }
        // ... (rest of the error checks from previous version)
    }

    void ClearPuzzleUI()
    {
        foreach (GameObject tile in spawnedLetterTiles) { if (tile != null) Destroy(tile); }
        spawnedLetterTiles.Clear();
        letterBankButtonChars.Clear(); // Clear the dictionary too

        foreach (GameObject slot in spawnedAnswerSlots) { if (slot != null) Destroy(slot); }
        spawnedAnswerSlots.Clear();
        spawnedAnswerSlotTexts.Clear(); // Clear this list too

        currentAnswerIndex = 0; // ADDED: Reset index
    }

    // Public method to be called by the UI Clear Button
    public void ClearAnswer()
    {
        foreach (Text slotText in spawnedAnswerSlotTexts)
        {
            if (slotText != null) slotText.text = "";
        }
        
        // Re-enable all letter bank buttons
        foreach (Button button in letterBankButtonChars.Keys) // Or iterate spawnedLetterTiles and get Button component
        {
            if (button != null) button.interactable = true;
        }
        
        currentAnswerIndex = 0; // ADDED: Reset index
        Debug.Log("Answer cleared.");
    }

    // SubmitCorrectAnswer and SubmitIncorrectAnswer remain as placeholders for now
    // They would be called by a dedicated "Submit Puzzle" button after player fills slots.
    public void SubmitCorrectAnswer()
    {
        // ... (existing logic) ...
        Debug.LogWarning("SubmitCorrectAnswer called - actual answer checking logic not yet implemented here.");
    }

    public void SubmitIncorrectAnswer()
    {
        // ... (existing logic) ...
        Debug.LogWarning("SubmitIncorrectAnswer called - actual answer checking logic not yet implemented here.");
    }
}
