using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.category_forms
{
    public partial class SFControl : UserControl
    {
        protected SFCategory category;
        protected int current_element;

        public SFControl()
        {
            InitializeComponent();
        }

        public virtual void show_element()
        {
            return;
        }
    }
}
