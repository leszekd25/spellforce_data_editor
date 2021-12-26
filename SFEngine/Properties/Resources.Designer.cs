﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SFEngine.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SFEngine.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec3 fragmentPosition;
        ///in vec3 fragmentNormal;
        ///in vec2 UV;
        ///in vec4 fragmentColor;
        ///#ifdef SHADING
        ///#ifdef SHADOWS
        ///#ifdef CASCADED_SHADOWS
        ///in vec4 fragmentPositionLightSpace1;
        ///in vec4 fragmentPositionLightSpace2;
        ///in vec4 fragmentPositionLightSpace3;
        ///#endif //CASCADED_SHADOWS
        ///#ifndef CASCADED_SHADOWS
        ///in vec4 fragmentPositionLightSpace;
        ///#endif //CASCADED_SHADOWS
        ///#endif //SHADOWS
        ///#ifdef QUALITY_SHADING
        ///in mat4 M;
        ///in vec3 fragmentNormalTangentSpace;
        ///in vec4 fragmentGroundAmbientColor;
        ///#endif  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader {
            get {
                return ResourceManager.GetString("fshader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec2 UV;
        ///uniform sampler2D DiffuseTexture;
        ///
        ///void main(){
        ///  vec4 temp_c = texture(DiffuseTexture, UV);
        ///  if(temp_c.a &lt; 0.9)
        ///    discard;
        ///}.
        /// </summary>
        internal static string fshader_depth_prepass {
            get {
                return ResourceManager.GetString("fshader_depth_prepass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to out vec4 FragColor;
        ///  
        ///in vec2 TexCoords;
        ///
        ///uniform sampler2D screenTexture;
        /////uniform int renderShadowMap;
        /////uniform float ZNear;
        /////uniform float ZFar;
        ///
        /////float LinearizeDepth(float z)
        /////{
        /////  return (2.0 * ZNear) / (ZFar + ZNear - z * (ZFar- ZNear));	
        /////}
        ///
        ///void main()
        ///{
        ///    // this is for shadowmap
        ///   /* if(renderShadowMap == 1)
        ///    {
        ///        float color = LinearizeDepth(texture(screenTexture, TexCoords).r);
        ///        FragColor = vec4(color, color, color, 1.0);
        ///    }*/
        ///   // else
        /// //   [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_framebuffer_simple {
            get {
                return ResourceManager.GetString("fshader_framebuffer_simple", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec3 fragmentPosition;
        ///in vec2 UV;
        ///in vec3 fragmentNormal;
        ///in vec4 fragmentPositionLightSpace;
        ///in vec3 vpos_orig;
        ///in float vBrightness;
        ///
        ///out vec4 color;
        ///
        ///uniform int GridSize;
        ///uniform vec4 GridColor;
        ///uniform float SunStrength;
        ///uniform vec3 SunDirection;
        ///uniform vec4 SunColor;
        ///uniform float AmbientStrength;
        ///uniform vec4 AmbientColor;
        ///uniform vec4 FogColor;
        ///uniform float FogStrength;
        ///uniform float FogStart;
        ///uniform float FogEnd;
        ///uniform float FogExponent;
        ///uniform float ShadowFadeStart; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_hmap {
            get {
                return ResourceManager.GetString("fshader_hmap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///void main(){
        ///  // depth bias, ugly hack to circumvent tesselation issues on nvidia with pre-pass
        ///  //gl_FragDepth = gl_FragCoord.z + 0.0002;
        ///}.
        /// </summary>
        internal static string fshader_hmap_depth_prepass {
            get {
                return ResourceManager.GetString("fshader_hmap_depth_prepass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec4 fragmentColor;
        ///
        ///out vec4 color;
        ///
        ///void main(){
        ///  color = fragmentColor;
        ///}.
        /// </summary>
        internal static string fshader_overlay {
            get {
                return ResourceManager.GetString("fshader_overlay", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec2 UV;
        ///uniform sampler2D DiffuseTexture;
        ///
        ///out vec2 col;
        ///
        ///void main()
        ///{
        ///    if(texture(DiffuseTexture, UV).a &lt; 0.5)
        ///        discard;
        ///
        ///    float depth = gl_FragCoord.z;
        ///
        ///    float dx = dFdx(depth);
        ///    float dy = dFdy(depth);
        ///    float moment2 = depth * depth + 0.25 * (dx * dx + dy * dy);
        ///    
        ///    col = vec2(depth, moment2);
        ///}.
        /// </summary>
        internal static string fshader_shadowmap {
            get {
                return ResourceManager.GetString("fshader_shadowmap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to out vec2 FragColor;
        ///
        ///in vec2 TexCoords;
        ///
        ///uniform sampler2D image;
        ///
        ///uniform int horizontal;
        ///uniform float weight[3] = float[] (0.250301, 0.221461, 0.153388);
        ///
        ///vec2 GaussianBlur( sampler2D tex0, vec2 centreUV, vec2 pixelOffset )
        ///{
        ///    vec2 colOut = vec2( 0.0, 0.0 );
        ///    const int stepCount = 2;
        ///    //
        ///    const float gWeights[stepCount] ={
        ///       0.44908,
        ///       0.05092
        ///    };
        ///    const float gOffsets[stepCount] ={
        ///       0.53805,
        ///       2.06278
        ///    };
        ///
        ///    for( int i = 0; i &lt; stepCount [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_shadowmap_blur {
            get {
                return ResourceManager.GetString("fshader_shadowmap_blur", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec3 fragmentPosition;
        ///in vec2 UV;
        ///in vec3 fragmentNormal;
        ///#ifdef SHADING
        ///#ifdef SHADOWS
        ///in vec4 fragmentPositionLightSpace;
        ///#endif //SHADOWS
        ///#ifndef QUALITY_SHADING
        ///in float vBrightness;
        ///#endif //QUALITY_SHADING
        ///#ifdef QUALITY_SHADING
        ///in vec3 fragmentNormalTangentSpace;
        ///in vec4 fragmentGroundAmbientColor;
        ///#endif //QUALITY_SHADING
        ///#endif //SHADING
        ///
        ///out vec4 color;
        ///
        ///uniform mat4 M;
        ///uniform float SunStrength;
        ///uniform vec3 SunDirection;
        ///uniform vec4 SunColor;
        ///uniform float AmbientStren [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_skel {
            get {
                return ResourceManager.GetString("fshader_skel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to out vec4 FragColor;
        ///  
        ///in vec2 TexCoords;
        ///
        ///uniform mat4 V;
        ///uniform float AspectRatio;
        ///uniform float AmbientStrength;
        ///uniform vec4 AmbientColor;
        ///uniform vec4 FogColor;
        ///uniform float FogStrength;
        ///
        ///vec3 CalcLookat(mat4 view, vec2 uv)
        ///{
        ///  return vec3(-view[3]-view[2]-(view[0]*AspectRatio*(uv.x-0.5)) + (view[1]*(uv.y-0.5)));
        ///}
        ///
        ///void main()
        ///{
        ///    vec3 raydir = CalcLookat(V, TexCoords);
        ///    float horizon_closeness = clamp(1.0 - dot(normalize(raydir), vec3(0.0, 1.0, 0.0)), 0.0, 1.0);    // 0% - p [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_sky {
            get {
                return ResourceManager.GetString("fshader_sky", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to out vec4 FragColor;
        ///  
        ///in vec2 TexCoords;
        ///
        ///uniform sampler2D screenTexture;
        ///uniform float exposure;
        ///
        ///float luminance(vec3 v)
        ///{
        ///    return dot(v, vec3(0.2126f, 0.7152f, 0.0722f));
        ///}
        ///
        ///vec3 change_luminance(vec3 c_in, float l_out)
        ///{
        ///    float l_in = luminance(c_in);
        ///    return c_in * (l_out / l_in);
        ///}
        ///
        ///vec3 reinhard(vec3 v)
        ///{
        ///    return v / (1.0f + v);
        ///}
        ///
        ///vec3 reinhard_extended(vec3 v, float max_white)
        ///{
        ///    vec3 numerator = v * (1.0f + (v / vec3(max_white * max_white)));
        ///    return n [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_tonemap {
            get {
                return ResourceManager.GetString("fshader_tonemap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec3 fragmentPosition;
        ///in vec3 fragmentNormal;
        ///in vec2 UV;
        ///in vec4 fragmentColor;
        ///#ifdef SHADING
        ///#ifdef SHADOWS
        ///in vec4 fragmentPositionLightSpace;
        ///#endif //SHADOWS
        ///#ifdef QUALITY_SHADING
        ///in mat4 M;
        ///in vec3 fragmentNormalTangentSpace;
        ///in vec4 fragmentGroundAmbientColor;
        ///#endif //QUALITY_SHADING
        ///#ifndef QUALITY_SHADING
        ///in float vBrightness;
        ///#endif //QUALITY_SHADING
        ///#endif //SHADING
        ///
        ///out vec4 color;
        ///
        /////uniform mat4 M;
        ///uniform float SunStrength;
        ///uniform vec3 SunDirection;
        ///uniform vec4 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_transparent {
            get {
                return ResourceManager.GetString("fshader_transparent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec2 UV;
        ///in vec4 color;
        ///
        ///out vec4 out_color;
        ///
        ///uniform sampler2D Tex;
        ///
        ///void main(){
        ///  vec4 temp_c;
        ///  temp_c = texture(Tex, UV);
        ///  if(temp_c.a == 0.0)
        ///    discard;
        ///
        ///  out_color = temp_c * color;
        ///}.
        /// </summary>
        internal static string fshader_ui {
            get {
                return ResourceManager.GetString("fshader_ui", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to layout(vertices = 4) out;
        ///
        ///uniform vec3 cameraPos;
        ///
        ///// for now, there&apos;s no correction for average  patch height (all patches are of height 0)
        ///// doing the correction will surely improve peformance
        ///
        ///float distance(vec2 p1, vec2 p2)
        ///{
        ///    return sqrt(dot(p1-p2, p1-p2));
        ///}
        ///
        ///float edgeCameraDistance(vec4 p1, vec4 p2)
        ///{
        ///    vec2 res_pos = (p1 + p2).xz * 0.5;
        ///    return distance(res_pos, cameraPos.xz);
        ///}
        ///
        ///float tesselationPerDistance(float d)
        ///{
        ///    // 0-50 -&gt; 1, 50-100 -&gt; 2, 100-200 -&gt; 4, 200- [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tcsshader_hmap_tesselated {
            get {
                return ResourceManager.GetString("tcsshader_hmap_tesselated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to layout(quads) in;
        ///
        ///out vec2 UV;
        ///
        ///// Values that stay constant for the whole mesh.
        ///
        ///uniform int GridSize;
        ///uniform sampler2D HeightMap;
        ///uniform mat4 LSM;
        ///
        ///vec4 GetTesselatedVertex(in vec4 v0, in vec4 v1, in vec4 v2, in vec4 v3)
        ///{
        ///    vec4 a = mix(v0, v1, gl_TessCoord.x);
        ///    vec4 b = mix(v2, v3, gl_TessCoord.x);
        ///    return mix(a, b, gl_TessCoord.y);
        ///}
        ///
        ///vec3 GetVertexPos(vec2 grid_pos)
        ///{
        ///    return vec3(grid_pos.x, texture(HeightMap, vec2(grid_pos.x, GridSize - 1 - grid_pos.y)/GridSize).r *  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tesshader_hmap_shadowmap_tesselated {
            get {
                return ResourceManager.GetString("tesshader_hmap_shadowmap_tesselated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to layout(quads) in;
        ///
        ///invariant gl_Position;
        ///
        ///out vec3 fragmentPosition;
        ///out vec2 UV;
        ///out vec3 fragmentNormal;
        ///out vec4 fragmentPositionLightSpace;
        ///out vec3 vpos_orig;
        ///out float vBrightness;
        ///
        ///// Values that stay constant for the whole mesh.
        ///
        ///uniform int GridSize;
        ///uniform sampler2D HeightMap;
        ///uniform mat4 VP;
        ///uniform mat4 LSM;
        ///uniform vec3 SunDirection;
        ///
        ///vec4 GetTesselatedVertex(in vec4 v0, in vec4 v1, in vec4 v2, in vec4 v3)
        ///{
        ///    vec4 a = mix(v0, v1, gl_TessCoord.x);
        ///    vec4 b = mix(v2, [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tesshader_hmap_tesselated {
            get {
                return ResourceManager.GetString("tesshader_hmap_tesselated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to layout(quads) in;
        ///
        ///invariant gl_Position;
        ///out vec3 fragmentPosition;
        ///
        ///// Values that stay constant for the whole mesh.
        ///
        ///uniform int GridSize;
        ///uniform sampler2D HeightMap;
        ///uniform mat4 VP;
        ///
        ///vec4 GetTesselatedVertex(in vec4 v0, in vec4 v1, in vec4 v2, in vec4 v3)
        ///{
        ///    vec4 a = mix(v0, v1, gl_TessCoord.x);
        ///    vec4 b = mix(v2, v3, gl_TessCoord.x);
        ///    return mix(a, b, gl_TessCoord.y);
        ///}
        ///
        ///vec3 GetVertexPos(vec2 grid_pos)
        ///{
        ///    return vec3(grid_pos.x, texture(HeightMap, vec2(grid_pos.x, Grid [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string tesshader_hmap_tesselated_depth_prepass {
            get {
                return ResourceManager.GetString("tesshader_hmap_tesselated_depth_prepass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        ///layout(location = 1) in vec3 vertexNormal;
        ///layout(location = 2) in vec4 vertexColor;
        ///layout(location = 3) in vec2 vertexUV;
        ///layout(location = 4) in mat4 instanceMatrix;
        ///
        ///out vec3 fragmentPosition;
        ///out vec3 fragmentNormal;
        ///out vec4 fragmentColor;
        ///out vec2 UV;
        ///#ifdef SHADING
        ///#ifdef SHADOWS
        ///#ifdef CASCADED_SHADOWS
        ///out vec4 fragmentPositionLightSpace1;
        ///out vec4 fragmentPositi [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vshader {
            get {
                return ResourceManager.GetString("vshader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        ///layout(location = 3) in vec2 vertexUV;
        ///layout(location = 4) in mat4 instanceMatrix;
        ///
        ///out vec3 fragmentPosition;
        ///out vec2 UV;
        ///
        ///// Values that stay constant for the whole mesh.
        ///uniform mat4 VP;
        ///  
        ///void main(){
        ///  // Output position of the vertex, in clip space : MVP * position
        ///  gl_Position = VP * instanceMatrix * vec4(vertexPosition_modelspace,1);
        ///  UV = vertexUV;
        ///}.
        /// </summary>
        internal static string vshader_depth_prepass {
            get {
                return ResourceManager.GetString("vshader_depth_prepass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to layout (location = 0) in vec2 aPos;
        ///layout (location = 1) in vec2 aTexCoords;
        ///
        ///out vec2 TexCoords;
        ///
        ///void main()
        ///{
        ///    gl_Position = vec4(aPos.x, aPos.y, 0.0, 1.0); 
        ///    TexCoords = aTexCoords;
        ///}.
        /// </summary>
        internal static string vshader_framebuffer {
            get {
                return ResourceManager.GetString("vshader_framebuffer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        /////layout(location = 1) in vec3 vertexNormal;
        /////layout(location = 2) in vec3 texID;
        /////layout(location = 3) in vec3 texWeight;
        ///
        ///invariant gl_Position;
        ///
        ///out vec3 fragmentPosition;
        ///out vec2 UV;
        ///out vec3 fragmentNormal;
        ///#ifdef SHADOWS
        ///#ifdef CASCADED_SHADOWS
        ///out vec4 fragmentPositionLightSpace1;
        ///out vec4 fragmentPositionLightSpace2;
        ///out vec4 fragmentPositionLightSpace3;
        ///#endi [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vshader_hmap {
            get {
                return ResourceManager.GetString("vshader_hmap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        ///
        ///invariant gl_Position;
        ///
        ///// Values that stay constant for the whole mesh.
        ///
        ///uniform int GridSize;
        ///uniform sampler2D HeightMap;
        ///uniform mat4 VP;
        ///
        ///vec3 GetVertexPos(vec2 grid_pos)
        ///{
        ///    return vec3(grid_pos.x, texture(HeightMap, vec2(grid_pos.x, GridSize - 1.0 - grid_pos.y)/GridSize).r * 655.35, grid_pos.y);
        ///}
        ///  
        ///void main(){
        ///  // Output position of the vertex, in clip spa [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vshader_hmap_depth_prepass {
            get {
                return ResourceManager.GetString("vshader_hmap_depth_prepass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        ///
        ///invariant gl_Position;
        ///
        ///void main(){
        ///  gl_Position = vec4(vertexPosition_modelspace, 1.0);
        ///}.
        /// </summary>
        internal static string vshader_hmap_tesselated {
            get {
                return ResourceManager.GetString("vshader_hmap_tesselated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        ///
        ///out vec4 fragmentColor;
        ///
        ///// Values that stay constant for the whole mesh.
        ///uniform mat4 MVP;
        ///uniform vec4 Color;
        ///  
        ///void main(){
        ///  // Output position of the vertex, in clip space : MVP * position
        ///  gl_Position = MVP * vec4(vertexPosition_modelspace,1);
        ///  fragmentColor = Color;
        ///}.
        /// </summary>
        internal static string vshader_overlay {
            get {
                return ResourceManager.GetString("vshader_overlay", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        ///layout(location = 1) in vec3 vertexNormal;
        ///layout(location = 2) in vec4 vertexColor;
        ///layout(location = 3) in vec2 vertexUV;
        ///layout(location = 4) in mat4 instanceMatrix;
        ///
        ///out vec2 UV;
        ///
        ///uniform mat4 LSM;  // light space matrix
        ///
        ///void main()
        ///{
        ///    gl_Position = LSM * instanceMatrix * vec4(vertexPosition_modelspace, 1.0);
        ///    UV = vertexUV;
        ///}.
        /// </summary>
        internal static string vshader_shadowmap {
            get {
                return ResourceManager.GetString("vshader_shadowmap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        ///layout(location = 1) in vec3 vertexNormal;
        ///layout(location = 2) in vec4 vertexBoneWeight;
        ///layout(location = 3) in vec2 vertexUV;
        ///layout(location = 4) in vec4 vertexBoneIndex;
        ///
        ///out vec2 UV;
        ///
        ///// Values that stay constant for the whole mesh.
        ///uniform mat4 LSM;
        ///uniform mat4 M;
        ///uniform mat4 boneTransforms[224];
        ///  
        ///void main(){
        ///  vec4 Vertex;
        ///  vec4 newVertex;
        ///  int index;
        ///
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vshader_shadowmap_animated {
            get {
                return ResourceManager.GetString("vshader_shadowmap_animated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        ///
        ///out vec2 UV;
        ///
        ///// Values that stay constant for the whole mesh.
        ///uniform int GridSize;
        ///uniform sampler2D HeightMap;
        ///uniform mat4 LSM;
        ///  
        ///vec3 GetVertexPos(vec2 grid_pos)
        ///{
        ///    return vec3(grid_pos.x, texture(HeightMap, vec2(grid_pos.x, GridSize - 1 - grid_pos.y)/GridSize).r * 655.35, grid_pos.y);
        ///}
        ///
        ///void main(){
        ///  vec3 vpos = GetVertexPos(vertexPosition_modelspace.xz);
        ///  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vshader_shadowmap_heightmap {
            get {
                return ResourceManager.GetString("vshader_shadowmap_heightmap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        ///layout(location = 1) in vec3 vertexNormal;
        ///layout(location = 2) in vec4 vertexBoneWeight;
        ///layout(location = 3) in vec2 vertexUV;
        ///layout(location = 4) in vec4 vertexBoneIndex;
        ///
        ///out vec3 fragmentPosition;
        ///out vec2 UV;
        ///out vec3 fragmentNormal;
        ///#ifdef SHADING
        ///#ifdef SHADOWS
        ///#ifdef CASCADED_SHADOWS
        ///out vec4 fragmentPositionLightSpace1;
        ///out vec4 fragmentPositionLightSpace2;
        ///out [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vshader_skel {
            get {
                return ResourceManager.GetString("vshader_skel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to layout (location = 0) in vec2 aPos;
        ///layout (location = 1) in vec2 aTexCoords;
        ///
        ///out vec2 TexCoords;
        ///
        ///void main()
        ///{
        ///    gl_Position = vec4(aPos.x, aPos.y, 1.0, 1.0); 
        ///    TexCoords = aTexCoords;
        ///}.
        /// </summary>
        internal static string vshader_sky {
            get {
                return ResourceManager.GetString("vshader_sky", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // Input vertex data, different for all executions of this shader.
        ///layout(location = 0) in vec3 vertexPosition_modelspace;
        ///layout(location = 1) in vec2 vertexUV;
        ///layout(location = 2) in vec4 vertexColor;
        ///
        ///out vec2 UV;
        ///out vec4 color;
        ///
        ///uniform vec2 offset;
        ///uniform vec2 ScreenSize;
        ///  
        ///void main(){
        ///  // Output position of the vertex, in clip space : MVP * position
        ///  
        ///  vec2 real_pos = vec2(vertexPosition_modelspace.x+offset.x - (ScreenSize.x/2), -(vertexPosition_modelspace.y + offset.y - (ScreenS [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vshader_ui {
            get {
                return ResourceManager.GetString("vshader_ui", resourceCulture);
            }
        }
    }
}
