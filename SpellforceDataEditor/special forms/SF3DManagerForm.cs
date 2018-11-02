/*
 * This form serves as a 3D model/animation viewer
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SpellforceDataEditor.SF3D;
using SpellforceDataEditor.SF3D.SceneSynchro;
using SpellforceDataEditor.SF3D.SFRender;

namespace SpellforceDataEditor.special_forms
{
    public partial class SF3DManagerForm : Form
    {
        SFRenderEngine render_engine = new SFRenderEngine();

        bool synchronized = false;

        Point mouse_pos;
        bool mouse_pressed = false;

        public SF3DManagerForm()
        {
            InitializeComponent();
        }

        //specify game directory button
        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (GameDirDialog.ShowDialog() == DialogResult.OK)
            {
                int result = specify_game_directory(GameDirDialog.SelectedPath);
                if(result == 0)
                {
                    File.WriteAllText("game_directory.txt", GameDirDialog.SelectedPath);
                    StatusText.Text = "Successfully loaded PAK data";
                }
                else
                {
                    StatusText.Text = "Failed to load PAK data";
                }
            }
        }

        int specify_game_directory(string dname)
        {
            StatusText.Text = "Loading...";
            statusStrip1.Refresh();
            int result = render_engine.SpecifyGameDirectory(dname);
            if(result == 0)
            {
                render_engine.resources.FindAllMeshes();
            }
            return result;
        }

        //on form open
        private void SF3DManagerForm_Load(object sender, EventArgs e)
        {
            int preload_success = -1;
            if(File.Exists("game_directory.txt"))
            {
                string gamedir = File.ReadAllText("game_directory.txt");
                preload_success = specify_game_directory(gamedir);
            }
            if (preload_success == 0)
                StatusText.Text = "PAK data preloaded successfully";
            else
                StatusText.Text = "Failed to preload PAK files. Specify game directory";

            render_engine.Initialize(new Vector2(glControl1.ClientSize.Width, glControl1.ClientSize.Height));
            render_engine.camera.Position = new Vector3(0, 1, 6);
            render_engine.camera.Lookat = new Vector3(0, 1, 0);

            glControl1.Invalidate();
        }

        //done by main form
        public void SetGameData(SFCategoryManager man)
        {
            render_engine.scene_manager.resources = render_engine.resources;
            render_engine.scene_manager.Init(man);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glControl1.MakeCurrent();
            render_engine.RenderFrame();
            glControl1.SwapBuffers();
        }

        private void SF3DManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            render_engine.scene_manager.ClearScene();
            render_engine.resources.DisposeAll();
        }

        private void ComboBrowseMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!render_engine.ready_to_use)
                return;

            DisableAnimation();
            ListEntries.Items.Clear();
            ListAnimations.Items.Clear();
            render_engine.scene_manager.ClearScene();
            render_engine.resources.DisposeAll();
            synchronized = false;

            if(ComboBrowseMode.SelectedIndex == 0)
            {
                //generate scene
                SFSceneDescription scene = new SFSceneDescription();
                scene.add_line(SCENE_ITEM_TYPE.OBJ_SIMPLE, new string[] { "", "", "simple_mesh" });
                render_engine.scene_manager.ParseSceneDescription(scene);

                foreach (string mesh_name in render_engine.resources.mesh_names)
                    ListEntries.Items.Add(mesh_name);
            }

            if(ComboBrowseMode.SelectedIndex == 1)
            {
                //generate scene
                SFSceneDescription scene = new SFSceneDescription();
                scene.add_line(SCENE_ITEM_TYPE.OBJ_ANIMATED, new string[] { "", "", "dynamic_mesh" });
                render_engine.scene_manager.ParseSceneDescription(scene);

                foreach (string skel_name in render_engine.resources.skeleton_names)
                    ListEntries.Items.Add(skel_name);
            }

            if (ComboBrowseMode.SelectedIndex == 2)
            {
                synchronized = true;
            }
        }

        private void ListEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!render_engine.ready_to_use)
                return;

            render_engine.resources.DisposeAll();

            SFSceneLoader scene = render_engine.scene_manager;

            if (ComboBrowseMode.SelectedIndex == 0)
            {
                ObjectSimple3D obj_s1 = scene.objects_static["simple_mesh"];
                obj_s1.Mesh = null;

                if (ListEntries.SelectedIndex != -1)
                {
                    string model_name = ListEntries.SelectedItem.ToString();
                    model_name = model_name.Substring(0, model_name.Length - 4);
                    int result = render_engine.resources.Models.Load(model_name);
                    if(result != 0)
                    {
                        StatusText.Text = "Failed to load model " + model_name;
                        return;
                    }
                    obj_s1.Mesh = render_engine.resources.Models.Get(model_name);
                    StatusText.Text = "Loaded model " + model_name;
                }
            }

            if(ComboBrowseMode.SelectedIndex == 1)
            {
                ListAnimations.Items.Clear();
                DisableAnimation();

                objectAnimated obj_d1 = scene.objects_dynamic["dynamic_mesh"];
                SFModelSkin skin = null;
                SFSkeleton skel = null;

                if (ListEntries.SelectedIndex != -1)
                {
                    string skin_name = ListEntries.SelectedItem.ToString();
                    skin_name = skin_name.Substring(0, skin_name.Length - 4);
                    int result = render_engine.resources.Skins.Load(skin_name);
                    if (result != 0)
                    {
                        StatusText.Text = "Failed to load skin " + skin_name + ", status code "+result.ToString();
                        obj_d1.SetSkeletonSkin(null, null);
                        return;
                    }
                    skin = render_engine.resources.Skins.Get(skin_name);
                    StatusText.Text = "Loaded skin " + skin_name;
                    statusStrip1.Refresh();

                    string skel_name = skin_name;
                    result = render_engine.resources.Skeletons.Load(skel_name);
                    if (result != 0)
                    {
                        StatusText.Text = "Failed to load skeleton " + skel_name;
                        obj_d1.SetSkeletonSkin(null, null);
                        return;
                    }
                    skel = render_engine.resources.Skeletons.Get(skel_name);
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

            if (!mouse_pressed)
                return;

            float cam_speed = 6; //limited by framerate

            //calculate movement vector
            Vector3 cam_move = (((render_engine.camera.Lookat - render_engine.camera.Position).Normalized())*cam_speed)/render_engine.frames_per_second;

            if (e.KeyChar == 'w')
                render_engine.camera.translate(cam_move);
            else if (e.KeyChar == 's')
                render_engine.camera.translate(-cam_move);
        }


        private void ListAnimations_SelectedIndexChanged(object sender, EventArgs e)
        {
            SFSceneLoader scene = render_engine.scene_manager;

            if (ComboBrowseMode.SelectedIndex == 1)
            {
                objectAnimated obj_d1 = scene.objects_dynamic["dynamic_mesh"];

                if (ListAnimations.SelectedIndex != -1)
                {
                    string anim_name = ListAnimations.SelectedItem.ToString();
                    anim_name = anim_name.Substring(0, anim_name.Length - 4);
                    int result = render_engine.resources.Animations.Load(anim_name);
                    if ((result != 0)&&(result != -1))
                    {
                        
                        StatusText.Text = "Failed to load animation " + anim_name + ", status code " + result.ToString();
                        DisableAnimation();
                        return;
                    }
                    if(render_engine.resources.Animations.Get(anim_name).bone_count != obj_d1.skeleton.bone_count)
                    {
                        StatusText.Text = "Invalid animation "+anim_name;
                        DisableAnimation();
                        return;
                    }

                    obj_d1.SetAnimation(render_engine.resources.Animations.Get(anim_name), true);
                    StatusText.Text = "Loaded animation " + anim_name;
                    statusStrip1.Refresh();

                    EnableAnimation();
                }
            }

            if (ComboBrowseMode.SelectedIndex == 2)
            {
                if (ListAnimations.SelectedIndex != -1)
                {
                    string anim_name = ListAnimations.SelectedItem.ToString();
                    anim_name = anim_name.Substring(0, anim_name.Length - 4);
                    int result = render_engine.resources.Animations.Load(anim_name);
                    if ((result != 0) && (result != -1))
                    {

                        StatusText.Text = "Failed to load animation " + anim_name + ", status code " + result.ToString();
                        DisableAnimation();
                        return;
                    }

                    foreach (KeyValuePair<string, string> kv in scene.scene_meta.obj_to_anim)
                    {
                        if (render_engine.resources.Animations.Get(anim_name).bone_count != scene.objects_dynamic[kv.Key].skeleton.bone_count)
                        {
                            StatusText.Text = "Invalid animation " + anim_name;
                            DisableAnimation();
                            return;
                        }
                        scene.objects_dynamic[kv.Key].SetAnimation(render_engine.resources.Animations.Get(anim_name), true);
                    }
                    
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
            TimerAnimation.Interval = 1000/render_engine.frames_per_second;
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

        private List<string> GetAllSkeletonAnimations(string skel_name)
        {
            List<string> anims = new List<string>();

            //do the following:
            //cut skel_name to the last underscore
            //find all anims that have that prefix
            //cut until there's at least one anim
            while((anims.Count == 0)&&(skel_name != ""))
            {
                foreach(string anim_name in render_engine.resources.animation_names)
                {
                    if (anim_name.StartsWith(skel_name))
                        anims.Add(anim_name);
                }
                skel_name = skel_name.Substring(0, skel_name.LastIndexOf('_'));
            }

            return anims;
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            //check pressed flag
            mouse_pressed = true;
            EnableAnimation();
            //set cursor position
            Cursor.Position = new Point(this.Location.X + glControl1.Location.X + (glControl1.Size.Width / 2), this.Location.Y + glControl1.Location.Y + (glControl1.Size.Height / 2));
            mouse_pos = Cursor.Position;
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            mouse_pressed = false;
            if (ComboBrowseMode.SelectedIndex != 1)
                DisableAnimation();
            if ((ComboBrowseMode.SelectedIndex == 2)&& (render_engine.scene_manager.scene_meta != null) && (render_engine.scene_manager.scene_meta.is_animated))
                EnableAnimation();
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouse_pressed)
                return;

            float mult_f = 0.003f;
            //get mouse delta
            float mx, my;
            mx = Cursor.Position.X - mouse_pos.X; my = Cursor.Position.Y - mouse_pos.Y;
            //produce angle difference
            render_engine.camera.Direction += new Vector2(mx * mult_f, -my * mult_f);
            //set cursor position
            Cursor.Position = new Point(this.Location.X + glControl1.Location.X + (glControl1.Size.Width / 2), this.Location.Y + glControl1.Location.Y + (glControl1.Size.Height / 2));

            //glControl1.Invalidate();
        }

        public void GenerateScene(int cat, int elem)
        {
            if (!synchronized)
                return;

            DisableAnimation();
            ListEntries.Items.Clear();
            ListAnimations.Items.Clear();
            render_engine.scene_manager.ClearScene();
            render_engine.resources.DisposeAll();
            GC.Collect(2, GCCollectionMode.Forced, false);

            System.Diagnostics.Debug.WriteLine("GENERATING SCENE " + cat.ToString() + "|" + elem.ToString());
            render_engine.scene_manager.ParseSceneDescription(render_engine.scene_manager.CatElemToScene(cat, elem));

            if (render_engine.scene_manager.scene_meta.is_animated)
            {
                if (render_engine.scene_manager.scene_meta.obj_to_anim.ContainsKey("MAIN"))
                {
                    ListAnimations.Items.Clear();
                    List<string> anims = GetAllSkeletonAnimations(render_engine.scene_manager.scene_meta.obj_to_anim["MAIN"]);
                    foreach (string n in anims)
                        ListAnimations.Items.Add(n);
                }
            }

            glControl1.Invalidate();
        }
    }
}
