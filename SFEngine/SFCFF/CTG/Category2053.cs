using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2053Item : ICategoryItem
    {
        public ushort PortalID;
        public uint MapID;
        public ushort PosX;
        public ushort PosY;
        public byte IsDefault;
        public ushort NameID;

        public int GetID() => PortalID;
        public void SetID(int id) => PortalID = (ushort)id;
    }

    public class Category2053 : CategoryBaseSingle<Category2053Item>
    {
        public override string GetName()
        {
            return "Portal locations";
        }

        public override short GetCategoryID()
        {
            return 2053;
        }

        public override short GetCategoryType()
        {
            return 3;
        }
    }
}
