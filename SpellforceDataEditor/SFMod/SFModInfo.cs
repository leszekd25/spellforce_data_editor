/*
 * SFModInfo represents metadata, information block of mod data
 * it plays no role in modification of game
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace SpellforceDataEditor.SFMod
{
    public struct SFModInfo
    {
        public string Name;
        public string Author;
        public int Revision;
        public string Description;
        //up to here is mod version 1

        // load game info from a stream
        public void Load(BinaryReader br)
        {
            br.ReadInt64();
            Name = br.ReadString();
            Author = br.ReadString();
            Revision = br.ReadInt32();
            Description = br.ReadString();
            return;
        }

        // save game info to a stream
        public int Save(BinaryWriter bw)
        {
            long init_pos = bw.BaseStream.Position;
            bw.Write((long)0);

            bw.Write(Name);
            bw.Write(Author);
            bw.Write(Revision);
            bw.Write(Description);

            long new_pos = bw.BaseStream.Position;
            bw.BaseStream.Position = init_pos;
            bw.Write(new_pos - init_pos);
            bw.BaseStream.Position = new_pos;
            return 0;
        }

        public void Unload()
        {

        }

        public override string ToString()
        {
            return Name + "\r\n\r\nAuthor: " + Author + "\r\nRevision " + Revision.ToString() + "\r\n\r\n" + Description;
        }
    }
}
