using SFEngine.SFCFF;
using System;
using System.Collections.Generic;
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
            column_dict.Add("Casts shadow", new int[1] { 2 });
            column_dict.Add("Vertex count", new int[1] { 3 });
            vertex_index = SFEngine.Utility.NO_INDEX;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            MainForm.data.op_queue.OpenCluster();
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                set_element_variant(current_element, i, 0, SFEngine.Utility.TryParseUInt16(textBox1.Text));
            }

            MainForm.data.op_queue.CloseCluster();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (ListPolygons.SelectedIndex == -1)
            {
                return;
            }

            set_element_variant(current_element, ListPolygons.SelectedIndex, 2, SFEngine.Utility.TryParseUInt8(textBox5.Text));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            vertex_index = listBox1.SelectedIndex;
            if (vertex_index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFOutlineData sd = (SFOutlineData)(category[current_element, ListPolygons.SelectedIndex][3]);

            textBox3.Text = sd.Data[vertex_index * 2 + 0].ToString();
            textBox4.Text = sd.Data[vertex_index * 2 + 1].ToString();
        }

        private void listbox1_update_vertex(int v_ind)
        {
            if ((v_ind < 0) || (v_ind >= listBox1.Items.Count))
            {
                return;
            }

            SFOutlineData sd = (SFOutlineData)(category[current_element, ListPolygons.SelectedIndex][3]);
            listBox1.Items[v_ind] = sd.Data[v_ind * 2 + 0].ToString() + " | " + sd.Data[v_ind * 2 + 1].ToString();
        }

        private void listBox1_update()
        {
            if (ListPolygons.SelectedIndex == -1)
            {
                return;
            }

            listBox1.Items.Clear();

            SFOutlineData sd = (SFOutlineData)(category[current_element, ListPolygons.SelectedIndex][3]);

            for (int i = 0; i < sd.Data.Count / 2; i++)
            {
                listBox1.Items.Add("");
                listbox1_update_vertex(i);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (vertex_index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            short new_x = SFEngine.Utility.TryParseInt16(textBox3.Text);
            if (new_x == ((SFOutlineData)(category[current_element, ListPolygons.SelectedIndex][3])).Data[vertex_index * 2 + 0])
            {
                return;
            }

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorModifyCategoryElementOutlineData()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = ListPolygons.SelectedIndex,
                VariantIndex = 3,
                VertexIndex = vertex_index,
                NewX = new_x,
                NewY = ((SFOutlineData)(category[current_element, ListPolygons.SelectedIndex][3])).Data[vertex_index * 2 + 1],
            });

            listBox1.SelectedIndex = vertex_index;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (vertex_index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            short new_y = SFEngine.Utility.TryParseInt16(textBox4.Text);
            if (new_y == ((SFOutlineData)(category[current_element, ListPolygons.SelectedIndex][3])).Data[vertex_index * 2 + 1])
            {
                return;
            }

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorModifyCategoryElementOutlineData()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = ListPolygons.SelectedIndex,
                VariantIndex = 3,
                VertexIndex = vertex_index,
                NewX = ((SFOutlineData)(category[current_element, ListPolygons.SelectedIndex][3])).Data[vertex_index * 2 + 0],
                NewY = new_y,
            });

            listBox1.SelectedIndex = vertex_index;
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListPolygons_update();

            show_element();
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0, 0);
            vertex_index = SFEngine.Utility.NO_INDEX;
            listBox1_update();
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int v_index = vertex_index;
            if (v_index == SFEngine.Utility.NO_INDEX)
            {
                v_index = listBox1.Items.Count;
            }

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElementOutlineData()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = p_index,
                VariantIndex = 3,
                VertexIndex = v_index,
                X = 0,
                Y = 0,
                IsSubElement = true
            });

            listBox1.SelectedIndex = v_index;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int v_index = vertex_index;
            if (v_index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            if (((SFOutlineData)category[current_element, p_index][3]).Data.Count <= 2)
            {
                return;
            }

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElementOutlineData()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = p_index,
                VariantIndex = 3,
                VertexIndex = v_index,
                IsSubElement = true,
                IsRemoving = true,
            });
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                step_into(textBox1, 2029);
            }
        }

        private void ListPolygons_update()
        {
            ListPolygons.Items.Clear();

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                ListPolygons.Items.Add((i + 1).ToString());
            }
        }

        private void ListPolygons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListPolygons.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            listBox1_update();
            textBox5.Text = variant_repr(ListPolygons.SelectedIndex, 2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == SFEngine.Utility.NO_INDEX)
            {
                p_index = ListPolygons.Items.Count;
            }

            SFCategoryElement elem = category[current_element, 0];

            Byte max_index = 0;
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                max_index = Math.Max(max_index, (Byte)(category[current_element, i][1]));
            }
            max_index += 1;

            SFCategoryElement new_elem = category.GetEmptyElement();
            new_elem[0] = (UInt16)(category[current_element, 0][0]);
            new_elem[1] = (Byte)max_index;
            new_elem[2] = (Byte)1;
            new_elem[3] = new SFOutlineData() { Data = new List<short>() { 0, 0 } };

            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = p_index,
                Element = new_elem,
                IsSubElement = true
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            if (ListPolygons.Items.Count <= 1)
            {
                return;
            }

            Byte cur_spell_index = (Byte)(category[current_element, p_index][1]);

            MainForm.data.op_queue.OpenCluster();
            MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorAddRemoveCategoryElement()
            {
                CategoryIndex = category.category_id,
                ElementIndex = current_element,
                SubElementIndex = p_index,
                IsRemoving = true,
                IsSubElement = true
            });

            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                if ((Byte)(category[current_element, i][1]) > cur_spell_index)
                {
                    MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorModifyCategoryElement()
                    {
                        CategoryIndex = category.category_id,
                        ElementIndex = current_element,
                        SubElementIndex = i,
                        VariantIndex = 1,
                        NewVariant = (Byte)((Byte)(category[current_element, i][1]) - 1),
                        IsSubElement = true
                    });
                }
            }

            MainForm.data.op_queue.CloseCluster();
        }


        public override string get_element_string(int index)
        {
            UInt16 building_id = (UInt16)category[index, 0][0];
            Byte b_index = (Byte)category[index, 0][1];
            string txt_building = SFCategoryManager.GetBuildingName(building_id);
            return building_id.ToString() + " " + txt_building + " [" + b_index.ToString() + "]";
        }

        public override void on_add_subelement(int subelem_index)
        {
            set_element(current_element);
        }

        public override void on_remove_subelement(int subelem_index)
        {
            set_element(current_element);
        }

        public override void on_update_subelement(int subelem_index)
        {
            if (ListPolygons.SelectedIndex != subelem_index)
            {
                return;
            }

            listBox1_update();
        }
    }
}
