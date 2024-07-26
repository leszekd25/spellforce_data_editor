using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2061Item : ICategoryItem
    {
        public uint QuestID;
        public uint ParentQuestID;
        public byte IsMainQuest;
        public ushort NameID;
        public ushort DescriptionID;
        public uint OrderIndex;

        public int GetID() => (int)QuestID;
        public void SetID(int id) => QuestID = (uint)id;
    }

    public class Category2061 : CategoryBaseSingle<Category2061Item>
    {
        public override string GetName()
        {
            return "Quest hierarchy/description data";
        }

        public override short GetCategoryID()
        {
            return 2061;
        }

        public override short GetCategoryType()
        {
            return 1;
        }
    }
}
