/*
 * SFResource is an interface for a resource type object
 * Resource must be possible to load, initialize, and to dispose of
 * Resources are meant to be unique across the application
 * * */
using System;
using System.IO;

namespace SFEngine.SFResources
{

    public class SFResource
    {
        public int StorageSize { get; set; } = 0;
        public int RAMSize { get; set; } = 0;
        public int DeviceSize { get; set; } = 0;
        public string Name { get; set; } = "";

        public virtual int Load(MemoryStream ms, object custom_data) { return -1; }
        public virtual void Init() { }
        public virtual void Dispose() { }
    }
}
