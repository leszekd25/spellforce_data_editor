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
        public int current_element;                                      //current element displayed by this control
        public Dictionary<string, string> column_dict = new();        //used for choosing a column for search function
        //this dictionary uses column name as a key, and a field name as a value

        public SFControl()
        {
            InitializeComponent();
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
        public void button_repr(Button bt, int cat_id)
        {
            if (bt == null)
            {
                return;
            }

            if ((bt.IsDisposed) || (bt.Disposing))
            {
                return;
            }

            if (!MainForm.data.CachedElementDisplays.TryGetValue(cat_id, out SFControl sfc))
            {
                return;
            }
            ICategory cat = sfc.category;

            category.GetID(current_element, out int cur_id);

            bt.Tag = !cat.GetItemIndex(cur_id, out int real_elem_id);
            if ((bool)bt.Tag)
            {
                bt.Text = String.Format("Add {0} for this {1}", cat.GetName(), category.GetName());
                bt.BackColor = Color.Yellow;
            }
            else
            {
                bt.Text = String.Format("Go to {0} of this {1}", cat.GetName(), category.GetName());
                bt.BackColor = Color.DarkOrange;
            }
        }

        // sets textbox (a helper function)
        public void textbox_repr(TextBox tb, int cat_id)
        {
            if (tb == null)
            {
                return;
            }

            if ((tb.IsDisposed) || (tb.Disposing))
            {
                return;
            }

            tb.BackColor = Color.White;

            if (!MainForm.data.CachedElementDisplays.TryGetValue(cat_id, out SFControl sfc))
            {
                return;
            }
            ICategory cat = sfc.category;

            int cur_id = SFEngine.Utility.TryParseInt32(tb.Text);
            bool found_index = cat.GetItemIndex(cur_id, out int real_elem_id);
            if ((!found_index) || (real_elem_id == 0))
            {
                tb.BackColor = Color.Yellow;
            }
            else
            {
                tb.BackColor = Color.DarkOrange;
            }
        }

        // tracer helpers
        public bool trace(int cat_id, int elem_id)
        {
            return MainForm.data.trace_id(cat_id, elem_id);
        }

        public bool textbox_trace(MouseEventArgs e, int cat_id, string elem_id_str)
        {
            if(e.Button != MouseButtons.Right)
            {
                return true;
            }

            int elem_id = SFEngine.Utility.TryParseInt32(elem_id_str);

            return trace(cat_id, elem_id);
        }

        public bool textbox_gen_elem(TextBox tb, int cat_id)
        {
            if(!MainForm.data.CachedElementDisplays.TryGetValue(cat_id, out SFControl sfc))
            {
                return false;
            }

            ICategory cat = sfc.category;
            cat.GetFirstUnusedID(out int new_elem_id, out int new_elem_index);
            cat.AddID(new_elem_index, new_elem_id);
            tb.Text = new_elem_id.ToString();
            return true;
        }

        public bool button_gen_elem(Button tb, int cat_id)
        {
            if (!MainForm.data.CachedElementDisplays.TryGetValue(cat_id, out SFControl sfc))
            {
                return false;
            }

            ICategory cat = sfc.category;
            category.GetID(current_element, out int cur_id);

            if (!cat.CalculateNewItemIndex(cur_id, out int elem_index))
            {
                trace(cat_id, cur_id);
            }
            else
            {
                cat.AddID(elem_index, cur_id);
                button_repr(tb, cat_id);
            }
            return true;
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
