using SFEngine.SFMap;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapTerrainTextureEditor : MapEditor
    {
        public MapBrush Brush = null;
        public int SelectedTile = 0;

        HashSet<SFCoord> pixels = new HashSet<SFCoord>();

        map_operators.MapOperatorTerrainTexture op_texture = null;
        bool first_clicked = false;

        public override void OnMousePress(SFCoord pos, MouseButtons b, ref special_forms.SpecialKeysPressed specials)
        {
            if ((b != MouseButtons.Left) && (b != MouseButtons.Right))
            {
                return;
            }

            if (b == MouseButtons.Left)
            {
                if (SelectedTile == 0)
                {
                    return;
                }

                if (op_texture == null)
                {
                    op_texture = new map_operators.MapOperatorTerrainTexture();
                }

                // if 1-31, increase to 224-254
                if (SelectedTile < 32)
                {
                    SelectedTile += 223;
                }

                int size = (int)Math.Ceiling(Brush.size);
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

                for (int i = topleft.x; i <= bottomright.x; i++)
                {
                    for (int j = topleft.y; j <= bottomright.y; j++)
                    {
                        SFCoord coord = new SFCoord(i, j);

                        if (Brush.GetInvertedDistanceNormalized(coord) == 1f)
                        {
                            continue;
                        }

                        if (map.heightmap.height_data[j * map.width + i] == 0)
                        {
                            continue;
                        }

                        if (map.heightmap.IsFlagSet(coord, SFMapHeightMapFlag.EDITOR_MASK))
                        {
                            continue;
                        }

                        if (!op_texture.PreOperatorTextures.ContainsKey(coord))
                        {
                            op_texture.PreOperatorTextures.Add(coord, map.heightmap.GetTile(coord));//map.heightmap.tile_data[j * map.width + i]);
                        }

                        //map.heightmap.tile_data[j * map.width + i] = (byte)(SelectedTile);
                        map.heightmap.SetTile(coord, (byte)(SelectedTile));
                        pixels.Add(coord);
                    }
                }

                map.heightmap.RebuildTerrainTexture(topleft, bottomright);
                Brush.size = tmp_size;
            }
            else if (b == MouseButtons.Right)
            {
                if ((pos.x < 0) || (pos.x >= map.width))
                {
                    return;
                }

                if ((pos.y <= 0) || (pos.y > map.height))
                {
                    return;
                }

                int tex_id = map.heightmap.GetTile(pos);//map.heightmap.tile_data[pos.y * map.width + pos.x];
                if (tex_id > 223)
                {
                    tex_id -= 223;
                } ((map_controls.MapTerrainTextureInspector)MainForm.mapedittool.selected_inspector).SelectTileType((byte)tex_id);
                MainForm.mapedittool.SetTileType(
                    (tex_id < 32) ? map_controls.TerrainTileType.BASE : map_controls.TerrainTileType.CUSTOM);
            }

            first_clicked = true;
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (!((b == MouseButtons.Left) || (b == MouseButtons.Right)))
            {
                return;
            }

            if (op_texture != null)
            {
                if (op_texture.PreOperatorTextures.Count != 0)
                {
                    op_texture.Finish(map);

                    MainForm.mapedittool.op_queue.Push(op_texture);
                }
            }
            op_texture = null;
            first_clicked = false;

            MainForm.mapedittool.ui.RedrawMinimap(pixels, (byte)(SelectedTile >= 224 ? SelectedTile - 223 : SelectedTile));
            pixels.Clear();
            MainForm.mapedittool.update_render = true;
            base.OnMouseUp(b);
        }
    }
}
