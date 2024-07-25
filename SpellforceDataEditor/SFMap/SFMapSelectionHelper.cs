using NAudio.Gui;
using OpenTK.Mathematics;
using System;
using SFEngine;
using SFEngine.SF3D;
using SFEngine.SF3D.SceneSynchro;
using SFEngine.SFMap;
using SFEngine.SF3D.UI;
using SFEngine.SFResources;
using SFEngine.SF3D.SFRender;
using SFEngine.SFCFF;
using SFEngine.SFLua;
using SFEngine.SFLua.lua_sql;
using SFEngine.LogUtils;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapSelectionHelper
    {
        enum SelectionType { NONE, UNIT, BUILDING, OBJECT, INTERACTIVE_OBJECT, PORTAL, LAKE }
        static SFModel3D selection_mesh = null;
        static SFModel3D cursor_mesh = null;
        static SFModel3D flood_visualizer_mesh = null;
        SFEngine.SFMap.SFMap map = null;
        SceneNodeSimple sel_obj = null;
        SceneNodeSimple cur_obj = null;
        SceneNodeSimple fld_obj = null;
        Vector2 offset = new Vector2(0, 0);
        SFCoord cursor_position = new SFCoord(0, 0);

        SFMapEntity selected_entity = null;
        SelectionType selection_type = SelectionType.NONE;

        SceneNode preview_entity = null;
        ushort preview_entity_angle = 0;
        ushort preview_unit_id = 0;
        ushort preview_building_id = 0;
        ushort preview_object_id = 0;
        Vector2 preview_entity_offset = Vector2.Zero;

        // ui stuff
        UIFont font_outline;
        UIFont font_main;

        UIElementIndex label_name_outline;
        UIElementIndex label_name;
        string current_name = "";
        float current_name_width = 0.0f;

        public SFMapSelectionHelper()
        {
            GenerateSelectionMesh(0.5f, 0.1f);

            Vector3[] vertices = new Vector3[8];
            Vector2[] uvs = new Vector2[8];
            byte[] colors = new byte[32];
            Vector3[] normals = new Vector3[8];

            uint[] indices;

            // generate mouse cursor selected position gizmo
            cursor_mesh = new SFModel3D();

            float g = 0.06f;   // gizmo width
            float h = 20.0f;   // gizmo height
            vertices[0] = new Vector3(-g, 0, -g);
            vertices[1] = new Vector3(g, 0, -g);
            vertices[2] = new Vector3(-g, 0, g);
            vertices[3] = new Vector3(g, 0, g);
            vertices[4] = new Vector3(-g, h, -g);
            vertices[5] = new Vector3(g, h, -g);
            vertices[6] = new Vector3(-g, h, g);
            vertices[7] = new Vector3(g, h, g);
            for (int i = 0; i < 8; i++)
            {
                colors[4 * i + 0] = 0xFF;
                colors[4 * i + 1] = 0xFF;
                colors[4 * i + 2] = 0x66;
                colors[4 * i + 3] = 0xFF;
                normals[i] = new Vector3(0.0f, 1.0f, 0.0f);
            }

            indices = new uint[] { 0, 2, 1,   1, 2, 3,   4, 5, 6,   5, 7, 6,
                                   0, 1, 4,   1, 5, 4,   1, 3, 5,   3, 7, 5,
                                   3, 2, 7,   2, 6, 7,   2, 0, 6,   0, 4, 6};

            SFSubModel3D sbm2 = new SFSubModel3D();
            sbm2.CreateRaw(vertices, uvs, colors, normals, indices, null);
            sbm2.material.apply_shading = false;
            sbm2.material.apply_shadow = false;
            sbm2.material.casts_shadow = false;
            sbm2.material.distance_fade = false;
            sbm2.material.transparent_pass = false;

            cursor_mesh.CreateRaw(new SFSubModel3D[] { sbm2 });
            SFResourceManager.Models.AddManually(cursor_mesh, "_CURSOR_");

            // generate flood mesh
            vertices = new Vector3[4];
            uvs = new Vector2[4];
            colors = new byte[16];
            normals = new Vector3[4];
            flood_visualizer_mesh = new SFModel3D();

            vertices[0] = new Vector3(-1024, 0, 0);
            vertices[1] = new Vector3(0, 0, 0);
            vertices[2] = new Vector3(-1024, 0, 1024);
            vertices[3] = new Vector3(0, 0, 1024);
            for (int i = 0; i < 4; i++)
            {
                colors[4 * i + 0] = 0x55;
                colors[4 * i + 1] = 0x88;
                colors[4 * i + 2] = 0xBB;
                colors[4 * i + 3] = 0xFF;
                normals[i] = new Vector3(0.0f, 1.0f, 0.0f);
            }

            indices = new uint[] { 0, 1, 2,  2, 1, 3 };

            SFSubModel3D sbm3 = new SFSubModel3D();
            sbm3.CreateRaw(vertices, uvs, colors, normals, indices, null);
            sbm3.material.apply_shading = true;
            sbm3.material.apply_shadow = false;
            sbm3.material.casts_shadow = false;
            sbm3.material.distance_fade = false;
            sbm3.material.transparent_pass = false;
            sbm3.material.water_pass = false;

            flood_visualizer_mesh.CreateRaw(new SFSubModel3D[] { sbm3 });
            SFResourceManager.Models.AddManually(flood_visualizer_mesh, "_FLOOD_MESH_");

            // ui
            font_outline = new UIFont() { space_between_letters = 2 };
            font_outline.Load("font_fonttable_0512_12px_outline_l9");
            font_main = new UIFont() { space_between_letters = 2 };
            font_main.Load("font_fonttable_0512_12px_l9");

            SFRenderEngine.ui.AddStorage(font_outline.font_texture, 256);
            SFRenderEngine.ui.AddStorage(font_main.font_texture, 256);

            label_name_outline = SFRenderEngine.ui.AddElementText(font_outline, 256, new Vector2(10, 25));
            label_name = SFRenderEngine.ui.AddElementText(font_main, 256, new Vector2(10, 25));

            SetName("");
        }

        public void AssignToMap(SFEngine.SFMap.SFMap _map)
        {
            map = _map;
            sel_obj = SFRenderEngine.scene.AddSceneNodeSimple(SFRenderEngine.scene.root, "_SELECTION_", "_SELECTION_");
            sel_obj.Rotation = Quaternion.FromEulerAngles(0, (float)Math.PI / 2, 0);
            cur_obj = SFRenderEngine.scene.AddSceneNodeSimple(SFRenderEngine.scene.root, "_CURSOR_", "_CURSOR_");
            cur_obj.Rotation = Quaternion.FromEulerAngles(0, (float)Math.PI / 2, 0);
            fld_obj = SFRenderEngine.scene.AddSceneNodeSimple(SFRenderEngine.scene.root, "_FLOOD_MESH_", "_FLOOD_MESH_");
            fld_obj.Rotation = Quaternion.FromEulerAngles(0, (float)Math.PI / 2, 0);
        }

        public void SetFloodVisible(bool visible)
        {
            fld_obj.Visible = visible;
        }

        public void SetFloodHeight(float z)
        {
            fld_obj.Position = new Vector3(0, z, 0);
        }

        public void SetSelectionPosition(SFCoord pos)
        {
            float z = map.heightmap.GetZ(pos) / 100.0f;
            sel_obj.Position = new Vector3((float)pos.x - offset.X, (float)z, (float)(map.height - pos.y - 1) + offset.Y);
        }

        public void SetSelectionVisibility(bool vis)
        {
            sel_obj.Visible = vis;
        }

        public void SetSelectionScale(float s, float s2)
        {
            GenerateSelectionMesh(s / 2, s2);
        }

        public void SetSelectionOffset(Vector2 off)
        {
            offset = off;
        }

        public void CancelSelection()
        {
            selected_entity = null;
            offset = new Vector2(0, 0);
            selection_type = SelectionType.NONE;

            SFRenderEngine.scene.selected_node = null;

            SetName("");
        }

        public void SelectUnit(SFMapUnit unit)
        {
            CancelSelection();
            selection_type = SelectionType.UNIT;
            selected_entity = unit;
            SetSelectionScale(1.0f, 0.033f);

            SFRenderEngine.scene.selected_node = unit.node;

            SetName(SFCategoryManager.GetUnitName((ushort)selected_entity.game_id, true));
        }

        public void SelectBuilding(SFMapBuilding building)
        {
            CancelSelection();
            selection_type = SelectionType.BUILDING;
            selected_entity = building;

            float sel_scale = 1.0f;
            SFLuaSQLBuildingData data = SFLuaEnvironment.buildings[building.game_id];
            if (data != null)
            {
                sel_scale = Math.Max(1.0f, (float)(data.SelectionScaling));
            }

            SetSelectionScale(sel_scale, 0.033f);

            SFRenderEngine.scene.selected_node = building.node;

            SetName(SFCategoryManager.GetBuildingName((ushort)selected_entity.game_id));
        }

        public void SelectObject(SFMapObject obj)
        {
            CancelSelection();
            selection_type = SelectionType.OBJECT;
            selected_entity = obj;

            float sel_scale = 1.0f;
            SFLuaSQLObjectData data = SFLuaEnvironment.objects[obj.game_id];
            if (data != null)
            {
                sel_scale = Math.Max(1.0f, (float)data.SelectionScaling);
            }

            SetSelectionScale(sel_scale, 0.033f);

            SFRenderEngine.scene.selected_node = obj.node;

            SetName(SFCategoryManager.GetObjectName((ushort)selected_entity.game_id));
        }

        public void SelectInteractiveObject(SFMapInteractiveObject io)
        {
            CancelSelection();
            selection_type = SelectionType.INTERACTIVE_OBJECT;
            selected_entity = io;

            float sel_scale = 1.0f;
            SFLuaSQLObjectData data = SFLuaEnvironment.objects[io.game_id];
            if (data != null)
            {
                sel_scale = Math.Max(1.0f, (float)(data.SelectionScaling));
            }

            SetSelectionScale(sel_scale, 0.033f);

            if (selected_entity.game_id == 769)   // bindstone
            {
                int player = map.metadata.FindPlayerBySpawnPos(io.grid_position);
                if (player == -1)
                {
                    SetName(Utility.S_UNKNOWN);
                }
                else
                {
                    if (map.metadata.spawns[player].text_id == 0)
                    {
                        SetName(Utility.S_NONAME);
                    }
                    else
                    {
                        SetName(SFCategoryManager.GetTextByLanguage(map.metadata.spawns[player].text_id, Settings.LanguageID));
                    }
                }
            }
            else
            {
                SetName(SFCategoryManager.GetObjectName((ushort)selected_entity.game_id));
            }

            SFRenderEngine.scene.selected_node = io.node;
        }

        public void SelectPortal(SFMapPortal p)
        {
            CancelSelection();
            selection_type = SelectionType.PORTAL;
            selected_entity = p;

            float sel_scale = 1.0f;
            SFLuaSQLObjectData data = SFLuaEnvironment.objects[778];
            if (data != null)
            {
                sel_scale = Math.Max(1.0f, (float)(data.SelectionScaling));
            }

            SetSelectionScale(sel_scale, 0.033f);

            string portal_name = Utility.S_UNKNOWN;
            int portal_id = selected_entity.game_id;
            if(SFCategoryManager.gamedata.c2053.GetItemIndex(portal_id, out int portal_index))
            {
                portal_name = SFCategoryManager.GetTextByLanguage(SFCategoryManager.gamedata.c2053[portal_index].NameID, Settings.LanguageID);
            }

            SFRenderEngine.scene.selected_node = p.node;

            SetName(portal_name);
        }

        public void SelectLake(SFMapLake lake)
        {
            CancelSelection();
            selection_type = SelectionType.LAKE;

            SFRenderEngine.scene.selected_node = lake.node;
        }

        // should be run once per render tick
        public void Update()
        {
            // selection and ui stuff
            SetSelectionVisibility((selection_type != SelectionType.NONE) && (selection_type != SelectionType.LAKE));

            SFMapHeightMap hmap = SFRenderEngine.scene.map.heightmap;
            SceneNodeCamera camera = SFRenderEngine.scene.camera;
            Vector2 text_pos = new Vector2(0, 0);

            if (selected_entity != null)
            {
                if (selection_type == SelectionType.BUILDING)//same with interactive object and object, todo...
                {
                    Vector2 off = map.building_manager.building_collision[(ushort)selected_entity.game_id].origin;
                    MathUtils.RotateVec2(in off, (float)(selected_entity.angle * Math.PI / 180), out Vector2 r_off);

                    SetSelectionOffset(r_off);
                }
                SetSelectionPosition(selected_entity.grid_position);

                Vector3 sPos = new Vector3(
                    selected_entity.grid_position.x,
                    hmap.GetRealZ(new Vector2(selected_entity.grid_position.x, hmap.height - selected_entity.grid_position.y - 1)),
                    hmap.height - selected_entity.grid_position.y - 1);
                if (Vector3.Dot(sPos - camera.position, camera.Lookat - camera.position) > 0)
                {
                    text_pos = camera.WorldToScreen(sPos);
                }
                else
                {
                    text_pos = new Vector2(1, 1);
                }
            }

            // todo: add more selection types

            text_pos.X *= SFRenderEngine.render_size.X;
            text_pos.Y *= SFRenderEngine.render_size.Y;
            text_pos.X = (float)Math.Floor(text_pos.X);
            text_pos.Y = (float)Math.Floor(text_pos.Y);

            SetNamePosition(text_pos);
        }

        public void SetPreviewEntityGridPosition(SFCoord pos)
        {
            float z = map.heightmap.GetZ(new SFCoord(pos.x, map.height - pos.y - 1)) / 100.0f;
            preview_entity.Position = new Vector3(pos.x - preview_entity_offset.X, z + 0.2f, pos.y + preview_entity_offset.Y);
        }

        // returns if cursor position changed
        public bool SetCursorPosition(SFCoord pos)
        {
            if (pos != cursor_position)
            {
                cursor_position = pos;
                float z = map.heightmap.GetZ(new SFCoord(pos.x, map.height - pos.y - 1)) / 100.0f;
                cur_obj.Position = new Vector3(pos.x, z, pos.y);

                if (preview_entity != null)
                {
                    SetPreviewEntityGridPosition(pos);
                }

                if(fld_obj.visible)
                {
                    SetFloodHeight(z);
                }

                return true;
            }
            return false;
        }

        public void SetCursorVisibility(bool vis)
        {
            cur_obj.Visible = vis;
        }

        public void ClearPreview()
        {
            if (preview_entity != null)
            {
                SFRenderEngine.scene.RemoveSceneNode(SFRenderEngine.scene.root.FindNode<SceneNode>("_PREVIEW_"));
                preview_entity = null;
            }

        }

        public void ResetPreview()
        {
            ClearPreview();
            preview_entity = SFRenderEngine.scene.AddSceneNodeEmpty(SFRenderEngine.scene.root, "_PREVIEW_");
            preview_entity.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);

            preview_entity_offset = Vector2.Zero;
        }

        public void SetPreviewUnit(ushort unit_id)
        {
            if (unit_id == 0)
            {
                ClearPreview();
                preview_unit_id = 0;
                return;
            }

            ResetPreview();

            // get unit
            preview_entity.AddNode(SFRenderEngine.scene.AddSceneUnit(unit_id, "_UNIT_" + unit_id.ToString()));

            if(!SFCategoryManager.gamedata.c2024.GetItemIndex(unit_id, out int unit_index))
            {
                return;
            }

            float unit_size = 1f;
            if (SFCategoryManager.gamedata.c2005.GetItemIndex(SFCategoryManager.gamedata.c2024[unit_index].StatsID, out unit_index))
            if (unit_index != -1)
            {
                unit_size = Math.Max(SFCategoryManager.gamedata.c2005[unit_index].UnitSize, (ushort)40) / 100.0f;
            }

            preview_entity.Scale = new Vector3(unit_size * 100 / 128);
            SetPreviewEntityGridPosition(cursor_position);

            preview_unit_id = unit_id;
        }

        public void SetPreviewBuilding(ushort building_id)
        {
            if (building_id == 0)
            {
                ClearPreview();
                preview_building_id = 0;
                return;
            }

            ResetPreview();

            // get building
            preview_entity.AddNode(SFRenderEngine.scene.AddSceneBuilding(building_id, "_BUILDING_" + building_id.ToString()));
            preview_entity.Scale = new Vector3(100 / 128f);
            map.building_manager.AddBuildingCollisionBoundary(building_id);

            preview_building_id = building_id;

            Vector2 off = map.building_manager.building_collision[building_id].origin;
            MathUtils.RotateVec2(in off, 0, out Vector2 r_off);

            preview_entity_offset = r_off;
            SetPreviewEntityGridPosition(cursor_position);
        }

        public void SetPreviewObject(ushort object_id)
        {
            if (object_id == 0)
            {
                ClearPreview();
                preview_object_id = 0;
                return;
            }

            ResetPreview();

            map.object_manager.AddObjectCollisionBoundary(object_id);

            // get building
            preview_entity.AddNode(SFRenderEngine.scene.AddSceneObject(object_id, "_OBJECT_" + object_id.ToString(), true, true));
            preview_entity.Scale = new Vector3(100 / 128f);
            SetPreviewEntityGridPosition(cursor_position);

            preview_object_id = object_id;
        }

        public void SetPreviewAngle(ushort angle)
        {
            preview_entity_angle = angle;
            if (preview_entity != null)
            {
                preview_entity.SetAnglePlane(angle);
            }
        }

        public void SetName(string name)
        {
            if (name == current_name)
            {
                return;
            }

            SFRenderEngine.ui.SetElementText(label_name_outline, font_outline, name);
            SFRenderEngine.ui.SetElementText(label_name, font_main, name);
            current_name = name;
            current_name_width = SFRenderEngine.ui.GetTextWidth(font_main, current_name);
        }

        public void SetNamePosition(Vector2 pos)
        {
            pos.X -= current_name_width / 2;
            SFRenderEngine.ui.MoveElement(label_name_outline, pos);
            SFRenderEngine.ui.MoveElement(label_name, pos);
        }

        public void GenerateSelectionMesh(float radius, float width)
        {
            // generate selection 3d model
            SFModel3D new_selection = new SFModel3D();

            // torus of NxM vertices
            int bigcircle_resolution = 64;
            int smallcircle_resolution = 12;

            Vector3[] vertices = new Vector3[bigcircle_resolution * smallcircle_resolution];
            Vector2[] uvs = new Vector2[bigcircle_resolution * smallcircle_resolution];
            byte[] colors = new byte[bigcircle_resolution * smallcircle_resolution * 4];
            Vector3[] normals = new Vector3[bigcircle_resolution * smallcircle_resolution];
            uint[] indices = new uint[bigcircle_resolution * smallcircle_resolution * 6];

            float radius_base = radius - width;
            for (int i = 0; i < bigcircle_resolution; i++)
            {
                float base_angle = (float)(((float)(i * 2) / bigcircle_resolution) * Math.PI);
                Vector3 base_coord = new Vector3((float)(radius_base * Math.Cos(base_angle)), 0.2f + width, (float)(radius_base * Math.Sin(base_angle)));

                for (int j = 0; j < smallcircle_resolution; j++)
                {
                    float offset_angle = (float)(((float)(j * 2) / smallcircle_resolution) * Math.PI);
                    Vector3 offset_coord = new Vector3(
                        (float)(width * Math.Cos(offset_angle) * Math.Cos(base_angle)),
                        (float)(width * Math.Sin(offset_angle)),
                        (float)(width * Math.Cos(offset_angle) * Math.Sin(base_angle)));

                    vertices[i * smallcircle_resolution + j] = base_coord + offset_coord;
                    uvs[i * smallcircle_resolution + j] = Vector2.One;
                    normals[i * smallcircle_resolution + j] = offset_coord.Normalized();
                    colors[(i * smallcircle_resolution + j) * 4 + 0] = 0x00;
                    colors[(i * smallcircle_resolution + j) * 4 + 1] = 0xDF;
                    colors[(i * smallcircle_resolution + j) * 4 + 2] = 0x00;
                    colors[(i * smallcircle_resolution + j) * 4 + 3] = 0xFF;
                    indices[(i * smallcircle_resolution + j) * 6 + 0] = (uint)(i * smallcircle_resolution + j);
                    indices[(i * smallcircle_resolution + j) * 6 + 1] = (uint)(((i + 1) % bigcircle_resolution) * smallcircle_resolution + j);
                    indices[(i * smallcircle_resolution + j) * 6 + 2] = (uint)(i * smallcircle_resolution + ((j + 1) % smallcircle_resolution));
                    indices[(i * smallcircle_resolution + j) * 6 + 3] = (uint)(i * smallcircle_resolution + ((j + 1) % smallcircle_resolution));
                    indices[(i * smallcircle_resolution + j) * 6 + 4] = (uint)(((i + 1) % bigcircle_resolution) * smallcircle_resolution + j);
                    indices[(i * smallcircle_resolution + j) * 6 + 5] = (uint)(((i + 1) % bigcircle_resolution) * smallcircle_resolution + ((j + 1) % smallcircle_resolution));
                }
            }

            SFSubModel3D sbm1 = new SFSubModel3D();
            sbm1.CreateRaw(vertices, uvs, colors, normals, indices, null);
            sbm1.material.apply_shading = false;
            sbm1.material.apply_shadow = false;
            sbm1.material.casts_shadow = false;
            sbm1.material.distance_fade = false;
            sbm1.material.transparent_pass = false;

            if (sel_obj != null)
            {
                SFRenderEngine.scene.RemoveSceneNode(sel_obj);
                SFResourceManager.Models.Dispose(selection_mesh);
            }
            new_selection.CreateRaw(new SFSubModel3D[] { sbm1 });
            SFResourceManager.Models.AddManually(new_selection, "_SELECTION_");
            selection_mesh = new_selection;
            if (sel_obj != null)
            {
                SFRenderEngine.scene.root.AddNode(sel_obj);
                sel_obj.Mesh = new_selection;
            }
        }

        public void Dispose()
        {
            Log.Info(LogSource.SFMap, "SFMapSelectionHelper.Dispose() called");
            SFRenderEngine.scene.RemoveSceneNode(SFRenderEngine.scene.root.FindNode<SceneNodeSimple>("_SELECTION_"));
            SFRenderEngine.scene.RemoveSceneNode(SFRenderEngine.scene.root.FindNode<SceneNodeSimple>("_CURSOR_"));
            SFRenderEngine.scene.RemoveSceneNode(SFRenderEngine.scene.root.FindNode<SceneNodeSimple>("_FLOOD_MESH_"));
            SFResourceManager.Models.Dispose(selection_mesh);
            SFResourceManager.Models.Dispose(cursor_mesh);
            SFResourceManager.Models.Dispose(flood_visualizer_mesh);
            sel_obj = null;
            cur_obj = null;
            fld_obj = null;
            ClearPreview();

            SFRenderEngine.ui.RemoveStorage(font_outline.font_texture);
            SFRenderEngine.ui.RemoveStorage(font_main.font_texture);
            font_outline.Dispose();
            font_main.Dispose();
        }
    }
}
