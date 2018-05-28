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
    public partial class Control25 : SpellforceDataEditor.category_forms.SFControl
    {
        private int vertex_index;

        public Control25()
        {
            InitializeComponent();
            column_dict.Add("Building ID", new int[1] { 0 });
            column_dict.Add("Polygon index", new int[1] { 1 });
            column_dict.Add("Unknown", new int[1] { 2 });
            column_dict.Add("Vertex count", new int[1] { 3 });
            vertex_index = -1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 0, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 1, Utility.TryParseUInt8(textBox2.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            category.set_element_variant(current_element, 2, Utility.TryParseUInt8(textBox5.Text));
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
            int vertex_count = ((category.get_element(current_element).get().Count)-4)/2;
            for(int i = 0; i < vertex_count; i++)
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
            category.set_element_variant(current_element, 4 + vertex_index * 2, Utility.TryParseInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (vertex_index == -1)
                return;
            category.set_element_variant(current_element, 4 + vertex_index * 2 + 1, Utility.TryParseInt16(textBox4.Text));
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

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            int new_index;
            if (listBox1.SelectedIndex == -1)
                if (listBox1.Items.Count == 0)
                    new_index = 0;
                else
                    return;
            else
                new_index = listBox1.SelectedIndex;

            SFCategoryElement elem = category.get_element(current_element);
            int len = elem.get().Count;
            int poly_count = (elem.get().Count - 4) / 2;
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array = new Object[len+2];
            
            for(int i = 0; i < 3; i++)
            {
                obj_array[i] = elem.get_single_variant(i).value;
            }
            obj_array[3] = (Byte)(poly_count + 1);

            int offset = 0;
            for(int i = 0; i < poly_count; i++)
            {
                //Console.WriteLine(elem.get_single_variant((i + offset) * 2 + 0).value);
                obj_array[4+(i + offset) * 2 + 0] = elem.get_single_variant(4+(i * 2) + 0).value;
                obj_array[4+(i + offset) * 2 + 1] = elem.get_single_variant(4+(i * 2) + 1).value;
                if(i == new_index)
                {
                    offset = 1;
                    //Console.WriteLine("cur ind: " + i.ToString());
                    obj_array[4+(i + offset) * 2 + 0] = (Int16)0;
                    obj_array[4+(i + offset) * 2 + 1] = (Int16)0;
                }
            }

            if(poly_count == 0)
            {
                obj_array[4] = (Int16)0;
                obj_array[5] = (Int16)0;
            }

            new_elem.set(obj_array);
            category.get_elements()[current_element] = new_elem;
            listBox1_update();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int new_index;
            if (listBox1.SelectedIndex == -1)
                    return;
            else
                new_index = listBox1.SelectedIndex;

            SFCategoryElement elem = category.get_element(current_element);
            new_index = listBox1.SelectedIndex;
            int len = elem.get().Count;
            int poly_count = (elem.get().Count - 4) / 2;
            SFCategoryElement new_elem = new SFCategoryElement();
            Object[] obj_array = new Object[len - 2];

            for (int i = 0; i < 3; i++)
            {
                obj_array[i] = elem.get_single_variant(i).value;
            }
            obj_array[3] = (Byte)(poly_count - 1);

            int offset = 0;
            for (int i = 0; i < poly_count-1; i++)
            {
                if (i == new_index)
                {
                    offset = 1;
                }
                //Console.WriteLine(elem.get_single_variant((i + offset) * 2 + 0).value);
                obj_array[4 + i * 2 + 0] = elem.get_single_variant(4 + ((i+offset) * 2) + 0).value;
                obj_array[4 + i * 2 + 1] = elem.get_single_variant(4 + ((i+offset) * 2) + 1).value;
            }

            new_elem.set(obj_array);
            category.get_elements()[current_element] = new_elem;
            listBox1_update();
        }
    }
}
