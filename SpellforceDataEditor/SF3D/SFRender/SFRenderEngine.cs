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

namespace SpellforceDataEditor.SF3D.SFRender
{
    public class SFRenderEngine
    {
        public SFSceneManager scene_manager { get; } = new SFSceneManager();
        SFResources.SFResourceManager resources;
        public bool ready_to_use { get; private set; } = false;

        public Camera3D camera { get; } = new Camera3D();

        SFShader shader_simple = new SFShader();
        SFShader shader_animated = new SFShader();

        public SFRenderEngine(SFResources.SFResourceManager res)
        {
            resources = res;
        }

        //called only once!
        public void Initialize(Vector2 view_size)
        {
            GL.ClearColor(Color.MidnightBlue);
            GL.Viewport(0, 0, (int)view_size.X, (int)view_size.Y);

            camera.ProjMatrix = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4, view_size.X / view_size.Y, 0.1f, 100f);

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
        }

        public int SpecifyGameDirectory(string dname)
        {
            int result = resources.unpacker.SpecifyGameDirectory(dname);
            ready_to_use = (result == 0);
            return result;
        }

        public void RenderFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (camera.Modified)
                camera.update_modelMatrix();

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

                    GL.Uniform1(shader_simple["texture_used"], 1);
                    GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);

                    GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt, (int)mat.indexStart * 4);
                }
                //transparent materials next
                for (int i = 0; i < obj.Mesh.materials.Length; i++)
                {
                    SFMaterial mat = obj.Mesh.materials[i];
                    if (mat.yet_to_be_drawn)
                    {
                        //todo: triangle sorting
                        GL.Uniform1(shader_simple["texture_used"], 1);
                        GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);

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

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
    }
}
