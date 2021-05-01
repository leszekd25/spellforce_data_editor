using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control31 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        static string[] item_types = { Utility.S_UNKNOWN, "Equipment", "Inventory rune", "Installed rune",
            "Spell scroll", "Equipped scroll", "Unit plan", "Building plan", "Equipped unit plan",
            "Equipped building plan", "Miscellaneous" };

        public Control31()
        {
            InitializeComponent();
            column_dict.Add("Merchant ID", new int[1] { 0 });
            column_dict.Add("Item type", new int[1] { 1 });
            column_dict.Add("Price multiplier", new int[1] { 2 });
        }

        private void load_item_types()
        {
            comboItemType.Items.Clear();
            
            int elem_count = item_types.Length;
            for (int i = 1; i < elem_count; i++)
            {
                comboItemType.Items.Add(item_types[i]);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            for (int i = 0; i < elem_count; i++)
                set_element_variant(current_element, i * 3 + 0, Utility.TryParseUInt16(textBox5.Text));
        }


        private void RefreshListItemTypes()
        {
            ListItemTypes.Items.Clear();

            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            for (int i = 0; i < elem_count; i++)
            {
                int res_index = (int)(Byte)elem[i * 3 + 1];
                string res_name = "";
                if (res_index == 0)
                    res_name = Utility.S_NONE;
                else
                    res_name = comboItemType.Items[res_index - 1].ToString();    //-1 because of null value
                ListItemTypes.Items.Add(res_name);
            }
        }

        public override void set_element(int index)
        {
            current_element = index;

            load_item_types();

            RefreshListItemTypes();

            ListItemTypes.SelectedIndex = 0;
        }

        public override void show_element()
        {
            textBox5.Text = variant_repr(0);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox5, 2029);
        }

        private void ListItemTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListItemTypes.SelectedIndex == Utility.NO_INDEX)
                return;

            int index = ListItemTypes.SelectedIndex;

            comboItemType.SelectedIndex = (Byte)category[current_element][index * 3 + 1] - 1;
            textBox4.Text = variant_repr(index * 3 + 2);
        }

        private void comboItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboItemType.SelectedIndex == Utility.NO_INDEX)
                return;

            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            int cur_index = ListItemTypes.SelectedIndex;
            Byte current_res = (Byte)elem[cur_index * 3 + 1];
            Byte new_res = (Byte)(comboItemType.SelectedIndex + 1);
            if (current_res == new_res)
                return;

            // check if resource like this already exists
            for (int i = 0; i < elem_count; i++)
            {
                Byte res_id = (Byte)elem[i * 3 + 1];
                if (res_id == new_res)
                {
                    new_res = 0;
                    break;
                }
            }

            // generate new element with reordered resources by resource id, ascending order
            object[] cur_data = elem.CopyRaw(cur_index * 3, 3);
            elem.RemoveRaw(cur_index * 3, 3);
            int new_index = elem_count - 1;
            for (int i = 0; i < elem_count - 1; i++)
            {
                if ((Byte)elem[i * 3 + 1] > new_res)
                {
                    new_index = i;
                    break;
                }
            }
            elem.PasteRaw(cur_data, new_index * 3);
            elem[new_index * 3 + 1] = new_res;
            RefreshListItemTypes();
            ListItemTypes.SelectedIndex = new_index;
        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int index = ListItemTypes.SelectedIndex;
            set_element_variant(current_element, 3 * index + 2, Utility.TryParseUInt16(textBox4.Text));
            RefreshListItemTypes();
            ListItemTypes.SelectedIndex = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            object[] paste_data = new object[3];
            paste_data[0] = (UInt16)elem[0];
            paste_data[1] = (Byte)0;
            paste_data[2] = (UInt16)0;

            elem.PasteRaw(paste_data, 0);

            RefreshListItemTypes();
            ListItemTypes.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListItemTypes.Items.Count == 1)
                return;

            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 3;

            int index = ListItemTypes.SelectedIndex;

            elem.RemoveRaw(index * 3, 3);

            RefreshListItemTypes();
            ListItemTypes.SelectedIndex = Math.Min(index, ListItemTypes.Items.Count - 1);
        }


        public override string get_element_string(int index)
        {
            UInt16 merchant_id = (UInt16)category[index][0];
            string txt_merchant = SFCategoryManager.GetMerchantName(merchant_id);
            return merchant_id.ToString() + " " + txt_merchant;
        }
    }
}
