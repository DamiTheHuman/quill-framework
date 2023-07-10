Shader "Unlit/Geometry_PaletteSwapShader"
{
    Properties
    {
        [NoScaleOffset] _MainTex("MainTexture", 2D) = "white" {}
        Vector1_687916C8("Fuzzy", Float) = 0
        Vector1_B6124959("Range", Float) = 0
        Color_From_1("Color From 1", Color) = (0, 0, 0, 1)
        Color_From_2("Color From 2", Color) = (0, 0, 0.5058824, 1)
        Color_From_3("Color From 3", Color) = (0, 0.2196079, 0.7529413, 1)
        Color_From_4("Color From 4", Color) = (0, 0.4117647, 0.937255, 1)
        Color_From_5("Color From 5", Color) = (0.09411766, 0.5333334, 0.937255, 1)
        Color_From_6("Color From 6", Color) = (0.1921569, 0.627451, 0.937255, 1)
        Color_From_7("Color From 7", Color) = (0.4078432, 0.8156863, 0.937255, 1)
        Color_From_8("Color From 8", Color) = (0.09411766, 0.3490196, 0.4078432, 1)
        Color_From_9("Color From 9", Color) = (0.3764706, 0.6313726, 0.6901961, 1)
        Color_From_10("Color From 10", Color) = (0.6235294, 0.8784314, 0.882353, 1)
        Color_From_11("Color From 11", Color) = (0.937255, 0.9411765, 0.937255, 1)
        Color_From_12("Color From 12", Color) = (0.7529413, 0.4117647, 0.1921569, 1)
        Color_From_13("Color From 13", Color) = (0.882353, 0.5686275, 0.3764706, 1)
        Color_From_14("Color From 14", Color) = (0.937255, 0.6901961, 0.5647059, 1)
        Color_From_15("Color From 15", Color) = (0.937255, 0.8156863, 0.7529413, 1)
        Color_From_16("Color From 16", Color) = (0.2588235, 0, 0, 1)
        Color_From_17("Color From 17", Color) = (0.5647059, 0, 0, 1)
        Color_From_18("Color From 18", Color) = (0.882353, 0, 0, 1)
        Color_To_1("Color To 1", Color) = (0, 0, 0, 1)
        Color_To_2("Color To 2", Color) = (0, 0, 0.5058824, 1)
        Color_To_3("Color To 3", Color) = (0, 0.2196079, 0.7529413, 1)
        Color_To_4("Color To 4", Color) = (0, 0.4117647, 0.937255, 1)
        Color_To_5("Color To 5", Color) = (0.09411766, 0.5333334, 0.937255, 1)
        Color_To_6("Color To 6", Color) = (0.1921569, 0.627451, 0.937255, 1)
        Color_To_7("Color To 7", Color) = (0.4078432, 0.8156863, 0.937255, 1)
        Color_To_8("Color To 8", Color) = (0.09411766, 0.3490196, 0.4078432, 1)
        Color_To_9("Color To 9", Color) = (0.3764706, 0.6313726, 0.6901961, 1)
        Color_To_10("Color To 10", Color) = (0.6235294, 0.8784314, 0.882353, 1)
        Color_To_11("Color To 11", Color) = (0.937255, 0.9411765, 0.937255, 1)
        Color_To_12("Color To 12", Color) = (0.7529413, 0.4117647, 0.1921569, 1)
        Color_To_13("Color To 13", Color) = (0.882353, 0.5686275, 0.3764706, 1)
        Color_To_14("Color To 14", Color) = (0.937255, 0.6901961, 0.5647059, 1)
        Color_To_15("Color To 15", Color) = (0.937255, 0.8156863, 0.7529413, 1)
        Color_To_16("Color To 16", Color) = (0.2588235, 0, 0, 1)
        Color_To_17("Color To 17", Color) = (0.5647059, 0, 0, 1)
        Color_To_18("Color To 18", Color) = (0.882353, 0, 0, 1)
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Geometry"
            "ShaderGraphShader" = "true"
            "ShaderGraphTargetId" = ""
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }

        // Render State
        Cull Off
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma exclude_renderers d3d11_9x
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        #define UNIVERSAL_USELEGACYSPRITEBLOCKS
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
    {
         float3 positionOS : POSITION;
         float3 normalOS : NORMAL;
         float4 tangentOS : TANGENT;
         float4 uv0 : TEXCOORD0;
         float4 color : COLOR;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
         float4 positionCS : SV_POSITION;
         float3 positionWS;
         float4 texCoord0;
         float4 color;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };
    struct SurfaceDescriptionInputs
    {
         float4 uv0;
    };
    struct VertexDescriptionInputs
    {
         float3 ObjectSpaceNormal;
         float3 ObjectSpaceTangent;
         float3 ObjectSpacePosition;
    };
    struct PackedVaryings
    {
         float4 positionCS : SV_POSITION;
         float3 interp0 : INTERP0;
         float4 interp1 : INTERP1;
         float4 interp2 : INTERP2;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        ZERO_INITIALIZE(PackedVaryings, output);
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyzw = input.texCoord0;
        output.interp2.xyzw = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }

    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.texCoord0 = input.interp1.xyzw;
        output.color = input.interp2.xyzw;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }


    // --------------------------------------------------
    // Graph

    // Graph Properties
    CBUFFER_START(UnityPerMaterial)
float4 _MainTex_TexelSize;
float Vector1_687916C8;
float Vector1_B6124959;
float4 Color_From_1;
float4 Color_From_2;
float4 Color_From_3;
float4 Color_From_4;
float4 Color_From_5;
float4 Color_From_6;
float4 Color_From_7;
float4 Color_From_8;
float4 Color_From_9;
float4 Color_From_10;
float4 Color_From_11;
float4 Color_From_12;
float4 Color_From_13;
float4 Color_From_14;
float4 Color_From_15;
float4 Color_From_16;
float4 Color_From_17;
float4 Color_From_18;
float4 Color_To_1;
float4 Color_To_2;
float4 Color_To_3;
float4 Color_To_4;
float4 Color_To_5;
float4 Color_To_6;
float4 Color_To_7;
float4 Color_To_8;
float4 Color_To_9;
float4 Color_To_10;
float4 Color_To_11;
float4 Color_To_12;
float4 Color_To_13;
float4 Color_To_14;
float4 Color_To_15;
float4 Color_To_16;
float4 Color_To_17;
float4 Color_To_18;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

// Graph Includes
// GraphIncludes: <None>

// -- Property used by ScenePickingPass
#ifdef SCENEPICKINGPASS
float4 _SelectionID;
#endif

// -- Properties used by SceneSelectionPass
#ifdef SCENESELECTIONPASS
int _ObjectId;
int _PassValue;
#endif

// Graph Functions

void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Preview_float4(float4 In, out float4 Out)
{
    Out = In;
}

void Unity_ColorMask_float(float3 In, float3 MaskColor, float Range, out float Out, float Fuzziness)
{
    float Distance = distance(MaskColor, In);
    Out = saturate(1 - (Distance - Range) / max(Fuzziness, 1e-5));
}

void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
{
    RGBA = float4(R, G, B, A);
    RGB = float3(R, G, B);
    RG = float2(R, G);
}

void Unity_Comparison_Equal_float(float A, float B, out float Out)
{
    Out = A == B ? 1 : 0;
}

void Unity_And_float(float A, float B, out float Out)
{
    Out = A && B;
}

void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
{
    Out = Predicate ? True : False;
}

struct Bindings_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float
{
};

void SG_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float(float4 Vector4_6DC11782, float4 Color_B32C2AE9, Bindings_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float IN, out float4 New1_1)
{
float4 _Property_2a2bcf87adbee28cb6c31451f3fda7b7_Out_0 = Color_B32C2AE9;
float _Split_55cf51d605d3c9898706de9808608534_R_1 = _Property_2a2bcf87adbee28cb6c31451f3fda7b7_Out_0[0];
float _Split_55cf51d605d3c9898706de9808608534_G_2 = _Property_2a2bcf87adbee28cb6c31451f3fda7b7_Out_0[1];
float _Split_55cf51d605d3c9898706de9808608534_B_3 = _Property_2a2bcf87adbee28cb6c31451f3fda7b7_Out_0[2];
float _Split_55cf51d605d3c9898706de9808608534_A_4 = _Property_2a2bcf87adbee28cb6c31451f3fda7b7_Out_0[3];
float4 _Combine_32da8360502e7a8e8a28901aebd31c3d_RGBA_4;
float3 _Combine_32da8360502e7a8e8a28901aebd31c3d_RGB_5;
float2 _Combine_32da8360502e7a8e8a28901aebd31c3d_RG_6;
Unity_Combine_float(_Split_55cf51d605d3c9898706de9808608534_R_1, _Split_55cf51d605d3c9898706de9808608534_G_2, _Split_55cf51d605d3c9898706de9808608534_B_3, 0, _Combine_32da8360502e7a8e8a28901aebd31c3d_RGBA_4, _Combine_32da8360502e7a8e8a28901aebd31c3d_RGB_5, _Combine_32da8360502e7a8e8a28901aebd31c3d_RG_6);
float _Comparison_611e61c407cae982984349b09d40a3ec_Out_2;
Unity_Comparison_Equal_float((_Combine_32da8360502e7a8e8a28901aebd31c3d_RGB_5).x, 0, _Comparison_611e61c407cae982984349b09d40a3ec_Out_2);
float _Comparison_49244d8ede2e11828a2d144434ba5b7f_Out_2;
Unity_Comparison_Equal_float(_Split_55cf51d605d3c9898706de9808608534_A_4, 0, _Comparison_49244d8ede2e11828a2d144434ba5b7f_Out_2);
float _And_5af41e8222280585bd75f616763c135f_Out_2;
Unity_And_float(_Comparison_611e61c407cae982984349b09d40a3ec_Out_2, _Comparison_49244d8ede2e11828a2d144434ba5b7f_Out_2, _And_5af41e8222280585bd75f616763c135f_Out_2);
float4 _Property_c41cdbd8ca5d6d8f9f6175805e60f684_Out_0 = Vector4_6DC11782;
float4 _Branch_558ba3f0fc26c186a66dfa020d7a303c_Out_3;
Unity_Branch_float4(_And_5af41e8222280585bd75f616763c135f_Out_2, float4(0, 0, 0, 0), _Property_c41cdbd8ca5d6d8f9f6175805e60f684_Out_0, _Branch_558ba3f0fc26c186a66dfa020d7a303c_Out_3);
New1_1 = _Branch_558ba3f0fc26c186a66dfa020d7a303c_Out_3;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
}

struct Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float
{
};

void SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(float Vector1_960086FD, float Vector1_7191537E, float4 Vector4_41053054, float4 Vector4_D6A9DA6C, float4 Vector4_1AECF0B2, float4 Vector4_373860C, float4 Vector4_5E07B3C1, Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float IN, out float4 OutVector4_1)
{
float4 _Property_db185fa9ab7b8b8fba026406c5bc5307_Out_0 = Vector4_41053054;
float4 _Preview_e710012ce5fe4b8cae6bbd295c750bc5_Out_1;
Unity_Preview_float4(_Property_db185fa9ab7b8b8fba026406c5bc5307_Out_0, _Preview_e710012ce5fe4b8cae6bbd295c750bc5_Out_1);
float4 _Property_0b9fe9c1987d408887931a9eb4dc2f55_Out_0 = Vector4_D6A9DA6C;
float _Property_45cba9cb23886480af47064f7c23ad4c_Out_0 = Vector1_7191537E;
float _Property_cc8f4c56a331ac8086dac11e6e0b4fb5_Out_0 = Vector1_960086FD;
float _ColorMask_c7b5065fffe28283b143fcad436ebbfe_Out_3;
Unity_ColorMask_float((_Preview_e710012ce5fe4b8cae6bbd295c750bc5_Out_1.xyz), (_Property_0b9fe9c1987d408887931a9eb4dc2f55_Out_0.xyz), _Property_45cba9cb23886480af47064f7c23ad4c_Out_0, _ColorMask_c7b5065fffe28283b143fcad436ebbfe_Out_3, _Property_cc8f4c56a331ac8086dac11e6e0b4fb5_Out_0);
float4 _Property_1ca134b78c7607838681a3a7c485ffa2_Out_0 = Vector4_D6A9DA6C;
Bindings_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float _IsAlphaSubShader_4d692f9636c9e58a85441f1a47593dc8;
float4 _IsAlphaSubShader_4d692f9636c9e58a85441f1a47593dc8_New1_1;
SG_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float((_ColorMask_c7b5065fffe28283b143fcad436ebbfe_Out_3.xxxx), _Property_1ca134b78c7607838681a3a7c485ffa2_Out_0, _IsAlphaSubShader_4d692f9636c9e58a85441f1a47593dc8, _IsAlphaSubShader_4d692f9636c9e58a85441f1a47593dc8_New1_1);
float4 _Property_fd1c7bbed3a17481bcdf97842520e892_Out_0 = Vector4_373860C;
float4 _Multiply_6bd1895176b8dc8d918899d66f9b4fd2_Out_2;
Unity_Multiply_float4_float4(_IsAlphaSubShader_4d692f9636c9e58a85441f1a47593dc8_New1_1, _Property_fd1c7bbed3a17481bcdf97842520e892_Out_0, _Multiply_6bd1895176b8dc8d918899d66f9b4fd2_Out_2);
float4 _Property_fcae524f2531908b8b82532d1b3ab74c_Out_0 = Vector4_1AECF0B2;
float _Property_2f056a3ca6c3a68883dc76c3b3aa9b81_Out_0 = Vector1_7191537E;
float _Property_5a6ef500448fe987bc2109096edeb1a4_Out_0 = Vector1_960086FD;
float _ColorMask_9650a41252fca38f9439f721c9b0208c_Out_3;
Unity_ColorMask_float((_Preview_e710012ce5fe4b8cae6bbd295c750bc5_Out_1.xyz), (_Property_fcae524f2531908b8b82532d1b3ab74c_Out_0.xyz), _Property_2f056a3ca6c3a68883dc76c3b3aa9b81_Out_0, _ColorMask_9650a41252fca38f9439f721c9b0208c_Out_3, _Property_5a6ef500448fe987bc2109096edeb1a4_Out_0);
float4 _Property_c12209c2ca1ac68193e6fd33eff67b36_Out_0 = Vector4_1AECF0B2;
Bindings_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float _IsAlphaSubShader_1bccb36102724e83b69f43bc85fd268e;
float4 _IsAlphaSubShader_1bccb36102724e83b69f43bc85fd268e_New1_1;
SG_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float((_ColorMask_9650a41252fca38f9439f721c9b0208c_Out_3.xxxx), _Property_c12209c2ca1ac68193e6fd33eff67b36_Out_0, _IsAlphaSubShader_1bccb36102724e83b69f43bc85fd268e, _IsAlphaSubShader_1bccb36102724e83b69f43bc85fd268e_New1_1);
float4 _Property_7980f2a99d65dc87bb7c0b7e5a2e4b7b_Out_0 = Vector4_5E07B3C1;
float4 _Multiply_e1b670f83d54b483adc61f2b830d50a2_Out_2;
Unity_Multiply_float4_float4(_IsAlphaSubShader_1bccb36102724e83b69f43bc85fd268e_New1_1, _Property_7980f2a99d65dc87bb7c0b7e5a2e4b7b_Out_0, _Multiply_e1b670f83d54b483adc61f2b830d50a2_Out_2);
float4 _Add_55eeca799efa028bb9f0ae4ef7bec55e_Out_2;
Unity_Add_float4(_Multiply_6bd1895176b8dc8d918899d66f9b4fd2_Out_2, _Multiply_e1b670f83d54b483adc61f2b830d50a2_Out_2, _Add_55eeca799efa028bb9f0ae4ef7bec55e_Out_2);
OutVector4_1 = _Add_55eeca799efa028bb9f0ae4ef7bec55e_Out_2;
}

void Unity_ReplaceColor_float(float3 In, float3 From, float3 To, float Range, out float3 Out, float Fuzziness)
{
    float Distance = distance(From, In);
    Out = lerp(To, In, saturate((Distance - Range) / max(Fuzziness, 1e-5f)));
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Step_float(float Edge, float In, out float Out)
{
    Out = step(Edge, In);
}

/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

// Graph Vertex
struct VertexDescription
{
    float3 Position;
    float3 Normal;
    float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
    VertexDescription description = (VertexDescription)0;
    description.Position = IN.ObjectSpacePosition;
    description.Normal = IN.ObjectSpaceNormal;
    description.Tangent = IN.ObjectSpaceTangent;
    return description;
}

    #ifdef FEATURES_GRAPH_VERTEX
Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
{
return output;
}
#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
#endif

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float4 SpriteColor;
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_2fd764fe4f4d3c8c8e3a830c6bd3004b_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
    float4 _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2fd764fe4f4d3c8c8e3a830c6bd3004b_Out_0.tex, _Property_2fd764fe4f4d3c8c8e3a830c6bd3004b_Out_0.samplerstate, _Property_2fd764fe4f4d3c8c8e3a830c6bd3004b_Out_0.GetTransformedUV(IN.uv0.xy));
    float _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_R_4 = _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0.r;
    float _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_G_5 = _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0.g;
    float _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_B_6 = _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0.b;
    float _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_A_7 = _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0.a;
    float4 _Multiply_c561897b48412d86b5cda6ff92ab408f_Out_2;
    Unity_Multiply_float4_float4(_SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0, float4(1, 1, 1, 1), _Multiply_c561897b48412d86b5cda6ff92ab408f_Out_2);
    float4 _Preview_b8a060a637eb7388b2a6d60d94a9b0cf_Out_1;
    Unity_Preview_float4(_Multiply_c561897b48412d86b5cda6ff92ab408f_Out_2, _Preview_b8a060a637eb7388b2a6d60d94a9b0cf_Out_1);
    float _ColorMask_11413a22eef11f8293f2f5a02b650be1_Out_3;
    Unity_ColorMask_float((_Preview_b8a060a637eb7388b2a6d60d94a9b0cf_Out_1.xyz), (_Preview_b8a060a637eb7388b2a6d60d94a9b0cf_Out_1.xyz), 0, _ColorMask_11413a22eef11f8293f2f5a02b650be1_Out_3, 0);
    float _Property_ea45658fefc23087864dc66f4fe7703d_Out_0 = Vector1_687916C8;
    float _Property_c785c8d3f190448583588345777b583e_Out_0 = Vector1_B6124959;
    float4 _Property_97926f8b9337bf8ea7fbe70dadfee01e_Out_0 = Color_From_1;
    float4 _Property_41dcd2a7b7f8ef84ad69e10acd8844dc_Out_0 = Color_From_2;
    float4 _Property_a56713464c91d18b898c9c5165ba9b8b_Out_0 = Color_To_1;
    float4 _Property_a89e419778e3db86b69c4e84dd8eeaf4_Out_0 = Color_To_2;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_099a84cc2ad0798c8bce7a763f5108f6;
    float4 _TwoPaletteExtractSubGraph_099a84cc2ad0798c8bce7a763f5108f6_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_ea45658fefc23087864dc66f4fe7703d_Out_0, _Property_c785c8d3f190448583588345777b583e_Out_0, _Multiply_c561897b48412d86b5cda6ff92ab408f_Out_2, _Property_97926f8b9337bf8ea7fbe70dadfee01e_Out_0, _Property_41dcd2a7b7f8ef84ad69e10acd8844dc_Out_0, _Property_a56713464c91d18b898c9c5165ba9b8b_Out_0, _Property_a89e419778e3db86b69c4e84dd8eeaf4_Out_0, _TwoPaletteExtractSubGraph_099a84cc2ad0798c8bce7a763f5108f6, _TwoPaletteExtractSubGraph_099a84cc2ad0798c8bce7a763f5108f6_OutVector4_1);
    float _Property_8391d69232e6a48f87150c3de729f119_Out_0 = Vector1_687916C8;
    float _Property_0001ebfc78ac518da115d1de9d1b195e_Out_0 = Vector1_B6124959;
    float4 _Preview_e478343349aef686a521e0245bbec6b8_Out_1;
    Unity_Preview_float4(_Multiply_c561897b48412d86b5cda6ff92ab408f_Out_2, _Preview_e478343349aef686a521e0245bbec6b8_Out_1);
    float4 _Property_752d827a08d2ef838ebca1871584db0d_Out_0 = Color_From_3;
    float4 _Property_9801843bab1e1183b98421b3fc1f8e39_Out_0 = Color_From_4;
    float4 _Property_b485f73fd05c0c8e8310bebc433e7f85_Out_0 = Color_To_3;
    float4 _Property_0c6f652d627da78aa325a866e48be3e7_Out_0 = Color_To_4;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_4eae0bd9a8ac6a8ca6b7ab3170b5bb3c;
    float4 _TwoPaletteExtractSubGraph_4eae0bd9a8ac6a8ca6b7ab3170b5bb3c_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_8391d69232e6a48f87150c3de729f119_Out_0, _Property_0001ebfc78ac518da115d1de9d1b195e_Out_0, _Preview_e478343349aef686a521e0245bbec6b8_Out_1, _Property_752d827a08d2ef838ebca1871584db0d_Out_0, _Property_9801843bab1e1183b98421b3fc1f8e39_Out_0, _Property_b485f73fd05c0c8e8310bebc433e7f85_Out_0, _Property_0c6f652d627da78aa325a866e48be3e7_Out_0, _TwoPaletteExtractSubGraph_4eae0bd9a8ac6a8ca6b7ab3170b5bb3c, _TwoPaletteExtractSubGraph_4eae0bd9a8ac6a8ca6b7ab3170b5bb3c_OutVector4_1);
    float4 _Add_c457cd4750eaff8eb7ab580c762af3e5_Out_2;
    Unity_Add_float4(_TwoPaletteExtractSubGraph_099a84cc2ad0798c8bce7a763f5108f6_OutVector4_1, _TwoPaletteExtractSubGraph_4eae0bd9a8ac6a8ca6b7ab3170b5bb3c_OutVector4_1, _Add_c457cd4750eaff8eb7ab580c762af3e5_Out_2);
    float _Property_0b8337991239488187db38938997b20e_Out_0 = Vector1_687916C8;
    float _Property_98ac839524317b828042576f7edc9628_Out_0 = Vector1_B6124959;
    float4 _Preview_cd6261c20854158b813081e9c9cd0c8c_Out_1;
    Unity_Preview_float4(_Preview_e478343349aef686a521e0245bbec6b8_Out_1, _Preview_cd6261c20854158b813081e9c9cd0c8c_Out_1);
    float4 _Property_bcd638ec9489df8783d4c4cf1882e6c7_Out_0 = Color_From_5;
    float4 _Property_a43e3c04949ac48ca2e984b89f81adf2_Out_0 = Color_From_6;
    float4 _Property_f89f7dfc97f42480a2cd3439aedf3c76_Out_0 = Color_To_5;
    float4 _Property_b1eabf779118d4848a79fbee43e89439_Out_0 = Color_To_6;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_b700307943a378878ec7fa7e72a52064;
    float4 _TwoPaletteExtractSubGraph_b700307943a378878ec7fa7e72a52064_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_0b8337991239488187db38938997b20e_Out_0, _Property_98ac839524317b828042576f7edc9628_Out_0, _Preview_cd6261c20854158b813081e9c9cd0c8c_Out_1, _Property_bcd638ec9489df8783d4c4cf1882e6c7_Out_0, _Property_a43e3c04949ac48ca2e984b89f81adf2_Out_0, _Property_f89f7dfc97f42480a2cd3439aedf3c76_Out_0, _Property_b1eabf779118d4848a79fbee43e89439_Out_0, _TwoPaletteExtractSubGraph_b700307943a378878ec7fa7e72a52064, _TwoPaletteExtractSubGraph_b700307943a378878ec7fa7e72a52064_OutVector4_1);
    float _Property_35f233c7a5dbc48ab9bddac95eb3d495_Out_0 = Vector1_687916C8;
    float _Property_10ad05481f462786a2aee961dd253d79_Out_0 = Vector1_B6124959;
    float4 _Property_7fb63e9e7b15f381aead56b7b55073f1_Out_0 = Color_From_7;
    float4 _Property_7d48a15798242f8391e7bdc870dbc12f_Out_0 = Color_From_8;
    float4 _Property_f6c653a363a77d82a9b281c9064f9012_Out_0 = Color_To_7;
    float4 _Property_8534d1058d4ee5898f6ca231516760e3_Out_0 = Color_To_8;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_b81f9e08b316378dbf53d23988187546;
    float4 _TwoPaletteExtractSubGraph_b81f9e08b316378dbf53d23988187546_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_35f233c7a5dbc48ab9bddac95eb3d495_Out_0, _Property_10ad05481f462786a2aee961dd253d79_Out_0, _Preview_cd6261c20854158b813081e9c9cd0c8c_Out_1, _Property_7fb63e9e7b15f381aead56b7b55073f1_Out_0, _Property_7d48a15798242f8391e7bdc870dbc12f_Out_0, _Property_f6c653a363a77d82a9b281c9064f9012_Out_0, _Property_8534d1058d4ee5898f6ca231516760e3_Out_0, _TwoPaletteExtractSubGraph_b81f9e08b316378dbf53d23988187546, _TwoPaletteExtractSubGraph_b81f9e08b316378dbf53d23988187546_OutVector4_1);
    float4 _Add_91824313bc86de859c7f70bf36533acc_Out_2;
    Unity_Add_float4(_TwoPaletteExtractSubGraph_b700307943a378878ec7fa7e72a52064_OutVector4_1, _TwoPaletteExtractSubGraph_b81f9e08b316378dbf53d23988187546_OutVector4_1, _Add_91824313bc86de859c7f70bf36533acc_Out_2);
    float4 _Add_362fdd4a03766c85989a20a7881fad94_Out_2;
    Unity_Add_float4(_Add_c457cd4750eaff8eb7ab580c762af3e5_Out_2, _Add_91824313bc86de859c7f70bf36533acc_Out_2, _Add_362fdd4a03766c85989a20a7881fad94_Out_2);
    float _Property_b5e00f60936ced8aa5f1aa04ba66da38_Out_0 = Vector1_687916C8;
    float _Property_a9a455694c97be8da66e4bf4ebec4a47_Out_0 = Vector1_B6124959;
    float4 _Preview_2da08f296b241d81a6dfc5e5c8288ffb_Out_1;
    Unity_Preview_float4(_Preview_cd6261c20854158b813081e9c9cd0c8c_Out_1, _Preview_2da08f296b241d81a6dfc5e5c8288ffb_Out_1);
    float4 _Property_f28bd1b523b846828a1f23e62a5a6226_Out_0 = Color_From_9;
    float4 _Property_3131ca163b193d8599397bb749a35f15_Out_0 = Color_From_10;
    float4 _Property_f5c2ce6bf43ee48793ad515fad941364_Out_0 = Color_To_9;
    float4 _Property_9e29edc8c5868985b9ffd46c0e96bc2c_Out_0 = Color_To_10;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_27209cc00667548f82d1e28d003e0a52;
    float4 _TwoPaletteExtractSubGraph_27209cc00667548f82d1e28d003e0a52_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_b5e00f60936ced8aa5f1aa04ba66da38_Out_0, _Property_a9a455694c97be8da66e4bf4ebec4a47_Out_0, _Preview_2da08f296b241d81a6dfc5e5c8288ffb_Out_1, _Property_f28bd1b523b846828a1f23e62a5a6226_Out_0, _Property_3131ca163b193d8599397bb749a35f15_Out_0, _Property_f5c2ce6bf43ee48793ad515fad941364_Out_0, _Property_9e29edc8c5868985b9ffd46c0e96bc2c_Out_0, _TwoPaletteExtractSubGraph_27209cc00667548f82d1e28d003e0a52, _TwoPaletteExtractSubGraph_27209cc00667548f82d1e28d003e0a52_OutVector4_1);
    float _Property_0988a06ffe0cba8e81c30723a7e7d470_Out_0 = Vector1_687916C8;
    float _Property_4884583405f6d885a000da9b5ee4186b_Out_0 = Vector1_B6124959;
    float4 _Property_bbb5609fa9286f84bac7a4579df24d84_Out_0 = Color_From_11;
    float4 _Property_552f436c7c84f787af2be7762da308de_Out_0 = Color_From_12;
    float4 _Property_985d2fdc53c02a8ab88ee34edbb6e355_Out_0 = Color_To_11;
    float4 _Property_0f7e77ffd7f1718c89b1140ae4f05e44_Out_0 = Color_To_12;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_a6af5b259fda658392ed8939fd5bcfb7;
    float4 _TwoPaletteExtractSubGraph_a6af5b259fda658392ed8939fd5bcfb7_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_0988a06ffe0cba8e81c30723a7e7d470_Out_0, _Property_4884583405f6d885a000da9b5ee4186b_Out_0, _Preview_2da08f296b241d81a6dfc5e5c8288ffb_Out_1, _Property_bbb5609fa9286f84bac7a4579df24d84_Out_0, _Property_552f436c7c84f787af2be7762da308de_Out_0, _Property_985d2fdc53c02a8ab88ee34edbb6e355_Out_0, _Property_0f7e77ffd7f1718c89b1140ae4f05e44_Out_0, _TwoPaletteExtractSubGraph_a6af5b259fda658392ed8939fd5bcfb7, _TwoPaletteExtractSubGraph_a6af5b259fda658392ed8939fd5bcfb7_OutVector4_1);
    float4 _Add_58d71335950e7183b2aa952391cb3464_Out_2;
    Unity_Add_float4(_TwoPaletteExtractSubGraph_27209cc00667548f82d1e28d003e0a52_OutVector4_1, _TwoPaletteExtractSubGraph_a6af5b259fda658392ed8939fd5bcfb7_OutVector4_1, _Add_58d71335950e7183b2aa952391cb3464_Out_2);
    float _Property_d48f12384180f585adbe6e654ecda93c_Out_0 = Vector1_687916C8;
    float _Property_95c0f2eb0585d58d910abe11f995af3c_Out_0 = Vector1_B6124959;
    float4 _Preview_a106f0de5b18b28d924fdea40eea7a80_Out_1;
    Unity_Preview_float4(_Preview_2da08f296b241d81a6dfc5e5c8288ffb_Out_1, _Preview_a106f0de5b18b28d924fdea40eea7a80_Out_1);
    float4 _Property_421e68ac8eef888c8258a132ab88799f_Out_0 = Color_From_13;
    float4 _Property_45629863027b5d8cbf614f2014425cf8_Out_0 = Color_From_14;
    float4 _Property_f2d00e438bbb8389ac9689d1ba6294de_Out_0 = Color_To_13;
    float4 _Property_ca8085263e1b5989b36803081f3ce723_Out_0 = Color_To_14;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_b9997a99d127cf86a46c28e1ec703fd5;
    float4 _TwoPaletteExtractSubGraph_b9997a99d127cf86a46c28e1ec703fd5_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_d48f12384180f585adbe6e654ecda93c_Out_0, _Property_95c0f2eb0585d58d910abe11f995af3c_Out_0, _Preview_a106f0de5b18b28d924fdea40eea7a80_Out_1, _Property_421e68ac8eef888c8258a132ab88799f_Out_0, _Property_45629863027b5d8cbf614f2014425cf8_Out_0, _Property_f2d00e438bbb8389ac9689d1ba6294de_Out_0, _Property_ca8085263e1b5989b36803081f3ce723_Out_0, _TwoPaletteExtractSubGraph_b9997a99d127cf86a46c28e1ec703fd5, _TwoPaletteExtractSubGraph_b9997a99d127cf86a46c28e1ec703fd5_OutVector4_1);
    float _Property_e4cea39a76b3d28ab1b1a581cf329388_Out_0 = Vector1_687916C8;
    float _Property_3b784a832c98048c9867bed534b13892_Out_0 = Vector1_B6124959;
    float4 _Property_ceb6486820d7528bbbde99250a0aa7d5_Out_0 = Color_From_15;
    float4 _Property_dda283044206bf808203e33e7db4f6ec_Out_0 = Color_From_16;
    float4 _Property_27bec9790509d281a1c08f19b1a7c4e3_Out_0 = Color_To_15;
    float4 _Property_dc3f101228998e848e1576584e93394d_Out_0 = Color_To_16;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_63932b5168c31f8d8777a9211cb01e16;
    float4 _TwoPaletteExtractSubGraph_63932b5168c31f8d8777a9211cb01e16_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_e4cea39a76b3d28ab1b1a581cf329388_Out_0, _Property_3b784a832c98048c9867bed534b13892_Out_0, _Preview_a106f0de5b18b28d924fdea40eea7a80_Out_1, _Property_ceb6486820d7528bbbde99250a0aa7d5_Out_0, _Property_dda283044206bf808203e33e7db4f6ec_Out_0, _Property_27bec9790509d281a1c08f19b1a7c4e3_Out_0, _Property_dc3f101228998e848e1576584e93394d_Out_0, _TwoPaletteExtractSubGraph_63932b5168c31f8d8777a9211cb01e16, _TwoPaletteExtractSubGraph_63932b5168c31f8d8777a9211cb01e16_OutVector4_1);
    float4 _Add_1e17dad8a1513d8598a55700a830ba9f_Out_2;
    Unity_Add_float4(_TwoPaletteExtractSubGraph_b9997a99d127cf86a46c28e1ec703fd5_OutVector4_1, _TwoPaletteExtractSubGraph_63932b5168c31f8d8777a9211cb01e16_OutVector4_1, _Add_1e17dad8a1513d8598a55700a830ba9f_Out_2);
    float _Property_9d8a402993260a8498d0d4cbac8d631f_Out_0 = Vector1_687916C8;
    float _Property_45c213ca1a9f0785908c69eb0f2f5b23_Out_0 = Vector1_B6124959;
    float4 _Preview_6a648177a486758bbdf8454f3822eaa9_Out_1;
    Unity_Preview_float4(_Preview_a106f0de5b18b28d924fdea40eea7a80_Out_1, _Preview_6a648177a486758bbdf8454f3822eaa9_Out_1);
    float4 _Property_66fabdb2c6e68e89a595771b9a93e59c_Out_0 = Color_From_17;
    float4 _Property_1a6b29ed18ba7b88a68421ac6f890d60_Out_0 = Color_From_18;
    float4 _Property_4c2b2924e123678c9019ce9460d838b5_Out_0 = Color_To_17;
    float4 _Property_3e32aec87b9d6181b6475cdb1e32ee09_Out_0 = Color_To_18;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_257e9b71a1645b8eb280c788b4d61be1;
    float4 _TwoPaletteExtractSubGraph_257e9b71a1645b8eb280c788b4d61be1_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_9d8a402993260a8498d0d4cbac8d631f_Out_0, _Property_45c213ca1a9f0785908c69eb0f2f5b23_Out_0, _Preview_6a648177a486758bbdf8454f3822eaa9_Out_1, _Property_66fabdb2c6e68e89a595771b9a93e59c_Out_0, _Property_1a6b29ed18ba7b88a68421ac6f890d60_Out_0, _Property_4c2b2924e123678c9019ce9460d838b5_Out_0, _Property_3e32aec87b9d6181b6475cdb1e32ee09_Out_0, _TwoPaletteExtractSubGraph_257e9b71a1645b8eb280c788b4d61be1, _TwoPaletteExtractSubGraph_257e9b71a1645b8eb280c788b4d61be1_OutVector4_1);
    float4 _Add_c8c1ba7a0d7d178eaa7d8e351235aa34_Out_2;
    Unity_Add_float4(_Add_1e17dad8a1513d8598a55700a830ba9f_Out_2, _TwoPaletteExtractSubGraph_257e9b71a1645b8eb280c788b4d61be1_OutVector4_1, _Add_c8c1ba7a0d7d178eaa7d8e351235aa34_Out_2);
    float4 _Add_165fa34ff328e48c8f8976de6baec60d_Out_2;
    Unity_Add_float4(_Add_58d71335950e7183b2aa952391cb3464_Out_2, _Add_c8c1ba7a0d7d178eaa7d8e351235aa34_Out_2, _Add_165fa34ff328e48c8f8976de6baec60d_Out_2);
    float4 _Add_05f30ff0d2e4bc859f439d225a4a9837_Out_2;
    Unity_Add_float4(_Add_362fdd4a03766c85989a20a7881fad94_Out_2, _Add_165fa34ff328e48c8f8976de6baec60d_Out_2, _Add_05f30ff0d2e4bc859f439d225a4a9837_Out_2);
    float4 _Preview_e6b769821fa2d48db79c891b6969ab87_Out_1;
    Unity_Preview_float4(_Add_05f30ff0d2e4bc859f439d225a4a9837_Out_2, _Preview_e6b769821fa2d48db79c891b6969ab87_Out_1);
    float3 _ReplaceColor_509a7494ab16438f8794efc4840550d3_Out_4;
    Unity_ReplaceColor_float((_Preview_e6b769821fa2d48db79c891b6969ab87_Out_1.xyz), IsGammaSpace() ? float3(0, 0, 0) : SRGBToLinear(float3(0, 0, 0)), IsGammaSpace() ? float3(1, 1, 1) : SRGBToLinear(float3(1, 1, 1)), 0, _ReplaceColor_509a7494ab16438f8794efc4840550d3_Out_4, 0);
    float _ColorMask_3308d3724f79438eb4233047afcfb730_Out_3;
    Unity_ColorMask_float((_Preview_e6b769821fa2d48db79c891b6969ab87_Out_1.xyz), _ReplaceColor_509a7494ab16438f8794efc4840550d3_Out_4, 0, _ColorMask_3308d3724f79438eb4233047afcfb730_Out_3, 0);
    float _Subtract_cb0b1b67049fbf8b9fd0f0ce6f62b3c7_Out_2;
    Unity_Subtract_float(_ColorMask_11413a22eef11f8293f2f5a02b650be1_Out_3, _ColorMask_3308d3724f79438eb4233047afcfb730_Out_3, _Subtract_cb0b1b67049fbf8b9fd0f0ce6f62b3c7_Out_2);
    float4 _Multiply_504f471c3296b68cb57b2822d5b9dafd_Out_2;
    Unity_Multiply_float4_float4((_Subtract_cb0b1b67049fbf8b9fd0f0ce6f62b3c7_Out_2.xxxx), _Preview_b8a060a637eb7388b2a6d60d94a9b0cf_Out_1, _Multiply_504f471c3296b68cb57b2822d5b9dafd_Out_2);
    float4 _Add_6322dd81dc54bd82ba5d39569229fbe8_Out_2;
    Unity_Add_float4(_Multiply_504f471c3296b68cb57b2822d5b9dafd_Out_2, _Preview_e6b769821fa2d48db79c891b6969ab87_Out_1, _Add_6322dd81dc54bd82ba5d39569229fbe8_Out_2);
    UnityTexture2D _Property_32b9ecde70c97b8ba88575a54fda038d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
    float4 _SampleTexture2D_07f0647075b17684b9a283a412388475_RGBA_0 = SAMPLE_TEXTURE2D(_Property_32b9ecde70c97b8ba88575a54fda038d_Out_0.tex, _Property_32b9ecde70c97b8ba88575a54fda038d_Out_0.samplerstate, _Property_32b9ecde70c97b8ba88575a54fda038d_Out_0.GetTransformedUV(IN.uv0.xy));
    float _SampleTexture2D_07f0647075b17684b9a283a412388475_R_4 = _SampleTexture2D_07f0647075b17684b9a283a412388475_RGBA_0.r;
    float _SampleTexture2D_07f0647075b17684b9a283a412388475_G_5 = _SampleTexture2D_07f0647075b17684b9a283a412388475_RGBA_0.g;
    float _SampleTexture2D_07f0647075b17684b9a283a412388475_B_6 = _SampleTexture2D_07f0647075b17684b9a283a412388475_RGBA_0.b;
    float _SampleTexture2D_07f0647075b17684b9a283a412388475_A_7 = _SampleTexture2D_07f0647075b17684b9a283a412388475_RGBA_0.a;
    float _Step_b3b149cddef7ea839a47d5786560f178_Out_2;
    Unity_Step_float(1, _SampleTexture2D_07f0647075b17684b9a283a412388475_A_7, _Step_b3b149cddef7ea839a47d5786560f178_Out_2);
    float4 _Multiply_d2cb735dfc9de089bd3701f932a23eb5_Out_2;
    Unity_Multiply_float4_float4(_Add_6322dd81dc54bd82ba5d39569229fbe8_Out_2, (_Step_b3b149cddef7ea839a47d5786560f178_Out_2.xxxx), _Multiply_d2cb735dfc9de089bd3701f932a23eb5_Out_2);
    surface.BaseColor = IsGammaSpace() ? float3(0.5, 0.5, 0.5) : SRGBToLinear(float3(0.5, 0.5, 0.5));
    surface.SpriteColor = _Multiply_d2cb735dfc9de089bd3701f932a23eb5_Out_2;
    surface.Alpha = 1;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







    output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

    return output;
}

    // --------------------------------------------------
    // Main

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "SceneSelectionPass"
    Tags
    {
        "LightMode" = "SceneSelectionPass"
    }

        // Render State
        Cull Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma exclude_renderers d3d11_9x
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        #define UNIVERSAL_USELEGACYSPRITEBLOCKS
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
    #define SCENESELECTIONPASS 1

        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
    {
         float3 positionOS : POSITION;
         float3 normalOS : NORMAL;
         float4 tangentOS : TANGENT;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
         float4 positionCS : SV_POSITION;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };
    struct SurfaceDescriptionInputs
    {
    };
    struct VertexDescriptionInputs
    {
         float3 ObjectSpaceNormal;
         float3 ObjectSpaceTangent;
         float3 ObjectSpacePosition;
    };
    struct PackedVaryings
    {
         float4 positionCS : SV_POSITION;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        ZERO_INITIALIZE(PackedVaryings, output);
        output.positionCS = input.positionCS;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }

    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }


    // --------------------------------------------------
    // Graph

    // Graph Properties
    CBUFFER_START(UnityPerMaterial)
float4 _MainTex_TexelSize;
float Vector1_687916C8;
float Vector1_B6124959;
float4 Color_From_1;
float4 Color_From_2;
float4 Color_From_3;
float4 Color_From_4;
float4 Color_From_5;
float4 Color_From_6;
float4 Color_From_7;
float4 Color_From_8;
float4 Color_From_9;
float4 Color_From_10;
float4 Color_From_11;
float4 Color_From_12;
float4 Color_From_13;
float4 Color_From_14;
float4 Color_From_15;
float4 Color_From_16;
float4 Color_From_17;
float4 Color_From_18;
float4 Color_To_1;
float4 Color_To_2;
float4 Color_To_3;
float4 Color_To_4;
float4 Color_To_5;
float4 Color_To_6;
float4 Color_To_7;
float4 Color_To_8;
float4 Color_To_9;
float4 Color_To_10;
float4 Color_To_11;
float4 Color_To_12;
float4 Color_To_13;
float4 Color_To_14;
float4 Color_To_15;
float4 Color_To_16;
float4 Color_To_17;
float4 Color_To_18;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

// Graph Includes
// GraphIncludes: <None>

// -- Property used by ScenePickingPass
#ifdef SCENEPICKINGPASS
float4 _SelectionID;
#endif

// -- Properties used by SceneSelectionPass
#ifdef SCENESELECTIONPASS
int _ObjectId;
int _PassValue;
#endif

// Graph Functions
// GraphFunctions: <None>

/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

// Graph Vertex
struct VertexDescription
{
    float3 Position;
    float3 Normal;
    float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
    VertexDescription description = (VertexDescription)0;
    description.Position = IN.ObjectSpacePosition;
    description.Normal = IN.ObjectSpaceNormal;
    description.Tangent = IN.ObjectSpaceTangent;
    return description;
}

    #ifdef FEATURES_GRAPH_VERTEX
Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
{
return output;
}
#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
#endif

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    surface.Alpha = 1;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

    return output;
}

    // --------------------------------------------------
    // Main

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "ScenePickingPass"
    Tags
    {
        "LightMode" = "Picking"
    }

        // Render State
        Cull Back

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma exclude_renderers d3d11_9x
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        #define UNIVERSAL_USELEGACYSPRITEBLOCKS
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
    #define SCENEPICKINGPASS 1

        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
    {
         float3 positionOS : POSITION;
         float3 normalOS : NORMAL;
         float4 tangentOS : TANGENT;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
         float4 positionCS : SV_POSITION;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };
    struct SurfaceDescriptionInputs
    {
    };
    struct VertexDescriptionInputs
    {
         float3 ObjectSpaceNormal;
         float3 ObjectSpaceTangent;
         float3 ObjectSpacePosition;
    };
    struct PackedVaryings
    {
         float4 positionCS : SV_POSITION;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        ZERO_INITIALIZE(PackedVaryings, output);
        output.positionCS = input.positionCS;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }

    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }


    // --------------------------------------------------
    // Graph

    // Graph Properties
    CBUFFER_START(UnityPerMaterial)
float4 _MainTex_TexelSize;
float Vector1_687916C8;
float Vector1_B6124959;
float4 Color_From_1;
float4 Color_From_2;
float4 Color_From_3;
float4 Color_From_4;
float4 Color_From_5;
float4 Color_From_6;
float4 Color_From_7;
float4 Color_From_8;
float4 Color_From_9;
float4 Color_From_10;
float4 Color_From_11;
float4 Color_From_12;
float4 Color_From_13;
float4 Color_From_14;
float4 Color_From_15;
float4 Color_From_16;
float4 Color_From_17;
float4 Color_From_18;
float4 Color_To_1;
float4 Color_To_2;
float4 Color_To_3;
float4 Color_To_4;
float4 Color_To_5;
float4 Color_To_6;
float4 Color_To_7;
float4 Color_To_8;
float4 Color_To_9;
float4 Color_To_10;
float4 Color_To_11;
float4 Color_To_12;
float4 Color_To_13;
float4 Color_To_14;
float4 Color_To_15;
float4 Color_To_16;
float4 Color_To_17;
float4 Color_To_18;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

// Graph Includes
// GraphIncludes: <None>

// -- Property used by ScenePickingPass
#ifdef SCENEPICKINGPASS
float4 _SelectionID;
#endif

// -- Properties used by SceneSelectionPass
#ifdef SCENESELECTIONPASS
int _ObjectId;
int _PassValue;
#endif

// Graph Functions
// GraphFunctions: <None>

/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

// Graph Vertex
struct VertexDescription
{
    float3 Position;
    float3 Normal;
    float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
    VertexDescription description = (VertexDescription)0;
    description.Position = IN.ObjectSpacePosition;
    description.Normal = IN.ObjectSpaceNormal;
    description.Tangent = IN.ObjectSpaceTangent;
    return description;
}

    #ifdef FEATURES_GRAPH_VERTEX
Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
{
return output;
}
#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
#endif

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    surface.Alpha = 1;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

    return output;
}

    // --------------------------------------------------
    // Main

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "Sprite Unlit"
    Tags
    {
        "LightMode" = "UniversalForward"
    }

        // Render State
        Cull Off
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma exclude_renderers d3d11_9x
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        #define UNIVERSAL_USELEGACYSPRITEBLOCKS
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
    {
         float3 positionOS : POSITION;
         float3 normalOS : NORMAL;
         float4 tangentOS : TANGENT;
         float4 uv0 : TEXCOORD0;
         float4 color : COLOR;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
         float4 positionCS : SV_POSITION;
         float3 positionWS;
         float4 texCoord0;
         float4 color;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };
    struct SurfaceDescriptionInputs
    {
         float4 uv0;
    };
    struct VertexDescriptionInputs
    {
         float3 ObjectSpaceNormal;
         float3 ObjectSpaceTangent;
         float3 ObjectSpacePosition;
    };
    struct PackedVaryings
    {
         float4 positionCS : SV_POSITION;
         float3 interp0 : INTERP0;
         float4 interp1 : INTERP1;
         float4 interp2 : INTERP2;
        #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        ZERO_INITIALIZE(PackedVaryings, output);
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyzw = input.texCoord0;
        output.interp2.xyzw = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }

    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.texCoord0 = input.interp1.xyzw;
        output.color = input.interp2.xyzw;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }


    // --------------------------------------------------
    // Graph

    // Graph Properties
    CBUFFER_START(UnityPerMaterial)
float4 _MainTex_TexelSize;
float Vector1_687916C8;
float Vector1_B6124959;
float4 Color_From_1;
float4 Color_From_2;
float4 Color_From_3;
float4 Color_From_4;
float4 Color_From_5;
float4 Color_From_6;
float4 Color_From_7;
float4 Color_From_8;
float4 Color_From_9;
float4 Color_From_10;
float4 Color_From_11;
float4 Color_From_12;
float4 Color_From_13;
float4 Color_From_14;
float4 Color_From_15;
float4 Color_From_16;
float4 Color_From_17;
float4 Color_From_18;
float4 Color_To_1;
float4 Color_To_2;
float4 Color_To_3;
float4 Color_To_4;
float4 Color_To_5;
float4 Color_To_6;
float4 Color_To_7;
float4 Color_To_8;
float4 Color_To_9;
float4 Color_To_10;
float4 Color_To_11;
float4 Color_To_12;
float4 Color_To_13;
float4 Color_To_14;
float4 Color_To_15;
float4 Color_To_16;
float4 Color_To_17;
float4 Color_To_18;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

// Graph Includes
// GraphIncludes: <None>

// -- Property used by ScenePickingPass
#ifdef SCENEPICKINGPASS
float4 _SelectionID;
#endif

// -- Properties used by SceneSelectionPass
#ifdef SCENESELECTIONPASS
int _ObjectId;
int _PassValue;
#endif

// Graph Functions

void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Preview_float4(float4 In, out float4 Out)
{
    Out = In;
}

void Unity_ColorMask_float(float3 In, float3 MaskColor, float Range, out float Out, float Fuzziness)
{
    float Distance = distance(MaskColor, In);
    Out = saturate(1 - (Distance - Range) / max(Fuzziness, 1e-5));
}

void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
{
    RGBA = float4(R, G, B, A);
    RGB = float3(R, G, B);
    RG = float2(R, G);
}

void Unity_Comparison_Equal_float(float A, float B, out float Out)
{
    Out = A == B ? 1 : 0;
}

void Unity_And_float(float A, float B, out float Out)
{
    Out = A && B;
}

void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
{
    Out = Predicate ? True : False;
}

struct Bindings_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float
{
};

void SG_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float(float4 Vector4_6DC11782, float4 Color_B32C2AE9, Bindings_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float IN, out float4 New1_1)
{
float4 _Property_2a2bcf87adbee28cb6c31451f3fda7b7_Out_0 = Color_B32C2AE9;
float _Split_55cf51d605d3c9898706de9808608534_R_1 = _Property_2a2bcf87adbee28cb6c31451f3fda7b7_Out_0[0];
float _Split_55cf51d605d3c9898706de9808608534_G_2 = _Property_2a2bcf87adbee28cb6c31451f3fda7b7_Out_0[1];
float _Split_55cf51d605d3c9898706de9808608534_B_3 = _Property_2a2bcf87adbee28cb6c31451f3fda7b7_Out_0[2];
float _Split_55cf51d605d3c9898706de9808608534_A_4 = _Property_2a2bcf87adbee28cb6c31451f3fda7b7_Out_0[3];
float4 _Combine_32da8360502e7a8e8a28901aebd31c3d_RGBA_4;
float3 _Combine_32da8360502e7a8e8a28901aebd31c3d_RGB_5;
float2 _Combine_32da8360502e7a8e8a28901aebd31c3d_RG_6;
Unity_Combine_float(_Split_55cf51d605d3c9898706de9808608534_R_1, _Split_55cf51d605d3c9898706de9808608534_G_2, _Split_55cf51d605d3c9898706de9808608534_B_3, 0, _Combine_32da8360502e7a8e8a28901aebd31c3d_RGBA_4, _Combine_32da8360502e7a8e8a28901aebd31c3d_RGB_5, _Combine_32da8360502e7a8e8a28901aebd31c3d_RG_6);
float _Comparison_611e61c407cae982984349b09d40a3ec_Out_2;
Unity_Comparison_Equal_float((_Combine_32da8360502e7a8e8a28901aebd31c3d_RGB_5).x, 0, _Comparison_611e61c407cae982984349b09d40a3ec_Out_2);
float _Comparison_49244d8ede2e11828a2d144434ba5b7f_Out_2;
Unity_Comparison_Equal_float(_Split_55cf51d605d3c9898706de9808608534_A_4, 0, _Comparison_49244d8ede2e11828a2d144434ba5b7f_Out_2);
float _And_5af41e8222280585bd75f616763c135f_Out_2;
Unity_And_float(_Comparison_611e61c407cae982984349b09d40a3ec_Out_2, _Comparison_49244d8ede2e11828a2d144434ba5b7f_Out_2, _And_5af41e8222280585bd75f616763c135f_Out_2);
float4 _Property_c41cdbd8ca5d6d8f9f6175805e60f684_Out_0 = Vector4_6DC11782;
float4 _Branch_558ba3f0fc26c186a66dfa020d7a303c_Out_3;
Unity_Branch_float4(_And_5af41e8222280585bd75f616763c135f_Out_2, float4(0, 0, 0, 0), _Property_c41cdbd8ca5d6d8f9f6175805e60f684_Out_0, _Branch_558ba3f0fc26c186a66dfa020d7a303c_Out_3);
New1_1 = _Branch_558ba3f0fc26c186a66dfa020d7a303c_Out_3;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
}

struct Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float
{
};

void SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(float Vector1_960086FD, float Vector1_7191537E, float4 Vector4_41053054, float4 Vector4_D6A9DA6C, float4 Vector4_1AECF0B2, float4 Vector4_373860C, float4 Vector4_5E07B3C1, Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float IN, out float4 OutVector4_1)
{
float4 _Property_db185fa9ab7b8b8fba026406c5bc5307_Out_0 = Vector4_41053054;
float4 _Preview_e710012ce5fe4b8cae6bbd295c750bc5_Out_1;
Unity_Preview_float4(_Property_db185fa9ab7b8b8fba026406c5bc5307_Out_0, _Preview_e710012ce5fe4b8cae6bbd295c750bc5_Out_1);
float4 _Property_0b9fe9c1987d408887931a9eb4dc2f55_Out_0 = Vector4_D6A9DA6C;
float _Property_45cba9cb23886480af47064f7c23ad4c_Out_0 = Vector1_7191537E;
float _Property_cc8f4c56a331ac8086dac11e6e0b4fb5_Out_0 = Vector1_960086FD;
float _ColorMask_c7b5065fffe28283b143fcad436ebbfe_Out_3;
Unity_ColorMask_float((_Preview_e710012ce5fe4b8cae6bbd295c750bc5_Out_1.xyz), (_Property_0b9fe9c1987d408887931a9eb4dc2f55_Out_0.xyz), _Property_45cba9cb23886480af47064f7c23ad4c_Out_0, _ColorMask_c7b5065fffe28283b143fcad436ebbfe_Out_3, _Property_cc8f4c56a331ac8086dac11e6e0b4fb5_Out_0);
float4 _Property_1ca134b78c7607838681a3a7c485ffa2_Out_0 = Vector4_D6A9DA6C;
Bindings_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float _IsAlphaSubShader_4d692f9636c9e58a85441f1a47593dc8;
float4 _IsAlphaSubShader_4d692f9636c9e58a85441f1a47593dc8_New1_1;
SG_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float((_ColorMask_c7b5065fffe28283b143fcad436ebbfe_Out_3.xxxx), _Property_1ca134b78c7607838681a3a7c485ffa2_Out_0, _IsAlphaSubShader_4d692f9636c9e58a85441f1a47593dc8, _IsAlphaSubShader_4d692f9636c9e58a85441f1a47593dc8_New1_1);
float4 _Property_fd1c7bbed3a17481bcdf97842520e892_Out_0 = Vector4_373860C;
float4 _Multiply_6bd1895176b8dc8d918899d66f9b4fd2_Out_2;
Unity_Multiply_float4_float4(_IsAlphaSubShader_4d692f9636c9e58a85441f1a47593dc8_New1_1, _Property_fd1c7bbed3a17481bcdf97842520e892_Out_0, _Multiply_6bd1895176b8dc8d918899d66f9b4fd2_Out_2);
float4 _Property_fcae524f2531908b8b82532d1b3ab74c_Out_0 = Vector4_1AECF0B2;
float _Property_2f056a3ca6c3a68883dc76c3b3aa9b81_Out_0 = Vector1_7191537E;
float _Property_5a6ef500448fe987bc2109096edeb1a4_Out_0 = Vector1_960086FD;
float _ColorMask_9650a41252fca38f9439f721c9b0208c_Out_3;
Unity_ColorMask_float((_Preview_e710012ce5fe4b8cae6bbd295c750bc5_Out_1.xyz), (_Property_fcae524f2531908b8b82532d1b3ab74c_Out_0.xyz), _Property_2f056a3ca6c3a68883dc76c3b3aa9b81_Out_0, _ColorMask_9650a41252fca38f9439f721c9b0208c_Out_3, _Property_5a6ef500448fe987bc2109096edeb1a4_Out_0);
float4 _Property_c12209c2ca1ac68193e6fd33eff67b36_Out_0 = Vector4_1AECF0B2;
Bindings_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float _IsAlphaSubShader_1bccb36102724e83b69f43bc85fd268e;
float4 _IsAlphaSubShader_1bccb36102724e83b69f43bc85fd268e_New1_1;
SG_IsAlphaSubShader_f1081fbfef11aa34987bf4affb33a859_float((_ColorMask_9650a41252fca38f9439f721c9b0208c_Out_3.xxxx), _Property_c12209c2ca1ac68193e6fd33eff67b36_Out_0, _IsAlphaSubShader_1bccb36102724e83b69f43bc85fd268e, _IsAlphaSubShader_1bccb36102724e83b69f43bc85fd268e_New1_1);
float4 _Property_7980f2a99d65dc87bb7c0b7e5a2e4b7b_Out_0 = Vector4_5E07B3C1;
float4 _Multiply_e1b670f83d54b483adc61f2b830d50a2_Out_2;
Unity_Multiply_float4_float4(_IsAlphaSubShader_1bccb36102724e83b69f43bc85fd268e_New1_1, _Property_7980f2a99d65dc87bb7c0b7e5a2e4b7b_Out_0, _Multiply_e1b670f83d54b483adc61f2b830d50a2_Out_2);
float4 _Add_55eeca799efa028bb9f0ae4ef7bec55e_Out_2;
Unity_Add_float4(_Multiply_6bd1895176b8dc8d918899d66f9b4fd2_Out_2, _Multiply_e1b670f83d54b483adc61f2b830d50a2_Out_2, _Add_55eeca799efa028bb9f0ae4ef7bec55e_Out_2);
OutVector4_1 = _Add_55eeca799efa028bb9f0ae4ef7bec55e_Out_2;
}

void Unity_ReplaceColor_float(float3 In, float3 From, float3 To, float Range, out float3 Out, float Fuzziness)
{
    float Distance = distance(From, In);
    Out = lerp(To, In, saturate((Distance - Range) / max(Fuzziness, 1e-5f)));
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Step_float(float Edge, float In, out float Out)
{
    Out = step(Edge, In);
}

/* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

// Graph Vertex
struct VertexDescription
{
    float3 Position;
    float3 Normal;
    float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
    VertexDescription description = (VertexDescription)0;
    description.Position = IN.ObjectSpacePosition;
    description.Normal = IN.ObjectSpaceNormal;
    description.Tangent = IN.ObjectSpaceTangent;
    return description;
}

    #ifdef FEATURES_GRAPH_VERTEX
Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
{
return output;
}
#define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
#endif

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float4 SpriteColor;
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_2fd764fe4f4d3c8c8e3a830c6bd3004b_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
    float4 _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2fd764fe4f4d3c8c8e3a830c6bd3004b_Out_0.tex, _Property_2fd764fe4f4d3c8c8e3a830c6bd3004b_Out_0.samplerstate, _Property_2fd764fe4f4d3c8c8e3a830c6bd3004b_Out_0.GetTransformedUV(IN.uv0.xy));
    float _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_R_4 = _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0.r;
    float _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_G_5 = _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0.g;
    float _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_B_6 = _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0.b;
    float _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_A_7 = _SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0.a;
    float4 _Multiply_c561897b48412d86b5cda6ff92ab408f_Out_2;
    Unity_Multiply_float4_float4(_SampleTexture2D_e9469e1edd5b2789b83950253df39a1f_RGBA_0, float4(1, 1, 1, 1), _Multiply_c561897b48412d86b5cda6ff92ab408f_Out_2);
    float4 _Preview_b8a060a637eb7388b2a6d60d94a9b0cf_Out_1;
    Unity_Preview_float4(_Multiply_c561897b48412d86b5cda6ff92ab408f_Out_2, _Preview_b8a060a637eb7388b2a6d60d94a9b0cf_Out_1);
    float _ColorMask_11413a22eef11f8293f2f5a02b650be1_Out_3;
    Unity_ColorMask_float((_Preview_b8a060a637eb7388b2a6d60d94a9b0cf_Out_1.xyz), (_Preview_b8a060a637eb7388b2a6d60d94a9b0cf_Out_1.xyz), 0, _ColorMask_11413a22eef11f8293f2f5a02b650be1_Out_3, 0);
    float _Property_ea45658fefc23087864dc66f4fe7703d_Out_0 = Vector1_687916C8;
    float _Property_c785c8d3f190448583588345777b583e_Out_0 = Vector1_B6124959;
    float4 _Property_97926f8b9337bf8ea7fbe70dadfee01e_Out_0 = Color_From_1;
    float4 _Property_41dcd2a7b7f8ef84ad69e10acd8844dc_Out_0 = Color_From_2;
    float4 _Property_a56713464c91d18b898c9c5165ba9b8b_Out_0 = Color_To_1;
    float4 _Property_a89e419778e3db86b69c4e84dd8eeaf4_Out_0 = Color_To_2;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_099a84cc2ad0798c8bce7a763f5108f6;
    float4 _TwoPaletteExtractSubGraph_099a84cc2ad0798c8bce7a763f5108f6_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_ea45658fefc23087864dc66f4fe7703d_Out_0, _Property_c785c8d3f190448583588345777b583e_Out_0, _Multiply_c561897b48412d86b5cda6ff92ab408f_Out_2, _Property_97926f8b9337bf8ea7fbe70dadfee01e_Out_0, _Property_41dcd2a7b7f8ef84ad69e10acd8844dc_Out_0, _Property_a56713464c91d18b898c9c5165ba9b8b_Out_0, _Property_a89e419778e3db86b69c4e84dd8eeaf4_Out_0, _TwoPaletteExtractSubGraph_099a84cc2ad0798c8bce7a763f5108f6, _TwoPaletteExtractSubGraph_099a84cc2ad0798c8bce7a763f5108f6_OutVector4_1);
    float _Property_8391d69232e6a48f87150c3de729f119_Out_0 = Vector1_687916C8;
    float _Property_0001ebfc78ac518da115d1de9d1b195e_Out_0 = Vector1_B6124959;
    float4 _Preview_e478343349aef686a521e0245bbec6b8_Out_1;
    Unity_Preview_float4(_Multiply_c561897b48412d86b5cda6ff92ab408f_Out_2, _Preview_e478343349aef686a521e0245bbec6b8_Out_1);
    float4 _Property_752d827a08d2ef838ebca1871584db0d_Out_0 = Color_From_3;
    float4 _Property_9801843bab1e1183b98421b3fc1f8e39_Out_0 = Color_From_4;
    float4 _Property_b485f73fd05c0c8e8310bebc433e7f85_Out_0 = Color_To_3;
    float4 _Property_0c6f652d627da78aa325a866e48be3e7_Out_0 = Color_To_4;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_4eae0bd9a8ac6a8ca6b7ab3170b5bb3c;
    float4 _TwoPaletteExtractSubGraph_4eae0bd9a8ac6a8ca6b7ab3170b5bb3c_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_8391d69232e6a48f87150c3de729f119_Out_0, _Property_0001ebfc78ac518da115d1de9d1b195e_Out_0, _Preview_e478343349aef686a521e0245bbec6b8_Out_1, _Property_752d827a08d2ef838ebca1871584db0d_Out_0, _Property_9801843bab1e1183b98421b3fc1f8e39_Out_0, _Property_b485f73fd05c0c8e8310bebc433e7f85_Out_0, _Property_0c6f652d627da78aa325a866e48be3e7_Out_0, _TwoPaletteExtractSubGraph_4eae0bd9a8ac6a8ca6b7ab3170b5bb3c, _TwoPaletteExtractSubGraph_4eae0bd9a8ac6a8ca6b7ab3170b5bb3c_OutVector4_1);
    float4 _Add_c457cd4750eaff8eb7ab580c762af3e5_Out_2;
    Unity_Add_float4(_TwoPaletteExtractSubGraph_099a84cc2ad0798c8bce7a763f5108f6_OutVector4_1, _TwoPaletteExtractSubGraph_4eae0bd9a8ac6a8ca6b7ab3170b5bb3c_OutVector4_1, _Add_c457cd4750eaff8eb7ab580c762af3e5_Out_2);
    float _Property_0b8337991239488187db38938997b20e_Out_0 = Vector1_687916C8;
    float _Property_98ac839524317b828042576f7edc9628_Out_0 = Vector1_B6124959;
    float4 _Preview_cd6261c20854158b813081e9c9cd0c8c_Out_1;
    Unity_Preview_float4(_Preview_e478343349aef686a521e0245bbec6b8_Out_1, _Preview_cd6261c20854158b813081e9c9cd0c8c_Out_1);
    float4 _Property_bcd638ec9489df8783d4c4cf1882e6c7_Out_0 = Color_From_5;
    float4 _Property_a43e3c04949ac48ca2e984b89f81adf2_Out_0 = Color_From_6;
    float4 _Property_f89f7dfc97f42480a2cd3439aedf3c76_Out_0 = Color_To_5;
    float4 _Property_b1eabf779118d4848a79fbee43e89439_Out_0 = Color_To_6;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_b700307943a378878ec7fa7e72a52064;
    float4 _TwoPaletteExtractSubGraph_b700307943a378878ec7fa7e72a52064_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_0b8337991239488187db38938997b20e_Out_0, _Property_98ac839524317b828042576f7edc9628_Out_0, _Preview_cd6261c20854158b813081e9c9cd0c8c_Out_1, _Property_bcd638ec9489df8783d4c4cf1882e6c7_Out_0, _Property_a43e3c04949ac48ca2e984b89f81adf2_Out_0, _Property_f89f7dfc97f42480a2cd3439aedf3c76_Out_0, _Property_b1eabf779118d4848a79fbee43e89439_Out_0, _TwoPaletteExtractSubGraph_b700307943a378878ec7fa7e72a52064, _TwoPaletteExtractSubGraph_b700307943a378878ec7fa7e72a52064_OutVector4_1);
    float _Property_35f233c7a5dbc48ab9bddac95eb3d495_Out_0 = Vector1_687916C8;
    float _Property_10ad05481f462786a2aee961dd253d79_Out_0 = Vector1_B6124959;
    float4 _Property_7fb63e9e7b15f381aead56b7b55073f1_Out_0 = Color_From_7;
    float4 _Property_7d48a15798242f8391e7bdc870dbc12f_Out_0 = Color_From_8;
    float4 _Property_f6c653a363a77d82a9b281c9064f9012_Out_0 = Color_To_7;
    float4 _Property_8534d1058d4ee5898f6ca231516760e3_Out_0 = Color_To_8;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_b81f9e08b316378dbf53d23988187546;
    float4 _TwoPaletteExtractSubGraph_b81f9e08b316378dbf53d23988187546_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_35f233c7a5dbc48ab9bddac95eb3d495_Out_0, _Property_10ad05481f462786a2aee961dd253d79_Out_0, _Preview_cd6261c20854158b813081e9c9cd0c8c_Out_1, _Property_7fb63e9e7b15f381aead56b7b55073f1_Out_0, _Property_7d48a15798242f8391e7bdc870dbc12f_Out_0, _Property_f6c653a363a77d82a9b281c9064f9012_Out_0, _Property_8534d1058d4ee5898f6ca231516760e3_Out_0, _TwoPaletteExtractSubGraph_b81f9e08b316378dbf53d23988187546, _TwoPaletteExtractSubGraph_b81f9e08b316378dbf53d23988187546_OutVector4_1);
    float4 _Add_91824313bc86de859c7f70bf36533acc_Out_2;
    Unity_Add_float4(_TwoPaletteExtractSubGraph_b700307943a378878ec7fa7e72a52064_OutVector4_1, _TwoPaletteExtractSubGraph_b81f9e08b316378dbf53d23988187546_OutVector4_1, _Add_91824313bc86de859c7f70bf36533acc_Out_2);
    float4 _Add_362fdd4a03766c85989a20a7881fad94_Out_2;
    Unity_Add_float4(_Add_c457cd4750eaff8eb7ab580c762af3e5_Out_2, _Add_91824313bc86de859c7f70bf36533acc_Out_2, _Add_362fdd4a03766c85989a20a7881fad94_Out_2);
    float _Property_b5e00f60936ced8aa5f1aa04ba66da38_Out_0 = Vector1_687916C8;
    float _Property_a9a455694c97be8da66e4bf4ebec4a47_Out_0 = Vector1_B6124959;
    float4 _Preview_2da08f296b241d81a6dfc5e5c8288ffb_Out_1;
    Unity_Preview_float4(_Preview_cd6261c20854158b813081e9c9cd0c8c_Out_1, _Preview_2da08f296b241d81a6dfc5e5c8288ffb_Out_1);
    float4 _Property_f28bd1b523b846828a1f23e62a5a6226_Out_0 = Color_From_9;
    float4 _Property_3131ca163b193d8599397bb749a35f15_Out_0 = Color_From_10;
    float4 _Property_f5c2ce6bf43ee48793ad515fad941364_Out_0 = Color_To_9;
    float4 _Property_9e29edc8c5868985b9ffd46c0e96bc2c_Out_0 = Color_To_10;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_27209cc00667548f82d1e28d003e0a52;
    float4 _TwoPaletteExtractSubGraph_27209cc00667548f82d1e28d003e0a52_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_b5e00f60936ced8aa5f1aa04ba66da38_Out_0, _Property_a9a455694c97be8da66e4bf4ebec4a47_Out_0, _Preview_2da08f296b241d81a6dfc5e5c8288ffb_Out_1, _Property_f28bd1b523b846828a1f23e62a5a6226_Out_0, _Property_3131ca163b193d8599397bb749a35f15_Out_0, _Property_f5c2ce6bf43ee48793ad515fad941364_Out_0, _Property_9e29edc8c5868985b9ffd46c0e96bc2c_Out_0, _TwoPaletteExtractSubGraph_27209cc00667548f82d1e28d003e0a52, _TwoPaletteExtractSubGraph_27209cc00667548f82d1e28d003e0a52_OutVector4_1);
    float _Property_0988a06ffe0cba8e81c30723a7e7d470_Out_0 = Vector1_687916C8;
    float _Property_4884583405f6d885a000da9b5ee4186b_Out_0 = Vector1_B6124959;
    float4 _Property_bbb5609fa9286f84bac7a4579df24d84_Out_0 = Color_From_11;
    float4 _Property_552f436c7c84f787af2be7762da308de_Out_0 = Color_From_12;
    float4 _Property_985d2fdc53c02a8ab88ee34edbb6e355_Out_0 = Color_To_11;
    float4 _Property_0f7e77ffd7f1718c89b1140ae4f05e44_Out_0 = Color_To_12;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_a6af5b259fda658392ed8939fd5bcfb7;
    float4 _TwoPaletteExtractSubGraph_a6af5b259fda658392ed8939fd5bcfb7_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_0988a06ffe0cba8e81c30723a7e7d470_Out_0, _Property_4884583405f6d885a000da9b5ee4186b_Out_0, _Preview_2da08f296b241d81a6dfc5e5c8288ffb_Out_1, _Property_bbb5609fa9286f84bac7a4579df24d84_Out_0, _Property_552f436c7c84f787af2be7762da308de_Out_0, _Property_985d2fdc53c02a8ab88ee34edbb6e355_Out_0, _Property_0f7e77ffd7f1718c89b1140ae4f05e44_Out_0, _TwoPaletteExtractSubGraph_a6af5b259fda658392ed8939fd5bcfb7, _TwoPaletteExtractSubGraph_a6af5b259fda658392ed8939fd5bcfb7_OutVector4_1);
    float4 _Add_58d71335950e7183b2aa952391cb3464_Out_2;
    Unity_Add_float4(_TwoPaletteExtractSubGraph_27209cc00667548f82d1e28d003e0a52_OutVector4_1, _TwoPaletteExtractSubGraph_a6af5b259fda658392ed8939fd5bcfb7_OutVector4_1, _Add_58d71335950e7183b2aa952391cb3464_Out_2);
    float _Property_d48f12384180f585adbe6e654ecda93c_Out_0 = Vector1_687916C8;
    float _Property_95c0f2eb0585d58d910abe11f995af3c_Out_0 = Vector1_B6124959;
    float4 _Preview_a106f0de5b18b28d924fdea40eea7a80_Out_1;
    Unity_Preview_float4(_Preview_2da08f296b241d81a6dfc5e5c8288ffb_Out_1, _Preview_a106f0de5b18b28d924fdea40eea7a80_Out_1);
    float4 _Property_421e68ac8eef888c8258a132ab88799f_Out_0 = Color_From_13;
    float4 _Property_45629863027b5d8cbf614f2014425cf8_Out_0 = Color_From_14;
    float4 _Property_f2d00e438bbb8389ac9689d1ba6294de_Out_0 = Color_To_13;
    float4 _Property_ca8085263e1b5989b36803081f3ce723_Out_0 = Color_To_14;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_b9997a99d127cf86a46c28e1ec703fd5;
    float4 _TwoPaletteExtractSubGraph_b9997a99d127cf86a46c28e1ec703fd5_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_d48f12384180f585adbe6e654ecda93c_Out_0, _Property_95c0f2eb0585d58d910abe11f995af3c_Out_0, _Preview_a106f0de5b18b28d924fdea40eea7a80_Out_1, _Property_421e68ac8eef888c8258a132ab88799f_Out_0, _Property_45629863027b5d8cbf614f2014425cf8_Out_0, _Property_f2d00e438bbb8389ac9689d1ba6294de_Out_0, _Property_ca8085263e1b5989b36803081f3ce723_Out_0, _TwoPaletteExtractSubGraph_b9997a99d127cf86a46c28e1ec703fd5, _TwoPaletteExtractSubGraph_b9997a99d127cf86a46c28e1ec703fd5_OutVector4_1);
    float _Property_e4cea39a76b3d28ab1b1a581cf329388_Out_0 = Vector1_687916C8;
    float _Property_3b784a832c98048c9867bed534b13892_Out_0 = Vector1_B6124959;
    float4 _Property_ceb6486820d7528bbbde99250a0aa7d5_Out_0 = Color_From_15;
    float4 _Property_dda283044206bf808203e33e7db4f6ec_Out_0 = Color_From_16;
    float4 _Property_27bec9790509d281a1c08f19b1a7c4e3_Out_0 = Color_To_15;
    float4 _Property_dc3f101228998e848e1576584e93394d_Out_0 = Color_To_16;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_63932b5168c31f8d8777a9211cb01e16;
    float4 _TwoPaletteExtractSubGraph_63932b5168c31f8d8777a9211cb01e16_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_e4cea39a76b3d28ab1b1a581cf329388_Out_0, _Property_3b784a832c98048c9867bed534b13892_Out_0, _Preview_a106f0de5b18b28d924fdea40eea7a80_Out_1, _Property_ceb6486820d7528bbbde99250a0aa7d5_Out_0, _Property_dda283044206bf808203e33e7db4f6ec_Out_0, _Property_27bec9790509d281a1c08f19b1a7c4e3_Out_0, _Property_dc3f101228998e848e1576584e93394d_Out_0, _TwoPaletteExtractSubGraph_63932b5168c31f8d8777a9211cb01e16, _TwoPaletteExtractSubGraph_63932b5168c31f8d8777a9211cb01e16_OutVector4_1);
    float4 _Add_1e17dad8a1513d8598a55700a830ba9f_Out_2;
    Unity_Add_float4(_TwoPaletteExtractSubGraph_b9997a99d127cf86a46c28e1ec703fd5_OutVector4_1, _TwoPaletteExtractSubGraph_63932b5168c31f8d8777a9211cb01e16_OutVector4_1, _Add_1e17dad8a1513d8598a55700a830ba9f_Out_2);
    float _Property_9d8a402993260a8498d0d4cbac8d631f_Out_0 = Vector1_687916C8;
    float _Property_45c213ca1a9f0785908c69eb0f2f5b23_Out_0 = Vector1_B6124959;
    float4 _Preview_6a648177a486758bbdf8454f3822eaa9_Out_1;
    Unity_Preview_float4(_Preview_a106f0de5b18b28d924fdea40eea7a80_Out_1, _Preview_6a648177a486758bbdf8454f3822eaa9_Out_1);
    float4 _Property_66fabdb2c6e68e89a595771b9a93e59c_Out_0 = Color_From_17;
    float4 _Property_1a6b29ed18ba7b88a68421ac6f890d60_Out_0 = Color_From_18;
    float4 _Property_4c2b2924e123678c9019ce9460d838b5_Out_0 = Color_To_17;
    float4 _Property_3e32aec87b9d6181b6475cdb1e32ee09_Out_0 = Color_To_18;
    Bindings_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float _TwoPaletteExtractSubGraph_257e9b71a1645b8eb280c788b4d61be1;
    float4 _TwoPaletteExtractSubGraph_257e9b71a1645b8eb280c788b4d61be1_OutVector4_1;
    SG_TwoPaletteExtractSubGraph_c494337ad85c01a409fffe7289700e85_float(_Property_9d8a402993260a8498d0d4cbac8d631f_Out_0, _Property_45c213ca1a9f0785908c69eb0f2f5b23_Out_0, _Preview_6a648177a486758bbdf8454f3822eaa9_Out_1, _Property_66fabdb2c6e68e89a595771b9a93e59c_Out_0, _Property_1a6b29ed18ba7b88a68421ac6f890d60_Out_0, _Property_4c2b2924e123678c9019ce9460d838b5_Out_0, _Property_3e32aec87b9d6181b6475cdb1e32ee09_Out_0, _TwoPaletteExtractSubGraph_257e9b71a1645b8eb280c788b4d61be1, _TwoPaletteExtractSubGraph_257e9b71a1645b8eb280c788b4d61be1_OutVector4_1);
    float4 _Add_c8c1ba7a0d7d178eaa7d8e351235aa34_Out_2;
    Unity_Add_float4(_Add_1e17dad8a1513d8598a55700a830ba9f_Out_2, _TwoPaletteExtractSubGraph_257e9b71a1645b8eb280c788b4d61be1_OutVector4_1, _Add_c8c1ba7a0d7d178eaa7d8e351235aa34_Out_2);
    float4 _Add_165fa34ff328e48c8f8976de6baec60d_Out_2;
    Unity_Add_float4(_Add_58d71335950e7183b2aa952391cb3464_Out_2, _Add_c8c1ba7a0d7d178eaa7d8e351235aa34_Out_2, _Add_165fa34ff328e48c8f8976de6baec60d_Out_2);
    float4 _Add_05f30ff0d2e4bc859f439d225a4a9837_Out_2;
    Unity_Add_float4(_Add_362fdd4a03766c85989a20a7881fad94_Out_2, _Add_165fa34ff328e48c8f8976de6baec60d_Out_2, _Add_05f30ff0d2e4bc859f439d225a4a9837_Out_2);
    float4 _Preview_e6b769821fa2d48db79c891b6969ab87_Out_1;
    Unity_Preview_float4(_Add_05f30ff0d2e4bc859f439d225a4a9837_Out_2, _Preview_e6b769821fa2d48db79c891b6969ab87_Out_1);
    float3 _ReplaceColor_509a7494ab16438f8794efc4840550d3_Out_4;
    Unity_ReplaceColor_float((_Preview_e6b769821fa2d48db79c891b6969ab87_Out_1.xyz), IsGammaSpace() ? float3(0, 0, 0) : SRGBToLinear(float3(0, 0, 0)), IsGammaSpace() ? float3(1, 1, 1) : SRGBToLinear(float3(1, 1, 1)), 0, _ReplaceColor_509a7494ab16438f8794efc4840550d3_Out_4, 0);
    float _ColorMask_3308d3724f79438eb4233047afcfb730_Out_3;
    Unity_ColorMask_float((_Preview_e6b769821fa2d48db79c891b6969ab87_Out_1.xyz), _ReplaceColor_509a7494ab16438f8794efc4840550d3_Out_4, 0, _ColorMask_3308d3724f79438eb4233047afcfb730_Out_3, 0);
    float _Subtract_cb0b1b67049fbf8b9fd0f0ce6f62b3c7_Out_2;
    Unity_Subtract_float(_ColorMask_11413a22eef11f8293f2f5a02b650be1_Out_3, _ColorMask_3308d3724f79438eb4233047afcfb730_Out_3, _Subtract_cb0b1b67049fbf8b9fd0f0ce6f62b3c7_Out_2);
    float4 _Multiply_504f471c3296b68cb57b2822d5b9dafd_Out_2;
    Unity_Multiply_float4_float4((_Subtract_cb0b1b67049fbf8b9fd0f0ce6f62b3c7_Out_2.xxxx), _Preview_b8a060a637eb7388b2a6d60d94a9b0cf_Out_1, _Multiply_504f471c3296b68cb57b2822d5b9dafd_Out_2);
    float4 _Add_6322dd81dc54bd82ba5d39569229fbe8_Out_2;
    Unity_Add_float4(_Multiply_504f471c3296b68cb57b2822d5b9dafd_Out_2, _Preview_e6b769821fa2d48db79c891b6969ab87_Out_1, _Add_6322dd81dc54bd82ba5d39569229fbe8_Out_2);
    UnityTexture2D _Property_32b9ecde70c97b8ba88575a54fda038d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
    float4 _SampleTexture2D_07f0647075b17684b9a283a412388475_RGBA_0 = SAMPLE_TEXTURE2D(_Property_32b9ecde70c97b8ba88575a54fda038d_Out_0.tex, _Property_32b9ecde70c97b8ba88575a54fda038d_Out_0.samplerstate, _Property_32b9ecde70c97b8ba88575a54fda038d_Out_0.GetTransformedUV(IN.uv0.xy));
    float _SampleTexture2D_07f0647075b17684b9a283a412388475_R_4 = _SampleTexture2D_07f0647075b17684b9a283a412388475_RGBA_0.r;
    float _SampleTexture2D_07f0647075b17684b9a283a412388475_G_5 = _SampleTexture2D_07f0647075b17684b9a283a412388475_RGBA_0.g;
    float _SampleTexture2D_07f0647075b17684b9a283a412388475_B_6 = _SampleTexture2D_07f0647075b17684b9a283a412388475_RGBA_0.b;
    float _SampleTexture2D_07f0647075b17684b9a283a412388475_A_7 = _SampleTexture2D_07f0647075b17684b9a283a412388475_RGBA_0.a;
    float _Step_b3b149cddef7ea839a47d5786560f178_Out_2;
    Unity_Step_float(1, _SampleTexture2D_07f0647075b17684b9a283a412388475_A_7, _Step_b3b149cddef7ea839a47d5786560f178_Out_2);
    float4 _Multiply_d2cb735dfc9de089bd3701f932a23eb5_Out_2;
    Unity_Multiply_float4_float4(_Add_6322dd81dc54bd82ba5d39569229fbe8_Out_2, (_Step_b3b149cddef7ea839a47d5786560f178_Out_2.xxxx), _Multiply_d2cb735dfc9de089bd3701f932a23eb5_Out_2);
    surface.BaseColor = IsGammaSpace() ? float3(0.5, 0.5, 0.5) : SRGBToLinear(float3(0.5, 0.5, 0.5));
    surface.SpriteColor = _Multiply_d2cb735dfc9de089bd3701f932a23eb5_Out_2;
    surface.Alpha = 1;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







    output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

    return output;
}

    // --------------------------------------------------
    // Main

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

    ENDHLSL
}
    }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
        FallBack "Hidden/Shader Graph/FallbackError"
}