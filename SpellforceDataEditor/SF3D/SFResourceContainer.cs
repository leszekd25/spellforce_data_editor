using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SF3D
{
    public class SFResourceContainer<T> where T: SFResource, new()
    {
        Dictionary<string, T> cont =new Dictionary<string, T>();
        string prefix_path = "";
        string suffix_extension = "";
        SFResourceManager manager;

        public SFResourceContainer()
        {
        }

        public SFResourceContainer(string p, string s, SFResourceManager _man)
        {
            cont = new Dictionary<string, T>();
            prefix_path = p;
            suffix_extension = s;
            manager = _man;
        }

        public int Load(string rname)
        {
            if (cont.ContainsKey(rname))
                return -1;
            MemoryStream ms = manager.unpacker.LoadFileFind(prefix_path+"\\" + rname + suffix_extension);
            if (ms == null)
                return -2;
            T resource = new T();
            int res_code = resource.Load(ms, manager);
            if (res_code != 0)
                return res_code;
            resource.Init();
            cont.Add(rname, resource);
            return 0;
        }

        public int Dispose(string rname)
        {
            if (!cont.ContainsKey(rname))
                return -1;
            cont[rname].Dispose();
            cont.Remove(rname);
            return 0;
        }

        public T Get(string rname)
        {
            if (cont.ContainsKey(rname))
                return cont[rname];
            return default(T);   //should return null
        }

        public void DisposeAll()
        {
            string[] names = cont.Keys.ToArray();
            foreach (string rname in names)
                Dispose(rname);
        }
    }
}
