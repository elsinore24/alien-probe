using UnityEngine;
using UnityEngine.UI; // Required for basic UI Text
// using TMPro; // Uncomment if using TextMeshPro
using System.Collections.Generic; // Required for Lists
using UnityEngine.InputSystem; // Required for new Input System

public class CharacterDialogueManager : MonoBehaviour
{
    [Header("Character Components")]
    [Tooltip("The Animator for Koharu character (Live2D).")]
    public Animator koharuAnimator;
    [Tooltip("The Animator for Zorp character (Penguin).")]
    public Animator zorpAnimator;
    [Tooltip("The Animator for Xylar character (Live2D).")]
    public Animator xylarAnimator;

    [Header("UI Elements")]
    [Tooltip("The UI Text element to display the dialogue.")]
    public Text dialogueTextUI; // For TextMeshPro: public TextMeshProUGUI dialogueTextUI;
    [Tooltip("The UI Text element to display the character's name.")] // ADDED
    public Text characterNameTextUI; // ADDED. For TextMeshPro: public TextMeshProUGUI characterNameTextUI;
    
    [Header("Dialogue Settings")]
    [Tooltip("Automatically advance to next dialogue after this delay (0 = manual advance)")]
    public float autoAdvanceDelay = 2.5f;

    [Header("Dialogue Conversation")]
    [Tooltip("Assign a list of DialogueLineData ScriptableObjects here for a conversation.")]
    public List<DialogueLineData> currentConversation;

    private int currentDialogueIndex = 0;
    private bool isConversationActive = false;
    private Coroutine autoAdvanceCoroutine;

    void Start()
    {
        Debug.Log("CharacterDialogueManager Start() called!");
        
        // Auto-find character animators if not assigned
        if (koharuAnimator == null)
        {
            GameObject koharuGO = GameObject.Find("Koharu");
            if (koharuGO != null)
            {
                koharuAnimator = koharuGO.GetComponent<Animator>();
                if (koharuAnimator != null)
                {
                    Debug.Log("CharacterDialogueManager: Auto-found Koharu Animator");
                }
            }
        }
        
        if (zorpAnimator == null)
        {
            GameObject zorpGO = GameObject.Find("Zorp_Character_Penguin");
            if (zorpGO != null)
            {
                zorpAnimator = zorpGO.GetComponent<Animator>();
                if (zorpAnimator != null)
                {
                    Debug.Log("CharacterDialogueManager: Auto-found Zorp Penguin Animator");
                }
            }
        }
        
        if (xylarAnimator == null)
        {
            GameObject xylarGO = GameObject.Find("Xylar_Character");
            if (xylarGO != null)
            {
                xylarAnimator = xylarGO.GetComponent<Animator>();
                if (xylarAnimator != null)
                {
                    Debug.Log("CharacterDialogueManager: Auto-found Xylar Animator");
                }
            }
        }
        
        if (dialogueTextUI == null)
        {
            GameObject dialogueTextGO = GameObject.Find("DialogueText");
            if (dialogueTextGO != null)
            {
                dialogueTextUI = dialogueTextGO.GetComponent<Text>();
                Debug.Log("CharacterDialogueManager: Auto-found Dialogue Text UI");
            }
            else
            {
                Debug.LogWarning("CharacterDialogueManager: Dialogue Text UI is not assigned and could not be auto-found!");
            }
        }
        
        if (characterNameTextUI == null) // ADDED
        {
            GameObject nameTextGO = GameObject.Find("CharacterNameText");
            if (nameTextGO != null)
            {
                characterNameTextUI = nameTextGO.GetComponent<Text>();
                Debug.Log("CharacterDialogueManager: Auto-found Character Name Text UI");
            }
            else
            {
                Debug.LogWarning("CharacterDialogueManager: Character Name Text UI is not assigned and could not be auto-found!");
            }
        }

        // Create test conversation if none assigned
        if (currentConversation == null || currentConversation.Count == 0)
        {
            Debug.Log("CharacterDialogueManager: No conversation assigned, creating test conversation...");
            CreateTestConversation();
        }

        // Start the conversation if available
        if (currentConversation != null && currentConversation.Count > 0)
        {
            Debug.Log($"CharacterDialogueManager: Starting conversation with {currentConversation.Count} dialogue lines.");
            StartConversation(currentConversation);
        }
        else
        {
            Debug.LogWarning("CharacterDialogueManager: No conversation assigned or conversation is empty.");
            if (dialogueTextUI != null)
            {
                dialogueTextUI.text = "CharacterDialogueManager initialized. Press space to test.";
            }
            if (characterNameTextUI != null) // ADDED
            {
                characterNameTextUI.text = "System"; // Clear name text
            }
        }
    }

    void CreateTestConversation()
    {
        // Create test dialogue data in code
        currentConversation = new List<DialogueLineData>();
        
        // Test dialogue 1
        DialogueLineData testLine1 = ScriptableObject.CreateInstance<DialogueLineData>();
        testLine1.characterSpeaking = Speaker.Narrator;
        testLine1.dialogueText = "System test initiated. Live2D and dialogue integration active.";
        testLine1.live2DAnimationTrigger = "body";
        currentConversation.Add(testLine1);
        
        // Test dialogue 2  
        DialogueLineData testLine2 = ScriptableObject.CreateInstance<DialogueLineData>();
        testLine2.characterSpeaking = Speaker.Xylar;
        testLine2.dialogueText = "Greetings! The puzzle system is ready for testing.";
        testLine2.live2DAnimationTrigger = "face01";
        currentConversation.Add(testLine2);
        
        // Test dialogue 3 - Updated for penguin Zorp
        DialogueLineData testLine3 = ScriptableObject.CreateInstance<DialogueLineData>();
        testLine3.characterSpeaking = Speaker.Zorp;
        testLine3.dialogueText = "ATTACK! The penguin alien has arrived! Prepare for chaos!";
        testLine3.live2DAnimationTrigger = "attack";
        currentConversation.Add(testLine3);
        
        Debug.Log("CharacterDialogueManager: Created test conversation with " + currentConversation.Count + " lines");
    }
    
    void Update()
    {
        // Check for input to advance dialogue using new Input System
        if (isConversationActive && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) // Ensure Keyboard.current is not null
        {
            AdvanceDialogue();
        }
    }

    public void StartConversation(List<DialogueLineData> conversation)
    {
        if (conversation == null || conversation.Count == 0)
        {
            Debug.LogWarning("StartConversation: Received an empty or null conversation list.");
            isConversationActive = false;
            if (dialogueTextUI != null) dialogueTextUI.text = ""; // Clear text
            if (characterNameTextUI != null) characterNameTextUI.text = ""; // MODIFIED: Clear name text
            return;
        }

        Debug.Log($"=== StartConversation: Starting with {conversation.Count} dialogue lines ===");
        for (int i = 0; i < conversation.Count; i++)
        {
            if (conversation[i] != null)
            {
                Debug.Log($"  [{i}] Speaker: {conversation[i].characterSpeaking}, Text: '{conversation[i].dialogueText.Substring(0, Mathf.Min(50, conversation[i].dialogueText.Length))}...', Animation: '{conversation[i].live2DAnimationTrigger}'");
            }
            else
            {
                Debug.LogError($"  [{i}] NULL DialogueLineData!");
            }
        }

        currentConversation = conversation;
        currentDialogueIndex = 0;
        isConversationActive = true;
        DisplayCurrentDialogueLine();
    }

    void AdvanceDialogue()
    {
        currentDialogueIndex++;
        Debug.Log($"AdvanceDialogue: Moving to index {currentDialogueIndex} of {currentConversation.Count}");
        if (currentDialogueIndex < currentConversation.Count)
        {
            DisplayCurrentDialogueLine();
        }
        else
        {
            EndConversation();
        }
    }

    void DisplayCurrentDialogueLine()
    {
        Debug.Log($"DisplayCurrentDialogueLine called. Index: {currentDialogueIndex}, Conversation count: {currentConversation?.Count ?? 0}");
        
        if (currentConversation == null || currentDialogueIndex >= currentConversation.Count)
        {
            Debug.LogError("DisplayCurrentDialogueLine: Invalid state or index.");
            EndConversation();
            return;
        }

        DialogueLineData dialogueLine = currentConversation[currentDialogueIndex];
        Debug.Log($"DialogueLine retrieved: {dialogueLine?.name ?? "null"}");

        if (dialogueLine == null)
        {
            Debug.LogError($"DisplayCurrentDialogueLine: DialogueLineData at index {currentDialogueIndex} is null!");
            if (dialogueTextUI != null) dialogueTextUI.text = $"Error: Null dialogue data at index {currentDialogueIndex}.";
            if (characterNameTextUI != null) characterNameTextUI.text = "Error"; // MODIFIED
            AdvanceDialogue();
            return;
        }

        // Display the dialogue text
        if (dialogueTextUI != null)
        {
            dialogueTextUI.text = dialogueLine.dialogueText;
            Debug.Log($"Displaying Dialogue ({currentDialogueIndex + 1}/{currentConversation.Count}): {dialogueLine.dialogueText}");
        }
        else
        {
            Debug.LogError("DisplayCurrentDialogueLine: dialogueTextUI is null!");
        }

        // MODIFIED: Display character name
        if (characterNameTextUI != null)
        {
            characterNameTextUI.text = dialogueLine.characterSpeaking.ToString();
            characterNameTextUI.gameObject.SetActive(true); // Ensure it's visible
        }

        // --- ENHANCED SECTION FOR MULTIPLE CHARACTER ANIMATIONS ---
        // Trigger the appropriate character animation based on speaker
        Debug.Log($"Character speaking: {dialogueLine.characterSpeaking}, Animation trigger: '{dialogueLine.live2DAnimationTrigger}' (length: {dialogueLine.live2DAnimationTrigger?.Length ?? 0})");
        
        if (!string.IsNullOrEmpty(dialogueLine.live2DAnimationTrigger))
        {
            Animator targetAnimator = GetAnimatorForSpeaker(dialogueLine.characterSpeaking);
            Debug.Log($"Got animator for {dialogueLine.characterSpeaking}: {targetAnimator != null} (GameObject: {targetAnimator?.gameObject.name ?? "null"})");
            
            if (targetAnimator != null)
            {
               // Trigger the animation on the appropriate character
               targetAnimator.SetTrigger(dialogueLine.live2DAnimationTrigger);
               Debug.Log($"Played animation trigger '{dialogueLine.live2DAnimationTrigger}' on {dialogueLine.characterSpeaking}");
               Debug.Log($"Animator name: {targetAnimator.gameObject.name}, enabled: {targetAnimator.enabled}");
            }
            else
            {
                Debug.LogWarning($"No animator found for character: {dialogueLine.characterSpeaking}");
                Debug.LogWarning($"xylarAnimator is null: {xylarAnimator == null}");
            }
        }
        else
        {
            Debug.LogWarning("DisplayCurrentDialogueLine: No Live2DAnimationTrigger specified in DialogueLineData.");
        }
        // --- END ENHANCED SECTION ---
        
        // Start auto-advance if enabled
        if (autoAdvanceDelay > 0)
        {
            if (autoAdvanceCoroutine != null)
                StopCoroutine(autoAdvanceCoroutine);
            
            autoAdvanceCoroutine = StartCoroutine(AutoAdvanceDialogue());
        }
    }
    
    System.Collections.IEnumerator AutoAdvanceDialogue()
    {
        yield return new WaitForSeconds(autoAdvanceDelay);
        
        if (isConversationActive)
        {
            AdvanceDialogue();
        }
    }

    // NEW METHOD: Get the appropriate animator for each character
    Animator GetAnimatorForSpeaker(Speaker speaker)
    {
        switch (speaker)
        {
            case Speaker.Xylar:
                return xylarAnimator;
            case Speaker.Zorp:
                return zorpAnimator;
            case Speaker.Narrator:
            case Speaker.HumanInternalMonologue:
                // Use Koharu for narrator and human thoughts
                return koharuAnimator;
            default:
                Debug.LogWarning($"Unknown speaker: {speaker}");
                return koharuAnimator; // Fallback to Koharu
        }
    }

    void EndConversation()
    {
        isConversationActive = false;
        
        // Stop auto-advance if running
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }
        
        if (dialogueTextUI != null)
        {
            dialogueTextUI.text = "";
        }
        if (characterNameTextUI != null) // MODIFIED: Clear name text
        {
            characterNameTextUI.text = "";
        }
        Debug.Log("=== Conversation ended ===");
    }

    public void TriggerConversation(List<DialogueLineData> conversation)
    {
        StartConversation(conversation);
    }
}