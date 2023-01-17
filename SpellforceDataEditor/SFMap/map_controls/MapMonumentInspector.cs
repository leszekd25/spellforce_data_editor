using SFEngine.SFCFF;
using SFEngine.SFMap;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapMonumentInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        bool move_camera_on_select = false;
        bool monument_selected_from_list = true;

        bool trackbar_clicked = false;
        int trackbar_initial_angle = -1;

        public MapMonumentInspector()
        {
            InitializeComponent();
        }

        private void MapMonumentInspector_Load(object sender, EventArgs e)
        {
            ReloadList();
            ResizeList();
        }

        private void ReloadList()
        {
            ListMonuments.Items.Clear();
            for (int i = 0; i < map.int_object_manager.monuments_index.Count; i++)
            {
                LoadNextMonument(i);
            }
        }

        private int GetMonumentIndex(SFMapInteractiveObject o)
        {
            int i = map.int_object_manager.int_objects.IndexOf(o);
            if (i != SFEngine.Utility.NO_INDEX)
            {
                return map.int_object_manager.monuments_index.IndexOf(i);
            }

            return SFEngine.Utility.NO_INDEX;
        }

        private string GetMonumentString(SFMapInteractiveObject io)
        {
            string ret = SFCategoryManager.GetObjectName((ushort)io.game_id) + " ";
            ret += io.grid_position.ToString();
            return ret;
        }

        private void ShowList()
        {
            if (ButtonResizeList.Text == "-")
            {
                return;
            }

            ResizeList();

            ButtonResizeList.Text = "-";
        }

        private void ResizeList()
        {
            PanelMonumentList.Height = Height - PanelMonumentList.Location.Y - 3;
            ListMonuments.Height = PanelMonumentList.Height - 75;
        }

        public void RemoveMonument(int index)
        {
            if (ListMonuments.SelectedIndex == index)
            {
                PanelProperties.Enabled = false;
            }

            ListMonuments.Items.RemoveAt(index);
        }

        public void LoadNextMonument(int index)
        {
            SFMapInteractiveObject io = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[index]];
            ListMonuments.Items.Insert(index, GetMonumentString(io));
        }

        private void HideList()
        {
            if (ButtonResizeList.Text == "+")
            {
                return;
            }

            PanelMonumentList.Height = 30;

            ButtonResizeList.Text = "+";
        }

        public override void OnSelect(object o)
        {
            move_camera_on_select = false;
            monument_selected_from_list = false;

            if (o == null)
            {
                map.selection_helper.CancelSelection();
                PanelProperties.Enabled = false;
            }
            else
            {
                ListMonuments.SelectedIndex = GetMonumentIndex((SFMapInteractiveObject)o);
            }
        }

        private void ListMonuments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListMonuments.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            PanelProperties.Enabled = true;
            SFMapInteractiveObject monument = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[ListMonuments.SelectedIndex]];
            PosX.Text = monument.grid_position.x.ToString();
            PosY.Text = monument.grid_position.y.ToString();
            AngleTrackbar.Value = monument.angle;
            // angle, angletrackbar

            map.selection_helper.SelectInteractiveObject(monument);
            if ((move_camera_on_select) || (monument_selected_from_list))
            {
                MainForm.mapedittool.SetCameraViewPoint(monument.grid_position);
            }

            move_camera_on_select = false;
            monument_selected_from_list = true;
        }

        private void ButtonResizeList_Click(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "-")
            {
                HideList();
            }
            else
            {
                ShowList();
            }
        }

        private void Angle_Validated(object sender, EventArgs e)
        {
            if (ListMonuments.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapInteractiveObject monument = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[ListMonuments.SelectedIndex]];

            int v = SFEngine.Utility.TryParseUInt16(Angle.Text, (ushort)monument.angle);
            v = (v >= 0 ? (v <= 359 ? v : 359) : 0);

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.MONUMENT,
                index = ListMonuments.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = monument.angle,
                PostChangeProperty = v
            });

            AngleTrackbar.Value = v;
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (ListMonuments.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFMapInteractiveObject monument = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[ListMonuments.SelectedIndex]];
            Angle.Text = AngleTrackbar.Value.ToString();
            map.RotateInteractiveObject(map.int_object_manager.monuments_index[ListMonuments.SelectedIndex], AngleTrackbar.Value);

            MainForm.mapedittool.update_render = true;
        }

        // this is to make sure the undo/redo queue only receives the latest angle changed as an action to perform
        private void AngleTrackbar_MouseDown(object sender, MouseEventArgs e)
        {
            if (ListMonuments.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            trackbar_clicked = true;

            if (trackbar_initial_angle == -1)
            {
                SFMapInteractiveObject monument = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[ListMonuments.SelectedIndex]];
                trackbar_initial_angle = monument.angle;
            }
        }

        private void AngleTrackbar_MouseUp(object sender, MouseEventArgs e)
        {
            if (!trackbar_clicked)
            {
                return;
            }

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.MONUMENT,
                index = ListMonuments.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ANGLE,
                PreChangeProperty = trackbar_initial_angle,
                PostChangeProperty = AngleTrackbar.Value
            });

            trackbar_clicked = false;
        }
    }
}
