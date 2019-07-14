/*
 * SFRenderEngine takes care of displaying 3D graphics in a window
 * It takes data from SFSceneManager and renders it using predefined shaders
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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

        static bool initialized = false;

        //called only once!
        public static void Initialize(Vector2 view_size)
        {
            if (initialized)
                return;

            render_size = view_size;

            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.Initialize() called");
            ResizeView(view_size);

            GL.ClearColor(Color.MidnightBlue);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.StencilTest);
            GL.Enable(EnableCap.Blend);
            GL.DepthFunc(DepthFunction.Less);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            shader_simple.CompileShader(Properties.Resources.vshader, Properties.Resources.fshader);
            shader_simple.AddParameter("MVP");
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

            shader_animated.CompileShader(Properties.Resources.vshader_skel, Properties.Resources.fshader_skel);
            shader_animated.AddParameter("MVP");
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

            shader_heightmap.CompileShader(Properties.Resources.vshader_hmap, Properties.Resources.fshader_hmap);
            shader_heightmap.AddParameter("MVP");
            shader_heightmap.AddParameter("M");
            shader_heightmap.AddParameter("LSM");
            shader_heightmap.AddParameter("ShadowMap");
            shader_heightmap.AddParameter("SunDirection");
            shader_heightmap.AddParameter("SunStrength");
            shader_heightmap.AddParameter("SunColor");
            shader_heightmap.AddParameter("AmbientStrength");
            shader_heightmap.AddParameter("AmbientColor");

            if (Settings.EnableShadows)
            {
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
            }

            shader_framebuffer_simple.CompileShader(Properties.Resources.vshader_framebuffer, Properties.Resources.fshader_framebuffer_simple);

            scene.sun_light.Direction = -(new Vector3(0, -1, 0).Normalized());
            scene.sun_light.Color = new Vector4(1, 1, 1, 1);
            scene.sun_light.Strength = 1.3f;
            scene.ambient_light.Strength = 1.0f;
            scene.ambient_light.Color = new Vector4(0.6f, 0.6f, 1.0f, 1.0f);

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

            // this doesnt work for now...
            //initialized = true;
        }

        public static void ResizeView(Vector2 view_size)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.ResizeView() called (view_size = " + view_size.ToString() + ")");
            render_size = view_size;
            scene.camera.ProjMatrix = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4, view_size.X / view_size.Y, 0.1f, 100f);
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

            GL.UseProgram(shader_animated.ProgramID);
            GL.Uniform1(shader_animated["SunStrength"], scene.sun_light.Strength);
            GL.Uniform3(shader_animated["SunDirection"], scene.sun_light.Direction);
            GL.Uniform4(shader_animated["SunColor"], scene.sun_light.Color);
            GL.Uniform1(shader_animated["AmbientStrength"], scene.ambient_light.Strength);
            GL.Uniform4(shader_animated["AmbientColor"], scene.ambient_light.Color);

            GL.UseProgram(shader_heightmap.ProgramID);
            GL.Uniform1(shader_heightmap["SunStrength"], scene.sun_light.Strength);
            GL.Uniform3(shader_heightmap["SunDirection"], scene.sun_light.Direction);
            GL.Uniform4(shader_heightmap["SunColor"], scene.sun_light.Color);
            GL.Uniform1(shader_heightmap["AmbientStrength"], scene.ambient_light.Strength);
            GL.Uniform4(shader_heightmap["AmbientColor"], scene.ambient_light.Color);

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

        private static void UpdateVisibleChunks()
        {
            scene.camera.Update(0);
            // 1. find collection of visible chunks
            List<SceneNodeMapChunk> vis_chunks = new List<SceneNodeMapChunk>();

            // test visibility of each chunk
            
            foreach(SceneNodeMapChunk chunk_node in scene.heightmap.chunk_nodes)
                if(!chunk_node.MapChunk.aabb.IsOutsideOfConvexHull(scene.camera.FrustrumPlanes))
                    vis_chunks.Add(chunk_node);
            
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

            // set light matrix
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
                if (chunk_node.MapChunk.aabb.b.Y > zmax)
                    zmax = chunk_node.MapChunk.aabb.b.Y;
            }
            SF3D.Physics.BoundingBox aabb = new Physics.BoundingBox(new Vector3(xmin, 0, ymin), new Vector3(xmax, zmax+15, ymax));
            scene.sun_light.SetupLightView(aabb);
        }

        private static void RenderHeightmap()
        {
            Matrix4 lsm_mat = scene.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
            if (current_pass == RenderPass.SCENE)
                GL.BindTexture(TextureTarget.Texture2DArray, scene.heightmap.texture_manager.terrain_texture);
            else if (current_pass == RenderPass.SHADOWMAP)
                GL.BindTexture(TextureTarget.Texture2D, opaque_tex.tex_id);
            foreach(SceneNodeMapChunk chunk_node in scene.heightmap.visible_chunks)
            {
                SFMapHeightMapChunk chunk = chunk_node.MapChunk;

                // get chunk position

                Matrix4 model_mat = chunk_node.ResultTransform;
                GL.UniformMatrix4(active_shader["M"], false, ref model_mat);

                Matrix4 mvp_mat = model_mat;
                if (current_pass == RenderPass.SCENE)
                {
                    mvp_mat *= scene.camera.ViewProjMatrix;
                    GL.UniformMatrix4(active_shader["MVP"], false, ref mvp_mat);
                }

                GL.BindVertexArray(chunk.vertex_array);
                GL.DrawArrays(PrimitiveType.Triangles, 0, chunk.vertices.Length);
            }
            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        // todo: make this be handled automatically by RenderSimple...
        // todo: optimize this a bit by caching textures in lakeManager and using them explicitly
        private static void RenderLakes()
        {
            GL.Uniform1(active_shader["apply_shading"], 0);
            GL.Uniform1(active_shader["texture_used"], 1);
            foreach (SceneNodeMapChunk chunk_node in scene.heightmap.visible_chunks)
            {
                SFMapHeightMapChunk chunk = chunk_node.MapChunk;

                if (chunk.lake_model == null)
                    continue;

                GL.BindVertexArray(chunk.lake_model.vertex_array);

                // get chunk position
                Matrix4 chunk_matrix = chunk_node.ResultTransform;
                Matrix4 MVP_mat = chunk_matrix * scene.camera.ViewProjMatrix;

                GL.UniformMatrix4(active_shader["MVP"], false, ref MVP_mat);
                GL.UniformMatrix4(active_shader["M"], false, ref chunk_matrix);

                foreach (SFMaterial mat in chunk.lake_model.materials)
                {
                    GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);
                    GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt, (int)mat.indexStart * 4);
                }
            }
        }

        // todo: move overlays to nodes
        public static void RenderOverlays()
        {
            GL.Uniform1(active_shader["texture_used"], 0);
            GL.Uniform1(active_shader["apply_shading"], 0);
            foreach (SceneNodeMapChunk chunk_node in scene.heightmap.visible_chunks)
            {
                Matrix4 chunk_matrix = chunk_node.ResultTransform;
                Matrix4 MVP_mat = chunk_matrix * scene.camera.ViewProjMatrix;

                GL.UniformMatrix4(active_shader["MVP"], false, ref MVP_mat);
                GL.UniformMatrix4(active_shader["M"], false, ref chunk_matrix);
                foreach (string o in scene.heightmap.visible_overlays)
                {
                    SFMap.MapEdit.MapOverlayChunk overlay = chunk_node.MapChunk.overlays[o];
                    if (overlay.mesh == null)
                        continue;
                    // get chunk position

                    SFMaterial mat = overlay.mesh.materials[0];

                    GL.BindVertexArray(overlay.mesh.vertex_array);
                    GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt, (int)mat.indexStart*4);
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
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, opaque_tex.tex_id);
            }

            for (int i = 0; i < scene.untextured_list_simple.elements.Count; i++)
            {
                if (!scene.untextured_list_simple.elem_active[i])
                    continue;
                TexturedGeometryListElementSimple elem = scene.untextured_list_simple.elements[i];

                Matrix4 model_mat = elem.node.ResultTransform;
                GL.UniformMatrix4(active_shader["M"], false, ref model_mat);

                Matrix4 mvp_mat = model_mat;
                if (current_pass == RenderPass.SCENE)
                {
                    mvp_mat *= scene.camera.ViewProjMatrix;
                    GL.UniformMatrix4(active_shader["MVP"], false, ref mvp_mat);
                }

                SFMaterial mat = elem.node.Mesh.materials[elem.submodel_index];

                GL.BindVertexArray(elem.node.Mesh.vertex_array);
                //GL.Uniform1(active_shader["apply_shading"], (mat.apply_shading ? 1 : 0));

                GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt, (int)mat.indexStart * 4);
            }

            // textured next
            if (current_pass == RenderPass.SCENE)
            {
                GL.Uniform1(active_shader["texture_used"], 1);
                GL.Uniform1(active_shader["apply_shading"], 1);
            }
            foreach (SFTexture tex in scene.tex_list_simple.Keys)
            {
                //if(current_pass == RenderPass.SCENE)
                    GL.BindTexture(TextureTarget.Texture2D, tex.tex_id);
                LinearPool<TexturedGeometryListElementSimple> elem_list = scene.tex_list_simple[tex];
                for(int i = 0; i < elem_list.elements.Count; i++)
                { 
                    if (!elem_list.elem_active[i])
                        continue;
                    TexturedGeometryListElementSimple elem = elem_list.elements[i];

                    Matrix4 model_mat = elem.node.ResultTransform;
                    GL.UniformMatrix4(active_shader["M"], false, ref model_mat);

                    Matrix4 mvp_mat = model_mat;
                    if (current_pass == RenderPass.SCENE)
                    {
                        mvp_mat *= scene.camera.ViewProjMatrix;
                        GL.UniformMatrix4(active_shader["MVP"], false, ref mvp_mat);
                    }

                    SFMaterial mat = elem.node.Mesh.materials[elem.submodel_index];
                    
                    GL.BindVertexArray(elem.node.Mesh.vertex_array);
                    //GL.Uniform1(active_shader["apply_shading"], (mat.apply_shading ? 1 : 0));

                    GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt, (int)mat.indexStart * 4);
                }
            }
        }

        public static void RenderAnimatedObjects()
        {
            Matrix4 lsm_mat = scene.sun_light.LightMatrix;
            GL.UniformMatrix4(active_shader["LSM"], false, ref lsm_mat);
            //UseShader(shader_animated);
            if (current_pass == RenderPass.SCENE)
                GL.Uniform1(active_shader["apply_shading"], 1);

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

                    Matrix4 mvp_mat = model_mat;
                    if (current_pass == RenderPass.SCENE)
                    {
                        mvp_mat *= scene.camera.ViewProjMatrix;
                        GL.UniformMatrix4(active_shader["MVP"], false, ref mvp_mat);
                    }

                    SFModelSkinChunk chunk = elem.node.Skin.submodels[elem.submodel_index];

                    Matrix4[] bones = new Matrix4[20];
                    for (int j = 0; j < chunk.bones.Length; j++)
                        bones[j] = elem.node.BoneTransforms[chunk.bones[j]];

                    GL.BindVertexArray(chunk.vertex_array);
                    GL.UniformMatrix4(active_shader["boneTransforms"], 20, false, ref bones[0].Row0.X);

                    GL.DrawElements(PrimitiveType.Triangles, chunk.face_indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }
        }

        public static void RenderScene()
        {
            if (scene.heightmap != null)
                UpdateVisibleChunks();
            else
                scene.sun_light.SetupLightView(new Physics.BoundingBox(new Vector3(-5, 0, -5), new Vector3(5, 30, 5)));

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
            current_pass = RenderPass.SCENE;
            //draw everything to a texture
            SetFramebuffer(screenspace_framebuffer);
            GL.ClearColor(Color.MidnightBlue);
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
                UseShader(shader_heightmap);
                RenderHeightmap();
                UseShader(shader_simple);
                RenderOverlays();
            }

            UseShader(shader_animated);
            RenderAnimatedObjects();

            UseShader(shader_simple);
            RenderSimpleObjects();

            if (scene.heightmap != null)
                RenderLakes();


            // what is below doesnt depend on whats above
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
            GL.BindTexture(TextureTarget.Texture2D, screenspace_intermediate.texture_color);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            UseShader(null);
            current_pass = RenderPass.NONE;
        }
    }
}
