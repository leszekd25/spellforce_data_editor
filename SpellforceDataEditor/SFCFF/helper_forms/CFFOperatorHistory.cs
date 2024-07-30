using SFEngine.SFCFF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.helper_forms
{
    public partial class CFFOperatorHistory : Form
    {
        public CFFOperatorHistory()
        {
            InitializeComponent();

            var queue = MainForm.data.urq;
            if (queue == null)
            {
                return;
            }

            PopulateTree(TreeOperators.Nodes, queue);
            RecolorNodes();
        }

        private void AddNode(TreeNodeCollection tnc, IUndoRedo iur)
        {
            TreeNode tn = new TreeNode(iur.ToString());

            if (iur is UndoRedoCluster)
            {
                for(int i = 0; i < ((UndoRedoCluster)iur).items.Count; i++)
                {
                    AddNode(tn.Nodes, ((UndoRedoCluster)iur).items[i]);
                }
            }

            tnc.Add(tn);
        }

        public void PopulateTree(TreeNodeCollection tnc, UndoRedoQueue queue)
        {
            for(int i = 0; i < queue.GetNumOfItems(); i++)
            {
                AddNode(tnc, queue[i]);
            }
        }

        // redoable nodes are red
        public void RecolorNodes()
        {
            var queue = MainForm.data.urq;
            if (queue == null)
            {
                return;
            }

            for (int i = 0; i <= queue.GetCurrentIndex(); i++)
            {
                TreeOperators.Nodes[i].ForeColor = Color.Black;
            }

            for (int i = queue.GetCurrentIndex() + 1; i < queue.GetNumOfItems(); i++)
            {
                TreeOperators.Nodes[i].ForeColor = Color.Red;
            }
        }

        // after new operator was added
        public void OnPush(IUndoRedo iur)
        {
            // add newest operator
            AddNode(TreeOperators.Nodes, iur);
        }

        public void OnPop()
        {
            TreeOperators.Nodes.RemoveAt(TreeOperators.Nodes.Count - 1);
        }

        public void OnUndo()
        {
            RecolorNodes();
        }

        public void OnRedo()
        {
            RecolorNodes();
        }
    }
}