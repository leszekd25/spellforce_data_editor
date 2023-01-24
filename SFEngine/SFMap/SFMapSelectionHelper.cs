using OpenTK;
using System;

namespace SFEngine.SFMap
{
    public class SFMapSelectionHelper
    {
        enum SelectionType { NONE, UNIT, BUILDING, OBJECT, INTERACTIVE_OBJECT, PORTAL, LAKE }

        static SF3D.SFModel3D selection_mesh = null;
        static SF3D.SFModel3D cursor_mesh = null;
        SFMap map = null;
        SF3D.SceneSynchro.SceneNodeSimple sel_obj = null;
        SF3D.SceneSynchro.SceneNodeSimple cur_obj = null;
        Vector2 offset = new Vector2(0, 0);
        SFCoord cursor_position = new SFCoord(0, 0);

        SFMapEntity selected_entity = null;
        SelectionType selection_type = SelectionType.NONE;

        SF3D.SceneSynchro.SceneNode preview_entity = null;
        ushort preview_entity_angle = 0;
        ushort preview_unit_id = 0;
        ushort preview_building_id = 0;
        ushort preview_object_id = 0;
        Vector2 preview_entity_offset = Vector2.Zero;

        // ui stuff
        SF3D.UI.UIFont font_outline;
        SF3D.UI.UIFont font_main;

        SF3D.UI.UIElementIndex label_name_outline;
        SF3D.UI.UIElementIndex label_name;
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
            cursor_mesh = new SF3D.SFModel3D();

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

            SF3D.SFSubModel3D sbm2 = new SF3D.SFSubModel3D();
            sbm2.CreateRaw(vertices, uvs, colors, normals, indices, null);
            sbm2.material.apply_shading = false;
            sbm2.material.apply_shadow = false;
            sbm2.material.casts_shadow = false;
            sbm2.material.distance_fade = false;
            sbm2.material.transparent_pass = false;

            cursor_mesh.CreateRaw(new SF3D.SFSubModel3D[] { sbm2 });
            SFResources.SFResourceManager.Models.AddManually(cursor_mesh, "_CURSOR_");

            // ui
            font_outline = new SF3D.UI.UIFont() { space_between_letters = 2 };
            font_outline.Load("font_fonttable_0512_12px_outline_l9");
            font_main = new SF3D.UI.UIFont() { space_between_letters = 2 };
            font_main.Load("font_fonttable_0512_12px_l9");

            SF3D.SFRender.SFRenderEngine.ui.AddStorage(font_outline.font_texture, 256);
            SF3D.SFRender.SFRenderEngine.ui.AddStorage(font_main.font_texture, 256);

            label_name_outline = SF3D.SFRender.SFRenderEngine.ui.AddElementText(font_outline, 256, new Vector2(10, 25));
            label_name = SF3D.SFRender.SFRenderEngine.ui.AddElementText(font_main, 256, new Vector2(10, 25));

            SetName("");
        }

        public void AssignToMap(SFMap _map)
        {
            map = _map;
            sel_obj = SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(SF3D.SFRender.SFRenderEngine.scene.root, "_SELECTION_", "_SELECTION_");
            sel_obj.Rotation = Quaternion.FromEulerAngles(0, (float)Math.PI / 2, 0);
            cur_obj = SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(SF3D.SFRender.SFRenderEngine.scene.root, "_CURSOR_", "_CURSOR_");
            cur_obj.Rotation = Quaternion.FromEulerAngles(0, (float)Math.PI / 2, 0);
        }

        public void SetSelectionPosition(SFCoord pos)
        {
            float z = map.heightmap.GetZ(pos) / 100.0f;
            sel_obj.Position = new OpenTK.Vector3((float)pos.x - offset.X, (float)z, (float)(map.height - pos.y - 1) + offset.Y);
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

            SF3D.SFRender.SFRenderEngine.scene.selected_node = null;

            SetName("");
        }

        public void SelectUnit(SFMapUnit unit)
        {
            CancelSelection();
            selection_type = SelectionType.UNIT;
            selected_entity = unit;
            SetSelectionScale(1.0f, 0.033f);

            SF3D.SFRender.SFRenderEngine.scene.selected_node = unit.node;

            SetName(SFCFF.SFCategoryManager.GetUnitName((ushort)selected_entity.game_id, true));
        }

        public void SelectBuilding(SFMapBuilding building)
        {
            CancelSelection();
            selection_type = SelectionType.BUILDING;
            selected_entity = building;

            float sel_scale = 1.0f;
            SFLua.lua_sql.SFLuaSQLBuildingData data = SFLua.SFLuaEnvironment.buildings[building.game_id];
            if (data != null)
            {
                sel_scale = Math.Max(1.0f, (float)(data.SelectionScaling));
            }

            SetSelectionScale(sel_scale, 0.033f);

            SF3D.SFRender.SFRenderEngine.scene.selected_node = building.node;

            SetName(SFCFF.SFCategoryManager.GetBuildingName((ushort)selected_entity.game_id));
        }

        public void SelectObject(SFMapObject obj)
        {
            CancelSelection();
            selection_type = SelectionType.OBJECT;
            selected_entity = obj;

            float sel_scale = 1.0f;
            SFLua.lua_sql.SFLuaSQLObjectData data = SFLua.SFLuaEnvironment.objects[obj.game_id];
            if (data != null)
            {
                sel_scale = Math.Max(1.0f, (float)data.SelectionScaling);
            }

            SetSelectionScale(sel_scale, 0.033f);

            SF3D.SFRender.SFRenderEngine.scene.selected_node = obj.node;

            SetName(SFCFF.SFCategoryManager.GetObjectName((ushort)selected_entity.game_id));
        }

        public void SelectInteractiveObject(SFMapInteractiveObject io)
        {
            CancelSelection();
            selection_type = SelectionType.INTERACTIVE_OBJECT;
            selected_entity = io;

            float sel_scale = 1.0f;
            SFLua.lua_sql.SFLuaSQLObjectData data = SFLua.SFLuaEnvironment.objects[io.game_id];
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
                    SetName(Utility.S_NONE);
                }
                else
                {
                    if (map.metadata.spawns[player].text_id == 0)
                    {
                        SetName(Utility.S_NONAME);
                    }
                    else
                    {
                        SFCFF.SFCategoryElement elem = SFCFF.SFCategoryManager.FindElementText(
                            map.metadata.spawns[player].text_id, Settings.LanguageID);
                        if (elem == null)
                        {
                            SetName(Utility.S_MISSING);
                        }
                        else
                        {
                            SetName(elem.variants[4].ToString());
                        }
                    }
                }
            }
            else
            {
                SetName(SFCFF.SFCategoryManager.GetObjectName((ushort)selected_entity.game_id));
            }

            SF3D.SFRender.SFRenderEngine.scene.selected_node = io.node;
        }

        public void SelectPortal(SFMapPortal p)
        {
            CancelSelection();
            selection_type = SelectionType.PORTAL;
            selected_entity = p;

            float sel_scale = 1.0f;
            SFLua.lua_sql.SFLuaSQLObjectData data = SFLua.SFLuaEnvironment.objects[778];
            if (data != null)
            {
                sel_scale = Math.Max(1.0f, (float)(data.SelectionScaling));
            }

            SetSelectionScale(sel_scale, 0.033f);

            string portal_name = Utility.S_MISSING;
            int portal_id = selected_entity.game_id;
            int portal_index = SFCFF.SFCategoryManager.gamedata[2053].GetElementIndex(portal_id);
            if (portal_index != -1)
            {
                SFCFF.SFCategoryElement portal_data = SFCFF.SFCategoryManager.gamedata[2053][portal_index];
                portal_name = SFCFF.SFCategoryManager.GetTextFromElement(portal_data, 5);
            }

            SF3D.SFRender.SFRenderEngine.scene.selected_node = p.node;

            SetName(portal_name);
        }

        public void SelectLake(SFMapLake lake)
        {
            CancelSelection();
            selection_type = SelectionType.LAKE;

            SF3D.SFRender.SFRenderEngine.scene.selected_node = lake.node;
        }

        // should be run once per render tick
        public void Update()
        {
            // selection and ui stuff
            SetSelectionVisibility((selection_type != SelectionType.NONE) && (selection_type != SelectionType.LAKE));

            SFMapHeightMap hmap = SF3D.SFRender.SFRenderEngine.scene.map.heightmap;
            SF3D.SceneSynchro.SceneNodeCamera camera = SF3D.SFRender.SFRenderEngine.scene.camera;
            Vector2 text_pos = new Vector2(0, 0);

            if (selected_entity != null)
            {
                if (selection_type == SelectionType.BUILDING)//same with interactive object and object, todo...
                {
                    Vector2 off = map.building_manager.building_collision[(ushort)selected_entity.game_id].origin;
                    float angle = (float)(selected_entity.angle * Math.PI / 180);
                    Vector2 r_off = new Vector2(off.X, off.Y);
                    r_off.X = (float)((Math.Cos(angle) * off.X) - (Math.Sin(angle) * off.Y));
                    r_off.Y = (float)((Math.Sin(angle) * off.X) + (Math.Cos(angle) * off.Y));

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

            text_pos.X *= SF3D.SFRender.SFRenderEngine.render_size.X;
            text_pos.Y *= SF3D.SFRender.SFRenderEngine.render_size.Y;
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
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(SF3D.SFRender.SFRenderEngine.scene.root.FindNode<SF3D.SceneSynchro.SceneNode>("_PREVIEW_"));
                preview_entity = null;
            }

        }

        public void ResetPreview()
        {
            ClearPreview();
            preview_entity = SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeEmpty(SF3D.SFRender.SFRenderEngine.scene.root, "_PREVIEW_");
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
            preview_entity.AddNode(SF3D.SFRender.SFRenderEngine.scene.AddSceneUnit(unit_id, "_UNIT_" + unit_id.ToString()));

            int unit_index = SFCFF.SFCategoryManager.gamedata[2024].GetElementIndex(unit_id);
            if (unit_index == -1)
            {
                return;
            }

            SFCFF.SFCategoryElement unit_data = SFCFF.SFCategoryManager.gamedata[2024][unit_index];
            unit_index = SFCFF.SFCategoryManager.gamedata[2005].GetElementIndex((ushort)unit_data[2]);
            float unit_size = 1f;
            if (unit_index != -1)
            {
                unit_data = SFCFF.SFCategoryManager.gamedata[2005][unit_index];
                unit_size = Math.Max((ushort)unit_data[18], (ushort)40) / 100.0f;
            }

            preview_entity.Scale = new OpenTK.Vector3(unit_size * 100 / 128);
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
            preview_entity.AddNode(SF3D.SFRender.SFRenderEngine.scene.AddSceneBuilding(building_id, "_BUILDING_" + building_id.ToString()));
            preview_entity.Scale = new OpenTK.Vector3(100 / 128f);
            map.building_manager.AddBuildingCollisionBoundary(building_id);

            preview_building_id = building_id;

            Vector2 off = map.building_manager.building_collision[building_id].origin;
            float angle = 0;
            Vector2 r_off = new Vector2(off.X, off.Y);
            r_off.X = (float)((Math.Cos(angle) * off.X) - (Math.Sin(angle) * off.Y));
            r_off.Y = (float)((Math.Sin(angle) * off.X) + (Math.Cos(angle) * off.Y));

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
            preview_entity.AddNode(SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(object_id, "_OBJECT_" + object_id.ToString(), true, true));
            preview_entity.Scale = new OpenTK.Vector3(100 / 128f);
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

            SF3D.SFRender.SFRenderEngine.ui.SetElementText(label_name_outline, font_outline, name);
            SF3D.SFRender.SFRenderEngine.ui.SetElementText(label_name, font_main, name);
            current_name = name;
            current_name_width = SF3D.SFRender.SFRenderEngine.ui.GetTextWidth(font_main, current_name);
        }

        public void SetNamePosition(Vector2 pos)
        {
            pos.X -= current_name_width / 2;
            SF3D.SFRender.SFRenderEngine.ui.MoveElement(label_name_outline, pos);
            SF3D.SFRender.SFRenderEngine.ui.MoveElement(label_name, pos);
        }

        public void GenerateSelectionMesh(float radius, float width)
        {
            // generate selection 3d model
            SF3D.SFModel3D new_selection = new SF3D.SFModel3D();

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

            SF3D.SFSubModel3D sbm1 = new SF3D.SFSubModel3D();
            sbm1.CreateRaw(vertices, uvs, colors, normals, indices, null);
            sbm1.material.apply_shading = false;
            sbm1.material.apply_shadow = false;
            sbm1.material.casts_shadow = false;
            sbm1.material.distance_fade = false;
            sbm1.material.transparent_pass = false;

            if (sel_obj != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(sel_obj);
                if (SFResources.SFResourceManager.Models.Get("_SELECTION_") != null)
                {
                    SFResources.SFResourceManager.Models.Dispose("_SELECTION_");
                }
            }
            new_selection.CreateRaw(new SF3D.SFSubModel3D[] { sbm1 });
            SFResources.SFResourceManager.Models.AddManually(new_selection, "_SELECTION_");
            selection_mesh = new_selection;
            if (sel_obj != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.root.AddNode(sel_obj);
                sel_obj.Mesh = new_selection;
            }
        }

        public void Dispose()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapSelectionHelper.Dispose() called");
            SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(SF3D.SFRender.SFRenderEngine.scene.root.FindNode<SF3D.SceneSynchro.SceneNodeSimple>("_SELECTION_"));
            SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(SF3D.SFRender.SFRenderEngine.scene.root.FindNode<SF3D.SceneSynchro.SceneNodeSimple>("_CURSOR_"));
            SFResources.SFResourceManager.Models.Dispose("_SELECTION_");
            SFResources.SFResourceManager.Models.Dispose("_CURSOR_");
            sel_obj = null;
            cur_obj = null;
            ClearPreview();

            SF3D.SFRender.SFRenderEngine.ui.RemoveStorage(font_outline.font_texture);
            SF3D.SFRender.SFRenderEngine.ui.RemoveStorage(font_main.font_texture);
            font_outline.Dispose();
            font_main.Dispose();
        }
    }
}
