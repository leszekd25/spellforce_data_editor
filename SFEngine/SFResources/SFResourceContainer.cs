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

namespace SFEngine.SFResources
{
    public class SFResourceContainer<T> where T : SFResource, new()
    {
        Dictionary<string, T> cont = new Dictionary<string, T>();
        Dictionary<string, int> reference_count = new Dictionary<string, int>();
        Dictionary<string, bool> remove_when_unused = new Dictionary<string, bool>();
        string prefix_path = "";
        string[] suffix_extensions = null;

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

        public SFResourceContainer()
        {
        }

        public SFResourceContainer(string p, string s)
        {
            cont = new Dictionary<string, T>();
            prefix_path = p;
            suffix_extensions = s.Replace("|", " ").Split(' ');
        }

        public void SetPrefixPath(string p)
        {
            prefix_path = p;
        }

        // name of the resource in the pak files
        public int Load(string rname, object custom_data = null)
        {
            // if resource is already loaded, increase reference count to that resource and return -1
            if (cont.ContainsKey(rname))
            {
                reference_count[rname] += 1;
                return -1;
            }

            // determine if resource exists in pak files, given the extension types
            string res_to_load = "";
            MemoryStream ms = null;
            foreach (string ext in suffix_extensions)
            {
                res_to_load = rname;
                if (!rname.Contains(ext))
                {
                    res_to_load += ext;
                }

                ms = SFUnPak.SFUnPak.LoadFileFind(prefix_path + "\\" + res_to_load);
                if (ms != null)
                {
                    break;
                }

                LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Could not find resource " + prefix_path + "\\" + res_to_load);
            }
            if (ms == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): None of the suffix extensions matched the given resource");
                return -2;
            }
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Loading resource " + res_to_load);

            //resource loading stack
            string prev_res = SFResourceManager.current_resource;
            SFResourceManager.current_resource = rname;
            T resource = new T();
            resource.StorageSize = (int)ms.Length;
            int res_code = resource.Load(ms, custom_data);
            SFResourceManager.current_resource = prev_res;
            //end of stack

            // if failed to load resource, return error code
            if (res_code != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Could not load resource " + prefix_path + "\\" + res_to_load);
                return res_code;
            }

            resource.Init();
            resource.Name = rname;
            cont.Add(rname, resource);
            reference_count.Add(rname, 1);
            remove_when_unused.Add(rname, true);
            ms.Close();
            return 0;
        }

        // memorystream with data, name for the resource
        public int LoadFromMemory(MemoryStream ms, string rname, object custom_data = null)
        {
            if (cont.ContainsKey(rname))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.LoadFromMemory(): Resource " + rname + " already exists!");
                throw new Exception("Can't load directly from memory: Resource already exists!");
            }
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.LoadFromMemory(): loading resource " + rname + " from memory");
            if (ms == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.LoadFromMemory(): Datastream not specified!");
                return -2;
            }
            //resource loading stack
            string prev_res = SFResourceManager.current_resource;
            SFResourceManager.current_resource = rname;
            T resource = new T();
            int res_code = resource.Load(ms, custom_data);
            SFResourceManager.current_resource = prev_res;
            //end of stack
            if (res_code != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.LoadFromMemory(): Could not load resource " + rname + " from memory!");
                return res_code;
            }

            resource.Init();
            resource.Name = rname;
            cont.Add(rname, resource);
            reference_count.Add(rname, 1);
            ms.Close();
            return 0;
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

        public T Get(string rname)
        {
            if (cont.ContainsKey(rname))
            {
                return cont[rname];
            }

            return default(T);   //should return null
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
                if (SFUnPak.SFUnPak.ExtractFileFind(prefix_path + "\\" + rname + ext, extract_fname) == 0)
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
