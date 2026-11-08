Shader "Majoras/City Sky"
{
    Properties
    {
        _Zenith ("Zenith", Color) = (0.12,0.36,0.66,1)
        _Horizon ("Horizon", Color) = (0.67,0.80,0.86,1)
        _Ground ("Lower hemisphere", Color) = (0.30,0.36,0.39,1)
        _CloudColor ("Cloud color", Color) = (0.92,0.94,0.91,1)
        [HDR] _SunColor ("Sun color", Color) = (1.1,0.93,0.70,1)
        _SunDirection ("Toward sun (zero uses scene light)", Vector) = (0,0,0,0)
        _SunEnabled ("Sun visibility", Range(0,1)) = 1
        _SunSize ("Sun angular radius in degrees", Range(0.1,8)) = 1.1
        _SunHalo ("Sun halo", Range(0,1)) = 0.13
        _CloudCoverage ("Cloud coverage", Range(0,1)) = 0.42
        _CloudSoftness ("Cloud softness", Range(0.02,0.4)) = 0.18
        _CloudScale ("Broad cloud scale", Range(0.5,8)) = 2.3
        _CloudOpacity ("Cloud opacity", Range(0,1)) = 0.68
        _Exposure ("Exposure", Range(0,4)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off
        ZTest LEqual
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex CitySkyVert
            #pragma fragment CitySkyFrag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            half4 _Zenith, _Horizon, _Ground, _CloudColor, _SunColor;
            float4 _SunDirection;
            half _SunEnabled, _SunSize, _SunHalo, _CloudCoverage;
            half _CloudSoftness, _CloudScale, _CloudOpacity, _Exposure;

            struct CitySkyVertex
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct CitySkyInterpolators
            {
                float4 position : SV_POSITION;
                float3 direction : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CitySkyInterpolators CitySkyVert(CitySkyVertex v)
            {
                CitySkyInterpolators o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.position = UnityObjectToClipPos(v.vertex);
                // Skybox cube vertices describe directions, not scene positions.
                o.direction = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
                return o;
            }

            float CitySkyHash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1,311.7))) * 43758.5453);
            }
            float CitySkyNoise(float2 p)
            {
                float2 cell = floor(p);
                float2 blend = frac(p);
                blend = blend * blend * (3-2*blend);
                float bottom = lerp(CitySkyHash(cell), CitySkyHash(cell+float2(1,0)), blend.x);
                float top = lerp(CitySkyHash(cell+float2(0,1)), CitySkyHash(cell+float2(1,1)), blend.x);
                return lerp(bottom, top, blend.y);
            }

            half4 CitySkyFrag(CitySkyInterpolators i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float3 direction = normalize(i.direction);
                // A broad warm/cool horizon band supports both day and dusk
                // palettes. Root-owned materials choose actual profile colors.
                float upper = pow(saturate(direction.y), 0.65);
                float lower = 1-smoothstep(-0.65,0,direction.y);
                float3 color = lerp(_Horizon.rgb, _Zenith.rgb, upper);
                color = lerp(color, _Ground.rgb, lower);

                float2 cloudUV = direction.xz / max(0.28,direction.y+0.32) * _CloudScale;
                float cloud = CitySkyNoise(cloudUV) * 0.60;
                cloud += CitySkyNoise(cloudUV*2.03 + float2(4.7,9.2)) * 0.28;
                cloud += CitySkyNoise(cloudUV*4.07 + float2(11.3,2.8)) * 0.12;
                float threshold = 1-_CloudCoverage;
                float softness = max(0.02,_CloudSoftness);
                float cloudMask = smoothstep(threshold-softness,threshold+softness,cloud);
                cloudMask *= smoothstep(-0.02,0.20,direction.y);
                cloudMask *= 1-smoothstep(0.80,1.0,direction.y);
                color = lerp(color,_CloudColor.rgb,saturate(cloudMask*_CloudOpacity));

                float3 lightDirection = _WorldSpaceLightPos0.xyz;
                // Material direction is deterministic for fixed-camera A/B.
                // A zero vector opts into the active directional scene light.
                if (dot(_SunDirection.xyz,_SunDirection.xyz) > 0.0001)
                    lightDirection = _SunDirection.xyz;
                if (dot(lightDirection,lightDirection) < 0.0001)
                    lightDirection = float3(0.3,0.65,-0.7);
                lightDirection = normalize(lightDirection);
                float facing = dot(direction,lightDirection);
                float radius = radians(_SunSize);
                float disc = smoothstep(cos(radius*1.15),cos(radius*0.8),facing);
                float halo = pow(saturate(facing),64) * _SunHalo;
                float sunOcclusion = 1-saturate(cloudMask*_CloudOpacity*0.8);
                color += _SunColor.rgb * (disc+halo) * _SunEnabled * sunOcclusion;
                return half4(max(0,color)*_Exposure,1);
            }
            ENDCG
        }
    }
    Fallback Off
}
