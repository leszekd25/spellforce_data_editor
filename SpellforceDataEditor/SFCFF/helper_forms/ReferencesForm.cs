using NAudio.CoreAudioApi;
using SFEngine.SFCFF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.helper_forms
{
    public partial class ReferencesForm : Form
    {
        private struct CatElem: IEquatable<CatElem>, IComparable<CatElem> 
        {
            public int category_id;
            public int element_index;   // only used during set_element
            public int element_id;      // used everywhere else

            public int CompareTo(CatElem other)
            {
                // A null value means that this object is greater.
                int r1 = category_id.CompareTo(other.category_id);
                if(r1 == 0)
                {
                    return element_index.CompareTo(other.element_index);
                }
                return r1;
            }
            public override int GetHashCode()
            {
                return category_id * 65536 + element_index;
            }
            public bool Equals(CatElem other)
            {
                return (category_id == other.category_id) && (element_index == other.element_index);
            }
        }

        static Dictionary<int, List<int>> category_ids = new();
        static Dictionary<int, int> category_redirect = new();
        static Dictionary<int, List<(int, string)>> category_fields = new();

        static ReferencesForm()
        {
            // add ID fields
            List<int> unit_stats = new() { 2005, 2006, 2067 };
            List<int> items = new() { 2003, 2004, 2013, 2015, 2017, 2014, 2012, 2018 };
            List<int> units = new() { 2024, 2025, 2026, 2028, 2040, 2001 };
            List<int> buildings = new() { 2029, 2030, 2031 };
            List<int> merchants = new() { 2041, 2042, 2047 };
            List<int> objects = new() { 2050, 2057, 2065 };

            foreach (List<int> l in new List<List<int>>(){ unit_stats, items, units, buildings, merchants, objects })
            {
                foreach(var i in l)
                {
                    category_ids.Add(i, l);
                }
            }

            // add remaining fields
            category_fields.Add(2002, new() { (2002, "<EXT1>"), (2067, "SpellID"), (2014, "EffectID"), (2018, "EffectID"), (2026, "SpellID") });
            category_fields.Add(2054, new() { (2002, "SpellLineID") });
            category_fields.Add(2005, new() { (2003, "UnitStatsID"), (2024, "StatsID") });
            category_redirect.Add(2006, 2005);
            category_redirect.Add(2067, 2005);
            category_fields.Add(2003, new() { (2013, "InstalledScrollItemID"), (2025, "ItemID"), (2040, "ItemID1"), (2040, "ItemID2"), (2040, "ItemID3"), (2042, "ItemID"), (2065, "ItemID1"), (2065, "ItemID2"), (2065, "ItemID3") });
            category_redirect.Add(2004, 2003);
            category_redirect.Add(2013, 2003);
            category_redirect.Add(2015, 2003);
            category_redirect.Add(2017, 2003);
            category_redirect.Add(2014, 2003);
            category_redirect.Add(2012, 2003);
            category_redirect.Add(2018, 2003);
            category_fields.Add(2016, new() { (2054, "TextID"), (2003, "NameID"), (2022, "TextID"), (2024, "NameID"), (2029, "NameID"), (2039, "TextID"), (2044, "TextID"), (2050, "NameID"), (2051, "TextID"), (2052, "NameID"), (2053, "NameID"), (2058, "TextID"), (2059, "TextID"), (2059, "ExtTextID"), (2061, "NameID"), (2061, "DescriptionID"), (2063, "NameID"), (2064, "NameID"), (2036, "ButtonNameID"), (2072, "DescriptionID") });
            category_fields.Add(2022, new() { (2005, "UnitRace") });
            category_fields.Add(2023, new() { (2022, "FactionID") });
            category_fields.Add(2024, new() { (2002, "<EXT2>"), (2003, "ArmyUnitID"), (2041, "UnitID") });
            category_redirect.Add(2025, 2024);
            category_redirect.Add(2026, 2024);
            category_redirect.Add(2028, 2024);
            category_redirect.Add(2040, 2024);
            category_redirect.Add(2001, 2024);
            category_fields.Add(2029, new() { (2003, "BuildingID"), (2001, "BuildingID"), (2029, "BuildingReqID"), (2036, "BuildingID") });
            category_redirect.Add(2030, 2029);
            category_redirect.Add(2031, 2029);
            category_fields.Add(2044, new() { (2028, "ResourceType"), (2031, "ResourceID") });
            category_fields.Add(2052, new() { (2053, "MapID") });
            category_fields.Add(2058, new() { (2054, "DescriptionID"), (2036, "ButtonDescriptionID") });
            category_fields.Add(2061, new() { (2061, "ParentQuestID") });
            category_fields.Add(2063, new() { (2015, "WeaponType") });
            category_fields.Add(2064, new() { (2015, "WeaponMaterial") });
            category_fields.Add(2072, new() { (2003, "ItemSetID") });

        }

        private List<CatElem> elements = new List<CatElem>();

        public ReferencesForm()
        {
            InitializeComponent();
        }

        public void FindElementReferences(int cat_id, int elem_index)
        {
            listBox1.Items.Clear();
            elements.Clear();

            if (!MainForm.data.data_loaded)
            {
                return;
            }

            ICategory elem_cat = MainForm.data.CachedElementDisplays[cat_id].category;
            elem_cat.GetID(elem_index, out int elem_id);


            HashSet<CatElem> tmp_elems = new();

            // find elements by ids
            if (category_ids.ContainsKey(cat_id))
            {
                foreach(int cat_id2 in category_ids[cat_id])
                {
                    ICategory cat = MainForm.data.CachedElementDisplays[cat_id2].category;
                    if(cat.GetItemIndex(elem_id, out int index))
                    {
                        tmp_elems.Add(new() { category_id = cat_id2, element_index = index, element_id = elem_id });
                    }
                }
            }

            // find elements by fields
            int ref_cat = cat_id;
            if (!category_fields.ContainsKey(cat_id))
            {
                if(category_redirect.ContainsKey(cat_id))
                {
                    ref_cat = category_redirect[cat_id];
                }
                else
                {
                    ref_cat = SFEngine.Utility.NO_INDEX;
                }
            }
            if(ref_cat != SFEngine.Utility.NO_INDEX)
            {
                foreach(var catfield in category_fields[ref_cat])
                {
                    ICategory cat = MainForm.data.CachedElementDisplays[catfield.Item1].category;
                    foreach(int found_index in cat.QueryItems(elem_id, catfield.Item2, SearchOption.IS_NUMBER))
                    {
                        cat.GetID(found_index, out int found_id);
                        tmp_elems.Add(new() { category_id = catfield.Item1, element_index = found_index, element_id = found_id });
                    }
                }
            }

            elements = tmp_elems.ToList();
            elements.Sort();

            for(int i = 0; i < elements.Count; i++)
            {
                ICategory cat = MainForm.data.CachedElementDisplays[elements[i].category_id].category;
                listBox1.Items.Add($"[{cat.GetName()}] {MainForm.data.CachedElementDisplays[elements[i].category_id].get_element_string(elements[i].element_index)}");
            }
            LabelRefNum.Text = $"Found {elements.Count} reference(s)";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                return;
            }

            CatElem elem = elements[listBox1.SelectedIndex];

            MainForm.data.trace_id(elem.category_id, elem.element_id);
        }
    }
}
