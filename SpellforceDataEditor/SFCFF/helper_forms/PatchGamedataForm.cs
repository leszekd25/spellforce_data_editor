using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFCFF;

namespace SpellforceDataEditor.SFCFF.helper_forms
{
    public partial class PatchGamedataForm : Form
    {
        public PatchGamedataForm()
        {
            InitializeComponent();
        }


        public int PatchGamedata()
        {
            ListPatched.Items.Clear();
            int entries_fixed = 0;
            foreach (SFCategory cat in SFCategoryManager.gamedata.categories.Values)
            {
                switch (cat.category_id)
                {
                    case 2001:    // UnitBuildings
                    case 2012:    // ItemGfx2D
                    case 2014:    // ItemSpells
                    case 2017:    // ItemConditions
                    case 2026:    // UnitSpells
                    case 2030:    // BuildingPolygons
                    case 2040:    // UnitLoot
                    case 2057:    // ObjectPolygons
                    case 2065:    // ObjectLoot
                    case 2067:    // CreoSpells
                        {
                            for (int i = 0; i < cat.element_lists.Count; i++)
                            {
                                bool needs_fix = false;
                                for (int j = 0; j < cat.element_lists[i].Elements.Count; j++)
                                {
                                    byte k = (byte)(cat.element_lists[i].Elements[j].variants[1]);
                                    if (k == 0)
                                    {
                                        needs_fix = true;
                                        break;
                                    }
                                }

                                if (needs_fix)
                                {
                                    for (int j = 0; j < cat.element_lists[i].Elements.Count; j++)
                                    {
                                        byte k = (byte)(cat.element_lists[i].Elements[j].variants[1]);
                                        MainForm.data.op_queue.Push(new SFCFF.operators.CFFOperatorModifyCategoryElement()
                                        {
                                            CategoryIndex = cat.category_id,
                                            ElementIndex = i,
                                            SubElementIndex = j,
                                            VariantIndex = 1,
                                            NewVariant = (byte)(k+1),
                                            IsSubElement = true
                                        });
                                        entries_fixed += 1;
                                    }
                                    ListPatched.Items.Add($"Category {cat.category_id}, element ID {cat.GetElementID(i)} ({cat.element_lists[i].Elements.Count} entries fixed)");
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return entries_fixed;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Entries fixed: " + PatchGamedata().ToString();
        }
    }
}
