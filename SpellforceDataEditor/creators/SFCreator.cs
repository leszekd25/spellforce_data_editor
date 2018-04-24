using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor
{
    public partial class SFCreator : Form
    {
        protected SFCategoryManager manager;

        public SFCreator()
        {
            InitializeComponent();
        }

        public void set_manager(SFCategoryManager man)
        {
            manager = man;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            AddNewElement();
        }

        public virtual void AddNewElement()
        {
            return;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
