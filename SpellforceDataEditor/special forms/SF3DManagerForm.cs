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
        Dictionary<string, objectAnimated> objects_dynamic = new Dictionary<string, objectAnimated>();
        SFResourceManager resources = new SFResourceManager();
        List<string> mesh_names = new List<string>();
        List<string> skeleton_names = new List<string>();
        List<string> animation_names = new List<string>();

        Camera3D camera = new Camera3D();
        Matrix4 proj_matrix;
        Matrix4 viewproj_matrix;
        float ax, ay, az = 0f;

        int programID;
        int programID_anim;
        int matrixID;
        int matrixID_anim;
        int bone_matrix_arrayID_anim;
        int texture_usedID;
        int texture_usedID_anim;


        bool ready_to_use = false;

        int frames_per_second = 25;
        int current_frame = 0;
        System.Diagnostics.Stopwatch delta_timer = new System.Diagnostics.Stopwatch();
        float deltatime = 0f;

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
            programID_anim = ShaderCompiler.Compile(Properties.Resources.vshader_skel, Properties.Resources.fshader_skel);
            matrixID = GL.GetUniformLocation(programID, "MVP");
            matrixID_anim = GL.GetUniformLocation(programID_anim, "MVP");
            texture_usedID = GL.GetUniformLocation(programID, "texture_used");
            texture_usedID_anim = GL.GetUniformLocation(programID_anim, "texture_used");
            bone_matrix_arrayID_anim = GL.GetUniformLocation(programID_anim, "boneTransforms");

            delta_timer.Start();
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

            camera.Position = new Vector3(0, 1, 6);
            camera.look_at(new Vector3(0, 1, 0));

            glControl1.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glControl1.MakeCurrent();
            RenderFrame();
        }

        private void RenderFrame()
        {
            delta_timer.Stop();
            deltatime = delta_timer.ElapsedMilliseconds / (float)1000;
            delta_timer.Restart();


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
                for (int i = 0; i < obj.Mesh.materials.Length; i++)
                {
                    SFMaterial mat = obj.Mesh.materials[i];
                    if (mat.texture != null)
                    {
                        GL.Uniform1(texture_usedID, 1);
                        GL.BindTexture(TextureTarget.Texture2D, mat.texture.tex_id);
                    }
                    else
                        GL.Uniform1(texture_usedID, 0);

                    GL.DrawElements(PrimitiveType.Triangles, (int)mat.indexCount, DrawElementsType.UnsignedInt, (int)mat.indexStart * 4);
                }
                
            }
            //animated objects
            GL.UseProgram(programID_anim);
            foreach(objectAnimated obj in objects_dynamic.Values)
            {
                if (obj.Modified)
                    obj.update_modelMatrix();

                if (!obj.Visible)
                    continue;
                if (obj.skin == null)
                    continue;

                Matrix4 MVP_mat = obj.ModelMatrix * viewproj_matrix;
                GL.UniformMatrix4(matrixID_anim, false, ref MVP_mat);

                Matrix4[] bones = new Matrix4[20];
                for(int i = 0; i < obj.skin.submodels.Count; i++)
                {
                    SFModelSkinChunk chunk = obj.skin.submodels[i];
                    for (int j = 0; j < chunk.bones.Length; j++)
                        bones[j] = obj.bone_transforms[chunk.bones[j]];
                    GL.BindVertexArray(chunk.vertex_array);
                    GL.UniformMatrix4(bone_matrix_arrayID_anim, 20, false, ref bones[0].Row0.X);
                    GL.BindTexture(TextureTarget.Texture2D, chunk.material.texture.tex_id);
                    GL.DrawElements(PrimitiveType.Triangles, chunk.face_indices.Length, DrawElementsType.UnsignedInt, 0);

                }

                if (obj.anim_playing)
                    obj.step_animation(deltatime);
            }


            GL.BindVertexArray(0);
            GL.UseProgram(0);
            
            glControl1.SwapBuffers();

            current_frame++;
            System.Diagnostics.Debug.WriteLine("FRAME " + current_frame.ToString() + " DELTATIME " + deltatime.ToString());
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

            DisableAnimation();
            ListEntries.Items.Clear();
            ListAnimations.Items.Clear();
            objects_static.Clear();
            objects_dynamic.Clear();
            resources.DisposeAll();

            if(ComboBrowseMode.SelectedIndex == 0)
            {
                ObjectSimple3D obj_s1 = new ObjectSimple3D();
                obj_s1.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                objects_static.Add("simple_mesh", obj_s1);

                foreach (string mesh_name in mesh_names)
                    ListEntries.Items.Add(mesh_name);
            }

            if(ComboBrowseMode.SelectedIndex == 1)
            {
                objectAnimated obj_d1 = new objectAnimated();
                obj_d1.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                objects_dynamic.Add("dynamic_mesh", obj_d1);

                foreach (string skel_name in skeleton_names)
                    ListEntries.Items.Add(skel_name);
            }
            
        }

        private void ListEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ready_to_use)
                return;

            resources.DisposeAll();

            if (ComboBrowseMode.SelectedIndex == 0)
            {
                ObjectSimple3D obj_s1 = objects_static["simple_mesh"];
                obj_s1.Mesh = null;
                

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
            }

            if(ComboBrowseMode.SelectedIndex == 1)
            {
                ListAnimations.Items.Clear();
                DisableAnimation();

                objectAnimated obj_d1 = objects_dynamic["dynamic_mesh"];
                SFModelSkin skin = null;
                SFSkeleton skel = null;

                if (ListEntries.SelectedIndex != -1)
                {
                    string skin_name = ListEntries.SelectedItem.ToString();
                    skin_name = skin_name.Substring(0, skin_name.Length - 4);
                    int result = resources.Skins.Load(skin_name);
                    if (result != 0)
                    {
                        StatusText.Text = "Failed to load skin " + skin_name + ", status code "+result.ToString();
                        obj_d1.SetSkeletonSkin(null, null);
                        return;
                    }
                    skin = resources.Skins.Get(skin_name);
                    StatusText.Text = "Loaded skin " + skin_name;
                    statusStrip1.Refresh();

                    string skel_name = skin_name;
                    result = resources.Skeletons.Load(skel_name);
                    if (result != 0)
                    {
                        StatusText.Text = "Failed to load skeleton " + skel_name;
                        obj_d1.SetSkeletonSkin(null, null);
                        return;
                    }
                    skel = resources.Skeletons.Get(skel_name);
                    StatusText.Text = "Loaded skeleton " + skel_name;

                    List<string> anims = GetAllSkeletonAnimations(skel_name);
                    foreach (string n in anims)
                        ListAnimations.Items.Add(n);
                }

                obj_d1.SetSkeletonSkin(skel, skin);
                obj_d1.SetAnimation(null, false);
            }

            glControl1.Invalidate();
        }

        private void glControl1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        private void ListAnimations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBrowseMode.SelectedIndex == 1)
            {
                objectAnimated obj_d1 = objects_dynamic["dynamic_mesh"];

                if (ListAnimations.SelectedIndex != -1)
                {
                    string anim_name = ListAnimations.SelectedItem.ToString();
                    anim_name = anim_name.Substring(0, anim_name.Length - 4);
                    int result = resources.Animations.Load(anim_name);
                    if ((result != 0)&&(result != -1))
                    {
                        
                        StatusText.Text = "Failed to load animation " + anim_name + ", status code " + result.ToString();
                        DisableAnimation();
                        return;
                    }
                    if(resources.Animations.Get(anim_name).bone_count != obj_d1.skeleton.bone_count)
                    {
                        StatusText.Text = "Invalid animation "+anim_name;
                        DisableAnimation();
                        return;
                    }

                    obj_d1.SetAnimation(resources.Animations.Get(anim_name), true);
                    StatusText.Text = "Loaded animation " + anim_name;
                    statusStrip1.Refresh();

                    EnableAnimation();
                }
            }

            glControl1.Invalidate();
        }

        private void EnableAnimation()
        {
            TimerAnimation.Enabled = true;
            TimerAnimation.Interval = 1000/frames_per_second;
            TimerAnimation.Start();
        }

        private void DisableAnimation()
        {
            TimerAnimation.Stop();
            TimerAnimation.Enabled = false;
        }

        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            TimerAnimation.Start();
            glControl1.Invalidate();
        }

        //generate mesh names
        private void FindAllMeshes()
        {
            string[] filter_mesh = { "sf8.pak", "sf22.pak", "sf32.pak" };
            string[] filter_skel = { "sf4.pak", "sf22.pak", "sf32.pak" };
            string[] filter_anim = { "sf5.pak", "sf22.pak", "sf32.pak" };
            mesh_names = resources.unpacker.ListAllWithExtension(".msb", filter_mesh);
            skeleton_names = resources.unpacker.ListAllWithExtension(".bor", filter_skel);
            skeleton_names.RemoveAll(x => !(x.StartsWith("figure")));
            animation_names = resources.unpacker.ListAllWithExtension(".bob", filter_anim);
            mesh_names.Sort();
            skeleton_names.Sort();
            animation_names.Sort();
        }

        private List<string> GetAllSkeletonAnimations(string skel_name)
        {
            List<string> anims = new List<string>();

            //do the following:
            //cut skel_name to the last underscore
            //find all anims that have that prefix
            //cut until there's at least one anim
            while((anims.Count == 0)&&(skel_name != ""))
            {
                foreach(string anim_name in animation_names)
                {
                    if (anim_name.StartsWith(skel_name))
                        anims.Add(anim_name);
                }
                skel_name = skel_name.Substring(0, skel_name.LastIndexOf('_'));
            }

            return anims;
        }
    }
}
