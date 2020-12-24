using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_dialog
{
    public partial class MapOperatorHistoryViewer : Form
    {
        public MapOperatorHistoryViewer()
        {
            InitializeComponent();

            var queue = MainForm.mapedittool.op_queue;
            if (queue == null)
                return;

            PopulateTree(TreeOperators.Nodes, queue.operators);
            RecolorNodes();
        }

        private void AddNode(TreeNodeCollection tnc, map_operators.IMapOperator op)
        {
            TreeNode tn = new TreeNode(op.ToString());

            if (op is map_operators.MapOperatorCluster)
                PopulateTree(tn.Nodes, ((map_operators.MapOperatorCluster)op).SubOperators);

            tnc.Add(tn);
        }

        public void PopulateTree(TreeNodeCollection tnc, List<map_operators.IMapOperator> ops)
        {
            foreach (var op in ops)
                AddNode(tnc, op);
        }

        // redoable nodes are red
        public void RecolorNodes()
        {
            var queue = MainForm.mapedittool.op_queue;
            if (queue == null)
                return;

            for(int i = 0; i <= queue.current_operator; i++)
                TreeOperators.Nodes[i].ForeColor = Color.Black;
            for (int i = queue.current_operator + 1; i < queue.operators.Count; i++)
                TreeOperators.Nodes[i].ForeColor = Color.Red;
        }

        // after new operator was added
        public void OnAddOperator()
        {
            // get latest operator index
            var queue = MainForm.mapedittool.op_queue;
            if (queue == null)
                return;

            int last_op_index = queue.current_operator - 1;

            // remove all operators preceding it
            for (int i = TreeOperators.Nodes.Count - 1; i > last_op_index; i--)
                TreeOperators.Nodes.RemoveAt(i);

            // add newest operator
            map_operators.IMapOperator op = queue.operators[queue.current_operator];

            AddNode(TreeOperators.Nodes, op);
            RecolorNodes();
        }

        public void OnRemoveOperator()
        {
            var queue = MainForm.mapedittool.op_queue;
            if (queue == null)
                return;

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
