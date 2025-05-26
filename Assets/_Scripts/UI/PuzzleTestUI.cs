using UnityEngine;
using UnityEngine.UI;

public class PuzzleTestUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Button for simulating correct answer")]
    public Button correctAnswerButton;
    
    [Tooltip("Button for simulating incorrect answer")]
    public Button incorrectAnswerButton;
    
    [Header("Puzzle System")]
    [Tooltip("Reference to the PuzzleController")]
    public PuzzleController puzzleController;
    
    void Start()
    {
        // Validate components
        if (correctAnswerButton == null)
        {
            Debug.LogError("PuzzleTestUI: Correct Answer Button is not assigned!");
        }
        else
        {
            correctAnswerButton.onClick.AddListener(OnCorrectAnswerClick);
        }
        
        if (incorrectAnswerButton == null)
        {
            Debug.LogError("PuzzleTestUI: Incorrect Answer Button is not assigned!");
        }
        else
        {
            incorrectAnswerButton.onClick.AddListener(OnIncorrectAnswerClick);
        }
        
        if (puzzleController == null)
        {
            Debug.LogError("PuzzleTestUI: PuzzleController is not assigned!");
        }
    }
    
    public void OnCorrectAnswerClick()
    {
        Debug.Log("PuzzleTestUI: Correct answer button clicked!");
        
        if (puzzleController != null)
        {
            puzzleController.SubmitCorrectAnswer();
        }
        else
        {
            Debug.LogError("Cannot submit correct answer: PuzzleController reference is missing!");
        }
    }
    
    public void OnIncorrectAnswerClick()
    {
        Debug.Log("PuzzleTestUI: Incorrect answer button clicked!");
        
        if (puzzleController != null)
        {
            puzzleController.SubmitIncorrectAnswer();
        }
        else
        {
            Debug.LogError("Cannot submit incorrect answer: PuzzleController reference is missing!");
        }
    }
    
    void OnDestroy()
    {
        // Clean up button listeners
        if (correctAnswerButton != null)
        {
            correctAnswerButton.onClick.RemoveListener(OnCorrectAnswerClick);
        }
        
        if (incorrectAnswerButton != null)
        {
            incorrectAnswerButton.onClick.RemoveListener(OnIncorrectAnswerClick);
        }
    }
}