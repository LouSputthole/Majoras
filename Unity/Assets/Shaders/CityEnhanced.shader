Shader "Majoras/City Enhanced"
{
    Properties
    {
        // Keep the complete reference material contract so CopyPropertiesFromMaterial
        // retains the combiner, source UV units and both independent tile transforms.
        _Tex0 ("Texture 0 (source RGB data)", 2D) = "white" {}
        _Tex1 ("Texture 1 (source RGB data)", 2D) = "white" {}
        _PrimColor ("Primitive", Vector) = (1,1,1,1)
        _EnvColor ("Environment", Vector) = (1,1,1,1)
        _PrimLodFrac ("Primitive LOD fraction", Float) = 0
        _UVBasis ("UV basis width/height and SP scale S/T", Vector) = (1,1,1,1)
        _Tile0 ("Tile 0 width/height and signed shift S/T", Vector) = (1,1,0,0)
        _Tile1 ("Tile 1 width/height and signed shift S/T", Vector) = (1,1,0,0)
        _TileLow0 ("Tile 0 low S/T in texels", Vector) = (0,0,0,0)
        _TileLow1 ("Tile 1 low S/T in texels", Vector) = (0,0,0,0)
        _Color1 ("Cycle 1 RGB inputs", Vector) = (3,0,6,0)
        _Alpha1 ("Cycle 1 alpha inputs", Vector) = (3,0,6,0)
        _Color2 ("Cycle 2 RGB inputs", Vector) = (0,0,0,2)
        _Alpha2 ("Cycle 2 alpha inputs", Vector) = (0,0,0,2)
        _TwoCycle ("Two cycle", Float) = 0
        _Lighting ("Source normal-lit flag", Float) = 0
        _Ambient ("Reference ambient (retained metadata)", Range(0,1)) = 0.65
        _Diffuse ("Reference diffuse (retained metadata)", Range(0,1)) = 0.35
        _LightDirection ("Reference light direction (retained metadata)", Vector) = (-0.4,0.8,-0.3,0)
        _Cutoff ("Alpha rejection", Range(0,1)) = 0.04
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Destination blend", Float) = 0
        [Toggle] _ZWrite ("Write depth", Float) = 1

        _TintColor ("City material tint", Color) = (1,1,1,1)
        _Smoothness ("Smoothness", Range(0,1)) = 0.18
        _Metallic ("Metallic", Range(0,1)) = 0
        _BakedShadeStrength ("Retained unlit vertex shade", Range(0,1)) = 0.5
        _EmissionStrength ("Small albedo emission floor", Range(0,0.25)) = 0.015
        _ReliefStrength ("Color-derived relief strength", Range(0,4)) = 0.4
        [Enum(Texture0,0,Texture1,1)] _ReliefSlot ("Relief source texture", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 300
        Cull [_Cull]
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]

        CGPROGRAM
        #pragma target 3.0
        #pragma surface CitySurf Standard fullforwardshadows addshadow alphatest:_Cutoff vertex:CityVert
        #include "UnityCG.cginc"

        sampler2D _Tex0, _Tex1;
        float4 _Tex0_TexelSize, _Tex1_TexelSize;
        float4 _PrimColor, _EnvColor, _Color1, _Alpha1, _Color2, _Alpha2;
        float4 _UVBasis, _Tile0, _Tile1, _TileLow0, _TileLow1;
        fixed4 _TintColor;
        float _TwoCycle, _Lighting, _PrimLodFrac;
        // alphatest:_Cutoff declares its uniform in generated surface passes.
        half _Smoothness, _Metallic, _BakedShadeStrength, _EmissionStrength;
        half _ReliefStrength, _ReliefSlot;

        struct Input
        {
            float2 citySourceUV;
            float4 cityVertexColor;
        };

        void CityVert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.citySourceUV = v.texcoord.xy;
            o.cityVertexColor = v.color;
            // Reference meshes can have no tangents. A stable basis prevents a
            // degenerate Standard normal transform; recalculated UV tangents on
            // the enhanced mesh are preferred for correctly oriented relief.
            if (dot(v.tangent.xyz, v.tangent.xyz) < 0.001)
            {
                float3 n = normalize(v.normal);
                float3 axis = abs(n.y) > 0.95 ? float3(1,0,0) : float3(0,1,0);
                v.tangent = float4(normalize(cross(axis, n)), 1);
            }
        }

        // The IDs match ImportRomReference, not raw hardware mux bitfields.
        float4 CitySource(int id, float4 combined, float4 tex0, float4 tex1, float4 shade)
        {
            if (id == 0) return 0;
            if (id == 1) return 1;
            if (id == 2) return combined;
            if (id == 3) return tex0;
            if (id == 4) return tex1;
            if (id == 5) return _PrimColor;
            if (id == 6) return shade;
            if (id == 7) return _EnvColor;
            if (id == 8) return combined.aaaa;
            if (id == 9) return tex0.aaaa;
            if (id == 10) return tex1.aaaa;
            if (id == 11) return _PrimColor.aaaa;
            if (id == 12) return shade.aaaa;
            if (id == 13) return _EnvColor.aaaa;
            if (id == 15) return _PrimLodFrac;
            // Retain the reference renderer's explicit approximation policy.
            if (id == 16) return 0.5;
            if (id == 18) return 1;
            return 0;
        }

        float4 CityCycle(float4 rgbIds, float4 alphaIds, float4 combined,
                         float4 tex0, float4 tex1, float4 shade)
        {
            float3 a = CitySource((int)rgbIds.x, combined, tex0, tex1, shade).rgb;
            float3 b = CitySource((int)rgbIds.y, combined, tex0, tex1, shade).rgb;
            float3 c = CitySource((int)rgbIds.z, combined, tex0, tex1, shade).rgb;
            float3 d = CitySource((int)rgbIds.w, combined, tex0, tex1, shade).rgb;
            float aa = CitySource((int)alphaIds.x, combined, tex0, tex1, shade).a;
            float ab = CitySource((int)alphaIds.y, combined, tex0, tex1, shade).a;
            float ac = CitySource((int)alphaIds.z, combined, tex0, tex1, shade).a;
            float ad = CitySource((int)alphaIds.w, combined, tex0, tex1, shade).a;
            return float4((a-b)*c+d, (aa-ab)*ac+ad);
        }

        float CityHeight(float3 color)
        {
            return dot(color, float3(0.2126, 0.7152, 0.0722));
        }

        float3 CityRelief(float2 uv0, float2 uv1)
        {
            if (_ReliefStrength <= 0.0001) return float3(0,0,1);
            float left, right, down, up;
            if (_ReliefSlot > 0.5)
            {
                float2 stepUV = max(abs(_Tex1_TexelSize.xy), float2(0.000001,0.000001));
                left = CityHeight(tex2D(_Tex1, uv1 - float2(stepUV.x,0)).rgb);
                right = CityHeight(tex2D(_Tex1, uv1 + float2(stepUV.x,0)).rgb);
                down = CityHeight(tex2D(_Tex1, uv1 - float2(0,stepUV.y)).rgb);
                up = CityHeight(tex2D(_Tex1, uv1 + float2(0,stepUV.y)).rgb);
            }
            else
            {
                float2 stepUV = max(abs(_Tex0_TexelSize.xy), float2(0.000001,0.000001));
                left = CityHeight(tex2D(_Tex0, uv0 - float2(stepUV.x,0)).rgb);
                right = CityHeight(tex2D(_Tex0, uv0 + float2(stepUV.x,0)).rgb);
                down = CityHeight(tex2D(_Tex0, uv0 - float2(0,stepUV.y)).rgb);
                up = CityHeight(tex2D(_Tex0, uv0 + float2(0,stepUV.y)).rgb);
            }
            // Sample the actual image's neighboring pixels, while source tile
            // dimensions still own UV placement. Bound relief at high contrast
            // edges so painted motifs do not acquire near-horizontal normals.
            float2 slope = clamp(float2(left-right, down-up) * _ReliefStrength * 2, -0.75, 0.75);
            return normalize(float3(slope,1));
        }

        void CitySurf(Input i, inout SurfaceOutputStandard o)
        {
            // Original N64 ST basis and tile resolution MUST remain unchanged
            // when replacing a 32/64px source image with a high-resolution one.
            // _TexN_TexelSize affects relief sampling only, never UV placement.
            float2 rawST = float2(i.citySourceUV.x, 1-i.citySourceUV.y) * _UVBasis.xy - 0.5;
            float2 st0 = rawST * _UVBasis.zw * exp2(-_Tile0.zw) - _TileLow0.xy;
            float2 st1 = rawST * _UVBasis.zw * exp2(-_Tile1.zw) - _TileLow1.xy;
            float2 uv0 = (st0 + 0.5) / _Tile0.xy;
            float2 uv1 = (st1 + 0.5) / _Tile1.xy;
            uv0.y = 1-uv0.y;
            uv1.y = 1-uv1.y;
            float4 tex0 = tex2D(_Tex0, uv0);
            float4 tex1 = tex2D(_Tex1, uv1);

            float4 shade = i.cityVertexColor;
            // Real Standard lighting replaces the source's approximate normal
            // lighting. Retain optional baked shade only for source-unlit data.
            shade.rgb = _Lighting > 0.5 ? float3(1,1,1) :
                lerp(float3(1,1,1), shade.rgb, saturate(_BakedShadeStrength));
            float4 combined = CityCycle(_Color1, _Alpha1, 0, tex0, tex1, shade);
            if (_TwoCycle > 0.5)
                combined = CityCycle(_Color2, _Alpha2, combined, tex0, tex1, shade);
            combined = saturate(combined);
            #ifndef UNITY_COLORSPACE_GAMMA
                combined.rgb = GammaToLinearSpace(combined.rgb);
            #endif

            o.Albedo = combined.rgb * _TintColor.rgb;
            o.Alpha = combined.a * _TintColor.a;
            o.Metallic = saturate(_Metallic);
            o.Smoothness = saturate(_Smoothness);
            o.Occlusion = 1;
            o.Normal = CityRelief(uv0, uv1);
            o.Emission = o.Albedo * _EmissionStrength;
        }
        ENDCG
    }
    Fallback "Transparent/Cutout/Diffuse"
}
