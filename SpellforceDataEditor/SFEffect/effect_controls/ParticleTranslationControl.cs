using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFEffect.effect_controls
{
    public partial class ParticleTranslationControl : SpellforceDataEditor.SFEffect.effect_controls.ParticleMovieEffectControl
    {
        public ParticleTranslationControl()
        {
            InitializeComponent();
        }

        protected override void ShowData()
        {
            base.ShowData();
            ParticleTranslation pt = (ParticleTranslation)MovieEffect_ref;
            TextBoxMinX.Text = pt.Min.X.ToString();
            TextBoxMinY.Text = pt.Min.Y.ToString();
            TextBoxMinZ.Text = pt.Min.Z.ToString();
            TextBoxMaxX.Text = pt.Max.X.ToString();
            TextBoxMaxY.Text = pt.Max.Y.ToString();
            TextBoxMaxZ.Text = pt.Max.Z.ToString();
        }

        private void TextBoxMinX_Validated(object sender, EventArgs e)
        {
            ParticleTranslation pt = (ParticleTranslation)MovieEffect_ref;
            pt.Min.X = Utility.TryParseFloat(TextBoxMinX.Text, pt.Min.X);
            TextBoxMinX.Text = pt.Min.X.ToString();
        }

        private void TextBoxMinY_Validated(object sender, EventArgs e)
        {
            ParticleTranslation pt = (ParticleTranslation)MovieEffect_ref;
            pt.Min.Y = Utility.TryParseFloat(TextBoxMinY.Text, pt.Min.Y);
            TextBoxMinY.Text = pt.Min.Y.ToString();
        }

        private void TextBoxMinZ_Validated(object sender, EventArgs e)
        {
            ParticleTranslation pt = (ParticleTranslation)MovieEffect_ref;
            pt.Min.Z = Utility.TryParseFloat(TextBoxMinZ.Text, pt.Min.Z);
            TextBoxMinZ.Text = pt.Min.Z.ToString();
        }

        private void TextBoxMaxX_Validated(object sender, EventArgs e)
        {
            ParticleTranslation pt = (ParticleTranslation)MovieEffect_ref;
            pt.Max.X = Utility.TryParseFloat(TextBoxMaxX.Text, pt.Max.X);
            TextBoxMaxX.Text = pt.Max.X.ToString();
        }

        private void TextBoxMaxY_Validated(object sender, EventArgs e)
        {
            ParticleTranslation pt = (ParticleTranslation)MovieEffect_ref;
            pt.Max.Y = Utility.TryParseFloat(TextBoxMaxY.Text, pt.Max.Y);
            TextBoxMaxY.Text = pt.Max.Y.ToString();
        }

        private void TextBoxMaxZ_Validated(object sender, EventArgs e)
        {
            ParticleTranslation pt = (ParticleTranslation)MovieEffect_ref;
            pt.Max.Z = Utility.TryParseFloat(TextBoxMaxZ.Text, pt.Max.Z);
            TextBoxMaxZ.Text = pt.Max.Z.ToString();
        }
    }
}
