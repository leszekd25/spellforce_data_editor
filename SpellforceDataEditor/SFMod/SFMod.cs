using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace SpellforceDataEditor.SFMod
{
    public class SFMod
    {
        public SFModInfo info = new SFModInfo();
        public SFModCFFChanges data = new SFModCFFChanges();
        public SFModAssetManager assets = new SFModAssetManager();
        public SFModBytePatchManager patches = new SFModBytePatchManager();

        public int LoadOnlyInfo(string fname)
        {
            FileStream fs = File.Open(fname, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            int mod_toolversion = br.ReadInt32();
            info.Load(br);
            br.Close();
            return 0;
        }

        public int Load(string fname)
        {
            FileStream fs = File.Open(fname, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            int mod_toolversion = br.ReadInt32();
            info.Load(br);
            data.Load(br);
            assets.Load(br, fname);
            //patches.Load(br);
            br.Close();
            return 0;
        }

        public void Unload()
        {
            info.Unload();
            data.Unload();
            assets.Unload();
            //patches.Unload();
        }
        // save to tmp first pls
        public int Save(string fname)
        {
            FileStream fs = File.Open(fname+".tmp", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
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
            bw.Flush();
            bw.Close();
            if(result == 0)
            {
                File.Copy(fname + ".tmp", fname);
            }
            File.Delete(fname + ".tmp");
            return result;
        }

        public int Apply(SFGameData gd)
        {
            data.Apply(gd);
            assets.Apply();
            return 0;
        }
    }
}
