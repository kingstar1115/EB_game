Shader "DoubleSided/Standard/StandardDoubleSided"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
		_ParallaxMap ("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}
		
		_DetailMask("Detail Mask", 2D) = "white" {}

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

		// UI-only data
		[HideInInspector] _EmissionScaleUI("Scale", Float) = 0.0
		[HideInInspector] _EmissionColorUI("Color", Color) = (1,1,1)

		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

	CGINCLUDE
		#define UNITY_SETUP_BRDF_INPUT MetallicSetup
	ENDCG

	SubShader
	{
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 300
	

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			Cull Off // culling off

			CGPROGRAM

			#pragma target 3.0
			#pragma glsl
			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles
			
			// -------------------------------------
					
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP 
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP
			
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
				
			#pragma vertex vertForwardBase
			#pragma fragment fragForwardBaseDS // use custom pixel shader

			#include "UnityStandardCore.cginc"

			/// pixel shader method
			/// using api VFACE semantic which give us face direction value 
			half4 fragForwardBaseDS (
				VertexOutputForwardBase i,
				 in float face : VFACE
				 ) : SV_Target
			{
				FRAGMENT_SETUP(s)

				/// flip direction of normal based on sign of face
				float3 normal = s.normalWorld * sign(face);

				UnityLight mainLight = MainLight (normal);
				half atten = SHADOW_ATTENUATION(i);
	
				half occlusion = Occlusion(i.tex.xy);
				UnityGI gi = FragmentGI (
					s.posWorld, occlusion, i.ambientOrLightmapUV, atten, s.oneMinusRoughness,normal, s.eyeVec, mainLight);

				half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, normal, -s.eyeVec, gi.light, gi.indirect);
				c.rgb += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, normal, -s.eyeVec, occlusion, gi);
				c.rgb += Emission(i.tex.xy);

				UNITY_APPLY_FOG(i.fogCoord, c.rgb);
				return OutputForward (c, s.alpha);
			}

			

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend [_SrcBlend] One
			Fog { Color (0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual
			Cull Off // culling off

			CGPROGRAM
			#pragma target 3.0
			#pragma glsl
			// GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles

			// -------------------------------------

			
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP
			
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog
			
			#pragma vertex vertForwardAdd
			#pragma fragment fragForwardAddDS // use custom pixel shader
			
			#include "UnityStandardCore.cginc"

			/// pixel shader method
			/// using api VFACE semantic which give us face direction value 
			half4 fragForwardAddDS (VertexOutputForwardAdd i, in float face : VFACE) : SV_Target
			{
				FRAGMENT_SETUP_FWDADD(s)
				
				/// flip direction of normal based on sign of face
				float3 normalWS = s.normalWorld * sign(face);
				
				
				UnityLight light = AdditiveLight (normalWS, IN_LIGHTDIR_FWDADD(i), LIGHT_ATTENUATION(i));
				UnityIndirect noIndirect = ZeroIndirect ();

				half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, normalWS, -s.eyeVec, light, noIndirect);
	
				UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0,0,0,0)); // fog towards black in additive pass
				return OutputForward (c, s.alpha);
			}



			ENDCG
		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass 
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			Cull Off // culling off

			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 3.0
			#pragma glsl
			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles
			
			// -------------------------------------


			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma multi_compile_shadowcaster

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "UnityStandardShadow.cginc"

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Deferred pass
		Pass
		{
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }

			Cull Off // culling off

			CGPROGRAM
			#pragma target 3.0
			#pragma glsl
			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers nomrt gles
			


			// -------------------------------------

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

			

			#pragma vertex vertDeferred
			#pragma fragment fragDeferredDS // use custom pixel shader

			#include "UnityStandardCore.cginc"
			
			/// pixel shader method
			/// using api VFACE semantic which give us face direction value 
			void fragDeferredDS (
				VertexOutputDeferred i,
				in float face : VFACE,
				out half4 outDiffuse : SV_Target0,			// RT0: diffuse color (rgb), occlusion (a)
				out half4 outSpecSmoothness : SV_Target1,	// RT1: spec color (rgb), smoothness (a)
				out half4 outNormal : SV_Target2,			// RT2: normal (rgb), --unused, very low precision-- (a) 
				out half4 outEmission : SV_Target3			// RT3: emission (rgb), --unused-- (a)
			)
			{
				#if (SHADER_TARGET < 30)
					outDiffuse = 1;
					outSpecSmoothness = 1;
					outNormal = 0;
					outEmission = 0;
					return;
				#endif

				FRAGMENT_SETUP(s)

				/// flip direction of normal based on sign of face
				float3 normalWS = s.normalWorld * sign(face);
				
				// no analytic lights in this pass
				UnityLight dummyLight = DummyLight (normalWS);
				half atten = 1;

				// only GI
				half occlusion = Occlusion(i.tex.xy);
				UnityGI gi = FragmentGI (
					s.posWorld, occlusion, i.ambientOrLightmapUV, atten, s.oneMinusRoughness, normalWS, s.eyeVec, dummyLight);

				half3 color = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, normalWS, -s.eyeVec, gi.light, gi.indirect).rgb;
				color += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, normalWS, -s.eyeVec, occlusion, gi);

				#ifdef _EMISSION
					color += Emission (i.tex.xy);
				#endif

				#ifndef UNITY_HDR_ON
					color.rgb = exp2(-color.rgb);
				#endif

				outDiffuse = half4(s.diffColor, occlusion) ; 
				outSpecSmoothness = half4(s.specColor, s.oneMinusRoughness);
				outNormal = half4( normalWS * 0.5 + 0.5, 1);
				outEmission = half4(color, 1);
			}	



			ENDCG
		}

		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
		{
			Name "META" 
			Tags { "LightMode"="Meta" }

			Cull Off // culling off

			CGPROGRAM
			#pragma vertex vert_meta
			#pragma fragment frag_meta

			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2

			#include "UnityStandardMeta.cginc"
			ENDCG
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	SubShader
	{
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 150

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }
			Cull Off // culling off

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			CGPROGRAM
			#pragma target 2.0

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION 
			#pragma shader_feature _METALLICGLOSSMAP 
			#pragma shader_feature ___ _DETAIL_MULX2
			// SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP

			#pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
	
			#pragma vertex vertForwardBase
			#pragma fragment fragForwardDS // use custom pixel shader

			#include "UnityStandardCore.cginc"

			/// pixel shader method
			/// using api VFACE semantic which give us face direction value 
			half4 fragForwardDS (VertexOutputForwardBase i, in float face : VFACE) : SV_Target
			{
			
				FRAGMENT_SETUP(s)

				/// flip direction of normal based on sign of face
				float3 normal = s.normalWorld * sign(face);

				UnityLight mainLight = MainLight (normal);
				half atten = SHADOW_ATTENUATION(i);
	
				half occlusion = Occlusion(i.tex.xy);
				UnityGI gi = FragmentGI (
					s.posWorld, occlusion, i.ambientOrLightmapUV, atten, s.oneMinusRoughness,normal, s.eyeVec, mainLight);

				half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, normal, -s.eyeVec, gi.light, gi.indirect);
				c.rgb += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, normal, -s.eyeVec, occlusion, gi);
				c.rgb += Emission(i.tex.xy);

				UNITY_APPLY_FOG(i.fogCoord, c.rgb);
				return OutputForward (c, s.alpha);
			}

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend [_SrcBlend] One
			Fog { Color (0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual

			Cull Off  // culling off
			
			CGPROGRAM
			#pragma target 2.0
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2
			// SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP
			#pragma skip_variants SHADOWS_SOFT
			
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog
			
			#pragma vertex vertForwardAdd
			#pragma fragment fragForwardAddDS // use custom pixel shader

			#include "UnityStandardCore.cginc"

			/// pixel shader method
			/// using api VFACE semantic which give us face direction value 
			half4 fragForwardAddDS (VertexOutputForwardAdd i, in float face : VFACE) : SV_Target
			{
				FRAGMENT_SETUP_FWDADD(s)
				
				/// flip direction of normal based on sign of face
				float3 normalWS = s.normalWorld * sign(face);
				
				UnityLight light = AdditiveLight (normalWS, IN_LIGHTDIR_FWDADD(i), LIGHT_ATTENUATION(i));
				UnityIndirect noIndirect = ZeroIndirect ();

				half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, normalWS, -s.eyeVec, light, noIndirect);
	
				UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0,0,0,0)); // fog towards black in additive pass
				return OutputForward (c, s.alpha);
			}

			ENDCG
		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			Cull Off // culling off

			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 2.0
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma skip_variants SHADOWS_SOFT
			#pragma multi_compile_shadowcaster

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "UnityStandardShadow.cginc"

			ENDCG
		}

		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
		{
			Name "META" 
			Tags { "LightMode"="Meta" }

			Cull Off // culling off

			CGPROGRAM
			#pragma vertex vert_meta
			#pragma fragment frag_meta

			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2

			#include "UnityStandardMeta.cginc"
			ENDCG
		}
	}

	FallBack "VertexLit"
	CustomEditor "StandardShaderGUI"
}
