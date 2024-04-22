using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2051Item : ICategoryItem
    {
        public uint NPCID;
        public ushort TextID;

        public int GetID() => (int)NPCID;
        public void SetID(int id) => NPCID = (uint)id;
    }

    public class Category2051 : CategoryBaseSingle<Category2051Item>
    {
        public override string GetName()
        {
            return "NPC link with text data";
        }

        public override short GetCategoryID()
        {
            return 2051;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
