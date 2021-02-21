/*
 * SFGameData is a structure which holds all data from a .cff file
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.IO;
using SpellforceDataEditor.SFChunk;

namespace SpellforceDataEditor.SFCFF
{
    public class SFGameData
    {
        private SFCategory[] categories;      //array of categories
        public const int categoryNumber = 49;

        public Byte[] mainHeader;            //gamedata.cff has a main header which is held here
        public string gamedata_md5 = "";     //currently loaded cff's MD5 hash (as string)

        public string fname = "";            // points nowhere if gamedata is not loaded, points to last know gamedata file which at some point of time had same content as this gamedata

        public SFGameData()
        {
            categories = new SFCategory[categoryNumber];
            for (int i = 1; i <= categoryNumber; i++)
            {
                categories[i - 1] = Assembly.GetExecutingAssembly().CreateInstance("SpellforceDataEditor.SFCFF.SFCategory" + i.ToString()) as SFCategory;
                //categories[i - 1].set_manager(this);
            }

            mainHeader = new Byte[20];
        }

        public SFCategory this[int index]
        {
            get
            {
                return categories[index];
            }
        }

        public int Load(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameData.Load() called");

            fname = "";
            SFChunkFile sfcf = new SFChunkFile();
            int result = sfcf.OpenFile(filename);
            if(result != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameData.Load() failed!");
                return result;
            }

            for (int i = 0; i < categoryNumber; i++)
            {
                int cat_status = categories[i].Read(sfcf);
                if (cat_status != 0)
                    return cat_status;
            }
            sfcf.Close();
            fname = filename;

            return result;
        }

        public int Save(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameData.Save() called");

            SFChunkFile sfcf = new SFChunkFile();
            int result = sfcf.CreateFile(filename, SFChunkFileType.GAMEDATA);
            if (result != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameData.Save() failed!");
                return result;
            }

            for (int i = 0; i < categoryNumber; i++)
            {
                int cat_status = categories[i].Write(sfcf);
                if (cat_status != 0)
                    return cat_status;
            }
            sfcf.Close();
            fname = filename;

            return 0;
        }


        // unloads all stored data
        // returns if succeeded
        public int Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameData.Unload() called");
            foreach (SFCategory cat in categories)
                cat.Unload();

            mainHeader = new Byte[20];
            for (int i = 0; i < 20; i++)
                mainHeader[i] = 0;

            fname = "";
            return 0;
        }
    }
}
