using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFEffect.effect_controls
{
    public partial class ParticleMovieEffectControl : UserControl
    {
        private ParticleMovieEffect movieeffect_ref;
        public ParticleMovieEffect MovieEffect_ref
        {
            get
            {
                return movieeffect_ref;
            }

            set
            {
                movieeffect_ref = value;
                if (value != null)
                    ShowData();
            }
        }
        public bool Maximized { get; protected set; } = true;
        public int MaxHeight { get; protected set; }
        public ParticleMovieEffectControl()
        {
            InitializeComponent();
        }

        private void ParticleMovieEffectControl_Load(object sender, EventArgs e)
        {
            MaxHeight = this.Height;
        }

        private void ButtonMoveUp_Click(object sender, EventArgs e)
        {
            int modifier_index = this.Parent.Controls.IndexOf(this);
            MainForm.viewer.effect_editor.MoveEffectModifierUp(modifier_index);
        }

        private void ButtonMoveDown_Click(object sender, EventArgs e)
        {
            int modifier_index = this.Parent.Controls.IndexOf(this);
            MainForm.viewer.effect_editor.MoveEffectModifierDown(modifier_index);
        }

        private void ButtonMaximize_Click(object sender, EventArgs e)
        {
            int modifier_index = this.Parent.Controls.IndexOf(this);
            if (Maximized)
            {
                MainForm.viewer.effect_editor.MinimizeEffectModifier(modifier_index);
                ButtonMaximize.Text = "+";
                Maximized = false;
            }
            else
            {
                MainForm.viewer.effect_editor.MaximizeEffectModifier(modifier_index);
                ButtonMaximize.Text = "-";
                Maximized = true;
            }
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            int modifier_index = this.Parent.Controls.IndexOf(this);
            MainForm.viewer.effect_editor.RemoveEffectModifier(modifier_index);
        }

        protected virtual void ShowData()
        {
            TextBoxStart.Text = movieeffect_ref.Start.ToString();
            TextBoxRange.Text = (movieeffect_ref.End - movieeffect_ref.Start).ToString();
            ComboPlay.SelectedIndex = (int)movieeffect_ref.Play;
            ComboDim.SelectedIndex = (int)movieeffect_ref.Dim;
            if(movieeffect_ref.UseCustomPath)
            {
                ComboPath.SelectedIndex = 6;
                ButtonCustomPath.Visible = true;
            }
            else
            {
                ComboPath.SelectedIndex = (int)movieeffect_ref.Path;
                ButtonCustomPath.Visible = false;
            }
            if(movieeffect_ref.UseCustomTrail)
            {
                ButtonCustomTrail.Location = TextBoxTrailConstant.Location;
                ButtonCustomTrail.Visible = true;
                TextBoxTrailConstant.Visible = false;
                ComboTrail.SelectedIndex = 1;
            }
            else
            {
                ButtonCustomTrail.Visible = false;
                TextBoxTrailConstant.Visible = true;
                ComboTrail.SelectedIndex = 0;
            }
        }

        private void TextBoxStart_Validated(object sender, EventArgs e)
        {
            movieeffect_ref.Start = Utility.TryParseFloat(TextBoxStart.Text, movieeffect_ref.Start);
            TextBoxStart.Text = movieeffect_ref.Start.ToString();
        }

        private void TextBoxRange_Validated(object sender, EventArgs e)
        {
            movieeffect_ref.End = movieeffect_ref.Start + Utility.TryParseFloat(TextBoxRange.Text, movieeffect_ref.End - movieeffect_ref.Start);
            TextBoxRange.Text = (movieeffect_ref.End - movieeffect_ref.Start).ToString();
        }

        private void ComboPlay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboPlay.SelectedIndex == -1)
                return;
            movieeffect_ref.Play = (MoviePlayMode)ComboPlay.SelectedIndex;
        }

        private void ComboDim_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboDim.SelectedIndex == -1)
                return;
            movieeffect_ref.Dim = (MovieDimMode)ComboDim.SelectedIndex;
        }

        private void ComboPath_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboPath.SelectedIndex == -1)
                return;
            
            if(ComboPath.SelectedIndex == 6)
            {
                movieeffect_ref.Path = MoviePathMode.kDrwPathLinear;
                movieeffect_ref.UseCustomPath = true;
                if (movieeffect_ref.CustomPath == null)
                    movieeffect_ref.CustomPath = new ParticlePath();
                ButtonCustomPath.Visible = true;
            }
            else
            {
                movieeffect_ref.Path = (MoviePathMode)ComboPath.SelectedIndex;
                movieeffect_ref.UseCustomPath = false;
                ButtonCustomPath.Visible = false;
            }
        }

        private void ButtonCustomPath_Click(object sender, EventArgs e)
        {

        }

        private void ComboTrail_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboTrail.SelectedIndex == -1)
                return;

            if(ComboTrail.SelectedIndex == 0)
            {
                ButtonCustomTrail.Visible = false;
                TextBoxTrailConstant.Visible = true;
                movieeffect_ref.UseCustomTrail = false;
            }
            else
            {
                ButtonCustomTrail.Location = TextBoxTrailConstant.Location;
                ButtonCustomTrail.Visible = true;
                TextBoxTrailConstant.Visible = false;
                movieeffect_ref.UseCustomTrail = true;
                if (movieeffect_ref.CustomTrail == null)
                    movieeffect_ref.CustomTrail = new ParticleTrail();
            }
        }

        private void TextBoxTrailConstant_Validated(object sender, EventArgs e)
        {
            movieeffect_ref.Trail = Utility.TryParseFloat(TextBoxTrailConstant.Text, movieeffect_ref.Trail);
            TextBoxTrailConstant.Text = movieeffect_ref.Trail.ToString();
        }
    }
}
