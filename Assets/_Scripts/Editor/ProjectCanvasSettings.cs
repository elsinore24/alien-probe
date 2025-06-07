using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using System.Reflection;

public class ProjectCanvasSettings : EditorWindow
{
    [MenuItem("Tools/Diagnose Project Canvas Settings")]
    static void ShowWindow()
    {
        GetWindow<ProjectCanvasSettings>("Project Canvas Diagnostics");
    }
    
    void OnGUI()
    {
        if (GUILayout.Button("Check Project Settings"))
        {
            CheckProjectSettings();
        }
        
        if (GUILayout.Button("Check Canvas at Runtime"))
        {
            CheckRuntimeCanvas();
        }
        
        if (GUILayout.Button("Create Test Canvas Outside Hierarchy"))
        {
            CreateTestCanvas();
        }
        
        if (GUILayout.Button("Check for Canvas Affecting Assets"))
        {
            CheckCanvasAssets();
        }
    }
    
    void CheckProjectSettings()
    {
        Debug.Log("=== Project Settings ===");
        
        // Check render pipeline
        var pipeline = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
        Debug.Log($"Render Pipeline: {(pipeline ? pipeline.name : "Built-in")}");
        
        // Check player settings
        Debug.Log($"Default Screen Width: {PlayerSettings.defaultScreenWidth}");
        Debug.Log($"Default Screen Height: {PlayerSettings.defaultScreenHeight}");
        
        // Check quality settings
        Debug.Log($"Pixel Light Count: {QualitySettings.pixelLightCount}");
        Debug.Log($"Anti Aliasing: {QualitySettings.antiAliasing}");
        
        // Check if there's a Canvas settings asset
        var canvasSettings = Resources.FindObjectsOfTypeAll<ScriptableObject>()
            .Where(obj => obj.GetType().Name.Contains("Canvas"))
            .ToList();
            
        Debug.Log($"Found {canvasSettings.Count} Canvas-related ScriptableObjects");
        foreach (var setting in canvasSettings)
        {
            Debug.Log($"  - {setting.name} ({setting.GetType().Name})");
        }
    }
    
    void CheckRuntimeCanvas()
    {
        // Create a temporary canvas and immediately check its scale
        GameObject tempGO = new GameObject("TempRuntimeCanvas");
        Canvas canvas = tempGO.AddComponent<Canvas>();
        RectTransform rt = tempGO.GetComponent<RectTransform>();
        
        Debug.Log($"=== Runtime Canvas Test ===");
        Debug.Log($"Initial scale: {rt.localScale}");
        
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Debug.Log($"After ScreenSpaceOverlay: {rt.localScale}");
        
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        Debug.Log($"After ScreenSpaceCamera: {rt.localScale}");
        
        // Check if scale changes after one frame
        EditorApplication.delayCall += () =>
        {
            if (tempGO != null)
            {
                Debug.Log($"Scale after delay: {rt.localScale}");
                DestroyImmediate(tempGO);
            }
        };
    }
    
    void CreateTestCanvas()
    {
        // Create a canvas completely detached from everything
        GameObject newCanvas = new GameObject("TestCanvas_Detached");
        newCanvas.transform.position = Vector3.zero;
        newCanvas.transform.rotation = Quaternion.identity;
        newCanvas.transform.localScale = Vector3.one;
        
        Canvas canvas = newCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        RectTransform rt = newCanvas.GetComponent<RectTransform>();
        Debug.Log($"Created detached canvas with scale: {rt.localScale}");
        
        // Add a visible element
        GameObject textGO = new GameObject("TestText");
        textGO.transform.SetParent(newCanvas.transform, false);
        var text = textGO.AddComponent<UnityEngine.UI.Text>();
        text.text = "TEST CANVAS";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 50;
        text.color = Color.red;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.sizeDelta = new Vector2(400, 100);
        textRT.anchoredPosition = Vector2.zero;
        
        Debug.Log("Created test canvas with text. Check if it's visible and what scale it has.");
    }
    
    void CheckCanvasAssets()
    {
        Debug.Log("=== Checking for Canvas-affecting assets ===");
        
        // Check for any custom canvas renderers
        var renderers = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
            .Where(m => m != null && m.GetType().Name.Contains("Canvas"))
            .Select(m => m.GetType().Name)
            .Distinct()
            .ToList();
            
        Debug.Log($"Canvas-related components in project:");
        foreach (var renderer in renderers)
        {
            Debug.Log($"  - {renderer}");
        }
        
        // Check camera settings
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Debug.Log($"\nMain Camera settings:");
            Debug.Log($"  FOV: {mainCam.fieldOfView}");
            Debug.Log($"  Near Clip: {mainCam.nearClipPlane}");
            Debug.Log($"  Far Clip: {mainCam.farClipPlane}");
            Debug.Log($"  Orthographic: {mainCam.orthographic}");
            if (mainCam.orthographic)
                Debug.Log($"  Orthographic Size: {mainCam.orthographicSize}");
        }
    }
}