using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SF3D
{
    public interface SFResource
    {
        int Load(MemoryStream ms, SFResourceManager rm);
        void Init();
        void Dispose();
    }
}
