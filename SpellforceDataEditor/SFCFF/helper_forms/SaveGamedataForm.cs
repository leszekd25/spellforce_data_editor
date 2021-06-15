using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.helper_forms
{
    public partial class SaveGamedataForm : Form
    {
        public enum GDMode { NONE = -1, FULL = 0, DEPENDENCY }

        public string MainGDFileName = "";

        public GDMode Mode = GDMode.NONE;

        public SaveGamedataForm()
        {
            InitializeComponent();
        }

        private void LoadGamedataForm_Load(object sender, EventArgs e)
        {
            RadioFullGD.Checked = true;
        }

        private void EvaluateResult()
        {
            switch(Mode)
            {
                case GDMode.FULL:
                    ButtonOK.Enabled = (MainGDFileName != "");
                    break;
                case GDMode.DEPENDENCY:
                    ButtonOK.Enabled = (MainGDFileName != "");
                    break;
                default:
                    ButtonOK.Enabled = false;
                    break;
            }
        }

        private void HideCurrentMode()
        {
            switch (Mode)
            {
                case GDMode.FULL:
                    HideFullGDMode();
                    break;
                case GDMode.DEPENDENCY:
                    HideDependencyGDMode();
                    break;
                default:
                    break;
            }
        }

        private void ShowFullGDMode()
        {
            HideCurrentMode();
            Mode = GDMode.FULL;

            ButtonMainGD.Visible = true;
            LabelGDMain.Visible = true;
            LabelGDMain.Text = MainGDFileName;

            EvaluateResult();
        }

        private void HideFullGDMode()
        {
            Mode = GDMode.NONE;

            ButtonMainGD.Visible = false;
            LabelGDMain.Text = "";
            LabelGDMain.Visible = false;
        }

        private void ShowDependencyGDMode()
        {
            HideCurrentMode();
            Mode = GDMode.DEPENDENCY;

            ButtonMainGD.Visible = true;
            LabelGDMain.Visible = true;
            LabelGDMain.Text = MainGDFileName;

            EvaluateResult();
        }

        private void HideDependencyGDMode()
        {
            Mode = GDMode.NONE;

            ButtonMainGD.Visible = false;
            LabelGDMain.Text = "";
            LabelGDMain.Visible = false;
        }

        private void RadioFullGD_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioFullGD.Checked)
                ShowFullGDMode();
        }

        private void RadioDependencyGD_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioDependencyGD.Checked)
                ShowDependencyGDMode();
        }

        private void ButtonMainGD_Click(object sender, EventArgs e)
        {
            switch(Mode)
            {
                case GDMode.FULL:
                case GDMode.DEPENDENCY:
                    if (SaveGD.ShowDialog() != DialogResult.OK)
                        break;

                    MainGDFileName = SaveGD.FileName;
                    LabelGDMain.Text = MainGDFileName;

                    break;
                default:
                    break;
            }

            EvaluateResult();
        }
    }
}
