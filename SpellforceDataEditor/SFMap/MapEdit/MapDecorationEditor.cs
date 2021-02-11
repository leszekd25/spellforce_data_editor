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

        public override void OnMousePress(SFCoord pos, MouseButtons b, ref special_forms.SpecialKeysPressed specials)
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
                        map.heightmap.overlay_data_decals[p.y * map.width + p.x] = 6;
                    }
                }

                map.heightmap.RefreshOverlay();
            }
            else if (b == MouseButtons.Right)
            {
                byte new_dec_group = map.decoration_manager.GetDecAssignment(new SFCoord(pos.x, pos.y));
                MainForm.mapedittool.SelectDecorationGroup(new_dec_group);
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (is_adding)
            {
                map_operators.MapOperatorDecorationPaint dec_op = new map_operators.MapOperatorDecorationPaint();
                foreach (SFCoord p in selection)
                    dec_op.PreOperatorDecals.Add(p, map.decoration_manager.GetDecAssignment(p));

                map.decoration_manager.SetDecorationsToGroup(selection, selected_dec_group);

                selection.Clear();
                is_adding = false;

                if(selected_dec_group == 0)
                {
                    for (int i = 0; i < map.width; i++)
                        for (int j = 0; j < map.height; j++)
                            map.heightmap.overlay_data_decals[j * map.width + i] = 0;

                    map.heightmap.RefreshOverlay();
                }

                dec_op.Finish(map);
                MainForm.mapedittool.op_queue.Push(dec_op);

                MainForm.mapedittool.update_render = true;
            }
        }
    }
}
