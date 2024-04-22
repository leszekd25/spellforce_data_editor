using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2004Item : ICategoryItem
    {
        public ushort ItemID;
        public short Strength;
        public short Stamina;
        public short Agility;
        public short Dexterity;
        public short Health;
        public short Charisma;
        public short Intelligence;
        public short Wisdom;
        public short Mana;
        public short Armor;
        public short ResistFire;
        public short ResistIce;
        public short ResistBlack;
        public short ResistMind;
        public short SpeedWalk;
        public short SpeedFight;
        public short SpeedCast;

        public int GetID() => ItemID;
        public void SetID(int id) => ItemID = (ushort)id;
    }

    public class Category2004 : CategoryBaseSingle<Category2004Item>
    {
        public override string GetName()
        {
            return "Item armor data";
        }

        public override short GetCategoryID()
        {
            return 2004;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
