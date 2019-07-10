/*
 * SFRenderEngine takes care of displaying 3D graphics in a window
 * It takes data from SFSceneManager and renders it using predefined shaders
 */

using System;
using System.Collections.Generic;
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
        public static SFScene scene { get; } = new SFScene();

        static SFShader shader_simple = new SFShader();
        static SFShader shader_animated = new SFShader();
        static SFShader shader_heightmap = new SFShader();
        static SFShader shader_overlay = new SFShader();  // ?
        static SFShader active_shader = null;

        static SFShader shader_framebuffer_simple = new SFShader();
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
            shader_heightmap.AddParameter("SunDirection");
            shader_heightmap.AddParameter("SunStrength");
            shader_heightmap.AddParameter("SunColor");
            shader_heightmap.AddParameter("AmbientStrength");
            shader_heightmap.AddParameter("AmbientColor");

            shader_overlay.CompileShader(Properties.Resources.vshader_overlay, Properties.Resources.fshader_overlay);
            shader_overlay.AddParameter("MVP");
            shader_overlay.AddParameter("Color");

            shader_framebuffer_simple.CompileShader(Properties.Resources.vshader_framebuffer, Properties.Resources.fshader_framebuffer_simple);

            scene.sun_light.Direction = -(new Vector3(0, -1, 0).Normalized());
            scene.sun_light.Color = new Vector4(1, 1, 1, 1);
            scene.sun_light.Strength = 1.3f;
            scene.ambient_light.Strength = 1.0f;
            scene.ambient_light.Color = new Vector4(0.5f, 0.5f, 1.0f, 1.0f);

            if (screenspace_framebuffer != null)
            {
                screenspace_framebuffer.Dispose();
                screenspace_intermediate.Dispose();
            }
            screenspace_framebuffer = new FrameBuffer((int)view_size.X, (int)view_size.Y, 4);
            screenspace_intermediate = new FrameBuffer((int)view_size.X, (int)view_size.Y, 0);

            ApplyLight();

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
                        // add visible to the next chunk
                    }
                    break;
                }
                if(next_end)
                {
                    for (int i = cur_list_id; i < scene.heightmap.visible_chunks.Count; i++)
                    {
                        scene.heightmap.visible_chunks[i].SetParent(null);
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
        }

        private static void UpdateVisibleLakes()
        {
            // reset visibility
            SFMapLakeManager lake_manager = scene.heightmap.map.lake_manager;
            for (int i = 0; i < lake_manager.lake_visible.Count; i++)
                lake_manager.lake_visible[i] = false;

            // update visibility
            foreach(SceneNodeMapChunk chunk_node in scene.heightmap.visible_chunks)
                for (int i = 0; i < lake_manager.lake_visible.Count; i++)
                    lake_manager.lake_visible[i] |= chunk_node.MapChunk.lakes_contained[i];
        }

        private static void RenderHeightmap()
        {
            GL.BindTexture(TextureTarget.Texture2DArray, scene.heightmap.texture_manager.terrain_texture);
            foreach(SceneNodeMapChunk chunk_node in scene.heightmap.visible_chunks)
            {
                SFMapHeightMapChunk chunk = chunk_node.MapChunk;

                // get chunk position
                Matrix4 chunk_matrix = chunk_node.ResultTransform;

                Matrix4 MVP_mat = chunk_matrix * scene.camera.ViewProjMatrix;

                GL.UniformMatrix4(shader_heightmap["MVP"], false, ref MVP_mat);
                GL.UniformMatrix4(shader_heightmap["M"], false, ref chunk_matrix);

                GL.BindVertexArray(chunk.vertex_array);
                GL.DrawArrays(PrimitiveType.Triangles, 0, chunk.vertices.Length);
            }
            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        // TODO! generate lake meshes for each map chunk (difficult?)
        private static void RenderLakes()
        {
            /*SFMapLakeManager lake_manager = scene.heightmap.map.lake_manager;
            GL.Uniform1(shader_simple["apply_shading"], 0);
            for(int i = 0; i < lake_manager.lakes.Count; i++)
            {
                if(lake_manager.lake_visible[i])
                {
                    ObjectSimple3D obj = scene.objects_static[lake_manager.lakes[i].GetObjectName()];
                    if (obj.Mesh == null)
                        continue;

                    Matrix4 MVP_mat = camera.ViewProjMatrix;
                    GL.UniformMatrix4(shader_simple["MVP"], false, ref MVP_mat);

                    GL.BindVertexArray(obj.Mesh.vertex_array);
                    SFMaterial mat = obj.Mesh.materials[0];
                    GL.Uniform1(shader_simple["texture_used"], (mat.texture != null ? 1 : 0));
                    if (mat.texture != null)
                        GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);
                    else
                        GL.BindTexture(TextureTarget.Texture2D, 0);

                    GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt, (int)mat.indexStart * 4);
                }
            }*/
        }

        // todo: move overlays to nodes
        public static void RenderOverlays()
        {
            /*GL.UseProgram(shader_overlay.ProgramID);
            foreach (string o in heightmap.visible_overlays)
            {
                foreach (SFMapHeightMapChunk chunk in heightmap.visible_chunks)
                {
                    if (chunk.overlays[o].points.Count == 0)
                        continue;
                    // get chunk position
                    Vector3 chunk_pos = new Vector3(chunk.ix * heightmap.chunk_size, 0, chunk.iy * heightmap.chunk_size);
                    Matrix4 chunk_matrix = Matrix4.CreateTranslation(chunk_pos);

                    Matrix4 MVP_mat = chunk_matrix * camera.ViewProjMatrix;

                    GL.UniformMatrix4(shader_overlay["MVP"], false, ref MVP_mat);
                    GL.Uniform4(shader_overlay["Color"], chunk.overlays[o].color);

                    GL.BindVertexArray(chunk.overlays[o].v_array);
                    GL.DrawElements(PrimitiveType.Triangles, chunk.overlays[o].elements.Length, DrawElementsType.UnsignedInt, 0);
                }
            }*/
        }

        // todo: sort transparent materials
        public static void RenderSimpleObjects()
        {
            //static objects
            GL.Uniform1(active_shader["texture_used"], 1);
            GL.Uniform1(active_shader["apply_shading"], 1);
            foreach (SFTexture tex in scene.tex_list_simple.Keys)
            {
                GL.BindTexture(TextureTarget.Texture2D, tex.tex_id);
                LinearPool<TexturedGeometryListElementSimple> elem_list = scene.tex_list_simple[tex];
                for(int i = 0; i < elem_list.elements.Count; i++)
                { 
                    if (!elem_list.elem_active[i])
                        continue;
                    TexturedGeometryListElementSimple elem = elem_list.elements[i];

                    Matrix4 model_mat = elem.node.ResultTransform;
                    Matrix4 mvp_mat = model_mat * scene.camera.ViewProjMatrix;
                    GL.UniformMatrix4(active_shader["M"], false, ref model_mat);
                    GL.UniformMatrix4(active_shader["MVP"], false, ref mvp_mat);

                    SFMaterial mat = elem.node.Mesh.materials[elem.submodel_index];
                    
                    GL.BindVertexArray(elem.node.Mesh.vertex_array);
                    //GL.Uniform1(active_shader["apply_shading"], (mat.apply_shading ? 1 : 0));

                    GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt, (int)mat.indexStart * 4);
                }
            }
        }

        public static void RenderAnimatedObjects()
        {
            //UseShader(shader_animated);
            foreach(SFTexture tex in scene.tex_list_animated.Keys)
            {
                GL.BindTexture(TextureTarget.Texture2D, tex.tex_id);
                LinearPool<TexturedGeometryListElementAnimated> elem_list = scene.tex_list_animated[tex];
                for (int i = 0; i < elem_list.elements.Count; i++)
                {
                    if (!elem_list.elem_active[i])
                        continue;
                    TexturedGeometryListElementAnimated elem = elem_list.elements[i];

                    Matrix4 model_mat = elem.node.ResultTransform;
                    Matrix4 mvp_mat = model_mat * scene.camera.ViewProjMatrix;
                    GL.UniformMatrix4(active_shader["M"], false, ref model_mat);
                    GL.UniformMatrix4(active_shader["MVP"], false, ref mvp_mat);

                    SFModelSkinChunk chunk = elem.node.Skin.submodels[elem.submodel_index];

                    Matrix4[] bones = new Matrix4[20];
                    for (int j = 0; j < chunk.bones.Length; j++)
                        bones[j] = elem.node.BoneTransforms[chunk.bones[j]];

                    GL.BindVertexArray(chunk.vertex_array);
                    GL.UniformMatrix4(active_shader["boneTransforms"], 20, false, ref bones[0].Row0.X);
                    GL.Uniform1(active_shader["apply_shading"], (chunk.material.apply_shading ? 1 : 0));

                    GL.DrawElements(PrimitiveType.Triangles, chunk.face_indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }
        }

        public static void RenderScene()
        {
            //draw everything to a texture
            SetFramebuffer(screenspace_framebuffer);
            GL.ClearColor(Color.MidnightBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Enable(EnableCap.DepthTest);

            // heightmap and overlays
            if (scene.heightmap != null)
            {
                UseShader(shader_heightmap);
                UpdateVisibleChunks();
                RenderHeightmap();
                UseShader(shader_simple);
                RenderOverlays();
            }

            UseShader(shader_animated);
            RenderAnimatedObjects();

            UseShader(shader_simple);
            RenderSimpleObjects();

            if (scene.heightmap != null)
            {
                UseShader(shader_simple);
                UpdateVisibleLakes();
                RenderLakes();
            }

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
        }
    }
}
