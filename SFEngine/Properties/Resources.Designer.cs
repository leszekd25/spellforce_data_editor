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
        ///in vec2 fragmentUV;
        ///in vec4 fragmentColor;
        ///#ifdef SHADOWS
        ///in vec4 fragmentPositionLightSpace;
        ///#endif //SHADOWS
        ///#ifdef SHADING
        ///#ifdef QUALITY_SHADING
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
        ///uniform vec4 SunColor;
        ///uniform float DepthBias;
        ///uniform float AlphaCutout;
        ///#ifdef SHADING
        ///uniform vec4 FogColor; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader {
            get {
                return ResourceManager.GetString("fshader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to out vec4 FragColor;
        ///  
        ///in vec2 TexCoords;
        ///
        ///uniform sampler2D screenTexture;
        ///
        ///void main()
        ///{
        ///        FragColor = texture(screenTexture, TexCoords);
        ///}.
        /// </summary>
        internal static string fshader_framebuffer_simple {
            get {
                return ResourceManager.GetString("fshader_framebuffer_simple", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec3 fragmentPosition;
        ///in vec2 UV;
        ///#if defined NO_TEXTURE || defined TEXTURE_LOD
        ///in vec3 fragmentColor;
        ///#endif //defined NO_TEXTURE || defined TEXTURE_LOD
        ///#ifdef SHADOWS
        ///in vec4 fragmentPositionLightSpace;
        ///#endif //SHADOWS
        ///#ifdef SHADING
        ///in float vBrightness;
        ///#endif //SHADING
        ///
        ///out vec4 color;
        ///
        ///uniform int GridSize;
        ///uniform vec4 SunColor;
        ///#ifdef EDITOR_MODE
        ///uniform vec4 GridColor;
        ///uniform int CurrentFlags;
        ///#endif //EDITOR_MODE
        ///#ifdef SHADING
        ///uniform vec4 FogColor;
        ///uniform float FogSt [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_hmap {
            get {
                return ResourceManager.GetString("fshader_hmap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///void main(){
        ///}.
        /// </summary>
        internal static string fshader_hmap_depth_prepass {
            get {
                return ResourceManager.GetString("fshader_hmap_depth_prepass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec2 TexCoords;
        ///
        ///out vec4 OutMoments;
        ///
        ///uniform sampler2DMS ShadowMap;
        ///uniform int TextureSize;
        ///
        ///void main()
        ///{
        ///	const mat4 quantization_transform = mat4(
        ///		1.5, 0.0, -2.0, 0.0,
        ///		0.0, 4.0, 0.0, -4.0,
        ///		sqrt(3)/2.0, 0.0, -sqrt(12)/9.0, 0.0,
        ///		0.0, 0.5, 0.0, 0.5
        ///		);
        ///	const vec4 quantization_shift = vec4(0.5, 0.0, 0.5, 0.0);
        ///
        ///	vec4 result = vec4(0.0);
        ///	for(int i = 0; i &lt; 4; i++)
        ///	{
        ///		float z = texelFetch(ShadowMap, ivec2(TexCoords * TextureSize), i).r;
        ///		vec4 depth_vector = vec4(z, z*z, [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_msm_resolve {
            get {
                return ResourceManager.GetString("fshader_msm_resolve", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec2 UV;
        ///
        ///out vec4 color;
        ///
        ///uniform sampler2D DiffuseTex;
        ///uniform float Time;
        ///uniform vec4 Color;
        ///
        ///void main(){
        ///  vec4 temp_c = texture(DiffuseTex, UV);
        ///  if(temp_c.a == 0.0)
        ///    discard;
        ///
        ///  color = mix(vec4(0.0), Color, clamp((sin(Time)+1.0)*0.6, 0.0, 1.0));
        ///}.
        /// </summary>
        internal static string fshader_selection {
            get {
                return ResourceManager.GetString("fshader_selection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec2 UV;
        ///uniform sampler2D DiffuseTexture;
        ///
        ///#ifdef VSM
        ///out vec2 col;
        ///#endif // VSM
        ///#ifdef MSM
        ///#endif // MSM
        ///
        ///void main()
        ///{
        ///    if(texture(DiffuseTexture, UV).a &lt; 0.5)
        ///        discard;
        ///
        ///	#ifdef VSM
        ///    float depth = gl_FragCoord.z;
        ///
        ///    float dx = dFdx(depth);
        ///    float dy = dFdy(depth);
        ///    float moment2 = depth * depth + 0.25 * (dx * dx + dy * dy);
        ///    
        ///    col = vec2(depth, moment2);
        ///	#endif // VSM
        ///}.
        /// </summary>
        internal static string fshader_shadowmap {
            get {
                return ResourceManager.GetString("fshader_shadowmap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #ifdef VSM
        ///#define TEX_TYPE vec2
        ///#endif // VSM
        ///#ifdef MSM
        ///#define TEX_TYPE vec4
        ///#endif // MSM
        ///
        ///out TEX_TYPE FragColor;
        ///
        ///in vec2 TexCoords;
        ///
        ///uniform sampler2D image;
        ///uniform int horizontal;
        ///
        ///TEX_TYPE GaussianBlur( sampler2D tex0, vec2 centreUV, vec2 pixelOffset )
        ///{
        ///    TEX_TYPE colOut = TEX_TYPE( 0.0 );
        ///    const int stepCount = 2;
        ///    //
        ///    const float gWeights[stepCount] ={
        ///       0.44908,
        ///       0.05092
        ///    };
        ///    const float gOffsets[stepCount] ={
        ///       0.53805,
        ///       2.06278        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_shadowmap_blur {
            get {
                return ResourceManager.GetString("fshader_shadowmap_blur", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to out vec4 FragColor;
        ///  
        ///in vec2 TexCoords;
        ///
        ///uniform mat4 V;
        ///uniform float AspectRatio;
        ///uniform vec4 AmbientColor;
        ///uniform vec4 FogColor;
        ///
        ///vec3 CalcLookat(mat4 view, vec2 uv)
        ///{
        ///  return vec3(-view[3]-view[2]-(view[0]*AspectRatio*(uv.x-0.5)) + (view[1]*(uv.y-0.5)));
        ///}
        ///
        ///void main()
        ///{
        ///    vec3 raydir = CalcLookat(V, TexCoords);
        ///    float horizon_closeness = clamp(1.0 - dot(normalize(raydir), vec3(0.0, 1.0, 0.0)), 0.0, 1.0);    // 0% - pole, 100% - horizon
        ///
        ///    vec3 skycol = vec3(mix(AmbientCol [rest of string was truncated]&quot;;.
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
        ///   Looks up a localized string similar to in vec2 TexCoords;
        ///
        ///out vec2 OutMoments;
        ///
        ///uniform sampler2DMS ShadowMap;
        ///uniform int TextureSize;
        ///
        ///void main()
        ///{
        ///	vec2 result = vec2(0.0);
        ///	for(int i = 0; i &lt; 4; i++)
        ///	{
        ///		float z = texelFetch(ShadowMap, ivec2(TexCoords * TextureSize), i).r;
        ///
        ///		float dx = dFdx(z);
        ///		float dy = dFdy(z);
        ///		float moment2 = z * z + 0.25 * (dx * dx + dy * dy);
        ///		
        ///		result += vec2(z, moment2);
        ///	}
        ///	
        ///	OutMoments = result/4.0;
        ///}.
        /// </summary>
        internal static string fshader_vsm_resolve {
            get {
                return ResourceManager.GetString("fshader_vsm_resolve", resourceCulture);
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
        ///
        ///#if defined NO_TEXTURE || defined TEXTURE_LOD
        ///out vec3 fragmentColor;
        ///#endif //defined NO_TEXTURE || defined TEXTURE_LOD
        ///
        ///#ifdef SHADOWS
        ///out vec4 fragmentPositionLightSpace;
        ///#endif //SHADOWS
        ///
        ///#ifdef SHADING
        ///out float vBrightness;
        ///#endif //SHADING
        ///
        ///// Values that stay constant for the whole mesh.
        ///
        ///uniform int GridSize;
        ///uniform sampler2D HeightMap;
        ///uniform mat4 VP;
        ///
        ///#ifdef SHADOWS
        ///uniform mat4 LSM;
        ///# [rest of string was truncated]&quot;;.
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
        ///out vec4 fragmentColor;
        ///out vec2 fragmentUV;
        ///#ifdef SHADOWS
        ///out vec4 fragmentPositionLightSpace;
        ///#endif //SHADOWS
        ///
        ///#ifdef SHADING
        ///#ifdef QUALITY_SHADING
        ///out vec3 fragmentNormal [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vshader {
            get {
                return ResourceManager.GetString("vshader", resourceCulture);
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
        ///
        ///invariant gl_Position;
        ///
        ///out vec3 fragmentPosition;
        ///out vec2 UV;
        ///#if defined NO_TEXTURE || defined TEXTURE_LOD
        ///out vec3 fragmentColor;
        ///#endif //defined NO_TEXTURE || defined TEXTURE_LOD
        ///
        ///#ifdef SHADOWS
        ///out vec4 fragmentPositionLightSpace;
        ///#endif //SHADOWS
        ///
        ///#ifdef SHADING
        ///out float vBrightness;
        ///#endif //SHADING
        ///
        ///// Values that stay constant for the whole mesh.
        ///
        ///unifo [rest of string was truncated]&quot;;.
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
        ///layout(location = 3) in vec2 vertexUV;
        ///layout(location = 4) in mat4 instanceMatrix;
        ///
        ///out vec2 UV;
        ///
        ///uniform mat4 VP;  // light space matrix
        ///
        ///void main()
        ///{
        ///    gl_Position = VP * instanceMatrix * vec4(vertexPosition_modelspace, 1.0);
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
        ///layout(location = 2) in vec4 vertexBoneWeight;
        ///layout(location = 3) in vec2 vertexUV;
        ///layout(location = 4) in vec4 vertexBoneIndex;
        ///
        ///out vec2 UV;
        ///
        ///// Values that stay constant for the whole mesh.
        ///uniform mat4 P;
        ///uniform mat4 V;
        ///uniform mat4 M;
        ///uniform mat4 boneTransforms[224];
        ///  
        ///void main(){
        ///  vec4 Vertex;
        ///  vec4 newVertex;
        ///  int index;
        ///
        ///  Vertex = vec4(vertexPositio [rest of string was truncated]&quot;;.
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
        ///uniform mat4 VP;
        ///  
        ///vec3 GetVertexPos(vec2 grid_pos)
        ///{
        ///    return vec3(grid_pos.x, texture(HeightMap, vec2(grid_pos.x, GridSize - 1 - grid_pos.y)/GridSize).r * 655.35, grid_pos.y);
        ///}
        ///
        ///void main(){
        ///  vec3 vpos = GetVertexPos(vertexPosition_modelspace.xz);
        ///   [rest of string was truncated]&quot;;.
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
        ///out vec2 fragmentUV;
        ///out vec4 fragmentColor;
        ///#ifdef SHADOWS
        ///out vec4 fragmentPositionLightSpace;
        ///#endif //SHADOWS
        ///
        ///#ifdef SHADING
        ///#ifdef QUALITY_SHADING
        ///out vec3 fragment [rest of string was truncated]&quot;;.
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
