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
using SpellforceDataEditor.SFUnPak;
using SpellforceDataEditor.SFMap;

namespace SpellforceDataEditor.SF3D.SFRender
{
    public static class SFRenderEngine
    {
        public enum RenderPass { NONE = -1, SHADOWMAP = 0, SCENE = 1, SCREENSPACE = 2 }
        public static SFScene scene { get; } = new SFScene();
        static float CurrentDepthBias = 0;
        static RenderMode CurrentRenderMode = RenderMode.SRCALPHA_INVSRCALPHA;

        static SFTexture opaque_tex = null;

        static SFShader shader_simple = new SFShader();
        static SFShader shader_animated = new SFShader();
        static SFShader shader_heightmap = new SFShader();
        static SFShader shader_shadowmap = new SFShader();
        static SFShader shader_shadowmap_animated = new SFShader();
        static SFShader shader_shadowmap_heightmap = new SFShader();
        static SFShader active_shader = null;
        static RenderPass current_pass = RenderPass.NONE;
        

        static SFShader shader_framebuffer_simple = new SFShader();
        static FrameBuffer shadowmap_depth = null;
        static FrameBuffer screenspace_framebuffer = null;
        static FrameBuffer screenspace_intermediate = null;

        static Vector2 render_size = new Vector2(0, 0);

        // for skeletal animation
        static Matrix4[] bone_array = new Matrix4[256];
        static int bone_buffer = -1;

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
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "OPENGL DEBUG CALLBACK: " + messageString);
            }
            else
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "OPENGL DEBUG CALLBACK: " + messageString);
            }
        }

        private static DebugProc _debugProcCallback = DebugCallback;
        private static GCHandle _debugProcCallbackHandle;

        //called only once!
        public static void Initialize(Vector2 view_size)
        {
            // debug setting
            if (!initialized)
            {
                _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);

                GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);
            }

            render_size = view_size;

            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.Initialize() called");
            ResizeView(view_size);

            GL.ClearColor(scene.atmosphere.FogColor.X, scene.atmosphere.FogColor.Y, 
                          scene.atmosphere.FogColor.Z, scene.atmosphere.FogColor.W);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.StencilTest);
            GL.Enable(EnableCap.Blend);
            GL.DepthFunc(DepthFunction.Less);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            shader_simple.CompileShader(Properties.Resources.vshader, Properties.Resources.fshader);
            shader_simple.AddParameter("VP");
            shader_simple.AddParameter("M");
            shader_simple.AddParameter("LSM");
            shader_simple.AddParameter("DiffuseTexture");
            shader_simple.AddParameter("ShadowMap");
            shader_simple.AddParameter("texture_used");
            shader_simple.AddParameter("apply_shading");
            shader_simple.AddParameter("SunDirection");
            shader_simple.AddParameter("SunStrength");
            shader_simple.AddParameter("SunColor");
            shader_simple.AddParameter("AmbientStrength");
            shader_simple.AddParameter("AmbientColor");
            shader_simple.AddParameter("FogColor");
            shader_simple.AddParameter("FogStart");
            shader_simple.AddParameter("FogEnd");
            shader_simple.AddParameter("DepthBias");

            shader_animated.CompileShader(Properties.Resources.vshader_skel, Properties.Resources.fshader_skel);
            shader_animated.AddParameter("VP");
            shader_animated.AddParameter("M");
            shader_animated.AddParameter("LSM");
            shader_animated.AddParameter("DiffuseTexture");
            shader_animated.AddParameter("ShadowMap");
            shader_animated.AddParameter("texture_used");
            shader_animated.AddParameter("apply_shading");
            shader_animated.AddParameter("boneTransforms");
            shader_animated.AddParameter("SunDirection");
            shader_animated.AddParameter("SunStrength");
            shader_animated.AddParameter("SunColor");
            shader_animated.AddParameter("AmbientStrength");
            shader_animated.AddParameter("AmbientColor");
            shader_animated.AddParameter("FogColor");
            shader_animated.AddParameter("FogStart");
            shader_animated.AddParameter("FogEnd");

            shader_heightmap.CompileShader(Properties.Resources.vshader_hmap, Properties.Resources.fshader_hmap);
            shader_heightmap.AddParameter("VP");
            shader_heightmap.AddParameter("M");
            shader_heightmap.AddParameter("VisualizeHeight");
            shader_heightmap.AddParameter("DisplayGrid");
            shader_heightmap.AddParameter("GridColor");
            shader_heightmap.AddParameter("LSM");
            shader_heightmap.AddParameter("ShadowMap");
            shader_heightmap.AddParameter("SunDirection");
            shader_heightmap.AddParameter("SunStrength");
            shader_heightmap.AddParameter("SunColor");
            shader_heightmap.AddParameter("AmbientStrength");
            shader_heightmap.AddParameter("AmbientColor");
            shader_heightmap.AddParameter("FogColor");
            shader_heightmap.AddParameter("FogStart");
            shader_heightmap.AddParameter("FogEnd");

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

            scene.sun_light.Direction = -(new Vector3(0, -1, 0).Normalized());
            scene.sun_light.Color = new Vector4(1, 1, 1, 1);
            scene.sun_light.Strength = 1.5f;
            scene.ambient_light.Color = new Vector4(1f, 1f, 1f, 1.0f);
            scene.ambient_light.Strength = 0.8f;
            scene.atmosphere.FogColor = new Vector4(0.55f, 0.55f, 0.85f, 1.0f);
            scene.atmosphere.FogStart = 100f;
            scene.atmosphere.FogEnd = 200f;

            if (screenspace_framebuffer != null)
            {
                shadowmap_depth.Dispose();
                screenspace_framebuffer.Dispose();
                screenspace_intermediate.Dispose();
            }
            shadowmap_depth = new FrameBuffer(Settings.ShadowMapSize, Settings.ShadowMapSize, 0, FrameBuffer.TextureType.DEPTH, FrameBuffer.RenderBufferType.NONE);
            screenspace_framebuffer = new FrameBuffer((int)view_size.X, (int)view_size.Y, Settings.AntiAliasingSamples);
            screenspace_intermediate = new FrameBuffer((int)view_size.X, (int)view_size.Y, 0);

            ApplyLight();
            ApplyTexturingUnits();

            if(opaque_tex == null)
            {
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
            }

            if(bone_buffer == -1)
            {
                int uniformBonesAnimated = GL.GetUniformBlockIndex((uint)(shader_animated.ProgramID), "Bones");
                int uniformBonesAnimatedShadowmap = GL.GetUniformBlockIndex((uint)(shader_shadowmap_animated.ProgramID), "Bones");

                GL.UniformBlockBinding(shader_animated.ProgramID, uniformBonesAnimated, 0);
                GL.UniformBlockBinding(shader_shadowmap_animated.ProgramID, uniformBonesAnimatedShadowmap, 0);

                bone_buffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.UniformBuffer, bone_buffer);
                GL.BufferData(BufferTarget.UniformBuffer, 256*16*4, new IntPtr(0), BufferUsageHint.StaticDraw); // allocate 150 bytes of memory
                GL.BindBuffer(BufferTarget.UniformBuffer, 0);

                GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, bone_buffer, new IntPtr(0), 256 * 16 * 4);
            }

            SFModelSkinMap.Init();
            
            initialized = true;
        }

        public static void ResizeView(Vector2 view_size)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.ResizeView() called (view_size = " + view_size.ToString() + ")");
            render_size = view_size;
            scene.camera.ProjMatrix = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4, view_size.X / view_size.Y, 0.1f, 200f);
            scene.camera.AspectRatio = (float)(view_size.X) / view_size.Y;
            GL.Viewport(0, 0, (int)view_size.X, (int)view_size.Y);
            if (screenspace_framebuffer != null)
            {
                screenspace_framebuffer.Resize((int)view_size.X, (int)view_size.Y);
                screenspace_intermediate.Resize((int)view_size.X, (int)view_size.Y);
            }
        }

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

            GL.UseProgram(shader_shadowmap.ProgramID);
            GL.Uniform1(shader_shadowmap["DiffuseTexture"], 0);

            GL.UseProgram(shader_shadowmap_animated.ProgramID);
            GL.Uniform1(shader_shadowmap_animated["DiffuseTexture"], 0);

            GL.UseProgram(shader_shadowmap_heightmap.ProgramID);
            GL.Uniform1(shader_shadowmap_heightmap["DiffuseTexture"], 0);
            
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
                    GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
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
        }

        // this could be optimized to not check all chunks every time...
        public static void UpdateVisibleChunks()
        {
            scene.camera.Update(0);
            // 1. find collection of visible chunks
            List<SceneNodeMapChunk> vis_chunks = new List<SceneNodeMapChunk>();

            // test visibility of each chunk

            ParallelOptions loop_options = new ParallelOptions();
            loop_options.MaxDegreeOfParallelism = 4;
            int chunks_per_task = scene.heightmap.chunk_nodes.Length/4;
            List<SceneNodeMapChunk>[] vis_chunks_per_task = new List<SceneNodeMapChunk>[4];
            Parallel.For(0, 4, (i) =>
            {
                vis_chunks_per_task[i] = new List<SceneNodeMapChunk>();
                int end = chunks_per_task * (i + 1);
                for (int j = chunks_per_task * i; j < end; j++)
                {
                    SceneNodeMapChunk chunk_node = scene.heightmap.chunk_nodes[j];
                    chunk_node.DistanceToCamera = Vector2.Distance(
                        new Vector2(chunk_node.MapChunk.collision_cache.aabb.center.X, 
                                    chunk_node.MapChunk.collision_cache.aabb.center.Z),
                        new Vector2(scene.camera.Position.X, scene.camera.Position.Z));
                    chunk_node.CameraHeightDifference = scene.camera.Position.Y - chunk_node.MapChunk.collision_cache.aabb.b.Y;
                    if (chunk_node.DistanceToCamera > 224)  // magic number: zFar+aspect_ratio*sqrt2*chunk size
                        continue;
                    if (!chunk_node.MapChunk.collision_cache.aabb.IsOutsideOfConvexHull(scene.camera.FrustumPlanes))
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
                if (cur_list_id == scene.heightmap.visible_chunks.Count)
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
                    for (int i = cur_list_id; i < scene.heightmap.visible_chunks.Count; i++)
                    {
                        scene.heightmap.visible_chunks[i].SetParent(null);
                        scene.heightmap.visible_chunks[i].MapChunk.UpdateVisible(false);
                        // add invisible to the current chunk
                    }
                    break;
                }

                cur_chunk_id = scene.heightmap.visible_chunks[cur_list_id].MapChunk.id;
                next_chunk_id = vis_chunks[next_list_id].MapChunk.id;
                // if next id > cur id, keep increasing cur id, while simultaneously turning chunks invisible
                // otherwise keep increasing next_id, while simultaneuosly turning chunks visible
                if (next_chunk_id > cur_chunk_id)
                {
                    while(next_chunk_id > cur_chunk_id)
                    {
                        scene.heightmap.visible_chunks[cur_list_id].SetParent(null);
                        scene.heightmap.visible_chunks[cur_list_id].MapChunk.UpdateVisible(false);
                        // turn chunk invisible

                        cur_list_id += 1;
                        if (cur_list_id == scene.heightmap.visible_chunks.Count)
                            break;
                        cur_chunk_id = scene.heightmap.visible_chunks[cur_list_id].MapChunk.id;
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
            scene.heightmap.visible_chunks = vis_chunks;

            // set light matrix and check decoration visibility
            /*
            float xmin, xmax, ymin, ymax, zmax;
            xmin = 9999; ymin = 9999; xmax = -9999; ymax = -9999; zmax = -9999;
            foreach(SceneNodeMapChunk chunk_node in vis_chunks)
            {
                Vector3 pos = chunk_node.Position;
                if (pos.X-16 < xmin)
                    xmin = pos.X-16;
                else if (pos.X+32 > xmax)
                    xmax = pos.X+32;
                if (pos.Z-16 < ymin)
                    ymin = pos.Z-16;
                else if (pos.Z+32 > ymax)
                    ymax = pos.Z+32;
                if (chunk_node.MapChunk.collision_cache.aabb.b.Y > zmax)
                    zmax = chunk_node.MapChunk.collision_cache.aabb.b.Y;

                chunk_node.MapChunk.UpdateDecorationVisible(chunk_node.DistanceToCamera);
            }
            SF3D.Physics.BoundingBox aabb = new Physics.BoundingBox(new Vector3(xmin, 0, ymin), new Vector3(xmax, zmax+15, ymax));
            scene.sun_light.SetupLightView(aabb);*/

            // set light matrix and check decoration visibility, this also sets level of detail
            
            foreach(SceneNodeMapChunk chunk_node in vis_chunks)
                chunk_node.MapChunk.UpdateDecorationVisible(chunk_node.DistanceToCamera, chunk_node.CameraHeightDifference);
            scene.sun_light.SetupLightView(scene.camera.Position);
        }

        // once per frame
        private static void ReloadInstanceMatrices()
        {
            foreach (SFSubModel3D sbm in scene.untex_entries_simple)
            {
                GL.BindVertexArray(sbm.vertex_array);
                    sbm.ReloadInstanceMatrices();
            }

            foreach (SFTexture tex in scene.tex_entries_simple.Keys)
            {
                HashSet<SFSubModel3D> submodels = scene.tex_entries_simple[tex];

                foreach (SFSubModel3D sbm in submodels)
                {
                    GL.BindVertexArray(sbm.vertex_array);
                    sbm.ReloadInstanceMatrices();
                }
            }
        }

        private static void RenderHeightmap()
        {
            Matrix4 lsm_mat = scene.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
            if (current_pass == RenderPass.SCENE)
            {
                GL.BindTexture(TextureTarget.Texture2DArray, scene.heightmap.texture_manager.terrain_texture);
                Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
                GL.Uniform1(active_shader["VisualizeHeight"], Settings.VisualizeHeight ? 1 : 0);
                GL.Uniform1(active_shader["DisplayGrid"], Settings.DisplayGrid ? 1 : 0);
                GL.Uniform4(active_shader["GridColor"], Settings.GridColor); 
            }
            else if (current_pass == RenderPass.SHADOWMAP)
                GL.BindTexture(TextureTarget.Texture2D, opaque_tex.tex_id);
            foreach(SceneNodeMapChunk chunk_node in scene.heightmap.visible_chunks)
            {
                SFMapHeightMapChunk chunk = chunk_node.MapChunk;

                // get chunk position

                Matrix4 model_mat = chunk_node.ResultTransform;
                GL.UniformMatrix4(active_shader["M"], false, ref model_mat);

                GL.BindVertexArray(chunk.vertex_array);
                GL.DrawArrays(PrimitiveType.Triangles, 0, chunk.vertices.Length);
            }
            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        // todo: make this be handled automatically by RenderSimple...
        // todo: optimize this a bit by caching textures in lakeManager and using them explicitly
        private static void RenderLakes()
        {
            if (!Settings.LakesVisible)
                return;

            GL.Uniform1(active_shader["apply_shading"], 0);
            GL.Uniform1(active_shader["texture_used"], 1);
            Matrix4 VP_mat = scene.camera.ViewProjMatrix;
            GL.UniformMatrix4(active_shader["VP"], false, ref VP_mat);

            foreach (SceneNodeMapChunk chunk_node in scene.heightmap.visible_chunks)
            {
                SFMapHeightMapChunk chunk = chunk_node.MapChunk;

                if (chunk.lake_model == null)
                    continue;

                // get chunk position
                Matrix4 chunk_matrix = chunk_node.ResultTransform;
                //GL.UniformMatrix4(active_shader["M"], false, ref chunk_matrix);

                foreach (SFSubModel3D sbm in chunk.lake_model.submodels)
                {
                    GL.BindVertexArray(sbm.vertex_array);
                    sbm.ReloadInstanceMatrices();

                    GL.BindTexture(TextureTarget.Texture2D, sbm.material.texture.tex_id);
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, (int)sbm.material.indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero, sbm.instance_matrices.Count);
                }
            }
        }

        // todo: move overlays to a shader now that grid is supported
        public static void RenderOverlays()
        {
            if (!Settings.OverlaysVisible)
                return;

            GL.Uniform1(active_shader["texture_used"], 0);
            GL.Uniform1(active_shader["apply_shading"], 0);
            Matrix4 VP_mat = scene.camera.ViewProjMatrix;
            GL.UniformMatrix4(active_shader["VP"], false, ref VP_mat);

            foreach (SceneNodeMapChunk chunk_node in scene.heightmap.visible_chunks)
            {
                Matrix4 chunk_matrix = chunk_node.ResultTransform;
                //GL.UniformMatrix4(active_shader["M"], false, ref chunk_matrix);

                foreach (string o in scene.heightmap.visible_overlays)
                {
                    SFMap.MapEdit.MapOverlayChunk overlay = chunk_node.MapChunk.overlays[o];
                    if (overlay.mesh == null)
                        continue;
                    // get chunk position

                    SFSubModel3D sbm = overlay.mesh.submodels[0];
                    GL.BindVertexArray(sbm.vertex_array);
                    // sbm.ReloadInstanceMatrices();

                    GL.DrawElementsInstanced(PrimitiveType.Triangles, (int)sbm.material.indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero, sbm.instance_matrices.Count);
                }
            }
        }

        // todo: sort transparent materials
        public static void RenderSimpleObjects()
        {
            Matrix4 lsm_mat = scene.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
            // untextured first

            if (current_pass == RenderPass.SCENE)
            {
                GL.Uniform1(active_shader["texture_used"], 0);
                GL.Uniform1(active_shader["apply_shading"], 0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                SetDepthBias(0);
                //SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);

                Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, opaque_tex.tex_id);
            }

            int untex_inst_count = 0;
            foreach (SFSubModel3D sbm in scene.untex_entries_simple)
                untex_inst_count += sbm.instance_matrices.Count;
            if (untex_inst_count != 0)
                foreach (SFSubModel3D sbm in scene.untex_entries_simple)
                {
                    GL.BindVertexArray(sbm.vertex_array);
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, (int)sbm.face_indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero, sbm.instance_matrices.Count);
                }

            // textured next
            if (current_pass == RenderPass.SCENE)
            {
                GL.Uniform1(active_shader["texture_used"], 1);
                GL.Uniform1(active_shader["apply_shading"], 1);
            }
            foreach(SFTexture tex in scene.tex_entries_simple.Keys)
            {
                HashSet<SFSubModel3D> submodels = scene.tex_entries_simple[tex];

                // count all instances using this texture
                int inst_count = 0;
                foreach (SFSubModel3D sbm in submodels)
                    inst_count += sbm.instance_matrices.Count;
                if (inst_count == 0)
                    continue;

                GL.BindTexture(TextureTarget.Texture2D, tex.tex_id);
                foreach (SFSubModel3D sbm in submodels)
                {
                    GL.BindVertexArray(sbm.vertex_array);
                    if (current_pass == RenderPass.SCENE)
                    {
                        //SetRenderMode(sbm.material.texRenderMode);
                        if ((sbm.material.matFlags & 4) != 0)
                            SetDepthBias(sbm.material.matDepthBias);
                        else
                            SetDepthBias(0);
                    }
                    //GL.Uniform1(active_shader["apply_shading"], (mat.apply_shading ? 1 : 0));

                    GL.DrawElementsInstanced(PrimitiveType.Triangles, (int)sbm.face_indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero, sbm.instance_matrices.Count);
                }
            }
        }

        /*// this is very slow, dunno
        public static void RenderAnimatedObjects()
        {
            Matrix4 lsm_mat = scene.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
            //UseShader(shader_animated);
            if (current_pass == RenderPass.SCENE)
            {
                GL.Uniform1(active_shader["apply_shading"], 1);
                Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
            }

            Matrix4[] bones = new Matrix4[20];
            foreach (SFTexture tex in scene.tex_list_animated.Keys)
            {
                //if(current_pass == RenderPass.SCENE)
                    GL.BindTexture(TextureTarget.Texture2D, tex.tex_id);

                LinearPool<TexturedGeometryListElementAnimated> elem_list = scene.tex_list_animated[tex];

                for (int i = 0; i < elem_list.elements.Count; i++)
                {
                    if (!elem_list.elem_active[i])
                        continue;
                    TexturedGeometryListElementAnimated elem = elem_list.elements[i];
                    
                    Matrix4 model_mat = elem.node.ResultTransform;
                    GL.UniformMatrix4(active_shader["M"], false, ref model_mat);

                    SFModelSkinChunk chunk = elem.node.Skin.submodels[elem.submodel_index];
                    
                    for (int j = 0; j < chunk.bones.Length; j++)
                        bones[j] = elem.node.BoneTransforms[chunk.bones[j]];

                    GL.BindVertexArray(chunk.vertex_array);
                    GL.UniformMatrix4(active_shader["boneTransforms"], 20, false, ref bones[0].Row0.X);
                    //if(current_pass == RenderPass.SCENE)
                    //    SetRenderMode(chunk.material.texRenderMode);

                    GL.DrawElements(PrimitiveType.Triangles, chunk.face_indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }
        }*/

        // this is very slow, dunno
        public static void RenderAnimatedObjects()
        {
            SFModelSkinMap.Update();

            Matrix4 lsm_mat = scene.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
            if (current_pass == RenderPass.SCENE)
            {
                GL.Uniform1(active_shader["apply_shading"], 1);
                Matrix4 vp_mat = scene.camera.ViewProjMatrix;
                GL.UniformMatrix4(active_shader["VP"], false, ref vp_mat);
            }


            if (SFModelSkinMap.vertex_array == -1)
                return;

            GL.BindVertexArray(SFModelSkinMap.vertex_array);
            foreach(SceneNodeAnimated node in scene.animated_objects)
            {
                SFModelSkin skin = node.Skin;
                if (skin == null)
                    continue;

                Matrix4 model_mat = node.ResultTransform;
                GL.UniformMatrix4(active_shader["M"], false, ref model_mat);

                Array.Copy(node.BoneTransforms, bone_array, node.Skeleton.bone_count);

                GL.BindBuffer(BufferTarget.UniformBuffer, bone_buffer);
                GL.BufferSubData<Matrix4>(BufferTarget.UniformBuffer, new IntPtr(0), 16 * 4 * node.Skeleton.bone_count, bone_array);

                for(int i = 0; i < skin.submodels.Length; i++)
                {
                    SFModelSkinChunk chunk = skin.submodels[i];
                    GL.BindTexture(TextureTarget.Texture2D, chunk.material.texture.tex_id);

                    //if(current_pass == RenderPass.SCENE)
                    //    SetRenderMode(chunk.material.texRenderMode);

                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)chunk.material.indexCount,
                        DrawElementsType.UnsignedInt, new IntPtr(chunk.material.indexStart*4), chunk.base_vertex);
                }
            }

            /*Matrix4[] bones = new Matrix4[20];
            foreach (SFTexture tex in scene.tex_list_animated.Keys)
            {
                //if(current_pass == RenderPass.SCENE)
                GL.BindTexture(TextureTarget.Texture2D, tex.tex_id);

                LinearPool<TexturedGeometryListElementAnimated> elem_list = scene.tex_list_animated[tex];

                for (int i = 0; i < elem_list.elements.Count; i++)
                {
                    if (!elem_list.elem_active[i])
                        continue;
                    TexturedGeometryListElementAnimated elem = elem_list.elements[i];

                    Matrix4 model_mat = elem.node.ResultTransform;
                    GL.UniformMatrix4(active_shader["M"], false, ref model_mat);

                    SFModelSkinChunk chunk = elem.node.Skin.submodels[elem.submodel_index];

                    for (int j = 0; j < chunk.bones.Length; j++)
                        bones[j] = elem.node.BoneTransforms[chunk.bones[j]];

                    GL.BindVertexArray(chunk.vertex_array);
                    GL.UniformMatrix4(active_shader["boneTransforms"], 20, false, ref bones[0].Row0.X);
                    //if(current_pass == RenderPass.SCENE)
                    //    SetRenderMode(chunk.material.texRenderMode);

                    GL.DrawElements(PrimitiveType.Triangles, chunk.face_indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }*/
        }

        public static void RenderScene()
        {
            if (scene.heightmap == null)
                scene.sun_light.SetupLightView(new Physics.BoundingBox(new Vector3(-5, 0, -5), new Vector3(5, 30, 5)));

            /*scene.sun_light.Direction = -new Vector3(
                (float)-Math.Sin(scene.frame_counter/ 10f),
                -1.0f,
                (float)Math.Cos(scene.frame_counter / 10f)).Normalized();
            ApplyLight();*/

            ReloadInstanceMatrices();

            // render shadowmap
            current_pass = RenderPass.SHADOWMAP;

            SetFramebuffer(shadowmap_depth);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (Settings.EnableShadows)
            {
                GL.Enable(EnableCap.DepthTest);

                if (scene.heightmap != null)
                {
                    UseShader(shader_shadowmap_heightmap);
                    RenderHeightmap();
                }

                UseShader(shader_shadowmap_animated);
                RenderAnimatedObjects();

                UseShader(shader_shadowmap);
                RenderSimpleObjects();
            }

            // render actual view

            current_pass = RenderPass.SCENE;
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
            if (scene.heightmap != null)
            {
                //SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);
                UseShader(shader_heightmap);
                RenderHeightmap();
                UseShader(shader_simple);
                SetDepthBias(0);
                RenderOverlays();
            }

            UseShader(shader_animated);
            RenderAnimatedObjects();

            UseShader(shader_simple);
            RenderSimpleObjects();

            if (scene.heightmap != null)
            {
                SetDepthBias(0);
                //SetRenderMode(RenderMode.SRCALPHA_INVSRCALPHA);
                RenderLakes();
            }


            // what is below doesnt depend on whats above
            // post-processing (currently not much happening here)

            current_pass = RenderPass.SCREENSPACE;
            // move from multisampled to intermediate framebuffer, to be able to use screenspace shader
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, screenspace_framebuffer.fbo);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, screenspace_intermediate.fbo);
            GL.BlitFramebuffer(0, 0, (int)render_size.X, (int)render_size.Y, 0, 0, (int)render_size.X, (int)render_size.Y, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            // final pass: draw a textured quad for post-processing effects - quad will be rendered on screen
            SetFramebuffer(null);
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            UseShader(shader_framebuffer_simple);
            GL.BindVertexArray(FrameBuffer.screen_vao);
            GL.Disable(EnableCap.DepthTest);
            GL.BindTexture(TextureTarget.Texture2D,screenspace_intermediate.texture_color);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            UseShader(null);
            current_pass = RenderPass.NONE;
        }
    }
}
