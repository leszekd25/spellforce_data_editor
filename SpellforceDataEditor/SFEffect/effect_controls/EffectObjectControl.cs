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
    public partial class EffectObjectControl : UserControl
    {
        private EffectObject effect_ref;
        public EffectObject Effect_ref 
        { 
            get 
            {
                return effect_ref; 
            }

            set
            {
                effect_ref = value;
                if (value != null)
                    ShowData();
            }
        }
        public EffectObjectControl()
        {
            InitializeComponent();
        }

        private void ShowData()
        {
            TextBoxName.Text = effect_ref.Name;
            TextBoxParticleNumber.Text = effect_ref.NumberOfParticles.ToString();
            if (effect_ref.Restriction == EffectRestrictionMode.kDrwCsParent)
                ComboRestriction.SelectedIndex = 0;
            else
                ComboRestriction.SelectedIndex = 1+(int)(Math.Log((int)effect_ref.Restriction, 2));
            TextBoxBoundingRadius.Text = effect_ref.BoundingRadius.ToString();
            TextBoxBoneTarget.Text = effect_ref.BoneTarget.ToString();
            TextBoxBoneSource.Text = effect_ref.BoneSource.ToString();
            ComboShadow.SelectedIndex = (effect_ref.Shadow ? 1 : 0);
            string s1 = "";
            foreach (string s in effect_ref.Billboards)
                s1 += s + "; ";
            TextBoxBillboards.Text = s1;
            string s2= "";
            foreach (string s in effect_ref.Meshes)
                s2 += s + "; ";
            TextBoxMeshes.Text = s2;
            string s3 = "";
            foreach (string s in effect_ref.Skins)
                s3 += s + "; ";
            TextBoxSkins.Text = s3;
            string s4 = "";
            foreach (string s in effect_ref.Lights)
                s4 += s + "; ";
            TextBoxLights.Text = s4;
        }

        private void TextBoxName_Validated(object sender, EventArgs e)
        {

        }

        private void TextBoxParticleNumber_Validated(object sender, EventArgs e)
        {

        }

        private void ComboRestriction_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void TextBoxBoundingRadius_Validated(object sender, EventArgs e)
        {

        }

        private void TextBoxBoneTarget_Validated(object sender, EventArgs e)
        {

        }

        private void TextBoxBoneSource_Validated(object sender, EventArgs e)
        {

        }

        private void ComboShadow_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void TextBoxBillboards_Validated(object sender, EventArgs e)
        {

        }

        private void TextBoxMeshes_Validated(object sender, EventArgs e)
        {

        }

        private void TextBoxSkins_Validated(object sender, EventArgs e)
        {

        }

        private void TextBoxLights_Validated(object sender, EventArgs e)
        {

        }
    }
}
