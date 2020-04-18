using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFEffect.effect_controls
{
    public partial class ParticleTrailControl : SpellforceDataEditor.SFEffect.effect_controls.ParticleMovieEffectControl
    {
        public ParticleTrailControl()
        {
            InitializeComponent();
        }

        protected override void ShowData()
        {
            base.ShowData();
            ParticleTrail pt = (ParticleTrail)MovieEffect_ref;
            TextBoxMin.Text = pt.Min.ToString();
            TextBoxMax.Text = pt.Max.ToString();
        }

        private void TextBoxMin_Validated(object sender, EventArgs e)
        {
            ParticleTrail pt = (ParticleTrail)MovieEffect_ref;
            pt.Min = Utility.TryParseFloat(TextBoxMin.Text, pt.Min);
            TextBoxMin.Text = pt.Min.ToString();
        }

        private void TextBoxMax_Validated(object sender, EventArgs e)
        {
            ParticleTrail pt = (ParticleTrail)MovieEffect_ref;
            pt.Max = Utility.TryParseFloat(TextBoxMax.Text, pt.Max);
            TextBoxMax.Text = pt.Max.ToString();
        }
    }
}
