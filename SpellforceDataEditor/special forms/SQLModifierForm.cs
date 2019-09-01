using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.special_forms
{
    public partial class SQLModifierForm : Form
    {
        public SQLModifierForm()
        {
            InitializeComponent();
        }

        private void ButtonRtsCoopSpawnGroups_Click(object sender, EventArgs e)
        {
            SFLua.SFLuaEnvironment.ShowRtsCoopSpawnGroupsForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SFLua.SFLuaEnvironment.ShowSQLItemForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SFLua.SFLuaEnvironment.ShowSQLObjectForm();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SFLua.SFLuaEnvironment.ShowSQLBuildingForm();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SFLua.SFLuaEnvironment.ShowSQLHeadForm();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SFLua.lua_controls.LuaDecompilerForm f = new SFLua.lua_controls.LuaDecompilerForm();
            f.ShowDialog();
        }
    }
}
