using UnityEngine;
using UnityEngine.UI; // Required for basic UI Text
// using TMPro; // Uncomment if using TextMeshPro
using System.Collections.Generic; // Required for Lists
using UnityEngine.InputSystem; // Required for new Input System

public class CharacterDialogueManager : MonoBehaviour
{
    [Header("Character Components")]
    [Tooltip("The Animator for the character (e.g., Koharu).")]
    public Animator characterAnimator;

    [Header("UI Elements")]
    [Tooltip("The UI Text element to display the dialogue.")]
    public Text dialogueTextUI; // For TextMeshPro: public TextMeshProUGUI dialogueTextUI;
    [Tooltip("The UI Text element to display the character's name.")] // ADDED
    public Text characterNameTextUI; // ADDED. For TextMeshPro: public TextMeshProUGUI characterNameTextUI;

    [Header("Dialogue Conversation")]
    [Tooltip("Assign a list of DialogueLineData ScriptableObjects here for a conversation.")]
    public List<DialogueLineData> currentConversation;

    private int currentDialogueIndex = 0;
    private bool isConversationActive = false;

    void Start()
    {
        Debug.Log("CharacterDialogueManager Start() called!");
        
        // Auto-find components if not assigned
        if (characterAnimator == null)
        {
            characterAnimator = FindObjectOfType<Animator>();
            if (characterAnimator != null)
            {
                Debug.Log("CharacterDialogueManager: Auto-found Character Animator: " + characterAnimator.name);
            }
            else
            {
                Debug.LogWarning("CharacterDialogueManager: Character Animator is not assigned and could not be auto-found!");
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
        
        // Test dialogue 3
        DialogueLineData testLine3 = ScriptableObject.CreateInstance<DialogueLineData>();
        testLine3.characterSpeaking = Speaker.Zorp;
        testLine3.dialogueText = "All systems are functioning correctly!";
        testLine3.live2DAnimationTrigger = "face02";
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

        currentConversation = conversation;
        currentDialogueIndex = 0;
        isConversationActive = true;
        DisplayCurrentDialogueLine();
    }

    void AdvanceDialogue()
    {
        currentDialogueIndex++;
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
        if (currentConversation == null || currentDialogueIndex >= currentConversation.Count)
        {
            Debug.LogError("DisplayCurrentDialogueLine: Invalid state or index.");
            EndConversation();
            return;
        }

        DialogueLineData dialogueLine = currentConversation[currentDialogueIndex];

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

        // --- MODIFIED SECTION FOR ANIMATION ---
        // Trigger the Live2D animation
        if (!string.IsNullOrEmpty(dialogueLine.live2DAnimationTrigger))
        {
            if (characterAnimator != null) // Ensure animator is still valid
            {
               // RE-ENABLED: Actually trigger the animation
               characterAnimator.SetTrigger(dialogueLine.live2DAnimationTrigger);
               Debug.Log($"Played animation trigger: {dialogueLine.live2DAnimationTrigger}");
            }
            else
            {
                Debug.LogError("CharacterAnimator is null, cannot play animation trigger.");
            }
        }
        else
        {
            Debug.LogWarning("DisplayCurrentDialogueLine: No Live2DAnimationTrigger specified in DialogueLineData.");
        }
        // --- END MODIFIED SECTION ---
    }

    void EndConversation()
    {
        isConversationActive = false;
        if (dialogueTextUI != null)
        {
            dialogueTextUI.text = "End of conversation. Press space to restart.";
        }
        if (characterNameTextUI != null) // MODIFIED: Clear name text
        {
            characterNameTextUI.text = "";
        }
        Debug.Log("Conversation ended.");
    }

    public void TriggerConversation(List<DialogueLineData> conversation)
    {
        StartConversation(conversation);
    }
}
