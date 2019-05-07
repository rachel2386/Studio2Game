
//  ------------------------------------------------------------------
//  Tessellation

//  Tess additional Inputs
    half    _LuxWater_EdgeLength;
    float   _LuxWater_Phong;
    

#ifdef UNITY_CAN_COMPILE_TESSELLATION

    #include "Tessellation.cginc"
    
    struct TessVertex {
        float4 vertex : INTERNALTESSPOS;
        float3 normal : NORMAL;
        float4 color : COLOR;

        #if !defined(ISWATERVOLUME)
            float4 tangent : TANGENT;
            float4 texcoord : TEXCOORD0;
        #endif
        //UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct OutputPatchConstant {
        float edge[3]         : SV_TessFactor;
        float inside          : SV_InsideTessFactor;
    };
    
    TessVertex tessvert (appdata_water v) {
        TessVertex o;
        o.vertex    = v.vertex;
        o.normal    = v.normal;
        o.color     = v.color;

        #if !defined(ISWATERVOLUME)
            o.tangent   = v.tangent;
            o.texcoord  = v.texcoord;
        #endif
        
        //UNITY_VERTEX_INPUT_INSTANCE_ID
        return o;
    }


    float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2) {
        // 1.6ms full screen water on GTX 970M
        return UnityEdgeLengthBasedTess(v.vertex, v1.vertex, v2.vertex, _LuxWater_EdgeLength);
        // 1.6ms full screen water on GTX 970M?
        // return UnityEdgeLengthBasedTessCull(v.vertex, v1.vertex, v2.vertex, _LuxWater_EdgeLength, 1.0f);
    }

    OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
        OutputPatchConstant o;
        float4 ts = Tessellation( v[0], v[1], v[2] );
        // ts = (v[2].color.r == 0) ? float4(1,1,1,1) : ts; // Bad idea...
        o.edge[0] = ts.x;
        o.edge[1] = ts.y;
        o.edge[2] = ts.z;
        o.inside = ts.w;
        return o;
    }

    [domain("tri")]
    [partitioning("fractional_odd")]
    [outputtopology("triangle_cw")]
    [patchconstantfunc("hullconst")]
    [outputcontrolpoints(3)]
    TessVertex hs_surf (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
        return v[id];
    }

    [domain("tri")]
    v2f ds_surf (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
        appdata_water v = (appdata_water)0;

        v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
    
    //  Phong
    //  Disabled as it does not make any sense.
/*      if (_LuxWater_Phong > 0) {
            float3 pp[3];
            for (int i = 0; i < 3; ++i) {
                pp[i] = v.vertex.xyz - vi[i].normal * (dot(v.vertex.xyz, vi[i].normal) - dot(vi[i].vertex.xyz, vi[i].normal));
            }
            v.vertex.xyz = _LuxWater_Phong * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f - _LuxWater_Phong) * v.vertex.xyz;
        }
*/
            
        v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
        v.color = vi[0].color*bary.x + vi[1].color*bary.y + vi[2].color*bary.z;

        #if !defined(ISWATERVOLUME)
            v.texcoord = vi[0].texcoord*bary.x + vi[1].texcoord*bary.y + vi[2].texcoord*bary.z;
            //#ifdef UNITY_PASS_FORWARDADD
            //v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
            //#endif
            v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
        #endif

    //  New call the regular vertex function
        v2f o = vert(v);

        return o;
    }

#endif