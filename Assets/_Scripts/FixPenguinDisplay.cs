using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FixPenguinDisplay : MonoBehaviour
{
    void Start()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            FixPenguin();
        }
        #endif
    }

    void FixPenguin()
    {
        // Check if this is the penguin object
        if (gameObject.name != "Zorp_Character_Penguin") return;

        // Get the SpriteRenderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        // Store the sprite
        Sprite penguinSprite = spriteRenderer.sprite;

        // Find the TV_Screen_Content_Area in the PuzzleTestCanvas
        GameObject puzzleCanvas = GameObject.Find("PuzzleTestCanvas");
        if (puzzleCanvas == null)
        {
            Debug.LogError("PuzzleTestCanvas not found!");
            return;
        }

        Transform tvUnit = puzzleCanvas.transform.Find("TV_Unit");
        if (tvUnit == null)
        {
            Debug.LogError("TV_Unit not found!");
            return;
        }

        Transform tvScreenArea = tvUnit.Find("TV_Screen_Content_Area");
        if (tvScreenArea == null)
        {
            Debug.LogError("TV_Screen_Content_Area not found!");
            return;
        }

        // Create a new UI GameObject for the penguin
        GameObject newPenguin = new GameObject("Zorp_Penguin_UI");
        newPenguin.transform.SetParent(tvScreenArea, false);

        // Add RectTransform
        RectTransform rectTransform = newPenguin.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0, -50); // Slightly below center
        rectTransform.sizeDelta = new Vector2(300, 300);
        rectTransform.localScale = Vector3.one;

        // Add Image component
        Image image = newPenguin.AddComponent<Image>();
        image.sprite = penguinSprite;
        image.preserveAspect = true;

        // Copy the Animator component
        Animator originalAnimator = GetComponent<Animator>();
        if (originalAnimator != null)
        {
            Animator newAnimator = newPenguin.AddComponent<Animator>();
            newAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;
        }

        // Place it after Rebus_Content_Display but before Screen_Effects_Overlay
        Transform rebusDisplay = tvScreenArea.Find("Rebus_Content_Display");
        if (rebusDisplay != null)
        {
            newPenguin.transform.SetSiblingIndex(rebusDisplay.GetSiblingIndex() + 1);
        }

        // Destroy the original penguin
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null && gameObject != null)
                {
                    DestroyImmediate(gameObject);
                }
            };
        }
        #endif

        Debug.Log("Penguin successfully converted to UI element and placed in TV mask!");
    }
}