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
    public partial class CalculatorsForm : Form
    {
        SFCFF.calculator_forms.CalcControl calc = null;

        public CalculatorsForm()
        {
            InitializeComponent();
        }

        private void Calculate()
        {
            TextResult.Text = calc.GetCalcText();
        }

        private void CalculatorsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (calc == null)
                    return;
                Calculate();
            }
        }

        private void ComboCalcMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboCalcMode.SelectedIndex == -1)
                return;
            if (calc != null)
                calc.Dispose();
            calc = null;
            switch(ComboCalcMode.SelectedIndex)
            {
                case 0:   // hit chance calc
                    calc = new SFCFF.calculator_forms.CalcHitChanceControl();
                    break;
                default:
                    break;
            }
            if (calc == null)
                return;
            CalcPanel.Controls.Clear();
            CalcPanel.Controls.Add(calc);
        }

        private void ButtonCalculate_Click(object sender, EventArgs e)
        {
            if (calc == null)
                return;
            Calculate();
        }
    }
}
