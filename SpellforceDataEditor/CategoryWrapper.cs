using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    public class CategoryWrapper
    {
        protected int current_index;

        public CategoryWrapper(Control form)
        {
            return;
        }

        public virtual void show_element(int ind)
        {
            current_index = ind;
        }
    }
}
