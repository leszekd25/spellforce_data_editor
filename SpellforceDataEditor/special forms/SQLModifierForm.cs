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

            SFLua.SFLuaEnvironment.Init();
        }

        private void ButtonRtsCoopSpawnGroups_Click(object sender, EventArgs e)
        {
            SFLua.SFLuaEnvironment.ShowRtsCoopSpawnGroupsForm();
        }
    }
}
