using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFCFF;

namespace SpellforceDataEditor.SFCFF.calculator_forms
{
    public partial class CalcControl : UserControl
    {
        public CalcControl()
        {
            InitializeComponent();
        }

        public virtual float[] GetCalcResult()
        {
            return new float[] { 0.0f };
        }

        public object GetValue(Control c)
        {
            if (c is TextBox)
                return SFEngine.Utility.TryParseDouble(c.Text);
            else if (c is CheckBox)
                return ((CheckBox)(c)).Checked;
            return null;
        }

        public void StepInto(TextBox tb, int cat_i)
        {
            if (MainForm.data == null)
                return;
            if (!SFCategoryManager.ready)
                return;

            int elem_id = SFEngine.Utility.TryParseInt32(tb.Text);
            SFCategory cat = SFCategoryManager.gamedata[cat_i];
            if (cat == null)
                return;

            int real_elem_id = cat.GetElementIndex(elem_id);
            if (real_elem_id == SFEngine.Utility.NO_INDEX)
                return;
            MainForm.data.Tracer_StepForward(cat_i, real_elem_id);
        }

        public virtual string GetCalcText()
        {
            return "; )";
        }
    }
}
