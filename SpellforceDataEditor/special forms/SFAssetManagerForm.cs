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
        SFRenderEngine render_engine;
        SFSoundEngine sound_engine;

        bool synchronized = false;

        Point mouse_pos;
        bool mouse_pressed = false;

        bool ready = false;

        public SFAssetManagerForm()
        {
            render_engine = new SFRenderEngine();
            sound_engine = new SFSoundEngine();
            InitializeComponent();
        }

        //specify game directory button
        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (GameDirDialog.ShowDialog() == DialogResult.OK)
            {
                int result = specify_game_directory(GameDirDialog.SelectedPath);
                if (result == 0)
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
            int result = SFUnPak.SFUnPak.SpecifyGameDirectory(dname); //render_engine.SpecifyGameDirectory(dname);
            if(result == 0)
            {
                ready = true;
                SFResourceManager.FindAllMeshes();
                SFLua.SFLuaEnvironment.Init();
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
            render_engine.scene_manager.gamedata = SFCFF.SFCategoryManager.gamedata;

            glControl1.Invalidate();
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
            SFResourceManager.DisposeAll();
            sound_engine.UnloadSound();
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
            if (!ready)
                return;

            DisableAnimation();
            ListEntries.Items.Clear();
            ListAnimations.Items.Clear();
            render_engine.scene_manager.ClearScene();
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
                SFSceneDescription scene = new SFSceneDescription();
                scene.add_line(SCENE_ITEM_TYPE.OBJ_SIMPLE, new string[] { "", "", "simple_mesh" });
                render_engine.scene_manager.ParseSceneDescription(scene);

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
                SFSceneDescription scene = new SFSceneDescription();
                scene.add_line(SCENE_ITEM_TYPE.OBJ_ANIMATED, new string[] { "", "", "dynamic_mesh" });
                render_engine.scene_manager.ParseSceneDescription(scene);

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
            if (!ready)
                return;

            SFResourceManager.DisposeAll();

            SFSceneManager scene = render_engine.scene_manager;

            if (ComboBrowseMode.SelectedIndex == 0)
            {
                ObjectSimple3D obj_s1 = scene.objects_static["simple_mesh"];
                obj_s1.Mesh = null;

                if (ListEntries.SelectedIndex != -1)
                {
                    string model_name = ListEntries.SelectedItem.ToString();
                    model_name = model_name.Substring(0, model_name.Length - 4);
                    int result = SFResourceManager.Models.Load(model_name);
                    if(result != 0)
                    {
                        StatusText.Text = "Failed to load model " + model_name;
                        return;
                    }
                    obj_s1.Mesh = SFResourceManager.Models.Get(model_name);
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
                    int result = SFResourceManager.Skins.Load(skin_name);
                    if (result != 0)
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
                    if (result != 0)
                    {
                        StatusText.Text = "Failed to load skeleton " + skel_name;
                        obj_d1.SetSkeletonSkin(null, null);
                        return;
                    }
                    skel = SFResourceManager.Skeletons.Get(skel_name);
                    StatusText.Text = "Loaded skeleton " + skel_name;

                    List<string> anims = GetAllSkeletonAnimations(skel_name);
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
                    if (result != 0)
                    {
                        StatusText.Text = "Failed to load music " + s_n;
                        return;
                    }

                    sound_engine.UnloadSound();

                    sound_engine.LoadSoundMP3(SFResourceManager.Musics.Get(s_n));
                    StatusText.Text = "Loaded music " + s_n;
                }
            }

            if (ComboBrowseMode.SelectedIndex == 4)
            {
                if (ListEntries.SelectedIndex != -1)
                {
                    string s_n = ListEntries.SelectedItem.ToString();
                    s_n = s_n.Substring(0, s_n.Length - 4);

                    int result = SFResourceManager.Sounds.Load(s_n);
                    if (result != 0)
                    {
                        StatusText.Text = "Failed to load sound " + s_n;
                        return;
                    }

                    sound_engine.UnloadSound();

                    sound_engine.LoadSoundWAV(SFResourceManager.Sounds.Get(s_n));
                    StatusText.Text = "Loaded sound " + s_n;
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
                    if (result != 0)
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
                }
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
            Vector3 cam_move = (((render_engine.camera.Lookat - render_engine.camera.Position).Normalized())*cam_speed)/render_engine.scene_manager.frames_per_second;

            if (e.KeyChar == 'w')
                render_engine.camera.translate(cam_move);
            else if (e.KeyChar == 's')
                render_engine.camera.translate(-cam_move);
        }


        private void ListAnimations_SelectedIndexChanged(object sender, EventArgs e)
        {
            SFSceneManager scene = render_engine.scene_manager;

            if (ComboBrowseMode.SelectedIndex == 1)
            {
                objectAnimated obj_d1 = scene.objects_dynamic["dynamic_mesh"];

                if (ListAnimations.SelectedIndex != -1)
                {
                    string anim_name = ListAnimations.SelectedItem.ToString();
                    anim_name = anim_name.Substring(0, anim_name.Length - 4);
                    int result = SFResourceManager.Animations.Load(anim_name);
                    if ((result != 0)&&(result != -1))
                    {
                        
                        StatusText.Text = "Failed to load animation " + anim_name + ", status code " + result.ToString();
                        DisableAnimation();
                        return;
                    }
                    if(SFResourceManager.Animations.Get(anim_name).bone_count != obj_d1.skeleton.bone_count)
                    {
                        StatusText.Text = "Invalid animation "+anim_name;
                        DisableAnimation();
                        return;
                    }

                    obj_d1.SetAnimation(SFResourceManager.Animations.Get(anim_name), true);
                    scene.SetSceneTime(0f);
                    scene.scene_meta.duration = obj_d1.animation.max_time;
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
                    int result = SFResourceManager.Animations.Load(anim_name);
                    if ((result != 0) && (result != -1))
                    {

                        StatusText.Text = "Failed to load animation " + anim_name + ", status code " + result.ToString();
                        DisableAnimation();
                        return;
                    }

                    foreach (KeyValuePair<string, string> kv in scene.scene_meta.obj_to_anim)
                    {
                        if (SFResourceManager.Animations.Get(anim_name).bone_count != scene.objects_dynamic[kv.Key].skeleton.bone_count)
                        {
                            StatusText.Text = "Invalid animation " + anim_name;
                            DisableAnimation();
                            return;
                        }
                        scene.objects_dynamic[kv.Key].SetAnimation(SFResourceManager.Animations.Get(anim_name), true);
                        scene.scene_meta.duration = scene.objects_dynamic[kv.Key].animation.max_time;
                    }

                    scene.SetSceneTime(0f);
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
            TimerAnimation.Interval = 1000/render_engine.scene_manager.frames_per_second;
            TimerAnimation.Start();
            render_engine.scene_manager.delta_timer.Restart();
        }

        private void DisableAnimation()
        {
            TimerAnimation.Stop();
            TimerAnimation.Enabled = false;
            render_engine.scene_manager.delta_timer.Stop();
        }

        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            if (render_engine.scene_manager.scene_meta == null)
                return;
            //set slider

            double ratio = render_engine.scene_manager.current_time / render_engine.scene_manager.scene_meta.duration; //sound_engine.GetSoundPosition() / sound_engine.GetSoundDuration();
            trackSoundDuration.Value = Math.Max(0, Math.Min(trackSoundDuration.Maximum, (int)(trackSoundDuration.Maximum * ratio)));
            TimeSpan cur = TimeSpan.FromSeconds(render_engine.scene_manager.current_time);
            TimeSpan tot = TimeSpan.FromSeconds(render_engine.scene_manager.scene_meta.duration);
            labelSoundDuration.Text = cur.ToString(@"m\:ss") + "/" + tot.ToString(@"m\:ss");

            TimerAnimation.Start();
            render_engine.scene_manager.LogicStep();
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
            SFResourceManager.DisposeAll();
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

        private void buttonSoundPlay_Click(object sender, EventArgs e)
        {
            if(ComboBrowseMode.SelectedIndex == 1)
            {
                EnableAnimation();
            }
            if(ComboBrowseMode.SelectedIndex == 2)
            {
                if (render_engine.scene_manager.scene_meta.is_animated)
                    EnableAnimation();
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
                DisableAnimation();
            }
            if (ComboBrowseMode.SelectedIndex == 2)
            {
                if (render_engine.scene_manager.scene_meta.is_animated)
                    DisableAnimation();
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
            double ratio = sound_engine.GetSoundPosition() / sound_engine.GetSoundDuration();
            trackSoundDuration.Value = Math.Max(0, Math.Min(trackSoundDuration.Maximum, (int)(trackSoundDuration.Maximum * ratio)));
            TimeSpan cur = TimeSpan.FromMilliseconds(sound_engine.GetSoundPosition());
            TimeSpan tot = TimeSpan.FromMilliseconds(sound_engine.GetSoundDuration());
            labelSoundDuration.Text = cur.ToString(@"m\:ss") + "/" + tot.ToString(@"m\:ss");

            TimerSoundDuration.Start();
        }

        private void trackSoundDuration_Scroll(object sender, EventArgs e)
        {
            double ratio = trackSoundDuration.Value / (double)trackSoundDuration.Maximum;

            if (ComboBrowseMode.SelectedIndex == 1)
            {
                render_engine.scene_manager.SetSceneTime((float)ratio * render_engine.scene_manager.scene_meta.duration);
                render_engine.scene_manager.LogicStep(false);
                glControl1.Invalidate();
            }
            if(ComboBrowseMode.SelectedIndex == 2)
            {
                if (!render_engine.scene_manager.scene_meta.is_animated)
                    return;
                render_engine.scene_manager.SetSceneTime((float)ratio * render_engine.scene_manager.scene_meta.duration);
                render_engine.scene_manager.LogicStep(false);
                glControl1.Invalidate();
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
                    foreach (SFMaterial mat in mod.materials)
                        if (mat.texture == SFResourceManager.Textures.Get(t))
                            SFResourceManager.Textures.Extract(t);

                StatusText.Text = "Extracted " + item;
            }

            if(ComboBrowseMode.SelectedIndex == 1)
            {
                SFModelSkin mod = SFResourceManager.Skins.Get(item);
                if (mod == null)
                    return;

                objectAnimated obj = render_engine.scene_manager.objects_dynamic["dynamic_mesh"];

                SFResourceManager.Skins.Extract(item);
                List<string> skels = SFResourceManager.Skeletons.GetNames();
                foreach (string skel in skels)
                    if(obj.skeleton == SFResourceManager.Skeletons.Get(skel))
                    {
                        SFResourceManager.Skeletons.Extract(skel);
                        SFResourceManager.BSIs.Extract(skel);
                        SFResourceManager.Skins.Extract(skel);
                        
                        List<string> tx = SFResourceManager.Textures.GetNames();
                        foreach (string t in tx)
                            foreach (SFModelSkinChunk ch in obj.skin.submodels)
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
                SFResourceManager.Messages.SetSuffixExtension(".wav");
            else
                SFResourceManager.Messages.SetSuffixExtension(".mp3");

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
            render_engine.scene_manager.ClearScene();
        }
    }
}
