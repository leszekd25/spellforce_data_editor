/*
 * SFPakMap contains a set of SFPakFileSystem objects, each bound to a unique PAK archive,
 *      and methods for retrieving binary data of specified files from any PAK (or some of them) in that set
 * For convenience, SFPakMap can preload data from directory for later use to speed up loading times
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SFEngine.SFUnPak
{
    public class SFPakMap
    {
        // (filename -> pakfilesystem)
        //Dictionary<string, SFPakFileSystem> pak_map = new Dictionary<string, SFPakFileSystem>();
        public SFPakFileSystem[] pak_map = null;
        public Dictionary<string, int> filename_to_pak = new Dictionary<string, int>();

        public SFPakMap()
        {

        }

        public int AddPaks(string[] pak_fnames)
        {
            pak_map = new SFPakFileSystem[pak_fnames.Length];
            for(int i = 0; i < pak_fnames.Length; i++)
            {
                string fname = pak_fnames[i];

                SFPakFileSystem fs = new SFPakFileSystem();
                int open_result = fs.Init(fname);
                if (open_result != 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakMap.AddPak(): Error initializing pak file " + fname);

                    System.Diagnostics.Debug.WriteLine("ERROR PAK INIT " + fname + " CODE" + open_result.ToString());
                }

                pak_map[i] = fs;
                filename_to_pak.Add(Path.GetFileName(fname), i);
            }
            return 0;
        }

        // gets pak data from container, given pak name
        public SFPakFileSystem GetPak(string pak_name)
        {
            if(filename_to_pak.ContainsKey(pak_name))
            {
                return pak_map[filename_to_pak[pak_name]];
            }

            LogUtils.Log.Warning(LogUtils.LogSource.SFUnPak, "SFPakMap.GetPak(): Could not find pak name " + pak_name);

            return null;
        }

        // saves all pak data to a single file
        // faster load times this way
        public int SaveData(string fname)
        {
            FileStream fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(pak_map.Length);
            for(int i = 0; i < pak_map.Length; i++)
            {
                bw.Write(pak_map[i].pak_fname);
                pak_map[i].WriteToFile(bw);
            }
            bw.Close();
            return 0;
        }

        // loads all pak data from a file
        public int LoadData(string fname)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakMap.LoadData(): File " + fname + " does not exist!");
                return -2;
            }
            BinaryReader br = new BinaryReader(fs);
            filename_to_pak.Clear();
            int num = br.ReadInt32();
            pak_map = new SFPakFileSystem[num];
            for (int i = 0; i < num; i++)
            {
                string key = br.ReadString();
                SFPakFileSystem sys = new SFPakFileSystem();
                sys.ReadFromFile(br);
                pak_map[i] = sys;
                filename_to_pak.Add(key, i);
            }
            br.Close();
            return 0;
        }

        // lists all files in a given directory with a fitting extension
        // pak_filter is a list of pak to search from
        public List<String> ListAllWithExtension(string path, string extname, string[] pak_filter)
        {
            List<String> names = new List<String>();
            if (pak_filter == null)
            {
                pak_filter = filename_to_pak.Keys.ToArray();
            }

            foreach (string pak in pak_filter)
            {
                if(!filename_to_pak.ContainsKey(pak))
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SFUnPak, "SFPakMap.ListAllWithExtension(): Missing pak " + pak);
                    continue;
                }

                if(pak_map.Length <= filename_to_pak[pak])
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SFUnPak, "SFPakMap.ListAllWithExtension(): Invalid pak index "+filename_to_pak[pak]);
                    continue;
                }

                SFPakFileSystem fs = pak_map[filename_to_pak[pak]];
                names = names.Union(fs.ListAllWithExtension(path, extname)).ToList();
            }
            return names;
        }

        // lists all files in a given directory with a fitting filename
        // pak_filter is a list of pak to search from
        public List<String> ListAllWithFilename(string path, string substr, string[] pak_filter)
        {
            List<String> names = new List<String>();
            if (pak_filter == null)
            {
                //pak_filter = pak_map.Keys.ToArray();
                pak_filter = filename_to_pak.Keys.ToArray();
            }

            foreach (string pak in pak_filter)
            {
                SFPakFileSystem fs = pak_map[filename_to_pak[pak]];
                names = names.Union(fs.ListAllWithFilename(path, substr)).ToList();
            }
            return names;
        }

        public List<string> ListAllInDirectory(string path, string[] pak_filter)
        {
            List<String> names = new List<String>();
            if (pak_filter == null)
            {
                pak_filter = filename_to_pak.Keys.ToArray();
            }

            foreach (string pak in pak_filter)
            {
                SFPakFileSystem fs = pak_map[filename_to_pak[pak]];
                names = names.Union(fs.ListAllInDirectory(path)).ToList();
            }
            return names;
        }

        public void CloseAllPaks()
        {
            foreach (SFPakFileSystem sys in pak_map)//.Values)
            {
                sys.Close();
            }
        }


        // frees memory*
        public void Clear()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFPakMap.Clear() called");
            if(pak_map == null)
            {
                return;
            }

            foreach (SFPakFileSystem sys in pak_map)//.Values)
            {
                sys.Dispose();
            }

            //pak_map.Clear();
            pak_map = null;
            filename_to_pak.Clear();
        }
    }
}
