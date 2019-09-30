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

        private List<int> FindAvailablePlayers(bool update_box = true)
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
                SFCFF.SFCategoryElement text_elem = SFCFF.SFCategoryManager.FindElementText(tp.text_id, Settings.LanguageID);
                if (text_elem != null)
                    LabelSelectedPlayerText.Text = Utility.CleanString(text_elem[4]);
                else
                    LabelSelectedPlayerText.Text = Utility.S_MISSING;
            }
        }

        private void TeamCompAdd_Click(object sender, EventArgs e)
        {
            if (map.metadata.map_type == SFMapType.CAMPAIGN)
                return;

            if (map.metadata.multi_teams == null)
                map.metadata.multi_teams = new List<SFMapMultiplayerTeamComposition>();

            int max_teamcomp_teams = 0;

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
            map.metadata.multi_teams.Add(tc);

            ReloadMultiplayerTeamCompositionList();
            ListTeamComps.SelectedIndex = map.metadata.multi_teams.Count - 1;
        }

        private void TeamCompRemove_Click(object sender, EventArgs e)
        {
            if (ListTeamComps.SelectedIndex == -1)
                return;

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

            tp.coop_map_type = SelectedPlayerName.Text;
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

            tp.coop_map_lvl = SelectedPlayerLevelRange.Text;
        }

        private void SelectedPlayerTextID_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(SelectedPlayerTextID.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[14].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(14, real_elem_id);
            }
        }
    }
}
