﻿using SFEngine.SFLua;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    public partial class SFLuaSQLHeadForm : Form
    {
        public SFLuaSQLHeadForm()
        {
            InitializeComponent();
        }

        private void SFLuaSQLHeadForm_Load(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.heads.Load() != 0)
            {
                MessageBox.Show("Could not load script/sql_head.lua");
                Close();
                return;
            }

            ReloadHeadList();
        }

        private void ReloadHeadList()
        {
            ListHeads.Items.Clear();
            ListHeads.SuspendLayout();

            for (int i = 0; i < SFLuaEnvironment.heads.items.Count; i++)
            {
                ListHeads.Items.Add((i + 1).ToString() + ". ");
            }

            ListHeads.ResumeLayout();
        }

        private void ListHeads_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListHeads.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                MM.Text = "";
                MF.Text = "";
                return;
            }

            int head_id = ListHeads.SelectedIndex + 1;

            var heads = SFLuaEnvironment.heads.items;

            MM.Text = heads[head_id].MeshMale;
            MF.Text = heads[head_id].MeshFemale;
        }

        private void MM_Validated(object sender, EventArgs e)
        {
            if (ListHeads.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFLuaEnvironment.heads.items[ListHeads.SelectedIndex + 1].MeshMale = MM.Text.ToString();
        }

        private void MF_Validated(object sender, EventArgs e)
        {
            if (ListHeads.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            SFLuaEnvironment.heads.items[ListHeads.SelectedIndex + 1].MeshFemale = MF.Text.ToString();
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            SFEngine.SFLua.lua_sql.SFLuaSQLHeadData head = new SFEngine.SFLua.lua_sql.SFLuaSQLHeadData();
            head.MeshMale = "";
            head.MeshFemale = "";

            int new_id = SFLuaEnvironment.heads.items.Count + 1;
            SFLuaEnvironment.heads.items.Add(new_id, head);
            ListHeads.Items.Add(new_id.ToString() + ". ");

            ListHeads.SelectedIndex = new_id - 1;
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            if (ListHeads.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            int old_index = ListHeads.SelectedIndex;
            SFLuaEnvironment.heads.items.Remove(ListHeads.SelectedIndex + 1);
            ListHeads.Items.RemoveAt(ListHeads.SelectedIndex);

            if (old_index == SFLuaEnvironment.heads.items.Count)
            {
                old_index -= 1;
            }

            ListHeads.SelectedIndex = old_index;
        }

        private void ButtonCancelChanges_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonSaveChanges_Click(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.heads.Save() != 0)
            {
                MessageBox.Show("Error while saving sql_head.lua");
            }
            Close();
        }
    }
}
