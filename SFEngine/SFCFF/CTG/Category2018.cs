using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2018Item : ICategoryItem
    {
        public ushort SpellItemID;
        public ushort EffectID;

        public int GetID() => SpellItemID;
        public void SetID(int id) => SpellItemID = (ushort)id;
    }

    public class Category2018 : CategoryBaseSingle<Category2018Item>
    {
        public override string GetName()
        {
            return "Item installed spell scroll link with spell";
        }

        public override short GetCategoryID()
        {
            return 2018;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
