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
            "Hostile [Gorge]", SFEngine.Utility.S_UNKNOWN, SFEngine.Utility.S_NONE, "Hostile [Blades]",
            SFEngine.Utility.S_NONE, "Hostile [Multiplayer enemies]", "Hostile [Ogres]", "Neutral [NPCs]",
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

        public Control17()
        {
            InitializeComponent();
            //how to deal with this one?
            column_dict.Add("Head ID", new int[1] { 0 });
            column_dict.Add("Head index", new int[1] { 1 });
            column_dict.Add("Unknown", new int[1] { 2 });
        }

        public override void set_element(int index)
        {
            RelationGrid.Rows.Clear();
            RelationGrid.ClearSelection();
            RelationGrid.Refresh();

            current_element = index;

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                Byte clan_id = (Byte)(category[current_element, i][1]);
                Byte relation = (Byte)(category[current_element, i][2]);

                string txt = "<MISSING!>";
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
            textBox1.Text = variant_repr(0, 0);
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

            set_element_variant(current_element, i, 2, relation);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            MainForm.data.op_queue.OpenCluster();
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                set_element_variant(current_element, i, 0, SFEngine.Utility.TryParseUInt8(textBox1.Text));
            }
            MainForm.data.op_queue.CloseCluster();
        }

        public override string get_element_string(int index)
        {
            string txt = clan_names[(int)(Byte)(category[index, 0][0]) - 1];
            return category[index, 0][0].ToString() + " " + txt;
        }

        public override void on_update_subelement(int subelem_index)
        {
            Byte clan_id = (Byte)(category[current_element, subelem_index][1]);
            Byte relation = (Byte)(category[current_element, subelem_index][2]);

            string txt = "<MISSING!>";
            if ((clan_id >= 1) && (clan_id <= (Byte)clan_names.Length))
            {
                txt = clan_names[clan_id - 1];
            }

            RelationGrid.Rows[subelem_index].Cells[0].Value = txt;
            ((DataGridViewComboBoxCell)RelationGrid.Rows[subelem_index].Cells[1]).Value = relations[relation];
        }
    }
}
