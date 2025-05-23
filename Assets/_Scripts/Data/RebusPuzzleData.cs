using UnityEngine;
using System.Collections.Generic; // For List if you choose it over array

[CreateAssetMenu(fileName = "NewRebusPuzzle", menuName = "Alien Probe/Rebus Puzzle Data")]
public class RebusPuzzleData : ScriptableObject
{
    public string puzzleID; // Unique identifier for this puzzle
    public Sprite rebusImage; // The visual rebus image
    public string solution; // The correct answer string
    public List<char> letterBank; // Available letters for the player
    // Or: public char[] letterBank;

    [Header("Banter Keys")]
    public string correctBanterKey; // Key to find correct dialogue line
    public string incorrectBanterKey; // Key to find incorrect dialogue line

    [Header("Human Reaction")]
    public Sprite humanSilhouetteReaction; // Optional: Specific silhouette change for this puzzle
    public float destructionMeterChangeOnCorrect; // e.g., -0.1 (decrease)
    public float destructionMeterChangeOnIncorrect; // e.g., +0.05 (increase)
}