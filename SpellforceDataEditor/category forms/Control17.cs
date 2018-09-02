using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.category_forms
{
    public partial class Control17 : SpellforceDataEditor.category_forms.SFControl
    {
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
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;


            for (int i = 0; i < elem_count; i++)
            {
                Byte clan_id = (Byte)(elem.get_single_variant(i * 3 + 1)).value;
                Byte relation = (Byte)(elem.get_single_variant(i * 3 + 2)).value;

                string txt = "<MISSING!>";
                if ((clan_id >= 1) && (clan_id <= (Byte)SFCategory17.clan_names.Length))
                    txt = SFCategory17.clan_names[clan_id-1];
                
                RelationGrid.Rows.Add();
                RelationGrid.Rows[i].Cells[0].Value = txt;
                ((DataGridViewComboBoxCell)RelationGrid.Rows[i].Cells[1]).Value = relations[relation];
            }

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (RelationGrid.CurrentCell == null)
                return;
            if (!(RelationGrid.CurrentCell is DataGridViewComboBoxCell))
                return;

            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)RelationGrid.CurrentCell;

            if (cell == null)
                return;

            int i = cell.RowIndex;
            Byte relation = inv_relations[(string)cell.Value];
            /*if ((int)cell.Value == 0)
                relation = 0;
            else if ((int)cell.Value == 1)
                relation = 100;
            else if ((int)cell.Value == 2)
                relation = 156;*/
            category.set_element_variant(current_element, i * 3 + 2, relation);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category.get_element(current_element);
            int elem_count = elem.get().Count / 3;

            for (int i = 0; i < elem_count; i++)
                category.set_element_variant(current_element, 0 + 3 * i, Utility.TryParseUInt8(textBox1.Text));
        }
    }
}
