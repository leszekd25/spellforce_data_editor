/*
 * SFUnPak is the main PAK manager
 * It can query underlying SFPakMap object for any file, and it can also load files from disk
 *      as long as the directory matches the specified file
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SFEngine.SFUnPak
{
    [Flags]
    public enum FileSource
    {
        NONE = 0x0,
        FILESYSTEM = 0x1,
        PAK = 0x2,
        ANY = FILESYSTEM | PAK
    };

    public static class SFUnPak
    {
        static public string game_directory_name { get; private set; } = "";
        static public bool game_directory_specified { get; private set; } = false;
        static public SFPakMap pak_map = new SFPakMap();

        // generates pak data given game directory to retrieve pak files from
        // returns 0 if succeeded
        static public int SpecifyGameDirectory(string dname)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory() called, directory: " + dname);
            game_directory_specified = false;
            if (!Directory.Exists(dname))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Directory " + dname + " does not exist!");
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

            if (game_directory_name != "")
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Directory changed, reloading data");
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Directory specified, loading data");
            }

            game_directory_name = dname;

            pak_map.Clear();
            List<string> paks = new List<string>();

            string[] files = Directory.GetFiles(dname + "\\pak", "*.pak");
            foreach (string file in files)
            {
                paks.Add(Path.GetFileName(file));
            }
            // organize paks in descending order
            List<string> ordered_paks = new List<string>();
            int max_pak = -1;
            int next_pak_old_index = -1;
            while (paks.Count != 0)
            {
                max_pak = -1;
                for (int i = 0; i < paks.Count; i++)
                {
                    string _s = new string(paks[i].Intersect("0123456789").ToArray());
                    int cur_pak_num = Int32.Parse(_s);
                    if (cur_pak_num > max_pak)
                    {
                        max_pak = cur_pak_num;
                        next_pak_old_index = i;
                    }
                }
                if (max_pak != -1)
                {
                    ordered_paks.Add(paks[next_pak_old_index]);
                    paks.RemoveAt(next_pak_old_index);
                }
            }
            paks = ordered_paks;

            if (pak_map.LoadData("pakdata.dat") == 0)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Pak map loaded");
                game_directory_specified = true;
                return 0;
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFUnPak.SpecifyGameDirectory(): Failed to load pak map, generating new one");

                pak_map.AddPaks(files);
                pak_map.SaveData("pakdata.dat");
            }
            game_directory_specified = true;
            return 0;
        }

        // extract file from pak to a given path
        static public int ExtractFileFrom(string pak_name, string filename, string new_name)
        {
            byte[] ms = LoadFileFrom(pak_name, filename);
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

            new_file.Write(ms, 0, ms.Length);
            new_file.Close();

            return 0;
        }

        // loads file from pak to memory given its index
        // returns stream of bytes which constitute for that file
        static public byte[] LoadFileFrom(int pak_index, string filename)
        {
            return pak_map.pak_map[pak_index].GetFileBuffer(filename);
        }

        // loads file from pak to memory given its name
        // returns stream of bytes which constitute for that file
        static public byte[] LoadFileFrom(string pak_name, string filename)
        {
            int pak_index;
            if(!pak_map.filename_to_pak.TryGetValue(pak_name, out pak_index))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFUnPak, "SFUnPak.LoadFileFrom(): Could not find pak file " + pak_name);
                return null;
            }

            return LoadFileFrom(pak_map.filename_to_pak[pak_name], filename);
        }

        // searches for a file in paks and extracts it if founs
        // returns if succeeded
        static public int ExtractFileFind(string filename, string new_name, IEnumerable<int> pakfiles)
        {
            byte[] ms = LoadFileFind(filename, pakfiles);
            if (ms == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFUnPak.ExtractFileFind(): Could not find file " + filename);
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

            new_file.Write(ms, 0, ms.Length);
            new_file.Close();

            return 0;
        }

        // searches for a file in paks and loads it to memory
        // returns stream of bytes which constitute for that file
        static public byte[] LoadFileFind(string filename, IEnumerable<int> pakfiles)
        {
            byte[] ret = null;

            foreach (int pf in pakfiles)
            {
                ret = LoadFileFrom(pf, filename);
                if (ret != null)
                {
                    break;
                }
            }
            return ret;
        }

        // searches for a file in paks and loads it to memory
        // returns stream of bytes which constitute for that file
        static public byte[] LoadFileFind(string filename, IEnumerable<string> pakfiles)
        {
            List<int> pakfile_inds = null;
            if (pakfiles != null)
            {
                pakfile_inds = new List<int>();
                foreach (string pf in pakfiles)
                {
                    int pak_index;
                    if (pak_map.filename_to_pak.TryGetValue(pf, out pak_index))
                    {
                        pakfile_inds.Add(pak_index);
                    }
                }
            }
            return LoadFileFind(filename, pakfile_inds);
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

        static public List<String> ListAllInDirectory(string path, string[] pak_filter)
        {
            List<String> result = pak_map.ListAllInDirectory(path, pak_filter);
            result.Sort();
            return result;
        }

        static public SFPakFileSystem GetPak(string pak)
        {
            return pak_map.GetPak(pak);
        }

        static public void CloseAllPaks()
        {
            pak_map.CloseAllPaks();
        }
    }
}
