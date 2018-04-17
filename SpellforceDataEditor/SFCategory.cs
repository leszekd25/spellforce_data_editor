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
        protected SFCategoryManager manager;

        //constructor 
        public SFCategory()
        {
            categoryHeader = new Byte[12];
            string_size = new int[1] { 0 };
        }

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
        public Object[] get_element(BinaryReader sr)
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
        public virtual void read(BinaryReader sr)
        {
            categoryHeader = sr.ReadBytes(categoryHeader.Length);
            block_length = BitConverter.ToUInt32(categoryHeader, 6);
            elements = new List<SFCategoryElement>();
            Byte[] block_buffer = new Byte[block_length];
            sr.Read(block_buffer, 0, (int)(block_length));
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
            categoryHeader = new Byte[12];
        }
    }

    //spells/skills (1st category)
    public class SFCategory1 : SFCategory
    {
        public SFCategory1() : base()
        {
            initialize("HHBBBBBBBBBBBBHIIHHBBIIIIIIIIIIBBBB");
            category_name = "Spells/skills";
        }

        //surprisingly ugly due to converting values in this function...
        //can this be done better?
        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 type_id = (UInt16)get_element_variant(index, 1).value;
            SFCategoryElement stype_elem = manager.get_category(1).find_binary_element<UInt16>(0, type_id);
            if(stype_elem == null)
            {
                return get_element_variant(index, 0).value.ToString();
            }
            UInt16 text_id = (UInt16)stype_elem.get_single_variant(1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                Byte spell_level = (Byte)get_element_variant(index, 4).value;
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return get_element_variant(index, 0).value.ToString() + " " + txt + " level " + spell_level.ToString();
            }
            return get_element_variant(index, 0).value.ToString() + " <text missing>";
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
                if (i != reqs.Count - 1)
                    req_str += "\r\n";
            }
            return "Requirements:\r\n" + req_str;
        }
    }

    //spell ui (2nd category)
    public class SFCategory2 : SFCategory
    {
        public SFCategory2() : base()
        {
            string_size = new int[1] { 64 };   //must go before initialize
            initialize("HHBBBBBsH");
            category_name = "Spell UI";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            int text_id = (int)(UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return get_element_variant(index, 0).value.ToString() + " " + txt;
            }
            return get_element_variant(index, 0).value.ToString() + " <text missing>";
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            string spell_name = "";
            string spell_desc = "";
            int text_id = (int)(UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                spell_name = Utility.CleanString(txt_elem.get_single_variant(4));
            }
            spell_desc = manager.get_description_name((UInt16)get_element_variant(index, 8).value);
            return spell_name + "\r\n" + spell_desc;
        }
    }

    //unknown1 (3rd category)
    public class SFCategory3 : SFCategory
    {
        public SFCategory3() : base()
        {
            initialize("BBBBBB");
            category_name = "Unknown (1)";
        }
    }

    //unit/hero stats (4th category)
    public class SFCategory4 : SFCategory
    {
        public SFCategory4() : base()
        {
            initialize("HHBHHHHHHHBBHHHHHHHHBBIBHB");
            category_name = "Unit/hero stats";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 stats_id = (UInt16)get_element_variant(index, 0).value;
            SFCategoryElement elem = manager.get_category(17).find_element<UInt16>(2, stats_id);
            int text_id = -1;
            if (elem == null)
                return stats_id.ToString() + " " + manager.get_runehero_name(stats_id);
            else
                text_id = (int)(UInt16)manager.get_category(17).find_element<UInt16>(2, stats_id).get_single_variant(1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return stats_id.ToString() + " " + txt;
            }
            return stats_id.ToString() + " <text missing>";
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            string race_name = "";
            Byte race_id = (Byte)get_element_variant(index, 2).value;
            race_name = manager.get_race_name(race_id);
            return "This unit race: " + race_name;
        }
    }

    //hero magic/combat arms and worker gathering (5th category)
    public class SFCategory5 : SFCategory
    {
        public SFCategory5() : base()
        {
            initialize("HBBB");
            category_name = "Hero magic/combat arms and worker gathering";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 stats_id = (UInt16)get_element_variant(index, 0).value;
            SFCategoryElement elem = manager.get_category(17).find_element<UInt16>(2, stats_id);
            int text_id = -1;
            if (elem == null)
                return stats_id.ToString() + " " + manager.get_runehero_name(stats_id);
            else
                text_id = (int)(UInt16)manager.get_category(17).find_element<UInt16>(2, stats_id).get_single_variant(1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return stats_id.ToString() + " " + txt;
            }
            return stats_id.ToString() + " <text missing>";
            
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
            category_name = "Hero skills/spells";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 spell_id = (UInt16)get_element_variant(index, 2).value;
            string txt = manager.get_effect_name(spell_id, true);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }
    }

    //item type/name ID/price (7th category)
    public class SFCategory7 : SFCategory
    {
        static string[] item_types = { "Unknown", "Equipment", "Inventory rune", "Installed rune",
            "Spell scroll", "Equipped scroll", "Unit plan", "Building plan", "Equipped unit plan",
            "Equipped building plan", "Miscellaneous" };

        public SFCategory7() : base()
        {
            initialize("HBBHHHHBIIB");
            category_name = "Item type/name ID/price";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            int text_id = (int)(UInt16)get_element_variant(index, 3).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return get_element_variant(index, 0).value.ToString() + " " + txt;
            }
            return get_element_variant(index, 0).value.ToString() + " <missing>";
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            string contains_text;
            string item_type_text = "";
            Byte item_type = (Byte)get_element_variant(index, 1).value;
            Byte bonus_type = (Byte)get_element_variant(index, 2).value;
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
            string total_text = item_type_text;
            if (contains_text != "")
            {
                contains_text += " (" + manager.get_race_name(bonus_type) + ")";
                //SFCategoryElement race_elem = manager.get_category
                total_text += "\r\nContains " + contains_text;
            }
            return total_text;
        }
    }

    //armor class item stats (8th category)
    public class SFCategory8 : SFCategory
    {
        public SFCategory8() : base()
        {
            initialize("Hhhhhhhhhhhhhhhhhh");
            category_name = "Armor class item stats";
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
            category_name = "Scroll ID with spell ID/rune ID with rune in slot...";
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
        public SFCategory10() : base()
        {
            initialize("HHHHHHHH");
            category_name = "Weapon class item sets";
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
            string type_name = "<no name>";
            string material_name = "<no name>";

            SFCategoryElement type_elem = manager.get_category(43).find_binary_element<UInt16>(0, type_id);
            if (type_elem != null)
            {
                UInt16 text_id = (UInt16)type_elem.get_single_variant(1).value;
                SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
                if (txt_elem != null)
                    type_name = Utility.CleanString(txt_elem.get_single_variant(4));
                else
                    type_name = "<text missing>";
            }
            SFCategoryElement material_elem = manager.get_category(44).find_binary_element<UInt16>(0, material_id);
            if (material_elem != null)
            {
                UInt16 text_id = (UInt16)material_elem.get_single_variant(1).value;
                SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
                if (txt_elem != null)
                    material_name = Utility.CleanString(txt_elem.get_single_variant(4));
                else
                    material_name = "<text missing>";
            }

            return "Weapon type: " + type_name + "\r\nWeapon material: " + material_name;
        }
    }

    //item requirements (11th category)
    public class SFCategory11 : SFCategory
    {
        public SFCategory11() : base()
        {
            initialize("HBBBB");
            category_name = "Item requirements";
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
            category_name = "Item spell effects";
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
            category_name = "Item UI";
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
            category_name = "Spell item ID with spell number";
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
            category_name = "Text ID";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            return get_element_variant(index, 0).value.ToString() + " " + Utility.CleanString(get_element_variant(index, 4));
        }
    }

    //race stats (16th category)
    public class SFCategory16 : SFCategory
    {
        public SFCategory16() : base()
        {
            initialize("BBBBBBBHBBBBBBBBBBBBBBBBBB");
            category_name = "Race stats";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 text_id = (UInt16)get_element_variant(index, 7).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return get_element_variant(index, 0).value.ToString() + " " + txt;
            }
            return get_element_variant(index, 0).value.ToString() + " <missing>";
        }
    }

    //head stats (17th category)
    public class SFCategory17 : SFCategory
    {
        public SFCategory17() : base()
        {
            initialize("BBB");
            category_name = "Head stats";
        }
    }

    //unit names (18th category)
    public class SFCategory18 : SFCategory
    {
        public SFCategory18() : base()
        {
            string_size = new int[1] { 40 };
            initialize("HHHIHIHBHHsB");
            category_name = "Unit names";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 text_id = (UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return get_element_variant(index, 0).value.ToString() + " " + txt;
            }
            return get_element_variant(index, 0).value.ToString() + " <text missing>";
        }
    }

    //unit equipment (19th category)
    public class SFCategory19 : SFCategory
    {
        public SFCategory19() : base()
        {
            initialize("HBH");
            category_name = "Unit equipment";
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
            category_name = "Unit spells/skills";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 unit_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 spell_id = (UInt16)get_element_variant(index, 2).value;
            string txt_unit = manager.get_unit_name(unit_id);
            string txt_spell = manager.get_effect_name(spell_id, true);
            return unit_id.ToString() + " " + txt_unit + " | " + txt_spell;
        }
    }

    //hero army unit resource requirements and... (21st category)
    public class SFCategory21 : SFCategory
    {
        public SFCategory21() : base()
        {
            initialize("HBB");
            category_name = "Hero army unit resource rquirements and...";
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
            string resource_name = "";
            SFCategoryElement elem = manager.get_category(31).find_element<Byte>(0, resource_id);
            if (elem == null)
                resource_name = "<no name>";
            else
            {
                int text_id = (int)(UInt16)elem.get_single_variant(1).value;
                SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
                if (txt_elem != null)
                    resource_name = Utility.CleanString(txt_elem.get_single_variant(4));
                else
                    resource_name = "<text missing>";
            }
            return "Requirement: " + resource_amount.ToString() + " " + resource_name;
        }
    }

    //unit corpse loot (22nd category)
    public class SFCategory22 : SFCategory
    {
        public SFCategory22() : base()
        {
            initialize("HBHBHBH");
            category_name = "Unit corpse loot";
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
            for(int i = 0; i < item_num; i++)
            {
                UInt16 item_id = (UInt16)get_element_variant(index, 2 + i * 2).value;
                Byte item_chance = (i != 2?(Byte)get_element_variant(index, 3 + i * 2).value:(Byte)100);
                string txt_item = manager.get_item_name(item_id);
                total_string += "Item #" + (i + 1).ToString() + ": " + txt_item + " (" + item_chance.ToString() + "% chance)\r\n";
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
            category_name = "Hero army unit building upgrade";
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
            category_name = "Building stats";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            int text_id = (int)(UInt16)get_element_variant(index, 5).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return get_element_variant(index, 0).value.ToString() + " " + txt;
            }
            return get_element_variant(index, 0).value.ToString() + " <missing>";
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
            category_name = "Building stats 2";
        }

        //reads a buffer and retrieves all expected elements
        //special override method for this category
        public override void read(BinaryReader sr)
        {
            categoryHeader = sr.ReadBytes(categoryHeader.Length);
            block_length = BitConverter.ToUInt32(categoryHeader, 6);
            elements = new List<SFCategoryElement>();
            Byte[] block_buffer = new Byte[block_length];
            sr.Read(block_buffer, 0, (int)(block_length));
            int fm_length = elem_format.Length;
            MemoryStream ms = new MemoryStream(block_buffer);
            BinaryReader mr = new BinaryReader(ms, Encoding.Default);
            while (mr.PeekChar() != -1)
            {
                SFCategoryElement elem = new SFCategoryElement();
                elem.set(get_element(mr));
                int vertex_count = (int)(Byte)elem.get_single_variant(3).value;
                for(int j = 0; j < vertex_count; j++)
                {
                    elem.add_single_variant(mr.ReadInt16());
                    elem.add_single_variant(mr.ReadInt16());
                }
                elements.Add(elem);
            }
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
            category_name = "Building requirements";
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
            Byte resource_amount = (Byte)get_element_variant(index, 2).value;
            Byte resource_id = (Byte)get_element_variant(index, 1).value;
            string resource_name = "";
            SFCategoryElement elem = manager.get_category(31).find_element<Byte>(0, resource_id);
            if (elem == null)
                resource_name = "<no name>";
            else
            {
                int text_id = (int)(UInt16)elem.get_single_variant(1).value;
                SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
                if (txt_elem != null)
                    resource_name = Utility.CleanString(txt_elem.get_single_variant(4));
                else
                    resource_name = "<text missing>";
            }
            return "Requirement: " + resource_amount.ToString() + " " + resource_name;
        }
    }

    //combat arms/magic ID with name ID (27th category)
    public class SFCategory27 : SFCategory
    {
        public SFCategory27() : base()
        {
            initialize("BBH");
            category_name = "Combat arms/magic ID with name ID";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            int text_id = (int)(UInt16)get_element_variant(index, 2).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return get_element_variant(index, 0).value.ToString() + " " + txt;
            }
            return get_element_variant(index, 0).value.ToString() + " <text missing>";
        }
    }

    //skills requirements (28th category)
    public class SFCategory28 : SFCategory
    {
        public SFCategory28() : base()
        {
            initialize("BBBBBBBBB");
            category_name = "Skill requirements";
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
            category_name = "Merchant ID with unit ID";
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
            category_name = "Merchant's inventory";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 merchant_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 item_id = (UInt16)get_element_variant(index, 1).value;
            string txt_merchant = manager.get_merchant_name(merchant_id);
            string txt_item = manager.get_item_name(item_id);
            return merchant_id.ToString() + " " + txt_merchant + " | " + txt_item;
        }
    }

    //merchant's sell and buy rate (?) (31th category)
    public class SFCategory31 : SFCategory
    {
        public SFCategory31() : base()
        {
            initialize("HBH");
            category_name = "Merchant's sell/buy rate";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 merchant_id = (UInt16)get_element_variant(index, 0).value;
            string txt_merchant = manager.get_merchant_name(merchant_id);
            return merchant_id.ToString() + " " + txt_merchant;
        }
    }

    //sql_good' names (32nd category)
    public class SFCategory32 : SFCategory
    {
        public SFCategory32() : base()
        {
            initialize("BH");
            category_name = "Resource names";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            int text_id = (int)(UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return get_element_variant(index, 0).value.ToString() + " " + txt;
            }
            return get_element_variant(index, 0).value.ToString() + " <text missing>";
        }
    }

    //player level stats (33rd category)
    public class SFCategory33 : SFCategory
    {
        public SFCategory33() : base()
        {
            initialize("BHHIBBHH");
            category_name = "Player level stats";
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
            initialize("HHBBBsBBBBBBB");
            category_name = "Object stats/names";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 object_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 text_id = (UInt16)get_element_variant(index, 1).value;
            string object_handle = Utility.CleanString(get_element_variant(index, 5));
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt = Utility.CleanString(txt_elem.get_single_variant(4));
                return object_id.ToString() + " " + object_handle + "/" + txt;
            }
            return object_id.ToString() + " " + object_handle + "/<text missing>";
        }
    }

    //monument/other world interactive object stats (35th category)
    public class SFCategory35 : SFCategory
    {
        public SFCategory35() : base()
        {
            initialize("HBBB");
            category_name = "Interactive object stats";
        }

        //reads a buffer and retrieves all expected elements
        //special override method for this category
        public override void read(BinaryReader sr)
        {
            categoryHeader = sr.ReadBytes(categoryHeader.Length);
            block_length = BitConverter.ToUInt32(categoryHeader, 6);
            block_length = BitConverter.ToUInt32(categoryHeader, 6);
            elements = new List<SFCategoryElement>();
            Byte[] block_buffer = new Byte[block_length];
            sr.Read(block_buffer, 0, (int)(block_length));
            int fm_length = elem_format.Length;
            MemoryStream ms = new MemoryStream(block_buffer);
            BinaryReader mr = new BinaryReader(ms, Encoding.Default);
            while (mr.PeekChar() != -1)
            {
                SFCategoryElement elem = new SFCategoryElement();
                elem.set(get_element(mr));
                int vertex_count = (int)(Byte)elem.get_single_variant(3).value;
                for (int j = 0; j < vertex_count; j++)
                {
                    elem.add_single_variant(mr.ReadInt16());
                    elem.add_single_variant(mr.ReadInt16());
                }
                elements.Add(elem);
            }
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
            category_name = "Chest/corpse loot";
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
            for (int i = 0; i < item_num; i++)
            {
                UInt16 item_id = (UInt16)get_element_variant(index, 2 + i * 2).value;
                Byte item_chance = (i != 2 ? (Byte)get_element_variant(index, 3 + i * 2).value : (Byte)100);
                string txt_item = manager.get_item_name(item_id);
                total_string += "Item #" + (i + 1).ToString() + ": " + txt_item + " (" + item_chance.ToString() + "% chance)\r\n";
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
            category_name = "NPC ID with names";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt32 object_id = (UInt32)get_element_variant(index, 0).value;
            UInt16 text_id = (UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt_unk = Utility.CleanString(txt_elem.get_single_variant(4));
                return object_id.ToString() + " " + txt_unk;
            }
            return object_id.ToString() + " <text missing>";
        }
    }

    //quest maps (?) (38th category)
    public class SFCategory38 : SFCategory
    {
        public SFCategory38() : base()
        {
            string_size = new int[1] { 64 };
            initialize("IBsH");
            category_name = "Quest maps (?)";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt32 map_id = (UInt32)get_element_variant(index, 0).value;
            UInt16 text_id = (UInt16)get_element_variant(index, 3).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt_map = Utility.CleanString(manager.find_element_text(text_id, 1).get_single_variant(4));
                return map_id.ToString() + " " + txt_map;
            }
            return map_id.ToString() + " <name missing>";
        }
    }

    //portals locations (39th category)
    public class SFCategory39 : SFCategory
    {
        public SFCategory39() : base()
        {
            initialize("HIHHBH");
            category_name = "Portal locations";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 object_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 text_id = (UInt16)get_element_variant(index, 5).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt_map = Utility.CleanString(manager.find_element_text(text_id, 1).get_single_variant(4));
                return object_id.ToString() + " " + txt_map;
            }
            return object_id.ToString() + " <name missing>";
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
            category_name = "Unknown (from sql_lua?)";
        }
    }

    //quest game menu (41st category)
    public class SFCategory41 : SFCategory
    {
        public SFCategory41() : base()
        {
            initialize("HH");
            category_name = "Quest game menu";
        }
        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 desc_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 text_id = (UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt_map = Utility.CleanString(manager.find_element_text(text_id, 1).get_single_variant(4));
                return desc_id.ToString() + " " + txt_map;
            }
            return desc_id.ToString() + " <text missing>";
        }

    }

    //game/button/menu description (42nd category)
    public class SFCategory42 : SFCategory
    {
        public SFCategory42() : base()
        {
            initialize("HHH");
            category_name = "Game/button/menu description";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 elem_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 desc_id = (UInt16)get_element_variant(index, 2).value;
            SFCategoryElement txt_elem = manager.find_element_text(desc_id, 1);
            if (txt_elem != null)
            {
                string txt_desc = Utility.CleanString(txt_elem.get_single_variant(4));
                return elem_id.ToString() + " " + txt_desc;
            }
            return elem_id.ToString() + " <text missing>";
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            UInt16 text_id = (UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            if (txt_elem != null)
            {
                string txt_desc = Utility.CleanString(txt_elem.get_single_variant(4));
                return "Text ID: "+ txt_desc;
            }
            return "Text ID: <text missing>";
        }
    }

    //quest IDs (43rd category)
    public class SFCategory43 : SFCategory
    {
        public SFCategory43() : base()
        {
            initialize("IIBHHI");
            category_name = "Quest IDs";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt32 elem_id = (UInt32)get_element_variant(index, 0).value;
            UInt16 desc_id = (UInt16)get_element_variant(index, 3).value;
            SFCategoryElement txt_elem = manager.find_element_text(desc_id, 1);
            if (txt_elem != null)
            {
                string txt_desc = Utility.CleanString(txt_elem.get_single_variant(4));
                return elem_id.ToString() + " " + txt_desc;
            }
            return elem_id.ToString() + " <text missing>";
        }

        public override string get_element_description(SFCategoryManager manager, int index)
        {
            UInt32 quest_id = (UInt32)get_element_variant(index, 1).value;
            string quest_name = "<no name>";
            UInt16 desc_id = (UInt16)get_element_variant(index, 4).value;
            string desc_text = "<text missing>";
            SFCategoryElement quest_elem = find_binary_element<UInt32>(0, quest_id);
            if(quest_elem != null)
            {
                SFCategoryElement txt_elem = manager.find_element_text((UInt16)quest_elem.get_single_variant(3).value, 1);
                if (txt_elem != null)
                {
                    quest_name = Utility.CleanString(txt_elem.get_single_variant(4));
                }
                else
                    quest_name = "<text missing>";
            }
            SFCategoryElement desc_elem = manager.find_element_text(desc_id, 1);
            if (desc_elem != null)
            {
                desc_text = Utility.CleanString(desc_elem.get_single_variant(4));
            }
            return desc_text + "\r\n\r\nPart of quest " + quest_name;
        }
    }

    //uweapon type stats (44th category)
    public class SFCategory44 : SFCategory
    {
        public SFCategory44() : base()
        {
            initialize("HHB");
            category_name = "Weapon type stats";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 elem_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 desc_id = (UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(desc_id, 1);
            if (txt_elem != null)
            {
                string txt_desc = Utility.CleanString(txt_elem.get_single_variant(4));
                return elem_id.ToString() + " " + txt_desc;
            }
            return elem_id.ToString() + " <text missing>";
        }
    }

    //weapon materials (45th category)
    public class SFCategory45 : SFCategory
    {
        public SFCategory45() : base()
        {
            initialize("HH");
            category_name = "Weapon materials";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 elem_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 desc_id = (UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(desc_id, 1);
            if (txt_elem != null)
            {
                string txt_desc = Utility.CleanString(txt_elem.get_single_variant(4));
                return elem_id.ToString() + " " + txt_desc;
            }
            return elem_id.ToString() + " <text missing>";
        }
    }

    //unknown3 (46th category)
    public class SFCategory46 : SFCategory
    {
        public SFCategory46() : base()
        {
            initialize("HBB");
            category_name = "Unknown (3)";
        }
    }

    //heads (47th category)
    public class SFCategory47 : SFCategory
    {
        public SFCategory47() : base()
        {
            initialize("BB");
            category_name = "Heads";
        }
    }

    //button upgrade stats ui (48th category)
    public class SFCategory48 : SFCategory
    {
        public SFCategory48() : base()
        {
            string_size = new int[1] { 64 };
            initialize("HHHHHHHHHHHsI");
            category_name = "Button upgrade stats UI";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 elem_id = (UInt16)get_element_variant(index, 0).value;
            UInt16 desc_id = (UInt16)get_element_variant(index, 2).value;
            SFCategoryElement txt_elem = manager.find_element_text(desc_id, 1);
            if (txt_elem != null)
            {
                string txt_desc = Utility.CleanString(txt_elem.get_single_variant(4));
                return elem_id.ToString() + " " + txt_desc;
            }
            return elem_id.ToString() + " <text missing>";
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
            category_name = "Item sets";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            Byte elem_id = (Byte)get_element_variant(index, 0).value;
            UInt16 desc_id = (UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(desc_id, 1);
            if (txt_elem != null)
            {
                string txt_desc = Utility.CleanString(txt_elem.get_single_variant(4));
                return elem_id.ToString() + " " + txt_desc;
            }
            return elem_id.ToString() + " <text missing>";
        }
    }

    //special category, is nor read/written to file, instead it's generated from loaded data
    public class SFCategoryRuneHeroes: SFCategory
    {
        public SFCategoryRuneHeroes(): base()
        {
            initialize("HH");
            category_name = "Rune heroes (special)";
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
