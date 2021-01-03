using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapCoopCampInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        bool move_camera_on_select = false;
        bool spawn_selected_from_list = true;

        public MapCoopCampInspector()
        {
            InitializeComponent();
        }

        private void MapCoopCampInspector_Load(object sender, EventArgs e)
        {
            ReloadList();
            ResizeList();
        }

        private void ReloadList()
        {
            ListCoopCamps.Items.Clear();
            for (int i = 0; i < map.metadata.coop_spawns.Count; i++)
                LoadNextCoopCamp(i);
        }

        private string GetCoopSpawnString(SFMapCoopAISpawn spawn)
        {
            string ret = "";
            if (SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types != null)
                if (SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types.ContainsKey(spawn.spawn_id))
                    ret += SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types[spawn.spawn_id].name + " ";
            ret += spawn.spawn_obj.grid_position.ToString();
            return ret;
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
            PanelCoopCampList.Height = this.Height - PanelCoopCampList.Location.Y - 3;
            ListCoopCamps.Height = PanelCoopCampList.Height - 75;
        }

        public void RemoveCoopCamp(int index)
        {
            if (ListCoopCamps.SelectedIndex == index)
                PanelProperties.Enabled = false;
            ListCoopCamps.Items.RemoveAt(index);
        }

        public void LoadNextCoopCamp(int index)
        {
            ListCoopCamps.Items.Insert(index, GetCoopSpawnString(map.metadata.coop_spawns[index]));
        }

        private void HideList()
        {
            if (ButtonResizeList.Text == "+")
                return;

            PanelCoopCampList.Height = 30;

            ButtonResizeList.Text = "+";
        }

        public override void OnSelect(object o)
        {
            move_camera_on_select = false;
            spawn_selected_from_list = false;

            if (o == null)
            {
                map.selection_helper.CancelSelection();
                PanelProperties.Enabled = false;
            }
            else
                ListCoopCamps.SelectedIndex = map.metadata.coop_spawns.IndexOf((SFMapCoopAISpawn)o);
        }

        private void ListCoopCamps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListCoopCamps.SelectedIndex == Utility.NO_INDEX)
                return;

            PanelProperties.Enabled = true;
            SFMapCoopAISpawn spawn = map.metadata.coop_spawns[ListCoopCamps.SelectedIndex];
            CampID.Text = spawn.spawn_id.ToString();
            PosX.Text = spawn.spawn_obj.grid_position.x.ToString();
            PosY.Text = spawn.spawn_obj.grid_position.y.ToString();
            Unknown1.Text = spawn.spawn_certain.ToString();

            if ((move_camera_on_select) || (spawn_selected_from_list))
                MainForm.mapedittool.SetCameraViewPoint(spawn.spawn_obj.grid_position);
            move_camera_on_select = false;
            spawn_selected_from_list = true;
        }

        private void CampID_Validated(object sender, EventArgs e)
        {
            if (ListCoopCamps.SelectedIndex == Utility.NO_INDEX)
                return;

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.COOPCAMP,
                index = ListCoopCamps.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ID,
                PreChangeProperty = map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_id,
                PostChangeProperty = Utility.TryParseUInt8(CampID.Text)
            });

            map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_id = Utility.TryParseUInt8(CampID.Text);
        }

        private void Unknown1_Validated(object sender, EventArgs e)
        {
            if (ListCoopCamps.SelectedIndex == Utility.NO_INDEX)
                return;

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.COOPCAMP,
                index = ListCoopCamps.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.COOPCAMPUNKNOWN,
                PreChangeProperty = map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_certain,
                PostChangeProperty = Utility.TryParseUInt16(Unknown1.Text)
            });

            map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_certain = Utility.TryParseUInt8(Unknown1.Text);
        }

        private void ButtonResizeList_Click(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "-")
                HideList();
            else
                ShowList();
        }
    }
}
