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
        public static SFSceneManager scene_manager { get; } = new SFSceneManager();
        public static SFMapHeightMap heightmap { get; private set; } = null;

        public static Camera3D camera { get; } = new Camera3D();

        static SFShader shader_simple = new SFShader();
        static SFShader shader_animated = new SFShader();
        static SFShader shader_heightmap = new SFShader();
        static SFShader shader_overlay = new SFShader();

        static bool initialized = false;

        //called only once!
        public static void Initialize(Vector2 view_size)
        {
            if (initialized)
                return;

            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.Initialize() called");
            ResizeView(view_size);

            GL.ClearColor(Color.MidnightBlue);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.DepthFunc(DepthFunction.Less);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            shader_simple.CompileShader(Properties.Resources.vshader, Properties.Resources.fshader);
            shader_simple.AddParameter("MVP");
            shader_simple.AddParameter("texture_used");

            shader_animated.CompileShader(Properties.Resources.vshader_skel, Properties.Resources.fshader_skel);
            shader_animated.AddParameter("MVP");
            shader_animated.AddParameter("texture_used");
            shader_animated.AddParameter("boneTransforms");

            shader_heightmap.CompileShader(Properties.Resources.vshader_hmap, Properties.Resources.fshader_hmap);
            shader_heightmap.AddParameter("MVP");
            shader_heightmap.AddParameter("texture_used");

            shader_overlay.CompileShader(Properties.Resources.vshader_overlay, Properties.Resources.fshader_overlay);
            shader_overlay.AddParameter("MVP");
            shader_overlay.AddParameter("Color");

            //initialized = true;
        }

        public static void ResizeView(Vector2 view_size)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFRenderEngine.ResizeView() called (view_size = " + view_size.ToString() + ")");
            camera.ProjMatrix = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4, view_size.X / view_size.Y, 0.1f, 100f);
            GL.Viewport(0, 0, (int)view_size.X, (int)view_size.Y);
        }
        
        public static void AssignHeightMap(SFMapHeightMap hm)
        {
            heightmap = hm;
        }

        private static void UpdateVisibleChunks()
        {
            // 1. find collection of visible chunks
            List<SFMapHeightMapChunk> vis_chunks = new List<SFMapHeightMapChunk>();

            // test visibility of each chunk
            foreach(SFMapHeightMapChunk chunk in heightmap.chunks)
            {
                if (!chunk.aabb.IsOutsideOfConvexHull(camera.FrustrumPlanes))  // frustrum planes updated when camera is updated
                {
                    vis_chunks.Add(chunk);
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
                        vis_chunks[i].Visible = true;
                        // add visible to the next chunk
                    }
                    break;
                }
                if(next_end)
                {
                    for (int i = cur_list_id; i < heightmap.visible_chunks.Count; i++)
                    {
                        heightmap.visible_chunks[i].Visible = false;
                        // add invisible to the current chunk
                    }
                    break;
                }

                cur_chunk_id = heightmap.visible_chunks[cur_list_id].id;
                next_chunk_id = vis_chunks[next_list_id].id;
                // if next id > cur id, keep increasing cur id, while simultaneously turning chunks invisible
                // otherwise keep increasing next_id, while simultaneuosly turning chunks visible
                if (next_chunk_id > cur_chunk_id)
                {
                    while(next_chunk_id > cur_chunk_id)
                    {
                        heightmap.visible_chunks[cur_list_id].Visible = false;
                        // turn chunk invisible

                        cur_list_id += 1;
                        if (cur_list_id == heightmap.visible_chunks.Count)
                            break;
                        cur_chunk_id = heightmap.visible_chunks[cur_list_id].id;
                    }
                }
                else if (next_chunk_id < cur_chunk_id)
                {
                    while (next_chunk_id < cur_chunk_id)
                    {
                        vis_chunks[next_list_id].Visible = true;
                        // turn chunk visible

                        next_list_id += 1;
                        if (next_list_id == vis_chunks.Count)
                            break;
                        next_chunk_id = vis_chunks[next_list_id].id;
                    }
                }
                else
                {
                    next_list_id += 1;
                    cur_list_id += 1;
                }
            }
            heightmap.visible_chunks = vis_chunks;
        }

        private static void UpdateVisibleLakes()
        {
            // reset visibility
            SFMapLakeManager lake_manager = heightmap.map.lake_manager;
            for (int i = 0; i < lake_manager.lake_visible.Count; i++)
                lake_manager.lake_visible[i] = false;

            // update visibility
            foreach (SFMapHeightMapChunk chunk in heightmap.visible_chunks)
                for (int i = 0; i < lake_manager.lake_visible.Count; i++)
                    lake_manager.lake_visible[i] |= chunk.lakes_contained[i];
        }

        private static void RenderHeightmap()
        {
            // special shader here...
            GL.UseProgram(shader_heightmap.ProgramID);
            foreach (SFMapHeightMapChunk chunk in heightmap.chunks)
            {
                if (!chunk.Visible)
                    continue;

                // get chunk position
                Vector3 chunk_pos = new Vector3(chunk.ix * heightmap.chunk_size, 0, chunk.iy * heightmap.chunk_size);
                Matrix4 chunk_matrix = Matrix4.CreateTranslation(chunk_pos);

                Matrix4 MVP_mat = chunk_matrix * camera.ViewProjMatrix;

                GL.UniformMatrix4(shader_heightmap["MVP"], false, ref MVP_mat);

                GL.BindVertexArray(chunk.vertex_array);
                GL.Uniform1(shader_heightmap["texture_used"], 1);
                GL.BindTexture(TextureTarget.Texture2DArray, heightmap.texture_manager.terrain_texture);

                GL.DrawArrays(PrimitiveType.Triangles, 0, chunk.vertices.Length);
                GL.BindTexture(TextureTarget.Texture2DArray, 0);
            }
        }

        private static void RenderLakes()
        {
            SFMapLakeManager lake_manager = heightmap.map.lake_manager;

            GL.UseProgram(shader_simple.ProgramID);
            for(int i = 0; i < lake_manager.lakes.Count; i++)
            {
                if(lake_manager.lake_visible[i])
                {
                    ObjectSimple3D obj = scene_manager.objects_static[lake_manager.lakes[i].GetObjectName()];
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
            }
        }

        public static void RenderOverlays()
        {
            GL.UseProgram(shader_overlay.ProgramID);
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
            }
        }

        public static void RenderFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (camera.Modified)
                camera.update_modelMatrix();

            // heightmap
            if (heightmap != null)
            {
                UpdateVisibleChunks();
                RenderHeightmap();
                RenderOverlays();
            }

            //static objects
            GL.UseProgram(shader_simple.ProgramID);
            foreach (ObjectSimple3D obj in scene_manager.objects_static.Values)
            {
                if (!obj.Visible)
                    continue;
                if (obj.Mesh == null)
                    continue;

                Matrix4 MVP_mat = obj.ModelMatrix * camera.ViewProjMatrix;
                GL.UniformMatrix4(shader_simple["MVP"], false, ref MVP_mat);

                //opaque materials first (no alpha except 0 and 1)
                GL.BindVertexArray(obj.Mesh.vertex_array);
                for (int i = 0; i < obj.Mesh.materials.Length; i++)
                {
                    SFMaterial mat = obj.Mesh.materials[i];

                    if ((mat.matFlags & 4) == 0)
                        continue;
                    mat.yet_to_be_drawn = false;

                    GL.Uniform1(shader_simple["texture_used"], (mat.texture != null ? 1 : 0));
                    if (mat.texture != null)
                        GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);
                    else
                        GL.BindTexture(TextureTarget.Texture2D, 0);

                    GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt, (int)mat.indexStart * 4);
                }
                //transparent materials next
                for (int i = 0; i < obj.Mesh.materials.Length; i++)
                {
                    SFMaterial mat = obj.Mesh.materials[i];
                    if (mat.yet_to_be_drawn)
                    {
                        //todo: triangle sorting
                        GL.Uniform1(shader_simple["texture_used"], (mat.texture != null?1:0));
                        if (mat.texture != null)
                            GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);
                        else
                            GL.BindTexture(TextureTarget.Texture2D, 0);

                        GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt, (int)mat.indexStart * 4);
                    }
                    mat.yet_to_be_drawn = true;    //reset
                }

            }
            //animated objects (assuming animated models are entirely opaque)
            GL.UseProgram(shader_animated.ProgramID);
            foreach (objectAnimated obj in scene_manager.objects_dynamic.Values)
            {
                if (!obj.Visible)
                    continue;
                if (obj.skin == null)
                    continue;

                Matrix4 MVP_mat = obj.ModelMatrix * camera.ViewProjMatrix;
                GL.UniformMatrix4(shader_animated["MVP"], false, ref MVP_mat);

                Matrix4[] bones = new Matrix4[20];
                for (int i = 0; i < obj.skin.submodels.Count; i++)
                {
                    SFModelSkinChunk chunk = obj.skin.submodels[i];
                    for (int j = 0; j < chunk.bones.Length; j++)
                        bones[j] = obj.bone_transforms[chunk.bones[j]];
                    GL.BindVertexArray(chunk.vertex_array);
                    GL.UniformMatrix4(shader_animated["boneTransforms"], 20, false, ref bones[0].Row0.X);
                    GL.BindTexture(TextureTarget.Texture2D, chunk.material.texture.tex_id);
                    GL.DrawElements(PrimitiveType.Triangles, chunk.face_indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }

            if(heightmap != null)
            {
                UpdateVisibleLakes();
                RenderLakes();
            }

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
    }
}
