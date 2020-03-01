using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapSelectionHelper
    {
        enum SelectionType { NONE, UNIT, BUILDING, OBJECT, INTERACTIVE_OBJECT, PORTAL }

        static SF3D.SFModel3D selection_mesh = new SF3D.SFModel3D();
        static SF3D.SFModel3D cursor_mesh = new SF3D.SFModel3D();
        SFMap map = null;
        SF3D.SceneSynchro.SceneNodeSimple sel_obj = null;
        SF3D.SceneSynchro.SceneNodeSimple cur_obj = null;
        Vector2 offset = new Vector2(0, 0);
        SFCoord cursor_position = new SFCoord(0, 0);

        SFMapUnit selected_unit = null;
        SFMapBuilding selected_building = null;
        SFMapObject selected_object = null;
        SFMapInteractiveObject selected_interactive_object = null;
        SFMapPortal selected_portal = null;
        SelectionType selection_type = SelectionType.NONE;

        SF3D.SceneSynchro.SceneNode preview_entity = null;
        ushort preview_entity_angle = 0;
        SelectionType preview_type = SelectionType.NONE;
        ushort preview_unit_id = 0;
        ushort preview_building_id = 0;
        ushort preview_object_id = 0;

        // ui stuff
        SF3D.UI.UIFont font_outline;
        SF3D.UI.UIFont font_main;

        SF3D.UI.UIElementIndex label_name_outline;
        SF3D.UI.UIElementIndex label_name;

        public SFMapSelectionHelper()
        {
            // generate selection 3d model
            Vector3[] vertices = new Vector3[8];
            Vector2[] uvs = new Vector2[8];
            Vector4[] colors = new Vector4[8];
            Vector3[] normals = new Vector3[8];

            vertices[0] = new Vector3(-0.5f, 0.1f, -0.5f);
            vertices[1] = new Vector3(-0.5f, 0.1f, 0.5f);
            vertices[2] = new Vector3(0.5f, 0.1f, -0.5f);
            vertices[3] = new Vector3(0.5f, 0.1f, 0.5f);
            vertices[4] = new Vector3(-0.4f, 0.1f, -0.4f);
            vertices[5] = new Vector3(-0.4f, 0.1f, 0.4f);
            vertices[6] = new Vector3(0.4f, 0.1f, -0.4f);
            vertices[7] = new Vector3(0.4f, 0.1f, 0.4f);
            for (int i = 0; i < 8; i++)
            {
                colors[i] = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
                normals[i] = new Vector3(0.0f, 1.0f, 0.0f);
            }

            uint[] indices = { 4, 0, 1,   1, 5, 4,   5, 1, 3,   3, 7, 5,
                               7, 3, 2,   2, 6, 7,   6, 2, 0,   0, 4, 2 };

            SF3D.SFSubModel3D sbm1 = new SF3D.SFSubModel3D();
            sbm1.CreateRaw(vertices, uvs, colors, normals, indices, null);

            selection_mesh.CreateRaw(new SF3D.SFSubModel3D[] { sbm1 });
            SFResources.SFResourceManager.Models.AddManually(selection_mesh, "_SELECTION_");

            // generate mouse cursor selected position gizmo
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
                colors[i] = new Vector4(1.0f, 1.0f, 0.4f, 1.0f);
                normals[i] = new Vector3(0.0f, 1.0f, 0.0f);
            }

            indices = new uint[] { 0, 1, 2,   1, 2, 3,   4, 5, 6,   5, 6, 7,
                                   0, 1, 4,   1, 4, 5,   1, 3, 5,   3, 5, 7,
                                   3, 2, 7,   2, 7, 6,   2, 0, 6,   0, 6, 4};

            SF3D.SFSubModel3D sbm2 = new SF3D.SFSubModel3D();
            sbm2.CreateRaw(vertices, uvs, colors, normals, indices, null);

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

        public void SetSelectionScale(float s)
        {
            sel_obj.Scale = new Vector3(s, s, s);
        }

        public void SetSelectionOffset(Vector2 off)
        {
            offset = off;
        }

        public void CancelSelection()
        {
            selected_unit = null;
            selected_building = null;
            selected_object = null;
            selected_interactive_object = null;
            selected_portal = null;
            offset = new Vector2(0, 0);
            selection_type = SelectionType.NONE;

            SetName("");
        }

        public void SelectUnit(SFMapUnit unit)
        {
            CancelSelection();
            selection_type = SelectionType.UNIT;
            selected_unit = unit;
            SetSelectionScale(1.0f);

            SetName(SFCFF.SFCategoryManager.GetUnitName((ushort)selected_unit.game_id, true));
        }

        public void SelectBuilding(SFMapBuilding building)
        {
            CancelSelection();
            selection_type = SelectionType.BUILDING;
            selected_building = building;

            float sel_scale = 0.0f;
            SFLua.lua_sql.SFLuaSQLBuildingData data = SFLua.SFLuaEnvironment.buildings[building.game_id];
            if (data != null)
                sel_scale = (float)(data.SelectionScaling / 2);
            SetSelectionScale(sel_scale);

            SetName(SFCFF.SFCategoryManager.GetBuildingName((ushort)selected_building.game_id));
        }

        public void SelectObject(SFMapObject obj)
        {
            CancelSelection();
            selection_type = SelectionType.OBJECT;
            selected_object = obj;

            float sel_scale = 0.0f;
            SFLua.lua_sql.SFLuaSQLObjectData data = SFLua.SFLuaEnvironment.objects[obj.game_id];
            if (data != null)
                sel_scale = (float)(data.SelectionScaling / 2);
            SetSelectionScale(sel_scale);

            SetName(SFCFF.SFCategoryManager.GetObjectName((ushort)selected_object.game_id));
        }

        public void SelectInteractiveObject(SFMapInteractiveObject io)
        {
            CancelSelection();
            selection_type = SelectionType.INTERACTIVE_OBJECT;
            selected_interactive_object = io;

            float sel_scale = 0.0f;
            SFLua.lua_sql.SFLuaSQLObjectData data = SFLua.SFLuaEnvironment.objects[io.game_id];
            if (data != null)
                sel_scale = (float)(data.SelectionScaling / 2);
            SetSelectionScale(sel_scale);

            if(selected_interactive_object.game_id == 769)   // bindstone
            {
                int player = map.metadata.FindPlayerBySpawnPos(io.grid_position);
                if (player == -1)
                    SetName(Utility.S_NONE);
                else
                {
                    if (map.metadata.spawns[player].text_id == 0)
                        SetName(Utility.S_NONAME);
                    else
                    {
                        SFCFF.SFCategoryElement elem = SFCFF.SFCategoryManager.FindElementText(
                            map.metadata.spawns[player].text_id, Settings.LanguageID);
                        if (elem == null)
                            SetName(Utility.S_MISSING);
                        else
                            SetName(Utility.CleanString(elem.variants[4]));
                    }
                }
            }
            else
                SetName(SFCFF.SFCategoryManager.GetObjectName((ushort)selected_interactive_object.game_id));
        }

        public void SelectPortal(SFMapPortal p)
        {
            CancelSelection();
            selection_type = SelectionType.PORTAL;
            selected_portal = p;

            float sel_scale = 0.0f;
            SFLua.lua_sql.SFLuaSQLObjectData data = SFLua.SFLuaEnvironment.objects[778];
            if (data != null)
                sel_scale = (float)(data.SelectionScaling / 2);
            SetSelectionScale(sel_scale);

            string portal_name = Utility.S_MISSING;
            int portal_id = selected_portal.game_id;
            int portal_index = SFCFF.SFCategoryManager.gamedata[38].GetElementIndex(portal_id);
            if (portal_index != -1)
            {
                SFCFF.SFCategoryElement portal_data = SFCFF.SFCategoryManager.gamedata[38][portal_index];
                portal_name = SFCFF.SFCategoryManager.GetTextFromElement(portal_data, 5);
            }

            SetName(portal_name);
        }

        // should be run once per render tick
        public void Update()
        {
            // selection and ui stuff
            SetSelectionVisibility(selection_type != SelectionType.NONE);

            SFMapHeightMap hmap = SF3D.SFRender.SFRenderEngine.scene.heightmap;
            SF3D.SceneSynchro.SceneNodeCamera camera = SF3D.SFRender.SFRenderEngine.scene.camera;
            Vector2 text_pos = new Vector2(0, 0);

            if (selection_type == SelectionType.UNIT)
            {
                if (selected_unit != null)
                {
                    SetSelectionPosition(selected_unit.grid_position);

                    text_pos = camera.WorldToScreen(new Vector3(
                        selected_unit.grid_position.x,
                        hmap.GetRealZ(new Vector2(selected_unit.grid_position.x, hmap.height-selected_unit.grid_position.y-1)),
                        hmap.height-selected_unit.grid_position.y-1));
                }
            }
            else if (selection_type == SelectionType.BUILDING)
            {
                if (selected_building != null)
                {
                    Vector2 off = map.building_manager.building_collision[(ushort)selected_building.game_id].collision_mesh.origin;
                    float angle = (float)(selected_building.angle * Math.PI / 180);
                    Vector2 r_off = new Vector2(off.X, off.Y);
                    r_off.X = (float)((Math.Cos(angle) * off.X) - (Math.Sin(angle) * off.Y));
                    r_off.Y = (float)((Math.Sin(angle) * off.X) + (Math.Cos(angle) * off.Y));

                    SetSelectionOffset(r_off);
                    SetSelectionPosition(selected_building.grid_position);

                    text_pos = camera.WorldToScreen(new Vector3(
                        selected_building.grid_position.x,
                        hmap.GetRealZ(new Vector2(selected_building.grid_position.x, hmap.height - selected_building.grid_position.y - 1)),
                        hmap.height - selected_building.grid_position.y - 1));
                }
            }
            else if (selection_type == SelectionType.OBJECT)
            {
                if (selected_object != null)
                {
                    /*Vector2 off = map.building_manager.building_collision[(ushort)selected_building.game_id].collision_mesh.origin;
                    float angle = (float)(selected_building.angle * Math.PI / 180);
                    Vector2 r_off = new Vector2(off.X, off.Y);
                    r_off.X = (float)((Math.Cos(angle) * off.X) - (Math.Sin(angle) * off.Y));
                    r_off.Y = (float)((Math.Sin(angle) * off.X) + (Math.Cos(angle) * off.Y));

                    SetSelectionOffset(r_off);*/
                    SetSelectionPosition(selected_object.grid_position);

                    text_pos = camera.WorldToScreen(new Vector3(
                        selected_object.grid_position.x,
                        hmap.GetRealZ(new Vector2(selected_object.grid_position.x, hmap.height - selected_object.grid_position.y - 1)),
                        hmap.height - selected_object.grid_position.y - 1));
                }
            }
            else if (selection_type == SelectionType.INTERACTIVE_OBJECT)
            {
                if (selected_interactive_object != null)
                {
                    /*Vector2 off = map.building_manager.building_collision[(ushort)selected_building.game_id].collision_mesh.origin;
                    float angle = (float)(selected_building.angle * Math.PI / 180);
                    Vector2 r_off = new Vector2(off.X, off.Y);
                    r_off.X = (float)((Math.Cos(angle) * off.X) - (Math.Sin(angle) * off.Y));
                    r_off.Y = (float)((Math.Sin(angle) * off.X) + (Math.Cos(angle) * off.Y));

                    SetSelectionOffset(r_off);*/
                    SetSelectionPosition(selected_interactive_object.grid_position);

                    text_pos = camera.WorldToScreen(new Vector3(
                        selected_interactive_object.grid_position.x,
                        hmap.GetRealZ(new Vector2(selected_interactive_object.grid_position.x, hmap.height - selected_interactive_object.grid_position.y - 1)),
                        hmap.height - selected_interactive_object.grid_position.y - 1));
                }
            }
            else if (selection_type == SelectionType.PORTAL)
            {
                if (selected_portal != null)
                    SetSelectionPosition(selected_portal.grid_position);

                text_pos = camera.WorldToScreen(new Vector3(
                    selected_portal.grid_position.x,
                    hmap.GetRealZ(new Vector2(selected_portal.grid_position.x, hmap.height - selected_portal.grid_position.y - 1)),
                    hmap.height - selected_portal.grid_position.y - 1));
            }
            // todo: add more selection types

            text_pos.X = (text_pos.X - 0.1f) * 1.2f;
            text_pos.Y = (text_pos.Y - 0.1f) * 1.2f;
            text_pos.X *= SF3D.SFRender.SFRenderEngine.render_size.X;
            text_pos.Y *= SF3D.SFRender.SFRenderEngine.render_size.Y;
            text_pos.X = (float)Math.Floor(text_pos.X);
            text_pos.Y = (float)Math.Floor(text_pos.Y);

            SetNamePosition(text_pos);
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
                    preview_entity.Position = new Vector3(pos.x, z + 0.2f, pos.y);

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
            preview_type = SelectionType.NONE;
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
            preview_type = SelectionType.UNIT;

            // get unit
            preview_entity.AddNode(SF3D.SFRender.SFRenderEngine.scene.AddSceneUnit(unit_id, "_UNIT_" + unit_id.ToString()));

            int unit_index = map.gamedata[17].GetElementIndex(unit_id);
            if (unit_index == -1)
                return;

            SFCFF.SFCategoryElement unit_data = map.gamedata[17][unit_index];
            unit_index = map.gamedata[3].GetElementIndex((ushort)unit_data[2]);
            float unit_size = 1f;
            if (unit_index != -1)
            {
                unit_data = map.gamedata[3][unit_index];
                unit_size = Math.Max((ushort)unit_data[19], (ushort)40) / 100.0f;
            }

            preview_entity.Scale = new OpenTK.Vector3(unit_size * 100 / 128);

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
            preview_type = SelectionType.BUILDING;

            // get building
            preview_entity.AddNode(SF3D.SFRender.SFRenderEngine.scene.AddSceneBuilding(building_id, "_BUILDING_" + building_id.ToString()));
            preview_entity.Scale = new OpenTK.Vector3(100 / 128f);

            preview_building_id = building_id;
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
            preview_type = SelectionType.OBJECT;

            // get building
            preview_entity.AddNode(SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(object_id, "_OBJECT_" + object_id.ToString(), true));
            preview_entity.Scale = new OpenTK.Vector3(100 / 128f);

            preview_object_id = object_id;
        }

        public void SetPreviewAngle(ushort angle)
        {
            preview_entity_angle = angle;
            if (preview_entity != null)
                preview_entity.SetAnglePlane(angle);
        }

        public void SetName(string name)
        {
            SF3D.SFRender.SFRenderEngine.ui.SetElementText(label_name_outline, font_outline, name);
            SF3D.SFRender.SFRenderEngine.ui.SetElementText(label_name, font_main, name);
        }

        public void SetNamePosition(Vector2 pos)
        {
            SF3D.SFRender.SFRenderEngine.ui.MoveElement(label_name_outline, pos);
            SF3D.SFRender.SFRenderEngine.ui.MoveElement(label_name, pos);
        }

        public void Dispose()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapSelectionHelper.Dispose() called");
            SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(SF3D.SFRender.SFRenderEngine.scene.root.FindNode<SF3D.SceneSynchro.SceneNodeSimple>("_SELECTION_"));
            SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(SF3D.SFRender.SFRenderEngine.scene.root.FindNode<SF3D.SceneSynchro.SceneNodeSimple>("_CURSOR_"));
            sel_obj = null;
            cur_obj = null;
            ClearPreview();

            font_outline.Dispose();
            font_main.Dispose();
        }
    }
}
