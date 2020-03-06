Shader "Particles/VertexSpikeDissolve"
{
	Properties
	{
		_Noise("Vert Noise Texture", 2D) = "white" {}
		_Scale("Vert Noise Scale", Range(0,10)) = 0.5
		_NoiseFrag("Frag Noise Texture", 2D) = "white" {}	
		_FragScale("Frag Noise Scale", Range(0,2)) = 0.5
		_ExtraNoise("Overlay Noise Texture", 2D) = "white" {}
		_ExtraScale("Overlay Noise Scale", Range(0,2)) = 0.5
		_Color("Overlay Noise Color", Color) = (1,0.5,0,0)
		_Tint("Tint", Color) = (1,1,0,0) // Color of the dissolve Line
		_EdgeColor("Edge", Color) = (1,0.5,0,0) // Color of the dissolve Line)
		_Fuzziness("Fuzziness", Range(0,2)) = 0.3
		_Stretch("Dissolve Stretch", Range(0,4)) = 2
		_Growth("Growth", Range(0,2)) = 0
		_Spike("Spike", Range(0,2)) = 1
			_Cutoff("Cutoff", Range(0,1)) = 0.9
		_Delay("Dissolve Delay", Range(0,2)) = 0
		[Toggle(SOFT)] _SOFT("Soft Spikes", Float) = 0
		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("Blend Op", Int) = 0// 0 = add, 4 = max, other ones probably won't look good
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
			Blend One OneMinusSrcAlpha
			ColorMask RGB
			Cull Off Lighting Off ZWrite Off
			ZTest Always
			BlendOp[_BlendOp]

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog
				#pragma shader_feature SOFT

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float3 uv : TEXCOORD0;// .z has particle age
					float4 color : COLOR;
					float4 normal :NORMAL;
				};

				struct v2f
				{
					float3 uv : TEXCOORD0; // .z has particle age
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
					float4 color: COLOR;
					float3 noise : TEXCOORD2;
					
				};

				sampler2D _Noise, _NoiseFrag, _ExtraNoise;
				float4 _Noise_ST, _Tint, _EdgeColor, _Color;
				float _Scale, _Cutoff, _Fuzziness, _Stretch, _EdgeWidth, _Growth, _Spike;
				float _Delay, _ExtraScale, _FragScale;
				v2f vert(appdata v)
				{
					v2f o;				
					o.uv.xy = TRANSFORM_TEX(v.uv.xy, _Noise);
					UNITY_TRANSFER_FOG(o, o.vertex);
					float3 noise = tex2Dlod(_Noise, float4(v.uv.xy * _Scale, 1, 1));// noise texture for spikes
					
#if SOFT
					v.vertex.xyz +=  (noise.r* v.normal) * _Spike;// creating spikes without the step cutoff, more shallower spikes
#else
				v.vertex.xyz += step(_Cutoff, noise.r) * (v.normal * _Spike);// harsh spikes using step cutoff
#endif		
					v.vertex.xyz += (v.normal *  (v.uv.z)) * _Growth;// increase overall size over particle age
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv.z = v.uv.z - _Delay;// subtract a number to delay the dissolve
					o.color = v.color;			
					o.noise = noise;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					half4 noise = tex2D(_NoiseFrag, i.uv.xy  * _FragScale);// dissolve texture
					half4 extraTexture = tex2D(_ExtraNoise, i.uv.xy * _ExtraScale);// extra overlay texture
					float combinedNoise = (noise.r + extraTexture.r) / 2; // combining noise for a more interesting result
					float dissolve = smoothstep(_Stretch * i.uv.z, _Stretch * i.uv.z + _Fuzziness, combinedNoise);// smooth dissolve
					float4 color = lerp(_EdgeColor,_Tint, dissolve.r);
					color += (extraTexture * _Color); // add extra texture (colored)
					color *= dissolve; // multiply with dissolve so theres no bleeding
					color *= i.color;// multiply with the color over time					

					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, color);
					return color;

				}
				ENDCG
			}
		}
}