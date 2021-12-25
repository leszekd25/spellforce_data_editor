using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFEngine.SFResources;

namespace SFEngine.SFSound
{
    public class StreamResource: SFResource
    {
        public Byte[] sound_data { get; private set; }
        string name = "";

        public void Init()
        {

        }

        public int Load(MemoryStream ms, object custom_data)
        {
            long data_length = ms.Length;
            sound_data = new byte[data_length];
            ms.Read(sound_data, 0, (int)data_length);
            return 0;
        }

        public void SetName(string s)
        {
            name = s;
        }

        public string GetName()
        {
            return name;
        }

        public int GetSizeBytes()
        {
            if (sound_data == null)
                return 0;
            return sound_data.Length;
        }

        public void Dispose()
        {
            sound_data = null;
        }
    }
}
