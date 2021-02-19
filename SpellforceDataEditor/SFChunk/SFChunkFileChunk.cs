// Files of type ChunkFile store its data in chunks, which are described using SFChunkFileChunk class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace SpellforceDataEditor.SFChunk
{
    public struct SFChunkFileChunkHeader
    {
        public  short ChunkID;
        public short ChunkOccurence;
        public short ChunkIsPacked;
        public  int ChunkDataLength;
        public  short ChunkDataType;
    }

    public class SFChunkFileChunk
    {
        public SFChunkFileChunkHeader header { get; private set; }
        public int unpacked_data_length { get; private set; }
        byte[] data;
        MemoryStream datastream = null;
        BinaryReader databr = null;

        public int get_original_data_length()
        {
            if (header.ChunkIsPacked == 0)
                return header.ChunkDataLength;
            else
                return unpacked_data_length;
        }

        public byte[] get_raw_data()
        {
            return data;
        }

        public BinaryReader Open()
        {
            if (datastream != null)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFileChunk.Open(): Chunk already open");
                databr.BaseStream.Position = 0;
                return databr;
            }
            datastream = new MemoryStream(data);
            databr = new BinaryReader(datastream, Encoding.GetEncoding(1252));
            return databr;
        }

        public void Close()
        {
            if (datastream == null)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFChunkFile, "SFChunkFileChunk.Close(): Chunk already closed");
                return;
            }
            databr.Close();
            datastream.Close();
            databr = null;
            datastream = null;
        }

        public static SFChunkFileChunkHeader ReadChunkHeader(BinaryReader br, bool proceed = true)
        {
            SFChunkFileChunkHeader h = new SFChunkFileChunkHeader();
            h.ChunkID = br.ReadInt16();
            h.ChunkOccurence = br.ReadInt16();
            h.ChunkIsPacked = br.ReadInt16();
            h.ChunkDataLength = br.ReadInt32();
            h.ChunkDataType = br.ReadInt16();
            if(!proceed)
                br.BaseStream.Position -= 12;
            return h;
        }

        public int Read(BinaryReader br)
        {
            header = ReadChunkHeader(br, true);
            if (header.ChunkIsPacked == 0)
            {
                try
                {
                    data = br.ReadBytes(header.ChunkDataLength);
                }
                catch (EndOfStreamException)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFileChunk.Read(): Error reading chunk data");
                    return -4;
                }
                return 0;
            }
            else
            {
                unpacked_data_length = br.ReadInt32();
                br.ReadBytes(2);

                byte[] tmp_data;
                try
                {
                    tmp_data = br.ReadBytes(header.ChunkDataLength - 2);
                }
                catch (EndOfStreamException)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFileChunk.Read(): Error reading chunk data");
                    return -4;
                }

                try
                {
                    using (MemoryStream ms_dest = new MemoryStream())
                    {
                        using (MemoryStream ms_src = new MemoryStream(tmp_data))
                        {
                            using (DeflateStream ds = new DeflateStream(ms_src, CompressionMode.Decompress))
                            {
                                ds.CopyTo(ms_dest);
                            }
                        }
                        data = ms_dest.ToArray();
                    }
                }
                catch (Exception)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFileChunk.Read(): Error decompressing chunk data!");
                    return -2;
                }

                if (data.Length != unpacked_data_length)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFChunkFile, "SFChunkFileChunk.Read(): Decompressed data length does not match expected length! Expected: "+unpacked_data_length.ToString()+", got: "+data.Length.ToString());
                    return -3;
                }
                return 0;
            }
        }
    }
}
