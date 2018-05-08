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
        protected SFCategory category;                          //control is linked to a category
        protected int current_element;                          //current element displayed by this control
        protected Dictionary<string, int[]> column_dict;        //used for choosing a column for search function
        //this dictionary uses column name as a key, and a column index as a value

        public SFControl()
        {
            InitializeComponent();
            column_dict = new Dictionary<string, int[]>();
        }

        //returns column index given its name
        public int[] get_column_index(string s)
        {
            return column_dict[s];
        }

        //returns all column names
        public Dictionary<string, int[]>.KeyCollection get_column_descriptions()
        {
            return column_dict.Keys;
        }

        //sets current element displayed
        public virtual void set_element(int index)
        {
            current_element = index;
        }

        //sets category for this control
        public void set_category(SFCategory cat)
        {
            category = cat;
        }

        //returns category
        public SFCategory get_category()
        {
            return category;
        }

        //returns current element
        public int get_element()
        {
            return current_element;
        }

        //this depends on actual control
        //each category has a corresponding control
        public virtual void show_element()
        {
            return;
        }

        //turns a given variant from current element into a text to display on text box
        //variant is numeric in this case
        public string variant_repr(int index)
        {
            SFVariant v = category.get_element_variant(current_element, index);
            if (v == null)
                return "ERROR: index out of bounds";
            return v.value.ToString();
        }

        //turns a given variant from current element into a text to display on text box
        //variant is a text in this case
        public string string_repr(int index)
        {
            SFVariant v = category.get_element_variant(current_element, index);
            if (v == null)
                return "ERROR: index out of bounds";
            if (v.vtype != TYPE.String)
                return "ERROR: wrong data type";
            return new string((char[])v.value);
        }

        //turns a given variant from current element into a text to display on text box
        //this actually operates on a sequence of byte variants (see Utility.TryParseByteArray)
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
