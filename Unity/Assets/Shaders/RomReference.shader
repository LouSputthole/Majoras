Shader "Majoras/ROM Reference Combiner"
{
    Properties
    {
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
        _Lighting ("Approximate normal lighting", Float) = 0
        _Ambient ("Approximate ambient", Range(0,1)) = 0.65
        _Diffuse ("Approximate diffuse", Range(0,1)) = 0.35
        _LightDirection ("Direction toward reference light", Vector) = (-0.4,0.8,-0.3,0)
        _Cutoff ("Approximate alpha rejection", Range(0,1)) = 0.04
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Destination blend", Float) = 0
        [Toggle] _ZWrite ("Write depth", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        Cull [_Cull]
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            sampler2D _Tex0, _Tex1;
            float4 _PrimColor, _EnvColor, _Color1, _Alpha1, _Color2, _Alpha2, _LightDirection;
            float4 _UVBasis, _Tile0, _Tile1, _TileLow0, _TileLow1;
            float _TwoCycle, _Lighting, _Ambient, _Diffuse, _Cutoff, _PrimLodFrac;
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };
            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }
            // IDs belong to the importer, not to N64 hardware mux bit encodings.
            float4 source(int id, float4 combined, float4 tex0, float4 tex1, float4 shade)
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
                // Export omits runtime LOD/chroma-key/YUV coefficients and noise state.
                // Their use is surfaced in the importer result, never silently claimed exact.
                if (id == 16) return 0.5;
                if (id == 18) return 1;
                return 0;
            }
            float4 cycle(float4 rgbIds, float4 alphaIds, float4 combined, float4 tex0, float4 tex1, float4 shade)
            {
                float3 a = source((int)rgbIds.x, combined, tex0, tex1, shade).rgb;
                float3 b = source((int)rgbIds.y, combined, tex0, tex1, shade).rgb;
                float3 c = source((int)rgbIds.z, combined, tex0, tex1, shade).rgb;
                float3 d = source((int)rgbIds.w, combined, tex0, tex1, shade).rgb;
                float aa = source((int)alphaIds.x, combined, tex0, tex1, shade).a;
                float ab = source((int)alphaIds.y, combined, tex0, tex1, shade).a;
                float ac = source((int)alphaIds.z, combined, tex0, tex1, shade).a;
                float ad = source((int)alphaIds.w, combined, tex0, tex1, shade).a;
                return float4((a-b)*c+d, (aa-ab)*ac+ad);
            }
            float4 frag(v2f i) : SV_Target
            {
                // Imported UVs describe the unscaled signed N64 ST relative to one basis image.
                // Texture slots can have different dimensions, shifts and tile origins.
                float2 rawST = float2(i.uv.x, 1-i.uv.y) * _UVBasis.xy - 0.5;
                float2 st0 = rawST * _UVBasis.zw * exp2(-_Tile0.zw) - _TileLow0.xy;
                float2 st1 = rawST * _UVBasis.zw * exp2(-_Tile1.zw) - _TileLow1.xy;
                float2 uv0 = (st0 + 0.5) / _Tile0.xy;
                float2 uv1 = (st1 + 0.5) / _Tile1.xy;
                uv0.y = 1-uv0.y;
                uv1.y = 1-uv1.y;
                float4 tex0 = tex2D(_Tex0, uv0);
                float4 tex1 = tex2D(_Tex1, uv1);
                // Preserve baked SHADE RGB for unlit geometry. Normal-lit geometry
                // uses a documented approximation rather than treating packed normals as color.
                float4 shade = i.color;
                if (_Lighting > 0.5)
                {
                    float diffuse = saturate(dot(normalize(i.normal), normalize(_LightDirection.xyz)));
                    shade.rgb = saturate(_Ambient + _Diffuse * diffuse);
                }
                float4 combined = cycle(_Color1, _Alpha1, 0, tex0, tex1, shade);
                if (_TwoCycle > 0.5)
                    combined = cycle(_Color2, _Alpha2, combined, tex0, tex1, shade);
                combined = saturate(combined);
                clip(combined.a - _Cutoff);
                #ifndef UNITY_COLORSPACE_GAMMA
                    combined.rgb = GammaToLinearSpace(combined.rgb);
                #endif
                return combined;
            }
            ENDCG
        }
    }
    Fallback Off
}
