using SFEngine.SFMap;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    //public enum TerrainFlagType { MOVEMENT, VISION }

    public class MapTerrainFlagsEditor : MapEditor
    {
        public MapBrush Brush { get; set; }
        public SFMapHeightMapFlag FlagType { get; set; }

        map_operators.MapOperatorTerrainFlag op_flag = null;
        bool first_clicked = false;

        public override void OnMousePress(SFCoord pos, MouseButtons button, ref special_forms.SpecialKeysPressed specials)
        {
            if ((button != MouseButtons.Left) && (button != MouseButtons.Right))
            {
                return;
            }

            int size = (int)Brush.size;
            Brush.center = pos;
            SFCoord topleft = new SFCoord(pos.x - size, pos.y - size);
            SFCoord bottomright = new SFCoord(pos.x + size, pos.y + size);
            if (topleft.x < 0)
            {
                topleft.x = 0;
            }

            if (topleft.y < 0)
            {
                topleft.y = 0;
            }

            if (bottomright.x >= map.width)
            {
                bottomright.x = (short)(map.width - 1);
            }

            if (bottomright.y >= map.height)
            {
                bottomright.y = (short)(map.height - 1);
            }

            float tmp_size = Brush.size;
            if (specials.Shift)
            {
                if (first_clicked)
                {
                    return;
                }

                Brush.size = 2048.0f;
                topleft = new SFCoord(0, 0);
                bottomright = new SFCoord(map.width - 1, map.height - 1);
            }

            if (op_flag == null)
            {
                op_flag = new map_operators.MapOperatorTerrainFlag();
            }

            if (button == MouseButtons.Left)
            {
                for (int i = topleft.x; i <= bottomright.x; i++)
                {
                    for (int j = topleft.y; j <= bottomright.y; j++)
                    {
                        SFCoord p = new SFCoord(i, j);
                        if (Brush.GetInvertedDistanceNormalized(p) == 1f)
                        {
                            continue;
                        }

                        if (map.heightmap.IsFlagSet(p, SFMapHeightMapFlag.EDITOR_MASK))
                        {
                            continue;
                        }

                        if (!op_flag.PreOperatorFlags.ContainsKey(p))
                        {
                            op_flag.PreOperatorFlags.Add(p, map.heightmap.GetFlag(p));
                            map.heightmap.SetFlag(p, FlagType, true);
                        }
                    }
                }
            }
            else
            {
                for (int i = topleft.x; i <= bottomright.x; i++)
                {
                    for (int j = topleft.y; j <= bottomright.y; j++)
                    {
                        SFCoord p = new SFCoord(i, j);
                        if (Brush.GetInvertedDistanceNormalized(p) == 1f)
                        {
                            continue;
                        }

                        if (map.heightmap.IsFlagSet(p, SFMapHeightMapFlag.EDITOR_MASK))
                        {
                            continue;
                        }

                        if (!op_flag.PreOperatorFlags.ContainsKey(p))
                        {
                            op_flag.PreOperatorFlags.Add(p, map.heightmap.GetFlag(p));
                        }

                        map.heightmap.SetFlag(p, FlagType, false);
                    }
                }
            }

            first_clicked = true;
            map.heightmap.RefreshOverlay();
            Brush.size = tmp_size;
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if ((b != MouseButtons.Left) && (b != MouseButtons.Right))
            {
                return;
            }

            // submit operators
            if (op_flag != null)
            {
                if (op_flag.PreOperatorFlags.Count != 0)
                {
                    op_flag.Finish(map);

                    MainForm.mapedittool.op_queue.Push(op_flag);
                }
            }
            op_flag = null;
            first_clicked = false;

            base.OnMouseUp(b);
        }
    }
}
