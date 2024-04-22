using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2032Item : ICategoryItem
    {
        public ushort TerrainID;
        public byte BlockValue;
        public byte CultivationFlags;

        public int GetID() => TerrainID;
        public void SetID(int id) => TerrainID = (ushort)id;
    }

    public class Category2032 : CategoryBaseSingle<Category2032Item>
    {
        public override string GetName()
        {
            return "Terrain data";
        }

        public override short GetCategoryID()
        {
            return 2032;
        }

        public override short GetCategoryType()
        {
            return 2;
        }
    }
}
