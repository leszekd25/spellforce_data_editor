using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapDecorationEditor: MapEditor
    {
        public int selected_dec_group = 0;
        public MapBrush Brush { get; set; } = null;
        bool is_adding = false;
        public HashSet<SFCoord> selection = new HashSet<SFCoord>();

        public override void OnMousePress(SFCoord pos, MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                is_adding = true;

                int size = (int)Math.Ceiling(Brush.size);
                Brush.center = pos;
                SFCoord topleft = new SFCoord(pos.x - size, pos.y - size);
                SFCoord bottomright = new SFCoord(pos.x + size, pos.y + size);
                if (topleft.x < 0)
                    topleft.x = 0;
                if (topleft.y < 0)
                    topleft.y = 0;
                if (bottomright.x >= map.width)
                    bottomright.x = (short)(map.width - 1);
                if (bottomright.y >= map.height)
                    bottomright.y = (short)(map.height - 1);

                for (int i = topleft.x; i <= bottomright.x; i++)
                {
                    for (int j = topleft.y; j <= bottomright.y; j++)
                    {
                        SFCoord p = new SFCoord(i, j);
                        if (Brush.GetInvertedDistanceNormalized(p) == 1)
                            continue;

                        selection.Add(p);
                        map.heightmap.OverlayAdd("DecorationTile", new SFCoord(p.x, p.y));
                    }
                }

                map.heightmap.RebuildOverlay(topleft, bottomright, "DecorationTile");
            }
            else if (b == MouseButtons.Right)
            {
                byte new_dec_group = map.decoration_manager.GetFixedDecAssignment(new SFCoord(pos.x, pos.y));
                // selection
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (is_adding)
            {
                map.decoration_manager.ModifyDecorations(selection, selected_dec_group);

                selection.Clear();
                is_adding = false;

                if(selected_dec_group == 0)
                    map.heightmap.OverlayClear("DecorationTile");

                MainForm.mapedittool.update_render = true;
            }
        }
    }
}
