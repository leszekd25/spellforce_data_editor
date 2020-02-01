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
        }

        public void SelectUnit(SFMapUnit unit)
        {
            CancelSelection();
            selection_type = SelectionType.UNIT;
            selected_unit = unit;
            SetSelectionScale(1.0f);
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
        }

        // should be run once per render tick
        public void UpdateSelection()
        {
            SetSelectionVisibility(selection_type != SelectionType.NONE);
            if (selection_type == SelectionType.UNIT)
            {
                if (selected_unit != null)
                    SetSelectionPosition(selected_unit.grid_position);
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
                }
            }
            else if (selection_type == SelectionType.PORTAL)
            {
                if (selected_portal != null)
                    SetSelectionPosition(selected_portal.grid_position);
            }
            // todo: add more selection types
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

        public void Dispose()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapSelectionHelper.Dispose() called");
            SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(SF3D.SFRender.SFRenderEngine.scene.root.FindNode<SF3D.SceneSynchro.SceneNodeSimple>("_SELECTION_"));
            SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(SF3D.SFRender.SFRenderEngine.scene.root.FindNode<SF3D.SceneSynchro.SceneNodeSimple>("_CURSOR_"));
            sel_obj = null;
            cur_obj = null;
            ClearPreview();
        }
    }
}
