using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.special_forms.utility_forms
{
    public partial class SelectLuaEnumForm : Form
    {
        public Type EnumType { get; set; } = null;
        public DialogResult Result { get; private set; }

        public SelectLuaEnumForm()
        {
            InitializeComponent();

            //fill listbox with enums
            foreach (Type t in SFLua.LuaEnumUtility.lua_enums)
                ListEnums.Items.Add(t.Name);
        }

        private void ListEnums_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnumType = (ListEnums.SelectedIndex == -1?
                        null:
                        SFLua.LuaEnumUtility.lua_enums.Find(t => t.Name == ListEnums.SelectedItem.ToString()));

            ButtonAccept.Enabled = (EnumType != null);
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }

        private void ButtonAccept_Click(object sender, EventArgs e)
        {
            Result = DialogResult.OK;
            Close();
        }

    }
}
