using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SF3D
{
    public class SFResourceManager
    {
        public SFUnPak.SFUnPak unpacker { get; private set; } = new SFUnPak.SFUnPak();
        public SFResourceContainer<SFTexture> Textures { get; private set; } = new SFResourceContainer<SFTexture>();
        public SFResourceContainer<SFModel3D> Models { get; private set; } = new SFResourceContainer<SFModel3D>();

        public SFResourceManager()
        {
            Textures = new SFResourceContainer<SFTexture>("texture", ".dds", this);
            Models = new SFResourceContainer<SFModel3D>("mesh", ".msb", this);
        }

        public void DisposeAll()
        {
            Textures.DisposeAll();
            Models.DisposeAll();
        }
    }
}
