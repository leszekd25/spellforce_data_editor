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
        static public string game_directory_name { get; private set; } = "";
        static public bool game_directory_specified { get; private set; } = false;
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
            LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory() called, directory: "+dname);
            game_directory_specified = false;
            if (!Directory.Exists(dname))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Directory "+dname+" does not exist!");
                return -1;
            }
            if (!Directory.Exists(dname + "\\pak"))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Directory " + dname + "\\pak" + " does not exist!");
                return -2;
            }

            if (game_directory_name == dname)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Directory is up-to-date");
                game_directory_specified = true;
                return 0;
            }

            if(game_directory_name != "")
                LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Directory changed, reloading data");
            else
                LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Directory specified, loading data");


            game_directory_name = dname;

            pak_map.Clear();
            paks.Clear();

            string[] files = Directory.GetFiles(dname+"\\pak", "*.pak");
            foreach(string file in files)
            {
                //pak_map.AddPak(file);
                paks.Add(Path.GetFileName(file));
            }
            // organize paks in descending order
            List<string> ordered_paks = new List<string>();
            int max_pak = -1;
            int next_pak_old_index = -1;
            while(paks.Count != 0)
            {
                max_pak = -1;
                for(int i = 0; i < paks.Count; i++)
                {
                    string _s = new string(paks[i].Intersect("0123456789").ToArray());
                    int cur_pak_num = Int32.Parse(_s);
                    if(cur_pak_num > max_pak)
                    {
                        max_pak = cur_pak_num;
                        next_pak_old_index = i;
                    }
                }
                if(max_pak!=-1)
                {
                    ordered_paks.Add(paks[next_pak_old_index]);
                    paks.RemoveAt(next_pak_old_index);
                }
            }
            paks = ordered_paks;

            if(pak_map.LoadData("pakdata.dat") == 0)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Pak map loaded");
                game_directory_specified = true;
                return 0;
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Failed to load pak map, generating new one");

                foreach (string file in files)
                {
                    pak_map.AddPak(file);
                }
                pak_map.SaveData("pakdata.dat");
            }
            game_directory_specified = true;
            return 0;
        }

        // extract file from pak to a given path
        static public int ExtractFileFrom(string pak_name, string filename, string new_name)
        {
            MemoryStream ms = LoadFileFrom(pak_name, filename);
            if (ms == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.ExtractFileFrom(): Could not load file! pak_name: " + pak_name + ", filename: " + filename);

                return -1;
            }

            string dir = Path.GetDirectoryName(new_name);
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.ExtractFileFrom(): Could not create directory " + dir + " to store extracted data!");
                    return -3;
                }
            }

            FileStream new_file;
            try
            {
                new_file = new FileStream(new_name, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.ExtractFileFrom(): Could not create file " + new_name + " to store extracted data!");
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
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.LoadFileFrom(): Could not find pak file "+pak_name);

                return null;
            }

            return fs.GetFileBuffer(filename);
        }

        // searches for a file in paks and extracts it if founs
        // returns if succeeded
        static public int ExtractFileFind(string filename, string new_name)
        {
            MemoryStream ms = LoadFileFind(filename);
            if (ms == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.ExtractFileFind(): Could not find file "+filename);
                return -1;
            }

            string dir = Path.GetDirectoryName(new_name);
            if (dir != "")
            {
                if (!Directory.Exists(dir))
                {
                    try
                    {
                        Directory.CreateDirectory(dir);
                    }
                    catch (Exception)
                    {
                        LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.ExtractFileFind(): Could not create directory " + dir + " to store extracted data!");
                        return -3;
                    }
                }
            }

            FileStream new_file;
            try
            {
                new_file = new FileStream(new_name, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.ExtractFileFind(): Could not create file " + new_name + " to store extracted data!");
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

        static public void CloseAllPaks()
        {
            pak_map.CloseAllPaks();
        }
    }
}
