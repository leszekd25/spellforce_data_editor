/*
 * SFModAssetElement is a description of a single asset
 * SFModAssetManager is a container for those descriptions
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace SpellforceDataEditor.SFMod
{
    public delegate void SFModAssetUpdate(string fname);

    public class SFModAssetElement
    {
        public string fname;
        public string dname;
        public long size;

        // loads description from stream
        // returns file size in bytes
        public long Load(BinaryReader br)
        {
            fname = br.ReadString();
            dname = br.ReadString();
            size = br.ReadInt64();
            return size;
        }

        // saves description to a stream
        public int Save(BinaryWriter bw)
        {
            bw.Write(fname);
            bw.Write(dname);
            bw.Write(size);
            return 0;
        }

        // writes a file this asset is describing to a stream
        // asset_dir is the root directory for the assets
        public int AppendFile(string asset_dir, BinaryWriter bw)
        {
            long ChunkMaxSize = 0x1000000; // 16 MB
            long cur_filesize = size;

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

        // reads file from the stream and saves it according to the description
        public int ExtractFile(BinaryReader br)
        {
            long ChunkMaxSize = 0x1000000; // 16 MB
            long cur_filesize = size;
            // open file
            if(!Directory.Exists(SFUnPak.SFUnPak.game_directory_name + "\\" + dname))
                Directory.CreateDirectory(SFUnPak.SFUnPak.game_directory_name + "\\" + dname);
            BinaryWriter abw = new BinaryWriter(File.Open(SFUnPak.SFUnPak.game_directory_name + "\\" + dname + "\\" + fname, FileMode.OpenOrCreate, FileAccess.Write));
            while (cur_filesize > 0)
            {
                int loaded_chunk_size = (int)Math.Min(ChunkMaxSize, cur_filesize);
                cur_filesize -= loaded_chunk_size;

                byte[] chunk = br.ReadBytes(loaded_chunk_size);
                abw.Write(chunk);
            }
            abw.Close();

            return 0;
        }
    }

    public class SFModAssetManager
    {
        public string unpacked_asset_directory;  // only for saving
        public string sfmd_filename;
        public List<SFModAssetElement> assets = new List<SFModAssetElement>();
        public event SFModAssetUpdate update_event = null;

        // loads all asset descriptions from a file
        public void Load(BinaryReader br, string fname)
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
            return;
        }

        // frees memory*
        public void Unload()
        {
            assets.Clear();
            update_event = null;
        }

        // saves all assets, including actual game files, to the mod file
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

        // generates asset descriptions, given root directory for assets
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
                // skip GameData.cff, it's handled separately
                if (asset.fname.ToLower()=="gamedata.cff")
                    continue;
                assets.Add(asset);
            }
            return 0;
        }

        // aplies assets to the game
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
                if (i % 10 == 9)
                    update_event?.Invoke(e.fname);    // same as if(update_event != null) update_event(e.fname)
            }
            return 0;
        }

        // returns a list of asset filenames, not including the root path
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
