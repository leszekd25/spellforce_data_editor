/*
 * This form allows user to find all references to currently selected game data element
 * References are displayed on a list, user can choose elements from that list to edit them
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using SpellforceDataEditor.SFCFF;
using SFEngine.SFCFF;



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
        
        CatElem referenced;
        private List<CatElem> elements = new List<CatElem>();

        public ReferencesForm()
        {
            InitializeComponent();
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

            foreach (var column in SFGameData.ReferenceCategoryTable[category])
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
                    if (SFCategoryManager.gamedata[element_looked_for.category].category_allow_multiple)
                    {
                        results_intermediate = SFSearchModule.Search(SFCategoryManager.gamedata[column_looked_at.category],
                                                                     preload_indices,
                                                                     SFCategoryManager.gamedata[element_looked_for.category].element_lists[element_looked_for.element].GetID().ToString(),
                                                                     SearchType.TYPE_NUMBER,
                                                                     column_looked_at.column,
                                                                     null);
                    }
                    else
                    {
                        results_intermediate = SFSearchModule.Search(SFCategoryManager.gamedata[column_looked_at.category],
                                                                     preload_indices,
                                                                     SFCategoryManager.gamedata[element_looked_for.category][element_looked_for.element][0].ToString(),
                                                                     SearchType.TYPE_NUMBER,
                                                                     column_looked_at.column,
                                                                     null);
                    }
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
