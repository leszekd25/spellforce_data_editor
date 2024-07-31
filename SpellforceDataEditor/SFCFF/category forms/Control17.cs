using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control17 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        static string[] clan_names = new string[] {
            "Neutral", "Friendly neutral [Humans]", "Friendly neutral [Elves]", "Neutral [animals for meat production]",
            "Friendly neutral [Dwarves]", "Hostile [Grargs]", "Hostile [Imperial]", "Hostile [Uroks]",
            "Hostile [Undead]", "Hostile [monsters/demons]", "Player", "Player Elves",
            "Player Humans", "Player Dwarves", "Player Orcs", "Player Trolls",
            "Player Darkelves", "Hostile [animals]", "KillAll", "Hostile [Beastmen]",
            "Hostile [Gorge]", SFEngine.Utility.S_UNKNOWN, SFEngine.Utility.S_UNKNOWN, "Hostile [Blades]",
            SFEngine.Utility.S_UNKNOWN, "Hostile [Multiplayer enemies]", "Hostile [Ogres]", "Neutral [NPCs]",
            "Hostile [Soulforger]", "Hostile [Bloodash]", SFEngine.Utility.S_UNKNOWN, "Hostile [Dervish]"};

        static private Dictionary<Byte, string> relations = new Dictionary<Byte, string>();
        static private Dictionary<string, Byte> inv_relations = new Dictionary<string, Byte>();

        static Control17()
        {
            relations[0] = "Neutral";
            relations[100] = "Friendly";
            relations[156] = "Hostile";
            inv_relations["Neutral"] = 0;
            inv_relations["Friendly"] = 100;
            inv_relations["Hostile"] = 156;
        }

        Category2023 c2023;

        public Control17()
        {
            InitializeComponent();

            c2023 = SFCategoryManager.gamedata.c2023;
            category = c2023;

            column_dict.Add("Clan ID", "ClanID");
            column_dict.Add("Clan ID 2", "ClanID2");
            column_dict.Add("Relation", "Relation");
        }

        public override void set_element(int index)
        {
            RelationGrid.Rows.Clear();
            RelationGrid.ClearSelection();
            RelationGrid.Refresh();

            current_element = index;

            for (int i = 0; i < c2023.GetItemSubItemNum(current_element); i++)
            {
                Byte clan_id = c2023[current_element, i].ClanID2;
                Byte relation = c2023[current_element, i].Relation;

                string txt = SFEngine.Utility.S_ITEM_MISSING;
                if ((clan_id >= 1) && (clan_id <= (Byte)clan_names.Length))
                {
                    txt = clan_names[clan_id - 1];
                }

                RelationGrid.Rows.Add();
                RelationGrid.Rows[i].Cells[0].Value = txt;
                ((DataGridViewComboBoxCell)RelationGrid.Rows[i].Cells[1]).Value = relations[relation];
            }

            show_element();
        }

        public override void show_element()
        {
            c2023.GetID(current_element, out int id);
            textBox1.Text = id.ToString();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (RelationGrid.CurrentCell == null)
            {
                return;
            }

            if (!(RelationGrid.CurrentCell is DataGridViewComboBoxCell))
            {
                return;
            }

            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)RelationGrid.CurrentCell;

            if (cell == null)
            {
                return;
            }

            int i = cell.RowIndex;
            Byte relation = inv_relations[(string)cell.Value];

            c2023.SetField(current_element, i, "Relation", relation);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2023.SetID(current_element, SFEngine.Utility.TryParseUInt8(textBox1.Text));
        }

        public override string get_element_string(int index)
        {
            return $"{c2023[index, 0].GetID()} {clan_names[c2023[index, 0].GetID()-1]}";
        }

        public override void on_update_subelement(int subelem_index)
        {
            Byte clan_id = c2023[current_element, subelem_index].ClanID2;
            Byte relation = c2023[current_element, subelem_index].Relation;

            string txt = SFEngine.Utility.S_ITEM_MISSING;
            if ((clan_id >= 1) && (clan_id <= (Byte)clan_names.Length))
            {
                txt = clan_names[clan_id - 1];
            }

            RelationGrid.Rows[subelem_index].Cells[0].Value = txt;
            ((DataGridViewComboBoxCell)RelationGrid.Rows[subelem_index].Cells[1]).Value = relations[relation];
        }
    }
}
