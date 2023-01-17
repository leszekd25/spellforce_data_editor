﻿using SFEngine.SFCFF;
using SFEngine.SFLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    public partial class SFLuaSQLBuildingForm : Form
    {
        List<int> index_to_key = null;
        int selected_id = SFEngine.Utility.NO_INDEX;

        public SFLuaSQLBuildingForm()
        {
            InitializeComponent();
        }

        private void SFLuaSQLBuildingForm_Load(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.buildings.Load() != 0)
            {
                MessageBox.Show("Could not load script/sql_building.lua");
                Close();
                return;
            }

            ReloadBuildingList();
        }

        private void ReloadBuildingList()
        {
            ListBuildings.Items.Clear();
            ListBuildings.SuspendLayout();

            var buildings = SFLuaEnvironment.buildings.buildings;
            index_to_key = buildings.Keys.ToList();
            index_to_key.Sort();

            foreach (int i in index_to_key)
            {
                ListBuildings.Items.Add(i.ToString() + ". " + GetBuildingString(i));
            }

            ListBuildings.ResumeLayout();
        }

        private string GetBuildingString(int id)
        {
            if (!SFCategoryManager.ready)
            {
                return "";
            }

            return SFCategoryManager.GetBuildingName((ushort)id);
        }

        private void ListBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBuildings.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                Mesh.Items.Clear();
                SelectedMesh.Text = "";
                SelectionSize.Text = "";
                return;
            }

            int building_id = index_to_key[ListBuildings.SelectedIndex];
            selected_id = building_id;

            var buildings = SFLuaEnvironment.buildings.buildings;
            if (!buildings.ContainsKey(building_id))
            {
                return;
            }

            Mesh.Items.Clear();
            for (int i = 0; i < buildings[building_id].Mesh.Count; i++)
            {
                Mesh.Items.Add(buildings[building_id].Mesh[i]);
            }

            SelectedMesh.Text = "";
            SelectionSize.Text = buildings[building_id].SelectionScaling.ToString();
            BuildingID.Text = building_id.ToString();
        }

        private void Mesh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            var buildings = SFLuaEnvironment.buildings.buildings;

            if (Mesh.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                SelectedMesh.Text = "";
            }
            else
            {
                SelectedMesh.Text = buildings[selected_id].Mesh[Mesh.SelectedIndex];
            }
        }

        private void ButtonAddMesh_Click(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            var buildings = SFLuaEnvironment.buildings.buildings;

            buildings[selected_id].Mesh.Add("");
            Mesh.Items.Add(SFEngine.Utility.S_MISSING);

            Mesh.SelectedIndex = buildings[selected_id].Mesh.Count - 1;
        }

        private void ButtonRemoveMesh_Click(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            if (Mesh.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            var buildings = SFLuaEnvironment.buildings.buildings;
            int cur_index = Mesh.SelectedIndex;

            buildings[selected_id].Mesh.RemoveAt(Mesh.SelectedIndex);
            Mesh.Items.RemoveAt(Mesh.SelectedIndex);

            if (cur_index == Mesh.Items.Count)
            {
                cur_index -= 1;
            }

            Mesh.SelectedIndex = cur_index;
        }

        private void SelectedMesh_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            if (Mesh.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            var buildings = SFLuaEnvironment.buildings.buildings;

            buildings[selected_id].Mesh[Mesh.SelectedIndex] = SelectedMesh.Text;
            Mesh.Items[Mesh.SelectedIndex] = SelectedMesh.Text;
        }

        private void SelectionSize_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFLuaEnvironment.buildings.buildings[selected_id].SelectionScaling =
                SFEngine.Utility.TryParseDouble(SelectionSize.Text,
                                       SFLuaEnvironment.buildings.buildings[selected_id].SelectionScaling);
        }

        private void BuildingID_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int previous_id = selected_id;
            int previous_index = ListBuildings.SelectedIndex;
            SFEngine.SFLua.lua_sql.SFLuaSQLBuildingData building = SFLuaEnvironment.buildings.buildings[previous_id];
            int new_id = (int)SFEngine.Utility.TryParseUInt32(BuildingID.Text, (uint)previous_id);
            if (new_id == previous_id)
            {
                return;
            }

            SFLuaEnvironment.buildings.buildings.Remove(selected_id);
            index_to_key.RemoveAt(previous_index);
            ListBuildings.Items.RemoveAt(previous_index);

            int new_index = SFEngine.Utility.FindNewIndexOf(index_to_key, new_id);
            if (new_index == SFEngine.Utility.NO_INDEX)
            {
                new_index = previous_index;
                new_id = previous_id;
                BuildingID.Text = previous_id.ToString();
            }

            index_to_key.Insert(new_index, new_id);
            SFLuaEnvironment.buildings.buildings.Add(new_id, building);
            ListBuildings.Items.Insert(new_index, new_id.ToString() + ". " + GetBuildingString(new_id));

            selected_id = new_id;
            ListBuildings.SelectedIndex = new_index;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (index_to_key[0] == 0)
            {
                selected_id = 0;
                ListBuildings.SelectedIndex = 0;
                return;
            }

            SFEngine.SFLua.lua_sql.SFLuaSQLBuildingData building = new SFEngine.SFLua.lua_sql.SFLuaSQLBuildingData();
            building.Mesh = new List<string>();
            building.SelectionScaling = 1;

            index_to_key.Insert(0, 0);
            SFLuaEnvironment.buildings.buildings.Add(0, building);
            ListBuildings.Items.Insert(0, "0. " + GetBuildingString(0));

            selected_id = 0;
            ListBuildings.SelectedIndex = 0;
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int ind = ListBuildings.SelectedIndex;
            SFLuaEnvironment.buildings.buildings.Remove(selected_id);
            index_to_key.RemoveAt(ListBuildings.SelectedIndex);
            ListBuildings.Items.RemoveAt(ListBuildings.SelectedIndex);

            if (ind == index_to_key.Count)
            {
                ind -= 1;
            }

            ListBuildings.SelectedIndex = ind;
        }

        private void ButtonCancelChanges_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonSaveChanges_Click(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.buildings.Save() != 0)
            {
                MessageBox.Show("Error while saving sql_building.lua");
            }
            Close();
        }
    }
}
