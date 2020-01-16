/*
 * SFResourceContainer is a generic container for any type of SFResource
 * It is managed by SFResourceManager, which provides it with data source (PAK files)
 * This container retrieves data based on provided path and extension (at init)
 * Manager manually disposes of all data in the containers
 * */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpellforceDataEditor.SFUnPak;

namespace SpellforceDataEditor.SFResources
{
    public class SFResourceContainer<T> where T: SFResource, new()
    {
        Dictionary<string, T> cont =new Dictionary<string, T>();
        Dictionary<string, int> reference_count = new Dictionary<string, int>();
        string prefix_path = "";
        string[] suffix_extensions = null;
        int total_size = 0;

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
        public int Load(string rname)
        {
            // if resource is already loaded, increase reference count to that resource and return -1
            if (cont.ContainsKey(rname))
            {
                reference_count[rname] += 1;
                // LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Resource " + rname + " ref counter = " + reference_count[rname].ToString());
                return -1;
            }

            // determine if resource exists in pak files, given the extension types
            string res_to_load = "";
            MemoryStream ms = null;
            foreach (string ext in suffix_extensions)
            {
                res_to_load = rname;
                if (!rname.Contains(ext))
                    res_to_load += ext;
                ms = SFUnPak.SFUnPak.LoadFileFind(prefix_path + "\\" + res_to_load);
                if (ms != null)
                    break;
                LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Could not find resource " + prefix_path + "\\" + res_to_load);
            }
            if(ms == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): None of the suffix extensions matched the given resource");
                return -2;
            }
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Loading resource " + res_to_load);

            //resource loading stack
            string prev_res = SFResourceManager.current_resource;
            SFResourceManager.current_resource = rname;
            T resource = new T();
            int res_code = resource.Load(ms);
            SFResourceManager.current_resource = prev_res;
            //end of stack

            // if failed to load resource, return error code
            if (res_code != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Could not load resource " + prefix_path + "\\" + res_to_load);
                return res_code;
            }

            total_size += resource.GetSizeBytes();
            resource.Init();
            resource.SetName(rname);
            cont.Add(rname, resource);
            reference_count.Add(rname, 1);
            // LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.Load(): Resource " + rname + " ref counter = " + reference_count[rname].ToString());
            ms.Close();
            //System.Diagnostics.Debug.WriteLine("LOADED " + rname + suffix_extension);
            return 0;
        }

        // memorystream with data, name for the resource
        public int LoadFromMemory(MemoryStream ms, string rname)
        {
            if (cont.ContainsKey(rname))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.LoadFromMemory(): Resource "+rname+" already exists!");
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
            int res_code = resource.Load(ms);
            SFResourceManager.current_resource = prev_res;
            //end of stack
            if (res_code != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFResources, "SFResourceContainer.LoadFromMemory(): Could not load resource " + rname + " from memory!");
                return res_code;
            }
            total_size += resource.GetSizeBytes();
            resource.Init();
            resource.SetName(rname);
            cont.Add(rname, resource);
            reference_count.Add(rname, 1);
            // LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.LoadFromMemory(): Resource " + rname + " ref counter = " + reference_count[rname].ToString());
            ms.Close();
            //System.Diagnostics.Debug.WriteLine("LOADED " + rname + suffix_extension);
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
            total_size += res.GetSizeBytes();
            res.Init();
            res.SetName(rname);
            cont.Add(rname, res);
            reference_count.Add(rname, 0);
            // LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.AddManually(): Resource " + rname + " ref counter = " + reference_count[rname].ToString());

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
            // LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.Dispose(): Resource " + rname + " ref counter = " + reference_count[rname].ToString());

            if (reference_count[rname] <= 0)
            {
                if (reference_count[rname] != 0)
                    LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceContainer.Dispose(): Negative reference count!");
                LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.Dispose(): Removing resource "+rname);

                total_size -= cont[rname].GetSizeBytes();
                cont[rname].Dispose();
                cont.Remove(rname);
                reference_count.Remove(rname);
                return 1;
            }
            //System.Diagnostics.Debug.WriteLine("DISPOSED " + rname + suffix_extension);
            return 0;
        }

        public T Get(string rname)
        {
            if (cont.ContainsKey(rname))
                return cont[rname];
            return default(T);   //should return null
        }

        public int Extract(string rname)
        {
            foreach(string ext in suffix_extensions)
            {
                string extract_fname = Settings.ExtractDirectory;
                if (extract_fname != "")
                    extract_fname += "\\";
                if (!Settings.ExtractAllInOne)
                    extract_fname += prefix_path + "\\";
                extract_fname += rname + ext;
                if (SFUnPak.SFUnPak.ExtractFileFind(prefix_path + "\\" + rname + ext, extract_fname) == 0)
                    return 0;
            }
            return -1;
        }

        public List<string> GetNames()
        {
            return cont.Keys.ToList();
        }

        public int GetResourceSize()
        {
            return total_size;
        }

        // removes all resources no matter the reference counters
        public void DisposeAll()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.DisposeAll() called");

            string[] names = cont.Keys.ToArray();
            foreach (string rname in names)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceContainer.DisposeAll(): Removing resource " + rname);

                cont[rname].Dispose();
                cont.Remove(rname);
                reference_count.Remove(rname);
            }
            total_size = 0;
        }
    }
}
