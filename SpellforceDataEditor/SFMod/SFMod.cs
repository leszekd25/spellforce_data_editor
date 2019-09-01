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
    public enum ModLoadOption { NONE = 0, INFO = 1, DATA = 2, ASSETS = 4, ALL = 7}


    public class SFMod
    {
        public SFModInfo info = new SFModInfo();
        public SFModCFFChanges data = new SFModCFFChanges();
        public SFModAssetManager assets = new SFModAssetManager();

        public void SkipChunk(BinaryReader br)
        {
            long shift = br.ReadInt64();
            br.BaseStream.Position += shift-8;
        }

        // loads all mod data from a file (excluding actual assets)
        // assumes file with the filename exists
        public int Load(string fname, ModLoadOption options)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMod, "SFMod.Load() called (filename: " + fname + ", options: "+options.ToString()+")");
            int load_status = 0;

            FileStream fs = File.Open(fname, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            try
            {
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
            }
            catch(Exception)
            {
                load_status = -1;
            }
            finally
            {
                br.Close();
            }
            
            return load_status;
        }

        // unloads all mod data
        public void Unload()
        {
            info.Unload();
            data.Unload();
            assets.Unload();
        }

        // saves mod data, including assets, to a specified file
        public int Save(string fname)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMod, "SFMod.Save() called (filename: "+fname+")");
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(special_forms.ModManagerForm.ModToolVersion);
            int result = 0;
            result += info.Save(bw);
            result += data.Save(bw);
            result += assets.Save(bw);

            FileStream fs = File.Open(fname + ".tmp", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter fbw = new BinaryWriter(fs);

            ms.Position = 0;
            BinaryReader mbr = new BinaryReader(ms);
            fbw.Write(mbr.ReadBytes((int)mbr.BaseStream.Length));
            
            fbw.Flush();
            fbw.Close();
            mbr.Close();
            if(result == 0)
                File.Copy(fname + ".tmp", fname);
            File.Delete(fname + ".tmp");
            return result;
        }

        // applies mod to the game directory
        public int Apply(SFCFF.SFGameData gd)
        {
            int status = 0;
            status += data.Apply(gd);
            status += assets.Apply();
            return status;
        }
    }
}
