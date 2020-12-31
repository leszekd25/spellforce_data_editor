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

        public virtual void Select(int index)
        {

        }

        public virtual void OnMousePress(SFCoord pos, MouseButtons b, ref special_forms.SpecialKeysPressed specials)
        {

        }

        public virtual void OnMouseUp(MouseButtons b)
        {

        }
    }
}
