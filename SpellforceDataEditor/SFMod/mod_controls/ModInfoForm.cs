using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMod.mod_controls
{
    public partial class ModInfoForm : Form
    {
        public SFModInfo mod_info;

        public ModInfoForm()
        {
            InitializeComponent();
            mod_info.Name = "";
            mod_info.Description = "";
            mod_info.Revision = 1;
            mod_info.Author = "";
        }

        private void ModInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mod_info.Name = textBox1.Text;
            mod_info.Author = textBox2.Text;
            mod_info.Revision = Utility.TryParseInt32(textBox3.Text);
            mod_info.Description = textBox4.Text;
        }
    }
}
