/*
 * This form allows user to find all references to currently selected game data element
 * References are displayed on a list, user can choose elements from that list to edit them
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using SpellforceDataEditor.SFCFF;



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
        
        CatElem referenced;
        private List<CatElem> elements = new List<CatElem>();
        private static Dictionary<int, CatColumn[]> ReferenceCategoryTable;

        public ReferencesForm()
        {
            InitializeComponent();
            //reference category table
            ReferenceCategoryTable = new Dictionary<int, CatColumn[]>();
            ReferenceCategoryTable.Add(2002, new CatColumn[] { new CatColumn(2002, -1), new CatColumn(2067, 2), new CatColumn(2014, 2), new CatColumn(2018, 1), new CatColumn(2026, 2) });  //-1 means custom columns
            ReferenceCategoryTable.Add(2054, new CatColumn[] { new CatColumn(2002, 1) });
            ReferenceCategoryTable.Add(2056, new CatColumn[] { });
            ReferenceCategoryTable.Add(2005, new CatColumn[] { new CatColumn(2006, 0), new CatColumn(2067, 0), new CatColumn(2003, 4), new CatColumn(2024, 2) });
            ReferenceCategoryTable.Add(2006, new CatColumn[] { new CatColumn(2005, 0), new CatColumn(2067, 0), new CatColumn(2003, 4), new CatColumn(2024, 2) });
            ReferenceCategoryTable.Add(2067, new CatColumn[] { new CatColumn(2005, 0), new CatColumn(2006, 0), new CatColumn(2003, 4), new CatColumn(2024, 2) });
            ReferenceCategoryTable.Add(2003, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2004, new CatColumn[] { new CatColumn(2003, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1),  new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2013, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2003, 0), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2015, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2003, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2017, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2003, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2014, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2003, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2012, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2003, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2018, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2003, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2016, new CatColumn[] { new CatColumn(2054, 1), new CatColumn(2003, 3), new CatColumn(2022, 7), new CatColumn (17, 1), new CatColumn(2029, 5), new CatColumn(2039, 2), new CatColumn(2044, 1), new CatColumn(2050, 1), new CatColumn(2051, 1), new CatColumn(2052, 3), new CatColumn(2053, 5), new CatColumn(2058, 1), new CatColumn(2059, 1), new CatColumn(2059, 2), new CatColumn(2061, 3), new CatColumn(2063, 1), new CatColumn(2064, 1), new CatColumn(2036, 2), new CatColumn(2072, 1)  });
            ReferenceCategoryTable.Add(2022, new CatColumn[] { new CatColumn(2005, 2) });
            ReferenceCategoryTable.Add(2023, new CatColumn[] { new CatColumn(2022, 9) });
            ReferenceCategoryTable.Add(2024, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2025, 0), new CatColumn(2026, 0), new CatColumn(2028, 0), new CatColumn(2040, 0), new CatColumn(2001, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2025, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2024, 0), new CatColumn(2026, 0), new CatColumn(2028, 0), new CatColumn(2040, 0), new CatColumn(2001, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2026, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2025, 0), new CatColumn(2024, 0), new CatColumn(2028, 0), new CatColumn(2040, 0), new CatColumn(2001, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2028, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2025, 0), new CatColumn(2026, 0), new CatColumn(2024, 0), new CatColumn(2040, 0), new CatColumn(2001, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2040, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2025, 0), new CatColumn(2026, 0), new CatColumn(2028, 0), new CatColumn(2024, 0), new CatColumn(2001, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2001, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2025, 0), new CatColumn(2026, 0), new CatColumn(2028, 0), new CatColumn(2040, 0), new CatColumn(2024, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2029, new CatColumn[] { new CatColumn(2003, 6), new CatColumn(2001, 2), new CatColumn(2029, 10), new CatColumn(2030, 0), new CatColumn(2031, 0), new CatColumn(2036, 1) });
            ReferenceCategoryTable.Add(2030, new CatColumn[] { new CatColumn(2003, 6), new CatColumn(2001, 2), new CatColumn(2029, 10), new CatColumn(2029, 0), new CatColumn(2031, 0), new CatColumn(2036, 1) });
            ReferenceCategoryTable.Add(2031, new CatColumn[] { new CatColumn(2003, 6), new CatColumn(2001, 2), new CatColumn(2029, 10), new CatColumn(2030, 0), new CatColumn(2029, 0), new CatColumn(2036, 1) });
            ReferenceCategoryTable.Add(2039, new CatColumn[] { });
            ReferenceCategoryTable.Add(2062, new CatColumn[] { });
            ReferenceCategoryTable.Add(2041, new CatColumn[] { new CatColumn(2042, 0), new CatColumn(2047, 0) });
            ReferenceCategoryTable.Add(2042, new CatColumn[] { new CatColumn(2041, 0), new CatColumn(2047, 0) });
            ReferenceCategoryTable.Add(2047, new CatColumn[] { new CatColumn(2042, 0), new CatColumn(2041, 0) });
            ReferenceCategoryTable.Add(2044, new CatColumn[] { new CatColumn(2028, 1), new CatColumn(2031, 1) });
            ReferenceCategoryTable.Add(2048, new CatColumn[] { });
            ReferenceCategoryTable.Add(2050, new CatColumn[] { new CatColumn(2057, 0), new CatColumn(2065, 0) });
            ReferenceCategoryTable.Add(2057, new CatColumn[] { new CatColumn(2050, 0), new CatColumn(2065, 0) });
            ReferenceCategoryTable.Add(2065, new CatColumn[] { new CatColumn(2050, 0), new CatColumn(2057, 0) });
            ReferenceCategoryTable.Add(2051, new CatColumn[] { });
            ReferenceCategoryTable.Add(2052, new CatColumn[] { new CatColumn(2053, 1) });
            ReferenceCategoryTable.Add(2053, new CatColumn[] { });
            ReferenceCategoryTable.Add(2055, new CatColumn[] { });
            ReferenceCategoryTable.Add(2058, new CatColumn[] { new CatColumn(2054, 8), new CatColumn(2036, 3) });
            ReferenceCategoryTable.Add(2059, new CatColumn[] { });
            ReferenceCategoryTable.Add(2061, new CatColumn[] { new CatColumn(2061, 1) });
            ReferenceCategoryTable.Add(2063, new CatColumn[] { new CatColumn(2015, 6) });
            ReferenceCategoryTable.Add(2064, new CatColumn[] { new CatColumn(2015, 7) });
            ReferenceCategoryTable.Add(2032, new CatColumn[] { });
            ReferenceCategoryTable.Add(2049, new CatColumn[] { });
            ReferenceCategoryTable.Add(2036, new CatColumn[] { });
            ReferenceCategoryTable.Add(2072, new CatColumn[] { new CatColumn(2003, 10) });
        }

        public void set_referenced_element(int c, int e)
        {
            referenced = new CatElem(c, e);

            labelRefElemName.Text = get_catelement_string(referenced);

            find_all_references();
        }

        private string get_catelement_string(CatElem ce)
        {
            return "Category " + ce.category.ToString() + " | " + MainForm.data.CachedElementDisplays[ce.category].get_element_string(ce.element);
        }

        private void find_all_references()
        {
            elements.Clear();
            int category = referenced.category;

            foreach (CatColumn column in ReferenceCategoryTable[category])
                add_entries(elements, referenced, column);

            fill_listbox(listBox1, elements);
        }

        private void add_entries(List<CatElem> elements_so_far, CatElem element_looked_for, CatColumn column_looked_at)
        {
            List<int> preload_indices = new List<int>();
            for (int i = 0; i < SFCategoryManager.gamedata[column_looked_at.category].GetElementCount(); i++)
                preload_indices.Add(i);

            List<int> results_intermediate = new List<int>();
            switch (column_looked_at.column)
            {
                case -1:    //spell id in category 1 (0)
                    //search for all spell references in category 0
                    SFCategory ctg = SFCategoryManager.gamedata[2002];
                    int id_looked_for = (int)((UInt16)SFCategoryManager.gamedata[element_looked_for.category][element_looked_for.element][0]);
                    for(int i = 0; i < ctg.GetElementCount(); i++)
                    {
                        SFCategoryElement elem = ctg[i];

                        int searched_id = -1;
                        UInt16 spell_type = (UInt16)elem[1];
                        switch (spell_type)
                        {
                            case 176: //elemental essence
                            case 180: //elemental almightness
                                searched_id = (int)((UInt32)elem[21]);   //parameter #1
                                break;
                            case 12:  //fireshield (cast)
                            case 15:  //iceshield (cast)
                            case 47:  //thornshield
                            case 175: //white essence
                            case 179: //white almightness
                                searched_id = (int)((UInt32)elem[22]);   //parameter #2
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
                                searched_id = (int)((UInt32)elem[24]);   //parameter #4
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
                                searched_id = (int)((UInt32)elem[27]);   //parameter #7
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
                    SFCategory ctg2 = SFCategoryManager.gamedata[2002];
                    int id_looked_for2 = (int)((UInt16)SFCategoryManager.gamedata[element_looked_for.category][element_looked_for.element][0]);
                    for (int i = 0; i < ctg2.GetElementCount(); i++)
                    {
                        SFCategoryElement elem = ctg2[i];

                        int searched_id = -1;
                        UInt16 spell_type = (UInt16)elem[1];
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
                                searched_id = (int)((UInt32)elem[23]);   //parameter #3
                                break;
                            default:
                                break;
                        }
                        if (searched_id == id_looked_for2)
                            results_intermediate.Add(i);
                    }
                    break;

                default:    //literally everything else
                    results_intermediate = SFSearchModule.Search(SFCategoryManager.gamedata[column_looked_at.category], 
                                                                 preload_indices, 
                                                                 SFCategoryManager.gamedata[element_looked_for.category][element_looked_for.element][0].ToString(), 
                                                                 SearchType.TYPE_NUMBER, 
                                                                 column_looked_at.column, 
                                                                 null);
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;

            CatElem elem = elements[listBox1.SelectedIndex];

            MainForm.data.Tracer_StepForward(elem.category, elem.element);
        }
    }
}
