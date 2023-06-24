/*
 * SFResourceContainer is a generic container for any type of SFResource
 * It is managed by SFResourceManager, which provides it with data source (PAK files)
 * This container retrieves data based on provided path and extension (at init)
 * Manager manually disposes of all data in the containers
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFEngine.SFUnPak;

namespace SFEngine.SFResources
{
    public class SFResourceContainer<T> where T : SFResource, new()
    {
        Dictionary<string, T> cont = new Dictionary<string, T>();
        Dictionary<string, int> reference_count = new Dictionary<string, int>();
        Dictionary<string, bool> remove_when_unused = new Dictionary<string, bool>();
        string prefix_path = "";
        string[] suffix_extensions = null;
        string[] paknames = null;
        int[] paks_to_search = null;
        HashSet<string> filesystem_resources = new HashSet<string>();

        public int RAMSize
        {
            get
            {
                int ret = 0;
                foreach (T res in cont.Values)
                    ret += res.RAMSize;
                return ret;
            }
        }

        public int DeviceSize
        {
            get
            {
                int ret = 0;
                foreach (T res in cont.Values)
                    ret += res.DeviceSize;
                return ret;
            }
        }


        public SFResourceContainer(string p, string s, string[] paks)
        {
            cont = new Dictionary<string, T>();
            prefix_path = p;
            suffix_extensions = s.Replace("|", " ").Split(' ');
            paknames = paks;
        }

        public void FindResourcePaks()
        {
            if(paknames == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.FindResourcePaks(): Did not specify paks!");
                return;
            }

            List<int> pak_list = new List<int>();
            foreach (string pf in paknames)
            {
                int pak_index;
                if (!SFUnPak.SFUnPak.pak_map.filename_to_pak.TryGetValue(pf, out pak_index))
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceContainer.FindResourcePaks(): Cound not find pak file " + pf);
                    continue;
                }
                pak_list.Add(pak_index);
            }
            paks_to_search = pak_list.ToArray();
        }

        public void SetPrefixPath(string p)
        {
            prefix_path = p;
        }

        public int ListAllFilesystemResources()
        {
            filesystem_resources.Clear();
            string dir = SFUnPak.SFUnPak.game_directory_name + "\\" + prefix_path;
            if (Directory.Exists(dir))
            {
                foreach(string e in suffix_extensions)
                {
                    foreach (string s in Directory.EnumerateFiles(dir, "*"+e))
                    {
                        filesystem_resources.Add(s);
                    }
                }
            }
            return filesystem_resources.Count;
        }

        // name of the resource in the pak files
        public bool Load(string rname, SFUnPak.FileSource source, out T res, out int err_code, object custom_data = null)
        {
            res = null;
            // if resource is already loaded, increase reference count to that resource and return -1
            if (cont.ContainsKey(rname))
            {
                reference_count[rname] += 1;
                res = cont[rname];
                err_code = -1;
                return true;
            }

            // determine if resource exists in pak files, given the extension types
            string res_to_load = "";
            string full_res_name = "";
            byte[] data = null;
            foreach (string ext in suffix_extensions)
            {
                res_to_load = rname;
                if (!rname.Contains(ext))
                {
                    res_to_load += ext;
                }
                full_res_name = prefix_path + "\\" + res_to_load;

                if((source & FileSource.FILESYSTEM) == FileSource.FILESYSTEM)
                {
                    string real_path = SFUnPak.SFUnPak.game_directory_name + "\\" + full_res_name;
                    if (filesystem_resources.Contains(real_path))
                    {
                        FileStream fs = new FileStream(real_path, FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(fs);
                        data = br.ReadBytes((int)br.BaseStream.Length);
                        br.Close();
                    }
                }
                if(data != null)
                {
                    break;
                }

                if ((source & FileSource.PAK) == FileSource.PAK)
                {
                    data = SFUnPak.SFUnPak.LoadFileFind(full_res_name, paks_to_search);
                }
                if (data != null)
                {
                    break;
                }

                LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Could not find resource " + full_res_name);
            }
            if (data == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): None of the suffix extensions matched the given resource");
                err_code = -2;
                return false;
            }
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Loading resource " + res_to_load);

            //resource loading stack
            string prev_res = SFResourceManager.current_resource;
            SFResourceManager.current_resource = rname;
            T resource = new T();
            resource.StorageSize = data.Length;
            int res_code = resource.Load(data, 0, custom_data);
            SFResourceManager.current_resource = prev_res;
            //end of stack

            // if failed to load resource, return error code
            if (res_code != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Could not load resource " + prefix_path + "\\" + res_to_load);

                err_code = res_code;
                return false;
            }

            resource.Init();
            resource.Name = rname;
            cont.Add(rname, resource);
            reference_count.Add(rname, 1);
            remove_when_unused.Add(rname, true);
            res = resource;
            err_code = 0;
            return true;
        }

        // resource, name for the resource
        public int AddManually(T res, string rname)
        {
            if (cont.ContainsKey(rname))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.AddManually(): Resource " + rname + " already exists!");
                throw new Exception("Can't load directly from memory: Resource already exists!");
            }

            res.Init();
            res.Name = rname;
            cont.Add(rname, res);
            reference_count.Add(rname, 1);
            remove_when_unused.Add(rname, true);

            return 0;
        }

        public int Dispose(T res)
        {
            if(res == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceContainer.Dispose(): Disposing null resource, skipped");
                return -2;
            }

            return Dispose(res.Name);
        }

        // decrements reference counter, only removes resource when the counter reaches 0
        public int Dispose(string rname)
        {
            if (!cont.ContainsKey(rname))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceContainer.Dispose(): Resource " + rname + " does not exist!");
                return -1;
            }
            reference_count[rname] -= 1;

            if (reference_count[rname] <= 0)
            {
                if (remove_when_unused[rname])
                {
                    if (reference_count[rname] != 0)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceContainer.Dispose(): Negative reference count!");
                    }

                    LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.Dispose(): Removing resource " + rname);

                    cont[rname].Dispose();
                    cont.Remove(rname);
                    reference_count.Remove(rname);
                    remove_when_unused.Remove(rname);
                    return 1;
                }
            }
            return 0;
        }

        // if a given resource has remove_when_unused set to true, it will be removed when reference count goes to 0
        public void SetRemoveWhenUnused(string rname, bool remove)
        {
            if (!cont.ContainsKey(rname))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceContainer.SetRemoveWhenUnused(): Resource " + rname + " does not exist!");
                return;
            }
            remove_when_unused[rname] = remove;
            if ((remove) && (reference_count[rname] <= 0))
            {
                reference_count[rname] = 1;
                Dispose(rname);
            }
        }

        public int Extract(string rname)
        {
            foreach (string ext in suffix_extensions)
            {
                string extract_fname = Settings.ExtractDirectory;
                if (extract_fname != "")
                {
                    extract_fname += "\\";
                }

                if (!Settings.ExtractAllInOne)
                {
                    extract_fname += prefix_path + "\\";
                }

                extract_fname += rname + ext;
                if (SFUnPak.SFUnPak.ExtractFileFind(prefix_path + "\\" + rname + ext, extract_fname, paks_to_search) == 0)
                {
                    return 0;
                }
            }
            return -1;
        }

        public List<string> GetNames()
        {
            return cont.Keys.ToList();
        }

        // removes all resources no matter the reference counters or if remove when unused is marked true
        public void DisposeAll()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.DisposeAll() called");

            foreach (string rname in cont.Keys)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.DisposeAll(): Removing resource " + rname);

                cont[rname].Dispose();
            }
            cont.Clear();
            reference_count.Clear();
            remove_when_unused.Clear();
        }

        public void LogUndisposedResources()
        {
            foreach (string res in cont.Keys)
                LogUtils.Log.Info(LogUtils.LogSource.SFResources, res + ": dangling references " + reference_count[res] + (remove_when_unused[res] ? "" : " (marked as permanent)"));
        }
    }
}
