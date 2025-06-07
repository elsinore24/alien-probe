using UnityEngine;
using System.Collections.Generic; // Still needed for DialogueLineData lists

[CreateAssetMenu(fileName = "NewRebusPuzzle", menuName = "Alien Probe/Rebus Puzzle Data")]
public class RebusPuzzleData : ScriptableObject
{
    public string puzzleID; // Unique identifier for this puzzle
    public Sprite rebusImage; // The visual rebus image
    public string solution; // The correct answer string
    
    // MODIFIED LINE BELOW: Changed from List<char> to string
    [Tooltip("A single string containing all available letters for the player (e.g., \"UYNITTES\").")]
    public string letterBank; // Available letters for the player as a single string

    [Header("Dialogue Feedback")]
    [Tooltip("Dialogue sequence to play when player answers correctly.")]
    public List<DialogueLineData> CorrectDialogue; // Direct dialogue for correct answers
    
    [Tooltip("Dialogue sequence to play when player answers incorrectly.")]
    public List<DialogueLineData> IncorrectDialogue; // Direct dialogue for incorrect answers

    [Header("Banter Keys (Legacy)")]
    public string correctBanterKey; // Key to find correct dialogue line
    public string incorrectBanterKey; // Key to find incorrect dialogue line

    [Header("Human Reaction")]
    [Tooltip("The silhouette sprite to display during the puzzle introduction sequence.")]
    public Sprite humanSilhouetteForPuzzle; // Silhouette shown during introduction
    public Sprite humanSilhouetteReaction; // Optional: Specific silhouette change for this puzzle
    public float destructionMeterChangeOnCorrect; // e.g., -0.1 (decrease)
    public float destructionMeterChangeOnIncorrect; // e.g., +0.05 (increase)
}