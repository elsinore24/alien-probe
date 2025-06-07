using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Level Progression")]
    [Tooltip("Ordered list of all puzzles in the game")]
    public List<RebusPuzzleData> allPuzzles = new List<RebusPuzzleData>();
    [Tooltip("Current level index (0-based)")]
    public int currentLevelIndex = 0;
    
    [Header("Game State")]
    [Tooltip("Earth's destruction level (0-100%)")]
    [Range(0f, 100f)]
    public float destructionMeter = 50f;
    [Tooltip("Game over when destruction meter reaches this value")]
    public float destructionThreshold = 100f;
    [Tooltip("Victory when destruction meter reaches this value")]
    public float salvationThreshold = 0f;
    
    [Header("Alien Relationships")]
    [Tooltip("Zorp's respect level (0-100%)")]
    [Range(0f, 100f)]
    public float zorpRespectMeter = 0f;
    [Tooltip("Xylar's curiosity level (0-100%)")]
    [Range(0f, 100f)]
    public float xylarCuriosityMeter = 25f;
    
    [Header("Connected Systems")]
    [Tooltip("Reference to the puzzle controller")]
    public PuzzleController puzzleController;
    [Tooltip("Reference to the dialogue manager")]
    public CharacterDialogueManager dialogueManager;
    
    [Header("Level Categories")]
    public LevelCategory currentCategory = LevelCategory.BasicCognition;
    
    public static LevelManager Instance { get; private set; }
    
    private bool isGameOver = false;
    private bool isTransitioning = false;
    
    public enum LevelCategory
    {
        BasicCognition,     // Levels 1-5: TIME, MONEY, FOOD, HOME, WORK
        EmotionalIntelligence, // Levels 6-10: LOVE, ANGER, HOPE, FEAR, JOY
        CulturalBehaviors,  // Levels 11-15: PARTY, MUSIC, DANCE, GIFT, HOLIDAY
        LogicParadoxes,     // Levels 16-20: IRONY, SARCASM, HUMOR, TRADITION
        AdvancedConcepts    // Levels 21+: ART, PHILOSOPHY, DREAMS, CREATIVITY
    }
    
    public enum GameEnding
    {
        None,
        Destruction,    // Destruction meter reaches 100%
        Salvation,      // Complete all levels with low destruction
        Conversion,     // Perfect performance - aliens become protectors
        AcademicExchange // Unlock all secret levels
    }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeLevelManager();
    }
    
    void InitializeLevelManager()
    {
        if (puzzleController == null)
            puzzleController = FindFirstObjectByType<PuzzleController>();
        if (dialogueManager == null)
            dialogueManager = FindFirstObjectByType<CharacterDialogueManager>();
            
        LoadGameProgress();
        
        if (allPuzzles.Count > 0)
        {
            LoadCurrentLevel();
        }
        else
        {
            Debug.LogWarning("LevelManager: No puzzles assigned! Please add puzzles to the allPuzzles list in the Inspector.");
            Debug.LogWarning("Available puzzles in project: TestPuzzle_001, TestPuzzle_002");
            
            // Try to auto-find and load test puzzles
            TryLoadTestPuzzles();
        }
    }
    
    public void LoadCurrentLevel()
    {
        if (isGameOver || isTransitioning) return;
        
        if (currentLevelIndex >= 0 && currentLevelIndex < allPuzzles.Count)
        {
            RebusPuzzleData currentPuzzle = allPuzzles[currentLevelIndex];
            UpdateLevelCategory();
            
            Debug.Log($"Loading Level {currentLevelIndex + 1}: {currentPuzzle.puzzleID} (Category: {currentCategory})");
            
            if (puzzleController != null)
            {
                puzzleController.LoadPuzzleFromLevelManager(currentPuzzle);
            }
            
            SaveGameProgress();
        }
        else
        {
            Debug.LogError($"LevelManager: Invalid level index {currentLevelIndex}. Total puzzles: {allPuzzles.Count}");
        }
    }
    
    public void OnPuzzleCompleted(bool wasCorrect)
    {
        Debug.Log($"=== OnPuzzleCompleted called: wasCorrect={wasCorrect} ===");
        Debug.Log($"isGameOver={isGameOver}, isTransitioning={isTransitioning}");
        Debug.Log($"currentLevel={currentLevelIndex + 1}, currentLevelIndex={currentLevelIndex}, totalPuzzles={allPuzzles.Count}");
        
        if (isGameOver || isTransitioning) 
        {
            Debug.LogWarning("OnPuzzleCompleted: Blocked because isGameOver or isTransitioning");
            return;
        }
        
        RebusPuzzleData currentPuzzle = GetCurrentPuzzle();
        if (currentPuzzle == null) 
        {
            Debug.LogError("OnPuzzleCompleted: currentPuzzle is null!");
            return;
        }
        
        if (wasCorrect)
        {
            Debug.Log("Processing correct answer...");
            OnCorrectAnswer(currentPuzzle);
        }
        else
        {
            Debug.Log("Processing incorrect answer...");
            OnIncorrectAnswer(currentPuzzle);
        }
        
        Debug.Log("Checking game end conditions...");
        CheckGameEndConditions();
        
        Debug.Log($"After CheckGameEndConditions: isGameOver={isGameOver}");
        
        if (!isGameOver)
        {
            Debug.Log("Starting transition to next level...");
            StartCoroutine(TransitionToNextLevel());
        }
        else
        {
            Debug.LogWarning("OnPuzzleCompleted: Not transitioning because game is over");
        }
    }
    
    void OnCorrectAnswer(RebusPuzzleData puzzle)
    {
        float destructionChange = puzzle.destructionMeterChangeOnCorrect;
        float zorpChange = CalculateZorpRespectChange(true);
        float xylarChange = CalculateXylarCuriosityChange(true);
        
        AdjustDestructionMeter(destructionChange);
        AdjustZorpRespect(zorpChange);
        AdjustXylarCuriosity(xylarChange);
        
        Debug.Log($"Correct Answer! Destruction: {destructionChange:F2}, Zorp: +{zorpChange:F1}, Xylar: +{xylarChange:F1}");
    }
    
    void OnIncorrectAnswer(RebusPuzzleData puzzle)
    {
        float destructionChange = puzzle.destructionMeterChangeOnIncorrect;
        float zorpChange = CalculateZorpRespectChange(false);
        float xylarChange = CalculateXylarCuriosityChange(false);
        
        AdjustDestructionMeter(destructionChange);
        AdjustZorpRespect(zorpChange);
        AdjustXylarCuriosity(xylarChange);
        
        Debug.Log($"Incorrect Answer! Destruction: +{destructionChange:F2}, Zorp: {zorpChange:F1}, Xylar: {xylarChange:F1}");
    }
    
    float CalculateZorpRespectChange(bool correct)
    {
        if (correct)
        {
            return Mathf.Lerp(5f, 2f, zorpRespectMeter / 100f);
        }
        else
        {
            return Mathf.Lerp(-3f, -1f, zorpRespectMeter / 100f);
        }
    }
    
    float CalculateXylarCuriosityChange(bool correct)
    {
        if (correct)
        {
            return Mathf.Lerp(3f, 4f, xylarCuriosityMeter / 100f);
        }
        else
        {
            return Mathf.Lerp(1f, 2f, xylarCuriosityMeter / 100f);
        }
    }
    
    public void AdjustDestructionMeter(float change)
    {
        float oldValue = destructionMeter;
        destructionMeter = Mathf.Clamp(destructionMeter + change, 0f, 100f);
        
        if (destructionMeter != oldValue)
        {
            OnDestructionMeterChanged(oldValue, destructionMeter);
        }
    }
    
    public void AdjustZorpRespect(float change)
    {
        float oldValue = zorpRespectMeter;
        zorpRespectMeter = Mathf.Clamp(zorpRespectMeter + change, 0f, 100f);
        
        if (zorpRespectMeter != oldValue)
        {
            OnZorpRespectChanged(oldValue, zorpRespectMeter);
        }
    }
    
    public void AdjustXylarCuriosity(float change)
    {
        float oldValue = xylarCuriosityMeter;
        xylarCuriosityMeter = Mathf.Clamp(xylarCuriosityMeter + change, 0f, 100f);
        
        if (xylarCuriosityMeter != oldValue)
        {
            OnXylarCuriosityChanged(oldValue, xylarCuriosityMeter);
        }
    }
    
    void OnDestructionMeterChanged(float oldValue, float newValue)
    {
        Debug.Log($"Destruction Meter: {oldValue:F1}% → {newValue:F1}%");
    }
    
    void OnZorpRespectChanged(float oldValue, float newValue)
    {
        Debug.Log($"Zorp Respect: {oldValue:F1}% → {newValue:F1}%");
    }
    
    void OnXylarCuriosityChanged(float oldValue, float newValue)
    {
        Debug.Log($"Xylar Curiosity: {oldValue:F1}% → {newValue:F1}%");
    }
    
    void CheckGameEndConditions()
    {
        if (destructionMeter >= destructionThreshold)
        {
            TriggerGameEnd(GameEnding.Destruction);
        }
        else if (currentLevelIndex >= allPuzzles.Count)
        {
            GameEnding ending = DetermineVictoryEnding();
            TriggerGameEnd(ending);
        }
    }
    
    GameEnding DetermineVictoryEnding()
    {
        if (destructionMeter <= 10f && zorpRespectMeter >= 90f && xylarCuriosityMeter >= 90f)
        {
            return GameEnding.Conversion;
        }
        else if (destructionMeter <= 20f && currentLevelIndex >= 20)
        {
            return GameEnding.AcademicExchange;
        }
        else if (destructionMeter <= salvationThreshold)
        {
            return GameEnding.Salvation;
        }
        else
        {
            return GameEnding.Salvation;
        }
    }
    
    void TriggerGameEnd(GameEnding ending)
    {
        isGameOver = true;
        Debug.Log($"Game Over: {ending}");
        StartCoroutine(HandleGameEnding(ending));
    }
    
    IEnumerator HandleGameEnding(GameEnding ending)
    {
        yield return new WaitForSeconds(2f);
        
        switch (ending)
        {
            case GameEnding.Destruction:
                Debug.Log("Earth has been destroyed! Game Over.");
                break;
            case GameEnding.Salvation:
                Debug.Log("Earth has been saved! The aliens leave peacefully.");
                break;
            case GameEnding.Conversion:
                Debug.Log("The aliens have become Earth's protectors!");
                break;
            case GameEnding.AcademicExchange:
                Debug.Log("Humanity and aliens establish an academic partnership!");
                break;
        }
    }
    
    IEnumerator TransitionToNextLevel()
    {
        Debug.Log("=== TransitionToNextLevel started ===");
        
        if (isTransitioning) 
        {
            Debug.LogWarning("TransitionToNextLevel: Already transitioning, aborting");
            yield break;
        }
        
        isTransitioning = true;
        Debug.Log("TransitionToNextLevel: Waiting 3 seconds...");
        yield return new WaitForSeconds(3f);
        
        currentLevelIndex++;
        Debug.Log($"TransitionToNextLevel: Incremented to level {currentLevelIndex + 1} (internal index: {currentLevelIndex})");
        SaveGameProgress();
        
        isTransitioning = false;  // Reset BEFORE calling LoadCurrentLevel
        
        if (currentLevelIndex < allPuzzles.Count)
        {
            Debug.Log($"TransitionToNextLevel: Loading level {currentLevelIndex + 1} of {allPuzzles.Count} total puzzles");
            LoadCurrentLevel();
        }
        else
        {
            Debug.LogWarning($"TransitionToNextLevel: No more levels! currentLevelIndex={currentLevelIndex}, allPuzzles.Count={allPuzzles.Count}");
        }
        Debug.Log("=== TransitionToNextLevel completed ===");
    }
    
    void UpdateLevelCategory()
    {
        if (currentLevelIndex < 5)
            currentCategory = LevelCategory.BasicCognition;
        else if (currentLevelIndex < 10)
            currentCategory = LevelCategory.EmotionalIntelligence;
        else if (currentLevelIndex < 15)
            currentCategory = LevelCategory.CulturalBehaviors;
        else if (currentLevelIndex < 20)
            currentCategory = LevelCategory.LogicParadoxes;
        else
            currentCategory = LevelCategory.AdvancedConcepts;
    }
    
    public RebusPuzzleData GetCurrentPuzzle()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < allPuzzles.Count)
        {
            return allPuzzles[currentLevelIndex];
        }
        return null;
    }
    
    public string GetCategoryDescription()
    {
        switch (currentCategory)
        {
            case LevelCategory.BasicCognition:
                return "Testing basic human cognitive functions...";
            case LevelCategory.EmotionalIntelligence:
                return "Analyzing human emotional responses...";
            case LevelCategory.CulturalBehaviors:
                return "Studying human cultural patterns...";
            case LevelCategory.LogicParadoxes:
                return "Examining human logic inconsistencies...";
            case LevelCategory.AdvancedConcepts:
                return "Evaluating advanced human consciousness...";
            default:
                return "Conducting alien research protocols...";
        }
    }
    
    public float GetProgressPercentage()
    {
        if (allPuzzles.Count == 0) return 0f;
        return (float)currentLevelIndex / allPuzzles.Count * 100f;
    }
    
    void SaveGameProgress()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex);
        PlayerPrefs.SetFloat("DestructionMeter", destructionMeter);
        PlayerPrefs.SetFloat("ZorpRespect", zorpRespectMeter);
        PlayerPrefs.SetFloat("XylarCuriosity", xylarCuriosityMeter);
        PlayerPrefs.Save();
    }
    
    void LoadGameProgress()
    {
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 0);
        destructionMeter = PlayerPrefs.GetFloat("DestructionMeter", 50f);
        zorpRespectMeter = PlayerPrefs.GetFloat("ZorpRespect", 0f);
        xylarCuriosityMeter = PlayerPrefs.GetFloat("XylarCuriosity", 25f);
    }
    
    public void ResetGameProgress()
    {
        currentLevelIndex = 0;
        destructionMeter = 50f;
        zorpRespectMeter = 0f;
        xylarCuriosityMeter = 25f;
        isGameOver = false;
        SaveGameProgress();
        Debug.Log("Game progress reset!");
    }
    
    void TryLoadTestPuzzles()
    {
        // Try to find test puzzles in Resources or by name
        RebusPuzzleData testPuzzle1 = Resources.Load<RebusPuzzleData>("TestPuzzle_001");
        RebusPuzzleData testPuzzle2 = Resources.Load<RebusPuzzleData>("TestPuzzle_002");
        
        if (testPuzzle1 != null)
        {
            allPuzzles.Add(testPuzzle1);
            Debug.Log("Auto-loaded TestPuzzle_001");
        }
        
        if (testPuzzle2 != null)
        {
            allPuzzles.Add(testPuzzle2);
            Debug.Log("Auto-loaded TestPuzzle_002");
        }
        
        if (allPuzzles.Count > 0)
        {
            Debug.Log($"Auto-loaded {allPuzzles.Count} test puzzles. Starting first level...");
            LoadCurrentLevel();
        }
        else
        {
            Debug.LogError("LevelManager: Could not auto-load test puzzles. Please manually assign puzzles in the Inspector.");
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameProgress();
        }
    }
}