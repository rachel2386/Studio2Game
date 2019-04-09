// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FoodGradient"
{
	Properties
	{
		_yOffset("yOffset", Float) = 0
		_Float3("Float 3", Float) = 0
		_factor("factor", Float) = 0
		_expand("expand", Float) = 0.2
		_bottomColor("bottomColor", Float) = 0
		_heightChange("heightChange", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
		};

		uniform float _expand;
		uniform float _Float3;
		uniform float _heightChange;
		uniform float _bottomColor;
		uniform float _yOffset;
		uniform float _factor;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float temp_output_29_0 = sin( ( ( -0.5 + ase_vertex3Pos.y ) * -1.0 * ( 0.9 * UNITY_PI ) ) );
			float4 appendResult9 = (float4(( ase_vertex3Pos.x * temp_output_29_0 * _expand ) , ( ( _Float3 + ase_vertex3Pos.y ) * _heightChange ) , ( temp_output_29_0 * ase_vertex3Pos.z * _expand ) , 0.0));
			v.vertex.xyz += appendResult9.xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 temp_cast_0 = (( ( ( 1.0 + ( -1.0 * _bottomColor ) ) * ( ( _yOffset + ase_vertex3Pos.y ) * _factor ) ) + _bottomColor )).xxx;
			o.Albedo = temp_cast_0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
354;72.66667;769;487;1395.442;-85.44197;3.673887;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;6;-414.6949,1160.458;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-386.0947,1059.047;Float;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;-0.5;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;325.6004,262.7303;Float;False;Constant;_Float2;Float 2;4;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-308.452,1318.56;Float;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;410.2368,518.0057;Float;False;Property;_bottomColor;bottomColor;4;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;31;-38.10288,1431.839;Float;False;1;0;FLOAT;0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-182.5874,327.4612;Float;False;Property;_yOffset;yOffset;0;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;1;-222.3875,415.1615;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-212.2748,1124.157;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;361.813,1176.222;Float;False;Property;_expand;expand;3;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;485.3108,149.3725;Float;False;Constant;_topColor;topColor;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-22.19564,1203.735;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-174.3674,561.9819;Float;False;Property;_factor;factor;2;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;363.0334,1438.413;Float;False;Property;_Float3;Float 3;1;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;32;323.2332,1526.113;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;4;-7.269668,330.2267;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;496.5859,260.073;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;17;568.0205,1050.271;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;538.3511,1441.178;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;116.8688,427.0688;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;503.4788,1702.33;Float;False;Property;_heightChange;heightChange;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;651.3197,153.3627;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;29;-9.142614,972.3617;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;13;180.2612,799.3958;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;708.072,1436.488;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;669.3457,1024.878;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;767.3068,284.0529;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;650.7405,824.9037;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;9;833.251,817.4473;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;841.4041,478.5283;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1274.04,503.3996;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;FoodGradient;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;7;0
WireConnection;8;1;6;2
WireConnection;10;0;8;0
WireConnection;10;1;11;0
WireConnection;10;2;31;0
WireConnection;4;0;5;0
WireConnection;4;1;1;2
WireConnection;20;0;21;0
WireConnection;20;1;18;0
WireConnection;17;0;16;0
WireConnection;34;0;33;0
WireConnection;34;1;32;2
WireConnection;2;0;4;0
WireConnection;2;1;3;0
WireConnection;26;0;25;0
WireConnection;26;1;20;0
WireConnection;29;0;10;0
WireConnection;36;0;34;0
WireConnection;36;1;37;0
WireConnection;15;0;29;0
WireConnection;15;1;13;3
WireConnection;15;2;16;0
WireConnection;27;0;26;0
WireConnection;27;1;2;0
WireConnection;12;0;13;1
WireConnection;12;1;29;0
WireConnection;12;2;17;0
WireConnection;9;0;12;0
WireConnection;9;1;36;0
WireConnection;9;2;15;0
WireConnection;23;0;27;0
WireConnection;23;1;18;0
WireConnection;0;0;23;0
WireConnection;0;11;9;0
ASEEND*/
//CHKSM=7B9B9C0861264C6DBCED755DA1F108FDF30F0E04