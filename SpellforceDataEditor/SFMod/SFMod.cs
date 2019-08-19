/*
 * SFMod is a structure which holds all mod data and loads/saves this data to a file
 * ModManagerForm uses this data to modify the game as the data says, using the supplied Apply method
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace SpellforceDataEditor.SFMod
{
    [Flags]
    public enum ModLoadOption { NONE = 0, INFO = 1, DATA = 2, ASSETS = 4, PATCHES = 8, ALL = 15}


    public class SFMod
    {
        public SFModInfo info = new SFModInfo();
        public SFModCFFChanges data = new SFModCFFChanges();
        public SFModAssetManager assets = new SFModAssetManager();
        public SFModBytePatchManager patches = new SFModBytePatchManager();

        public void SkipChunk(BinaryReader br)
        {
            long shift = br.ReadInt64();
            br.BaseStream.Position += shift-8;
        }

        // loads all mod data from a file (excluding actual assets)
        public int Load(string fname, ModLoadOption options)
        {
            FileStream fs = File.Open(fname, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            int mod_toolversion = br.ReadInt32();

            if ((options & ModLoadOption.INFO) != 0)
                info.Load(br);
            else
                SkipChunk(br);

            if ((options & ModLoadOption.DATA) != 0)
                data.Load(br);
            else
                SkipChunk(br);

            if ((options & ModLoadOption.ASSETS) != 0)
                assets.Load(br, fname);
            else
                SkipChunk(br);

            /*if ((options & ModLoadOption.PATCHES) != 0)
                patches.Load(br);
            else
                SkipChunk(br);*/

            br.Close();
            return 0;
        }

        // unloads all mod data
        public void Unload()
        {
            info.Unload();
            data.Unload();
            assets.Unload();
            //patches.Unload();
        }

        // saves mod data, including assets, to a specified file
        public int Save(string fname)
        {

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(special_forms.ModManagerForm.ModToolVersion);
            int result = 0;
            if(info.Save(bw) != 0)
                result = -1;
            else if(data.Save(bw) != 0)
                result = -1;
            else if(assets.Save(bw) != 0)
                result = -1;
            // else if(patches.Save(bw) != 0)    // not for this version!
            //     result = -1;

            FileStream fs = File.Open(fname + ".tmp", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter fbw = new BinaryWriter(fs);

            ms.Position = 0;
            BinaryReader mbr = new BinaryReader(ms);
            fbw.Write(mbr.ReadBytes((int)mbr.BaseStream.Length));
            
            fbw.Flush();
            fbw.Close();
            mbr.Close();
            if(result == 0)
            {
                File.Copy(fname + ".tmp", fname);
            }
            File.Delete(fname + ".tmp");
            return result;
        }

        // applies mod to the game directory
        public int Apply(SFCFF.SFGameData gd)
        {
            data.Apply(gd);
            assets.Apply();
            return 0;
        }
    }
}
