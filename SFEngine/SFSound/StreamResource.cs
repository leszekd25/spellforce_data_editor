using SFEngine.SFResources;
using System;
using System.IO;

namespace SFEngine.SFSound
{
    public class StreamResource : SFResource
    {
        public Byte[] sound_data { get; private set; }

        public override void Init()
        {

        }

        public override int Load(MemoryStream ms, object custom_data)
        {
            long data_length = ms.Length;
            sound_data = new byte[data_length];
            ms.Read(sound_data, 0, (int)data_length);
            RAMSize = (int)data_length;
            return 0;
        }

        public override void Dispose()
        {
            sound_data = null;
            RAMSize = 0;
        }
    }
}
