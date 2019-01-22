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
    public static class SFUnPak
    {
        static public string game_directory_name = "";
        static List<string> paks= new List<string>();
        static SFPakMap pak_map = new SFPakMap();

        static public int PakMap_SaveData(string fname)
        {
            return pak_map.SaveData(fname);
        }

        static public int PakMap_LoadData(string fname)
        {
            return pak_map.LoadData(fname);
        }

        // generates pak data given game directory to retrieve pak files from
        // returns if succeeded
        static public int SpecifyGameDirectory(string dname)
        {
            if (!Directory.Exists(dname))
                return -1;
            if (!Directory.Exists(dname + "\\pak"))
                return -2;

            if (game_directory_name == dname)
                return 0;

            game_directory_name = dname;

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

        // extract file from pak to a given path
        static public int ExtractFileFrom(string pak_name, string filename, string new_name)
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

        // loads file from pak to memory given its name
        // returns stream of bytes which constitute for that file
        static public MemoryStream LoadFileFrom(string pak_name, string filename)
        {
            SFPakFileSystem fs = pak_map.GetPak(pak_name);

            if (fs == null)
                return null;

            return fs.GetFileBuffer(filename);
        }

        // searches for a file in paks and extracts it if founs
        // returns if succeeded
        static public int ExtractFileFind(string filename, string new_name)
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
        
        // searches for a file in paks and loads it to memory
        // returns stream of bytes which constitute for that file
        static public MemoryStream LoadFileFind(string filename)
        {
            MemoryStream ms = null;
            string real_path = game_directory_name + "\\" + filename;
            if(File.Exists(real_path))
            {
                FileStream fs = new FileStream(real_path, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                Byte[] data = br.ReadBytes((int)br.BaseStream.Length);
                ms = new MemoryStream(data);
                br.Close();
                return ms;
            }
            else
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

        static public List<String> ListAllWithExtension(string path, string extname, string[] pak_filter)
        {
            List<String> result = pak_map.ListAllWithExtension(path, extname, pak_filter);
            result.Sort();
            return result;
        }

        static public List<String> ListAllWithFilename(string path, string substr, string[] pak_filter)
        {
            List<String> result = pak_map.ListAllWithFilename(path, substr, pak_filter);
            result.Sort();
            return result;
        }
    }
}
