using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SFEngine.SFMap;
using SFEngine.SFCFF;
using SFEngine.SFLua;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapObjectInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        bool move_camera_on_select = false;
        bool object_selected_from_list = true;

        bool trackbar_clicked = false;
        int trackbar_initial_angle = -1;

        public MapObjectInspector()
        {
            InitializeComponent();
        }

        private void MapObjectInspector_Load(object sender, EventArgs e)
        {
            ReloadList();
            ResizeList();
        }

        private void ReloadList()
        {
            ListObjects.Items.Clear();
            for (int i = 0; i < map.object_manager.objects.Count; i++)
                LoadNextObject(i);
        }

        private void ShowList()
        {
            if (ButtonResizeList.Text == "-")
                return;

            ResizeList();

            ButtonResizeList.Text = "-";
        }

        private void ResizeList()
        {
            PanelObjectList.Height = this.Height - PanelObjectList.Location.Y - 3;
            ListObjects.Height = PanelObjectList.Height - 125;
            SearchObjectText.Location = new Point(SearchObjectText.Location.X, ListObjects.Location.Y + ListObjects.Height + 8);
            SearchObjectNext.Location = new Point(SearchObjectNext.Location.X, SearchObjectText.Location.Y + 28);
            SearchObjectPrevious.Location = new Point(SearchObjectPrevious.Location.X, SearchObjectText.Location.Y + 28);
        }

        private void HideList()
        {
            if (ButtonResizeList.Text == "+")
                return;

            PanelObjectList.Height = 30;

            ButtonResizeList.Text = "+";
        }

        public void RemoveObject(int index)
        {
            if (ListObjects.SelectedIndex == index)
                PanelProperties.Enabled = false;
            ListObjects.Items.RemoveAt(index);
        }

        public void LoadNextObject(int index)
        {
            string object_name = SFCategoryManager.GetObjectName((ushort)map.object_manager.objects[index].game_id);
            object_name += " " + map.object_manager.objects[index].grid_position.ToString();
            ListObjects.Items.Insert(index, object_name);
        }

        private void MapObjectInspector_Resize(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "+")
                return;

            ResizeList();
        }

        public override void OnSelect(object o)
        {
            move_camera_on_select = false;
            object_selected_from_list = false;

            if (o == null)
            {
                map.selection_helper.CancelSelection();
                PanelProperties.Enabled = false;
            }
            else
                ListObjects.SelectedIndex = map.object_manager.objects.IndexOf((SFMapObject)o);
        }

        private void ButtonResizeList_Click(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "-")
                HideList();
            else
                ShowList();
        }

        private void ListObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListObjects.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            PanelProperties.Enabled = true;
            SFMapObject obj = map.object_manager.objects[ListObjects.SelectedIndex];
            ObjectID.Text = obj.game_id.ToString();
            NPCID.Text = obj.npc_id.ToString();
            PosX.Text = obj.grid_position.x.ToString();
            PosY.Text = obj.grid_position.y.ToString();
            AngleTrackbar.Value = obj.angle;
            Unknown1.Text = obj.unknown1.ToString();
            
            map.selection_helper.SelectObject(obj);
            if ((move_camera_on_select)||(object_selected_from_list))
                MainForm.mapedittool.SetCameraViewPoint(obj.grid_position);
            move_camera_on_select = false;
            object_selected_from_list = true;
        }

        private void ObjectID_Validated(object sender, EventArgs e)
        {
            if (ListObjects.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            ushort new_object_id = SFEngine.Utility.TryParseUInt16(ObjectID.Text);

            SFMapObject obj = map.object_manager.objects[ListObjects.SelectedIndex];
            if (obj.game_id == new_object_id)
                return;

            // check if its not a special object ID
            if (map.object_manager.ObjectIDIsReserved(new_object_id))
            {
                ObjectID.Text = obj.game_id.ToString();
                return;
            }

            // check if new object exists
            if (SFCategoryManager.gamedata[2050].GetElementIndex(new_object_id) == SFEngine.Utility.NO_INDEX)
                return;

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.OBJECT,
                index = ListObjects.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ID,
                PreChangeProperty = obj.game_id,
                PostChangeProperty = new_object_id
            });

            map.ReplaceObject(ListObjects.SelectedIndex, new_object_id);

            LabelObjectName.Text = SFCategoryManager.GetObjectName(new_object_id);
            ListObjects.Items[ListObjects.SelectedIndex] = LabelObjectName.Text + " "
                + map.object_manager.objects[ListObjects.SelectedIndex].grid_position.ToString();
            MainForm.mapedittool.update_render = true;
        }

        private void NPCID_Validated(object sender, EventArgs e)
        {
            if (ListObjects.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            SFMapObject obj = map.object_manager.objects[ListObjects.SelectedIndex];

            int npc_id = SFEngine.Utility.TryParseInt32(NPCID.Text);

            // find if any npc exists
            SFMapEntity entity = map.FindNPCEntity(npc_id);
            if ((entity != null) && (!(entity is SFMapObject) || ((SFMapObject)entity) != obj))
            {
                MessageBox.Show("Duplicate NPC ID " + npc_id + " found. Unable to change selected unit ID.");
                NPCID.Text = obj.npc_id.ToString();
            }

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.OBJECT,
                index = ListObjects.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.NPCID,
                PreChangeProperty = obj.npc_id,
                PostChangeProperty = npc_id
            });

            obj.npc_id = npc_id;
        }

        private void NPCScript_Click(object sender, EventArgs e)
        {
            if (ListObjects.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            SFMapObject obj = map.object_manager.objects[ListObjects.SelectedIndex];
            if (obj.npc_id == 0)
                return;

            string fname = "script\\p" + map.PlatformID.ToString() + "\\n" + obj.npc_id.ToString() + ".lua";
            if (SFLuaEnvironment.OpenNPCScript((int)map.PlatformID, obj.npc_id) != 0)
                MessageBox.Show("Could not open " + fname);
        }

        private void Angle_Validated(object sender, EventArgs e)
        {
            if (ListObjects.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            SFMapObject obj = map.object_manager.objects[ListObjects.SelectedIndex];

            int v = SFEngine.Utility.TryParseUInt16(Angle.Text, (ushort)obj.angle);
            v = (v >= 0 ? (v <= 359 ? v : 359) : 0);

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.OBJECT,
                index = ListObjects.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = obj.angle,
                PostChangeProperty = v
            });

            AngleTrackbar.Value = v;
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (ListObjects.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            SFMapObject obj = map.object_manager.objects[ListObjects.SelectedIndex];
            Angle.Text = AngleTrackbar.Value.ToString();
            obj.angle = AngleTrackbar.Value;
            map.RotateObject(ListObjects.SelectedIndex, obj.angle);

            MainForm.mapedittool.update_render = true;
        }

        // this is to make sure the undo/redo queue only receives the latest angle changed as an action to perform
        private void AngleTrackbar_MouseDown(object sender, MouseEventArgs e)
        {
            if (ListObjects.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            trackbar_clicked = true;

            if (trackbar_initial_angle == -1)
            {
                SFMapObject obj = map.object_manager.objects[ListObjects.SelectedIndex];
                trackbar_initial_angle = obj.angle;
            }
        }

        private void AngleTrackbar_MouseUp(object sender, MouseEventArgs e)
        {
            if (!trackbar_clicked)
                return;

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.OBJECT,
                index = ListObjects.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = trackbar_initial_angle,
                PostChangeProperty = AngleTrackbar.Value
            });

            trackbar_clicked = false;
            trackbar_initial_angle = -1;
        }


        private void Unknown1_Validated(object sender, EventArgs e)
        {
            if (ListObjects.SelectedIndex == SFEngine.Utility.NO_INDEX)
                return;

            SFMapObject obj = map.object_manager.objects[ListObjects.SelectedIndex];

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.OBJECT,
                index = ListObjects.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.OBJECTUNKNOWN,
                PreChangeProperty = obj.unknown1,
                PostChangeProperty = SFEngine.Utility.TryParseUInt16(Unknown1.Text)
            });

            obj.unknown1 = SFEngine.Utility.TryParseUInt16(Unknown1.Text);
        }

        private void SearchObjectNext_Click(object sender, EventArgs e)
        {
            string search_phrase = SearchObjectText.Text.Trim().ToLower();
            if (search_phrase == "")
                return;

            int search_start = ListObjects.SelectedIndex;

            move_camera_on_select = true;

            for (int i = search_start + 1; i < map.object_manager.objects.Count; i++)
                if (ListObjects.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListObjects.SelectedIndex = i;
                    return;
                }

            for (int i = 0; i <= search_start; i++)
                if (ListObjects.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListObjects.SelectedIndex = i;
                    return;
                }
        }

        private void SearchObjectPrevious_Click(object sender, EventArgs e)
        {
            string search_phrase = SearchObjectText.Text.Trim().ToLower();
            if (search_phrase == "")
                return;

            int search_start = ListObjects.SelectedIndex;

            move_camera_on_select = true;

            for (int i = search_start - 1; i >= 0; i--)
                if (ListObjects.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListObjects.SelectedIndex = i;
                    return;
                }

            if (search_start == SFEngine.Utility.NO_INDEX)
                search_start = 0;

            for (int i = map.object_manager.objects.Count - 1; i >= search_start; i--)
                if (ListObjects.Items[i].ToString().ToLower().Contains(search_phrase))
                {
                    ListObjects.SelectedIndex = i;
                    return;
                }
        }

        private void ObjectID_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = SFEngine.Utility.TryParseUInt16(ObjectID.Text);
                int real_elem_id = SFCategoryManager.gamedata[2050].GetElementIndex(elem_id);
                if (real_elem_id != SFEngine.Utility.NO_INDEX)
                    MainForm.data.Tracer_StepForward(33, real_elem_id);
            }
        }
    }
}
