using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspectorObjectControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        bool drag_enabled = false;
        int selected_object = -1;

        public MapInspectorObjectControl()
        {
            InitializeComponent();
        }

        private void MapInspectorObjectControl_VisibleChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (Visible)
            {
                ReloadObjectList();
            }

            MainForm.mapedittool.update_render = true;
        }

        private void ReloadObjectList()
        {
            ListObjects.Items.Clear();

            foreach (SFMapObject o in map.object_manager.objects)
                ListObjects.Items.Add(GetObjectString(o));
        }

        private string GetObjectString(SFMapObject o)
        {
            string ret = SFCFF.SFCategoryManager.get_object_name((ushort)o.game_id);
            ret += " " + o.grid_position.ToString();
            return ret;
        }

        private void ListObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectObject(ListObjects.SelectedIndex, true);
        }

        public void SelectObject(int object_map_index, bool move_vision)
        {
            if (object_map_index < 0)
                object_map_index = -1;
            if (object_map_index >= map.object_manager.objects.Count)
                object_map_index = -1;

            selected_object = object_map_index;

            if (object_map_index == -1)
            {
                SelectedObjectID.Text = "";
                SelectedObjectName.Text = "";
                SelectedObjectNPCID.Text = "";
                SelectedObjectX.Text = "";
                SelectedObjectY.Text = "";
                SelectedObjectAngle.Text = "";
                SelectedObjectUnk1.Text = "";
                map.selection_helper.CancelSelection();
            }
            else if (map.object_manager.objects[object_map_index].game_id != 2541)   // spawn point
            {

                SFMapObject obj = map.object_manager.objects[object_map_index];
                SelectedObjectID.Text = obj.game_id.ToString();
                SelectedObjectName.Text = SFCFF.SFCategoryManager.get_object_name((ushort)obj.game_id);
                SelectedObjectNPCID.Text = obj.npc_id.ToString();
                SelectedObjectX.Text = obj.grid_position.x.ToString();
                SelectedObjectY.Text = obj.grid_position.y.ToString();
                SelectedObjectAngle.Text = obj.angle.ToString();
                SelectedObjectUnk1.Text = obj.unknown1.ToString();
                map.selection_helper.SelectObject(obj);
                if (move_vision)
                    MainForm.mapedittool.SetCameraViewPoint(obj.grid_position);
            }
            else ListObjects.SelectedIndex = -1;
        }

        private void SelectedObjectID_Validated(object sender, EventArgs e)
        {
            if (selected_object == -1)
                return;

            ushort new_object_id = Utility.TryParseUInt16(SelectedObjectID.Text);

            SFMapObject obj = map.object_manager.objects[selected_object];
            if (obj.game_id == new_object_id)
                return;

            // check if new building exists
            if (map.gamedata.categories[33].get_element_index(new_object_id) == -1)
                return;

            map.ReplaceObject(selected_object, new_object_id);

            SelectedObjectName.Text = SFCFF.SFCategoryManager.get_object_name(new_object_id);
            ListObjects.Items[selected_object] = GetObjectString(obj);
            MainForm.mapedittool.update_render = true;
        }

        private void SelectedObjectID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MainForm.data == null)
                    return;
                if (!SFCFF.SFCategoryManager.ready)
                    return;

                ushort object_id = Utility.TryParseUInt16(SelectedObjectID.Text);

                MainForm.data.Tracer_StepForward(33, object_id, false);
            }
        }

        private void SelectedObjectAngleTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (selected_object == -1)
                return;

            SFMapObject obj = map.object_manager.objects[selected_object];
            SelectedObjectAngle.Text = SelectedObjectAngleTrackBar.Value.ToString();
            obj.angle = SelectedObjectAngleTrackBar.Value;
            map.RotateBuilding(selected_object, obj.angle);

            MainForm.mapedittool.update_render = true;
        }

        private void SelectedObjectAngle_Validated(object sender, EventArgs e)
        {
            SelectedObjectAngleTrackBar.Value = (int)(Math.Max((ushort)0, Math.Min((ushort)359, Utility.TryParseUInt16(SelectedObjectAngle.Text))));
        }

        private void SelectedObjectUnk1_Validated(object sender, EventArgs e)
        {
            if (selected_object == -1)
                return;

            SFMapObject obj = map.object_manager.objects[selected_object];
            obj.unknown1 = Utility.TryParseUInt16(SelectedObjectUnk1.Text);
        }
    }
}
