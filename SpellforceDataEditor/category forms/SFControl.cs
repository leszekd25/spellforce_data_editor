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

        public void set_element(int index)
        {
            current_element = index;
        }

        public void set_category(SFCategory cat)
        {
            category = cat;
        }

        public SFCategory get_category()
        {
            return category;
        }

        public int get_element()
        {
            return current_element;
        }

        public virtual void show_element()
        {
            return;
        }

        public string variant_repr(int index)
        {
            SFVariant v = category.get_element_variant(current_element, index);
            if (v == null)
                return "ERROR: index out of bounds";
            return v.value.ToString();
        }

        public string string_repr(int index)
        {
            SFVariant v = category.get_element_variant(current_element, index);
            if (v == null)
                return "ERROR: index out of bounds";
            if (v.vtype != TYPE.String)
                return "ERROR: wrong data type";
            return new string((char[])v.value);
        }

        public string bytearray_repr(int index, int count)
        {
            Byte[] bytes = new Byte[count];
            for(int i = 0; i < count; i++)
            {
                SFVariant v = category.get_element_variant(current_element, index + i);
                if (v == null)
                    return "ERROR: index out of bounds";
                if (v.vtype != TYPE.UByte)
                    return "ERROR: wrong data type";
                bytes[i] = (Byte)category.get_element_variant(current_element, index + i).value;
            }
            return BitConverter.ToString(bytes).Replace('-', ' ');
        }
    }
}
