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

            var queue = MainForm.data.op_queue;
            if (queue == null)
            {
                return;
            }

            PopulateTree(TreeOperators.Nodes, queue.operators);
            RecolorNodes();
        }

        private void AddNode(TreeNodeCollection tnc, SFCFF.operators.ICFFOperator op)
        {
            TreeNode tn = new TreeNode(op.ToString());

            if (op is SFCFF.operators.CFFOperatorCluster)
            {
                PopulateTree(tn.Nodes, ((SFCFF.operators.CFFOperatorCluster)op).SubOperators);
            }

            tnc.Add(tn);
        }

        public void PopulateTree(TreeNodeCollection tnc, List<SFCFF.operators.ICFFOperator> ops)
        {
            foreach (var op in ops)
            {
                AddNode(tnc, op);
            }
        }

        // redoable nodes are red
        public void RecolorNodes()
        {
            var queue = MainForm.data.op_queue;
            if (queue == null)
            {
                return;
            }

            for (int i = 0; i <= queue.current_operator; i++)
            {
                TreeOperators.Nodes[i].ForeColor = Color.Black;
            }

            for (int i = queue.current_operator + 1; i < queue.operators.Count; i++)
            {
                TreeOperators.Nodes[i].ForeColor = Color.Red;
            }
        }

        // after new operator was added
        public void OnAddOperator()
        {
            // get latest operator index
            var queue = MainForm.data.op_queue;
            if (queue == null)
            {
                return;
            }

            int last_op_index = queue.current_operator - 1;

            // remove all operators preceding it
            for (int i = TreeOperators.Nodes.Count - 1; i > last_op_index; i--)
            {
                TreeOperators.Nodes.RemoveAt(i);
            }

            // add newest operator
            SFCFF.operators.ICFFOperator op = queue.operators[queue.current_operator];

            AddNode(TreeOperators.Nodes, op);
            RecolorNodes();
        }

        public void OnRemoveOperator()
        {
            var queue = MainForm.data.op_queue;
            if (queue == null)
            {
                return;
            }

            TreeOperators.Nodes.RemoveAt(queue.current_operator + 1);
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
