using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    public partial class SFLuaSQLHeadForm : Form
    {
        bool can_load = true;

        public SFLuaSQLHeadForm()
        {
            InitializeComponent();
        }

        private void SFLuaSQLHeadForm_Load(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.heads.Load() != 0)
            {
                MessageBox.Show("Could not load script/sql_head.lua");
                can_load = false;
                this.Close();
                return;
            }

            ReloadHeadList();
        }

        private void ReloadHeadList()
        {
            ListHeads.Items.Clear();
            ListHeads.SuspendLayout();

            for(int i=0;i<SFLuaEnvironment.heads.heads.Count; i++)
                ListHeads.Items.Add((i+1).ToString() + ". ");

            ListHeads.ResumeLayout();
        }

        private void ListHeads_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListHeads.SelectedIndex == Utility.NO_INDEX)
            {
                MM.Text = "";
                MF.Text = "";
                return;
            }

            int head_id = ListHeads.SelectedIndex + 1;

            var heads = SFLuaEnvironment.heads.heads;

            MM.Text = heads[head_id].MeshMale;
            MF.Text = heads[head_id].MeshFemale;
        }

        private void MM_Validated(object sender, EventArgs e)
        {
            if (ListHeads.SelectedIndex == Utility.NO_INDEX)
                return;

            SFLuaEnvironment.heads.heads[ListHeads.SelectedIndex+1].MeshMale = MM.Text.ToString();
        }

        private void MF_Validated(object sender, EventArgs e)
        {
            if (ListHeads.SelectedIndex == Utility.NO_INDEX)
                return;

            SFLuaEnvironment.heads.heads[ListHeads.SelectedIndex+1].MeshFemale = MF.Text.ToString();
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            lua_sql.SFLuaSQLHeadData head = new lua_sql.SFLuaSQLHeadData();
            head.MeshMale = "";
            head.MeshFemale = "";

            int new_id = SFLuaEnvironment.heads.heads.Count + 1;
            SFLuaEnvironment.heads.heads.Add(new_id, head);
            ListHeads.Items.Add(new_id.ToString()+". ");

            ListHeads.SelectedIndex = new_id - 1;
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            if (ListHeads.SelectedIndex == Utility.NO_INDEX)
                return;

            int old_index = ListHeads.SelectedIndex;
            SFLuaEnvironment.heads.heads.Remove(ListHeads.SelectedIndex+1);
            ListHeads.Items.RemoveAt(ListHeads.SelectedIndex);

            if (old_index == SFLuaEnvironment.heads.heads.Count)
                old_index -= 1;

            ListHeads.SelectedIndex = old_index;
        }

        private void ButtonCancelChanges_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonSaveChanges_Click(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.heads.Save() != 0)
            {
                MessageBox.Show("Error while saving sql_head.lua");
            }
            this.Close();
        }
    }
}
