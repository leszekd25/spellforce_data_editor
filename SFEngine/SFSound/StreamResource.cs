using SFEngine.SFResources;
using System;
using System.IO;

namespace SFEngine.SFSound
{
    public class StreamResource : SFResource
    {
        public Byte[] sound_data { get; private set; }
        public int offset { get; private set; }

        public override void Init()
        {

        }

        public override int Load(byte[] data, int offset, object custom_data)
        {
            sound_data = data;
            this.offset = offset;
            RAMSize = data.Length - offset;
            return 0;
        }

        public override void Dispose()
        {
            sound_data = null;
            offset = 0;
            RAMSize = 0;
        }
    }
}
