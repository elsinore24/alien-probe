using UnityEngine;
using TMPro;

[System.Serializable]
public class TextMeshProAutoFixer : MonoBehaviour
{
    void Start()
    {
        // Automatically fix TextMeshPro issues on start
        FixTextMeshProIssues();
    }
    
    public void FixTextMeshProIssues()
    {
        Debug.Log("=== Starting TextMeshPro Auto-Fix ===");
        
        // Find all TextMeshPro components in the scene
        TextMeshProUGUI[] allTextComponents = FindObjectsOfType<TextMeshProUGUI>();
        Debug.Log($"Found {allTextComponents.Length} TextMeshPro components");
        
        // Load the correct font asset
        TMP_FontAsset correctFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (correctFont == null)
        {
            Debug.LogError("Could not load LiberationSans SDF font!");
            return;
        }
        
        Debug.Log($"Loaded font: {correctFont.name}");
        
        // Fix each component
        int fixedCount = 0;
        foreach (var textComponent in allTextComponents)
        {
            string objectName = textComponent.gameObject.name;
            string currentText = textComponent.text;
            string currentFontName = textComponent.font != null ? textComponent.font.name : "NULL";
            
            Debug.Log($"Checking: '{objectName}' - Text: '{currentText}' - Font: '{currentFontName}'");
            
            // Fix problematic components
            bool needsFix = false;
            
            // Check if font is missing or incorrect
            if (textComponent.font == null || textComponent.font != correctFont)
            {
                needsFix = true;
                Debug.Log($"Font issue detected on '{objectName}'");
            }
            
            // Check for problematic text content
            if (currentText.Contains("TTTT") || currentText.Length > 20)
            {
                needsFix = true;
                Debug.Log($"Text content issue detected on '{objectName}': '{currentText}'");
            }
            
            if (needsFix)
            {
                // Fix the font
                textComponent.font = correctFont;
                
                // Fix the text content based on object name
                if (objectName.Contains("Button_SubmitAnswer") || objectName.Contains("Text (TMP)"))
                {
                    textComponent.text = "SUBMIT";
                }
                else if (objectName.Contains("SlotText") || objectName.Contains("AnswerSlot"))
                {
                    textComponent.text = "";
                }
                
                // Force update
                textComponent.SetAllDirty();
                textComponent.ForceMeshUpdate();
                
                fixedCount++;
                Debug.Log($"FIXED: '{objectName}' - New text: '{textComponent.text}'");
            }
        }
        
        Debug.Log($"=== TextMeshPro Auto-Fix Complete: Fixed {fixedCount} components ===");
    }
}