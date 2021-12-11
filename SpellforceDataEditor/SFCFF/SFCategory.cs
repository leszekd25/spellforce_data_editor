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
        public bool AllowSubelementID;
        public int[] StringSizes;

        public ChunkFormatInfo(string elem_format, string name, bool allow_multiple, bool allow_subelem_id, int[] string_sizes = null) 
        { 
            ElementFormat = elem_format;
            Name = name;
            AllowMultiple = allow_multiple;
            AllowSubelementID = allow_subelem_id;
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
            ChunkFormats.Add(Tuple.Create((ushort)2002, (ushort)3), new ChunkFormatInfo("HHBBBBBBBBBBBBHIIHHBBIIIIIIIIIIHH", "Spell data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2054, (ushort)5), new ChunkFormatInfo("HHBBBBBsH", "Spell type data", false, false, new int[] { 64 }));
            ChunkFormats.Add(Tuple.Create((ushort)2056, (ushort)1), new ChunkFormatInfo("BBBBBB", "Unknown (1)", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2005, (ushort)9), new ChunkFormatInfo("HHBHHHHHHHHHHHHHHHHHIBHB", "Unit/hero stats", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2006, (ushort)1), new ChunkFormatInfo("HBBB", "Hero/worker skills", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2067, (ushort)1), new ChunkFormatInfo("HBH", "Hero spells", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2003, (ushort)4), new ChunkFormatInfo("HBBHHHHBIIB", "Item general info", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2004, (ushort)1), new ChunkFormatInfo("Hhhhhhhhhhhhhhhhhh", "Item armor data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2013, (ushort)1), new ChunkFormatInfo("HH", "Inventory spell scroll link with installed spell scroll", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2015, (ushort)2), new ChunkFormatInfo("HHHHHHHH", "Item weapon data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2017, (ushort)1), new ChunkFormatInfo("HBBBB", "Item skill requirements", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2014, (ushort)1), new ChunkFormatInfo("HBH", "Item weapon effects/inventory scroll link with spell", true, true));    // fix empty element
            ChunkFormats.Add(Tuple.Create((ushort)2012, (ushort)1), new ChunkFormatInfo("HBsH", "Item UI data", true, true, new int[] { 64 }));    // fix empty element
            ChunkFormats.Add(Tuple.Create((ushort)2018, (ushort)1), new ChunkFormatInfo("HH", "Item installed spell scroll link with spell", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2016, (ushort)3), new ChunkFormatInfo("HBBss", "Text data", true, true, new int[] { 50, 512 }));
            ChunkFormats.Add(Tuple.Create((ushort)2022, (ushort)7), new ChunkFormatInfo("BBBBBBBHBHBBHBBBBHHHB", "Race stats", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2023, (ushort)2), new ChunkFormatInfo("BBB", "Faction relations", true, true));
            ChunkFormats.Add(Tuple.Create((ushort)2024, (ushort)8), new ChunkFormatInfo("HHHIHIHBHHsB", "Unit general data/link with unit stats", false, false, new int[] { 40 }));
            ChunkFormats.Add(Tuple.Create((ushort)2025, (ushort)1), new ChunkFormatInfo("HBH", "Unit equipment", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2026, (ushort)1), new ChunkFormatInfo("HBH", "Unit spells", true, false));    // fix empty element
            ChunkFormats.Add(Tuple.Create((ushort)2028, (ushort)1), new ChunkFormatInfo("HBB", "Army unit resource requirements", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2040, (ushort)1), new ChunkFormatInfo("HBHBHBH", "Corpse loot", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2001, (ushort)1), new ChunkFormatInfo("HBH", "Army unit building requirements", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2029, (ushort)9), new ChunkFormatInfo("HBBBHHhhBHHHHB", "Building data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2030, (ushort)2), new ChunkFormatInfo("HBBC", "Building collision data", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2031, (ushort)2), new ChunkFormatInfo("HBH", "Building resource requirements", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2039, (ushort)1), new ChunkFormatInfo("BBH", "Skills link with text data", true, true));
            ChunkFormats.Add(Tuple.Create((ushort)2062, (ushort)1), new ChunkFormatInfo("BBBBBBBBB", "Skill point requirements", true, true));
            ChunkFormats.Add(Tuple.Create((ushort)2041, (ushort)1), new ChunkFormatInfo("HH", "Merchants link with unit general data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2042, (ushort)1), new ChunkFormatInfo("HHH", "Merchant inventory", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2047, (ushort)1), new ChunkFormatInfo("HBH", "Merchant sell/buy rate", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2044, (ushort)1), new ChunkFormatInfo("BH", "Resources link with data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2048, (ushort)3), new ChunkFormatInfo("BHHIBBHH", "Player level scaling", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2050, (ushort)6), new ChunkFormatInfo("HHBBBsHHH", "Environment objects data", false, false, new int[] { 41 }));
            ChunkFormats.Add(Tuple.Create((ushort)2057, (ushort)2), new ChunkFormatInfo("HBBC", "Interactive objects collision data", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2065, (ushort)1), new ChunkFormatInfo("HBHBHBH", "Environment object loot", true, false));
            ChunkFormats.Add(Tuple.Create((ushort)2051, (ushort)1), new ChunkFormatInfo("IH", "NPC link with text data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2052, (ushort)2), new ChunkFormatInfo("IBsH", "Map data", false, false, new int[] { 64 }));
            ChunkFormats.Add(Tuple.Create((ushort)2053, (ushort)3), new ChunkFormatInfo("HIHHBH", "Portal locations", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2055, (ushort)1), new ChunkFormatInfo("BBB", "Unknown (from sql_lua?)", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2058, (ushort)1), new ChunkFormatInfo("HH", "Description link with txt data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2059, (ushort)1), new ChunkFormatInfo("HHH", "Extended description data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2061, (ushort)1), new ChunkFormatInfo("IIBHHI", "Quest hierarchy/description data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2063, (ushort)1), new ChunkFormatInfo("HHB", "Weapon types", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2064, (ushort)1), new ChunkFormatInfo("HH", "Weapon materials", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2032, (ushort)2), new ChunkFormatInfo("HBB", "Terrain data", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2049, (ushort)1), new ChunkFormatInfo("BB", "Heads", false, false));
            ChunkFormats.Add(Tuple.Create((ushort)2036, (ushort)1), new ChunkFormatInfo("HHHHHHHHHHHsI", "Unit upgrade data", false, false, new int[] { 64 }));
            ChunkFormats.Add(Tuple.Create((ushort)2072, (ushort)1), new ChunkFormatInfo("BHB", "Item sets", false, false));
        }

        public List<SFCategoryElement> elements { get; protected set; } = new List<SFCategoryElement>();     //list of all elements
        public List<SFCategoryElementList> element_lists = new List<SFCategoryElementList>();                //list of all subelements; for any element in here, it will always have at least 1 subelement
        public List<SFCategoryElementStatus> element_status = new List<SFCategoryElementStatus>();           //list in which status of every element is kept
        public bool category_allow_multiple;                                                                 //if true, element_lists is used, if false, elements is used
        public bool category_allow_subelement_id;                                                            //if true, subelements have their own ID at position 1 on the list
        // of above, only one can be used at any time


        public uint block_length;                    //size of all data that belongs to this category
        public string elem_format;                   //element format (see get_single_variant)
        public int elem_base_size;                   //calculated from elem_format
        public string category_name;
        public int[] string_size;                    //if category element holds a string (one or more), a list of string lengths is required
        public int current_string;                   //helper variable to enable searching and manipulating string variants
        public short category_id;                       //each category has a unique id the game looks for when reading data
        public short category_type;
        public bool category_is_known;
        public byte[] category_unknown_data;

        public SFCategory()
        {

        }

        public SFCategory(Tuple<ushort, ushort> cat_id)
        {
            category_id = (short)cat_id.Item1;
            category_type = (short)cat_id.Item2;

            category_is_known = ChunkFormats.ContainsKey(cat_id);

            if (category_is_known)
            {
                ChunkFormatInfo cat_info = ChunkFormats[cat_id];
                category_name = cat_info.Name;
                string_size = cat_info.StringSizes;
                category_allow_multiple = cat_info.AllowMultiple;
                category_allow_subelement_id = cat_info.AllowSubelementID;
                elem_format = cat_info.ElementFormat;
                elem_base_size = GetEmptyElement().GetSize();

                if (string_size == null)
                    string_size = new int[] { 0 };
            }
            else
            {
                category_unknown_data = null;
            }
        }

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
                        elem.AddVariant(new SFString() { RawData = new byte[string_size[current_string]], LanguageID = 0 });
                        current_string = Math.Min(string_size.Length - 1, current_string + 1);
                        break;
                    case 'C':
                        elem.AddVariant(new SFOutlineData() { Data = new List<short>() });
                        break;
                    default:
                        LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFCategory.generate_empty_element(): Unrecognized variant type (category: " + category_name + ")");
                        throw new InvalidDataException("SFCategory.GenerateEmptyElement(): Unknown variant type!");
                }
            }

            // empty elements for certain categories dont have all zeros
            // todo: move this somewhere else
            switch(category_id)
            {
                case 2012:
                case 2014:
                case 2026:
                    {
                        elem[1] = (byte)1;
                    }
                    break;
                default:
                    break;
            }
            return elem;
        }

        // adds empty element list (because element lists must have at least 1 element, this will add an empty element to the list
        public SFCategoryElementList GetEmptyElementList()
        {
            SFCategoryElementList elem_list = new SFCategoryElementList();

            elem_list.Elements.Add(GetEmptyElement());
            elem_list.ElementStatus.Add(SFCategoryElementStatus.ADDED);

            return elem_list;
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
                    SFString str = new SFString() { RawData = sr.ReadBytes(s_size), LanguageID = 0 };
                    return str;
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
            else if (t == typeof(SFString))
                sw.Write(((SFString)var).RawData);
            else if (t == typeof(SFOutlineData))
            {
                if (((SFOutlineData)var).Data.Count < 3)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFCategory.WriteVariantToBuffer(): Insufficient outline points!");
                    throw new Exception("SFCategory.WriteVariantToBuffer(): Invalid outline data!");
                }
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
            if (sr.BaseStream.Position + elem_base_size > sr.BaseStream.Length)
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
        public SFCategoryElementList GetMultipleElementsFromBuffer(BinaryReader sr)
        {
            SFCategoryElementList elements_loaded = new SFCategoryElementList();
            char char_load = elem_format[0];
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
                    SFCategoryElement elem = new SFCategoryElement();
                    cur_id = next_id;
                    for (int i = 0; i < elem_format.Length; i++)
                        elem.AddVariant(ReadVariantFromBuffer(sr, elem_format[i], string_size[current_string]));

                    // sometimes gamedata is malformed; this attempts to fix issue here multiple sub-elements with the same sub-ID exist within one element
                    if (category_allow_subelement_id)
                    {
                        int cur_subelem_id = elem.ToInt(1);
                        int prev_subelem_index = elements_loaded.GetIndexByID(cur_subelem_id);
                        if (prev_subelem_index == Utility.NO_INDEX)
                            elements_loaded.Elements.Add(elem);
                        else
                            elements_loaded.Elements[prev_subelem_index] = elem;
                    }
                    else
                    {
                        elements_loaded.Elements.Add(elem);
                    }
                }
                else
                    break;
            }

            return elements_loaded;
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

        //searches for an element given column index and searched value and returns its index if it exists
        //else returns -1
        //this is binary search variant, and it requires that elements are sorted by given column
        //multiple only, and only looks for values in first element
        public int FindMultipleElementIndexBinary<T>(int v_index, T value) where T : IComparable
        {
            int current_start = 0;
            int current_end = element_lists.Count - 1;
            int current_center;
            T val;
            while (current_start <= current_end)
            {

                current_center = (current_start + current_end) / 2;    //care about overflow
                val = (T)element_lists[current_center].Elements[0].variants[v_index];
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
        public void WriteElementToBuffer(BinaryWriter sw, SFCategoryElement elem)
        {
            foreach(var v in elem.variants)
                WriteVariantToBuffer(sw, v);
        }

        //returns size of all category elements (in bytes)
        public int GetSize()
        {
            int s = 0;
            if (category_allow_multiple)
            {
                foreach(SFCategoryElementList elem_list in element_lists)
                {
                    foreach(SFCategoryElement elem in elem_list.Elements)
                    {
                        s += elem.GetSize();
                    }
                }
            }
            else
            {
                foreach (SFCategoryElement elem in elements)
                {
                    s += elem.GetSize();
                }
            }
            return s;
        }

        public int Read(SFChunkFileChunk sfcfc)
        {
            using (BinaryReader br = sfcfc.Open())
            {
                if (category_is_known)
                {
                    //int i = 0;
                    int ind = 0;
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        try
                        {
                            if (category_allow_multiple)
                            {
                                element_lists.Add(GetMultipleElementsFromBuffer(br));
                            }
                            else
                            {
                                SFCategoryElement elem = new SFCategoryElement();
                                elem.AddVariants(GetElementFromBuffer(br));
                                elements.Add(elem);
                            }
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

                       /* if (GetElementID(ind) < i)
                            i = 0;                           // breakpoint for when data is not sorted in ascending order

                        i = GetElementID(ind);*/
                        ind += 1;
                    }

                    LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.read(): Items read: " + ind.ToString());
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
                    if (category_allow_multiple)
                    {
                        for (int i = 0; i < GetElementCount(); i++)
                        {
                            for (int j = 0; j < element_lists[i].Elements.Count; j++)
                            {
                                WriteElementToBuffer(bw, element_lists[i].Elements[j]);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < GetElementCount(); i++)
                        {
                            WriteElementToBuffer(bw, elements[i]);
                        }
                    }
                }
            }

            return data;
        }

        public byte[] ToRawDataDiff()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.ToRawDataDiff() called, category name: " + category_name);
            UInt32 new_block_size_tmp = (UInt32)GetSize();
            byte[] data = new byte[new_block_size_tmp];
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    if (category_allow_multiple)
                    {
                        for (int i = 0; i < GetElementCount(); i++)
                        {
                            for (int j = 0; j < element_lists[i].Elements.Count; j++)
							{
								if ((element_lists[i].ElementStatus[j] == SFCategoryElementStatus.ADDED) || (element_lists[i].ElementStatus[j] == SFCategoryElementStatus.MODIFIED))
								{
									WriteElementToBuffer(bw, element_lists[i].Elements[j]);
								}
							}
                        }
                    }
                    else
                    {
                        for (int i = 0; i < GetElementCount(); i++)
                        {
                            if ((element_status[i] == SFCategoryElementStatus.ADDED) || (element_status[i] == SFCategoryElementStatus.MODIFIED))
                            {
                                WriteElementToBuffer(bw, elements[i]);
                            }
                        }
                    }

                    new_block_size_tmp = (UInt32)bw.BaseStream.Position;
                }
            }

            if (new_block_size_tmp == 0)
                return null;

            return data.Take((int)new_block_size_tmp).ToArray();
        }

        public int Write(SFChunkFile sfcf)
        {
            sfcf.AddChunk(category_id, 0, false, category_type, ToRawData());
            return 0;
        }

        public int WriteDiff(SFChunkFile sfcf)
        {
            byte[] data = ToRawDataDiff();
            if (data != null)
                sfcf.AddChunk(category_id, 0, false, category_type, data);
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
            if (index < 0)
                return Utility.NO_INDEX;

            if (category_allow_multiple)
            {
                if (index >= element_lists.Count)
                    return Utility.NO_INDEX;
                return element_lists[index].GetID();
            }
            else
            {
                if (index >= elements.Count)
                    return Utility.NO_INDEX;
                return elements[index].ToInt(0);
            }
        }

        // returns index of an element with specified id (or -1, if it doesnt exist)
        public virtual int GetElementIndex(int id)
        {
            if (category_allow_multiple)
            {
                switch (elem_format[0])
                {
                    case 'B':
                        return FindMultipleElementIndexBinary(0, (Byte)id);
                    case 'H':
                        return FindMultipleElementIndexBinary(0, (UInt16)id);
                    case 'I':
                        return FindMultipleElementIndexBinary(0, (UInt32)id);
                    default:
                        return Utility.NO_INDEX;
                }
            }
            else
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
        }

        // if an element of id X was to be inserted into a list, where should it be placed to preserve ascending order?
        // this function ansvers the question above
        // returns -1 if such element ID already exists
        public int GetNewElementIndex(int id)
        {
            int current_start = 0;
            int current_end = GetElementCount() - 1;
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

        // if an element of id greater or equal X was to be inserted into a list, where should it be placed to preserve order, and which ID should it have?
        // only for categories without multiple elements
        // O(n)
        public int GetNextNewElementIndex(int id, out int new_id)
        {
            new_id = id;

            int start_index = Utility.NO_INDEX;

            int c = GetElementCount();
            for(int i = 0; i < c; i++)
            {
                if(GetElementID(i) >= id)
                {
                    if(GetElementID(i) == id)
                    {
                        // there already exists element with given ID
                        start_index = i;
                        break;
                    }
                    else
                    {
                        // there is no element with given ID, return now
                        return i;
                    }
                }
            }

            // all IDs are smaller than new ID -> there is no element with given ID
            if (start_index == Utility.NO_INDEX)
                return c;

            for(int i = start_index+1; i < c; i++)
            {
                // next ID is successor of this ID
                if (GetElementID(i) == id + 1)
                    id += 1;
                else
                {
                    // there is a gap between this ID and next ID, so set return ID to fit the gap
                    new_id = id + 1;
                    return i;
                }
            }

            // all IDs are sequential -> set return ID to next after last
            new_id = id + 1;
            return c - 1;
        }

        // this function merges element lists of the same type, as long as the element allows multiple subelements
        // used on categories which allow subelement IDs
        public static int MergeWithSubID(SFCategoryElementList e1, SFCategoryElementList e2, out SFCategoryElementList ret)
        {
            ret = new SFCategoryElementList();

            // make dictionary for both elements: key: subelement id, value: subelement position
            int max_id = -1;
            Dictionary<int, int> subelem1 = new Dictionary<int, int>();
            Dictionary<int, int> subelem2 = new Dictionary<int, int>();

            for (int i = 0; i < e1.Elements.Count; i++)
            {
                int id = e1.Elements[i].ToInt(1);
                if (!subelem1.ContainsKey(id))
                    subelem1.Add(id, i);
                else
                    subelem1[id] = i;
                max_id = Math.Max(max_id, id);
            }

            for (int i = 0; i < e2.Elements.Count; i++)
            {
                int id = e2.Elements[i].ToInt(1);
                if (!subelem2.ContainsKey(id))
                    subelem2.Add(id, i);
                else
                    subelem2[id] = i;
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

        // used on categories which do not have subelement IDs
        public static int MergeWithoutSubID(SFCategoryElementList e1, SFCategoryElementList e2, out SFCategoryElementList ret)
        {
            ret = new SFCategoryElementList();
            for(int i = 0; i < e2.Elements.Count; i++)
            {
                ret.Elements.Add(e2.Elements[i].GetCopy());
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
                category_allow_subelement_id = cat1.category_allow_subelement_id,
                category_id = cat1.category_id,
                category_is_known = cat1.category_is_known,
                category_name = cat1.category_name,
                category_type = cat1.category_type,
                category_unknown_data = null,
                current_string = 0,
                elem_format = cat1.elem_format,
                elem_base_size = cat1.GetEmptyElement().GetSize()
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
                            if (cat1.category_allow_subelement_id)
                                MergeWithSubID(cat1.element_lists[orig_i], cat2.element_lists[new_i], out new_elem);
                            else
                                MergeWithoutSubID(cat1.element_lists[orig_i], cat2.element_lists[new_i], out new_elem);
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

        // calculates status of each element on the list in relation to original element list, and stores the result in an external list
        // used for categories that allow subelement IDs
        public static void CalculateStatusWithSubID(SFCategoryElementList elem_base, SFCategoryElementList elem_new, ref SFCategoryElementList elem_status)
        {
            elem_status.ElementStatus.Clear();

            // make dictionary for both elements: key: subelement id, value: subelement position
            int max_id = -1;
            Dictionary<int, int> subelem1 = new Dictionary<int, int>();
            Dictionary<int, int> subelem2 = new Dictionary<int, int>();

            for (int i = 0; i < elem_base.Elements.Count; i++)
            {
                int id = elem_base.Elements[i].ToInt(1);
                subelem1.Add(id, i);
                max_id = Math.Max(max_id, id);
            }

            for (int i = 0; i < elem_new.Elements.Count; i++)
            {
                int id = elem_new.Elements[i].ToInt(1);
                subelem2.Add(id, i);
                max_id = Math.Max(max_id, id);
            }

            for (int i = 0; i <= max_id; i++)
            {
                if(subelem2.ContainsKey(i))
                {
                    if(subelem1.ContainsKey(i))
                    {
                        if (elem_base[subelem1[i]].SameAs(elem_new[subelem2[i]]))
                            elem_status.ElementStatus.Add(SFCategoryElementStatus.UNCHANGED);
                        else
                            elem_status.ElementStatus.Add(SFCategoryElementStatus.MODIFIED);
                    }
                    else
                    {
                        elem_status.ElementStatus.Add(SFCategoryElementStatus.ADDED);
                    }
                }
                else if(subelem1.ContainsKey(i))
                {
                    elem_status.ElementStatus.Add(SFCategoryElementStatus.REMOVED);
                }
            }

            if (elem_status.Elements.Count != elem_status.ElementStatus.Count)
                throw new Exception("meme");
        }

        public static void CalculateStatusWithoutSubID(SFCategoryElementList elem_base, SFCategoryElementList elem_new, ref SFCategoryElementList elem_status)
        {
            elem_status.ElementStatus.Clear();

            if (elem_base.SameAs(elem_new))
            {
                for (int i = 0; i < elem_new.Elements.Count; i++)
                {
                    elem_status.ElementStatus.Add(SFCategoryElementStatus.UNCHANGED);
                }
            }
            else
            {
                for (int i = 0; i < elem_new.Elements.Count; i++)
                {
                    elem_status.ElementStatus.Add(SFCategoryElementStatus.ADDED);
                }
            }

            // make dictionary for both elements: key: subelement id, value: subelement position
            /*Dictionary<int, int> subelem1 = new Dictionary<int, int>();
            Dictionary<int, int> subelem2 = new Dictionary<int, int>();

            for (int i = 0; i < elem_base.Elements.Count; i++)
            {
                int id = elem_base.Elements[i].GetHashCode();
                subelem1.Add(id, i);
            }

            for (int i = 0; i < elem_new.Elements.Count; i++)
            {
                int id = elem_new.Elements[i].GetHashCode();
                subelem2.Add(id, i);
            }

            foreach (var kv in subelem1)
            {
                if (subelem2.ContainsKey(kv.Key))
                    elem_status.ElementStatus.Add(SFCategoryElementStatus.UNCHANGED);
                else
                    elem_status.ElementStatus.Add(SFCategoryElementStatus.REMOVED);
            }
            foreach (var kv in subelem2)
            {
                if (!subelem1.ContainsKey(kv.Key))
                    elem_status.ElementStatus.Add(SFCategoryElementStatus.ADDED);
            }


            if (elem_status.Elements.Count != elem_status.ElementStatus.Count)
                throw new Exception("meme");*/
        }

        // only used if both categories are of the same ID and type, and are known
        // detects if new elements were added to the gamedata compared to some other gamedata
        public static void CalculateStatus(SFCategory cat_base, SFCategory cat_new, ref SFCategory cat_status)
        {
            // double list ladder
            int orig_i = 0;
            int new_i = 0;
            int orig_id, new_id;
            bool orig_end = false;
            bool new_end = false;

            while (true)
            {
                if (new_i == cat_new.GetElementCount())
                    new_end = true;
                if (orig_i == cat_base.GetElementCount())
                    orig_end = true;

                if (orig_end && new_end)
                    break;

                orig_id = cat_base.GetElementID(orig_i);
                new_id = cat_new.GetElementID(new_i);

                if (orig_end)
                {
                    cat_status.element_status.Add(SFCategoryElementStatus.ADDED);
                    if (cat_base.category_allow_multiple)
                        cat_status.element_lists[new_i].SetStatusAll(SFCategoryElementStatus.ADDED);

                }
                else if (new_end)
                {
                    cat_status.element_status.Add(SFCategoryElementStatus.REMOVED);
                    if (cat_base.category_allow_multiple)
                        cat_status.element_lists[orig_i].SetStatusAll(SFCategoryElementStatus.REMOVED);
                }
                else   // orig_i == new_i
                {
                    if (orig_id == new_id)
                    {
                        if (cat_base.category_allow_multiple)
                        {
                            if (cat_base.element_lists[orig_i].SameAs(cat_new.element_lists[new_i]))
                            {
                                cat_status.element_status.Add(SFCategoryElementStatus.UNCHANGED);
                                cat_status.element_lists[cat_status.element_status.Count-1].SetStatusAll(SFCategoryElementStatus.UNCHANGED);
                            }
                            else
                            {
                                cat_status.element_status.Add(SFCategoryElementStatus.MODIFIED);
                                SFCategoryElementList list_status = cat_status.element_lists[cat_status.element_status.Count - 1];
                                if (cat_status.category_allow_subelement_id)
                                    CalculateStatusWithSubID(cat_base.element_lists[orig_i], cat_new.element_lists[new_i], ref list_status);
                                else
                                    CalculateStatusWithoutSubID(cat_base.element_lists[orig_i], cat_new.element_lists[new_i], ref list_status);
                            }
                        }
                        else
                        {
                            if (cat_base[orig_i].SameAs(cat_new[new_i]))
                                cat_status.element_status.Add(SFCategoryElementStatus.UNCHANGED);
                            else
                                cat_status.element_status.Add(SFCategoryElementStatus.MODIFIED);
                        }
                    }
                    else if (orig_id > new_id)
                    {
                        cat_status.element_status.Add(SFCategoryElementStatus.ADDED);
                        if (cat_base.category_allow_multiple)
                            cat_status.element_lists[cat_status.element_status.Count - 1].SetStatusAll(SFCategoryElementStatus.ADDED);

                        orig_i -= 1;
                    }
                    else if (orig_id < new_id)
                    {
                        cat_status.element_status.Add(SFCategoryElementStatus.REMOVED);
                        if (cat_base.category_allow_multiple)
                            cat_status.element_lists[cat_status.element_status.Count - 1].SetStatusAll(SFCategoryElementStatus.REMOVED);

                        new_i -= 1;
                    }
                }

                if (orig_i < cat_base.GetElementCount())
                    orig_i += 1;
                if (new_i < cat_new.GetElementCount())
                    new_i += 1;
            }
        }

        public void special_cat2016_DetermineLanguageIDs()
        {
            foreach(var list in element_lists)
            {
                for(int i = 0; i < list.Elements.Count; i++)
                {
                    byte l_id = (byte)(list[i][1]);
                    list[i][4] = new SFString() { LanguageID = l_id, RawData = ((SFString)(list[i][4])).RawData };
                }
            }
        }



        //removes all elements and resets category
        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFCategory.unload() called, category name: " + category_name);
            elements.Clear();
        }
    }
}
