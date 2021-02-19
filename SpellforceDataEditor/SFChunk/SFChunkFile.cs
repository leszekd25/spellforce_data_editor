// ChunkFile format is used by Spellforce for storing map data, save data and game data
// While game data is already handled in SFCFF, all of those types can be handled by SFChunkFile class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace SpellforceDataEditor.SFChunk
{
    public enum SFChunkFileType { GAMEDATA, MAP, SAVE }

    public struct SFChunkLookupKey
    {
        public short ChunkID;
        public short ChunkOccurence;

        public SFChunkLookupKey(short id, short occ)
        {
            ChunkID = id;
            ChunkOccurence = occ;
        }

        public static bool operator ==(SFChunkLookupKey k1, SFChunkLookupKey k2)
        {
            return (k1.ChunkID == k2.ChunkID) && (k1.ChunkOccurence == k2.ChunkOccurence);
        }

        public static bool operator !=(SFChunkLookupKey k1, SFChunkLookupKey k2)
        {
            return (k1.ChunkID != k2.ChunkID) || (k1.ChunkOccurence != k2.ChunkOccurence);
        }

        public override bool Equals(object obj)
        {
            return (obj.GetType() == typeof(SFChunkLookupKey)) && ((SFChunkLookupKey)obj == this);
        }

        public override int GetHashCode()
        {
            return ((ChunkID << 16) + ChunkOccurence).GetHashCode();
        }
    }

    public enum SFChunkFileSource { NONE, FILESYSTEM, MEMORY }

    public class SFChunkFile
    {
        SFChunkFileSource source = SFChunkFileSource.NONE;
        Stream s = null;
        BinaryReader br = null;
        BinaryWriter bw = null;
        byte[] header;   // for map: -579674862, 3, 1, 0, 0; for gamedata: -579674862, 2, 2, 1, 0
        Dictionary<SFChunkLookupKey, long> lookup_dict = null;
        int total_size = 0;

        public int get_data_type()
        {
            return BitConverter.ToInt32(header, 4);
        }

        public bool is_valid_header()
        {
            return (BitConverter.ToInt32(header, 0) == -579674862);
        }

        public int OpenRaw(byte[] data)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.OpenRaw() called");
            if (s != null)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.OpenRaw(): Stream already open");
                br.BaseStream.Position = 0;
                return 0;
            }

            if ((data == null)||(data.Length < 20))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFChunkFile, "SFChunkFile.OpenRaw(): No data provided or it is too short");
                return -1;
            }
            s = new MemoryStream(data);

            header = new byte[20];
            s.Read(header, 0, 20);
            if (!is_valid_header())
            {
                s.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.OpenRaw(): Header is not valid!");
                return -3;
            }
            source = SFChunkFileSource.MEMORY;
            br = new BinaryReader(s, Encoding.GetEncoding(1252));

            GenerateLookupDict();

            return 0;
        }

        // make sure you have enough bytes for that
        public int CreateRaw(ref byte[] data, SFChunkFileType type)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.CreateFile() called");
            if(data == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.CreateRaw(): Error creating a stream");
                return -2;
            }
            s = new MemoryStream(data);

            header = new byte[20];
            bw = new BinaryWriter(new MemoryStream(header), Encoding.GetEncoding(1252));
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

            bw = new BinaryWriter(s);
            bw.Write(header);
            source = SFChunkFileSource.MEMORY;
            total_size = 20;

            return 0;
        }


        public int OpenFile(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.OpenFile() called");
            if(s != null)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.OpenFile(): Stream already open");
                br.BaseStream.Position = 0;
                return 0;
            }

            if (!File.Exists(filename))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFChunkFile, "SFChunkFile.OpenFile(): File does not exist (filename: "+filename+")");
                return -1;
            }
            try
            {
                s = new FileStream(filename, FileMode.Open);
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.OpenFile(): Error opening file (filename: "+filename+")");
                return -2;
            }

            header = new byte[20];
            s.Read(header, 0, 20);
            if(!is_valid_header())
            {
                s.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.OpenFile(): Header is not valid! (filename: "+filename+")");
                return -3;
            }
            source = SFChunkFileSource.FILESYSTEM;
            br = new BinaryReader(s, Encoding.GetEncoding(1252));

            GenerateLookupDict();

            return 0;
        }

        public int CreateFile(string filename, SFChunkFileType type)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.CreateFile() called");
            try
            {
                s = new FileStream(filename, FileMode.Create, FileAccess.Write);
            }
            catch(Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.CreateFile(): Error opening or creating a file (filename: "+filename+")");
                return -2;
            }
            header = new byte[20];
            bw = new BinaryWriter(new MemoryStream(header), Encoding.GetEncoding(1252));
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

            bw = new BinaryWriter(s);
            bw.Write(header);
            source = SFChunkFileSource.FILESYSTEM;
            total_size = 20;

            return 0;
        }

        // return total bytes written/read
        public int Close()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.Close() called");
            if (s == null)
                return 0;

            if (br != null)
                br.Close();
            if (bw != null)
                bw.Close();
            s = null;
            br = null;
            bw = null;
            source = SFChunkFileSource.NONE;

            int ret = total_size;
            total_size = 0;
            return ret;
        }

        public void GenerateLookupDict()
        {
            if (s == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GenerateLookupDict(): Stream is not open!");
                throw new InvalidOperationException("SFChunkFile.GenerateLookupDict(): Stream not open!");
            }
            if (br == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GenerateLookupDict(): Stream is not open for reading!");
                throw new InvalidOperationException("SFChunkFile.GenerateLookupDict(): Stream not open for reading!");
            }

            lookup_dict = new Dictionary<SFChunkLookupKey, long>();
            br.BaseStream.Position = 20;
            int cm = get_data_type();

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                long offset = br.BaseStream.Position;
                SFChunkFileChunkHeader header = SFChunkFileChunk.ReadChunkHeader(br, false);
                if ((header.ChunkDataLength < 0)|| (br.BaseStream.Position + (cm == 3 ? 16 : 12) + header.ChunkDataLength > br.BaseStream.Length))
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GenerateLookupDict(): Malformed chunk found, stopping here");
                    break;
                }
                lookup_dict.Add(new SFChunkLookupKey(header.ChunkID, header.ChunkOccurence), offset);

                br.BaseStream.Position += (cm==3?16:12)+header.ChunkDataLength;
            }

            total_size = (int)br.BaseStream.Position;
        }

        public SFChunkFileChunk GetChunkByID(short id, short occ_id = 0)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GetChunkByID() called (id = "+id.ToString()+", occurence id = "+occ_id.ToString()+")");
            if (s == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GetChunkByID(): Stream is not open!");
                throw new InvalidOperationException("SFChunkFile.GetChunkByID(): Stream not open!");
            }
            if (br == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GetChunkByID(): Stream is not open for reading!");
                throw new InvalidOperationException("SFChunkFile.GetChunkByID(): Stream not open for reading!");
            }
            SFChunkLookupKey key = new SFChunkLookupKey(id, occ_id);
            if (!lookup_dict.ContainsKey(key))
                return null;
            
            br.BaseStream.Position = lookup_dict[key];

            SFChunkFileChunk chunk = new SFChunkFileChunk();
            if (chunk.Read(br) != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GetChunkByID(): Chunk data is not valid!");
                throw new InvalidDataException("SFChunkFileChunk.GetChunkById(): Invalid chunk data!");
            }
            LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GetChunkByID(): Found chunk, chunk type: "
                + chunk.header.ChunkDataType.ToString() + ", data length: " + chunk.get_original_data_length().ToString());
            return chunk;
        }

        public List<SFChunkFileChunk> GetAllChunks()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GetAllChunks() called");
            if (s == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GetAllChunks(): Stream is not open!");
                throw new InvalidOperationException("SFChunkFile.GetAllChunks(): Stream not open!");
            }
            if (br == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GetAllChunks(): Stream is not open for reading!");
                throw new InvalidOperationException("SFChunkFile.GetAllChunks(): Stream not open for reading!");
            }

            List<SFChunkFileChunk> ret = new List<SFChunkFileChunk>();
            foreach(var kv in lookup_dict)
            {
                br.BaseStream.Position = lookup_dict[kv.Key];

                SFChunkFileChunk chunk = new SFChunkFileChunk();
                if (chunk.Read(br) != 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GetAllChunks(): Chunk data is not valid!");
                    throw new InvalidDataException("SFChunkFileChunk.GetAllChunks(): Invalid chunk data!");
                }
                LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.GetAllChunks(): Found chunk, chunk type: "
                    + chunk.header.ChunkDataType.ToString() + ", data length: " + chunk.get_original_data_length().ToString());

                ret.Add(chunk);
            }

            return ret;
        }


        public void AddChunk(short chunk_id, short occ_id, bool is_compressed, short data_type, byte[] raw_data)
        { 
            LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFile.AddChunk() called (chunk id = "
                +chunk_id.ToString()+", occurence id = "
                +occ_id.ToString()+", compressed = "
                +(is_compressed?"true":"false")+", data type = "
                +data_type.ToString()+", data length = "
                +raw_data.Length.ToString()+")");
            if (s == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.AddChunk(): Stream is not open!");
                throw new InvalidOperationException("SFChunkFile.AddChunk(): Stream not open!");
            }
            if (bw == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFile.AddChunk(): Stream is not open for writing!");
                throw new InvalidOperationException("SFChunkFile.AddChunk(): Stream not open for writing!");
            }
            if(raw_data.Length == 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFChunkFile, "SFChunkFile.AddChunk(): Provided data is empty! Omitting chunk...");
                return;
            }

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

            total_size = (int)bw.BaseStream.Position;
        }
    }
}
