/*
 * SFRenderEngine takes care of displaying 3D graphics in a window
 * It takes data from SFScene and renders it using predefined shaders
 */

using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFEngine.SF3D.SceneSynchro;
using SFEngine.SF3D.UI;
using SFEngine.SFMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SFEngine.SF3D.SFRender
{
    public static class SFRenderEngine
    {
        public enum RenderPass { NONE = -1, SHADOWMAP = 0, SCENE = 1, SCREENSPACE = 2, UI = 3 }
        public static SFScene scene { get; } = new SFScene();
        public static UIManager ui { get; } = new UIManager();

        public static float min_render_distance = 0.1f;
        public static float max_render_distance = 1000f;

        static bool CurrentCullEnabled = false;
        static CullFaceMode CurrentCullMode = CullFaceMode.FrontAndBack;
        static float CurrentDepthBias = 0.0f;
        static RenderMode CurrentRenderMode = RenderMode.SRCALPHA_INVSRCALPHA;
        static bool CurrentDistanceFade = true;
        static float CurrentFadeStart = 150.0f;
        static float CurrentFadeEnd = 200.0f;
        static Vector4 CurrentEmissionColor = new Vector4(-1.0f);
        static bool CurrentApplyShading = true;
        static int[] CurrentTexture = new int[16];
        static int CurrentActiveTexture = Utility.NO_INDEX;

        public static SFTexture opaque_tex { get; private set; } = null;

        static SFShader shader_simple = new SFShader();
        static SFShader shader_simple_transparency = new SFShader();
        static SFShader shader_animated = new SFShader();
        static SFShader shader_heightmap = new SFShader();
        static SFShader shader_heightmap_depth_prepass = new SFShader();
        static SFShader shader_shadowmap = new SFShader();
        static SFShader shader_shadowmap_animated = new SFShader();
        static SFShader shader_shadowmap_heightmap = new SFShader();
        static SFShader shader_selection = new SFShader();
        static SFShader shader_selection_animated = new SFShader();
        static SFShader shader_sky = new SFShader();
        static SFShader shader_ui = new SFShader();
        static SFShader active_shader = null;
        static RenderPass current_pass = RenderPass.NONE;


        static SFShader shader_framebuffer_simple = new SFShader();
        static SFShader shader_framebuffer_tonemapped = new SFShader();
        static SFShader shader_shadowmap_blur = new SFShader();
        static SFShader shader_msm_resolve = new SFShader();
        static SFShader shader_vsm_resolve = new SFShader();

        // VSM framebuffers
        static FrameBuffer shadowmap_vsm_multisample = null;
        static FrameBuffer shadowmap_vsm_base = null;
        static FrameBuffer shadowmap_vsm_hpass = null;
        // MSM framebuffers
        static FrameBuffer shadowmap_msm_multisample = null;
        static FrameBuffer shadowmap_msm_base = null;
        static FrameBuffer shadowmap_msm_hpass = null;

        static FrameBuffer screenspace_framebuffer = null;
        static FrameBuffer screenspace_intermediate = null;

        public static bool render_shadowmap_depth = false;

        public static Vector2 render_size = Vector2.Zero;

        public static bool is_debug = false;

        public static bool is_rendering = false;
        public static bool initialized = false;

#if DEBUG
        public static int[] queries;
        public static int current_query = 0;
        public static Dictionary<int, int> query_results = new Dictionary<int, int>();

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
#endif //DEBUG
        public static void Initialize(Vector2 view_size)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.Initialize() called");

            // this happens only once per program run
            if (!initialized)
            {
#if DEBUG
                // setup GL debug info
                GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);
#endif //DEBUG

                // get max possible anisotropy
                GetPName MAX_TEXTURE_MAX_ANISOTROPY = (GetPName)0x84FF;
                Settings.MaxAnisotropy = (int)GL.GetFloat(MAX_TEXTURE_MAX_ANISOTROPY);
            }

            // initialize static model cache
            if (SFSubModel3D.Cache != null)
            {
                SFSubModel3D.Cache.Dispose();
            }

            SFSubModel3D.Cache = new MeshCache(true);
            SFSubModel3D.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // positions
            SFSubModel3D.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // normals
            SFSubModel3D.Cache.AddVertexAttribute(4, VertexAttribPointerType.UnsignedByte, true);   // colors
            SFSubModel3D.Cache.AddVertexAttribute(2, VertexAttribPointerType.Float, false);   // UVs
            SFSubModel3D.Cache.SetVertexSize(40);                                             // extra 4 bytes are unused, but still pushed to gpu
            SFSubModel3D.Cache.Init(1 << 19, 1 << 19);

            // initialize animated (skin) model cache
            if (SFModelSkinChunk.Cache != null)
            {
                SFModelSkinChunk.Cache.Dispose();
            }

            SFModelSkinChunk.Cache = new MeshCache(false);
            SFModelSkinChunk.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // positions
            SFModelSkinChunk.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // normals
            SFModelSkinChunk.Cache.AddVertexAttribute(4, VertexAttribPointerType.UnsignedByte, true);   // bone weights
            SFModelSkinChunk.Cache.AddVertexAttribute(2, VertexAttribPointerType.Float, false);   // uvs
            SFModelSkinChunk.Cache.AddVertexAttribute(4, VertexAttribPointerType.UnsignedByte, false);  // bone indices
            SFModelSkinChunk.Cache.SetVertexSize(40);
            SFModelSkinChunk.Cache.Init(1 << 15, 1 << 15);

            // opaque texture is a 1x1 white pixel that's used for blending operations on models that would otherwise have no texture assigned
            if (opaque_tex != null)
            {
                opaque_tex.Dispose();
            }

            opaque_tex = new SFTexture();
            byte[] tex_data = new byte[] { 255, 255, 255, 255 };
            using (MemoryStream ms = new MemoryStream(tex_data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    opaque_tex.LoadUncompressedRGBA(br, 1, 1);
                    opaque_tex.Init();
                }
            }

            scene.atmosphere.FogStart = Settings.FogStart;
            scene.atmosphere.FogEnd = Settings.FogEnd;

            // set up shaders
            RecompileAuxilliaryShaders();
            RecompileMainShaders();

            // set up main gl state
            GL.Enable(EnableCap.Blend);
            GL.DepthFunc(DepthFunction.Lequal);

            // prepare view and framebuffers
            ClearFramebuffers();

            render_size = view_size;
            ResizeView(view_size);

            InitFramebuffers();


#if DEBUG
            if (queries != null)
            {
                GL.DeleteQueries(4000, queries);
                queries = null;
            }
            queries = new int[4000];
            GL.GenQueries(4000, queries);
#endif //DEBUG

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
            shader_simple.SetDefine("VSM", (Settings.ShadowType == Settings.ShadowMapTechnique.VSM));
            shader_simple.SetDefine("MSM", (Settings.ShadowType == Settings.ShadowMapTechnique.MSM));
            shader_simple.SetDefine("FORCE_OPAQUE", true);
            shader_simple_transparency.SetDefine("SHADING", (Settings.ShadingQuality > 0));
            shader_simple_transparency.SetDefine("QUALITY_SHADING", (Settings.ShadingQuality > 1));
            shader_simple_transparency.SetDefine("SHADOWS", (Settings.EnableShadows));
            shader_simple_transparency.SetDefine("TONEMAPPING", (Settings.ToneMapping));
            shader_simple_transparency.SetDefine("VSM", (Settings.ShadowType == Settings.ShadowMapTechnique.VSM));
            shader_simple_transparency.SetDefine("MSM", (Settings.ShadowType == Settings.ShadowMapTechnique.MSM));
            shader_simple_transparency.SetDefine("FORCE_OPAQUE", false);
            shader_animated.SetDefine("SHADING", (Settings.ShadingQuality > 0));
            shader_animated.SetDefine("QUALITY_SHADING", (Settings.ShadingQuality > 1));
            shader_animated.SetDefine("SHADOWS", (Settings.EnableShadows));
            shader_animated.SetDefine("TONEMAPPING", (Settings.ToneMapping));
            shader_animated.SetDefine("VSM", (Settings.ShadowType == Settings.ShadowMapTechnique.VSM));
            shader_animated.SetDefine("MSM", (Settings.ShadowType == Settings.ShadowMapTechnique.MSM));
            shader_animated.SetDefine("FORCE_OPAQUE", false);
            shader_heightmap.SetDefine("EDITOR_MODE", (Settings.EditorMode));
            shader_heightmap.SetDefine("DISPLAY_GRID", Settings.DisplayGrid);
            shader_heightmap.SetDefine("VISUALIZE_HEIGHT", Settings.VisualizeHeight);
            shader_heightmap.SetDefine("SHADING", (Settings.ShadingQuality > 0));
            shader_heightmap.SetDefine("QUALITY_SHADING", (Settings.ShadingQuality > 1));
            shader_heightmap.SetDefine("SHADOWS", (Settings.EnableShadows));
            shader_heightmap.SetDefine("TONEMAPPING", (Settings.ToneMapping));
            shader_heightmap.SetDefine("TEXTURE_LOD", ((Settings.TerrainTextureLOD == 1) || ((Settings.TerrainTextureLOD == 2) && (Settings.ForceTerrainTextureLOD1))));
            shader_heightmap.SetDefine("NO_TEXTURE", ((Settings.TerrainTextureLOD == 2) && (!Settings.ForceTerrainTextureLOD1)));
            shader_heightmap.SetDefine("VSM", (Settings.ShadowType == Settings.ShadowMapTechnique.VSM));
            shader_heightmap.SetDefine("MSM", (Settings.ShadowType == Settings.ShadowMapTechnique.MSM));

            shader_simple.CompileShader(new ShaderInfo[]
            {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader }
            });
            shader_simple.AddParameter("VP");
            shader_simple.AddParameter("DiffuseTex");
            shader_simple.AddParameter("SunColor");
            shader_simple.AddParameter("DepthBias");
            shader_simple.AddParameter("AlphaCutout");

            if((Settings.EnableShadows)||(Settings.ShadingQuality >= 1))
            {
                shader_simple.AddParameter("AmbientColor");
            }
            if (Settings.EnableShadows)
            {
                shader_simple.AddParameter("LSM");
                shader_simple.AddParameter("ShadowMap");
            }
            if (Settings.ShadingQuality >= 1)
            {
                shader_simple.AddParameter("SunDirection");
                shader_simple.AddParameter("FogColor");
                shader_simple.AddParameter("FogStart");
                shader_simple.AddParameter("FogEnd");
                shader_simple.AddParameter("FogExponent");
                shader_simple.AddParameter("ObjectFadeStart");
                shader_simple.AddParameter("ObjectFadeEnd");
                shader_simple.AddParameter("DistanceFade");
                shader_simple.AddParameter("ApplyShading");

                if (Settings.ShadingQuality >= 2)
                {
                    shader_simple.AddParameter("GroundMap");
                    shader_simple.AddParameter("GridSize");
                    shader_simple.AddParameter("EmissionColor");
                    shader_simple.AddParameter("ViewPos");

                    int uniform_tilecol_simple = GL.GetUniformBlockIndex(shader_simple.ProgramID, "TileColors");
                    GL.UniformBlockBinding(shader_simple.ProgramID, uniform_tilecol_simple, 2);
                }
            }

            shader_simple_transparency.CompileShader(new ShaderInfo[]
            {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader }
            });
            shader_simple_transparency.AddParameter("VP");
            shader_simple_transparency.AddParameter("DiffuseTex");
            shader_simple_transparency.AddParameter("SunColor");
            shader_simple_transparency.AddParameter("DepthBias");
            shader_simple_transparency.AddParameter("AlphaCutout");

            if ((Settings.EnableShadows) || (Settings.ShadingQuality >= 1))
            {
                shader_simple_transparency.AddParameter("AmbientColor");
            }
            if (Settings.EnableShadows)
            {
                shader_simple_transparency.AddParameter("LSM");
                shader_simple_transparency.AddParameter("ShadowMap");
            }
            if (Settings.ShadingQuality >= 1)
            {
                shader_simple_transparency.AddParameter("SunDirection");
                shader_simple_transparency.AddParameter("FogColor");
                shader_simple_transparency.AddParameter("FogStart");
                shader_simple_transparency.AddParameter("FogEnd");
                shader_simple_transparency.AddParameter("FogExponent");
                shader_simple_transparency.AddParameter("ObjectFadeStart");
                shader_simple_transparency.AddParameter("ObjectFadeEnd");
                shader_simple_transparency.AddParameter("DistanceFade");
                shader_simple_transparency.AddParameter("ApplyShading");

                if (Settings.ShadingQuality >= 2)
                {
                    shader_simple_transparency.AddParameter("GroundMap");
                    shader_simple_transparency.AddParameter("GridSize");
                    shader_simple_transparency.AddParameter("EmissionColor");
                    shader_simple_transparency.AddParameter("ViewPos");

                    int uniform_tilecol_simple = GL.GetUniformBlockIndex(shader_simple_transparency.ProgramID, "TileColors");
                    GL.UniformBlockBinding(shader_simple_transparency.ProgramID, uniform_tilecol_simple, 2);
                }
            }

            shader_animated.CompileShader(new ShaderInfo[]
            {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_skel },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader }
            });
            shader_animated.AddParameter("P");
            shader_animated.AddParameter("V");
            shader_animated.AddParameter("M");
            shader_animated.AddParameter("DiffuseTex");
            shader_animated.AddParameter("boneTransforms");
            shader_animated.AddParameter("SunColor");
            shader_animated.AddParameter("AlphaCutout");
            shader_animated.AddParameter("DepthBias");

            if ((Settings.EnableShadows) || (Settings.ShadingQuality >= 1))
            {
                shader_animated.AddParameter("AmbientColor");
            }
            if (Settings.EnableShadows)
            {
                shader_animated.AddParameter("LSM");
                shader_animated.AddParameter("ShadowMap");
            }
            if (Settings.ShadingQuality >= 1)
            {
                shader_animated.AddParameter("SunDirection");
                shader_animated.AddParameter("FogColor");
                shader_animated.AddParameter("FogStart");
                shader_animated.AddParameter("FogEnd");
                shader_animated.AddParameter("FogExponent");
                shader_animated.AddParameter("ObjectFadeStart");
                shader_animated.AddParameter("ObjectFadeEnd");
                shader_animated.AddParameter("DistanceFade");
                shader_animated.AddParameter("ApplyShading");

                if (Settings.ShadingQuality >= 2)
                {
                    shader_animated.AddParameter("GroundMap");
                    shader_animated.AddParameter("GridSize");
                    shader_animated.AddParameter("EmissionColor");
                    shader_animated.AddParameter("ViewPos");

                    int uniform_tilecol_simple = GL.GetUniformBlockIndex(shader_animated.ProgramID, "TileColors");
                    GL.UniformBlockBinding(shader_animated.ProgramID, uniform_tilecol_simple, 2);
                }
            }

            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
            {
                shader_heightmap.CompileShader(new ShaderInfo[]
                {
                    new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_hmap },
                    new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_hmap }
                });
            }
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
            shader_heightmap.AddParameter("TileMap");
            shader_heightmap.AddParameter("HeightMap");
            shader_heightmap.AddParameter("BumpMap");
            shader_heightmap.AddParameter("SunColor");
            int uniform_tilecol = GL.GetUniformBlockIndex(shader_heightmap.ProgramID, "TileColors");
            GL.UniformBlockBinding(shader_heightmap.ProgramID, uniform_tilecol, 2);
            if (Settings.EditorMode)
            {
                shader_heightmap.AddParameter("GridColor");
                shader_heightmap.AddParameter("CurrentFlags");
                shader_heightmap.AddParameter("FlagMap");
                int uniform_overlaycol = GL.GetUniformBlockIndex(shader_heightmap.ProgramID, "Overlays");
                GL.UniformBlockBinding(shader_heightmap.ProgramID, uniform_overlaycol, 1);
            }
            if (Settings.TerrainTextureLOD != 2)
            {
                shader_heightmap.AddParameter("myTextureSampler");
            }
            if ((Settings.EnableShadows) || (Settings.ShadingQuality >= 1))
            {
                shader_heightmap.AddParameter("AmbientColor");
            }
            if (Settings.EnableShadows)
            {
                shader_heightmap.AddParameter("LSM");
                shader_heightmap.AddParameter("ShadowMap");
                shader_heightmap.AddParameter("ShadowFadeStart");
                shader_heightmap.AddParameter("ShadowFadeEnd");
            }
            if (Settings.ShadingQuality >= 1)
            {
                shader_heightmap.AddParameter("SunDirection");
                shader_heightmap.AddParameter("FogColor");
                shader_heightmap.AddParameter("FogStart");
                shader_heightmap.AddParameter("FogEnd");
                shader_heightmap.AddParameter("FogExponent");
                if(Settings.ShadingQuality >= 2)
                {
                    shader_heightmap.AddParameter("ViewPos");
                }
            }


            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
            {
                shader_heightmap_depth_prepass.CompileShader(new ShaderInfo[]
                {
                    new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_hmap_depth_prepass },
                    new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_hmap_depth_prepass }
                });
            }
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
                {
                    screenspace_framebuffer.Resize((int)view_size.X, (int)view_size.Y);
                }

                screenspace_intermediate.Resize((int)view_size.X, (int)view_size.Y);
            }
        }

        // updates shader lighting parameters
        public static void ApplyLight()
        {
            GL.UseProgram(shader_simple.ProgramID);
            GL.Uniform4(shader_simple["SunColor"], scene.atmosphere.sun_light.Color * scene.atmosphere.sun_light.Strength);
            if ((Settings.ShadingQuality >= 1) || (Settings.EnableShadows))
            {
                GL.Uniform4(shader_simple["AmbientColor"], scene.atmosphere.ambient_light.Color * scene.atmosphere.ambient_light.Strength);
            }
            if (Settings.ShadingQuality >= 1)
            {
                GL.Uniform3(shader_simple["SunDirection"], scene.atmosphere.sun_light.Direction);
                GL.Uniform4(shader_simple["FogColor"], scene.atmosphere.FogColor * scene.atmosphere.FogStrength);
                GL.Uniform1(shader_simple["FogStart"], scene.atmosphere.FogStart);
                GL.Uniform1(shader_simple["FogEnd"], scene.atmosphere.FogEnd);
                GL.Uniform1(shader_simple["FogExponent"], scene.atmosphere.FogExponent);
            }

            GL.UseProgram(shader_simple_transparency.ProgramID);
            GL.Uniform4(shader_simple_transparency["SunColor"], scene.atmosphere.sun_light.Color * scene.atmosphere.sun_light.Strength);
            if ((Settings.ShadingQuality >= 1) || (Settings.EnableShadows))
            {
                GL.Uniform4(shader_simple_transparency["AmbientColor"], scene.atmosphere.ambient_light.Color * scene.atmosphere.ambient_light.Strength);
            }
            if (Settings.ShadingQuality >= 1)
            {
                GL.Uniform3(shader_simple_transparency["SunDirection"], scene.atmosphere.sun_light.Direction);
                GL.Uniform4(shader_simple_transparency["FogColor"], scene.atmosphere.FogColor * scene.atmosphere.FogStrength);
                GL.Uniform1(shader_simple_transparency["FogStart"], scene.atmosphere.FogStart);
                GL.Uniform1(shader_simple_transparency["FogEnd"], scene.atmosphere.FogEnd);
                GL.Uniform1(shader_simple_transparency["FogExponent"], scene.atmosphere.FogExponent);
            }

            GL.UseProgram(shader_animated.ProgramID);
            GL.Uniform4(shader_animated["SunColor"], scene.atmosphere.sun_light.Color * scene.atmosphere.sun_light.Strength);
            if ((Settings.ShadingQuality >= 1) || (Settings.EnableShadows))
            {
                GL.Uniform4(shader_animated["AmbientColor"], scene.atmosphere.ambient_light.Color * scene.atmosphere.ambient_light.Strength);
            }
            if (Settings.ShadingQuality >= 1)
            {
                GL.Uniform3(shader_animated["SunDirection"], scene.atmosphere.sun_light.Direction);
                GL.Uniform4(shader_animated["FogColor"], scene.atmosphere.FogColor * scene.atmosphere.FogStrength);
                GL.Uniform1(shader_animated["FogStart"], scene.atmosphere.FogStart);
                GL.Uniform1(shader_animated["FogEnd"], scene.atmosphere.FogEnd);
                GL.Uniform1(shader_animated["FogExponent"], scene.atmosphere.FogExponent);
            }

            GL.UseProgram(shader_heightmap.ProgramID);
            GL.Uniform4(shader_heightmap["SunColor"], scene.atmosphere.sun_light.Color * scene.atmosphere.sun_light.Strength);
            if ((Settings.ShadingQuality >= 1) || (Settings.EnableShadows))
            {
                GL.Uniform4(shader_heightmap["AmbientColor"], scene.atmosphere.ambient_light.Color * scene.atmosphere.ambient_light.Strength);
            }
            if (Settings.ShadingQuality >= 1)
            {
                GL.Uniform3(shader_heightmap["SunDirection"], scene.atmosphere.sun_light.Direction);
                GL.Uniform4(shader_heightmap["FogColor"], scene.atmosphere.FogColor * scene.atmosphere.FogStrength);
                GL.Uniform1(shader_heightmap["FogStart"], scene.atmosphere.FogStart);
                GL.Uniform1(shader_heightmap["FogEnd"], scene.atmosphere.FogEnd);
                GL.Uniform1(shader_heightmap["FogExponent"], scene.atmosphere.FogExponent);
            }

            if (Settings.ToneMapping)
            {
                GL.UseProgram(shader_sky.ProgramID);
                GL.Uniform4(shader_sky["AmbientColor"], scene.atmosphere.ambient_light.Color);
                GL.Uniform4(shader_sky["FogColor"], scene.atmosphere.FogColor * scene.atmosphere.FogStrength);
            }

            GL.UseProgram(0);
        }

        // assigns texture slots to shaders
        private static void ApplyTexturingUnits()
        {
            GL.UseProgram(shader_simple.ProgramID);
            GL.Uniform1(shader_simple["DiffuseTex"], 0);
            if (Settings.EnableShadows)
            {
                GL.Uniform1(shader_simple["ShadowMap"], 1);
            }
            if (Settings.ShadingQuality >= 1)
            {
                if (Settings.ShadingQuality >= 2)
                {
                    GL.Uniform1(shader_simple["GroundMap"], 2);
                }
            }

            GL.UseProgram(shader_simple_transparency.ProgramID);
            GL.Uniform1(shader_simple_transparency["DiffuseTex"], 0);
            if (Settings.EnableShadows)
            {
                GL.Uniform1(shader_simple_transparency["ShadowMap"], 1);
            }
            if (Settings.ShadingQuality >= 1)
            {
                if (Settings.ShadingQuality >= 2)
                {
                    GL.Uniform1(shader_simple_transparency["GroundMap"], 2);
                }
            }

            GL.UseProgram(shader_animated.ProgramID);
            GL.Uniform1(shader_animated["DiffuseTex"], 0);
            if (Settings.EnableShadows)
            {
                GL.Uniform1(shader_animated["ShadowMap"], 1);
            }
            if (Settings.ShadingQuality >= 1)
            {
                if (Settings.ShadingQuality >= 2)
                {
                    GL.Uniform1(shader_animated["GroundMap"], 2);
                }
            }

            GL.UseProgram(shader_heightmap.ProgramID);
            if (Settings.TerrainTextureLOD != 2)
            {
                GL.Uniform1(shader_heightmap["myTextureSampler"], 0);
            }
            if (Settings.EnableShadows)
            {
                GL.Uniform1(shader_heightmap["ShadowMap"], 1);
            }
            GL.Uniform1(shader_heightmap["TileMap"], 2);
            if (Settings.EditorMode)
            {
                GL.Uniform1(shader_heightmap["FlagMap"], 3);
            }
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

                if (Settings.ShadowType == Settings.ShadowMapTechnique.VSM)
                {
                    GL.UseProgram(shader_vsm_resolve.ProgramID);
                    GL.Uniform1(shader_vsm_resolve["ShadowMap"], 0);
                }
                else if (Settings.ShadowType == Settings.ShadowMapTechnique.MSM)
                {
                    GL.UseProgram(shader_msm_resolve.ProgramID);
                    GL.Uniform1(shader_msm_resolve["ShadowMap"], 0);
                }
            }

            GL.UseProgram(shader_ui.ProgramID);
            GL.Uniform1(shader_ui["Tex"], 0);

            GL.UseProgram(shader_selection.ProgramID);
            GL.Uniform1(shader_selection["DiffuseTex"], 0);

            GL.UseProgram(shader_selection_animated.ProgramID);
            GL.Uniform1(shader_selection_animated["DiffuseTex"], 0);

            GL.UseProgram(0);
        }

        public static void ApplyFade()
        {
            GL.UseProgram(shader_simple.ProgramID);
            if (Settings.ShadingQuality >= 1)
            {
                GL.Uniform1(shader_simple["ObjectFadeStart"], CurrentFadeStart);
                GL.Uniform1(shader_simple["ObjectFadeEnd"], CurrentFadeEnd);
            }

            GL.UseProgram(shader_simple_transparency.ProgramID);
            if (Settings.ShadingQuality >= 1)
            {
                GL.Uniform1(shader_simple_transparency["ObjectFadeStart"], CurrentFadeStart);
                GL.Uniform1(shader_simple_transparency["ObjectFadeEnd"], CurrentFadeEnd);
            }

            GL.UseProgram(shader_animated.ProgramID);
            if (Settings.ShadingQuality >= 1)
            {
                GL.Uniform1(shader_animated["ObjectFadeStart"], CurrentFadeStart);
                GL.Uniform1(shader_animated["ObjectFadeEnd"], CurrentFadeEnd);
            }

            GL.UseProgram(shader_heightmap.ProgramID);
            if ((Settings.EnableShadows)&&(Settings.ShadingQuality >= 1))
            {
                GL.Uniform1(shader_heightmap["ShadowFadeStart"], CurrentFadeStart);
                GL.Uniform1(shader_heightmap["ShadowFadeEnd"], CurrentFadeEnd);
            }
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
            {
                return;
            }

            active_shader = shader;
            if (shader != null)
            {
                GL.UseProgram(shader.ProgramID);
                if ((shader == shader_simple) || (shader == shader_simple_transparency) || (shader == shader_animated))
                {
                    CurrentDepthBias = 0;
                    GL.Uniform1(active_shader["DepthBias"], 0.0f);
                    if (Settings.ShadingQuality >= 1)
                    {
                        CurrentDistanceFade = false;
                        GL.Uniform1(active_shader["DistanceFade"], 0.0f);
                        CurrentApplyShading = false;
                        GL.Uniform1(active_shader["ApplyShading"], 0);
                        if (Settings.ShadingQuality >= 2)
                        {
                            CurrentEmissionColor = Vector4.Zero;
                            GL.Uniform4(active_shader["EmissionColor"], CurrentEmissionColor);
                        }
                    }
                }
            }
            else
            {
                GL.UseProgram(0);
            }
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

        private static void SetDistanceFade(bool b)
        {
            if (b != CurrentDistanceFade)
            {
                CurrentDistanceFade = b;
                GL.Uniform1(active_shader["DistanceFade"], b ? 1.0f : 0.0f);
            }
        }

        private static void SetCullEnabled(bool enable)
        {
            if(CurrentCullEnabled != enable)
            {
                CurrentCullEnabled = enable;
                if (enable)
                    GL.Enable(EnableCap.CullFace);
                else
                    GL.Disable(EnableCap.CullFace);
            }
        }

        private static void SetCullMode(CullFaceMode mode)
        {
            if(CurrentCullMode != mode)
            {
                CurrentCullMode = mode;
                GL.CullFace(mode);
            }
        }

        private static void SetRenderMode(RenderMode rm)
        {
            if (CurrentRenderMode == rm)
            {
                return;
            }

            switch (rm)
            {
                case RenderMode.DESTCOLOR_INVSRCALPHA:
                    GL.BlendFunc(BlendingFactor.DstColor, BlendingFactor.OneMinusSrcAlpha);
                    break;
                case RenderMode.DESTCOLOR_ONE:
                    //GL.BlendFunc(BlendingFactor.DstColor, BlendingFactor.One);
                    GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
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

        private static void SetEmission(Vector4 col)
        {
            if (CurrentEmissionColor != col)
            {
                CurrentEmissionColor = col;
                GL.Uniform4(active_shader["EmissionColor"], col);
            }
        }

        public static void SetTexture(int slot, TextureTarget target, int texture)
        {
            if (texture <= 0)
            {
                texture = opaque_tex.tex_id;
            }

            if (CurrentActiveTexture != slot)
            {
                CurrentActiveTexture = slot;
                if (CurrentTexture[slot] != texture)
                {
                    GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + slot));
                    GL.BindTexture(target, texture);
                    CurrentTexture[slot] = texture;
                }
            }
            else
            {
                if (CurrentTexture[slot] != texture)
                {
                    GL.BindTexture(target, texture);
                    CurrentTexture[slot] = texture;
                }
            }
        }

        public static void ResetTextures()
        {
            for (int i = 0; i < 16; i++)
            {
                GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + i));
                GL.BindTexture(TextureTarget.Texture2D, 0);
                CurrentTexture[i] = 0;
            }
            CurrentActiveTexture = 15;
        }

        public static void ResetTexture(int id)
        {
            for (int i = 0; i < 16; i++)
            {
                if (CurrentTexture[i] == id)
                {
                    CurrentTexture[i] = 0;
                    break;
                }
            }
        }

        private static void SetApplyShading(bool apply_shading)
        {
            if (apply_shading != CurrentApplyShading)
            {
                CurrentApplyShading = apply_shading;
                GL.Uniform1(active_shader["ApplyShading"], apply_shading ? 1 : 0);
            }
        }

        private static void RecompileAuxilliaryShaders()
        {
            // set up shadow shaders
            if (Settings.EnableShadows)
            {
                shader_shadowmap.SetDefine("VSM", (Settings.ShadowType == Settings.ShadowMapTechnique.VSM));
                shader_shadowmap.SetDefine("MSM", (Settings.ShadowType == Settings.ShadowMapTechnique.MSM));
                shader_shadowmap_animated.SetDefine("VSM", (Settings.ShadowType == Settings.ShadowMapTechnique.VSM));
                shader_shadowmap_animated.SetDefine("MSM", (Settings.ShadowType == Settings.ShadowMapTechnique.MSM));
                shader_shadowmap_heightmap.SetDefine("VSM", (Settings.ShadowType == Settings.ShadowMapTechnique.VSM));
                shader_shadowmap_heightmap.SetDefine("MSM", (Settings.ShadowType == Settings.ShadowMapTechnique.MSM));
                shader_shadowmap_blur.SetDefine("VSM", (Settings.ShadowType == Settings.ShadowMapTechnique.VSM));
                shader_shadowmap_blur.SetDefine("MSM", (Settings.ShadowType == Settings.ShadowMapTechnique.MSM));

                // shader compilation
                shader_shadowmap.CompileShader(new ShaderInfo[]
                {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_shadowmap },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_shadowmap }
                });
                shader_shadowmap.AddParameter("VP");
                shader_shadowmap.AddParameter("DiffuseTexture");

                shader_shadowmap_animated.CompileShader(new ShaderInfo[]
                {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_shadowmap_animated },
                new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_shadowmap }
                });
                shader_shadowmap_animated.AddParameter("P");
                shader_shadowmap_animated.AddParameter("V");
                shader_shadowmap_animated.AddParameter("M");
                shader_shadowmap_animated.AddParameter("boneTransforms");
                shader_shadowmap_animated.AddParameter("DiffuseTexture");

                if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
                {
                    shader_shadowmap_heightmap.CompileShader(new ShaderInfo[]
                    {
                    new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_shadowmap_heightmap },
                    new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_shadowmap }
                    });
                }
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
                shader_shadowmap_heightmap.AddParameter("VP");
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

                if (Settings.ShadowType == Settings.ShadowMapTechnique.VSM)
                {
                    shader_vsm_resolve.CompileShader(new ShaderInfo[]
                    {
                        new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_framebuffer },
                        new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_vsm_resolve }
                    });
                    shader_vsm_resolve.AddParameter("ShadowMap");
                    shader_vsm_resolve.AddParameter("TextureSize");
                }
                else if (Settings.ShadowType == Settings.ShadowMapTechnique.MSM)
                {
                    shader_msm_resolve.CompileShader(new ShaderInfo[]
                    {
                        new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_framebuffer },
                        new ShaderInfo() { type = ShaderType.FragmentShader, data = Properties.Resources.fshader_msm_resolve }
                    });
                    shader_msm_resolve.AddParameter("ShadowMap");
                    shader_msm_resolve.AddParameter("TextureSize");
                }
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
                shader_sky.AddParameter("AmbientColor");
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

            shader_selection.CompileShader(new ShaderInfo[]
            {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_shadowmap },
                new ShaderInfo() {type = ShaderType.FragmentShader, data = Properties.Resources.fshader_selection }
            });
            shader_selection.AddParameter("VP");
            shader_selection.AddParameter("DiffuseTex");
            shader_selection.AddParameter("Time");
            shader_selection.AddParameter("Color");

            shader_selection_animated.CompileShader(new ShaderInfo[]
            {
                new ShaderInfo() { type = ShaderType.VertexShader, data = Properties.Resources.vshader_shadowmap_animated },
                new ShaderInfo() {type = ShaderType.FragmentShader, data = Properties.Resources.fshader_selection }
            });
            shader_selection_animated.AddParameter("M");
            shader_selection_animated.AddParameter("V");
            shader_selection_animated.AddParameter("P");
            shader_selection_animated.AddParameter("boneTransforms");
            shader_selection_animated.AddParameter("DiffuseTex");
            shader_selection_animated.AddParameter("Time");
            shader_selection_animated.AddParameter("Color");
        }

        private static void ClearFramebuffers()
        {
            if (Settings.EnableShadows)
            {
                if (Settings.ShadowType == Settings.ShadowMapTechnique.VSM)
                {
                    if (shadowmap_vsm_base != null)
                    {
                        shadowmap_vsm_base.Dispose();
                        shadowmap_vsm_hpass.Dispose();
                        shadowmap_vsm_multisample.Dispose();
                        shadowmap_vsm_base = null;
                        shadowmap_vsm_hpass = null;
                        shadowmap_vsm_multisample = null;
                    }
                }
                else if (Settings.ShadowType == Settings.ShadowMapTechnique.MSM)
                {
                    if (shadowmap_msm_base != null)
                    {
                        shadowmap_msm_base.Dispose();
                        shadowmap_msm_hpass.Dispose();
                        shadowmap_msm_multisample.Dispose();
                        shadowmap_msm_base = null;
                        shadowmap_msm_hpass = null;
                        shadowmap_msm_multisample = null;
                    }
                }
            }

            if (screenspace_intermediate != null)
            {
                if (Settings.AntiAliasingSamples > 1)
                {
                    screenspace_framebuffer.Dispose();
                }

                screenspace_intermediate.Dispose();
                screenspace_framebuffer = null;
                screenspace_intermediate = null;
            }
        }

        private static void InitFramebuffers()
        {
            if (Settings.EnableShadows)
            {
                float[] col = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

                if (Settings.ShadowType == Settings.ShadowMapTechnique.VSM)
                {
                    // VSM framebuffers required:
                    // - fb1, to which depth is written - multisampled
                    // - fb2, to which depth and depth squared are written from fb1
                    // - fb3; horizontal_pass(fb2) -> fb3
                    // - fb4; vertical_pass(fb3) -> fb4
                    // fb4 gets mipmapped and used as resulting shadowmap
                    // due to the technique used, fb2 and fb4 can point to the same data
                    shadowmap_vsm_multisample = new FrameBuffer(
                        Settings.ShadowMapSize, Settings.ShadowMapSize,
                        new FramebufferAttachmentInfo[]
                        {
                            new FramebufferAttachmentInfo()
                            {
                                format = PixelFormat.DepthComponent, internal_format = PixelInternalFormat.DepthComponent16, pixel_type = PixelType.UnsignedInt, attachment_type = FramebufferAttachment.DepthAttachment,
                                sample_count = 4, min_filter = (int)All.Linear, mag_filter = (int)All.Linear, wrap_s = (int)All.ClampToEdge, wrap_t = (int)All.ClampToEdge, anisotropy = 0
                            }
                        });

                    shadowmap_vsm_base = new FrameBuffer(
                        Settings.ShadowMapSize, Settings.ShadowMapSize,
                        new FramebufferAttachmentInfo[]
                        {
                            new FramebufferAttachmentInfo()
                            {
                                format = PixelFormat.Rg, internal_format = PixelInternalFormat.Rg16f, pixel_type = PixelType.Float, attachment_type = FramebufferAttachment.ColorAttachment0,
                                sample_count = 0, min_filter = (int)All.LinearMipmapLinear, mag_filter = (int)All.Linear, wrap_s = (int)All.ClampToBorder, wrap_t = (int)All.ClampToBorder,
                                wrap_border_col = new Vector4(1.0f), anisotropy = Settings.MaxAnisotropy
                            }
                        });

                    shadowmap_vsm_hpass = new FrameBuffer(
                        Settings.ShadowMapSize / 2, Settings.ShadowMapSize / 2,
                        new FramebufferAttachmentInfo[]
                        {
                            new FramebufferAttachmentInfo()
                            {
                                format = PixelFormat.Rg, internal_format = PixelInternalFormat.Rg16f, pixel_type = PixelType.Float, attachment_type = FramebufferAttachment.ColorAttachment0,
                                sample_count = 0, min_filter = (int)All.LinearMipmapLinear, mag_filter = (int)All.Linear, wrap_s = (int)All.ClampToBorder, wrap_t = (int)All.ClampToBorder,
                                wrap_border_col = new Vector4(1.0f), anisotropy = Settings.MaxAnisotropy
                            }
                        });

                }
                else if (Settings.ShadowType == Settings.ShadowMapTechnique.MSM)    // this will only work if antialiasing is on
                {
                    // MSM framebuffers required:
                    // - fb1, to which depth is written (multisampled)
                    // - fb2, input - depth from all samples of fb1, output - calculated moments for those samples
                    // - fb3; horizontal pass
                    // - fb4; vertical pass
                    // fb4 gets mipmapped and used as resulting shadowmap
                    // due to the technique used, fb2 and fb4 can point to the same data
                    shadowmap_msm_multisample = new FrameBuffer(
                        Settings.ShadowMapSize, Settings.ShadowMapSize,
                        new FramebufferAttachmentInfo[]
                        {
                            new FramebufferAttachmentInfo()
                            {
                                format = PixelFormat.DepthComponent, internal_format = PixelInternalFormat.DepthComponent16, pixel_type = PixelType.UnsignedInt, attachment_type = FramebufferAttachment.DepthAttachment,
                                sample_count = 4, min_filter = (int)All.Linear, mag_filter = (int)All.Linear, wrap_s = (int)All.ClampToEdge, wrap_t = (int)All.ClampToEdge, anisotropy = 0
                            }
                        });

                    shadowmap_msm_base = new FrameBuffer(
                        Settings.ShadowMapSize, Settings.ShadowMapSize,
                        new FramebufferAttachmentInfo[]
                        {
                            new FramebufferAttachmentInfo()
                            {
                                format = PixelFormat.Rgba, internal_format = PixelInternalFormat.Rgba16, pixel_type = PixelType.UnsignedInt, attachment_type = FramebufferAttachment.ColorAttachment0,
                                sample_count = 0, min_filter = (int)All.LinearMipmapLinear, mag_filter = (int)All.Linear, wrap_s = (int)All.ClampToBorder, wrap_t = (int)All.ClampToBorder,
                                wrap_border_col = new Vector4(1.0f), anisotropy = Settings.MaxAnisotropy
                            }
                        });

                    shadowmap_msm_hpass = new FrameBuffer(
                        Settings.ShadowMapSize / 2, Settings.ShadowMapSize / 2,
                        new FramebufferAttachmentInfo[]
                        {
                            new FramebufferAttachmentInfo()
                            {
                                format = PixelFormat.Rgba, internal_format = PixelInternalFormat.Rgba16, pixel_type = PixelType.UnsignedInt, attachment_type = FramebufferAttachment.ColorAttachment0,
                                sample_count = 0, min_filter = (int)All.LinearMipmapLinear, mag_filter = (int)All.Linear, wrap_s = (int)All.ClampToBorder, wrap_t = (int)All.ClampToBorder,
                                wrap_border_col = new Vector4(1.0f), anisotropy = Settings.MaxAnisotropy
                            }
                        });
                }
            }

            // anti-aliased framebuffer - everything is drawn here if anti-aliasing is enabled
            // if anything got drawn into it, this is later copied into screenspace_intermediate
            if (Settings.AntiAliasingSamples > 1)
            {
                screenspace_framebuffer = new FrameBuffer(
                    (int)render_size.X, (int)render_size.Y,
                    new FramebufferAttachmentInfo[]
                    {
                        new FramebufferAttachmentInfo()
                        {
                            format = PixelFormat.Rgb, internal_format = (Settings.ToneMapping ? PixelInternalFormat.Rgb16f : PixelInternalFormat.Rgb),
                            pixel_type = (Settings.ToneMapping ? PixelType.Float : PixelType.UnsignedByte), attachment_type = FramebufferAttachment.ColorAttachment0,
                            sample_count = Settings.AntiAliasingSamples, min_filter = (int)All.Nearest, mag_filter = (int)All.Nearest, wrap_s = (int)All.ClampToEdge,
                            wrap_t = (int)All.ClampToEdge, anisotropy = 0
                        },
                        new FramebufferAttachmentInfo()
                        {
                            format = PixelFormat.DepthComponent, internal_format = PixelInternalFormat.DepthComponent32, pixel_type = PixelType.Float,
                            attachment_type = FramebufferAttachment.DepthAttachment, min_filter = (int)All.Nearest, mag_filter = (int)All.Nearest,
                            wrap_s = (int)All.ClampToEdge, wrap_t = (int)All.ClampToEdge, sample_count = Settings.AntiAliasingSamples, anisotropy = 0
                        }
                    });
            }

            // this framebuffer stores result of anti-aliased framebuffer (if anti-aliasing is disabled, everything is drawn here)
            // tonemapping is performed in this framebuffer
            screenspace_intermediate = new FrameBuffer(
                (int)render_size.X, (int)render_size.Y,
                new FramebufferAttachmentInfo[]
                {
                    new FramebufferAttachmentInfo()
                    {
                        format = PixelFormat.Rgb, internal_format = (Settings.ToneMapping ? PixelInternalFormat.Rgb16f : PixelInternalFormat.Rgb),
                        pixel_type = (Settings.ToneMapping ? PixelType.Float : PixelType.UnsignedByte), attachment_type = FramebufferAttachment.ColorAttachment0,
                        sample_count = 0, min_filter = (int)All.Nearest, mag_filter = (int)All.Nearest, wrap_s = (int)All.ClampToEdge,
                        wrap_t = (int)All.ClampToEdge, anisotropy = 0
                    },
                    new FramebufferAttachmentInfo()
                    {
                        format = PixelFormat.DepthComponent, internal_format = PixelInternalFormat.DepthComponent32, pixel_type = PixelType.Float,
                        attachment_type = FramebufferAttachment.DepthAttachment, min_filter = (int)All.Nearest, mag_filter = (int)All.Nearest,
                        wrap_s = (int)All.ClampToEdge, wrap_t = (int)All.ClampToEdge, sample_count = 0, anisotropy = 0
                    }
                });
        }

#if DEBUG
        static void StartQuery()
        {
            if (is_debug)
            {
                GL.BeginQuery(QueryTarget.TimeElapsed, queries[current_query]);
            }
        }

        static void EndQuery()
        {
            if (is_debug)
            {
                GL.EndQuery(QueryTarget.TimeElapsed);
                current_query++;
            }
        }
#else
        static void StartQuery()
        {
        }

        static void EndQuery()
        {
        }
#endif //DEBUG

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
                    {
                        continue;
                    }

                    if (!chunk_node.MapChunk.aabb.IsOutsideOfConvexHull(scene.camera.Frustum.frustum_planes))
                    {
                        vis_chunks.Add(chunk_node);
                    }
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
                {
                    cur_end = true;
                }

                if (next_list_id == vis_chunks.Count)
                {
                    next_end = true;
                }

                if (next_end && cur_end)
                {
                    break;
                }

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

                if (next_chunk_id > cur_chunk_id)
                {
                    while (next_chunk_id > cur_chunk_id)
                    {
                        heightmap.visible_chunks[cur_list_id].SetParent(null);
                        heightmap.visible_chunks[cur_list_id].MapChunk.UpdateVisible(false);
                        // turn chunk invisible

                        cur_list_id += 1;
                        if (cur_list_id == heightmap.visible_chunks.Count)
                        {
                            break;
                        }

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
                        {
                            break;
                        }

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
        }

        static void RenderSky()
        {
            Matrix4 v_mat = scene.camera.ViewMatrix;
            GL.UniformMatrix4(active_shader["V"], true, ref v_mat);
            GL.Uniform1(active_shader["AspectRatio"], 1.0f);
            GL.BindVertexArray(Atmosphere.sky_vao);
            StartQuery();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            EndQuery();
        }

        static void RenderHeightmapDepthPrePass()
        {
            SFMapHeightMap heightmap = scene.map.heightmap;

            GL.Uniform1(active_shader["GridSize"], heightmap.width);
            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
            {
                GL.BindVertexArray(heightmap.mesh.vertex_array);
            }
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                GL.BindVertexArray(heightmap.mesh_tesselated.vertex_array);
                GL.Uniform3(active_shader["cameraPos"], scene.camera.position);
            }

            SetTexture(4, TextureTarget.Texture2D, heightmap.height_data_texture.tex_id);

            Matrix4 vp_mat = scene.camera.ViewProjMatrix;
            GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);

            StartQuery();
            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
            {
                GL.DrawElements(PrimitiveType.TriangleStrip, 2 * (heightmap.width + 1) * heightmap.height, DrawElementsType.UnsignedInt, 0);
            }
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                GL.DrawArrays(PrimitiveType.Patches, 0, 4 * heightmap.mesh_tesselated.patch_count);
            }
            EndQuery();
        }

        static void RenderHeightmap()
        {
            SFMapHeightMap heightmap = scene.map.heightmap;

            Matrix4 lsm_mat = scene.atmosphere.sun_light.LightMatrix;

            GL.Uniform1(active_shader["GridSize"], heightmap.width);
            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
            {
                GL.BindVertexArray(heightmap.mesh.vertex_array);
            }
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                GL.BindVertexArray(heightmap.mesh_tesselated.vertex_array);
                GL.Uniform3(active_shader["cameraPos"], scene.camera.position);
            }

            SetTexture(4, TextureTarget.Texture2D, heightmap.height_data_texture.tex_id);

            if (current_pass == RenderPass.SCENE)
            {
                if (Settings.EditorMode)
                {
                    SetTexture(3, TextureTarget.Texture2D, (heightmap.overlay_texture == null ? opaque_tex.tex_id : heightmap.overlay_texture.tex_id));
                }

                SetTexture(5, TextureTarget.Texture2D, heightmap.terrain_texture_lod_bump.tex_id);
                SetTexture(2, TextureTarget.Texture2D, heightmap.tile_data_texture.tex_id);
                SetTexture(0, TextureTarget.Texture2DArray, heightmap.texture_manager.terrain_texture);
                Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                if (Settings.EnableShadows)
                {
                    GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
                }

                GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
                if (Settings.EditorMode)
                {
                    GL.Uniform4(active_shader["GridColor"], Settings.GridColor);
                    GL.Uniform1(active_shader["CurrentFlags"], (Settings.OverlaysVisible ? (ushort)heightmap.overlay_flags : 0));
                }
                if (Settings.ShadingQuality >= 2)
                {
                    GL.Uniform3(active_shader["ViewPos"], scene.camera.position);
                }
            }
            else if (current_pass == RenderPass.SHADOWMAP)
            {
                GL.UniformMatrix4(active_shader["VP"], false, ref lsm_mat);
                SetTexture(0, TextureTarget.Texture2D, 0);
            }

            StartQuery();
            if (Settings.TerrainLOD == SFMapHeightMapLOD.NONE)
            {
                GL.DrawElements(PrimitiveType.TriangleStrip, 2 * (heightmap.width + 1) * heightmap.height, DrawElementsType.UnsignedInt, 0);
            }
            else if (Settings.TerrainLOD == SFMapHeightMapLOD.TESSELATION)
            {
                GL.DrawArrays(PrimitiveType.Patches, 0, 4 * heightmap.mesh_tesselated.patch_count);
            }
            EndQuery();
        }

        static void ApplyMaterial(SFMaterial mat)
        {
            if(Settings.ShadingQuality >= 1)
            {
                SetDistanceFade(mat.distance_fade);
                SetApplyShading(mat.apply_shading);
                if (Settings.ShadingQuality >= 2)
                {
                    SetEmission(mat.emission_color * mat.emission_strength);
                }
            }

            SetRenderMode(mat.texRenderMode);
            SetDepthBias(mat.matDepthBias);
            SetTexture(0, TextureTarget.Texture2D, mat.texture.tex_id);
            SetCullEnabled((mat.matFlags & 4) != 0);
            if(CurrentCullEnabled)
            {
                SetCullMode(CullFaceMode.Front);
            }
        }

        static void RenderStaticObjectsShadowmap(IEnumerable<SFSubModel3D> models)
        {
            Matrix4 lsm_mat = scene.atmosphere.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["VP"], false, ref lsm_mat);

            foreach (SFSubModel3D submodel in models)
            {
                SFMaterial mat = submodel.material;
                if (!mat.casts_shadow)
                {
                    continue;
                }

                int mii = submodel.cache_index;

                if ((mat.matFlags & 4) == 0)
                {
                    SetTexture(0, TextureTarget.Texture2D, mat.texture.tex_id);
                }
                else
                {
                    SetTexture(0, TextureTarget.Texture2D, opaque_tex.tex_id);
                }

                int vri = SFSubModel3D.Cache.Meshes.elements[mii].VertexRangeIndex;
                int eri = SFSubModel3D.Cache.Meshes.elements[mii].ElementRangeIndex;

                StartQuery();
                GL.DrawElementsInstancedBaseVertexBaseInstance(PrimitiveType.Triangles,
                    SFSubModel3D.Cache.ElementRanges[eri].Count,
                    DrawElementsType.UnsignedInt,
                    new IntPtr(SFSubModel3D.Cache.ElementRanges[eri].Start * 4),
                    submodel.owner.MatrixCount,
                    SFSubModel3D.Cache.VertexRanges[vri].Start,
                    submodel.owner.MatrixOffset);
                EndQuery();
            }
        }

        static void RenderStaticObjectsScene(IEnumerable<SFSubModel3D> models, float alpha_cutout)
        {
            SetDepthBias(0);
            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

            Matrix4 vp_mat = scene.camera.ViewProjMatrix;
            GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
            if (Settings.EnableShadows)
            {
                Matrix4 lsm_mat = scene.atmosphere.sun_light.LightMatrix;
                GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
            }

            if (Settings.ShadingQuality >= 2)
            {
                GL.Uniform1(active_shader["GridSize"], scene.map.heightmap.width);
                GL.Uniform3(active_shader["ViewPos"], scene.camera.position);
                SetTexture(3, TextureTarget.Texture2D, scene.map.heightmap.tile_data_texture.tex_id);
            }
            GL.Uniform1(active_shader["AlphaCutout"], alpha_cutout);

            foreach (SFSubModel3D submodel in models)
            {
                int mii = submodel.cache_index;

                SFMaterial mat = submodel.material;

                ApplyMaterial(mat);

                int vri = SFSubModel3D.Cache.Meshes.elements[mii].VertexRangeIndex;
                int eri = SFSubModel3D.Cache.Meshes.elements[mii].ElementRangeIndex;

                StartQuery();
                GL.DrawElementsInstancedBaseVertexBaseInstance(PrimitiveType.Triangles,
                    SFSubModel3D.Cache.ElementRanges[eri].Count,
                    DrawElementsType.UnsignedInt,
                    new IntPtr(SFSubModel3D.Cache.ElementRanges[eri].Start * 4),
                    submodel.owner.MatrixCount,
                    SFSubModel3D.Cache.VertexRanges[vri].Start,
                    submodel.owner.MatrixOffset);
                EndQuery();
            }
        }

        // this is very slow, dunno
        static void RenderAnimatedObjects()
        {
            Matrix4 lsm_mat = scene.atmosphere.sun_light.LightMatrix;

            if (current_pass == RenderPass.SCENE)
            {
                Matrix4 p_mat = scene.camera.ProjMatrix;
                GL.UniformMatrix4(active_shader["P"], false, ref p_mat);
                Matrix4 v_mat = scene.camera.ViewMatrix;
                GL.UniformMatrix4(active_shader["V"], false, ref v_mat);
                if (Settings.EnableShadows)
                {
                    GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
                }

                if (Settings.ShadingQuality >= 2)
                {
                    GL.Uniform1(active_shader["GridSize"], scene.map.heightmap.width);
                    SetTexture(2, TextureTarget.Texture2D, scene.map.heightmap.tile_data_texture.tex_id);
                    GL.Uniform3(active_shader["ViewPos"], scene.camera.position);
                }
                GL.Uniform1(active_shader["AlphaCutout"], 0.01f);
            }
            else if (current_pass == RenderPass.SHADOWMAP)
            {
                Matrix4 p_mat = Matrix4.Identity;
                GL.UniformMatrix4(active_shader["P"], false, ref p_mat);
                GL.UniformMatrix4(active_shader["V"], false, ref lsm_mat);
            }

            foreach (SceneNodeAnimated an in scene.an_primary_nodes)
            {
                GL.UniformMatrix4(active_shader["M"], false, ref an.result_transform);
                GL.UniformMatrix4(active_shader["boneTransforms"], an.BoneTransforms.Length, false, ref an.BoneTransforms[0].Row0.X);

                // if an is in an_nodes, it must have skin
                for (int n = 0; n < an.DrivenNodes.Count; n++)
                {
                    SFModelSkin skin = an.DrivenNodes[n].Skin;
                    if(skin == null)
                    {
                        continue;
                    }

                    for (int i = 0; i < skin.submodels.Length; i++)
                    {
                        var msc = skin.submodels[i];
                        if (current_pass == RenderPass.SHADOWMAP)
                        {
                            if ((msc.material.matFlags & 4) == 0)
                            {
                                SetTexture(0, TextureTarget.Texture2D, msc.material.texture.tex_id);
                            }
                            else
                            {
                                SetTexture(0, TextureTarget.Texture2D, opaque_tex.tex_id);
                            }
                        }
                        else
                        {
                            ApplyMaterial(msc.material);
                        }

                        int vri = SFModelSkinChunk.Cache.Meshes.elements[msc.cache_index].VertexRangeIndex;
                        int eri = SFModelSkinChunk.Cache.Meshes.elements[msc.cache_index].ElementRangeIndex;

                        StartQuery();
                        GL.DrawElementsBaseVertex(PrimitiveType.Triangles,
                            SFModelSkinChunk.Cache.ElementRanges[eri].Count,
                            DrawElementsType.UnsignedInt,
                            new IntPtr(SFModelSkinChunk.Cache.ElementRanges[eri].Start * 4),
                            SFModelSkinChunk.Cache.VertexRanges[vri].Start);
                        EndQuery();
                    }
                }
            }
        }

        public static void RenderUI()
        {
            GL.Uniform2(active_shader["ScreenSize"], ref render_size);

            foreach (SFTexture tex in ui.storages.Keys)
            {
                SetTexture(0, TextureTarget.Texture2D, tex.tex_id);

                UIQuadStorage storage = ui.storages[tex];
                GL.BindVertexArray(storage.vertex_array);

                for (int i = 0; i < storage.spans.Count; i++)
                {
                    if (!storage.spans[i].visible)
                    {
                        continue;
                    }

                    if (storage.spans[i].used == 0)
                    {
                        continue;
                    }

                    GL.Uniform2(active_shader["offset"], storage.spans[i].position);
                    StartQuery();
                    GL.DrawArrays(PrimitiveType.Triangles, storage.spans[i].start * 6, storage.spans[i].used * 6);
                    EndQuery();
                }
            }
        }

        // special way to render animated selection around node
        public static void RenderSelection(SceneNode node)
        {
            if (node is SceneNodeSimple)
            {
                SceneNodeSimple n = (SceneNodeSimple)node;
                if ((n.visible) && (n.Mesh != null))
                {
                    GL.BindVertexArray(SFSubModel3D.Cache.VertexArrayObjectID);
                    UseShader(shader_selection);

                    Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                    GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);

                    GL.Uniform1(active_shader["Time"], scene.current_time * 2.0f);
                    GL.Uniform4(active_shader["Color"], new Vector4(0.1f, 0.1f, 0.1f, 0.8f));

                    foreach (var submodel in n.Mesh.submodels)
                    {
                        int mii = submodel.cache_index;

                        SetTexture(0, TextureTarget.Texture2D, submodel.material.texture.tex_id);

                        int vri = SFSubModel3D.Cache.Meshes.elements[mii].VertexRangeIndex;
                        int eri = SFSubModel3D.Cache.Meshes.elements[mii].ElementRangeIndex;

                        StartQuery();
                        GL.DrawElementsInstancedBaseVertexBaseInstance(PrimitiveType.Triangles,
                            SFSubModel3D.Cache.ElementRanges[eri].Count,
                            DrawElementsType.UnsignedInt,
                            new IntPtr(SFSubModel3D.Cache.ElementRanges[eri].Start * 4),
                            1,
                            SFSubModel3D.Cache.VertexRanges[vri].Start,
                            n.Mesh.MatrixOffset + n.CurrentMeshMatrixIndex);
                        EndQuery();
                    }
                }
            }
            else if (node is SceneNodeAnimated)
            {
                SceneNodeAnimated an = (SceneNodeAnimated)node;

                if ((an.visible) && (an.Skin != null))
                {
                    GL.BindVertexArray(SFModelSkinChunk.Cache.VertexArrayObjectID);
                    UseShader(shader_selection_animated);

                    GL.UniformMatrix4(active_shader["M"], false, ref an.result_transform);
                    Matrix4 p_mat = scene.camera.ProjMatrix;
                    GL.UniformMatrix4(active_shader["P"], false, ref p_mat);
                    Matrix4 v_mat = scene.camera.ViewMatrix;
                    GL.UniformMatrix4(active_shader["V"], false, ref v_mat);

                    GL.Uniform1(active_shader["Time"], scene.current_time * 2.0f);
                    GL.Uniform4(active_shader["Color"], new Vector4(0.1f, 0.1f, 0.1f, 0.8f));

                    GL.UniformMatrix4(active_shader["M"], false, ref an.result_transform);
                    GL.UniformMatrix4(active_shader["boneTransforms"], an.BoneTransforms.Length, false, ref an.BoneTransforms[0].Row0.X);

                    // if an is in an_nodes, it must have skin
                    for (int n = 0; n < an.DrivenNodes.Count; n++)
                    {
                        SFModelSkin skin = an.DrivenNodes[n].Skin;
                        if (skin == null)
                        {
                            continue;
                        }

                        for (int i = 0; i < skin.submodels.Length; i++)
                        {
                            var msc = skin.submodels[i];
                            SetTexture(0, TextureTarget.Texture2D, msc.material.texture.tex_id);

                            int vri = SFModelSkinChunk.Cache.Meshes.elements[msc.cache_index].VertexRangeIndex;
                            int eri = SFModelSkinChunk.Cache.Meshes.elements[msc.cache_index].ElementRangeIndex;

                            StartQuery();
                            GL.DrawElementsBaseVertex(PrimitiveType.Triangles,
                                SFModelSkinChunk.Cache.ElementRanges[eri].Count,
                                DrawElementsType.UnsignedInt,
                                new IntPtr(SFModelSkinChunk.Cache.ElementRanges[eri].Start * 4),
                                SFModelSkinChunk.Cache.VertexRanges[vri].Start);
                            EndQuery();
                        }
                    }
                }
            }

            foreach (SceneNode n in node.Children)
            {
                RenderSelection(n);
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
#if DEBUG
            if (is_debug)
            {
                current_query = 0;
                query_results.Clear();
            }
#endif //DEBUG

            // generate shadowmap
            current_pass = RenderPass.SHADOWMAP;
            GL.Disable(EnableCap.Blend);

            if (Settings.EnableShadows)
            {
                // depth testing enabled for shadowmap rendering, cull front faces (helps with variance/moment shadow mapping technique)
                GL.Enable(EnableCap.DepthTest);

                // clear existing shadowmap
                if (Settings.ShadowType == Settings.ShadowMapTechnique.VSM)
                {
                    SetFramebuffer(shadowmap_vsm_multisample);
                }
                else if (Settings.ShadowType == Settings.ShadowMapTechnique.MSM)
                {
                    SetFramebuffer(shadowmap_msm_multisample);
                }
                GL.Clear(ClearBufferMask.DepthBufferBit);

                // draw heightmap shadows
                if (enable_flags.HasFlag(RenderEnable.SHADOWMAP_HMAP))
                {
                    if (scene.map != null)
                    {
                        UseShader(shader_shadowmap_heightmap);
                        RenderHeightmap();
                    }
                }

                SetCullEnabled(true);
                SetCullMode(CullFaceMode.Front);
                // draw animated object shadows
                GL.BindVertexArray(SFModelSkinChunk.Cache.VertexArrayObjectID);
                if (enable_flags.HasFlag(RenderEnable.SHADOWMAP_ANIM))
                {
                    UseShader(shader_shadowmap_animated);
                    RenderAnimatedObjects();
                }

                // draw simple (and additive!) object shadows
                GL.BindVertexArray(SFSubModel3D.Cache.VertexArrayObjectID);
                if (enable_flags.HasFlag(RenderEnable.SHADOWMAP_SIMPLE))
                {
                    UseShader(shader_shadowmap);
                    RenderStaticObjectsShadowmap(scene.opaque_pass_models);
                    RenderStaticObjectsShadowmap(scene.additive_pass_models);
                }

                // revert cull mode, disable depth test
                GL.Disable(EnableCap.DepthTest);
                SetCullEnabled(false);

                // resolve multisample
                if (Settings.ShadowType == Settings.ShadowMapTechnique.VSM)
                {
                    UseShader(shader_vsm_resolve);
                    SetFramebuffer(shadowmap_vsm_base);
                    SetTexture(0, TextureTarget.Texture2DMultisample, shadowmap_vsm_multisample.textures[0].tex_id);
                }
                else if (Settings.ShadowType == Settings.ShadowMapTechnique.MSM)
                {
                    UseShader(shader_msm_resolve);
                    SetFramebuffer(shadowmap_msm_base);
                    SetTexture(0, TextureTarget.Texture2DMultisample, shadowmap_msm_multisample.textures[0].tex_id);
                }
                GL.BindVertexArray(FrameBuffer.screen_vao);
                GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Uniform1(active_shader["TextureSize"], Settings.ShadowMapSize);
                StartQuery();
                GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                EndQuery();

                // variance/moment shadow mapping technique allows smooth shadows by simply blurring the shadowmap
                if ((enable_flags.HasFlag(RenderEnable.SHADOWMAP_BLUR)) && (Settings.SoftShadows))
                {
                    GL.BindVertexArray(FrameBuffer.screen_vao);
                    UseShader(shader_shadowmap_blur);
                    if (Settings.ShadowType == Settings.ShadowMapTechnique.VSM)
                    {
                        // blur shadowmap horizontally
                        SetFramebuffer(shadowmap_vsm_hpass);
                        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                        GL.Clear(ClearBufferMask.ColorBufferBit);

                        SetTexture(0, TextureTarget.Texture2D, shadowmap_vsm_base.textures[0].tex_id);
                        GL.Uniform1(active_shader["horizontal"], 1);
                        StartQuery();
                        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                        EndQuery();
                        // blur shadowmap vertically

                        SetFramebuffer(shadowmap_vsm_base);
                        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                        GL.Clear(ClearBufferMask.ColorBufferBit);

                        SetTexture(0, TextureTarget.Texture2D, shadowmap_vsm_hpass.textures[0].tex_id);
                        GL.Uniform1(active_shader["horizontal"], 0);
                        StartQuery();
                        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                        EndQuery();
                    }
                    else if (Settings.ShadowType == Settings.ShadowMapTechnique.MSM)
                    {
                        // blur shadowmap horizontally
                        SetFramebuffer(shadowmap_msm_hpass);
                        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                        GL.Clear(ClearBufferMask.ColorBufferBit);

                        SetTexture(0, TextureTarget.Texture2D, shadowmap_msm_base.textures[0].tex_id);
                        GL.Uniform1(active_shader["horizontal"], 1);
                        StartQuery();
                        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                        EndQuery();
                        // blur shadowmap vertically

                        SetFramebuffer(shadowmap_msm_base);
                        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                        GL.Clear(ClearBufferMask.ColorBufferBit);

                        SetTexture(0, TextureTarget.Texture2D, shadowmap_msm_hpass.textures[0].tex_id);
                        GL.Uniform1(active_shader["horizontal"], 0);
                        StartQuery();
                        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                        EndQuery();

                        SetTexture(0, TextureTarget.Texture2D, shadowmap_msm_base.textures[0].tex_id);
                    }
                }

                if (Settings.ShadowType == Settings.ShadowMapTechnique.VSM)
                {
                    SetTexture(0, TextureTarget.Texture2D, shadowmap_vsm_base.textures[0].tex_id);
                }
                else if (Settings.ShadowType == Settings.ShadowMapTechnique.MSM)
                {
                    SetTexture(0, TextureTarget.Texture2D, shadowmap_msm_base.textures[0].tex_id);
                }

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            // render actual view
            current_pass = RenderPass.SCENE;

            // setup blend mode, render to screenspace framebuffer
            GL.Enable(EnableCap.Blend);
            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);
            SetFramebuffer(Settings.AntiAliasingSamples > 1 ? screenspace_framebuffer : screenspace_intermediate);

            // bind generated shadowmap
            if (Settings.EnableShadows)
            {
                if (Settings.ShadowType == Settings.ShadowMapTechnique.VSM)
                {
                    SetTexture(1, TextureTarget.Texture2D, shadowmap_vsm_base.textures[0].tex_id);
                }
                else if (Settings.ShadowType == Settings.ShadowMapTechnique.MSM)
                {
                    SetTexture(1, TextureTarget.Texture2D, shadowmap_msm_base.textures[0].tex_id);
                }
            }

            // clear contents of framebuffer
            if (Settings.ToneMapping)
            {
                GL.ClearColor(scene.atmosphere.FogColor.X * scene.atmosphere.FogStrength,
                    scene.atmosphere.FogColor.Y * scene.atmosphere.FogStrength,
                    scene.atmosphere.FogColor.Z * scene.atmosphere.FogStrength,
                    0.0f);
            }
            else
            {
                GL.ClearColor(scene.atmosphere.ambient_light.Color.X,
                    scene.atmosphere.ambient_light.Color.Y,
                    scene.atmosphere.ambient_light.Color.Z,
                    0.0f);
            }
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // enable depth test
            GL.Enable(EnableCap.DepthTest);
            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

            // depth function set to less or equal, for heightmap pre-pass
            // first draw just heightmap, without anything fancy, only then draw details of the heightmap
            SetCullEnabled(true);
            SetCullMode(CullFaceMode.Back);
            GL.DepthFunc(DepthFunction.Lequal);
            if ((scene.map != null) && (enable_flags.HasFlag(RenderEnable.HMAP)))
            {
                if ((Settings.TerrainTextureLOD != 2) || (Settings.ForceTerrainTextureLOD1))
                {
                    // depth pre-pass
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
                RenderStaticObjectsScene(scene.opaque_pass_models, 0.9f);
            }

            // animated objects
            GL.BindVertexArray(SFModelSkinChunk.Cache.VertexArrayObjectID);

            if (enable_flags.HasFlag(RenderEnable.ANIM))
            {
                UseShader(shader_animated);
                RenderAnimatedObjects();
            }

            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);


            // disable depth write
            // note that this is done *after* water is drawn, to improve performance and reduce artifacts involved with drawing transparent things underwater
            GL.DepthMask(false);
            SetCullEnabled(false);
            // render sky
            if ((Settings.ToneMapping) && (enable_flags.HasFlag(RenderEnable.SKY)))
            {
                UseShader(shader_sky);
                RenderSky();
            }

            // water
            GL.BindVertexArray(SFSubModel3D.Cache.VertexArrayObjectID);
            GL.DepthMask(true);

            if (enable_flags.HasFlag(RenderEnable.WATER))
            {
                UseShader(shader_simple_transparency);
                RenderStaticObjectsScene(scene.water_pass_models, 0.01f);
            }

            GL.DepthMask(false);

            // simple transparent objects
            if (enable_flags.HasFlag(RenderEnable.TRANSPARENT))
            {
                UseShader(shader_simple_transparency);
                GL.DepthFunc(DepthFunction.Less);
                RenderStaticObjectsScene(scene.transparent_pass_models, 0.01f);
                GL.DepthFunc(DepthFunction.Lequal);
            }

            // simple additive objects
            if (enable_flags.HasFlag(RenderEnable.SIMPLE))
            {
                UseShader(shader_simple);
                RenderStaticObjectsScene(scene.additive_pass_models, 0.9f);
            }

            // selection last
            // disable depth test, cull front
            if ((scene.selected_node != null) && (scene.selected_node.Parent != null))
            {
                GL.Disable(EnableCap.DepthTest);
                SetRenderMode(RenderMode.ONE_ONE);
                // shader is set in this function
                RenderSelection(scene.selected_node);
            }
            SetCullEnabled(false);

            // re-enable depth write
            GL.DepthMask(true);

            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

            // what is below doesnt depend on whats above

            // anitialiasing - move antialiased framebuffer to the non-antialiased one, since nothing else will rely on multisampled AA
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
            SetTexture(0, TextureTarget.Texture2D, screenspace_intermediate.textures[0].tex_id);
            if ((Settings.ToneMapping) && (enable_flags.HasFlag(RenderEnable.TONEMAP)))
            {
                UseShader(shader_framebuffer_tonemapped);
                GL.Uniform1(active_shader["exposure"], 1.0f);
            }
            else
            {
                UseShader(shader_framebuffer_simple);
            }
            GL.BindVertexArray(FrameBuffer.screen_vao);
            StartQuery();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            EndQuery();

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
#if DEBUG
            if (is_debug)
            {
                for (int i = 0; i < current_query; i++)
                {
                    int result = 0;
                    GL.GetQueryObject(queries[i], GetQueryObjectParam.QueryResult, out result);
                    query_results.Add(i, result);
                }

                is_debug = false;
            }
#endif //DEBUG
            scene.frame_counter++;
        }
    }
}
