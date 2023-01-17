using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.special_forms.utility_forms
{
    public partial class GetStringForm : Form
    {
        public string ResultString { get; set; } = "";
        public DialogResult Result { get; private set; }

        public GetStringForm()
        {
            InitializeComponent();
        }

        public void SetDescription(string title, string label, string default_str)
        {
            Text = title;
            LabelInput.Text = label;
            TextBoxString.Text = default_str;
        }

        private void TextBoxString_TextChanged(object sender, EventArgs e)
        {
            ResultString = TextBoxString.Text;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Result = DialogResult.OK;
            Close();
        }
    }
}
