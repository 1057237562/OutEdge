﻿// Standard shader with triplanar mapping
// https://github.com/keijiro/StandardTriplanar

Shader "Custom/StandardTriplanar"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("Texture1", 2D) = "white" {}
		_BumpMap("Bump", 2D) = "bump" {}
		_OcclusionMap("Occlusion", 2D) = "white" {}
		_MainTex2("Texture2", 2D) = "white" {}
		_BumpMap2("Bump2", 2D) = "bump" {}
		_OcclusionMap2("Occlusion2", 2D) = "white" {}
        _MainTex3("Texture3", 2D) = "white" {}
        _BumpMap3("Bump3", 2D) = "bump" {}
        _OcclusionMap3("Occlusion3", 2D) = "white" {}
        _MainTex4("Texture4", 2D) = "white" {}
        _BumpMap4("Bump4", 2D) = "bump" {}
        _OcclusionMap4("Occlusion4", 2D) = "white" {}

        _Glossiness("Glossiness", Range(0, 1)) = 0.5
        [Gamma] _Metallic("Metallic", Range(0, 1)) = 0

        _BumpScale("Bump Scale", Float) = 1

        _OcclusionStrength("OcclusionStrength", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard vertex:vert fullforwardshadows addshadow

        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _OCCLUSIONMAP

        #pragma target 3.0

        half4 _Color;
        sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _OcclusionMap;
		sampler2D _MainTex2;
		sampler2D _BumpMap2;
		sampler2D _OcclusionMap2;
        sampler2D _MainTex3;
        sampler2D _BumpMap3;
        sampler2D _OcclusionMap3;
        sampler2D _MainTex4;
        sampler2D _BumpMap4;
        sampler2D _OcclusionMap4;

        half _Glossiness;
        half _Metallic;

        half _BumpScale;

        half _OcclusionStrength;


        struct Input
        {
            float3 localCoord;
            float3 localNormal;
			float4 color;
        };

        void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);
            data.localCoord = v.vertex.xyz;
            data.localNormal = v.normal.xyz;
			data.color = v.color;
        }

        float when_eq(float a, float b) {
            return 1.0 - abs(sign(a - b));
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Blending factor of triplanar mapping
            float3 bf = normalize(abs(IN.localNormal));
            bf /= dot(bf, (float3)1);

            // Triplanar mapping
            float2 tx = IN.localCoord.yz * 0.2;
            float2 ty = IN.localCoord.zx * 0.2;
            float2 tz = IN.localCoord.xy * 0.2;

            // Base color
            half4 cx = tex2D(_MainTex, tx) * bf.x;
            half4 cy = tex2D(_MainTex, ty) * bf.y;
            half4 cz = tex2D(_MainTex, tz) * bf.z;
            half4 color = (cx + cy + cz) * _Color;
			half4 nx = tex2D(_BumpMap, tx) * bf.x;
			half4 ny = tex2D(_BumpMap, ty) * bf.y;
			half4 nz = tex2D(_BumpMap, tz) * bf.z;
			half ox = tex2D(_OcclusionMap, tx).g * bf.x;
			half oy = tex2D(_OcclusionMap, ty).g * bf.y;
			half oz = tex2D(_OcclusionMap, tz).g * bf.z;

			half4 cx2 = tex2D(_MainTex2, tx) * bf.x;
			half4 cy2 = tex2D(_MainTex2, ty) * bf.y;
			half4 cz2 = tex2D(_MainTex2, tz) * bf.z;
			half4 color2 = (cx2 + cy2 + cz2) * _Color;
			half4 nx2 = tex2D(_BumpMap2, tx) * bf.x;
			half4 ny2 = tex2D(_BumpMap2, ty) * bf.y;
			half4 nz2 = tex2D(_BumpMap2, tz) * bf.z;
			half ox2 = tex2D(_OcclusionMap2, tx).g * bf.x;
			half oy2 = tex2D(_OcclusionMap2, ty).g * bf.y;
			half oz2 = tex2D(_OcclusionMap2, tz).g * bf.z;

            half4 cx3 = tex2D(_MainTex3, tx) * bf.x;
            half4 cy3 = tex2D(_MainTex3, ty) * bf.y;
            half4 cz3 = tex2D(_MainTex3, tz) * bf.z;
            half4 color3 = (cx3 + cy3 + cz3) * _Color;
            half4 nx3 = tex2D(_BumpMap3, tx) * bf.x;
            half4 ny3 = tex2D(_BumpMap3, ty) * bf.y;
            half4 nz3 = tex2D(_BumpMap3, tz) * bf.z;
            half ox3 = tex2D(_OcclusionMap3, tx).g * bf.x;
            half oy3 = tex2D(_OcclusionMap3, ty).g * bf.y;
            half oz3 = tex2D(_OcclusionMap3, tz).g * bf.z;

            half4 cx4 = tex2D(_MainTex4, tx) * bf.x;
            half4 cy4 = tex2D(_MainTex4, ty) * bf.y;
            half4 cz4 = tex2D(_MainTex4, tz) * bf.z;
            half4 color4 = (cx4 + cy4 + cz4) * _Color;
            half4 nx4 = tex2D(_BumpMap4, tx) * bf.x;
            half4 ny4 = tex2D(_BumpMap4, ty) * bf.y;
            half4 nz4 = tex2D(_BumpMap4, tz) * bf.z;
            half ox4 = tex2D(_OcclusionMap4, tx).g * bf.x;
            half oy4 = tex2D(_OcclusionMap4, ty).g * bf.y;
            half oz4 = tex2D(_OcclusionMap4, tz).g * bf.z;

            o.Albedo = color.rgb * (IN.color.r) + color2.rgb * (IN.color.r - 1.0);
			//o.Albedo = color.rgb * when_eq(IN.color.r,0)+color2.rgb* when_eq(IN.color.r,1) + color3.rgb * when_eq(IN.color.r,2) + color4.rgb * when_eq(IN.color.r,3);
			o.Alpha = color.a;
            o.Normal = UnpackNormal(nx + ny + nz) * (IN.color.r ) + UnpackNormal(nx2 + ny2 + nz2) * (IN.color.r - 1.0);
            o.Occlusion = lerp((half4)1, ox + oy + oz, _OcclusionStrength) * (IN.color.r ) + lerp((half4)1, ox2 + oy2 + oz2, _OcclusionStrength) * (IN.color.r - 1.0);
			//o.Normal = UnpackNormal(nx + ny + nz)* when_eq(IN.color.r, 0) + UnpackNormal(nx2 + ny2 + nz2) * when_eq(IN.color.r, 1) + UnpackNormal(nx3 + ny3 + nz3) * when_eq(IN.color.r, 2) + UnpackNormal(nx4 + ny4 + nz4) * when_eq(IN.color.r,3);
			//o.Occlusion = lerp((half4)1, ox + oy + oz, _OcclusionStrength)* when_eq(IN.color.r, 0) + lerp((half4)1, ox2 + oy2 + oz2, _OcclusionStrength) * when_eq(IN.color.r, 1) + lerp((half4)1, ox3 + oy3 + oz3, _OcclusionStrength) * when_eq(IN.color.r, 2) + lerp((half4)1, ox4 + oy4 + oz4, _OcclusionStrength) * when_eq(IN.color.r,3);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
    CustomEditor "StandardTriplanarInspector"
}
