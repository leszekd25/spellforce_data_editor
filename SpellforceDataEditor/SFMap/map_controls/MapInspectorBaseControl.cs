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
    public partial class MapInspectorBaseControl : UserControl
    {
        public MapInspectorBaseControl()
        {
            InitializeComponent();
        }

        public virtual void OnMouseDown(SFMap map, SFCoord clicked_pos)
        {

        }
    }
}
