/*
 * SFPakMap contains a set of SFPakFileSystem objects, each bound to a unique PAK archive,
 *      and methods for retrieving binary data of specified files from any PAK (or some of them) in that set
 * For convenience, SFPakMap can preload data from directory for later use to speed up loading times
 * */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFUnPak
{
    public class SFPakMap
    {
        Dictionary<string, SFPakFileSystem> pak_map= new Dictionary<string, SFPakFileSystem>();

        public SFPakMap()
        {

        }

        // adds a new pak to the container, given pak filename
        public int AddPak(string pak_fname)
        {
            if (!File.Exists(pak_fname))
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFPakMap.AddPak(): Pak file "+pak_fname+" does not exist!");

                return -4;
            }

            SFPakFileSystem fs = new SFPakFileSystem();
            int open_result = fs.Init(pak_fname);
            if (open_result != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakMap.AddPak(): Error initializing pak file " + pak_fname);

                System.Diagnostics.Debug.WriteLine("ERROR PAK INIT " + pak_fname + " CODE" + open_result.ToString());
                return open_result;
            }

            pak_map.Add(Path.GetFileName(pak_fname), fs);
            return 0;
        }

        // gets pak data from container, given pak name
        public SFPakFileSystem GetPak(string pak_name)
        {
            if (pak_map.ContainsKey(pak_name))
                return pak_map[pak_name];
            LogUtils.Log.Warning(LogUtils.LogSource.SFUnPak, "SFPakMap.GetPak(): Could not find pak name "+pak_name);

            return null;
        }

        // saves all pak data to a single file
        // faster load times this way
        public int SaveData(string fname)
        {
            FileStream fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(pak_map.Count);
            foreach (KeyValuePair<string, SFPakFileSystem> kv in pak_map)
            {
                bw.Write(kv.Key);
                kv.Value.WriteToFile(bw);
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
            catch(Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFUnPak, "SFPakMap.LoadData(): File "+fname+" does not exist!");
                return -2;
            }
            BinaryReader br = new BinaryReader(fs);
            pak_map.Clear();
            int num = br.ReadInt32();
            for(int i = 0; i < num; i++)
            {
                string key = br.ReadString();
                SFPakFileSystem sys = new SFPakFileSystem();
                sys.ReadFromFile(br);
                pak_map.Add(key, sys);
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
                pak_filter = pak_map.Keys.ToArray();
            foreach (string pak in pak_filter)
            {
                SFPakFileSystem fs = pak_map[pak];
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
                pak_filter = pak_map.Keys.ToArray();
            foreach (string pak in pak_filter)
            {
                SFPakFileSystem fs = pak_map[pak];
                names = names.Union(fs.ListAllWithFilename(path, substr)).ToList();
            }
            return names;
        }

        public List<string> ListAllInDirectory(string path, string[] pak_filter)
        {
            List<String> names = new List<String>();
            if (pak_filter == null)
                pak_filter = pak_map.Keys.ToArray();
            foreach (string pak in pak_filter)
            {
                SFPakFileSystem fs = pak_map[pak];
                names = names.Union(fs.ListAllInDirectory(path)).ToList();
            }
            return names;
        }

        public void  CloseAllPaks()
        {
            foreach (SFPakFileSystem sys in pak_map.Values)
                sys.Close();
        }


        // frees memory*
        public void Clear()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFUnPak, "SFPakMap.Clear() called");

            foreach (SFPakFileSystem sys in pak_map.Values)
                sys.Dispose();
            pak_map.Clear();
        }
    }
}
