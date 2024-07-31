using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control35 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        private int vertex_index;

        Category2057 c2057;

        public Control35()
        {
            InitializeComponent();

            c2057 = SFCategoryManager.gamedata.c2057;
            category = c2057;
            c2057.SetOnVertexAdded(on_add_vertex);
            c2057.SetOnVertexModified(on_modify_vertex);
            c2057.SetOnVertexRemoved(on_remove_vertex);

            column_dict.Add("Object ID", "ObjectID");
            column_dict.Add("Polygon index", "PolygonID");
            column_dict.Add("Casts shadow", "CastsShadow");
            vertex_index = -1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2057.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (ListPolygons.SelectedIndex == -1)
            {
                return;
            }

            c2057.SetField(current_element, ListPolygons.SelectedIndex, "CastsShadow", SFEngine.Utility.TryParseUInt8(textBox5.Text));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            vertex_index = listBox1.SelectedIndex;
            if (vertex_index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            textBox3.Text = c2057[current_element, ListPolygons.SelectedIndex].Coords[vertex_index * 2 + 0].ToString();
            textBox4.Text = c2057[current_element, ListPolygons.SelectedIndex].Coords[vertex_index * 2 + 1].ToString();
        }

        private void listbox1_update_vertex(int v_ind)
        {
            if ((v_ind < 0) || (v_ind >= listBox1.Items.Count))
            {
                return;
            }

            listBox1.Items[v_ind] = $"{c2057[current_element, ListPolygons.SelectedIndex].Coords[v_ind * 2 + 0]} | {c2057[current_element, ListPolygons.SelectedIndex].Coords[v_ind * 2 + 1]}";
        }

        private void listBox1_update()
        {
            if (ListPolygons.SelectedIndex == -1)
            {
                return;
            }

            listBox1.Items.Clear();

            for (int i = 0; i < c2057[current_element, ListPolygons.SelectedIndex].Coords.Count / 2; i++)
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
            short new_y = c2057[current_element, ListPolygons.SelectedIndex].Coords[vertex_index * 2 + 1];
            if (new_x == c2057[current_element, ListPolygons.SelectedIndex].Coords[vertex_index * 2 + 0])
            {
                return;
            }

            c2057.SetCoord(current_element, ListPolygons.SelectedIndex, vertex_index, new_x, new_y);

            listBox1.SelectedIndex = vertex_index;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (vertex_index == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            short new_x = c2057[current_element, ListPolygons.SelectedIndex].Coords[vertex_index * 2 + 0];
            short new_y = SFEngine.Utility.TryParseInt16(textBox4.Text);
            if (new_y == c2057[current_element, ListPolygons.SelectedIndex].Coords[vertex_index * 2 + 1])
            {
                return;
            }

            c2057.SetCoord(current_element, ListPolygons.SelectedIndex, vertex_index, new_x, new_y);

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
            textBox1.Text = c2057[current_element, 0].ObjectID.ToString();
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

            c2057.AddCoord(current_element, p_index, v_index, 0, 0);

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

            if (c2057[current_element, p_index].Coords.Count <= 2)
            {
                return;
            }

            c2057.RemoveCoord(current_element, p_index, v_index);
        }


        private void ListPolygons_update()
        {
            ListPolygons.Items.Clear();

            for (int i = 0; i < c2057.GetItemSubItemNum(current_element); i++)
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
            textBox5.Text = c2057[current_element, ListPolygons.SelectedIndex].CastsShadow.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int p_index = ListPolygons.SelectedIndex;
            if (p_index == SFEngine.Utility.NO_INDEX)
            {
                p_index = ListPolygons.Items.Count;
            }

            Byte max_index = 0;
            for (int i = 0; i < c2057.GetItemSubItemNum(current_element); i++)
            {
                max_index = Math.Max(max_index, c2057[current_element, i].PolygonID);
            }
            max_index += 1;

            c2057.AddSubItem(current_element, p_index, new()
            {
                ObjectID = c2057[current_element, 0].ObjectID,
                PolygonID = max_index,
                CastsShadow = 1,
                Coords = new()
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

            Byte cur_spell_index = c2057[current_element, p_index].PolygonID;

            c2057.RemoveSub(current_element, p_index);

            for (int i = 0; i < c2057.GetItemSubItemNum(current_element); i++)
            {
                if (c2057[current_element, i].PolygonID > cur_spell_index)
                {
                    c2057.SetField(current_element, i, "PolygonID", (byte)(c2057[current_element, i].PolygonID - 1));
                }
            }
        }


        public override string get_element_string(int index)
        {
            return $"{c2057[index, 0].ObjectID} {SFCategoryManager.GetObjectName(c2057[index, 0].ObjectID)} [{c2057.GetItemSubItemNum(index)}]";
        }

        public override void on_add_subelement(int subelem_index)
        {
            set_element(current_element);
            SFCategoryManager.gamedata.c2050.GetItemIndex(c2057[current_element].GetID(), out int object_index);
            SFCategoryManager.gamedata.c2050.SetField(object_index, "PolygonNum", (byte)c2057.GetItemSubItemNum(current_element));
        }

        public override void on_remove_subelement(int subelem_index)
        {
            set_element(current_element);
            SFCategoryManager.gamedata.c2050.GetItemIndex(c2057[current_element].GetID(), out int object_index);
            SFCategoryManager.gamedata.c2050.SetField(object_index, "PolygonNum", (byte)c2057.GetItemSubItemNum(current_element));
        }

        public override void on_update_subelement(int subelem_index)
        {
            if (ListPolygons.SelectedIndex != subelem_index)
            {
                return;
            }

            listBox1_update();
        }

        private void on_add_vertex(int elem_index, int subelem_index, int v_index)
        {
            if (Parent == null)
            {
                return;
            }
            if (elem_index != current_element)
            {
                return;
            }
            if (subelem_index != ListPolygons.SelectedIndex)
            {
                return;
            }

            listBox1.Items.Insert(v_index, "");
            listbox1_update_vertex(v_index);
        }

        private void on_modify_vertex(int elem_index, int subelem_index, int v_index)
        {
            if (Parent == null)
            {
                return;
            }
            if (elem_index != current_element)
            {
                return;
            }
            if (subelem_index != ListPolygons.SelectedIndex)
            {
                return;
            }

            listbox1_update_vertex(v_index);
        }

        private void on_remove_vertex(int elem_index, int subelem_index, int v_index)
        {
            if (Parent == null)
            {
                return;
            }
            if (elem_index != current_element)
            {
                return;
            }
            if (subelem_index != ListPolygons.SelectedIndex)
            {
                return;
            }

            listBox1.Items.RemoveAt(v_index);
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2050, textBox1.Text);
        }
    }
}
