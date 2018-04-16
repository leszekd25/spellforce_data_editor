using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    //this class is responsible for category management
    //it provides with general functions to perform on categories as a database
    public class SFCategoryManager
    {
        private SFCategory[] categories;      //array of categories
        private int categoryNumber;           //amount of categories (basically a constant)
        private Byte[] mainHeader;            //gamedata.cff has a main header which is held here

        //constructor, it creates categories
        public SFCategoryManager()
        {
            categoryNumber = 49;
            categories = new SFCategory[categoryNumber];
            for (int i = 1; i <= categoryNumber; i++)
            {
                categories[i - 1] = Assembly.GetExecutingAssembly().CreateInstance("SpellforceDataEditor.SFCategory" + i.ToString()) as SFCategory;
            }
            mainHeader = new Byte[20];
        }

        //returns category, given its index
        public SFCategory get_category(int index)
        {
            return categories[index];
        }

        //loads gamedata.cff file
        public void load_cff(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs, Encoding.ASCII);

            mainHeader = br.ReadBytes(mainHeader.Length);
            for (int i = 0; i < categoryNumber; i++)
            {
                get_category(i).read(br);
            }

            br.Close();
            fs.Close();
        }

        //saves gamedata.cff file
        public void save_cff(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            fs.SetLength(0);
            BinaryWriter bw = new BinaryWriter(fs, Encoding.ASCII);

            bw.Write(mainHeader);
            for (int i = 0; i < categoryNumber; i++)
            {
                get_category(i).write(bw);
            }

            bw.Close();
            fs.Close();
        }

        //returns category count
        public int get_category_number()
        {
            return categoryNumber;
        }

        //using the fact that all text data is saved in category 15 (ind 14)
        //searches for a text with a given ID and in a given language
        public SFCategoryElement find_element_text(int t_index, int t_lang)
        {
            int closest_index = categories[14].find_binary_element_index<UInt16>(0, (UInt16)t_index);
            int cur_index = closest_index;
            int backup_index = -1;
            while (true)
            {
                if (cur_index < 0)
                    break;
                if ((int)(UInt16)categories[14].get_element(cur_index).get_single_variant(0).value != t_index)
                    break;
                if ((int)(Byte)categories[14].get_element(cur_index).get_single_variant(1).value == t_lang)
                    return categories[14].get_element(cur_index);
                if (backup_index == -1)
                    if ((int)(Byte)categories[14].get_element(cur_index).get_single_variant(1).value == 0)
                        backup_index = cur_index;
                cur_index--;
            }
            cur_index = closest_index + 1;
            while (true)
            {
                if (cur_index >= categories[14].get_element_count())
                    break;
                if ((int)(UInt16)categories[14].get_element(cur_index).get_single_variant(0).value != t_index)
                    break;
                if ((int)(Byte)categories[14].get_element(cur_index).get_single_variant(1).value == t_lang)
                    return categories[14].get_element(cur_index);
                if (backup_index == -1)
                    if ((int)(Byte)categories[14].get_element(cur_index).get_single_variant(1).value == 0)
                        backup_index = cur_index;
                cur_index++;
            }
            return categories[14].get_element(backup_index);
        }

        //returns a name of a given effect
        //it can also add the effect level
        public string get_effect_name(UInt16 effect_id, bool effect_level = false)
        {
            SFCategoryElement effect_elem = get_category(0).find_binary_element<UInt16>(0, effect_id);
            if (effect_elem == null)
                return "<no name>";
            UInt16 spell_type = (UInt16)effect_elem.get_single_variant(1).value;
            SFCategoryElement spell_elem = get_category(1).find_binary_element<UInt16>(0, spell_type);
            int text_id = (int)(UInt16)spell_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            if (effect_level)
                txt += " level " + effect_elem.get_single_variant(4).value.ToString();
            return txt;
        }

        //returns a name of a given unit
        public string get_unit_name(UInt16 unit_id)
        {
            SFCategoryElement unit_elem = get_category(17).find_binary_element<UInt16>(0, unit_id);
            if (unit_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)unit_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        //returns a name of a given skill
        public string get_skill_name(Byte skill_major, Byte skill_minor, Byte skill_lvl)
        {
            int major_index = (int)(UInt16)get_category(26).find_element_index<Byte>(0, skill_major);
            if (major_index == 0)
                return "<unknown string>";
            int total_index = major_index + (int)skill_minor;
            int text_id_major = (int)(UInt16)get_category(26).get_element_variant(major_index, 2).value;
            SFCategoryElement txt_elem_major = find_element_text(text_id_major, 1);
            string txt_major = Utility.CleanString(txt_elem_major.get_single_variant(4));
            string txt_minor = "";
            if (skill_minor != 101)
            {
                int text_id_minor = (int)(UInt16)get_category(26).get_element_variant(total_index, 2).value;
                if ((text_id_minor != 0) && (major_index != total_index))
                {
                    SFCategoryElement txt_elem_minor = find_element_text(text_id_minor, 1);
                    txt_minor = Utility.CleanString(txt_elem_minor.get_single_variant(4));
                }
            }
            return txt_major + " " + txt_minor + " " + skill_lvl.ToString();
        }

        //returns a name of a given item
        public string get_item_name(UInt16 item_id)
        {
            SFCategoryElement item_elem = get_category(6).find_binary_element<UInt16>(0, item_id);
            if (item_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)item_elem.get_single_variant(3).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        //returns a name of a given building
        public string get_building_name(UInt16 building_id)
        {
            SFCategoryElement building_elem = get_category(23).find_binary_element<UInt16>(0, building_id);
            if (building_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)building_elem.get_single_variant(5).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        //returns a name of a given merchant
        public string get_merchant_name(UInt16 merchant_id)
        {
            SFCategoryElement merchant_elem = get_category(28).find_binary_element<UInt16>(0, merchant_id);
            if (merchant_elem == null)
                return "<no name>";
            return get_unit_name((UInt16)merchant_elem.get_single_variant(1).value);
        }

        //returns a name of a given object
        public string get_object_name(UInt16 object_id)
        {
            SFCategoryElement object_elem = get_category(33).find_binary_element<UInt16>(0, object_id);
            if (object_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)object_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        //returns a description given its id
        public string get_description_name(UInt16 desc_id)
        {
            SFCategoryElement desc_elem = get_category(40).find_binary_element<UInt16>(0, desc_id);
            if (desc_elem == null)
                return "<no name>";
            int text_id = (int)(UInt16)desc_elem.get_single_variant(1).value;
            SFCategoryElement text_elem = find_element_text(text_id, 1);
            string txt = Utility.CleanString(text_elem.get_single_variant(4));
            return txt;
        }

        //returns a list of indices
        //these indices correspond with all elements which contain given value in a given column
        //value is numeric in this query
        public List<int> query_by_column_numeric(int categoryindex, int columnindex, int value)
        {
            List<int> items = new List<int>();
            SFCategory cat = get_category(categoryindex);
            for (int i = 0; i < cat.get_element_count(); i++)
            {
                SFVariant variant = cat.get_element_variant(i, columnindex);
                int current_value = variant.to_int();
                if (current_value == value)
                    items.Add(i);
            }
            return items;
        }

        //returns a list of indices
        //these indices correspond with all elements which contain given value in a given column
        //value is a text in this query
        public List<int> query_by_column_text(int categoryindex, int columnindex, string value)
        {
            List<int> items = new List<int>();
            SFCategory cat = get_category(categoryindex);
            for (int i = 0; i < cat.get_element_count(); i++)
            {
                SFVariant variant = cat.get_element_variant(i, columnindex);
                string current_value = Utility.CleanString(variant);
                if (current_value.Contains(value))
                    items.Add(i);
            }
            return items;
        }

        //frees all data, only empty categories remain
        public void unload_all()
        {
            foreach (SFCategory cat in categories)
                cat.unload();
            mainHeader = new Byte[20];
        }
    }
}
