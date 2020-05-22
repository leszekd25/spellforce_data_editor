using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapQuickSelectHelper
    {
        public ushort[] ID { get; } = new ushort[10];

        public SFMapQuickSelectHelper()
        {
            ID.Initialize();
        }
    }
}
