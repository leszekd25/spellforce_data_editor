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
            byte overlay_color;
            if (FlagType == TerrainFlagType.MOVEMENT)
            {
                data_list = map.heightmap.chunk42_data;
                overlay_color = 2;
            }
            else if (FlagType == TerrainFlagType.VISION)
            {
                data_list = map.heightmap.chunk56_data;
                overlay_color = 5;
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
                            // determine color
                            byte cur_color = map.heightmap.overlay_data_flags[j * map.width + i];
                            if ((overlay_color == 2) && ((cur_color == 5) || (cur_color == 10)))
                                overlay_color = 10;
                            else if ((overlay_color == 5) && ((cur_color == 2) || (cur_color == 9) || (cur_color == 10)))
                                overlay_color = 10;

                            map.heightmap.overlay_data_flags[j * map.width + i] = overlay_color;
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
                            // determine color
                            byte cur_color = map.heightmap.overlay_data_flags[j * map.width + i];
                            if(overlay_color == 5)
                            {
                                if (cur_color == 5)
                                    overlay_color = 0;
                                else if (cur_color == 10)
                                {
                                    if (!map.heightmap.chunk42_data.Contains(p))
                                        overlay_color = 9;
                                    else
                                        overlay_color = 2;
                                }
                                else
                                    overlay_color = cur_color;
                            }
                            else if(overlay_color == 2)
                            {
                                if (cur_color == 2)
                                {
                                    if (map.heightmap.texture_manager.texture_tiledata[map.heightmap.tile_data[j * map.width + i]].blocks_movement)
                                        overlay_color = 9;
                                    else
                                        overlay_color = 0;
                                }
                                else if (cur_color == 10)
                                {
                                    if (map.heightmap.texture_manager.texture_tiledata[map.heightmap.tile_data[j * map.width + i]].blocks_movement)
                                        overlay_color = 10;
                                    else
                                        overlay_color = 5;
                                }
                                else
                                    overlay_color = cur_color;
                            }
                            map.heightmap.overlay_data_flags[j * map.width + i] = overlay_color;
                        }
                    }
                }
            }

            map.heightmap.RefreshOverlay();
        }
    }
}
