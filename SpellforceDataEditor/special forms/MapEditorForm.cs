﻿using OpenTK;
using SFEngine;
using SFEngine.SF3D;
using SFEngine.SF3D.Physics;
using SFEngine.SF3D.SceneSynchro;
using SFEngine.SF3D.SFRender;
using SFEngine.SF3D.UI;
using SFEngine.SFCFF;
using SFEngine.SFLua;
using SFEngine.SFMap;
using SFEngine.SFResources;
using SFEngine.SFUnPak;
using SpellforceDataEditor.SFMap.MapEdit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

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


            UIElementIndex image_minimap;
            UIElementIndex image_minimap_frame_left;
            UIElementIndex image_minimap_frame_top;
            UIElementIndex image_minimap_icons_border;
            UIElementIndex image_minimap_icons;
            int next_icon = 0;
            bool icons_visible = true;

            // minimap texture
            public SFTexture minimap_tex = null;
            public SFTexture minimap_icons_tex = null;

            SFEngine.SFMap.SFMap map = null;
            int minimap_size;
            bool resizing = false;
            bool clicked = false;
            SFCoord clicked_pos = new SFCoord(0, 0);

            public MapEditorUI(SFEngine.SFMap.SFMap _map)
            {
                map = _map;
            }

            public void InitMinimap(int m_width, int m_height)
            {
                minimap_tex = SFTexture.DynamicTexture((ushort)m_width, (ushort)m_height, 1, TextureTarget.Texture2D, InternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte, (int)All.LinearMipmapLinear, (int)All.Linear, (int)All.ClampToEdge, (int)All.ClampToEdge, Vector4.One, Settings.MaxAnisotropy, true, true);
                SFResourceManager.Textures.AddManually(minimap_tex, "minimap");

                SFRenderEngine.ui.AddStorage(minimap_tex, 1);
                SFRenderEngine.ui.AddStorage(SFRenderEngine.opaque_tex, 2);

                image_minimap = SFRenderEngine.ui.AddElementImage(minimap_tex, new Vector2(m_width, m_height), new Vector2(0, 0), new Vector2(0, 0), true);
                image_minimap_frame_left = SFRenderEngine.ui.AddElementImage(SFRenderEngine.opaque_tex, new Vector2(3, m_height), new Vector2(0, 0), new Vector2(0, 0), false);
                image_minimap_frame_top = SFRenderEngine.ui.AddElementImage(SFRenderEngine.opaque_tex, new Vector2(m_width, 3), new Vector2(0, 0), new Vector2(0, 0), false);

                // minimap icons
                if(!SFResourceManager.Textures.Load("ui_oth1", FileSource.PAK, out minimap_icons_tex, out int ec, new SFTexture.SFTextureLoadArgs() { IgnoreMipmapSettings = true }))
                {
                    SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.SF3D, "MapEditorUI.InitMinimap(): Could not load texture (texture name = ui_oth1)");
                    throw new Exception("MapEditorUI.InitMinimap(): Could not load texture ui_oth1");
                }

                // 2000 units, 1000 buildings, 500 interactive objects, 100 portals
                SFRenderEngine.ui.AddStorage(minimap_icons_tex, 7200);
                image_minimap_icons_border = SFRenderEngine.ui.AddElementMulti(minimap_icons_tex, 3600);
                image_minimap_icons = SFRenderEngine.ui.AddElementMulti(minimap_icons_tex, 3600);
            }

            // full map redraw
            public void RedrawMinimap()
            {
                if (minimap_tex == null)
                {
                    return;
                }

                SFMapHeightMap hmap = map.heightmap;
                Color col;
                SFCoord coord;
                for (int i = 0; i < hmap.width; i++)
                {
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
                        {
                            col = hmap.texture_manager.tile_ocean_color;
                        }

                        minimap_tex.data[(j * hmap.width + i) * 4 + 0] = (byte)(col.R * col_str);
                        minimap_tex.data[(j * hmap.width + i) * 4 + 1] = (byte)(col.G * col_str);
                        minimap_tex.data[(j * hmap.width + i) * 4 + 2] = (byte)(col.B * col_str);
                        minimap_tex.data[(j * hmap.width + i) * 4 + 3] = 255;
                    }
                }

                foreach (SFMapLake lake in map.lake_manager.lakes)
                {
                    ushort lake_z = (ushort)(map.heightmap.GetZ(lake.start) + lake.z_diff);
                    byte alpha_shallow = 0x7F;
                    if ((lake.type == 2) || (lake.type == 3))
                    {
                        alpha_shallow = 0xFF;
                    }

                    col = map.lake_manager.GetLakeMinimapColor(lake.type);
                    foreach (SFCoord p in lake.cells)
                    {
                        short lake_cell_z_diff = (short)(lake_z - map.heightmap.GetZ(p));
                        if (lake_cell_z_diff >= SFMapLakeManager.LAKE_SHALLOW_DEPTH)
                        {
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 0] = col.R;
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 1] = col.G;
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 2] = col.B;
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 3] = 255;
                        }
                        else
                        {
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 0] = (byte)(((minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 0] * (0xFF - alpha_shallow)) + (col.R * alpha_shallow)) / 0xFF);
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 1] = (byte)(((minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 1] * (0xFF - alpha_shallow)) + (col.G * alpha_shallow)) / 0xFF);
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 2] = (byte)(((minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 2] * (0xFF - alpha_shallow)) + (col.B * alpha_shallow)) / 0xFF);
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 3] = 255;
                        }
                    }
                }

                minimap_tex.UpdateImage(minimap_tex.data, 0, 0, 0);
            }

            // redraws given area with chosen tile color
            // shades pixels if they're on terrain
            // draws lakes and water if the pixels are on water
            public void RedrawMinimap(IEnumerable<SFCoord> pixels, byte tile_id)
            {
                if (minimap_tex == null)
                {
                    return;
                }

                SFMapHeightMap hmap = map.heightmap;

                Color col = hmap.texture_manager.tile_average_color[tile_id];
                foreach (SFCoord p in pixels)
                {
                    int i = p.x;
                    int j = p.y;
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


                    if (map.lake_manager.GetLakeIndexAt(p) != SFEngine.Utility.NO_INDEX)
                    {
                        SFMapLake lake = map.lake_manager.lakes[map.lake_manager.GetLakeIndexAt(p)];
                        ushort lake_z = (ushort)(map.heightmap.GetZ(lake.start) + lake.z_diff);
                        short lake_cell_z_diff = (short)(lake_z - map.heightmap.GetZ(p));

                        Color col2 = map.lake_manager.GetLakeMinimapColor(lake.type);

                        if (lake_cell_z_diff >= SFMapLakeManager.LAKE_SHALLOW_DEPTH)
                        {
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 0] = col2.R;
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 1] = col2.G;
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 2] = col2.B;
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 3] = 255;
                        }
                        else
                        {
                            byte alpha_shallow = 0x7F;
                            if ((lake.type == 2) || (lake.type == 3))
                            {
                                alpha_shallow = 0xFF;
                            }

                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 0] = (byte)(((col2.R * alpha_shallow) + (col.R * (0xFF - alpha_shallow))) / 0xFF);
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 1] = (byte)(((col2.G * alpha_shallow) + (col.G * (0xFF - alpha_shallow))) / 0xFF);
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 2] = (byte)(((col2.B * alpha_shallow) + (col.B * (0xFF - alpha_shallow))) / 0xFF);
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 3] = 255;
                        }
                    }
                }

                minimap_tex.UpdateImage(minimap_tex.data, 0, 0, 0);
            }

            // only redraws selected pixels, according to the contents of the map
            public void RedrawMinimap(IEnumerable<SFCoord> pixels)
            {
                if (minimap_tex == null)
                {
                    return;
                }

                SFMapHeightMap hmap = map.heightmap;

                foreach (SFCoord p in pixels)
                {
                    int i = p.x;
                    int j = p.y;
                    if (map.lake_manager.GetLakeIndexAt(p) != SFEngine.Utility.NO_INDEX)
                    {
                        SFMapLake lake = map.lake_manager.lakes[map.lake_manager.GetLakeIndexAt(p)];
                        ushort lake_z = (ushort)(map.heightmap.GetZ(lake.start) + lake.z_diff);
                        short lake_cell_z_diff = (short)(lake_z - map.heightmap.GetZ(p));

                        Color col = map.lake_manager.GetLakeMinimapColor(lake.type);

                        if (lake_cell_z_diff >= SFMapLakeManager.LAKE_SHALLOW_DEPTH)
                        {
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 0] = col.R;
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 1] = col.G;
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 2] = col.B;
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 3] = 255;
                        }
                        else
                        {
                            byte alpha_shallow = 0x7F;
                            if ((lake.type == 2) || (lake.type == 3))
                            {
                                alpha_shallow = 0xFF;
                            }

                            Color col2 = hmap.texture_manager.tile_average_color[hmap.GetTileFixed(p)];

                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 0] = (byte)(((col.R * alpha_shallow) + (col2.R * (0xFF - alpha_shallow))) / 0xFF);
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 1] = (byte)(((col.G * alpha_shallow) + (col2.G * (0xFF - alpha_shallow))) / 0xFF);
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 2] = (byte)(((col.B * alpha_shallow) + (col2.B * (0xFF - alpha_shallow))) / 0xFF);
                            minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 3] = 255;
                        }
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

                minimap_tex.UpdateImage(minimap_tex.data, 0, 0, 0);
            }

            // only redraws selected pixels, paint them on specified color
            public void RedrawMinimap(IEnumerable<SFCoord> pixels, Color c)
            {
                if (minimap_tex == null)
                {
                    return;
                }

                SFMapHeightMap hmap = map.heightmap;

                foreach (SFCoord p in pixels)
                {
                    minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 0] = c.R;
                    minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 1] = c.G;
                    minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 2] = c.B;
                    minimap_tex.data[(p.y * hmap.width + p.x) * 4 + 3] = 255;
                }

                minimap_tex.UpdateImage(minimap_tex.data, 0, 0, 0);
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
                {
                    return;
                }

                pos = new SFCoord(pos.x, map.height - pos.y - 1);
                Vector2 final_icon_center_offset = new Vector2((float)Math.Round((float)(pos.x * minimap_size) / minimap_tex.width), (float)Math.Round((float)(pos.y * minimap_size) / minimap_tex.height));
                if (icon_type == MapEditorUIIconType.BUILDING)
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
                var unit_data = SFCategoryManager.gamedata[2024].FindElementBinary(0, (ushort)unit_id);
                if (unit_data == null)
                {
                    return 6;
                }

                var unit_stats_data = SFCategoryManager.gamedata[2005].FindElementBinary(0, (ushort)unit_data[2]);
                if (unit_stats_data == null)
                {
                    return 6;
                }

                var race_data = SFCategoryManager.gamedata[2022].FindElementBinary(0, (byte)unit_stats_data[2]);
                if (race_data == null)
                {
                    return 6;
                }

                var clan_data_index = SFCategoryManager.gamedata[2023].FindMultipleElementIndexBinary(0, (byte)((ushort)race_data[9]));
                if (clan_data_index == SFEngine.Utility.NO_INDEX)
                {
                    return 6;
                }

                var clan_data = SFCategoryManager.gamedata[2023].element_lists[clan_data_index];
                var player_relation = (byte)clan_data[10][2];
                if (player_relation == 0)
                {
                    return 1;
                }

                if (player_relation == 100)
                {
                    return 2;
                }

                if (player_relation == 156)
                {
                    return 0;
                }

                return 6;
            }

            private int GetBuildingRelationToMainChar(SFMapBuilding bld)
            {
                // clan player = 11
                SFCategoryElement race_data;
                if (bld.race_id == 0)
                {
                    var building_data = SFCategoryManager.gamedata[2029].FindElementBinary(0, (ushort)bld.game_id);
                    if (building_data == null)
                    {
                        return 6;
                    }

                    race_data = SFCategoryManager.gamedata[2022].FindElementBinary(0, (byte)building_data[1]);
                }
                else
                {
                    race_data = SFCategoryManager.gamedata[2022].FindElementBinary(0, (byte)bld.race_id);
                }

                if (race_data == null)
                {
                    return 6;
                }

                var clan_data_index = SFCategoryManager.gamedata[2023].FindMultipleElementIndexBinary(0, (byte)((ushort)race_data[9]));
                if (clan_data_index == SFEngine.Utility.NO_INDEX)
                {
                    return 6;
                }

                var clan_data = SFCategoryManager.gamedata[2023].element_lists[clan_data_index];
                var player_relation = (byte)clan_data[10][2];
                if (player_relation == 0)
                {
                    return 1;
                }

                if (player_relation == 100)
                {
                    return 2;
                }

                if (player_relation == 156)
                {
                    return 0;
                }

                return 6;
            }

            public void RedrawMinimapIcons()
            {
                ClearMinimapIcons();
                foreach (var unit in map.unit_manager.units)
                {
                    AddMinimapIcon(MapEditorUIIconType.UNIT, GetUnitRelationToMainChar(unit.game_id), unit.grid_position);
                }

                foreach (var building in map.building_manager.buildings)
                {
                    AddMinimapIcon(MapEditorUIIconType.BUILDING, GetBuildingRelationToMainChar(building), building.grid_position);
                }

                foreach (var obj in map.object_manager.objects)
                {
                    if (obj.npc_id > 0)
                    {
                        AddMinimapIcon(MapEditorUIIconType.UNIT, 5, obj.grid_position);
                    }
                }
                foreach (var iobject in map.int_object_manager.int_objects)
                {
                    AddMinimapIcon(MapEditorUIIconType.BUILDING, 3, iobject.grid_position);
                }

                foreach (var portal in map.portal_manager.portals)
                {
                    AddMinimapIcon(MapEditorUIIconType.BUILDING, 4, portal.grid_position);
                }

                SFRenderEngine.ui.UpdateElementAll(image_minimap_icons_border);
                SFRenderEngine.ui.UpdateElementAll(image_minimap_icons);

            }

            public void SetMinimapVisible(bool visible)
            {
                if (minimap_tex == null)
                {
                    return;
                }

                SFRenderEngine.ui.SetElementVisible(image_minimap, visible);
                SFRenderEngine.ui.SetElementVisible(image_minimap_frame_left, visible);
                SFRenderEngine.ui.SetElementVisible(image_minimap_frame_top, visible);

                SFRenderEngine.ui.SetElementVisible(image_minimap_icons_border, visible & icons_visible);
                SFRenderEngine.ui.SetElementVisible(image_minimap_icons, visible & icons_visible);
            }

            public void SetMinimapIconsVisible(bool visible)
            {
                if (minimap_tex == null)
                {
                    return;
                }

                icons_visible = visible;
                SFRenderEngine.ui.SetElementVisible(image_minimap_icons_border, GetMinimapVisible() & icons_visible);
                SFRenderEngine.ui.SetElementVisible(image_minimap_icons, GetMinimapVisible() & icons_visible);
            }

            public void SetMinimapSize(int size)
            {
                if (minimap_tex == null)
                {
                    return;
                }

                minimap_size = Math.Min((int)Math.Min(SFRenderEngine.render_size.X - 3, SFRenderEngine.render_size.Y - 3), size);
                size = minimap_size;

                SFRenderEngine.ui.SetImageSize(image_minimap, new Vector2(size, size));
                SFRenderEngine.ui.SetImageSize(image_minimap_frame_left, new Vector2(3, size));
                SFRenderEngine.ui.SetImageSize(image_minimap_frame_top, new Vector2(size + 3, 3));

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
                {
                    return;
                }

                minimap_tex.Export(name);
            }

            public void OnResize()
            {
                SetMinimapSize(minimap_size);
            }

            public bool ProcessInput(float mx, float my, bool mouse_pressed)
            {
                if (minimap_tex == null)
                {
                    return false;
                }

                if (!SFRenderEngine.ui.GetElementVisible(image_minimap))
                {
                    return false;
                }

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

                if ((Math.Abs(mx - (SFRenderEngine.render_size.X - minimap_size)) < 16) && (Math.Abs(my - (SFRenderEngine.render_size.Y - minimap_size)) < 16))
                {
                    resizing = true;
                    return true;
                }

                if (!mouse_pressed)
                {
                    clicked = false;
                }

                if ((mx > SFRenderEngine.render_size.X - minimap_size) && (my > SFRenderEngine.render_size.Y - minimap_size))
                {
                    if ((mouse_pressed) && (!clicked))
                    {
                        clicked_pos = new SFCoord(
                            SFRenderEngine.scene.map.heightmap.width - (int)(((SFRenderEngine.render_size.X - mx)) * (SFRenderEngine.scene.map.heightmap.width / (float)minimap_size)) - 1,
                            (int)((SFRenderEngine.render_size.Y - my) * (SFRenderEngine.scene.map.heightmap.height / (float)minimap_size)));
                        clicked = true;
                    }

                    return true;
                }

                return false;
            }

            public bool GetMinimapPosClicked(ref SFCoord pos)
            {
                if (clicked)
                {
                    pos = clicked_pos;
                }

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
                SFResourceManager.Textures.Dispose(minimap_tex);
                minimap_tex = null;
                SFResourceManager.Textures.Dispose(minimap_icons_tex);
                minimap_icons_tex = null;
            }
        }

        // operator is a controller that can perform an action and its reverse
        // in context of map editor, such operators include: heightmap edition, removing lake etc.
        // this class allows performing actions in specific order, and reversing them in specific order
        // essentially, this is an implementation of undo/redo function in map editor
        public class MapEditorOperatorQueue
        {
            public SFEngine.SFMap.SFMap map = null;
            public List<SFMap.map_operators.IMapOperator> operators { get; private set; } = new List<SFMap.map_operators.IMapOperator>();
            public int current_operator { get; set; } = SFEngine.Utility.NO_INDEX;

            // the following is data that operators accrue as they're being applied
            // this is done so a cluster of 100 operators does not needlessly do unnecessary stuff
            public bool RebuildGeometry = false;
            public SFCoord RebuildGeometryTopLeft = new SFCoord(10000, 10000);
            public SFCoord RebuildGeometryBottomRight = new SFCoord(-1, -1);
            public bool RedrawMinimap = false;
            public bool RedrawMinimapAll = false;
            public HashSet<SFCoord> RedrawMinimapCells = new HashSet<SFCoord>();
            public bool RebuildTerrainTexture = false;
            public SFCoord RebuildTerrainTextureTopLeft = new SFCoord(10000, 10000);
            public SFCoord RebuildTerrainTextureBottomRight = new SFCoord(-1, -1);
            public bool RefreshOverlay = false;
            public bool RedrawMinimapIcons = false;

            // operator queue can contain clusters
            // cluster is a sub-list of operators
            // a cluster is open in the queue if it's the last operator in the list, and if it's not finished
            // only one cluster can be open at any time
            public bool IsClusterOpen()
            {
                return ((operators.Count != 0)
                    && (operators[operators.Count - 1] is SFMap.map_operators.MapOperatorCluster)
                    && (!operators[operators.Count - 1].Finished));
            }

            // add new operator to the queue
            // if a cluster is open, add the operator to the cluster instead
            public void Push(SFMap.map_operators.IMapOperator op)
            {
                while ((operators.Count - 1) > current_operator)
                {
                    operators.RemoveAt(operators.Count - 1);
                }

                if (IsClusterOpen())
                {
                    ((SFMap.map_operators.MapOperatorCluster)(operators[operators.Count - 1])).SubOperators.Add(op);
                }
                else
                {
                    operators.Add(op);
                    current_operator += 1;
                    if (op.ApplyOnPush)
                    {
                        op.Apply(map);
                        UpdateState();
                    }
                }

                if (MainForm.mapedittool.undohistory_form != null)
                {
                    MainForm.mapedittool.undohistory_form.OnAddOperator();
                }
            }

            // opens a new cluster
            // will do nothing if a cluster is already open
            public void OpenCluster(bool apply_on_push = false)
            {
                if (IsClusterOpen())
                {
                    SFEngine.LogUtils.Log.Warning(SFEngine.LogUtils.LogSource.SFMap, "MapEditorOperatorQueue.OpenCluster(): Another cluster is already open, skipping");
                    return;
                }

                SFMap.map_operators.MapOperatorCluster op_cluster = new SFMap.map_operators.MapOperatorCluster() { ApplyOnPush = apply_on_push };
                Push(op_cluster);
            }

            // closes the currently open cluster
            // does nothing if there's no open clusters
            public void CloseCluster()
            {
                if (IsClusterOpen())
                {
                    operators[operators.Count - 1].Finish(null);

                    // if no changes are in the cluster, it's safe to remove it
                    if (((SFMap.map_operators.MapOperatorCluster)(operators[operators.Count - 1])).SubOperators.Count == 0)
                    {
                        operators.RemoveAt(current_operator);
                        current_operator -= 1;

                        if (MainForm.mapedittool.undohistory_form != null)
                        {
                            MainForm.mapedittool.undohistory_form.OnRemoveOperator();
                        }
                    }
                    else
                    {
                        if (operators[operators.Count - 1].ApplyOnPush)
                        {
                            operators[operators.Count - 1].Apply(map);
                        }
                    }

                    UpdateState();
                }
                else
                {
                    SFEngine.LogUtils.Log.Warning(SFEngine.LogUtils.LogSource.SFMap, "MapEditorOperatorQueue.CloseCluster(): Not a cluster, or already closed!");
                }
            }

            // implementation of Undo function
            // given the current state of the map, operator performs reverse of the action it contains
            // will do nothing if there are no actions to undo
            public void Undo()
            {
                if (current_operator == SFEngine.Utility.NO_INDEX)
                {
                    return;
                }

                SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMap, "MapEditorOperatorQueue.Undo(): operator " + operators[current_operator].ToString());
                operators[current_operator].Revert(map);
                current_operator -= 1;

                UpdateState();

                if (MainForm.mapedittool.undohistory_form != null)
                {
                    MainForm.mapedittool.undohistory_form.OnUndo();
                }
            }

            // implementation of Redo function
            // given the current state of the map, operator performs action it contains
            // will do nothing if there are no actions to redo
            public void Redo()
            {
                if (current_operator == operators.Count - 1)
                {
                    return;
                }

                current_operator += 1;
                SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMap, "MapEditorOperatorQueue.Redo(): operator " + operators[current_operator].ToString());
                operators[current_operator].Apply(map);

                UpdateState();

                if (MainForm.mapedittool.undohistory_form != null)
                {
                    MainForm.mapedittool.undohistory_form.OnRedo();
                }
            }

            public void UpdateState()
            {
                if (RebuildGeometry)
                {
                    map.heightmap.RebuildGeometry(RebuildGeometryTopLeft, RebuildGeometryBottomRight);
                }

                if (RedrawMinimap)
                {
                    if (RedrawMinimapAll)
                    {
                        MainForm.mapedittool.ui.RedrawMinimap();
                    }
                    else
                    {
                        MainForm.mapedittool.ui.RedrawMinimap(RedrawMinimapCells);
                    }
                }

                if (RebuildTerrainTexture)
                {
                    map.heightmap.RebuildTerrainTexture(RebuildTerrainTextureTopLeft, RebuildTerrainTextureBottomRight);
                }

                if (RefreshOverlay)
                {
                    map.heightmap.RefreshOverlay();
                }

                if (RedrawMinimapIcons)
                {
                    MainForm.mapedittool.ui.RedrawMinimapIcons();
                }

                RebuildGeometry = false;
                RebuildGeometryTopLeft = new SFCoord(10000, 10000);
                RebuildGeometryBottomRight = new SFCoord(-1, -1);
                RedrawMinimap = false;
                RedrawMinimapAll = false;
                RedrawMinimapCells.Clear();
                RebuildTerrainTexture = false;
                RebuildTerrainTextureTopLeft = new SFCoord(10000, 10000);
                RebuildTerrainTextureBottomRight = new SFCoord(-1, -1);
                RefreshOverlay = false;
                RedrawMinimapIcons = false;

                MainForm.mapedittool.update_render = true;
            }
        }

        public enum MapMaskSource
        {
            ALL = 0,
            PAINT,
            MASK,
            TEXTURE,
            HEIGHT,
            SLOPE,
            ATTRIBUTE,
            FEATURE
        };

        public enum MapMaskFilter
        {
            NONE = 0,
            BORDER,
            RANDOM
        };

        public enum MapMaskOperation
        {
            ZERO = 0,
            ONE,
            AND,
            OR,
            XOR,
            SUBTRACT
        };

        public enum MapMaskAttribute
        {
            TERRAIN_BLOCK = 0,
            OBJECT_BLOCK,
            BUILDING_BLOCK,
            MANUAL_BLOCK,
            LAKE,
            SHORE
        };

        public enum MapMaskFeature
        {
            BUILDING = 0,
            OBJECT,
            LAKE,
            WALKABLE
        }

        public enum MapMaskComparison
        {
            EQ = 0,
            GEQ,
            GT,
            NEQ,
            LT,
            LEQ
        };

        SFEngine.SFMap.SFMap map = null;
        public bool ready = false;

        GLControl RenderWindow = null;
        bool initialized_view = false;   // is true when the map render control is visible and loaded
        bool dynamic_render = false;     // if true, window will redraw every frame
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
        int updates_this_second = 0;

        public MapEditor selected_editor { get; private set; } = new MapHeightMapEditor();
        public SFMap.map_controls.MapInspector selected_inspector { get; private set; } = null;

        MapBrush terrain_brush = new MapBrush();

        SFMap.map_dialog.MapManageTeamCompositions teamcomp_form = null;
        SFMap.map_dialog.MapVisibilitySettings visibility_form = null;
        SFMap.map_dialog.MapImportHeightmapDialog importhmap_form = null;
        SFMap.map_dialog.MapExportHeightmapDialog exporthmap_form = null;
        SFMap.map_dialog.MapOperatorHistoryViewer undohistory_form = null;

        Dictionary<string, TreeNode> unit_tree = null;
        Dictionary<string, TreeNode> building_tree = null;
        Dictionary<string, TreeNode> obj_tree = null;

        SFMapQuickSelectHelper qs_unit = new SFMapQuickSelectHelper();
        SFMapQuickSelectHelper qs_building = new SFMapQuickSelectHelper();
        SFMapQuickSelectHelper qs_object = new SFMapQuickSelectHelper();

        List<int> heightmap_mode_values = new List<int>(new int[3] { 20, 300, 5 });
        int lake_mode_value = 50;

        public MapMaskSource mask_source = MapMaskSource.PAINT;
        public MapMaskOperation mask_operation = MapMaskOperation.ONE;
        public MapMaskFilter mask_filter = MapMaskFilter.NONE;
        public MapMaskAttribute mask_attribute = MapMaskAttribute.TERRAIN_BLOCK;
        public MapMaskComparison mask_comparison = MapMaskComparison.EQ;
        public MapMaskFeature mask_feature = MapMaskFeature.OBJECT;
        public object mask_feature_object = null;
        public int mask_value = 0;
        public int mask_percentage = 100;
        public int mask_random_seed = DateTime.Now.Millisecond;
        public bool mask_border_is_outer = true;
        public HashSet<SFCoord> mask_selection = new HashSet<SFCoord>();

        public MapEditorUI ui { get; private set; } = null;

        public MapEditorOperatorQueue op_queue { get; private set; } = null;

        public MapEditorForm()
        {
            InitializeComponent();
        }

        private void MapEditorForm_Load(object sender, EventArgs e)
        {
            TimerAnimation.Enabled = true;
            TimerAnimation.Interval = 1000 / SFRenderEngine.scene.frames_per_second;
            TimerAnimation.Start();

            InspectorHide();
        }

        private void MapEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseMap() != 0)
            {
                e.Cancel = true;
            }
            else
            {
                selected_editor = null;

                SFRenderEngine.scene.root = null;
                SFRenderEngine.scene.camera = null;
                SFRenderEngine.scene.map = null;

                if (initialized_view)
                {
                    SFRenderEngine.scene.atmosphere.Dispose();

                    SFSubModel3D.Cache.Dispose();
                    SFModelSkinChunk.Cache.Dispose();

                    DestroyRenderWindow();
                    SFResourceManager.DisposeAll();
                }

                if ((SFCategoryManager.ready) && (MainForm.data == null))
                {
                    SFCategoryManager.UnloadAll();
                }
            }
        }

        private void MapEditorForm_Resize(object sender, EventArgs e)
        {
            TabEditorModes.Width = Width - 22;
            TabEditorModes.ItemSize = new Size((TabEditorModes.Width - 80) / TabEditorModes.TabPages.Count, TabEditorModes.ItemSize.Height);
            ResizeWindow();

            PanelUtility.Location = new Point(Width - PanelUtility.Width, StatusStrip.Location.Y);
        }

        private void MapEditorForm_Deactivate(object sender, EventArgs e)
        {
            DisableAnimation(false);
        }

        private void MapEditorForm_Activated(object sender, EventArgs e)
        {
            EnableAnimation(false);
        }

        private void createNewMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseMap() == 0)
            {
                CreateMap();
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseMap() == 0)
            {
                LoadMap();
            }
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
            Close();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            op_queue?.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            op_queue?.Redo();
        }

        private int CreateMap()
        {
            SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMap, "MapEditorForm.CreateMap() called");

            // get new map parameters
            ushort map_size = 0;
            SFEngine.SFMap.MapGen.MapGenerator generator = null;
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
            {
                return -1;
            }

            // close current gamedata
            bool is_gd_correct = ((SFCategoryManager.gamedata != null) && (SFCategoryManager.gamedata.fname == SFUnPak.game_directory_name + "\\data\\GameData.cff"));

            if (!is_gd_correct)
            {
                if (MainForm.data != null)
                {
                    if (MainForm.data.data_loaded)
                    {
                        if (MainForm.data.close_data() == DialogResult.Cancel)
                        {
                            StatusText.Text = "Could not load game data: Another game data is already loaded, which will cause desync!";
                            return -1;
                        }
                    }
                }
                else if (SFCategoryManager.ready)
                {
                    SFCategoryManager.UnloadAll();
                }
            }

            // first, load view
            if (!initialized_view)
            {
                ForceSetStatusText("Initializing view...", Color.Green);
                CreateRenderWindow();
                if (!initialized_view)
                {
                    StatusText.Text = "Could not initialize view. Aborting";
                    SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.SFMap, "MapEditorForm.CreateMap(): Failed to initialize view!");
                    return -1;
                }
            }

            // load SQL Lua files
            if (!SFLuaEnvironment.data_loaded)
            {
                ForceSetStatusText("Loading SQL Lua files...", Color.Blue);
                SFLuaEnvironment.LoadSQL(false);
                if (!SFLuaEnvironment.data_loaded)
                {
                    StatusText.Text = "Could not load SQL Lua files. Can't create new map";
                    SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.SFMap, "MapEditorForm.CreateMap(): Failed to load SQL data!");
                    return -1;
                }
            }

            // load in gamedata from game directory
            if (!is_gd_correct)
            {
                ForceSetStatusText("Loading GameData.cff...", Color.DarkOrange);
                if (SFCategoryManager.gamedata.Load(SFUnPak.game_directory_name + "\\data\\GameData.cff") != 0)
                {
                    StatusText.Text = "Failed to load gamedata";
                    return -2;
                }

                if (MainForm.data != null)
                {
                    MainForm.data.mapeditor_set_gamedata();
                }
                else
                {
                    SFCategoryManager.manual_SetGamedata();
                }
            }
            // load resource names
            if (!SFResourceManager.pak_animations_listed)
            {
                ForceSetStatusText("Loading resource names...", Color.Purple);
                SFResourceManager.ListAllPakAnimations();
            }

            SFRenderEngine.ResetTextures();
            SFRenderEngine.scene.root.Visible = true;
            SFRenderEngine.scene.GenerateMissingMesh();

            // create and generate map
            map = new SFEngine.SFMap.SFMap();
            map.OnMapLoadStateChange = ForceSetStatusText;
            map.CreateDefault(map_size, generator);

            SFRenderEngine.scene.map = map;
            InitEditorMode();

            map.selection_helper.SetCursorPosition(new SFCoord(1, 1));
            map.selection_helper.SetCursorVisibility(true);

            ResetCamera();

            ui = new MapEditorUI(map);
            ui.InitMinimap(map.width, map.height);
            ui.SetMinimapSize(256);
            ui.RedrawMinimap();
            ui.RedrawMinimapIcons();

            op_queue = new MapEditorOperatorQueue();
            op_queue.map = map;

            // finishing actions
            RenderWindow.Enabled = true;
            RenderWindow.Invalidate();

            if (SFEngine.Settings.DynamicMap)
            {
                EnableAnimation(true);
            }

            if (MainForm.data != null)
            {
                SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMap, "MapEditorForm.CreateMap(): Synchronized with gamedata editor");
                if (!is_gd_correct)
                {
                    MessageBox.Show("Note: Editor now operates on gamedata file in your Spellforce directory. Modifying in-editor gamedata and saving results will result in permanent change to your gamedata in your Spellforce directory.");
                }
            }

            ready = true;

            GC.Collect();
            Text = "Map Editor - new map";

            SFEngine.LogUtils.Log.TotalMemoryUsage();
            return 0;
        }

        private void ForceSetStatusText(string text, Color col)
        {
            StatusText.Text = text;
            StatusText.ForeColor = col;
            StatusText.GetCurrentParent().Refresh();
        }

        private int LoadMap()
        {
            SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMap, "MapEditorForm.LoadMap() called");

            if (OpenMap.ShowDialog() == DialogResult.OK)
            {
                bool is_gd_correct = ((SFCategoryManager.gamedata != null) && (SFCategoryManager.gamedata.fname == SFUnPak.game_directory_name + "\\data\\GameData.cff"));
                // check if gamedata is open in the editor and prompt to close it
                if (!is_gd_correct)
                {
                    if (MainForm.data != null)
                    {
                        if (MainForm.data.data_loaded)
                        {
                            if (MainForm.data.close_data() == DialogResult.Cancel)
                            {
                                StatusText.Text = "Could not load game data: Another game data is already loaded, which will cause desync!";
                                return -1;
                            }
                        }
                    }
                    else if (SFCategoryManager.ready)
                    {
                        SFCategoryManager.UnloadAll();
                    }
                }

                // first, load view
                if (!initialized_view)
                {
                    ForceSetStatusText("Initializing view...", Color.Green);
                    CreateRenderWindow();
                    if (!initialized_view)
                    {
                        StatusText.Text = "Could not initialize view. Aborting";
                        SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.SFMap, "MapEditorForm.LoadMap(): Failed to initialize view!");
                        return -1;
                    }
                }

                // load SQL Lua files
                if (!SFLuaEnvironment.data_loaded)
                {
                    ForceSetStatusText("Loading SQL Lua files...", Color.Blue);
                    SFLuaEnvironment.LoadSQL(false);
                    if (!SFLuaEnvironment.data_loaded)
                    {
                        StatusText.Text = "Could not load SQL Lua files. Can't load map.";
                        SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.SFMap, "MapEditorForm.LoadMap(): Failed to load SQL data!");
                        return -1;
                    }
                }

                // load gamedata
                if (!is_gd_correct)
                {
                    ForceSetStatusText("Loading GameData.cff...", Color.DarkOrange);
                    if (SFCategoryManager.gamedata.Load(SFUnPak.game_directory_name + "\\data\\GameData.cff") != 0)
                    {
                        StatusText.Text = "Failed to load gamedata";
                        return -2;
                    }

                    if (MainForm.data != null)
                    {
                        MainForm.data.mapeditor_set_gamedata();
                    }
                    else
                    {
                        SFCategoryManager.manual_SetGamedata();
                    }
                }

                // load resource names
                if (!SFResourceManager.pak_animations_listed)
                {
                    ForceSetStatusText("Loading resource names...", Color.Purple);
                    SFResourceManager.ListAllPakAnimations();
                }

                SFRenderEngine.ResetTextures();
                SFRenderEngine.scene.root.Visible = true;
                SFRenderEngine.scene.GenerateMissingMesh();

                map = new SFEngine.SFMap.SFMap();
                map.OnMapLoadStateChange = ForceSetStatusText;
                try
                {
                    if (map.Load(OpenMap.FileName) != 0)
                    {
                        ForceSetStatusText("Failed to load map", Color.Red);
                        map = null;
                        DestroyRenderWindow();
                        return -3;
                    }
                }
                catch (InvalidDataException)
                {
                    ForceSetStatusText("Map contains invalid data!", Color.Red);
                    map = null;
                    DestroyRenderWindow();
                    return -4;
                }

                SFRenderEngine.scene.map = map;
                InitEditorMode();

                map.selection_helper.SetCursorPosition(new SFCoord(1, 1));
                map.selection_helper.SetCursorVisibility(true);

                ResetCamera();

                ui = new MapEditorUI(map);
                ui.InitMinimap(map.width, map.height);
                ui.SetMinimapSize(256);
                ui.RedrawMinimap();
                ui.RedrawMinimapIcons();

                op_queue = new MapEditorOperatorQueue();
                op_queue.map = map;

                RenderWindow.Enabled = true;
                RenderWindow.Invalidate();

                if (SFEngine.Settings.DynamicMap)
                {
                    EnableAnimation(true);
                }

                if (MainForm.data != null)
                {
                    SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMap, "MapEditorForm.LoadMap(): Synchronized with gamedata editor");
                    if (!is_gd_correct)
                    {
                        MessageBox.Show("Note: Editor now operates on gamedata file in your Spellforce directory. Modifying in-editor gamedata and saving results will result in permanent change to your gamedata in your Spellforce directory.");
                    }
                }

                GC.Collect();

                Text = "Map Editor - " + OpenMap.FileName;

                ready = true;

                SFEngine.LogUtils.Log.TotalMemoryUsage();
                return 0;
            }

            return -5;
        }



        private DialogResult SaveMap()
        {
            SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMap, "MapEditorForm.SaveMap() called");

            if (map == null)
            {
                return DialogResult.No;
            }

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
                    if (MainForm.data.op_queue.operators.Count != 0)
                    {
                        MainForm.data.save_data();
                    }
                }
            }

            return dr;
        }



        private int CloseMap()
        {
            SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMap, "MapEditorForm.CloseMap() called");

            if (map == null)
            {
                return 0;
            }

            Focus();

            DialogResult dr = MessageBox.Show(
                "Do you want to save the map before quitting? This will also overwrite gamedata if modified", "Save before quit?", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Cancel)
            {
                return -1;
            }
            else if (dr == DialogResult.Yes)
            {
                if (SaveMap() == DialogResult.Cancel)
                {
                    return -2;
                }
            }

            ready = false;

            if (teamcomp_form != null)
            {
                teamcomp_form.Close();
            }

            if (visibility_form != null)
            {
                visibility_form.Close();
            }

            if (importhmap_form != null)
            {
                importhmap_form.Close();
            }

            if (exporthmap_form != null)
            {
                exporthmap_form.Close();
            }

            if (undohistory_form != null)
            {
                undohistory_form.Close();
            }

            TabEditorModes.Enabled = false;
            InspectorClear();

            RenderWindow.Enabled = false;

            TreeEntities.Nodes.Clear();
            unit_tree = null;
            obj_tree = null;

            map.Unload();

            DisableAnimation(false);

            mask_feature_object = null;
            mask_value = 0;
            mask_selection.Clear();

            PanelDecalGroups.Controls.Clear();

            SFRenderEngine.scene.map = null;
            SFRenderEngine.scene.RemoveSceneNode(SFRenderEngine.scene.root);

            SFRenderEngine.scene.Clear();

            SFSubModel3D.Cache.Clear();
            SFModelSkinChunk.Cache.Clear();

            //ui.UninitMinimap();
            if (ui != null)
            {
                ui.Dispose();
                ui = null;
            }
            SFRenderEngine.ui.Dispose();

            op_queue.map = null;
            op_queue = null;

            QuickSelect.QsRef = null;

            if (MainForm.viewer != null)
            {
                MainForm.viewer.ResetScene();
            }

            map = null;

            Text = "Map Editor";
            GC.Collect();

            SFEngine.LogUtils.Log.TotalMemoryUsage();

            return 0;
        }

        private void CreateRenderWindow()
        {
            if (initialized_view)
            {
                return;
            }

            RenderWindow = new GLControl(new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(32), 24, 8), 4, 2, OpenTK.Graphics.GraphicsContextFlags.Default);
            Controls.Add(RenderWindow);

            int ystart = PanelObjectSelector.Location.Y;
            int yend = StatusStrip.Location.Y;
            int w_height = Math.Max(100, yend - ystart - 3);
            int w_width = Math.Max(100, Width - 22 - (PanelInspector.Visible ? PanelInspector.Width : 0)
                - (PanelObjectSelector.Visible ? PanelObjectSelector.Width : 0));
            int xstart = (PanelObjectSelector.Visible ? PanelObjectSelector.Location.X + PanelObjectSelector.Width + 3 : 0);
            RenderWindow.Location = new Point(xstart, ystart);
            RenderWindow.Size = new Size(w_width, w_height);
            RenderWindow.Enabled = false;
            RenderWindow.VSync = SFEngine.Settings.VSync;

            RenderWindow.Paint += new System.Windows.Forms.PaintEventHandler(RenderWindow_Paint);
            RenderWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(RenderWindow_MouseDown);
            RenderWindow.MouseEnter += new System.EventHandler(RenderWindow_MouseEnter);
            RenderWindow.MouseLeave += new System.EventHandler(RenderWindow_MouseLeave);
            RenderWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(RenderWindow_MouseMove);
            RenderWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(RenderWindow_MouseUp);
            RenderWindow.MouseWheel += new MouseEventHandler(RenderWindow_MouseWheel);

            RenderWindow.MakeCurrent();

            // after render window is created, scene and engine are initialized
            SFRenderEngine.scene.Init();
            SFRenderEngine.Initialize(new Vector2(RenderWindow.ClientSize.Width, RenderWindow.ClientSize.Height));
            TimerUpdatesPerSecond.Start();

            // set light direction (move somewhere else in the future, before this function is called)
            SFRenderEngine.scene.atmosphere.SetSunLocation(135, 60);
            // set up object fadein fadeout
            SFRenderEngine.SetObjectFadeRange(SFEngine.Settings.ObjectFadeMin, SFEngine.Settings.ObjectFadeMax);

            initialized_view = true;

            SFEngine.LogUtils.Log.TotalMemoryUsage();
        }

        // after this is called, memory will be freed (?)
        private void DestroyRenderWindow()
        {
            if (!initialized_view)
            {
                return;
            }

            RenderWindow.MouseDown -= new System.Windows.Forms.MouseEventHandler(RenderWindow_MouseDown);
            RenderWindow.MouseEnter -= new System.EventHandler(RenderWindow_MouseEnter);
            RenderWindow.MouseLeave -= new System.EventHandler(RenderWindow_MouseLeave);
            RenderWindow.MouseMove -= new System.Windows.Forms.MouseEventHandler(RenderWindow_MouseMove);
            RenderWindow.MouseUp -= new System.Windows.Forms.MouseEventHandler(RenderWindow_MouseUp);
            RenderWindow.MouseWheel -= new MouseEventHandler(RenderWindow_MouseWheel);
            RenderWindow.Paint -= new System.Windows.Forms.PaintEventHandler(RenderWindow_Paint);
            Controls.Remove(RenderWindow);
            RenderWindow.Dispose();
            RenderWindow = null;
            initialized_view = false;

            TimerUpdatesPerSecond.Stop();
            UpdatesText.Text = "";
        }

        private void RenderWindow_Paint(object sender, PaintEventArgs e)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            SFRenderEngine.RenderScene();
            RenderWindow.SwapBuffers();
            timer.Stop();
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
            if (selected_editor != null)
            {
                selected_editor.OnMouseUp(e.Button);
            }

            update_render = true;

        }

        private void RenderWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouse_on_view)
            {
                return;
            }
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

        // enables unit animation, if DynamicMap is enabled
        public void EnableAnimation(bool force_load = false)
        {
            dynamic_render = SFEngine.Settings.DynamicMap;
            if (!SFEngine.Settings.DynamicMap)
            {
                return;
            }

            SFRenderEngine.scene.delta_timer.Restart();
            if (!force_load)
            {
                return;
            }

            if (map != null)
            {
                foreach (var unit in map.unit_manager.units)
                {
                    map.unit_manager.RestartAnimation(unit);
                }
            }
        }

        // disables unit animation
        public void DisableAnimation(bool force_unload = false)
        {
            dynamic_render = false;
            SFRenderEngine.scene.delta_timer.Stop();
            if (!force_unload)
            {
                return;
            }

            if (map != null)
            {
                foreach (var unit in map.unit_manager.units)
                {
                    foreach (SceneNodeAnimated anim_node in unit.node.Children)
                    {
                        anim_node.SetAnimation(null, false);
                        anim_node.SetSkeleton(anim_node.Skeleton);
                        //anim_node.SetSkeletonSkin(anim_node.Skeleton, anim_node.Skin);
                    }
                }
            }
        }

        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            if (map == null)
            {
                return;
            }

            TimerAnimation.Start();

            bool update_ui = false;

            // rotating view by mouse
            if (mouse_scroll)
            {
                Vector2 scroll_mouse_end = new Vector2(Cursor.Position.X, Cursor.Position.Y);
                Vector2 scroll_translation = (scroll_mouse_end - scroll_mouse_start) * SFRenderEngine.scene.DeltaTime / 250f;

                if (scroll_translation != Vector2.Zero)
                {
                    SetCameraAzimuthAltitude(SFRenderEngine.scene.camera.Direction.X - scroll_translation.X, SFRenderEngine.scene.camera.Direction.Y - scroll_translation.Y);
                }

                update_render = true;
                update_ui = true;
            }

            // moving view by arrow keys
            Vector2 movement_vector = new Vector2(0, 0);
            if (arrows_pressed[0])
            {
                movement_vector += new Vector2(1, 0);
            }

            if (arrows_pressed[1])
            {
                movement_vector += new Vector2(-1, 0);
            }

            if (arrows_pressed[2])
            {
                movement_vector += new Vector2(0, -1);
            }

            if (arrows_pressed[3])
            {
                movement_vector += new Vector2(0, +1);
            }

            if (movement_vector != new Vector2(0, 0))
            {
                movement_vector = MathUtils.RotateVec2Mirrored(movement_vector, SFRenderEngine.scene.camera.Direction.X + (float)(Math.PI / 2));
                movement_vector *= 60.0f * camera_speed_factor * SFRenderEngine.scene.DeltaTime;
                MoveCameraWorldMapPos(SFRenderEngine.scene.camera.position.Xz + movement_vector);
                update_render = true;
                update_ui = true;
            }

            // rotating view by home/end/pageup/pagedown
            movement_vector = new Vector2(0, 0);
            if (rotation_pressed[0])
            {
                movement_vector += new Vector2(-1, 0);
            }

            if (rotation_pressed[1])
            {
                movement_vector += new Vector2(1, 0);
            }

            if (rotation_pressed[2])
            {
                movement_vector += new Vector2(0, -1);
            }

            if (rotation_pressed[3])
            {
                movement_vector += new Vector2(0, 1);
            }

            if (movement_vector != new Vector2(0, 0))
            {
                movement_vector *= 2.0f * SFRenderEngine.scene.DeltaTime;
                SetCameraAzimuthAltitude(SFRenderEngine.scene.camera.Direction.X - movement_vector.X, SFRenderEngine.scene.camera.Direction.Y - movement_vector.Y);
                update_render = true;
                update_ui = true;
            }

            SFRenderEngine.scene.camera.Update(0);

            // mouse actions
            if (mouse_on_view)
            {
                float px, py;
                px = Cursor.Position.X - Location.X - RenderWindow.Location.X - 8;
                py = Cursor.Position.Y - Location.Y - RenderWindow.Location.Y - 29;

                if (!ui.ProcessInput(px, py, mouse_pressed))
                {
                    //find point which mouse hovers at
                    float wx, wy;
                    wx = px / RenderWindow.Size.Width;
                    wy = py / RenderWindow.Size.Height;
                    Vector3 r_start = SFRenderEngine.scene.camera.position;
                    Vector3 r_end = SFRenderEngine.scene.camera.ScreenToWorld(new Vector2(wx, wy));
                    r_end = (((r_end - r_start).Normalized()) * 400.0f) + r_start;    // 400 - ray length
                    Ray ray = new Ray(r_start, r_end - r_start);

                    bool ray_success = ray.Intersect(map.heightmap, out Vector3 result);

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
                            StatusText.Text = $"Cursor position: {inv_cursor_coord}";
                            SetSpecificText(inv_cursor_coord);
                            StatusStrip.ResumeLayout();
                        }

                        // on click action
                        if ((mouse_pressed) && (selected_editor != null))
                        {
                            System.Diagnostics.Debug.WriteLine($"IX {map.heightmap.GetChunk(inv_cursor_coord).ix} | IY {map.heightmap.GetChunk(inv_cursor_coord).iy} | PRESS");
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
                    {
                        SetCameraViewPoint(clicked_pos);
                    }
                }
            }

            // render time
            // heavy tasks go here
            if (update_render)
            {
                map.ocean.SetPosition(SFRenderEngine.scene.camera.position);
                SFRenderEngine.scene.UpdateVisibleChunks(map.heightmap);
                map.selection_helper.Update();

                SFRenderEngine.scene.delta_timer.Stop();
                SFRenderEngine.scene.deltatime = SFRenderEngine.scene.delta_timer.ElapsedMilliseconds / (float)1000;
                SFRenderEngine.scene.delta_timer.Restart();
                SFRenderEngine.scene.Update(SFRenderEngine.scene.deltatime);

                SFRenderEngine.ui.Update();
                UpdateSunFrustum();
                RenderWindow.Invalidate();
                updates_this_second += 1;
                update_render = false;
            }

            if (dynamic_render)
            {
                update_render = true;
            }

            if ((SFRenderEngine.scene.selected_node != null) && ((SFRenderEngine.scene.frame_counter % 2) == 0) && (this.ContainsFocus))
            {
                update_render = true;
            }

            if (!update_ui)
            {
                SFRenderEngine.scene.StopTimeFlow();
            }
            else
            {
                SFRenderEngine.scene.ResumeTimeFlow();
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
            byte dec_assign = map.decoration_manager.GetDecAssignment(pos);
            int lake_assign = map.lake_manager.GetLakeIndexAt(pos);
            SpecificText.Text = "H: " + map.heightmap.GetHeightAt(pos.x, pos.y).ToString() + "  "
                              + "T: " + map.heightmap.GetTileFixed(pos).ToString() + "  "
                              + "D: " + (dec_assign == 0 ? "X" : dec_assign.ToString()) + " "
                              + "L: " + (lake_assign == -1 ? "X" : lake_assign.ToString());
        }

        private void AddCameraZoom(int delta)
        {
            if (delta < 0)
            {
                zoom_level *= 1.1f;
                if (zoom_level > 6)
                {
                    zoom_level = 6;
                }
            }
            else
            {
                zoom_level *= 0.9f;
                if (zoom_level < 0.1f)
                {
                    zoom_level = 0.1f;
                }
            }
            AdjustCameraZ();
            update_render = true;
        }

        private void AdjustCameraZ()
        {
            if (map != null)
            {
                SetCameraElevation(25 * zoom_level);
            }
        }

        private void UpdateSunFrustum()
        {
            Vector2 p = new Vector2(SFRenderEngine.scene.camera.position.X, SFRenderEngine.scene.camera.position.Z);
            // calculate light bounding box

            // calculate visible heightmap bounding box, using chunks that are close enough
            float max_dist = Math.Max(
                60, 50 * zoom_level * (float)Math.Min(
                    3.0f, Math.Max(
                        0.6f, 1.0f / (0.001f + Math.Abs(
                            Math.Tan(
                                SFRenderEngine.scene.camera.Direction.Y))))));

            float xmin, xmax, ymin, ymax, zmin, zmax;
            xmin = 9999; ymin = 9999; xmax = -9999; ymax = -9999; zmin = 9999; zmax = -9999;
            foreach (SceneNodeMapChunk chunk_node in map.heightmap.visible_chunks)
            {
                Vector3 pos = chunk_node.position;

                if (max_dist < (p - new Vector2(pos.X + 8, pos.Z + 8)).Length)
                {
                    continue;
                }
                // 25 * zoom_level

                if (pos.X < xmin)
                {
                    xmin = pos.X;
                }
                else if (pos.X + 16 > xmax)
                {
                    xmax = pos.X + 16;
                }

                if (pos.Z < ymin)
                {
                    ymin = pos.Z;
                }
                else if (pos.Z + 16 > ymax)
                {
                    ymax = pos.Z + 16;
                }

                if (chunk_node.MapChunk.aabb.a.Y < zmin)
                {
                    zmin = chunk_node.MapChunk.aabb.a.Y;
                }

                if (chunk_node.MapChunk.aabb.b.Y > zmax)
                {
                    zmax = chunk_node.MapChunk.aabb.b.Y;
                }
            }
            BoundingBox aabb = new BoundingBox(new Vector3(xmin, zmin, ymin), new Vector3(xmax, zmax, ymax));

            SFRenderEngine.scene.atmosphere.sun_light.SetupLightView(aabb);
            SFRenderEngine.scene.atmosphere.sun_light.ShadowDepth = max_dist;
        }

        public void SetCameraElevation(float h)
        {
            // preserve lookat
            Vector3 cur_lookat = SFRenderEngine.scene.camera.Lookat - SFRenderEngine.scene.camera.position;

            SFRenderEngine.scene.camera.Position = new Vector3(
                    SFRenderEngine.scene.camera.position.X,
                    h + map.heightmap.GetRealZ(SFRenderEngine.scene.camera.position.Xz),
                    SFRenderEngine.scene.camera.position.Z);

            SFRenderEngine.scene.camera.SetLookat(SFRenderEngine.scene.camera.position + cur_lookat);
        }

        // moves camera to given map coordinate, preserving camera elevation
        public void SetCameraMapPos(SFCoord pos)
        {
            SFRenderEngine.scene.camera.Position = new Vector3(pos.x, 0, map.height - 1 - pos.y);
            AdjustCameraZ();
        }

        // moves camera to an arbitrary point in the world
        public void SetCameraWorldPos(Vector3 pos)
        {
            SFRenderEngine.scene.camera.Position = pos;
        }

        // moves camera to a given point on the map, preserving camera elevation
        // not limited to grid points
        public void SetCameraWorldMapPos(Vector2 pos)
        {
            SFRenderEngine.scene.camera.Position = new Vector3(pos.X, 0, pos.Y);
            AdjustCameraZ();
        }

        public void MoveCameraWorldMapPos(Vector2 pos)
        {
            Vector3 cur_lookat = SFRenderEngine.scene.camera.Lookat - SFRenderEngine.scene.camera.position;
            SetCameraWorldMapPos(pos);
            SFRenderEngine.scene.camera.SetLookat(SFRenderEngine.scene.camera.position + cur_lookat);
        }

        // sets camera angles (this also modifies direction)
        // 0, 0 = UnitX
        // 270, 0 = UnitZ
        public void SetCameraAzimuthAltitude(float azimuth, float altitude)
        {
            SFRenderEngine.scene.camera.SetAzimuthAltitude(new Vector2(azimuth, altitude));
        }

        // sets camera direction (this also modifies angle)
        public void SetCameraLookAt(Vector3 pos)
        {
            SFRenderEngine.scene.camera.SetLookat(pos);
        }

        // attempts to center camera on the selected map position, preserving camera angle
        public void SetCameraViewPoint(SFCoord pos)
        {
            SetCameraWorldMapPos(new Vector2(pos.x, map.height - 1 - pos.y + 10));
            SetCameraAzimuthAltitude((float)((90 * Math.PI) / 180.0f), (float)((-70 * Math.PI) / 180.0f));
            zoom_level = 1;
            AdjustCameraZ();
            update_render = true;
        }

        public void ResetCamera()
        {
            SetCameraWorldMapPos(new Vector2(map.width / 2, map.height / 2));
            SetCameraAzimuthAltitude((float)((90 * Math.PI) / 180.0f), (float)((-70 * Math.PI) / 180.0f));
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
            int ystart = PanelObjectSelector.Location.Y;
            int yend = StatusStrip.Location.Y;
            int w_height = Math.Max(100, yend - ystart - 3);
            int w_width = Math.Max(100, Width - 22 - (PanelInspector.Visible ? PanelInspector.Width : 0)
                - (PanelObjectSelector.Visible ? PanelObjectSelector.Width : 0));
            int xstart = (PanelObjectSelector.Visible ? PanelObjectSelector.Location.X + PanelObjectSelector.Width + 3 : 0);
            PanelObjectSelector.Height = w_height;
            TreeEntities.Height = w_height - 32;
            TreeEntitytFilter.Location = new Point(TreeEntitytFilter.Location.X, w_height - 23);

            if (!initialized_view)
            {
                return;
            }

            RenderWindow.Location = new Point(xstart, ystart);
            RenderWindow.Size = new Size(w_width, w_height);
            PanelInspector.Location = new Point(6 + RenderWindow.Width + (PanelObjectSelector.Visible ? PanelObjectSelector.Width : 0), ystart);

            SFRenderEngine.ResizeView(new Vector2(w_width, w_height));
            if (ui != null)
            {
                ui.OnResize();
            }

            update_render = true;
            RenderWindow.MakeCurrent();
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
                    SFEngine.Settings.DisplayGrid = !SFEngine.Settings.DisplayGrid;
                    SFRenderEngine.RecompileMainShaders();
                    update_render = true;
                    return true;
                case Keys.H | Keys.Control:
                    SFEngine.Settings.VisualizeHeight = !SFEngine.Settings.VisualizeHeight;
                    SFRenderEngine.RecompileMainShaders();
                    update_render = true;
                    return true;
                case Keys.M | Keys.Control:
                    ui.SetMinimapVisible(!ui.GetMinimapVisible());
                    update_render = true;
                    return true;
                case Keys.F | Keys.Control:
                    SFRenderEngine.render_shadowmap_depth = !SFRenderEngine.render_shadowmap_depth;
                    update_render = true;
                    return true;
                case Keys.P | Keys.Control:
                    ui.ExportMinimap("minimap");
                    return true;
                case Keys.V | Keys.Control:
                    if (TabEditorModes.SelectedIndex == 2)
                    {
                        EntityHidePreview.Checked = !EntityHidePreview.Checked;
                    }

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
                        QuickSelect_GetCurrent().ID[0] = SFEngine.Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D2 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[1] = SFEngine.Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D3 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[2] = SFEngine.Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D4 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[3] = SFEngine.Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D5 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[4] = SFEngine.Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D6 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[5] = SFEngine.Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D7 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[6] = SFEngine.Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D8 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[7] = SFEngine.Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D9 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[8] = SFEngine.Utility.TryParseUInt16(EntityID.Text, 0);
                        QuickSelect.UpdateIDs();
                    }
                    return true;
                case Keys.D0 | Keys.Control:
                    if (QuickSelect_GetCurrent() != null)
                    {
                        QuickSelect_GetCurrent().ID[9] = SFEngine.Utility.TryParseUInt16(EntityID.Text, 0);
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
                {
                    arrows_pressed[0] = false;
                }
                else if ((int)msg.WParam == 0x27) // right
                {
                    arrows_pressed[1] = false;
                }
                else if ((int)msg.WParam == 0x26) // up
                {
                    arrows_pressed[2] = false;
                }
                else if ((int)msg.WParam == 0x28) // down
                {
                    arrows_pressed[3] = false;
                }
                else if ((int)msg.WParam == 0x24) // home
                {
                    rotation_pressed[0] = false;
                }
                else if ((int)msg.WParam == 0x23) // end
                {
                    rotation_pressed[1] = false;
                }
                else if ((int)msg.WParam == 0x21) // pageup
                {
                    rotation_pressed[2] = false;
                }
                else if ((int)msg.WParam == 0x22) // pagedown
                {
                    rotation_pressed[3] = false;
                }
                else if ((int)msg.WParam == 0x10) // shift
                {
                    special_pressed.Shift = false;
                }
                else if ((int)msg.WParam == 0x11) // ctrl
                {
                    special_pressed.Ctrl = false;
                }
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
            Focus();
            InspectorSelect(null);
            PanelInspector.Controls.Clear();
            selected_inspector = null;
            InspectorHide();
        }

        private void InspectorHide()
        {
            if (!PanelInspector.Visible)
            {
                return;
            }

            PanelInspector.Visible = false;
            ResizeWindow();
        }

        private void InspectorShow()
        {
            if (PanelInspector.Visible)
            {
                return;
            }

            PanelInspector.Visible = true;
            ResizeWindow();
        }

        private void InspectorSet(SFMap.map_controls.MapInspector inspector)
        {
            if (selected_inspector != null)
            {
                InspectorClear();
            }

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
            PanelInspector.Height = Height - 25 - PanelInspector.Location.Y;
            PanelInspector.Location = new Point(Width - width - 22, PanelInspector.Location.Y);
            if (selected_inspector != null)
            {
                selected_inspector.Height = PanelInspector.Height;
            }

            if (PanelInspector.Visible)
            {
                ResizeWindow();
            }
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
            {
                return;
            }

            map.heightmap.overlay_flags = SFMapHeightMapFlag.NONE;
            update_render = true;
            PanelObjectSelector.Visible = false;
            external_DeselectMaskFeature();

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
            else if (TabEditorModes.SelectedIndex == 5) // MASK/SELECTION
            {
                ReselectMaskMode();
            }

            if (Settings.TerrainTextureLOD == 2)
            {
                bool cur_forcelod = Settings.ForceTerrainTextureLOD1;
                if (TabEditorModes.SelectedIndex == 1)
                {
                    Settings.ForceTerrainTextureLOD1 = true;
                }
                else
                {
                    Settings.ForceTerrainTextureLOD1 = false;
                }

                if (cur_forcelod != Settings.ForceTerrainTextureLOD1)
                {
                    SFRenderEngine.RecompileMainShaders();
                }
            }

            EntityID.Text = "0";
            ConfirmPlacementEntity();
        }

        //TERRAIN EDIT

        private void ReselectTerrainMode()
        {
            map.heightmap.overlay_flags &= SFMapHeightMapFlag.EDITOR_MASK;    // only mask visible
            PanelBrushShape.Parent = TabEditorModes.TabPages[0];
            PanelBrushShape.Location = new Point(188, 3);
            update_render = true;

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
            if (RadioWeather.Checked)
            {
                RadioWeather.Checked = false;
                RadioWeather.Checked = true;
            }
        }

        private HMapEditMode GetHeightMapEditMode()
        {
            if (RadioModeRaise.Checked)
            {
                return HMapEditMode.RAISE;
            }
            else if (RadioModeSet.Checked)
            {
                return HMapEditMode.SET;
            }
            else if (RadioModeSmooth.Checked)
            {
                return HMapEditMode.SMOOTH;
            }
            else
            {
                return HMapEditMode.SET;
            }
        }

        private HMapBrushInterpolationMode GetHeightMapInterpolationMode()
        {
            if (RadioIntConstant.Checked)
            {
                return HMapBrushInterpolationMode.CONSTANT;
            }
            else if (RadioIntLinear.Checked)
            {
                return HMapBrushInterpolationMode.LINEAR;
            }
            else if (RadioIntSquare.Checked)
            {
                return HMapBrushInterpolationMode.SQUARE;
            }
            else if (RadioIntSinusoidal.Checked)
            {
                return HMapBrushInterpolationMode.SINUSOIDAL;
            }
            else
            {
                return HMapBrushInterpolationMode.SINUSOIDAL;
            }
        }

        private BrushShape GetTerrainBrushShape()
        {
            if (RadioDiamond.Checked)
            {
                return BrushShape.DIAMOND;
            }
            else if (RadioCircle.Checked)
            {
                return BrushShape.CIRCLE;
            }
            else if (RadioSquare.Checked)
            {
                return BrushShape.SQUARE;
            }
            else
            {
                return BrushShape.CIRCLE;
            }
        }

        private void RadioHMap_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioHMap.Checked)
            {
                return;
            }

            InspectorClear();

            selected_editor = new MapHeightMapEditor()
            {
                Brush = terrain_brush,
                Value = SFEngine.Utility.TryParseUInt16(TerrainValue.Text),
                EditMode = GetHeightMapEditMode(),
                Interpolation = GetHeightMapInterpolationMode(),
                map = map
            };

            PanelFlags.Visible = false;
            PanelBrushShape.Visible = true;
            PanelStrength.Visible = true;
            PanelTerrainSettings.Visible = true;
            PanelWeather.Visible = false;
            PanelAtmoPreview.Visible = false;
            PanelLakeMode.Visible = false;

            terrain_brush.size = (float)SFEngine.Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();
            ReselectHeightMapEditMode();

            map.heightmap.overlay_flags = SFMapHeightMapFlag.EDITOR_MASK;
            update_render = true;
        }

        private void ReselectHeightMapEditMode()
        {
            if (RadioModeRaise.Checked)
            {
                RadioModeRaise.Checked = false;
                RadioModeRaise.Checked = true;
            }
            if (RadioModeSet.Checked)
            {
                RadioModeSet.Checked = false;
                RadioModeSet.Checked = true;
            }
            if (RadioModeSmooth.Checked)
            {
                RadioModeSmooth.Checked = false;
                RadioModeSmooth.Checked = true;
            }
        }

        public void HMapEditSetHeight(int h)
        {
            TerrainValue.Text = h.ToString();
            UpdateHeightmapModeValueCache();
        }

        private void BrushSizeVal_Validated(object sender, EventArgs e)
        {
            int v = SFEngine.Utility.TryParseUInt16(BrushSizeVal.Text);
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
            int v = SFEngine.Utility.TryParseUInt16(TerrainValue.Text);
            TerrainTrackbar.Value = (v < TerrainTrackbar.Minimum ? TerrainTrackbar.Minimum :
                                      (v > TerrainTrackbar.Maximum ? TerrainTrackbar.Maximum : v));

            if (RadioHMap.Checked)
            {
                UpdateHeightmapModeValueCache();
                ((MapHeightMapEditor)selected_editor).Value = v;
            }
            else if (RadioLakes.Checked)
            {
                TerrainValue.Text = v.ToString();
                ((MapLakesEditor)selected_editor).Depth = v;
                lake_mode_value = v;
            }
        }

        private void UpdateHeightmapModeValueCache()
        {
            int v = SFEngine.Utility.TryParseUInt16(TerrainValue.Text);

            switch (GetHeightMapEditMode())
            {
                case HMapEditMode.RAISE:
                    heightmap_mode_values[0] = v;
                    break;
                case HMapEditMode.SET:
                    heightmap_mode_values[1] = v;
                    break;
                case HMapEditMode.SMOOTH:
                    heightmap_mode_values[2] = v;
                    break;
                default:
                    break;
            }
        }

        private void TerrainTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (RadioHMap.Checked)
            {
                if (RadioModeSet.Checked)
                {
                    return;
                }

                TerrainValue.Text = TerrainTrackbar.Value.ToString();
                ((MapHeightMapEditor)selected_editor).Value = TerrainTrackbar.Value;
                UpdateHeightmapModeValueCache();
            }
            else if (RadioLakes.Checked)
            {
                TerrainValue.Text = TerrainTrackbar.Value.ToString();
                ((MapLakesEditor)selected_editor).Depth = TerrainTrackbar.Value;
                lake_mode_value = SFEngine.Utility.TryParseUInt16(TerrainValue.Text);
            }
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

        private void UpdateValueLabel(string text, int val_index)
        {
            TerrainValueLabel.Text = text;
            if (val_index == 3)
            {
                ((MapLakesEditor)selected_editor).Depth = lake_mode_value;
                TerrainValue.Text = lake_mode_value.ToString();
            }
            else
            {
                ((MapHeightMapEditor)selected_editor).EditMode = GetHeightMapEditMode();
                ((MapHeightMapEditor)selected_editor).Value = heightmap_mode_values[val_index];
                TerrainValue.Text = heightmap_mode_values[val_index].ToString();
            }
        }

        private void RadioModeRaise_CheckedChanged(object sender, EventArgs e)
        {
            UpdateValueLabel("Strength", 0);
        }

        private void RadioModeSet_CheckedChanged(object sender, EventArgs e)
        {

            UpdateValueLabel("Value", 1);
        }

        private void RadioModeSmooth_CheckedChanged(object sender, EventArgs e)
        {

            UpdateValueLabel("Strength %", 2);
        }

        // TERRAIN FLAGS

        private SFMapHeightMapFlag GetTerrainFlagType()
        {
            if (RadioFlagMovement.Checked)
            {
                return SFMapHeightMapFlag.FLAG_MOVEMENT;
            }
            else if (RadioFlagVision.Checked)
            {
                return SFMapHeightMapFlag.FLAG_VISION;
            }
            else
            {
                return SFMapHeightMapFlag.FLAG_MOVEMENT;
            }
        }

        private void RadioFlags_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioFlags.Checked)
            {
                return;
            }

            InspectorClear();

            selected_editor = new MapTerrainFlagsEditor()
            {
                Brush = terrain_brush,
                FlagType = GetTerrainFlagType(),
                map = map
            };

            PanelFlags.Visible = true;
            PanelFlags.Location = PanelTerrainSettings.Location;
            PanelBrushShape.Visible = true;
            PanelStrength.Visible = false;
            PanelTerrainSettings.Visible = false;
            PanelWeather.Visible = false;
            PanelAtmoPreview.Visible = false;
            PanelLakeMode.Visible = false;

            terrain_brush.size = (float)SFEngine.Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();

            map.heightmap.overlay_flags =
                SFMapHeightMapFlag.FLAG_MOVEMENT
                | SFMapHeightMapFlag.FLAG_VISION
                | SFMapHeightMapFlag.TERRAIN_MOVEMENT
                | SFMapHeightMapFlag.ENTITY_BUILDING_COLLISION
                | SFMapHeightMapFlag.ENTITY_OBJECT_COLLISION
                | SFMapHeightMapFlag.ENTITY_UNIT
                | SFMapHeightMapFlag.EDITOR_MASK;
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
            {
                return;
            }

            selected_editor = new MapLakesEditor()
            {
                Brush = terrain_brush,
                Depth = SFEngine.Utility.TryParseUInt16(TerrainValue.Text),
                map = map,
                is_selecting = (RadioLakeSelect.Checked)
            };

            PanelFlags.Visible = false;
            PanelBrushShape.Visible = true;
            PanelStrength.Visible = true;
            PanelTerrainSettings.Visible = false;
            PanelWeather.Visible = false;
            PanelAtmoPreview.Visible = false;
            PanelLakeMode.Visible = true;
            PanelLakeMode.Location = PanelTerrainSettings.Location;

            InspectorSet(new SFMap.map_controls.MapLakeInspector());

            UpdateValueLabel("Depth", 3);
            map.heightmap.overlay_flags = SFMapHeightMapFlag.LAKE_SHALLOW | SFMapHeightMapFlag.LAKE_DEEP | SFMapHeightMapFlag.LAKE_SHORE | SFMapHeightMapFlag.EDITOR_MASK;
            update_render = true;
        }

        private void RadioLakePaint_CheckedChanged(object sender, EventArgs e)
        {
            if ((TabEditorModes.SelectedIndex == 0) && (RadioLakes.Checked) && (RadioLakePaint.Checked))
            {
                ((MapLakesEditor)selected_editor).is_selecting = false;
                ((MapLakesEditor)selected_editor).SelectLake(null);
            }
        }

        private void RadioLakeSelect_CheckedChanged(object sender, EventArgs e)
        {
            if ((TabEditorModes.SelectedIndex == 0) && (RadioLakes.Checked) && (RadioLakeSelect.Checked))
            {
                ((MapLakesEditor)selected_editor).is_selecting = true;
            }
        }

        public void external_UpdateLakeDepth(short depth)
        {
            TerrainValue.Text = depth.ToString();
            if ((TabEditorModes.SelectedIndex == 0) && (RadioLakes.Checked))
            {
                ((MapLakesEditor)selected_editor).Depth = depth;
            }

            lake_mode_value = depth;
        }

        // WEATHER
        private void UpdateSunDirection()
        {
            SFRenderEngine.scene.atmosphere.SetSunLocation(SunAzimuthTrackbar.Value, SunAltitudeTrackbar.Value);
            update_render = true;
        }

        private void RadioWeather_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioWeather.Checked)
            {
                return;
            }

            InspectorClear();

            selected_editor = null;

            PanelFlags.Visible = false;
            PanelBrushShape.Visible = false;
            PanelStrength.Visible = false;
            PanelTerrainSettings.Visible = false;
            PanelWeather.Visible = true;
            PanelWeather.Location = PanelBrushShape.Location;
            PanelLakeMode.Visible = false;
            PanelAtmoPreview.Visible = true;
            PanelAtmoPreview.Location = new Point(PanelWeather.Location.X + PanelWeather.Width + 3, PanelWeather.Location.Y);

            map.heightmap.overlay_flags = SFMapHeightMapFlag.NONE;
            update_render = true;

            UpdateWeatherPanel();
            UpdateAtmoPanel();
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

        private void UpdateAtmoPanel()
        {
            SunAzimuthVal.Text = ((int)(SFRenderEngine.scene.atmosphere.sun_light.Azimuth)).ToString();
            SunAzimuthTrackbar.Value = ((int)(SFRenderEngine.scene.atmosphere.sun_light.Azimuth));
            SunAltitudeVal.Text = ((int)(SFRenderEngine.scene.atmosphere.sun_light.Altitude)).ToString();
            SunAltitudeTrackbar.Value = ((int)(SFRenderEngine.scene.atmosphere.sun_light.Altitude));
        }

        private void WClear_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[0] = SFEngine.Utility.TryParseUInt8(WClear.Text, map.weather_manager.weather[0]);
        }

        private void WCloud_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[1] = SFEngine.Utility.TryParseUInt8(WCloud.Text, map.weather_manager.weather[1]);
        }

        private void WStorm_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[2] = SFEngine.Utility.TryParseUInt8(WStorm.Text, map.weather_manager.weather[2]);
        }

        private void WLavafog_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[3] = SFEngine.Utility.TryParseUInt8(WLavafog.Text, map.weather_manager.weather[3]);
        }

        private void WLavafogBright_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[4] = SFEngine.Utility.TryParseUInt8(WLavafogBright.Text, map.weather_manager.weather[4]);
        }

        private void WDesertfog_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[5] = SFEngine.Utility.TryParseUInt8(WDesertfog.Text, map.weather_manager.weather[5]);
        }

        private void WSwampfog_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[6] = SFEngine.Utility.TryParseUInt8(WSwampfog.Text, map.weather_manager.weather[6]);
        }

        private void WLavanight_Validated(object sender, EventArgs e)
        {
            map.weather_manager.weather[7] = SFEngine.Utility.TryParseUInt8(WLavanight.Text, map.weather_manager.weather[7]);
        }

        private void SunAzimuthVal_Validated(object sender, EventArgs e)
        {
            int v = SFEngine.Utility.TryParseInt32(SunAzimuthVal.Text, (int)(SFRenderEngine.scene.atmosphere.sun_light.Azimuth));
            if (v < 0)
            {
                v = 0;
            }

            if (v > 359)
            {
                v = 359;
            }

            SunAzimuthTrackbar.Value = v;
        }

        private void SunAltitudeVal_Validated(object sender, EventArgs e)
        {
            int v = SFEngine.Utility.TryParseInt32(SunAltitudeVal.Text, (int)(SFRenderEngine.scene.atmosphere.sun_light.Altitude));
            if (v < -89)
            {
                v = -89;
            }

            if (v > 89)
            {
                v = 89;
            }

            SunAltitudeTrackbar.Value = v;
        }

        private void SunAzimuthTrackbar_ValueChanged(object sender, EventArgs e)
        {
            SunAzimuthVal.Text = SunAzimuthTrackbar.Value.ToString();
            UpdateSunDirection();
        }

        private void SunAltitudeTrackbar_ValueChanged(object sender, EventArgs e)
        {
            SunAltitudeVal.Text = SunAltitudeTrackbar.Value.ToString();
            UpdateSunDirection();
        }

        private void ButtonInvertMask_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.width; x++)
                {
                    SFCoord p = new SFCoord(x, y);
                    map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_MASK, (map.heightmap.GetFlag(p) & SFMapHeightMapFlag.EDITOR_MASK) != SFMapHeightMapFlag.EDITOR_MASK);
                }
            }
            map.heightmap.RefreshOverlay();
            update_render = true;
        }

        private void ButtonClearMask_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.width; x++)
                {
                    SFCoord p = new SFCoord(x, y);
                    map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_MASK, false);
                }
            }
            map.heightmap.RefreshOverlay();
            update_render = true;
        }

        // TERRAIN PAINT

        private SFMap.map_controls.TerrainTileType GetTileType()
        {
            if (RadioTileTypeBase.Checked)
            {
                return SFMap.map_controls.TerrainTileType.BASE;
            }
            else if (RadioTileTypeCustom.Checked)
            {
                return SFMap.map_controls.TerrainTileType.CUSTOM;
            }
            else
            {
                return SFMap.map_controls.TerrainTileType.BASE;
            }
        }

        public void SetTileType(SFMap.map_controls.TerrainTileType ttype)
        {
            if (ttype == SFMap.map_controls.TerrainTileType.BASE)
            {
                RadioTileTypeBase.Checked = false;
                RadioTileTypeBase.Checked = true;
            }
            else if (ttype == SFMap.map_controls.TerrainTileType.CUSTOM)
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
            PanelBrushShape.Visible = true;
            PanelBrushShape.Location = new Point(102, 3);

            InspectorSet(new SFMap.map_controls.MapTerrainTextureInspector());
            ((SFMap.map_controls.MapTerrainTextureInspector)selected_inspector).SetInspectorType(GetTileType());

            selected_editor = new MapTerrainTextureEditor
            {
                Brush = terrain_brush,
                map = map,
                SelectedTile = 0
            };


            terrain_brush.size = (float)SFEngine.Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();

            map.heightmap.overlay_flags |= SFMapHeightMapFlag.EDITOR_MASK;
            update_render = true;
        }

        public void external_operator_ModifyTextureSet()
        {
            if (TabEditorModes.SelectedIndex != 1)
            {
                return;
            } ((SFMap.map_controls.MapTerrainTextureInspector)selected_inspector).RefreshTexturePreview();
        }

        public void external_operator_TileChangeState(byte tile_index)
        {
            if (TabEditorModes.SelectedIndex != 1)
            {
                return;
            } ((SFMap.map_controls.MapTerrainTextureInspector)selected_inspector).UpdateTile(tile_index);
        }

        private void RadioTileTypeBase_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioTileTypeBase.Checked)
            {
                ((SFMap.map_controls.MapTerrainTextureInspector)selected_inspector).SetInspectorType(GetTileType());
            }
        }

        private void RadioTileTypeCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioTileTypeCustom.Checked)
            {
                ((SFMap.map_controls.MapTerrainTextureInspector)selected_inspector).SetInspectorType(GetTileType());
            }
        }

        private void ButtonModifyTextureSet_Click(object sender, EventArgs e)
        {
            SFMap.map_dialog.MapModifyTextureSet mmts = new SFMap.map_dialog.MapModifyTextureSet(map);
            mmts.ShowDialog();

            if (mmts.operator_modify_texture_set.PreOperatorTextureIDMap.Count != 0)
            {
                op_queue.Push(mmts.operator_modify_texture_set);
            }

            update_render = true;
            ReselectTextureMode();
        }

        // ENTITIES

        private void ReselectEntityMode()
        {
            EditCoopCampTypes.Location = PanelEntityPlacementSelect.Location;
            PanelMonumentType.Location = PanelEntityPlacementSelect.Location;
            EntityHidePreview.Location = new Point(486, 94);
            QuickSelect.Location = new Point(PanelEntityPlacementSelect.Location.X + PanelEntityPlacementSelect.Width + 6, PanelEntityPlacementSelect.Location.Y);
            QuickSelect.QsRef = null;
            InspectorSelect(null);
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
            {
                return null;
            }

            if (TabEditorModes.SelectedIndex != 2)
            {
                return null;
            }

            if (RadioEntityModeUnit.Checked)
            {
                return qs_unit;
            }

            if (RadioEntityModeBuilding.Checked)
            {
                return qs_building;
            }

            if (RadioEntityModeObject.Checked)
            {
                return qs_object;
            }

            return null;
        }

        public void external_QuickSelect_OnSet(int index, ushort id)
        {
            if (QuickSelect_GetCurrent() == null)
            {
                return;
            }

            QuickSelect_GetCurrent().ID[index] = id;
        }

        public int external_QuickSelect_DetermineCategory()
        {
            int cat_id = -1;
            if (RadioEntityModeUnit.Checked)
            {
                cat_id = 2024;
            }
            else if (RadioEntityModeObject.Checked)
            {
                cat_id = 2050;
            }
            else if (RadioEntityModeBuilding.Checked)
            {
                cat_id = 2029;
            }

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
                WinFormsUtility.TreeShallowCopy(unit_tree, TreeEntities.Nodes);
                return;
            }

            unit_tree = new Dictionary<string, TreeNode>();
            // generate race nodes
            SFCategory cat = SFCategoryManager.gamedata[2022];
            for (int i = 0; i < cat.GetElementCount(); i++)
            {
                byte race_id = (byte)cat[i][0];
                ushort race_name_index = (ushort)(cat[i][7]);
                SFCategoryElement name_elem = SFCategoryManager.FindElementText(race_name_index, SFEngine.Settings.LanguageID);

                string race_name;
                if (name_elem != null)
                {
                    race_name = name_elem[4].ToString();
                }
                else
                {
                    race_name = SFEngine.Utility.S_MISSING;
                }

                race_name = race_id.ToString() + ". " + race_name;
                unit_tree.Add(race_name, new TreeNode(race_name));
            }
            // generate unit nodes
            SFCategory units_cat = SFCategoryManager.gamedata[2024];
            for (int i = 0; i < units_cat.GetElementCount(); i++)
            {
                ushort unit_id = (ushort)units_cat[i][0];
                string unit_name = unit_id.ToString() + ". " + SFCategoryManager.GetUnitName(unit_id, true);

                ushort stats_id = (ushort)(units_cat[i][2]);
                SFCategoryElement stats_elem = SFCategoryManager.gamedata[2005].FindElementBinary(0, stats_id);
                if (stats_elem == null)
                {
                    unit_tree.Add(unit_name, new TreeNode(unit_name) { Tag = unit_id });
                    continue;
                }

                byte unit_race_id = (byte)stats_elem[2];
                int race_cat_index = cat.GetElementIndex(unit_race_id);

                ushort race_name_index = (ushort)(cat[race_cat_index][7]);
                SFCategoryElement name_elem = SFCategoryManager.FindElementText(race_name_index, SFEngine.Settings.LanguageID);
                string race_name;
                if (name_elem != null)
                {
                    race_name = name_elem[4].ToString();
                }
                else
                {
                    race_name = SFEngine.Utility.S_MISSING;
                }

                race_name = unit_race_id.ToString() + ". " + race_name;
                if (unit_tree.ContainsKey(race_name))
                {
                    unit_tree[race_name].Nodes.Add(new TreeNode(unit_name) { Tag = unit_id });
                }
                else
                {
                    unit_tree.Add(unit_name, new TreeNode(unit_name) { Tag = unit_id });
                }
            }
            // clear empty nodes
            HashSet<string> nodes_to_remove = new HashSet<string>();
            foreach (string s in unit_tree.Keys)
            {
                if ((unit_tree[s].Nodes.Count == 0) && (unit_tree[s].Tag == null))
                {
                    nodes_to_remove.Add(s);
                }
            }

            foreach (string s in nodes_to_remove)
            {
                unit_tree.Remove(s);
            }

            WinFormsUtility.TreeShallowCopy(unit_tree, TreeEntities.Nodes);
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
            SFCategory cat = SFCategoryManager.gamedata[2022];
            for (int i = 0; i < cat.GetElementCount(); i++)
            {
                byte race_id = (byte)cat[i][0];
                ushort race_name_index = (ushort)(cat[i][7]);
                SFCategoryElement name_elem = SFCategoryManager.FindElementText(race_name_index, SFEngine.Settings.LanguageID);

                string race_name;
                if (name_elem != null)
                {
                    race_name = name_elem[4].ToString();
                }
                else
                {
                    race_name = SFEngine.Utility.S_MISSING;
                }

                race_name = race_id.ToString() + ". " + race_name;
                TreeEntities.Nodes.Add(race_name, race_name);
            }
            // generate unit nodes
            SFCategory units_cat = SFCategoryManager.gamedata[2024];
            for (int i = 0; i < units_cat.GetElementCount(); i++)
            {
                ushort unit_id = (ushort)units_cat[i][0];
                string unit_name = unit_id.ToString() + ". " + SFCategoryManager.GetUnitName(unit_id, true);

                ushort stats_id = (ushort)(units_cat[i][2]);
                SFCategoryElement stats_elem = SFCategoryManager.gamedata[2005].FindElementBinary(0, stats_id);
                if (stats_elem == null)
                {
                    if (unit_name.ToLower().Contains(txt))
                    {
                        TreeEntities.Nodes.Add(new TreeNode(unit_name) { Tag = unit_id });
                    }

                    continue;
                }

                byte unit_race_id = (byte)stats_elem[2];
                int race_cat_index = cat.GetElementIndex(unit_race_id);

                ushort race_name_index = (ushort)(cat[race_cat_index][7]);
                SFCategoryElement name_elem = SFCategoryManager.FindElementText(race_name_index, SFEngine.Settings.LanguageID);
                string race_name;
                if (name_elem != null)
                {
                    race_name = name_elem[4].ToString();
                }
                else
                {
                    race_name = SFEngine.Utility.S_MISSING;
                }

                race_name = unit_race_id.ToString() + ". " + race_name;

                if ((!race_name.ToLower().Contains(txt)) && (!unit_name.ToLower().Contains(txt)))
                {
                    continue;
                }

                if (TreeEntities.Nodes.ContainsKey(race_name))
                {
                    TreeEntities.Nodes[race_name].Nodes.Add(new TreeNode(unit_name) { Tag = unit_id });
                }
                else
                {
                    TreeEntities.Nodes.Add(new TreeNode(unit_name) { Tag = unit_id });
                }
            }
            // clear empty nodes
            HashSet<string> nodes_to_remove = new HashSet<string>();
            foreach (TreeNode n in TreeEntities.Nodes)
            {
                if ((n.Nodes.Count == 0) && (n.Tag == null))
                {
                    nodes_to_remove.Add(n.Name);
                }
            }

            foreach (string s in nodes_to_remove)
            {
                TreeEntities.Nodes.RemoveByKey(s);
            }

            GC.Collect();
        }

        private void ConfirmPlacementEntity()
        {
            map.selection_helper.SetPreviewAngle(0);

            if (TabEditorModes.SelectedIndex != 2)
            {
                map.selection_helper.ResetPreview();
                return;
            }
            if (EntityHidePreview.Checked)
            {
                return;
            }

            if (RadioEntityModeUnit.Checked)
            {
                ((MapUnitEditor)selected_editor).placement_unit = SFEngine.Utility.TryParseUInt16(EntityID.Text);
                map.selection_helper.SetPreviewUnit(SFEngine.Utility.TryParseUInt16(EntityID.Text));
            }
            if (RadioEntityModeBuilding.Checked)
            {
                ((MapBuildingEditor)selected_editor).placement_building = SFEngine.Utility.TryParseUInt16(EntityID.Text);
                map.selection_helper.SetPreviewBuilding(SFEngine.Utility.TryParseUInt16(EntityID.Text));
            }
            if (RadioEntityModeObject.Checked)
            {
                ((MapObjectEditor)selected_editor).placement_object = SFEngine.Utility.TryParseUInt16(EntityID.Text);
                map.selection_helper.SetPreviewObject(SFEngine.Utility.TryParseUInt16(EntityID.Text));
                map.selection_helper.SetPreviewAngle((ushort)AngleTrackbar.Value);
            }
            if (RadioModeCoopCamps.Checked)
            {
                map.selection_helper.SetPreviewObject(2541);
            }

            if (RadioModeBindstones.Checked)
            {
                map.selection_helper.SetPreviewObject(769);
            }

            if (RadioModePortals.Checked)
            {
                map.selection_helper.SetPreviewObject(778);
            }

            if (RadioModeMonuments.Checked)
            {
                map.selection_helper.SetPreviewObject((ushort)(771 + (int)GetMonumentType()));
            }
        }

        private void TreeEntities_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                if (TabEditorModes.SelectedIndex == 2)
                {
                    EntityID.Text = e.Node.Tag.ToString();
                    ConfirmPlacementEntity();
                }
                else if (TabEditorModes.SelectedIndex == 3)
                {
                    if (selected_inspector != null)
                    {
                        ((SFMap.map_controls.MapDecorationInspector)selected_inspector).external_AssignIDToSelectedRow((ushort)(e.Node.Tag));
                    }
                }
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
            {
                GetUnitNodesByName(TreeEntitytFilter.Text);
            }

            if (RadioEntityModeBuilding.Checked)
            {
                GetBuildingNodesByName(TreeEntitytFilter.Text);
            }

            if (RadioEntityModeObject.Checked)
            {
                GetObjectNodesByName(TreeEntitytFilter.Text);
            }

            TimerTreeEntityFilter.Stop();
        }

        private void EntityHidePreview_CheckedChanged(object sender, EventArgs e)
        {
            if (EntityHidePreview.Checked)
            {
                map.selection_helper.ClearPreview();
            }
            else
            {
                ConfirmPlacementEntity();
            }
        }

        private void RadioEntityModeUnit_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioEntityModeUnit.Checked)
            {
                return;
            }

            PanelObjectSelector.Visible = true;
            InspectorSet(new SFMap.map_controls.MapUnitInspector());
            GenerateUnitTree();

            selected_editor = new MapUnitEditor()
            {
                map = map
            };

            PanelEntityPlacementSelect.Visible = true;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = false;

            QuickSelect.Visible = true;
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
            {
                return;
            }

            int cat_id = -1;
            if (RadioEntityModeUnit.Checked)
            {
                cat_id = 2024;
            }
            else if (RadioEntityModeObject.Checked)
            {
                cat_id = 2050;
            }
            else if (RadioEntityModeBuilding.Checked)
            {
                cat_id = 2029;
            }

            if (cat_id == -1)
            {
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = SFEngine.Utility.TryParseUInt16(EntityID.Text);
                int real_elem_id = SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                {
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
                }
            }
        }

        // load object picker tree
        private void GenerateBuildingTree()
        {
            TreeEntities.Nodes.Clear();
            if (building_tree != null)
            {
                WinFormsUtility.TreeShallowCopy(building_tree, TreeEntities.Nodes);
                return;
            }

            building_tree = new Dictionary<string, TreeNode>();
            // generate race nodes
            SFCategory cat = SFCategoryManager.gamedata[2022];
            for (int i = 0; i < cat.GetElementCount(); i++)
            {
                byte race_id = (byte)cat[i][0];
                ushort race_name_index = (ushort)(cat[i][7]);
                SFCategoryElement name_elem = SFCategoryManager.FindElementText(race_name_index, SFEngine.Settings.LanguageID);

                string race_name;
                if (name_elem != null)
                {
                    race_name = name_elem[4].ToString();
                }
                else
                {
                    race_name = SFEngine.Utility.S_MISSING;
                }

                race_name = race_id.ToString() + ". " + race_name;
                building_tree.Add(race_name, new TreeNode(race_name));
            }
            // generate building nodes
            SFCategory buildings_cat = SFCategoryManager.gamedata[2029];
            for (int i = 0; i < buildings_cat.GetElementCount(); i++)
            {
                ushort building_id = (ushort)buildings_cat[i][0];

                byte building_race_id = (byte)buildings_cat[i][1];
                int race_cat_index = cat.GetElementIndex(building_race_id);

                ushort race_name_index = (ushort)(cat[race_cat_index][7]);
                SFCategoryElement name_elem = SFCategoryManager.FindElementText(race_name_index, SFEngine.Settings.LanguageID);
                string race_name;
                if (name_elem != null)
                {
                    race_name = name_elem[4].ToString();
                }
                else
                {
                    race_name = SFEngine.Utility.S_MISSING;
                }

                race_name = building_race_id.ToString() + ". " + race_name;
                string building_name = building_id.ToString() + ". " + SFCategoryManager.GetBuildingName(building_id);
                if (building_tree.ContainsKey(race_name))
                {
                    building_tree[race_name].Nodes.Add(new TreeNode(building_name) { Tag = building_id });
                }
                else
                {
                    building_tree.Add(building_name, new TreeNode(building_name) { Tag = building_id });
                }
            }
            // clear empty nodes
            HashSet<string> nodes_to_remove = new HashSet<string>();
            foreach (string s in building_tree.Keys)
            {
                if ((building_tree[s].Nodes.Count == 0) && (building_tree[s].Tag == null))
                {
                    nodes_to_remove.Add(s);
                }
            }

            foreach (string s in nodes_to_remove)
            {
                building_tree.Remove(s);
            }

            WinFormsUtility.TreeShallowCopy(building_tree, TreeEntities.Nodes);
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
            SFCategory cat = SFCategoryManager.gamedata[2022];
            for (int i = 0; i < cat.GetElementCount(); i++)
            {
                byte race_id = (byte)cat[i][0];
                ushort race_name_index = (ushort)(cat[i][7]);
                SFCategoryElement name_elem = SFCategoryManager.FindElementText(race_name_index, SFEngine.Settings.LanguageID);

                string race_name;
                if (name_elem != null)
                {
                    race_name = name_elem[4].ToString();
                }
                else
                {
                    race_name = SFEngine.Utility.S_MISSING;
                }

                race_name = race_id.ToString() + ". " + race_name;
                TreeEntities.Nodes.Add(race_name, race_name);
            }
            // generate building nodes
            SFCategory buildings_cat = SFCategoryManager.gamedata[2029];
            for (int i = 0; i < buildings_cat.GetElementCount(); i++)
            {
                ushort building_id = (ushort)buildings_cat[i][0];

                byte building_race_id = (byte)buildings_cat[i][1];
                int race_cat_index = cat.GetElementIndex(building_race_id);

                ushort race_name_index = (ushort)(cat[race_cat_index][7]);
                SFCategoryElement name_elem = SFCategoryManager.FindElementText(race_name_index, SFEngine.Settings.LanguageID);
                string race_name;
                if (name_elem != null)
                {
                    race_name = name_elem[4].ToString();
                }
                else
                {
                    race_name = SFEngine.Utility.S_MISSING;
                }

                race_name = building_race_id.ToString() + ". " + race_name;
                string building_name = building_id.ToString() + ". " + SFCategoryManager.GetBuildingName(building_id);
                if ((!race_name.ToLower().Contains(txt)) && (!building_name.ToLower().Contains(txt)))
                {
                    continue;
                }

                if (TreeEntities.Nodes.ContainsKey(race_name))
                {
                    TreeEntities.Nodes[race_name].Nodes.Add(new TreeNode(building_name) { Tag = building_id });
                }
                else
                {
                    TreeEntities.Nodes.Add(new TreeNode(building_name) { Tag = building_id });
                }
            }
            // clear empty nodes
            HashSet<string> nodes_to_remove = new HashSet<string>();
            foreach (TreeNode n in TreeEntities.Nodes)
            {
                if ((n.Nodes.Count == 0) && (n.Tag == null))
                {
                    nodes_to_remove.Add(n.Name);
                }
            }

            foreach (string s in nodes_to_remove)
            {
                TreeEntities.Nodes.RemoveByKey(s);
            }

            GC.Collect();
        }

        private void RadioEntityModeBuilding_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioEntityModeBuilding.Checked)
            {
                return;
            }

            PanelObjectSelector.Visible = true;
            InspectorSet(new SFMap.map_controls.MapBuildingInspector());
            GenerateBuildingTree();

            selected_editor = new MapBuildingEditor()
            {
                map = map
            };

            PanelEntityPlacementSelect.Visible = true;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = false;

            QuickSelect.Visible = true;
            QuickSelect.QsRef = qs_building;

            EntityID.Text = "0";
            map.heightmap.overlay_flags = SFMapHeightMapFlag.ENTITY_BUILDING_COLLISION | SFMapHeightMapFlag.ENTITY_BUILDING;
            ConfirmPlacementEntity();
        }

        // load object picker tree
        private void GenerateObjectTree()
        {
            TreeEntities.Nodes.Clear();
            if (obj_tree != null)
            {
                WinFormsUtility.TreeShallowCopy(obj_tree, TreeEntities.Nodes);
                return;
            }

            obj_tree = new Dictionary<string, TreeNode>();

            SFCategory cat = SFCategoryManager.gamedata[2050];
            foreach (SFCategoryElement e in cat.elements)
            {
                UInt16 id = (UInt16)e[0];
                if ((id > 64) && (id < 128))
                {
                    continue;
                }

                if ((id >= 771) && (id <= 778))
                {
                    continue;
                }

                if (id == 769)
                {
                    continue;
                }

                if (id == 2541)
                {
                    continue;
                }

                string name = id.ToString() + ". " + SFCategoryManager.GetObjectName(id);
                string path = e[5].ToString();
                string[] path_items = path.Split('/');
                if ((path_items.Length == 1) && (path_items[0] == ""))
                {
                    path_items = new string[] { };
                }

                // add entry
                if (path_items.Length == 0)
                {
                    obj_tree.Add(name, new TreeNode(name) { Tag = id });
                    continue;
                }

                TreeNode tnc = null;
                if (!obj_tree.ContainsKey(path_items[0]))
                {
                    obj_tree.Add(path_items[0], new TreeNode(path_items[0]));
                }

                tnc = obj_tree[path_items[0]];

                for (int i = 1; i < path_items.Length; i++)
                {
                    if (!tnc.Nodes.ContainsKey(path_items[i]))
                    {
                        tnc.Nodes.Add(path_items[i], path_items[i]);
                    }

                    tnc = tnc.Nodes[path_items[i]];
                }
                tnc.Nodes.Add(new TreeNode(name) { Tag = id });
            }

            WinFormsUtility.TreeShallowCopy(obj_tree, TreeEntities.Nodes);
            GC.Collect();
        }

        private void GetObjectNodesByName(string txt)
        {
            if (txt == "")
            {
                GenerateObjectTree();
                return;
            }
            txt = txt.ToLower();

            TreeEntities.Nodes.Clear();

            SFCategory cat = SFCategoryManager.gamedata[2050];
            foreach (SFCategoryElement e in cat.elements)
            {
                UInt16 id = (UInt16)e[0];
                if ((id > 64) && (id < 128))
                {
                    continue;
                }

                if ((id >= 771) && (id <= 778))
                {
                    continue;
                }

                if (id == 769)
                {
                    continue;
                }

                if (id == 2541)
                {
                    continue;
                }

                string name = id.ToString() + ". " + SFCategoryManager.GetObjectName(id);
                string path = e[5].ToString();
                string[] path_items = path.Split('/');
                if ((path_items.Length == 1) && (path_items[0] == ""))
                {
                    path_items = new string[] { };
                }

                bool include = true;
                if (!name.ToLower().Contains(txt))
                {
                    include = false;
                    foreach (string s in path_items)
                    {
                        if (s.ToLower().Contains(txt))
                        {
                            include = true;
                            break;
                        }
                    }
                }
                if (!include)
                {
                    continue;
                }

                // add entry
                if (path_items.Length == 0)
                {
                    TreeEntities.Nodes.Add(new TreeNode(name) { Tag = id });
                    continue;
                }

                TreeNode tnc = null;
                if (!TreeEntities.Nodes.ContainsKey(path_items[0]))
                {
                    TreeEntities.Nodes.Add(path_items[0], path_items[0]);
                }

                tnc = TreeEntities.Nodes[path_items[0]];

                for (int i = 1; i < path_items.Length; i++)
                {
                    if (!tnc.Nodes.ContainsKey(path_items[i]))
                    {
                        tnc.Nodes.Add(path_items[i], path_items[i]);
                    }

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
            int v = SFEngine.Utility.TryParseUInt16(Angle.Text);
            AngleTrackbar.Value = (v >= 0 ? (v <= 359 ? v : 359) : 0);
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            Angle.Text = AngleTrackbar.Value.ToString();
            if (RadioEntityModeObject.Checked)
            {
                map.selection_helper.SetPreviewAngle((ushort)AngleTrackbar.Value);
            }
        }

        private void CheckRandomRange_CheckedChanged(object sender, EventArgs e)
        {
            Angle.Enabled = !CheckRandomRange.Checked;
            AngleTrackbar.Enabled = !CheckRandomRange.Checked;
        }

        public AngleInfo GetAngleInfo()
        {
            return new AngleInfo() { angle = SFEngine.Utility.TryParseUInt16(Angle.Text), random = CheckRandomRange.Checked };
        }

        private void RadioEntityModeObject_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioEntityModeObject.Checked)
            {
                return;
            }

            PanelObjectSelector.Visible = true;
            InspectorSet(new SFMap.map_controls.MapObjectInspector());
            GenerateObjectTree();

            selected_editor = new MapObjectEditor()
            {
                map = map
            };

            PanelEntityPlacementSelect.Visible = true;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = true;

            QuickSelect.Visible = true;
            QuickSelect.QsRef = qs_object;

            EntityID.Text = "0";
            map.heightmap.overlay_flags = SFMapHeightMapFlag.ENTITY_OBJECT_COLLISION;
            ConfirmPlacementEntity();
        }

        private void RadioModeCoopCamps_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioModeCoopCamps.Checked)
            {
                return;
            }

            PanelObjectSelector.Visible = false;
            InspectorSet(new SFMap.map_controls.MapCoopCampInspector());

            selected_editor = new MapCoopCampEditor()
            {
                map = map
            };

            QuickSelect.Visible = false;

            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = true;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = false;

            ConfirmPlacementEntity();
        }

        private void EditCoopCampTypes_Click(object sender, EventArgs e)
        {
            SFLua.SFLuaWinformManager.ShowRtsCoopSpawnGroupsForm();
            InspectorSet(new SFMap.map_controls.MapCoopCampInspector());
        }

        private void RadioModeBindstones_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioModeBindstones.Checked)
            {
                return;
            }

            PanelObjectSelector.Visible = false;
            InspectorSet(new SFMap.map_controls.MapBindstoneInspector());

            selected_editor = new MapBindstoneEditor()
            {
                map = map
            };

            QuickSelect.Visible = false;

            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = false;

            ConfirmPlacementEntity();
        }

        private void RadioModePortals_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioModePortals.Checked)
            {
                return;
            }

            PanelObjectSelector.Visible = false;
            InspectorSet(new SFMap.map_controls.MapPortalInspector());

            selected_editor = new MapPortalEditor()
            {
                map = map
            };


            QuickSelect.Visible = false;

            PanelEntityPlacementSelect.Visible = false;
            EditCoopCampTypes.Visible = false;
            PanelMonumentType.Visible = false;
            PanelObjectAngle.Visible = false;

            ConfirmPlacementEntity();
        }

        private MonumentType GetMonumentType()
        {
            if (MonumentHuman.Checked)
            {
                return MonumentType.HUMAN;
            }
            else if (MonumentElf.Checked)
            {
                return MonumentType.ELF;
            }
            else if (MonumentDwarf.Checked)
            {
                return MonumentType.DWARF;
            }
            else if (MonumentOrc.Checked)
            {
                return MonumentType.ORC;
            }
            else if (MonumentTroll.Checked)
            {
                return MonumentType.TROLL;
            }
            else if (MonumentDarkElf.Checked)
            {
                return MonumentType.DARKELF;
            }
            else if (MonumentHero.Checked)
            {
                return MonumentType.HERO;
            }
            else
            {
                return MonumentType.HERO;
            }
        }

        private void RadioModeMonuments_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioModeMonuments.Checked)
            {
                return;
            }

            PanelObjectSelector.Visible = false;
            InspectorSet(new SFMap.map_controls.MapMonumentInspector());

            selected_editor = new MapMonumentEditor()
            {
                map = map
            };

            QuickSelect.Visible = false;

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
                ConfirmPlacementEntity();
            }
        }

        private void MonumentDwarf_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentDwarf.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                ConfirmPlacementEntity();
            }
        }

        private void MonumentOrc_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentOrc.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                ConfirmPlacementEntity();
            }
        }

        private void MonumentTroll_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentTroll.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                ConfirmPlacementEntity();
            }
        }

        private void MonumentDarkElf_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentDarkElf.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                ConfirmPlacementEntity();
            }
        }

        private void MonumentHero_CheckedChanged(object sender, EventArgs e)
        {
            if (MonumentHero.Checked)
            {
                ((MapMonumentEditor)selected_editor).selected_type = GetMonumentType();
                ConfirmPlacementEntity();
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
            {
                return Color.IndianRed;
            }

            for (int k = 0; k < 30; k++)
            {
                if (map.decoration_manager.dec_groups[i].dec_id[k] != 0)
                {
                    return Color.LightGreen;
                }
            }
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
            if (TabEditorModes.SelectedIndex != 3)
            {
                return;
            }

            PanelDecalGroups.Controls[i].BackColor = GetDecGroupButtonColor(i);
        }

        public void SelectDecorationGroup(int i)
        {
            if (TabEditorModes.SelectedIndex != 3)
            {
                return;
            }

            OnDecButtonPress(PanelDecalGroups.Controls[i], null);
        }

        private void ReselectDecorationMode()
        {
            PanelBrushShape.Parent = TabEditorModes.TabPages[3];

            if (PanelDecalGroups.Controls.Count == 0)
            {
                ResetDecGroups();
            }

            PanelObjectSelector.Visible = true;
            selected_editor = new MapDecorationEditor
            {
                Brush = terrain_brush,
                map = map
            };
            InspectorSet(new SFMap.map_controls.MapDecorationInspector());
            GenerateObjectTree();

            PanelBrushShape.Visible = true;
            PanelDecalGroups.Location = new Point(PanelBrushShape.Location.X + PanelBrushShape.Width + 3, PanelDecalGroups.Location.Y);

            terrain_brush.size = (float)SFEngine.Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();

            map.heightmap.overlay_flags = SFMapHeightMapFlag.ENTITY_DECAL | SFMapHeightMapFlag.EDITOR_DECAL | SFMapHeightMapFlag.EDITOR_MASK;
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
            SetMapType(map.metadata.map_type);
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

        public void SetMapType(SFMapType mt)
        {
            if (map == null)
            {
                return;
            }

            map.metadata.map_type = mt;

            if (map.metadata.original_minimap == null)
            {
                byte[] image_data = new byte[128 * 128 * 3];
                for (int i = 0; i < 128 * 128 * 3; i++)
                {
                    image_data[i] = (byte)((i * 1024) / 3);
                }

                map.metadata.original_minimap = new SFMapMinimap();
                map.metadata.original_minimap.width = 128;
                map.metadata.original_minimap.height = 128;
                map.metadata.original_minimap.data = image_data;
            }

            switch (mt)
            {
                case SFMapType.CAMPAIGN:
                    ButtonTeams.Visible = false;
                    ButtonMinimap.Visible = false;
                    PanelCoopParams.Visible = false;

                    MapTypeCampaign.Checked = true;
                    break;
                case SFMapType.COOP:
                    ButtonTeams.Visible = true;
                    ButtonMinimap.Visible = true;
                    PanelCoopParams.Visible = true;
                    UpdateCoopSpawnParameters();

                    MapTypeCoop.Checked = true;
                    break;
                case SFMapType.MULTIPLAYER:

                    ButtonTeams.Visible = true;
                    ButtonMinimap.Visible = true;
                    PanelCoopParams.Visible = false;

                    MapTypeMultiplayer.Checked = true;
                    break;
            }
        }

        private void MapTypeCampaign_Click(object sender, EventArgs e)
        {
            op_queue.Push(new SFMap.map_operators.MapOperatorChangeMapType()
            { PreOperatorMapType = map.metadata.map_type, PostOperatorMapType = SFMapType.CAMPAIGN, ApplyOnPush = true });
        }

        private void MapTypeCoop_Click(object sender, EventArgs e)
        {
            op_queue.Push(new SFMap.map_operators.MapOperatorChangeMapType()
            { PreOperatorMapType = map.metadata.map_type, PostOperatorMapType = SFMapType.COOP, ApplyOnPush = true });
        }

        private void MapTypeMultiplayer_Click(object sender, EventArgs e)
        {
            op_queue.Push(new SFMap.map_operators.MapOperatorChangeMapType()
            { PreOperatorMapType = map.metadata.map_type, PostOperatorMapType = SFMapType.MULTIPLAYER, ApplyOnPush = true });
        }

        private void CoopSpawnParam11_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[0].param1 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam11.Text, map.metadata.coop_spawn_params[0].param1);
        }

        private void CoopSpawnParam12_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[0].param2 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam12.Text, map.metadata.coop_spawn_params[0].param2);
        }

        private void CoopSpawnParam13_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[0].param3 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam13.Text, map.metadata.coop_spawn_params[0].param3);
        }

        private void CoopSpawnParam14_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[0].param4 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam14.Text, map.metadata.coop_spawn_params[0].param4);
        }

        private void CoopSpawnParam21_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[1].param1 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam21.Text, map.metadata.coop_spawn_params[1].param1);
        }

        private void CoopSpawnParam22_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[1].param2 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam22.Text, map.metadata.coop_spawn_params[1].param2);
        }

        private void CoopSpawnParam23_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[1].param3 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam23.Text, map.metadata.coop_spawn_params[1].param3);
        }

        private void CoopSpawnParam24_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[1].param4 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam24.Text, map.metadata.coop_spawn_params[1].param4);
        }

        private void CoopSpawnParam31_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[2].param1 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam31.Text, map.metadata.coop_spawn_params[2].param1);
        }

        private void CoopSpawnParam32_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[2].param2 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam32.Text, map.metadata.coop_spawn_params[2].param2);
        }

        private void CoopSpawnParam33_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[2].param3 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam33.Text, map.metadata.coop_spawn_params[2].param3);
        }

        private void CoopSpawnParam34_Validated(object sender, EventArgs e)
        {
            map.metadata.coop_spawn_params[2].param4 = SFEngine.Utility.TryParseFloat(
                CoopSpawnParam34.Text, map.metadata.coop_spawn_params[2].param4);
        }

        private void ButtonTeams_Click(object sender, EventArgs e)
        {
            if (map == null)
            {
                return;
            }

            if (teamcomp_form != null)
            {
                return;
            }

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

        // these 3 below are UGLY, think of something to rid of them

        public void external_operator_AddOrRemoveTeamComp()
        {
            if (teamcomp_form != null)
            {
                teamcomp_form.external_RefreshTeamcompList();
            }
        }

        public void external_operator_AddTeamMember(SFMapTeamPlayer player)
        {
            if (teamcomp_form == null)
            {
                return;
            }

            teamcomp_form.AddTeamMemberEntry(player);
            teamcomp_form.FindAvailablePlayers();
        }

        public void external_operator_RemoveTeamMember(int index)
        {
            if (teamcomp_form == null)
            {
                return;
            }

            teamcomp_form.RemoveTeamMemberEntry(index);
            teamcomp_form.FindAvailablePlayers();
        }

        public void external_operator_SetPlayerState(int teamcomp_index, int team_index, int teamplayer_index)
        {
            if (teamcomp_form == null)
            {
                return;
            }

            teamcomp_form.UpdatePlayerDataUI(teamcomp_index, team_index, teamplayer_index);
        }

        // MASK / SELECTION

        private void ReselectMaskMode()
        {
            map.heightmap.overlay_flags = SFMapHeightMapFlag.EDITOR_MASK | SFMapHeightMapFlag.EDITOR_SELECTION;    // only mask visible
            update_render = true;

            PanelBrushShape.Parent = TabEditorModes.TabPages[5];
            //PanelBrushShape.Location = new Point(3, 3);

            InspectorClear();

            selected_editor = new MapPaintMaskEditor()
            {
                Brush = terrain_brush,
                map = map
            };

            terrain_brush.size = (float)Utility.TryParseUInt8(BrushSizeVal.Text);
            terrain_brush.shape = GetTerrainBrushShape();

            UpdateMaskUI(false);
        }

        private void UpdateMaskUI(bool update_flags)
        {
            PanelMaskAttribute.Visible = false;
            PanelBrushShape.Visible = false;
            PanelMaskBorderType.Visible = false;
            PanelMaskComparisonMode.Visible = false;
            PanelMaskRandom.Visible = false;
            PanelMaskSourceValue.Visible = false;
            PanelMaskFeature.Visible = false;

            Point loc_start = new Point(322, 3);

            switch (mask_source)
            {
                case MapMaskSource.ALL:
                case MapMaskSource.MASK:
                    break;
                case MapMaskSource.PAINT:
                    PanelBrushShape.Visible = true;
                    PanelBrushShape.Location = loc_start;
                    loc_start.Y += PanelBrushShape.Height + 3;
                    break;
                case MapMaskSource.TEXTURE:
                    PanelMaskSourceValue.Visible = true;
                    PanelMaskSourceValue.Location = loc_start;
                    UpdateMaskSourceValueUI();
                    loc_start.Y += PanelMaskSourceValue.Height + 3;
                    break;
                case MapMaskSource.HEIGHT:
                case MapMaskSource.SLOPE:
                    PanelMaskSourceValue.Visible = true;
                    PanelMaskSourceValue.Location = loc_start;
                    UpdateMaskSourceValueUI();
                    loc_start.Y += PanelMaskSourceValue.Height + 3;
                    PanelMaskComparisonMode.Visible = true;
                    PanelMaskComparisonMode.Location = loc_start;
                    loc_start.Y += PanelMaskComparisonMode.Height + 3;
                    break;
                case MapMaskSource.ATTRIBUTE:
                    PanelMaskAttribute.Visible = true;
                    PanelMaskAttribute.Location = loc_start;
                    UpdateMaskAttributeUI(new Point(loc_start.X + PanelMaskAttribute.Width + 3, loc_start.Y));
                    loc_start.Y += PanelMaskAttribute.Height + 3;
                    break;
                case MapMaskSource.FEATURE:
                    PanelMaskFeature.Visible = true;
                    PanelMaskFeature.Location = loc_start;
                    UpdateMaskFeatureUI(new Point(loc_start.X + PanelMaskFeature.Width + 3, loc_start.Y));
                    loc_start.Y += PanelMaskFeature.Height + 3;
                    break;
                default:
                    break;
            }

            switch (mask_filter)
            {
                case MapMaskFilter.NONE:
                    break;
                case MapMaskFilter.BORDER:
                    PanelMaskBorderType.Visible = true;
                    PanelMaskBorderType.Location = loc_start;
                    loc_start.Y += PanelMaskBorderType.Height + 3;
                    break;
                case MapMaskFilter.RANDOM:
                    PanelMaskRandom.Visible = true;
                    PanelMaskRandom.Location = loc_start;
                    MaskRandomValue.Text = mask_percentage.ToString();
                    loc_start.Y += PanelMaskRandom.Height + 3;
                    break;
                default:
                    break;
            }

            if (update_flags)
            {
                EvaluateMask();
            }
        }

        private void UpdateMaskSourceValueUI()
        {
            switch (mask_source)
            {
                case MapMaskSource.HEIGHT:
                    LabelMaskSourceType.Text = "Terrain height";
                    break;
                case MapMaskSource.TEXTURE:
                    LabelMaskSourceType.Text = "Terrain texture ID";
                    break;
                case MapMaskSource.SLOPE:
                    LabelMaskSourceType.Text = "Terrain slope";
                    break;
                default:
                    break;
            }

            MaskSourceValue.Text = mask_value.ToString();
        }

        private void UpdateMaskAttributeUI(Point loc_start)
        {
            PanelMaskSourceValue.Visible = false;
            PanelMaskComparisonMode.Visible = false;
            switch (mask_attribute)
            {
                case MapMaskAttribute.BUILDING_BLOCK:
                case MapMaskAttribute.MANUAL_BLOCK:
                case MapMaskAttribute.OBJECT_BLOCK:
                case MapMaskAttribute.SHORE:
                case MapMaskAttribute.TERRAIN_BLOCK:
                    break;
                case MapMaskAttribute.LAKE:
                    PanelMaskSourceValue.Visible = true;
                    PanelMaskSourceValue.Location = loc_start;
                    LabelMaskSourceType.Text = "Depth";
                    loc_start.Y += PanelMaskSourceValue.Height + 3;
                    PanelMaskComparisonMode.Visible = true;
                    PanelMaskComparisonMode.Location = loc_start;
                    loc_start.Y += PanelMaskComparisonMode.Height + 3;
                    break;
                default:
                    break;
            }
        }

        private void UpdateMaskFeatureUI(Point loc_start)
        {
            PanelMaskSourceValue.Visible = false;
            PanelMaskComparisonMode.Visible = false;
            switch (mask_feature)
            {
                case MapMaskFeature.BUILDING:
                case MapMaskFeature.OBJECT:
                case MapMaskFeature.WALKABLE:
                    break;
                case MapMaskFeature.LAKE:
                    PanelMaskSourceValue.Visible = true;
                    PanelMaskSourceValue.Location = loc_start;
                    LabelMaskSourceType.Text = "Depth";
                    loc_start.Y += PanelMaskSourceValue.Height + 3;
                    PanelMaskComparisonMode.Visible = true;
                    PanelMaskComparisonMode.Location = loc_start;
                    loc_start.Y += PanelMaskComparisonMode.Height + 3;
                    break;
                default:
                    break;
            }

            external_DeselectMaskFeature();
        }

        public void external_DeselectMaskFeature()
        {
            mask_feature_object = null;
            map.selection_helper.CancelSelection();
            EvaluateMask();
        }

        // BUILDING - SFMapBuilding, OBJECT - SFMapObject/InteractiveObject/Portal/Bindstone, LAKE - SFMapLake
        public void external_SelectMaskFeature(object o)
        {
            if (o == null)
            {
                external_DeselectMaskFeature();
                return;
            }

            mask_feature_object = o;
            if (o is SFMapBuilding)
            {
                map.selection_helper.SelectBuilding((SFMapBuilding)o);
            }
            else if (o is SFMapObject)
            {
                map.selection_helper.SelectObject((SFMapObject)o);
            }
            else if (o is SFMapInteractiveObject)
            {
                map.selection_helper.SelectInteractiveObject((SFMapInteractiveObject)o);
            }
            else if (o is SFMapPortal)
            {
                map.selection_helper.SelectPortal((SFMapPortal)o);
            }
            else if (o is SFMapLake)
            {
                map.selection_helper.SelectLake((SFMapLake)o);
            }
            else if (o is SFCoord)
            {
                map.selection_helper.CancelSelection();
            }
            else
            {
                external_DeselectMaskFeature();
                return;
            }

            EvaluateMask();
        }

        private void ComboSelectionSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboSelectionSource.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            mask_source = (MapMaskSource)ComboSelectionSource.SelectedIndex;
            UpdateMaskUI(true);
        }

        private void ComboSelectionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboSelectionFilter.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            mask_filter = (MapMaskFilter)ComboSelectionFilter.SelectedIndex;
            UpdateMaskUI(true);
        }

        private void ComboSelectionOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboSelectionOperation.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            mask_operation = (MapMaskOperation)ComboSelectionOperation.SelectedIndex;
        }

        private void MaskSourceValue_Leave(object sender, EventArgs e)
        {
            int tmp;
            tmp = Utility.TryParseInt32(MaskSourceValue.Text, mask_value);

            int mx = 65535;
            switch (mask_source)
            {
                case MapMaskSource.HEIGHT:
                    mx = 65535;
                    break;
                case MapMaskSource.SLOPE:
                    mx = 90;
                    break;
                case MapMaskSource.TEXTURE:
                    mx = 255;
                    break;
                default:   // lake
                    break;
            }

            tmp = Math.Max(Math.Min(mx, tmp), 0);
            if (tmp == mask_value)
            {
                return;
            }

            mask_value = tmp;
            MaskSourceValue.Text = mask_value.ToString();
            EvaluateMask();
        }

        private void ComboMaskAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboMaskAttribute.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            mask_attribute = (MapMaskAttribute)ComboMaskAttribute.SelectedIndex;
            UpdateMaskAttributeUI(new Point(PanelMaskAttribute.Location.X + PanelMaskAttribute.Width + 3, PanelMaskAttribute.Location.Y));
            EvaluateMask();
        }

        private void ComboMaskSourceComparison_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboMaskSourceComparison.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            mask_comparison = (MapMaskComparison)ComboMaskSourceComparison.SelectedIndex;
            EvaluateMask();
        }

        private void RadioMaskBorderOuter_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioMaskBorderOuter.Checked)
            {
                return;
            }

            mask_border_is_outer = true;
            EvaluateMask();
        }

        private void RadioMaskBorderInner_CheckedChanged(object sender, EventArgs e)
        {
            if (!RadioMaskBorderInner.Checked)
            {
                return;
            }

            mask_border_is_outer = false;
            EvaluateMask();
        }
        private void MaskRandomValue_Leave(object sender, EventArgs e)
        {
            mask_percentage = Utility.TryParseInt32(MaskRandomValue.Text, mask_percentage);
            mask_percentage = Math.Max(Math.Min(100, mask_percentage), 0);
            MaskRandomValue.Text = mask_percentage.ToString();
            EvaluateMask();
        }

        private void ButtonMaskRandomSeed_Click(object sender, EventArgs e)
        {
            mask_random_seed = DateTime.Now.Millisecond;
            EvaluateMask();
        }

        private void ComboMaskFeature_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboMaskFeature.SelectedIndex == Utility.NO_INDEX)
            {
                return;
            }

            mask_feature = (MapMaskFeature)ComboMaskFeature.SelectedIndex;
            UpdateMaskFeatureUI(new Point(PanelMaskFeature.Location.X + PanelMaskFeature.Width + 3, PanelMaskFeature.Location.Y));
            EvaluateMask();
        }

        private bool EvaluateComparison(float v1, float v2, MapMaskComparison comp)
        {
            switch (comp)
            {
                case MapMaskComparison.EQ:
                    return v1 == v2;
                case MapMaskComparison.GEQ:
                    return v1 >= v2;
                case MapMaskComparison.GT:
                    return v1 > v2;
                case MapMaskComparison.LEQ:
                    return v1 <= v2;
                case MapMaskComparison.LT:
                    return v1 < v2;
                case MapMaskComparison.NEQ:
                    return v1 != v2;
            }

            return false;
        }

        public void EvaluateMask()
        {
            if (TabEditorModes.SelectedIndex != 5)
            {
                return;
            }

            // 1. evaluate source
            HashSet<SFCoord> source_cells = new HashSet<SFCoord>();
            switch (mask_source)
            {
                case MapMaskSource.ALL:
                    for (int y = 0; y < map.height; y++)
                    {
                        for (int x = 0; x < map.width; x++)
                        {
                            source_cells.Add(new SFCoord(x, y));
                        }
                    }

                    break;
                case MapMaskSource.PAINT:
                    if ((selected_editor != null) && (selected_editor is MapPaintMaskEditor))
                    {
                        source_cells.UnionWith(((MapPaintMaskEditor)selected_editor).cells);
                    }

                    break;
                case MapMaskSource.MASK:
                    for (int y = 0; y < map.height; y++)
                    {
                        for (int x = 0; x < map.width; x++)
                        {
                            if (map.heightmap.IsFlagSet(new SFCoord(x, y), SFMapHeightMapFlag.EDITOR_MASK))
                            {
                                source_cells.Add(new SFCoord(x, y));
                            }
                        }
                    }

                    break;
                case MapMaskSource.TEXTURE:
                    for (int y = 0; y < map.height; y++)
                    {
                        for (int x = 0; x < map.width; x++)
                        {
                            if (map.heightmap.GetTile(new SFCoord(x, y)) == mask_value)
                            {
                                source_cells.Add(new SFCoord(x, y));
                            }
                        }
                    }

                    break;
                case MapMaskSource.HEIGHT:
                    for (int y = 0; y < map.height; y++)
                    {
                        for (int x = 0; x < map.width; x++)
                        {
                            if (EvaluateComparison(map.heightmap.GetHeightAt(x, y), mask_value, mask_comparison))
                            {
                                source_cells.Add(new SFCoord(x, y));
                            }
                        }
                    }

                    break;
                case MapMaskSource.SLOPE:
                    float slope_limit = (float)Math.Cos(mask_value * Math.PI / 180);
                    for (int y = 0; y < map.height; y++)
                    {
                        for (int x = 0; x < map.width; x++)
                        {
                            if (EvaluateComparison(-map.heightmap.GetVertexNormal(x, y).Y, -slope_limit, mask_comparison))
                            {
                                source_cells.Add(new SFCoord(x, y));
                            }
                        }
                    }

                    break;
                case MapMaskSource.ATTRIBUTE:
                    SFMapHeightMapFlag mask_flag = SFMapHeightMapFlag.NONE;
                    switch (mask_attribute)
                    {
                        case MapMaskAttribute.BUILDING_BLOCK:
                            mask_flag = SFMapHeightMapFlag.ENTITY_BUILDING_COLLISION;
                            break;
                        case MapMaskAttribute.OBJECT_BLOCK:
                            mask_flag = SFMapHeightMapFlag.ENTITY_OBJECT_COLLISION;
                            break;
                        case MapMaskAttribute.SHORE:
                            mask_flag = SFMapHeightMapFlag.LAKE_SHORE;
                            break;
                        case MapMaskAttribute.MANUAL_BLOCK:
                            mask_flag = SFMapHeightMapFlag.FLAG_MOVEMENT;
                            break;
                        case MapMaskAttribute.TERRAIN_BLOCK:
                            mask_flag = SFMapHeightMapFlag.TERRAIN_MOVEMENT;
                            break;
                        default:
                            break;
                    }
                    if (mask_flag != SFMapHeightMapFlag.NONE)
                    {
                        for (int y = 0; y < map.height; y++)
                        {
                            for (int x = 0; x < map.width; x++)
                            {
                                if (map.heightmap.IsFlagSet(new SFCoord(x, y), mask_flag))
                                {
                                    source_cells.Add(new SFCoord(x, y));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (SFMapLake lake in map.lake_manager.lakes)
                        {
                            ushort level = (ushort)(map.heightmap.GetHeightAt(lake.start.x, lake.start.y) + lake.z_diff);
                            foreach (SFCoord p in lake.cells)
                            {
                                if (EvaluateComparison(level - map.heightmap.GetHeightAt(p.x, p.y), mask_value, mask_comparison))
                                {
                                    source_cells.Add(p);
                                }
                            }
                        }
                        // lake
                    }
                    break;
                case MapMaskSource.FEATURE:
                    if (mask_feature_object == null)
                    {
                        break;
                    }

                    if (mask_feature_object is SFMapLake)
                    {
                        SFMapLake lake = (SFMapLake)mask_feature_object;

                        ushort level = (ushort)(map.heightmap.GetHeightAt(lake.start.x, lake.start.y) + lake.z_diff);
                        foreach (SFCoord p in lake.cells)
                        {
                            if (EvaluateComparison(level - map.heightmap.GetHeightAt(p.x, p.y), mask_value, mask_comparison))
                            {
                                source_cells.Add(p);
                            }
                        }
                    }
                    else if (mask_feature_object is SFCoord)
                    {
                        SFCoord mask_feature_pos = (SFCoord)mask_feature_object;
                        if (mask_feature_pos != new SFCoord(-1, -1))
                        {
                            if (mask_feature == MapMaskFeature.WALKABLE)
                            {
                                source_cells.UnionWith(map.heightmap.GetIslandByWalkable(mask_feature_pos));
                            }
                        }
                    }
                    else
                    {
                        SFMapCollisionBoundary cb = null;
                        SFCoord cpos = new SFCoord(-1, -1);
                        int angle;

                        if (mask_feature_object is SFMapBuilding)
                        {
                            map.building_manager.building_collision.TryGetValue((ushort)(((SFMapBuilding)mask_feature_object).game_id), out cb);
                        }
                        else if (mask_feature_object is SFMapObject)
                        {
                            map.object_manager.object_collision.TryGetValue((ushort)(((SFMapObject)mask_feature_object).game_id), out cb);
                        }
                        else if (mask_feature_object is SFMapInteractiveObject)
                        {
                            map.object_manager.object_collision.TryGetValue((ushort)(((SFMapInteractiveObject)mask_feature_object).game_id), out cb);
                        }
                        else if (mask_feature_object is SFMapPortal)
                        {
                            map.object_manager.object_collision.TryGetValue(778, out cb);
                        }

                        cpos = ((SFMapEntity)mask_feature_object).grid_position;
                        angle = ((SFMapEntity)mask_feature_object).angle;
                        if (cb != null)
                        {
                            source_cells.UnionWith(cb.GetCells(cpos, angle));
                        }
                    }

                    break;
                default:
                    break;
            }

            mask_selection.Clear();

            // 2. perform filter on source
            switch (mask_filter)
            {
                case MapMaskFilter.NONE:
                    mask_selection = source_cells;
                    break;
                case MapMaskFilter.BORDER:
                    if (mask_border_is_outer)
                    {
                        mask_selection = map.heightmap.GetBorder(source_cells);
                    }
                    else
                    {
                        mask_selection = map.heightmap.GetInnerBorder(source_cells);
                    }

                    break;
                case MapMaskFilter.RANDOM:
                    MathUtils.RandSetSeed(mask_random_seed);
                    foreach (SFCoord p in source_cells)
                    {
                        if ((MathUtils.Rand() % 100) < mask_percentage)
                        {
                            mask_selection.Add(p);
                        }
                    }

                    break;
                default:
                    break;
            }

            // 3. update flags
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.width; x++)
                {
                    map.heightmap.SetFlag(new SFCoord(x, y), SFMapHeightMapFlag.EDITOR_SELECTION, false);
                }
            }

            foreach (SFCoord p in mask_selection)
            {
                map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_SELECTION, true);
            }

            map.heightmap.RefreshOverlay();
            update_render = true;
        }

        private void ButtonSelectionApply_Click(object sender, EventArgs e)
        {
            if (mask_operation == MapMaskOperation.ZERO)
            {
                return;
            }

            HashSet<SFCoord> current_mask = new HashSet<SFCoord>();
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.width; x++)
                {
                    if (map.heightmap.IsFlagSet(new SFCoord(x, y), SFMapHeightMapFlag.EDITOR_MASK))
                    {
                        current_mask.Add(new SFCoord(x, y));
                    }

                    map.heightmap.SetFlag(new SFCoord(x, y), SFMapHeightMapFlag.EDITOR_MASK, false);
                }
            }

            switch (mask_operation)
            {
                case MapMaskOperation.ONE:
                    foreach (SFCoord p in mask_selection)
                    {
                        map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_MASK, true);
                    }

                    break;
                case MapMaskOperation.AND:
                    current_mask.IntersectWith(mask_selection);
                    foreach (SFCoord p in current_mask)
                    {
                        map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_MASK, true);
                    }

                    break;
                case MapMaskOperation.OR:
                    current_mask.UnionWith(mask_selection);
                    foreach (SFCoord p in current_mask)
                    {
                        map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_MASK, true);
                    }

                    break;
                case MapMaskOperation.SUBTRACT:
                    current_mask.ExceptWith(mask_selection);
                    foreach (SFCoord p in current_mask)
                    {
                        map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_MASK, true);
                    }

                    break;
                case MapMaskOperation.XOR:
                    foreach (SFCoord p in current_mask)
                    {
                        if (!mask_selection.Contains(p))
                        {
                            map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_MASK, true);
                        }
                    }

                    foreach (SFCoord p in mask_selection)
                    {
                        if (!current_mask.Contains(p))
                        {
                            map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_MASK, true);
                        }
                    }

                    break;
                default:
                    break;
            }

            EvaluateMask();
        }

        public void external_MaskSetValue(int val)
        {
            MaskSourceValue.Text = val.ToString();
            MaskSourceValue_Leave(null, null);
        }

        private void ButtonMaskInvert_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.width; x++)
                {
                    map.heightmap.SetFlag(new SFCoord(x, y), SFMapHeightMapFlag.EDITOR_MASK, !map.heightmap.IsFlagSet(new SFCoord(x, y), SFMapHeightMapFlag.EDITOR_MASK));
                }
            }

            map.heightmap.RefreshOverlay();
            update_render = true;
        }

        private void ButtonMaskClear_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.width; x++)
                {
                    map.heightmap.SetFlag(new SFCoord(x, y), SFMapHeightMapFlag.EDITOR_MASK, false);
                }
            }

            map.heightmap.RefreshOverlay();
            update_render = true;
        }

        private void visibilitySettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map == null)
            {
                return;
            }

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
            {
                return;
            }

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
            {
                return;
            }

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

        private void operationHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map == null)
            {
                return;
            }

            if (undohistory_form != null)
            {
                undohistory_form.BringToFront();
                return;
            }

            undohistory_form = new SFMap.map_dialog.MapOperatorHistoryViewer();
            undohistory_form.FormClosed += new FormClosedEventHandler(undohistory_FormClosed);
            undohistory_form.Show();
        }

        private void undohistory_FormClosed(object sender, FormClosedEventArgs e)
        {
            undohistory_form.FormClosed -= new FormClosedEventHandler(undohistory_FormClosed);
            undohistory_form = null;
        }

        private void ButtonMinimap_Click(object sender, EventArgs e)
        {
            SFMap.map_dialog.MapMinimapSettingsForm mmsf = new SFMap.map_dialog.MapMinimapSettingsForm();
            mmsf.map = map;
            mmsf.ShowDialog();
            if (mmsf.DialogResult != DialogResult.OK)
            {
                return;
            }

            map.metadata.minimap_source = mmsf.new_source;
            if (map.metadata.minimap_source == SFMapMinimapSource.EDITOR)
            {
                map.metadata.new_minimap = mmsf.new_minimap;
            }
        }
    }
}
