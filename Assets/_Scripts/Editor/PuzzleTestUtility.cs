using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PuzzleTestUtility : EditorWindow
{
    private PuzzleController puzzleController;
    private CharacterDialogueManager dialogueManager;
    private RebusPuzzleData selectedPuzzle;
    private bool showCorrectDialogue = true;
    
    [MenuItem("Alien Probe/Puzzle Test Utility")]
    public static void ShowWindow()
    {
        GetWindow<PuzzleTestUtility>("Puzzle Test Utility");
    }
    
    void OnGUI()
    {
        EditorGUILayout.LabelField("Puzzle Animation Tester", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to use this utility", MessageType.Info);
            return;
        }
        
        if (puzzleController == null)
            puzzleController = FindObjectOfType<PuzzleController>();
        if (dialogueManager == null)
            dialogueManager = FindObjectOfType<CharacterDialogueManager>();
            
        if (puzzleController == null || dialogueManager == null)
        {
            EditorGUILayout.HelpBox("PuzzleController or DialogueManager not found in scene", MessageType.Error);
            return;
        }
        
        EditorGUILayout.LabelField("Current Puzzle", EditorStyles.boldLabel);
        selectedPuzzle = (RebusPuzzleData)EditorGUILayout.ObjectField("Puzzle Data", selectedPuzzle, typeof(RebusPuzzleData), false);
        
        if (selectedPuzzle != null)
        {
            EditorGUILayout.LabelField($"Solution: {selectedPuzzle.solution}");
            EditorGUILayout.LabelField($"Letter Bank: {selectedPuzzle.letterBank}");
            
            if (GUILayout.Button("Load This Puzzle"))
            {
                puzzleController.currentPuzzle = selectedPuzzle;
                puzzleController.SetupPuzzleUI(selectedPuzzle);
            }
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test Dialogue Responses", EditorStyles.boldLabel);
        
        showCorrectDialogue = EditorGUILayout.Toggle("Test Correct Response", showCorrectDialogue);
        
        if (GUILayout.Button("Play Test Dialogue"))
        {
            if (selectedPuzzle != null)
            {
                List<DialogueLineData> dialogueToPlay = showCorrectDialogue ? 
                    selectedPuzzle.CorrectDialogue : selectedPuzzle.IncorrectDialogue;
                    
                if (dialogueToPlay != null && dialogueToPlay.Count > 0)
                {
                    dialogueManager.StartConversation(dialogueToPlay);
                    Debug.Log($"Playing {(showCorrectDialogue ? "correct" : "incorrect")} dialogue with {dialogueToPlay.Count} lines");
                }
                else
                {
                    Debug.LogWarning("No dialogue assigned for this response type");
                }
            }
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Test Zorp Attack Animation"))
        {
            if (dialogueManager.zorpAnimator != null)
            {
                dialogueManager.zorpAnimator.SetTrigger("atack");
                Debug.Log("Triggered Zorp attack animation");
            }
        }
        
        if (GUILayout.Button("Test Zorp Jump Animation"))
        {
            if (dialogueManager.zorpAnimator != null)
            {
                dialogueManager.zorpAnimator.SetTrigger("jump");
                Debug.Log("Triggered Zorp jump animation");
            }
        }
        
        if (GUILayout.Button("Test Zorp Walk Animation"))
        {
            if (dialogueManager.zorpAnimator != null)
            {
                dialogueManager.zorpAnimator.SetTrigger("walk");
                Debug.Log("Triggered Zorp walk animation");
            }
        }
        
        if (GUILayout.Button("Test Zorp Slide Animation"))
        {
            if (dialogueManager.zorpAnimator != null)
            {
                dialogueManager.zorpAnimator.SetTrigger("slide");
                Debug.Log("Triggered Zorp slide animation");
            }
        }
    }
}