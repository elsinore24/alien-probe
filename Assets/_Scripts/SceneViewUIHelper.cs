using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AlienProbe
{
    /// <summary>
    /// Helper script to visualize UI layouts in Scene view during development
    /// </summary>
    [System.Serializable]
    public class UILayoutPreview
    {
        public bool showDialogueUI = true;
        public bool showPuzzleUI = true;
        public bool showLetterTiles = true;
        public bool showAnswerSlots = true;
        
        [Header("Preview Settings")]
        public Color dialogueAreaColor = new Color(0.2f, 0.8f, 0.2f, 0.3f);
        public Color puzzleAreaColor = new Color(0.8f, 0.2f, 0.2f, 0.3f);
        public Color tileAreaColor = new Color(0.2f, 0.2f, 0.8f, 0.3f);
    }

    public class SceneViewUIHelper : MonoBehaviour
    {
        [Header("UI Layout Preview")]
        public UILayoutPreview layoutPreview = new UILayoutPreview();
        
        [Header("UI Positioning References")]
        [SerializeField] private RectTransform dialogueAreaRef;
        [SerializeField] private RectTransform puzzleAreaRef;
        [SerializeField] private RectTransform tileAreaRef;
        
        [Header("Character Integration")]
        [SerializeField] private Transform characterTransform;

        private void OnValidate()
        {
            // Auto-find character if not assigned
            if (characterTransform == null)
            {
                // Find the Koharu GameObject by name
                var koharuObject = GameObject.Find("Koharu");
                if (koharuObject != null)
                {
                    characterTransform = koharuObject.transform;
                }
            }
        }

        /// <summary>
        /// Preview UI layout areas in Scene view
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                DrawUILayoutPreview();
            }
        }

        private void DrawUILayoutPreview()
        {
            // Set up camera-relative positioning
            Camera sceneCamera = Camera.current;
            if (sceneCamera == null) sceneCamera = Camera.main;
            if (sceneCamera == null) return;

            Vector3 charPos = characterTransform != null ? characterTransform.position : Vector3.zero;
            
            // Draw dialogue area preview
            if (layoutPreview.showDialogueUI)
            {
                DrawDialogueArea(charPos, sceneCamera);
            }
            
            // Draw puzzle area preview
            if (layoutPreview.showPuzzleUI)
            {
                DrawPuzzleArea(charPos, sceneCamera);
            }
            
            // Draw letter tiles area
            if (layoutPreview.showLetterTiles)
            {
                DrawLetterTilesArea(charPos, sceneCamera);
            }
            
            // Draw answer slots area
            if (layoutPreview.showAnswerSlots)
            {
                DrawAnswerSlotsArea(charPos, sceneCamera);
            }
        }

        private void DrawDialogueArea(Vector3 charPos, Camera cam)
        {
            Gizmos.color = layoutPreview.dialogueAreaColor;
            
            // Position dialogue area relative to character
            Vector3 dialoguePos = charPos + Vector3.down * 3f + Vector3.left * 2f;
            Vector3 dialogueSize = new Vector3(6f, 2f, 0.1f);
            
            Gizmos.DrawCube(dialoguePos, dialogueSize);
            
            // Draw label using Unity Editor handles
            #if UNITY_EDITOR
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(dialoguePos + Vector3.up * 1.2f, "Dialogue Area");
            #endif
        }

        private void DrawPuzzleArea(Vector3 charPos, Camera cam)
        {
            Gizmos.color = layoutPreview.puzzleAreaColor;
            
            // Position puzzle area in center-right
            Vector3 puzzlePos = charPos + Vector3.right * 4f + Vector3.up * 1f;
            Vector3 puzzleSize = new Vector3(4f, 4f, 0.1f);
            
            Gizmos.DrawCube(puzzlePos, puzzleSize);
            
            // Draw label
            #if UNITY_EDITOR
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(puzzlePos + Vector3.up * 2.2f, "Puzzle Area");
            #endif
        }

        private void DrawLetterTilesArea(Vector3 charPos, Camera cam)
        {
            Gizmos.color = layoutPreview.tileAreaColor;
            
            // Position letter tiles at bottom
            Vector3 tilesPos = charPos + Vector3.down * 4f;
            Vector3 tilesSize = new Vector3(8f, 1.5f, 0.1f);
            
            Gizmos.DrawCube(tilesPos, tilesSize);
            
            // Draw individual tile previews
            for (int i = 0; i < 8; i++)
            {
                Vector3 tilePos = tilesPos + Vector3.left * 3.5f + Vector3.right * (i * 1f);
                Vector3 tileSize = new Vector3(0.8f, 0.8f, 0.05f);
                Gizmos.DrawWireCube(tilePos, tileSize);
            }
            
            // Draw label
            #if UNITY_EDITOR
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(tilesPos + Vector3.up * 1f, "Letter Tiles");
            #endif
        }

        private void DrawAnswerSlotsArea(Vector3 charPos, Camera cam)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            
            // Position answer slots above tiles
            Vector3 slotsPos = charPos + Vector3.down * 2f;
            Vector3 slotsSize = new Vector3(6f, 1f, 0.1f);
            
            Gizmos.DrawCube(slotsPos, slotsSize);
            
            // Draw individual slot previews
            for (int i = 0; i < 6; i++)
            {
                Vector3 slotPos = slotsPos + Vector3.left * 2.5f + Vector3.right * (i * 1f);
                Vector3 slotSize = new Vector3(0.8f, 0.8f, 0.05f);
                Gizmos.DrawWireCube(slotPos, slotSize);
            }
            
            // Draw label
            #if UNITY_EDITOR
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(slotsPos + Vector3.up * 0.7f, "Answer Slots");
            #endif
        }

        /// <summary>
        /// Test UI creation for development purposes
        /// </summary>
        [ContextMenu("Test Create UI Layout")]
        public void TestCreateUILayout()
        {
            if (Application.isPlaying)
            {
                Debug.Log("SceneViewUIHelper: UI layout can be tested in Play Mode through PuzzleController");
                return;
            }
            
            Debug.Log("SceneViewUIHelper: UI layout preview active in Scene view. Enter Play Mode to test actual UI creation.");
        }
        
        /// <summary>
        /// Focus Scene view on character and UI areas
        /// </summary>
        [ContextMenu("Focus Scene Camera on UI Layout")]
        public void FocusOnUILayout()
        {
            if (characterTransform != null)
            {
                // Position scene view camera to show character and UI areas
                Vector3 focusPoint = characterTransform.position;
                
                #if UNITY_EDITOR
                UnityEditor.SceneView.lastActiveSceneView.LookAt(focusPoint, Quaternion.identity, 10f);
                #endif
                
                Debug.Log("SceneViewUIHelper: Focused Scene view on UI layout");
            }
        }
    }
}
