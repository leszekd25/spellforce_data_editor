using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SpellforceDataEditor.SFUnPak;
using SpellforceDataEditor.SF3D;

namespace SpellforceDataEditor.special_forms
{
    public partial class SF3DManagerForm : Form
    {
        Dictionary<string, ObjectSimple3D> objects_static = new Dictionary<string, ObjectSimple3D>();
        SFResourceManager resources = new SFResourceManager();
        List<string> mesh_names = new List<string>();
        List<string> skeleton_names = new List<string>();
        List<string> animation_names = new List<string>();

        Camera3D camera = new Camera3D();
        Matrix4 proj_matrix;
        Matrix4 viewproj_matrix;
        float ax, ay, az = 0f;

        int programID;
        int matrixID;
        int texture_usedID;

        bool ready_to_use = false;

        int current_frame = 0;

        public SF3DManagerForm()
        {
            InitializeComponent();
        }

        void PrepareGLView()
        {
            if (glControl1.ClientSize.Height == 0)
                glControl1.ClientSize = new System.Drawing.Size(glControl1.ClientSize.Width, 1);

            GL.Viewport(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height);
            
            proj_matrix = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI/4, glControl1.ClientSize.Width / glControl1.ClientSize.Height, 0.1f, 100f);
            viewproj_matrix = proj_matrix;
            
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            
            programID = ShaderCompiler.Compile(Properties.Resources.vshader, Properties.Resources.fshader);
            matrixID = GL.GetUniformLocation(programID, "MVP");
            texture_usedID = GL.GetUniformLocation(programID, "texture_used");
        }

        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (GameDirDialog.ShowDialog() == DialogResult.OK)
            {
                int result = specify_game_directory(GameDirDialog.SelectedPath);
                if(result == 0)
                {
                    File.WriteAllText("game_directory.txt", GameDirDialog.SelectedPath);
                    StatusText.Text = "Successfully loaded PAK data";
                    ready_to_use = true;
                }
                else
                {
                    StatusText.Text = "Failed to load PAK data";
                    ready_to_use = false;
                }
            }
        }

        int specify_game_directory(string dname)
        {
            StatusText.Text = "Loading...";
            statusStrip1.Refresh();
            int result = resources.unpacker.SpecifyGameDirectory(dname);
            if(result == 0)
            {
                ObjectSimple3D obj_s1 = new ObjectSimple3D();
                obj_s1.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                objects_static.Add("simple_mesh", obj_s1);

                FindAllMeshes();
            }
            return result;
        }

        private void SF3DManagerForm_Load(object sender, EventArgs e)
        {
            //base.OnLoad(e);

            int preload_success = -1;
            if(File.Exists("game_directory.txt"))
            {
                string gamedir = File.ReadAllText("game_directory.txt");
                preload_success = specify_game_directory(gamedir);
            }
            if (preload_success == 0)
            {
                StatusText.Text = "PAK data preloaded successfully";
                ready_to_use = true;
            }
            else
                StatusText.Text = "Failed to preload PAK files. Specify game directory";


            GL.ClearColor(Color.MidnightBlue);
            
            PrepareGLView();

            camera.Position = new Vector3(3, 3, 3);
            camera.look_at(new Vector3(0, 0, 0));

            glControl1.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glControl1.MakeCurrent();
            RenderFrame();
        }

        private void RenderFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (camera.Modified)
            {
                camera.update_modelMatrix();
                viewproj_matrix = camera.ModelMatrix * proj_matrix;
            }

            GL.UseProgram(programID);

            foreach (ObjectSimple3D obj in objects_static.Values)
            {
                if (obj.Modified)
                    obj.update_modelMatrix();

                if (!obj.Visible)
                    continue;
                if (obj.Mesh == null)
                    continue;

                Matrix4 MVP_mat = obj.ModelMatrix * viewproj_matrix;
                //Matrix4 modelview_matrix = camera.ModelMatrix * obj.ModelMatrix;
                
                //GL.LoadMatrix(ref modelview_matrix);
                GL.UniformMatrix4(matrixID, false, ref MVP_mat);
                GL.BindVertexArray(obj.Mesh.vertex_array);
                for(int i = 0; i < obj.Mesh.materials.Length; i++)
                {
                    SFMaterial mat = obj.Mesh.materials[i];
                    if (mat.texture != null)
                    {
                        GL.Uniform1(texture_usedID, 1);
                        GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);
                    }
                    else
                        GL.Uniform1(texture_usedID, 0);

                    GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt,(int)mat.indexStart*4);
                }
                
            }

            GL.BindVertexArray(0);
            GL.UseProgram(0);
            
            glControl1.SwapBuffers();

            current_frame++;
            //angle += 0.5f;
        }

        private void SF3DManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            objects_static.Clear();
            resources.DisposeAll();
        }

        private void ComboBrowseMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ready_to_use)
                return;

            ListEntries.Items.Clear();
            ListAnimations.Items.Clear();
            if(ComboBrowseMode.SelectedIndex == 0)
            {
                foreach (string mesh_name in mesh_names)
                    ListEntries.Items.Add(mesh_name);
            }
            
        }

        private void ListEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ready_to_use)
                return;

            if(ComboBrowseMode.SelectedIndex == 0)
            {
                ObjectSimple3D obj_s1 = objects_static["simple_mesh"];
                obj_s1.Mesh = null;

                resources.DisposeAll();

                if (ListEntries.SelectedIndex != -1)
                {
                    string model_name = ListEntries.SelectedItem.ToString();
                    model_name = model_name.Substring(0, model_name.Length - 4);
                    int result = resources.Models.Load(model_name);
                    if(result != 0)
                    {
                        StatusText.Text = "Failed to load model " + model_name;
                        return;
                    }
                    obj_s1.Mesh = resources.Models.Get(model_name);
                    StatusText.Text = "Loaded model " + model_name;
                }


                glControl1.Invalidate();
            }
        }

        private void glControl1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        //generate mesh names
        private void FindAllMeshes()
        {
            string[] filter = { "sf8.pak" };
            mesh_names = resources.unpacker.ListAllWithExtension(".msb", filter);
            mesh_names.Sort();
        }
    }
}
