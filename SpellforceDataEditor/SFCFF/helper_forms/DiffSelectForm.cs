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
    public partial class DiffSelectForm : Form
    {
        public string Gamedata1 = "";
        public string Gamedata2 = "";

        public DiffSelectForm()
        {
            InitializeComponent();
        }

        private void ButtonGD1_Click(object sender, EventArgs e)
        {
            if(GDSelect.ShowDialog() == DialogResult.OK)
            {
                Gamedata1 = GDSelect.FileName;
                LabelGD1Name.Text = Gamedata1;

                if ((Gamedata1 != "") && (Gamedata2 != ""))
                    ButtonOK.Enabled = true;
            }
        }

        private void ButtonGD2_Click(object sender, EventArgs e)
        {
            if (GDSelect.ShowDialog() == DialogResult.OK)
            {
                Gamedata2 = GDSelect.FileName;
                LabelGD2Name.Text = Gamedata2;

                if((Gamedata1 != "")&&(Gamedata2 != ""))
                    ButtonOK.Enabled = true;
            }
        }
    }
}
