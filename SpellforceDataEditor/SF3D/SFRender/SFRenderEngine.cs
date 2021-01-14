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
        public static float max_render_distance = 200f;
        static float CurrentDepthBias = 0;
        static RenderMode CurrentRenderMode = RenderMode.SRCALPHA_INVSRCALPHA;

        public static SFTexture opaque_tex { get; private set; } = null;

        static SFShader shader_simple = new SFShader();
        static SFShader shader_animated = new SFShader();
        static SFShader shader_heightmap = new SFShader();
        static SFShader shader_shadowmap = new SFShader();
        static SFShader shader_shadowmap_animated = new SFShader();
        static SFShader shader_shadowmap_heightmap = new SFShader();
        static SFShader shader_ui = new SFShader();
        static SFShader active_shader = null;
        static RenderPass current_pass = RenderPass.NONE;
        

        static SFShader shader_framebuffer_simple = new SFShader();
        static FrameBuffer shadowmap_depth = null;
        static FrameBuffer screenspace_framebuffer = null;
        static FrameBuffer screenspace_intermediate = null;
        public static bool render_shadowmap_depth = false;

        public static Vector2 render_size = Vector2.Zero;

        static bool initialized = false;
        static int drawcalls_simple = 0;
        static int drawcalls_anim = 0;
        static int drawcalls_hmap = 0;
        static int drawcalls_ui = 0;
        static int triangles = 0;

        public static bool is_rendering = false;
        public static bool prepare_dump = false;
        private static StringBuilder dump_sb = null;
        private static string dump_preline = "";
        private static Dictionary<SFTexture, int> dump_texture_binds = new Dictionary<SFTexture, int>();
        private static Dictionary<SFTexture, int> dump_texture_simpledraw = new Dictionary<SFTexture, int>();
        private static Dictionary<SFTexture, int> dump_texture_animdraw = new Dictionary<SFTexture, int>();
        private static Dictionary<SFModel3D, int[]> dump_mesh_draw = new Dictionary<SFModel3D, int[]>();
        private static Dictionary<SFModelSkin, int> dump_skin_draw = new Dictionary<SFModelSkin, int>();

        private static void DebugCallback(DebugSource source,
                                    DebugType type,
                                    int id,
                                    DebugSeverity severity,
                                    int length,
                                    IntPtr message,
                                    IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            //Console.WriteLine($"{Enum.GetName(typeof(DebugSeverity), severity)}" +
            //   $" {Enum.GetName(typeof(DebugType), type) } | {messageString}");

            if (type == DebugType.DebugTypeError)
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "OPENGL DEBUG CALLBACK: " + messageString);
        }

        private static DebugProc _debugProcCallback = DebugCallback;
        private static GCHandle _debugProcCallbackHandle;


        // frame dump utility
        private static void DumpStart()
        {
            if (dump_sb != null)
                return;

            dump_sb = new StringBuilder();
            dump_texture_binds.Clear();
            dump_texture_simpledraw.Clear();
            dump_texture_animdraw.Clear();
        }

        private static void DumpAddTexDict(SFTexture tex, int dict_id)
        {
            if (dump_sb == null)
                return;

            if (!dump_texture_binds.ContainsKey(tex))
            {
                dump_texture_binds.Add(tex, 0);
                dump_texture_simpledraw.Add(tex, 0);
                dump_texture_animdraw.Add(tex, 0);
            }

            switch (dict_id)
            {
                case 0:
                    dump_texture_binds[tex] += 1;
                    break;
                case 1:
                    dump_texture_simpledraw[tex] += 1;
                    break;
                case 2:
                    dump_texture_animdraw[tex] += 1;
                    break;
            }
        }

        private static void DumpAddMeshDict(SFModel3D mesh, int submodel_index)
        {
            if (dump_sb == null)
                return;

            if (!dump_mesh_draw.ContainsKey(mesh))
            {
                dump_mesh_draw.Add(mesh, new int[mesh.submodels.Length]);
                dump_mesh_draw[mesh].Initialize();
            }

            dump_mesh_draw[mesh][submodel_index] += 1;
        }
        private static void DumpAddSkinDict(SFModelSkin skin)
        {
            if (dump_sb == null)
                return;

            if (!dump_skin_draw.ContainsKey(skin))
                dump_skin_draw.Add(skin, 0);

            dump_skin_draw[skin] += 1;
        }

        private static void DumpLevelUp()
        {
            dump_preline = dump_preline + "-";
        }

        private static void DumpLevelDown()
        {
            if (dump_preline == "")
                return;

            dump_preline = dump_preline.Substring(1);
        }

        private static void DumpPush(string line)
        {
            dump_sb.AppendLine(dump_preline + line);
        }

        private static void DumpEnd()
        {
            if (dump_sb == null)
                return;

            DumpPush("FRAME DUMP");
            DumpPush("");

            // texture bind amount
            int sum = 0;
            DumpPush("Texture bind stats:");
            DumpLevelUp();
            foreach (var tex in dump_texture_binds.Keys)
            {
                if (dump_texture_binds[tex] == 0)
                    continue;

                DumpPush(tex.GetName() + ": " + dump_texture_binds[tex].ToString());
                sum += dump_texture_binds[tex];
            }
            DumpPush("Total binds: " + sum.ToString());
            DumpLevelDown();
            DumpPush("");

            sum = 0;
            DumpPush("Simple model draws with given texture (texture name, draw count):");
            DumpLevelUp();
            foreach (var tex in dump_texture_simpledraw.Keys)
            {
                if (dump_texture_simpledraw[tex] == 0)
                    continue;

                DumpPush(tex.GetName() + ": " + dump_texture_simpledraw[tex].ToString());
                sum += dump_texture_simpledraw[tex];
            }
            DumpPush("Total simple model draws: " + sum.ToString());
            DumpLevelDown();
            DumpPush("");

            sum = 0;
            DumpPush("Animated model draws with given texture (texture name, draw count):");
            DumpLevelUp();
            foreach (var tex in dump_texture_animdraw.Keys)
            {
                if (dump_texture_animdraw[tex] == 0)
                    continue;

                DumpPush(tex.GetName() + ": " + dump_texture_animdraw[tex].ToString());
                sum += dump_texture_animdraw[tex];
            }
            DumpPush("Total animated model draws: " + sum.ToString());
            DumpLevelDown();
            DumpPush("");

            sum = 0;
            DumpPush("Simple model draw stats (model name, per submodel draw count, total draw count):");
            DumpLevelUp();
            foreach (var mesh in dump_mesh_draw.Keys)
            {
                int msum = 0;
                string s = "[";
                foreach (var i in dump_mesh_draw[mesh])
                {
                    s += i.ToString() + ", ";
                    msum += i;
                }
                s = s.Substring(0, s.Length - 2);
                s += "]";

                if (msum == 0)
                    continue;

                DumpPush(mesh.GetName() + ": " + s.ToString() + ", " + msum.ToString());
                sum += msum;
            }
            DumpPush("Total simple model draws: " + sum.ToString());
            DumpLevelDown();
            DumpPush("");

            sum = 0;
            DumpPush("Animated model draw stats (model name, total draw count):");
            DumpLevelUp();
            foreach (var skin in dump_skin_draw.Keys)
            {
                if (dump_skin_draw[skin] == 0)
                    continue;

                DumpPush(skin.GetName() + ": " + dump_skin_draw[skin].ToString());
                sum += dump_skin_draw[skin];
            }
            DumpPush("Total animated model draws: " + sum.ToString());
            DumpLevelDown();
            DumpPush("");

            DumpPush("Measured simple model drawcalls: " + drawcalls_simple.ToString());
            DumpPush("Measured animated model drawcalls: " + drawcalls_anim.ToString());
            DumpPush("Measured heightmap drawcalls: " + drawcalls_hmap.ToString());
            DumpPush("Measured UI drawcalls: " + drawcalls_ui.ToString());
            DumpPush("Measured triangles drawn: " + triangles.ToString());


            File.WriteAllText("frame_dump.txt", dump_sb.ToString());

            dump_sb = null;
        }


        //called only once!
        public static void Initialize(Vector2 view_size)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.Initialize() called");

            if (!initialized)
            {
                _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);

                GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);


            }

            // initialize static model cache
            if(SFSubModel3D.Cache != null)
                SFSubModel3D.Cache.Dispose();
            SFSubModel3D.Cache = new MeshCache();
            SFSubModel3D.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // positions
            SFSubModel3D.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // normals
            SFSubModel3D.Cache.AddVertexAttribute(4, VertexAttribPointerType.UnsignedByte, true);   // colors
            SFSubModel3D.Cache.AddVertexAttribute(2, VertexAttribPointerType.Float, false);   // UVs
            SFSubModel3D.Cache.SetVertexSize(40);
            SFSubModel3D.Cache.Init(2 << 14, 2 << 14);

            if (SFModelSkinChunk.Cache != null)
                SFModelSkinChunk.Cache.Dispose();
            SFModelSkinChunk.Cache = new MeshCache();
            SFModelSkinChunk.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // positions
            SFModelSkinChunk.Cache.AddVertexAttribute(3, VertexAttribPointerType.Float, false);   // normals
            SFModelSkinChunk.Cache.AddVertexAttribute(4, VertexAttribPointerType.UnsignedByte, true);   // bone weights
            SFModelSkinChunk.Cache.AddVertexAttribute(2, VertexAttribPointerType.Float, false);   // uvs
            SFModelSkinChunk.Cache.AddVertexAttribute(4, VertexAttribPointerType.UnsignedByte, false);
            SFModelSkinChunk.Cache.SetVertexSize(40);
            SFModelSkinChunk.Cache.Init(2 << 6, 2 << 6);

            scene.sun_light.SetLightDirection(-(new Vector3(0, -1, 0).Normalized()));
            scene.sun_light.Color = new Vector4(1, 1, 1f, 1);
            scene.sun_light.Strength = 1.6f;
            scene.ambient_light.Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            scene.ambient_light.Strength = 0.8f;
            scene.atmosphere.FogColor = new Vector4(0.55f, 0.55f, 0.85f, 1.0f);
            scene.atmosphere.FogStart = max_render_distance * 0.5f;
            scene.atmosphere.FogEnd = max_render_distance;

            shader_shadowmap.CompileShader(Properties.Resources.vshader_shadowmap, Properties.Resources.fshader_shadowmap);
            shader_shadowmap.AddParameter("LSM");
            shader_shadowmap.AddParameter("M");
            shader_shadowmap.AddParameter("DiffuseTexture");

            shader_shadowmap_animated.CompileShader(Properties.Resources.vshader_shadowmap_animated, Properties.Resources.fshader_shadowmap);
            shader_shadowmap_animated.AddParameter("LSM");
            shader_shadowmap_animated.AddParameter("M");
            shader_shadowmap_animated.AddParameter("boneTransforms");
            shader_shadowmap_animated.AddParameter("DiffuseTexture");

            shader_shadowmap_heightmap.CompileShader(Properties.Resources.vshader_shadowmap_heightmap, Properties.Resources.fshader_shadowmap);
            shader_shadowmap_heightmap.AddParameter("LSM");
            shader_shadowmap_heightmap.AddParameter("M");
            shader_shadowmap_heightmap.AddParameter("DiffuseTexture");

            shader_framebuffer_simple.CompileShader(Properties.Resources.vshader_framebuffer, Properties.Resources.fshader_framebuffer_simple);
            shader_framebuffer_simple.AddParameter("renderShadowMap");
            shader_framebuffer_simple.AddParameter("ZNear");
            shader_framebuffer_simple.AddParameter("ZFar");

            shader_ui.CompileShader(Properties.Resources.vshader_ui, Properties.Resources.fshader_ui);
            shader_ui.AddParameter("Tex");
            shader_ui.AddParameter("ScreenSize");
            shader_ui.AddParameter("offset");

            RecompileMainShaders();

            GL.ClearColor(scene.atmosphere.FogColor.X, scene.atmosphere.FogColor.Y,
                          scene.atmosphere.FogColor.Z, scene.atmosphere.FogColor.W);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.StencilTest);
            GL.Enable(EnableCap.Blend);
            GL.DepthFunc(DepthFunction.Less);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            render_size = view_size;
            ResizeView(view_size);

            if (screenspace_framebuffer != null)
            {
                shadowmap_depth.Dispose();
                screenspace_framebuffer.Dispose();
                screenspace_intermediate.Dispose();
            }
            shadowmap_depth = new FrameBuffer(Settings.ShadowMapSize, Settings.ShadowMapSize, 0, FrameBuffer.TextureType.DEPTH, FrameBuffer.RenderBufferType.NONE);
            screenspace_framebuffer = new FrameBuffer((int)view_size.X, (int)view_size.Y, Settings.AntiAliasingSamples);
            screenspace_intermediate = new FrameBuffer((int)view_size.X, (int)view_size.Y, 0);

            if(opaque_tex != null)
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
            shader_simple.SetDefine("APPLY_SHADING", (Settings.EnableShadows));
            shader_animated.SetDefine("APPLY_SHADING", Settings.EnableShadows);
            shader_heightmap.SetDefine("DISPLAY_GRID", Settings.DisplayGrid);
            shader_heightmap.SetDefine("VISUALIZE_HEIGHT", Settings.VisualizeHeight);

            shader_simple.CompileShader(Properties.Resources.vshader, Properties.Resources.fshader);
            shader_simple.AddParameter("VP");
            shader_simple.AddParameter("M");
            shader_simple.AddParameter("LSM");
            shader_simple.AddParameter("DiffuseTexture");
            shader_simple.AddParameter("ShadowMap");
            shader_simple.AddParameter("SunDirection");
            shader_simple.AddParameter("SunStrength");
            shader_simple.AddParameter("SunColor");
            shader_simple.AddParameter("AmbientStrength");
            shader_simple.AddParameter("AmbientColor");
            shader_simple.AddParameter("FogColor");
            shader_simple.AddParameter("FogStart");
            shader_simple.AddParameter("FogEnd");
            shader_simple.AddParameter("DepthBias");
            shader_simple.AddParameter("ShadowDepth");

            shader_animated.CompileShader(Properties.Resources.vshader_skel, Properties.Resources.fshader_skel);
            shader_animated.AddParameter("P");
            shader_animated.AddParameter("V");
            shader_animated.AddParameter("M");
            shader_animated.AddParameter("LSM");
            shader_animated.AddParameter("DiffuseTexture");
            shader_animated.AddParameter("ShadowMap");
            shader_animated.AddParameter("boneTransforms");
            shader_animated.AddParameter("SunDirection");
            shader_animated.AddParameter("SunStrength");
            shader_animated.AddParameter("SunColor");
            shader_animated.AddParameter("AmbientStrength");
            shader_animated.AddParameter("AmbientColor");
            shader_animated.AddParameter("FogColor");
            shader_animated.AddParameter("FogStart");
            shader_animated.AddParameter("FogEnd");
            shader_animated.AddParameter("ShadowDepth");

            shader_heightmap.CompileShader(Properties.Resources.vshader_hmap, Properties.Resources.fshader_hmap);
            shader_heightmap.AddParameter("VP");
            shader_heightmap.AddParameter("M");
            shader_heightmap.AddParameter("GridSize");
            shader_heightmap.AddParameter("GridColor");
            shader_heightmap.AddParameter("LSM");
            shader_heightmap.AddParameter("ShadowMap");
            shader_heightmap.AddParameter("TileMap");
            shader_heightmap.AddParameter("OverlayMap");
            shader_heightmap.AddParameter("SunDirection");
            shader_heightmap.AddParameter("SunStrength");
            shader_heightmap.AddParameter("SunColor");
            shader_heightmap.AddParameter("AmbientStrength");
            shader_heightmap.AddParameter("AmbientColor");
            shader_heightmap.AddParameter("FogColor");
            shader_heightmap.AddParameter("FogStart");
            shader_heightmap.AddParameter("FogEnd");
            shader_heightmap.AddParameter("ShadowDepth");
            // tiledata ubo binding
            int uniform_tiledata = GL.GetUniformBlockIndex(shader_heightmap.ProgramID, "Tiles");
            GL.UniformBlockBinding(shader_heightmap.ProgramID, uniform_tiledata, 0);
            int uniform_overlaycol = GL.GetUniformBlockIndex(shader_heightmap.ProgramID, "Overlays");
            GL.UniformBlockBinding(shader_heightmap.ProgramID, uniform_overlaycol, 1);

            ApplyLight();
            ApplyTexturingUnits();
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
            if (screenspace_framebuffer != null)
            {
                screenspace_framebuffer.Resize((int)view_size.X, (int)view_size.Y);
                screenspace_intermediate.Resize((int)view_size.X, (int)view_size.Y);
            }
        }

        // updates shader lighting parameters
        private static void ApplyLight()
        {
            GL.UseProgram(shader_simple.ProgramID);
            GL.Uniform1(shader_simple["SunStrength"], scene.sun_light.Strength);
            GL.Uniform3(shader_simple["SunDirection"], scene.sun_light.Direction);
            GL.Uniform4(shader_simple["SunColor"], scene.sun_light.Color);
            GL.Uniform1(shader_simple["AmbientStrength"], scene.ambient_light.Strength);
            GL.Uniform4(shader_simple["AmbientColor"], scene.ambient_light.Color);
            GL.Uniform4(shader_simple["FogColor"], scene.atmosphere.FogColor);
            GL.Uniform1(shader_simple["FogStart"], scene.atmosphere.FogStart);
            GL.Uniform1(shader_simple["FogEnd"], scene.atmosphere.FogEnd);

            GL.UseProgram(shader_animated.ProgramID);
            GL.Uniform1(shader_animated["SunStrength"], scene.sun_light.Strength);
            GL.Uniform3(shader_animated["SunDirection"], scene.sun_light.Direction);
            GL.Uniform4(shader_animated["SunColor"], scene.sun_light.Color);
            GL.Uniform1(shader_animated["AmbientStrength"], scene.ambient_light.Strength);
            GL.Uniform4(shader_animated["AmbientColor"], scene.ambient_light.Color);
            GL.Uniform4(shader_animated["FogColor"], scene.atmosphere.FogColor);
            GL.Uniform1(shader_animated["FogStart"], scene.atmosphere.FogStart);
            GL.Uniform1(shader_animated["FogEnd"], scene.atmosphere.FogEnd);

            GL.UseProgram(shader_heightmap.ProgramID);
            GL.Uniform1(shader_heightmap["SunStrength"], scene.sun_light.Strength);
            GL.Uniform3(shader_heightmap["SunDirection"], scene.sun_light.Direction);
            GL.Uniform4(shader_heightmap["SunColor"], scene.sun_light.Color);
            GL.Uniform1(shader_heightmap["AmbientStrength"], scene.ambient_light.Strength);
            GL.Uniform4(shader_heightmap["AmbientColor"], scene.ambient_light.Color);
            GL.Uniform4(shader_heightmap["FogColor"], scene.atmosphere.FogColor);
            GL.Uniform1(shader_heightmap["FogStart"], scene.atmosphere.FogStart);
            GL.Uniform1(shader_heightmap["FogEnd"], scene.atmosphere.FogEnd);

            GL.UseProgram(0);
        }

        // assigns texture slots to shaders
        private static void ApplyTexturingUnits()
        {
            GL.UseProgram(shader_simple.ProgramID);
            GL.Uniform1(shader_simple["DiffuseTexture"], 0);
            GL.Uniform1(shader_simple["ShadowMap"], 1);

            GL.UseProgram(shader_animated.ProgramID);
            GL.Uniform1(shader_animated["DiffuseTexture"], 0);
            GL.Uniform1(shader_animated["ShadowMap"], 1);

            GL.UseProgram(shader_heightmap.ProgramID);
            GL.Uniform1(shader_heightmap["ShadowMap"], 1);
            GL.Uniform1(shader_heightmap["TileMap"], 2);
            GL.Uniform1(shader_heightmap["OverlayMap"], 3);

            GL.UseProgram(shader_shadowmap.ProgramID);
            GL.Uniform1(shader_shadowmap["DiffuseTexture"], 0);

            GL.UseProgram(shader_shadowmap_animated.ProgramID);
            GL.Uniform1(shader_shadowmap_animated["DiffuseTexture"], 0);

            GL.UseProgram(shader_shadowmap_heightmap.ProgramID);
            GL.Uniform1(shader_shadowmap_heightmap["DiffuseTexture"], 0);

            GL.UseProgram(shader_ui.ProgramID);
            GL.Uniform1(shader_ui["Tex"], 0);
            
            GL.UseProgram(0);
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
            if(v != CurrentDepthBias)
            {
                CurrentDepthBias = v;
                GL.Uniform1(active_shader["DepthBias"], v);
            }
        }

        private static void SetRenderMode(RenderMode rm)
        {
            if (CurrentRenderMode == rm)
                return;

            switch(rm)
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
                    //GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
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

        // this could be optimized to not check all chunks every time...
        // turns heightmap nodes visible/invisible depending on if theyre in camera frustum
        public static void UpdateVisibleChunks()
        {
            scene.camera.Update(0);
            // 1. find collection of visible chunks
            List<SceneNodeMapChunk> vis_chunks = new List<SceneNodeMapChunk>();
            SFMapHeightMap heightmap = scene.map.heightmap;

            // test visibility of each chunk

            ParallelOptions loop_options = new ParallelOptions();
            loop_options.MaxDegreeOfParallelism = 4;
            int chunks_per_task = heightmap.chunk_nodes.Length/4;
            List<SceneNodeMapChunk>[] vis_chunks_per_task = new List<SceneNodeMapChunk>[4];
            Parallel.For(0, 4, (i) =>
            {
                vis_chunks_per_task[i] = new List<SceneNodeMapChunk>();
                int end = chunks_per_task * (i + 1);
                for (int j = chunks_per_task * i; j < end; j++)
                {
                    SceneNodeMapChunk chunk_node = heightmap.chunk_nodes[j];
                    chunk_node.DistanceToCamera = Vector2.Distance(
                        new Vector2(chunk_node.MapChunk.aabb.center.X, 
                                    chunk_node.MapChunk.aabb.center.Z),
                        new Vector2(scene.camera.Position.X, scene.camera.Position.Z));
                    chunk_node.CameraHeightDifference = scene.camera.Position.Y - chunk_node.MapChunk.aabb.b.Y;
                    if (chunk_node.DistanceToCamera > 224)  // magic number: zFar+aspect_ratio*sqrt2*chunk size
                        continue;
                    if (!chunk_node.MapChunk.aabb.IsOutsideOfConvexHull(scene.camera.FrustumPlanes))
                        vis_chunks_per_task[i].Add(chunk_node);
                }
            });

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < vis_chunks_per_task[i].Count; j++)
                    vis_chunks.Add(vis_chunks_per_task[i][j]);
            
            // NOTE: chunks are already sorted due to how lists operate
            // 2. compare with existing collection of visible chunks: march two lists at once
            // chunk ID is chunk.iy*map.width/16+chunk.ix
            // any previously visible chunks which are now invisible turn off visibility of its objects
            // vice versa
            int next_list_id = 0, cur_list_id = 0;
            int next_chunk_id, cur_chunk_id;
            bool next_end = false, cur_end = false;
            while(true)
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
                if(next_end)
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
                    while(next_chunk_id > cur_chunk_id)
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
        }

        static void RenderHeightmap()
        {
            SFMapHeightMap heightmap = scene.map.heightmap;

            Matrix4 lsm_mat = scene.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
            GL.BindVertexArray(heightmap.geometry_pool.vertex_array);
            if (current_pass == RenderPass.SCENE)
            {
                GL.ActiveTexture(TextureUnit.Texture3);
                GL.BindTexture(TextureTarget.Texture2D, heightmap.overlay_active_texture);
                GL.ActiveTexture(TextureUnit.Texture2);
                GL.BindTexture(TextureTarget.Texture2D, heightmap.tile_data_texture);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2DArray, heightmap.texture_manager.terrain_texture);
                Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
                GL.Uniform1(active_shader["GridSize"], heightmap.width);
                GL.Uniform4(active_shader["GridColor"], Settings.GridColor);
                GL.Uniform1(active_shader["ShadowDepth"], scene.sun_light.ShadowDepth);
            }
            else if (current_pass == RenderPass.SHADOWMAP)
                GL.BindTexture(TextureTarget.Texture2D, opaque_tex.tex_id);

            Matrix4 model_mat = Matrix4.Identity;
            GL.UniformMatrix4(active_shader["M"], false, ref model_mat);

            for (int i = 0; i <= heightmap.geometry_pool.last_used; i++)
                if (heightmap.geometry_pool.active[i])
                {
                    GL.DrawElementsBaseVertex(PrimitiveType.TriangleStrip,
                        SFMapHeightMapGeometryPool.INDICES_COUNT_PER_CHUNK, 
                        DrawElementsType.UnsignedShort,
                        new IntPtr(i * SFMapHeightMapGeometryPool.INDICES_COUNT_PER_CHUNK * 2),
                        i * (SFMapHeightMapGeometryPool.CHUNK_SIZE + 1) * (SFMapHeightMapGeometryPool.CHUNK_SIZE + 1));
                    //drawcalls_hmap += 1;
                    //triangles += 2 * SFMapHeightMapGeometryPool.CHUNK_SIZE * SFMapHeightMapGeometryPool.CHUNK_SIZE;
                }

            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        // todo: make this be handled automatically by RenderSimple...
        // todo: optimize this a bit by caching textures in lakeManager and using them explicitly
        static void RenderLakes()
        {
            if (!Settings.LakesVisible)
                return;

            Matrix4 VP_mat = scene.camera.ViewProjMatrix;
            GL.UniformMatrix4(active_shader["VP"], false, ref VP_mat);
            Matrix4 M_mat = Matrix4.Identity;
            GL.UniformMatrix4(active_shader["M"], false, ref M_mat);

            for(int i = 0; i < scene.map.lake_manager.lakes.Count; i++)
            {
                if (!scene.map.lake_manager.lake_visible[i])
                    continue;

                SFModel3D lake_mesh = SFResources.SFResourceManager.Models.Get(scene.map.lake_manager.lakes[i].GetObjectName());

                if (lake_mesh == null)
                    continue;

                foreach (SFSubModel3D sbm in lake_mesh.submodels)
                {
                    int mii = sbm.cache_index;
                    if (mii == Utility.NO_INDEX)
                        continue;
                    // sbm.ReloadInstanceMatrices();
                    GL.BindTexture(TextureTarget.Texture2D, sbm.material.texture.tex_id);
                    //DumpAddTexDict(sbm.material.texture, 0);

                    int vri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].VertexRangeIndex;
                    int eri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].ElementRangeIndex;

                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles,
                        SFSubModel3D.Cache.ElementRanges[eri].Count,
                        DrawElementsType.UnsignedInt,
                        new IntPtr(SFSubModel3D.Cache.ElementRanges[eri].Start * 4),
                        SFSubModel3D.Cache.VertexRanges[vri].Start);
                    //DumpAddTexDict(sbm.material.texture, 1);
                    //DumpAddMeshDict(lake_mesh, sbm.submodel_id);

                    //drawcalls_simple += 1;
                    //triangles += SFSubModel3D.Cache.ElementRanges[eri].Count / 3;
                }
            }
        }

        // todo: sort transparent materials
        static void RenderSimpleObjects()
        {
            Matrix4 lsm_mat = scene.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);

            if (current_pass == RenderPass.SCENE)
            {
                SetDepthBias(0);
                SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

                Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
                GL.Uniform1(active_shader["ShadowDepth"], scene.sun_light.ShadowDepth);
            }

            foreach (SFTexture tex in scene.tex_list_simple.Keys)
            {
                GL.BindTexture(TextureTarget.Texture2D, tex.tex_id);
                //DumpAddTexDict(tex, 0);

                LinearPool<TexturedGeometryListElementSimple> elem_list = scene.tex_list_simple[tex];
                for (int i = 0; i < elem_list.elements.Count; i++)
                {
                    if (!elem_list.elem_active[i])
                        continue;
                    TexturedGeometryListElementSimple elem = elem_list.elements[i];

                    GL.UniformMatrix4(active_shader["M"], false, ref elem.node.ResultTransform);

                    SFMaterial mat = elem.node.Mesh.submodels[elem.submodel_index].material;
                    int mii = elem.node.Mesh.submodels[elem.submodel_index].cache_index;
                    if (mii == Utility.NO_INDEX)
                        continue;

                    if (current_pass == RenderPass.SCENE)
                    {
                        //GL.Uniform1(active_shader["apply_shading"], sbm.material.matFlags);
                        SetRenderMode(mat.texRenderMode);
                        if ((mat.matFlags & 4) != 0)
                            SetDepthBias(mat.matDepthBias);
                        else
                            SetDepthBias(0);
                    }
                    else if (current_pass == RenderPass.SHADOWMAP)
                    {
                        if (!mat.casts_shadow)
                            continue;
                    }

                    int vri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].VertexRangeIndex;
                    int eri = SFSubModel3D.Cache.Meshes[SFSubModel3D.Cache.MeshesIndex[mii]].ElementRangeIndex;

                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles,
                        SFSubModel3D.Cache.ElementRanges[eri].Count,
                        DrawElementsType.UnsignedInt,
                        new IntPtr(SFSubModel3D.Cache.ElementRanges[eri].Start * 4),
                        SFSubModel3D.Cache.VertexRanges[vri].Start);
                    //DumpAddTexDict(tex, 1);
                    //DumpAddMeshDict(elem.node.Mesh, elem.submodel_index);

                    //drawcalls_simple += 1;
                    //triangles += SFSubModel3D.Cache.ElementRanges[eri].Count / 3;
                }
            }
        }

        // this is very slow, dunno
        static void RenderAnimatedObjects()
        {
            Matrix4 lsm_mat = scene.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);

            if (current_pass == RenderPass.SCENE)
            {
                Matrix4 p_mat = scene.camera.ProjMatrix;
                GL.UniformMatrix4(active_shader["P"], false, ref p_mat);
                Matrix4 v_mat = scene.camera.ViewMatrix;
                GL.UniformMatrix4(active_shader["V"], false, ref v_mat);
                GL.Uniform1(active_shader["ShadowDepth"], scene.sun_light.ShadowDepth);
            }

            foreach(SceneNodeAnimated an in scene.an_nodes)
            {
                if (an.Skin == null)
                    continue;

                GL.UniformMatrix4(active_shader["M"], false, ref an.ResultTransform);

                for (int i = 0; i < an.Skin.submodels.Length; i++)
                {
                    var msc = an.Skin.submodels[i];
                    GL.BindTexture(TextureTarget.Texture2D, msc.material.texture.tex_id);
                    //DumpAddTexDict(msc.material.texture, 0);

                    GL.UniformMatrix4(active_shader["boneTransforms"], SFSkeleton.MAX_BONE_PER_CHUNK, false, ref an.BoneTransformsPerSkinChunk[i][0].Row0.X);
                    if (current_pass == RenderPass.SCENE)
                        SetRenderMode(msc.material.texRenderMode);

                    int vri = SFModelSkinChunk.Cache.Meshes[SFModelSkinChunk.Cache.MeshesIndex[msc.cache_index]].VertexRangeIndex;
                    int eri = SFModelSkinChunk.Cache.Meshes[SFModelSkinChunk.Cache.MeshesIndex[msc.cache_index]].ElementRangeIndex;

                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles,
                        SFModelSkinChunk.Cache.ElementRanges[eri].Count,
                        DrawElementsType.UnsignedInt,
                        new IntPtr(SFModelSkinChunk.Cache.ElementRanges[eri].Start * 4),
                        SFModelSkinChunk.Cache.VertexRanges[vri].Start);
                    //DumpAddTexDict(msc.material.texture, 2);
                    //DumpAddSkinDict(an.Skin);

                    //drawcalls_anim += 1;
                    //triangles += SFModelSkinChunk.Cache.ElementRanges[eri].Count / 3;
                }
            }
        }


        public static void RenderUI()
        {
            GL.Uniform2(active_shader["ScreenSize"], ref render_size);

            foreach (SFTexture tex in ui.storages.Keys)
            {
                GL.BindTexture(TextureTarget.Texture2D, tex.tex_id);
                //DumpAddTexDict(tex, 0);

                UIQuadStorage storage = ui.storages[tex];
                GL.BindVertexArray(storage.vertex_array);

                for(int i = 0; i < storage.spans.Count; i++)
                {
                    if (!storage.spans[i].visible)
                        continue;
                    if (storage.spans[i].used == 0)
                        continue;

                    GL.Uniform2(active_shader["offset"], storage.spans[i].position);
                    GL.DrawArrays(PrimitiveType.Triangles, storage.spans[i].start * 6, storage.spans[i].used * 6);

                    //drawcalls_ui += 1;
                    //triangles += storage.spans[i].used * 2;
                }
            }
        }

        public static void RenderScene()
        {
            //drawcalls_simple = 0;
            //drawcalls_anim = 0;
            //drawcalls_hmap = 0;
            //drawcalls_ui = 0;

            is_rendering = true;

            //if (prepare_dump)
            //    DumpStart();

            if (scene.map == null)
                scene.sun_light.SetupLightView(new Physics.BoundingBox(new Vector3(-5, 0, -5), new Vector3(5, 30, 5)));

            ApplyLight();

            // render shadowmap
            current_pass = RenderPass.SHADOWMAP;

            SetFramebuffer(shadowmap_depth);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (Settings.EnableShadows)
            {
                SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);
                GL.Enable(EnableCap.DepthTest);
                GL.CullFace(CullFaceMode.Front);

                if (scene.map != null)
                {
                    UseShader(shader_shadowmap_heightmap);
                    RenderHeightmap();
                }


                GL.BindVertexArray(SFModelSkinChunk.Cache.VertexArrayObjectID);
                UseShader(shader_shadowmap_animated);
                RenderAnimatedObjects();

                GL.BindVertexArray(SFSubModel3D.Cache.VertexArrayObjectID);
                UseShader(shader_shadowmap);
                RenderSimpleObjects();

                GL.CullFace(CullFaceMode.Back);
            }

            // render actual view
            //triangles = 0;

            current_pass = RenderPass.SCENE;
            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);
            //draw everything to a texture
            SetFramebuffer(screenspace_framebuffer);
            GL.ClearColor(scene.atmosphere.FogColor.X, scene.atmosphere.FogColor.Y,
                          scene.atmosphere.FogColor.Z, scene.atmosphere.FogColor.W);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Enable(EnableCap.DepthTest);

            // only used here! setting shadow map texture to texture unit 1
            // every other operation on textures happens on texture unit 0
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, shadowmap_depth.texture_depth);
            GL.ActiveTexture(TextureUnit.Texture0);

            // heightmap and overlays
            if (scene.map != null)
            {
                UseShader(shader_heightmap);
                RenderHeightmap();
            }

            GL.BindVertexArray(SFModelSkinChunk.Cache.VertexArrayObjectID);
            UseShader(shader_animated);
            RenderAnimatedObjects();

            GL.BindVertexArray(SFSubModel3D.Cache.VertexArrayObjectID);
            UseShader(shader_simple);
            RenderSimpleObjects();

            SetDepthBias(0);
            SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

            if (scene.map != null)
                RenderLakes();

            // now draw UI
            GL.Disable(EnableCap.DepthTest);
            UseShader(shader_ui);
            RenderUI();

            // what is below doesnt depend on whats above
            // post-processing (currently not much happening here)

            current_pass = RenderPass.SCREENSPACE;
            // move from multisampled to intermediate framebuffer, to be able to use screenspace shader
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, screenspace_framebuffer.fbo);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, screenspace_intermediate.fbo);
            GL.BlitFramebuffer(0, 0, (int)render_size.X, (int)render_size.Y, 0, 0, (int)render_size.X, (int)render_size.Y, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            // final pass: draw a textured quad for post-processing effects - quad will be rendered on screen
            SetFramebuffer(null);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            UseShader(shader_framebuffer_simple);
            GL.BindVertexArray(FrameBuffer.screen_vao);
            GL.Disable(EnableCap.DepthTest);
            GL.BindTexture(TextureTarget.Texture2D, render_shadowmap_depth ? shadowmap_depth.texture_depth : screenspace_intermediate.texture_color);
            GL.Uniform1(active_shader["renderShadowMap"], render_shadowmap_depth ? 1 : 0);
            GL.Uniform1(active_shader["ZNear"], scene.sun_light.ZNear);
            GL.Uniform1(active_shader["ZFar"], scene.sun_light.ZFar);
            //GL.BindTexture(TextureTarget.Texture2D,screenspace_intermediate.texture_color);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            UseShader(null);
            GL.BindVertexArray(0);
            current_pass = RenderPass.NONE;

            is_rendering = false;

            //if (prepare_dump)
            //    DumpEnd();

            //prepare_dump = false;
            //System.Diagnostics.Debug.WriteLine("DRAWCALLS: S " + drawcalls_simple.ToString() + ", A " + drawcalls_anim.ToString() + ", H " + drawcalls_hmap.ToString() + ", U " + drawcalls_ui.ToString() + " - - - TRIANGLES: " +triangles.ToString());
        }
    }
}
