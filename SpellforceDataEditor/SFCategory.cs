using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    //this class implements methods for all kinds of manipulation of its elements
    //each category holds elements of single type
    public abstract class SFCategory
    {
        protected uint block_length;                    //size of all data that belongs to this category
        protected List<SFCategoryElement> elements;     //list of all elements
        protected string elem_format;                   //element format (see get_single_variant)
        protected string category_name;   
        protected int[] string_size;                    //if category element holds a string (one or more), a list of string lengths is required
        protected int current_string;                   //helper variable to enable searching and manipulating string variants
        protected Byte[] categoryHeader;                //each category starts with a header
        protected uint category_id;                      //each category has a unique id the game looks for when reading data
        protected SFCategoryManager manager;

        //constructor 
        public SFCategory()
        {
            categoryHeader = new Byte[12];
            string_size = new int[1] { 0 };
        }

        //getter/setter for manager property
        public SFCategoryManager get_manager()
        {
            return manager;
        }

        public void set_manager(SFCategoryManager m)
        {
            manager = m;
        }

        //initialization, sets format for an element
        protected void initialize(string fm)
        {
            elem_format = fm;
        }

        //returns category name
        public string get_name()
        {
            return category_name;
        }

        //returns a new empty element for this category (used for adding new elements)
        public SFCategoryElement generate_empty_element()
        {
            current_string = 0;
            SFCategoryElement elem = new SFCategoryElement();
            foreach (char c in elem_format)
            {
                switch (c)
                {
                    case 'b':
                        elem.add_single_variant((SByte)0);
                        break;
                    case 'B':
                        elem.add_single_variant((Byte)0);
                        break;
                    case 'h':
                        elem.add_single_variant((Int16)0);
                        break;
                    case 'H':
                        elem.add_single_variant((UInt16)0);
                        break;
                    case 'i':
                        elem.add_single_variant((Int32)0);
                        break;
                    case 'I':
                        elem.add_single_variant((UInt32)0);
                        break;
                    case 'f':
                        elem.add_single_variant((Single)0);
                        break;
                    case 's':
                        elem.add_single_variant(new char[string_size[current_string]]);
                        current_string = Math.Min(string_size.Length - 1, current_string + 1);
                        break;
                    default:
                        elem.add_single_variant(new object());
                        break;
                }
            }
            return elem;
        }

        //retrieves next variant from a buffer, given a type (indicated by a character contained in a format)
        //s_size refers to a string length (for if the variant holds a string)
        public Object get_single_variant(BinaryReader sr, char t, int s_size)
        {
            switch (t)
            {
                case 'b':
                    return sr.ReadSByte();
                case 'B':
                    return sr.ReadByte();
                case 'h':
                    return sr.ReadInt16();
                case 'H':
                    return sr.ReadUInt16();
                case 'i':
                    return sr.ReadInt32();
                case 'I':
                    return sr.ReadUInt32();
                case 'f':
                    return sr.ReadSingle();
                case 's':
                    current_string = Math.Min(string_size.Length - 1, current_string + 1);
                    return sr.ReadChars(s_size);
                default:
                    return null;
            }
        }

        //puts a single variant to a buffer
        //s_size refers to a string length (for if the variant holds a string)
        public void put_single_variant(BinaryWriter sw, SFVariant var, int s_size)
        {
            switch (var.vtype)
            {
                case TYPE.Byte:
                    sw.Write((SByte)var.value);
                    break;
                case TYPE.UByte:
                    sw.Write((Byte)var.value);
                    break;
                case TYPE.Short:
                    sw.Write((Int16)var.value);
                    break;
                case TYPE.UShort:
                    sw.Write((UInt16)var.value);
                    break;
                case TYPE.Int:
                    sw.Write((Int32)var.value);
                    break;
                case TYPE.UInt:
                    sw.Write((UInt32)var.value);
                    break;
                case TYPE.Float:
                    sw.Write((Single)var.value);
                    break;
                case TYPE.String:
                    sw.Write((char[])var.value);
                    break;
                default:
                    break;
            }
        }

        //retrieves next element (sequence of variants as an array of objects) from a buffer
        public virtual Object[] get_element(BinaryReader sr)
        {
            current_string = 0;
            Object[] objs = new Object[elem_format.Length];
            for (int i = 0; i < elem_format.Length; i++)
            {
                objs[i] = get_single_variant(sr, elem_format[i], string_size[current_string]);
            }
            return objs;
        }

        //returns an element given element index, or null if it doesn't exist
        public SFCategoryElement get_element(int index)
        {
            if ((index >= 0) && (index < elements.Count))
                return elements[index];
            return null;
        }

        //returns list of elements the category holds
        public List<SFCategoryElement> get_elements()
        {
            return elements;
        }

        //returns a single variant provided element index and variant index
        public SFVariant get_element_variant(int elem_index, int var_index)
        {
            if (elem_index >= elements.Count)
                return null;
            return elements[elem_index].get_single_variant(var_index);
        }

        //searches for an element given column index and searched value and returns it if it exists
        //else returns null
        public SFCategoryElement find_element<T>(int v_index, T value) where T : IComparable
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (((T)get_element(i).get_single_variant(v_index).value).CompareTo(value) == 0)
                    return get_element(i);
            }
            return null;
        }

        //searches for an element given column index and searched value and returns its index if it exists
        //else returns -1
        public int find_element_index<T>(int v_index, T value) where T : IComparable
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (((T)get_element(i).get_single_variant(v_index).value).CompareTo(value) == 0)
                    return i;
            }
            return -1;
        }

        //searches for an element given column index and searched value and returns it if it exists
        //else returns null
        //this is binary search variant, and it requires that elements are sorted by given column
        public SFCategoryElement find_binary_element<T>(int v_index, T value) where T : IComparable
        {
            int current_start = 0;
            int current_end = elements.Count - 1;
            int current_center;
            T val;
            while (current_start <= current_end)
            {
                current_center = (current_start + current_end) / 2;    //care about overflow
                val = (T)get_element(current_center).get_single_variant(v_index).value;
                if (val.CompareTo(value) == 0)
                    return get_element(current_center);
                if (val.CompareTo(value) < 0)
                    current_start = current_center + 1;
                else
                    current_end = current_center - 1;
            }
            return null;
        }

        //searches for an element given column index and searched value and returns its index if it exists
        //else returns -1
        //this is binary search variant, and it requires that elements are sorted by given column
        public int find_binary_element_index<T>(int v_index, T value) where T : IComparable
        {
            int current_start = 0;
            int current_end = elements.Count - 1;
            int current_center;
            T val;
            while (current_start <= current_end)
            {

                current_center = (current_start + current_end) / 2;    //care about overflow
                val = (T)get_element(current_center).get_single_variant(v_index).value;
                if (val.CompareTo(value) == 0)
                    return current_center;
                if (val.CompareTo(value) < 0)
                    current_start = current_center + 1;
                else
                    current_end = current_center - 1;
            }
            return -1;
        }

        //finds text string given element and column index where the element holds text IDs
        public string get_text_from_element(SFCategoryElement elem, int cat_index)
        {
            if (elem == null)
                return "<no name>";
            else
            {
                int text_id = (int)(UInt16)elem.get_single_variant(cat_index).value;
                SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
                if (txt_elem != null)
                    return Utility.CleanString(txt_elem.get_single_variant(4));
                else
                    return "<text missing>";
            }
        }

        //sets a single variant given element index and variant index
        public void set_element_variant(int elem_index, int var_index, object obj)
        {
            if (elem_index < elements.Count)
                elements[elem_index].get()[var_index].set(obj);
        }

        //puts a new element (as a list of variants) to a buffer
        public void put_element(BinaryWriter sw, List<SFVariant> vars)
        {
            current_string = 0;
            for (int i = 0; i < vars.Count; i++)
            {
                put_single_variant(sw, vars[i], string_size[current_string]);
            }
        }

        //returns size of all category elements (in bytes)
        public int get_size()
        {
            int s = 0;
            foreach(SFCategoryElement elem in elements)
            {
                s += elem.get_size();
            }
            return s;
        }

        //reads a buffer and retrieves all expected elements
        public int read(BinaryReader sr)
        {
            categoryHeader = sr.ReadBytes(categoryHeader.Length);

            bool bad_header = false;
            uint read_id = BitConverter.ToUInt32(categoryHeader, 0);
            if (read_id != category_id)
                bad_header = true;

            block_length = BitConverter.ToUInt32(categoryHeader, 6);
            elements = new List<SFCategoryElement>();
            Byte[] block_buffer = new Byte[block_length];
            sr.Read(block_buffer, 0, (int)(block_length));
            /*string s = "";
            for (int i = 0; i < categoryHeader.Length; i++)
                s += categoryHeader[i].ToString() + " ";
            Console.WriteLine(category_name + " " + s);*/
            MemoryStream ms = new MemoryStream(block_buffer);
            BinaryReader mr = new BinaryReader(ms, Encoding.Default);
            while (mr.PeekChar() != -1)
            {
                SFCategoryElement elem = new SFCategoryElement();
                elem.set(get_element(mr));
                elements.Add(elem);
            }
            mr.Dispose();
            ms.Dispose();
            block_buffer = null;
            if (bad_header)
                return -1;
            return 0;
        }

        //inserts an element (given element index) into the buffer
        public void set_element(BinaryWriter sw, int elem_index)
        {
            current_string = 0;
            List<SFVariant> vars = get_element(elem_index).get();
            for (int i = 0; i < vars.Count; i++)
            {
                put_single_variant(sw, vars[i], string_size[current_string]);
                if(vars[i].vtype == TYPE.String)
                    current_string = Math.Min(string_size.Length - 1, current_string + 1);
            }
        }

        //inserts all elements into the buffer
        public void write(BinaryWriter sw)
        {
            UInt32 new_block_size = (UInt32)get_size();
            Utility.CopyUInt32ToByteArray(category_id, ref categoryHeader, 0);
            categoryHeader[4] = 0;
            categoryHeader[5] = 0;
            Utility.CopyUInt32ToByteArray(new_block_size, ref categoryHeader, 6);
            sw.Write(categoryHeader);
            for(int i = 0; i < get_element_count(); i++)
            {
                put_element(sw, elements[i].get());
            }
        }

        //manager is required to communicate with other categories to construct a short description
        public virtual string get_element_string(SFCategoryManager manager, int index)
        {
            return index.ToString();
        }

        public virtual string get_element_description(SFCategoryManager manager, int index)
        {
            return "";
        }

        //returns element count
        public int get_element_count()
        {
            return elements.Count;
        }

        //returns element format string
        public string get_element_format()
        {
            return elem_format;
        }

        //removes all elements and resets category
        public void unload()
        {
            elements.Clear();
            for (int i = 0; i < 12; i++)
                categoryHeader[i] = 0;
        }
    }




    //spells/skills (1st category)
    public class SFCategory1 : SFCategory
    {
        private string get_target_mode(Byte tm)
        {
            switch(tm)
            {
                case 1:
                    return "Figure";
                case 2:
                    return "Building";
                case 3:
                    return "Object";
                case 4:
                    return "in World";
                case 5:
                    return "in Area";
                default:
                    return "<no name>";
            }
        }

        private string get_target_type(Byte tm)
        {
            switch(tm)
            {
                case 1:
                    return "Enemy";
                case 2:
                    return "Ally";
                case 3:
                    return "Other";
                default:
                    return "<no name>";
            }
        }





        public SFCategory1() : base()
        {
            initialize("HHBBBBBBBBBBBBHIIHHBBIIIIIIIIIIBBBB");
            category_name = "1. Spell data";
            category_id = 2002;
        }

        //surprisingly ugly due to converting values in this function...
        //can this be done better?
        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 type_id = (UInt16)get_element_variant(index, 1).value;
            SFCategoryElement stype_elem = manager.get_category(1).find_binary_element<UInt16>(0, type_id);
            string stype_txt = get_text_from_element(stype_elem, 1);
            Byte spell_level = (Byte)get_element_variant(index, 4).value;
            return get_element_variant(index, 0).value.ToString() + " " + stype_txt + " level " + spell_level.ToString();
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            List<string> reqs = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                Byte skill_major = (Byte)get_element_variant(index, 2 + i * 3).value;
                Byte skill_minor = (Byte)get_element_variant(index, 3 + i * 3).value;
                Byte skill_level = (Byte)get_element_variant(index, 4 + i * 3).value;
                if (skill_major == 0)
                    break;
                reqs.Add(manager.get_skill_name(skill_major, skill_minor, skill_level));
            }
            string req_str = "";
            for(int i = 0; i < reqs.Count; i++)
            {
                req_str += reqs[i];
                req_str += "\r\n";
            }
            string target = "";
            target += get_target_type((Byte)get_element_variant(index, 19).value);
            target += " " + get_target_mode((Byte)get_element_variant(index, 20).value);
            return "Requirements:\r\n" + req_str + "Target: "+target;
        }
    }

    //spell ui (2nd category)
    public class SFCategory2 : SFCategory
    {
        public SFCategory2() : base()
        {
            string_size = new int[1] { 64 };   //must go before initialize
            initialize("HHBBBBBsH");
            category_name = "2. Spell type data";
            category_id = 2054;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            string stype_txt = get_text_from_element(elements[index], 1);
            return get_element_variant(index, 0).value.ToString() + " " + stype_txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            string spell_name = get_text_from_element(elements[index], 1);
            string spell_desc = manager.get_description_name((UInt16)get_element_variant(index, 8).value);
            return spell_name + "\r\n" + spell_desc;
        }
    }

    //unknown1 (3rd category)
    public class SFCategory3 : SFCategory
    {
        public SFCategory3() : base()
        {
            initialize("BBBBBB");
            category_name = "3. Unknown (1)";
            category_id = 2056;
        }
    }

    //unit/hero stats (4th category)
    public class SFCategory4 : SFCategory
    {
        public SFCategory4() : base()
        {
            initialize("HHBHHHHHHHBBHHHHHHHHBBIBHB");
            category_name = "4. Unit/hero stats";
            category_id = 2005;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 stats_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 stats_level = (UInt16)get_element_variant(index, 1).value;
            SFCategoryElement elem = manager.get_category(17).find_element<UInt16>(2, stats_id);
            string unit_txt = get_text_from_element(elem, 1);
            if (unit_txt == "<no name>")
                unit_txt = manager.get_runehero_name(stats_id);
            return stats_id.ToString() + " " + unit_txt + " (lvl " + stats_level.ToString() + ")";
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            string race_name = "";
            int hp = (int)(UInt16)get_element_variant(index, 7).value;
            int mana = (int)(UInt16)get_element_variant(index, 9).value;
            int lvl = ((int)(UInt16)get_element_variant(index, 1).value)-1;
            string stat_txt = "";
            SFCategoryElement lvl_elem = manager.get_category(32).get_element(lvl);
            if (lvl_elem != null)
            {
                hp *= (int)(UInt16)lvl_elem.get_single_variant(1).value;
                System.Diagnostics.Debug.WriteLine(hp);
                mana *= (int)(UInt16)lvl_elem.get_single_variant(2).value;
                hp /= 100;
                mana /= 100;
                stat_txt = "\r\nHealth: " + hp.ToString() + "\r\nMana: " + mana.ToString();
            }
            Byte race_id = (Byte)get_element_variant(index, 2).value;
            race_name = manager.get_race_name(race_id);
            return "This unit race: " + race_name + stat_txt; ;
        }
    }

    //hero magic/combat arms and worker gathering (5th category)
    public class SFCategory5 : SFCategory
    {
        public SFCategory5() : base()
        {
            initialize("HBBB");
            category_name = "5. Hero/worker skills";
            category_id = 2006;
        }

        public override Object[] get_element(BinaryReader sr)
        {
            List<Object[]> elements_for_single_unit = new List<Object[]>();
            int cur_unit_id = -1;
            while (true)
            {
                if (sr.PeekChar() == -1)
                    break;
                UInt16 next_unit_id = sr.ReadUInt16();
                sr.BaseStream.Seek(-2, SeekOrigin.Current);
                if ((next_unit_id == cur_unit_id) || (cur_unit_id == -1))
                {
                    cur_unit_id = next_unit_id;
                    Object[] objs = new Object[elem_format.Length];
                    for (int i = 0; i < elem_format.Length; i++)
                    {
                        objs[i] = get_single_variant(sr, elem_format[i], string_size[current_string]);
                    }

                    elements_for_single_unit.Add(objs);
                }
                else
                    break;
            }

            Object[] real_objs = new Object[elem_format.Length * elements_for_single_unit.Count];
            for (int i = 0; i < elements_for_single_unit.Count; i++)
            {
                for (int j = 0; j < elem_format.Length; j++)
                {
                    real_objs[i * elem_format.Length + j] = elements_for_single_unit[i][j];
                }
            }

            return real_objs;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 stats_id = (UInt16)get_element_variant(index, 0).value;
            SFCategoryElement elem = manager.get_category(17).find_element<UInt16>(2, stats_id);
            string unit_txt = get_text_from_element(elem, 1);
            if (unit_txt == "<no name>")
                unit_txt = manager.get_runehero_name(stats_id);
            return stats_id.ToString() + " " + unit_txt;
            
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            Byte skill_major = (Byte)get_element_variant(index, 1).value;
            Byte skill_minor = (Byte)get_element_variant(index, 2).value;
            Byte skill_level = (Byte)get_element_variant(index, 3).value;
            return "This unit skill: " + manager.get_skill_name(skill_major, skill_minor, skill_level);
        }
    }

    //hero skills/spells (6th category)
    public class SFCategory6 : SFCategory
    {
        public SFCategory6() : base()
        {
            initialize("HBH");
            category_name = "6. Hero spells";
            category_id = 2067;
        }

        public override Object[] get_element(BinaryReader sr)
        {
            List<Object[]> elements_for_single_unit = new List<Object[]>();
            int cur_unit_id = -1;
            while (true)
            {
                if (sr.PeekChar() == -1)
                    break;
                UInt16 next_unit_id = sr.ReadUInt16();
                sr.BaseStream.Seek(-2, SeekOrigin.Current);
                if ((next_unit_id == cur_unit_id) || (cur_unit_id == -1))
                {
                    cur_unit_id = next_unit_id;
                    Object[] objs = new Object[elem_format.Length];
                    for (int i = 0; i < elem_format.Length; i++)
                    {
                        objs[i] = get_single_variant(sr, elem_format[i], string_size[current_string]);
                    }

                    elements_for_single_unit.Add(objs);
                }
                else
                    break;
            }

            Object[] real_objs = new Object[elem_format.Length * elements_for_single_unit.Count];
            for (int i = 0; i < elements_for_single_unit.Count; i++)
            {
                for (int j = 0; j < elem_format.Length; j++)
                {
                    real_objs[i * elem_format.Length + j] = elements_for_single_unit[i][j];
                }
            }

            return real_objs;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 hero_id = (UInt16)get_element_variant(index, 0).value;
            string txt = manager.get_runehero_name(hero_id);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }
    }

    //item type/name ID/price (7th category)
    public class SFCategory7 : SFCategory
    {
        static public string[] item_types = { "Unknown", "Equipment", "Inventory rune", "Installed rune",
            "Spell scroll", "Equipped scroll", "Unit plan", "Building plan", "Equipped unit plan",
            "Equipped building plan", "Miscellaneous" };

        static public string[] equipment_types = { "Unknown", "Headpiece", "Chestpiece", "Legpiece", "Unknown", "Unknown", "Ring",
            "1H Weapon", "2H Weapon", "Shield", "Robe", "ItemChestFake (monsters)", "Ranged Weapon", "ItemChestFake (playable)" };

        public SFCategory7() : base()
        {
            initialize("HBBHHHHBIIB");
            category_name = "7. Item general info";
            category_id = 2003;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            string txt = get_text_from_element(elements[index], 3);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            string contains_text;
            string item_type_text = "";
            Byte item_type = (Byte)get_element_variant(index, 1).value;
            Byte bonus_type = (Byte)get_element_variant(index, 2).value;
            Byte special = (Byte)get_element_variant(index, 7).value;
            Byte set_type = (Byte)get_element_variant(index, 10).value;

            if ((item_type > 0) && (item_type < item_types.Length))
                item_type_text += item_types[item_type];
            /*switch (item_type)
            { }*/
            switch (item_type)
            {
                case 2:
                case 3:
                    UInt16 rune_id = (UInt16)get_element_variant(index, 4).value;
                    contains_text = manager.get_runehero_name(rune_id);
                    break;
                case 6:
                case 8:
                    UInt16 army_id = (UInt16)get_element_variant(index, 5).value;
                    contains_text = manager.get_unit_name(army_id);
                    break;
                case 7:
                case 9:
                    UInt16 building_id = (UInt16)get_element_variant(index, 6).value;
                    contains_text = manager.get_building_name(building_id);
                    break;
                default:
                    contains_text = "";
                    break;
            }

            if(item_type == 1)
            {
                string bonus_type_text = "Unknown";
                if ((bonus_type > 0) && (bonus_type < (Byte)equipment_types.Length))
                    bonus_type_text = equipment_types[(int)bonus_type];
                item_type_text += " (" + bonus_type_text + ")";
            }

            string total_text = item_type_text;
            if (contains_text != "")
            {
                contains_text += " (" + manager.get_race_name(bonus_type) + ")";
                //SFCategoryElement race_elem = manager.get_category
                total_text += "\r\nContains " + contains_text;
            }

            if(set_type != 0)
            {
                Byte elem_id = set_type;
                string txt;
                SFCategoryElement set_elem = manager.get_category(48).find_binary_element<Byte>(0, elem_id);
                if (set_elem == null)
                    txt = "<no name>";
                else
                {
                    SFCategoryElement txt_elem = manager.find_element_text((UInt16)(set_elem.get_single_variant(1).value), 1);
                    if (txt_elem == null)
                        txt = "<text missing>";
                    else
                        txt = Utility.CleanString(txt_elem.get_single_variant(4));
                }
                total_text += "\r\nPart of set: " + txt;
            }

            if (special == 4)
                total_text += "\r\nQuest item (can not be sold)";
            else if (special == 8)
                total_text += "\r\nQuest item (can be sold)";
            else if (special != 0)
                total_text += "\r\nUnknown optional data";
            return total_text;
        }
    }

    //armor class item stats (8th category)
    public class SFCategory8 : SFCategory
    {
        public SFCategory8() : base()
        {
            initialize("Hhhhhhhhhhhhhhhhhh");
            category_name = "8. Item armor data";
            category_id = 2004;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 item_id = (UInt16)get_element_variant(index, 0).value;
            string txt = manager.get_item_name(item_id);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }
    }

    //scroll ID with spell ID/rune ID with rune in slot... (9th category)
    public class SFCategory9 : SFCategory
    {
        public SFCategory9() : base()
        {
            initialize("HH");
            category_name = "9. Inventory spell scroll link with installed spell scroll";
            category_id = 2013;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 item_id1 = (UInt16)get_element_variant(index, 0).value;
            string txt1 = manager.get_item_name(item_id1);

            UInt16 item_id2 = (UInt16)get_element_variant(index, 1).value;
            string txt2 = manager.get_item_name(item_id2);

            return txt1 + " | " + txt2;
        }
    }

    //weapon class item sets (10th category)
    public class SFCategory10 : SFCategory
    {
        private float get_dmg(int min_dmg, int max_dmg, int sp)
        {
            Single mean = ((Single)min_dmg+(Single)max_dmg)/ 2;
            Single ratio = ((Single)sp) / 100;
            return mean * ratio;
        }

        public SFCategory10() : base()
        {
            initialize("HHHHHHHH");
            category_name = "10. Item weapon data";
            category_id = 2015;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 item_id = (UInt16)get_element_variant(index, 0).value;
            string txt = manager.get_item_name(item_id);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            UInt16 type_id = (UInt16)get_element_variant(index, 6).value;
            UInt16 material_id = (UInt16)get_element_variant(index, 7).value;         

            SFCategoryElement type_elem = manager.get_category(43).find_binary_element<UInt16>(0, type_id);
            string type_name = get_text_from_element(type_elem, 1);
            SFCategoryElement material_elem = manager.get_category(44).find_binary_element<UInt16>(0, material_id);
            string material_name = get_text_from_element(material_elem, 1);

            UInt16 min_dmg = (UInt16)get_element_variant(index, 1).value;
            UInt16 max_dmg = (UInt16)get_element_variant(index, 2).value;
            UInt16 spd = (UInt16)get_element_variant(index, 5).value;

            return "Weapon type: " + type_name
                + "\r\nWeapon material: " + material_name
                + "\r\nDamage per second: " + get_dmg((int)min_dmg, (int)max_dmg, (int)spd).ToString();
        }
    }

    //item requirements (11th category)
    public class SFCategory11 : SFCategory
    {
        public SFCategory11() : base()
        {
            initialize("HBBBB");
            category_name = "11. Item skill requirements";
            category_id = 2017;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 item_id = (UInt16)get_element_variant(index, 0).value;
            string txt = manager.get_item_name(item_id);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            Byte skill_major = (Byte)get_element_variant(index, 2).value;
            Byte skill_minor = (Byte)get_element_variant(index, 3).value;
            Byte skill_level = (Byte)get_element_variant(index, 4).value;
            Byte req_ind = (Byte)get_element_variant(index, 1).value;
            string req_txt = manager.get_skill_name(skill_major, skill_minor, skill_level);
            return "Requirement "+req_ind.ToString()+": "+req_txt;
        }
    }

    //item spell effects (12th category)
    public class SFCategory12 : SFCategory
    {
        public SFCategory12() : base()
        {
            initialize("HBH");
            category_name = "12. Item weapon effects/inventory scroll link with spell";
            category_id = 2014;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 item_id = (UInt16)get_element_variant(index, 0).value;
            string txt_item = manager.get_item_name(item_id);

            UInt16 effect_id = (UInt16)get_element_variant(index, 2).value;
            string txt_effect = manager.get_effect_name(effect_id, true);

            return get_element_variant(index, 0).value.ToString() + " " + txt_item + " | " + txt_effect;
        }
    }

    //item ui (13th category)
    public class SFCategory13 : SFCategory
    {
        public SFCategory13() : base()
        {
            string_size = new int[1] { 64 };
            initialize("HBsH");
            category_name = "13. Item UI data";
            category_id = 2012;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 item_id = (UInt16)get_element_variant(index, 0).value;
            string txt_item = manager.get_item_name(item_id);
            return get_element_variant(index, 0).value.ToString() + " " + txt_item;
        }
    }

    //spell item ID with spell number (14th category)
    public class SFCategory14 : SFCategory
    {
        public SFCategory14() : base()
        {
            initialize("HH");
            category_name = "14. Item installed spell scroll link with spell";
            category_id = 2018;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 item_id = (UInt16)get_element_variant(index, 0).value;
            string txt_item = manager.get_item_name(item_id);
            UInt16 effect_id = (UInt16)get_element_variant(index, 1).value;
            string txt_effect = manager.get_effect_name(effect_id, true);
            return get_element_variant(index, 0).value.ToString() + " " + txt_item + " | " + txt_effect;
        }
    }

    //text IDs (15th category)
    public class SFCategory15 : SFCategory
    {
        public SFCategory15() : base()
        {
            string_size = new int[2] { 50, 512 };
            initialize("HBBss");
            category_name = "15. Text data";
            category_id = 2016;
        }

        public override Object[] get_element(BinaryReader sr)
        {
            List<Object[]> elements_for_single_text = new List<object[]>();
            int cur_text_id = -1;
            while (true)
            {
                if (sr.PeekChar() == -1)
                    break;
                UInt16 next_text_id = sr.ReadUInt16();
                sr.BaseStream.Seek(-2, SeekOrigin.Current);
                if ((next_text_id == cur_text_id) || (cur_text_id == -1))
                {
                    cur_text_id = next_text_id;
                    current_string = 0;
                    Object[] objs = new Object[elem_format.Length];
                    for (int i = 0; i < elem_format.Length; i++)
                    {
                        objs[i] = get_single_variant(sr, elem_format[i], string_size[current_string]);
                    }

                    elements_for_single_text.Add(objs);
                }
                else
                    break;
            }

            Object[] real_objs = new Object[elem_format.Length * elements_for_single_text.Count];
            for(int i = 0; i < elements_for_single_text.Count; i++)
            {
                for(int j = 0; j < elem_format.Length; j++)
                {
                    real_objs[i * elem_format.Length + j] = elements_for_single_text[i][j];
                }
            }
            
            return real_objs;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            string txt;
            SFCategoryElement elem = manager.find_element_text((UInt16)(get_element_variant(index, 0).value), 1);
            if (elem == null)
                txt = "<no name>";
            else
                txt = Utility.CleanString(elem.get_single_variant(4));
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }
    }

    //race stats (16th category)
    public class SFCategory16 : SFCategory
    {
        static public string[] race_flags = new string[] { "Undead", "Natural?", "Animal (meat gathering)", "Animal", "Living being?", "?", "??", "??? (not used)" };

        public SFCategory16() : base()
        {
            initialize("BBBBBBBHBHBBHBBBBHHHB");
            category_name = "16. Race stats";
            category_id = 2022;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            string txt = get_text_from_element(elements[index], 7);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            Byte flags = (Byte)get_element_variant(index, 8).value;
            string flag_text = "Race flags: ";
            bool first_flag_set = false;
            for(int i = 0; i < 8; i++)
            {
                if(((flags >> i) & 0x1) == 0x1)
                {
                    if (first_flag_set)
                        flag_text += " | ";
                    flag_text += race_flags[i];
                    first_flag_set = true;
                }
            }

            return flag_text;
        }
    }

    //head stats (17th category)
    public class SFCategory17 : SFCategory
    {
        static public string[] clan_names = new string[] {
            "Neutral", "Friendly neutral [Humans]", "Friendly neutral [Elves]", "Neutral [animals for meat production]",
            "Friendly neutral [Dwarves]", "Hostile [Grargs]", "Hostile [Imperial]", "Hostile [Uroks]",
            "Hostile [Undead]", "Hostile [monsters/demons]", "Player", "Player Elves",
            "Player Humans", "Player Dwarves", "Player Orcs", "Player Trolls",
            "Player Darkelves", "Hostile [animals]", "KillAll", "Hostile [Beastmen]",
            "Hostile [Gorge]", "<unknown>", "<none>", "Hostile [Blades]",
            "<none>", "Hostile [Multiplayer enemies]", "Hostile [Ogres]", "Neutral [NPCs]",
            "Hostile [Soulforger]", "Hostile [Bloodash]", "<unknown>", "Hostile [Dervish]"};

        public SFCategory17() : base()
        {
            initialize("BBB");
            category_name = "17. Clan relations";
            category_id = 2023;
        }

        public override Object[] get_element(BinaryReader sr)
        {
            List<Byte[]> elements_for_single_clan = new List<Byte[]>();
            int cur_clan_id = -1;
            while (true)
            {
                if (sr.PeekChar() == -1)
                    break;
                Byte next_clan_id = sr.ReadByte();
                sr.BaseStream.Seek(-1, SeekOrigin.Current);
                if ((next_clan_id == cur_clan_id) || (cur_clan_id == -1))
                {
                    cur_clan_id = next_clan_id;
                    Byte[] objs = new Byte[elem_format.Length];
                    for (int i = 0; i < elem_format.Length; i++)
                    {
                        objs[i] = sr.ReadByte();
                    }

                    elements_for_single_clan.Add(objs);
                }
                else
                    break;
            }

            Object[] real_objs = new Object[elem_format.Length * elements_for_single_clan.Count];
            for (int i = 0; i < elements_for_single_clan.Count; i++)
            {
                for (int j = 0; j < elem_format.Length; j++)
                {
                    real_objs[i * elem_format.Length + j] = elements_for_single_clan[i][j];
                }
            }

            return real_objs;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            string txt = clan_names[(int)(Byte)(get_element_variant(index, 0).value)-1];
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }
    }

    //unit names (18th category)
    public class SFCategory18 : SFCategory
    {
        private int calculate_total_xp(UInt32 xp_gain, UInt16 xp_falloff)
        {
            if ((xp_gain == 0) || (xp_falloff == 0))
                return 0;
            int max_units = 500;
            int s = 0;
            for(int i = 0; i < max_units; i++)
            {
                s += (int)Math.Floor((Single)xp_gain * ((Single)(xp_falloff) / (Single)(xp_falloff + i)));
            }
            return s;
        }

        public SFCategory18() : base()
        {
            string_size = new int[1] { 40 };
            initialize("HHHIHIHBHHsB");
            category_name = "18. Unit general data/link with unit stats";
            category_id = 2024;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            string txt = get_text_from_element(elements[index], 1);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            UInt32 xp_gain = (UInt32)get_element_variant(index, 3).value;
            UInt16 xp_falloff = (UInt16)get_element_variant(index, 4).value;
            return "Max XP gained from this unit: " + calculate_total_xp(xp_gain, xp_falloff).ToString();
        }
    }

    //unit equipment (19th category)
    public class SFCategory19 : SFCategory
    {
        public SFCategory19() : base()
        {
            initialize("HBH");
            category_name = "19. Unit equipment";
            category_id = 2025;
        }

        public override Object[] get_element(BinaryReader sr)
        {
            List<Object[]> elements_for_single_unit = new List<Object[]>();
            int cur_unit_id = -1;
            while (true)
            {
                if (sr.PeekChar() == -1)
                    break;
                UInt16 next_unit_id = sr.ReadUInt16();
                sr.BaseStream.Seek(-2, SeekOrigin.Current);
                if ((next_unit_id == cur_unit_id) || (cur_unit_id == -1))
                {
                    cur_unit_id = next_unit_id;
                    Object[] objs = new Object[elem_format.Length];
                    for (int i = 0; i < elem_format.Length; i++)
                    {
                        objs[i] = get_single_variant(sr, elem_format[i], string_size[current_string]);
                    }

                    elements_for_single_unit.Add(objs);
                }
                else
                    break;
            }

            Object[] real_objs = new Object[elem_format.Length * elements_for_single_unit.Count];
            for (int i = 0; i < elements_for_single_unit.Count; i++)
            {
                for (int j = 0; j < elem_format.Length; j++)
                {
                    real_objs[i * elem_format.Length + j] = elements_for_single_unit[i][j];
                }
            }

            return real_objs;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 unit_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 item_id = (UInt16)get_element_variant(index, 2).value;
            string txt_unit = manager.get_unit_name(unit_id);
            string txt_item = manager.get_item_name(item_id);
            return unit_id.ToString() + " " + txt_unit + " | " + txt_item;
        }
    }

    //unit equspells/skills (20th category)
    public class SFCategory20 : SFCategory
    {
        public SFCategory20() : base()
        {
            initialize("HBH");
            category_name = "20. Unit spells";
            category_id = 2026;
        }

        public override Object[] get_element(BinaryReader sr)
        {
            List<Object[]> elements_for_single_unit = new List<Object[]>();
            int cur_unit_id = -1;
            while (true)
            {
                if (sr.PeekChar() == -1)
                    break;
                UInt16 next_unit_id = sr.ReadUInt16();
                sr.BaseStream.Seek(-2, SeekOrigin.Current);
                if ((next_unit_id == cur_unit_id) || (cur_unit_id == -1))
                {
                    cur_unit_id = next_unit_id;
                    Object[] objs = new Object[elem_format.Length];
                    for (int i = 0; i < elem_format.Length; i++)
                    {
                        objs[i] = get_single_variant(sr, elem_format[i], string_size[current_string]);
                    }

                    elements_for_single_unit.Add(objs);
                }
                else
                    break;
            }

            Object[] real_objs = new Object[elem_format.Length * elements_for_single_unit.Count];
            for (int i = 0; i < elements_for_single_unit.Count; i++)
            {
                for (int j = 0; j < elem_format.Length; j++)
                {
                    real_objs[i * elem_format.Length + j] = elements_for_single_unit[i][j];
                }
            }

            return real_objs;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 unit_id = (UInt16)get_element_variant(index, 0).value;
            string txt_unit = manager.get_unit_name(unit_id);
            return unit_id.ToString() + " " + txt_unit;
        }
    }

    //hero army unit resource requirements and... (21st category)
    public class SFCategory21 : SFCategory
    {
        public SFCategory21() : base()
        {
            initialize("HBB");
            category_name = "21. Army unit resource requirements";
            category_id = 2028;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 unit_id = (UInt16)get_element_variant(index, 0).value;
            string txt_unit = manager.get_unit_name(unit_id);
            return unit_id.ToString() + " " + txt_unit;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            Byte resource_amount = (Byte)get_element_variant(index, 2).value;
            Byte resource_id = (Byte)get_element_variant(index, 1).value;
            SFCategoryElement elem = manager.get_category(31).find_element<Byte>(0, resource_id);
            string resource_name = get_text_from_element(elem, 1);
            return "Requirement: " + resource_amount.ToString() + " " + resource_name;
        }
    }

    //unit corpse loot (22nd category)
    public class SFCategory22 : SFCategory
    {
        public SFCategory22() : base()
        {
            initialize("HBHBHBH");
            category_name = "22. Corpse loot";
            category_id = 2040;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 unit_id = (UInt16)get_element_variant(index, 0).value;
            Byte slot_id = (Byte)get_element_variant(index, 1).value;
            string txt_unit = manager.get_unit_name(unit_id);
            return unit_id.ToString() + " " + txt_unit + " (" + slot_id.ToString() + ")";
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            int item_num = 0;
            for(int i = 0; i<3; i++)
            {
                if ((UInt16)get_element_variant(index, 2 + i * 2).value != 0)
                    item_num++;
            }
            string total_string = "";
            Single[] chances = new Single[3];
            for(int i = 0; i < item_num; i++)
            {
                UInt16 item_id = (UInt16)get_element_variant(index, 2 + i * 2).value;
                SFVariant data_variant = get_element_variant(index, 3 + i * 2);
                Byte data_chance = (data_variant != null?(Byte)data_variant.value:(Byte)0);
                if (i == 0)
                    chances[0] = (Single)(data_chance) / 100;
                else if (i == 1)
                    chances[1] = ((Single)(data_chance) / 100) * (1 - chances[0]);
                else if (i == 2)
                    chances[2] = 1 - chances[0] - chances[1];
                string txt_item = manager.get_item_name(item_id);
                total_string += "Item #" + (i + 1).ToString() + ": " + txt_item + " (" + (chances[i]*100).ToString() + "% chance)\r\n";
            }
            return total_string;
        }
    }

    //hero army unit building upgrade (23rd category)
    public class SFCategory23 : SFCategory
    {
        public SFCategory23() : base()
        {
            initialize("HBH");
            category_name = "23. Army unit building requirements";
            category_id = 2001;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 unit_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 building_id = (UInt16)get_element_variant(index, 2).value;
            string txt_unit = manager.get_unit_name(unit_id);
            string txt_building = manager.get_building_name(building_id);
            return unit_id.ToString() + " " + txt_unit + " | " + txt_building;
        }
    }

    //building stats (24th category)
    public class SFCategory24 : SFCategory
    {
        public SFCategory24() : base()
        {
            initialize("HBBBHHhhBHHHHB");
            category_name = "24. Building data";
            category_id = 2029;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            string txt = get_text_from_element(elements[index], 5);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            Byte race_id = (Byte)get_element_variant(index, 1).value;
            string race_name = manager.get_race_name(race_id);
            UInt16 other_building_id = (UInt16)get_element_variant(index, 10).value;
            string building_name = manager.get_building_name(other_building_id);
            return "Race: " + race_name + "\r\nRequires " + building_name;
        }
    }

    //building stats2 (25th category)
    public class SFCategory25 : SFCategory
    {
        public SFCategory25() : base()
        {
            initialize("HBBB");
            category_name = "25. Building collision data";
            category_id = 2030;
        }

        public override Object[] get_element(BinaryReader sr)
        {
            current_string = 0;
            Object[] objs = new Object[elem_format.Length];
            for (int i = 0; i < elem_format.Length; i++)
            {
                objs[i] = get_single_variant(sr, elem_format[i], string_size[current_string]);
            }
            int vertex_count = (int)((Byte)(objs[3]));
            Object[] real_objs = new Object[elem_format.Length + vertex_count * 2];
            for (int i = 0; i < elem_format.Length; i++)
                real_objs[i] = objs[i];
            for (int j = 0; j < vertex_count; j++)
            {
                real_objs[elem_format.Length + j * 2] = get_single_variant(sr, 'h', string_size[current_string]);
                real_objs[elem_format.Length + j * 2 + 1] = get_single_variant(sr, 'h', string_size[current_string]);
            }
            return real_objs;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 building_id = (UInt16)get_element_variant(index, 0).value;
            Byte b_index = (Byte)get_element_variant(index, 1).value;
            string txt_building = manager.get_building_name(building_id);
            return building_id.ToString() + " " + txt_building + " [" + b_index.ToString() + "]";
        }
    }

    //building requirements (26th category)
    public class SFCategory26 : SFCategory
    {
        public SFCategory26() : base()
        {
            initialize("HBH");
            category_name = "26. Building resource requirements";
            category_id = 2031;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 building_id = (UInt16)get_element_variant(index, 0).value;
            Byte b_index = (Byte)get_element_variant(index, 1).value;
            string txt_building = manager.get_building_name(building_id);
            return building_id.ToString() + " " + txt_building + " [" + b_index.ToString() + "]";
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            UInt16 resource_amount = (UInt16)get_element_variant(index, 2).value;
            Byte resource_id = (Byte)get_element_variant(index, 1).value;
            SFCategoryElement elem = manager.get_category(31).find_element<Byte>(0, resource_id);
            string resource_name = get_text_from_element(elem, 1);
            return "Requirement: " + resource_amount.ToString() + " " + resource_name;
        }
    }

    //combat arms/magic ID with name ID (27th category)
    public class SFCategory27 : SFCategory
    {
        public SFCategory27() : base()
        {
            initialize("BBH");
            category_name = "27. Skills link with text data";
            category_id = 2039;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            string txt = get_text_from_element(elements[index], 2);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }
    }

    //skills requirements (28th category)
    public class SFCategory28 : SFCategory
    {
        public SFCategory28() : base()
        {
            initialize("BBBBBBBBB");
            category_name = "28. Skill point requirements";
            category_id = 2062;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            Byte skill_major = (Byte)get_element_variant(index, 0).value;
            Byte skill_level = (Byte)get_element_variant(index, 1).value;
            string txt_skill = manager.get_skill_name(skill_major, 101, skill_level);
            return txt_skill;
        }
    }

    //merchant ID with unit ID (29th category)
    public class SFCategory29 : SFCategory
    {
        public SFCategory29() : base()
        {
            initialize("HH");
            category_name = "29. Merchants link with unit general data";
            category_id = 2041;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 merchant_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 unit_id = (UInt16)get_element_variant(index, 1).value;
            string txt_unit = manager.get_unit_name(unit_id);
            return merchant_id.ToString() + " " + txt_unit;
        }
    }

    //merchant's inventory (30th category)
    public class SFCategory30 : SFCategory
    {
        public SFCategory30() : base()
        {
            initialize("HHH");
            category_name = "30. Merchant inventory";
            category_id = 2042;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 merchant_id = (UInt16)get_element_variant(index, 0).value;
            string txt_merchant = manager.get_merchant_name(merchant_id);
            return merchant_id.ToString() + " " + txt_merchant;
        }

        public override Object[] get_element(BinaryReader sr)
        {
            List<Object[]> elements_for_single_merchant = new List<Object[]>();
            int cur_merchant_id = -1;
            while (true)
            {
                if (sr.PeekChar() == -1)
                    break;
                UInt16 next_merchant_id = sr.ReadUInt16();
                sr.BaseStream.Seek(-2, SeekOrigin.Current);
                if ((next_merchant_id == cur_merchant_id) || (cur_merchant_id == -1))
                {
                    cur_merchant_id = next_merchant_id;
                    Object[] objs = new Object[elem_format.Length];
                    for (int i = 0; i < elem_format.Length; i++)
                    {
                        objs[i] = get_single_variant(sr, elem_format[i], string_size[current_string]);
                    }

                    elements_for_single_merchant.Add(objs);
                }
                else
                    break;
            }

            Object[] real_objs = new Object[elem_format.Length * elements_for_single_merchant.Count];
            for (int i = 0; i < elements_for_single_merchant.Count; i++)
            {
                for (int j = 0; j < elem_format.Length; j++)
                {
                    real_objs[i * elem_format.Length + j] = elements_for_single_merchant[i][j];
                }
            }

            return real_objs;
        }
    }

    //merchant's sell and buy rate (?) (31th category)
    public class SFCategory31 : SFCategory
    {
        public SFCategory31() : base()
        {
            initialize("HBH");
            category_name = "31. Merchant sell/buy rate";
            category_id = 2047;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 merchant_id = (UInt16)get_element_variant(index, 0).value;
            string txt_merchant = manager.get_merchant_name(merchant_id);
            return merchant_id.ToString() + " " + txt_merchant;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            Byte item_type = (Byte)get_element_variant(index, 1).value;
            UInt16 perc = (UInt16)get_element_variant(index, 2).value;
            string item_text = "<no_name>";
            if ((item_type > 0) && (item_type < SFCategory7.item_types.Length))
                item_text = SFCategory7.item_types[item_type];
            return "Selling price for " + item_text + " type items: " + perc.ToString() + "% of base price\r\nBuying price: "+(200-perc).ToString()+"% of base price";
        }
    }

    //sql_good' names (32nd category)
    public class SFCategory32 : SFCategory
    {
        public SFCategory32() : base()
        {
            initialize("BH");
            category_name = "32. Resources link with data";
            category_id = 2044;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            string txt = get_text_from_element(elements[index], 1);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }
    }

    //player level stats (33rd category)
    public class SFCategory33 : SFCategory
    {
        public SFCategory33() : base()
        {
            initialize("BHHIBBHH");
            category_name = "33. Player level scaling";
            category_id = 2048;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            Byte level = (Byte)get_element_variant(index, 0).value;
            return "Level "+level.ToString();
        }
    }

    //object stats/names (34th category)
    public class SFCategory34 : SFCategory
    {
        public SFCategory34() : base()
        {
            string_size = new int[1] { 40 };
            initialize("HHBBBsBHHH");
            category_name = "34. Environment objects data";
            category_id = 2050;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 object_id = (UInt16)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 1);
            string object_handle = Utility.CleanString(get_element_variant(index, 5));
            return object_id.ToString() + " " + object_handle + "/" + txt;
        }
    }

    //monument/other world interactive object stats (35th category)
    public class SFCategory35 : SFCategory
    {
        public SFCategory35() : base()
        {
            initialize("HBBB");
            category_name = "35. Interactive objects collision data";
            category_id = 2057;
        }

        public override Object[] get_element(BinaryReader sr)
        {
            current_string = 0;
            Object[] objs = new Object[elem_format.Length];
            for (int i = 0; i < elem_format.Length; i++)
            {
                objs[i] = get_single_variant(sr, elem_format[i], string_size[current_string]);
            }
            int vertex_count = (int)((Byte)(objs[3]));
            Object[] real_objs = new Object[elem_format.Length + vertex_count * 2];
            for (int i = 0; i < elem_format.Length; i++)
                real_objs[i] = objs[i];
            for (int j = 0; j < vertex_count; j++)
            {
                real_objs[elem_format.Length + j * 2] = get_single_variant(sr, 'h', string_size[current_string]);
                real_objs[elem_format.Length + j * 2 + 1] = get_single_variant(sr, 'h', string_size[current_string]);
            }
            return real_objs;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 object_id = (UInt16)get_element_variant(index, 0).value;
            Byte b_index = (Byte)get_element_variant(index, 1).value;
            string txt_building = manager.get_object_name(object_id);
            return object_id.ToString() + " " + txt_building + " [" + b_index.ToString() + "]";
        }
    }

    //chest/corpse loot (36th category)
    public class SFCategory36 : SFCategory
    {
        public SFCategory36() : base()
        {
            initialize("HBHBHBH");
            category_name = "36. Environment object loot";
            category_id = 2065;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 object_id = (UInt16)get_element_variant(index, 0).value;
            Byte slot_id = (Byte)get_element_variant(index, 1).value;
            string txt_unit = manager.get_object_name(object_id);
            return object_id.ToString() + " " + txt_unit + " (" + slot_id.ToString() + ")";
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            int item_num = 0;
            for (int i = 0; i < 3; i++)
            {
                if ((UInt16)get_element_variant(index, 2 + i * 2).value != 0)
                    item_num++;
            }
            string total_string = "";
            Single[] chances = new Single[3];
            for (int i = 0; i < item_num; i++)
            {
                UInt16 item_id = (UInt16)get_element_variant(index, 2 + i * 2).value;
                SFVariant data_variant = get_element_variant(index, 3 + i * 2);
                Byte data_chance = (data_variant != null ? (Byte)data_variant.value : (Byte)0);
                if (i == 0)
                    chances[0] = (Single)(data_chance) / 100;
                else if (i == 1)
                    chances[1] = ((Single)(data_chance) / 100) * (1 - chances[0]);
                else if (i == 2)
                    chances[2] = 1 - chances[0] - chances[1];
                string txt_item = manager.get_item_name(item_id);
                total_string += "Item #" + (i + 1).ToString() + ": " + txt_item + " (" + (chances[i] * 100).ToString() + "% chance)\r\n";
            }
            return total_string;
        }
    }

    //unknown2 (37th category)
    public class SFCategory37 : SFCategory
    {
        public SFCategory37() : base()
        {
            initialize("IH");
            category_name = "37. NPC link with text data";
            category_id = 2051;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt32 object_id = (UInt32)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 1);
            return object_id.ToString() + " " + txt;
        }
    }

    //quest maps (?) (38th category)
    public class SFCategory38 : SFCategory
    {
        public SFCategory38() : base()
        {
            string_size = new int[1] { 64 };
            initialize("IBsH");
            category_name = "38. Map data";
            category_id = 2052;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt32 map_id = (UInt32)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 3);
            return map_id.ToString() + " " + txt;
        }
    }

    //portals locations (39th category)
    public class SFCategory39 : SFCategory
    {
        public SFCategory39() : base()
        {
            initialize("HIHHBH");
            category_name = "39. Portal locations";
            category_id = 2053;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 object_id = (UInt16)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 5);
            return object_id.ToString() + " " + txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            string map_handle = "";
            UInt32 map_id = (UInt32)get_element_variant(index, 1).value;
            SFCategoryElement map_elem = manager.get_category(37).find_binary_element<UInt32>(0, map_id);
            if (map_elem == null)
                map_handle = "<no name>";
            else
                map_handle = Utility.CleanString(map_elem.get_single_variant(2));
            return "Map handle: " + map_handle;
        }
    }

    //unknown (from sql_lua?) (40th category)
    public class SFCategory40 : SFCategory
    {
        public SFCategory40() : base()
        {
            initialize("BBB");
            category_name = "40. Unknown (from sql_lua?)";
            category_id = 2055;
        }
    }

    //quest game menu (41st category)
    public class SFCategory41 : SFCategory
    {
        public SFCategory41() : base()
        {
            initialize("HH");
            category_name = "41. Description link with txt data";
            category_id = 2058;
        }
        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 desc_id = (UInt16)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 1);
            return desc_id.ToString() + " " + txt;
        }

    }

    //game/button/menu description (42nd category)
    public class SFCategory42 : SFCategory
    {
        public SFCategory42() : base()
        {
            initialize("HHH");
            category_name = "42. Extended description data";
            category_id = 2059;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 elem_id = (UInt16)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 2);
            return elem_id.ToString() + " " + txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            string txt = get_text_from_element(elements[index], 1);
            return "Text ID: " + txt;
        }
    }

    //quest IDs (43rd category)
    public class SFCategory43 : SFCategory
    {
        public SFCategory43() : base()
        {
            initialize("IIBHHI");
            category_name = "43. Quest hierarchy/description data";
            category_id = 2061;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt32 elem_id = (UInt32)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 3);
            return elem_id.ToString() + " " + txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            UInt32 quest_id = (UInt32)get_element_variant(index, 1).value;
            SFCategoryElement quest_elem = find_binary_element<UInt32>(0, quest_id);
            string quest_name = get_text_from_element(quest_elem, 3);
            string desc_text = get_text_from_element(elements[index], 4);
            return desc_text + "\r\n\r\nPart of quest " + quest_name;
        }
    }

    //uweapon type stats (44th category)
    public class SFCategory44 : SFCategory
    {
        public SFCategory44() : base()
        {
            initialize("HHB");
            category_name = "44. Weapon types";
            category_id = 2063;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 elem_id = (UInt16)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 1);
            return elem_id.ToString() + " " + txt;
        }
    }

    //weapon materials (45th category)
    public class SFCategory45 : SFCategory
    {
        public SFCategory45() : base()
        {
            initialize("HH");
            category_name = "45. Weapon materials";
            category_id = 2064;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 elem_id = (UInt16)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 1);
            return elem_id.ToString() + " " + txt;
        }
    }

    //unknown3 (46th category)
    public class SFCategory46 : SFCategory
    {
        public SFCategory46() : base()
        {
            initialize("HBB");
            category_name = "46. Unknown (3)";
            category_id = 2032;
        }
    }

    //heads (47th category)
    public class SFCategory47 : SFCategory
    {
        public SFCategory47() : base()
        {
            initialize("BB");
            category_name = "47. Heads";
            category_id = 2049;
        }
    }

    //button upgrade stats ui (48th category)
    public class SFCategory48 : SFCategory
    {
        public SFCategory48() : base()
        {
            string_size = new int[1] { 64 };
            initialize("HHHHHHHHHHHsI");
            category_name = "48. Unit upgrade data";
            category_id = 2036;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 elem_id = (UInt16)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 2);
            return elem_id.ToString() + " " + txt;
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            UInt16 elem_id = (UInt16)get_element_variant(index, 1).value;
            string building_name = manager.get_building_name(elem_id);
            UInt16 desc_id = (UInt16)get_element_variant(index, 3).value;
            string desc_name = manager.get_description_name(desc_id);
            return desc_name + "\r\n\r\nUpgraded in building: " + building_name;
        }
    }

    //item sets (49th category)
    public class SFCategory49 : SFCategory
    {
        public SFCategory49() : base()
        {
            initialize("BHB");
            category_name = "49. Item sets";
            category_id = 2072;
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            Byte elem_id = (Byte)get_element_variant(index, 0).value;
            string txt = get_text_from_element(elements[index], 1);
            return elem_id.ToString() + " " + txt;
        }
    }

    //special category, is not read/written to file, instead it's generated from loaded data
    public class SFCategoryRuneHeroes: SFCategory
    {
        public SFCategoryRuneHeroes(): base()
        {
            initialize("HH");
            category_name = "X. Rune heroes (special)";
            category_id = 0;
        }

        public void generate(SFCategoryManager manager)
        {
            elements = new List<SFCategoryElement>();
            List<int> rune_indices = manager.query_by_column_numeric(6, 1, 3);
            SortedDictionary<UInt16, UInt16> kv = new SortedDictionary<UInt16, UInt16>();
            for (int i = 0; i < rune_indices.Count; i++)
            {
                UInt16 stats_id = (UInt16)manager.get_category(6).get_element_variant(rune_indices[i], 4).value;
                UInt16 text_id = (UInt16)manager.get_category(6).get_element_variant(rune_indices[i], 3).value;
                if(!kv.ContainsKey(stats_id))
                    kv.Add(stats_id, text_id);
            }
            foreach(KeyValuePair<UInt16, UInt16> pair in kv)
            {
                SFCategoryElement elem = generate_empty_element();
                elem.set_single_variant(0, pair.Key);
                elem.set_single_variant(1, pair.Value);
                elements.Add(elem);
            }
        }
    }
}
