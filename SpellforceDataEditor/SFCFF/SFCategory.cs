using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFCFF
{
    //this class implements methods for all kinds of manipulation of its elements
    //each category holds elements of single type (and multiples of it)
    public abstract class SFCategory
    {
        public List<SFCategoryElement> elements { get; protected set; }     //list of all elements
        protected uint block_length;                    //size of all data that belongs to this category
        protected string elem_format;                   //element format (see get_single_variant)
        protected string category_name;   
        protected int[] string_size;                    //if category element holds a string (one or more), a list of string lengths is required
        protected int current_string;                   //helper variable to enable searching and manipulating string variants
        protected Byte[] categoryHeader;                //each category starts with a header
        protected uint category_id;                      //each category has a unique id the game looks for when reading data

        //constructor 
        public SFCategory()
        {
            categoryHeader = new Byte[12];
            string_size = new int[1] { 0 };
            elements = new List<SFCategoryElement>();
        }

        public SFCategoryElement this[int  index]
        {
            get
            {
                return elements[index];
            }
            set
            {
                elements[index] = value;
            }
        }

        //initialization, sets format for an element
        protected void Initialize(string fm)
        {
            elem_format = fm;
        }

        //returns category name
        public string GetName()
        {
            return category_name;
        }

        //returns a new empty element for this category (used for adding new elements)
        public SFCategoryElement GetEmptyElement()
        {
            current_string = 0;
            SFCategoryElement elem = new SFCategoryElement();
            foreach (char c in elem_format)
            {
                switch (c)
                {
                    case 'b':
                        elem.AddVariant((SByte)0);
                        break;
                    case 'B':
                        elem.AddVariant((Byte)0);
                        break;
                    case 'h':
                        elem.AddVariant((Int16)0);
                        break;
                    case 'H':
                        elem.AddVariant((UInt16)0);
                        break;
                    case 'i':
                        elem.AddVariant((Int32)0);
                        break;
                    case 'I':
                        elem.AddVariant((UInt32)0);
                        break;
                    case 's':
                        elem.AddVariant(new byte[string_size[current_string]]);
                        current_string = Math.Min(string_size.Length - 1, current_string + 1);
                        break;
                    default:
                        LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFCategory.generate_empty_element(): Unrecognized variant type (category: " + category_name + ")");
                        throw new InvalidDataException("SFCategory.GenerateEmptyElement(): Unknown variant type!");
                }
            }
            return elem;
        }

        //retrieves next variant from a buffer, given a type (indicated by a character contained in a format)
        //s_size refers to a string length (for if the variant holds a string)
        //variant is returned as a raw object
        public Object ReadVariantFromBuffer(BinaryReader sr, char t, int s_size)
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
                case 's':
                    current_string = Math.Min(string_size.Length - 1, current_string + 1);
                    return sr.ReadBytes(s_size);
                default:
                    LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFCategory[): Unrecognized variant type (category: " + category_name+")");

                    return null;
            }
        }

        //puts a single variant to a buffer
        public void WriteVariantToBuffer(BinaryWriter sw, object var)
        {
            Type t = var.GetType();
            if (t == typeof(SByte))
                sw.Write((SByte)var);
            else if (t == typeof(Byte))
                sw.Write((Byte)var);
            else if (t == typeof(Int16))
                sw.Write((Int16)var);
            else if (t == typeof(UInt16))
                sw.Write((UInt16)var);
            else if (t == typeof(Int32))
                sw.Write((Int32)var);
            else if (t == typeof(UInt32))
                sw.Write((UInt32)var);
            else if (t == typeof(byte[]))
                sw.Write((byte[])var);
            else
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFCategory.put_single_variant(): Unrecognized variant type (category: " + category_name + ")");
        }

        //retrieves next element (sequence of variants as an array of objects) from a buffer
        public virtual Object[] GetElementFromBuffer(BinaryReader sr)
        {
            current_string = 0;
            Object[] objs = new Object[elem_format.Length];
            if (sr.BaseStream.Position + elem_format.Length > sr.BaseStream.Length)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFCategory(): Can't read past buffer! category: " + category_name);

                throw new EndOfStreamException();
            }

            for (int i = 0; i < elem_format.Length; i++)
            {
                objs[i] = ReadVariantFromBuffer(sr, elem_format[i], string_size[current_string]);
            }
            return objs;
        }

        //an overload for the standard get_element, which allows loading elements until the first variant matches that of a previous element's
        public Object[] GetMultipleElementsFromBuffer(BinaryReader sr, char char_load)
        {
            List<Object> elements_loaded = new List<Object>();
            int cur_id = Utility.NO_INDEX;
            while (true)
            {
                if (sr.BaseStream.Position >= sr.BaseStream.Length)
                    break;
                int next_id = Utility.NO_INDEX;
                if (char_load == 'B')
                {
                    next_id = sr.ReadByte();
                    sr.BaseStream.Seek(-1, SeekOrigin.Current);
                }
                else if (char_load == 'H')
                {
                    next_id = sr.ReadUInt16();
                    sr.BaseStream.Seek(-2, SeekOrigin.Current);
                }
                if (next_id == Utility.NO_INDEX)
                    break;
                current_string = 0;
                if ((next_id == cur_id) || (cur_id == Utility.NO_INDEX))
                {
                    cur_id = next_id;
                    for (int i = 0; i < elem_format.Length; i++)
                        elements_loaded.Add(ReadVariantFromBuffer(sr, elem_format[i], string_size[current_string]));
                }
                else
                    break;
            }

            return elements_loaded.ToArray();
        }

        //searches for an element given column index and searched value and returns it if it exists
        //else returns null
        public SFCategoryElement FindElement<T>(int v_index, T value) where T : IComparable
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (((T)elements[i].variants[v_index]).CompareTo(value) == 0)   // if elements[i] == value
                    return elements[i];
            }
            LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFCategory.find_element(): Element not found (variant index = " + v_index.ToString() + ", value = " + value.ToString() + ", category: " + category_name + ")");
            return null;
        }

        //searches for an element given column index and searched value and returns its index if it exists
        //else returns -1
        public int FindElementIndex<T>(int v_index, T value) where T : IComparable
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (((T)elements[i].variants[v_index]).CompareTo(value) == 0)
                    return i;
            }
            LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFCategory.find_element_index(): Element not found (variant index = " + v_index.ToString() + ", value = " + value.ToString() + ", category: " + category_name + ")");
            return Utility.NO_INDEX;
        }

        //searches for an element given column index and searched value and returns it if it exists
        //else returns null
        //this is binary search variant, and it requires that elements are sorted by given column
        public SFCategoryElement FindElementBinary<T>(int v_index, T value) where T : IComparable
        {
            int current_start = 0;
            int current_end = elements.Count - 1;
            int current_center;
            T val;
            while (current_start <= current_end)
            {
                current_center = (current_start + current_end) / 2;    //care about overflow
                val = (T)elements[current_center].variants[v_index];
                if (val.CompareTo(value) == 0)
                    return elements[current_center];
                if (val.CompareTo(value) < 0)
                    current_start = current_center + 1;
                else
                    current_end = current_center - 1;
            }
            LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFCategory.find_binary_element(): Element not found (variant index = " + v_index.ToString() + ", value = " + value.ToString() + ", category: " + category_name + ")");
            return null;
        }

        //searches for an element given column index and searched value and returns its index if it exists
        //else returns -1
        //this is binary search variant, and it requires that elements are sorted by given column
        public int FindElementIndexBinary<T>(int v_index, T value) where T : IComparable
        {
            int current_start = 0;
            int current_end = elements.Count - 1;
            int current_center;
            T val;
            while (current_start <= current_end)
            {

                current_center = (current_start + current_end) / 2;    //care about overflow
                val = (T)elements[current_center].variants[v_index];
                if (val.CompareTo(value) == 0)
                    return current_center;
                if (val.CompareTo(value) < 0)
                    current_start = current_center + 1;
                else
                    current_end = current_center - 1;
            }
    
            return Utility.NO_INDEX;
        }

        //puts a new element (as a list of variants) to a buffer
        public void WriteElementToBuffer(BinaryWriter sw, List<object> vars)
        {
            for (int i = 0; i < vars.Count; i++)
            {
                WriteVariantToBuffer(sw, vars[i]);
            }
        }

        //returns size of all category elements (in bytes)
        public int GetSize()
        {
            int s = 0;
            foreach(SFCategoryElement elem in elements)
            {
                s += elem.GetSize();
            }
            return s;
        }

        //reads a buffer and retrieves all expected elements
        public int Read(BinaryReader sr)
        {
            // 00-01 - chunk id
            // 02-03 - chunk occurence index
            // 06-09 - chunk data length
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.read() called, category name: "+category_name);
            categoryHeader = sr.ReadBytes(categoryHeader.Length);

            bool bad_header = false;
            uint read_id = BitConverter.ToUInt16(categoryHeader, 0);
            if (read_id != category_id)
                bad_header = true;

            block_length = BitConverter.ToUInt32(categoryHeader, 6);
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.read(): Data size: " + block_length.ToString()+" bytes");
            elements.Clear();
            Byte[] block_buffer = new Byte[block_length];
            sr.Read(block_buffer, 0, (int)(block_length));
            
            MemoryStream ms = new MemoryStream(block_buffer);
            BinaryReader mr = new BinaryReader(ms, Encoding.Default);
            while (mr.BaseStream.Position < mr.BaseStream.Length)
            {
                SFCategoryElement elem = new SFCategoryElement();
                try
                {
                    elem.AddVariants(GetElementFromBuffer(mr));
                }
                catch (EndOfStreamException)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFCategory.read(): Can't read past buffer! Category: "+category_name);
                    return -2;
                }
                catch (Exception)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFCategory.read(): Unknown error while reading! Category: " + category_name);
                    return -3;
                }
                elements.Add(elem);
            }
            mr.Close();
            block_buffer = null;
            if (bad_header)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFCategory.read(): Retrieved chunk ID did not match supposed chunk ID! Category: " + category_name);
                return -1;
            }
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.read(): Items read: " + elements.Count.ToString());
            return 0;
        }

        //inserts all elements into the buffer
        public void Write(BinaryWriter sw)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.write() called, category name: " + category_name);
            UInt32 new_block_size = (UInt32)GetSize();
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.write(): Presumed data size: "+new_block_size.ToString()+" bytes");
            Utility.CopyUInt32ToByteArray(category_id, ref categoryHeader, 0);
            categoryHeader[4] = 0;
            categoryHeader[5] = 0;
            Utility.CopyUInt32ToByteArray(new_block_size, ref categoryHeader, 6);
            sw.Write(categoryHeader);
            for(int i = 0; i < GetElementCount(); i++)
            {
                WriteElementToBuffer(sw, elements[i].variants);
            }
        }

        //returns element's name that will be displayed on the list
        public virtual string GetElementString(int index)
        {
            return index.ToString();
        }

        //returns element's description that will be displayed in the description box
        public virtual string GetElementDescription(int index)
        {
            return "";
        }

        //returns element count
        public int GetElementCount()
        {
            return elements.Count;
        }

        //returns element format string
        public string GetElementFormat()
        {
            return elem_format;
        }

        // returns id of an element at specified index, or -1 if it doesnt exist
        public virtual int GetElementID(int index)
        {
            if (index >= elements.Count)
                return Utility.NO_INDEX;
            if (index < 0)
                return Utility.NO_INDEX;
            
            switch(elem_format[0])
            {
                case 'B':
                    return (int)(byte)elements[index].variants[0];
                case 'H':
                    return (int)(UInt16)elements[index].variants[0];
                case 'I':
                    return (int)(UInt32)elements[index].variants[0];
                default:
                    return Utility.NO_INDEX;
            }
        }

        // returns index of an element with specified id (or -1, if it doesnt exist)
        public virtual int GetElementIndex(int id)
        {
            switch (elem_format[0])
            {
                case 'B':
                    return FindElementIndexBinary(0, (Byte)id);
                case 'H':
                    return FindElementIndexBinary(0, (UInt16)id);
                case 'I':
                    return FindElementIndexBinary(0, (UInt32)id);
                default:
                    return Utility.NO_INDEX;
            }
        }

        // if an element of id X was to be inserted into a list, where should it be placed to preserve ascending order?
        // this function ansvers the question above
        // returns -1 if such element ID already exists
        public int GetNewElementIndex(int id)
        {
            int current_start = 0;
            int current_end = elements.Count - 1;
            int current_center;
            int val;
            while (current_start <= current_end)
            {

                current_center = (current_start + current_end) / 2;    //care about overflow (though its not happening in this case)
                val = GetElementID(current_center);
                if (val.CompareTo(id) == 0)
                    return -1;
                if (val.CompareTo(id) < 0)
                    current_start = current_center + 1;
                else
                    current_end = current_center - 1;
            }
            return current_start;
        }

        //removes all elements and resets category
        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.unload() called, category name: " + category_name);
            elements.Clear();

            categoryHeader = new byte[12];
            for (int i = 0; i < 12; i++)
                categoryHeader[i] = 0;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
                    return Utility.S_NONAME;
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
                    return Utility.S_NONAME;
            }
        }

        public SFCategory1() : base()
        {
            Initialize("HHBBBBBBBBBBBBHIIHHBBIIIIIIIIIIBBBB");
            category_name = "1. Spell data";
            category_id = 2002;
        }

        //surprisingly ugly due to converting values in this function...
        //can this be done better?
        public override string GetElementString(int index)
        {
            UInt16 type_id = (UInt16)this[index][1];
            SFCategoryElement stype_elem = SFCategoryManager.gamedata[1].FindElementBinary<UInt16>(0, type_id);
            string stype_txt = SFCategoryManager.GetTextFromElement(stype_elem, 1);
            Byte spell_level = (Byte)this[index][4];
            return this[index][0].ToString() + " " + stype_txt + " level " + spell_level.ToString();
        }

        public override string GetElementDescription(int index)
        {
            List<string> reqs = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                Byte skill_major = (Byte)this[index][2 + i * 3];
                Byte skill_minor = (Byte)this[index][3 + i * 3];
                Byte skill_level = (Byte)this[index][4 + i * 3];
                if (skill_major == 0)
                    break;
                reqs.Add(SFCategoryManager.GetSkillName(skill_major, skill_minor, skill_level));
            }
            string req_str = "";
            for(int i = 0; i < reqs.Count; i++)
            {
                req_str += reqs[i];
                req_str += "\r\n";
            }
            string target = "";
            target += get_target_type((Byte)this[index][19]);
            target += " " + get_target_mode((Byte)this[index][20]);
            return "Requirements:\r\n" + req_str + "Target: "+target;
        }
    }

    //spell ui (2nd category)
    public class SFCategory2 : SFCategory
    {
        public SFCategory2() : base()
        {
            string_size = new int[1] { 64 };   //must go before initialize
            Initialize("HHBBBBBsH");
            category_name = "2. Spell type data";
            category_id = 2054;
        }

        public override string GetElementString(int index)
        {
            string stype_txt = SFCategoryManager.GetTextFromElement(elements[index], 1);
            return this[index][0].ToString() + " " + stype_txt;
        }

        public override string GetElementDescription(int index)
        {
            string spell_name = SFCategoryManager.GetTextFromElement(elements[index], 1);
            string spell_desc = SFCategoryManager.GetDescriptionName((UInt16)this[index][8]);
            return spell_name + "\r\n" + spell_desc;
        }
    }

    //unknown1 (3rd category)
    public class SFCategory3 : SFCategory
    {
        public SFCategory3() : base()
        {
            Initialize("BBBBBB");
            category_name = "3. Unknown (1)";
            category_id = 2056;
        }

        public override int GetElementID(int index)
        {
            return (int)(Byte)this[index][3];
        }

        public override int GetElementIndex(int id)
        {
            return (int)FindElementIndexBinary(3, (Byte)id);
        }
    }

    //unit/hero stats (4th category)
    public class SFCategory4 : SFCategory
    {
        public SFCategory4() : base()
        {
            Initialize("HHBHHHHHHHBBHHHHHHHHBBIBHB");
            category_name = "4. Unit/hero stats";
            category_id = 2005;
        }

        public override string GetElementString(int index)
        {
            UInt16 stats_id = (UInt16)this[index][0];
            UInt16 stats_level = (UInt16)this[index][1];
            SFCategoryElement elem = SFCategoryManager.gamedata[17].FindElement<UInt16>(2, stats_id);
            string unit_txt = SFCategoryManager.GetTextFromElement(elem, 1);
            if (unit_txt == Utility.S_NONAME)
                unit_txt = SFCategoryManager.GetRuneheroName(stats_id);
            return stats_id.ToString() + " " + unit_txt + " (lvl " + stats_level.ToString() + ")";
        }

        public override string GetElementDescription(int index)
        {
            string race_name = "";
            int hp = (int)(UInt16)this[index][7];
            int mana = (int)(UInt16)this[index][9];
            int lvl = ((int)(UInt16)this[index][1]) -1;
            string stat_txt = "";
            if ((lvl >= 0)&&(lvl < SFCategoryManager.gamedata[32].GetElementCount()))
            {
                SFCategoryElement lvl_elem = SFCategoryManager.gamedata[32][lvl];
                if (lvl_elem != null)
                {
                    hp *= (int)(UInt16)lvl_elem[1];
                    mana *= (int)(UInt16)lvl_elem[2];
                    hp /= 100;
                    mana /= 100;
                    stat_txt = "\r\nHealth: " + hp.ToString() + "\r\nMana: " + mana.ToString();
                }
            }
            Byte race_id = (Byte)this[index][2];
            race_name = SFCategoryManager.GetRaceName(race_id);
            Byte flags = (Byte)this[index][23];
            bool isMale = (flags & 1) == 0;
            bool isUnkillable = (flags & 2) == 2;
            string textFlags = "\r\nUnit gender: ";
            if (isMale)
                textFlags += "male";
            else
                textFlags += "female";
            if (isUnkillable)
                textFlags += "\r\nThis unit is unkillable";
            return "This unit race: " + race_name + stat_txt + textFlags;
        }
    }

    //hero magic/combat arms and worker gathering (5th category)
    public class SFCategory5 : SFCategory
    {
        public SFCategory5() : base()
        {
            Initialize("HBBB");
            category_name = "5. Hero/worker skills";
            category_id = 2006;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 stats_id = (UInt16)this[index][0];
            SFCategoryElement elem = SFCategoryManager.gamedata[17].FindElement<UInt16>(2, stats_id);
            string unit_txt = SFCategoryManager.GetTextFromElement(elem, 1);
            if (unit_txt == Utility.S_NONAME)
                unit_txt = SFCategoryManager.GetRuneheroName(stats_id);
            return stats_id.ToString() + " " + unit_txt;
            
        }
    }

    //hero skills/spells (6th category)
    public class SFCategory6 : SFCategory
    {
        public SFCategory6() : base()
        {
            Initialize("HBH");
            category_name = "6. Hero spells";
            category_id = 2067;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 hero_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetRuneheroName(hero_id);
            return this[index][0].ToString() + " " + txt;
        }
    }

    //item type/name ID/price (7th category)
    public class SFCategory7 : SFCategory
    {
        static public string[] item_types = { Utility.S_UNKNOWN, "Equipment", "Inventory rune", "Installed rune",
            "Spell scroll", "Equipped scroll", "Unit plan", "Building plan", "Equipped unit plan",
            "Equipped building plan", "Miscellaneous" };

        static public string[] equipment_types = { Utility.S_UNKNOWN, "Headpiece", "Chestpiece", "Legpiece", "Unknown", "Unknown", "Ring",
            "1H Weapon", "2H Weapon", "Shield", "Robe", "ItemChestFake (monsters)", "Ranged Weapon", "ItemChestFake (playable)" };

        public SFCategory7() : base()
        {
            Initialize("HBBHHHHBIIB");
            category_name = "7. Item general info";
            category_id = 2003;
        }

        public override string GetElementString(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 3);
            return this[index][0].ToString() + " " + txt;
        }

        public override string GetElementDescription(int index)
        {
            string contains_text;
            string item_type_text = "";
            Byte item_type = (Byte)this[index][1];
            Byte bonus_type = (Byte)this[index][2];
            Byte special = (Byte)this[index][7];
            Byte set_type = (Byte)this[index][10];

            if ((item_type > 0) && (item_type < item_types.Length))
                item_type_text += item_types[item_type];
            /*switch (item_type)
            { }*/
            switch (item_type)
            {
                case 2:
                case 3:
                    UInt16 rune_id = (UInt16)this[index][4];
                    contains_text = SFCategoryManager.GetRuneheroName(rune_id);
                    break;
                case 6:
                case 8:
                    UInt16 army_id = (UInt16)this[index][5];
                    contains_text = SFCategoryManager.GetUnitName(army_id);
                    break;
                case 7:
                case 9:
                    UInt16 building_id = (UInt16)this[index][6];
                    contains_text = SFCategoryManager.GetBuildingName(building_id);
                    break;
                default:
                    contains_text = "";
                    break;
            }

            if(item_type == 1)
            {
                string bonus_type_text = String.Copy(Utility.S_UNKNOWN);
                if ((bonus_type > 0) && (bonus_type < (Byte)equipment_types.Length))
                    bonus_type_text = equipment_types[(int)bonus_type];
                item_type_text += " (" + bonus_type_text + ")";
            }

            string total_text = item_type_text;
            if (contains_text != "")
            {
                contains_text += " (" + SFCategoryManager.GetRaceName(bonus_type) + ")";
                //SFCategoryElement race_elem = SFCategoryManager.get_category
                total_text += "\r\nContains " + contains_text;
            }

            if(set_type != 0)
            {
                Byte elem_id = set_type;
                string txt;
                SFCategoryElement set_elem = SFCategoryManager.gamedata[48].FindElementBinary<Byte>(0, elem_id);
                txt = SFCategoryManager.GetTextFromElement(set_elem, 1);
                total_text += "\r\nPart of set: " + txt;
            }

            if ((special & 4) == 4)
                total_text += "\r\nQuest item (can not be sold)";
            if ((special & 8) == 8)
                total_text += "\r\nQuest item (can be sold)";
            if ((special & 16) == 16)
                total_text += "\r\nYou need to meet all item requirements to use this item";
            if ((special & (0b11100011)) != 0)
                total_text += "\r\nUnknown optional data";
            return total_text;
        }
    }

    //armor class item stats (8th category)
    public class SFCategory8 : SFCategory
    {
        public SFCategory8() : base()
        {
            Initialize("Hhhhhhhhhhhhhhhhhh");
            category_name = "8. Item armor data";
            category_id = 2004;
        }

        public override string GetElementString(int index)
        {
            UInt16 item_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetItemName(item_id);
            return this[index][0].ToString() + " " + txt;
        }
    }

    //scroll ID with spell ID/rune ID with rune in slot... (9th category)
    public class SFCategory9 : SFCategory
    {
        public SFCategory9() : base()
        {
            Initialize("HH");
            category_name = "9. Inventory spell scroll link with installed spell scroll";
            category_id = 2013;
        }

        public override string GetElementString(int index)
        {
            UInt16 item_id1 = (UInt16)this[index][0];
            string txt1 = SFCategoryManager.GetItemName(item_id1);

            UInt16 item_id2 = (UInt16)this[index][1];
            string txt2 = SFCategoryManager.GetItemName(item_id2);

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
            Initialize("HHHHHHHH");
            category_name = "10. Item weapon data";
            category_id = 2015;
        }

        public override string GetElementString(int index)
        {
            UInt16 item_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetItemName(item_id);
            return this[index][0].ToString() + " " + txt;
        }

        public override string GetElementDescription(int index)
        {
            UInt16 type_id = (UInt16)this[index][6];
            UInt16 material_id = (UInt16)this[index][7];         

            SFCategoryElement type_elem = SFCategoryManager.gamedata[43].FindElementBinary<UInt16>(0, type_id);
            string type_name = SFCategoryManager.GetTextFromElement(type_elem, 1);
            SFCategoryElement material_elem = SFCategoryManager.gamedata[44].FindElementBinary<UInt16>(0, material_id);
            string material_name = SFCategoryManager.GetTextFromElement(material_elem, 1);

            UInt16 min_dmg = (UInt16)this[index][1];
            UInt16 max_dmg = (UInt16)this[index][2];
            UInt16 spd = (UInt16)this[index][5];

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
            Initialize("HBBBB");
            category_name = "11. Item skill requirements";
            category_id = 2017;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 item_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetItemName(item_id);
            return this[index][0].ToString() + " " + txt;
        }
    }

    //item spell effects (12th category)
    public class SFCategory12 : SFCategory
    {
        public SFCategory12() : base()
        {
            Initialize("HBH");
            category_name = "12. Item weapon effects/inventory scroll link with spell";
            category_id = 2014;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 item_id = (UInt16)this[index][0];
            string txt_item = SFCategoryManager.GetItemName(item_id);

            UInt16 effect_id = (UInt16)this[index][2];
            string txt_effect = SFCategoryManager.GetEffectName(effect_id, true);

            return this[index][0].ToString() + " " + txt_item + " | " + txt_effect;
        }
    }

    //item ui (13th category)
    public class SFCategory13 : SFCategory
    {
        public SFCategory13() : base()
        {
            string_size = new int[1] { 64 };
            Initialize("HBsH");
            category_name = "13. Item UI data";
            category_id = 2012;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 item_id = (UInt16)this[index][0];
            string txt_item = SFCategoryManager.GetItemName(item_id);
            return this[index][0].ToString() + " " + txt_item;
        }
    }

    //spell item ID with spell number (14th category)
    public class SFCategory14 : SFCategory
    {
        public SFCategory14() : base()
        {
            Initialize("HH");
            category_name = "14. Item installed spell scroll link with spell";
            category_id = 2018;
        }

        public override string GetElementString(int index)
        {
            UInt16 item_id = (UInt16)this[index][0];
            string txt_item = SFCategoryManager.GetItemName(item_id);
            UInt16 effect_id = (UInt16)this[index][1];
            string txt_effect = SFCategoryManager.GetEffectName(effect_id, true);
            return this[index][0].ToString() + " " + txt_item + " | " + txt_effect;
        }
    }

    //text IDs (15th category)
    public class SFCategory15 : SFCategory
    {
        public SFCategory15() : base()
        {
            string_size = new int[2] { 50, 512 };
            Initialize("HBBss");
            category_name = "15. Text data";
            category_id = 2016;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            string txt= SFCategoryManager.GetTextFromElement(elements[index], 0);
            return this[index][0].ToString() + " " + txt;
        }
    }

    //race stats (16th category)
    public class SFCategory16 : SFCategory
    {
        static public string[] race_flags = new string[] { "Undead", "Natural?", "Animal (meat gathering)", "Animal", "Living being?", "?", "??", "??? (not used)" };

        public SFCategory16() : base()
        {
            Initialize("BBBBBBBHBHBBHBBBBHHHB");
            category_name = "16. Race stats";
            category_id = 2022;
        }

        public override string GetElementString(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 7);
            return this[index][0].ToString() + " " + txt;
        }

        public override string GetElementDescription(int index)
        {
            Byte flags = (Byte)this[index][8];
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
            "Hostile [Gorge]", Utility.S_UNKNOWN, Utility.S_NONE, "Hostile [Blades]",
            Utility.S_NONE, "Hostile [Multiplayer enemies]", "Hostile [Ogres]", "Neutral [NPCs]",
            "Hostile [Soulforger]", "Hostile [Bloodash]", Utility.S_UNKNOWN, "Hostile [Dervish]"};

        public SFCategory17() : base()
        {
            Initialize("BBB");
            category_name = "17. Clan relations";
            category_id = 2023;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'B');
        }

        public override string GetElementString(int index)
        {
            string txt = clan_names[(int)(Byte)(this[index][0]) -1];
            return this[index][0].ToString() + " " + txt;
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
            Initialize("HHHIHIHBHHsB");
            category_name = "18. Unit general data/link with unit stats";
            category_id = 2024;
        }

        public override string GetElementString(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 1);
            return this[index][0].ToString() + " " + txt;
        }

        public override string GetElementDescription(int index)
        {
            UInt32 xp_gain = (UInt32)this[index][3];
            UInt16 xp_falloff = (UInt16)this[index][4];
            return "Max XP gained from this unit: " + calculate_total_xp(xp_gain, xp_falloff).ToString();
        }
    }

    //unit equipment (19th category)
    public class SFCategory19 : SFCategory
    {
        public SFCategory19() : base()
        {
            Initialize("HBH");
            category_name = "19. Unit equipment";
            category_id = 2025;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 unit_id = (UInt16)this[index][0];
            UInt16 item_id = (UInt16)this[index][2];
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            string txt_item = SFCategoryManager.GetItemName(item_id);
            return unit_id.ToString() + " " + txt_unit + " | " + txt_item;
        }
    }

    //unit equspells/skills (20th category)
    public class SFCategory20 : SFCategory
    {
        public SFCategory20() : base()
        {
            Initialize("HBH");
            category_name = "20. Unit spells";
            category_id = 2026;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 unit_id = (UInt16)this[index][0];
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return unit_id.ToString() + " " + txt_unit;
        }
    }

    //hero army unit resource requirements and... (21st category)
    public class SFCategory21 : SFCategory
    {
        public SFCategory21() : base()
        {
            Initialize("HBB");
            category_name = "21. Army unit resource requirements";
            category_id = 2028;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 unit_id = (UInt16)this[index][0];
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return unit_id.ToString() + " " + txt_unit;
        }
    }

    //unit corpse loot (22nd category)
    public class SFCategory22 : SFCategory
    {
        public SFCategory22() : base()
        {
            Initialize("HBHBHBH");
            category_name = "22. Corpse loot";
            category_id = 2040;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 unit_id = (UInt16)this[index][0];
            int slot_count = this[index].variants.Count / 7;
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return unit_id.ToString() + " " + txt_unit + " - " + slot_count.ToString() + ((slot_count == 1)?" slot":" slots");
        }
    }

    //hero army unit building upgrade (23rd category)
    public class SFCategory23 : SFCategory
    {
        public SFCategory23() : base()
        {
            Initialize("HBH");
            category_name = "23. Army unit building requirements";
            category_id = 2001;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 unit_id = (UInt16)this[index][0];
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return unit_id.ToString() + " " + txt_unit;
        }
    }

    //building stats (24th category)
    public class SFCategory24 : SFCategory
    {
        public SFCategory24() : base()
        {
            Initialize("HBBBHHhhBHHHHB");
            category_name = "24. Building data";
            category_id = 2029;
        }

        public override string GetElementString(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 5);
            return this[index][0].ToString() + " " + txt;
        }

        public override string GetElementDescription(int index)
        {
            Byte race_id = (Byte)this[index][1];
            string race_name = SFCategoryManager.GetRaceName(race_id);
            UInt16 other_building_id = (UInt16)this[index][0];
            string building_name = SFCategoryManager.GetBuildingName(other_building_id);
            return "Race: " + race_name + "\r\nRequires " + building_name;
        }
    }

    //building stats2 (25th category)
    public class SFCategory25 : SFCategory
    {
        public SFCategory25() : base()
        {
            Initialize("HBBB");
            category_name = "25. Building collision data";
            category_id = 2030;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            List<Object> elements_loaded = new List<Object>();
            int cur_id = Utility.NO_INDEX;
            while (true)
            {
                if (sr.BaseStream.Position >= sr.BaseStream.Length)
                    break;
                int next_id = Utility.NO_INDEX;
                next_id = sr.ReadUInt16();
                sr.BaseStream.Seek(-2, SeekOrigin.Current);
                if (next_id == Utility.NO_INDEX)
                    break;
                current_string = 0;
                if ((next_id == cur_id) || (cur_id == Utility.NO_INDEX))
                {
                    cur_id = next_id;
                    for (int i = 0; i < elem_format.Length; i++)
                        elements_loaded.Add(ReadVariantFromBuffer(sr, elem_format[i], string_size[current_string]));
                    Byte vcount = (Byte)elements_loaded[elements_loaded.Count - 1];
                    for (int i = 0; i < vcount*2; i++)
                        elements_loaded.Add(ReadVariantFromBuffer(sr, 'h', string_size[current_string]));
                }
                else
                    break;
            }

            return elements_loaded.ToArray();
        }

        public override string GetElementString(int index)
        {
            UInt16 building_id = (UInt16)this[index][0];
            Byte b_index = (Byte)this[index][1];
            string txt_building = SFCategoryManager.GetBuildingName(building_id);
            return building_id.ToString() + " " + txt_building + " [" + b_index.ToString() + "]";
        }
    }

    //building requirements (26th category)
    public class SFCategory26 : SFCategory
    {
        public SFCategory26() : base()
        {
            Initialize("HBH");
            category_name = "26. Building resource requirements";
            category_id = 2031;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 building_id = (UInt16)this[index][0];
            Byte b_index = (Byte)this[index][1];
            string txt_building = SFCategoryManager.GetBuildingName(building_id);
            return building_id.ToString() + " " + txt_building + " [" + b_index.ToString() + "]";
        }
    }

    //combat arms/magic ID with name ID (27th category)
    public class SFCategory27 : SFCategory
    {
        public SFCategory27() : base()
        {
            Initialize("BBH");
            category_name = "27. Skills link with text data";
            category_id = 2039;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'B');
        }

        public override string GetElementString(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 2);
            return this[index][0].ToString() + " " + txt;
        }
    }

    //skills requirements (28th category)
    public class SFCategory28 : SFCategory
    {
        public SFCategory28() : base()
        {
            Initialize("BBBBBBBBB");
            category_name = "28. Skill point requirements";
            category_id = 2062;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'B');
        }

        public override string GetElementString(int index)
        {
            Byte skill_major = (Byte)this[index][0];
            Byte skill_level = (Byte)this[index][1];
            string txt_skill = SFCategoryManager.GetSkillName(skill_major, 101, skill_level);
            return txt_skill;
        }
    }

    //merchant ID with unit ID (29th category)
    public class SFCategory29 : SFCategory
    {
        public SFCategory29() : base()
        {
            Initialize("HH");
            category_name = "29. Merchants link with unit general data";
            category_id = 2041;
        }

        public override string GetElementString(int index)
        {
            UInt16 merchant_id = (UInt16)this[index][0];
            UInt16 unit_id = (UInt16)this[index][1];
            string txt_unit = SFCategoryManager.GetUnitName(unit_id);
            return merchant_id.ToString() + " " + txt_unit;
        }
    }

    //merchant's inventory (30th category)
    public class SFCategory30 : SFCategory
    {
        public SFCategory30() : base()
        {
            Initialize("HHH");
            category_name = "30. Merchant inventory";
            category_id = 2042;
        }

        public override string GetElementString(int index)
        {
            UInt16 merchant_id = (UInt16)this[index][0];
            string txt_merchant = SFCategoryManager.GetMerchantName(merchant_id);
            return merchant_id.ToString() + " " + txt_merchant;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }
    }

    //merchant's sell and buy rate (?) (31th category)
    public class SFCategory31 : SFCategory
    {
        public SFCategory31() : base()
        {
            Initialize("HBH");
            category_name = "31. Merchant sell/buy rate";
            category_id = 2047;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 merchant_id = (UInt16)this[index][0];
            string txt_merchant = SFCategoryManager.GetMerchantName(merchant_id);
            return merchant_id.ToString() + " " + txt_merchant;
        }
    }

    //sql_good' names (32nd category)
    public class SFCategory32 : SFCategory
    {
        public SFCategory32() : base()
        {
            Initialize("BH");
            category_name = "32. Resources link with data";
            category_id = 2044;
        }

        public override string GetElementString(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 1);
            return this[index][0].ToString() + " " + txt;
        }
    }

    //player level stats (33rd category)
    public class SFCategory33 : SFCategory
    {
        public SFCategory33() : base()
        {
            Initialize("BHHIBBHH");
            category_name = "33. Player level scaling";
            category_id = 2048;
        }

        public override string GetElementString(int index)
        {
            Byte level = (Byte)this[index][0];
            return "Level "+level.ToString();
        }
    }

    //object stats/names (34th category)
    public class SFCategory34 : SFCategory
    {
        public SFCategory34() : base()
        {
            string_size = new int[1] { 40 };
            Initialize("HHBBBsBHHH");
            category_name = "34. Environment objects data";
            category_id = 2050;
        }

        public override string GetElementString(int index)
        {
            UInt16 object_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 1);
            string object_handle = Utility.CleanString(this[index][5]);
            return object_id.ToString() + " " + object_handle + "/" + txt;
        }
    }

    //monument/other world interactive object stats (35th category)
    public class SFCategory35 : SFCategory
    {
        public SFCategory35() : base()
        {
            Initialize("HBBB");
            category_name = "35. Interactive objects collision data";
            category_id = 2057;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            List<Object> elements_loaded = new List<Object>();
            int cur_id = Utility.NO_INDEX;
            while (true)
            {
                if (sr.BaseStream.Position >= sr.BaseStream.Length)
                    break;
                int next_id = Utility.NO_INDEX;
                next_id = sr.ReadUInt16();
                sr.BaseStream.Seek(-2, SeekOrigin.Current);
                if (next_id == Utility.NO_INDEX)
                    break;
                current_string = 0;
                if ((next_id == cur_id) || (cur_id == Utility.NO_INDEX))
                {
                    cur_id = next_id;
                    for (int i = 0; i < elem_format.Length; i++)
                        elements_loaded.Add(ReadVariantFromBuffer(sr, elem_format[i], string_size[current_string]));
                    Byte vcount = (Byte)elements_loaded[elements_loaded.Count - 1];
                    for (int i = 0; i < vcount * 2; i++)
                        elements_loaded.Add(ReadVariantFromBuffer(sr, 'h', string_size[current_string]));
                }
                else
                    break;
            }

            return elements_loaded.ToArray();
        }

        public override string GetElementString(int index)
        {
            UInt16 object_id = (UInt16)this[index][0];
            Byte b_index = (Byte)this[index][1];
            string txt_building = SFCategoryManager.GetObjectName(object_id);
            return object_id.ToString() + " " + txt_building + " [" + b_index.ToString() + "]";
        }
    }

    //chest/corpse loot (36th category)
    public class SFCategory36 : SFCategory
    {
        public SFCategory36() : base()
        {
            Initialize("HBHBHBH");
            category_name = "36. Environment object loot";
            category_id = 2065;
        }

        public override Object[] GetElementFromBuffer(BinaryReader sr)
        {
            return GetMultipleElementsFromBuffer(sr, 'H');
        }

        public override string GetElementString(int index)
        {
            UInt16 object_id = (UInt16)this[index][0];
            int slot_count = this[index].variants.Count / 7;
            string txt_unit = SFCategoryManager.GetObjectName(object_id);
            return object_id.ToString() + " " + txt_unit + " - " + slot_count.ToString() + ((slot_count == 1) ? " slot" : " slots");
        }
    }
    
    // NPC ids
    public class SFCategory37 : SFCategory
    {
        public SFCategory37() : base()
        {
            Initialize("IH");
            category_name = "37. NPC link with text data";
            category_id = 2051;
        }

        public override string GetElementString(int index)
        {
            UInt32 object_id = (UInt32)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 1);
            return object_id.ToString() + " " + txt;
        }
    }

    //quest maps (?) (38th category)
    public class SFCategory38 : SFCategory
    {
        public SFCategory38() : base()
        {
            string_size = new int[1] { 64 };
            Initialize("IBsH");
            category_name = "38. Map data";
            category_id = 2052;
        }

        public override string GetElementString(int index)
        {
            UInt32 map_id = (UInt32)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 3);
            return map_id.ToString() + " " + txt;
        }
    }

    //portals locations (39th category)
    public class SFCategory39 : SFCategory
    {
        public SFCategory39() : base()
        {
            Initialize("HIHHBH");
            category_name = "39. Portal locations";
            category_id = 2053;
        }

        public override string GetElementString(int index)
        {
            UInt16 object_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 5);
            return object_id.ToString() + " " + txt;
        }

        public override string GetElementDescription(int index)
        {
            string map_handle = "";
            UInt32 map_id = (UInt32)this[index][1];
            SFCategoryElement map_elem = SFCategoryManager.gamedata[37].FindElementBinary<UInt32>(0, map_id);
            if (map_elem == null)
                map_handle = Utility.S_NONAME;
            else
                map_handle = Utility.CleanString(map_elem[2]);
            return "Map handle: " + map_handle;
        }
    }

    //unknown (from sql_lua?) (40th category)
    public class SFCategory40 : SFCategory
    {
        public SFCategory40() : base()
        {
            Initialize("BBB");
            category_name = "40. Unknown (from sql_lua?)";
            category_id = 2055;
        }
    }

    //quest game menu (41st category)
    public class SFCategory41 : SFCategory
    {
        public SFCategory41() : base()
        {
            Initialize("HH");
            category_name = "41. Description link with txt data";
            category_id = 2058;
        }
        public override string GetElementString(int index)
        {
            UInt16 desc_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 1);
            return desc_id.ToString() + " " + txt;
        }

    }

    //game/button/menu description (42nd category)
    public class SFCategory42 : SFCategory
    {
        public SFCategory42() : base()
        {
            Initialize("HHH");
            category_name = "42. Extended description data";
            category_id = 2059;
        }

        public override string GetElementString(int index)
        {
            UInt16 elem_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 2);
            return elem_id.ToString() + " " + txt;
        }

        public override string GetElementDescription(int index)
        {
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 1);
            return "Text ID: " + txt;
        }
    }

    //quest IDs (43rd category)
    public class SFCategory43 : SFCategory
    {
        public SFCategory43() : base()
        {
            Initialize("IIBHHI");
            category_name = "43. Quest hierarchy/description data";
            category_id = 2061;
        }

        public override string GetElementString(int index)
        {
            UInt32 elem_id = (UInt32)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 3);
            return elem_id.ToString() + " " + txt;
        }

        public override string GetElementDescription(int index)
        {
            UInt32 quest_id = (UInt32)this[index][1];
            SFCategoryElement quest_elem = FindElementBinary<UInt32>(0, quest_id);
            string quest_name = SFCategoryManager.GetTextFromElement(quest_elem, 3);
            string desc_text = SFCategoryManager.GetTextFromElement(elements[index], 4);
            return desc_text + "\r\n\r\nPart of quest " + quest_name;
        }
    }

    //uweapon type stats (44th category)
    public class SFCategory44 : SFCategory
    {
        public SFCategory44() : base()
        {
            Initialize("HHB");
            category_name = "44. Weapon types";
            category_id = 2063;
        }

        public override string GetElementString(int index)
        {
            UInt16 elem_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 1);
            return elem_id.ToString() + " " + txt;
        }
    }

    //weapon materials (45th category)
    public class SFCategory45 : SFCategory
    {
        public SFCategory45() : base()
        {
            Initialize("HH");
            category_name = "45. Weapon materials";
            category_id = 2064;
        }

        public override string GetElementString(int index)
        {
            UInt16 elem_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 1);
            return elem_id.ToString() + " " + txt;
        }
    }

    //unknown3 (46th category)
    public class SFCategory46 : SFCategory
    {
        public SFCategory46() : base()
        {
            Initialize("HBB");
            category_name = "46. Terrain material data";
            category_id = 2032;
        }
    }

    //heads (47th category)
    public class SFCategory47 : SFCategory
    {
        public SFCategory47() : base()
        {
            Initialize("BB");
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
            Initialize("HHHHHHHHHHHsI");
            category_name = "48. Unit upgrade data";
            category_id = 2036;
        }

        public override string GetElementString(int index)
        {
            UInt16 elem_id = (UInt16)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 2);
            return elem_id.ToString() + " " + txt;
        }

        public override string GetElementDescription(int index)
        {
            UInt16 elem_id = (UInt16)this[index][1];
            string building_name = SFCategoryManager.GetBuildingName(elem_id);
            UInt16 desc_id = (UInt16)this[index][3];
            string desc_name = SFCategoryManager.GetDescriptionName(desc_id);
            return desc_name + "\r\n\r\nUpgraded in building: " + building_name;
        }
    }

    //item sets (49th category)
    public class SFCategory49 : SFCategory
    {
        public SFCategory49() : base()
        {
            Initialize("BHB");
            category_name = "49. Item sets";
            category_id = 2072;
        }

        public override string GetElementString(int index)
        {
            Byte elem_id = (Byte)this[index][0];
            string txt = SFCategoryManager.GetTextFromElement(elements[index], 1);
            return elem_id.ToString() + " " + txt;
        }
    }

    //special category, is not read/written to file, instead it's generated from loaded data
    public class SFCategoryRuneHeroes: SFCategory
    {
        public SFCategoryRuneHeroes(): base()
        {
            Initialize("HH");
            category_name = "X. Rune heroes (special)";
            category_id = 0;
        }

        public void generate()
        {
            elements = new List<SFCategoryElement>();
            List<int> rune_indices = SFSearchModule.Search(SFCategoryManager.gamedata[6], null, "3", SearchType.TYPE_NUMBER, 1, null);
            SortedDictionary<UInt16, UInt16> kv = new SortedDictionary<UInt16, UInt16>();
            for (int i = 0; i < rune_indices.Count; i++)
            {
                UInt16 stats_id = (UInt16)SFCategoryManager.gamedata[6][rune_indices[i]][4];
                UInt16 text_id = (UInt16)SFCategoryManager.gamedata[6][rune_indices[i]][3];
                if (!kv.ContainsKey(stats_id))
                    kv.Add(stats_id, text_id);
            }
            foreach(KeyValuePair<UInt16, UInt16> pair in kv)
            {
                SFCategoryElement elem = GetEmptyElement();
                elem[0] = pair.Key;
                elem[1] = pair.Value;
                elements.Add(elem);
            }
        }
    }
}
