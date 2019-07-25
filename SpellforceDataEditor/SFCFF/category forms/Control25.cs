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
    public partial class Control25 : SpellforceDataEditor.SFCFF.category_forms.SFControl
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

        public List<int> GetOffsets()
        {
            List<int> offsets = new List<int>();
            int cur_variant = 0;
            SFCategoryElement elem = category[current_element];
            while(cur_variant < elem.variants.Count)
            {
                offsets.Add(cur_variant);
                cur_variant += 4;
                Byte vcount = (Byte)elem[cur_variant - 1];
                cur_variant += (vcount * 2);
            }
            return offsets;
        }

        public int GetVertexCount(int p_index)
        {
            if (p_index == -1)
                return 0;
            List<int> offsets = GetOffsets();
            
            if (p_index == ListPolygons.Items.Count - 1)
                return ((category[current_element].variants.Count - offsets[ListPolygons.Items.Count - 1]) - 4) / 2;
            else
               return ((offsets[p_index + 1] - offsets[p_index] - 4) / 2);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            foreach(int off in GetOffsets())
                set_element_variant(current_element, off, Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (ListPolygons.SelectedIndex == -1)
                return;
            set_element_variant(current_element, GetOffsets()[ListPolygons.SelectedIndex]+2, Utility.TryParseUInt8(textBox5.Text));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            vertex_index = listBox1.SelectedIndex;
            if (vertex_index == -1)
                return;
            List<int> offsets = GetOffsets();
            textBox3.Text = variant_repr(offsets[ListPolygons.SelectedIndex] + 4 + vertex_index * 2);
            textBox4.Text = variant_repr(offsets[ListPolygons.SelectedIndex] + 4 + vertex_index * 2 + 1);
        }

        private void listBox1_update()
        {
            listBox1.Items.Clear();
            List<int> offsets = GetOffsets();

            int vertex_count = GetVertexCount(ListPolygons.SelectedIndex);

            for(int i = 0; i < vertex_count; i++)
            {
                Int16 x = (Int16)category[current_element][offsets[ListPolygons.SelectedIndex] + 4 + i * 2];
                Int16 y = (Int16)category[current_element][offsets[ListPolygons.SelectedIndex] + 4 + i * 2 + 1];
                listBox1.Items.Add(x.ToString() + " | " + y.ToString());
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (vertex_index == -1)
                return;
            set_element_variant(current_element, GetOffsets()[ListPolygons.SelectedIndex] + 4 + vertex_index * 2, Utility.TryParseInt16(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (vertex_index == -1)
                return;
            set_element_variant(current_element, GetOffsets()[ListPolygons.SelectedIndex] + 4 + vertex_index * 2 + 1, Utility.TryParseInt16(textBox4.Text));
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
            vertex_index = -1;
            listBox1_update();
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == -1)
                return;
            int v_index = vertex_index;
            if (v_index == -1)
                v_index = listBox1.Items.Count;

            SFCategoryElement elem = category[current_element];

            object[] paste_data = new object[2];
            paste_data[0] = (Int16)0;
            paste_data[1] = (Int16)0;

            int vc_offset = GetOffsets()[p_index] + 3;
            Byte vcount = (Byte)elem[vc_offset];

            elem.PasteRaw(paste_data, GetOffsets()[p_index] + 4 + v_index * 2);

            elem[vc_offset] = (Byte)(vcount + 1);

            listBox1_update();
            
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == -1)
                return;
            int v_index = vertex_index;
            if (v_index == -1)
                return;
            if (GetVertexCount(p_index) <= 1)
                return;

            SFCategoryElement elem = category[current_element];

            int vc_offset = GetOffsets()[p_index] + 3;
            Byte vcount = (Byte)elem[vc_offset];

            elem.RemoveRaw(GetOffsets()[p_index] + 4 + v_index * 2, 2);

            elem[vc_offset] = (Byte)(vcount - 1);

            listBox1_update();
            
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                step_into(textBox1, 23);
        }

        private void ListPolygons_update()
        {
            SFCategoryElement elem = category[current_element];

            ListPolygons.Items.Clear();

            foreach (int o in GetOffsets())
            {
                Byte p_index = (Byte)elem[o + 1];
                ListPolygons.Items.Add(p_index.ToString());
            }
        }

        private void ListPolygons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListPolygons.SelectedIndex == -1)
                return;

            listBox1_update();
            textBox5.Text = variant_repr(GetOffsets()[ListPolygons.SelectedIndex] + 2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == -1)
                p_index = ListPolygons.Items.Count;

            SFCategoryElement elem = category[current_element];

            Byte max_index = 0;
            foreach(int o in GetOffsets())
            {
                max_index = Math.Max(max_index, (Byte)(elem[o + 1]));
            }
            max_index = (Byte)(max_index + 1);

            object[] paste_data = new object[6];
            paste_data[0] = (UInt16)elem[0];
            paste_data[1] = (Byte)max_index;
            paste_data[2] = (Byte)1;
            paste_data[3] = (Byte)1;
            paste_data[4] = (Int16)0;
            paste_data[5] = (Int16)0;

            elem.PasteRaw(paste_data, GetOffsets()[p_index]);

            set_element(current_element);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == -1)
                return;
            if (ListPolygons.Items.Count <= 1)
                return;

            SFCategoryElement elem = category[current_element];

            int count = 4 + (GetVertexCount(p_index)*2);

            elem.RemoveRaw(GetOffsets()[p_index], count);

            set_element(current_element);
        }
    }
}
