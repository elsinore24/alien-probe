using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelManagerEditor : EditorWindow
{
    private LevelManager levelManager;
    private GameStatusUI gameStatusUI;
    private Vector2 scrollPosition;
    private bool showDestructionSettings = true;
    private bool showAlienRelationships = true;
    private bool showLevelList = true;
    private bool showTestControls = true;
    
    [MenuItem("Alien Probe/Level Manager Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelManagerEditor>("Level Manager");
    }
    
    void OnGUI()
    {
        EditorGUILayout.LabelField("Level Manager Control Panel", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to use Level Manager controls", MessageType.Info);
            DrawDesignTimeControls();
            return;
        }
        
        FindComponents();
        
        if (levelManager == null)
        {
            EditorGUILayout.HelpBox("LevelManager not found in scene", MessageType.Error);
            return;
        }
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        DrawGameState();
        DrawDestructionMeter();
        DrawAlienRelationships();
        DrawLevelControls();
        DrawTestControls();
        
        EditorGUILayout.EndScrollView();
    }
    
    void FindComponents()
    {
        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();
        if (gameStatusUI == null)
            gameStatusUI = FindObjectOfType<GameStatusUI>();
    }
    
    void DrawDesignTimeControls()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Design Time Tools", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Create Test Level Sequence"))
        {
            CreateTestLevelSequence();
        }
        
        if (GUILayout.Button("Validate Puzzle Assets"))
        {
            ValidatePuzzleAssets();
        }
    }
    
    void DrawGameState()
    {
        EditorGUILayout.LabelField("Game State", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Current Level: {levelManager.currentLevelIndex + 1}");
        EditorGUILayout.LabelField($"Category: {levelManager.currentCategory}");
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Total Puzzles: {levelManager.allPuzzles.Count}");
        EditorGUILayout.LabelField($"Progress: {levelManager.GetProgressPercentage():F1}%");
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
    }
    
    void DrawDestructionMeter()
    {
        showDestructionSettings = EditorGUILayout.Foldout(showDestructionSettings, "Destruction Meter", true);
        if (showDestructionSettings)
        {
            EditorGUI.indentLevel++;
            
            float oldDestruction = levelManager.destructionMeter;
            float newDestruction = EditorGUILayout.Slider("Earth Threat Level", oldDestruction, 0f, 100f);
            if (newDestruction != oldDestruction)
            {
                levelManager.destructionMeter = newDestruction;
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset to 50%"))
            {
                levelManager.destructionMeter = 50f;
            }
            if (GUILayout.Button("Trigger Warning (80%)"))
            {
                levelManager.destructionMeter = 80f;
            }
            if (GUILayout.Button("Trigger Critical (95%)"))
            {
                levelManager.destructionMeter = 95f;
            }
            EditorGUILayout.EndHorizontal();
            
            Color meterColor = Color.green;
            if (levelManager.destructionMeter > 66f) meterColor = Color.red;
            else if (levelManager.destructionMeter > 33f) meterColor = Color.yellow;
            
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 10), meterColor);
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
    }
    
    void DrawAlienRelationships()
    {
        showAlienRelationships = EditorGUILayout.Foldout(showAlienRelationships, "Alien Relationships", true);
        if (showAlienRelationships)
        {
            EditorGUI.indentLevel++;
            
            float oldZorp = levelManager.zorpRespectMeter;
            float newZorp = EditorGUILayout.Slider("Zorp Respect", oldZorp, 0f, 100f);
            if (newZorp != oldZorp)
            {
                levelManager.zorpRespectMeter = newZorp;
            }
            EditorGUILayout.LabelField($"Status: {GetZorpStatus(levelManager.zorpRespectMeter)}");
            
            EditorGUILayout.Space(5);
            
            float oldXylar = levelManager.xylarCuriosityMeter;
            float newXylar = EditorGUILayout.Slider("Xylar Curiosity", oldXylar, 0f, 100f);
            if (newXylar != oldXylar)
            {
                levelManager.xylarCuriosityMeter = newXylar;
            }
            EditorGUILayout.LabelField($"Status: {GetXylarStatus(levelManager.xylarCuriosityMeter)}");
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Relationships"))
            {
                levelManager.zorpRespectMeter = 0f;
                levelManager.xylarCuriosityMeter = 25f;
            }
            if (GUILayout.Button("Max Relationships"))
            {
                levelManager.zorpRespectMeter = 100f;
                levelManager.xylarCuriosityMeter = 100f;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
    }
    
    void DrawLevelControls()
    {
        showLevelList = EditorGUILayout.Foldout(showLevelList, "Level Controls", true);
        if (showLevelList)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous Level") && levelManager.currentLevelIndex > 0)
            {
                levelManager.currentLevelIndex--;
                levelManager.LoadCurrentLevel();
            }
            if (GUILayout.Button("Next Level") && levelManager.currentLevelIndex < levelManager.allPuzzles.Count - 1)
            {
                levelManager.currentLevelIndex++;
                levelManager.LoadCurrentLevel();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Restart Game"))
            {
                levelManager.ResetGameProgress();
                levelManager.LoadCurrentLevel();
            }
            if (GUILayout.Button("Reload Current Level"))
            {
                levelManager.LoadCurrentLevel();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField("Available Puzzles:");
            for (int i = 0; i < levelManager.allPuzzles.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                bool isCurrent = i == levelManager.currentLevelIndex;
                GUI.color = isCurrent ? Color.yellow : Color.white;
                
                if (levelManager.allPuzzles[i] != null)
                {
                    EditorGUILayout.LabelField($"{i + 1}. {levelManager.allPuzzles[i].puzzleID} - {levelManager.allPuzzles[i].solution}");
                    if (GUILayout.Button("Load", GUILayout.Width(50)))
                    {
                        levelManager.currentLevelIndex = i;
                        levelManager.LoadCurrentLevel();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField($"{i + 1}. [NULL PUZZLE]");
                }
                
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
    }
    
    void DrawTestControls()
    {
        showTestControls = EditorGUILayout.Foldout(showTestControls, "Test Controls", true);
        if (showTestControls)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.LabelField("Simulate Answer Results:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Simulate Correct Answer"))
            {
                levelManager.OnPuzzleCompleted(true);
            }
            if (GUILayout.Button("Simulate Incorrect Answer"))
            {
                levelManager.OnPuzzleCompleted(false);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Quick Meter Adjustments:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reduce Destruction (-10)"))
            {
                levelManager.AdjustDestructionMeter(-10f);
            }
            if (GUILayout.Button("Increase Destruction (+10)"))
            {
                levelManager.AdjustDestructionMeter(10f);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Boost Zorp (+20)"))
            {
                levelManager.AdjustZorpRespect(20f);
            }
            if (GUILayout.Button("Boost Xylar (+20)"))
            {
                levelManager.AdjustXylarCuriosity(20f);
            }
            EditorGUILayout.EndHorizontal();
            
            if (gameStatusUI != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("UI Controls:");
                if (GUILayout.Button("Force Update UI"))
                {
                    gameStatusUI.ForceUpdateUI();
                }
            }
            
            EditorGUI.indentLevel--;
        }
    }
    
    string GetZorpStatus(float respect)
    {
        if (respect < 25f) return "Hostile";
        if (respect < 50f) return "Skeptical";
        if (respect < 75f) return "Impressed";
        return "Protective";
    }
    
    string GetXylarStatus(float curiosity)
    {
        if (curiosity < 25f) return "Detached";
        if (curiosity < 50f) return "Interested";
        if (curiosity < 75f) return "Fascinated";
        return "Invested";
    }
    
    void CreateTestLevelSequence()
    {
        Debug.Log("Creating test level sequence...");
        string[] testSolutions = { "TIME", "LOVE", "HOME", "FOOD", "WORK", "HOPE", "MUSIC", "PARTY" };
        
        for (int i = 0; i < testSolutions.Length; i++)
        {
            Debug.Log($"Test Level {i + 1}: {testSolutions[i]}");
        }
        
        Debug.Log("Use the RebusPuzzleData ScriptableObjects to create actual puzzle assets");
    }
    
    void ValidatePuzzleAssets()
    {
        Debug.Log("Validating puzzle assets...");
        
        string[] guids = AssetDatabase.FindAssets("t:RebusPuzzleData");
        Debug.Log($"Found {guids.Length} RebusPuzzleData assets");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            RebusPuzzleData puzzle = AssetDatabase.LoadAssetAtPath<RebusPuzzleData>(path);
            
            if (puzzle != null)
            {
                bool isValid = !string.IsNullOrEmpty(puzzle.solution) && 
                              !string.IsNullOrEmpty(puzzle.letterBank) &&
                              puzzle.rebusImage != null;
                              
                Debug.Log($"{puzzle.name}: {(isValid ? "VALID" : "INVALID")} - Solution: {puzzle.solution}");
            }
        }
    }
}