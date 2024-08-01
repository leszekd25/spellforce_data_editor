using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.helper_forms
{
    public partial class LoadGamedataForm : Form
    {
        public enum GDMode { NONE = -1, FULL = 0, MERGE, DIFF }

        public string MainGDFileName = "";
        public string OtherGDFileName = "";

        public GDMode Mode = GDMode.NONE;

        public LoadGamedataForm()
        {
            InitializeComponent();
        }

        private void LoadGamedataForm_Load(object sender, EventArgs e)
        {
            RadioFullGD.Checked = true;
        }

        private void EvaluateResult()
        {
            switch (Mode)
            {
                case GDMode.FULL:
                    ButtonOK.Enabled = (MainGDFileName != "");
                    break;
                case GDMode.DIFF:
                    ButtonOK.Enabled = (MainGDFileName != "") && (OtherGDFileName != "");
                    break;
                case GDMode.MERGE:
                    ButtonOK.Enabled = (MainGDFileName != "") && (OtherGDFileName != "");
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
                case GDMode.DIFF:
                    HideDiffGDMode();
                    break;
                case GDMode.MERGE:
                    HideMergeGDMode();
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

        private void ShowDiffGDMode()
        {
            HideCurrentMode();
            Mode = GDMode.DIFF;

            ButtonMainGD.Visible = true;
            LabelGDMain.Visible = true;
            LabelGDMain.Text = MainGDFileName;

            ButtonOtherGD.Visible = true;
            LabelGDOther.Visible = true;
            LabelGDOther.Text = OtherGDFileName;

            EvaluateResult();
        }

        private void HideDiffGDMode()
        {
            Mode = GDMode.NONE;

            ButtonMainGD.Visible = false;
            LabelGDMain.Text = "";
            LabelGDMain.Visible = false;

            ButtonOtherGD.Visible = false;
            LabelGDOther.Text = "";
            LabelGDOther.Visible = false;
        }

        private void ShowMergeGDMode()
        {
            HideCurrentMode();
            Mode = GDMode.MERGE;

            ButtonMainGD.Visible = true;
            LabelGDMain.Visible = true;
            LabelGDMain.Text = MainGDFileName;

            ButtonOtherGD.Visible = true;
            LabelGDOther.Visible = true;
            LabelGDOther.Text = OtherGDFileName;

            EvaluateResult();
        }

        private void HideMergeGDMode()
        {
            Mode = GDMode.NONE;

            ButtonMainGD.Visible = false;
            LabelGDMain.Text = "";
            LabelGDMain.Visible = false;

            ButtonOtherGD.Visible = false;
            LabelGDOther.Text = "";
            LabelGDOther.Visible = false;
        }

        private void RadioFullGD_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioFullGD.Checked)
            {
                ShowFullGDMode();
            }
        }

        private void RadioDiffGD_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioDiffGD.Checked)
            {
                ShowDiffGDMode();
            }
        }

        private void RadioMergeGD_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioMergeGD.Checked)
            {
                ShowMergeGDMode();
            }
        }

        private void ButtonMainGD_Click(object sender, EventArgs e)
        {
            switch (Mode)
            {
                case GDMode.FULL:
                case GDMode.MERGE:
                case GDMode.DIFF:
                    if (OpenGD.ShowDialog() != DialogResult.OK)
                    {
                        break;
                    }

                    MainGDFileName = OpenGD.FileName;
                    LabelGDMain.Text = MainGDFileName;

                    break;
                default:
                    break;
            }

            EvaluateResult();
        }

        private void ButtonOtherGD_Click(object sender, EventArgs e)
        {
            switch (Mode)
            {
                case GDMode.MERGE:
                case GDMode.DIFF:
                    if (OpenGD.ShowDialog() != DialogResult.OK)
                    {
                        break;
                    }

                    OtherGDFileName = OpenGD.FileName;
                    LabelGDOther.Text = OtherGDFileName;

                    break;
                default:
                    break;
            }

            EvaluateResult();
        }
    }
}
