/*
 * SFUnPak is the main PAK manager
 * It can query underlying SFPakMap object for any file, and it can also load files from disk
 *      as long as the directory matches the specified file
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFUnPak
{
    public class SFUnPak
    {
        string directory_name = "";
        List<string> paks= new List<string>();
        SFPakMap pak_map = new SFPakMap();

        public SFUnPak()
        {

        }

        public int PakMap_SaveData(string fname)
        {
            return pak_map.SaveData(fname);
        }

        public int PakMap_LoadData(string fname)
        {
            return pak_map.LoadData(fname);
        }

        public int SpecifyGameDirectory(string dname)
        {
            if (!Directory.Exists(dname))
                return -1;
            if (!Directory.Exists(dname + "\\pak"))
                return -2;

            directory_name = dname;

            pak_map.Clear();
            paks.Clear();

            string[] files = Directory.GetFiles(dname+"\\pak", "*.pak");
            foreach(string file in files)
            {
                //pak_map.AddPak(file);
                paks.Add(Path.GetFileName(file));
            }
            if(pak_map.LoadData("pakdata.dat") == 0)
            {
                return 0;
            }
            else
            {
                foreach (string file in files)
                {
                    pak_map.AddPak(file);
                }
                pak_map.SaveData("pakdata.dat");
            }
            return 0;
        }

        public int ExtractFileFrom(string pak_name, string filename, string new_name)
        {
            MemoryStream ms = LoadFileFrom(pak_name, filename);
            if (ms == null)
                return -1;

            string dir = Path.GetDirectoryName(new_name);
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception e)
                {
                    return -3;
                }
            }

            FileStream new_file;
            try
            {
                new_file = new FileStream(new_name, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch (Exception e)
            {
                return -2;
            }

            new_file.Write(ms.ToArray(), 0, (int)ms.Length);
            ms.Close();
            new_file.Close();

            return 0;
        }

        public MemoryStream LoadFileFrom(string pak_name, string filename)
        {
            SFPakFileSystem fs = pak_map.GetPak(pak_name);

            if (fs == null)
                return null;

            return fs.GetFileBuffer(filename);
        }

        public int ExtractFileFind(string filename, string new_name)
        {
            MemoryStream ms = LoadFileFind(filename);
            if (ms == null)
                return -1;

            string dir = Path.GetDirectoryName(new_name);
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch(Exception e)
                {
                    return -3;
                }
            }

            FileStream new_file;
            try
            {
                new_file = new FileStream(new_name, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch (Exception e)
            {
                return -2;
            }

            new_file.Write(ms.ToArray(), 0, (int)ms.Length);
            ms.Close();
            new_file.Close();

            return 0;
        }

        public MemoryStream LoadFileFind(string filename)
        {
            MemoryStream ms = null;
            string real_path = directory_name + "\\" + filename;
            try
            {
                FileStream fs = new FileStream(real_path, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                Byte[] data = br.ReadBytes((int)br.BaseStream.Length);
                ms = new MemoryStream(data);
                br.Close();
                fs.Close();
                return ms;
            }
            catch(Exception e)
            {
                foreach(string pak in paks)
                {
                    ms = LoadFileFrom(pak, filename);
                    if (ms != null)
                        return ms;
                }
                return null;
            }
        }

        public List<String> ListAllWithExtension(string path, string extname, string[] pak_filter)
        {
            List<String> result = pak_map.ListAllWithExtension(path, extname, pak_filter);
            result.Sort();
            return result;
        }
    }
}
