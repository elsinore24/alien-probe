using UnityEngine;
using TMPro;

public class RuntimeTextMeshProFix : MonoBehaviour
{
    [Header("Auto-Fix Settings")]
    public bool fixOnStart = true;
    public bool fixOnCanvasChange = true;
    public bool debugOutput = true;
    
    [Header("Monitoring")]
    public float checkInterval = 2f;
    
    private TMP_FontAsset correctFont;
    
    void Start()
    {
        LoadCorrectFont();
        
        if (fixOnStart)
        {
            FixAllTextMeshProIssues();
        }
        
        if (checkInterval > 0)
        {
            InvokeRepeating(nameof(CheckForCorruption), checkInterval, checkInterval);
        }
    }
    
    void LoadCorrectFont()
    {
        correctFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (correctFont == null)
        {
            Debug.LogError("RuntimeTextMeshProFix: Could not load LiberationSans SDF font!");
        }
        else if (debugOutput)
        {
            Debug.Log($"RuntimeTextMeshProFix: Loaded font '{correctFont.name}'");
        }
    }
    
    void CheckForCorruption()
    {
        if (correctFont == null) return;
        
        TextMeshProUGUI[] textComponents = FindObjectsOfType<TextMeshProUGUI>();
        
        foreach (var textComponent in textComponents)
        {
            if (IsCorrupted(textComponent))
            {
                if (debugOutput)
                {
                    Debug.LogWarning($"RuntimeTextMeshProFix: Detected corruption in '{textComponent.gameObject.name}' - Text: '{textComponent.text}'");
                }
                
                FixTextComponent(textComponent);
            }
        }
    }
    
    bool IsCorrupted(TextMeshProUGUI textComponent)
    {
        if (textComponent.font != correctFont)
            return true;
            
        string text = textComponent.text;
        
        // Check for TTTTT corruption
        if (text.Contains("TTTT"))
            return true;
            
        // Check for suspiciously long text that might be corruption
        if (text.Length > 100 && text.Contains("m_"))
            return true;
            
        return false;
    }
    
    void FixTextComponent(TextMeshProUGUI textComponent)
    {
        if (correctFont == null) return;
        
        string objectName = textComponent.gameObject.name;
        string originalText = textComponent.text;
        
        // Fix font
        textComponent.font = correctFont;
        
        // Fix text content based on object name
        if (objectName.Contains("SlotText") || objectName.Contains("AnswerSlot"))
        {
            textComponent.text = "";
        }
        else if (objectName.Contains("Button_SubmitAnswer") || objectName.Contains("Submit"))
        {
            textComponent.text = "SUBMIT";
        }
        else if (originalText.Contains("TTTT") || originalText.Length > 100)
        {
            // Clear corrupted text but preserve reasonable content
            if (originalText.Length <= 50 && !originalText.Contains("m_"))
            {
                // Keep the text if it seems reasonable
            }
            else
            {
                textComponent.text = "";
            }
        }
        
        // Force update
        textComponent.SetAllDirty();
        textComponent.ForceMeshUpdate();
        
        if (debugOutput)
        {
            Debug.Log($"RuntimeTextMeshProFix: FIXED '{objectName}' - Old: '{originalText}' -> New: '{textComponent.text}'");
        }
    }
    
    [ContextMenu("Fix All TextMeshPro Now")]
    public void FixAllTextMeshProIssues()
    {
        if (correctFont == null)
        {
            LoadCorrectFont();
        }
        
        if (correctFont == null)
        {
            Debug.LogError("RuntimeTextMeshProFix: Cannot fix - no font available!");
            return;
        }
        
        TextMeshProUGUI[] textComponents = FindObjectsOfType<TextMeshProUGUI>();
        int fixedCount = 0;
        
        if (debugOutput)
        {
            Debug.Log($"RuntimeTextMeshProFix: Found {textComponents.Length} TextMeshPro components");
        }
        
        foreach (var textComponent in textComponents)
        {
            if (IsCorrupted(textComponent))
            {
                FixTextComponent(textComponent);
                fixedCount++;
            }
        }
        
        if (debugOutput)
        {
            Debug.Log($"RuntimeTextMeshProFix: Fixed {fixedCount} corrupted TextMeshPro components");
        }
    }
    
    // This can be called when Canvas settings change
    public void OnCanvasSettingsChanged()
    {
        if (fixOnCanvasChange)
        {
            // Delay fix to allow Canvas to finish updating
            Invoke(nameof(FixAllTextMeshProIssues), 0.1f);
        }
    }
}