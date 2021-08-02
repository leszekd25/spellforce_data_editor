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
    public partial class LoadGamedataForm : Form
    {
        public enum GDMode { NONE = -1, FULL = 0, DEPENDENCY, DIFF, MERGE }

        public string MainGDFileName = "";
        public string DiffGDFileName = "";
        public List<string> DependencyGDFileNames = new List<string>();
        public List<string> MergeGDFileNames = new List<string>();

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
            switch(Mode)
            {
                case GDMode.FULL:
                    ButtonOK.Enabled = (MainGDFileName != "");
                    break;
                case GDMode.DEPENDENCY:
                    ButtonOK.Enabled = (MainGDFileName != "") && (DependencyGDFileNames.Count != 0);
                    break;
                case GDMode.DIFF:
                    ButtonOK.Enabled = (MainGDFileName != "") && (DiffGDFileName != "");
                    break;
                case GDMode.MERGE:
                    ButtonOK.Enabled = (MergeGDFileNames.Count > 1);
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

        private void ShowDependencyGDMode()
        {
            HideCurrentMode();
            Mode = GDMode.DEPENDENCY;

            ButtonMainGD.Visible = true;
            LabelGDMain.Visible = true;
            LabelGDMain.Text = MainGDFileName;
            ListboxDependencyGD.Visible = true;
            ButtonAddDependencyGD.Visible = true;
            ButtonAddDependencyGD.Text = "Add dependency";
            ButtonRemoveDependencyGD.Visible = true;
            ButtonRemoveDependencyGD.Text = "Remove dependency";
            ButtonMoveUpDependencyGD.Visible = true;
            ButtonMoveDownDependencyGD.Visible = true;

            foreach (var s in DependencyGDFileNames)
                ListboxDependencyGD.Items.Add(s);

            EvaluateResult();
        }

        private void HideDependencyGDMode()
        {
            Mode = GDMode.NONE;

            ListboxDependencyGD.Items.Clear();

            ButtonMainGD.Visible = false;
            LabelGDMain.Text = "";
            LabelGDMain.Visible = false;
            ListboxDependencyGD.Visible = false;
            ButtonAddDependencyGD.Visible = false;
            ButtonRemoveDependencyGD.Visible = false;
            ButtonMoveUpDependencyGD.Visible = false;
            ButtonMoveDownDependencyGD.Visible = false;
        }

        private void ShowDiffGDMode()
        {
            HideCurrentMode();
            Mode = GDMode.DIFF;

            ButtonMainGD.Visible = true;
            LabelGDMain.Visible = true;
            LabelGDMain.Text = MainGDFileName;

            ButtonDiffGD.Visible = true;
            LabelGDDiff.Visible = true;
            LabelGDDiff.Text = DiffGDFileName;

            ListboxDependencyGD.Visible = true;
            ButtonAddDependencyGD.Visible = true;
            ButtonAddDependencyGD.Text = "Add dependency";
            ButtonRemoveDependencyGD.Visible = true;
            ButtonRemoveDependencyGD.Text = "Remove dependency";
            ButtonMoveUpDependencyGD.Visible = true;
            ButtonMoveDownDependencyGD.Visible = true;

            foreach (var s in DependencyGDFileNames)
                ListboxDependencyGD.Items.Add(s);

            EvaluateResult();
        }

        private void HideDiffGDMode()
        {
            Mode = GDMode.NONE;

            ButtonMainGD.Visible = false;
            LabelGDMain.Text = "";
            LabelGDMain.Visible = false;

            ButtonDiffGD.Visible = false;
            LabelGDDiff.Text = "";
            LabelGDDiff.Visible = false;

            ListboxDependencyGD.Visible = false;
            ButtonAddDependencyGD.Visible = false;
            ButtonRemoveDependencyGD.Visible = false;
            ButtonMoveUpDependencyGD.Visible = false;
            ButtonMoveDownDependencyGD.Visible = false;
        }

        private void ShowMergeGDMode()
        {
            HideCurrentMode();
            Mode = GDMode.MERGE;

            ListboxDependencyGD.Visible = true;
            ButtonAddDependencyGD.Visible = true;
            ButtonAddDependencyGD.Text = "Add gamedata";
            ButtonRemoveDependencyGD.Visible = true;
            ButtonRemoveDependencyGD.Text = "Remove gamedata";
            ButtonMoveUpDependencyGD.Visible = true;
            ButtonMoveDownDependencyGD.Visible = true;

            foreach (var s in MergeGDFileNames)
                ListboxDependencyGD.Items.Add(s);

            EvaluateResult();
        }

        private void HideMergeGDMode()
        {
            Mode = GDMode.NONE;

            ListboxDependencyGD.Items.Clear();

            ListboxDependencyGD.Visible = false;
            ButtonAddDependencyGD.Visible = false;
            ButtonRemoveDependencyGD.Visible = false;
            ButtonMoveUpDependencyGD.Visible = false;
            ButtonMoveDownDependencyGD.Visible = false;
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

        private void RadioDiffGD_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioDiffGD.Checked)
                ShowDiffGDMode();
        }

        private void RadioMergeGD_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioMergeGD.Checked)
                ShowMergeGDMode();
        }

        private void ButtonMainGD_Click(object sender, EventArgs e)
        {
            switch(Mode)
            {
                case GDMode.FULL:
                case GDMode.DEPENDENCY:
                case GDMode.DIFF:
                    if (OpenGD.ShowDialog() != DialogResult.OK)
                        break;

                    if (OpenGD.FileNames.Length != 1)
                        break;

                    MainGDFileName = OpenGD.FileNames[0];
                    LabelGDMain.Text = MainGDFileName;

                    break;
                default:
                    break;
            }

            EvaluateResult();
        }

        private void ButtonDiffGD_Click(object sender, EventArgs e)
        {
            switch (Mode)
            {
                case GDMode.DIFF:
                    if (OpenGD.ShowDialog() != DialogResult.OK)
                        break;

                    if (OpenGD.FileNames.Length != 1)
                        break;

                    DiffGDFileName = OpenGD.FileNames[0];
                    LabelGDDiff.Text = DiffGDFileName;

                    break;
                default:
                    break;
            }

            EvaluateResult();
        }

        private void ButtonAddDependencyGD_Click(object sender, EventArgs e)
        {
            List<string> ref_list;
            switch(Mode)
            {
                case GDMode.DEPENDENCY:
                case GDMode.DIFF:
                    ref_list = DependencyGDFileNames;
                    break;
                case GDMode.MERGE:
                    ref_list = MergeGDFileNames;
                    break;
                default:
                    return;
            }

            if (OpenGD.ShowDialog() != DialogResult.OK)
                return;

            if (OpenGD.FileNames.Length < 1)
                return;

            foreach(var s in OpenGD.FileNames)
            {
                ref_list.Add(s);
                ListboxDependencyGD.Items.Add(s);
            }

            EvaluateResult();
        }

        private void ButtonRemoveDependencyGD_Click(object sender, EventArgs e)
        {
            List<string> ref_list;
            switch (Mode)
            {
                case GDMode.DEPENDENCY:
                case GDMode.DIFF:
                    ref_list = DependencyGDFileNames;
                    break;
                case GDMode.MERGE:
                    ref_list = MergeGDFileNames;
                    break;
                default:
                    return;
            }

            int index = ListboxDependencyGD.SelectedIndex;

            if (index == Utility.NO_INDEX)
                return;

            ref_list.RemoveAt(index);
            ListboxDependencyGD.Items.RemoveAt(index);
            if (index == ListboxDependencyGD.Items.Count)
                index -= 1;
            ListboxDependencyGD.SelectedIndex = index;

            EvaluateResult();
        }

        private void ButtonMoveUpDependencyGD_Click(object sender, EventArgs e)
        {
            List<string> ref_list;
            switch (Mode)
            {
                case GDMode.DEPENDENCY:
                case GDMode.DIFF:
                    ref_list = DependencyGDFileNames;
                    break;
                case GDMode.MERGE:
                    ref_list = MergeGDFileNames;
                    break;
                default:
                    return;
            }

            int index = ListboxDependencyGD.SelectedIndex;

            if (index == Utility.NO_INDEX)
                return;
            if (index == 0)
                return;

            string elem1 = ref_list[index];
            string elem2 = ref_list[index - 1];
            ref_list.RemoveAt(index - 1);
            ref_list.RemoveAt(index - 1);
            ref_list.Insert(index - 1, elem1);
            ref_list.Insert(index, elem2);

            ListboxDependencyGD.Items.Clear();
            foreach (var s in ref_list)
                ListboxDependencyGD.Items.Add(s);

            ListboxDependencyGD.SelectedIndex = index - 1;

            EvaluateResult();
        }

        private void ButtonMoveDownDependencyGD_Click(object sender, EventArgs e)
        {
            List<string> ref_list;
            switch (Mode)
            {
                case GDMode.DEPENDENCY:
                case GDMode.DIFF:
                    ref_list = DependencyGDFileNames;
                    break;
                case GDMode.MERGE:
                    ref_list = MergeGDFileNames;
                    break;
                default:
                    return;
            }

            int index = ListboxDependencyGD.SelectedIndex;

            if (index == Utility.NO_INDEX)
                return;
            if (index == ref_list.Count - 1)
                return;

            string elem1 = ref_list[index];
            string elem2 = ref_list[index + 1];
            ref_list.RemoveAt(index);
            ref_list.RemoveAt(index);
            ref_list.Insert(index, elem2);
            ref_list.Insert(index + 1, elem1);

            ListboxDependencyGD.Items.Clear();
            foreach (var s in ref_list)
                ListboxDependencyGD.Items.Add(s);

            ListboxDependencyGD.SelectedIndex = index + 1;

            EvaluateResult();
        }
    }
}
