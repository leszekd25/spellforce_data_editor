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
            SFCategoryElement elem = category[current_element];

            for (int i = 0; i < elem.variants.Count; i += 4)
                set_element_variant(current_element, i, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (ListPolygons.SelectedIndex == -1)
                return;

            set_element_variant(current_element, ListPolygons.SelectedIndex * 4 + 2, Utility.TryParseUInt8(textBox5.Text));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            vertex_index = listBox1.SelectedIndex;
            if (vertex_index == Utility.NO_INDEX)
                return;

            SFCategoryElement elem = category[current_element];

            textBox3.Text = ((SFOutlineData)elem[ListPolygons.SelectedIndex * 4 + 3]).Data[vertex_index * 2 + 0].ToString();
            textBox4.Text = ((SFOutlineData)elem[ListPolygons.SelectedIndex * 4 + 3]).Data[vertex_index * 2 + 1].ToString();
        }

        private void listbox1_update_vertex(int v_ind)
        {
            if ((v_ind < 0) || (v_ind >= listBox1.Items.Count))
                return;
            Int16 x = ((SFOutlineData)category[current_element][ListPolygons.SelectedIndex * 4 + 3]).Data[v_ind * 2 + 0];
            Int16 y = ((SFOutlineData)category[current_element][ListPolygons.SelectedIndex * 4 + 3]).Data[v_ind * 2 + 1];
            listBox1.Items[v_ind] = (x.ToString() + " | " + y.ToString());
        }

        private void listBox1_update()
        {
            if (ListPolygons.SelectedIndex == -1)
                return;

            listBox1.Items.Clear();
            SFCategoryElement elem = category[current_element];

            int vertex_count = ((SFOutlineData)elem[ListPolygons.SelectedIndex * 4 + 3]).Data.Count / 2;

            for (int i = 0; i < vertex_count; i++)
            {
                listBox1.Items.Add("");
                listbox1_update_vertex(i);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (vertex_index == Utility.NO_INDEX)
                return;

            ((SFOutlineData)(category[current_element][ListPolygons.SelectedIndex * 4 + 3])).Data[vertex_index * 2 + 0] = Utility.TryParseInt16(textBox3.Text);
            listbox1_update_vertex(vertex_index);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (vertex_index == Utility.NO_INDEX)
                return;

            ((SFOutlineData)(category[current_element][ListPolygons.SelectedIndex * 4 + 3])).Data[vertex_index * 2 + 1] = Utility.TryParseInt16(textBox3.Text);
            listbox1_update_vertex(vertex_index);
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListPolygons_update();

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0);
            vertex_index = Utility.NO_INDEX;
            listBox1_update();
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == Utility.NO_INDEX)
                return;
            int v_index = vertex_index;
            if (v_index == Utility.NO_INDEX)
                v_index = listBox1.Items.Count;

            SFCategoryElement elem = category[current_element];

            ((SFOutlineData)elem[p_index * 4 + 3]).Data.Insert(v_index * 2, 0);
            ((SFOutlineData)elem[p_index * 4 + 3]).Data.Insert(v_index * 2, 0);

            listBox1_update();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == Utility.NO_INDEX)
                return;
            int v_index = vertex_index;
            if (v_index == Utility.NO_INDEX)
                return;

            SFCategoryElement elem = category[current_element];

            if (((SFOutlineData)elem[p_index * 4 + 3]).Data.Count <= 1)
                return;

            ((SFOutlineData)elem[p_index * 4 + 3]).Data.RemoveAt(v_index * 2);
            ((SFOutlineData)elem[p_index * 4 + 3]).Data.RemoveAt(v_index * 2);

            listBox1_update();

        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 2057);
        }

        private void ListPolygons_update()
        {
            SFCategoryElement elem = category[current_element];
            int elem_count = elem.variants.Count / 4;

            ListPolygons.Items.Clear();

            for (int i = 0; i < elem_count; i++)
                ListPolygons.Items.Add(i.ToString());
        }

        private void ListPolygons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListPolygons.SelectedIndex == Utility.NO_INDEX)
                return;

            listBox1_update();
            textBox5.Text = variant_repr(ListPolygons.SelectedIndex * 4 + 2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == Utility.NO_INDEX)
                p_index = ListPolygons.Items.Count;

            SFCategoryElement elem = category[current_element];

            Byte max_index = 0;
            for (int i = 1; i < elem.variants.Count; i += 4)
                max_index = Math.Max(max_index, (Byte)(elem[i + 1]));
            max_index = (Byte)(max_index + 1);

            object[] paste_data = new object[4];
            paste_data[0] = (UInt16)elem[0];
            paste_data[1] = (Byte)max_index;
            paste_data[2] = (Byte)1;
            paste_data[3] = new SFOutlineData() { Data = new List<short>() };
            ((SFOutlineData)paste_data[3]).Data.Add(0);
            ((SFOutlineData)paste_data[3]).Data.Add(0);

            elem.PasteRaw(paste_data, p_index * 4);

            set_element(current_element);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == Utility.NO_INDEX)
                return;
            if (ListPolygons.Items.Count <= 1)
                return;

            SFCategoryElement elem = category[current_element];
            elem.RemoveRaw(p_index * 4, 4);

            set_element(current_element);
        }


        public override string get_element_string(int index)
        {
            UInt16 object_id = (UInt16)category[index][0];
            Byte b_index = (Byte)category[index][1];
            string txt_building = SFCategoryManager.GetObjectName(object_id);
            return object_id.ToString() + " " + txt_building + " [" + b_index.ToString() + "]";
        }
    }
}
