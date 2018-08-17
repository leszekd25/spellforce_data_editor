using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.special_forms
{
    public partial class ReferencesForm : Form
    {
        private struct CatElem
        {
            public int category;
            public int element;
            public CatElem(int c, int e)
            {
                category = c;
                element = e;
            }
        }

        private struct CatColumn
        {
            public int category;
            public int column;
            public CatColumn(int c, int co)
            {
                category = c;
                column = co;
            }
        }

        SpelllforceCFFEditor sf_form;
        CatElem referenced;
        private List<CatElem> elements = new List<CatElem>();
        private static CatColumn[][] ReferenceCategoryTable;

        public ReferencesForm()
        {
            InitializeComponent();
            //reference category table
            ReferenceCategoryTable = new CatColumn[49][];
            ReferenceCategoryTable[0] = new CatColumn[] { new CatColumn(0, -1), new CatColumn(5, 2), new CatColumn(11, 2), new CatColumn(13, 1), new CatColumn(19, 2) };  //-1 means custom columns
            ReferenceCategoryTable[1] = new CatColumn[] { new CatColumn(0, 1) };
            ReferenceCategoryTable[2] = new CatColumn[] { };
            ReferenceCategoryTable[3] = new CatColumn[] { new CatColumn(4, 0), new CatColumn(5, 0), new CatColumn(6, 4), new CatColumn(17, 2) };
            ReferenceCategoryTable[4] = new CatColumn[] { new CatColumn(3, 0), new CatColumn(5, 0), new CatColumn(6, 4), new CatColumn(17, 2) };
            ReferenceCategoryTable[5] = new CatColumn[] { new CatColumn(3, 0), new CatColumn(4, 0), new CatColumn(6, 4), new CatColumn(17, 2) };
            ReferenceCategoryTable[6] = new CatColumn[] { new CatColumn(7, 0), new CatColumn(8, 0), new CatColumn(8, 1), new CatColumn(9, 0), new CatColumn(10, 0), new CatColumn(11, 0), new CatColumn(12, 0), new CatColumn(13, 0), new CatColumn(18, 2), new CatColumn(21, 2), new CatColumn(21, 4), new CatColumn(21, 6), new CatColumn(29, 1), new CatColumn(35, 2), new CatColumn(35, 4), new CatColumn(35, 6), };
            ReferenceCategoryTable[7] = new CatColumn[] { new CatColumn(6, 0), new CatColumn(8, 0), new CatColumn(8, 1), new CatColumn(9, 0), new CatColumn(10, 0), new CatColumn(11, 0), new CatColumn(12, 0), new CatColumn(13, 0), new CatColumn(18, 2), new CatColumn(21, 2), new CatColumn(21, 4), new CatColumn(21, 6), new CatColumn(29, 1),  new CatColumn(35, 2), new CatColumn(35, 4), new CatColumn(35, 6), };
            ReferenceCategoryTable[8] = new CatColumn[] { new CatColumn(7, 0), new CatColumn(6, 0), new CatColumn(9, 0), new CatColumn(10, 0), new CatColumn(11, 0), new CatColumn(12, 0), new CatColumn(13, 0), new CatColumn(18, 2), new CatColumn(21, 2), new CatColumn(21, 4), new CatColumn(21, 6), new CatColumn(29, 1), new CatColumn(35, 2), new CatColumn(35, 4), new CatColumn(35, 6), };
            ReferenceCategoryTable[9] = new CatColumn[] { new CatColumn(7, 0), new CatColumn(8, 0), new CatColumn(8, 1), new CatColumn(6, 0), new CatColumn(10, 0), new CatColumn(11, 0), new CatColumn(12, 0), new CatColumn(13, 0), new CatColumn(18, 2), new CatColumn(21, 2), new CatColumn(21, 4), new CatColumn(21, 6), new CatColumn(29, 1), new CatColumn(35, 2), new CatColumn(35, 4), new CatColumn(35, 6), };
            ReferenceCategoryTable[10] = new CatColumn[] { new CatColumn(7, 0), new CatColumn(8, 0), new CatColumn(8, 1), new CatColumn(9, 0), new CatColumn(6, 0), new CatColumn(11, 0), new CatColumn(12, 0), new CatColumn(13, 0), new CatColumn(18, 2), new CatColumn(21, 2), new CatColumn(21, 4), new CatColumn(21, 6), new CatColumn(29, 1), new CatColumn(35, 2), new CatColumn(35, 4), new CatColumn(35, 6), };
            ReferenceCategoryTable[11] = new CatColumn[] { new CatColumn(7, 0), new CatColumn(8, 0), new CatColumn(8, 1), new CatColumn(9, 0), new CatColumn(10, 0), new CatColumn(6, 0), new CatColumn(12, 0), new CatColumn(13, 0), new CatColumn(18, 2), new CatColumn(21, 2), new CatColumn(21, 4), new CatColumn(21, 6), new CatColumn(29, 1), new CatColumn(35, 2), new CatColumn(35, 4), new CatColumn(35, 6), };
            ReferenceCategoryTable[12] = new CatColumn[] { new CatColumn(7, 0), new CatColumn(8, 0), new CatColumn(8, 1), new CatColumn(9, 0), new CatColumn(10, 0), new CatColumn(11, 0), new CatColumn(6, 0), new CatColumn(13, 0), new CatColumn(18, 2), new CatColumn(21, 2), new CatColumn(21, 4), new CatColumn(21, 6), new CatColumn(29, 1), new CatColumn(35, 2), new CatColumn(35, 4), new CatColumn(35, 6), };
            ReferenceCategoryTable[13] = new CatColumn[] { new CatColumn(7, 0), new CatColumn(8, 0), new CatColumn(8, 1), new CatColumn(9, 0), new CatColumn(10, 0), new CatColumn(11, 0), new CatColumn(12, 0), new CatColumn(6, 0), new CatColumn(18, 2), new CatColumn(21, 2), new CatColumn(21, 4), new CatColumn(21, 6), new CatColumn(29, 1), new CatColumn(35, 2), new CatColumn(35, 4), new CatColumn(35, 6), };
            ReferenceCategoryTable[14] = new CatColumn[] { new CatColumn(1, 1), new CatColumn(6, 3), new CatColumn(15, 7), new CatColumn (17, 1), new CatColumn(23, 5), new CatColumn(26, 2), new CatColumn(31, 1), new CatColumn(33, 1), new CatColumn(36, 1), new CatColumn(37, 3), new CatColumn(38, 5), new CatColumn(40, 1), new CatColumn(41, 1), new CatColumn(41, 2), new CatColumn(42, 3), new CatColumn(43, 1), new CatColumn(44, 1), new CatColumn(47, 2), new CatColumn(48, 1)  };
            ReferenceCategoryTable[15] = new CatColumn[] { new CatColumn(3, 2) };
            ReferenceCategoryTable[16] = new CatColumn[] { new CatColumn(15, 9) };
            ReferenceCategoryTable[17] = new CatColumn[] { new CatColumn(0, -2), new CatColumn(6, 5), new CatColumn(18, 0), new CatColumn(19, 0), new CatColumn(20, 0), new CatColumn(21, 0), new CatColumn(22, 0), new CatColumn(28, 1) };
            ReferenceCategoryTable[18] = new CatColumn[] { new CatColumn(0, -2), new CatColumn(6, 5), new CatColumn(17, 0), new CatColumn(19, 0), new CatColumn(20, 0), new CatColumn(21, 0), new CatColumn(22, 0), new CatColumn(28, 1) };
            ReferenceCategoryTable[19] = new CatColumn[] { new CatColumn(0, -2), new CatColumn(6, 5), new CatColumn(18, 0), new CatColumn(17, 0), new CatColumn(20, 0), new CatColumn(21, 0), new CatColumn(22, 0), new CatColumn(28, 1) };
            ReferenceCategoryTable[20] = new CatColumn[] { new CatColumn(0, -2), new CatColumn(6, 5), new CatColumn(18, 0), new CatColumn(19, 0), new CatColumn(17, 0), new CatColumn(21, 0), new CatColumn(22, 0), new CatColumn(28, 1) };
            ReferenceCategoryTable[21] = new CatColumn[] { new CatColumn(0, -2), new CatColumn(6, 5), new CatColumn(18, 0), new CatColumn(19, 0), new CatColumn(20, 0), new CatColumn(17, 0), new CatColumn(22, 0), new CatColumn(28, 1) };
            ReferenceCategoryTable[22] = new CatColumn[] { new CatColumn(0, -2), new CatColumn(6, 5), new CatColumn(18, 0), new CatColumn(19, 0), new CatColumn(20, 0), new CatColumn(21, 0), new CatColumn(17, 0), new CatColumn(28, 1) };
            ReferenceCategoryTable[23] = new CatColumn[] { new CatColumn(6, 6), new CatColumn(22, 2), new CatColumn(23, 10), new CatColumn(24, 0), new CatColumn(25, 0), new CatColumn(47, 1) };
            ReferenceCategoryTable[24] = new CatColumn[] { new CatColumn(6, 6), new CatColumn(22, 2), new CatColumn(23, 10), new CatColumn(23, 0), new CatColumn(25, 0), new CatColumn(47, 1) };
            ReferenceCategoryTable[25] = new CatColumn[] { new CatColumn(6, 6), new CatColumn(22, 2), new CatColumn(23, 10), new CatColumn(24, 0), new CatColumn(23, 0), new CatColumn(47, 1) };
            ReferenceCategoryTable[26] = new CatColumn[] { };
            ReferenceCategoryTable[27] = new CatColumn[] { };
            ReferenceCategoryTable[28] = new CatColumn[] { new CatColumn(29, 0), new CatColumn(30, 0) };
            ReferenceCategoryTable[29] = new CatColumn[] { new CatColumn(28, 0), new CatColumn(30, 0) };
            ReferenceCategoryTable[30] = new CatColumn[] { new CatColumn(29, 0), new CatColumn(28, 0) };
            ReferenceCategoryTable[31] = new CatColumn[] { new CatColumn(20, 1), new CatColumn(25, 1) };
            ReferenceCategoryTable[32] = new CatColumn[] { };
            ReferenceCategoryTable[33] = new CatColumn[] { new CatColumn(34, 0), new CatColumn(35, 0) };
            ReferenceCategoryTable[34] = new CatColumn[] { new CatColumn(33, 0), new CatColumn(35, 0) };
            ReferenceCategoryTable[35] = new CatColumn[] { new CatColumn(33, 0), new CatColumn(34, 0) };
            ReferenceCategoryTable[36] = new CatColumn[] { };
            ReferenceCategoryTable[37] = new CatColumn[] { new CatColumn(38, 1) };
            ReferenceCategoryTable[38] = new CatColumn[] { };
            ReferenceCategoryTable[39] = new CatColumn[] { };
            ReferenceCategoryTable[40] = new CatColumn[] { new CatColumn(1, 8), new CatColumn(47, 3) };
            ReferenceCategoryTable[41] = new CatColumn[] { };
            ReferenceCategoryTable[42] = new CatColumn[] { new CatColumn(42, 1) };
            ReferenceCategoryTable[43] = new CatColumn[] { new CatColumn(9, 6) };
            ReferenceCategoryTable[44] = new CatColumn[] { new CatColumn(9, 7) };
            ReferenceCategoryTable[45] = new CatColumn[] { };
            ReferenceCategoryTable[46] = new CatColumn[] { };
            ReferenceCategoryTable[47] = new CatColumn[] { };
            ReferenceCategoryTable[48] = new CatColumn[] { new CatColumn(6, 10) };
        }

        public void set_referenced_element(SpelllforceCFFEditor form, int c, int e)
        {
            referenced = new CatElem(c, e);
            sf_form = form;

            labelRefElemName.Text = get_catelement_string(referenced);
        }

        private string get_catelement_string(CatElem ce)
        {
            return "Category " + (ce.category + 1).ToString() + " | " + sf_form.get_manager().get_category(ce.category).get_element_string(ce.element);
        }

        private void add_entries(List<CatElem> elements_so_far, CatElem element_looked_for, CatColumn column_looked_at)
        {
            SFCategoryManager manager = sf_form.get_manager();
            List<int> preload_indices = new List<int>();
            for (int i = 0; i < manager.get_category(column_looked_at.category).get_element_count(); i++)
                preload_indices.Add(i);

            List<int> results_intermediate = new List<int>();
            switch (column_looked_at.column)
            {
                case -1:    //spell id in category 1 (0)
                    //search for all spell references in category 0
                    SFCategory ctg = manager.get_category(0);
                    int id_looked_for = (int)((UInt16)manager.get_category(element_looked_for.category).get_element(element_looked_for.element).get_single_variant(0).value);
                    for(int i = 0; i < ctg.get_element_count(); i++)
                    {
                        SFCategoryElement elem = ctg.get_element(i);

                        int searched_id = -1;
                        UInt16 spell_type = (UInt16)elem.get_single_variant(1).value;
                        switch (spell_type)
                        {
                            case 176: //elemental essence
                            case 180: //elemental almightness
                                searched_id = (int)((UInt32)elem.get_single_variant(21).value);   //parameter #1
                                break;
                            case 12:  //fireshield (cast)
                            case 15:  //iceshield (cast)
                            case 47:  //thornshield
                            case 175: //white essence
                            case 179: //white almightness
                                searched_id = (int)((UInt32)elem.get_single_variant(22).value);   //parameter #2
                                break;
                            case 13:  //fireball (cast)
                            case 73:  //rain of fire
                            case 74:  //blizzard
                            case 76:  //stone rain
                            case 134: //wave of fire
                            case 137: //wave of ice
                            case 142: //wave of rocks
                            case 190: //chain hallow
                            case 193: //chain pain
                            case 196: //chain lifetap
                            case 201: //chain mutation
                            case 202: //chain fireburst
                            case 204: //chain fireball
                            case 205: //chain iceburst
                            case 208: //chain rock bullet
                            case 212: //chain charm
                            case 214: //chain shock
                            case 217: //chain manatap
                                searched_id = (int)((UInt32)elem.get_single_variant(24).value);   //parameter #4
                                break;
                            case 88:  //aura of weakness
                            case 89:  //aura of suffocation
                            case 91:  //aura of lifetap
                            case 94:  //aura of slow fighting
                            case 95:  //aura of inflexibility
                            case 97:  //aura of slow walking
                            case 98:  //aura of inability
                            case 102: //aura of strength
                            case 103: //aura of healing
                            case 104: //aura of endurance
                            case 107: //aura of regeneration
                            case 110: //aura of fast fighting
                            case 111: //aura of flexibility
                            case 113: //aura of fast walking
                            case 114: //aura of light
                            case 115: //aura of dexterity
                            case 127: //aura of hypnotization
                            case 129: //aura of brilliance
                            case 131: //aura of manatap
                            case 192: //aura of eternity
                            case 223: //aura siege human
                            case 225: //aura siege elf
                            case 226: //aura siege orc
                            case 227: //aura siege troll
                            case 228: //aura siege darkelf
                                searched_id = (int)((UInt32)elem.get_single_variant(27).value);   //parameter #7
                                break;
                            default:
                                break;
                        }
                        if (searched_id == id_looked_for)
                            results_intermediate.Add(i);
                    }
                    break;

                case -2:    //unit id in category 1 (0)
                    //search for all spell references in category 0
                    SFCategory ctg2 = manager.get_category(0);
                    int id_looked_for2 = (int)((UInt16)manager.get_category(element_looked_for.category).get_element(element_looked_for.element).get_single_variant(0).value);
                    for (int i = 0; i < ctg2.get_element_count(); i++)
                    {
                        SFCategoryElement elem = ctg2.get_element(i);

                        int searched_id = -1;
                        UInt16 spell_type = (UInt16)elem.get_single_variant(1).value;
                        switch (spell_type)
                        {
                            case 20:  //summon undead goblin
                            case 29:  //summon skeleton
                            case 92:  //summon spectre
                            case 106: //summon wolf
                            case 109: //summon bear
                            case 133: //fire elemental
                            case 136: //ice elemental
                            case 141: //earth elemental
                            case 188: //treewraith
                            case 198: //summon blade
                            case 203: //summon fire golem
                            case 206: //summon ice golem
                            case 209: //summon stonee golem
                                searched_id = (int)((UInt32)elem.get_single_variant(23).value);   //parameter #3
                                break;
                            default:
                                break;
                        }
                        if (searched_id == id_looked_for2)
                            results_intermediate.Add(i);
                    }
                    break;

                default:    //literally everything else
                    results_intermediate = SFSearchModule.Search(manager.get_category(column_looked_at.category), preload_indices, manager.get_category(element_looked_for.category).get_element_variant(element_looked_for.element, 0).value.ToString(), SearchType.TYPE_NUMBER, column_looked_at.column, null);
                    break;
            }

            foreach(int elem_i in results_intermediate)
                elements_so_far.Add(new CatElem(column_looked_at.category, elem_i));
        }

        private void fill_listbox(ListBox lb, List<CatElem> elems)
        {
            lb.Items.Clear();
            foreach (CatElem elem in elems)
                lb.Items.Add(get_catelement_string(elem));
        }

        private void find_all_references()
        {
            if (sf_form == null)
                return;

            elements.Clear();
            int category = referenced.category;

            System.Diagnostics.Debug.WriteLine("working...");
            foreach(CatColumn column in ReferenceCategoryTable[category])
                add_entries(elements, referenced, column);

            fill_listbox(listBox1, elements);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;

            CatElem elem = elements[listBox1.SelectedIndex];

            sf_form.Tracer_StepForward(elem.category, elem.element);
        }

        private void ReferencesForm_Shown(object sender, EventArgs e)
        {
            find_all_references();
        }
    }
}
