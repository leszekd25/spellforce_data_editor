using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public enum TerrainFlagType { MOVEMENT, VISION }

    public class MapTerrainFlagsEditor: MapEditor
    {
        public MapBrush Brush { get; set; }
        public TerrainFlagType FlagType { get; set; }

        public override void OnMousePress(SFCoord pos, MouseButtons button)
        {
            int size = (int)Brush.size;
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

            List<SFCoord> data_list;
            string o_name;
            if (FlagType == TerrainFlagType.MOVEMENT)
            {
                data_list = map.heightmap.chunk42_data;
                o_name = "ManualMovementBlock";
            }
            else if (FlagType == TerrainFlagType.VISION)
            {
                data_list = map.heightmap.chunk56_data;
                o_name = "ManualVisionBlock";
            }
            else
                throw new Exception("MapTerrainFlagsEditor.OnMousePress(): Invalid flag value!");

            if (button == MouseButtons.Left)
            {
                for (int i = topleft.x; i <= bottomright.x; i++)
                {
                    for (int j = topleft.y; j <= bottomright.y; j++)
                    {
                        SFCoord p = new SFCoord(i, j);
                        if (Brush.GetInvertedDistanceNormalized(p) == 1f)
                            continue;
                        if (!data_list.Contains(p))
                        {
                            data_list.Add(p);
                            map.heightmap.OverlayAdd(o_name, p);
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
                            continue;
                        if (data_list.Contains(p))
                        {
                            data_list.Remove(p);
                            map.heightmap.OverlayRemove(o_name, p);
                        }
                    }
                }
            }

            map.heightmap.RebuildOverlay(topleft, bottomright, o_name);
        }
    }
}
