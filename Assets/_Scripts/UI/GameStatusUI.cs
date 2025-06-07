using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStatusUI : MonoBehaviour
{
    [Header("Destruction Meter")]
    [Tooltip("Slider to show Earth's destruction level")]
    public Slider destructionMeterSlider;
    [Tooltip("Text to show destruction percentage")]
    public TextMeshProUGUI destructionMeterText;
    [Tooltip("Image that changes color based on destruction level")]
    public Image destructionMeterFill;
    
    [Header("Alien Relationships")]
    [Tooltip("Slider to show Zorp's respect level")]
    public Slider zorpRespectSlider;
    [Tooltip("Text to show Zorp's respect")]
    public TextMeshProUGUI zorpRespectText;
    [Tooltip("Slider to show Xylar's curiosity level")]
    public Slider xylarCuriositySlider;
    [Tooltip("Text to show Xylar's curiosity")]
    public TextMeshProUGUI xylarCuriosityText;
    
    [Header("Level Information")]
    [Tooltip("Text to show current level")]
    public TextMeshProUGUI levelText;
    [Tooltip("Text to show level category description")]
    public TextMeshProUGUI categoryText;
    [Tooltip("Slider to show overall progress")]
    public Slider progressSlider;
    
    [Header("Color Settings")]
    [Tooltip("Color when destruction is low (safe)")]
    public Color safeColor = Color.green;
    [Tooltip("Color when destruction is medium (warning)")]
    public Color warningColor = Color.yellow;
    [Tooltip("Color when destruction is high (danger)")]
    public Color dangerColor = Color.red;
    
    [Header("Animation Settings")]
    [Tooltip("Speed of UI animations")]
    public float animationSpeed = 2f;
    [Tooltip("Should meters animate when changing")]
    public bool animateChanges = true;
    
    private float targetDestructionValue = 50f;
    private float targetZorpValue = 0f;
    private float targetXylarValue = 25f;
    private float currentDestructionValue = 50f;
    private float currentZorpValue = 0f;
    private float currentXylarValue = 25f;
    
    void Start()
    {
        InitializeUI();
        
        if (LevelManager.Instance != null)
        {
            UpdateAllValues();
        }
    }
    
    void Update()
    {
        if (animateChanges)
        {
            AnimateMeters();
        }
        
        if (LevelManager.Instance != null)
        {
            CheckForValueChanges();
        }
    }
    
    void InitializeUI()
    {
        if (destructionMeterSlider != null)
        {
            destructionMeterSlider.minValue = 0f;
            destructionMeterSlider.maxValue = 100f;
            destructionMeterSlider.value = currentDestructionValue;
        }
        
        if (zorpRespectSlider != null)
        {
            zorpRespectSlider.minValue = 0f;
            zorpRespectSlider.maxValue = 100f;
            zorpRespectSlider.value = currentZorpValue;
        }
        
        if (xylarCuriositySlider != null)
        {
            xylarCuriositySlider.minValue = 0f;
            xylarCuriositySlider.maxValue = 100f;
            xylarCuriositySlider.value = currentXylarValue;
        }
        
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 100f;
            progressSlider.value = 0f;
        }
    }
    
    void CheckForValueChanges()
    {
        LevelManager levelManager = LevelManager.Instance;
        
        if (targetDestructionValue != levelManager.destructionMeter)
        {
            targetDestructionValue = levelManager.destructionMeter;
        }
        
        if (targetZorpValue != levelManager.zorpRespectMeter)
        {
            targetZorpValue = levelManager.zorpRespectMeter;
        }
        
        if (targetXylarValue != levelManager.xylarCuriosityMeter)
        {
            targetXylarValue = levelManager.xylarCuriosityMeter;
        }
    }
    
    void AnimateMeters()
    {
        float deltaTime = Time.deltaTime * animationSpeed;
        
        currentDestructionValue = Mathf.Lerp(currentDestructionValue, targetDestructionValue, deltaTime);
        currentZorpValue = Mathf.Lerp(currentZorpValue, targetZorpValue, deltaTime);
        currentXylarValue = Mathf.Lerp(currentXylarValue, targetXylarValue, deltaTime);
        
        UpdateDestructionMeter();
        UpdateZorpRespect();
        UpdateXylarCuriosity();
    }
    
    void UpdateAllValues()
    {
        if (LevelManager.Instance == null) return;
        
        LevelManager levelManager = LevelManager.Instance;
        
        if (animateChanges)
        {
            targetDestructionValue = levelManager.destructionMeter;
            targetZorpValue = levelManager.zorpRespectMeter;
            targetXylarValue = levelManager.xylarCuriosityMeter;
        }
        else
        {
            currentDestructionValue = levelManager.destructionMeter;
            currentZorpValue = levelManager.zorpRespectMeter;
            currentXylarValue = levelManager.xylarCuriosityMeter;
            
            UpdateDestructionMeter();
            UpdateZorpRespect();
            UpdateXylarCuriosity();
        }
        
        UpdateLevelInfo();
        UpdateProgress();
    }
    
    void UpdateDestructionMeter()
    {
        if (destructionMeterSlider != null)
        {
            destructionMeterSlider.value = currentDestructionValue;
        }
        
        if (destructionMeterText != null)
        {
            destructionMeterText.text = $"Earth Threat: {currentDestructionValue:F0}%";
        }
        
        if (destructionMeterFill != null)
        {
            destructionMeterFill.color = GetDestructionColor(currentDestructionValue);
        }
    }
    
    void UpdateZorpRespect()
    {
        if (zorpRespectSlider != null)
        {
            zorpRespectSlider.value = currentZorpValue;
        }
        
        if (zorpRespectText != null)
        {
            string respectLevel = GetZorpRespectDescription(currentZorpValue);
            zorpRespectText.text = $"Zorp: {respectLevel}";
        }
    }
    
    void UpdateXylarCuriosity()
    {
        if (xylarCuriositySlider != null)
        {
            xylarCuriositySlider.value = currentXylarValue;
        }
        
        if (xylarCuriosityText != null)
        {
            string curiosityLevel = GetXylarCuriosityDescription(currentXylarValue);
            xylarCuriosityText.text = $"Xylar: {curiosityLevel}";
        }
    }
    
    void UpdateLevelInfo()
    {
        if (LevelManager.Instance == null) return;
        
        LevelManager levelManager = LevelManager.Instance;
        
        if (levelText != null)
        {
            levelText.text = $"Level {levelManager.currentLevelIndex + 1}";
        }
        
        if (categoryText != null)
        {
            categoryText.text = levelManager.GetCategoryDescription();
        }
    }
    
    void UpdateProgress()
    {
        if (LevelManager.Instance == null || progressSlider == null) return;
        
        float progress = LevelManager.Instance.GetProgressPercentage();
        progressSlider.value = progress;
    }
    
    Color GetDestructionColor(float destructionLevel)
    {
        if (destructionLevel < 33f)
        {
            return Color.Lerp(safeColor, warningColor, destructionLevel / 33f);
        }
        else if (destructionLevel < 66f)
        {
            return Color.Lerp(warningColor, dangerColor, (destructionLevel - 33f) / 33f);
        }
        else
        {
            return dangerColor;
        }
    }
    
    string GetZorpRespectDescription(float respectLevel)
    {
        if (respectLevel < 25f)
            return "Hostile";
        else if (respectLevel < 50f)
            return "Skeptical";
        else if (respectLevel < 75f)
            return "Impressed";
        else
            return "Protective";
    }
    
    string GetXylarCuriosityDescription(float curiosityLevel)
    {
        if (curiosityLevel < 25f)
            return "Detached";
        else if (curiosityLevel < 50f)
            return "Interested";
        else if (curiosityLevel < 75f)
            return "Fascinated";
        else
            return "Invested";
    }
    
    public void ForceUpdateUI()
    {
        if (!animateChanges)
        {
            UpdateAllValues();
        }
        else
        {
            currentDestructionValue = targetDestructionValue;
            currentZorpValue = targetZorpValue;
            currentXylarValue = targetXylarValue;
            
            UpdateDestructionMeter();
            UpdateZorpRespect();
            UpdateXylarCuriosity();
            UpdateLevelInfo();
            UpdateProgress();
        }
    }
    
    public void ShowDestructionWarning()
    {
        if (currentDestructionValue > 80f)
        {
            Debug.Log("CRITICAL: Earth destruction imminent!");
        }
        else if (currentDestructionValue > 60f)
        {
            Debug.Log("WARNING: Earth in serious danger!");
        }
    }
    
    public void ShowRespectMilestone()
    {
        if (currentZorpValue >= 75f)
        {
            Debug.Log("MILESTONE: Zorp now respects humanity!");
        }
        else if (currentZorpValue >= 50f)
        {
            Debug.Log("PROGRESS: Zorp is starting to be impressed!");
        }
    }
}