using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    //this class implements methods to read elements from gamedata.cff file (and write to it?)
    public abstract class SFCategory
    {
        protected string name;
        protected uint id;
        protected uint item_count;
        protected uint block_length;
        protected List<SFCategoryElement> elements;
        protected string elem_format;
        protected string category_name;
        protected int elem_size;
        protected int[] string_size;   //if category element holds a string (one or more), a list of string lengths is required
        protected int current_string;
        protected Byte[] categoryHeader;
        protected char[] unknown_buffer;

        //constructor (requires block size in bytes)
        //each block has different size, and it determines how many elements belong to a given category
        public SFCategory()
        {
            item_count = 1;
            categoryHeader = new Byte[12];
            string_size = new int[1] { 0 };
        }

        //calculates size of element given a format, and calculates number of elements that belong to this category
        protected void initialize(string fm)
        {
            elem_format = fm;
            calculate_element_size(elem_format);
        }

        protected void skip()
        {
            item_count = 0;
            return;
        }

        public string get_name()
        {
            return category_name;
        }

        //calculates size of element given a format
        protected void calculate_element_size(string fm)
        {
            current_string = 0;
            int s = 0;
            foreach(char c in fm)
            {
                if (c == 'b' || c == 'B')
                    s += 1;
                else if (c == 'h' || c == 'H')
                    s += 2;
                else if (c == 's')
                {
                    s += string_size[current_string];
                    current_string += 1;
                }
                else
                    s += 4;
            }
            elem_size = s;
        }

        //retrieves next variant from a buffer, given a type (indicated by a character contained in a format)
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

        //retrieves next element from a buffer
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

        public SFCategoryElement get_element(int index)
        {
            return elements[index];
        }

        public SFVariant get_element_variant(int elem_index, int var_index)
        {
            if (elem_index >= elements.Count)
                return null;
            return elements[elem_index].get_single_variant(var_index);
        }

        public SFCategoryElement find_element<T>(int v_index, T value) where T : IComparable
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (((T)get_element(i).get_single_variant(v_index).value).CompareTo(value) == 0)
                    return get_element(i);
            }
            return null;
        }

        public int find_element_index<T>(int v_index, T value) where T : IComparable
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (((T)get_element(i).get_single_variant(v_index).value).CompareTo(value) == 0)
                    return i;
            }
            return -1;
        }

        //binary search for an element, assumes elements are sorted by a given column index
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

        //binary search for an element, assumes elements are sorted by a given column index
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

        public void set_element_variant(int elem_index, int var_index, object obj)
        {
            if(elem_index < elements.Count)
                elements[elem_index].get()[var_index].set(obj);
        }

        //puts a new element to a buffer
        public void put_element(BinaryWriter sw, SFVariant[] vars)
        {
            current_string = 0;
            for (int i = 0; i < elem_format.Length; i++)
            {
                put_single_variant(sw, vars[i], string_size[current_string]);
            }
        }

        //reads a buffer and retrieves all expected elements
        public virtual void read(BinaryReader sr)
        {
            categoryHeader = sr.ReadBytes(categoryHeader.Length);
            block_length = BitConverter.ToUInt32(categoryHeader, 6);
            Byte[] block_buffer = new Byte[block_length];
            sr.Read(block_buffer, 0, (int)(block_length));
            if (item_count == 0)
            {
                Console.WriteLine("not ok");
                return;
            }
            MemoryStream ms = new MemoryStream(block_buffer);
            BinaryReader mr = new BinaryReader(ms, Encoding.ASCII);
            elements = new List<SFCategoryElement>();
            while(mr.PeekChar() != -1)
            {
                SFCategoryElement elem = new SFCategoryElement();
                elem.set(get_element(mr));
                elements.Add(elem);
            }
        }

        //public abstract SFVariant[] set_element(StreamWriter sw);

        //public void write(StreamWriter sw);

        //manager is required to communicate with other cateegories to construct a short description
        public virtual string get_element_string(SFCategoryManager manager, int index)
        {
            return index.ToString();
        }

        public int get_element_count()
        {
            return elements.Count;
        }
    }

    //spells/skills (1st category)
    public class SFCategory1 : SFCategory
    {
        public SFCategory1() : base()
        {
            initialize("HHBBBBBBBBBBBBHIIHHBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
            category_name = "Spells/skills";
        }

        //surprisingly ugly due to converting values in this function...
        //can this be done better?
        public override string get_element_string(SFCategoryManager manager, int index)
        {
            string txt = manager.get_effect_name((UInt16)get_element_variant(index, 1).value, true);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
        }
    }

    //spell ui (2nd category)
    public class SFCategory2 : SFCategory
    {
        public SFCategory2() : base()
        {
            string_size = new int[1] { 64 };   //must go before initialize
            initialize("HHBBBBBsBB");
            category_name = "Spell UI";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            int text_id = (int)(UInt16)get_element_variant(index, 1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            string txt = Utility.CleanString(txt_elem.get_single_variant(4));
            return get_element_variant(index, 0).value.ToString() + " " + txt;
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
                return stats_id.ToString() + " <no name>";
            else
                text_id = (int)(UInt16)manager.get_category(17).find_element<UInt16>(2, stats_id).get_single_variant(1).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            string txt = Utility.CleanString(txt_elem.get_single_variant(4));
            return stats_id.ToString() + " " + txt;
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
            Byte skill_major = (Byte)get_element_variant(index, 1).value;
            Byte skill_minor = (Byte)get_element_variant(index, 2).value;
            Byte skill_level = (Byte)get_element_variant(index, 3).value;
            return manager.get_skill_name(skill_major, skill_minor, skill_level);
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
        public SFCategory7() : base()
        {
            initialize("HBBHHHHBIIB");
            category_name = "Item type/name ID/price";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            int text_id = (int)(UInt16)get_element_variant(index, 3).value;
            SFCategoryElement txt_elem = manager.find_element_text(text_id, 1);
            string txt = Utility.CleanString(txt_elem.get_single_variant(4));
            return get_element_variant(index, 0).value.ToString() + " " +txt;
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
            initialize("HHHHHHBBBB");
            category_name = "Weapon class item sets";
        }

        public override string get_element_string(SFCategoryManager manager, int index)
        {
            UInt16 item_id = (UInt16)get_element_variant(index, 0).value;
            string txt = manager.get_item_name(item_id);
            return get_element_variant(index, 0).value.ToString() + " " + txt;
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
            return get_element_variant(index, 0).value.ToString() + " " + Utility.CleanString(get_element_variant(index, 3));
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
            string txt = Utility.CleanString(txt_elem.get_single_variant(4));
            return get_element_variant(index, 0).value.ToString() + " " + txt;
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
            string txt = Utility.CleanString(txt_elem.get_single_variant(4));
            return get_element_variant(index, 0).value.ToString() + " " + txt;
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
            string txt_spell = manager.get_effect_name(spell_id);
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
            string txt = Utility.CleanString(txt_elem.get_single_variant(4));
            return get_element_variant(index, 0).value.ToString() + " " + txt;
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
            Byte[] block_buffer = new Byte[block_length];
            sr.Read(block_buffer, 0, (int)(block_length));
            if (item_count == 0)
            {
                Console.WriteLine("not ok");
                return;
            }
            int fm_length = elem_format.Length;
            MemoryStream ms = new MemoryStream(block_buffer);
            BinaryReader mr = new BinaryReader(ms, Encoding.ASCII);
            elements = new List<SFCategoryElement>();
            while (mr.PeekChar() != -1)
            {
                SFCategoryElement elem = new SFCategoryElement();
                elem.set(get_element(mr));
                int vertex_count = (int)(Byte)elem.get_single_variant(3).value;
                elem.resize(fm_length + 2 * vertex_count);
                for(int j = 0; j < vertex_count; j++)
                {
                    elem.set_single_variant(4 + j * 2, mr.ReadInt16());
                    elem.set_single_variant(4 + j * 2 + 1, mr.ReadInt16());
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
    }

    //combat arms/magic ID with name ID (27th category)
    public class SFCategory27 : SFCategory
    {
        public SFCategory27() : base()
        {
            initialize("BBH");
            category_name = "Combat arms/magic ID with name ID";
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
    }

    //merchant ID with unit ID (29th category)
    public class SFCategory29 : SFCategory
    {
        public SFCategory29() : base()
        {
            initialize("HH");
            category_name = "Merchant ID with unit ID";
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
    }

    //merchant's sell and buy rate (?) (31th category)
    public class SFCategory31 : SFCategory
    {
        public SFCategory31() : base()
        {
            initialize("BBBBB");
            category_name = "Merchant's sell/buy rate";
        }
    }

    //sql_good' names (32nd category)
    public class SFCategory32 : SFCategory
    {
        public SFCategory32() : base()
        {
            initialize("BBB");
            category_name = "sql_good names";
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
    }

    //object stats/names (34th category)
    public class SFCategory34 : SFCategory
    {
        public SFCategory34() : base()
        {
            string_size = new int[1] { 40 };
            initialize("HBBBBBsBBBBBBB");
            category_name = "Object stats/names";
        }
    }

    //monument/other world interactive object stats (35th category)
    public class SFCategory35 : SFCategory
    {
        public SFCategory35() : base()
        {
            //initialize("");     //todo
            skip();
            category_name = "Monument/other world interactive object stats";
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
    }

    //unknown2 (37th category)
    public class SFCategory37 : SFCategory
    {
        public SFCategory37() : base()
        {
            initialize("HBBBB");
            category_name = "Unknown (2)";
        }
    }

    //quest maps (?) (38th category)
    public class SFCategory38 : SFCategory
    {
        public SFCategory38() : base()
        {
            string_size = new int[1] { 64 };
            initialize("HHBsH");
            category_name = "Quest maps (?)";
        }
    }

    //portals locations (39th category)
    public class SFCategory39 : SFCategory
    {
        public SFCategory39() : base()
        {
            initialize("HIHHBBB");
            category_name = "Portal locations";
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
    }

    //game/button/menu description (42nd category)
    public class SFCategory42 : SFCategory
    {
        public SFCategory42() : base()
        {
            initialize("HHH");
            category_name = "Game/button/menu description";
        }
    }

    //quest IDs (43rd category)
    public class SFCategory43 : SFCategory
    {
        public SFCategory43() : base()
        {
            initialize("HBBBBBBBBBBBBBBB");
            category_name = "Quest IDs";
        }
    }

    //uweapon type stats (44th category)
    public class SFCategory44 : SFCategory
    {
        public SFCategory44() : base()
        {
            initialize("HBBB");
            category_name = "Weapon type stats";
        }
    }

    //weapon materials (45th category)
    public class SFCategory45 : SFCategory
    {
        public SFCategory45() : base()
        {
            initialize("HBB");
            category_name = "Weapon materials";
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
    }

    //item sets (49th category)
    public class SFCategory49 : SFCategory
    {
        public SFCategory49() : base()
        {
            initialize("BHB");
            category_name = "Item sets";
        }
    }

}
