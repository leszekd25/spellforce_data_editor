using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspector : UserControl
    {
        public SFEngine.SFMap.SFMap map = null;

        public MapInspector()
        {
            InitializeComponent();
        }

        public virtual void OnSelect(object o)
        {

        }
    }
}
