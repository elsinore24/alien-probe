using UnityEngine;
using UnityEngine.UI;
using TMPro; // Added TextMeshPro support
using System.Collections.Generic;
using System.Linq;

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
    [Tooltip("Button to clear the current answer in the slots.")]
    public Button clearButton;

    private List<GameObject> spawnedLetterTiles = new List<GameObject>();
    private List<GameObject> spawnedAnswerSlots = new List<GameObject>();
    private List<Component> spawnedAnswerSlotTexts = new List<Component>(); // Generic component list for both Text and TMP
    private Dictionary<Button, char> letterBankButtonChars = new Dictionary<Button, char>();

    private int currentAnswerIndex = 0;

    void Start()
    {
        if (currentPuzzle == null)
        {
            Debug.LogWarning("No puzzle asset assigned in Inspector. Attempting to create a test puzzle in code...");
            CreateTestPuzzleInCode();
        }

        if (characterDialogueManager == null)
            characterDialogueManager = FindObjectOfType<CharacterDialogueManager>();
        if (letterBankContainer == null)
            letterBankContainer = GameObject.Find("LetterBankContainer")?.transform;
        if (answerSlotsContainer == null)
            answerSlotsContainer = GameObject.Find("AnswerSlotsContainer")?.transform;
        
        if (clearButton != null)
        {
            clearButton.onClick.RemoveAllListeners();
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

    void CreateTestPuzzleInCode()
    {
        RebusPuzzleData testPuzzle = ScriptableObject.CreateInstance<RebusPuzzleData>();
        testPuzzle.puzzleID = "test001_code";
        testPuzzle.solution = "UNITY";
        testPuzzle.letterBank = "UNITYWFVD"; // String with extra letters
        testPuzzle.CorrectDialogue = new List<DialogueLineData>();
        testPuzzle.IncorrectDialogue = new List<DialogueLineData>();
        currentPuzzle = testPuzzle;
        Debug.Log("Test puzzle created in code: " + testPuzzle.solution + " with " + testPuzzle.letterBank.Length + " letters");
    }

    GameObject CreateLetterTile(char letter, Transform parent)
    {
        if (letterTilePrefab == null) { Debug.LogError("LetterTilePrefab is not assigned!"); return null; }

        GameObject tileInstance = Instantiate(letterTilePrefab, parent);
        
        // Support both Text and TextMeshPro
        Text tileText = tileInstance.GetComponentInChildren<Text>(true);
        TextMeshProUGUI tmproText = tileInstance.GetComponentInChildren<TextMeshProUGUI>(true);

        if (tileText != null) 
        { 
            tileText.text = letter.ToString(); 
        }
        else if (tmproText != null) 
        { 
            tmproText.text = letter.ToString(); 
        }
        else 
        { 
            Debug.LogWarning($"LetterTile '{letter}' created without a Text/TMP_Text component in its children."); 
        }

        tileInstance.name = "LetterTile_Inst_" + letter;
        
        Button tileButton = tileInstance.GetComponent<Button>();
        if (tileButton != null)
        {
            if (!letterBankButtonChars.ContainsKey(tileButton))
            {
                 letterBankButtonChars.Add(tileButton, letter);
            }
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

        // Support both Text and TextMeshPro
        Text slotText = slotInstance.GetComponentInChildren<Text>(true);
        TextMeshProUGUI slotTmproText = slotInstance.GetComponentInChildren<TextMeshProUGUI>(true);
        
        if (slotText != null)
        {
            spawnedAnswerSlotTexts.Add(slotText);
            slotText.text = "";
            Debug.Log("Found Legacy Text component in answer slot");
        }
        else if (slotTmproText != null) 
        { 
            spawnedAnswerSlotTexts.Add(slotTmproText); 
            slotTmproText.text = ""; 
            Debug.Log("Found TextMeshPro component in answer slot");
        }
        else
        {
            Debug.LogWarning("AnswerSlotPrefab does not have a Text/TMP_Text child to display the letter.");
        }
        return slotInstance;
    }

    public void OnLetterTileClicked(char letter, Button clickedButton)
    {
        if (currentAnswerIndex < spawnedAnswerSlotTexts.Count)
        {
            Component textComponent = spawnedAnswerSlotTexts[currentAnswerIndex];
            
            // Set text based on component type
            if (textComponent is Text legacyText)
            {
                legacyText.text = letter.ToString();
            }
            else if (textComponent is TextMeshProUGUI tmproText)
            {
                tmproText.text = letter.ToString();
            }
            
            clickedButton.interactable = false;
            currentAnswerIndex++;
            
            Debug.Log($"Letter '{letter}' placed in slot {currentAnswerIndex - 1}");
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

        ClearPuzzleUI();

        if (!string.IsNullOrEmpty(puzzle.letterBank) && letterBankContainer != null)
        {
            foreach (char letter_char in puzzle.letterBank)
            {
                GameObject tile = CreateLetterTile(letter_char, letterBankContainer);
                if (tile != null) spawnedLetterTiles.Add(tile);
            }
        }

        if (!string.IsNullOrEmpty(puzzle.solution) && answerSlotsContainer != null)
        {
            for (int i = 0; i < puzzle.solution.Length; i++)
            {
                GameObject slot = CreateAnswerSlot(answerSlotsContainer);
                if (slot != null) spawnedAnswerSlots.Add(slot);
            }
        }
    }

    void ClearPuzzleUI()
    {
        foreach (GameObject tile in spawnedLetterTiles) { if (tile != null) Destroy(tile); }
        spawnedLetterTiles.Clear();
        letterBankButtonChars.Clear();

        foreach (GameObject slot in spawnedAnswerSlots) { if (slot != null) Destroy(slot); }
        spawnedAnswerSlots.Clear();
        spawnedAnswerSlotTexts.Clear();

        currentAnswerIndex = 0;
    }

    public void ClearAnswer()
    {
        foreach (Component textComponent in spawnedAnswerSlotTexts)
        {
            if (textComponent is Text legacyText && legacyText != null)
            {
                legacyText.text = "";
            }
            else if (textComponent is TextMeshProUGUI tmproText && tmproText != null)
            {
                tmproText.text = "";
            }
        }
        
        foreach (Button button in letterBankButtonChars.Keys)
        {
            if (button != null) button.interactable = true;
        }
        
        currentAnswerIndex = 0;
        Debug.Log("Answer cleared.");
    }

    public void SubmitCorrectAnswer()
    {
        Debug.LogWarning("SubmitCorrectAnswer called - actual answer checking logic not yet implemented here.");
    }

    public void SubmitIncorrectAnswer()
    {
        Debug.LogWarning("SubmitIncorrectAnswer called - actual answer checking logic not yet implemented here.");
    }
}
