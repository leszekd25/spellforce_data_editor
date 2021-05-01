using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_dialog
{
    public partial class MapManageTeamCompositions : Form
    {
        public SFMap map = null;

        public MapManageTeamCompositions()
        {
            InitializeComponent();
        }

        private void MapManageTeamCompositions_Load(object sender, EventArgs e)
        {
            ReloadMultiplayerTeamCompositionList();
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

        // used by teamcomp operator
        // this should get changed for sure, to not use this ugly design
        public void external_RefreshTeamcompList()
        {
            ReloadMultiplayerTeamCompositionList();

            if (map == null)
                return;
            if (map.metadata == null)
                return;

            if (map.metadata.multi_teams.Count > 0)
                ListTeamComps.SelectedIndex = map.metadata.multi_teams.Count - 1;
        }

        private void ListTeamComps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;

            ListTeams.Items.Clear();
            for (int i = 0; i < map.metadata.multi_teams[ListTeamComps.SelectedIndex].team_count; i++)
                ListTeams.Items.Add("Team #" + (i + 1).ToString());
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

        public void AddTeamMemberEntry(SFMapTeamPlayer player)
        {
            ListTeamMembers.Items.Add("Player #" + (player.player_id+1).ToString()
                + " " + map.metadata.spawns[player.player_id].pos.ToString());
        }

        public void RemoveTeamMemberEntry(int index)
        {
            ListTeamMembers.Items.RemoveAt(index);
        }

        // returns list of available players to add to a team, and optionally updates the form with available players
        // ugly design...
        public List<int> FindAvailablePlayers(bool update_box = true)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return null;

            if (ListTeams.SelectedIndex == -1)
                return null;

            if (update_box)
                ListAvailablePlayers.Items.Clear();

            List<int> ret = new List<int>();
            SFMapMultiplayerTeamComposition comp = map.metadata.multi_teams[ListTeamComps.SelectedIndex];
            for (int i = 0; i < map.metadata.spawns.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < comp.players.Count; j++)
                {
                    for (int k = 0; k < comp.players[j].Count; k++)
                    {
                        if (comp.players[j][k].player_id == i)
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
                    if (update_box)
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

            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorMultiplayerMovePlayer()
            {
                player = map.metadata.multi_teams[ListTeamComps.SelectedIndex].players[ListTeams.SelectedIndex][ListTeamMembers.SelectedIndex],
                comp_index = ListTeamComps.SelectedIndex,
                PreOperatorPlayerTeam = ListTeamComps.SelectedIndex,
                PostOperatorPlayerTeam = -1,
                player_index = ListTeamMembers.SelectedIndex
            });

            map.metadata.multi_teams[ListTeamComps.SelectedIndex]
                .players[ListTeams.SelectedIndex]
                .RemoveAt(ListTeamMembers.SelectedIndex);

            RemoveTeamMemberEntry(ListTeamMembers.SelectedIndex);
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

            SFMapTeamPlayer player = new SFMapTeamPlayer(available_players[ListAvailablePlayers.SelectedIndex], (ushort)0);

            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorMultiplayerMovePlayer()
            {
                player = player,
                comp_index = ListTeamComps.SelectedIndex,
                PreOperatorPlayerTeam = -1,
                PostOperatorPlayerTeam = ListTeams.SelectedIndex,
                player_index = ListTeamMembers.Items.Count
            });

            map.metadata.multi_teams[ListTeamComps.SelectedIndex]
                .players[ListTeams.SelectedIndex]
                .Add(player);

            AddTeamMemberEntry(player);
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
                SFCFF.SFCategoryElement text_elem = SFCFF.SFCategoryManager.FindElementText(tp.text_id, Settings.LanguageID);
                if (text_elem != null)
                    LabelSelectedPlayerText.Text = Utility.CleanString(text_elem[4]);
                else
                    LabelSelectedPlayerText.Text = Utility.S_MISSING;
            }
            SelectedPlayerName.Text = tp.coop_map_type;
            SelectedPlayerLevelRange.Text = tp.coop_map_lvl;

            SFMapSpawn spawn = map.metadata.spawns[tp.player_id];
            MainForm.mapedittool.SetCameraViewPoint(spawn.pos);
        }

        public void UpdatePlayerDataUI(int teamcomp_index, int team_index, int teamplayer_index)
        {
            SFMapTeamPlayer tp = map.metadata.multi_teams[teamcomp_index]
                   .players[team_index][teamplayer_index];

            if (SFCFF.SFCategoryManager.ready)
            {
                SFCFF.SFCategoryElement text_elem = SFCFF.SFCategoryManager.FindElementText(tp.text_id, Settings.LanguageID);
                if (text_elem != null)
                    LabelSelectedPlayerText.Text = Utility.CleanString(text_elem[4]);
                else
                    LabelSelectedPlayerText.Text = Utility.S_MISSING;
            }

            SelectedPlayerLevelRange.Text = tp.coop_map_lvl;
            SelectedPlayerName.Text = tp.coop_map_type;
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

            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorMultiplayerSetPlayerState()
            {
                state_type = map_operators.MapOperatorPlayerStateType.TEXT_ID,
                comp_index = ListTeamComps.SelectedIndex,
                team_index = ListTeams.SelectedIndex,
                player_index = ListTeamMembers.SelectedIndex,
                PreOperatorState = tp.text_id,
                PostOperatorState = Utility.TryParseUInt16(SelectedPlayerTextID.Text)
            });
            tp.text_id = Utility.TryParseUInt16(SelectedPlayerTextID.Text);

            UpdatePlayerDataUI(ListTeamComps.SelectedIndex, ListTeams.SelectedIndex, ListTeamMembers.SelectedIndex);
        }

        // adds a new team composition to map metadata
        // it does so by finding the least available unused team composition (that is, the one with fewest teams)
        private void TeamCompAdd_Click(object sender, EventArgs e)
        {
            if (map.metadata.map_type == SFMapType.CAMPAIGN)
                return;

            if (map.metadata.multi_teams == null)
                map.metadata.multi_teams = new List<SFMapMultiplayerTeamComposition>();

            int max_teamcomp_teams = 0;

            // coop maps can have team composition of one team, other maps must have at least 2 teams
            int start = (map.metadata.map_type == SFMapType.COOP ? 1 : 2);
            for (int i = start; i <= 4; i++)
            {
                bool success = false;
                foreach (SFMapMultiplayerTeamComposition t_comp in map.metadata.multi_teams)
                    if (t_comp.team_count == i)
                    {
                        success = true;
                        break;
                    }

                if (!success)
                {
                    max_teamcomp_teams = i;
                    break;
                }
            }

            if (max_teamcomp_teams == 0)
                return;


            SFMapMultiplayerTeamComposition tc = new SFMapMultiplayerTeamComposition();
            tc.team_count = max_teamcomp_teams;
            tc.players = new List<List<SFMapTeamPlayer>>();
            for (int i = 0; i < max_teamcomp_teams; i++)
                tc.players.Add(new List<SFMapTeamPlayer>());


            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorMultiplayerAddOrRemoveTeamComp()
            { team_comp = tc, is_adding = true });
            map.metadata.InsertTeamComp(tc);

            ReloadMultiplayerTeamCompositionList();
            ListTeamComps.SelectedIndex = map.metadata.multi_teams.Count - 1;
        }

        private void TeamCompRemove_Click(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;


            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorMultiplayerAddOrRemoveTeamComp()
            { team_comp = map.metadata.multi_teams[ListTeamComps.SelectedIndex], is_adding = false });

            map.metadata.multi_teams.RemoveAt(ListTeamComps.SelectedIndex);

            ReloadMultiplayerTeamCompositionList();
            ListTeamComps.SelectedIndex = map.metadata.multi_teams.Count - 1;
        }

        private void SelectedPlayerName_Validated(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;

            if (ListTeams.SelectedIndex == -1)
                return;

            if (ListTeamMembers.SelectedIndex == -1)
                return;

            SFMapTeamPlayer tp = map.metadata.multi_teams[ListTeamComps.SelectedIndex]
                .players[ListTeams.SelectedIndex][ListTeamMembers.SelectedIndex];

            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorMultiplayerSetPlayerState()
            {
                state_type = map_operators.MapOperatorPlayerStateType.NAME,
                comp_index = ListTeamComps.SelectedIndex,
                team_index = ListTeams.SelectedIndex,
                player_index = ListTeamMembers.SelectedIndex,
                PreOperatorState = tp.coop_map_type,
                PostOperatorState = SelectedPlayerName.Text
            });

            tp.coop_map_type = SelectedPlayerName.Text;

            UpdatePlayerDataUI(ListTeamComps.SelectedIndex, ListTeams.SelectedIndex, ListTeamMembers.SelectedIndex);
        }

        private void SelectedPlayerLevelRange_Validated(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;

            if (ListTeams.SelectedIndex == -1)
                return;

            if (ListTeamMembers.SelectedIndex == -1)
                return;

            SFMapTeamPlayer tp = map.metadata.multi_teams[ListTeamComps.SelectedIndex]
                .players[ListTeams.SelectedIndex][ListTeamMembers.SelectedIndex];

            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorMultiplayerSetPlayerState()
            {
                state_type = map_operators.MapOperatorPlayerStateType.LEVEL_RANGE,
                comp_index = ListTeamComps.SelectedIndex,
                team_index = ListTeams.SelectedIndex,
                player_index = ListTeamMembers.SelectedIndex,
                PreOperatorState = tp.coop_map_lvl,
                PostOperatorState = SelectedPlayerLevelRange.Text
            });

            tp.coop_map_lvl = SelectedPlayerLevelRange.Text;

            UpdatePlayerDataUI(ListTeamComps.SelectedIndex, ListTeams.SelectedIndex, ListTeamMembers.SelectedIndex);
        }

        private void SelectedPlayerTextID_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(SelectedPlayerTextID.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[2016].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(14, real_elem_id);
            }
        }
    }
}
