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

namespace SpellforceDataEditor.SFCFF
{
    public class SFGameData
    {
        private SFCategory[] categories;      //array of categories
        public const int categoryNumber = 49;

        public Byte[] mainHeader;            //gamedata.cff has a main header which is held here
        public string gamedata_md5 = "";     //currently loaded cff's MD5 hash (as string)

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

        // load gamedata from given file
        // returns whether it succeeded
        public int Load(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameData.Read() called");
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

            //md5 calculation for data diff tool
            //MD5 md5_gen = MD5.Create();
            //gamedata_md5 = BitConverter.ToString(md5_gen.ComputeHash(fs)).Replace("-", "").ToLower();

            fs.Seek(0, SeekOrigin.Begin);

            BinaryReader br = new BinaryReader(fs, Encoding.Default);

            int result = 0;

            mainHeader = br.ReadBytes(mainHeader.Length);
            for (int i = 0; i < categoryNumber; i++)
            {
                int cat_status = categories[i].Read(br);
                if (cat_status == -1)
                {
                    //MessageBox.Show("Category '" + get_category(i).get_name() + "' has corrupted header, but it will fix itself upon the next data save.");
                }
                else if (cat_status == -2)
                {
                    result = -1;
                    break;
                }
                else if (cat_status == -3)
                {
                    result = -2;
                    break;
                }
            }

            //br.Close();
            fs.Close();

            if(result != 0)
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameData.Read() failed!");
            return result;
        }

        // saves ganedata to a given file
        // returns if succeeded
        public int Save(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            fs.SetLength(0);

            BinaryWriter bw = new BinaryWriter(fs, Encoding.Default);

            bw.Write(mainHeader);

            for (int i = 0; i < categoryNumber; i++)
            {
                categories[i].Write(bw);
            }

            //bw.Close();
            fs.Close();

            return 0;

            //md5 calculation for data diff tool
            //FileStream fs2 = new FileStream(filename, FileMode.Open, FileAccess.Read);

            //MD5 md5_gen = MD5.Create();
            //gamedata_md5 = BitConverter.ToString(md5_gen.ComputeHash(fs2)).Replace("-", "").ToLower();

            //fs2.Close();
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

            return 0;
        }
    }
}
