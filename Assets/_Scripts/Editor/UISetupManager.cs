using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

public class UISetupManager : EditorWindow
{
    [MenuItem("Tools/Complete UI Camera Setup")]
    static void ShowWindow()
    {
        GetWindow<UISetupManager>("UI Camera Setup");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Complete UI Camera Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("1. Create UI Layer (if needed)"))
        {
            CreateUILayer();
        }
        
        if (GUILayout.Button("2. Setup Cameras and Canvases"))
        {
            SetupUISystem();
        }
        
        if (GUILayout.Button("3. Verify Setup"))
        {
            VerifySetup();
        }
        
        GUILayout.Space(20);
        GUILayout.Label("Click buttons in order 1, 2, 3", EditorStyles.helpBox);
    }
    
    void CreateUILayer()
    {
        // Check if UI layer exists
        string[] layers = UnityEditorInternal.InternalEditorUtility.layers;
        bool hasUILayer = layers.Contains("UI");
        
        if (!hasUILayer)
        {
            Debug.LogError("UI Layer not found in project! Please add 'UI' to Layers in Project Settings.");
            EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
        }
        else
        {
            Debug.Log("UI Layer found!");
        }
    }
    
    void SetupUISystem()
    {
        // Step 1: Configure Main Camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("No Main Camera found!");
            return;
        }
        
        // Remove UI layer from main camera
        mainCam.cullingMask &= ~LayerMask.GetMask("UI");
        EditorUtility.SetDirty(mainCam);
        Debug.Log("Main Camera: Removed UI layer from culling mask");
        
        // Step 2: Create or find UI Camera
        GameObject uiCamGO = GameObject.Find("UI Camera");
        if (uiCamGO == null)
        {
            uiCamGO = new GameObject("UI Camera");
            Debug.Log("Created new UI Camera");
        }
        
        Camera uiCam = uiCamGO.GetComponent<Camera>();
        if (uiCam == null)
        {
            uiCam = uiCamGO.AddComponent<Camera>();
        }
        
        // Configure UI Camera
        uiCam.orthographic = true;
        uiCam.orthographicSize = 5.4f; // Standard for 1080p UI
        uiCam.clearFlags = CameraClearFlags.Depth;
        uiCam.depth = 10; // Higher than main camera
        uiCam.cullingMask = LayerMask.GetMask("UI");
        uiCam.nearClipPlane = -100;
        uiCam.farClipPlane = 100;
        
        // Position UI Camera
        uiCamGO.transform.position = new Vector3(0, 0, -10);
        uiCamGO.transform.rotation = Quaternion.identity;
        
        EditorUtility.SetDirty(uiCam);
        Debug.Log("UI Camera configured: Orthographic size = 5.4, Only renders UI layer");
        
        // Step 3: Fix all canvases
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            GameObject canvasGO = canvas.gameObject;
            
            // Set layer to UI
            canvasGO.layer = LayerMask.NameToLayer("UI");
            
            // Configure canvas
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = uiCam;
            canvas.planeDistance = 100;
            canvas.sortingLayerName = "Default";
            
            // Reset transform
            RectTransform rt = canvas.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            
            // Ensure Canvas Scaler is set up
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvasGO.AddComponent<CanvasScaler>();
            }
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            // Ensure Graphic Raycaster
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvasGO.AddComponent<GraphicRaycaster>();
            }
            
            // Set all children to UI layer
            SetLayerRecursively(canvasGO, LayerMask.NameToLayer("UI"));
            
            EditorUtility.SetDirty(canvas);
            Debug.Log($"Fixed {canvas.name}: Scale={rt.localScale}, Layer=UI, Camera=UI Camera");
        }
        
        // Mark scene dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("\nSetup complete! Save the scene to persist changes.");
    }
    
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    
    void VerifySetup()
    {
        Debug.Log("=== Verifying UI Setup ===");
        
        // Check Main Camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            bool rendersUI = (mainCam.cullingMask & LayerMask.GetMask("UI")) != 0;
            Debug.Log($"Main Camera: Orthographic Size = {mainCam.orthographicSize}, Renders UI = {rendersUI}");
            if (rendersUI)
            {
                Debug.LogWarning("Main Camera still renders UI layer! This might cause issues.");
            }
        }
        
        // Check UI Camera
        GameObject uiCamGO = GameObject.Find("UI Camera");
        if (uiCamGO != null)
        {
            Camera uiCam = uiCamGO.GetComponent<Camera>();
            if (uiCam != null)
            {
                Debug.Log($"UI Camera: Orthographic Size = {uiCam.orthographicSize}, Depth = {uiCam.depth}");
                Debug.Log($"UI Camera: Only renders UI = {uiCam.cullingMask == LayerMask.GetMask("UI")}");
            }
        }
        else
        {
            Debug.LogError("UI Camera not found!");
        }
        
        // Check Canvases
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            RectTransform rt = canvas.GetComponent<RectTransform>();
            Debug.Log($"\n{canvas.name}:");
            Debug.Log($"  Scale: {rt.localScale}");
            Debug.Log($"  Layer: {LayerMask.LayerToName(canvas.gameObject.layer)}");
            Debug.Log($"  Render Mode: {canvas.renderMode}");
            Debug.Log($"  World Camera: {(canvas.worldCamera ? canvas.worldCamera.name : "None")}");
            
            if (rt.localScale != Vector3.one)
            {
                Debug.LogWarning($"  WARNING: Scale is not (1,1,1)!");
            }
        }
    }
}