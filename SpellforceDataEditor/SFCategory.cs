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
        protected SFCategoryElement[] elements;
        protected string elem_format;
        protected int elem_size;
        protected int[] string_size;   //if category element holds a string (one or more), a list of string lengths is required
        protected int current_string;

        //constructor (requires block size in bytes)
        //each block has different size, and it determines how many elements belong to a given category
        public SFCategory(int size)
        {
            block_length = (uint)size;
            string_size = new int[1] { 0 };
        }

        //calculates size of element given a format, and calculates number of elements that belong to this category
        protected void initialize(string fm)
        {
            elem_format = fm;
            calculate_element_size(elem_format);
            item_count = (uint)(block_length / elem_size);
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
                    current_string = Math.Min(string_size.Length, current_string + 1);
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
        public void read(BinaryReader sr)
        {
            elements = new SFCategoryElement[item_count];
            for (int i = 0; i < item_count; i++)
            {
                elements[i].set(get_element(sr));
            }
        }

        //writes all elements to a buffer
        public void write(BinaryWriter sw)
        {
            for (int i = 0; i < item_count; i++)
            {
                put_element(sw, elements[i].get());
            }
        }

        //public abstract SFVariant[] set_element(StreamWriter sw);

        //public abstract void write(StreamWriter sw);
    }

    //spells/skills (1st category)
    public class SFCategory1 : SFCategory
    {
        public SFCategory1(int size) : base(size)
        {
            initialize("HHBBBBBBBBBBBBHIIHHIIIIIIIIIIBBBB");
        }
    }

    //spell ui (2nd category)
    public class SFCategory2 : SFCategory
    {
        public SFCategory2(int size) : base(size)
        {
            string_size = new int[1] { 64 };   //must go before initialize
            initialize("HHBBBBBsBB");
        }
    }

    //unknown1 (3rd category)
    public class SFCategory3 : SFCategory
    {
        public SFCategory3(int size) : base(size)
        {
            initialize("BBBBBB");
        }
    }

    //unit/hero stats (4th category)
    public class SFCategory4 : SFCategory
    {
        public SFCategory4(int size) : base(size)
        {
            initialize("HHBHHHHHHHHHHHHHHHHHIBHB");
        }
    }

    //hero magic/combat arms and worker gathering (5th category)
    public class SFCategory5 : SFCategory
    {
        public SFCategory5(int size) : base(size)
        {
            initialize("HBBB");
        }
    }

    //hero skills/spells (6th category)
    public class SFCategory6 : SFCategory
    {
        public SFCategory6(int size) : base(size)
        {
            initialize("HBH");
        }
    }

    //item type/name ID/price (7th category)
    public class SFCategory7 : SFCategory
    {
        public SFCategory7(int size) : base(size)
        {
            initialize("HHHHHHBIIB");
        }
    }

    //armor class item stats (8th category)
    public class SFCategory8 : SFCategory
    {
        public SFCategory8(int size) : base(size)
        {
            initialize("Hhhhhhhhhhhhhhhhhh");
        }
    }

    //scroll ID with spell ID/rune ID with rune in slot... (9th category)
    public class SFCategory9 : SFCategory
    {
        public SFCategory9(int size) : base(size)
        {
            initialize("HH");
        }
    }

    //weapon class item sets (10th category)
    public class SFCategory10 : SFCategory
    {
        public SFCategory10(int size) : base(size)
        {
            initialize("HHHHHHBBBB");
        }
    }

    //item requirements (11th category)
    public class SFCategory11 : SFCategory
    {
        public SFCategory11(int size) : base(size)
        {
            initialize("HBBBB");
        }
    }

    //item spell effects (12th category)
    public class SFCategory12 : SFCategory
    {
        public SFCategory12(int size) : base(size)
        {
            initialize("HBH");
        }
    }

    //item ui (13th category)
    public class SFCategory13 : SFCategory
    {
        public SFCategory13(int size) : base(size)
        {
            string_size = new int[1] { 64 };
            initialize("HBsH");
        }
    }

    //spell item ID with spell number (14th category)
    public class SFCategory14 : SFCategory
    {
        public SFCategory14(int size) : base(size)
        {
            initialize("HH");
        }
    }

    //text IDs (15th category)
    public class SFCategory15 : SFCategory
    {
        public SFCategory15(int size) : base(size)
        {
            string_size = new int[2] { 50, 512 };
            initialize("HBBss");
        }
    }

    //race stats (16th category)
    public class SFCategory16 : SFCategory
    {
        public SFCategory16(int size) : base(size)
        {
            initialize("BBBBBBBHBBBBBBBBBBBBBBBBBB");
        }
    }

    //head stats (17th category)
    public class SFCategory17 : SFCategory
    {
        public SFCategory17(int size) : base(size)
        {
            initialize("BBB");
        }
    }

    //unit names (18th category)
    public class SFCategory18 : SFCategory
    {
        public SFCategory18(int size) : base(size)
        {
            string_size = new int[1] { 40 };
            initialize("HHHIHHHHHBHsB");
        }
    }

    //unit equipment (19th category)
    public class SFCategory19 : SFCategory
    {
        public SFCategory19(int size) : base(size)
        {
            initialize("HBH");
        }
    }

    //unit equspells/skills (20th category)
    public class SFCategory20 : SFCategory
    {
        public SFCategory20(int size) : base(size)
        {
            initialize("HBH");
        }
    }

    //hero army unit resource requirements and... (21st category)
    public class SFCategory21 : SFCategory
    {
        public SFCategory21(int size) : base(size)
        {
            initialize("HBB");
        }
    }

    //unit corpse loot (22nd category)
    public class SFCategory22 : SFCategory
    {
        public SFCategory22(int size) : base(size)
        {
            initialize("HBHBHBH");
        }
    }

    //hero army unit building upgrade (23rd category)
    public class SFCategory23 : SFCategory
    {
        public SFCategory23(int size) : base(size)
        {
            initialize("HBH");
        }
    }

    //building stats (24th category)
    public class SFCategory24 : SFCategory
    {
        public SFCategory24(int size) : base(size)
        {
            initialize("HBBBBBBBBBBBBBBBBBBBBB");
        }
    }

    //building stats2 (25th category)
    public class SFCategory25 : SFCategory
    {
        public SFCategory25(int size) : base(size)
        {
            initialize("");    //todo
        }
    }

    //building requirements (26th category)
    public class SFCategory26 : SFCategory
    {
        public SFCategory26(int size) : base(size)
        {
            initialize("HBH");
        }
    }

    //combat arms/magic ID with name ID (27th category)
    public class SFCategory27 : SFCategory
    {
        public SFCategory27(int size) : base(size)
        {
            initialize("BBBB");
        }
    }

    //skills requirements (28th category)
    public class SFCategory28 : SFCategory
    {
        public SFCategory28(int size) : base(size)
        {
            initialize("BBBBBBBBB");
        }
    }

    //merchant ID with unit ID (29th category)
    public class SFCategory29 : SFCategory
    {
        public SFCategory29(int size) : base(size)
        {
            initialize("HH");
        }
    }

    //merchant's inventory (30th category)
    public class SFCategory30 : SFCategory
    {
        public SFCategory30(int size) : base(size)
        {
            initialize("HHH");
        }
    }

    //merchant's sell and buy rate (?) (31th category)
    public class SFCategory31 : SFCategory
    {
        public SFCategory31(int size) : base(size)
        {
            initialize("BBBBB");
        }
    }

    //sql_good' names (32nd category)
    public class SFCategory32 : SFCategory
    {
        public SFCategory32(int size) : base(size)
        {
            initialize("BH");
        }
    }

    //player level stats (33rd category)
    public class SFCategory33 : SFCategory
    {
        public SFCategory33(int size) : base(size)
        {
            initialize("BHHIBBHH");
        }
    }

    //object stats/names (34th category)
    public class SFCategory34 : SFCategory
    {
        public SFCategory34(int size) : base(size)
        {
            string_size = new int[1] { 47 };
            initialize("HBBBBBs");
        }
    }

    //monument/other world interactive object stats (35th category)
    public class SFCategory35 : SFCategory
    {
        public SFCategory35(int size) : base(size)
        {
            initialize("");     //todo
        }
    }

    //chest/corpse loot (36th category)
    public class SFCategory36 : SFCategory
    {
        public SFCategory36(int size) : base(size)
        {
            initialize("HBHBHBH");
        }
    }

    //unknown2 (37th category)
    public class SFCategory37 : SFCategory
    {
        public SFCategory37(int size) : base(size)
        {
            initialize("HHH");
        }
    }

    //quest maps (?) (38th category)
    public class SFCategory38 : SFCategory
    {
        public SFCategory38(int size) : base(size)
        {
            string_size = new int[1] { 64 };
            initialize("HHBsH");
        }
    }

    //portals locations (39th category)
    public class SFCategory39 : SFCategory
    {
        public SFCategory39(int size) : base(size)
        {
            initialize("HHHHHBH");
        }
    }

    //unknown (from sql_lua?) (40th category)
    public class SFCategory40 : SFCategory
    {
        public SFCategory40(int size) : base(size)
        {
            initialize("BBB");
        }
    }

    //quest game menu (41st category)
    public class SFCategory41 : SFCategory
    {
        public SFCategory41(int size) : base(size)
        {
            initialize("HH");
        }
    }

    //game/button/menu description (42nd category)
    public class SFCategory42 : SFCategory
    {
        public SFCategory42(int size) : base(size)
        {
            initialize("HHH");
        }
    }

    //quest IDs (43rd category)
    public class SFCategory43 : SFCategory
    {
        public SFCategory43(int size) : base(size)
        {
            initialize("HBBBBBBBBBBBBBBB");
        }
    }

    //uweapon type stats (44th category)
    public class SFCategory44 : SFCategory
    {
        public SFCategory44(int size) : base(size)
        {
            initialize("HBBB");
        }
    }

    //weapon materials (45th category)
    public class SFCategory45 : SFCategory
    {
        public SFCategory45(int size) : base(size)
        {
            initialize("HBB");
        }
    }

    //unknown3 (46th category)
    public class SFCategory46 : SFCategory
    {
        public SFCategory46(int size) : base(size)
        {
            initialize("HBB");
        }
    }

    //heads (47th category)
    public class SFCategory47 : SFCategory
    {
        public SFCategory47(int size) : base(size)
        {
            initialize("BB");
        }
    }

    //button upgrade stats ui (48th category)
    public class SFCategory48 : SFCategory
    {
        public SFCategory48(int size) : base(size)
        {
            string_size = new int[1] { 40 };
            initialize("HHHHHHHHHHHsHI");
        }
    }

    //item sets (49th category)
    public class SFCategory49 : SFCategory
    {
        public SFCategory49(int size) : base(size)
        {
            initialize("BHB");
        }
    }

}
