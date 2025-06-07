using UnityEngine;
using TMPro;
using UnityEditor;

public class PrefabTextMeshProFixer : MonoBehaviour
{
    [Header("Prefab Fixing")]
    [SerializeField] private string prefabPath = "Assets/_Prefabs/AnswerSlot.prefab";
    
    [ContextMenu("Fix AnswerSlot Prefab")]
    public void FixAnswerSlotPrefab()
    {
        Debug.Log("=== Starting Prefab TextMeshPro Fix ===");
        
#if UNITY_EDITOR
        // Load the prefab
        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefabAsset == null)
        {
            Debug.LogError($"Could not load prefab at path: {prefabPath}");
            return;
        }
        
        Debug.Log($"Loaded prefab: {prefabAsset.name}");
        
        // Get all TextMeshPro components in the prefab
        TextMeshProUGUI[] textComponents = prefabAsset.GetComponentsInChildren<TextMeshProUGUI>(true);
        Debug.Log($"Found {textComponents.Length} TextMeshPro components in prefab");
        
        // Load the correct font
        TMP_FontAsset correctFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (correctFont == null)
        {
            Debug.LogError("Could not load LiberationSans SDF font!");
            return;
        }
        
        bool prefabModified = false;
        
        // Check and fix each component
        foreach (var textComponent in textComponents)
        {
            string objectName = textComponent.gameObject.name;
            string currentText = textComponent.text;
            string currentFontName = textComponent.font != null ? textComponent.font.name : "NULL";
            
            Debug.Log($"Checking prefab component: '{objectName}' - Text: '{currentText}' - Font: '{currentFontName}'");
            
            bool needsFix = false;
            
            // Check for problematic text content or wrong font
            if (textComponent.font != correctFont)
            {
                needsFix = true;
                Debug.Log($"Font issue detected in prefab component '{objectName}'");
            }
            
            if (currentText.Contains("TTTT") || currentText.Length > 20)
            {
                needsFix = true;
                Debug.Log($"Text content issue detected in prefab component '{objectName}': '{currentText}'");
            }
            
            if (needsFix)
            {
                // Fix the font
                textComponent.font = correctFont;
                
                // Fix text content for AnswerSlot
                if (objectName.Contains("SlotText") || objectName.Contains("Text"))
                {
                    textComponent.text = "";
                }
                
                // Force update
                textComponent.SetAllDirty();
                prefabModified = true;
                
                Debug.Log($"FIXED prefab component: '{objectName}' - New text: '{textComponent.text}' - Font: {correctFont.name}");
            }
        }
        
        if (prefabModified)
        {
            // Save the prefab changes
            EditorUtility.SetDirty(prefabAsset);
            AssetDatabase.SaveAssets();
            Debug.Log("Prefab changes saved!");
        }
        else
        {
            Debug.Log("No changes needed to prefab.");
        }
        
#else
        Debug.LogError("This function only works in the Unity Editor!");
#endif
        
        Debug.Log("=== Prefab TextMeshPro Fix Complete ===");
    }
    
    [ContextMenu("Fix All Scene TextMeshPro After Prefab Fix")]
    public void FixAllSceneTextMeshProAfterPrefabFix()
    {
        Debug.Log("=== Fixing all scene TextMeshPro components ===");
        
        // Load the correct font
        TMP_FontAsset correctFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (correctFont == null)
        {
            Debug.LogError("Could not load LiberationSans SDF font!");
            return;
        }
        
        // Find all TextMeshPro components in the scene (including inactive ones)
        TextMeshProUGUI[] allTextComponents = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        Debug.Log($"Found {allTextComponents.Length} TextMeshPro components in scene");
        
        int fixedCount = 0;
        foreach (var textComponent in allTextComponents)
        {
            // Skip prefab assets, only fix scene objects
            if (textComponent.gameObject.scene.name == null) continue;
            
            string objectName = textComponent.gameObject.name;
            string currentText = textComponent.text;
            
            Debug.Log($"Checking scene component: '{objectName}' - Text: '{currentText}'");
            
            bool needsFix = false;
            
            if (textComponent.font != correctFont)
            {
                needsFix = true;
            }
            
            if (currentText.Contains("TTTT") || currentText.Length > 20)
            {
                needsFix = true;
            }
            
            if (needsFix)
            {
                textComponent.font = correctFont;
                
                if (objectName.Contains("SlotText") || objectName.Contains("AnswerSlot"))
                {
                    textComponent.text = "";
                }
                else if (objectName.Contains("Button_SubmitAnswer") || objectName.Contains("Submit"))
                {
                    textComponent.text = "SUBMIT";
                }
                
                textComponent.SetAllDirty();
                fixedCount++;
                
                Debug.Log($"FIXED scene component: '{objectName}' - New text: '{textComponent.text}'");
            }
        }
        
        Debug.Log($"=== Scene TextMeshPro Fix Complete: Fixed {fixedCount} components ===");
    }
}