using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFCFF.CTG
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Category2002Item: ICategoryItem
    {
        public ushort SpellID;
        public ushort SpellLineID;
        public fixed byte SkillReq[12];
        public ushort ManaCost;
        public uint CastTime;
        public uint RecastTime;
        public ushort MinRange;
        public ushort MaxRange;
        public byte CastType1;
        public byte CastType2;
        public fixed uint Params[10];
        public ushort EffectPower;
        public ushort EffectRange;

        public int GetID() => SpellID;
        public void SetID(int id) => SpellID = (ushort)id;

        public byte GetSkillReq(int index)
        {
            if((index < 0)||(index >= 12))
            {
                throw new Exception();
            }
            return SkillReq[index];
        }

        public uint GetParam(int index)
        {
            if ((index < 0) || (index >= 10))
            {
                throw new Exception();
            }
            return Params[index];
        }

        public byte GetSpellLevel()
        {
            return SkillReq[2];
        }
    }

    public class Category2002: CategoryBaseSingle<Category2002Item>
    {
        public override string GetName()
        {
            return "Spell data";
        }

        public override short GetCategoryID()
        {
            return 2002;
        }

        public override short GetCategoryType()
        {
            return 3;
        }

        public override List<string> GetSearchableFields()
        {
            return new()
            {
                "SpellID",
                "SpellLineID",
                "SkillReq[0]",
                "SkillReq[1]",
                "SkillReq[2]",
                "SkillReq[3]",
                "SkillReq[4]",
                "SkillReq[5]",
                "SkillReq[6]",
                "SkillReq[7]",
                "SkillReq[8]",
                "SkillReq[9]",
                "SkillReq[10]",
                "SkillReq[11]",
                "ManaCost",
                "CastTime",
                "RecastTime",
                "MinRange",
                "MaxRange",
                "CastType1",
                "CastType2",
                "Params[0]",
                "Params[1]",
                "Params[2]",
                "Params[3]",
                "Params[4]",
                "Params[5]",
                "Params[6]",
                "Params[7]",
                "Params[8]",
                "Params[9]",
                "EffectPower",
                "EffectRange"
            };
        }
    }
}
