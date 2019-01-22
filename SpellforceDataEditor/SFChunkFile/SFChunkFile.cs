using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpellforceDataEditor.SFChunkFile
{
    public class SFChunkFile
    {
        FileStream fs = null;
        BinaryReader br = null;
        byte[] header;

        public int get_data_type()
        {
            return BitConverter.ToInt32(header, 4);
        }

        public bool is_valid_header()
        {
            return (BitConverter.ToInt32(header, 0) == -579674862);
        }

        public int Open(string filename)
        {
            if(fs != null)
            {
                br.BaseStream.Position = 0;
                return 0;
            }

            if (!File.Exists(filename))
                return -1;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
            }
            catch (Exception e)
            {
                return -2;
            }

            header = new byte[20];
            fs.Read(header, 0, 20);
            if(!is_valid_header())
            {
                fs.Close();
                return -3;
            }
            br = new BinaryReader(fs);

            return 0;
        }

        public void Close()
        {
            if (fs == null)
                return;
            br.Close();
            fs = null;
            br = null;
        }

        public List<int> GetChunkOffsets()
        {
            if (fs == null)
                throw new InvalidOperationException("SFCuhnkFile.GetChunkOffsets(): File not open!");

            List<int> ret = new List<int>();
            br.BaseStream.Position = 20;
            int cm = get_data_type();

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                ret.Add((int)br.BaseStream.Position);
                SFChunkFileChunk chunk = new SFChunkFileChunk();
                chunk.ReadHeader(br, cm, true);
                br.BaseStream.Position += chunk.get_data_length();
            }

            return ret;
        }

        public SFChunkFileChunk GetChunkByID(short id, short occ_id = 0)
        {
            if (fs == null)
                throw new InvalidOperationException("SFChunkFile.GetChunkByID(): File not open!");

            bool found = false;
            SFChunkFileChunk chunk = null;

            br.BaseStream.Position = 20;
            int cm = get_data_type();

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                chunk = new SFChunkFileChunk();
                chunk.ReadHeader(br, cm, false);
                if ((chunk.get_id() == id) && (chunk.get_occurence() == occ_id))
                {
                    chunk.Read(br, cm);
                    found = true;
                    break;
                }
                br.BaseStream.Position += (cm == 2?12:16) + chunk.get_data_length();
            }
            
            return (found ? chunk : null);
        }

        public SFChunkFileChunk GetChunkByOffset(int offset)
        {
            if (fs == null)
                throw new InvalidOperationException("SFChunkFile.GetChunkByOffset(): File not open!");

            SFChunkFileChunk chunk = new SFChunkFileChunk();
            br.BaseStream.Position = offset;
            int cm = get_data_type();
                
            chunk.Read(br, cm);

            return chunk;
        }
    }
}
