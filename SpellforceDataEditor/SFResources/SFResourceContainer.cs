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
        string suffix_extension = "";

        public SFResourceContainer()
        {
        }

        public SFResourceContainer(string p, string s)
        {
            cont = new Dictionary<string, T>();
            prefix_path = p;
            suffix_extension = s;
        }

        public void SetPrefixPath(string p)
        {
            prefix_path = p;
        }

        public void SetSuffixExtension(string e)
        {
            suffix_extension = e;
        }

        // name of the resource in the pak files
        public int Load(string rname)
        {
            if (cont.ContainsKey(rname))
            {
                reference_count[rname] += 1;
                return -1;
            }
            System.Diagnostics.Debug.WriteLine("LOADING "+rname+suffix_extension);
            MemoryStream ms = SFUnPak.SFUnPak.LoadFileFind(prefix_path+"\\" + rname + suffix_extension);
            if (ms == null)
                return -2;
            //resource loading stack
            string prev_res = SFResourceManager.current_resource;
            SFResourceManager.current_resource = rname;
            T resource = new T();
            int res_code = resource.Load(ms);
            SFResourceManager.current_resource = prev_res;
            //end of stack
            if (res_code != 0)
                return res_code;
            resource.Init();
            resource.SetName(rname);
            cont.Add(rname, resource);
            reference_count.Add(rname, 1);
            ms.Close();
            //System.Diagnostics.Debug.WriteLine("LOADED " + rname + suffix_extension);
            return 0;
        }

        // memorystream with data, name for the resource
        public int LoadFromMemory(MemoryStream ms, string rname)
        {
            if (cont.ContainsKey(rname))
                throw new Exception("Can't load directly from memory: Resource already exists!");
            if (ms == null)
                return -2;
            //resource loading stack
            string prev_res = SFResourceManager.current_resource;
            SFResourceManager.current_resource = rname;
            T resource = new T();
            int res_code = resource.Load(ms);
            SFResourceManager.current_resource = prev_res;
            //end of stack
            if (res_code != 0)
                return res_code;
            resource.Init();
            resource.SetName(rname);
            cont.Add(rname, resource);
            reference_count.Add(rname, 1);
            ms.Close();
            //System.Diagnostics.Debug.WriteLine("LOADED " + rname + suffix_extension);
            return 0;
        }

        // resource, name for the resource
        public int AddManually(T res, string rname)
        {
            if (cont.ContainsKey(rname))
                throw new Exception("Can't load directly from memory: Resource already exists!");
            res.Init();
            res.SetName(rname);
            cont.Add(rname, res);
            reference_count.Add(rname, 1);
            return 0;
        }

        // decrements reference counter, only removes resource when the counter reaches 0
        public int Dispose(string rname)
        {
            if (!cont.ContainsKey(rname))
                return -1;
            reference_count[rname] -= 1;
            if (reference_count[rname] == 0)
            {
                cont[rname].Dispose();
                cont.Remove(rname);
                reference_count.Remove(rname);
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
            return SFUnPak.SFUnPak.ExtractFileFind(prefix_path + "\\" + rname + suffix_extension, prefix_path + "\\" + rname + suffix_extension);
        }

        public List<string> GetNames()
        {
            return cont.Keys.ToList();
        }

        // removes all resources no matter the reference counters
        public void DisposeAll()
        {
            string[] names = cont.Keys.ToArray();
            foreach (string rname in names)
            {
                cont[rname].Dispose();
                cont.Remove(rname);
                reference_count.Remove(rname);
            }
        }
    }
}
