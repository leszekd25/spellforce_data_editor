using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace SpellforceDataEditor.SFChunkFile
{
    public enum SFChunkFileType { GAMEDATA, MAP }

    public class SFChunkFile
    {
        FileStream fs = null;
        BinaryReader br = null;
        BinaryWriter bw = null;
        byte[] header;   // for map: -579674862, 3, 1, 0, 0; for gamedata: -579674862, 2, 2, 1, 0

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
            catch (Exception)
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

        public int Create(string filename, SFChunkFileType type)
        {
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            }
            catch(Exception)
            {
                return -2;
            }
            header = new byte[20];
            bw = new BinaryWriter(new MemoryStream(header));
            bw.Write(-579674862);
            if (type == SFChunkFileType.MAP)
            {
                bw.Write(3);
                bw.Write(1);
                bw.Write(0);
            }
            else
            {
                bw.Write(2);
                bw.Write(2);
                bw.Write(1);
            }
            bw.Write(0);
            bw.Close();

            bw = new BinaryWriter(fs);
            bw.Write(header);

            return 0;
        }

        public void Close()
        {
            if (fs == null)
                return;
            if (br != null)
                br.Close();
            if (bw != null)
                bw.Close();
            fs = null;
            br = null;
            bw = null;
        }

        public List<int> GetChunkOffsets()
        {
            if (fs == null)
                throw new InvalidOperationException("SFChunkFile.GetChunkOffsets(): File not open!");
            if (br == null)
                throw new InvalidOperationException("SFChunkFile.GetChunkOffsets(): File not open for reading!");

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
            if (br == null)
                throw new InvalidOperationException("SFChunkFile.GetChunkByID(): File not open for reading!");

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
                    if(chunk.Read(br) != 0)
                    {
                        throw new InvalidDataException("SFChunkFileChunk.GetChunkById(): Invalid chunk data!");
                    }
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
            if (br == null)
                throw new InvalidOperationException("SFChunkFile.GetChunkByOffset(): File not open for reading!");

            SFChunkFileChunk chunk = new SFChunkFileChunk();
            br.BaseStream.Position = offset;

            if (chunk.Read(br) != 0)
            {
                throw new InvalidDataException("SFChunkFileChunk.GetChunkByOffset(): Invalid chunk data!");
            }

            return chunk;
        }

        public void AddChunk(short chunk_id, short occ_id, bool is_compressed, short data_type, byte[] raw_data)
        {
            if (fs == null)
                throw new InvalidOperationException("SFChunkFile.AddChunk(): File not open!");
            if (bw == null)
                throw new InvalidOperationException("SFChunkFile.AddChunk(): File not open for writing!");

            SFChunkFileChunk chunk = new SFChunkFileChunk();

            byte[] chunk_header;
            if (is_compressed)
                chunk_header = new byte[16];
            else
                chunk_header = new byte[12];

            byte[] compressed_data = null;
            if(is_compressed)
            {
                using (MemoryStream ms_dest = new MemoryStream())
                {
                    using (MemoryStream ms_src = new MemoryStream(raw_data))
                    {
                        using (DeflateStream ds = new DeflateStream(ms_dest, CompressionMode.Compress))
                        {
                            ms_src.CopyTo(ds);
                        }
                    }
                    compressed_data = ms_dest.ToArray();
                }
            }

            BinaryWriter header_bw = new BinaryWriter(new MemoryStream(chunk_header));
            header_bw.Write(chunk_id);
            header_bw.Write(occ_id);
            header_bw.Write((short)(is_compressed ? 1 : 0));
            if (is_compressed)
                header_bw.Write(compressed_data.Length+6);    // to do?
            else
                header_bw.Write(raw_data.Length);
            header_bw.Write(data_type);
            if (is_compressed)
                header_bw.Write(raw_data.Length);
            header_bw.Close();

            bw.Write(chunk_header);
            if (is_compressed)
            {
                bw.Write((byte)120);
                bw.Write((byte)156);
                bw.Write(compressed_data);    // to do?
                byte[] checksum_inverted = BitConverter.GetBytes(Utility.CalculateAdler32Checksum(raw_data)).Reverse().ToArray();
                bw.Write(checksum_inverted);
            }
            else
                bw.Write(raw_data);
            System.Diagnostics.Debug.WriteLine("CURRENT POSITION: " + bw.BaseStream.Position);
        }
    }
}
