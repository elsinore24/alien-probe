using UnityEngine;
using UnityEngine.UI;
using TMPro; // Added TextMeshPro support
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text; // Required for StringBuilder

public class PuzzleController : MonoBehaviour
{
    [Header("Puzzle Data")]
    [Tooltip("Assign the current PuzzleData ScriptableObject here.")]
    public RebusPuzzleData currentPuzzle;
    
    // Public getter for current puzzle
    public RebusPuzzleData GetCurrentPuzzle() => currentPuzzle;

    [Header("Dialogue System")]
    [Tooltip("Assign the CharacterDialogueManager (e.g., on Koharu).")]
    public CharacterDialogueManager characterDialogueManager;

    [Header("TV Display")]
    [Tooltip("The Image component within TV_Screen_Content_Area for displaying the rebus content.")]
    public Image tvRebusContentDisplay;
    [Tooltip("The Image component within TV_Screen_Content_Area for displaying silhouette content.")]
    public Image tvSilhouetteDisplay;

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
    [Tooltip("Button to submit the player's answer.")]
    public Button submitAnswerButton; // ADDED: For submitting the answer

    private List<GameObject> spawnedLetterTiles = new List<GameObject>();
    private List<GameObject> spawnedAnswerSlots = new List<GameObject>();
    private List<Component> spawnedAnswerSlotTexts = new List<Component>(); // Generic component list for both Text and TMP
    private Dictionary<Button, char> letterBankButtonChars = new Dictionary<Button, char>();

    private int currentAnswerIndex = 0;

    [Header("Character Controllers")]
    [Tooltip("Reference to Xylar's idle animation controller.")]
    public XylarIdleController xylarIdleController;

    [Header("Puzzle Sequence Timing")]
    [Tooltip("Duration to show standby screen before silhouette appears.")]
    public float standbyDuration = 2.0f;
    [Tooltip("Duration to show silhouette before revealing rebus content.")]
    public float silhouetteAppearDuration = 3.0f;

    void Start()
    {
        Debug.Log("PuzzleController Start() called");
        
        // Initialize references
        if (characterDialogueManager == null)
            characterDialogueManager = FindFirstObjectByType<CharacterDialogueManager>();
        
        // Auto-find XylarIdleController if not assigned
        if (xylarIdleController == null)
        {
            GameObject xylarGO = GameObject.Find("Xylar_Character");
            if (xylarGO != null)
            {
                xylarIdleController = xylarGO.GetComponent<XylarIdleController>();
                if (xylarIdleController == null)
                {
                    xylarIdleController = xylarGO.AddComponent<XylarIdleController>();
                    Debug.Log("PuzzleController: Added XylarIdleController component to Xylar_Character");
                }
            }
        }
        if (letterBankContainer == null)
            letterBankContainer = GameObject.Find("LetterBankContainer")?.transform;
        if (answerSlotsContainer == null)
            answerSlotsContainer = GameObject.Find("AnswerSlotsContainer")?.transform;
        
        // Setup button listeners
        if (clearButton != null)
        {
            clearButton.onClick.RemoveAllListeners();
            clearButton.onClick.AddListener(ClearAnswer);
            Debug.Log("Clear button listener assigned");
        }
        else
        {
            Debug.LogWarning("PuzzleController: Clear Button is not assigned in the Inspector. Clear functionality will be unavailable via UI.");
        }

        if (submitAnswerButton != null)
        {
            submitAnswerButton.onClick.RemoveAllListeners();
            submitAnswerButton.onClick.AddListener(ProcessPlayerAnswer);
            Debug.Log("Submit answer button listener assigned");
        }
        else
        {
            Debug.LogError("PuzzleController: Submit Answer Button is not assigned in the Inspector!");
        }

        // LevelManager will now control puzzle loading, so we don't setup puzzles here
        Debug.Log("PuzzleController initialized. Waiting for LevelManager to provide puzzles...");
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
        Debug.Log($"Letter tile clicked: {letter}");
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

    // NEW METHOD: To process the player's answer
    public void ProcessPlayerAnswer()
    {
        Debug.Log("ProcessPlayerAnswer() called");
        if (currentPuzzle == null)
        {
            Debug.LogError("ProcessPlayerAnswer: No current puzzle data!");
            return;
        }

        StringBuilder playerAnswerBuilder = new StringBuilder();
        foreach (Component textComponent in spawnedAnswerSlotTexts)
        {
            string slotText = "";
            
            // Get text based on component type
            if (textComponent is Text legacyText && legacyText != null)
            {
                slotText = legacyText.text;
            }
            else if (textComponent is TextMeshProUGUI tmproText && tmproText != null)
            {
                slotText = tmproText.text;
            }
            
            if (!string.IsNullOrEmpty(slotText))
            {
                playerAnswerBuilder.Append(slotText);
            }
            else // Optional: Handle partially filled answers - perhaps treat as incorrect or prevent submission
            {
                Debug.LogWarning("ProcessPlayerAnswer: An answer slot is empty. Treating as incomplete/incorrect.");
                SubmitIncorrectAnswer(); 
                return; 
            }
        }

        string playerAnswer = playerAnswerBuilder.ToString();
        Debug.Log($"Player submitted answer: {playerAnswer}");

        if (playerAnswer.Equals(currentPuzzle.solution, System.StringComparison.OrdinalIgnoreCase)) // Case-insensitive compare
        {
            Debug.Log("Answer is CORRECT!");
            SubmitCorrectAnswer();
        }
        else
        {
            Debug.Log("Answer is INCORRECT!");
            SubmitIncorrectAnswer();
        }
    }

    // NEW: Coroutine to handle the puzzle introduction sequence with silhouette
    private IEnumerator SetupPuzzleUISequence(RebusPuzzleData puzzle)
    {
        if (puzzle == null) 
        { 
            Debug.LogError("SetupPuzzleUISequence: Null puzzle data."); 
            yield break; 
        }

        currentPuzzle = puzzle;
        ClearPuzzleUI();

        // STAGE 1: Standby Screen - Hide both displays initially
        Debug.Log("STAGE 1: Standby Screen");
        if (tvRebusContentDisplay != null) tvRebusContentDisplay.gameObject.SetActive(false);
        if (tvSilhouetteDisplay != null) tvSilhouetteDisplay.gameObject.SetActive(false); // INTENTIONALLY DEACTIVATED
        
        yield return new WaitForSeconds(standbyDuration);

        // STAGE 2: Silhouette Appears
        Debug.Log("STAGE 2: Silhouette Appears");
        if (tvSilhouetteDisplay != null && currentPuzzle.humanSilhouetteForPuzzle != null)
        {
            Debug.Log("ACTIVATING Silhouette_Display with sprite: " + currentPuzzle.humanSilhouetteForPuzzle.name);
            tvSilhouetteDisplay.sprite = currentPuzzle.humanSilhouetteForPuzzle;
            tvSilhouetteDisplay.color = Color.white; 
            tvSilhouetteDisplay.gameObject.SetActive(true); // SHOULD BE ACTIVATED HERE
            if (tvRebusContentDisplay != null) tvRebusContentDisplay.gameObject.SetActive(false); 
        }
        else if (tvSilhouetteDisplay != null) 
        {
            // This block executes if currentPuzzle.humanSilhouetteForPuzzle IS NULL
            Debug.LogWarning("No silhouette sprite for puzzle: " + currentPuzzle.puzzleID + ". Silhouette display will remain inactive.");
            tvSilhouetteDisplay.gameObject.SetActive(false); // REMAINS/IS SET INACTIVE
        }
        else
        {
            Debug.LogError("tvSilhouetteDisplay is not assigned in the PuzzleController Inspector!");
        }

        yield return new WaitForSeconds(silhouetteAppearDuration);

        // STAGE 3: Show Rebus Content and Setup Puzzle UI
        Debug.Log("STAGE 3: Show Rebus Content");
        SetupPuzzleUI(currentPuzzle);
        
        // Start idle animations when puzzle is ready
        if (xylarIdleController != null)
        {
            xylarIdleController.StartIdleAnimations();
        }
    }

    public void SetupPuzzleUI(RebusPuzzleData puzzle)
    {
        if (puzzle == null) { Debug.LogError("SetupPuzzleUI: Null puzzle data."); return; }
        currentPuzzle = puzzle;

        ClearPuzzleUI();

        // Handle TV-based rebus image display
        if (tvRebusContentDisplay != null)
        {
            if (currentPuzzle.rebusImage != null)
            {
                tvRebusContentDisplay.sprite = currentPuzzle.rebusImage;
                tvRebusContentDisplay.color = Color.white;
                tvRebusContentDisplay.gameObject.SetActive(true);
                tvRebusContentDisplay.preserveAspect = true; // Maintain aspect ratio
                
                // Hide silhouette when showing rebus content
                if (tvSilhouetteDisplay != null) 
                {
                    tvSilhouetteDisplay.gameObject.SetActive(false);
                }
            }
            else
            {
                tvRebusContentDisplay.gameObject.SetActive(false); 
                Debug.LogWarning("SetupPuzzleUI: currentPuzzle.rebusImage is null for puzzle: " + (currentPuzzle != null ? currentPuzzle.puzzleID : "UNKNOWN"));
            }
        }
        else
        {
            Debug.LogError("SetupPuzzleUI: tvRebusContentDisplay is not assigned in the Inspector!");
        }

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
        Debug.Log("ClearAnswer() called");
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
        if (currentPuzzle == null || characterDialogueManager == null) return;
        Debug.Log("=== SubmitCorrectAnswer called ===");
        Debug.Log("Correct answer submitted for puzzle: " + currentPuzzle.name);
        Debug.Log($"CorrectDialogue count: {currentPuzzle.CorrectDialogue?.Count ?? 0}");
        
        // Stop idle animations during dialogue
        if (xylarIdleController != null)
        {
            xylarIdleController.StopIdleAnimations();
        }
        
        if (currentPuzzle.CorrectDialogue != null && currentPuzzle.CorrectDialogue.Count > 0)
        {
            Debug.Log("Starting correct dialogue conversation...");
            characterDialogueManager.StartConversation(currentPuzzle.CorrectDialogue);
        }
        else
        {
            Debug.LogWarning("No correct feedback dialogue assigned.");
        }
        
        if (LevelManager.Instance != null)
        {
            Debug.Log("Notifying LevelManager of puzzle completion...");
            LevelManager.Instance.OnPuzzleCompleted(true);
        }
    }

    public void SubmitIncorrectAnswer()
    {
        if (currentPuzzle == null || characterDialogueManager == null) return;
        Debug.Log("Incorrect answer submitted for puzzle: " + currentPuzzle.name);
        
        // Stop idle animations during dialogue
        if (xylarIdleController != null)
        {
            xylarIdleController.StopIdleAnimations();
        }
        
        if (currentPuzzle.IncorrectDialogue != null && currentPuzzle.IncorrectDialogue.Count > 0)
        {
            characterDialogueManager.StartConversation(currentPuzzle.IncorrectDialogue);
        }
        else
        {
            Debug.LogWarning("No incorrect feedback dialogue assigned.");
        }
        
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnPuzzleCompleted(false);
        }
        
        // Clear the answer slots so player can try again
        StartCoroutine(ResetPuzzleAfterIncorrect());
    }
    
    private IEnumerator ResetPuzzleAfterIncorrect()
    {
        // Wait for dialogue to finish (adjust timing as needed)
        yield return new WaitForSeconds(3f);
        
        // Clear the answer but keep the puzzle loaded
        ClearAnswer();
        Debug.Log("Puzzle reset. Player can try again.");
        
        // Restart idle animations after reset
        if (xylarIdleController != null)
        {
            xylarIdleController.StartIdleAnimations();
        }
    }
    
    public void LoadPuzzleFromLevelManager(RebusPuzzleData puzzle)
    {
        if (puzzle == null) 
        { 
            Debug.LogError("LoadPuzzleFromLevelManager: Received null puzzle data from LevelManager"); 
            return; 
        }
        
        Debug.Log($"PuzzleController: Received puzzle {puzzle.puzzleID} from LevelManager");
        currentPuzzle = puzzle;
        StartCoroutine(SetupPuzzleUISequence(puzzle));
    }
    
    public void ShowSilhouette(Sprite silhouette)
    {
        if (silhouette == null)
        {
            Debug.LogWarning("ShowSilhouette: Received null silhouette sprite");
            return;
        }
        
        Debug.Log("PuzzleController: Showing person silhouette");
        
        if (tvSilhouetteDisplay != null)
        {
            tvSilhouetteDisplay.sprite = silhouette;
            tvSilhouetteDisplay.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ShowSilhouette: tvSilhouetteDisplay is not assigned!");
        }
        
        if (tvRebusContentDisplay != null)
        {
            tvRebusContentDisplay.gameObject.SetActive(false);
        }
        
        ClearPuzzleUI();
    }
}
