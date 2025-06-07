using UnityEngine;

public class AutoPrefabFixer : MonoBehaviour
{
    void Start()
    {
        // Get the PrefabTextMeshProFixer component
        PrefabTextMeshProFixer prefabFixer = GetComponent<PrefabTextMeshProFixer>();
        
        if (prefabFixer != null)
        {
            Debug.Log("=== Starting Automatic Prefab Fix ===");
            
            // Fix the AnswerSlot prefab first
            prefabFixer.FixAnswerSlotPrefab();
            
            // Then fix all scene components
            prefabFixer.FixAllSceneTextMeshProAfterPrefabFix();
            
            Debug.Log("=== Automatic Prefab Fix Complete ===");
        }
        else
        {
            Debug.LogError("PrefabTextMeshProFixer component not found!");
        }
        
        // Destroy this component after running once
        Destroy(this);
    }
}