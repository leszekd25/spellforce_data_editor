using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFEffect.effect_controls
{
    public partial class ParticleSoundControl : SpellforceDataEditor.SFEffect.effect_controls.ParticleMovieEffectControl
    {
        public ParticleSoundControl()
        {
            InitializeComponent();
        }

        protected override void ShowData()
        {
            base.ShowData();
            ParticleSound pr = (ParticleSound)MovieEffect_ref;
            TextBoxSoundName.Text = pr.Name;
            TextBoxLength.Text = pr.Length.ToString();
        }

        private void TextBoxSoundName_Validated(object sender, EventArgs e)
        {
            ParticleSound pr = (ParticleSound)MovieEffect_ref;
            pr.Name = TextBoxSoundName.Text.Trim();
        }

        private void TextBoxLength_Validated(object sender, EventArgs e)
        {
            ParticleSound pr = (ParticleSound)MovieEffect_ref;
            pr.Length = Utility.TryParseFloat(TextBoxLength.Text, pr.Length);
            TextBoxLength.Text = pr.Length.ToString();
        }
    }
}
