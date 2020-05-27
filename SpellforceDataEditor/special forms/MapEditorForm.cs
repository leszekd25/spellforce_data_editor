﻿using OpenTK;
using SpellforceDataEditor.SF3D.SFRender;
using SpellforceDataEditor.SFMap;
using SpellforceDataEditor.SFMap.MapEdit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SpellforceDataEditor.special_forms
{
    public enum MapEditMainMode { TERRAIN, TEXTURES, ENTITIES, MISC };
    public enum MapEditTerrainMode { HEIGHTMAP, FLAGS, LAKES, WEATHER };
    public struct SpecialKeysPressed
    {
        public bool Ctrl;
        public bool Shift;
    }

    public partial class MapEditorForm : Form
    {
        public class MapEditorUI
        {
            public enum MapEditorUIIconType { UNIT = 0, BUILDING = 1 }
            public const int COLOR_ENEMY = 0;
            public const int COLOR_NEUTRAL = 1;
            public const int COLOR_ALLY = 2;
            public const int COLOR_MONUMENT = 3;
            public const int COLOR_PORTAL = 4;
            public const int COLOR_WHITE = 5;

            static Vector4[] minimap_icons_colors =
            {
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 1.0f, 0.3f, 1.0f),
                new Vector4(1.0f, 1.0f, 0.7f, 1.0f),
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                new Vector4(0.8f, 0.0f, 0.8f, 1.0f)  // ERROR
            };


            SF3D.UI.UIElementIndex image_minimap;
            SF3D.UI.UIElementIndex image_minimap_frame_left;
            SF3D.UI.UIElementIndex image_minimap_frame_top;
            SF3D.UI.UIElementIndex image_minimap_icons_border;
            SF3D.UI.UIElementIndex image_minimap_icons;
            int next_icon = 0;
            bool icons_visible = true;

            // minimap texture
            public SF3D.SFTexture minimap_tex { get; private set; } = null;
            public SF3D.SFTexture minimap_icons_tex { get; private set; } = null;

            SFMap.SFMap map = null;
            int minimap_size;
            bool resizing = false;
            bool clicked = false;
            SFCoord clicked_pos = new SFCoord(0, 0);

            public MapEditorUI(SFMap.SFMap _map)
            {
                map = _map;
            }

            public void InitMinimap(int m_width, int m_height)
            {
                minimap_tex = SF3D.SFTexture.RGBAImage((ushort)m_width, (ushort)m_height);
                minimap_tex.Init();
                minimap_tex.SetName("minimap");

                SFRenderEngine.ui.AddStorage(minimap_tex, 1);
                SFRenderEngine.ui.AddStorage(SFRenderEngine.opaque_tex, 2);

                image_minimap = SFRenderEngine.ui.AddElementImage(minimap_tex, new Vector2(m_width, m_height), new Vector2(0, 0), new Vector2(0, 0), true);
                image_minimap_frame_left = SFRenderEngine.ui.AddElementImage(SFRenderEngine.opaque_tex, new Vector2(3, m_height), new Vector2(0, 0), new Vector2(0, 0), false);
                image_minimap_frame_top = SFRenderEngine.ui.AddElementImage(SFRenderEngine.opaque_tex, new Vector2(m_width, 3), new Vector2(0, 0), new Vector2(0, 0), false);

                // minimap icons
                int tex_code = SFResources.SFResourceManager.Textures.Load("ui_oth1", (int)1);
                if ((tex_code != 0) && (tex_code != -1))
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SF3D, "MapEditorUI.InitMinimap(): Could not load texture (texture name = ui_oth1)");
                    throw new Exception("MapEditorUI.InitMinimap(): Could not load texture ui_oth1");
                }
                minimap_icons_tex = SFResources.SFResourceManager.Textures.Get("ui_oth1");

                // 2000 units, 1000 buildings, 500 interactive objects, 100 portals
                SFRenderEngine.ui.AddStorage(minimap_icons_tex, 7200);
                image_minimap_icons_border = SFRenderEngine.ui.AddElementMulti(minimap_icons_tex, 3600);
                image_minimap_icons = SFRenderEngine.ui.AddElementMulti(minimap_icons_tex, 3600);
            }

            // full map redraw
            public void RedrawMinimap()
            {
                if (minimap_tex == null)
                    return;

                SFMapHeightMap hmap = map.heightmap;
                Color col;
                SFCoord coord;
                for (int i = 0; i < hmap.width; i++)
                    for (int j = 0; j < hmap.height; j++)
                    {
                        coord = new SFCoord(i, j);
                        float col_str = 1.0f;
                        if (hmap.GetZ(coord) > 300)
                        {
                            col = hmap.texture_manager.tile_average_color[hmap.GetTileFixed(coord)];

                            // shading
                            Vector3 normal = hmap.GetVertexNormal(i, j);
                            col_str = (Vector3.Dot(normal, new Vector3(0, 1, 0)) / 2) + 0.5f;
                        }
                        else
                            col = hmap.texture_manager.tile_ocean_color;

                        minimap_tex.data[(j * hmap.width + i) * 4 + 0] = (byte)(col.R * col_str);
                        minimap_tex.data[(j * hmap.width + i) * 4 + 1] = (byte)(col.G * col_str);
                        minimap_tex.data[(j * hmap.width + i) * 4 + 2] = (byte)(col.B * col_str);
                        minimap_tex.data[(j * hmap.width + i) * 4 + 3] = 255;
                    }

                foreach(SFMapLake lake in map.lake_manager.lakes)
                {
                    col = map.lake_manager.GetLakeMinimapColor(lake.type);
                    foreach(SFCoord p in lake.cells)
                    {
                        minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 0] = col.R;
                        minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 1] = col.G;
                        minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 2] = col.B;
                        minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 3] = 255;
                    }
                }

                minimap_tex.UpdateImage();
            }

            // redraw only selected pixels, and set them to chosen color
            public void RedrawMinimap(HashSet<SFCoord> pixels, byte tile_id)
            {
                if (minimap_tex == null)
                    return;

                SFMapHeightMap hmap = map.heightmap;

                Color col = hmap.texture_manager.tile_average_color[tile_id];
                foreach (SFCoord p in pixels)
                {
                    int i = p.x;
                    int j = p.y;
                    if (hmap.lake_data[j * hmap.width + i] != 0)
                        continue;
                    // shading
                    if (hmap.GetZ(p) > 300)
                    {
                        Vector3 normal = hmap.GetVertexNormal(i, j);
                        float col_str = (Vector3.Dot(normal, new Vector3(0, 1, 0)) / 2) + 0.5f;

                        minimap_tex.data[(j * hmap.width + i) * 4 + 0] = (byte)(col.R * col_str);
                        minimap_tex.data[(j * hmap.width + i) * 4 + 1] = (byte)(col.G * col_str);
                        minimap_tex.data[(j * hmap.width + i) * 4 + 2] = (byte)(col.B * col_str);
                        minimap_tex.data[(j * hmap.width + i) * 4 + 3] = 255;
                    }
                    else
                    {
                        minimap_tex.data[(j * hmap.width + i) * 4 + 0] = hmap.texture_manager.tile_ocean_color.R;
                        minimap_tex.data[(j * hmap.width + i) * 4 + 1] = hmap.texture_manager.tile_ocean_color.G;
                        minimap_tex.data[(j * hmap.width + i) * 4 + 2] = hmap.texture_manager.tile_ocean_color.B;
                        minimap_tex.data[(j * hmap.width + i) * 4 + 3] = 255;
                    }
                }

                minimap_tex.UpdateImage();
            }

            // only redraw selected pixels
            public void RedrawMinimap(HashSet<SFCoord> pixels)
            {
                if (minimap_tex == null)
                    return;

                SFMapHeightMap hmap = map.heightmap;

                foreach (SFCoord p in pixels)
                {
                    int i = p.x;
                    int j = p.y;
                    if (hmap.lake_data[j * hmap.width + i] != 0)
                    {
                        Color col = map.lake_manager.GetLakeMinimapColor(map.lake_manager.lakes[hmap.lake_data[j*hmap.width+i]-1].type);

                        minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 0] = col.R;
                        minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 1] = col.G;
                        minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 2] = col.B;
                        minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 3] = 255;
                    }
                    else
                    {
                        Color col = hmap.texture_manager.tile_average_color[hmap.GetTileFixed(p)];
                        // shading
                        if (hmap.GetZ(p) > 300)
                        {
                            Vector3 normal = hmap.GetVertexNormal(i, j);
                            float col_str = (Vector3.Dot(normal, new Vector3(0, 1, 0)) / 2) + 0.5f;

                            minimap_tex.data[(j * hmap.width + i) * 4 + 0] = (byte)(col.R * col_str);
                            minimap_tex.data[(j * hmap.width + i) * 4 + 1] = (byte)(col.G * col_str);
                            minimap_tex.data[(j * hmap.width + i) * 4 + 2] = (byte)(col.B * col_str);
                            minimap_tex.data[(j * hmap.width + i) * 4 + 3] = 255;
                        }
                        else
                        {
                            minimap_tex.data[(j * hmap.width + i) * 4 + 0] = hmap.texture_manager.tile_ocean_color.R;
                            minimap_tex.data[(j * hmap.width + i) * 4 + 1] = hmap.texture_manager.tile_ocean_color.G;
                            minimap_tex.data[(j * hmap.width + i) * 4 + 2] = hmap.texture_manager.tile_ocean_color.B;
                            minimap_tex.data[(j * hmap.width + i) * 4 + 3] = 255;
                        }
                    }
                }

                minimap_tex.UpdateImage();
            }

            public void ClearMinimapIcons()
            {
                SFRenderEngine.ui.ClearElementMulti(image_minimap_icons_border);
                SFRenderEngine.ui.ClearElementMulti(image_minimap_icons);
                next_icon = 0;
            }

            public void AddMinimapIcon(MapEditorUIIconType icon_type, int color_index, SFCoord pos)
            {
                if (next_icon >= 3600)
                    return;

                pos = new SFCoord(pos.x, map.height - pos.y - 1);
                Vector2 final_icon_center_offset = new Vector2((float)Math.Round((float)(pos.x * minimap_size) / minimap_tex.width), (float)Math.Round((float)(pos.y * minimap_size) / minimap_tex.height));
                if(icon_type == MapEditorUIIconType.BUILDING)
                {
                    SFRenderEngine.ui.SetElementMultiQuad(image_minimap_icons_border, next_icon, new Vector2(7, 7), -(final_icon_center_offset - new Vector2(3, 3)), new Vector2(244, 54), new Vector2(251, 61), minimap_icons_colors[color_index]);
                    SFRenderEngine.ui.SetElementMultiQuad(image_minimap_icons, next_icon, new Vector2(5, 5), -(final_icon_center_offset - new Vector2(2, 2)), new Vector2(245, 43), new Vector2(250, 48), minimap_icons_colors[color_index]);
                }
                else
                {
                    SFRenderEngine.ui.SetElementMultiQuad(image_minimap_icons_border, next_icon, new Vector2(4, 4), -(final_icon_center_offset - new Vector2(1, 1)), new Vector2(244, 94), new Vector2(248, 98), minimap_icons_colors[color_index]);
                    SFRenderEngine.ui.SetElementMultiQuad(image_minimap_icons, next_icon, new Vector2(2, 2), -(final_icon_center_offset - new Vector2(0, 0)), new Vector2(245, 87), new Vector2(247, 89), minimap_icons_colors[color_index]);
                }

                next_icon += 1;
            }

            private int GetUnitRelationToMainChar(int unit_id)
            {
                // clan player = 11
                var unit_data = SFCFF.SFCategoryManager.gamedata[17].FindElementBinary(0, (ushort)unit_id);
                if (unit_data == null)
                    return 6;
                var unit_stats_data = SFCFF.SFCategoryManager.gamedata[3].FindElementBinary(0, (ushort)unit_data[2]);
                if (unit_stats_data == null)
                    return 6;
                var race_data = SFCFF.SFCategoryManager.gamedata[15].FindElementBinary(0, (byte)unit_stats_data[2]);
                if (race_data == null)
                    return 6;
                var clan_data = SFCFF.SFCategoryManager.gamedata[16].FindElementBinary(0, (byte)((ushort)race_data[9]));
                if (clan_data == null)
                    return 6;
                var player_relation = (byte)clan_data[32];
                if (player_relation == 0)
                    return 1;
                if (player_relation == 100)
                    return 2;
                if (player_relation == 156)
                    return 0;
                return 6;
            }

            private int GetBuildingRelationToMainChar(SFMapBuilding bld)
            {
                // clan player = 11
                SFCFF.SFCategoryElement race_data;
                if (bld.race_id == 0)
                {
                    var building_data = SFCFF.SFCategoryManager.gamedata[23].FindElementBinary(0, (ushort)bld.game_id);
                    if (building_data == null)
                        return 6;
                    race_data = SFCFF.SFCategoryManager.gamedata[15].FindElementBinary(0, (byte)building_data[1]);
                }
                else
                    race_data = SFCFF.SFCategoryManager.gamedata[15].FindElementBinary(0, (byte)bld.race_id);
                if (race_data == null)
                    return 6;
                var clan_data = SFCFF.SFCategoryManager.gamedata[16].FindElementBinary(0, (byte)((ushort)race_data[9]));
                if (clan_data == null)
                    return 6;
                var player_relation = (byte)clan_data[32];
                if (player_relation == 0)
                    return 1;
                if (player_relation == 100)
                    return 2;
                if (player_relation == 156)
                    return 0;
                return 6;
            }

            public void RedrawMinimapIcons()
            {
                ClearMinimapIcons();
                foreach (var unit in map.unit_manager.units)
                    AddMinimapIcon(MapEditorUIIconType.UNIT, GetUnitRelationToMainChar(unit.game_id), unit.grid_position);
                foreach (var building in map.building_manager.buildings)
                    AddMinimapIcon(MapEditorUIIconType.BUILDING, GetBuildingRelationToMainChar(building), building.grid_position);
                foreach(var obj in map.object_manager.objects)
                {
                    if (obj.npc_id > 0)
                        AddMinimapIcon(MapEditorUIIconType.UNIT, 5, obj.grid_position);
                }
                foreach (var iobject in map.int_object_manager.int_objects)
                    AddMinimapIcon(MapEditorUIIconType.BUILDING, 3, iobject.grid_position);
                foreach (var portal in map.portal_manager.portals)
                    AddMinimapIcon(MapEditorUIIconType.BUILDING, 4, portal.grid_position);
                SFRenderEngine.ui.UpdateElementAll(image_minimap_icons_border);
                SFRenderEngine.ui.UpdateElementAll(image_minimap_icons);

            }

            public void SetMinimapVisible(bool visible)
            {
                if (minimap_tex == null)
                    return;

                SFRenderEngine.ui.SetElementVisible(image_minimap, visible);
                SFRenderEngine.ui.SetElementVisible(image_minimap_frame_left, visible);
                SFRenderEngine.ui.SetElementVisible(image_minimap_frame_top, visible);

                SFRenderEngine.ui.SetElementVisible(image_minimap_icons_border, visible & icons_visible);
                SFRenderEngine.ui.SetElementVisible(image_minimap_icons, visible & icons_visible);
            }

            public void SetMinimapIconsVisible(bool visible)
            {
                if (minimap_tex == null)
                    return;

                icons_visible = visible;
                SFRenderEngine.ui.SetElementVisible(image_minimap_icons_border, GetMinimapVisible() & icons_visible);
                SFRenderEngine.ui.SetElementVisible(image_minimap_icons, GetMinimapVisible() & icons_visible);
            }

            public void SetMinimapSize(int size)
            {
                if (minimap_tex == null)
                    return;

                minimap_size = Math.Min((int)Math.Min(SFRenderEngine.render_size.X-3, SFRenderEngine.render_size.Y-3), size);
                size = minimap_size;

                SFRenderEngine.ui.SetImageSize(image_minimap, new Vector2(size, size));
                SFRenderEngine.ui.SetImageSize(image_minimap_frame_left, new Vector2(3, size));
                SFRenderEngine.ui.SetImageSize(image_minimap_frame_top, new Vector2(size+3, 3));

                SFRenderEngine.ui.MoveElement(image_minimap, new Vector2(SFRenderEngine.render_size.X - size, SFRenderEngine.render_size.Y - size));
                SFRenderEngine.ui.MoveElement(image_minimap_frame_left, new Vector2(SFRenderEngine.render_size.X - size - 3, SFRenderEngine.render_size.Y - size));
                SFRenderEngine.ui.MoveElement(image_minimap_frame_top, new Vector2(SFRenderEngine.render_size.X - size - 3, SFRenderEngine.render_size.Y - size - 3));
                SFRenderEngine.ui.MoveElement(image_minimap_icons_border, new Vector2(SFRenderEngine.render_size.X - size, SFRenderEngine.render_size.Y - size));
                SFRenderEngine.ui.MoveElement(image_minimap_icons, new Vector2(SFRenderEngine.render_size.X - size, SFRenderEngine.render_size.Y - size));

                SFRenderEngine.ui.UpdateElementGeometry(image_minimap);
                SFRenderEngine.ui.UpdateElementGeometry(image_minimap_frame_left);
                SFRenderEngine.ui.UpdateElementGeometry(image_minimap_frame_top);
            }

            public void ExportMinimap(string name)
            {
                if (minimap_tex == null)
                    return;

                minimap_tex.Export(name);
            }

            public void OnResize()
            {
                SetMinimapSize(minimap_size);
            }

            public bool ProcessInput(float mx, float my, bool mouse_pressed)
            {
                if (minimap_tex == null)
                    return false;
                if (!SFRenderEngine.ui.GetElementVisible(image_minimap))
                    return false;

                if (resizing)
                {
                    if (mouse_pressed == false)
                    {
                        resizing = false;
                        RedrawMinimapIcons();
                    }
                    else
                    {
                        float s = Math.Max(SFRenderEngine.render_size.X - mx, SFRenderEngine.render_size.Y - my);
                        if ((int)s != minimap_size)
                        {
                            SetMinimapSize((int)s);
                            MainForm.mapedittool.update_render = true;
                        }
                    }

                    return true;
                }

                if((Math.Abs(mx-(SFRenderEngine.render_size.X-minimap_size)) < 16)&&(Math.Abs(my-(SFRenderEngine.render_size.Y-minimap_size)) < 16))
                {
                    resizing = true;
                    return true;
                }

                if (!mouse_pressed)
                    clicked = false;
                if ((mx > SFRenderEngine.render_size.X - minimap_size) && (my > SFRenderEngine.render_size.Y - minimap_size))
                {
                    if ((mouse_pressed) && (!clicked))
                    {
                        clicked_pos = new SFCoord(
                            SFRenderEngine.scene.heightmap.width -  (int)(((SFRenderEngine.render_size.X - mx)) * (SFRenderEngine.scene.heightmap.width / (float)minimap_size)) - 1,
                            (int)((SFRenderEngine.render_size.Y - my) * (SFRenderEngine.scene.heightmap.height / (float)minimap_size)));
                        clicked = true;
                    }

                    return true;
                }

                return false;
            }

            public bool GetMinimapPosClicked(ref SFCoord pos)
            {
                if (clicked)
                    pos = clicked_pos;

                return clicked;
            }

            public bool GetMinimapVisible()
            {
                return SFRenderEngine.ui.GetElementVisible(image_minimap);
            }

            public bool GetMinimapIconsVisible()
            {
                return icons_visible;
            }

            public void Dispose()
            {
                map = null;
                if(minimap_tex != null)
                    minimap_tex.Dispose();
                if (minimap_icons_tex != null)
                    minimap_icons_tex.Dispose();
            }
        }




        SFMap.SFMap map = null;
        SFCFF.SFGameData gamedata = null;

        bool dynamic_render = false;     // if true, window will redraw every frame at 25 fps
        bool mouse_pressed = false;      // if true, mouse is pressed and in render window
        MouseButtons mouse_last_pressed = MouseButtons.Left;  // last mouse button pressed

        bool mouse_on_view = false;      // if true, mouse is in render window
        Vector2 scroll_mouse_start = new Vector2(0, 0);
        bool mouse_scroll = false;
        public float zoom_level = 1.0f;
        float camera_speed_factor = 1.0f;

        bool[] arrows_pressed = new bool[] { false, false, false, false };  // left, right, up, down
        bool[] rotation_pressed = new bool[] { false, false, false, false };// left, right, up, down
        SpecialKeysPressed special_pressed = new SpecialKeysPressed();

        public bool update_render = false;  // whenever this is true, window will be repainted, and this switched to false
        int gc_timer = 0;                   // when this reaches 8x fps, garbage collector runs
        int updates_this_second = 0;

        public MapEditor selected_editor { get; private set; } = new MapHeightMapEditor();
        public SFMap.map_controls.MapInspector selected_inspector { get; private set; } = null;

        MapBrush terrain_brush = new MapBrush();

        SFMap.map_dialog.MapAutoTextureDialog autotexture_form = null;
        SFMap.map_dialog.MapManageTeamCompositions teamcomp_form = null;
        SFMap.map_dialog.MapVisibilitySettings visibility_form = null;
        SFMap.map_dialog.MapImportHeightmapDialog importhmap_form = null;
        SFMap.map_dialog.MapExportHeightmapDialog exporthmap_form = null;

        Dictionary<string, TreeNode> unit_tree = null;
        Dictionary<string, TreeNode> building_tree = null;
        Dictionary<string, TreeNode> obj_tree = null;

        SFMapQuickSelectHelper qs_unit = new SFMapQuickSelectHelper();
        SFMapQuickSelectHelper qs_building = new SFMapQuickSelectHelper();
        SFMapQuickSelectHelper qs_object = new SFMapQuickSelectHelper();

        public MapEditorUI ui { get; private set; } = null;

        public MapEditorForm()
        {
            InitializeComponent();
        }

        private void MapEditorForm_Load(object sender, EventArgs e)
        {
            SFLua.SFLuaEnvironment.LoadSQL(false);
            if (!SFLua.SFLuaEnvironment.data_loaded)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "MapEditorForm_Load(): Failed to load SQL data!");
                Close();
                throw new Exception("MapEditorForm_Load(): Failed to load SQL data");
            }

            TimerAnimation.Enabled = true;
            TimerAnimation.Interval = 1000 / SFRenderEngine.scene.frames_per_second;
            TimerAnimation.Start();

            gamedata = SFCFF.SFCategoryManager.gamedata;

            SFRenderEngine.scene.Init();
            CreateRenderWindow();
            InspectorHide();

            this.WindowState = FormWindowState.Maximized;

            LogUtils.Log.TotalMemoryUsage();
        }

        private void MapEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseMap() != 0)
                e.Cancel = true;
            else
            {
                selected_editor = null;

                SFRenderEngine.scene.root = null;
                SFRenderEngine.scene.camera = null;
                SFRenderEngine.scene.heightmap = null;
                DestroyRenderWindow();
            }
        }

        private void MapEditorForm_Resize(object sender, EventArgs e)
        {
            TabEditorModes.Width = this.Width - 22;
            TabEditorModes.Padding = new Point(Math.Max(10, ((this.Width - 350)) / TabEditorModes.TabPages.Count / 2), TabEditorModes.Padding.Y);
            ResizeWindow();

            PanelUtility.Location = new Point(this.Width - PanelUtility.Width, StatusStrip.Location.Y);
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

            SFRenderEngine.scene.root.Visible = true;

            // create and generate map
            map = new SFMap.SFMap();
            map.CreateDefault(map_size, generator, gamedata, StatusText);

            SFRenderEngine.scene.heightmap = map.heightmap;
            InitEditorMode();

            map.selection_helper.SetCursorPosition(new SFCoord(1, 1));
            map.selection_helper.SetCursorVisibility(true);

            SFRenderEngine.scene.camera.SetParent(SFRenderEngine.scene.root);
            SetCameraViewPoint(new SFCoord(map.width / 2, map.height / 2));
            ResetCamera();

            ui = new MapEditorUI(map);
            ui.InitMinimap(map.width, map.height);
            ui.SetMinimapSize(256);
            ui.RedrawMinimap();
            ui.RedrawMinimapIcons();

            RenderWindow.Invalidate();

            if (MainForm.data != null)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "MapEditorForm.CreateMap(): Synchronized with gamedata editor");
                MessageBox.Show("Note: Editor now operates on gamedata file in your Spellforce directory. Modifying in-editor gamedata and saving results will result in permanent change to your gamedata in your Spellforce directory.");
            }

            GC.Collect();
            this.Text = "Map Editor - new map";

            LogUtils.Log.TotalMemoryUsage();
            SFResources.SFResourceManager.LogMemoryUsage();
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
                {
                    SFCFF.SFCategoryManager.manual_SetGamedata(gamedata);
                }

                SFRenderEngine.scene.root.Visible = true;

                map = new SFMap.SFMap();
                try
                {
                    if (map.Load(OpenMap.FileName, gamedata, StatusText) != 0)
                    {
                        StatusText.Text = "Failed to load map";
                        map = null;
                        DestroyRenderWindow();
                        return -3;
                    }
                }
                catch (InvalidDataException)
                {
                    StatusText.Text = "Map contains invalid data!";
                    map = null;
                    DestroyRenderWindow();
                    return -4;
                }

                SFRenderEngine.scene.heightmap = map.heightmap;
                InitEditorMode();

                map.selection_helper.SetCursorPosition(new SFCoord(1, 1));
                map.selection_helper.SetCursorVisibility(true);

                SFRenderEngine.scene.camera.SetParent(SFRenderEngine.scene.root);
                SetCameraViewPoint(new SFCoord(map.width / 2, map.height / 2));
                ResetCamera();

                ui = new MapEditorUI(map);
                ui.InitMinimap(map.width, map.height);
                ui.SetMinimapSize(256);
                ui.RedrawMinimap();
                ui.RedrawMinimapIcons();

                RenderWindow.Invalidate();

                if (MainForm.data != null)
                {
                    LogUtils.Log.Info(LogUtils.LogSource.SFMap, "MapEditorForm.LoadMap(): Synchronized with gamedata editor");
                    MessageBox.Show("Note: Editor now operates on gamedata file in your Spellforce directory. Modifying in-editor gamedata and saving results will result in permanent change to your gamedata in your Spellforce directory.");
                }

                GC.Collect();

                this.Text = "Map Editor - " + OpenMap.FileName;

                LogUtils.Log.TotalMemoryUsage();
                SFResources.SFResourceManager.LogMemoryUsage();
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
            SFResources.SFResourceManager.LogMemoryUsage();

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
            if (importhmap_form != null)
                importhmap_form.Close();
            if (exporthmap_form != null)
                exporthmap_form.Close();

            TabEditorModes.Enabled = false;
            InspectorClear();

            TreeEntities.Nodes.Clear();
            unit_tree = null;
            obj_tree = null;

            map.Unload();
            if (MainForm.data != null)
            {
                MainForm.data.close_data();
                MainForm.data.mapeditor_desynchronize();
            }
            else
                SFCFF.SFCategoryManager.UnloadAll();

            SFRenderEngine.scene.heightmap = null;
            SFRenderEngine.scene.RemoveSceneNode(SFRenderEngine.scene.root, true);

            foreach (SF3D.SFTexture tex in SFRenderEngine.scene.tex_entries_simple.Keys)
                SFRenderEngine.scene.tex_entries_simple[tex].Clear();
            SFRenderEngine.scene.tex_entries_simple.Clear();
            //SFRenderEngine.scene.untex_entries_simple.Clear();

            //ui.UninitMinimap();
            if (ui != null)
            {
                ui.Dispose();
                ui = null;
            }
            SFRenderEngine.ui.Dispose();

            if (MainForm.viewer != null)
                MainForm.viewer.ResetScene();
            map = null;
            // for good measure (bad! bad!) (TODO: make this do nothing since all resources should be properly disposed at this point)
            SFResources.SFResourceManager.DisposeAll();
            //DestroyRenderWindow();
            this.Text = "Map Editor";
            GC.Collect();

            LogUtils.Log.TotalMemoryUsage();
            SFResources.SFResourceManager.LogMemoryUsage();

            return 0;
        }

        private void CreateRenderWindow()
        {
            RenderWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseDown);
            RenderWindow.MouseEnter += new System.EventHandler(this.RenderWindow_MouseEnter);
            RenderWindow.MouseLeave += new System.EventHandler(this.RenderWindow_MouseLeave);
            RenderWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseMove);
            RenderWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseUp);
            RenderWindow.MouseWheel += new MouseEventHandler(this.RenderWindow_MouseWheel);

            // it seems shaders must always be compiled upon creating new window
            SFRenderEngine.Initialize(new Vector2(RenderWindow.ClientSize.Width, RenderWindow.ClientSize.Height));
            TimerUpdatesPerSecond.Start();

            RenderWindow.MakeCurrent();
        }

        // after this is called, memory will be freed (?)
        private void DestroyRenderWindow()
        {
            RenderWindow.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseDown);
            RenderWindow.MouseEnter -= new System.EventHandler(this.RenderWindow_MouseEnter);
            RenderWindow.MouseLeave -= new System.EventHandler(this.RenderWindow_MouseLeave);
            RenderWindow.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseMove);
            RenderWindow.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseUp);
            RenderWindow.MouseWheel -= new MouseEventHandler(this.RenderWindow_MouseWheel);

            TimerUpdatesPerSecond.Stop();
            UpdatesText.Text = "";
        }

        private void RenderWindow_Paint(object sender, PaintEventArgs e)
        {
            //RenderWindow.MakeCurrent();   // needs to only be done during resize, because cant run asset viewer anyways :^)
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
            if(selected_editor != null)
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
            AddCameraZoom(e.Delta);
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

            TimerAnimation.Start();

            bool update_ui = false;

            // mouse actions
            if (mouse_on_view)
            {
                float px, py;
                px = Cursor.Position.X - this.Location.X - RenderWindow.Location.X - 8;
                py = Cursor.Position.Y - this.Location.Y - RenderWindow.Location.Y - 29;

                if (!ui.ProcessInput(px, py, mouse_pressed))
                {
                    //find point which mouse hovers at
                    float wx, wy;
                    wx = ((px / RenderWindow.Size.Width) + 0.1f) / 1.2f;
                    wy = ((py / RenderWindow.Size.Height) + 0.1f) / 1.2f;
                    Vector3[] frustrum_vertices = SFRenderEngine.scene.camera.FrustumVertices;
                    Vector3 r_start = SFRenderEngine.scene.camera.Position;
                    Vector3 r_end = frustrum_vertices[4]
                        + wx * (frustrum_vertices[5] - frustrum_vertices[4])
                        + wy * (frustrum_vertices[6] - frustrum_vertices[4]);
                    SF3D.Physics.Ray ray = new SF3D.Physics.Ray(r_start, r_end - r_start) { Length = 1000 };

                    Vector3 result = new Vector3(0, 0, 0);
                    bool ray_success = ray.Intersect(map.heightmap, out result);

                    if (ray_success)
                    {
                        SFCoord cursor_coord = new SFCoord(
                            (int)(Math.Max
                                (0, Math.Min
                                    (Math.Round(result.X), map.width - 1))),
                            (int)(Math.Max
                                (0, Math.Min
                                    (Math.Round(result.Z), map.height - 1))));
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
                            System.Diagnostics.Debug.WriteLine("IX " + map.heightmap.GetChunk(inv_cursor_coord).ix.ToString()
                                + " | IY " + map.heightmap.GetChunk(inv_cursor_coord).iy.ToString()
                                + " | RET "+ map.heightmap.GetChunk(inv_cursor_coord).pool_index.ToString()
                                + " | PRESS");
                            selected_editor.OnMousePress(inv_cursor_coord, mouse_last_pressed, ref special_pressed);
                            update_render = true;
                            update_ui = true;
                        }
                    }
                }
                else
                {
                    SFCoord clicked_pos = new SFCoord(0, 0);
                    if (ui.GetMinimapPosClicked(ref clicked_pos))
                        SetCameraViewPoint(clicked_pos);
                }
            }

            // rotating view by mouse
            if (mouse_scroll)
            {
                Vector2 scroll_mouse_end = new Vector2(Cursor.Position.X, Cursor.Position.Y);
                Vector2 scroll_translation = (scroll_mouse_end - scroll_mouse_start) * SFRenderEngine.scene.DeltaTime / 250f;

                SFRenderEngine.scene.camera.Direction += new Vector2(scroll_translation.X, -scroll_translation.Y);

                update_render = true;
                update_ui = true;
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
                movement_vector *= 60.0f * camera_speed_factor * SFRenderEngine.scene.DeltaTime;
                SFRenderEngine.scene.camera.translate(new Vector3(movement_vector.X, 0, movement_vector.Y));
                update_render = true;
                update_ui = true;
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
                movement_vector *= 2.0f * SFRenderEngine.scene.DeltaTime;
                SFRenderEngine.scene.camera.Direction += new Vector2(movement_vector.X, -movement_vector.Y);
                update_render = true;
                update_ui = true;
            }

            // render time
            if (update_render)
            {
                SFRenderEngine.scene.sun_light.ShadowSize = Math.Max(50f, Math.Min(zoom_level * 60f, 200f));
                map.selection_helper.Update();
                map.ocean.SetPosition(SFRenderEngine.scene.camera.Position);
                AdjustCameraZ();
                SFRenderEngine.UpdateVisibleChunks();
                SFRenderEngine.scene.Update();
                SFRenderEngine.ui.Update();
                RenderWindow.Invalidate();
                updates_this_second += 1;
                update_render = false;
            }

            if (dynamic_render)
                update_render = true;

            if (!update_ui)
                SFRenderEngine.scene.StopTimeFlow();
            else
                SFRenderEngine.scene.ResumeTimeFlow();

            // garbage collector
            gc_timer += 1;
            if (gc_timer >= 8*SFRenderEngine.scene.frames_per_second)
            {
                GC.Collect();
                gc_timer = 0;
            }
        }


        private void TimerUpdatesPerSecond_Tick(object sender, EventArgs e)
        {
            TimerUpdatesPerSecond.Start();
            UpdatesText.Text = "Updates / s: " + updates_this_second.ToString();
            updates_this_second = 0;
        }

        private void SetSpecificText(SFCoord pos)
        {
            byte dec_assign = map.decoration_manager.GetFixedDecAssignment(pos);
            SpecificText.Text = "H: " + map.heightmap.GetHeightAt(pos.x, pos.y).ToString() + "  "
                              + "T: " + map.heightmap.GetTileFixed(pos).ToString() + "  "
                              + "D: " + (dec_assign == 0 ? "X" : dec_assign.ToString());
        }

        private void AddCameraZoom(int delta)
        {
            if(delta < 0)
                {
                    zoom_level *= 1.1f;
                    if(zoom_level > 6)
                        zoom_level = 6;
                }
            else
                {
                    zoom_level *= 0.9f;
                    if(zoom_level < 0.1f)
                        zoom_level = 0.1f;
                }
            AdjustCameraZ();
            update_render = true;
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

        private void TrackbarCameraSpeed_ValueChanged(object sender, EventArgs e)
        {
            camera_speed_factor = TrackbarCameraSpeed.Value / 100.0f;
        }

        private void ResizeWindow()
        {
            int ystart = RenderWindow.Location.Y;
            int yend = StatusStrip.Location.Y;
            int w_height = Math.Max(100, yend - ystart - 3);
            int w_width = Math.Max(100, this.Width - 22 - (PanelInspector.Visible ? PanelInspector.Width : 0)
                - (PanelObjectSelector.Visible ? PanelObjectSelector.Width : 0));
            int xstart = (PanelObjectSelector.Visible ? PanelObjectSelector.Location.X + PanelObjectSelector.Width + 3 : 0);
            PanelObjectSelector.Height = w_height;
            TreeEntities.Height = w_height - 32;
            TreeEntitytFilter.Location = new Point(TreeEntitytFilter.Location.X, w_height - 23);
            RenderWindow.Location = new Point(xstart, ystart);
            RenderWindow.Size = new Size(w_width, w_height);
            PanelInspector.Location = new Point(6 + RenderWindow.Width + (PanelObjectSelector.Visible ? PanelObjectSelector.Width : 0), ystart);

            SFRenderEngine.ResizeView(new Vector2(w_width, w_height));
            if(ui != null)
                ui.OnResize();
            update_render = true;
            RenderWindow.MakeCurrent();
        }

        private void slopebasedPaintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;
            if (autotexture_form != null)
            {
                autotexture_form.BringToFront();
                return;
            }
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
                case Keys.Insert:
                    AddCameraZoom(-1);
                    return true;
                case Keys.Delete:
                    AddCameraZoom(1);
                    return true;
                case Keys.G | Keys.Control:
                    Settings.DisplayGrid = !Settings.DisplayGrid;
                    SF3D.SFRender.SFRenderEngine.RecompileMainShaders();
                    update_render = true;
                    return true;
                case Keys.H | Keys.Control:
                    Settings.VisualizeHeight = !Settings.VisualizeHeight;
                    SF3D.SFRender.SFRenderEngine.RecompileMainShaders();
                    update_render = true;
                    return true;
                case Keys.M | Keys.Control:
                    ui.SetMinimapVisible(!ui.GetMinimapVisible());
                    update_render = true;
                    return true;
                case Keys.P | Keys.Control:
                    ui.ExportMinimap("minimap");
                    return true;
                case Keys.V | Keys.Control:
                    if (TabEditorModes.SelectedIndex == 2)
                        EntityHidePreview.Checked = !EntityHidePreview.Checked;
                    return true;
                case Keys.D1:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        EntityID.Text = QuickSelect_GetCurrent().ID[0].ToString();
                        ConfirmPlacementEntity();
                    }
                    return base.ProcessDialogKey(keyData);
                case Keys.D2:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        EntityID.Text = QuickSelect_GetCurrent().ID[1].ToString();
                        ConfirmPlacementEntity();
                    }
                    return base.ProcessDialogKey(keyData);
                case Keys.D3:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        EntityID.Text = QuickSelect_GetCurrent().ID[2].ToString();
                        ConfirmPlacementEntity();
                    }
                    return base.ProcessDialogKey(keyData);
                case Keys.D4:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        EntityID.Text = QuickSelect_GetCurrent().ID[3].ToString();
                        ConfirmPlacementEntity();
                    }
                    return base.ProcessDialogKey(keyData);
                case Keys.D5:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        EntityID.Text = QuickSelect_GetCurrent().ID[4].ToString();
                        ConfirmPlacementEntity();
                    }
                    return base.ProcessDialogKey(keyData);
                case Keys.D6:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        EntityID.Text = QuickSelect_GetCurrent().ID[5].ToString();
                        ConfirmPlacementEntity();
                    }
                    return base.ProcessDialogKey(keyData);
                case Keys.D7:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        EntityID.Text = QuickSelect_GetCurrent().ID[6].ToString();
                        ConfirmPlacementEntity();
                    }
                    return base.ProcessDialogKey(keyData);
                case Keys.D8:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        EntityID.Text = QuickSelect_GetCurrent().ID[7].ToString();
                        ConfirmPlacementEntity();
                    }
                    return base.ProcessDialogKey(keyData);
                case Keys.D9:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        EntityID.Text = QuickSelect_GetCurrent().ID[8].ToString();
                        ConfirmPlacementEntity();
                    }
                    return base.ProcessDialogKey(keyData);
                case Keys.D0:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        EntityID.Text = QuickSelect_GetCurrent().ID[9].ToString();
                        ConfirmPlacementEntity();
                    }
                    return base.ProcessDialogKey(keyData);
                case Keys.D1 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[0] = Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D2 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[1] = Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D3 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[2] = Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D4 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[3] = Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D5 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[4] = Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D6 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[5] = Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D7 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[6] = Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D8 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[7] = Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D9 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[8] = Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D0 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[9] = Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.ControlKey | Keys.Control:
                    special_pressed.Ctrl = true;
                    return true;
                case Keys.ShiftKey | Keys.Shift:
                    special_pressed.Shift = true;
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
                else if ((int)msg.WParam == 0x10) // shift
                    special_pressed.Shift = false;
                else if ((int)msg.WParam == 0x11) // ctrl
                    special_pressed.Ctrl = false;
            }
            return base.ProcessKeyPreview(ref msg);
        }

        private void ResetRotation_Click(object sender, EventArgs e)
        {
            ResetCamera();
        }



        // map editor controls below




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

            map.heightmap.overlay_active_texture = -1;
            update_render = true;
            PanelObjectSelector.Visible = false;

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

            EntityID.Text = "0";

            ConfirmPlacementEntity();
        }

        //TERRAIN EDIT

        private void ReselectTerrainMode()
        {
            map.heightmap.overlay_active_texture = -1;
            update_render = true;

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
            if(RadioWeather.Checked)
            {
                RadioWeather.Checked = false;
                RadioWeather.Checked = true;
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
            PanelWeather.Visible = false;

            terrain_brush.size = (float)Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();

            map.heightmap.overlay_active_texture = -1;
            update_render = true;
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
            PanelWeather.Visible = false;

            terrain_brush.size = (float)Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();

            map.heightmap.overlay_active_texture = map.heightmap.overlay_texture_flags;
            map.heightmap.ResetFlagOverlay();
            map.heightmap.RefreshOverlay();
            update_render = true;
        }

        private void RadioFlagMovement_CheckedChanged(object sender, EventArgs e)
        {
            ((MapTerrainFlagsEditor)selected_editor).FlagType = GetTerrainFlagType();
        }

        private void RadioFlagVision_CheckedChanged(object sender, EventArgs e)
        {
            ((MapTerrainFlagsEditor)selected_editor).FlagType = GetTerrainFlagType();
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
            PanelWeather.Visible = false;

            InspectorSet(new SFMap.map_controls.MapLakeInspector());

            map.heightmap.overlay_active_texture = -1;
            update_render = true;
        }

        // WEATHER
        private void RadioWeather_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioWeather.Checked)
                return;

            InspectorClear();

            selected_editor = null;

            PanelFlags.Visible = false;
            PanelBrushShape.Visible = false;
            PanelStrength.Visible = false;
            PanelTerrainSettings.Visible = false;
            PanelWeather.Visible = true;
            PanelWeather.Location = PanelBrushShape.Location;

            map.heightmap.overlay_active_texture = -1;
            update_render = true;

            UpdateWeatherPanel();
        }

        private void UpdateWeatherPanel()
        {
            WClear.Text = map.weather_manager.weather[0].ToString();
            WCloud.Text = map.weather_manager.weather[1].ToString();
            WStorm.Text = map.weather_manager.weather[2].ToString();
            WLavafog.Text = map.weather_manager.weather[3].ToString();
            WLavafogBright.Text = map.weather_manager.weather[4].ToString();
            WDesertfog.Text = map.weather_manager.weather[5].ToString();
            WSwampfog.Text = map.weather_manager.weather[6].ToString();
            WLavanight.Text = map.weather_manager.weather[7].ToString();
        }


        private void WClear_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[0] = Utility.TryParseUInt8(WClear.Text, map.weather_manager.weather[0]);
        }

        private void WCloud_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[1] = Utility.TryParseUInt8(WCloud.Text, map.weather_manager.weather[1]);
        }

        private void WStorm_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[2] = Utility.TryParseUInt8(WStorm.Text, map.weather_manager.weather[2]);
        }

        private void WLavafog_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[3] = Utility.TryParseUInt8(WLavafog.Text, map.weather_manager.weather[3]);
        }

        private void WLavafogBright_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[4] = Utility.TryParseUInt8(WLavafogBright.Text, map.weather_manager.weather[4]);
        }

        private void WDesertfog_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[5] = Utility.TryParseUInt8(WDesertfog.Text, map.weather_manager.weather[5]);
        }

        private void WSwampfog_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[6] = Utility.TryParseUInt8(WSwampfog.Text, map.weather_manager.weather[6]);
        }

        private void WLavanight_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[7] = Utility.TryParseUInt8(WLavanight.Text, map.weather_manager.weather[7]);
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

        public void SetTileType(SFMap.map_controls.TerrainTileType ttype)
        {
            if(ttype == SFMap.map_controls.TerrainTileType.BASE)
            {
                RadioTileTypeBase.Checked = false;
                RadioTileTypeBase.Checked = true;
            }
            else if(ttype == SFMap.map_controls.TerrainTileType.CUSTOM)
            {
                RadioTileTypeCustom.Checked = false;
                RadioTileTypeCustom.Checked = true;
            }
            else
            {
                RadioTileTypeBase.Checked = false;
                RadioTileTypeBase.Checked = true;
            }
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
            EntityHidePreview.Location = new Point(486, 94);
            QuickSelect.Location = new Point(PanelEntityPlacementSelect.Location.X + PanelEntityPlacementSelect.Width + 6, PanelEntityPlacementSelect.Location.Y);
            QuickSelect.QsRef = null;
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
            QuickSelect.UpdateIDs();
        }

        // quick select utilities
        private SFMapQuickSelectHelper QuickSelect_GetCurrent()
        {
            if (!TabEditorModes.Enabled)
                return null;
            if (TabEditorModes.SelectedIndex != 2)
                return null;

            if (RadioEntityModeUnit.Checked)
                return qs_unit;
            if (RadioEntityModeBuilding.Checked)
                return qs_building;
            if (RadioEntityModeObject.Checked)
                return qs_object;

            return null;
        }

        public void external_QuickSelect_OnSet(int index, ushort id)
        {
            if (QuickSelect_GetCurrent() == null)
                return;

            QuickSelect_GetCurrent().ID[index] = id;
        }

        public int external_QuickSelect_DetermineCategory()
        {
            int cat_id = -1;
            if (RadioEntityModeUnit.Checked)
                cat_id = 17;
            else if (RadioEntityModeObject.Checked)
                cat_id = 33;
            else if (RadioEntityModeBuilding.Checked)
                cat_id = 23;
            return cat_id;
        }

        // this tree code is very ugly, i wish you could instantiate TreeNodeCollection outside of TreeView
        // forgive me for i have sinned


        // load object picker tree
        private void GenerateUnitTree()
        {
            TreeEntities.Nodes.Clear();
            if (unit_tree != null)
            {
                Utility.TreeShallowCopy(unit_tree, TreeEntities.Nodes);
                return;
            }

            unit_tree = new Dictionary<string, TreeNode>();
            // generate race nodes
            SFCFF.SFCategory cat = SFCFF.SFCategoryManager.gamedata[15];
            for (int i = 0; i < cat.GetElementCount(); i++)
            {
                byte race_id = (byte)cat[i][0];
                ushort race_name_index = (ushort)(cat[i][7]);
                SFCFF.SFCategoryElement name_elem = SFCFF.SFCategoryManager.FindElementText(race_name_index, Settings.LanguageID);

                string race_name;
                if (name_elem != null)
                    race_name = Utility.CleanString(name_elem[4]);
                else
                    race_name = Utility.S_MISSING;

                race_name = race_id.ToString() + ". " + race_name;
                unit_tree.Add(race_name, new TreeNode(race_name));
                //UnitRace.Items.Add(race_name);

            }
            // generate unit nodes
            SFCFF.SFCategory units_cat = SFCFF.SFCategoryManager.gamedata[17];
            for (int i = 0; i < units_cat.GetElementCount(); i++)
            {
                ushort unit_id = (ushort)units_cat[i][0];
                string unit_name = unit_id.ToString() + ". " + SFCFF.SFCategoryManager.GetUnitName(unit_id, true);

                ushort stats_id = (ushort)(units_cat[i][2]);
                SFCFF.SFCategoryElement stats_elem = SFCFF.SFCategoryManager.gamedata[3].FindElementBinary(0, stats_id);
                if (stats_elem == null)
                {
                    unit_tree.Add(unit_name, new TreeNode(unit_name) { Tag = unit_id });
                    continue;
                }

                byte unit_race_id = (byte)stats_elem[2];
                int race_cat_index = cat.GetElementIndex(unit_race_id);

                ushort race_name_index = (ushort)(cat[race_cat_index][7]);
                SFCFF.SFCategoryElement name_elem = SFCFF.SFCategoryManager.FindElementText(race_name_index, Settings.LanguageID);
                string race_name;
                if (name_elem != null)
                    race_name = Utility.CleanString(name_elem[4]);
                else
                    race_name = Utility.S_MISSING;

                race_name = unit_race_id.ToString() + ". " + race_name;
                if (unit_tree.ContainsKey(race_name))
                    unit_tree[race_name].Nodes.Add(new TreeNode(unit_name) { Tag = unit_id });
                else
                    unit_tree.Add(unit_name, new TreeNode(unit_name) { Tag = unit_id });
            }            
            // clear empty nodes
            HashSet<string> nodes_to_remove = new HashSet<string>();
            foreach (string s in unit_tree.Keys)
                if ((unit_tree[s].Nodes.Count == 0) && (unit_tree[s].Tag == null))
                    nodes_to_remove.Add(s);
            foreach (string s in nodes_to_remove)
                unit_tree.Remove(s);

            Utility.TreeShallowCopy(unit_tree, TreeEntities.Nodes);
            GC.Collect();
        }

        private void GetUnitNodesByName(string txt)
        {
            if (txt == "")
            {
                GenerateUnitTree();
                return;
            }
            txt = txt.ToLower();

            TreeEntities.Nodes.Clear();
            // generate race nodes
            SFCFF.SFCategory cat = SFCFF.SFCategoryManager.gamedata[15];
            for (int i = 0; i < cat.GetElementCount(); i++)
            {
                byte race_id = (byte)cat[i][0];
                ushort race_name_index = (ushort)(cat[i][7]);
                SFCFF.SFCategoryElement name_elem = SFCFF.SFCategoryManager.FindElementText(race_name_index, Settings.LanguageID);

                string race_name;
                if (name_elem != null)
                    race_name = Utility.CleanString(name_elem[4]);
                else
                    race_name = Utility.S_MISSING;

                race_name = race_id.ToString() + ". " + race_name;
                TreeEntities.Nodes.Add(race_name, race_name);
                //UnitRace.Items.Add(race_name);

            }
            // generate unit nodes
            SFCFF.SFCategory units_cat = SFCFF.SFCategoryManager.gamedata[17];
            for (int i = 0; i < units_cat.GetElementCount(); i++)
            {
                ushort unit_id = (ushort)units_cat[i][0];
                string unit_name = unit_id.ToString() + ". " + SFCFF.SFCategoryManager.GetUnitName(unit_id, true);

                ushort stats_id = (ushort)(units_cat[i][2]);
                SFCFF.SFCategoryElement stats_elem = SFCFF.SFCategoryManager.gamedata[3].FindElementBinary(0, stats_id);
                if (stats_elem == null)
                {
                    if (unit_name.ToLower().Contains(txt))
                        TreeEntities.Nodes.Add(new TreeNode(unit_name) { Tag = unit_id });
                    continue;
                }

                byte unit_race_id = (byte)stats_elem[2];
                int race_cat_index = cat.GetElementIndex(unit_race_id);

                ushort race_name_index = (ushort)(cat[race_cat_index][7]);
                SFCFF.SFCategoryElement name_elem = SFCFF.SFCategoryManager.FindElementText(race_name_index, Settings.LanguageID);
                string race_name;
                if (name_elem != null)
                    race_name = Utility.CleanString(name_elem[4]);
                else
                    race_name = Utility.S_MISSING;

                race_name = unit_race_id.ToString() + ". " + race_name;

                if ((!race_name.ToLower().Contains(txt)) && (!unit_name.ToLower().Contains(txt)))
                    continue;

                if (TreeEntities.Nodes.ContainsKey(race_name))
                    TreeEntities.Nodes[race_name].Nodes.Add(new TreeNode(unit_name) { Tag = unit_id });
                else
                    TreeEntities.Nodes.Add(new TreeNode(unit_name) { Tag = unit_id });
            }
            // clear empty nodes
            HashSet<string> nodes_to_remove = new HashSet<string>();
            foreach (TreeNode n in TreeEntities.Nodes)
                if ((n.Nodes.Count == 0) && (n.Tag == null))
                    nodes_to_remove.Add(n.Name);
            foreach (string s in nodes_to_remove)
                TreeEntities.Nodes.RemoveByKey(s);


            GC.Collect();
        }

        private void ConfirmPlacementEntity()
        {
            map.selection_helper.SetPreviewAngle(0);

            if (TabEditorModes.SelectedIndex != 2)
                return;
            if (EntityHidePreview.Checked)
                return;

            if (RadioEntityModeUnit.Checked)
            {
                ((MapUnitEditor)selected_editor).placement_unit = Utility.TryParseUInt16(EntityID.Text);
                map.selection_helper.SetPreviewUnit(Utility.TryParseUInt16(EntityID.Text));
            }
            if (RadioEntityModeBuilding.Checked)
            {
                ((MapBuildingEditor)selected_editor).placement_building = Utility.TryParseUInt16(EntityID.Text);
                map.selection_helper.SetPreviewBuilding(Utility.TryParseUInt16(EntityID.Text));
            }
            if (RadioEntityModeObject.Checked)
            {
                ((MapObjectEditor)selected_editor).placement_object = Utility.TryParseUInt16(EntityID.Text);
                map.selection_helper.SetPreviewObject(Utility.TryParseUInt16(EntityID.Text));
                map.selection_helper.SetPreviewAngle((ushort)AngleTrackbar.Value);
            }
            if(RadioModeCoopCamps.Checked)
                map.selection_helper.SetPreviewObject(2541);
            if(RadioModeBindstones.Checked)
                map.selection_helper.SetPreviewObject(769);
            if (RadioModePortals.Checked)
                map.selection_helper.SetPreviewObject(778);
            if (RadioModeMonuments.Checked)
                map.selection_helper.SetPreviewObject((ushort)(771 + (int)GetMonumentType()));
        }

        private void TreeEntities_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                EntityID.Text = e.Node.Tag.ToString();
                ConfirmPlacementEntity();
            }
        }
        private void TreeEntityFilter_TextChanged(object sender, EventArgs e)
        {
            TimerTreeEntityFilter.Stop();
            TimerTreeEntityFilter.Start();
        }

        private void TimerTreeEntityFilter_Tick(object sender, EventArgs e)
        {
            if (RadioEntityModeUnit.Checked)
                GetUnitNodesByName(TreeEntitytFilter.Text);
            if (RadioEntityModeBuilding.Checked)
                GetBuildingNodesByName(TreeEntitytFilter.Text);
            if(RadioEntityModeObject.Checked)
                GetObjectNodesByName(TreeEntitytFilter.Text);
            TimerTreeEntityFilter.Stop();
        }

        private void EntityHidePreview_CheckedChanged(object sender, EventArgs e)
        {
            if (EntityHidePreview.Checked)
                map.selection_helper.ClearPreview();
            else
                ConfirmPlacementEntity();
        }

        private void RadioEntityModeUnit_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioEntityModeUnit.Checked)
                return;


            PanelObjectSelector.Visible = true;
            InspectorSet(new SFMap.map_controls.MapUnitInspector());
            GenerateUnitTree();

            selected_editor = new MapUnitEditor()
            {
                map = this.map
            };

            PanelEntityPlacementSelect.Visible = true;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = false;

            QuickSelect.QsRef = qs_unit;

            EntityID.Text = "0";
            ConfirmPlacementEntity();
        }

        private void EntityID_Validated(object sender, EventArgs e)
        {
            ConfirmPlacementEntity();
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
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(EntityID.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        // load object picker tree
        private void GenerateBuildingTree()
        {
            TreeEntities.Nodes.Clear();
            if (building_tree != null)
            {
                Utility.TreeShallowCopy(building_tree, TreeEntities.Nodes);
                return;
            }

            building_tree = new Dictionary<string, TreeNode>();
            // generate race nodes
            SFCFF.SFCategory cat = SFCFF.SFCategoryManager.gamedata[15];
            for (int i = 0; i < cat.GetElementCount(); i++)
            {
                byte race_id = (byte)cat[i][0];
                ushort race_name_index = (ushort)(cat[i][7]);
                SFCFF.SFCategoryElement name_elem = SFCFF.SFCategoryManager.FindElementText(race_name_index, Settings.LanguageID);

                string race_name;
                if (name_elem != null)
                    race_name = Utility.CleanString(name_elem[4]);
                else
                    race_name = Utility.S_MISSING;

                race_name = race_id.ToString() + ". " + race_name;
                building_tree.Add(race_name, new TreeNode(race_name));
                //UnitRace.Items.Add(race_name);

            }
            // generate building nodes
            SFCFF.SFCategory buildings_cat = SFCFF.SFCategoryManager.gamedata[23];
            for (int i = 0; i < buildings_cat.GetElementCount(); i++)
            {
                ushort building_id = (ushort)buildings_cat[i][0];

                byte building_race_id = (byte)buildings_cat[i][1];
                int race_cat_index = cat.GetElementIndex(building_race_id);

                ushort race_name_index = (ushort)(cat[race_cat_index][7]);
                SFCFF.SFCategoryElement name_elem = SFCFF.SFCategoryManager.FindElementText(race_name_index, Settings.LanguageID);
                string race_name;
                if (name_elem != null)
                    race_name = Utility.CleanString(name_elem[4]);
                else
                    race_name = Utility.S_MISSING;

                race_name = building_race_id.ToString() + ". " + race_name;
                string building_name = building_id.ToString() + ". " + SFCFF.SFCategoryManager.GetBuildingName(building_id);
                if (building_tree.ContainsKey(race_name))
                    building_tree[race_name].Nodes.Add(new TreeNode(building_name) { Tag = building_id });
                else
                    building_tree.Add(building_name, new TreeNode(building_name) { Tag = building_id });
            }
            // clear empty nodes
            HashSet<string> nodes_to_remove = new HashSet<string>();
            foreach (string s in building_tree.Keys)
                if ((building_tree[s].Nodes.Count == 0) && (building_tree[s].Tag == null))
                    nodes_to_remove.Add(s);
            foreach (string s in nodes_to_remove)
                building_tree.Remove(s);

            Utility.TreeShallowCopy(building_tree, TreeEntities.Nodes);
            GC.Collect();
        }

        private void GetBuildingNodesByName(string txt)
        {
            if (txt == "")
            {
                GenerateBuildingTree();
                return;
            }
            txt = txt.ToLower();

            TreeEntities.Nodes.Clear();

            // generate race nodes
            SFCFF.SFCategory cat = SFCFF.SFCategoryManager.gamedata[15];
            for (int i = 0; i < cat.GetElementCount(); i++)
            {
                byte race_id = (byte)cat[i][0];
                ushort race_name_index = (ushort)(cat[i][7]);
                SFCFF.SFCategoryElement name_elem = SFCFF.SFCategoryManager.FindElementText(race_name_index, Settings.LanguageID);

                string race_name;
                if (name_elem != null)
                    race_name = Utility.CleanString(name_elem[4]);
                else
                    race_name = Utility.S_MISSING;

                race_name = race_id.ToString() + ". " + race_name;
                TreeEntities.Nodes.Add(race_name, race_name);
                //UnitRace.Items.Add(race_name);

            }
            // generate building nodes
            SFCFF.SFCategory buildings_cat = SFCFF.SFCategoryManager.gamedata[23];
            for (int i = 0; i < buildings_cat.GetElementCount(); i++)
            {
                ushort building_id = (ushort)buildings_cat[i][0];

                byte building_race_id = (byte)buildings_cat[i][1];
                int race_cat_index = cat.GetElementIndex(building_race_id);

                ushort race_name_index = (ushort)(cat[race_cat_index][7]);
                SFCFF.SFCategoryElement name_elem = SFCFF.SFCategoryManager.FindElementText(race_name_index, Settings.LanguageID);
                string race_name;
                if (name_elem != null)
                    race_name = Utility.CleanString(name_elem[4]);
                else
                    race_name = Utility.S_MISSING;

                race_name = building_race_id.ToString() + ". " + race_name;
                string building_name = building_id.ToString() + ". " + SFCFF.SFCategoryManager.GetBuildingName(building_id);
                if ((!race_name.ToLower().Contains(txt)) && (!building_name.ToLower().Contains(txt)))
                    continue;

                if (TreeEntities.Nodes.ContainsKey(race_name))
                    TreeEntities.Nodes[race_name].Nodes.Add(new TreeNode(building_name) { Tag = building_id });
                else
                    TreeEntities.Nodes.Add(new TreeNode(building_name) { Tag = building_id });
            }
            // clear empty nodes
            HashSet<string> nodes_to_remove = new HashSet<string>();
            foreach (TreeNode n in TreeEntities.Nodes)
                if ((n.Nodes.Count == 0) && (n.Tag == null))
                    nodes_to_remove.Add(n.Name);
            foreach (string s in nodes_to_remove)
                TreeEntities.Nodes.RemoveByKey(s);


            GC.Collect();
        }

        private void RadioEntityModeBuilding_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioEntityModeBuilding.Checked)
                return;

            PanelObjectSelector.Visible = true;
            InspectorSet(new SFMap.map_controls.MapBuildingInspector());
            GenerateBuildingTree();

            selected_editor = new MapBuildingEditor()
            {
                map = this.map
            };

            PanelEntityPlacementSelect.Visible = true;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = false;

            QuickSelect.QsRef = qs_building;

            EntityID.Text = "0";
            ConfirmPlacementEntity();
        }

        // load object picker tree
        private void GenerateObjectTree()
        {
            TreeEntities.Nodes.Clear();
            if (obj_tree != null)
            {
                Utility.TreeShallowCopy(obj_tree, TreeEntities.Nodes);
                return;
            }

            obj_tree = new Dictionary<string, TreeNode>();

            SFCFF.SFCategory cat = SFCFF.SFCategoryManager.gamedata[33];
            foreach(SFCFF.SFCategoryElement e in cat.elements)
            {
                UInt16 id = (UInt16)e[0];
                if((id > 64)&&(id < 128))
                    continue;
                if((id >= 771)&&(id <= 778))
                    continue;
                if (id == 769)
                    continue;
                if (id == 2541)
                    continue;


                string name = id.ToString()+". "+SFCFF.SFCategoryManager.GetObjectName(id);
                string path = Utility.CleanString(e[5]);
                string[] path_items = path.Split('/');
                if ((path_items.Length == 1) && (path_items[0] == ""))
                    path_items = new string[] { };

                // add entry
                if (path_items.Length == 0)
                {
                    obj_tree.Add(name, new TreeNode(name) { Tag = id });
                    continue;
                }

                TreeNode tnc = null;
                if (!obj_tree.ContainsKey(path_items[0]))
                    obj_tree.Add(path_items[0], new TreeNode(path_items[0]));
                tnc = obj_tree[path_items[0]];

                for(int i = 1; i < path_items.Length; i++)
                {
                    if (!tnc.Nodes.ContainsKey(path_items[i]))
                        tnc.Nodes.Add(path_items[i], path_items[i]);
                    tnc = tnc.Nodes[path_items[i]];
                }
                tnc.Nodes.Add(new TreeNode(name) { Tag = id });
            }

            Utility.TreeShallowCopy(obj_tree, TreeEntities.Nodes);
            GC.Collect();
        }

        private void GetObjectNodesByName(string txt)
        {
            if(txt == "")
            {
                GenerateObjectTree();
                return;
            }
            txt = txt.ToLower();

            TreeEntities.Nodes.Clear();

            SFCFF.SFCategory cat = SFCFF.SFCategoryManager.gamedata[33];
            foreach (SFCFF.SFCategoryElement e in cat.elements)
            {
                UInt16 id = (UInt16)e[0];
                if ((id > 64) && (id < 128))
                    continue;
                if ((id >= 771) && (id <= 778))
                    continue;
                if (id == 769)
                    continue;
                if (id == 2541)
                    continue;


                string name = id.ToString() + ". " + SFCFF.SFCategoryManager.GetObjectName(id);
                string path = Utility.CleanString(e[5]);
                string[] path_items = path.Split('/');
                if ((path_items.Length == 1) && (path_items[0] == ""))
                    path_items = new string[] { };

                bool include = true;
                if (!name.ToLower().Contains(txt))
                {
                    include = false;
                    foreach(string s in path_items)
                        if(s.ToLower().Contains(txt))
                        {
                            include = true;
                            break;
                        }
                }
                if (!include)
                    continue;

                // add entry
                if (path_items.Length == 0)
                {
                    TreeEntities.Nodes.Add(new TreeNode(name) { Tag = id });
                    continue;
                }

                TreeNode tnc = null;
                if (!TreeEntities.Nodes.ContainsKey(path_items[0]))
                    TreeEntities.Nodes.Add(path_items[0], path_items[0]);
                tnc = TreeEntities.Nodes[path_items[0]];

                for (int i = 1; i < path_items.Length; i++)
                {
                    if (!tnc.Nodes.ContainsKey(path_items[i]))
                        tnc.Nodes.Add(path_items[i], path_items[i]);
                    tnc = tnc.Nodes[path_items[i]];
                }
                tnc.Nodes.Add(new TreeNode(name) { Tag = id });
            }


            GC.Collect();
        }


        public struct AngleInfo
        {
            public UInt16 angle;
            public bool random;
        }

        private void Angle_Validated(object sender, EventArgs e)
        {
            int v = Utility.TryParseUInt16(Angle.Text);
            AngleTrackbar.Value = (v >= 0 ? (v <= 359 ? v : 359) : 0);
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            Angle.Text = AngleTrackbar.Value.ToString();
            if (RadioEntityModeObject.Checked)
                map.selection_helper.SetPreviewAngle((ushort)AngleTrackbar.Value);
        }

        private void CheckRandomRange_CheckedChanged(object sender, EventArgs e)
        {
            Angle.Enabled = !CheckRandomRange.Checked;
            AngleTrackbar.Enabled = !CheckRandomRange.Checked;
        }

        public AngleInfo GetAngleInfo()
        {
            return new AngleInfo() { angle = Utility.TryParseUInt16(Angle.Text), random = CheckRandomRange.Checked };
        }

        private void RadioEntityModeObject_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioEntityModeObject.Checked)
                return;

            PanelObjectSelector.Visible = true;
            InspectorSet(new SFMap.map_controls.MapObjectInspector());
            GenerateObjectTree();

            selected_editor = new MapObjectEditor()
            {
                map = this.map
            };

            PanelEntityPlacementSelect.Visible = true;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = true; 
            
            QuickSelect.QsRef = qs_object;

            EntityID.Text = "0";
            ConfirmPlacementEntity();
        }

        private void RadioModeCoopCamps_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioModeCoopCamps.Checked)
                return;

            PanelObjectSelector.Visible = false;
            InspectorSet(new SFMap.map_controls.MapCoopCampInspector());

            selected_editor = new MapCoopCampEditor()
            {
                map = this.map
            };

            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = true;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = false;

            ConfirmPlacementEntity();
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

            PanelObjectSelector.Visible = false;
            InspectorSet(new SFMap.map_controls.MapBindstoneInspector());

            selected_editor = new MapBindstoneEditor()
            {
                map = this.map
            };

            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = false;

            ConfirmPlacementEntity();
        }

        private void RadioModePortals_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioModePortals.Checked)
                return;

            PanelObjectSelector.Visible = false;
            InspectorSet(new SFMap.map_controls.MapPortalInspector());

            selected_editor = new MapPortalEditor()
            {
                map = this.map
            };

            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = false;

            ConfirmPlacementEntity();
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

            PanelObjectSelector.Visible = false;
            InspectorSet(new SFMap.map_controls.MapMonumentInspector());

            selected_editor = new MapMonumentEditor()
            {
                map = this.map
            };

            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = true;
            PanelObjectAngle.Visible = false;

            ConfirmPlacementEntity();
        }

        private void MonumentHuman_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentHuman.Checked)
            { 
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                map.selection_helper.SetPreviewObject((ushort)(771 + (int)GetMonumentType()));
            }
        }

        private void MonumentElf_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentElf.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                map.selection_helper.SetPreviewObject((ushort)(771 + (int)GetMonumentType()));
            }
        }

        private void MonumentDwarf_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentDwarf.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                map.selection_helper.SetPreviewObject((ushort)(771 + (int)GetMonumentType()));
            }
        }

        private void MonumentOrc_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentOrc.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                map.selection_helper.SetPreviewObject((ushort)(771 + (int)GetMonumentType()));
            }
        }

        private void MonumentTroll_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentTroll.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                map.selection_helper.SetPreviewObject((ushort)(771 + (int)GetMonumentType()));
            }
        }

        private void MonumentDarkElf_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentDarkElf.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                map.selection_helper.SetPreviewObject((ushort)(771 + (int)GetMonumentType()));
            }
        }

        private void MonumentHero_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentHero.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                map.selection_helper.SetPreviewObject((ushort)(771 + (int)GetMonumentType()));
            }
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

            PanelBrushShape.Visible = true;

            terrain_brush.size = (float)Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();

            map.heightmap.overlay_active_texture = map.heightmap.overlay_texture_decals;
            map.heightmap.RefreshOverlay();
            update_render = true;
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

            if (map.metadata.minimap == null)
            {
                byte[] image_data = new byte[128 * 128 * 3];
                for (int i = 0; i < 128 * 128 * 3; i++)
                    image_data[i] = (byte)((i * 1024) / 3);
                map.metadata.minimap = new SFMapMinimap();
                map.metadata.minimap.width = 128;
                map.metadata.minimap.height = 128;
                map.metadata.minimap.texture_data = image_data;
                map.metadata.minimap.GenerateTexture();
            }

            ButtonTeams.Visible = true;
            PanelCoopParams.Visible = true;
            UpdateCoopSpawnParameters();
        }

        private void MapTypeMultiplayer_CheckedChanged(object sender, EventArgs e)
        {
            if (MapTypeMultiplayer.Checked == false)
                return;

            map.metadata.map_type = SFMapType.MULTIPLAYER;

            if (map.metadata.minimap == null)
            {
                byte[] image_data = new byte[128 * 128 * 3];
                for (int i = 0; i < 128 * 128 * 3; i++)
                    image_data[i] = (byte)((i * 1024) / 3);
                map.metadata.minimap = new SFMapMinimap();
                map.metadata.minimap.width = 128;
                map.metadata.minimap.height = 128;
                map.metadata.minimap.texture_data = image_data;
                map.metadata.minimap.GenerateTexture();
            }

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
            if (map == null)
                return;

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
            if (map == null)
                return;

            if (visibility_form != null)
            {
                visibility_form.BringToFront();
                return;
            }

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

        private void importHeightmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (importhmap_form != null)
            {
                importhmap_form.BringToFront();
                return;
            }

            importhmap_form = new SFMap.map_dialog.MapImportHeightmapDialog();
            importhmap_form.map = map;
            importhmap_form.FormClosed += new FormClosedEventHandler(importhmap_FormClosed);
            importhmap_form.Show();
        }

        private void importhmap_FormClosed(object sender, FormClosedEventArgs e)
        {
            importhmap_form.FormClosed -= new FormClosedEventHandler(importhmap_FormClosed);
            importhmap_form = null;
        }

        private void exportHeightmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (exporthmap_form != null)
            {
                exporthmap_form.BringToFront();
                return;
            }

            exporthmap_form = new SFMap.map_dialog.MapExportHeightmapDialog();
            exporthmap_form.map = map;
            exporthmap_form.FormClosed += new FormClosedEventHandler(exporthmap_FormClosed);
            exporthmap_form.Show();
        }

        private void exporthmap_FormClosed(object sender, FormClosedEventArgs e)
        {
            exporthmap_form.FormClosed -= new FormClosedEventHandler(exporthmap_FormClosed);
            exporthmap_form = null;
        }

    }
}
