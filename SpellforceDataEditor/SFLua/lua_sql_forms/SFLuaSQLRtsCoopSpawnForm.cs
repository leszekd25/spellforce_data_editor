using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SpellforceDataEditor.SFLua.lua_sql;
using SpellforceDataEditor.SFMap;

namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    public partial class SFLuaSQLRtsCoopSpawnForm : Form
    {
        bool can_load = true;

        public SFLuaSQLRtsCoopSpawnForm()
        {
            InitializeComponent();

            foreach (string s in Enum.GetNames(typeof(LuaEnumAiGoal)))
                GroupGoal.Items.Add(s);
                
        }

        private void SFLuaSQLRtsCoopSpawnForm_Load(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.coop_spawns.Load() != 0)
            {
                MessageBox.Show("Could not load script/GdsRtsCoopSpawnGroups.lua");
                can_load = false;
                this.Close();
                return;
            }

            ReloadSpawnTypeList();
        }

        private void SFLuaSQLRtsCoopSpawnForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(can_load)
                SFLuaEnvironment.coop_spawns.Load();
        }

        private void ReloadSpawnTypeList()
        {
            ListSpawnTypes.Items.Clear();
            var spawn_types = SFLuaEnvironment.coop_spawns.coop_spawn_types;

            List<int> types = spawn_types.Keys.ToList();
            types.Sort();

            foreach(int i in types)
                ListSpawnTypes.Items.Add(i.ToString() + ". " + spawn_types[i].name);
        }

        private void ReloadSpawnDataList()
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            GroupSpawnData.Items.Clear();

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            var spawn_data = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data;

            List<int> activation_keys = spawn_data.Keys.ToList();
            activation_keys.Sort();

            foreach (int i in activation_keys)
                GroupSpawnData.Items.Add("At " + i.ToString() + " minutes");
        }

        private void ListSpawnTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
            {
                GroupName.Text = "";
                GroupLevelRange.Text = "";
                GroupGoal.Text = "";
                GroupClanSize.Text = "";
                GroupStartUnits.Items.Clear();
                SelectedUnitID.Text = "";
                return;
            }

            int group_id = ListSpawnTypes.SelectedIndex + 1;    // even though its a dictionary, they're listed sequentially!

            var spawn_types = SFLuaEnvironment.coop_spawns.coop_spawn_types;
            if (!spawn_types.ContainsKey(group_id))
                return;

            GroupName.Text = spawn_types[group_id].name;
            GroupLevelRange.Text = spawn_types[group_id].level_range;
            GroupGoal.SelectedItem = spawn_types[group_id].goal.ToString();
            GroupClanSize.Text = spawn_types[group_id].max_units.ToString();

            GroupStartUnits.Items.Clear();
            if (spawn_types[group_id].start_units != null)
            {
                foreach (int i in spawn_types[group_id].start_units)
                {
                    if (SFCFF.SFCategoryManager.ready)
                        GroupStartUnits.Items.Add(i.ToString() + " - " + SFCFF.SFCategoryManager.get_unit_name((ushort)i, true));
                    else
                        GroupStartUnits.Items.Add(i.ToString());
                }
            }

            ReloadSpawnDataList();
            GroupSpawnData.SelectedIndex = -1;
        }

        private void GroupSpawnData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupSpawnData.SelectedIndex == -1)
            {
                SpawnDataActivation.Text = "";
                SpawnDataHours.Text = "";
                SpawnDataMinutes.Text = "";
                SpawnDataSeconds.Text = "";
                SpawnDataUnits.Items.Clear();
                SelectedSpawnDataUnitID.Text = "";
                return;
            }

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            var spawn_data = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data;

            List<int> activation_keys = spawn_data.Keys.ToList();
            activation_keys.Sort();
            int key = activation_keys[GroupSpawnData.SelectedIndex];

            SpawnDataActivation.Text = key.ToString();

            int hours, minutes;
            int seconds = spawn_data[key].seconds_per_tick;
            hours = seconds / 3600; seconds -= hours * 3600;
            minutes = seconds / 60; seconds -= minutes * 60;

            SpawnDataHours.Text = hours.ToString();
            SpawnDataMinutes.Text = minutes.ToString();
            SpawnDataSeconds.Text = seconds.ToString();

            SpawnDataUnits.Items.Clear();
            if (spawn_data[key].units != null)
            {
                foreach (int i in spawn_data[key].units)
                {
                    if (SFCFF.SFCategoryManager.ready)
                        SpawnDataUnits.Items.Add(i.ToString() + " - " + SFCFF.SFCategoryManager.get_unit_name((ushort)i, true));
                    else
                        SpawnDataUnits.Items.Add(i.ToString());
                }
            }
        }

        private void ButtonSaveChanges_Click(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.coop_spawns.Save() != 0)
            {
                MessageBox.Show("Error while saving GdsRtsCoopSpawnGroups.lua");
            }
            this.Close();
        }

        private void ButtonCancelChanges_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GroupName_Validated(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].name = GroupName.Text;
        }

        private void GroupLevelRange_Validated(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].level_range = GroupLevelRange.Text;
        }

        private void GroupGoal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GroupGoal.SelectedIndex == -1)
                return;

            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;

            LuaEnumAiGoal goal;
            Enum.TryParse(GroupGoal.SelectedItem.ToString(), out goal);

            SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].goal = goal;
        }

        private void GroupClanSize_Validated(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].max_units = Utility.TryParseUInt16(GroupClanSize.Text);
        }

        private void SelectedUnitID_Validated(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupStartUnits.SelectedIndex == -1)
                return;
            
            int group_id = ListSpawnTypes.SelectedIndex + 1;
            ushort unit_id = Utility.TryParseUInt16(SelectedUnitID.Text);
            SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].start_units[GroupStartUnits.SelectedIndex] = (int)unit_id;

            if (SFCFF.SFCategoryManager.ready)
                GroupStartUnits.Items[GroupStartUnits.SelectedIndex] = (unit_id.ToString() + " - " + SFCFF.SFCategoryManager.get_unit_name(unit_id, true));
            else
                GroupStartUnits.Items[GroupStartUnits.SelectedIndex] = (unit_id.ToString());
        }

        private void SpawnDataActivation_Validated(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupSpawnData.SelectedIndex == -1)
                return;

            int new_key = (int)Utility.TryParseUInt32(SpawnDataActivation.Text);
            if (new_key == 0)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            var spawn_data = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data;
            if (spawn_data.ContainsKey(new_key))
                return;

            List<int> activation_keys = spawn_data.Keys.ToList();
            activation_keys.Sort();
            int key = activation_keys[GroupSpawnData.SelectedIndex];

            var selected_spawn_data = spawn_data[key];
            spawn_data.Remove(key);
            spawn_data.Add(new_key, selected_spawn_data);

            ReloadSpawnDataList();

            activation_keys = spawn_data.Keys.ToList();
            activation_keys.Sort();
            int key_index = activation_keys.IndexOf(new_key);
            GroupSpawnData.SelectedIndex = key_index;
        }

        private void SpawnDataHours_Validated(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupSpawnData.SelectedIndex == -1)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            var spawn_data = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data;

            List<int> activation_keys = spawn_data.Keys.ToList();
            activation_keys.Sort();
            int key = activation_keys[GroupSpawnData.SelectedIndex];

            int hours, minutes;
            int seconds = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data[key].seconds_per_tick;
            hours = seconds / 3600; seconds -= hours * 3600;
            minutes = seconds / 60; seconds -= minutes * 60;

            hours = Utility.TryParseUInt16(SpawnDataHours.Text);
            seconds += minutes * 60; seconds += hours * 3600;
            SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data[key].seconds_per_tick = seconds;

            SpawnDataHours.Text = hours.ToString();
        }

        private void SpawnDataMinutes_Validated(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupSpawnData.SelectedIndex == -1)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            var spawn_data = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data;

            List<int> activation_keys = spawn_data.Keys.ToList();
            activation_keys.Sort();
            int key = activation_keys[GroupSpawnData.SelectedIndex];

            int hours, minutes;
            int seconds = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data[key].seconds_per_tick;
            hours = seconds / 3600; seconds -= hours * 3600;
            minutes = seconds / 60; seconds -= minutes * 60;

            minutes = (int)Utility.TryParseUInt32(SpawnDataMinutes.Text);
            hours += minutes / 60; minutes = minutes % 60;
            seconds += minutes * 60; seconds += hours * 60;
            SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data[key].seconds_per_tick = seconds;

            SpawnDataHours.Text = hours.ToString();
            SpawnDataMinutes.Text = minutes.ToString();
        }

        private void SpawnDataSeconds_Validated(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupSpawnData.SelectedIndex == -1)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            var spawn_data = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data;

            List<int> activation_keys = spawn_data.Keys.ToList();
            activation_keys.Sort();
            int key = activation_keys[GroupSpawnData.SelectedIndex];

            int hours, minutes;
            int seconds = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data[key].seconds_per_tick;
            hours = seconds / 3600; seconds -= hours * 3600;
            minutes = seconds / 60; seconds -= minutes * 60;

            seconds = (int)Utility.TryParseUInt32(SpawnDataSeconds.Text);
            minutes += seconds / 60; seconds = seconds % 60;
            hours += minutes / 60; minutes = minutes % 60;
            seconds += minutes * 60; seconds += hours * 60;
            SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data[key].seconds_per_tick = seconds;

            SpawnDataHours.Text = hours.ToString();
            SpawnDataMinutes.Text = minutes.ToString();
            SpawnDataSeconds.Text = seconds.ToString();
        }

        private void SelectedSpawnDataUnitID_Validated(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupSpawnData.SelectedIndex == -1)
                return;

            if (SpawnDataUnits.SelectedIndex == -1)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            var spawn_data = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data;

            List<int> activation_keys = spawn_data.Keys.ToList();
            activation_keys.Sort();
            int key = activation_keys[GroupSpawnData.SelectedIndex];
            
            ushort unit_id = Utility.TryParseUInt16(SelectedSpawnDataUnitID.Text);
            SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data[key].units[SpawnDataUnits.SelectedIndex] = (int)unit_id;

            if (SFCFF.SFCategoryManager.ready)
                SpawnDataUnits.Items[SpawnDataUnits.SelectedIndex] = (unit_id.ToString() + " - " + SFCFF.SFCategoryManager.get_unit_name(unit_id, true));
            else
                SpawnDataUnits.Items[SpawnDataUnits.SelectedIndex] = (unit_id.ToString());
        }

        private void SelectedUnitID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MainForm.data == null)
                    return;
                if (!SFCFF.SFCategoryManager.ready)
                    return;

                ushort unit_id = Utility.TryParseUInt16(SelectedUnitID.Text);

                MainForm.data.Tracer_StepForward(17, unit_id, false);
            }
        }

        private void SelectedSpawnDataUnitID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MainForm.data == null)
                    return;
                if (!SFCFF.SFCategoryManager.ready)
                    return;

                ushort unit_id = Utility.TryParseUInt16(SelectedSpawnDataUnitID.Text);

                MainForm.data.Tracer_StepForward(17, unit_id, false);
            }
        }

        private void ButtonAddCoopSpawn_Click(object sender, EventArgs e)
        {
            int max_type = 0;
            foreach (int key in SFLuaEnvironment.coop_spawns.coop_spawn_types.Keys)
                if (key > max_type)
                    max_type = key;

            SFMapCoopSpawnTypeInfo new_type = new SFMapCoopSpawnTypeInfo();
            new_type.name = "New spawn type";
            new_type.level_range = "???";
            new_type.goal = LuaEnumAiGoal.GoalCoopAggressive;
            new_type.max_units = 0;
            SFLuaEnvironment.coop_spawns.coop_spawn_types.Add(max_type + 1, new_type);

            ListSpawnTypes.Items.Add((max_type+1).ToString() + ". " + new_type.name);
            ListSpawnTypes.SelectedIndex = max_type;
        }

        private void GroupUnitAdd_Click(object sender, EventArgs e)
        {
            if (GroupStartUnits.SelectedIndex == -1)
                return;

            int key = ListSpawnTypes.SelectedIndex + 1;
            var spawn_type = SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types[key];

            if (spawn_type.start_units == null)
                spawn_type.start_units = new List<int>();
            spawn_type.start_units.Add(0);

            if (SFCFF.SFCategoryManager.ready)
                GroupStartUnits.Items.Add("0 - " + SFCFF.SFCategoryManager.get_unit_name((ushort)0, true));
            else
                GroupStartUnits.Items.Add("0");

            GroupStartUnits.SelectedIndex = spawn_type.start_units.Count - 1;
        }

        private void GroupUnitRemove_Click(object sender, EventArgs e)
        {
            int index = GroupStartUnits.SelectedIndex;
            if (index == -1)
                return;

            int key = ListSpawnTypes.SelectedIndex + 1;
            var spawn_type = SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types[key];

            spawn_type.start_units.RemoveAt(index);
            if (spawn_type.start_units.Count == 0)
                spawn_type.start_units = null;
            GroupStartUnits.Items.RemoveAt(index);

            if (index == GroupStartUnits.Items.Count)
                index -= 1;
            GroupStartUnits.SelectedIndex = index;
        }

        private void SpawnDataAdd_Click(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;
            
            int key = ListSpawnTypes.SelectedIndex + 1;
            var spawn_type = SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types[key];

            if (spawn_type.data == null)
                spawn_type.data = new Dictionary<int, SFMapCoopSpawnTypeDataInfo>();

            if (spawn_type.data.ContainsKey(0))
                return;

            SFMapCoopSpawnTypeDataInfo new_data = new SFMapCoopSpawnTypeDataInfo();
            new_data.seconds_per_tick = 60;
            spawn_type.data.Add(0, new_data);

            ReloadSpawnDataList();
            GroupSpawnData.SelectedIndex = 0;
        }

        private void SpawnDataRemove_Click(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            int index = GroupSpawnData.SelectedIndex;
            if (index == -1)
                return;
            
            int key = ListSpawnTypes.SelectedIndex + 1;
            var spawn_type = SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types[key];

            List<int> activation_keys = spawn_type.data.Keys.ToList();
            activation_keys.Sort();
            int selected_key = activation_keys[index];

            spawn_type.data.Remove(selected_key);
            if (spawn_type.data.Count == 0)
                spawn_type.data = null;
            GroupSpawnData.Items.Remove(index);

            ReloadSpawnDataList();
            if (index == GroupSpawnData.Items.Count)
                index -= 1;
            GroupSpawnData.SelectedIndex = index;
        }

        private void SpawnDataUnitAdd_Click(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupSpawnData.SelectedIndex == -1)
                return;

            int key = ListSpawnTypes.SelectedIndex + 1;
            var spawn_type = SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types[key];
            if (spawn_type.data == null)
                return;

            List<int> activation_keys = spawn_type.data.Keys.ToList();
            activation_keys.Sort();
            int selected_key = activation_keys[GroupSpawnData.SelectedIndex];

            if (spawn_type.data[selected_key].units == null)
                spawn_type.data[selected_key].units = new List<int>();
            spawn_type.data[selected_key].units.Add(0);

            if (SFCFF.SFCategoryManager.ready)
                SpawnDataUnits.Items.Add("0 - " + SFCFF.SFCategoryManager.get_unit_name((ushort)0, true));
            else
                SpawnDataUnits.Items.Add("0");

            SpawnDataUnits.SelectedIndex = spawn_type.data[selected_key].units.Count - 1;
        }

        private void SpawnDataUnitRemove_Click(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupSpawnData.SelectedIndex == -1)
                return;

            int index = SpawnDataUnits.SelectedIndex;
            if (index == -1)
                return;

            int key = ListSpawnTypes.SelectedIndex + 1;
            var spawn_type = SFLua.SFLuaEnvironment.coop_spawns.coop_spawn_types[key];
            if (spawn_type.data == null)
                return;

            List<int> activation_keys = spawn_type.data.Keys.ToList();
            activation_keys.Sort();
            int selected_key = activation_keys[GroupSpawnData.SelectedIndex];

            spawn_type.data[selected_key].units.RemoveAt(index);
            if (spawn_type.data[selected_key].units.Count == 0)
                spawn_type.data[selected_key].units = null;
            SpawnDataUnits.Items.RemoveAt(index);

            if (index == SpawnDataUnits.Items.Count)
                index -= 1;
            SpawnDataUnits.SelectedIndex = index;
        }

        private void GroupStartUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupStartUnits.SelectedIndex == -1)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            SelectedUnitID.Text = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].start_units[GroupStartUnits.SelectedIndex].ToString();
        }

        private void SpawnDataUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListSpawnTypes.SelectedIndex == -1)
                return;

            if (GroupSpawnData.SelectedIndex == -1)
                return;

            if (SpawnDataUnits.SelectedIndex == -1)
                return;

            int group_id = ListSpawnTypes.SelectedIndex + 1;
            var spawn_data = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data;

            List<int> activation_keys = spawn_data.Keys.ToList();
            activation_keys.Sort();
            int key = activation_keys[GroupSpawnData.SelectedIndex];

            SelectedSpawnDataUnitID.Text = SFLuaEnvironment.coop_spawns.coop_spawn_types[group_id].data[key].units[SpawnDataUnits.SelectedIndex].ToString();
        }
    }
}
