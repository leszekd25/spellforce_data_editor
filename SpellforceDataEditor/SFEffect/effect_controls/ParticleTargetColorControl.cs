using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFEffect.effect_controls
{
    public partial class ParticleTargetColorControl : SpellforceDataEditor.SFEffect.effect_controls.ParticleMovieEffectControl
    {
        public ParticleTargetColorControl()
        {
            InitializeComponent();
        }

        protected override void ShowData()
        {
            base.ShowData();
            ParticleTargetColor pr = (ParticleTargetColor)MovieEffect_ref;
            ButtonMinColor.BackColor = Color.FromArgb((int)(pr.Min.X * 255), (int)(pr.Min.Y * 255), (int)(pr.Min.Z * 255));
            ButtonMaxColor.BackColor = Color.FromArgb((int)(pr.Max.X * 255), (int)(pr.Max.Y * 255), (int)(pr.Max.Z * 255));
            TrackBarMinAlpha.Value = (int)(pr.Min.W * 255);
            TrackBarMaxAlpha.Value = (int)(pr.Max.W * 255);
        }

        private void ButtonMinColor_Click(object sender, EventArgs e)
        {
            if (ColorPick.ShowDialog() == DialogResult.OK)
            {
                ParticleTargetColor pr = (ParticleTargetColor)MovieEffect_ref;
                pr.Min = new OpenTK.Vector4(ColorPick.Color.R / 255.0f, ColorPick.Color.G / 255.0f, ColorPick.Color.B / 255.0f, pr.Min.W);
            }
        }

        private void ButtonMaxColor_Click(object sender, EventArgs e)
        {
            if (ColorPick.ShowDialog() == DialogResult.OK)
            {
                ParticleTargetColor pr = (ParticleTargetColor)MovieEffect_ref;
                pr.Max = new OpenTK.Vector4(ColorPick.Color.R / 255.0f, ColorPick.Color.G / 255.0f, ColorPick.Color.B / 255.0f, pr.Max.W);
            }
        }

        private void TrackBarMinAlpha_ValueChanged(object sender, EventArgs e)
        {
            ParticleTargetColor pr = (ParticleTargetColor)MovieEffect_ref;
            pr.Min = new OpenTK.Vector4(pr.Min.X, pr.Min.Y, pr.Min.Z, TrackBarMinAlpha.Value / 255.0f);
        }

        private void TrackBarMaxAlpha_ValueChanged(object sender, EventArgs e)
        {
            ParticleTargetColor pr = (ParticleTargetColor)MovieEffect_ref;
            pr.Max = new OpenTK.Vector4(pr.Max.X, pr.Max.Y, pr.Max.Z, TrackBarMaxAlpha.Value / 255.0f);
        }
    }
}
