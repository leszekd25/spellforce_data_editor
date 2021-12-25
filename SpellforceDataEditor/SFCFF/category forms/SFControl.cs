using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFCFF;


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

        // updates UI when subelement is added at given index
        public virtual void on_add_subelement(int subelem_index)
        {
            throw new NotImplementedException("Category '" + category.category_name + "' does not support add subelement operation.");
        }

        // updates UI when subelement is removed at given index
        public virtual void on_remove_subelement(int subelem_index)
        {
            throw new NotImplementedException("Category '" + category.category_name + "' does not support remove subelement operation.");
        }

        // updates UI when a subelement is updated at given index
        public virtual void on_update_subelement(int subelem_index)
        {
            throw new NotImplementedException("Category '" + category.category_name + "' does not support update subelement operation.");
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
            System.Diagnostics.Debug.Assert(!category.category_allow_multiple, "SFControl.set_element_variant(): Invalid category type");

            category[elem_index][var_index] = obj;
            MainForm.data.external_set_element_select_string(category, elem_index);
        }

        //updates data element and displayed description
        public void set_element_variant(int elem_index, int subelem_index, int var_index, object obj)
        {
            System.Diagnostics.Debug.Assert(category.category_allow_multiple, "SFControl.set_element_variant(): Invalid category type");

            category[elem_index, subelem_index][var_index] = obj;
            MainForm.data.external_set_element_select_string(category, elem_index);
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
            System.Diagnostics.Debug.Assert(!category.category_allow_multiple, "SFControl.set_element_variant(): Invalid category type");
            return category[current_element][index].ToString();
        }

        //turns a given variant from current element into a text to display on text box
        //variant is numeric in this case
        public string variant_repr(int subelem_index, int index)
        {
            System.Diagnostics.Debug.Assert(category.category_allow_multiple, "SFControl.set_element_variant(): Invalid category type");
            return category[current_element, subelem_index][index].ToString();
        }

        //turns a given variant from current element into a text to display on text box
        //variant is a text in this case
        public string string_repr(int index)
        {
            System.Diagnostics.Debug.Assert(!category.category_allow_multiple, "SFControl.set_element_variant(): Invalid category type");
            return category[current_element][index].ToString();
        }

        //turns a given variant from current element into a text to display on text box
        //variant is a text in this case
        public string string_repr(int subelem_index, int index)
        {
            System.Diagnostics.Debug.Assert(category.category_allow_multiple, "SFControl.set_element_variant(): Invalid category type");
            return category[current_element, subelem_index][index].ToString();
        }

        //turns a given variant from current element into a text to display on text box
        //this actually operates on a sequence of byte variants (see SFEngine.Utility.TryParseByteArray)
        public string bytearray_repr(int index, int count)
        {
            System.Diagnostics.Debug.Assert(!category.category_allow_multiple, "SFControl.set_element_variant(): Invalid category type");

            Byte[] bytes = new Byte[count];
            for (int i = 0; i < count; i++)
                bytes[i] = (Byte)category[current_element][index + i];
            return BitConverter.ToString(bytes).Replace('-', ' ');
        }

        // sets button text (a helper function)
        public void button_repr(Button bt, int cat_i, string label1, string label2)
        {
            System.Diagnostics.Debug.Assert(!category.category_allow_multiple, "SFControl.set_element_variant(): Invalid category type");

            if (bt == null)
                return;
            if ((bt.IsDisposed) || (bt.Disposing))
                return;

            int cur_id = category.GetElementID(current_element);

            SFCategory cat = SFCategoryManager.gamedata[cat_i];
            if (cat == null)
                return;

            int real_elem_id = cat.GetElementIndex(cur_id);
            bt.Tag = (real_elem_id == SFEngine.Utility.NO_INDEX);
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
        public void textbox_repr(TextBox tb, int cat_i)
        {
            System.Diagnostics.Debug.Assert(!category.category_allow_multiple, "SFControl.set_element_variant(): Invalid category type");

            if (tb == null)
                return;
            if ((tb.IsDisposed) || (tb.Disposing))
                return;

            int cur_id = SFEngine.Utility.TryParseInt32(tb.Text);

            SFCategory cat = SFCategoryManager.gamedata[cat_i];
            if (cat == null)
                return;

            int real_elem_id = cat.GetElementIndex(cur_id);
            if ((real_elem_id == SFEngine.Utility.NO_INDEX)||(real_elem_id == 0))
                tb.BackColor = Color.Yellow;
            else
                tb.BackColor = Color.DarkOrange;
        }

        public void button_step_into(Button bt, int cat_i)
        {
            if((bool)bt.Tag)     // add new entry
            {
                int cur_id = category.GetElementID(current_element);

                SFCategory cat = SFCategoryManager.gamedata[cat_i];
                if(cat == null)
                {
                    // todo: create new category for this element
                    // problem: how to handle versioning?
                    return;
                }

                int new_ind = cat.GetNewElementIndex(cur_id);
                SFCategoryElement new_elem = cat.GetEmptyElement();
                switch (cat.GetElementFormat()[0])
                {
                    case 'B':
                        new_elem[0] = (byte)cur_id;
                        break;
                    case 'H':
                        new_elem[0] = (ushort)cur_id;
                        break;
                    case 'I':
                        new_elem[0] = (uint)cur_id;
                        break;
                    default:
                        throw new Exception("SFControl.button_step_into(): Invalid element format!");
                }

                if (cat.category_allow_multiple)
                {
                    SFCategoryElementList new_elem_list = new SFCategoryElementList();
                    new_elem_list.Elements.Add(new_elem);

                    cat.element_lists.Insert(new_ind, new_elem_list);
                }
                else
                {
                    cat.elements.Insert(new_ind, new_elem);
                }

                bt.Tag = false;
            }
            else
            {
                int cur_id = category.GetElementID(current_element);
                step_into(cat_i, cur_id);
            }
        }

        public void step_into(TextBox tb, int cat_i)
        {
            int elem_id = SFEngine.Utility.TryParseInt32(tb.Text);
            step_into(cat_i, elem_id);
        }

        public void step_into(int cat_i, int elem_key)
        {
            SFCategory cat = SFCategoryManager.gamedata[cat_i];
            if (cat == null)
                return;

            int real_elem_id = cat.GetElementIndex(elem_key);
            if (real_elem_id == SFEngine.Utility.NO_INDEX)
                return;
            MainForm.data.Tracer_StepForward(cat_i, real_elem_id);
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
