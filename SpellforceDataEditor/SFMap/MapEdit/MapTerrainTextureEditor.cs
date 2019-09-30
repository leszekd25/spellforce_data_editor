using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapTerrainTextureEditor: MapEditor
    {
        public MapBrush Brush = null;
        public int SelectedTile = 0;
        public bool EditSimilar = false;

        public override void OnMousePress(SFCoord pos, MouseButtons b)
        {
            if (b == MouseButtons.Left)
            {
                if(SelectedTile == 0)
                    return;

                // if 1-31, increase to 224-254
                if (SelectedTile < 32)
                    SelectedTile += 223;

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
                        if (Brush.GetInvertedDistanceNormalized(new SFCoord(i, j)) == 1f)
                            continue;
                        if (map.heightmap.height_data[j * map.width + i] == 0)
                            continue;
                        if (EditSimilar)
                        {
                            bool b_mov = map.heightmap.texture_manager.texture_tiledata[SelectedTile].blocks_movement ^
                                         map.heightmap.texture_manager.texture_tiledata[map.heightmap.tile_data[j * map.width + i]].blocks_movement;
                            if (b_mov)
                                continue;
                        }
                        map.heightmap.tile_data[j * map.width + i] = (byte)(SelectedTile);
                    }
                }

                map.heightmap.RebuildTerrainTexture(topleft, bottomright);
            }
            else if (b == MouseButtons.Right)
            {
                if ((pos.x < 0) || (pos.x >= map.width))
                    return;
                if ((pos.y <= 0) || (pos.y > map.height))
                    return;
                int tex_id = map.heightmap.tile_data[pos.y * map.width + pos.x];
                if (tex_id > 223)
                    tex_id -= 223;
                ((map_controls.MapTerrainTextureInspector)MainForm.mapedittool.selected_inspector).SelectTileType((byte)tex_id);
            }
        }
    }
}
