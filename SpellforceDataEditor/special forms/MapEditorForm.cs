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

namespace SpellforceDataEditor.special_forms
{
    public enum MAPEDIT_MODE { HMAP = 0, TEXTURE, FLAG, LAKE, UNIT, BUILDING, OBJECT, DECAL, NPC, MISC_OBJECT, META, MAX }

    public partial class MapEditorForm : Form
    {
        SFMap.SFMap map = null;
        SF3D.SFRender.SFRenderEngine render_engine = new SF3D.SFRender.SFRenderEngine();
        SFCFF.SFGameData gamedata = null;

        bool dynamic_render = false;     // if true, window will redraw every frame at 25 fps
        bool mouse_pressed = false;      // if true, mouse is pressed and in render window
        MouseButtons mouse_last_pressed = MouseButtons.Left;  // last mouse button pressed

        bool mouse_on_view = false;      // if true, mouse is in render window
        public bool update_render = false;
        Vector2 scroll_movement = new Vector2(0, 0);   // how much is camera going to move this frame
        SFCoord cursor_coord = new SFCoord(0, 0);      // map coordinates at mouse cursor
        int gc_timer = 0;

        MAPEDIT_MODE edit_mode = MAPEDIT_MODE.HMAP;
        SFMap.map_controls.MapInspectorBaseControl[] edit_controls = new SFMap.map_controls.MapInspectorBaseControl[(int)MAPEDIT_MODE.MAX];

        public MapEditorForm()
        {
            InitializeComponent();
            TimerAnimation.Enabled = true;
            TimerAnimation.Interval = 1000 / render_engine.scene_manager.frames_per_second;
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
                if (edit_controls[i] != null)
                {
                    InspectorPanel.Controls.Add(edit_controls[i]);
                    edit_controls[i].map = map;
                }

            for (int i = 0; i < InspectorPanel.Controls.Count; i++)
            { 
                InspectorPanel.Controls[i].Hide();
                InspectorPanel.Controls[i].Location = new Point(3, 3);
            }

            gamedata = SFCFF.SFCategoryManager.gamedata;

            SetEditMode(MAPEDIT_MODE.HMAP);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                                return;
                            }
                        }
                    }
                }
                else if (SFCFF.SFCategoryManager.ready)
                    SFCFF.SFCategoryManager.unload_all();

                StatusText.Text = "Loading GameData.cff...";
                StatusText.GetCurrentParent().Refresh();
                if (gamedata.Load(SFUnPak.SFUnPak.game_directory_name + "\\data\\GameData.cff") != 0)
                {
                    StatusText.Text = "Failed to load gamedata";
                    return;
                }

                if(MainForm.data != null)
                    MainForm.data.mapeditor_set_gamedata(gamedata);
                else
                    SFCFF.SFCategoryManager.manual_set_gamedata(gamedata);

                map = new SFMap.SFMap();
                try
                {
                    if (map.Load(OpenMap.FileName, render_engine, gamedata, StatusText) != 0)
                    {
                        StatusText.Text = "Failed to load map";
                        return;
                    }
                }
                catch (InvalidDataException)
                {
                    StatusText.Text = "Map contains invalid data!";
                    return;
                }
                render_engine.AssignHeightMap(map.heightmap);

                for (int i = 0; i < (int)MAPEDIT_MODE.MAX; i++)
                    if (edit_controls[i] != null)
                    {
                        InspectorPanel.Controls.Add(edit_controls[i]);
                        edit_controls[i].map = map;
                    }

                // ui initialization
                ((SFMap.map_controls.MapInspectorTerrainTextureControl)(edit_controls[1])).GenerateBaseTexturePreviews();
                ((SFMap.map_controls.MapInspectorTerrainTextureControl)(edit_controls[1])).GenerateTileListEntries();

                RenderWindow.Invalidate();

                if(MainForm.data != null)
                    MessageBox.Show("Note: Editor now operates on gamedata file in your Spellforce directory. Modifying in-editor gamedata and saving results will result in permanent change to your gamedata in your Spellforce directory.");
            }
        }

        private void MapEditorForm_Load(object sender, EventArgs e)
        {
            if (File.Exists("game_directory.txt"))
            {
                string gamedir = File.ReadAllText("game_directory.txt");
                int result = SFUnPak.SFUnPak.SpecifyGameDirectory(gamedir); //render_engine.SpecifyGameDirectory(dname);
                if (result == 0)
                {
                    //ready = true;
                    SFResources.SFResourceManager.FindAllMeshes();
                    SFLua.SFLuaEnvironment.Init();
                }
                else
                    Close();
                //return result;
            }

            render_engine.Initialize(new Vector2(RenderWindow.ClientSize.Width, RenderWindow.ClientSize.Height));
            render_engine.camera.Position = new Vector3(0, 25, 12);
            render_engine.camera.Lookat = new Vector3(0, 0, 0);
            render_engine.scene_manager.gamedata = gamedata;

            RenderWindow.Invalidate();
        }

        private void RenderWindow_Paint(object sender, PaintEventArgs e)
        {
            RenderWindow.MakeCurrent();
            render_engine.RenderFrame();
            RenderWindow.SwapBuffers();
        }

        private void RenderWindow_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_pressed = true;
            mouse_last_pressed = e.Button;
        }

        private void RenderWindow_MouseUp(object sender, MouseEventArgs e)
        {
            mouse_pressed = false;
            edit_controls[(int)edit_mode].OnMouseUp();
            update_render = true;
        }

        private void RenderWindow_MouseMove(object sender, MouseEventArgs e)
        {  
            if (!mouse_on_view)
                return;

            float cam_speed = 51; //limited by framerate
            Vector2 camera_movement = new Vector2(0, 0);
            // location of mouse relative to window

            float px, py;
            px = Cursor.Position.X - this.Location.X - RenderWindow.Location.X;
            py = Cursor.Position.Y - this.Location.Y - RenderWindow.Location.Y;
            if (px < 32)
                camera_movement.X = -cam_speed / render_engine.scene_manager.frames_per_second;
            else if (px > RenderWindow.Size.Width - 32)
                camera_movement.X = cam_speed / render_engine.scene_manager.frames_per_second;
            if (py < 64)
                camera_movement.Y = -cam_speed / render_engine.scene_manager.frames_per_second;
            else if (py > RenderWindow.Size.Height - 32)
                camera_movement.Y = cam_speed / render_engine.scene_manager.frames_per_second;

            scroll_movement = camera_movement;
            if (camera_movement != new Vector2(0, 0))
                EnableAnimation();
            else
                DisableAnimation();
            
        }

        private void RenderWindow_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {

        }

        private void EnableAnimation()
        {
            dynamic_render = true;
            render_engine.scene_manager.delta_timer.Restart();
        }

        private void DisableAnimation()
        {
            dynamic_render = false;
            render_engine.scene_manager.delta_timer.Stop();
            scroll_movement = new Vector2(0, 0);
        }

        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (mouse_pressed)
            {
                // generate ray
                float px, py;
                px = Cursor.Position.X - this.Location.X - RenderWindow.Location.X;
                py = Cursor.Position.Y - this.Location.Y - RenderWindow.Location.Y;
                float wx, wy;
                wx = px / RenderWindow.Size.Width;
                wy = py / RenderWindow.Size.Height;
                Vector3[] frustrum_vertices = render_engine.camera.FrustrumVertices;
                Vector3 r_start = render_engine.camera.Position;
                Vector3 r_end = frustrum_vertices[4]
                    + wx * (frustrum_vertices[5] - frustrum_vertices[4])
                    + wy * (frustrum_vertices[6] - frustrum_vertices[4]);
                SF3D.Physics.Ray ray = new SF3D.Physics.Ray(r_start, r_end - r_start) { length = 1000 };
                // collide with every visible chunk
                Vector3 result = new Vector3(0, 0, 0);
                Vector3 offset;
                bool ray_success = false;
                for (int i = 0; i < map.heightmap.visible_chunks.Count; i++)
                {
                    SFMapHeightMapChunk chunk = map.heightmap.visible_chunks[i];
                    offset = new Vector3(chunk.ix * 16, 0, chunk.iy * 16);
                    if (ray.Intersect(chunk.vertices, offset, out result))
                    {
                        ray_success = true;
                        result += offset;
                        break;
                    }
                }
                if (ray_success)
                {
                    cursor_coord = new SFCoord((int)result.X, (int)result.Z);
                    StatusText.Text = "MAP POS: " + cursor_coord.ToString();

                    // on click action
                    edit_controls[(int)edit_mode].OnMouseDown(cursor_coord, mouse_last_pressed);
                    update_render = true;
                }
            }

            if ((dynamic_render)||(update_render))
            {
                map.selection_helper.UpdateSelection();
                render_engine.camera.translate(new Vector3(scroll_movement.X, 0, scroll_movement.Y));
                AdjustCameraZ();
                render_engine.scene_manager.LogicStep();
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
                Vector2 p = new Vector2(render_engine.camera.Position.X, render_engine.camera.Position.Z);
                float z = map.heightmap.GetRealZ(p);
                render_engine.camera.translate(new Vector3(0, 25+z - render_engine.camera.Position.Y, 0));
            }
        }

        public void SetCameraViewPoint(SFCoord pos)
        {
            Vector3 new_camera_pos = new Vector3(pos.x, 0, map.heightmap.height - pos.y - 1 + 12);
            render_engine.camera.translate(new_camera_pos - render_engine.camera.Position);
            AdjustCameraZ();
            update_render = true;
        }

        private void MapEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (map != null)
            {
                map.Unload();
                if (MainForm.data != null)
                {
                    MainForm.data.close_data();
                    MainForm.data.mapeditor_desynchronize();
                }
                else
                    SFCFF.SFCategoryManager.unload_all();
                render_engine.scene_manager.ClearScene();
                render_engine.AssignHeightMap(null);
                if (MainForm.viewer != null)
                    MainForm.viewer.ResetScene();
                // for good measure (bad! bad!)
                SFResources.SFResourceManager.DisposeAll();
                GC.Collect();
            }
        }

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (SaveMap.ShowDialog() == DialogResult.OK)
            {
                StatusText.Text = "Saving the map...";
                if (map.Save(SaveMap.FileName) != 0)
                    StatusText.Text = "Failed to save map";
                else
                    StatusText.Text = SaveMap.FileName+ " saved successfully";
                if(MainForm.data != null)
                {
                    if(MainForm.data.data_changed)
                    {
                        // important: remove created NPC IDs that are not used
                        foreach(int npc_id in ((SFMap.map_controls.MapInspectorNPCControl)edit_controls[8]).created_npc_ids)
                            if (!map.npc_manager.npc_info.ContainsKey(npc_id))
                                ((SFMap.map_controls.MapInspectorNPCControl)edit_controls[8]).RemoveNewNPCID(npc_id);
                        MainForm.data.save_data();
                        ((SFMap.map_controls.MapInspectorNPCControl)edit_controls[8]).created_npc_ids.Clear();
                    }
                }
            }
        }

        private void ResizeWindow()
        {
            int ystart = RenderWindow.Location.Y;
            int yend = StatusStrip.Location.Y;
            int w_size = yend - ystart - 3;
            RenderWindow.Location = new Point(this.Size.Width - w_size - 1, ystart);
            RenderWindow.Size = new Size(w_size, w_size);
            PanelModes.Size = new Size(PanelModes.Size.Width, w_size + 3);
            InspectorPanel.Size = new Size(RenderWindow.Location.X - PanelModes.Size.Width - 12, w_size);
            render_engine.ResizeView(new Vector2(w_size, w_size));
            RenderWindow.Invalidate();
        }

        private void MapEditorForm_Resize(object sender, EventArgs e)
        {
            ResizeWindow();
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
                    LabelMode.Text = "Edit Heightmap";
                    break;
                case MAPEDIT_MODE.TEXTURE:
                    LabelMode.Text = "Edit terrain textures";
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
    }
}
