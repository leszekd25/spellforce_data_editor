using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpellforceDataEditor.SFChunk;

namespace SpellforceDataEditor.SFCFF
{
    public struct ChunkFormatInfo
    {
        public string ElementFormat;
        public string Name;
        public bool AllowMultiple;
        public int[] StringSizes;

        public ChunkFormatInfo(string elem_format, string name, bool allow_multiple, int[] string_sizes = null) 
        { 
            ElementFormat = elem_format;
            Name = name;
            AllowMultiple = allow_multiple; 
            StringSizes = string_sizes;
        }
    }

    //this class implements methods for all kinds of manipulation of its elements
    //each category holds elements of single type (and multiples of it)
    public class SFCategory
    {
        public static Dictionary<Tuple<ushort, ushort>, ChunkFormatInfo> ChunkFormats = new Dictionary<Tuple<ushort, ushort>, ChunkFormatInfo>();

        static SFCategory()
        {
            ChunkFormats.Add(Tuple.Create((ushort)2002, (ushort)3), new ChunkFormatInfo("HHBBBBBBBBBBBBHIIHHBBIIIIIIIIIIHH", "Spell data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2054, (ushort)5), new ChunkFormatInfo("HHBBBBBsH", "Spell type data", false, new int[] { 64 }));
            ChunkFormats.Add(Tuple.Create((ushort)2056, (ushort)1), new ChunkFormatInfo("BBBBBB", "Unknown (1)", false));
            ChunkFormats.Add(Tuple.Create((ushort)2005, (ushort)9), new ChunkFormatInfo("HHBHHHHHHHHHHHHHHHHHIBHB", "4. Unit/hero stats", false));
            ChunkFormats.Add(Tuple.Create((ushort)2006, (ushort)1), new ChunkFormatInfo("HBBB", "Hero/worker skills", true));
            ChunkFormats.Add(Tuple.Create((ushort)2067, (ushort)1), new ChunkFormatInfo("HBH", "Hero spells", true));
            ChunkFormats.Add(Tuple.Create((ushort)2003, (ushort)4), new ChunkFormatInfo("HBBHHHHBIIB", "Item general info", false));
            ChunkFormats.Add(Tuple.Create((ushort)2004, (ushort)1), new ChunkFormatInfo("Hhhhhhhhhhhhhhhhhh", "Item armor data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2013, (ushort)1), new ChunkFormatInfo("HH", "Inventory spell scroll link with installed spell scroll", false));
            ChunkFormats.Add(Tuple.Create((ushort)2015, (ushort)2), new ChunkFormatInfo("HHHHHHHH", "Item weapon data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2017, (ushort)1), new ChunkFormatInfo("HBBBB", "Item skill requirements", true));
            ChunkFormats.Add(Tuple.Create((ushort)2014, (ushort)1), new ChunkFormatInfo("HBH", "Item weapon effects/inventory scroll link with spell", true));
            ChunkFormats.Add(Tuple.Create((ushort)2012, (ushort)1), new ChunkFormatInfo("HBsH", "Item UI data", true, new int[] { 64 }));
            ChunkFormats.Add(Tuple.Create((ushort)2018, (ushort)1), new ChunkFormatInfo("HH", "Item installed spell scroll link with spell", false));
            ChunkFormats.Add(Tuple.Create((ushort)2016, (ushort)3), new ChunkFormatInfo("HBBss", "Text data", true, new int[] { 50, 512 }));
            ChunkFormats.Add(Tuple.Create((ushort)2022, (ushort)7), new ChunkFormatInfo("BBBBBBBHBHBBHBBBBHHHB", "Race stats", false));
            ChunkFormats.Add(Tuple.Create((ushort)2023, (ushort)2), new ChunkFormatInfo("BBB", "Faction relations", true));
            ChunkFormats.Add(Tuple.Create((ushort)2024, (ushort)8), new ChunkFormatInfo("HHHIHIHBHHsB", "Unit general data/link with unit stats", false, new int[] { 40 }));
            ChunkFormats.Add(Tuple.Create((ushort)2025, (ushort)1), new ChunkFormatInfo("HBH", "Unit equipment", true));
            ChunkFormats.Add(Tuple.Create((ushort)2026, (ushort)1), new ChunkFormatInfo("HBH", "Unit spells", true));
            ChunkFormats.Add(Tuple.Create((ushort)2028, (ushort)1), new ChunkFormatInfo("HBB", "Army unit resource requirements", true));
            ChunkFormats.Add(Tuple.Create((ushort)2040, (ushort)1), new ChunkFormatInfo("HBHBHBH", "Corpse loot", true));
            ChunkFormats.Add(Tuple.Create((ushort)2001, (ushort)1), new ChunkFormatInfo("HBH", "Army unit building requirements", true));
            ChunkFormats.Add(Tuple.Create((ushort)2029, (ushort)9), new ChunkFormatInfo("HBBBHHhhBHHHHB", "Building data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2030, (ushort)2), new ChunkFormatInfo("HBBC", "Building collision data", true));
            ChunkFormats.Add(Tuple.Create((ushort)2031, (ushort)2), new ChunkFormatInfo("HBH", "Building resource requirements", true));
            ChunkFormats.Add(Tuple.Create((ushort)2039, (ushort)1), new ChunkFormatInfo("BBH", "Skills link with text data", true));
            ChunkFormats.Add(Tuple.Create((ushort)2062, (ushort)1), new ChunkFormatInfo("BBBBBBBBB", "Skill point requirements", true));
            ChunkFormats.Add(Tuple.Create((ushort)2041, (ushort)1), new ChunkFormatInfo("HH", "Merchants link with unit general data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2042, (ushort)1), new ChunkFormatInfo("HHH", "Merchant inventory", true));
            ChunkFormats.Add(Tuple.Create((ushort)2047, (ushort)1), new ChunkFormatInfo("HBH", "Merchant sell/buy rate", true));
            ChunkFormats.Add(Tuple.Create((ushort)2044, (ushort)1), new ChunkFormatInfo("BH", "Resources link with data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2048, (ushort)3), new ChunkFormatInfo("BHHIBBHH", "Player level scaling", false));
            ChunkFormats.Add(Tuple.Create((ushort)2050, (ushort)6), new ChunkFormatInfo("HHBBBsHHH", "Environment objects data", false, new int[] { 41 }));
            ChunkFormats.Add(Tuple.Create((ushort)2057, (ushort)2), new ChunkFormatInfo("HBBC", "Interactive objects collision data", true));
            ChunkFormats.Add(Tuple.Create((ushort)2065, (ushort)1), new ChunkFormatInfo("HBHBHBH", "Environment object loot", true));
            ChunkFormats.Add(Tuple.Create((ushort)2051, (ushort)1), new ChunkFormatInfo("IH", "NPC link with text data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2052, (ushort)2), new ChunkFormatInfo("IBsH", "Map data", false, new int[] { 64 }));
            ChunkFormats.Add(Tuple.Create((ushort)2053, (ushort)3), new ChunkFormatInfo("HIHHBH", "Portal locations", false));
            ChunkFormats.Add(Tuple.Create((ushort)2055, (ushort)1), new ChunkFormatInfo("BBB", "Unknown (from sql_lua?)", false));
            ChunkFormats.Add(Tuple.Create((ushort)2058, (ushort)1), new ChunkFormatInfo("HH", "Description link with txt data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2059, (ushort)1), new ChunkFormatInfo("HHH", "Extended description data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2061, (ushort)1), new ChunkFormatInfo("IIBHHI", "Quest hierarchy/description data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2063, (ushort)1), new ChunkFormatInfo("HHB", "Weapon types", false));
            ChunkFormats.Add(Tuple.Create((ushort)2064, (ushort)1), new ChunkFormatInfo("HH", "Weapon materials", false));
            ChunkFormats.Add(Tuple.Create((ushort)2032, (ushort)2), new ChunkFormatInfo("HBB", "Terrain data", false));
            ChunkFormats.Add(Tuple.Create((ushort)2049, (ushort)1), new ChunkFormatInfo("BB", "Heads", false));
            ChunkFormats.Add(Tuple.Create((ushort)2036, (ushort)1), new ChunkFormatInfo("HHHHHHHHHHHsI", "Unit upgrade data", false, new int[] { 64 }));
            ChunkFormats.Add(Tuple.Create((ushort)2072, (ushort)1), new ChunkFormatInfo("BHB", "Item sets", false));
        }

        public List<SFCategoryElement> elements { get; protected set; } = new List<SFCategoryElement>();     //list of all elements
        public List<SFCategoryElementList> element_lists = new List<SFCategoryElementList>();                //list of all subelements; for any element in here, it will always have at least 1 subelement
        public bool category_allow_multiple;                                                                 //if true, element_lists is used, if false, elements is used
        // of above, only one can be used at any time


        public uint block_length;                    //size of all data that belongs to this category
        public string elem_format;                   //element format (see get_single_variant)
        public string category_name;
        public int[] string_size;                    //if category element holds a string (one or more), a list of string lengths is required
        public int current_string;                   //helper variable to enable searching and manipulating string variants
        public short category_id;                       //each category has a unique id the game looks for when reading data
        public short category_type;
        public bool category_is_known;
        public byte[] category_unknown_data;

        // returns an element at given index (multiples not allowed)
        public SFCategoryElement this[int index]
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

        // returns a subelement of given element (multiples allowed)
        public SFCategoryElement this[int index1, int index2]
        {
            get
            {
                return element_lists[index1][index2];
            }
            set
            {
                element_lists[index1][index2] = value;
            }
        }

        //returns a new empty element for this category (used for adding new elements or sub-elements)
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
                    case 'C':
                        elem.AddVariant((Byte)0);
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
        // todo: move somewhere else
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
                case 'C':
                    byte vcount = sr.ReadByte();
                    SFOutlineData ret = new SFOutlineData() { Data = new List<short>() };

                    for(int i = 0; i < vcount*2; i++)
                        ret.Data.Add(sr.ReadInt16());

                    return ret;
                default:
                    LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFCategory[): Unrecognized variant type (category: " + category_name+")");

                    return null;
            }
        }

        //puts a single variant to a buffer
        // todo: move somewhere else
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
            else if (t == typeof(SFOutlineData))
            {
                sw.Write((byte)(((SFOutlineData)var).Data.Count/2));
                for (int i = 0; i < ((SFOutlineData)var).Data.Count; i++)
                    sw.Write(((SFOutlineData)var).Data[i]);
            }
            else
                LogUtils.Log.Warning(LogUtils.LogSource.SFCFF, "SFCategory.put_single_variant(): Unrecognized variant type (category: " + category_name + ")");
        }

        //retrieves next element (sequence of variants as an array of objects) from a buffer
        public Object[] GetElementFromBuffer(BinaryReader sr)
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
        //multiple not allowed
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
        //multiple not allowed
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
        //multiple not allowed
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
        //multiple not allowed
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

        public int Read(SFChunkFileChunk sfcfc)
        {
            category_id = sfcfc.header.ChunkID;
            category_type = sfcfc.header.ChunkDataType;

            var key = Tuple.Create((ushort)category_id, (ushort)category_type);
            category_is_known = ChunkFormats.ContainsKey(Tuple.Create((ushort)category_id, (ushort)category_type));

            using (BinaryReader br = sfcfc.Open())
            {
                if (category_is_known)
                {
                    category_name = ChunkFormats[key].Name;
                    string_size = ChunkFormats[key].StringSizes;
                    category_allow_multiple = ChunkFormats[key].AllowMultiple;
                    elem_format = ChunkFormats[key].ElementFormat;

                    if (string_size == null)
                        string_size = new int[] { 0 };

                    int i = 0;
                    int ind = 0;
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        SFCategoryElement elem = new SFCategoryElement();
                        try
                        {
                            elem.AddVariants(category_allow_multiple ? GetMultipleElementsFromBuffer(br, elem_format[0]): GetElementFromBuffer(br));
                        }
                        catch (EndOfStreamException)
                        {
                            LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFCategory.read(): Can't read past buffer! Category: " + category_name);
                            return -2;
                        }
                        catch (Exception)
                        {
                            LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFCategory.read(): Unknown error while reading! Category: " + category_name);
                            return -3;
                        }
                        elements.Add(elem);

                        if (GetElementID(ind) < i)
                            i = 0;                           // breakpoint for when data is not sorted in ascending order

                        i = GetElementID(ind);
                        ind += 1;
                    }

                    LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.read(): Items read: " + elements.Count.ToString());
                }
                else
                {
                    category_unknown_data = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                    LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.read(): Unknown category!");
                }
            }

            return 0;
        }

        public byte[] ToRawData()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.ToRawData() called, category name: " + category_name);

            UInt32 new_block_size = (UInt32)GetSize();
            byte[] data = new byte[new_block_size];
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < GetElementCount(); i++)
                    {
                        WriteElementToBuffer(bw, elements[i].variants);
                    }
                }
            }

            return data;
        }

        public int Write(SFChunkFile sfcf)
        {
            sfcf.AddChunk(category_id, 0, false, category_type, ToRawData());
            return 0;
        }

        //returns element count
        public int GetElementCount()
        {
            if (category_allow_multiple)
                return element_lists.Count;
            else
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

            if (category_allow_multiple)
                return element_lists[index].GetID();
            else
                return elements[index].ToInt(0);
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

        public int GetSubElementID(int index1, int index2)
        {
            if (index1 >= element_lists.Count)
                return Utility.NO_INDEX;
            if (index1 < 0)
                return Utility.NO_INDEX;

            return element_lists[index1].Elements[index2].ToInt(1);
        }

        public int GetSubElementIndex(int index, int id2)
        {
            for(int i = 0; i < element_lists[index].Elements.Count; i++)
            {
                int id = element_lists[index].Elements[i].ToInt(1);
                if (id == id2)
                    return i;

            }

            return Utility.NO_INDEX;
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

        // this function merges element lists of the same type, as long as the element allows multiple subelements
        public static int Merge(string format, SFCategoryElementList e1, SFCategoryElementList e2, out SFCategoryElementList ret)
        {
            ret = new SFCategoryElementList();

            // make dictionary for both elements: key: subelement id, value: subelement position
            int max_id = -1;
            Dictionary<int, int> subelem1 = new Dictionary<int, int>();
            Dictionary<int, int> subelem2 = new Dictionary<int, int>();

            for (int i = 0; i < e1.Elements.Count; i++)
            {
                int id = e1.Elements[i].ToInt(1);
                subelem1.Add(id, i);
                max_id = Math.Max(max_id, id);
            }

            for (int i = 0; i < e2.Elements.Count; i++)
            {
                int id = e2.Elements[i].ToInt(1);
                subelem2.Add(id, i);
                max_id = Math.Max(max_id, id);
            }

            for(int i = 0; i <= max_id; i++)
            {
                if (subelem2.ContainsKey(i))
                    ret.Elements.Add(e2.Elements[subelem2[i]].GetCopy());
                else if(subelem1.ContainsKey(i))
                    ret.Elements.Add(e1.Elements[subelem1[i]].GetCopy());
            }

            return 0;
        }

        // this function makes a diff of elements of the same type, as long as the elements allows multiple subelements
        public static int Diff(string format, SFCategoryElementList e1, SFCategoryElementList e2, out SFCategoryElementList ret)
        {
            ret = new SFCategoryElementList();

            int elem_len = format.Length;

            // make dictionary for both elements: key: subelement id, value: subelement position
            int max_id = -1;
            Dictionary<int, int> subelem1 = new Dictionary<int, int>();
            Dictionary<int, int> subelem2 = new Dictionary<int, int>();

            for (int i = 0; i < e1.Elements.Count; i++)
            {
                int id = e1.Elements[i].ToInt(1);
                subelem1.Add(id, i);
                max_id = Math.Max(max_id, id);
            }

            for (int i = 0; i < e2.Elements.Count; i++)
            {
                int id = e2.Elements[i].ToInt(1);
                subelem2.Add(id, i);
                max_id = Math.Max(max_id, id);
            }

            for (int i = 0; i <= max_id; i++)
            {
                if (subelem2.ContainsKey(i))
                {
                    SFCategoryElement el1, el2;
                    el2 = e2.Elements[subelem2[i]];

                    if (!subelem1.ContainsKey(i))
                        ret.Elements.Add(el2.GetCopy());
                    else
                    {
                        el1 = e1.Elements[subelem1[i]];

                        if (!SFCategoryElement.Compare(el1, el2, subelem1[i] * elem_len, subelem2[i] * elem_len, elem_len))
                            ret.Elements.Add(el2.GetCopy());
                    }
                }
            }

            return 0;
        }


        // this function merges chunks of same type
        // if elements have the same id, they are replaced by elements from cat2
        public static int Merge(SFCategory cat1, SFCategory cat2, out SFCategory ret)
        {
            ret = null;

            if (!cat1.category_is_known)
                return -1;
            if (!cat2.category_is_known)
                return -1;

            ret = new SFCategory()
            {
                block_length = 0,
                category_allow_multiple = cat1.category_allow_multiple,
                category_id = cat1.category_id,
                category_is_known = cat1.category_is_known,
                category_name = cat1.category_name,
                category_type = cat1.category_type,
                category_unknown_data = null,
                current_string = 0,
                elem_format = cat1.elem_format
            };
            ret.string_size = new int[cat1.string_size.Length];
            Array.Copy(cat1.string_size, ret.string_size, cat1.string_size.Length);

            // double list ladder
            int orig_i = 0;
            int new_i = 0;
            int orig_id, new_id;
            bool orig_end = false;
            bool new_end = false;

            while (true)
            {
                if (new_i == cat2.GetElementCount())
                    new_end = true;
                if (orig_i == cat1.GetElementCount())
                    orig_end = true;

                if (orig_end && new_end)
                    break;

                orig_id = cat1.GetElementID(orig_i);
                new_id = cat2.GetElementID(new_i);

                if (orig_end)
                {
                    if (cat2.category_allow_multiple)
                        ret.element_lists.Add(cat2.element_lists[new_i].GetCopy());
                    else
                        ret.elements.Add(cat2[new_i].GetCopy());
                }
                else if (new_end)
                {
                    if (cat1.category_allow_multiple)
                        ret.element_lists.Add(cat1.element_lists[orig_i].GetCopy());
                    else
                        ret.elements.Add(cat1[orig_i].GetCopy());
                }
                else
                {
                    if (orig_id == new_id)
                    {
                        if(cat1.category_allow_multiple)
                        {
                            SFCategoryElementList new_elem;
                            Merge(cat1.elem_format, cat1.element_lists[orig_i], cat2.element_lists[orig_i], out new_elem);
                            ret.element_lists.Add(new_elem);
                        }
                        else
                            ret.elements.Add(cat2[new_i].GetCopy());
                    }
                    else if (orig_id > new_id)
                    {
                        if(cat2.category_allow_multiple)
                            ret.element_lists.Add(cat2.element_lists[new_i].GetCopy());
                        else
                            ret.elements.Add(cat2[new_i].GetCopy());
                        // addition!

                        orig_i -= 1;
                    }
                    else if (orig_id < new_id)
                    {
                        if (cat1.category_allow_multiple)
                            ret.element_lists.Add(cat1.element_lists[orig_i].GetCopy());
                        else
                            ret.elements.Add(cat1[orig_i].GetCopy());

                        new_i -= 1;
                    }
                }

                if (orig_i < cat1.GetElementCount())
                    orig_i += 1;
                if (new_i < cat2.GetElementCount())
                    new_i += 1;
            }

            return 0;
        }

        // this function creates a diff of categories of same type and version
        // diff contains all elements from cat2 that are not in cat1
        public static int Diff(SFCategory cat1, SFCategory cat2, out SFCategory ret)
        {
            ret = null;

            if (!cat1.category_is_known)
                return -1;
            if (!cat2.category_is_known)
                return -1;

            ret = new SFCategory()
            {
                block_length = 0,
                category_allow_multiple = cat1.category_allow_multiple,
                category_id = cat1.category_id,
                category_is_known = cat1.category_is_known,
                category_name = cat1.category_name,
                category_type = cat1.category_type,
                category_unknown_data = null,
                current_string = 0,
                elem_format = cat1.elem_format
            };
            ret.string_size = new int[cat1.string_size.Length];
            Array.Copy(cat1.string_size, ret.string_size, cat1.string_size.Length);

            // double list ladder
            int orig_i = 0;
            int new_i = 0;
            int orig_id, new_id;
            bool orig_end = false;
            bool new_end = false;

            while (true)
            {
                if (new_i == cat2.GetElementCount())
                    new_end = true;
                if (orig_i == cat1.GetElementCount())
                    orig_end = true;

                if (orig_end && new_end)
                    break;

                orig_id = cat1.GetElementID(orig_i);
                new_id = cat2.GetElementID(new_i);

                if (orig_end)
                {
                    ret.elements.Add(cat2[new_i].GetCopy());
                }
                else if (!new_end)
                {
                    if (orig_id == new_id)
                    {
                        if(cat1.category_allow_multiple)
                        {
                            SFCategoryElementList new_elem;
                            Diff(cat1.elem_format, cat1.element_lists[orig_i], cat2.element_lists[orig_i], out new_elem);
                            if(new_elem.Elements.Count > 0)
                                ret.element_lists.Add(new_elem);
                        }
                        else
                        {
                            if (!cat1[orig_i].SameAs(cat2[new_i]))
                                ret.elements.Add(cat2[new_i].GetCopy());
                        }
                    }
                    else if (orig_id > new_id)
                    {
                        if(cat2.category_allow_multiple)
                            ret.element_lists.Add(cat2.element_lists[new_i].GetCopy());
                        else
                            ret.elements.Add(cat2[new_i].GetCopy());
                        // addition!

                        orig_i -= 1;
                    }
                    else if (orig_id < new_id)
                    {
                        new_i -= 1;
                    }
                }

                if (orig_i < cat1.GetElementCount())
                    orig_i += 1;
                if (new_i < cat2.GetElementCount())
                    new_i += 1;
            }

            return 0;
        }



        //removes all elements and resets category
        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.unload() called, category name: " + category_name);
            elements.Clear();
        }
    }
}
