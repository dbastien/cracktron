// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#include "UnityCG.cginc"

struct shadowcast_a2v
{
    float4 vertex : POSITION;
//    float4 color : COLOR;
//    float2 mainUV : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct shadowcast_v2f
{
//    float4 color : COLOR;
//    float3 texXYFadeZ : TEXCOORD0;
    V2F_SHADOW_CASTER;
    UNITY_VERTEX_OUTPUT_STEREO
};

shadowcast_v2f shadowcast_vert(shadowcast_a2v v)
{
    shadowcast_v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TRANSFER_SHADOW_CASTER(o)
    return o;
}

fixed4 shadowcast_frag(shadowcast_v2f IN) : SV_Target
{
    //TODO: optionally support alpha testing cutouts for shadows
    SHADOW_CASTER_FRAGMENT(IN)
}