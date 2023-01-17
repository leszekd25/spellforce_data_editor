using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.special_forms.utility_forms
{
    public partial class ShowCodeForm : Form
    {
        public ShowCodeForm()
        {
            InitializeComponent();
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void SetText(string t)
        {
            TextBoxCode.Text = t;
        }

        public string GetText()
        {
            return TextBoxCode.Text;
        }
    }
}
