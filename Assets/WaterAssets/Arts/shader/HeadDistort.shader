// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:Particles/Additive,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|alpha-8049-OUT,refract-35-OUT;n:type:ShaderForge.SFN_Tex2d,id:932,x:31994,y:32592,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_932,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1064-UVOUT;n:type:ShaderForge.SFN_Append,id:7886,x:32252,y:32625,varname:node_7886,prsc:2|A-932-R,B-932-G;n:type:ShaderForge.SFN_Vector1,id:8049,x:32956,y:32678,varname:node_8049,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:35,x:32971,y:32887,varname:node_35,prsc:2|A-2244-OUT,B-5977-OUT,C-6904-OUT;n:type:ShaderForge.SFN_Slider,id:6919,x:31915,y:33019,ptovrint:False,ptlb:Head_Force,ptin:_Head_Force,varname:node_6919,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_VertexColor,id:2394,x:31994,y:33103,varname:node_2394,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:3729,x:31994,y:32822,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_3729,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5977,x:32446,y:32916,varname:node_5977,prsc:2|A-932-A,B-3729-A,C-6919-OUT,D-2394-A;n:type:ShaderForge.SFN_Panner,id:1064,x:31788,y:32581,varname:node_1064,prsc:2,spu:1,spv:0|UVIN-1443-UVOUT,DIST-4639-OUT;n:type:ShaderForge.SFN_TexCoord,id:1443,x:31529,y:32561,varname:node_1443,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:5225,x:31440,y:32908,ptovrint:False,ptlb:Head_Speed,ptin:_Head_Speed,varname:node_5225,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Time,id:2592,x:31576,y:32752,varname:node_2592,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4639,x:31788,y:32790,varname:node_4639,prsc:2|A-2592-T,B-5225-OUT;n:type:ShaderForge.SFN_RemapRange,id:2244,x:32446,y:32707,varname:node_2244,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7886-OUT;n:type:ShaderForge.SFN_TexCoord,id:1666,x:32283,y:33115,varname:node_1666,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector2,id:1916,x:32283,y:33288,varname:node_1916,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Distance,id:2752,x:32466,y:33163,varname:node_2752,prsc:2|A-1666-UVOUT,B-1916-OUT;n:type:ShaderForge.SFN_OneMinus,id:6904,x:32818,y:33167,varname:node_6904,prsc:2|IN-7934-OUT;n:type:ShaderForge.SFN_Power,id:7934,x:32658,y:33167,varname:node_7934,prsc:2|VAL-2752-OUT,EXP-2942-OUT;n:type:ShaderForge.SFN_Vector1,id:2942,x:32507,y:33315,varname:node_2942,prsc:2,v1:0.3;proporder:932-3729-5225-6919;pass:END;sub:END;*/

Shader "Jan_shader/HeadDistort" {
    Properties {
        _Texture ("Texture", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _Head_Speed ("Head_Speed", Range(-1, 1)) = 0
        _Head_Force ("Head_Force", Range(0, 2)) = 0.5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 3000
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            //#pragma multi_compile_fwdbase
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float _Head_Force;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Head_Speed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float4 node_2592 = _Time + _TimeEditor;
                float2 node_1064 = (i.uv0+(node_2592.g*_Head_Speed)*float2(1,0));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_1064, _Texture));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + ((float2(_Texture_var.r,_Texture_var.g)*2.0+-1.0)*(_Texture_var.a*_Mask_var.a*_Head_Force*i.vertexColor.a)*(1.0 - pow(distance(i.uv0,float2(0.5,0.5)),0.3)));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                return fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
            }
            ENDCG
        }
    }
    FallBack "Particles/Additive"
    //CustomEditor "ShaderForgeMaterialInspector"
}
