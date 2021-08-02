/*
 * SFRenderEngine takes care of displaying 3D graphics in a window
 * It takes data from SFScene and renders it using predefined shaders
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SpellforceDataEditor.SF3D;
using SpellforceDataEditor.SF3D.SceneSynchro;
using SpellforceDataEditor.SF3D.UI;
using SpellforceDataEditor.SFUnPak;
using SpellforceDataEditor.SFMap;

namespace SpellforceDataEditor.SF3D.SFRender
{
    public static class SFRenderEngine
    {
        public enum RenderPass { NONE = -1, SHADOWMAP = 0, SCENE = 1, SCREENSPACE = 2, UI = 3 }
        public static SFScene scene { get; } = new SFScene();
        public static UIManager ui { get; } = new UIManager();

        public static float min_render_distance = 0.1f;
        public static float max_render_distance = 1000f;

        static float CurrentDepthBias = 0;
        static RenderMode CurrentRenderMode = RenderMode.SRCALPHA_INVSRCALPHA;
        static bool CurrentApplyShadow = true;
        static bool CurrentDistanceFade = true;
        static bool CurrentApplyShading = true;
        static float CurrentFadeStart = 150.0f;
        static float CurrentFadeEnd = 200.0f;

        public static SFTexture opaque_tex { get; private set; } = null;

        static SFShader shader_simple = new SFShader();
        static SFShader shader_simple_transparency = new SFShader();
        static SFShader shader_animated = new SFShader();
        static SFShader shader_heightmap = new SFShader();
        static SFShader shader_heightmap_depth_prepass = new SFShader();
        static SFShader shader_shadowmap = new SFShader();
        static SFShader shader_shadowmap_animated = new SFShader();
        static SFShader shader_shadowmap_heightmap = new SFShader();
        static SFShader shader_sky = new SFShader();
        static SFShader shader_ui = new SFShader();
        static SFShader active_shader = null;
        static RenderPass current_pass = RenderPass.NONE;


        static SFShader shader_framebuffer_simple = new SFShader();
        static SFShader shader_framebuffer_tonemapped = new SFShader();
        static SFShader shader_shadowmap_blur = new SFShader();

        static FrameBuffer shadowmap_depth = null;
        static FrameBuffer shadowmap_depth_helper = null;
        static FrameBuffer screenspace_framebuffer = null;
        static FrameBuffer screenspace_intermediate = null;
        public static bool render_shadowmap_depth = false;

        public static Vector2 render_size = Vector2.Zero;

        public static bool is_rendering = false;
        static bool initialized = false;

        private static void DebugCallback(DebugSource source,
                                    DebugType type,
                                    int id,
                                    DebugSeverity severity,
                                    int length,
                                    IntPtr message,
                                    IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            Console.WriteLine($"{Enum.GetName(typeof(DebugSeverity), severity)}" +
               $" {Enum.GetName(typeof(DebugType), type) } | {messageString}");

            if (type == DebugType.DebugTypeError)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "OpenGL API error: " + messageString);
                throw new Exception("OpenGL API error! Commencing shutdown...");
            }
        }

        private static DebugProc _debugProcCallback = DebugCallback;
        private static GCHandle _debugProcCallbackHandle;
        public static void Initialize(Vector2 view_size)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.Initialize() called");

            // this happens only once per program run
            if (!initialized)
            {
                // setup GL debug info
                _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);

                GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);

                // get max possible anisotropy
                GetPName MAX_TEXTURE_MAX_ANISOTROPY = (GetPName)0x84FF;
                Settings.MaxAnisotropy = (int)GL.GetFloat(MAX_TEXTURE_MAX_ANISOTROPY);
            }

            // initialize static model cache
            if (SFSubModel3D.Cache != null)
                SFSubModel3D.Cache.Dispose();
            SFSubModel3D.Cache = new MeshCache(true);
            SFSubModel3D.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // positions
            SFSubModel3D.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // normals
            SFSubModel3D.Cache.AddVertexAttribute(4, VertexAttribPointerType.UnsignedByte, true);   // colors
            SFSubModel3D.Cache.AddVertexAttribute(2, VertexAttribPointerType.Float, false);   // UVs
            SFSubModel3D.Cache.SetVertexSize(40);                                             // extra 4 bytes are unused, but still pushed to gpu
            SFSubModel3D.Cache.Init(2 << 14, 2 << 14);

            // initialize animated (skin) model cache
            if (SFModelSkinChunk.Cache != null)
                SFModelSkinChunk.Cache.Dispose();
            SFModelSkinChunk.Cache = new MeshCache(false);
            SFModelSkinChunk.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // positions
            SFModelSkinChunk.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // normals
            SFModelSkinChunk.Cache.AddVertexAttribute(4, VertexAttribPointerType.UnsignedByte, true);   // bone weights
            SFModelSkinChunk.Cache.AddVertexAttribute(2, VertexAttribPointerType.Float, false);   // uvs
            SFModelSkinChunk.Cache.AddVertexAttribute(4, VertexAttribPointerType.UnsignedByte, false);  // bone indices
            SFModelSkinChunk.Cache.SetVertexSize(40);
            SFModelSkinChunk.Cache.Init(2 << 6, 2 << 6);

            scene.atmosphere.FogStart = Settings.FogStart;
            scene.atmosphere.FogEnd = Settings.FogEnd;

            // set up shadow shaders
            if (Settings.EnableShadows)
            {
                // shader compilation
                shader_shadowmap.CompileShader(new ShaderInfo[]
                {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_shadowmap },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_shadowmap }
                });
                shader_shadowmap.AddParameter("LSM");
                shader_shadowmap.AddParameter("DiffuseTexture");

                shader_shadowmap_animated.CompileShader(new ShaderInfo[]
                {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_shadowmap_animated },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_shadowmap }
                });
                shader_shadowmap_animated.AddParameter("LSM");
                shader_shadowmap_animated.AddParameter("M");
                shader_shadowmap_animated.AddParameter("boneTransforms");
                shader_shadowmap_animated.AddParameter("DiffuseTexture");

                if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
                    shader_shadowmap_heightmap.CompileShader(new ShaderInfo[]
                    {
                    new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_shadowmap_heightmap },
                    new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_shadowmap }
                    });
                else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
                {
                    shader_shadowmap_heightmap.CompileShader(new ShaderInfo[]
                    {
                    new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_hmap_tesselated },
                    new ShaderInfo() { type = ShaderType.TessControlShader, data = Properties.Resources.tcsshader_hmap_tesselated },
                    new ShaderInfo() { type = ShaderType.TessEvaluationShader, data = Properties.Resources.tesshader_hmap_shadowmap_tesselated },
                    new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_shadowmap }
                    });
                    shader_shadowmap_heightmap.AddParameter("cameraPos");
                }
                shader_shadowmap_heightmap.AddParameter("LSM");
                shader_shadowmap_heightmap.AddParameter("GridSize");
                shader_shadowmap_heightmap.AddParameter("HeightMap");
                shader_shadowmap_heightmap.AddParameter("DiffuseTexture");

                shader_shadowmap_blur.CompileShader(new ShaderInfo[]
                {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_framebuffer },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_shadowmap_blur }
                });
                shader_shadowmap_blur.AddParameter("image");
                shader_shadowmap_blur.AddParameter("horizontal");
            }

            shader_framebuffer_simple.CompileShader(new ShaderInfo[]
            {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_framebuffer },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_framebuffer_simple }
            });
            shader_framebuffer_simple.AddParameter("screenTexture");

            // set up eye candy shaders (tonemap, sky)
            if (Settings.ToneMapping)
            {
                shader_framebuffer_tonemapped.CompileShader(new ShaderInfo[]
                {
                    new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_framebuffer },
                    new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_tonemap }
                });
                shader_framebuffer_tonemapped.AddParameter("exposure");

                shader_sky.CompileShader(new ShaderInfo[]
                {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_sky },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_sky }
                });
                shader_sky.AddParameter("V");
                shader_sky.AddParameter("AspectRatio");
                shader_sky.AddParameter("AmbientStrength");
                shader_sky.AddParameter("AmbientColor");
                shader_sky.AddParameter("FogStrength");
                shader_sky.AddParameter("FogColor");
            }


            shader_ui.CompileShader(new ShaderInfo[]
            {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_ui },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_ui }
            });
            shader_ui.AddParameter("Tex");
            shader_ui.AddParameter("ScreenSize");
            shader_ui.AddParameter("offset");

            RecompileMainShaders();

            // set up main gl state
            GL.Enable(EnableCap.Blend);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // prepare view and framebuffers
            if (screenspace_intermediate != null)
            {
                if ((Settings.EnableShadows) && (shadowmap_depth != null))
                {
                    shadowmap_depth.Dispose();
                    shadowmap_depth_helper.Dispose();
                }
                if (Settings.AntiAliasingSamples > 1)
                    screenspace_framebuffer.Dispose();
                screenspace_intermediate.Dispose();

                screenspace_framebuffer = null;
                screenspace_intermediate = null;
                shadowmap_depth = null;
                shadowmap_depth_helper = null;
            }

            render_size = view_size;
            ResizeView(view_size);

            if (Settings.EnableShadows)
            {
                // shadowmap framebuffer - shadows are drawn into this buffer
                // after that, this is horizontally blurred and stored in shadowmap_depth_helper
                shadowmap_depth = new FrameBuffer(
                    Settings.ShadowMapSize,
                    Settings.ShadowMapSize,
                    new FrameBufferColorAttachmentInfo[]
                    {
                    new FrameBufferColorAttachmentInfo()
                    {
                        format = PixelFormat.Rgba,
                        internal_format = PixelInternalFormat.Rg32f,
                        pixeltype = PixelType.Float
                    }
                    },
                    0,
                    new FrameBufferDepthInfo() { use_depth = true, parent_depth = null });
                GL.BindTexture(TextureTarget.Texture2D, shadowmap_depth.texture_colors[0]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
                float[] col = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, col);

                // shadowmap helper framebuffer - horizontal blur result is stored here
                // after that, vertical blur is performed and drawn back onto shadowmap_depth
                shadowmap_depth_helper = new FrameBuffer(
                    Settings.ShadowMapSize/2,
                    Settings.ShadowMapSize/2,
                    new FrameBufferColorAttachmentInfo[]
                    {
                    new FrameBufferColorAttachmentInfo()
                    {
                        format = PixelFormat.Rgba,
                        internal_format = PixelInternalFormat.Rg32f,
                        pixeltype = PixelType.Float
                    }
                    },
                    0,
                    new FrameBufferDepthInfo() { use_depth = true, parent_depth = null });
                GL.BindTexture(TextureTarget.Texture2D, shadowmap_depth_helper.texture_colors[0]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, col);
            }

            List<FrameBufferColorAttachmentInfo> col_attachments = new List<FrameBufferColorAttachmentInfo>();
            col_attachments.Add(new FrameBufferColorAttachmentInfo()
            {
                format = PixelFormat.Rgb,
                internal_format = (Settings.ToneMapping ? PixelInternalFormat.Rgba16f : PixelInternalFormat.Rgb),
                pixeltype = (Settings.ToneMapping ? PixelType.Float : PixelType.UnsignedByte)
            });

            // anti-aliased framebuffer - everything is drawn here if anti-aliasing is enabled
            // if anything got drawn into it, this is later copied into screenspace_intermediate
            if (Settings.AntiAliasingSamples > 1)
            {
                screenspace_framebuffer = new FrameBuffer(
                    (int)view_size.X,
                    (int)view_size.Y,
                    col_attachments.ToArray(),
                    Settings.AntiAliasingSamples,
                    new FrameBufferDepthInfo() { use_depth = true, parent_depth = null });
            }

            // this framebuffer stores result of anti-aliased framebuffer (if anti-aliasing is disabled, everything is drawn here)
            // tonemapping is performed in this framebuffer
            screenspace_intermediate = new FrameBuffer(
                (int)view_size.X,
                (int)view_size.Y,
                col_attachments.ToArray(),
                0,
                new FrameBufferDepthInfo() { use_depth = true, parent_depth = null });

            // opaque texture is a 1x1 white pixel that's used for blending operations on models that would otherwise have no texture assigned
            if (opaque_tex != null)
                opaque_tex.Dispose();

            opaque_tex = new SFTexture();
            byte[] tex_data = new byte[] { 255, 255, 255, 255 };
            using (MemoryStream ms = new MemoryStream(tex_data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    opaque_tex.LoadUncompressedRGBA(br, 1, 1, 0);
                    opaque_tex.Init();
                }
            }

            initialized = true;

            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.Initialize(): GPU used: " + GL.GetString(StringName.Renderer));
        }

        // recompiles certain shaders, uses Settings utility to conditionally assemble shader code
        public static void RecompileMainShaders()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.RecompileMainShaders() called");
            shader_simple.SetDefine("SHADING", (Settings.ShadingQuality > 0));
            shader_simple.SetDefine("QUALITY_SHADING", (Settings.ShadingQuality > 1));
            shader_simple.SetDefine("SHADOWS", (Settings.EnableShadows));
            shader_simple.SetDefine("TONEMAPPING", (Settings.ToneMapping));
            shader_simple_transparency.SetDefine("SHADING", (Settings.ShadingQuality > 0));
            shader_simple_transparency.SetDefine("QUALITY_SHADING", (Settings.ShadingQuality > 1));
            shader_simple_transparency.SetDefine("SHADOWS", (Settings.EnableShadows));
            shader_simple_transparency.SetDefine("TONEMAPPING", (Settings.ToneMapping));
            shader_animated.SetDefine("SHADING", (Settings.ShadingQuality > 0));
            shader_animated.SetDefine("QUALITY_SHADING", (Settings.ShadingQuality > 1));
            shader_animated.SetDefine("SHADOWS", (Settings.EnableShadows));
            shader_animated.SetDefine("TONEMAPPING", (Settings.ToneMapping));
            shader_heightmap.SetDefine("DISPLAY_GRID", Settings.DisplayGrid);
            shader_heightmap.SetDefine("VISUALIZE_HEIGHT", Settings.VisualizeHeight);
            shader_heightmap.SetDefine("SHADING", (Settings.ShadingQuality > 0));
            shader_heightmap.SetDefine("SHADOWS", (Settings.EnableShadows));
            shader_heightmap.SetDefine("TONEMAPPING", (Settings.ToneMapping));
            shader_heightmap.SetDefine("TEXTURE_LOD", (Settings.TerrainTextureLOD));

            shader_simple.CompileShader(new ShaderInfo[]
            {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader }
            });
            shader_simple.AddParameter("VP");
            // shader_simple.AddParameter("M");
            if (Settings.EnableShadows)
                shader_simple.AddParameter("LSM");
            shader_simple.AddParameter("DiffuseTex");
            shader_simple.AddParameter("ShadowMap");
            shader_simple.AddParameter("SunDirection");
            shader_simple.AddParameter("SunStrength");
            shader_simple.AddParameter("SunColor");
            shader_simple.AddParameter("AmbientStrength");
            shader_simple.AddParameter("AmbientColor");
            shader_simple.AddParameter("FogColor");
            shader_simple.AddParameter("FogStrength");
            shader_simple.AddParameter("FogStart");
            shader_simple.AddParameter("FogEnd");
            shader_simple.AddParameter("FogExponent");
            shader_simple.AddParameter("DepthBias");
            shader_simple.AddParameter("ObjectFadeStart");
            shader_simple.AddParameter("ObjectFadeEnd");
            shader_simple.AddParameter("ShadowDepth");
            shader_simple.AddParameter("ApplyShadow");
            shader_simple.AddParameter("ApplyShading");
            shader_simple.AddParameter("DistanceFade");

            shader_simple_transparency.CompileShader(new ShaderInfo[]
{
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_transparent }
});
            shader_simple_transparency.AddParameter("VP");
            if (Settings.EnableShadows)
                shader_simple_transparency.AddParameter("LSM");
            shader_simple_transparency.AddParameter("DiffuseTex");
            shader_simple_transparency.AddParameter("ShadowMap");
            shader_simple_transparency.AddParameter("SunDirection");
            shader_simple_transparency.AddParameter("SunStrength");
            shader_simple_transparency.AddParameter("SunColor");
            shader_simple_transparency.AddParameter("AmbientStrength");
            shader_simple_transparency.AddParameter("AmbientColor");
            shader_simple_transparency.AddParameter("FogColor");
            shader_simple_transparency.AddParameter("FogStrength");
            shader_simple_transparency.AddParameter("FogStart");
            shader_simple_transparency.AddParameter("FogEnd");
            shader_simple_transparency.AddParameter("FogExponent");
            shader_simple_transparency.AddParameter("DepthBias");
            shader_simple_transparency.AddParameter("ObjectFadeStart");
            shader_simple_transparency.AddParameter("ObjectFadeEnd");
            shader_simple_transparency.AddParameter("ShadowDepth");
            shader_simple_transparency.AddParameter("ApplyShadow");
            shader_simple_transparency.AddParameter("ApplyShading");
            shader_simple_transparency.AddParameter("DistanceFade");

            shader_animated.CompileShader(new ShaderInfo[]
            {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_skel },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_skel }
            });
            shader_animated.AddParameter("P");
            shader_animated.AddParameter("V");
            shader_animated.AddParameter("M");
            if (Settings.EnableShadows)
                shader_animated.AddParameter("LSM");
            shader_animated.AddParameter("DiffuseTex");
            shader_animated.AddParameter("ShadowMap");
            shader_animated.AddParameter("boneTransforms");
            shader_animated.AddParameter("SunDirection");
            shader_animated.AddParameter("SunStrength");
            shader_animated.AddParameter("SunColor");
            shader_animated.AddParameter("AmbientStrength");
            shader_animated.AddParameter("AmbientColor");
            shader_animated.AddParameter("FogColor");
            shader_animated.AddParameter("FogStrength");
            shader_animated.AddParameter("FogStart");
            shader_animated.AddParameter("FogEnd");
            shader_animated.AddParameter("FogExponent");
            shader_animated.AddParameter("ShadowDepth");

            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
                shader_heightmap.CompileShader(new ShaderInfo[]
                {
                    new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_hmap },
                    new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_hmap }
                });
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                shader_heightmap.CompileShader(new ShaderInfo[]
                {
                    new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_hmap_tesselated },
                    new ShaderInfo() { type = ShaderType.TessControlShader, data = Properties.Resources.tcsshader_hmap_tesselated },
                    new ShaderInfo() { type = ShaderType.TessEvaluationShader, data = Properties.Resources.tesshader_hmap_tesselated },
                    new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_hmap }
                });
                shader_heightmap.AddParameter("cameraPos");
            }
            shader_heightmap.AddParameter("VP");
            shader_heightmap.AddParameter("GridSize");
            shader_heightmap.AddParameter("GridColor");
            shader_heightmap.AddParameter("LSM");
            shader_heightmap.AddParameter("myTextureSampler");
            shader_heightmap.AddParameter("HeightMap");
            shader_heightmap.AddParameter("ShadowMap");
            shader_heightmap.AddParameter("TileMap");
            shader_heightmap.AddParameter("OverlayMap");
            shader_heightmap.AddParameter("BumpMap");
            shader_heightmap.AddParameter("SunDirection");
            shader_heightmap.AddParameter("SunStrength");
            shader_heightmap.AddParameter("SunColor");
            shader_heightmap.AddParameter("AmbientStrength");
            shader_heightmap.AddParameter("AmbientColor");
            shader_heightmap.AddParameter("FogColor");
            shader_heightmap.AddParameter("FogStrength");
            shader_heightmap.AddParameter("FogStart");
            shader_heightmap.AddParameter("FogEnd");
            shader_heightmap.AddParameter("FogExponent");
            shader_heightmap.AddParameter("ShadowFadeStart");
            shader_heightmap.AddParameter("ShadowFadeEnd");
            // tiledata ubo binding
            int uniform_tiledata = GL.GetUniformBlockIndex(shader_heightmap.ProgramID, "Tiles");
            GL.UniformBlockBinding(shader_heightmap.ProgramID, uniform_tiledata, 0);
            int uniform_overlaycol = GL.GetUniformBlockIndex(shader_heightmap.ProgramID, "Overlays");
            GL.UniformBlockBinding(shader_heightmap.ProgramID, uniform_overlaycol, 1);
            int uniform_tilecol = GL.GetUniformBlockIndex(shader_heightmap.ProgramID, "TileColors");
            GL.UniformBlockBinding(shader_heightmap.ProgramID, uniform_tilecol, 2);


            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
                shader_heightmap_depth_prepass.CompileShader(new ShaderInfo[]
                {
                    new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_hmap_depth_prepass },
                    new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_hmap_depth_prepass }
                });
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                shader_heightmap_depth_prepass.CompileShader(new ShaderInfo[]
                {
                    new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_hmap_tesselated },
                    new ShaderInfo() { type = ShaderType.TessControlShader, data = Properties.Resources.tcsshader_hmap_tesselated },
                    new ShaderInfo() { type = ShaderType.TessEvaluationShader, data = Properties.Resources.tesshader_hmap_tesselated_depth_prepass },
                    new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_hmap_depth_prepass }
                });
                shader_heightmap_depth_prepass.AddParameter("cameraPos");
            }

            shader_heightmap_depth_prepass.AddParameter("VP");
            shader_heightmap_depth_prepass.AddParameter("GridSize");
            shader_heightmap_depth_prepass.AddParameter("HeightMap");

            ApplyLight();
            ApplyTexturingUnits();
            ApplyFade();
        }

        // resizes render view to given size
        public static void ResizeView(Vector2 view_size)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.ResizeView() called (view_size = " + view_size.ToString() + ")");
            render_size = view_size;
            scene.camera.ProjMatrix = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4, view_size.X / view_size.Y, min_render_distance, max_render_distance);
            scene.camera.AspectRatio = (float)(view_size.X) / view_size.Y;
            GL.Viewport(0, 0, (int)view_size.X, (int)view_size.Y);
            if (screenspace_intermediate != null)
            {
                if (Settings.AntiAliasingSamples > 1)
                    screenspace_framebuffer.Resize((int)view_size.X, (int)view_size.Y);
                screenspace_intermediate.Resize((int)view_size.X, (int)view_size.Y);
            }
        }

        // updates shader lighting parameters
        public static void ApplyLight()
        {
            GL.UseProgram(shader_simple.ProgramID);
            GL.Uniform1(shader_simple["SunStrength"], scene.atmosphere.sun_light.Strength);
            GL.Uniform3(shader_simple["SunDirection"], scene.atmosphere.sun_light.Direction);
            GL.Uniform4(shader_simple["SunColor"], scene.atmosphere.sun_light.Color);
            GL.Uniform1(shader_simple["AmbientStrength"], scene.atmosphere.ambient_light.Strength);
            GL.Uniform4(shader_simple["AmbientColor"], scene.atmosphere.ambient_light.Color);
            GL.Uniform4(shader_simple["FogColor"], scene.atmosphere.FogColor);
            GL.Uniform1(shader_simple["FogStrength"], scene.atmosphere.FogStrength);
            GL.Uniform1(shader_simple["FogStart"], scene.atmosphere.FogStart);
            GL.Uniform1(shader_simple["FogEnd"], scene.atmosphere.FogEnd);
            GL.Uniform1(shader_simple["FogExponent"], scene.atmosphere.FogExponent);

            GL.UseProgram(shader_simple_transparency.ProgramID);
            GL.Uniform1(shader_simple_transparency["SunStrength"], scene.atmosphere.sun_light.Strength);
            GL.Uniform3(shader_simple_transparency["SunDirection"], scene.atmosphere.sun_light.Direction);
            GL.Uniform4(shader_simple_transparency["SunColor"], scene.atmosphere.sun_light.Color);
            GL.Uniform1(shader_simple_transparency["AmbientStrength"], scene.atmosphere.ambient_light.Strength);
            GL.Uniform4(shader_simple_transparency["AmbientColor"], scene.atmosphere.ambient_light.Color);
            GL.Uniform4(shader_simple_transparency["FogColor"], scene.atmosphere.FogColor);
            GL.Uniform1(shader_simple_transparency["FogStrength"], scene.atmosphere.FogStrength);
            GL.Uniform1(shader_simple_transparency["FogStart"], scene.atmosphere.FogStart);
            GL.Uniform1(shader_simple_transparency["FogEnd"], scene.atmosphere.FogEnd);
            GL.Uniform1(shader_simple_transparency["FogExponent"], scene.atmosphere.FogExponent);

            GL.UseProgram(shader_animated.ProgramID);
            GL.Uniform1(shader_animated["SunStrength"], scene.atmosphere.sun_light.Strength);
            GL.Uniform3(shader_animated["SunDirection"], scene.atmosphere.sun_light.Direction);
            GL.Uniform4(shader_animated["SunColor"], scene.atmosphere.sun_light.Color);
            GL.Uniform1(shader_animated["AmbientStrength"], scene.atmosphere.ambient_light.Strength);
            GL.Uniform4(shader_animated["AmbientColor"], scene.atmosphere.ambient_light.Color);
            GL.Uniform4(shader_animated["FogColor"], scene.atmosphere.FogColor);
            GL.Uniform1(shader_animated["FogStrength"], scene.atmosphere.FogStrength);
            GL.Uniform1(shader_animated["FogStart"], scene.atmosphere.FogStart);
            GL.Uniform1(shader_animated["FogEnd"], scene.atmosphere.FogEnd);
            GL.Uniform1(shader_animated["FogExponent"], scene.atmosphere.FogExponent);

            GL.UseProgram(shader_heightmap.ProgramID);
            GL.Uniform1(shader_heightmap["SunStrength"], scene.atmosphere.sun_light.Strength);
            GL.Uniform3(shader_heightmap["SunDirection"], scene.atmosphere.sun_light.Direction);
            GL.Uniform4(shader_heightmap["SunColor"], scene.atmosphere.sun_light.Color);
            GL.Uniform1(shader_heightmap["AmbientStrength"], scene.atmosphere.ambient_light.Strength);
            GL.Uniform4(shader_heightmap["AmbientColor"], scene.atmosphere.ambient_light.Color);
            GL.Uniform4(shader_heightmap["FogColor"], scene.atmosphere.FogColor);
            GL.Uniform1(shader_heightmap["FogStrength"], scene.atmosphere.FogStrength);
            GL.Uniform1(shader_heightmap["FogStart"], scene.atmosphere.FogStart);
            GL.Uniform1(shader_heightmap["FogEnd"], scene.atmosphere.FogEnd);
            GL.Uniform1(shader_heightmap["FogExponent"], scene.atmosphere.FogExponent);

            if (Settings.ToneMapping)
            {
                GL.UseProgram(shader_sky.ProgramID);
                GL.Uniform1(shader_sky["AmbientStrength"], scene.atmosphere.ambient_light.Strength);
                GL.Uniform4(shader_sky["AmbientColor"], scene.atmosphere.ambient_light.Color);
                GL.Uniform4(shader_sky["FogColor"], scene.atmosphere.FogColor);
                GL.Uniform1(shader_sky["FogStrength"], scene.atmosphere.FogStrength);
            }

            GL.UseProgram(0);
        }

        // assigns texture slots to shaders
        private static void ApplyTexturingUnits()
        {
            GL.UseProgram(shader_simple.ProgramID);
            GL.Uniform1(shader_simple["DiffuseTex"], 0);
            GL.Uniform1(shader_simple["ShadowMap"], 1);

            GL.UseProgram(shader_simple_transparency.ProgramID);
            GL.Uniform1(shader_simple_transparency["DiffuseTex"], 0);
            GL.Uniform1(shader_simple_transparency["ShadowMap"], 1);

            GL.UseProgram(shader_animated.ProgramID);
            GL.Uniform1(shader_animated["DiffuseTex"], 0);
            GL.Uniform1(shader_animated["ShadowMap"], 1);

            GL.UseProgram(shader_heightmap.ProgramID);
            GL.Uniform1(shader_heightmap["myTextureSampler"], 0);
            GL.Uniform1(shader_heightmap["ShadowMap"], 1);
            GL.Uniform1(shader_heightmap["TileMap"], 2);
            GL.Uniform1(shader_heightmap["OverlayMap"], 3);
            GL.Uniform1(shader_heightmap["HeightMap"], 4);
            GL.Uniform1(shader_heightmap["BumpMap"], 5);

            GL.UseProgram(shader_heightmap_depth_prepass.ProgramID);
            GL.Uniform1(shader_heightmap_depth_prepass["HeightMap"], 4);

            if (Settings.EnableShadows)
            {
                GL.UseProgram(shader_shadowmap.ProgramID);
                GL.Uniform1(shader_shadowmap["DiffuseTexture"], 0);

                GL.UseProgram(shader_shadowmap_animated.ProgramID);
                GL.Uniform1(shader_shadowmap_animated["DiffuseTexture"], 0);

                GL.UseProgram(shader_shadowmap_heightmap.ProgramID);
                GL.Uniform1(shader_shadowmap_heightmap["DiffuseTexture"], 0);
                GL.Uniform1(shader_shadowmap_heightmap["HeightMap"], 4);

                GL.UseProgram(shader_shadowmap_blur.ProgramID);
                GL.Uniform1(shader_shadowmap_blur["image"], 0);
            }

            GL.UseProgram(shader_ui.ProgramID);
            GL.Uniform1(shader_ui["Tex"], 0);

            GL.UseProgram(0);
        }

        public static void ApplyFade()
        {
            GL.UseProgram(shader_simple.ProgramID);
            GL.Uniform1(shader_simple["ObjectFadeStart"], CurrentFadeStart);
            GL.Uniform1(shader_simple["ObjectFadeEnd"], CurrentFadeEnd);
            GL.UseProgram(0);

            GL.UseProgram(shader_simple_transparency.ProgramID);
            GL.Uniform1(shader_simple_transparency["ObjectFadeStart"], CurrentFadeStart);
            GL.Uniform1(shader_simple_transparency["ObjectFadeEnd"], CurrentFadeEnd);
            GL.UseProgram(0);

            GL.UseProgram(shader_heightmap.ProgramID);
            GL.Uniform1(shader_heightmap["ShadowFadeStart"], CurrentFadeStart);
            GL.Uniform1(shader_heightmap["ShadowFadeEnd"], CurrentFadeEnd);
            GL.UseProgram(0);
        }

        public static void SetObjectFadeRange(float start, float end)
        {
            CurrentFadeStart = start;
            CurrentFadeEnd = end;
            ApplyFade();
        }

        private static void UseShader(SFShader shader)
        {
            if (shader == active_shader)
                return;
            active_shader = shader;
            if (shader != null)
                GL.UseProgram(shader.ProgramID);
            else
                GL.UseProgram(0);
        }

        private static void SetFramebuffer(FrameBuffer f)
        {
            if (f == null)
            {
                GL.Viewport(0, 0, (int)render_size.X, (int)render_size.Y);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
            else
            {
                GL.Viewport(0, 0, f.width, f.height);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, f.fbo);
            }
        }

        private static void SetDepthBias(float v)
        {
            if (v != CurrentDepthBias)
            {
                CurrentDepthBias = v;
                GL.Uniform1(active_shader["DepthBias"], v);
            }
        }

        private static void SetApplyShadow(bool b)
        {
            if (b != CurrentApplyShadow)
            {
                CurrentApplyShadow = b;
                GL.Uniform1(active_shader["ApplyShadow"], b ? 1 : 0);
            }
        }

        private static void SetApplyShading(bool b)
        {
            if (b != CurrentApplyShading)
            {
                CurrentApplyShading = b;
                GL.Uniform1(active_shader["ApplyShading"], b ? 1 : 0);
            }
        }

        private static void SetDistanceFade(bool b)
        {
            if (b != CurrentDistanceFade)
            {
                CurrentDistanceFade = b;
                GL.Uniform1(active_shader["DistanceFade"], b ? 1.0f : 0.0f);
            }
        }

        private static void SetRenderMode(RenderMode rm)
        {
            if (CurrentRenderMode == rm)
                return;

            switch (rm)
            {
                case RenderMode.DESTCOLOR_INVSRCALPHA:
                    GL.BlendFunc(BlendingFactor.DstColor, BlendingFactor.OneMinusSrcAlpha);
                    break;
                case RenderMode.DESTCOLOR_ONE:
                    GL.BlendFunc(BlendingFactor.DstColor, BlendingFactor.One);
                    break;
                case RenderMode.DESTCOLOR_SRCCOLOR:
                    GL.BlendFunc(BlendingFactor.DstColor, BlendingFactor.SrcColor);
                    break;
                case RenderMode.DESTCOLOR_ZERO:
                    GL.BlendFunc(BlendingFactor.DstColor, BlendingFactor.Zero);
                    break;
                case RenderMode.ONE_INVSRCALPHA:
                    GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                    break;
                case RenderMode.ONE_ONE:
                    GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
                    break;
                case RenderMode.ONE_ZERO:
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    break;
                case RenderMode.SRCALPHA_INVSRCALPHA:
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    break;
                case RenderMode.SRCALPHA_ONE:
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                    break;
                case RenderMode.ZERO_INVSRCCOLOR:
                    GL.BlendFunc(BlendingFactor.Zero, BlendingFactor.OneMinusSrcColor);
                    break;
            }

            CurrentRenderMode = rm;
        }

        // turns heightmap nodes visible/invisible depending on if theyre in camera frustum
        public static void UpdateVisibleChunks()
        {
            // 1. find collection of visible chunks
            List<SceneNodeMapChunk> vis_chunks = new List<SceneNodeMapChunk>();
            SFMapHeightMap heightmap = scene.map.heightmap;
            int chunks_per_row = (heightmap.width / SFMapHeightMapMesh.CHUNK_SIZE);
            int chunks_per_column = (heightmap.height / SFMapHeightMapMesh.CHUNK_SIZE);

            // approximation of the visible chunk area
            // this one is veeeery big overestimation
            // todo: cut the frustum by plane Y=0 before making bounding box out of it

            Physics.BoundingBox aabb = Physics.BoundingBox.FromPoints(scene.camera.Frustum.frustum_vertices);
            // get ix, iy of first and last chunk
            int ix1, ix2, iy1, iy2;

            SFMapHeightMapChunk chunk = null;
            SFCoord point;
            // get first chunk
            point = new SFCoord(Math.Max((short)0, Math.Min((short)(heightmap.width - 1), (short)(aabb.a.X))), Math.Max((short)0, Math.Min((short)(heightmap.height - 1), (short)(aabb.a.Z))));

            chunk = heightmap.GetChunk(point);
            ix1 = chunk.ix;
            iy1 = chunks_per_column - 1 - chunk.iy;

            // get last chunk   
            point = new SFCoord(Math.Max((short)0, Math.Min((short)(heightmap.width - 1), (short)(aabb.b.X))), Math.Max((short)0, Math.Min((short)(heightmap.height - 1), (short)(aabb.b.Z))));

            chunk = heightmap.GetChunk(point);
            ix2 = chunk.ix;
            iy2 = chunks_per_column - 1 - chunk.iy;

            float max_dist = 1.136f * Settings.ObjectFadeMax;
            Vector2 xz;
            for (int j = iy1; j <= iy2; j++)
            {
                for (int i = ix1; i <= ix2; i++)
                {
                    SceneNodeMapChunk chunk_node = heightmap.chunk_nodes[j * chunks_per_row + i];
                    xz = chunk_node.MapChunk.aabb.center.Xz - scene.camera.position.Xz;
                    chunk_node.DistanceToCamera = xz.Length;
                    chunk_node.CameraHeightDifference = scene.camera.position.Y - chunk_node.MapChunk.aabb.b.Y;
                    if (chunk_node.DistanceToCamera > max_dist)
                        continue;
                    if (!chunk_node.MapChunk.aabb.IsOutsideOfConvexHull(scene.camera.Frustum.frustum_planes))
                        vis_chunks.Add(chunk_node);
                }
            }

            // NOTE: chunks are already sorted due to how lists operate
            // 2. compare with existing collection of visible chunks: march two lists at once
            // chunk ID is chunk.iy*map.width/16+chunk.ix
            // any previously visible chunks which are now invisible turn off visibility of its objects
            // vice versa
            int next_list_id = 0, cur_list_id = 0;
            int next_chunk_id, cur_chunk_id;
            bool next_end = false, cur_end = false;
            while (true)
            {
                if (cur_list_id == heightmap.visible_chunks.Count)
                    cur_end = true;
                if (next_list_id == vis_chunks.Count)
                    next_end = true;
                if (next_end && cur_end)
                    break;

                if (cur_end)
                {
                    for (int i = next_list_id; i < vis_chunks.Count; i++)
                    {
                        vis_chunks[i].SetParent(scene.root);
                        vis_chunks[i].MapChunk.UpdateVisible(true);
                        // add visible to the next chunk
                    }
                    break;
                }
                if (next_end)
                {
                    for (int i = cur_list_id; i < heightmap.visible_chunks.Count; i++)
                    {
                        heightmap.visible_chunks[i].SetParent(null);
                        heightmap.visible_chunks[i].MapChunk.UpdateVisible(false);
                        // add invisible to the current chunk
                    }
                    break;
                }

                cur_chunk_id = heightmap.visible_chunks[cur_list_id].MapChunk.id;
                next_chunk_id = vis_chunks[next_list_id].MapChunk.id;
                // if next id > cur id, keep increasing cur id, while simultaneously turning chunks invisible
                // otherwise keep increasing next_id, while simultaneuosly turning chunks visible
                if (next_chunk_id > cur_chunk_id)
                {
                    while (next_chunk_id > cur_chunk_id)
                    {
                        heightmap.visible_chunks[cur_list_id].SetParent(null);
                        heightmap.visible_chunks[cur_list_id].MapChunk.UpdateVisible(false);
                        // turn chunk invisible

                        cur_list_id += 1;
                        if (cur_list_id == heightmap.visible_chunks.Count)
                            break;
                        cur_chunk_id = heightmap.visible_chunks[cur_list_id].MapChunk.id;
                    }
                }
                else if (next_chunk_id < cur_chunk_id)
                {
                    while (next_chunk_id < cur_chunk_id)
                    {
                        vis_chunks[next_list_id].SetParent(scene.root);
                        vis_chunks[next_list_id].MapChunk.UpdateVisible(true);
                        // turn chunk visible

                        next_list_id += 1;
                        if (next_list_id == vis_chunks.Count)
                            break;
                        next_chunk_id = vis_chunks[next_list_id].MapChunk.id;
                    }
                }
                else
                {
                    next_list_id += 1;
                    cur_list_id += 1;
                }
            }
            heightmap.visible_chunks = vis_chunks;

            // check decoration visibility
            foreach (SceneNodeMapChunk chunk_node in vis_chunks)
            {
                chunk_node.MapChunk.UpdateDecorationVisible(chunk_node.DistanceToCamera, chunk_node.CameraHeightDifference);
                chunk_node.MapChunk.UpdateUnitVisible(chunk_node.DistanceToCamera, chunk_node.CameraHeightDifference);
            }

            if(Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                heightmap.mesh_tesselated.Regenerate(heightmap, scene.camera.Frustum);
            }
        }

        static void RenderSky()
        {
            Matrix4 v_mat = scene.camera.ViewMatrix;
            GL.UniformMatrix4(active_shader["V"], true, ref v_mat);
            GL.Uniform1(active_shader["AspectRatio"], 1.0f);
            GL.BindVertexArray(Atmosphere.sky_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        static void RenderHeightmapDepthPrePass()
        {
            SFMapHeightMap heightmap = scene.map.heightmap;

            GL.Uniform1(active_shader["GridSize"], heightmap.width);
            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
                GL.BindVertexArray(heightmap.mesh.vertex_array);
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                GL.BindVertexArray(heightmap.mesh_tesselated.vertex_array);
                GL.Uniform3(active_shader["cameraPos"], scene.camera.position);
            }

            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, heightmap.height_data_texture);

            Matrix4 vp_mat = scene.camera.ViewProjMatrix;
            GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);

            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
            {
                GL.DrawElements(PrimitiveType.TriangleStrip, 2 * (heightmap.width + 1) * heightmap.height, DrawElementsType.UnsignedInt, 0);
            }
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                GL.DrawArrays(PrimitiveType.Patches, 0, 4 * heightmap.mesh_tesselated.patch_count);
            }

            GL.ActiveTexture(TextureUnit.Texture0);
        }

        static void RenderHeightmap()
        {
            SFMapHeightMap heightmap = scene.map.heightmap;

            Matrix4 lsm_mat = scene.atmosphere.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
            GL.Uniform1(active_shader["GridSize"], heightmap.width);
            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
                GL.BindVertexArray(heightmap.mesh.vertex_array);
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                GL.BindVertexArray(heightmap.mesh_tesselated.vertex_array);
                GL.Uniform3(active_shader["cameraPos"], scene.camera.position);
            }
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, heightmap.height_data_texture);
            if (current_pass == RenderPass.SCENE)
            {
                if (heightmap.overlay_active_texture != -1)
                {
                    GL.ActiveTexture(TextureUnit.Texture3);
                    GL.BindTexture(TextureTarget.Texture2D, heightmap.overlay_active_texture);
                }
                else
                {
                    GL.ActiveTexture(TextureUnit.Texture3);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }
                GL.ActiveTexture(TextureUnit.Texture5);
                GL.BindTexture(TextureTarget.Texture2D, heightmap.terrain_texture_lod_bump.tex_id);
                GL.ActiveTexture(TextureUnit.Texture2);
                GL.BindTexture(TextureTarget.Texture2D, heightmap.tile_data_texture);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2DArray, heightmap.texture_manager.terrain_texture);
                Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
                GL.Uniform4(active_shader["GridColor"], Settings.GridColor);
            }
            else if (current_pass == RenderPass.SHADOWMAP)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, opaque_tex.tex_id);
            }

            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
            {
                GL.DrawElements(PrimitiveType.TriangleStrip, 2 * (heightmap.width + 1) * heightmap.height, DrawElementsType.UnsignedInt, 0);
            }
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                GL.DrawArrays(PrimitiveType.Patches, 0, 4 * heightmap.mesh_tesselated.patch_count);
            }

            if(current_pass == RenderPass.SCENE)
                GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        // todo: sort transparent models
        static void RenderSimpleObjects()
        {
            Matrix4 lsm_mat = scene.atmosphere.sun_light.LightMatrix;
            if(Settings.EnableShadows)
                GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);

            if (current_pass == RenderPass.SCENE)
            {
                SetDepthBias(0);
                SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

                Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
                GL.Uniform1(active_shader["ShadowDepth"], scene.atmosphere.sun_light.ShadowDepth);
            }

            foreach (var submodel in scene.opaque_pass_models)
            {
                SFMaterial mat = submodel.material;

                int mii = submodel.cache_index;
                if (mii == Utility.NO_INDEX)
                    continue;

                if (current_pass == RenderPass.SCENE)
                {
                    SetApplyShading(mat.apply_shading);
                    SetApplyShadow(mat.apply_shadow);
                    SetDistanceFade(mat.distance_fade);
                    SetRenderMode(mat.texRenderMode);
                    if ((mat.matFlags & 4) == 0)
                        SetDepthBias(0);
                    else
                        SetDepthBias(mat.matDepthBias);
                }
                else if (current_pass == RenderPass.SHADOWMAP)
                {
                    if (!mat.casts_shadow)
                        continue;
                }

                GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);

                int vri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].VertexRangeIndex;
                int eri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].ElementRangeIndex;

                GL.DrawElementsInstancedBaseVertexBaseInstance(PrimitiveType.Triangles,
                    SFSubModel3D.Cache.ElementRanges[eri].Count,
                    DrawElementsType.UnsignedInt,
                    new IntPtr(SFSubModel3D.Cache.ElementRanges[eri].Start * 4),
                    submodel.owner.MatrixCount,
                    SFSubModel3D.Cache.VertexRanges[vri].Start,
                    submodel.owner.MatrixOffset);
            }
        }

        static void RenderWaterObjects()
        {
            Matrix4 lsm_mat = scene.atmosphere.sun_light.LightMatrix;
            if (Settings.EnableShadows)
                GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);

            SetDepthBias(0);
            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

            Matrix4 vp_mat = scene.camera.ViewProjMatrix;
            GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
            GL.Uniform1(active_shader["ShadowDepth"], scene.atmosphere.sun_light.ShadowDepth);

            GL.ActiveTexture(TextureUnit.Texture0);
            foreach (var submodel in scene.water_pass_models)
            {
                SFMaterial mat = submodel.material;

                int mii = submodel.cache_index;
                if (mii == Utility.NO_INDEX)
                    continue;

                SetApplyShading(mat.apply_shading);
                SetApplyShadow(mat.apply_shadow);
                SetDistanceFade(mat.distance_fade);
                SetRenderMode(mat.texRenderMode);
                if ((mat.matFlags & 4) == 0)
                    SetDepthBias(0);
                else
                    SetDepthBias(mat.matDepthBias);

                GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);

                int vri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].VertexRangeIndex;
                int eri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].ElementRangeIndex;

                GL.DrawElementsInstancedBaseVertexBaseInstance(PrimitiveType.Triangles,
                    SFSubModel3D.Cache.ElementRanges[eri].Count,
                    DrawElementsType.UnsignedInt,
                    new IntPtr(SFSubModel3D.Cache.ElementRanges[eri].Start * 4),
                    submodel.owner.MatrixCount,
                    SFSubModel3D.Cache.VertexRanges[vri].Start,
                    submodel.owner.MatrixOffset);
            }
        }

        static void RenderAdditiveObjects()
        {
            Matrix4 lsm_mat = scene.atmosphere.sun_light.LightMatrix;
            if (Settings.EnableShadows)
                GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);

            if (current_pass == RenderPass.SCENE)
            {
                SetDepthBias(0);
                SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

                Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
                GL.Uniform1(active_shader["ShadowDepth"], scene.atmosphere.sun_light.ShadowDepth);
            }

            // additive pass
            foreach (SFSubModel3D submodel in scene.additive_pass_models)
            {
                SFMaterial mat = submodel.material;

                int mii = submodel.cache_index;
                if (mii == Utility.NO_INDEX)
                    continue;

                if (current_pass == RenderPass.SCENE)
                {
                    SetApplyShading(mat.apply_shading);
                    SetApplyShadow(mat.apply_shadow);
                    SetDistanceFade(mat.distance_fade);
                    SetRenderMode(mat.texRenderMode);
                    if ((mat.matFlags & 4) == 0)
                        SetDepthBias(0);
                    else
                        SetDepthBias(mat.matDepthBias);
                }
                else if (current_pass == RenderPass.SHADOWMAP)
                {
                    if (!mat.casts_shadow)
                        continue;
                }

                GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);

                int vri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].VertexRangeIndex;
                int eri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].ElementRangeIndex;

                GL.DrawElementsInstancedBaseVertexBaseInstance(PrimitiveType.Triangles,
                    SFSubModel3D.Cache.ElementRanges[eri].Count,
                    DrawElementsType.UnsignedInt,
                    new IntPtr(SFSubModel3D.Cache.ElementRanges[eri].Start * 4),
                    submodel.owner.MatrixCount,
                    SFSubModel3D.Cache.VertexRanges[vri].Start,
                    submodel.owner.MatrixOffset);
            }
        }

        static void RenderTransparentObjects()
        {
            Matrix4 lsm_mat = scene.atmosphere.sun_light.LightMatrix;
            if (Settings.EnableShadows)
                GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
            Matrix4 vp_mat = scene.camera.ViewProjMatrix;
            GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);

            GL.Uniform1(active_shader["ShadowDepth"], scene.atmosphere.sun_light.ShadowDepth);

            // transparent pass
            foreach (SFSubModel3D submodel in scene.transparent_pass_models)
            {
                SFMaterial mat = submodel.material;

                int mii = submodel.cache_index;
                if (mii == Utility.NO_INDEX)
                    continue;

                //GL.Uniform1(active_shader["apply_shading"], sbm.material.matFlags);
                SetApplyShading(mat.apply_shading);
                SetApplyShadow(mat.apply_shadow);
                SetDistanceFade(mat.distance_fade);
                SetRenderMode(mat.texRenderMode);
                if ((mat.matFlags & 4) == 0)
                    SetDepthBias(0);
                else
                    SetDepthBias(mat.matDepthBias);

                GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);

                int vri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].VertexRangeIndex;
                int eri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].ElementRangeIndex;

                GL.DrawElementsInstancedBaseVertexBaseInstance(PrimitiveType.Triangles,
                    SFSubModel3D.Cache.ElementRanges[eri].Count,
                    DrawElementsType.UnsignedInt,
                    new IntPtr(SFSubModel3D.Cache.ElementRanges[eri].Start * 4),
                    submodel.owner.MatrixCount,
                    SFSubModel3D.Cache.VertexRanges[vri].Start,
                    submodel.owner.MatrixOffset);
            }
        }

        // this is very slow, dunno
        static void RenderAnimatedObjects()
        {
            Matrix4 lsm_mat = scene.atmosphere.sun_light.LightMatrix;
            if (Settings.EnableShadows)
                GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);

            if (current_pass == RenderPass.SCENE)
            {
                Matrix4 p_mat = scene.camera.ProjMatrix;
                GL.UniformMatrix4(active_shader["P"], false, ref p_mat);
                Matrix4 v_mat = scene.camera.ViewMatrix;
                GL.UniformMatrix4(active_shader["V"], false, ref v_mat);
                GL.Uniform1(active_shader["ShadowDepth"], scene.atmosphere.sun_light.ShadowDepth);
            }

            foreach (SceneNodeAnimated an in scene.an_nodes)
            {
                if (an.Skin == null)
                    continue;

                GL.UniformMatrix4(active_shader["M"], false, ref an.ResultTransform);
                GL.UniformMatrix4(active_shader["boneTransforms"], an.BoneTransforms.Length, false, ref an.BoneTransforms[0].Row0.X);

                for (int i = 0; i < an.Skin.submodels.Length; i++)
                {
                    var msc = an.Skin.submodels[i];
                    GL.BindTexture(TextureTarget.Texture2D, msc.material.texture.tex_id);

                    if (current_pass == RenderPass.SCENE)
                        SetRenderMode(msc.material.texRenderMode);

                    int vri = SFModelSkinChunk.Cache.Meshes[SFModelSkinChunk.Cache.MeshesIndex[msc.cache_index]].VertexRangeIndex;
                    int eri = SFModelSkinChunk.Cache.Meshes[SFModelSkinChunk.Cache.MeshesIndex[msc.cache_index]].ElementRangeIndex;

                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles,
                        SFModelSkinChunk.Cache.ElementRanges[eri].Count,
                        DrawElementsType.UnsignedInt,
                        new IntPtr(SFModelSkinChunk.Cache.ElementRanges[eri].Start * 4),
                        SFModelSkinChunk.Cache.VertexRanges[vri].Start);
                }
            }
        }


        public static void RenderUI()
        {
            GL.Uniform2(active_shader["ScreenSize"], ref render_size);

            foreach (SFTexture tex in ui.storages.Keys)
            {
                GL.BindTexture(TextureTarget.Texture2D, tex.tex_id);

                UIQuadStorage storage = ui.storages[tex];
                GL.BindVertexArray(storage.vertex_array);

                for (int i = 0; i < storage.spans.Count; i++)
                {
                    if (!storage.spans[i].visible)
                        continue;
                    if (storage.spans[i].used == 0)
                        continue;

                    GL.Uniform2(active_shader["offset"], storage.spans[i].position);
                    GL.DrawArrays(PrimitiveType.Triangles, storage.spans[i].start * 6, storage.spans[i].used * 6);
                }
            }
        }

        [Flags]
        enum RenderEnable
        {
            NONE = 0,
            SHADOWMAP_HMAP = 1,
            SHADOWMAP_ANIM = 2,
            SHADOWMAP_SIMPLE = 4,
            SHADOWMAP_BLUR = 8,
            SHADOWMAP = SHADOWMAP_HMAP | SHADOWMAP_ANIM | SHADOWMAP_SIMPLE | SHADOWMAP_BLUR,
            SKY = 16,
            HMAP = 32,
            ANIM = 64,
            SIMPLE = 128,
            WATER = 256,
            TRANSPARENT = 512,
            MAIN = SKY | HMAP | ANIM | SIMPLE | WATER | TRANSPARENT,
            TONEMAP = 1024,
            UI = 2048,
            POSTPROCESS = TONEMAP | UI,
            ALL = SHADOWMAP | MAIN | POSTPROCESS
        }

        public static void RenderScene()
        {
            RenderEnable enable_flags = RenderEnable.ALL;

            is_rendering = true;

            // generate shadowmap
            current_pass = RenderPass.SHADOWMAP;
            GL.Disable(EnableCap.Blend);


            if (Settings.EnableShadows)
            {
                SetFramebuffer(shadowmap_depth);
                GL.ClearColor(Color.White);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.Enable(EnableCap.DepthTest);
                GL.CullFace(CullFaceMode.Front);

                if (enable_flags.HasFlag(RenderEnable.SHADOWMAP_HMAP))
                {
                    if (scene.map != null)
                    {
                        UseShader(shader_shadowmap_heightmap);
                        RenderHeightmap();
                    }
                }


                GL.BindVertexArray(SFModelSkinChunk.Cache.VertexArrayObjectID);
                if (enable_flags.HasFlag(RenderEnable.SHADOWMAP_ANIM))
                {
                    UseShader(shader_shadowmap_animated);
                    RenderAnimatedObjects();
                }

                GL.BindVertexArray(SFSubModel3D.Cache.VertexArrayObjectID);
                if (enable_flags.HasFlag(RenderEnable.SHADOWMAP_SIMPLE))
                {
                    UseShader(shader_shadowmap);
                    RenderSimpleObjects();
                    RenderAdditiveObjects();
                }

                GL.CullFace(CullFaceMode.Back);

                GL.Disable(EnableCap.DepthTest);

                if (enable_flags.HasFlag(RenderEnable.SHADOWMAP_BLUR))
                {
                    GL.BindVertexArray(FrameBuffer.screen_vao);
                    UseShader(shader_shadowmap_blur);

                    // blur shadowmap horizontally
                    SetFramebuffer(shadowmap_depth_helper);
                    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                    GL.Clear(ClearBufferMask.ColorBufferBit);

                    GL.BindTexture(TextureTarget.Texture2D, shadowmap_depth.texture_colors[0]);
                    GL.Uniform1(active_shader["horizontal"], 1);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                    // blur shadowmap vertically
                    SetFramebuffer(shadowmap_depth);
                    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                    GL.Clear(ClearBufferMask.ColorBufferBit);

                    GL.BindTexture(TextureTarget.Texture2D, shadowmap_depth_helper.texture_colors[0]);
                    GL.Uniform1(active_shader["horizontal"], 0);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                }
            }


            // render actual view
            current_pass = RenderPass.SCENE;
            GL.Enable(EnableCap.Blend);
            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);
            SetFramebuffer(Settings.AntiAliasingSamples > 1 ? screenspace_framebuffer : screenspace_intermediate);

            // bind generated shadowmap
            if (Settings.EnableShadows)
            {
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, shadowmap_depth.texture_colors[0]);
                GL.ActiveTexture(TextureUnit.Texture0);
            }

            GL.ClearColor(scene.atmosphere.ambient_light.Color.X,
                scene.atmosphere.ambient_light.Color.Y,
                scene.atmosphere.ambient_light.Color.Z,
                0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // render sky
            if ((Settings.ToneMapping) && (enable_flags.HasFlag(RenderEnable.SKY)))
            {
                GL.Disable(EnableCap.DepthTest);
                UseShader(shader_sky);
                GL.DepthMask(false);
                RenderSky();
                GL.DepthMask(true);
            }

            GL.Enable(EnableCap.DepthTest);

            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

            GL.DepthFunc(DepthFunction.Lequal);
            // terrain
            if ((scene.map != null) && (enable_flags.HasFlag(RenderEnable.HMAP)))
            {
                // depth pre-pass
                if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
                {
                    UseShader(shader_heightmap_depth_prepass);
                    RenderHeightmapDepthPrePass();
                }
                // colored
                UseShader(shader_heightmap);
                RenderHeightmap();
            }


            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

            // simple opaque objects
            GL.BindVertexArray(SFSubModel3D.Cache.VertexArrayObjectID);

            if (enable_flags.HasFlag(RenderEnable.SIMPLE))
            {
                UseShader(shader_simple);
                RenderSimpleObjects();
            }

            // animated objects
            GL.BindVertexArray(SFModelSkinChunk.Cache.VertexArrayObjectID);

            if (enable_flags.HasFlag(RenderEnable.ANIM))
            {
                UseShader(shader_animated);
                RenderAnimatedObjects();
            }

            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

            // water
            GL.BindVertexArray(SFSubModel3D.Cache.VertexArrayObjectID);

            if (enable_flags.HasFlag(RenderEnable.WATER))
            {
                UseShader(shader_simple_transparency);
                RenderWaterObjects();
            }

            GL.DepthMask(false);

            // simple transparent objects
            if (enable_flags.HasFlag(RenderEnable.TRANSPARENT))
            {
                UseShader(shader_simple_transparency);
                GL.DepthFunc(DepthFunction.Less);
                RenderTransparentObjects();
                GL.DepthFunc(DepthFunction.Lequal);
            }

            // simple additive objects

            if (enable_flags.HasFlag(RenderEnable.SIMPLE))
            {
                UseShader(shader_simple);
                RenderAdditiveObjects();
            }

            GL.DepthMask(true);

            SetDepthBias(0);
            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

            // what is below doesnt depend on whats above

            // anitialiasing
            if (Settings.AntiAliasingSamples > 1)
            {
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, screenspace_framebuffer.fbo);
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, screenspace_intermediate.fbo);
                GL.BlitFramebuffer(0, 0, (int)render_size.X, (int)render_size.Y, 0, 0, (int)render_size.X, (int)render_size.Y, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            }

            current_pass = RenderPass.SCREENSPACE;

            SetFramebuffer(null);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Disable(EnableCap.DepthTest);

            // tonemapping
            GL.BindTexture(TextureTarget.Texture2D, screenspace_intermediate.texture_colors[0]);
            if ((Settings.ToneMapping)&&(enable_flags.HasFlag(RenderEnable.TONEMAP)))
            {
                UseShader(shader_framebuffer_tonemapped);
                GL.Uniform1(active_shader["exposure"], 1.0f);
            }
            else
            {
                UseShader(shader_framebuffer_simple);
            }
            GL.BindVertexArray(FrameBuffer.screen_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            // UI
            if (enable_flags.HasFlag(RenderEnable.UI))
            {
                UseShader(shader_ui);
                RenderUI();
            }

            UseShader(null);
            GL.BindVertexArray(0);
            current_pass = RenderPass.NONE;

            is_rendering = false;
        }
    }
}
