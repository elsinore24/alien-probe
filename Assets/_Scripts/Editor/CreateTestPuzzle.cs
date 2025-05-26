using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateTestPuzzle // MonoBehaviour is not needed for static editor scripts
{
    [MenuItem("Tools/Create Test Puzzle")]
    public static void CreateTestPuzzleData()
    {
        // Create a new RebusPuzzleData asset
        RebusPuzzleData testPuzzle = ScriptableObject.CreateInstance<RebusPuzzleData>();
        
        // Configure the test puzzle
        testPuzzle.puzzleID = "test001";
        testPuzzle.solution = "CAT";
        // MODIFIED LINE BELOW: Assign a string directly
        testPuzzle.letterBank = "CATXYZ"; 
        
        // Create dialogue for testing (can be empty or populated if needed)
        testPuzzle.CorrectDialogue = new List<DialogueLineData>();
        testPuzzle.IncorrectDialogue = new List<DialogueLineData>();
        
        // Define the path and name for the new asset
        string path = "Assets/_ScriptableObjects/RebusPuzzles/EditorTestPuzzle.asset"; // Changed path to be more organized

        // Ensure the directory exists
        string directory = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        // Create the asset, ensuring the path is unique to avoid overwriting
        AssetDatabase.CreateAsset(testPuzzle, AssetDatabase.GenerateUniqueAssetPath(path));
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.FocusProjectWindow(); // Optional: to highlight the new asset
        Selection.activeObject = testPuzzle; // Optional: to select the new asset

        Debug.Log("Test puzzle created at " + AssetDatabase.GetAssetPath(testPuzzle));
    }
}