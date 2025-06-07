Shader "AlienProbe/TVScreenEffectsV2"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        
        [Header(Static Noise)]
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.1
        _NoiseScale ("Noise Scale", Range(1, 100)) = 50
        _NoiseSpeed ("Noise Animation Speed", Range(0, 10)) = 5
        
        [Header(Scanlines)]
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.3
        _ScanlineCount ("Scanline Count", Range(10, 500)) = 100
        _ScanlineSpeed ("Scanline Scroll Speed", Range(0, 5)) = 0.5
        
        [Header(Glitch Effect)]
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0
        _GlitchSpeed ("Glitch Speed", Range(0, 20)) = 10
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.1)) = 0.01
        
        [Header(Screen Glow)]
        _GlowColor ("Glow Color", Color) = (0.2, 0.8, 0.4, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 2)) = 0.5
        _VignetteIntensity ("Vignette Intensity", Range(0, 2)) = 0.8
        
        [Header(Overall Effect)]
        _EffectBlend ("Effect Blend", Range(0, 1)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100" }
        LOD 100
        
        Blend One OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float _NoiseIntensity;
            float _NoiseScale;
            float _NoiseSpeed;
            
            float _ScanlineIntensity;
            float _ScanlineCount;
            float _ScanlineSpeed;
            
            float _GlitchIntensity;
            float _GlitchSpeed;
            float _ChromaticAberration;
            
            float4 _GlowColor;
            float _GlowIntensity;
            float _VignetteIntensity;
            
            float _EffectBlend;

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                
                float a = rand(i);
                float b = rand(i + float2(1.0, 0.0));
                float c = rand(i + float2(0.0, 1.0));
                float d = rand(i + float2(1.0, 1.0));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y;
                
                // Sample the base texture (UI element texture, usually white)
                fixed4 baseTex = tex2D(_MainTex, uv);
                
                // Start with transparent black
                float3 effectColor = float3(0, 0, 0);
                float effectAlpha = 0;
                
                // Static noise effect
                float staticNoise = noise(uv * _NoiseScale + float2(time * _NoiseSpeed, 0));
                staticNoise = staticNoise * _NoiseIntensity;
                effectColor += float3(staticNoise, staticNoise, staticNoise);
                effectAlpha = max(effectAlpha, staticNoise * 0.8);
                
                // Scanlines
                float scanline = sin((uv.y + time * _ScanlineSpeed) * _ScanlineCount * 3.14159);
                scanline = saturate(scanline + 0.5);
                float scanlineDarkness = _ScanlineIntensity * (1 - scanline);
                effectAlpha = max(effectAlpha, scanlineDarkness * 0.5);
                
                // Glitch effect
                if (_GlitchIntensity > 0)
                {
                    float glitchTime = time * _GlitchSpeed;
                    float glitchNoise = step(0.99 - _GlitchIntensity * 0.1, rand(float2(glitchTime, 0)));
                    
                    if (glitchNoise > 0)
                    {
                        // Chromatic aberration for glitch
                        float2 offset = float2(_ChromaticAberration * _GlitchIntensity, 0);
                        float r = tex2D(_MainTex, uv + offset).r;
                        float g = tex2D(_MainTex, uv).g;
                        float b = tex2D(_MainTex, uv - offset).b;
                        
                        float3 glitchColor = float3(r, g, b) * _GlitchIntensity;
                        effectColor = lerp(effectColor, glitchColor, _GlitchIntensity);
                        effectAlpha = max(effectAlpha, _GlitchIntensity);
                    }
                }
                
                // Vignette effect
                float2 center = uv - 0.5;
                float vignette = 1 - dot(center, center) * _VignetteIntensity;
                vignette = saturate(vignette);
                
                // Screen glow
                float3 glowEffect = _GlowColor.rgb * _GlowIntensity * vignette;
                effectColor += glowEffect;
                effectAlpha = max(effectAlpha, length(glowEffect) * 0.3);
                
                // Apply overall effect blend
                effectAlpha *= _EffectBlend;
                
                // Combine with base texture alpha (for masking)
                effectAlpha *= baseTex.a;
                
                // Pre-multiply alpha for correct blending
                effectColor *= effectAlpha;
                
                return fixed4(effectColor, effectAlpha);
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}