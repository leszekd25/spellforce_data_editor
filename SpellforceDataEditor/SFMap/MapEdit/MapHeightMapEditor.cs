using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public enum HMapBrushInterpolationMode { CONSTANT = 0, LINEAR, SQUARE, SINUSOIDAL }
    public enum HMapEditMode { RAISE = 0, SET, SMOOTH }

    public class MapHeightMapEditor: MapEditor
    {
        public MapBrush Brush { get; set; }
        public int Value { get; set; }
        public HMapBrushInterpolationMode Interpolation { get; set; }
        public HMapEditMode EditMode { get; set; }

        HashSet<SFCoord> pixels = new HashSet<SFCoord>();

        private float GetStrengthAt(SFCoord pos)
        {
            float k = Brush.GetInvertedDistanceNormalized(pos);
            switch (Interpolation)
            {
                case HMapBrushInterpolationMode.CONSTANT:
                    return 1;
                case HMapBrushInterpolationMode.LINEAR:
                    return 1 - k;
                case HMapBrushInterpolationMode.SQUARE:
                    return 1 - (float)Math.Pow(k, 2);
                case HMapBrushInterpolationMode.SINUSOIDAL:
                    return (float)(Math.Sin(Math.PI * (0.5 - k)) + 1) / 2;
            }
            return 0;
        }

        public override void OnMousePress(SFCoord clicked_pos, MouseButtons button, ref special_forms.SpecialKeysPressed specials)
        {
            int size = (int)Math.Ceiling(Brush.size);
            Brush.center = clicked_pos;
            SFCoord topleft = new SFCoord(clicked_pos.x - size, clicked_pos.y - size);
            SFCoord bottomright = new SFCoord(clicked_pos.x + size, clicked_pos.y + size);
            if (topleft.x < 0)
                topleft.x = 0;
            if (topleft.y < 0)
                topleft.y = 0;
            if (bottomright.x >= map.width)
                bottomright.x = (short)(map.width - 1);
            if (bottomright.y >= map.height)
                bottomright.y = (short)(map.height - 1);
            double terrain_sum = 0;
            double terrain_weight = 0;
            bool update_texture = false;

            if (button == MouseButtons.Left)
            {
                switch (EditMode)
                {
                    case HMapEditMode.RAISE:  // add
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                float cell_strength = GetStrengthAt(new SFCoord(i, j));
                                if (map.heightmap.lake_data[j * map.width + i] != 0)
                                    continue;
                                map.heightmap.height_data[j * map.width + i] += (ushort)(Value * cell_strength);
                                if (map.heightmap.height_data[j * map.width + i] > 65535)
                                    map.heightmap.height_data[j * map.width + i] = 65535;

                                pixels.Add(new SFCoord(i, j));
                            }
                        }
                        break;

                    case HMapEditMode.SET:  // set
                        if (Value == 0)
                            update_texture = true;
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                float cell_strength = GetStrengthAt(new SFCoord(i, j));
                                if (cell_strength == 0)
                                    continue;
                                if (map.heightmap.lake_data[j * map.width + i] != 0)
                                    continue;
                                map.heightmap.height_data[j * map.width + i] = (ushort)(Value);
                                if (Value == 0)
                                    map.heightmap.tile_data[j * map.width + i] = 0;

                                pixels.Add(new SFCoord(i, j));
                            }
                        }
                        break;

                    case HMapEditMode.SMOOTH:  // smooth
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                float cell_strength = GetStrengthAt(new SFCoord(i, j));
                                if (map.heightmap.lake_data[j * map.width + i] != 0)
                                    continue;
                                terrain_sum += map.heightmap.height_data[j * map.width + i] * cell_strength;
                                terrain_weight += cell_strength;
                            }
                        }
                        if (terrain_weight == 0)
                            break;
                        terrain_sum /= terrain_weight;

                        float smooth_str = Value / 100.0f;
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                float cell_strength = GetStrengthAt(new SFCoord(i, j));
                                if (map.heightmap.lake_data[j * map.width + i] != 0)
                                    continue;
                                map.heightmap.height_data[j * map.width + i] +=
                                    (ushort)((terrain_sum - map.heightmap.height_data[j * map.width + i]) * cell_strength * smooth_str);

                                pixels.Add(new SFCoord(i, j));
                            }
                        }
                        break;
                        
                    default:
                        break;
                }
            }
            else if (button == MouseButtons.Right)
            {
                switch (EditMode)
                {
                    case HMapEditMode.RAISE:  // sub
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                float cell_strength = GetStrengthAt(new SFCoord(i, j));
                                if (map.heightmap.lake_data[j * map.width + i] != 0)
                                    continue;
                                if (Value * cell_strength >= map.heightmap.height_data[j * map.width + i])
                                {
                                    map.heightmap.height_data[j * map.width + i] = 0;
                                    map.heightmap.tile_data[j * map.width + i] = 0;
                                    update_texture = true;
                                }
                                else
                                    map.heightmap.height_data[j * map.width + i] -= (ushort)(Value * cell_strength);

                                pixels.Add(new SFCoord(i, j));
                            }
                        }
                        break;

                    case HMapEditMode.SET:  // get
                        Value = map.heightmap.height_data[clicked_pos.y * map.width + clicked_pos.x];
                        MainForm.mapedittool.HMapEditSetHeight(Value);
                        return;

                    case HMapEditMode.SMOOTH:  // rough
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                float cell_strength = GetStrengthAt(new SFCoord(i, j));
                                if (map.heightmap.lake_data[j * map.width + i] != 0)
                                    continue;
                                terrain_sum += map.heightmap.height_data[j * map.width + i] * cell_strength;
                                terrain_weight += cell_strength;
                            }
                        }
                        if (terrain_weight == 0)
                            break;
                        terrain_sum /= terrain_weight;

                        float rough_str = Value / 100.0f;
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                float cell_strength = GetStrengthAt(new SFCoord(i, j));
                                if (map.heightmap.lake_data[j * map.width + i] != 0)
                                    continue;

                                int v = (int)((terrain_sum - map.heightmap.height_data[j * map.width + i]) * cell_strength * rough_str);
                                if (v > map.heightmap.height_data[j * map.width + i])
                                {
                                    map.heightmap.height_data[j * map.width + i] = 0;
                                    map.heightmap.tile_data[j * map.width + i] = 0;
                                    update_texture = true;
                                }
                                else if (v + map.heightmap.height_data[j * map.width + i] > 65535)
                                    map.heightmap.height_data[j * map.width + i] = 65535;
                                else
                                    map.heightmap.height_data[j * map.width + i] =
                                        (ushort)(map.heightmap.height_data[j * map.width + i] -
                                        (ushort)(v * cell_strength * rough_str));

                                pixels.Add(new SFCoord(i, j));
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            map.heightmap.RebuildGeometry(topleft, bottomright);
            if (update_texture)
                map.heightmap.RebuildTerrainTexture(topleft, bottomright);
        }

        public override void OnMouseUp(MouseButtons b)
        {
            MainForm.mapedittool.ui.RedrawMinimap(map, pixels);
            pixels.Clear();
            MainForm.mapedittool.update_render = true;
            base.OnMouseUp(b);
        }
    }
}
