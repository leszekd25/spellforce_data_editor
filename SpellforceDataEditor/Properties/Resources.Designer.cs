﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SpellforceDataEditor.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SpellforceDataEditor.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap building_icon {
            get {
                object obj = ResourceManager.GetObject("building_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap decoration_icon {
            get {
                object obj = ResourceManager.GetObject("decoration_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap flag_icon {
            get {
                object obj = ResourceManager.GetObject("flag_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to in vec3 fragmentPosition;
        ///in vec3 fragmentNormal;
        ///in vec2 UV;
        ///in vec4 fragmentColor;
        ///in vec4 fragmentPositionLightSpace;
        ///in mat4 M;
        ///
        ///out vec4 color;
        ///
        /////uniform mat4 M;
        ///uniform float SunStrength;
        ///uniform vec3 SunDirection;
        ///uniform vec4 SunColor;
        ///uniform float AmbientStrength;
        ///uniform vec4 AmbientColor;
        ///uniform sampler2D DiffuseTex;
        ///uniform sampler2D ShadowMap;
        ///uniform vec4 FogColor;
        ///uniform float FogStart;
        ///uniform float FogEnd;
        ///uniform float FogExponent;
        ///uniform float DepthBias;
        ///uniform [rest of string was truncated]&quot;;.
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
        ///uniform int renderShadowMap;
        ///uniform float ZNear;
        ///uniform float ZFar;
        ///
        ///float LinearizeDepth(float z)
        ///{
        ///  return (2.0 * ZNear) / (ZFar + ZNear - z * (ZFar- ZNear));	
        ///}
        ///
        ///void main()
        ///{
        ///    // this is for shadowmap
        ///    if(renderShadowMap == 1)
        ///    {
        ///        float color = LinearizeDepth(texture(screenTexture, TexCoords).r);
        ///        FragColor = vec4(color, color, color, 1.0);
        ///    }
        ///    else
        ///    {
        ///        // this is [rest of string was truncated]&quot;;.
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
        ///
        ///out vec4 color;
        ///
        ///uniform mat4 M;
        ///
        ///uniform int GridSize;
        ///uniform vec4 GridColor;
        ///uniform float SunStrength;
        ///uniform vec3 SunDirection;
        ///uniform vec4 SunColor;
        ///uniform float AmbientStrength;
        ///uniform vec4 AmbientColor;
        ///uniform vec4 FogColor;
        ///uniform float FogStart;
        ///uniform float FogEnd;
        ///uniform float FogExponent;
        ///uniform float ShadowDepth;
        ///
        ///uniform sampler2DArray myTexture [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_hmap {
            get {
                return ResourceManager.GetString("fshader_hmap", resourceCulture);
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
        ///out vec4 col;
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
        ///    col = vec4(depth, moment2, 1.0, 1.0);
        ///}.
        /// </summary>
        internal static string fshader_shadowmap {
            get {
                return ResourceManager.GetString("fshader_shadowmap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to out vec4 FragColor;
        ///  
        ///in vec2 TexCoords;
        ///
        ///uniform sampler2D image;
        ///  
        ///uniform bool horizontal;
        ///uniform float weight[3] = float[] (0.250301, 0.221461, 0.153388);
        ///
        ///void main()
        ///{             
        ///    vec2 tex_offset = 1.0 / textureSize(image, 0); // gets size of single texel
        ///    vec2 result = texture(image, TexCoords).rg * weight[0]; // current fragment&apos;s contribution
        ///    if(horizontal)
        ///    {
        ///        for(int i = 1; i &lt; 3; ++i)
        ///        {
        ///            result += texture(image, TexCoords + vec2(tex_of [rest of string was truncated]&quot;;.
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
        ///in vec4 fragmentPositionLightSpace;
        ///
        ///out vec4 color;
        ///
        ///uniform mat4 M;
        ///uniform float SunStrength;
        ///uniform vec3 SunDirection;
        ///uniform vec4 SunColor;
        ///uniform float AmbientStrength;
        ///uniform vec4 AmbientColor;
        ///uniform sampler2D DiffuseTex;
        ///uniform sampler2D ShadowMap;
        ///uniform vec4 FogColor;
        ///uniform float FogStart;
        ///uniform float FogEnd;
        ///uniform float FogExponent;
        ///uniform float ShadowDepth;
        ///
        ///vec2 poissonDisk[4] = vec2[](
        ///  vec2( -0 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string fshader_skel {
            get {
                return ResourceManager.GetString("fshader_skel", resourceCulture);
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
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap hmap_icon {
            get {
                object obj = ResourceManager.GetObject("hmap_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap lake_icon {
            get {
                object obj = ResourceManager.GetObject("lake_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap metadata_icon {
            get {
                object obj = ResourceManager.GetObject("metadata_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap monument_icon {
            get {
                object obj = ResourceManager.GetObject("monument_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap npc_icon {
            get {
                object obj = ResourceManager.GetObject("npc_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap object_icon {
            get {
                object obj = ResourceManager.GetObject("object_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap texture_icon {
            get {
                object obj = ResourceManager.GetObject("texture_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap unit_icon {
            get {
                object obj = ResourceManager.GetObject("unit_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
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
        ///out vec4 fragmentPositionLightSpace;
        ///out mat4 M;
        ///
        ///// Values that stay constant for the whole mesh.
        /////uniform mat4 M [rest of string was truncated]&quot;;.
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
        ///layout(location = 1) in vec3 vertexNormal;
        /////layout(location = 2) in vec3 texID;
        /////layout(location = 3) in vec3 texWeight;
        ///
        ///out vec3 fragmentPosition;
        ///out vec2 UV;
        ///out vec3 fragmentNormal;
        /////flat out vec3 textureID;
        ///out vec4 fragmentPositionLightSpace;
        ///out vec3 vpos_orig;
        ///
        ///// Values that stay constant for the whole mesh.
        ///uniform mat4 VP;
        ///uniform mat4 LSM;
        ///uniform mat4 M; [rest of string was truncated]&quot;;.
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
        ///layout(location = 1) in vec3 vertexNormal;
        ///layout(location = 2) in vec2 vertexUV;
        ///
        ///out vec2 UV;
        ///
        ///// Values that stay constant for the whole mesh.
        ///uniform mat4 LSM;
        ///uniform mat4 M;
        ///  
        ///void main(){
        ///  // Output position of the vertex, in clip space : MVP * position
        ///  gl_Position = LSM * M* vec4(vertexPosition_modelspace,1);
        ///  UV = vertexUV;
        ///}.
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
        ///out vec4 fragmentPositionLightSpace;
        ///
        ///// Values that stay constant for the whole mesh.
        ///uniform mat4 P;
        ///uniform mat4 V;
        ///uniform mat4 L [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vshader_skel {
            get {
                return ResourceManager.GetString("vshader_skel", resourceCulture);
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
