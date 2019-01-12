using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpellforceDataEditor.SFResources;

namespace SpellforceDataEditor.SFSound
{
    public class StreamResource: SFResource
    {
        public Byte[] sound_data { get; private set; }

        public void Init()
        {

        }

        public int Load(MemoryStream ms)
        {
            long data_length = ms.Length;
            sound_data = new byte[data_length];
            ms.Read(sound_data, 0, (int)data_length);
            return 0;
        }

        public void Dispose()
        {

        }
    }
}
