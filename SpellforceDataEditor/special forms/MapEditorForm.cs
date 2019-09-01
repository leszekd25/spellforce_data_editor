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

namespace SpellforceDataEditor.special_forms
{
    public enum MAPEDIT_MODE { HMAP = 0, TEXTURE, FLAG, LAKE, UNIT, BUILDING, OBJECT, DECAL, NPC, MISC_OBJECT, META, MAX }

    public partial class MapEditorForm : Form
    {
        SFMap.SFMap map = null;
        SFCFF.SFGameData gamedata = null;

        bool dynamic_render = false;     // if true, window will redraw every frame at 25 fps
        bool mouse_pressed = false;      // if true, mouse is pressed and in render window
        MouseButtons mouse_last_pressed = MouseButtons.Left;  // last mouse button pressed

        bool mouse_on_view = false;      // if true, mouse is in render window
        public bool update_render = false;
        Vector2 scroll_mouse_start = new Vector2(0, 0);
        bool mouse_scroll = false;
        float zoom_level = 1.0f;
        int gc_timer = 0;

        MAPEDIT_MODE edit_mode = MAPEDIT_MODE.HMAP;
        SFMap.map_controls.MapInspectorBaseControl[] edit_controls = new SFMap.map_controls.MapInspectorBaseControl[(int)MAPEDIT_MODE.MAX];

        OpenTK.GLControl RenderWindow = null;

        SFMap.map_dialog.MapAutoTextureDialog autotexture_form = null;

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

            // add controls
            for (int i = 0; i < (int)MAPEDIT_MODE.MAX; i++)
                edit_controls[i] = null;

            edit_controls[0] = new SFMap.map_controls.MapInspectorHeightMapControl();
            edit_controls[1] = new SFMap.map_controls.MapInspectorTerrainTextureControl();
            edit_controls[2] = new SFMap.map_controls.MapInspectorFlagControl();
            edit_controls[3] = new SFMap.map_controls.MapInspectorLakeControl();
            edit_controls[4] = new SFMap.map_controls.MapInspectorUnitControl();
            edit_controls[5] = new SFMap.map_controls.MapInspectorBuildingControl();
            edit_controls[6] = new SFMap.map_controls.MapInspectorObjectControl();
            edit_controls[7] = new SFMap.map_controls.MapInspectorDecorationControl();
            edit_controls[8] = new SFMap.map_controls.MapInspectorNPCControl();
            edit_controls[9] = new SFMap.map_controls.MapInspectorMiscellaneuosObjectsControl();
            edit_controls[10] = new SFMap.map_controls.MapInspectorMetadataControl();

            for (int i = 0; i < (int)MAPEDIT_MODE.MAX; i++)
            {
                if (edit_controls[i] != null)
                {
                    InspectorPanel.Controls.Add(edit_controls[i]);
                    edit_controls[i].Enabled = false;
                }
            }

            for (int i = 0; i < InspectorPanel.Controls.Count; i++)
            { 
                InspectorPanel.Controls[i].Hide();
                InspectorPanel.Controls[i].Location = new Point(3, 3);
            }

            gamedata = SFCFF.SFCategoryManager.gamedata;

            SetEditMode(MAPEDIT_MODE.HMAP);
        }
        
        private void MapEditorForm_Load(object sender, EventArgs e)
        {
            LogUtils.Log.MemoryUsage();
        }

        private void MapEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(CloseMap() != 0)
                e.Cancel = true;
        }

        private void MapEditorForm_Resize(object sender, EventArgs e)
        {
            if(RenderWindow!=null)
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

            map = new SFMap.SFMap();
            map.CreateDefault(map_size, generator, gamedata, StatusText);

            SFRenderEngine.scene.heightmap = map.heightmap;

            for (int i = 0; i < (int)MAPEDIT_MODE.MAX; i++)
                if (edit_controls[i] != null)
                {
                    edit_controls[i].map = map;
                    edit_controls[i].Enabled = true;
                }

            // ui initialization
            ((SFMap.map_controls.MapInspectorTerrainTextureControl)(edit_controls[1])).GenerateBaseTexturePreviews();
            ((SFMap.map_controls.MapInspectorTerrainTextureControl)(edit_controls[1])).GenerateTileListEntries();
            ((SFMap.map_controls.MapInspectorUnitControl)(edit_controls[4])).InitializeComboRaces();

            map.selection_helper.SetCursorPosition(new SFCoord(1, 1));
            map.selection_helper.SetCursorVisibility(true);

            SetCameraViewPoint(new SFCoord(map.width / 2, map.height / 2));

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

                for (int i = 0; i < (int)MAPEDIT_MODE.MAX; i++)
                    if (edit_controls[i] != null)
                    {
                        edit_controls[i].map = map;
                        edit_controls[i].Enabled = true;
                    }

                // ui initialization
                ((SFMap.map_controls.MapInspectorTerrainTextureControl)(edit_controls[1])).GenerateBaseTexturePreviews();
                ((SFMap.map_controls.MapInspectorTerrainTextureControl)(edit_controls[1])).GenerateTileListEntries();
                ((SFMap.map_controls.MapInspectorUnitControl)(edit_controls[4])).InitializeComboRaces();

                map.selection_helper.SetCursorPosition(new SFCoord(1, 1));
                map.selection_helper.SetCursorVisibility(true);

                SetCameraViewPoint(new SFCoord(map.width/2, map.height/2));

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
                        // important: remove created NPC IDs that are not used
                        foreach (int npc_id in ((SFMap.map_controls.MapInspectorNPCControl)edit_controls[8]).created_npc_ids)
                            if (!map.npc_manager.npc_info.ContainsKey(npc_id))
                                ((SFMap.map_controls.MapInspectorNPCControl)edit_controls[8]).RemoveNewNPCID(npc_id);
                        MainForm.data.save_data();
                        ((SFMap.map_controls.MapInspectorNPCControl)edit_controls[8]).created_npc_ids.Clear();
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
            else if(dr == DialogResult.Yes)
            {
                if (SaveMap() == DialogResult.Cancel)
                    return -2;
            }

            if (autotexture_form != null)
                autotexture_form.Close();

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
            if (MainForm.viewer != null)
                MainForm.viewer.ResetScene();
            for (int i = 0; i < (int)MAPEDIT_MODE.MAX; i++)
                if (edit_controls[i] != null)
                {
                    edit_controls[i].Enabled = false;
                    edit_controls[i].map = null;
                }
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
            this.RenderWindow.Location = new System.Drawing.Point(511, 52);
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
            if(e.Button == MouseButtons.Middle)
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
            edit_controls[(int)edit_mode].OnMouseUp();
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

        private void RenderWindow_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {

        }

        private void RenderWindow_MouseWheel(object sender, MouseEventArgs e)
        {
            if(e.Delta < 0)
            {
                zoom_level *= 1.1f;
                if (zoom_level > 2)
                    zoom_level = 2;
            }
            else if(e.Delta > 0)
            {
                zoom_level *= 0.9f;
                if (zoom_level < 0.3f)
                    zoom_level = 0.3f;
            }
            AdjustCameraZ();
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

            if (mouse_on_view)
            {
                // generate ray
                float px, py;
                px = Cursor.Position.X - this.Location.X - RenderWindow.Location.X;
                py = Cursor.Position.Y - this.Location.Y - RenderWindow.Location.Y - 29;
                float wx, wy;
                wx = px / RenderWindow.Size.Width;  wx = (wx+0.09f)*0.84f;
                wy = py / RenderWindow.Size.Height; wy = (wy+0.11f)*0.84f;
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
                    SFMapHeightMapChunk chunk = map.heightmap.visible_chunks[map.heightmap.visible_chunks.Count-i-1].MapChunk;
                    Vector3 offset = new Vector3(chunk.ix * 16, 0, chunk.iy * 16);
                    if (ray.Intersect(chunk.collision_cache, out local_result))
                    {
                        breakState.Break();
                        result = local_result;
                        ray_success = true;
                        result += offset;
                    }
                });
                /*for (int i = map.heightmap.visible_chunks.Count-1; i >= 0; i--)
                {
                    SFMapHeightMapChunk chunk = map.heightmap.visible_chunks[i].MapChunk;
                    offset = new Vector3(chunk.ix * 16, 0, chunk.iy * 16);
                    if (ray.Intersect(chunk.collision_cache, offset, out result))
                    {
                        ray_success = true;
                        result += offset;
                        break;
                    }
                }*/


                if (ray_success)
                {
                    SFCoord cursor_coord = new SFCoord((int)(Math.Max(0, Math.Min(result.X, map.width-1))), (int)(Math.Max(0,Math.Min(result.Z, map.height-1))));
                    SFCoord inv_cursor_coord = new SFCoord(cursor_coord.x, map.height - cursor_coord.y - 1);

                    if (map.selection_helper.SetCursorPosition(cursor_coord))
                    {
                        update_render = true;
                        StatusText.Text = "Cursor position: " + inv_cursor_coord.ToString();

                        switch (edit_mode)
                        {
                            case MAPEDIT_MODE.HMAP:
                            case MAPEDIT_MODE.LAKE:
                                SpecificText.Text = "Height: " + map.heightmap.GetZ(inv_cursor_coord).ToString();
                                break;
                            case MAPEDIT_MODE.TEXTURE:
                                SpecificText.Text = "Tile ID: " + map.heightmap.GetTileFixed(inv_cursor_coord).ToString();
                                break;
                            case MAPEDIT_MODE.DECAL:
                                SpecificText.Text = "Decoration group ID: " + map.decoration_manager.GetFixedDecAssignment(inv_cursor_coord).ToString();
                                break;
                            default:
                                SpecificText.Text = "";
                                break;
                        }
                    }

                    // on click action
                    if (mouse_pressed)
                    {
                        edit_controls[(int)edit_mode].OnMouseDown(cursor_coord, mouse_last_pressed);
                        update_render = true;
                    }
                }
            }

            if(mouse_scroll)
            {
                float mouse_scroll_factor = 0.01f;
                Vector2 scroll_mouse_end = new Vector2(Cursor.Position.X, Cursor.Position.Y);
                Vector2 scroll_translation = (scroll_mouse_end - scroll_mouse_start)*mouse_scroll_factor;
                SFRenderEngine.scene.camera.translate(new Vector3(scroll_translation.X, 0, scroll_translation.Y));
                update_render = true;
            }

            if ((dynamic_render)||(update_render))
            {
                map.selection_helper.UpdateSelection();
                AdjustCameraZ();
                SFRenderEngine.UpdateVisibleChunks();
                SFRenderEngine.scene.Update();
                RenderWindow.Invalidate();
                update_render = false;
            }

            gc_timer += 1;
            if (gc_timer == 200)
            {
                GC.Collect();
                gc_timer = 0;
            }

            TimerAnimation.Start();
        }

        private void AdjustCameraZ()
        {
            if(map != null)
            {
                Vector2 p = new Vector2(SFRenderEngine.scene.camera.Position.X, SFRenderEngine.scene.camera.Position.Z);
                float z = map.heightmap.GetRealZ(p);
                
                SFRenderEngine.scene.camera.translate(new Vector3(0, (25*zoom_level)+z - SFRenderEngine.scene.camera.Position.Y, 0));
            }
        }

        public void SetCameraViewPoint(SFCoord pos)
        {
            // these two decide camera angle
            SFRenderEngine.scene.camera.Position = new Vector3(0, 25, 12);
            SFRenderEngine.scene.camera.Lookat = new Vector3(0, 0, 0);

            Vector3 new_camera_pos = new Vector3(pos.x+0.03f, 0, map.heightmap.height - pos.y - 1 + 12+0.03f);
            SFRenderEngine.scene.camera.translate(new_camera_pos - SFRenderEngine.scene.camera.Position);
            AdjustCameraZ();
            update_render = true;
        }

        private void ResizeWindow()
        {
            int ystart = RenderWindow.Location.Y;
            int yend = StatusStrip.Location.Y;
            int w_size = yend - ystart - 3;
            RenderWindow.Location = new Point(this.Size.Width - w_size - 1, ystart);
            RenderWindow.Size = new Size(w_size, w_size);
            PanelModes.Size = new Size(PanelModes.Size.Width, w_size + 3);
            InspectorPanel.Size = new Size(RenderWindow.Location.X - PanelModes.Size.Width - 12, w_size-25);
            SFRenderEngine.ResizeView(new Vector2(w_size, w_size));
            RenderWindow.Invalidate();
        }

        public void SetEditMode(MAPEDIT_MODE mode)
        {
            edit_mode = mode;
            int c_index = InspectorPanel.Controls.IndexOf(edit_controls[(int)mode]);
            if (c_index < 0)
                return;

            for (int i = 0; i < InspectorPanel.Controls.Count; i++)
                InspectorPanel.Controls[i].Hide();

            InspectorPanel.Controls[c_index].Show();
            InspectorPanel.Controls[c_index].BringToFront();

            switch(mode)
            {
                case MAPEDIT_MODE.HMAP:
                    LabelMode.Text = "Edit heightmap";
                    break;
                case MAPEDIT_MODE.TEXTURE:
                    LabelMode.Text = "Edit textures";
                    break;
                case MAPEDIT_MODE.FLAG:
                    LabelMode.Text = "Edit terrain flags";
                    break;
                case MAPEDIT_MODE.LAKE:
                    LabelMode.Text = "Edit lakes";
                    break;
                case MAPEDIT_MODE.UNIT:
                    LabelMode.Text = "Edit units";
                    break;
                case MAPEDIT_MODE.BUILDING:
                    LabelMode.Text = "Edit buildings";
                    break;
                case MAPEDIT_MODE.OBJECT:
                    LabelMode.Text = "Edit objects";
                    break;
                case MAPEDIT_MODE.DECAL:
                    LabelMode.Text = "Edit decorations";
                    break;
                case MAPEDIT_MODE.NPC:
                    LabelMode.Text = "Edit NPCs";
                    break;
                case MAPEDIT_MODE.MISC_OBJECT:
                    LabelMode.Text = "Edit camps, bindstones, portals and monuments";
                    return;
                case MAPEDIT_MODE.META:
                    LabelMode.Text = "Edit metadata";
                    break;
                default:
                    break;
            }
        }

        private void ButtonHeightmap_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.HMAP);
        }

        private void ButtonTerrainTexture_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.TEXTURE);
        }

        private void ButtonFlag_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.FLAG);
        }

        private void ButtonLake_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.LAKE);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.UNIT);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.BUILDING);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.OBJECT);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.DECAL);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.NPC);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.MISC_OBJECT);
        }
        
        private void button11_Click(object sender, EventArgs e)
        {
            SetEditMode(MAPEDIT_MODE.META);
        }

        public void GoToBuildingProperties(SFMapBuilding bld)
        {
            SFMap.map_controls.MapInspectorBuildingControl b_inspector = (SFMap.map_controls.MapInspectorBuildingControl)edit_controls[5];
            SetEditMode(MAPEDIT_MODE.BUILDING);
            b_inspector.SelectBuilding(map.building_manager.buildings.IndexOf(bld), true);
        }

        public void GoToObjectProperties(SFMapObject obj)
        {
            SFMap.map_controls.MapInspectorObjectControl o_inspector = (SFMap.map_controls.MapInspectorObjectControl)edit_controls[6];
            SetEditMode(MAPEDIT_MODE.OBJECT);
            o_inspector.SelectObject(map.object_manager.objects.IndexOf(obj), true);
        }

        public void GoToUnitProperties(SFMapUnit unit)
        {
            SFMap.map_controls.MapInspectorUnitControl u_inspector = (SFMap.map_controls.MapInspectorUnitControl)edit_controls[4];
            SetEditMode(MAPEDIT_MODE.UNIT);
            u_inspector.SelectUnit(map.unit_manager.units.IndexOf(unit), true);
        }

        public SFMap.map_controls.MapInspectorBaseControl GetEditorControl(int mode)
        {
            return edit_controls[mode];
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

        private void autotextureform_FormClosing(object  sender, FormClosingEventArgs e)
        {
            autotexture_form.FormClosing -= new FormClosingEventHandler(autotextureform_FormClosing);
            autotexture_form = null;
        }
    }
}
