using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

using SpellforceDataEditor.SF3D;
using SpellforceDataEditor.SFEffect;

namespace SpellforceDataEditor.special_forms
{
    public partial class EffectEditorForm : Form
    {
        EffectObject MainEffect = null;
        ParticleMovie MainMovie = null;

        EffectObject SelectedEffect = null;
        ParticleMovie SelectedMovie = null;

        public EffectEditorForm()
        {
            InitializeComponent();
        }

        private void newEffectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(MainEffect != null)
                MainEffect = null;

            MainEffect = new EffectObject();

            if (MainMovie != null)
                MainEffect.Movie = MainMovie;
            else
                MainEffect.Movie = new ParticleMovie();

            MainMovie = null;
            SelectedEffect = MainEffect;
            SelectedMovie = null;

            TreeObject.Nodes.Clear();
            TreeObject.Nodes.Add(EffectObjectToNode(MainEffect));
            SelectNode(TreeObject.Nodes[0]);
        }

        private void asNewEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void asSubeffectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadMovieToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveMovieToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void SelectNode(TreeNode tn)
        {
            TreeObject.SelectedNode = tn;
            if (tn.Tag is EffectObject)
            {
                SelectedEffect = (EffectObject)(tn.Tag);
                SelectedMovie = null;
                InspectEffect(SelectedEffect);
            }
            else if (tn.Tag is ParticleMovie)
            {
                SelectedEffect = null;
                SelectedMovie = (ParticleMovie)(tn.Tag);
                InspectMovie(SelectedMovie);
            }
        }

        private TreeNode EffectObjectToNode(EffectObject eo)
        {
            TreeNode tn = new TreeNode(eo.Name);
            tn.Tag = eo;
            tn.Nodes.Add(MovieToNode(eo.Movie));
            return tn;
        }

        private TreeNode MovieToNode(ParticleMovie pm)
        {
            TreeNode tn = new TreeNode("Movie");
            tn.Tag = pm;
            return tn;
        }

        private void InspectEffect(EffectObject eo)
        {
            PropertyInspector.Controls.Clear();
            PropertyInspector.Controls.Add(new SFEffect.effect_controls.EffectObjectControl() { Effect_ref = eo });
        }

        private void InspectMovie(ParticleMovie pm)
        {
            PropertyInspector.Controls.Clear();
            int h = 0;
            foreach(ParticleMovieEffect pme in pm.Modifiers)
            {
                SFEffect.effect_controls.ParticleMovieEffectControl pmec = pme.NewControl();
                PropertyInspector.Controls.Add(pmec);
                pmec.Location = new Point(0, h);
                h += pmec.Height + 6;
                pmec.MovieEffect_ref = pme;
            }
        }

        public void MoveEffectModifierUp(int modifier_index)
        {
            if (modifier_index == 0)
                return;

            SFEffect.effect_controls.ParticleMovieEffectControl pmec = (SFEffect.effect_controls.ParticleMovieEffectControl)PropertyInspector.Controls[modifier_index];
            SFEffect.effect_controls.ParticleMovieEffectControl pmec2 = (SFEffect.effect_controls.ParticleMovieEffectControl)PropertyInspector.Controls[modifier_index - 1];

            SelectedMovie.Modifiers.RemoveAt(modifier_index);
            SelectedMovie.Modifiers.RemoveAt(modifier_index - 1);

            SelectedMovie.Modifiers.Insert(modifier_index - 1, (ParticleMovieEffect)pmec.Tag);
            SelectedMovie.Modifiers.Insert(modifier_index, (ParticleMovieEffect)pmec2.Tag);

            pmec.Location = new Point(pmec2.Location.X, pmec2.Location.Y);
            pmec2.Location = new Point(pmec.Location.X, pmec.Location.Y + pmec.Height + 6);

            PropertyInspector.Controls.SetChildIndex(pmec, PropertyInspector.Controls.GetChildIndex(pmec2));
        }

        public void MoveEffectModifierDown(int modifier_index)
        {
            if (modifier_index == PropertyInspector.Controls.Count - 1)
                return;

            SFEffect.effect_controls.ParticleMovieEffectControl pmec = (SFEffect.effect_controls.ParticleMovieEffectControl)PropertyInspector.Controls[modifier_index];
            SFEffect.effect_controls.ParticleMovieEffectControl pmec2 = (SFEffect.effect_controls.ParticleMovieEffectControl)PropertyInspector.Controls[modifier_index + 1];

            SelectedMovie.Modifiers.RemoveAt(modifier_index + 1);
            SelectedMovie.Modifiers.RemoveAt(modifier_index);

            SelectedMovie.Modifiers.Insert(modifier_index, (ParticleMovieEffect)pmec2.Tag);
            SelectedMovie.Modifiers.Insert(modifier_index + 1, (ParticleMovieEffect)pmec.Tag);

            pmec2.Location = new Point(pmec.Location.X, pmec.Location.Y);
            pmec.Location = new Point(pmec2.Location.X, pmec2.Location.Y + pmec2.Height + 6);

            PropertyInspector.Controls.SetChildIndex(pmec2, PropertyInspector.Controls.GetChildIndex(pmec));
        }

        public void MaximizeEffectModifier(int modifier_index)
        {
            SFEffect.effect_controls.ParticleMovieEffectControl pmec = (SFEffect.effect_controls.ParticleMovieEffectControl)PropertyInspector.Controls[modifier_index];

            pmec.Height = pmec.MaxHeight;

            int h = pmec.Location.Y + pmec.Height + 6;

            for (int i = modifier_index + 1; i < PropertyInspector.Controls.Count; i++)
            {
                PropertyInspector.Controls[i].Location = new Point(PropertyInspector.Controls[i].Location.X, h);
                h += PropertyInspector.Controls[i].Height + 6;
            }
        }

        public void MinimizeEffectModifier(int modifier_index)
        {
            SFEffect.effect_controls.ParticleMovieEffectControl pmec = (SFEffect.effect_controls.ParticleMovieEffectControl)PropertyInspector.Controls[modifier_index];

            pmec.Height = 40;

            int h = pmec.Location.Y + pmec.Height + 6;

            for (int i = modifier_index + 1; i < PropertyInspector.Controls.Count; i++)
            {
                PropertyInspector.Controls[i].Location = new Point(PropertyInspector.Controls[i].Location.X, h);
                h += PropertyInspector.Controls[i].Height + 6;
            }
        }

        public void RemoveEffectModifier(int modifier_index)
        {
            SFEffect.effect_controls.ParticleMovieEffectControl pmec = (SFEffect.effect_controls.ParticleMovieEffectControl)PropertyInspector.Controls[modifier_index];

            int h = pmec.Location.Y;

            SelectedMovie.Modifiers.RemoveAt(modifier_index);
            PropertyInspector.Controls.RemoveAt(modifier_index);

            for (int i = modifier_index; i < PropertyInspector.Controls.Count; i++)
            {
                PropertyInspector.Controls[i].Location = new Point(PropertyInspector.Controls[i].Location.X, h);
                h += PropertyInspector.Controls[i].Height + 6;
            }
        }

        private void TreeObject_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SelectNode(e.Node);
        }

        private void MovieModifiers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MovieModifiers.SelectedIndex == -1)
                return;

            if(SelectedMovie != null)
            {
                int h = -PropertyInspector.VerticalScroll.Value;
                foreach (Control c in PropertyInspector.Controls)
                    h += c.Height + 6;

                ParticleMovieEffect pme = Assembly.GetExecutingAssembly().CreateInstance("SpellforceDataEditor.SFEffect.Particle" + MovieModifiers.SelectedItem.ToString()) as ParticleMovieEffect;
                SelectedMovie.Modifiers.Add(pme);

                SFEffect.effect_controls.ParticleMovieEffectControl pmec = pme.NewControl();
                PropertyInspector.Controls.Add(pmec);
                pmec.Location = new Point(0, h);
                pmec.MovieEffect_ref = pme;
            }
        }

        private void ButtonAddEffect_Click(object sender, EventArgs e)
        {
            if ((SelectedEffect == null)&&(SelectedMovie == null))
                return;

            EffectObject neo = new EffectObject();
            neo.Movie = new ParticleMovie();

            if(TreeObject.SelectedNode != null)
            {
                if (TreeObject.SelectedNode.Tag is EffectObject)
                {
                    ((EffectObject)TreeObject.SelectedNode.Tag).SubObjects.Add(neo);
                    TreeObject.SelectedNode.Nodes.Add(EffectObjectToNode(neo));
                }
                else if(TreeObject.SelectedNode.Tag is ParticleMovie)
                {
                    ((EffectObject)TreeObject.SelectedNode.Parent.Tag).SubObjects.Add(neo);
                    TreeObject.SelectedNode.Parent.Nodes.Add(EffectObjectToNode(neo));
                }

            }
        }

        private void ButtonRemoveEffect_Click(object sender, EventArgs e)
        {
            if ((SelectedEffect == null) && (SelectedMovie == null))
                return;
            
            if (TreeObject.SelectedNode != null)
            {
                if (TreeObject.SelectedNode.Tag is EffectObject)
                {
                    if (TreeObject.SelectedNode.Parent != null)
                    {
                        EffectObject reo = ((EffectObject)TreeObject.SelectedNode.Tag);
                        ((EffectObject)TreeObject.SelectedNode.Parent.Tag).SubObjects.Remove(reo);
                        TreeObject.SelectedNode.Parent.Nodes.Remove(TreeObject.SelectedNode);
                    }
                }
                else if (TreeObject.SelectedNode.Tag is ParticleMovie)
                {
                    if (TreeObject.SelectedNode.Parent.Parent != null)
                    {
                        EffectObject reo = ((EffectObject)TreeObject.SelectedNode.Parent.Tag);
                        ((EffectObject)TreeObject.SelectedNode.Parent.Parent.Tag).SubObjects.Remove(reo);
                        TreeObject.SelectedNode.Parent.Parent.Nodes.Remove(TreeObject.SelectedNode.Parent);
                    }
                }
            }
        }



        // pressing this button will create and animate a scene in asset viewer
        private void ButtonRunEffect_Click(object sender, EventArgs e)
        {
            MainForm.viewer.ExternalRunEffect(MainEffect);
        }
    }
}
