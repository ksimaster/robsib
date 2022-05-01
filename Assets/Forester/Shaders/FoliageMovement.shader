// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

 Shader "Forester/FoliageMovement" {
     Properties {
         _Color ("Color", Color) = (1,1,1,1)
         _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
         _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		 _BumpTex ("Normal Map",2D) = "bump" {}
		 _BumpStrength ("Normal Strength", float) = 1
		 _Glossiness("Smoothness", Range(0,1)) = 0.5
		 _Metallic("Metallic", Range(0,1)) = 0.0
		 _Influence("Adjust Influence",Range(0,1)) = 1
		 _Wave("Add Wave",Range(0,1)) = 0.1
		 _Springness("Springyness",float) = 0.01

     }
     SubShader {
         Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "DisableBatching" = "LODFading"}
         LOD 200
		 Cull Off

         
         CGPROGRAM        
         #pragma surface surf Standard fullforwardshadows alphatest:_Cutoff vertex:vert addshadow     
         #pragma target 3.0
 
         sampler2D _MainTex;   
		 sampler2D _BumpTex;
		 half _BumpStrength;
		 half _Speed;
		 half _Springness;
		 half4 _Color;
		 half4 _Offset;
		 half _Glossiness;
		 half _Metallic;
		 half _Influence;
		 half _Wave;
		 half _PulseFrequency;
		 half _PulseMagnitude;
 
         struct Input {
             float2 uv_MainTex;
			 float2 uv_BumpTex;
			 float3 viewDir;
         };

		 void vert (inout appdata_full v) 
		{

				_Offset = fixed4(_Offset.x, _Offset.y,_Offset.z,_Offset.w);

				half wave = _Wave / 10;
				half pulseTime = sin(_Time.x * _PulseFrequency);
				half pulseSpeed = clamp(_Speed/2,_Speed,pulseTime);
			    half speed = lerp(pulseSpeed, pulseSpeed * _PulseMagnitude,pulseTime/2)*10;

				//Tune Influence
				fixed4 newPos = mul(unity_ObjectToWorld,v.vertex);
				half influenceX = (_Influence * _Offset.x);
				half influenceZ = (_Influence * _Offset.z);

				half sinY = sin(_Time.y + v.vertex.y) * wave * speed * _Springness;
				half sinX = sin(_Time.x)  * pulseSpeed;

				half x1 = clamp(v.color.g * sinX * influenceX,-1,1);
				half x2 = clamp(v.color.g * sinX * -influenceX,-1,1);
				half y1 = clamp(v.color.g * sinY,-1,1);

				half z1 = clamp(v.color.g * sinX * influenceZ,-1,1);
				half z2 = clamp(v.color.g * sinX * -influenceZ,-1,1);

				//Top
				if(_Offset.x > 0)
				{
				newPos.x  += x1;
				
				newPos.x += y1;
				}else{
				newPos.x  -= x2;
				newPos.x -= y1;
				}

				if(_Offset.z > 0)
				{
				newPos.z += z1;
				newPos.z += y1;
				}else{
				newPos.z -= z2;
				newPos.z -= y1;
				}
				//Base
				newPos.x += v.color.r * 0;
				newPos.z += v.color.r * 0;
				newPos.y += v.color.r * 0;
				newPos.w += v.color.r * 0;
			
				newPos.y += y1;
				v.vertex = mul(unity_WorldToObject,newPos);
				}
 
         void surf (Input IN, inout SurfaceOutputStandard o) {  
		 fixed3 n = lerp(fixed3(0.5,0.5,1),UnpackNormal(tex2D(_BumpTex, IN.uv_BumpTex)),_BumpStrength);

             fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
             o.Albedo = c.rgb;
             o.Alpha = c.a;
			 o.Normal = n.rgb;
			 // Metallic and smoothness come from slider variables
			 o.Metallic = _Metallic;
			 o.Smoothness = _Glossiness;
         }
         ENDCG
     }
     FallBack "Transparent/Cutout/Diffuse"
 }