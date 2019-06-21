using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspectorNPCControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        int selected_npc = 0;
        int selected_npc_index = -1;
        public List<int> indices_to_keys { get; private set; } = new List<int>();     // it must be ALWAYS up to date
        public HashSet<int> created_npc_ids { get; private set; } = new HashSet<int>();

        public MapInspectorNPCControl()
        {
            InitializeComponent();
        }

        private void MapInspectorNPCControl_VisibleChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (Visible)
            {
                ReloadNPCList();
            }
        }

        private void ReloadNPCList()
        {
            selected_npc = 0;
            selected_npc_index = -1;

            ListNPCs.Items.Clear();

            indices_to_keys = map.npc_manager.npc_info.Keys.ToList();
            indices_to_keys.Sort();
            foreach (int npc_key in indices_to_keys)
            {
                string new_item = npc_key.ToString() + ". ";
                new_item += GetNPCName(npc_key);

                ListNPCs.Items.Add(new_item);
            }
        }

        private string GetNPCString(int npc_id)
        {
            NPCType type = map.npc_manager.npc_info[npc_id].npc_type;
            string new_item = "";
            if (type == NPCType.BUILDING)
                new_item += SFCFF.SFCategoryManager.get_building_name((ushort)((SFMapBuilding)map.npc_manager.npc_info[npc_id].npc_ref).game_id);
            else if (type == NPCType.OBJECT)
                new_item += SFCFF.SFCategoryManager.get_object_name((ushort)((SFMapObject)map.npc_manager.npc_info[npc_id].npc_ref).game_id);
            else if (type == NPCType.UNIT)
                new_item += SFCFF.SFCategoryManager.get_unit_name((ushort)((SFMapUnit)map.npc_manager.npc_info[npc_id].npc_ref).game_id, true);

            return new_item;
        }

        private string GetNPCName(int npc_id)
        {
            int npc_index = SFCFF.SFCategoryManager.gamedata.categories[36].get_element_index(npc_id);
            if (npc_index == -1)
                return Utility.S_MISSING;
            SFCFF.SFCategoryElement npc_data = SFCFF.SFCategoryManager.gamedata.categories[36].get_element(npc_index);
            ushort text_id = (ushort)npc_data.get_single_variant(1).value;
            SFCFF.SFCategoryElement text_data = SFCFF.SFCategoryManager.find_element_text(text_id, 1);
            if (text_data == null)
                return Utility.S_NONAME;
            return Utility.CleanString(text_data.get_single_variant(4));
        }

        // assumes indices_to_keys are up-to-date
        private void ListNPCs_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectNPC(ListNPCs.SelectedIndex, true);
        }

        public void SelectNPC(int npc_index, bool move_view)
        {
            selected_npc_index = npc_index;
            if (selected_npc_index == -1)
            {
                LabelNPCName.Text = "";
                return;
            }

            selected_npc = indices_to_keys[selected_npc_index];

            SFMapNPCInfo npc_info = map.npc_manager.npc_info[selected_npc];

            LabelNPCName.Text = GetNPCString(selected_npc);
            SelectedNPCID.Text = selected_npc.ToString();

            // move screen to selected npc
            if (move_view)
            {
                NPCType npc_type = npc_info.npc_type;
                object npc_ref = npc_info.npc_ref;
                if (npc_type == NPCType.BUILDING)
                    MainForm.mapedittool.SetCameraViewPoint(((SFMapBuilding)npc_ref).grid_position);
                else if (npc_type == NPCType.OBJECT)
                    MainForm.mapedittool.SetCameraViewPoint(((SFMapObject)npc_ref).grid_position);
                else if (npc_type == NPCType.UNIT)
                    MainForm.mapedittool.SetCameraViewPoint(((SFMapUnit)npc_ref).grid_position);
            }
        }

        private void SelectedNPCID_Validated(object sender, EventArgs e)
        {
            if (selected_npc_index == -1)
                return;

            int npc_id = (int)Utility.TryParseUInt32(SelectedNPCID.Text);
            if(npc_id == 0)
            {
                SelectedNPCID.Text = selected_npc.ToString();
                return;
            }

            // test if given NPC ID exists
            if(SFCFF.SFCategoryManager.gamedata.categories[36].get_element_index(npc_id) == -1)
            {
                SelectedNPCID.Text = selected_npc.ToString();
                return;
            }

            // test if given NPC ID is not taken yet
            if(map.npc_manager.npc_info.ContainsKey(npc_id))
            {
                SelectedNPCID.Text = selected_npc.ToString();
                return;
            }

            ChangeNPCID(selected_npc, npc_id);
        }

        // assumes current_id is used, new_id is not, and both ids exist in gamedata
        private void ChangeNPCID(int current_id, int new_id)
        {
            SFMapNPCInfo npc_info = map.npc_manager.npc_info[current_id];
            // update object npc id
            NPCType npc_type = npc_info.npc_type;
            object npc_ref = npc_info.npc_ref;
            if (npc_type == NPCType.BUILDING)
                ((SFMapBuilding)npc_ref).npc_id = new_id;
            else if (npc_type == NPCType.OBJECT)
                ((SFMapObject)npc_ref).npc_id = new_id;
            else if (npc_type == NPCType.UNIT)
                ((SFMapUnit)npc_ref).npc_id = new_id;

            // remove previous key, add new key
            map.npc_manager.npc_info.Remove(current_id);
            map.npc_manager.npc_info.Add(new_id, new SFMapNPCInfo(npc_ref));

            // update list
            ReloadNPCList();
            int npc_index = indices_to_keys.IndexOf(new_id);
            SelectNPC(npc_index, false);
        }

        // n log n, can be improved to n...
        public int FindLastUnusedNPCID()
        {
            for (int id = 12000; id < 1000000; id++)
            {
                if (SFCFF.SFCategoryManager.gamedata.categories[36].get_element_index(id) == -1)
                    return id;
            }
            return -1;
        }

        // assumes npc_id doesn't exist in gamedata
        public int AddNewNPCID(int npc_id)
        {
            int new_elem_index = SFCFF.SFCategoryManager.gamedata.categories[36].get_new_element_index(npc_id);
            if (new_elem_index == -1)
                return -1;

            SFCFF.SFCategoryElement new_elem = new SFCFF.SFCategoryElement();
            new_elem.add_single_variant((uint)npc_id);
            new_elem.add_single_variant((ushort)0);

            if (MainForm.data != null)
                MainForm.data.mapeditor_insert_npc(new_elem_index, new_elem);
            else
                SFCFF.SFCategoryManager.gamedata.categories[36].get_elements().Insert(new_elem_index, new_elem);

            created_npc_ids.Add(npc_id);

            return 0;
        }

        // used when newly assigned NPC IDs are not used, before gamedata save
        public int RemoveNewNPCID(int npc_id)
        {
            int new_elem_index = SFCFF.SFCategoryManager.gamedata.categories[36].get_new_element_index(npc_id);
            if (new_elem_index == -1)
                return -1;

            if (MainForm.data != null)
                MainForm.data.mapeditor_remove_npc(new_elem_index);
            else
                SFCFF.SFCategoryManager.gamedata.categories[36].get_elements().RemoveAt(new_elem_index);

            return 0;
        }

        private void ButtonCreateNPC_Click(object sender, EventArgs e)
        {
            if (selected_npc_index == -1)
                return;

            int new_npc_id = FindLastUnusedNPCID();
            if(new_npc_id != -1)
            {
                AddNewNPCID(new_npc_id);
                int cur_npc_id = Utility.TryParseInt32(SelectedNPCID.Text);
                ChangeNPCID(cur_npc_id, new_npc_id);
            }
        }

        private void ButtonNPCMapProperties_Click(object sender, EventArgs e)
        {
            if (selected_npc_index == -1)
                return;

            SFMapNPCInfo npc_info = map.npc_manager.npc_info[selected_npc];
            // update object npc id
            NPCType npc_type = npc_info.npc_type;
            object npc_ref = npc_info.npc_ref;
            if (npc_type == NPCType.BUILDING)
                MainForm.mapedittool.GoToBuildingProperties((SFMapBuilding)npc_ref);
            else if (npc_type == NPCType.OBJECT)
                MainForm.mapedittool.GoToObjectProperties((SFMapObject)npc_ref);
            else if (npc_type == NPCType.UNIT)
                MainForm.mapedittool.GoToUnitProperties((SFMapUnit)npc_ref);
        }

        private void ButtonRemoveNPC_Click(object sender, EventArgs e)
        {
            if (selected_npc_index == -1)
                return;

            int new_selected = selected_npc_index;

            map.npc_manager.RemoveNPCRef(selected_npc);
            ListNPCs.SelectedIndex = -1;
            ReloadNPCList();

            if (new_selected >= indices_to_keys.Count)
                new_selected -= 1;
            ListNPCs.SelectedIndex = new_selected;
        }
    }
}
