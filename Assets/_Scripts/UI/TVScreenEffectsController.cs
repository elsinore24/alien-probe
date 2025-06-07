using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class TVScreenEffectsController : MonoBehaviour
{
    [Header("Shader Reference")]
    [Tooltip("The TV effects material using the TVScreenEffects shader")]
    private Material tvEffectsMaterial;
    
    [Header("Effect Presets")]
    [Tooltip("Settings for standby/idle TV state")]
    public TVEffectPreset standbyPreset;
    
    [Tooltip("Settings for glitch transition effect")]
    public TVEffectPreset glitchPreset;
    
    [Tooltip("Settings for normal viewing")]
    public TVEffectPreset normalPreset;
    
    [Header("Transition Settings")]
    [Tooltip("Duration for transitioning between effect presets")]
    public float transitionDuration = 0.5f;
    
    private Image effectImage;
    private Coroutine activeTransition;
    
    [System.Serializable]
    public class TVEffectPreset
    {
        [Header("Static Noise")]
        [Range(0, 1)] public float noiseIntensity = 0.1f;
        [Range(1, 100)] public float noiseScale = 50f;
        [Range(0, 10)] public float noiseSpeed = 5f;
        
        [Header("Scanlines")]
        [Range(0, 1)] public float scanlineIntensity = 0.3f;
        [Range(10, 500)] public float scanlineCount = 100f;
        [Range(0, 5)] public float scanlineSpeed = 0.5f;
        
        [Header("Glitch")]
        [Range(0, 1)] public float glitchIntensity = 0f;
        [Range(0, 20)] public float glitchSpeed = 10f;
        [Range(0, 0.1f)] public float chromaticAberration = 0.01f;
        
        [Header("Glow")]
        public Color glowColor = new Color(0.2f, 0.8f, 0.4f, 1f);
        [Range(0, 2)] public float glowIntensity = 0.5f;
        [Range(0, 2)] public float vignetteIntensity = 0.8f;
        
        [Header("Overall")]
        [Range(0, 1)] public float effectBlend = 1f;
    }
    
    void Awake()
    {
        effectImage = GetComponent<Image>();
        
        Shader tvShader = Shader.Find("AlienProbe/TVScreenEffects");
        if (tvShader == null)
        {
            Debug.LogError("TVScreenEffectsController: Could not find AlienProbe/TVScreenEffects shader!");
            return;
        }
        
        tvEffectsMaterial = new Material(tvShader);
        effectImage.material = tvEffectsMaterial;
        
        InitializePresets();
        ApplyPreset(standbyPreset);
    }
    
    void InitializePresets()
    {
        if (standbyPreset == null)
        {
            standbyPreset = new TVEffectPreset
            {
                noiseIntensity = 0.3f,
                scanlineIntensity = 0.5f,
                glitchIntensity = 0f,
                glowIntensity = 0.3f,
                effectBlend = 0.8f
            };
        }
        
        if (glitchPreset == null)
        {
            glitchPreset = new TVEffectPreset
            {
                noiseIntensity = 0.8f,
                scanlineIntensity = 0.2f,
                glitchIntensity = 0.9f,
                chromaticAberration = 0.05f,
                glowIntensity = 0.7f,
                effectBlend = 1f
            };
        }
        
        if (normalPreset == null)
        {
            normalPreset = new TVEffectPreset
            {
                noiseIntensity = 0.05f,
                scanlineIntensity = 0.2f,
                glitchIntensity = 0f,
                glowIntensity = 0.4f,
                effectBlend = 0.6f
            };
        }
    }
    
    public void SetToStandby()
    {
        TransitionToPreset(standbyPreset);
    }
    
    public void SetToNormal()
    {
        TransitionToPreset(normalPreset);
    }
    
    public void TriggerGlitch(float duration = -1)
    {
        if (duration < 0) duration = transitionDuration;
        
        if (activeTransition != null) StopCoroutine(activeTransition);
        activeTransition = StartCoroutine(GlitchSequence(duration));
    }
    
    IEnumerator GlitchSequence(float duration)
    {
        TVEffectPreset startPreset = GetCurrentPreset();
        
        float halfDuration = duration * 0.5f;
        yield return TransitionToPresetCoroutine(glitchPreset, halfDuration);
        
        yield return TransitionToPresetCoroutine(normalPreset, halfDuration);
        
        activeTransition = null;
    }
    
    public void TransitionToPreset(TVEffectPreset preset)
    {
        if (activeTransition != null) StopCoroutine(activeTransition);
        activeTransition = StartCoroutine(TransitionToPresetCoroutine(preset, transitionDuration));
    }
    
    IEnumerator TransitionToPresetCoroutine(TVEffectPreset targetPreset, float duration)
    {
        TVEffectPreset startPreset = GetCurrentPreset();
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0, 1, t);
            
            LerpPresets(startPreset, targetPreset, t);
            yield return null;
        }
        
        ApplyPreset(targetPreset);
        activeTransition = null;
    }
    
    void LerpPresets(TVEffectPreset from, TVEffectPreset to, float t)
    {
        tvEffectsMaterial.SetFloat("_NoiseIntensity", Mathf.Lerp(from.noiseIntensity, to.noiseIntensity, t));
        tvEffectsMaterial.SetFloat("_NoiseScale", Mathf.Lerp(from.noiseScale, to.noiseScale, t));
        tvEffectsMaterial.SetFloat("_NoiseSpeed", Mathf.Lerp(from.noiseSpeed, to.noiseSpeed, t));
        
        tvEffectsMaterial.SetFloat("_ScanlineIntensity", Mathf.Lerp(from.scanlineIntensity, to.scanlineIntensity, t));
        tvEffectsMaterial.SetFloat("_ScanlineCount", Mathf.Lerp(from.scanlineCount, to.scanlineCount, t));
        tvEffectsMaterial.SetFloat("_ScanlineSpeed", Mathf.Lerp(from.scanlineSpeed, to.scanlineSpeed, t));
        
        tvEffectsMaterial.SetFloat("_GlitchIntensity", Mathf.Lerp(from.glitchIntensity, to.glitchIntensity, t));
        tvEffectsMaterial.SetFloat("_GlitchSpeed", Mathf.Lerp(from.glitchSpeed, to.glitchSpeed, t));
        tvEffectsMaterial.SetFloat("_ChromaticAberration", Mathf.Lerp(from.chromaticAberration, to.chromaticAberration, t));
        
        tvEffectsMaterial.SetColor("_GlowColor", Color.Lerp(from.glowColor, to.glowColor, t));
        tvEffectsMaterial.SetFloat("_GlowIntensity", Mathf.Lerp(from.glowIntensity, to.glowIntensity, t));
        tvEffectsMaterial.SetFloat("_VignetteIntensity", Mathf.Lerp(from.vignetteIntensity, to.vignetteIntensity, t));
        
        tvEffectsMaterial.SetFloat("_EffectBlend", Mathf.Lerp(from.effectBlend, to.effectBlend, t));
    }
    
    void ApplyPreset(TVEffectPreset preset)
    {
        if (tvEffectsMaterial == null) return;
        
        tvEffectsMaterial.SetFloat("_NoiseIntensity", preset.noiseIntensity);
        tvEffectsMaterial.SetFloat("_NoiseScale", preset.noiseScale);
        tvEffectsMaterial.SetFloat("_NoiseSpeed", preset.noiseSpeed);
        
        tvEffectsMaterial.SetFloat("_ScanlineIntensity", preset.scanlineIntensity);
        tvEffectsMaterial.SetFloat("_ScanlineCount", preset.scanlineCount);
        tvEffectsMaterial.SetFloat("_ScanlineSpeed", preset.scanlineSpeed);
        
        tvEffectsMaterial.SetFloat("_GlitchIntensity", preset.glitchIntensity);
        tvEffectsMaterial.SetFloat("_GlitchSpeed", preset.glitchSpeed);
        tvEffectsMaterial.SetFloat("_ChromaticAberration", preset.chromaticAberration);
        
        tvEffectsMaterial.SetColor("_GlowColor", preset.glowColor);
        tvEffectsMaterial.SetFloat("_GlowIntensity", preset.glowIntensity);
        tvEffectsMaterial.SetFloat("_VignetteIntensity", preset.vignetteIntensity);
        
        tvEffectsMaterial.SetFloat("_EffectBlend", preset.effectBlend);
    }
    
    TVEffectPreset GetCurrentPreset()
    {
        TVEffectPreset current = new TVEffectPreset();
        
        if (tvEffectsMaterial == null) return current;
        
        current.noiseIntensity = tvEffectsMaterial.GetFloat("_NoiseIntensity");
        current.noiseScale = tvEffectsMaterial.GetFloat("_NoiseScale");
        current.noiseSpeed = tvEffectsMaterial.GetFloat("_NoiseSpeed");
        
        current.scanlineIntensity = tvEffectsMaterial.GetFloat("_ScanlineIntensity");
        current.scanlineCount = tvEffectsMaterial.GetFloat("_ScanlineCount");
        current.scanlineSpeed = tvEffectsMaterial.GetFloat("_ScanlineSpeed");
        
        current.glitchIntensity = tvEffectsMaterial.GetFloat("_GlitchIntensity");
        current.glitchSpeed = tvEffectsMaterial.GetFloat("_GlitchSpeed");
        current.chromaticAberration = tvEffectsMaterial.GetFloat("_ChromaticAberration");
        
        current.glowColor = tvEffectsMaterial.GetColor("_GlowColor");
        current.glowIntensity = tvEffectsMaterial.GetFloat("_GlowIntensity");
        current.vignetteIntensity = tvEffectsMaterial.GetFloat("_VignetteIntensity");
        
        current.effectBlend = tvEffectsMaterial.GetFloat("_EffectBlend");
        
        return current;
    }
    
    void OnDestroy()
    {
        if (tvEffectsMaterial != null)
        {
            DestroyImmediate(tvEffectsMaterial);
        }
    }
}