using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    public abstract class SFCategory
    {
        private string name;
        private uint id;
        private uint item_count;
        private SFCategoryElement[] elements;
        public abstract Object[] get_element(StreamReader sr);
        public abstract SFVariant[] set_element(StreamWriter sw);
        public abstract void read(StreamReader sr);
        public abstract void write(StreamWriter sw);
    }
}
