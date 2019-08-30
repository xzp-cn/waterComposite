// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-7322-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32309,y:32676,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:0.5019608;n:type:ShaderForge.SFN_Tex2d,id:2529,x:32098,y:32829,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:_Diffuse,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-630-OUT;n:type:ShaderForge.SFN_Multiply,id:7322,x:32532,y:32738,varname:node_7322,prsc:2|A-7241-RGB,B-8785-OUT;n:type:ShaderForge.SFN_Multiply,id:8785,x:32309,y:32866,varname:node_8785,prsc:2|A-2529-RGB,B-9663-RGB;n:type:ShaderForge.SFN_Tex2d,id:9663,x:32098,y:33017,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:_Mask,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3307,x:31530,y:32830,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:_Noise,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6584-UVOUT;n:type:ShaderForge.SFN_Multiply,id:8502,x:31756,y:32816,varname:node_8502,prsc:2|A-4851-OUT,B-3307-R;n:type:ShaderForge.SFN_Slider,id:4851,x:31432,y:32725,ptovrint:False,ptlb:Noise_Power,ptin:_Noise_Power,varname:_Noise_Power,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_Add,id:630,x:31933,y:32798,varname:node_630,prsc:2|A-6550-UVOUT,B-8502-OUT;n:type:ShaderForge.SFN_TexCoord,id:6550,x:31756,y:32665,varname:node_6550,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:6584,x:31330,y:32813,varname:node_6584,prsc:2,uv:0;proporder:7241-2529-9663-3307-4851;pass:END;sub:END;*/

Shader "Jan_shader/Noise Add Mask" {
    Properties {
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,0.5019608)
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _Noise_Power ("Noise_Power", Range(0, 1)) = 0.1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 3.0  
            #include "UnityCG.cginc"
            uniform float4 _Color;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Noise_Power;
            struct VertexInput {
                half4 vertex : POSITION;
                half2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                half4 pos : SV_POSITION;
                half2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            fixed4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                half isFrontFace = ( facing >= 0 ? 1 : 0 );
                half faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                half4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                half2 node_630 = (i.uv0+(_Noise_Power*_Noise_var.r));
                half4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(node_630, _Diffuse));
                half4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                half3 emissive = (_Color.rgb*(_Diffuse_var.rgb*_Mask_var.rgb));
                half3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    //FallBack "Diffuse"
    //CustomEditor "ShaderForgeMaterialInspector"
}
