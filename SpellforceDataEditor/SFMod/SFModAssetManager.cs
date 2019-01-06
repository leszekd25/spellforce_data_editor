using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
// IMPORTANT: LOAD BIG FILES IN 16 MB CHUNKS MAX! it's so the ram usage wont go through the roof

namespace SpellforceDataEditor.SFMod
{
    public class SFModAssetElement
    {
        public string fname;
        public string dname;
        public long size;

        // returns file size in bytes
        public long Load(BinaryReader br)
        {
            fname = br.ReadString();
            dname = br.ReadString();
            size = br.ReadInt64();
            return size;
        }

        public int Save(BinaryWriter bw)
        {
            bw.Write(fname);
            bw.Write(dname);
            bw.Write(size);
            return 0;
        }

        public int AppendFile(string asset_dir, BinaryWriter bw)
        {
            long ChunkMaxSize = 0x1000000; // 16 MB
            // chunks of 16 MB
            long cur_filesize = size;
            // open file
            BinaryReader abr = new BinaryReader(File.Open(asset_dir + "\\" + dname + "\\" + fname, FileMode.Open, FileAccess.Read));
            while(cur_filesize > 0)
            {
                int loaded_chunk_size = (int)Math.Min(ChunkMaxSize, cur_filesize);
                cur_filesize -= loaded_chunk_size;

                byte[] chunk = abr.ReadBytes(loaded_chunk_size);
                bw.Write(chunk);
            }
            abr.Close();

            return 0;
        }

        public int ExtractFile(BinaryReader br)
        {
            long ChunkMaxSize = 0x1000000; // 16 MB
            // chunks of 16 MB
            long cur_filesize = size;
            // open file
            if(!Directory.Exists(SFUnPak.SFUnPak.game_directory_name + "\\" + dname))
                Directory.CreateDirectory(SFUnPak.SFUnPak.game_directory_name + "\\" + dname);
            StreamWriter abw = new StreamWriter(File.Open(SFUnPak.SFUnPak.game_directory_name + "\\" + dname + "\\" + fname, FileMode.OpenOrCreate, FileAccess.Write));
            while (cur_filesize > 0)
            {
                int loaded_chunk_size = (int)Math.Min(ChunkMaxSize, cur_filesize);
                cur_filesize -= loaded_chunk_size;

                byte[] chunk = br.ReadBytes(loaded_chunk_size);
                abw.Write(chunk);
            }
            abw.Flush();
            abw.Close();

            return 0;
        }
    }

    public class SFModAssetManager
    {
        public string unpacked_asset_directory;  // only for saving
        public string sfmd_filename;
        public List<SFModAssetElement> assets = new List<SFModAssetElement>();

        public int Load(BinaryReader br, string fname)
        {
            assets.Clear();
            sfmd_filename = fname;
            br.ReadInt64();
            int asset_count = br.ReadInt32();
            for(int i = 0; i < asset_count; i++)
            {
                SFModAssetElement aelem = new SFModAssetElement();
                long size = aelem.Load(br);
                assets.Add(aelem);

                br.BaseStream.Position += size;
            }
            return 0;
        }

        public void Unload()
        {
            assets.Clear();
        }

        public int Save(BinaryWriter bw)
        {
            long init_pos = bw.BaseStream.Position;
            bw.Write((long)0);

            bw.Write(assets.Count);
            for(int i = 0; i < assets.Count; i++)
            {
                assets[i].Save(bw);
                assets[i].AppendFile(unpacked_asset_directory, bw);
            }

            long new_pos = bw.BaseStream.Position;
            bw.BaseStream.Position = init_pos;
            bw.Write(new_pos - init_pos);
            bw.BaseStream.Position = new_pos;
            return 0;
        }

        public int GenerateAssetInfo()
        {
            // get all files in directory
            // create asset for each file
            string[] filenames = Directory.GetFiles(unpacked_asset_directory, "*", SearchOption.AllDirectories);
            foreach(string s in filenames)
            {
                SFModAssetElement asset = new SFModAssetElement();
                asset.dname = Path.GetDirectoryName(s).Replace(unpacked_asset_directory + "\\", "").Replace(unpacked_asset_directory, "");
                asset.fname = Path.GetFileName(s);
                asset.size = new FileInfo(s).Length;
                assets.Add(asset);
            }
            return 0;
        }

        // it loads assets independently from the manager
        public int Apply()
        {
            if (!File.Exists(sfmd_filename))
                return -1;
            FileStream fs = File.Open(sfmd_filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            br.ReadInt32();
            long cur_pos = 4;
            cur_pos += br.ReadInt64();   // info chunk
            br.BaseStream.Position = cur_pos;
            cur_pos += br.ReadInt64();   // data chunk
            br.BaseStream.Position = cur_pos;
            // assets chunk
            br.ReadInt64();
            int asset_count = br.ReadInt32();
            for (int i = 0; i < asset_count; i++)
            {
                SFModAssetElement e = new SFModAssetElement();
                e.Load(br);
                e.ExtractFile(br);
            }
            return 0;
        }

        public List<string> GetFileNames()
        {
            List<string> fnames = new List<string>();
            foreach(SFModAssetElement e in assets)
            {
                string s = e.dname + "\\" + e.fname;
                fnames.Add(s);
            }
            return fnames;
        }

        public override string ToString()
        {
            return "Asset count: " + assets.Count().ToString();
        }
    }
}
