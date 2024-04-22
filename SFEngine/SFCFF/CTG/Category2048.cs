using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2048Item : ICategoryItem
    {
        public byte Level;
        public ushort HealthFactor;
        public ushort ManaFactor;
        public uint ExperienceRequired;
        public byte AttributePointLimit;
        public byte SkillPointLimit;
        public ushort DamageFactor;
        public ushort ArmorClassFactor;

        public int GetID() => Level;
        public void SetID(int id) => Level = (byte)id;
    }

    public class Category2048 : CategoryBaseSingle<Category2048Item>
    {
        public override string GetName()
        {
            return "Player level scaling";
        }

        public override short GetCategoryID()
        {
            return 2048;
        }

        public override short GetCategoryType()
        {
            return 3;
        }
    }
}
