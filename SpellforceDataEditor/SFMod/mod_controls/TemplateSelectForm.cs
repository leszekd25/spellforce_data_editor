using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

namespace SpellforceDataEditor.SFMod.mod_controls
{
    public partial class TemplateSelectForm : Form
    {
        public string result = "";

        public TemplateSelectForm()
        {
            InitializeComponent();

            List<string> files = Directory.EnumerateFiles("modtemplates", "*.tmpl").ToList();

            foreach (string fn in files)
                listBox1.Items.Add(Path.GetFileNameWithoutExtension(fn));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            result = listBox1.SelectedItem.ToString();
        }
    }
}
