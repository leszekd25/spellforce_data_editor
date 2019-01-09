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
    public partial class Control35 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        private int vertex_index;

        public Control35()
        {
            InitializeComponent();
            column_dict.Add("Object ID", new int[1] { 0 });
            column_dict.Add("Polygon index", new int[1] { 1 });
            column_dict.Add("Unknown", new int[1] { 2 });
            column_dict.Add("Vertex count", new int[1] { 3 });
            vertex_index = -1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, 2, Utility.TryParseUInt8(textBox5.Text));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            vertex_index = listBox1.SelectedIndex;
            textBox3.Text = variant_repr(4 + vertex_index * 2);
            textBox4.Text = variant_repr(4 + vertex_index * 2 + 1);
        }

        private void listBox1_update()
        {
            listBox1.Items.Clear();
            int vertex_count = ((category.get_element(current_element).get().Count) - 4) / 2;
            for (int i = 0; i < vertex_count; i++)
            {
                Int16 x = (Int16)category.get_element_variant(current_element, 4 + i * 2).value;
                Int16 y = (Int16)category.get_element_variant(current_element, 4 + i * 2 + 1).value;
                listBox1.Items.Add(x.ToString() + " | " + y.ToString());
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (vertex_index == -1)
                return;
            set_element_variant(current_element, 4 + vertex_index * 2, Utility.TryParseInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (vertex_index == -1)
                return;
            set_element_variant(current_element, 4 + vertex_index * 2 + 1, Utility.TryParseInt16(textBox4.Text));
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            textBox2.Text = variant_repr(1);
            textBox5.Text = variant_repr(2);
            vertex_index = -1;
            listBox1_update();
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 33);
        }
    }
}
