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
    public class SFRenderEngine
    {
        public SFSceneManager scene_manager { get; } = new SFSceneManager();
        public SFMapHeightMap heightmap { get; private set; } = null;

        public Camera3D camera { get; } = new Camera3D();

        static SFShader shader_simple = new SFShader();
        static SFShader shader_animated = new SFShader();
        static SFShader shader_heightmap = new SFShader();

        //called only once!
        public void Initialize(Vector2 view_size)
        {
            camera.ProjMatrix = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4, view_size.X / view_size.Y, 0.1f, 100f);
            GL.Viewport(0, 0, (int)view_size.X, (int)view_size.Y);

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
        }

        public void AssignHeightMap(SFMapHeightMap hm)
        {
            // remove previous heightmap

            // add new heightmap
            heightmap = hm;
        }

        private void RenderHeightmap()
        {
            // special shader here...
            GL.UseProgram(shader_heightmap.ProgramID);
            foreach (SFMapHeightMapChunk chunk in heightmap.chunks)
            {
                // todo: precalculate chunk position and matrix in chunk generation

                // get chunk position
                Vector3 chunk_pos = new Vector3(chunk.ix * heightmap.chunk_size, 0, chunk.iy * heightmap.chunk_size);
                // prune invisible chunks here...
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

        public void RenderFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (camera.Modified)
                camera.update_modelMatrix();

            // heightmap
            if (heightmap != null)
                RenderHeightmap();

            //static objects
            GL.UseProgram(shader_simple.ProgramID);
            foreach (ObjectSimple3D obj in scene_manager.objects_static.Values)
            {
                if (!obj.Visible)
                    continue;
                if (obj.Mesh == null)
                    continue;
                // heightmap pruning

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
                // heightmap pruning

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

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
    }
}
