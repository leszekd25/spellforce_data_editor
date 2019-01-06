using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace SpellforceDataEditor.SFMod
{
    public enum SFModBytePatchParameterBasicType
    {
        NONE = -1, U8, U16, U32, U64, I8, I16, I32, I64, FLOAT, DOUBLE, CHAR, WCHAR
    }

    public enum SFModBytePatchParameterComplexType
    {
        NONE = -1, STRUCT, ARRAY
    }

    public class SFModBytePatchParameterNode
    {
        SFModBytePatchParameterComplexType type1;
        SFModBytePatchParameterBasicType type2;
        List<SFModBytePatchParameterNode> nodes = null;
        object value = null; // BasicType, array of Byte, or null if type1 == STRUCT
                             // this is default value btw

        public void WriteBasicValue(BinaryWriter bw)
        {
            switch(type2)
            {
                case SFModBytePatchParameterBasicType.U8:
                    bw.Write((byte)value);
                    break;
                case SFModBytePatchParameterBasicType.U16:
                    bw.Write((UInt16)value);
                    break;
                case SFModBytePatchParameterBasicType.U32:
                    bw.Write((UInt32)value);
                    break;
                case SFModBytePatchParameterBasicType.U64:
                    bw.Write((UInt64)value);
                    break;
                case SFModBytePatchParameterBasicType.I8:
                    bw.Write((SByte)value);
                    break;
                case SFModBytePatchParameterBasicType.CHAR:
                    bw.Write((char)value);
                    break;
                case SFModBytePatchParameterBasicType.I16:
                case SFModBytePatchParameterBasicType.WCHAR:
                    bw.Write((Int16)value);
                    break;
                case SFModBytePatchParameterBasicType.I32:
                    bw.Write((Int32)value);
                    break;
                case SFModBytePatchParameterBasicType.I64:
                    bw.Write((Int64)value);
                    break;
                case SFModBytePatchParameterBasicType.FLOAT:
                    bw.Write((Single)value);
                    break;
                case SFModBytePatchParameterBasicType.DOUBLE:
                    bw.Write((Double)value);
                    break;
                default:
                    throw new InvalidDataException("SFModBytePatchParameterNode.WriteBasicValue: Invalid type!");
            }
        }

        public object ReadBasicValue(BinaryReader br)
        {
            switch(type2)
            {
                case SFModBytePatchParameterBasicType.U8:
                    return br.ReadByte();
                    break;
                case SFModBytePatchParameterBasicType.U16:
                    return br.ReadUInt16();
                    break;
                case SFModBytePatchParameterBasicType.U32:
                    return br.ReadUInt32();
                    break;
                case SFModBytePatchParameterBasicType.U64:
                    return br.ReadUInt64();
                    break;
                case SFModBytePatchParameterBasicType.I8:
                    return br.ReadSByte();
                    break;
                case SFModBytePatchParameterBasicType.CHAR:
                    return br.ReadChar();
                    break;
                case SFModBytePatchParameterBasicType.I16:
                case SFModBytePatchParameterBasicType.WCHAR:
                    return br.ReadInt16();
                    break;
                case SFModBytePatchParameterBasicType.I32:
                    return br.ReadInt32();
                    break;
                case SFModBytePatchParameterBasicType.I64:
                    return br.ReadInt64();
                    break;
                case SFModBytePatchParameterBasicType.FLOAT:
                    return br.ReadSingle();
                    break;
                case SFModBytePatchParameterBasicType.DOUBLE:
                    return br.ReadDouble();
                    break;
                default:
                    throw new InvalidDataException("SFModBytePatchParameterNode.WriteBasicValue: Invalid type!");
            }
        }

        // recursive!
        public int Load(BinaryReader br)
        {
            type1 = (SFModBytePatchParameterComplexType)br.ReadSByte();
            type2 = (SFModBytePatchParameterBasicType)br.ReadSByte();
            if (type1 == SFModBytePatchParameterComplexType.NONE)
                value = ReadBasicValue(br);
            else if (type1 == SFModBytePatchParameterComplexType.ARRAY)
            {
                int bnum = br.ReadInt32();
                value = br.ReadBytes(bnum);
            }
            else if(type1 == SFModBytePatchParameterComplexType.STRUCT)
            {
                int pnum = br.ReadInt32();
                nodes.Clear();
                for (int i = 0; i < pnum; i++)
                {
                    SFModBytePatchParameterNode snode = new SFModBytePatchParameterNode();
                    snode.Load(br);
                    nodes.Add(snode);
                }
            }
            return 0;
        }

        public int Save(BinaryWriter bw)
        {
            bw.Write((SByte)type1);
            bw.Write((SByte)type2);
            if (type1 == SFModBytePatchParameterComplexType.NONE)
                WriteBasicValue(bw);
            else if(type1 == SFModBytePatchParameterComplexType.ARRAY)
            {
                bw.Write(((byte[])value).Length);
                bw.Write((byte[])value);
            }
            else if(type1 == SFModBytePatchParameterComplexType.STRUCT)
            {
                bw.Write(nodes.Count);
                for(int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Save(bw);
                }
            }
            return 0;
        }
    }

    public class SFModBytePatchParameter
    {
        public int patch_id;
        public int patch_byte_offset;
        public SFModBytePatchParameterNode param = new SFModBytePatchParameterNode();

        public int Load(BinaryReader br)
        {
            patch_id = br.ReadInt32();
            patch_byte_offset = br.ReadInt32();
            param.Load(br);
            return 0;
        }

        public int Save(BinaryWriter bw)
        {
            bw.Write(patch_id);
            bw.Write(patch_byte_offset);
            param.Save(bw);
            return 0;
        }
    }

    public struct SFModBytePatch
    {
        public int offset;
        public byte[] bytes;

        public static SFModBytePatch Load(BinaryReader br)
        {
            SFModBytePatch patch = new SFModBytePatch();
            patch.offset = br.ReadInt32();
            int patch_size = br.ReadInt32();
            patch.bytes = br.ReadBytes(patch_size);
            return patch;
        }

        public int Save(BinaryWriter bw)
        {
            bw.Write(offset);
            bw.Write(bytes);
            return 0;
        }
    }

    public class SFModBytePatchContainer
    {
        public int version_major, version_minor, version_number;
        public List<SFModBytePatch> patches = new List<SFModBytePatch>();
        public List<SFModBytePatchParameter> parameters = new List<SFModBytePatchParameter>();

        public int Load(BinaryReader br)
        {
            patches.Clear();
            parameters.Clear();
            version_major = br.ReadInt32();
            version_minor = br.ReadInt32();
            version_number = br.ReadInt32();
            int patch_count = br.ReadInt32();
            int param_count = br.ReadInt32();
            for(int i = 0; i < patch_count; i++)
            {
                patches.Add(SFModBytePatch.Load(br));
            }
            for(int i = 0; i < param_count; i++)
            {
                SFModBytePatchParameter param = new SFModBytePatchParameter();
                param.Load(br);
                parameters.Add(param);
            }
            return 0;
        }

        public int Save(BinaryWriter bw)
        {
            bw.Write(version_major);
            bw.Write(version_minor);
            bw.Write(version_number);
            bw.Write(patches.Count);
            bw.Write(parameters.Count);
            for(int i = 0; i < patches.Count; i++)
            {
                patches[i].Save(bw);
            }
            for(int i = 0; i < parameters.Count; i++)
            {
                parameters[i].Save(bw);
            }
            return 0;
        }
    }

    public class SFModBytePatchManager
    {
        public const int GAME_OFFSET = 0x400000;
        public List<SFModBytePatchContainer> patches_by_version { get; private set; } = new List<SFModBytePatchContainer>();

        public int Load(BinaryReader br)
        {
            patches_by_version.Clear();
            int version_count = br.ReadInt32();
            for(int i = 0; i < version_count; i++)
            {
                SFModBytePatchContainer patch_cont = new SFModBytePatchContainer();
                patch_cont.Load(br);
                patches_by_version.Add(patch_cont);
            }
            return 0;
        }

        public void Unload()
        {
            patches_by_version.Clear();
        }

        public int Save(BinaryWriter bw)
        {
            bw.Write(patches_by_version.Count);
            for(int i = 0; i < patches_by_version.Count; i++)
            {
                patches_by_version[i].Save(bw);
            }
            return 0;
        }

        public void ApplyPatches(string version)
        {

        }
    }
}
