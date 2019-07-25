using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SpellforceDataEditor.SFCFF;

namespace SpellforceDataEditor.SFCFF.category_forms
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

        //updates data element and displayed description
        public void set_element_variant(int elem_index, int var_index, object obj)
        {
            category[elem_index][var_index] = obj;
            ((special_forms.SpelllforceCFFEditor)ParentForm).external_set_element_select_string(category, elem_index);
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
            return category[current_element][index].ToString();
        }

        //turns a given variant from current element into a text to display on text box
        //variant is a text in this case
        public string string_repr(int index)
        {
            return Encoding.UTF8.GetString((byte[])category[current_element][index]);
        }

        //turns a given variant from current element into a text to display on text box
        //this actually operates on a sequence of byte variants (see Utility.TryParseByteArray)
        public string bytearray_repr(int index, int count)
        {
            Byte[] bytes = new Byte[count];
            for(int i = 0; i < count; i++)
                bytes[i] = (Byte)category[current_element][index+i];
            return BitConverter.ToString(bytes).Replace('-', ' ');
        }

        public void step_into(TextBox tb, int cat_i)
        {
            int elem_id = Utility.TryParseInt32(tb.Text);
            step_into(cat_i, elem_id);
        }

        public void step_into(int cat_i, int elem_key)
        {
            SFCategory cat = SFCategoryManager.gamedata[cat_i];
            int real_elem_id = cat.GetElementIndex(elem_key);
            if (real_elem_id == -1)
                return;
            MainForm.data.Tracer_StepForward(cat_i, real_elem_id);
        }
    }
}
