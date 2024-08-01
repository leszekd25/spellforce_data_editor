using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.helper_forms
{
    public partial class SaveGamedataForm : Form
    {
        public string MainGDFileName = "";

        public SaveGamedataForm()
        {
            InitializeComponent();
        }

        private void LoadGamedataForm_Load(object sender, EventArgs e)
        {

        }

        private void EvaluateResult()
        {
             ButtonOK.Enabled = (MainGDFileName != "");
        }

        private void ButtonMainGD_Click(object sender, EventArgs e)
        {
            if (SaveGD.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            MainGDFileName = SaveGD.FileName;
            LabelGDMain.Text = MainGDFileName;

            EvaluateResult();
        }
    }
}
