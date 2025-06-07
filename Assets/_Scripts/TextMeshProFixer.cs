using UnityEngine;
using TMPro;

public class TextMeshProFixer : MonoBehaviour
{
    [Header("TextMeshPro Debug")]
    public TextMeshProUGUI[] textComponents;
    
    [Header("Font Assets")]
    public TMP_FontAsset defaultFont;
    public TMP_FontAsset liberationSansFont;
    
    [ContextMenu("Find All TextMeshPro Components")]
    public void FindAllTextMeshProComponents()
    {
        textComponents = FindObjectsOfType<TextMeshProUGUI>();
        Debug.Log($"Found {textComponents.Length} TextMeshPro components in scene");
        
        for (int i = 0; i < textComponents.Length; i++)
        {
            var tmp = textComponents[i];
            Debug.Log($"TextMeshPro #{i}: '{tmp.gameObject.name}' - Text: '{tmp.text}' - Font: {(tmp.font != null ? tmp.font.name : "NULL")}");
        }
    }
    
    [ContextMenu("Load Default Font")]
    public void LoadDefaultFont()
    {
        // Try to load the default TextMeshPro font
        defaultFont = Resources.GetBuiltinResource<TMP_FontAsset>("LiberationSans SDF");
        if (defaultFont == null)
        {
            defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        }
        
        Debug.Log($"Default font loaded: {(defaultFont != null ? defaultFont.name : "FAILED TO LOAD")}");
    }
    
    [ContextMenu("Fix All TextMeshPro Components")]
    public void FixAllTextMeshProComponents()
    {
        if (defaultFont == null)
        {
            LoadDefaultFont();
        }
        
        if (defaultFont == null)
        {
            Debug.LogError("Cannot fix TextMeshPro components - no default font available!");
            return;
        }
        
        FindAllTextMeshProComponents();
        
        foreach (var tmp in textComponents)
        {
            if (tmp.font == null || tmp.font.name.Contains("LiberationSans"))
            {
                Debug.Log($"Fixing TextMeshPro component on '{tmp.gameObject.name}'");
                tmp.font = defaultFont;
                tmp.SetAllDirty(); // Force refresh
            }
        }
        
        Debug.Log("TextMeshPro fix complete!");
    }
    
    [ContextMenu("Reset Text Content")]
    public void ResetTextContent()
    {
        FindAllTextMeshProComponents();
        
        foreach (var tmp in textComponents)
        {
            if (tmp.gameObject.name.Contains("Button_SubmitAnswer"))
            {
                tmp.text = "SUBMIT";
                Debug.Log("Reset submit button text to 'SUBMIT'");
            }
            else if (tmp.gameObject.name.Contains("SlotText"))
            {
                tmp.text = "";
                Debug.Log($"Reset slot text for '{tmp.gameObject.name}'");
            }
        }
    }
}