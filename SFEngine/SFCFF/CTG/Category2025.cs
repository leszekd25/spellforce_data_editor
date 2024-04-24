using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2025Item : ICategorySubItem
    {
        public ushort UnitID;
        public byte EquipmentIndex;
        public ushort ItemID;

        public int GetID() => UnitID;
        public void SetID(int id) => UnitID = (ushort)id;
        public int GetSubID() => EquipmentIndex;
        public void SetSubID(int subid) => EquipmentIndex = (byte)subid;

    }

    public class Category2025 : CategoryBaseMultiple<Category2025Item>
    {
        public override string GetName()
        {
            return "Unit equipment";
        }

        public override short GetCategoryID()
        {
            return 2025;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
