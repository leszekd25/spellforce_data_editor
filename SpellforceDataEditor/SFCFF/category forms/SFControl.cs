using SFEngine.SFCFF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class SFControl : UserControl
    {
        public ICategory category { get; protected set; }
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

        // updates UI when subelement is added at given index
        public virtual void on_add_subelement(int subelem_index)
        {
            throw new NotImplementedException("Category '" + category.GetName() + "' does not support add subelement operation.");
        }

        // updates UI when subelement is removed at given index
        public virtual void on_remove_subelement(int subelem_index)
        {
            throw new NotImplementedException("Category '" + category.GetName() + "' does not support remove subelement operation.");
        }

        // updates UI when a subelement is updated at given index
        public virtual void on_update_subelement(int subelem_index)
        {
            throw new NotImplementedException("Category '" + category.GetName() + "' does not support update subelement operation.");
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

        // sets button text (a helper function)
        public void button_repr(Button bt, ICategory cat_i, string label1, string label2)
        {
            if (bt == null)
            {
                return;
            }

            if ((bt.IsDisposed) || (bt.Disposing))
            {
                return;
            }

            category.GetID(current_element, out int cur_id);

            bt.Tag = !cat_i.GetItemIndex(cur_id, out int real_elem_id);
            if ((bool)bt.Tag)
            {
                bt.Text = String.Format("Add {0} for this {1}", label1, label2);
                bt.BackColor = Color.Yellow;
            }
            else
            {
                bt.Text = String.Format("Go to {0} of this {1}", label1, label2);
                bt.BackColor = Color.DarkOrange;
            }
        }

        // sets textbox (a helper function)
        public void textbox_repr(TextBox tb, ICategory cat_i)
        {
            if (tb == null)
            {
                return;
            }

            if ((tb.IsDisposed) || (tb.Disposing))
            {
                return;
            }

            int cur_id = SFEngine.Utility.TryParseInt32(tb.Text);

            bool found_index = cat_i.GetItemIndex(cur_id, out int real_elem_id);
            if ((found_index) || (real_elem_id == 0))
            {
                tb.BackColor = Color.Yellow;
            }
            else
            {
                tb.BackColor = Color.DarkOrange;
            }
        }

        public virtual string get_element_string(int elem_key)
        {
            return elem_key.ToString();
        }

        public virtual string get_description_string(int elem_key)
        {
            return "";
        }
    }
}
