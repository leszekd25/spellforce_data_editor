using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFEffect.effect_controls
{
    public partial class ParticleRotationControl : SpellforceDataEditor.SFEffect.effect_controls.ParticleMovieEffectControl
    {
        public ParticleRotationControl()
        {
            InitializeComponent();
        }

        protected override void ShowData()
        {
            base.ShowData();
            ParticleRotation pr = (ParticleRotation)MovieEffect_ref;
            ComboAxis.SelectedIndex = (int)pr.Axis;
            TextBoxAngle.Text = pr.Angle.ToString();
            TextBoxMin.Text = pr.Min.ToString();
            TextBoxMax.Text = pr.Max.ToString();
        }

        private void ComboAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboAxis.SelectedIndex == -1)
                return;

            ParticleRotation pr = (ParticleRotation)MovieEffect_ref;
            pr.Axis = (RotationMode)ComboAxis.SelectedIndex;
        }

        private void TextBoxAngle_Validated(object sender, EventArgs e)
        {
            ParticleRotation pt = (ParticleRotation)MovieEffect_ref;
            pt.Angle= Utility.TryParseFloat(TextBoxAngle.Text, pt.Angle);
            TextBoxAngle.Text = pt.Angle.ToString();
        }

        private void TextBoxMin_Validated(object sender, EventArgs e)
        {
            ParticleRotation pt = (ParticleRotation)MovieEffect_ref;
            pt.Min = Utility.TryParseFloat(TextBoxMin.Text, pt.Min);
            TextBoxMin.Text = pt.Min.ToString();
        }

        private void TextBoxMax_Validated(object sender, EventArgs e)
        {
            ParticleRotation pt = (ParticleRotation)MovieEffect_ref;
            pt.Max = Utility.TryParseFloat(TextBoxMax.Text, pt.Max);
            TextBoxMax.Text = pt.Max.ToString();
        }
    }
}
