using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2029Item : ICategoryItem
    {
        public ushort BuildingID;
        public byte RaceID;
        public byte CanEnter;
        public byte Slots;
        public ushort Health;
        public ushort NameID;
        public short RotCenterX;
        public short RotCenterY;
        public byte NumOfPolygons;
        public ushort WorkerCycleTime;
        public ushort BuildingReqID;
        public ushort InitialAngle;
        public ushort DescriptionExtID;
        public byte Flags;

        public int GetID() => BuildingID;
        public void SetID(int id) => BuildingID = (ushort)id;
    }

    public class Category2029 : CategoryBaseSingle<Category2029Item>
    {
        public override string GetName()
        {
            return "Building data";
        }

        public override short GetCategoryID()
        {
            return 2029;
        }

        public override short GetCategoryType()
        {
            return 9;
        }
    }
}
