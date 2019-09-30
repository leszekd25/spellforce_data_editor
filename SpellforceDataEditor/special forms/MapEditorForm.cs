using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;
using System.IO;
using SpellforceDataEditor.SFMap;
using SpellforceDataEditor.SF3D.SFRender;
using SpellforceDataEditor.SFMap.MapEdit;

namespace SpellforceDataEditor.special_forms
{
    public enum MapEditMainMode { TERRAIN, TEXTURES, ENTITIES, MISC };
    public enum MapEditTerrainMode { HEIGHTMAP, FLAGS, LAKES };

    public partial class MapEditorForm : Form
    {
        SFMap.SFMap map = null;
        SFCFF.SFGameData gamedata = null;

        bool dynamic_render = false;     // if true, window will redraw every frame at 25 fps
        bool mouse_pressed = false;      // if true, mouse is pressed and in render window
        MouseButtons mouse_last_pressed = MouseButtons.Left;  // last mouse button pressed

        bool mouse_on_view = false;      // if true, mouse is in render window
        Vector2 scroll_mouse_start = new Vector2(0, 0);
        bool mouse_scroll = false;
        float zoom_level = 1.0f;

        bool[] arrows_pressed = new bool[] { false, false, false, false };  // left, right, up, down
        bool[] rotation_pressed = new bool[] { false, false, false, false };// left, right, up, down

        public bool update_render = false;                // whenever this is true, window will be repainted, and this switched to false
        int gc_timer = 0;                // when this reaches 200, garbage collector runs

        OpenTK.GLControl RenderWindow = null;

        public SFMap.MapEdit.MapEditor selected_editor { get; private set; } = new SFMap.MapEdit.MapHeightMapEditor();
        public SFMap.map_controls.MapInspector selected_inspector { get; private set; } = null;

        MapEditMainMode main_mode = MapEditMainMode.TERRAIN;
        MapEditTerrainMode terrain_mode = MapEditTerrainMode.HEIGHTMAP;
        MapBrush terrain_brush = new MapBrush();

        SFMap.map_dialog.MapAutoTextureDialog autotexture_form = null;
        SFMap.map_dialog.MapManageTeamCompositions teamcomp_form = null;
        SFMap.map_dialog.MapVisibilitySettings visibility_form = null;

        List<int> unitcombo_to_unitindex = new List<int>();

        public MapEditorForm()
        {
            InitializeComponent();

            SFLua.SFLuaEnvironment.LoadSQL(false);
            if (!SFLua.SFLuaEnvironment.data_loaded)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "MapEditorForm(): Failed to load SQL data!");
                Close();
                return;
            }

            TimerAnimation.Enabled = true;
            TimerAnimation.Interval = 1000 / SFRenderEngine.scene.frames_per_second;
            TimerAnimation.Start();

            gamedata = SFCFF.SFCategoryManager.gamedata;
        }

        private void MapEditorForm_Load(object sender, EventArgs e)
        {
            LogUtils.Log.MemoryUsage();
        }

        private void MapEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseMap() != 0)
                e.Cancel = true;
        }

        private void MapEditorForm_Resize(object sender, EventArgs e)
        {
            TabEditorModes.Width = this.Width - 22;
            TabEditorModes.Padding = new Point(((this.Width - 350) / TabEditorModes.TabPages.Count / 2), TabEditorModes.Padding.Y);
            if (RenderWindow != null)
                ResizeWindow();
        }

        private void createNewMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseMap() == 0)
                CreateMap();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseMap() == 0)
                LoadMap();
        }

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMap();
        }

        private void closeMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseMap();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int CreateMap()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "MapEditorForm.CreateMap() called");

            // get new map parameters
            ushort map_size = 0;
            SFMap.MapGen.MapGenerator generator = null;
            SFMap.map_dialog.MapPromptNewMap newmap = new SFMap.map_dialog.MapPromptNewMap();
            if (newmap.ShowDialog() == DialogResult.OK)
            {
                map_size = newmap.MapSize;
                if (newmap.use_generator)
                {
                    newmap.UpdateGenerator();
                    generator = newmap.generator;
                }
            }
            else
                return -1;

            // close current gamedata
            if (MainForm.data != null)
            {
                if (MainForm.data.data_loaded)
                {
                    if (!MainForm.data.synchronized_with_mapeditor)
                    {
                        if (MainForm.data.close_data() == DialogResult.Cancel)
                        {
                            StatusText.Text = "Could not load game data: Another game data is already loaded, which will cause desync!";
                            return -1;
                        }
                    }
                }
            }
            else if (SFCFF.SFCategoryManager.ready)
                SFCFF.SFCategoryManager.UnloadAll();

            // load in gamedata from game directory
            StatusText.Text = "Loading GameData.cff...";
            StatusText.GetCurrentParent().Refresh();
            if (gamedata.Load(SFUnPak.SFUnPak.game_directory_name + "\\data\\GameData.cff") != 0)
            {
                StatusText.Text = "Failed to load gamedata";
                return -2;
            }

            if (MainForm.data != null)
                MainForm.data.mapeditor_set_gamedata(gamedata);
            else
                SFCFF.SFCategoryManager.manual_SetGamedata(gamedata);

            // display init
            SFRenderEngine.scene.Init();
            CreateRenderWindow();
            InspectorHide();

            // create and generate map
            map = new SFMap.SFMap();
            map.CreateDefault(map_size, generator, gamedata, StatusText);

            SFRenderEngine.scene.heightmap = map.heightmap;
            selected_editor.map = map;
            InitEditorMode();

            map.selection_helper.SetCursorPosition(new SFCoord(1, 1));
            map.selection_helper.SetCursorVisibility(true);

            SetCameraViewPoint(new SFCoord(map.width / 2, map.height / 2));
            ResetCamera();

            RenderWindow.Invalidate();

            if (MainForm.data != null)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "MapEditorForm.CreateMap(): Synchronized with gamedata editor");
                MessageBox.Show("Note: Editor now operates on gamedata file in your Spellforce directory. Modifying in-editor gamedata and saving results will result in permanent change to your gamedata in your Spellforce directory.");
            }

            this.Text = "Map Editor - new map";

            LogUtils.Log.MemoryUsage();
            return 0;
        }

        private int LoadMap()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "MapEditorForm.LoadMap() called");

            if (OpenMap.ShowDialog() == DialogResult.OK)
            {
                // check if gamedata is open in the editor and prompt to close it
                if (MainForm.data != null)
                {
                    if (MainForm.data.data_loaded)
                    {
                        if (!MainForm.data.synchronized_with_mapeditor)
                        {
                            if (MainForm.data.close_data() == DialogResult.Cancel)
                            {
                                StatusText.Text = "Could not load game data: Another game data is already loaded, which will cause desync!";
                                return -1;
                            }
                        }
                    }
                }
                else if (SFCFF.SFCategoryManager.ready)
                    SFCFF.SFCategoryManager.UnloadAll();

                StatusText.Text = "Loading GameData.cff...";
                StatusText.GetCurrentParent().Refresh();
                if (gamedata.Load(SFUnPak.SFUnPak.game_directory_name + "\\data\\GameData.cff") != 0)
                {
                    StatusText.Text = "Failed to load gamedata";
                    return -2;
                }

                if (MainForm.data != null)
                    MainForm.data.mapeditor_set_gamedata(gamedata);
                else
                    SFCFF.SFCategoryManager.manual_SetGamedata(gamedata);

                SFRenderEngine.scene.Init();
                CreateRenderWindow();
                InspectorHide();

                map = new SFMap.SFMap();
                try
                {
                    if (map.Load(OpenMap.FileName, gamedata, StatusText) != 0)
                    {
                        StatusText.Text = "Failed to load map";
                        DestroyRenderWindow();
                        return -3;
                    }
                }
                catch (InvalidDataException)
                {
                    StatusText.Text = "Map contains invalid data!";
                    DestroyRenderWindow();
                    return -4;
                }

                SFRenderEngine.scene.heightmap = map.heightmap;
                selected_editor.map = map;
                InitEditorMode();

                map.selection_helper.SetCursorPosition(new SFCoord(1, 1));
                map.selection_helper.SetCursorVisibility(true);

                SetCameraViewPoint(new SFCoord(map.width / 2, map.height / 2));
                ResetCamera();

                RenderWindow.Invalidate();

                if (MainForm.data != null)
                {
                    LogUtils.Log.Info(LogUtils.LogSource.SFMap, "MapEditorForm.LoadMap(): Synchronized with gamedata editor");
                    MessageBox.Show("Note: Editor now operates on gamedata file in your Spellforce directory. Modifying in-editor gamedata and saving results will result in permanent change to your gamedata in your Spellforce directory.");
                }

                this.Text = "Map Editor - " + OpenMap.FileName;

                LogUtils.Log.MemoryUsage();
                return 0;
            }

            return -5;
        }



        private DialogResult SaveMap()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "MapEditorForm.SaveMap() called");

            if (map == null)
                return DialogResult.No;


            DialogResult dr = DialogSaveMap.ShowDialog();

            if (dr == DialogResult.OK)
            {
                StatusText.Text = "Saving the map...";
                if (map.Save(DialogSaveMap.FileName) != 0)
                {
                    StatusText.Text = "Failed to save map";
                    return DialogResult.No;
                }
                StatusText.Text = DialogSaveMap.FileName + " saved successfully";
                if (MainForm.data != null)
                {
                    if (MainForm.data.data_changed)
                    {
                        MainForm.data.save_data();
                    }
                }
            }

            return dr;
        }



        private int CloseMap()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "MapEditorForm.CloseMap() called");

            if (map == null)
                return 0;

            Focus();

            DialogResult dr = MessageBox.Show(
                "Do you want to save the map before quitting? This will also overwrite gamedata if modified", "Save before quit?", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Cancel)
                return -1;
            else if (dr == DialogResult.Yes)
            {
                if (SaveMap() == DialogResult.Cancel)
                    return -2;
            }

            if (autotexture_form != null)
                autotexture_form.Close();
            if (teamcomp_form != null)
                teamcomp_form.Close();
            if (visibility_form != null)
                visibility_form.Close();

            TabEditorModes.Enabled = false;
            InspectorClear();

            map.Unload();
            if (MainForm.data != null)
            {
                MainForm.data.close_data();
                MainForm.data.mapeditor_desynchronize();
            }
            else
                SFCFF.SFCategoryManager.UnloadAll();
            SFRenderEngine.scene.RemoveSceneNode(SFRenderEngine.scene.root, true);
            SFRenderEngine.scene.root = null;
            SFRenderEngine.scene.camera = null;
            SFRenderEngine.scene.heightmap = null;
            foreach (SF3D.SFTexture tex in SFRenderEngine.scene.tex_entries_simple.Keys)
                SFRenderEngine.scene.tex_entries_simple[tex].Clear();
            SFRenderEngine.scene.tex_entries_simple.Clear();
            if (MainForm.viewer != null)
                MainForm.viewer.ResetScene();
            map = null;
            // for good measure (bad! bad!) (TODO: make this do nothing since all resources should be properly disposed at this point)                for (int i = 0; i < (int)MAPEDIT_MODE.MAX; i++)
            SFResources.SFResourceManager.DisposeAll();
            DestroyRenderWindow();
            this.Text = "Map Editor";
            GC.Collect();

            LogUtils.Log.MemoryUsage();

            return 0;
        }

        // moved from designer to code, experimenting with memory usage
        private void CreateRenderWindow()
        {
            if (RenderWindow != null)
                return;

            this.RenderWindow = new OpenTK.GLControl(new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(32), 24, 8, 4));
            this.RenderWindow.BackColor = System.Drawing.Color.Black;
            this.RenderWindow.Location = new System.Drawing.Point(3, TabEditorModes.Location.Y + TabEditorModes.Size.Height);
            this.RenderWindow.Name = "RenderWindow";
            this.RenderWindow.Size = new System.Drawing.Size(589, 589);
            this.RenderWindow.TabIndex = 2;
            this.RenderWindow.VSync = true;
            this.RenderWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.RenderWindow_Paint);
            this.RenderWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseDown);
            this.RenderWindow.MouseEnter += new System.EventHandler(this.RenderWindow_MouseEnter);
            this.RenderWindow.MouseLeave += new System.EventHandler(this.RenderWindow_MouseLeave);
            this.RenderWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseMove);
            this.RenderWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseUp);
            this.RenderWindow.MouseWheel += new MouseEventHandler(this.RenderWindow_MouseWheel);
            this.Controls.Add(this.RenderWindow);

            // it seems shaders must always be compiled upon creating new window
            SFRenderEngine.Initialize(new Vector2(RenderWindow.ClientSize.Width, RenderWindow.ClientSize.Height));

            ResizeWindow();
        }

        // after this is called, memory will be freed (?)
        private void DestroyRenderWindow()
        {
            if (RenderWindow == null)
                return;

            this.RenderWindow.Paint -= new System.Windows.Forms.PaintEventHandler(this.RenderWindow_Paint);
            this.RenderWindow.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseDown);
            this.RenderWindow.MouseEnter -= new System.EventHandler(this.RenderWindow_MouseEnter);
            this.RenderWindow.MouseLeave -= new System.EventHandler(this.RenderWindow_MouseLeave);
            this.RenderWindow.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseMove);
            this.RenderWindow.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseUp);
            this.RenderWindow.MouseWheel -= new MouseEventHandler(this.RenderWindow_MouseWheel);

            this.Controls.Remove(RenderWindow);
            this.RenderWindow.Dispose();
            this.RenderWindow = null;
        }

        private void RenderWindow_Paint(object sender, PaintEventArgs e)
        {
            RenderWindow.MakeCurrent();
            SFRenderEngine.RenderScene();
            RenderWindow.SwapBuffers();
        }

        private void RenderWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                scroll_mouse_start = new Vector2(Cursor.Position.X, Cursor.Position.Y);
                mouse_scroll = true;
                return;
            }
            mouse_pressed = true;
            mouse_last_pressed = e.Button;
        }

        private void RenderWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                scroll_mouse_start = new Vector2(0, 0);
                mouse_scroll = false;
                return;
            }
            mouse_pressed = false;
            selected_editor.OnMouseUp(e.Button);
            update_render = true;
        }

        private void RenderWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouse_on_view)
                return;
        }

        private void RenderWindow_MouseLeave(object sender, EventArgs e)
        {
            mouse_on_view = false;
            mouse_pressed = false;
        }

        private void RenderWindow_MouseEnter(object sender, EventArgs e)
        {
            mouse_on_view = true;
        }

        private void RenderWindow_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                zoom_level *= 1.1f;
                if (zoom_level > 6)
                    zoom_level = 6;
            }
            else if (e.Delta > 0)
            {
                zoom_level *= 0.9f;
                if (zoom_level < 0.1f)
                    zoom_level = 0.1f;
            }
            AdjustCameraZ();
            update_render = true;
        }

        private void EnableAnimation()
        {
            dynamic_render = true;
            SFRenderEngine.scene.delta_timer.Restart();
        }

        private void DisableAnimation()
        {
            dynamic_render = false;
            SFRenderEngine.scene.delta_timer.Stop();
        }

        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            if (map == null)
                return;

            // find point which mouse hovers at
            if (mouse_on_view)
            {
                // generate ray
                float px, py;
                px = Cursor.Position.X - this.Location.X - RenderWindow.Location.X;
                py = Cursor.Position.Y - this.Location.Y - RenderWindow.Location.Y - 29;
                float wx, wy;
                wx = px / RenderWindow.Size.Width; wx = (wx + 0.09f) * 0.84f;
                wy = py / RenderWindow.Size.Height; wy = (wy + 0.11f) * 0.84f;
                Vector3[] frustrum_vertices = SFRenderEngine.scene.camera.FrustumVertices;
                Vector3 r_start = SFRenderEngine.scene.camera.Position;
                Vector3 r_end = frustrum_vertices[4]
                    + wx * (frustrum_vertices[5] - frustrum_vertices[4])
                    + wy * (frustrum_vertices[6] - frustrum_vertices[4]);
                SF3D.Physics.Ray ray = new SF3D.Physics.Ray(r_start, r_end - r_start) { length = 1000 };

                // collide with every visible chunk
                Vector3 result = new Vector3(0, 0, 0);
                bool ray_success = false;

                ParallelOptions loop_options = new ParallelOptions();
                loop_options.MaxDegreeOfParallelism = 4;
                Parallel.For(0, map.heightmap.visible_chunks.Count, loop_options, (i, breakState) =>
                {
                    Vector3 local_result;
                    SFMapHeightMapChunk chunk = map.heightmap.visible_chunks[map.heightmap.visible_chunks.Count - i - 1].MapChunk;
                    Vector3 offset = new Vector3(chunk.ix * 16, 0, chunk.iy * 16);
                    if (ray.Intersect(chunk.collision_cache, out local_result))
                    {
                        breakState.Break();
                        result = local_result;
                        ray_success = true;
                        result += offset;
                    }
                });

                if (ray_success)
                {
                    SFCoord cursor_coord = new SFCoord(
                        (int)(Math.Max
                            (0, Math.Min
                                (result.X, map.width - 1))),
                        (int)(Math.Max
                            (0, Math.Min
                                (result.Z, map.height - 1))));
                    SFCoord inv_cursor_coord = new SFCoord(cursor_coord.x, map.height - cursor_coord.y - 1);

                    if (map.selection_helper.SetCursorPosition(cursor_coord))
                    {
                        update_render = true;
                        StatusStrip.SuspendLayout();
                        StatusText.Text = "Cursor position: " + inv_cursor_coord.ToString();
                        SetSpecificText(inv_cursor_coord);
                        StatusStrip.ResumeLayout();
                    }

                    // on click action
                    if ((mouse_pressed) && (selected_editor != null))
                    {
                        selected_editor.OnMousePress(inv_cursor_coord, mouse_last_pressed);
                        update_render = true;
                    }
                }
            }

            // rotating view by mouse
            if (mouse_scroll)
            {
                Vector2 scroll_mouse_end = new Vector2(Cursor.Position.X, Cursor.Position.Y);
                Vector2 scroll_translation = (scroll_mouse_end - scroll_mouse_start) * 50 / 250000f;

                SFRenderEngine.scene.camera.Direction += new Vector2(scroll_translation.X, -scroll_translation.Y);

                update_render = true;
            }

            // moving view by arrow keys
            Vector2 movement_vector = new Vector2(0, 0);
            if (arrows_pressed[0])
                movement_vector += new Vector2(-1, 0);
            if (arrows_pressed[1])
                movement_vector += new Vector2(1, 0);
            if (arrows_pressed[2])
                movement_vector += new Vector2(0, -1);
            if (arrows_pressed[3])
                movement_vector += new Vector2(0, 1);

            if (movement_vector != new Vector2(0, 0))
            {
                float angle = SFRenderEngine.scene.camera.Direction.X - (float)(Math.PI * 3 / 2);
                movement_vector = MathUtils.RotateVec2(movement_vector, angle);
                SFRenderEngine.scene.camera.translate(new Vector3(movement_vector.X, 0, movement_vector.Y)
                    * 50 / 50f);
                update_render = true;
            }

            // rotating view by home/end/pageup/pagedown
            movement_vector = new Vector2(0, 0);
            if (rotation_pressed[0])
                movement_vector += new Vector2(-1, 0);
            if (rotation_pressed[1])
                movement_vector += new Vector2(1, 0);
            if (rotation_pressed[2])
                movement_vector += new Vector2(0, -1);
            if (rotation_pressed[3])
                movement_vector += new Vector2(0, 1);

            if (movement_vector != new Vector2(0, 0))
            {
                SFRenderEngine.scene.camera.Direction += new Vector2(movement_vector.X, -movement_vector.Y)
                    * 50 / 2000f;
                update_render = true;
            }

            // render time
            if (update_render)
            {
                SFRenderEngine.scene.sun_light.ShadowSize = Math.Max(25f, zoom_level * 40f);
                map.selection_helper.UpdateSelection();
                AdjustCameraZ();
                SFRenderEngine.UpdateVisibleChunks();
                SFRenderEngine.scene.Update();
                RenderWindow.Invalidate();
                update_render = false;
            }

            if (dynamic_render)
                update_render = true;

            // garbage collector
            gc_timer += 1;
            if (gc_timer == 200)
            {
                GC.Collect();
                gc_timer = 0;
            }

            TimerAnimation.Start();
        }

        private void SetSpecificText(SFCoord pos)
        {
            byte dec_assign = map.decoration_manager.GetFixedDecAssignment(pos);
            SpecificText.Text = "H: " + map.heightmap.GetHeightAt(pos.x, pos.y).ToString() + "  "
                              + "T: " + map.heightmap.GetTileFixed(pos).ToString() + "  "
                              + "D: " + (dec_assign == 0 ? "X" : dec_assign.ToString());
        }

        private void AdjustCameraZ()
        {
            if (map != null)
            {
                Vector2 p = new Vector2(SFRenderEngine.scene.camera.Position.X, SFRenderEngine.scene.camera.Position.Z);
                float z = map.heightmap.GetRealZ(p);

                SFRenderEngine.scene.camera.translate(new Vector3(0, (25 * zoom_level) + z - SFRenderEngine.scene.camera.Position.Y, 0));
            }
        }

        // attempts to center camera on the selected position with preservation of camera angle
        public void SetCameraViewPoint(SFCoord pos)
        {
            // camera angle in radians
            Vector2 cam_dir = SFRenderEngine.scene.camera.Direction;
            // angle shift necessary due to fact that the map is mirrored in Z axis
            float angle_shift = (float)-Math.PI * 3 / 2;
            // camera inclination means that we need to increase distance of camera to the position to preserve camera angle
            float angle_factor = 1;
            if (cam_dir.Y != 0)
                angle_factor = (float)Math.Abs(1 / Math.Tan(cam_dir.Y));
            // resulting shift of camera positon from desired coordinates to canter the view on them
            Vector2 cam_shift = new Vector2((float)-Math.Sin(cam_dir.X + angle_shift),
                                            (float)Math.Cos(cam_dir.X + angle_shift)) * (25 * angle_factor * zoom_level);
            SFRenderEngine.scene.camera.Position = new Vector3(cam_shift.X, 25, cam_shift.Y);
            SFRenderEngine.scene.camera.Direction = cam_dir;

            Vector3 new_camera_pos = new Vector3(pos.x + cam_shift.X, 0, map.heightmap.height - pos.y - 1 + cam_shift.Y);
            SFRenderEngine.scene.camera.translate(new_camera_pos - SFRenderEngine.scene.camera.Position);
            AdjustCameraZ();
            update_render = true;
        }

        public void ResetCamera()
        {
            SFRenderEngine.scene.camera.Direction = new Vector2((float)(Math.PI * 3 / 2), -1.2f);
            zoom_level = 1;
            AdjustCameraZ();
            update_render = true;
        }

        private void ResizeWindow()
        {
            int ystart = RenderWindow.Location.Y;
            int yend = StatusStrip.Location.Y;
            int w_height = yend - ystart - 3;
            int w_width = this.Width - 22 - (PanelInspector.Visible ? PanelInspector.Width : 0);
            RenderWindow.Location = new Point(3, ystart);
            RenderWindow.Size = new Size(w_width, w_height);
            PanelInspector.Location = new Point(6 + RenderWindow.Width, ystart);
            SFRenderEngine.ResizeView(new Vector2(w_width, w_height));
            update_render = true;
        }

        private void slopebasedPaintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;
            if (autotexture_form != null)
                return;
            autotexture_form = new SFMap.map_dialog.MapAutoTextureDialog();
            autotexture_form.map = map;
            autotexture_form.FormClosing += new FormClosingEventHandler(autotextureform_FormClosing);
            autotexture_form.Show();
        }

        private void autotextureform_FormClosing(object sender, FormClosingEventArgs e)
        {
            autotexture_form.FormClosing -= new FormClosingEventHandler(autotextureform_FormClosing);
            autotexture_form = null;
        }

        // keyboard control of the 3d camera
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
                case Keys.Home:
                    rotation_pressed[0] = true;
                    return true;
                case Keys.End:
                    rotation_pressed[1] = true;
                    return true;
                case Keys.PageUp:
                    rotation_pressed[2] = true;
                    return true;
                case Keys.PageDown:
                    rotation_pressed[3] = true;
                    return true;
                case Keys.D1 | Keys.Control:
                    if (TabEditorModes.Enabled)
                        TabEditorModes.SelectedIndex = 0;
                    return true;
                case Keys.D2 | Keys.Control:
                    if (TabEditorModes.Enabled)
                        TabEditorModes.SelectedIndex = 1;
                    return true;
                case Keys.D3 | Keys.Control:
                    if (TabEditorModes.Enabled)
                        TabEditorModes.SelectedIndex = 2;
                    return true;
                case Keys.D4 | Keys.Control:
                    if (TabEditorModes.Enabled)
                        TabEditorModes.SelectedIndex = 3;
                    return true;
                case Keys.D5 | Keys.Control:
                    if (TabEditorModes.Enabled)
                        TabEditorModes.SelectedIndex = 4;
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
                else if ((int)msg.WParam == 0x24) // home
                    rotation_pressed[0] = false;
                else if ((int)msg.WParam == 0x23) // end
                    rotation_pressed[1] = false;
                else if ((int)msg.WParam == 0x21) // pageup
                    rotation_pressed[2] = false;
                else if ((int)msg.WParam == 0x22) // pagedown
                    rotation_pressed[3] = false;
            }
            return base.ProcessKeyPreview(ref msg);
        }

        private void ResetRotation_Click(object sender, EventArgs e)
        {
            ResetCamera();
        }

        private void InspectorClear()
        {
            this.Focus();
            PanelInspector.Controls.Clear();
            selected_inspector = null;
            InspectorHide();
        }

        private void InspectorHide()
        {
            if (!PanelInspector.Visible)
                return;

            PanelInspector.Visible = false;
            ResizeWindow();
        }

        private void InspectorShow()
        {
            if (PanelInspector.Visible)
                return;

            PanelInspector.Visible = true;
            ResizeWindow();
        }

        private void InspectorSet(SFMap.map_controls.MapInspector inspector)
        {
            if (selected_inspector != null)
                InspectorClear();
            if (inspector != null)
            {
                inspector.map = map;
                selected_inspector = inspector;
                PanelInspector.Controls.Add(inspector);
                inspector.Location = new Point(0, 0);
                InspectorResize(inspector.Width);
                InspectorShow();
            }
        }

        public void InspectorSelect(object o)
        {
            if (selected_inspector == null)
            {
                map.selection_helper.CancelSelection();
                return;
            }
            selected_inspector.OnSelect(o);
        }

        private void InspectorResize(int width)
        {
            PanelInspector.Width = width;
            PanelInspector.Height = this.Height - 25 - PanelInspector.Location.Y;
            PanelInspector.Location = new Point(this.Width - width - 22, PanelInspector.Location.Y);
            if (selected_inspector != null)
                selected_inspector.Height = PanelInspector.Height;
            if (PanelInspector.Visible)
                ResizeWindow();
        }

        private void InitEditorMode()
        {
            TabEditorModes.Enabled = true;
            TabEditorModes.SelectedIndex = -1;
            TabEditorModes.SelectedIndex = 0;
        }

        private void TabEditorModes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabEditorModes.SelectedIndex == -1)
                return;

            FlagOverlaysSetInvisible();
            FlagDecalSetInvisible();

            if (TabEditorModes.SelectedIndex == 0) // TERRAIN
            {
                ReselectTerrainMode();
            }
            else if (TabEditorModes.SelectedIndex == 1) // TEXTURE
            {
                ReselectTextureMode();
            }
            else if (TabEditorModes.SelectedIndex == 2) // ENTITIES
            {
                ReselectEntityMode();
            }
            else if (TabEditorModes.SelectedIndex == 3) // DECORATIONS
            {
                ReselectDecorationMode();
            }
            else if (TabEditorModes.SelectedIndex == 4) // METADATA
            {
                ReselectMetadataMode();
            }
        }

        //TERRAIN EDIT

        private void ReselectTerrainMode()
        {
            PanelBrushShape.Parent = TabEditorModes.TabPages[0];
            if (RadioHMap.Checked)
            {
                RadioHMap.Checked = false;
                RadioHMap.Checked = true;
            }
            if (RadioFlags.Checked)
            {
                RadioFlags.Checked = false;
                RadioFlags.Checked = true;
            }
            if (RadioLakes.Checked)
            {
                RadioLakes.Checked = false;
                RadioLakes.Checked = true;
            }
        }

        private HMapEditMode GetHeightMapEditMode()
        {
            if (RadioModeRaise.Checked)
                return HMapEditMode.RAISE;
            else if (RadioModeSet.Checked)
                return HMapEditMode.SET;
            else if (RadioModeSmooth.Checked)
                return HMapEditMode.SMOOTH;
            else
                return HMapEditMode.SET;
        }

        private HMapBrushInterpolationMode GetHeightMapInterpolationMode()
        {
            if (RadioIntConstant.Checked)
                return HMapBrushInterpolationMode.CONSTANT;
            else if (RadioIntLinear.Checked)
                return HMapBrushInterpolationMode.LINEAR;
            else if (RadioIntSquare.Checked)
                return HMapBrushInterpolationMode.SQUARE;
            else if (RadioIntSinusoidal.Checked)
                return HMapBrushInterpolationMode.SINUSOIDAL;
            else
                return HMapBrushInterpolationMode.SINUSOIDAL;
        }

        private BrushShape GetTerrainBrushShape()
        {
            if (RadioDiamond.Checked)
                return BrushShape.DIAMOND;
            else if (RadioCircle.Checked)
                return BrushShape.CIRCLE;
            else if (RadioSquare.Checked)
                return BrushShape.SQUARE;
            else
                return BrushShape.CIRCLE;
        }

        private void RadioHMap_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioHMap.Checked)
                return;

            InspectorClear();

            selected_editor = new MapHeightMapEditor()
            {
                Brush = terrain_brush,
                Value = Utility.TryParseUInt16(TerrainValue.Text),
                EditMode = GetHeightMapEditMode(),
                Interpolation = GetHeightMapInterpolationMode(),
                map = this.map
            };

            PanelFlags.Visible = false;
            PanelBrushShape.Visible = true;
            PanelStrength.Visible = true;
            PanelTerrainSettings.Visible = true;

            terrain_brush.size = (float)Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();

            FlagOverlaysSetInvisible();
        }

        public void HMapEditSetHeight(int h)
        {
            TerrainValue.Text = h.ToString();
        }

        private void BrushSizeVal_Validated(object sender, EventArgs e)
        {
            int v = Utility.TryParseUInt16(BrushSizeVal.Text);
            BrushSizeTrackbar.Value = (v < BrushSizeTrackbar.Minimum ? BrushSizeTrackbar.Minimum :
                                      (v > BrushSizeTrackbar.Maximum ? BrushSizeTrackbar.Maximum : v));
            terrain_brush.size = (float)v;
        }

        private void BrushSizeTrackbar_ValueChanged(object sender, EventArgs e)
        {
            BrushSizeVal.Text = BrushSizeTrackbar.Value.ToString();
            terrain_brush.size = (float)BrushSizeTrackbar.Value;
        }

        private void RadioSquare_CheckedChanged(object sender, EventArgs e)
        {
            terrain_brush.shape = GetTerrainBrushShape();
        }

        private void RadioCircle_CheckedChanged(object sender, EventArgs e)
        {
            terrain_brush.shape = GetTerrainBrushShape();
        }

        private void RadioDiamond_CheckedChanged(object sender, EventArgs e)
        {
            terrain_brush.shape = GetTerrainBrushShape();
        }

        private void TerrainValue_Validated(object sender, EventArgs e)
        {
            int v = Utility.TryParseUInt16(TerrainValue.Text);
            TerrainTrackbar.Value = (v < TerrainTrackbar.Minimum ? TerrainTrackbar.Minimum :
                                      (v > TerrainTrackbar.Maximum ? TerrainTrackbar.Maximum : v));
            ((MapHeightMapEditor)selected_editor).Value = v;
        }

        private void TerrainTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (RadioModeSet.Checked)
                return;
            TerrainValue.Text = TerrainTrackbar.Value.ToString();
            ((MapHeightMapEditor)selected_editor).Value = TerrainTrackbar.Value;
        }

        private void RadioIntConstant_CheckedChanged(object sender, EventArgs e)
        {
            ((MapHeightMapEditor)selected_editor).Interpolation = GetHeightMapInterpolationMode();
        }

        private void RadioIntLinear_CheckedChanged(object sender, EventArgs e)
        {
            ((MapHeightMapEditor)selected_editor).Interpolation = GetHeightMapInterpolationMode();
        }

        private void RadioIntSquare_CheckedChanged(object sender, EventArgs e)
        {
            ((MapHeightMapEditor)selected_editor).Interpolation = GetHeightMapInterpolationMode();
        }

        private void RadioIntSinusoidal_CheckedChanged(object sender, EventArgs e)
        {
            ((MapHeightMapEditor)selected_editor).Interpolation = GetHeightMapInterpolationMode();
        }

        private void RadioModeRaise_CheckedChanged(object sender, EventArgs e)
        {
            ((MapHeightMapEditor)selected_editor).EditMode = GetHeightMapEditMode();
            TerrainValueLabel.Text = "Strength";
        }

        private void RadioModeSet_CheckedChanged(object sender, EventArgs e)
        {
            ((MapHeightMapEditor)selected_editor).EditMode = GetHeightMapEditMode();
            TerrainValueLabel.Text = "Value";
        }

        private void RadioModeSmooth_CheckedChanged(object sender, EventArgs e)
        {
            ((MapHeightMapEditor)selected_editor).EditMode = GetHeightMapEditMode();
            TerrainValueLabel.Text = "Strength %";
        }

        // TERRAIN FLAGS

        private TerrainFlagType GetTerrainFlagType()
        {
            if (RadioFlagMovement.Checked)
                return TerrainFlagType.MOVEMENT;
            else if (RadioFlagVision.Checked)
                return TerrainFlagType.VISION;
            else
                return TerrainFlagType.MOVEMENT;
        }

        private void FlagOverlaysSetInvisible()
        {
            if (!map.heightmap.OverlayIsVisible("TileMovementBlock"))
                return;

            map.heightmap.OverlayClear("TileMovementBlock");
            map.heightmap.OverlaySetVisible("TileMovementBlock", false);

            map.heightmap.OverlayClear("ManualMovementBlock");
            map.heightmap.OverlaySetVisible("ManualMovementBlock", false);

            map.heightmap.OverlayClear("ManualVisionBlock");
            map.heightmap.OverlaySetVisible("ManualVisionBlock", false);

            foreach (SF3D.SceneSynchro.SceneNodeMapChunk chunk_node in map.heightmap.chunk_nodes)
            {
                chunk_node.MapChunk.OverlayUpdate("TileMovementBlock");
                chunk_node.MapChunk.OverlayUpdate("ManualMovementBlock");
                chunk_node.MapChunk.OverlayUpdate("ManualVisionBlock");
            }
        }

        private void FlagOverlaysSetVisible()
        {
            if (map.heightmap.OverlayIsVisible("TileMovementBlock"))
                return;

            for (int i = 0; i < map.height; i++)
                for (int j = 0; j < map.width; j++)
                    if (map.heightmap.texture_manager.texture_tiledata[map.heightmap.tile_data[i * map.width + j]].blocks_movement)
                        map.heightmap.OverlayAdd("TileMovementBlock", new SFCoord(j, i));

            foreach (SFCoord p in map.heightmap.chunk42_data)
                map.heightmap.OverlayAdd("ManualMovementBlock", p);

            foreach (SFCoord p in map.heightmap.chunk56_data)
                map.heightmap.OverlayAdd("ManualVisionBlock", p);

            map.heightmap.OverlaySetVisible("TileMovementBlock", true);
            map.heightmap.OverlaySetVisible("ManualMovementBlock", true);
            map.heightmap.OverlaySetVisible("ManualVisionBlock", true);
            foreach (SF3D.SceneSynchro.SceneNodeMapChunk chunk_node in map.heightmap.chunk_nodes)
            {
                chunk_node.MapChunk.OverlayUpdate("TileMovementBlock");
                chunk_node.MapChunk.OverlayUpdate("ManualMovementBlock");
                chunk_node.MapChunk.OverlayUpdate("ManualVisionBlock");
            }
        }

        private void RadioFlags_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioFlags.Checked)
                return;

            InspectorClear();

            selected_editor = new MapTerrainFlagsEditor()
            {
                Brush = terrain_brush,
                FlagType = GetTerrainFlagType(),
                map = this.map
            };

            PanelFlags.Visible = true;
            PanelFlags.Location = PanelTerrainSettings.Location;
            PanelBrushShape.Visible = true;
            PanelStrength.Visible = false;
            PanelTerrainSettings.Visible = false;

            terrain_brush.size = (float)Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();

            CheckDisplayFlags_CheckedChanged(null, null);
        }

        private void RadioFlagMovement_CheckedChanged(object sender, EventArgs e)
        {
            ((MapTerrainFlagsEditor)selected_editor).FlagType = GetTerrainFlagType();
        }

        private void RadioFlagVision_CheckedChanged(object sender, EventArgs e)
        {
            ((MapTerrainFlagsEditor)selected_editor).FlagType = GetTerrainFlagType();
        }

        private void CheckDisplayFlags_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckDisplayFlags.Checked)
                FlagOverlaysSetVisible();
            else
                FlagOverlaysSetInvisible();
        }

        // LAKES

        private void RadioLakes_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioLakes.Checked)
                return;

            selected_editor = new MapLakesEditor()
            {
                map = this.map
            };

            PanelFlags.Visible = false;
            PanelBrushShape.Visible = false;
            PanelStrength.Visible = false;
            PanelTerrainSettings.Visible = false;

            FlagOverlaysSetInvisible();

            InspectorSet(new SFMap.map_controls.MapLakeInspector());
        }

        // TERRAIN PAINT

        private SFMap.map_controls.TerrainTileType GetTileType()
        {
            if (RadioTileTypeBase.Checked)
                return SFMap.map_controls.TerrainTileType.BASE;
            else if (RadioTileTypeCustom.Checked)
                return SFMap.map_controls.TerrainTileType.CUSTOM;
            else
                return SFMap.map_controls.TerrainTileType.BASE;
        }

        private void ReselectTextureMode()
        {
            PanelBrushShape.Parent = TabEditorModes.TabPages[1];

            InspectorSet(new SFMap.map_controls.MapTerrainTextureInspector());
            ((SFMap.map_controls.MapTerrainTextureInspector)selected_inspector).SetInspectorType(GetTileType());

            selected_editor = new MapTerrainTextureEditor
            {
                Brush = terrain_brush,
                map = this.map,
                SelectedTile = 0,
                EditSimilar = TTexMatchMovementFlags.Checked
            };

            PanelBrushShape.Visible = true;

            terrain_brush.size = (float)Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();
        }

        private void RadioTileTypeBase_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioTileTypeBase.Checked)
                ((SFMap.map_controls.MapTerrainTextureInspector)selected_inspector).SetInspectorType(GetTileType());
        }

        private void RadioTileTypeCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioTileTypeCustom.Checked)
                ((SFMap.map_controls.MapTerrainTextureInspector)selected_inspector).SetInspectorType(GetTileType());
        }

        private void TTexMatchMovementFlags_CheckedChanged(object sender, EventArgs e)
        {
            ((MapTerrainTextureEditor)selected_editor).EditSimilar = TTexMatchMovementFlags.Checked;
        }

        private void ButtonModifyTextureSet_Click(object sender, EventArgs e)
        {
            SFMap.map_dialog.MapModifyTextureSet mmts = new SFMap.map_dialog.MapModifyTextureSet(map);
            mmts.ShowDialog();
            update_render = true;
            ReselectTextureMode();
        }

        private void ButtonSlopePaint_Click(object sender, EventArgs e)
        {
            if (autotexture_form != null)
                return;
            autotexture_form = new SFMap.map_dialog.MapAutoTextureDialog();
            autotexture_form.map = map;
            autotexture_form.FormClosing += new FormClosingEventHandler(autotextureform_FormClosing);
            autotexture_form.Show();
        }

        // ENTITIES

        private void ReselectEntityMode()
        {
            EditCoopCampTypes.Location = PanelEntityPlacementSelect.Location;
            PanelMonumentType.Location = PanelEntityPlacementSelect.Location;
            if (RadioEntityModeUnit.Checked)
            {
                RadioEntityModeUnit.Checked = false;
                RadioEntityModeUnit.Checked = true;
            }
            if (RadioEntityModeBuilding.Checked)
            {
                RadioEntityModeBuilding.Checked = false;
                RadioEntityModeBuilding.Checked = true;
            }
            if (RadioEntityModeObject.Checked)
            {
                RadioEntityModeObject.Checked = false;
                RadioEntityModeObject.Checked = true;
            }
            if (RadioModeCoopCamps.Checked)
            {
                RadioModeCoopCamps.Checked = false;
                RadioModeCoopCamps.Checked = true;
            }
            if (RadioModeBindstones.Checked)
            {
                RadioModeBindstones.Checked = false;
                RadioModeBindstones.Checked = true;
            }
            if (RadioModePortals.Checked)
            {
                RadioModePortals.Checked = false;
                RadioModePortals.Checked = true;
            }
            if (RadioModeMonuments.Checked)
            {
                RadioModeMonuments.Checked = false;
                RadioModeMonuments.Checked = true;
            }
        }

        public void InitializeComboRaces()
        {
            if (!SFCFF.SFCategoryManager.ready)
                return;

            UnitRace.Items.Clear();

            SFCFF.SFCategory races_cat = SFCFF.SFCategoryManager.gamedata[15];
            for (int i = 0; i < races_cat.GetElementCount(); i++)
            {
                ushort race_name_index = (ushort)(races_cat[i][7]);
                SFCFF.SFCategoryElement name_elem = SFCFF.SFCategoryManager.FindElementText(race_name_index, Settings.LanguageID);
                string race_name;
                if (name_elem != null)
                    race_name = Utility.CleanString(name_elem[4]);
                else
                    race_name = Utility.S_MISSING;
                UnitRace.Items.Add(race_name);
            }
        }

        public void ReloadRaceUnits()
        {
            byte race_id = (byte)SFCFF.SFCategoryManager.gamedata[15][UnitRace.SelectedIndex][0];
            List<int> unit_indices = new List<int>();
            SFCFF.SFCategory units_cat = SFCFF.SFCategoryManager.gamedata[17];

            for (int i = 0; i < units_cat.GetElementCount(); i++)
            {
                ushort stats_id = (ushort)(units_cat[i][2]);
                SFCFF.SFCategoryElement stats_elem = SFCFF.SFCategoryManager.gamedata[3].FindElementBinary(0, stats_id);
                if (stats_elem == null)
                    continue;
                byte unit_race_id = (byte)stats_elem[2];
                if (race_id != unit_race_id)
                    continue;

                unit_indices.Add((ushort)(units_cat[i][0]));
            }

            //  TODO: sort by level
            // right now:  just passing whole list
            unitcombo_to_unitindex = unit_indices;

            for (int i = 0; i < unitcombo_to_unitindex.Count; i++)
                UnitName.Items.Add(SFCFF.SFCategoryManager.GetUnitName((ushort)unitcombo_to_unitindex[i], true));
        }

        private void RadioEntityModeUnit_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioEntityModeUnit.Checked)
                return;

            InspectorSet(new SFMap.map_controls.MapUnitInspector());

            InitializeComboRaces();
            unitcombo_to_unitindex.Clear();
            UnitName.Items.Clear();

            selected_editor = new MapUnitEditor()
            {
                map = this.map
            };

            PanelUnitPlacementSelect.Visible = true;
            PanelEntityPlacementSelect.Visible = true;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
        }

        private void EntityID_Validated(object sender, EventArgs e)
        {
            if (RadioEntityModeUnit.Checked)
                ((MapUnitEditor)selected_editor).placement_unit = Utility.TryParseUInt16(EntityID.Text);
            else if (RadioEntityModeBuilding.Checked)
                ((MapBuildingEditor)selected_editor).placement_building = Utility.TryParseUInt16(EntityID.Text);
            else if (RadioEntityModeObject.Checked)
                ((MapObjectEditor)selected_editor).placement_object = Utility.TryParseUInt16(EntityID.Text);
        }

        private void EntityID_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = -1;
            if (RadioEntityModeUnit.Checked)
                cat_id = 17;
            else if (RadioEntityModeObject.Checked)
                cat_id = 33;
            else if (RadioEntityModeBuilding.Checked)
                cat_id = 23;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(EntityID.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        private void UnitRace_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnitName.Items.Clear();
            unitcombo_to_unitindex.Clear();
            if (UnitRace.SelectedIndex == -1)
                return;

            ReloadRaceUnits();
        }

        private void UnitName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UnitName.SelectedIndex == -1)
                return;

            EntityID.Text = unitcombo_to_unitindex[UnitName.SelectedIndex].ToString();
            ((MapUnitEditor)selected_editor).placement_unit = Utility.TryParseUInt16(EntityID.Text);
        }

        private void RadioEntityModeBuilding_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioEntityModeBuilding.Checked)
                return;

            InspectorSet(new SFMap.map_controls.MapBuildingInspector());

            selected_editor = new MapBuildingEditor()
            {
                map = this.map
            };

            PanelUnitPlacementSelect.Visible = false;
            PanelEntityPlacementSelect.Visible = true;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
        }

        private void RadioEntityModeObject_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioEntityModeObject.Checked)
                return;

            InspectorSet(new SFMap.map_controls.MapObjectInspector());

            selected_editor = new MapObjectEditor()
            {
                map = this.map
            };

            PanelUnitPlacementSelect.Visible = false;
            PanelEntityPlacementSelect.Visible = true;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
        }

        private void RadioModeCoopCamps_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioModeCoopCamps.Checked)
                return;

            InspectorSet(new SFMap.map_controls.MapCoopCampInspector());

            selected_editor = new MapCoopCampEditor()
            {
                map = this.map
            };

            PanelUnitPlacementSelect.Visible = false;
            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = true;
            PanelMonumentType.Visible = false;
        }

        private void EditCoopCampTypes_Click(object sender, EventArgs e)
        {
            SFLua.SFLuaEnvironment.ShowRtsCoopSpawnGroupsForm();
            InspectorSet(new SFMap.map_controls.MapCoopCampInspector());
        }

        private void RadioModeBindstones_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioModeBindstones.Checked)
                return;

            InspectorSet(new SFMap.map_controls.MapBindstoneInspector());

            selected_editor = new MapBindstoneEditor()
            {
                map = this.map
            };

            PanelUnitPlacementSelect.Visible = false;
            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
        }

        private void RadioModePortals_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioModePortals.Checked)
                return;

            InspectorSet(new SFMap.map_controls.MapPortalInspector());

            selected_editor = new MapPortalEditor()
            {
                map = this.map
            };

            PanelUnitPlacementSelect.Visible = false;
            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
        }

        private MonumentType GetMonumentType()
        {
            if (MonumentHuman.Checked)
                return MonumentType.HUMAN;
            else if (MonumentElf.Checked)
                return MonumentType.ELF;
            else if (MonumentDwarf.Checked)
                return MonumentType.DWARF;
            else if (MonumentOrc.Checked)
                return MonumentType.ORC;
            else if (MonumentTroll.Checked)
                return MonumentType.TROLL;
            else if (MonumentDarkElf.Checked)
                return MonumentType.DARKELF;
            else if (MonumentHero.Checked)
                return MonumentType.HERO;
            else
                return MonumentType.HERO;
        }

        private void RadioModeMonuments_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioModeMonuments.Checked)
                return;

            InspectorSet(new SFMap.map_controls.MapMonumentInspector());

            selected_editor = new MapMonumentEditor()
            {
                map = this.map
            };

            PanelUnitPlacementSelect.Visible = false;
            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = true;
        }

        private void MonumentHuman_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentHuman.Checked)
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
        }

        private void MonumentElf_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentElf.Checked)
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
        }

        private void MonumentDwarf_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentDwarf.Checked)
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
        }

        private void MonumentOrc_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentOrc.Checked)
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
        }

        private void MonumentTroll_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentTroll.Checked)
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
        }

        private void MonumentDarkElf_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentDarkElf.Checked)
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
        }

        private void MonumentHero_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentHero.Checked)
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
        }

        // DECORATIONS

        private void OnDecButtonPress(object sender, EventArgs e)
        {
            selected_inspector.OnSelect(((Button)sender).Tag);
            ((MapDecorationEditor)selected_editor).selected_dec_group = (int)((Button)sender).Tag;
        }

        private Color GetDecGroupButtonColor(int i)
        {
            if (i == 0)
                return Color.IndianRed;
            if (map.decoration_manager.dec_groups[i].random_cache.Count != 0)
                return Color.LightGreen;
            else
                return SystemColors.Control;
        }

        private void ResetDecGroups()
        {
            PanelDecalGroups.Controls.Clear();
            for (int i = 0; i < 255; i++)
            {
                Button decbutton = new Button()
                {
                    Size = new Size(35, 23),
                    Text = (i != 0 ? i.ToString() : "X"),
                    Tag = i,
                    Font = new Font("Arial", 8),
                    Margin = new Padding(0, 0, 0, 0),
                    Padding = new Padding(0, 0, 0, 0),
                    BackColor = GetDecGroupButtonColor(i)
                };
                decbutton.Click += new EventHandler(OnDecButtonPress);
                PanelDecalGroups.Controls.Add(decbutton);
                decbutton.Location = new Point(41 * (i % 12), 29 * (i / 12));
            }
        }

        public void UpdateDecGroup(int i)
        {
            PanelDecalGroups.Controls[i].BackColor = GetDecGroupButtonColor(i);
        }

        private void FlagDecalSetInvisible()
        {
            if (!map.heightmap.OverlayIsVisible("DecorationTile"))
                return;

            map.heightmap.OverlayClear("DecorationTile");
            map.heightmap.OverlaySetVisible("DecorationTile", false);

            foreach (SF3D.SceneSynchro.SceneNodeMapChunk chunk_node in map.heightmap.chunk_nodes)
                chunk_node.MapChunk.OverlayUpdate("DecorationTile");
        }

        private void FlagDecalSetVisible()
        {
            if (map.heightmap.OverlayIsVisible("DecorationTile"))
                return;

            map.heightmap.OverlaySetVisible("DecorationTile", true);
            foreach (SF3D.SceneSynchro.SceneNodeMapChunk chunk_node in map.heightmap.chunk_nodes)
                chunk_node.MapChunk.OverlayUpdate("DecorationTile");
        }

        private void ReselectDecorationMode()
        {
            PanelBrushShape.Parent = TabEditorModes.TabPages[3];

            if(PanelDecalGroups.Controls.Count == 0)
                ResetDecGroups();

            selected_editor = new MapDecorationEditor
            {
                Brush = terrain_brush,
                map = this.map
            };
            InspectorSet(new SFMap.map_controls.MapDecorationInspector());

            FlagDecalSetVisible();

            PanelBrushShape.Visible = true;

            terrain_brush.size = (float)Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();
        }

        // METADATA

        private void ReselectMetadataMode()
        {
            if (map.metadata.map_type == SFMapType.CAMPAIGN)
            {
                MapTypeCampaign.Checked = false;
                MapTypeCampaign.Checked = true;
            }
            else if (map.metadata.map_type == SFMapType.COOP)
            {
                MapTypeCoop.Checked = false;
                MapTypeCoop.Checked = true;
            }
            else if (map.metadata.map_type == SFMapType.MULTIPLAYER)
            {
                MapTypeMultiplayer.Checked = false;
                MapTypeMultiplayer.Checked = true;
            }

            selected_editor = null;
            InspectorSet(null);
        }

        private void UpdateCoopSpawnParameters()
        {
            CoopSpawnParam11.Text = map.metadata.coop_spawn_params[0].param1.ToString();
            CoopSpawnParam12.Text = map.metadata.coop_spawn_params[0].param2.ToString();
            CoopSpawnParam13.Text = map.metadata.coop_spawn_params[0].param3.ToString();
            CoopSpawnParam14.Text = map.metadata.coop_spawn_params[0].param4.ToString();
            CoopSpawnParam21.Text = map.metadata.coop_spawn_params[1].param1.ToString();
            CoopSpawnParam22.Text = map.metadata.coop_spawn_params[1].param2.ToString();
            CoopSpawnParam23.Text = map.metadata.coop_spawn_params[1].param3.ToString();
            CoopSpawnParam24.Text = map.metadata.coop_spawn_params[1].param4.ToString();
            CoopSpawnParam31.Text = map.metadata.coop_spawn_params[2].param1.ToString();
            CoopSpawnParam32.Text = map.metadata.coop_spawn_params[2].param2.ToString();
            CoopSpawnParam33.Text = map.metadata.coop_spawn_params[2].param3.ToString();
            CoopSpawnParam34.Text = map.metadata.coop_spawn_params[2].param4.ToString();
        }

        private void MapTypeCampaign_CheckedChanged(object sender, EventArgs e)
        {
            if (MapTypeCampaign.Checked == false)
                return;

            map.metadata.map_type = SFMapType.CAMPAIGN;

            ButtonTeams.Visible = false;
            PanelCoopParams.Visible = false;
        }

        private void MapTypeCoop_CheckedChanged(object sender, EventArgs e)
        {
            if (MapTypeCoop.Checked == false)
                return;

            map.metadata.map_type = SFMapType.COOP;

            ButtonTeams.Visible = true;
            PanelCoopParams.Visible = true;
            UpdateCoopSpawnParameters();
        }

        private void MapTypeMultiplayer_CheckedChanged(object sender, EventArgs e)
        {
            if (MapTypeMultiplayer.Checked == false)
                return;

            map.metadata.map_type = SFMapType.MULTIPLAYER;

            ButtonTeams.Visible = true;
            PanelCoopParams.Visible = false;
        }

        private void CoopSpawnParam11_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[0].param1 = Utility.TryParseFloat(
                CoopSpawnParam11.Text, map.metadata.coop_spawn_params[0].param1);
        }

        private void CoopSpawnParam12_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[0].param2 = Utility.TryParseFloat(
                CoopSpawnParam12.Text, map.metadata.coop_spawn_params[0].param2);
        }

        private void CoopSpawnParam13_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[0].param3 = Utility.TryParseFloat(
                CoopSpawnParam13.Text, map.metadata.coop_spawn_params[0].param3);
        }

        private void CoopSpawnParam14_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[0].param4 = Utility.TryParseFloat(
                CoopSpawnParam14.Text, map.metadata.coop_spawn_params[0].param4);
        }

        private void CoopSpawnParam21_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[1].param1 = Utility.TryParseFloat(
                CoopSpawnParam21.Text, map.metadata.coop_spawn_params[1].param1);
        }

        private void CoopSpawnParam22_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[1].param2 = Utility.TryParseFloat(
                CoopSpawnParam22.Text, map.metadata.coop_spawn_params[1].param2);
        }

        private void CoopSpawnParam23_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[1].param3 = Utility.TryParseFloat(
                CoopSpawnParam23.Text, map.metadata.coop_spawn_params[1].param3);
        }

        private void CoopSpawnParam24_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[1].param4 = Utility.TryParseFloat(
                CoopSpawnParam24.Text, map.metadata.coop_spawn_params[1].param4);
        }

        private void CoopSpawnParam31_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[2].param1 = Utility.TryParseFloat(
                CoopSpawnParam31.Text, map.metadata.coop_spawn_params[2].param1);
        }

        private void CoopSpawnParam32_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[2].param2 = Utility.TryParseFloat(
                CoopSpawnParam32.Text, map.metadata.coop_spawn_params[2].param2);
        }

        private void CoopSpawnParam33_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[2].param3 = Utility.TryParseFloat(
                CoopSpawnParam33.Text, map.metadata.coop_spawn_params[2].param3);
        }

        private void CoopSpawnParam34_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[2].param4 = Utility.TryParseFloat(
                CoopSpawnParam34.Text, map.metadata.coop_spawn_params[2].param4);
        }

        private void ButtonTeams_Click(object sender, EventArgs e)
        {
            if (teamcomp_form != null)
                return;

            teamcomp_form = new SFMap.map_dialog.MapManageTeamCompositions();
            teamcomp_form.map = map;
            teamcomp_form.FormClosing += new FormClosingEventHandler(teamcompform_FormClosing);
            teamcomp_form.Show();
        }

        private void teamcompform_FormClosing(object sender, FormClosingEventArgs e)
        {
            teamcomp_form.FormClosing -= new FormClosingEventHandler(teamcompform_FormClosing);
            teamcomp_form = null;
        }

        private void visibilitySettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (visibility_form != null)
                return;

            visibility_form = new SFMap.map_dialog.MapVisibilitySettings();
            visibility_form.map = map;
            visibility_form.FormClosing += new FormClosingEventHandler(visibilityform_FormClosing);
            visibility_form.Show();
        }

        private void visibilityform_FormClosing(object sender, FormClosingEventArgs e)
        {
            visibility_form.FormClosing -= new FormClosingEventHandler(visibilityform_FormClosing);
            visibility_form = null;
        }

    }
}
