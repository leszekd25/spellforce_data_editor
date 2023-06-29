using SFEngine.SFLua;
using SFEngine.SFMap;
using System;

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
            {
                LoadNextCoopCamp(i);
            }
        }

        private string GetCoopSpawnString(SFMapCoopAISpawn spawn)
        {
            string ret = "";
            if (SFLuaEnvironment.coop_spawns.items != null)
            {
                if (SFLuaEnvironment.coop_spawns.items.ContainsKey(spawn.spawn_id))
                {
                    ret += SFLuaEnvironment.coop_spawns.items[spawn.spawn_id].name + " ";
                }
            }

            ret += spawn.spawn_obj.grid_position.ToString();
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
            PanelCoopCampList.Height = Height - PanelCoopCampList.Location.Y - 3;
            ListCoopCamps.Height = PanelCoopCampList.Height - 75;
        }

        public void RemoveCoopCamp(int index)
        {
            if (ListCoopCamps.SelectedIndex == index)
            {
                PanelProperties.Enabled = false;
            }

            ListCoopCamps.Items.RemoveAt(index);
        }

        public void LoadNextCoopCamp(int index)
        {
            ListCoopCamps.Items.Insert(index, GetCoopSpawnString(map.metadata.coop_spawns[index]));
        }

        private void HideList()
        {
            if (ButtonResizeList.Text == "+")
            {
                return;
            }

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
            {
                ListCoopCamps.SelectedIndex = map.metadata.coop_spawns.IndexOf((SFMapCoopAISpawn)o);
            }
        }

        private void ListCoopCamps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListCoopCamps.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            PanelProperties.Enabled = true;
            SFMapCoopAISpawn spawn = map.metadata.coop_spawns[ListCoopCamps.SelectedIndex];
            CampID.Text = spawn.spawn_id.ToString();
            PosX.Text = spawn.spawn_obj.grid_position.x.ToString();
            PosY.Text = spawn.spawn_obj.grid_position.y.ToString();
            Unknown1.Text = spawn.spawn_certain.ToString();

            if ((move_camera_on_select) || (spawn_selected_from_list))
            {
                MainForm.mapedittool.SetCameraViewPoint(spawn.spawn_obj.grid_position);
            }

            move_camera_on_select = false;
            spawn_selected_from_list = true;
        }

        private void CampID_Validated(object sender, EventArgs e)
        {
            if (ListCoopCamps.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.COOPCAMP,
                index = ListCoopCamps.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.ID,
                PreChangeProperty = map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_id,
                PostChangeProperty = (int)(SFEngine.Utility.TryParseUInt16(CampID.Text))
            });

            map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_id = SFEngine.Utility.TryParseUInt16(CampID.Text);
        }

        private void Unknown1_Validated(object sender, EventArgs e)
        {
            if (ListCoopCamps.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            // undo/redo
            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorEntityChangeProperty()
            {
                type = map_operators.MapOperatorEntityType.COOPCAMP,
                index = ListCoopCamps.SelectedIndex,
                property = map_operators.MapOperatorEntityProperty.COOPCAMPUNKNOWN,
                PreChangeProperty = map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_certain,
                PostChangeProperty = (int)(SFEngine.Utility.TryParseUInt8(Unknown1.Text))
            });

            map.metadata.coop_spawns[ListCoopCamps.SelectedIndex].spawn_certain = SFEngine.Utility.TryParseUInt8(Unknown1.Text);
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
    }
}
