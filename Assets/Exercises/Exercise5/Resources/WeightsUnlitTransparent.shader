// adaptet from https://en.wikibooks.org/wiki/Cg_Programming/Unity/Transparency

Shader "AiG/WeightsUnlitTransparent"
{
    Properties
    {
        _alpha ("alpha value", Range(0, 1)) = 0.3
    }

    SubShader {
        Tags { "Queue" = "Transparent" } // draw after all opaque geometry has been drawn
        
        Pass {
            Cull Front // first pass renders only back faces (the "inside")
            ZWrite Off // don't write to depth buffer in order not to occlude other objects
            Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

            CGPROGRAM 
 
            #pragma vertex vert 
            #pragma fragment frag
         
            float _alpha;
 
            struct vIn
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };
            struct v2f
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };    
              
            v2f vert(vIn i) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.color = i.color; // passthrough vertex color
                return o;
            }
 
            float4 frag(v2f i) : COLOR
            {
                // use the vertex color and alpha slider
                return float4(i.color.xyz, _alpha);  
            }
 
            ENDCG  
        }

        Pass {
            Cull Back // second pass renders only front faces (the "outside")
            ZWrite Off // don't write to depth buffer in order not to occlude other objects
            Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

            CGPROGRAM 
 
            #pragma vertex vert 
            #pragma fragment frag
 
            float _alpha;
 
            struct vIn
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };
            struct v2f
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };    
         
            v2f vert(vIn i) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.color = i.color; // passthrough vertex color
                return o;
            }
  
            float4 frag(v2f i) : COLOR
            {
                // use the vertex color and alpha slider
                return float4(i.color.xyz, _alpha);
            }
 
            ENDCG  
        }
    }
}
