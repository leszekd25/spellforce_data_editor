using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapEditor
    {
        public SFMap map = null;

        public virtual void OnMousePress(SFCoord pos, MouseButtons b)
        {

        }

        public virtual void OnMouseUp(MouseButtons b)
        {

        }
    }
}
