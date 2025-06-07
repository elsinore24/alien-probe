using UnityEngine;

[RequireComponent(typeof(Canvas))]
[ExecuteInEditMode]
public class CanvasScaleEnforcer : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 lastScale = Vector3.one;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();
    }
    
    void Start()
    {
        EnforceScale();
    }
    
    void Update()
    {
        if (rectTransform.localScale != Vector3.one)
        {
            EnforceScale();
        }
    }
    
    void OnEnable()
    {
        EnforceScale();
    }
    
    void OnTransformParentChanged()
    {
        EnforceScale();
    }
    
    void OnRectTransformDimensionsChange()
    {
        if (rectTransform && rectTransform.localScale != Vector3.one)
        {
            EnforceScale();
        }
    }
    
    void EnforceScale()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
            
        if (rectTransform.localScale != Vector3.one)
        {
            rectTransform.localScale = Vector3.one;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(gameObject);
            }
            #endif
        }
    }
    
    void OnValidate()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        
        EnforceScale();
    }
}