using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspectorMetadataControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        public MapInspectorMetadataControl()
        {
            InitializeComponent();
        }

        private void MapInspectorMetadataControl_VisibleChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (Visible == true)
            {
                ComboMapType.SelectedIndex = (int)map.metadata.map_type;
                UpdateCoopParameters();
                ReloadBindstoneCombo();
                ReloadPlayerSpawnList();
                ReloadMultiplayerTeamCompositionList();
            }
        }

        private void ComboMapType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PanelCoopParameters.Enabled = false;
            PanelMultiplayerCompositions.Enabled = false;

            if (ComboMapType.SelectedIndex == -1)
                return;

            map.metadata.map_type = (SFMapType)ComboMapType.SelectedIndex;
            switch(map.metadata.map_type)
            {
                case SFMapType.CAMPAIGN:
                    break;
                case SFMapType.COOP:
                case SFMapType.MULTIPLAYER:
                    PanelCoopParameters.Enabled = true;
                    PanelMultiplayerCompositions.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        private void UpdateCoopParameters()
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopMaxClanSize1.Text = map.metadata.coop_spawn_params[0].param1.ToString();
            CoopMaxClanSize2.Text = map.metadata.coop_spawn_params[1].param1.ToString();
            CoopMaxClanSize3.Text = map.metadata.coop_spawn_params[2].param1.ToString();
            CoopInitSpawn1.Text = map.metadata.coop_spawn_params[0].param2.ToString();
            CoopInitSpawn2.Text = map.metadata.coop_spawn_params[1].param2.ToString();
            CoopInitSpawn3.Text = map.metadata.coop_spawn_params[2].param2.ToString();
            CoopBeginWave1.Text = map.metadata.coop_spawn_params[0].param3.ToString();
            CoopBeginWave2.Text = map.metadata.coop_spawn_params[1].param3.ToString();
            CoopBeginWave3.Text = map.metadata.coop_spawn_params[2].param3.ToString();
            CoopSpawnDelay1.Text = map.metadata.coop_spawn_params[0].param4.ToString();
            CoopSpawnDelay2.Text = map.metadata.coop_spawn_params[1].param4.ToString();
            CoopSpawnDelay3.Text = map.metadata.coop_spawn_params[2].param4.ToString();
        }

        private void ReloadPlayerSpawnList()
        {
            ListPlayerSpawns.Items.Clear();
            if (map.metadata.spawns == null)
                return;

            for (int i = 0; i < map.metadata.spawns.Count; i++)
                ListPlayerSpawns.Items.Add((i + 1).ToString() + ". " + GetSpawnString(map.metadata.spawns[i]));
        }

        private string GetSpawnString(SFMapSpawn spawn)
        {
            string ret = "";
            if(SFCFF.SFCategoryManager.ready)
            {
                SFCFF.SFCategoryElement text_elem = SFCFF.SFCategoryManager.find_element_text(spawn.text_id, Settings.LanguageID);
                if (text_elem != null)
                    ret += Utility.CleanString(text_elem.get_single_variant(4));
                else
                    ret += Utility.S_MISSING;
                ret += " ";
            }
            ret += spawn.pos.ToString();
            return ret;
        }

        private void CoopMaxClanSize1_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopMaxClanSize1.Text = CoopMaxClanSize1.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[0].param1 = Utility.TryParseFloat(CoopMaxClanSize1.Text);
        }

        private void CoopMaxClanSize2_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopMaxClanSize2.Text = CoopMaxClanSize2.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[1].param1 = Utility.TryParseFloat(CoopMaxClanSize2.Text);
        }

        private void CoopMaxClanSize3_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopMaxClanSize3.Text = CoopMaxClanSize3.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[2].param1 = Utility.TryParseFloat(CoopMaxClanSize3.Text);
        }

        private void CoopInitSpawn1_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopInitSpawn1.Text = CoopInitSpawn1.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[0].param2 = Utility.TryParseFloat(CoopInitSpawn1.Text);
        }

        private void CoopInitSpawn2_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopInitSpawn2.Text = CoopInitSpawn2.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[1].param2 = Utility.TryParseFloat(CoopInitSpawn2.Text);
        }

        private void CoopInitSpawn3_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopInitSpawn3.Text = CoopInitSpawn3.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[2].param2 = Utility.TryParseFloat(CoopInitSpawn3.Text);
        }

        private void CoopBeginWave1_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopBeginWave1.Text = CoopBeginWave1.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[0].param3 = Utility.TryParseFloat(CoopBeginWave1.Text);
        }

        private void CoopBeginWave2_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopBeginWave2.Text = CoopBeginWave2.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[1].param3 = Utility.TryParseFloat(CoopBeginWave2.Text);
        }

        private void CoopBeginWave3_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopBeginWave3.Text = CoopBeginWave3.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[2].param3 = Utility.TryParseFloat(CoopBeginWave3.Text);
        }

        private void CoopSpawnDelay1_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopSpawnDelay1.Text = CoopSpawnDelay1.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[0].param4 = Utility.TryParseFloat(CoopSpawnDelay1.Text);
        }

        private void CoopSpawnDelay2_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopSpawnDelay2.Text = CoopSpawnDelay2.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[1].param4 = Utility.TryParseFloat(CoopSpawnDelay2.Text);
        }

        private void CoopSpawnDelay3_Validated(object sender, EventArgs e)
        {
            if (map.metadata.coop_spawn_params == null)
                return;

            CoopSpawnDelay3.Text = CoopSpawnDelay3.Text.Replace('.', ',');
            map.metadata.coop_spawn_params[2].param4 = Utility.TryParseFloat(CoopSpawnDelay3.Text);
        }

        private void ReloadBindstoneCombo()
        {
            ComboBindstoneList.Items.Clear();
            if (map.metadata.spawns == null)
                return;

            foreach (SFMapInteractiveObject io in map.int_object_manager.int_objects)
                if (io.game_id == 769)
                   ComboBindstoneList.Items.Add("Bindstone at "+io.grid_position.ToString());
        }

        private void ListPlayerSpawns_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListPlayerSpawns.SelectedIndex == -1)
                return;

            int selected_index = 0;
            foreach (SFMapInteractiveObject io in map.int_object_manager.int_objects)
            {
                if (io.game_id == 769)
                {
                    if (io.grid_position == map.metadata.spawns[ListPlayerSpawns.SelectedIndex].pos)
                    {
                        ComboBindstoneList.SelectedIndex = selected_index;
                        SelectedSpawnTextID.Text = map.metadata.spawns[ListPlayerSpawns.SelectedIndex].text_id.ToString();
                        SelectedSpawnUnknown.Text = map.metadata.spawns[ListPlayerSpawns.SelectedIndex].unknown.ToString();
                        MainForm.mapedittool.SetCameraViewPoint(io.grid_position);
                        break;
                    }
                    else
                        selected_index += 1;
                }
            }
        }

        private void ComboBindstoneList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBindstoneList.SelectedIndex == -1)
                return;

            if (ListPlayerSpawns.SelectedIndex == -1)
                return;

            if (map.metadata.spawns == null)
                return;

            List<int> bindstone_indexes = new List<int>();
            for (int i = 0; i < map.int_object_manager.int_objects.Count; i++)
                if (map.int_object_manager.int_objects[i].game_id == 769)
                    bindstone_indexes.Add(i);

            // bindstone
            int selected_bindstone_index = bindstone_indexes[ComboBindstoneList.SelectedIndex];
            SFMapInteractiveObject io = map.int_object_manager.int_objects[selected_bindstone_index];

            // selected spawn
            int selected_spawn_index = ListPlayerSpawns.SelectedIndex;

            // search for spawn on the bindstone position
            bool found_spawn = false;
            foreach(SFMapSpawn spawn in map.metadata.spawns)
            {
                if(spawn.pos == io.grid_position)
                {
                    found_spawn = true;
                    if (spawn == map.metadata.spawns[selected_spawn_index])
                        break;
                    // swap positions
                    SFCoord tmp_pos = spawn.pos;
                    spawn.pos = map.metadata.spawns[selected_spawn_index].pos;
                    map.metadata.spawns[selected_spawn_index].pos = tmp_pos;
                    break;
                }
            }
            if(found_spawn == false)
            {
                map.metadata.spawns[selected_spawn_index].pos = io.grid_position;
            }
            ReloadPlayerSpawnList();
            ListPlayerSpawns.SelectedIndex = selected_spawn_index;
        }

        private void SelectedSpawnTextID_Validated(object sender, EventArgs e)
        {
            if (ListPlayerSpawns.SelectedIndex == -1)
                return;

            if (map.metadata.spawns == null)
                return;

            int selected_spawn_index = ListPlayerSpawns.SelectedIndex;

            map.metadata.spawns[selected_spawn_index].text_id = Utility.TryParseUInt16(SelectedSpawnTextID.Text);
            ReloadPlayerSpawnList();
            ListPlayerSpawns.SelectedIndex = selected_spawn_index;
        }

        private void SelectedSpawnUnknown_Validated(object sender, EventArgs e)
        {
            if (ListPlayerSpawns.SelectedIndex == -1)
                return;

            if (map.metadata.spawns == null)
                return;

            int selected_spawn_index = ListPlayerSpawns.SelectedIndex;

            map.metadata.spawns[selected_spawn_index].unknown = Utility.TryParseInt16(SelectedSpawnUnknown.Text);
        }

        private void ReloadMultiplayerTeamCompositionList()
        {
            if (map.metadata.multi_teams == null)
                return;

            ListTeamComps.Items.Clear();
            ListTeams.Items.Clear();
            ListTeamMembers.Items.Clear();
            ListAvailablePlayers.Items.Clear();

            for (int i = 0; i < map.metadata.multi_teams.Count; i++)
                ListTeamComps.Items.Add(map.metadata.multi_teams[i].team_count.ToString() + " team"
                    + (map.metadata.multi_teams[i].team_count == 1 ? "" : "s"));
        }

        private void ListTeamComps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;

            ListTeams.Items.Clear();
            for (int i = 0; i < map.metadata.multi_teams[ListTeamComps.SelectedIndex].team_count; i++)
                ListTeams.Items.Add("Team #"+(i + 1).ToString());
        }

        private void ListTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;

            if (ListTeams.SelectedIndex == -1)
                return;

            ListTeamMembers.Items.Clear();
            FindAvailablePlayers();
            for (int i = 0; i < map.metadata.multi_teams[ListTeamComps.SelectedIndex].players[ListTeams.SelectedIndex].Count; i++)
            {
                SFMapSpawn spawn = map.metadata.spawns[map.metadata.multi_teams[ListTeamComps.SelectedIndex].players[ListTeams.SelectedIndex][i].player_id];
                ListTeamMembers.Items.Add("Player #"
                    + (map.metadata.multi_teams[ListTeamComps.SelectedIndex].players[ListTeams.SelectedIndex][i].player_id + 1).ToString()
                    + " " + spawn.pos.ToString());
            }
        }

        private List<int> FindAvailablePlayers(bool update_box = true)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return null;

            if (ListTeams.SelectedIndex == -1)
                return null;

            if(update_box)
                ListAvailablePlayers.Items.Clear();

            List<int> ret = new List<int>();
            SFMapMultiplayerTeamComposition comp = map.metadata.multi_teams[ListTeamComps.SelectedIndex];
            for (int i = 0; i < map.metadata.player_count; i++)
            {
                bool found = false;
                for(int j = 0; j < comp.players.Count; j++)
                {
                    for(int k = 0; k < comp.players[j].Count; k++)
                    {
                        if(comp.players[j][k].player_id == i)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }
                if (!found)
                {
                    if(update_box)
                        ListAvailablePlayers.Items.Add("Player #" + (i + 1).ToString() + " " + map.metadata.spawns[i].pos.ToString());
                    ret.Add(i);
                }
            }
            return ret;
        }

        private void TeamKickMember_Click(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;

            if (ListTeams.SelectedIndex == -1)
                return;

            if (ListTeamMembers.SelectedIndex == -1)
                return;

            map.metadata.multi_teams[ListTeamComps.SelectedIndex]
                .players[ListTeams.SelectedIndex]
                .RemoveAt(ListTeamMembers.SelectedIndex);
            ListTeamMembers.Items.RemoveAt(ListTeamMembers.SelectedIndex);
            FindAvailablePlayers();
        }

        private void TeamMovePlayerToTeam_Click(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;

            if (ListTeams.SelectedIndex == -1)
                return;

            if (ListAvailablePlayers.SelectedIndex == -1)
                return;

            List<int> available_players = FindAvailablePlayers(false);
            if (available_players == null)
                return;

            map.metadata.multi_teams[ListTeamComps.SelectedIndex]
                .players[ListTeams.SelectedIndex]
                .Add(new SFMapTeamPlayer(available_players[ListAvailablePlayers.SelectedIndex], (ushort)0));
            ListTeamMembers.Items.Add("Player #" + (available_players[ListAvailablePlayers.SelectedIndex] + 1).ToString()
                + " " + map.metadata.spawns[available_players[ListAvailablePlayers.SelectedIndex]].pos.ToString());
            FindAvailablePlayers();
        }

        private void ListTeamMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;

            if (ListTeams.SelectedIndex == -1)
                return;

            if (ListTeamMembers.SelectedIndex == -1)
                return;

            SFMapTeamPlayer tp = map.metadata.multi_teams[ListTeamComps.SelectedIndex]
                .players[ListTeams.SelectedIndex][ListTeamMembers.SelectedIndex];
            SelectedPlayerTextID.Text = tp.text_id.ToString();
            if (SFCFF.SFCategoryManager.ready)
            {
                SFCFF.SFCategoryElement text_elem = SFCFF.SFCategoryManager.find_element_text(tp.text_id, Settings.LanguageID);
                if (text_elem != null)
                    LabelSelectedPlayerText.Text = Utility.CleanString(text_elem.get_single_variant(4));
                else
                    LabelSelectedPlayerText.Text = Utility.S_MISSING;
            }
            SelectedPlayerName.Text = tp.coop_map_type;
            SelectedPlayerLevelRange.Text = tp.coop_map_lvl;
        }

        private void SelectedPlayerTextID_Validated(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;

            if (ListTeams.SelectedIndex == -1)
                return;

            if (ListTeamMembers.SelectedIndex == -1)
                return;

            SFMapTeamPlayer tp = map.metadata.multi_teams[ListTeamComps.SelectedIndex]
                .players[ListTeams.SelectedIndex][ListTeamMembers.SelectedIndex];
            tp.text_id = Utility.TryParseUInt16(SelectedPlayerTextID.Text);
            if (SFCFF.SFCategoryManager.ready)
            {
                SFCFF.SFCategoryElement text_elem = SFCFF.SFCategoryManager.find_element_text(tp.text_id, Settings.LanguageID);
                if (text_elem != null)
                    LabelSelectedPlayerText.Text = Utility.CleanString(text_elem.get_single_variant(4));
                else
                    LabelSelectedPlayerText.Text = Utility.S_MISSING;
            }
        }
    }
}
