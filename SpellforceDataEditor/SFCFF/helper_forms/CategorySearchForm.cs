using OpenTK.Windowing.Common.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.helper_forms
{
    public partial class CategorySearchForm : Form
    {
        int current_cat = 0;
        List<int> current_indices = new();

        public CategorySearchForm()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            current_indices.Clear();
            listResults.Items.Clear();
        }

        void Reload(int cat, List<int> result)
        {
            Clear();
            current_cat = cat;
            current_indices.AddRange(result);
            current_indices.Sort();
            for (int i = 0; i < current_indices.Count; i++)
            {
                listResults.Items.Add(MainForm.data.CachedElementDisplays[cat].get_element_string(current_indices[i]));
            }
        }

        public void Populate(int cat, List<int> result)
        {
            Reload(cat, result);
        }

        public void Update(int cat, List<int> result)
        {
            if(cat != current_cat)
            {
                Clear();
                return;
            }

            HashSet<int> r2 = current_indices.ToHashSet();
            r2.IntersectWith(result);
            Reload(cat, r2.ToList());
        }

        public void OnItemAdd(int cat, int index)
        {
            if (cat != current_cat)
            {
                return;
            }

            for(int i = 0; i < current_indices.Count; i++)
            {
                if (current_indices[i] >= index)
                {
                    current_indices[i]++;
                }
            }
        }

        public void OnItemRemove(int cat, int index)
        {
            if (cat != current_cat)
            {
                return;
            }

            for (int i = 0; i < current_indices.Count; i++)
            {
                if (current_indices[i] == index)
                {
                    current_indices.RemoveAt(i);
                    listResults.Items.RemoveAt(i);
                    i--;
                    continue;
                }
                if (current_indices[i] > index)
                {
                    current_indices[i]--;
                }
            }
        }

        public void OnItemModify(int cat, int index)
        {
            if (cat != current_cat)
            {
                return;
            }

            for(int i = 0; i < current_indices.Count; i++)
            {
                if (current_indices[i] == index)
                {
                    listResults.Items[i] = MainForm.data.CachedElementDisplays[cat].get_element_string(current_indices[i]);
                    break;
                }
            }

        }

        private void listResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listResults.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                return;
            }

            MainForm.data.CachedElementDisplays[current_cat].category.GetID(current_indices[listResults.SelectedIndex], out int id);
            MainForm.data.trace_id(current_cat, id);
        }
    }
}
