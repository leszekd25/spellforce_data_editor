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
    public partial class ExtractionSettingsForm : Form
    {
        public ExtractionSettingsForm()
        {
            InitializeComponent();
        }

        private void ExtractionSettingsForm_Load(object sender, EventArgs e)
        {
            TextBoxExtractionDirectory.Text = Settings.ExtractDirectory;
            if (Settings.ExtractAllInOne)
                AllInOne.Checked = true;
            else
                Subdirectories.Checked = true;
        }

        private void AllInOne_CheckedChanged(object sender, EventArgs e)
        {
            if(AllInOne.Checked)
            {
                Settings.ExtractAllInOne = true;
                DescriptionExtractionMode.Text = "Using \"Extract\" option will extract all relevant assets to the directory specified above.";
            }
        }

        private void Subdirectories_CheckedChanged(object sender, EventArgs e)
        {
            if (Subdirectories.Checked)
            {
                Settings.ExtractAllInOne = false;
                DescriptionExtractionMode.Text = "Using \"Extract\" option will extract all relevant assets to respective subdirectories in the directory specified above.";
            }
        }

        private void ButtonSelectExtractionDirectory_Click(object sender, EventArgs e)
        {
            if(SelectExtractionDirectory.ShowDialog() == DialogResult.OK)
            {
                Settings.ExtractDirectory = SelectExtractionDirectory.SelectedPath;
                TextBoxExtractionDirectory.Text = Settings.ExtractDirectory;
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
