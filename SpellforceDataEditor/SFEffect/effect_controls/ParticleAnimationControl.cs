using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFEffect.effect_controls
{
    public partial class ParticleAnimationControl : SpellforceDataEditor.SFEffect.effect_controls.ParticleMovieEffectControl
    {
        public ParticleAnimationControl()
        {
            InitializeComponent();
        }

        protected override void ShowData()
        {
            base.ShowData();
            ParticleAnimation pr = (ParticleAnimation)MovieEffect_ref;
            TextBoxAnimation.Text = pr.Animation;
        }

        private void TextBoxAnimation_Validated(object sender, EventArgs e)
        {
            ParticleAnimation pr = (ParticleAnimation)MovieEffect_ref;
            pr.Animation = TextBoxAnimation.Text.Trim();
        }
    }
}
