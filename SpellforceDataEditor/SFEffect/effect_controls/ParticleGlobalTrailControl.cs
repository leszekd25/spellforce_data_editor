using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFEffect.effect_controls
{
    public partial class ParticleGlobalTrailControl : SpellforceDataEditor.SFEffect.effect_controls.ParticleMovieEffectControl
    {
        public ParticleGlobalTrailControl()
        {
            InitializeComponent();
        }

        protected override void ShowData()
        {
            base.ShowData();
            ParticleGlobalTrail pn = (ParticleGlobalTrail)MovieEffect_ref;
            TextBoxBuffer.Text = pn.Buffer.ToString();
        }

        private void TextBoxBuffer_Validated(object sender, EventArgs e)
        {
            ParticleGlobalTrail pn = (ParticleGlobalTrail)MovieEffect_ref;
            pn.Buffer = Utility.TryParseFloat(TextBoxBuffer.Text, pn.Buffer);
            TextBoxBuffer.Text = pn.Buffer.ToString();
        }
    }
}
