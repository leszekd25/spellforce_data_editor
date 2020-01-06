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
using SpellforceDataEditor.SFSound;
using SpellforceDataEditor.SFResources;

namespace SpellforceDataEditor.special_forms
{
    public partial class SFAssetManagerForm : Form
    {
        SFSoundEngine sound_engine = new SFSoundEngine();

        bool synchronized = false;

        Point mouse_pos;
        bool mouse_pressed = false;
        Vector2 scroll_mouse_start = new Vector2(0, 0);
        bool[] arrows_pressed = new bool[] { false, false, false, false };  // left, right, up, down

        bool tmp_shadows = Settings.EnableShadows;
        bool update_render = true;
        bool dynamic_render = false;

        public SFAssetManagerForm()
        {
            InitializeComponent();
        }

        // on form open
        // assumes game directory is specified
        private void SF3DManagerForm_Load(object sender, EventArgs e)
        {
            tmp_shadows = Settings.EnableShadows;
            Settings.EnableShadows = false;

            SFResourceManager.FindAllMeshes();

            SFLua.SFLuaEnvironment.LoadSQL(false);
            if (!SFLua.SFLuaEnvironment.data_loaded)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFAssetManagerForm(): Failed to load SQL data!");
                Close();
                return;
            }

            SFRenderEngine.scene.Init();
            SFRenderEngine.Initialize(new Vector2(glControl1.ClientSize.Width, glControl1.ClientSize.Height));
            SFRenderEngine.scene.camera.Position = new Vector3(0, 1, 6);
            SFRenderEngine.scene.camera.Lookat = new Vector3(0, 1, 0);

            TimerAnimation.Enabled = true;
            TimerAnimation.Interval = 1000 / SFRenderEngine.scene.frames_per_second;
            TimerAnimation.Start();
            SFRenderEngine.scene.delta_timer.Restart();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glControl1.MakeCurrent();
            SFRenderEngine.RenderScene();
            glControl1.SwapBuffers();
        }

        private void SF3DManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SFRenderEngine.scene.RemoveSceneNode(SFRenderEngine.scene.root, true);
            SFRenderEngine.scene.root = null;
            SFRenderEngine.scene.camera = null;
            SFResourceManager.DisposeAll();
            sound_engine.UnloadSound();

            Settings.EnableShadows = tmp_shadows;
        }

        private void SFAssetManagerForm_Resize(object sender, EventArgs e)
        {
            int rcheight = this.Height - 89;
            int rcwidth = this.Width - 371;
            int new_rcsize = Math.Min(rcheight, rcwidth);
            glControl1.Size = new Size(new_rcsize, new_rcsize);

            int rcx = this.Width - new_rcsize - 16;
            glControl1.Location = new Point(rcx, glControl1.Location.Y);

            int listwidth = rcx - 87 - 18;
            int listheight = this.Height - 342;
            ListEntries.Size = new Size(listwidth, listheight);
            PanelSound.Location = new Point(PanelSound.Location.X, ListEntries.Location.Y + listheight + 6);
            ListAnimations.Size = new Size(listwidth, ListAnimations.Height);
            ListAnimations.Location = new Point(ListAnimations.Location.X, PanelSound.Location.Y + PanelSound.Height + 6);
            button1Extract.Location = new Point(rcx - 87, button1Extract.Location.Y);
            button2Extract.Location = new Point(rcx - 87, ListAnimations.Location.Y);



            SFRenderEngine.ResizeView(new Vector2(new_rcsize, new_rcsize));
            update_render = true;
        }

        private void HideAllPanels()
        {
            comboMessages.Hide();
            ListEntries.Hide();
            ListAnimations.Hide();
            button1Extract.Hide();
            button2Extract.Hide();
            PanelSound.Hide();
        }

        private void ComboBrowseMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListEntries.Items.Clear();
            ListAnimations.Items.Clear();
            ResetScene();
            dynamic_render = false;
            sound_engine.UnloadSound();
            TimerSoundDuration.Stop();
            trackSoundDuration.Value = 0;
            SFResourceManager.DisposeAll();
            HideAllPanels();
            synchronized = false;

            if(ComboBrowseMode.SelectedIndex == 0)
            {
                ListEntries.Show();
                button1Extract.Show();
                //generate scene
                SceneNodeSimple simple_node = SFRenderEngine.scene.AddSceneNodeSimple(SFRenderEngine.scene.root, "", "simple_mesh");
                simple_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);


                foreach (string mesh_name in SFResourceManager.mesh_names)
                    ListEntries.Items.Add(mesh_name);
            }

            if(ComboBrowseMode.SelectedIndex == 1)
            {
                ListEntries.Show();
                button1Extract.Show();
                PanelSound.Show();
                ListAnimations.Show();
                button2Extract.Show();
                //generate scene
                SceneNodeAnimated animated_node = SFRenderEngine.scene.AddSceneNodeAnimated(SFRenderEngine.scene.root, "", "dynamic_mesh");
                animated_node.Rotation =Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);


                foreach (string skel_name in SFResourceManager.skeleton_names)
                    ListEntries.Items.Add(skel_name);
            }

            if (ComboBrowseMode.SelectedIndex == 2)
            {
                PanelSound.Show();
                ListAnimations.Show();
                synchronized = true;
            }

            if (ComboBrowseMode.SelectedIndex == 3)
            {
                ListEntries.Show();
                button1Extract.Show();
                PanelSound.Show();
                foreach (string musi_name in SFResourceManager.music_names)
                    ListEntries.Items.Add(musi_name);
            }

            if (ComboBrowseMode.SelectedIndex == 4)
            {
                ListEntries.Show();
                button1Extract.Show();
                PanelSound.Show();
                foreach (string snd_name in SFResourceManager.sound_names)
                    ListEntries.Items.Add(snd_name);
            }

            if (ComboBrowseMode.SelectedIndex == 5)
            {
                ListEntries.Show();
                button1Extract.Show();
                PanelSound.Show();
                comboMessages.SelectedIndex = -1;
                comboMessages.Show();
            }
        }

        private void ListEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            SFScene scene = SFRenderEngine.scene;

            if (ComboBrowseMode.SelectedIndex == 0)
            {
                SceneNodeSimple obj_s1 = scene.root.FindNode<SceneNodeSimple>("simple_mesh");
                // WORKAROUND!!!
                // caching simple meshes works differently from animated meshes
                // to be fixed...
                scene.RemoveSceneNode(obj_s1);
                //obj_s1.Mesh = null;         <- this should be instead of the above

                if (ListEntries.SelectedIndex != -1)
                {
                    string model_name = ListEntries.SelectedItem.ToString();
                    model_name = model_name.Substring(0, model_name.Length - 4);
                    int result = SFResourceManager.Models.Load(model_name);
                    if ((result != 0) && (result != -1))
                    {
                        StatusText.Text = "Failed to load model " + model_name;
                        return;
                    }
                    // WORKAROUND!!!
                    obj_s1 = scene.AddSceneNodeSimple(scene.root, model_name, "simple_mesh");
                    obj_s1.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                    //obj_s1.Mesh = SFResourceManager.Models.Get(model_name);     <- this should be instead of above
                    StatusText.Text = "Loaded model " + model_name;
                }
            }

            if(ComboBrowseMode.SelectedIndex == 1)
            {
                ListAnimations.Items.Clear();

                SceneNodeAnimated obj_d1 = scene.root.FindNode<SceneNodeAnimated>("dynamic_mesh");
                SFModelSkin skin = null;
                SFSkeleton skel = null;

                if (ListEntries.SelectedIndex != -1)
                {
                    string skin_name = ListEntries.SelectedItem.ToString();
                    skin_name = skin_name.Substring(0, skin_name.Length - 4);
                    int result = SFResourceManager.Skins.Load(skin_name);
                    if ((result != 0)&&(result != -1))
                    {
                        StatusText.Text = "Failed to load skin " + skin_name + ", status code "+result.ToString();
                        obj_d1.SetSkeletonSkin(null, null);
                        return;
                    }
                    skin = SFResourceManager.Skins.Get(skin_name);
                    StatusText.Text = "Loaded skin " + skin_name;
                    statusStrip1.Refresh();

                    string skel_name = skin_name;
                    result = SFResourceManager.Skeletons.Load(skel_name);
                    if ((result != 0) && (result != -1))
                    {
                        StatusText.Text = "Failed to load skeleton " + skel_name;
                        obj_d1.SetSkeletonSkin(null, null);
                        return;
                    }
                    skel = SFResourceManager.Skeletons.Get(skel_name);
                    StatusText.Text = "Loaded skeleton " + skel_name;

                    List<string> anims = GetAllSkeletonAnimations(skel);
                    foreach (string n in anims)
                        ListAnimations.Items.Add(n);
                }

                obj_d1.SetSkeletonSkin(skel, skin);
                obj_d1.SetAnimation(null, false);
            }

            if (ComboBrowseMode.SelectedIndex == 3)
            {
                if (ListEntries.SelectedIndex != -1)
                {
                    string s_n = ListEntries.SelectedItem.ToString();
                    s_n = s_n.Substring(0, s_n.Length - 4);

                    int result = SFResourceManager.Musics.Load(s_n);
                    if ((result != 0)&&(result != -1))
                    {
                        StatusText.Text = "Failed to load music " + s_n;
                        return;
                    }

                    sound_engine.UnloadSound();

                    sound_engine.LoadSoundMP3(SFResourceManager.Musics.Get(s_n));
                    StatusText.Text = "Loaded music " + s_n;
                    UpdateSliderSound();
                }
            }

            if (ComboBrowseMode.SelectedIndex == 4)
            {
                if (ListEntries.SelectedIndex != -1)
                {
                    string s_n = ListEntries.SelectedItem.ToString();
                    s_n = s_n.Substring(0, s_n.Length - 4);

                    int result = SFResourceManager.Sounds.Load(s_n);
                    if ((result != 0) && (result != -1))
                    {
                        StatusText.Text = "Failed to load sound " + s_n;
                        return;
                    }

                    sound_engine.UnloadSound();

                    sound_engine.LoadSoundWAV(SFResourceManager.Sounds.Get(s_n));
                    StatusText.Text = "Loaded sound " + s_n;
                    UpdateSliderSound();
                }
            }

            if (ComboBrowseMode.SelectedIndex == 5)
            {
                if (ListEntries.SelectedIndex != -1)
                {
                    string s_n = ListEntries.SelectedItem.ToString();
                    string type = s_n.Substring(s_n.Length - 4, 4);
                    s_n = s_n.Substring(0, s_n.Length - 4);

                    int result = SFResourceManager.Messages.Load(s_n);
                    if ((result != 0) && (result != -1))
                    {
                        StatusText.Text = "Failed to load message " + s_n;
                        return;
                    }
                    
                    sound_engine.UnloadSound();

                    if (type == ".wav")
                        sound_engine.LoadSoundWAV(SFResourceManager.Messages.Get(s_n));
                    else if (type == ".mp3")
                        sound_engine.LoadSoundMP3(SFResourceManager.Messages.Get(s_n));
                    else
                    {
                        StatusText.Text = "Failed to load message " + s_n;
                        return;
                    }
                    StatusText.Text = "Loaded message " + s_n;
                    UpdateSliderSound();
                }
            }

            update_render = true;
            dynamic_render = false;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    arrows_pressed[0] = true;
                    return true;
                case Keys.Right:
                    arrows_pressed[1] = true;
                    return true;
                case Keys.Up:
                    arrows_pressed[2] = true;
                    return true;
                case Keys.Down:
                    arrows_pressed[3] = true;
                    return true;
                default:
                    return base.ProcessDialogKey(keyData);
            }
        }

        protected override bool ProcessKeyPreview(ref Message msg)
        {
            if (msg.Msg == 0x101)
            {
                if ((int)msg.WParam == 0x25)      // left
                    arrows_pressed[0] = false;
                else if ((int)msg.WParam == 0x27) // right
                    arrows_pressed[1] = false;
                else if ((int)msg.WParam == 0x26) // up
                    arrows_pressed[2] = false;
                else if ((int)msg.WParam == 0x28) // down
                    arrows_pressed[3] = false;
            }
            return base.ProcessKeyPreview(ref msg);
        }

        private void ListAnimations_SelectedIndexChanged(object sender, EventArgs e)
        {
            SFScene scene = SFRenderEngine.scene;

            if (ComboBrowseMode.SelectedIndex == 1)
            {
                SceneNodeAnimated obj_d1 = scene.root.FindNode<SceneNodeAnimated>("dynamic_mesh");

                if (ListAnimations.SelectedIndex != -1)
                {
                    string anim_name = ListAnimations.SelectedItem.ToString();
                    anim_name = anim_name.Substring(0, anim_name.Length - 4);
                    int result = SFResourceManager.Animations.Load(anim_name);
                    if ((result != 0)&&(result != -1))
                    {
                        StatusText.Text = "Failed to load animation " + anim_name + ", status code " + result.ToString();
                        dynamic_render = false;
                        return;
                    }
                    if(SFResourceManager.Animations.Get(anim_name).bone_count != obj_d1.Skeleton.bone_count)
                    {
                        StatusText.Text = "Invalid animation "+anim_name;
                        dynamic_render = false;
                        return;
                    }

                    obj_d1.SetAnimation(SFResourceManager.Animations.Get(anim_name), true);
                    scene.SetSceneTime(0f);
                    scene.scene_meta.duration = obj_d1.Animation.max_time;
                    StatusText.Text = "Loaded animation " + anim_name;
                    UpdateSliderAnimation();
                    dynamic_render = true;
                    statusStrip1.Refresh();
                }
            }

            if (ComboBrowseMode.SelectedIndex == 2)
            {
                if (ListAnimations.SelectedIndex != -1)
                {
                    string anim_name = ListAnimations.SelectedItem.ToString();
                    anim_name = anim_name.Substring(0, anim_name.Length - 4);
                    int result = SFResourceManager.Animations.Load(anim_name);
                    if ((result != 0) && (result != -1))
                    {
                        StatusText.Text = "Failed to load animation " + anim_name + ", status code " + result.ToString();
                        dynamic_render = false;
                        return;
                    }

                    SceneNode obj = SFRenderEngine.scene.root.FindNode<SceneNode>("unit");
                    if (obj != null)
                    {
                        foreach(SceneNodeAnimated node in obj.Children)
                        {
                            if(node.Skeleton.bone_count != SFResourceManager.Animations.Get(anim_name).bone_count)
                            {
                                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFAssetManagerForm.ListAnimations_SelectedIndexChanged(): invalid bone count!");
                                StatusText.Text = "Invalid animation " + anim_name;
                                dynamic_render = false;
                                return;
                            }
                            node.SetAnimation(SFResourceManager.Animations.Get(anim_name), true);
                            scene.scene_meta.duration = node.Animation.max_time;
                        }
                    }

                    scene.SetSceneTime(0f);
                    StatusText.Text = "Loaded animation " + anim_name;
                    UpdateSliderAnimation();
                    dynamic_render = true;
                    statusStrip1.Refresh();

                }
            }

            update_render = true;
        }

        private void UpdateSliderSound()
        {
            double ratio = sound_engine.GetSoundPosition() / sound_engine.GetSoundDuration();
            trackSoundDuration.Value = Math.Max(0, Math.Min(trackSoundDuration.Maximum, (int)(trackSoundDuration.Maximum * ratio)));
            TimeSpan cur = TimeSpan.FromMilliseconds(sound_engine.GetSoundPosition());
            TimeSpan tot = TimeSpan.FromMilliseconds(sound_engine.GetSoundDuration());
            labelSoundDuration.Text = cur.ToString(@"m\:ss") + "/" + tot.ToString(@"m\:ss");
        }

        private void UpdateSliderAnimation()
        {
            double ratio = SFRenderEngine.scene.current_time / SFRenderEngine.scene.scene_meta.duration; //sound_engine.GetSoundPosition() / sound_engine.GetSoundDuration();
            trackSoundDuration.Value = Math.Max(0, Math.Min(trackSoundDuration.Maximum, (int)(trackSoundDuration.Maximum * ratio)));
            TimeSpan cur = TimeSpan.FromSeconds(SFRenderEngine.scene.current_time);
            TimeSpan tot = TimeSpan.FromSeconds(SFRenderEngine.scene.scene_meta.duration);
            labelSoundDuration.Text = cur.ToString(@"m\:ss") + "/" + tot.ToString(@"m\:ss");
        }

        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            bool update_ui = false;

            // moving camera using mouse
            if (mouse_pressed)
            {
                Vector2 scroll_mouse_end = new Vector2(Cursor.Position.X, Cursor.Position.Y);
                Vector2 scroll_translation = (scroll_mouse_end - scroll_mouse_start) * SFRenderEngine.scene.DeltaTime / 250f;

                SFRenderEngine.scene.camera.Direction += new Vector2(scroll_translation.X, -scroll_translation.Y);

                update_render = true;
                update_ui = true;
            }

            // moving camera using arrows
            float cam_speed = 2;

            Vector3 forward = (SFRenderEngine.scene.camera.Lookat - SFRenderEngine.scene.camera.Position).Normalized();
            Vector3 up = new Vector3(0, 1, 0);
            Vector3 right = Vector3.Cross(forward, up).Normalized();
            float speed_factor = cam_speed * SFRenderEngine.scene.DeltaTime;
            Vector3 movement_vector = new Vector3(0, 0, 0);
            if (arrows_pressed[0])
                movement_vector -= right;
            if (arrows_pressed[1])
                movement_vector += right;
            if (arrows_pressed[2])
                movement_vector += forward;
            if (arrows_pressed[3])
                movement_vector -= forward;

            movement_vector *= cam_speed * speed_factor;

            if(movement_vector != new Vector3(0, 0, 0))
            {
                SFRenderEngine.scene.camera.translate(movement_vector);
                update_render = true;
                update_ui = true;
            }

            if (update_render)
            {
                SFRenderEngine.scene.Update();
                glControl1.Invalidate();
                update_render = false;
            }

            if (dynamic_render)
            {
                UpdateSliderAnimation();
                update_render = true;
            }

            if (!update_ui)
                SFRenderEngine.scene.StopTimeFlow();
            else
                SFRenderEngine.scene.ResumeTimeFlow();

            TimerAnimation.Start();
        }

        private List<string> GetAllSkeletonAnimations(SFSkeleton skel)
        {
            if(skel == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFAssetManagerForm.GetAllSkeletonAnimations(): Skeleton is null!");
                return new List<string>();
            }
            string skel_name = skel.GetName();
            List<string> anims = new List<string>();

            //do the following:
            //cut skel_name to the last underscore
            //find all anims that have that prefix
            //cut until there's at least one anim
            while((anims.Count == 0)&&(skel_name != ""))
            {
                foreach(string anim_name in SFResourceManager.animation_names)
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
            mouse_pressed = true;
            scroll_mouse_start = new Vector2(Cursor.Position.X, Cursor.Position.Y);
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            scroll_mouse_start = new Vector2(0, 0);
            mouse_pressed = false;
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouse_pressed)
                return;
        }

        public void GenerateScene(int cat, int elem)
        {
            if (!synchronized)
                return;

            ListEntries.Items.Clear();
            ListAnimations.Items.Clear();
            ResetScene();
            SFResourceManager.DisposeAll();
            GC.Collect(2, GCCollectionMode.Forced, false);
            
            SFRenderEngine.scene.CatElemToScene(cat, elem);

            if (SFRenderEngine.scene.scene_meta.is_animated)   // only if selected element is a unit...
            {
                SceneNode unit_node = SFRenderEngine.scene.root.FindNode<SceneNode>("unit");
                if (unit_node != null)
                {
                    if (unit_node.Children.Count != 0)
                    {
                        ListAnimations.Items.Clear();
                        SceneNodeAnimated chest_node = unit_node.FindNode<SceneNodeAnimated>("Chest");
                        if (chest_node != null)
                        {
                            List<string> anims = GetAllSkeletonAnimations(chest_node.Skeleton);
                            foreach (string n in anims)
                                ListAnimations.Items.Add(n);
                        }
                    }
                }
            }

            update_render = true;
        }

        private void buttonSoundPlay_Click(object sender, EventArgs e)
        {
            if(ComboBrowseMode.SelectedIndex == 1)
            {
                dynamic_render = true;
            }
            if(ComboBrowseMode.SelectedIndex == 2)
            {
                if (SFRenderEngine.scene.scene_meta.is_animated)
                    dynamic_render = true;
            }
            if ((ComboBrowseMode.SelectedIndex == 3)||(ComboBrowseMode.SelectedIndex == 4)||(ComboBrowseMode.SelectedIndex == 5))
            {
                if (!sound_engine.loaded)
                    return;
                sound_engine.PlaySound();
                TimerSoundDuration.Start();
            }
        }

        private void buttonSoundStop_Click(object sender, EventArgs e)
        {
            if (ComboBrowseMode.SelectedIndex == 1)
            {
                dynamic_render = false;
            }
            if (ComboBrowseMode.SelectedIndex == 2)
            {
                if (SFRenderEngine.scene.scene_meta.is_animated)
                    dynamic_render = false;
            }
            if ((ComboBrowseMode.SelectedIndex == 3) || (ComboBrowseMode.SelectedIndex == 4) || (ComboBrowseMode.SelectedIndex == 5))
            {
                if (!sound_engine.loaded)
                    return;
                sound_engine.PauseSound();
                TimerSoundDuration.Stop();
            }
        }

        private void TimerSoundDuration_Tick(object sender, EventArgs e)
        {
            //set slider
            UpdateSliderSound();

            TimerSoundDuration.Start();
        }

        private void trackSoundDuration_Scroll(object sender, EventArgs e)
        {
            double ratio = trackSoundDuration.Value / (double)trackSoundDuration.Maximum;

            if (ComboBrowseMode.SelectedIndex == 1)
            {
                SFRenderEngine.scene.SetSceneTime((float)ratio * SFRenderEngine.scene.scene_meta.duration);
                update_render = true;
            }
            if(ComboBrowseMode.SelectedIndex == 2)
            {
                if (!SFRenderEngine.scene.scene_meta.is_animated)
                    return;
                SFRenderEngine.scene.SetSceneTime((float)ratio * SFRenderEngine.scene.scene_meta.duration);
                update_render = true;
            }
            if ((ComboBrowseMode.SelectedIndex == 3) || (ComboBrowseMode.SelectedIndex == 4) || (ComboBrowseMode.SelectedIndex == 5))
            {
                if (!sound_engine.loaded)
                    return;
                sound_engine.SetSound(ratio * sound_engine.GetSoundDuration());
                if (sound_engine.GetSoundStatus() == NAudio.Wave.PlaybackState.Playing)
                    TimerSoundDuration.Start();
            }
        }

        private void button1Extract_Click(object sender, EventArgs e)
        {
            if (ListEntries.SelectedItem == null)
                return;
            string item = ListEntries.SelectedItem.ToString();
            if (item == "")
                return;
            item = item.Substring(0, item.Length - 4);

            if(ComboBrowseMode.SelectedIndex == 0)
            {
                SFModel3D mod = SFResourceManager.Models.Get(item);
                if (mod == null)
                    return;

                SFResourceManager.Models.Extract(item);
                List<string> tx = SFResourceManager.Textures.GetNames();
                foreach (string t in tx)
                    foreach (SFSubModel3D sbm in mod.submodels)
                        if (sbm.material.texture == SFResourceManager.Textures.Get(t))
                            SFResourceManager.Textures.Extract(t);

                StatusText.Text = "Extracted " + item;
            }

            if(ComboBrowseMode.SelectedIndex == 1)
            {
                SFModelSkin mod = SFResourceManager.Skins.Get(item);
                if (mod == null)
                    return;
                SceneNodeAnimated obj = SFRenderEngine.scene.root.FindNode<SceneNodeAnimated>("dynamic_mesh");

                SFResourceManager.Skins.Extract(item);
                List<string> skels = SFResourceManager.Skeletons.GetNames();
                foreach (string skel in skels)
                    if(obj.Skeleton == SFResourceManager.Skeletons.Get(skel))
                    {
                        SFResourceManager.Skeletons.Extract(skel);
                        SFResourceManager.BSIs.Extract(skel);
                        SFResourceManager.Skins.Extract(skel);
                        
                        List<string> tx = SFResourceManager.Textures.GetNames();
                        foreach (string t in tx)
                            foreach (SFModelSkinChunk ch in obj.Skin.submodels)
                            {
                                SFMaterial mat = ch.material;
                                if (mat.texture == SFResourceManager.Textures.Get(t))
                                    SFResourceManager.Textures.Extract(t);
                            }

                        break;
                    }
                StatusText.Text = "Extracted " + item;
            }

            if (ComboBrowseMode.SelectedIndex == 3)
            {
                StreamResource s = SFResourceManager.Musics.Get(item);
                if (s == null)
                    return;

                SFResourceManager.Musics.Extract(item);

                StatusText.Text = "Extracted " + item;
            }

            if (ComboBrowseMode.SelectedIndex == 4)
            {
                StreamResource s = SFResourceManager.Sounds.Get(item);
                if (s == null)
                    return;

                SFResourceManager.Sounds.Extract(item);

                StatusText.Text = "Extracted " + item;
            }

            if (ComboBrowseMode.SelectedIndex == 5)
            {
                StreamResource s = SFResourceManager.Messages.Get(item);
                if (s == null)
                    return;

                SFResourceManager.Messages.Extract(item);

                StatusText.Text = "Extracted " + item;
            }
        }

        private void button2Extract_Click(object sender, EventArgs e)
        {
            if (ListAnimations.SelectedItem == null)
                return;
            string item = ListAnimations.SelectedItem.ToString();
            if (item == "")
                return;
            item = item.Substring(0, item.Length - 4);

            if((ComboBrowseMode.SelectedIndex == 1)||(ComboBrowseMode.SelectedIndex == 2))
            {
                SFAnimation anim = SFResourceManager.Animations.Get(item);
                if (anim == null)
                    return;

                SFResourceManager.Animations.Extract(item);

                StatusText.Text = "Extracted " + item;
            }
        }

        private void comboMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBrowseMode.SelectedIndex != 5)
                return;

            if(comboMessages.SelectedItem == null)
                return;
            string item = comboMessages.SelectedItem.ToString();
            if (item == "")
                return;

            if (item == "RTS Battle")
                SFResourceManager.Messages.SetPrefixPath("sound\\speech\\battle");
            else if (item == "Male")
                SFResourceManager.Messages.SetPrefixPath("sound\\speech\\male");
            else if (item == "Female")
                SFResourceManager.Messages.SetPrefixPath("sound\\speech\\female");
            else if (item == "RTS Workers")
                SFResourceManager.Messages.SetPrefixPath("sound\\speech\\messages");
            else if (item == "NPC")
                SFResourceManager.Messages.SetPrefixPath("sound\\speech");
            else
                return;

            ListEntries.Items.Clear();
            foreach (string s in SFResourceManager.message_names[item])
                ListEntries.Items.Add(s);
        }

        // failsafe for the case map is unloaded and viewer is opened
        public void ResetScene()
        {
            SFRenderEngine.scene.RemoveSceneNode(SFRenderEngine.scene.root, true);
            SFRenderEngine.scene.root.Visible = true;
            SFRenderEngine.scene.camera.SetParent(SFRenderEngine.scene.root);
        }
    }
}
