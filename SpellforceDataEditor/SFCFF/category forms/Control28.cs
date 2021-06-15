﻿using System;
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
    public partial class Control28 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        public Control28()
        {
            InitializeComponent();
            column_dict.Add("Skill ID", new int[1] { 0 });
            column_dict.Add("Skill level", new int[1] { 1 });
            column_dict.Add("Strength", new int[1] { 2 });
            column_dict.Add("Stamina", new int[1] { 3 });
            column_dict.Add("Agility", new int[1] { 4 });
            column_dict.Add("Dexterity", new int[1] { 5 });
            column_dict.Add("Chrisma", new int[1] { 6 });
            column_dict.Add("Intelligence", new int[1] { 7 });
            column_dict.Add("Wisdom", new int[1] { 8 });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
                set_element_variant(current_element, i, 0, Utility.TryParseUInt8(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex, 2, Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex, 3, Utility.TryParseUInt8(textBox5.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex, 4, Utility.TryParseUInt8(textBox4.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex, 5, Utility.TryParseUInt8(textBox7.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex, 6, Utility.TryParseUInt8(textBox6.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex, 7, Utility.TryParseUInt8(textBox9.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            set_element_variant(current_element, ListLevels.SelectedIndex, 8, Utility.TryParseUInt8(textBox8.Text));
        }

        public override void set_element(int index)
        {
            current_element = index;

            ListLevels.Items.Clear();

            for(int i = 0; i < category.element_lists[current_element].Elements.Count; i++)
            {
                ListLevels.Items.Add("Level " + (i + 1).ToString());
            }

            ListLevels.SelectedIndex = 0;
        }

        public override void show_element()
        {
            textBox1.Text = variant_repr(0, 0);
        }

        private void ListLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListLevels.SelectedIndex == Utility.NO_INDEX)
                return;

            int index = ListLevels.SelectedIndex;
            textBox3.Text = variant_repr(index, 2);
            textBox5.Text = variant_repr(index, 3);
            textBox4.Text = variant_repr(index, 4);
            textBox7.Text = variant_repr(index, 5);
            textBox6.Text = variant_repr(index, 6);
            textBox9.Text = variant_repr(index, 7);
            textBox8.Text = variant_repr(index, 8);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SFCategoryElement elem = category[current_element, 0];
            int index = ListLevels.Items.Count;

            category.element_lists[current_element].Elements.Insert(index, category.GetEmptyElement());
            category[current_element, index][0] = (Byte)elem[0];
            category[current_element, index][1] = (Byte)(index + 1);

            ListLevels.Items.Add("Level " + (index + 1).ToString());
            ListLevels.SelectedIndex = index;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (category.element_lists[current_element].Elements.Count == 1)
                return;

            int index = ListLevels.Items.Count - 1;

            category.element_lists[current_element].Elements.RemoveAt(index);
            ListLevels.Items.RemoveAt(index);

            ListLevels.SelectedIndex = Math.Min(index, ListLevels.Items.Count - 1);
        }


        public override string get_element_string(int index)
        {
            Byte skill_major = (Byte)category[index, 0][0];
            Byte skill_level = (Byte)category[index, 0][1];
            string txt_skill = SFCategoryManager.GetSkillName(skill_major, 101, skill_level);
            return txt_skill;
        }
    }
}
