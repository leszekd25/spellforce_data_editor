using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SFEngine.SFMap;
using SFEngine.SFCFF;
using SFEngine.SFLua;

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

        map_operators.MapOperatorTerrainHeight op_height = null;
        map_operators.MapOperatorTerrainTexture op_tex_correction = null;

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
            if (!((button == MouseButtons.Left) || (button == MouseButtons.Right)))
                return;

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

            if (op_height == null)
                op_height = new map_operators.MapOperatorTerrainHeight();
            if (op_tex_correction == null)
                op_tex_correction = new map_operators.MapOperatorTerrainTexture();

            if (button == MouseButtons.Left)
            {
                switch (EditMode)
                {
                    case HMapEditMode.RAISE:  // add
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                SFCoord coord = new SFCoord(i, j);

                                float cell_strength = GetStrengthAt(coord);
                                if (map.lake_manager.GetLakeIndexAt(coord) != SFEngine.Utility.NO_INDEX)
                                    continue;

                                if (!op_height.PreOperatorHeights.ContainsKey(coord))
                                    op_height.PreOperatorHeights.Add(coord, map.heightmap.height_data[j * map.width + i]);

                                map.heightmap.height_data[j * map.width + i] += (ushort)(Value * cell_strength);
                                if (map.heightmap.height_data[j * map.width + i] > 65535)
                                    map.heightmap.height_data[j * map.width + i] = 65535;

                                pixels.Add(coord);
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
                                SFCoord coord = new SFCoord(i, j);

                                float cell_strength = GetStrengthAt(coord);
                                if (cell_strength == 0)
                                    continue;
                                if (map.lake_manager.GetLakeIndexAt(coord) != SFEngine.Utility.NO_INDEX)
                                    continue;

                                if (!op_height.PreOperatorHeights.ContainsKey(coord))
                                    op_height.PreOperatorHeights.Add(coord, map.heightmap.height_data[j * map.width + i]);

                                map.heightmap.height_data[j * map.width + i] = (ushort)(Value);
                                if (Value == 0)
                                    map.heightmap.tile_data[j * map.width + i] = 0;

                                pixels.Add(coord);
                            }
                        }
                        break;

                    case HMapEditMode.SMOOTH:  // smooth
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                SFCoord coord = new SFCoord(i, j);

                                float cell_strength = GetStrengthAt(coord);
                                if (map.lake_manager.GetLakeIndexAt(coord) != SFEngine.Utility.NO_INDEX)
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
                                SFCoord coord = new SFCoord(i, j);

                                float cell_strength = GetStrengthAt(coord);
                                if (map.lake_manager.GetLakeIndexAt(coord) != SFEngine.Utility.NO_INDEX)
                                    continue;

                                if (!op_height.PreOperatorHeights.ContainsKey(coord))
                                    op_height.PreOperatorHeights.Add(coord, map.heightmap.height_data[j * map.width + i]);

                                map.heightmap.height_data[j * map.width + i] +=
                                    (ushort)((terrain_sum - map.heightmap.height_data[j * map.width + i]) * cell_strength * smooth_str);

                                pixels.Add(coord);
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
                                SFCoord coord = new SFCoord(i, j);

                                float cell_strength = GetStrengthAt(coord);
                                if (map.lake_manager.GetLakeIndexAt(coord) != SFEngine.Utility.NO_INDEX)
                                    continue;

                                if (!op_height.PreOperatorHeights.ContainsKey(coord))
                                    op_height.PreOperatorHeights.Add(coord, map.heightmap.height_data[j * map.width + i]);

                                if (Value * cell_strength >= map.heightmap.height_data[j * map.width + i])
                                {
                                    if (!op_tex_correction.PreOperatorTextures.ContainsKey(coord))
                                        op_tex_correction.PreOperatorTextures.Add(coord, map.heightmap.tile_data[j * map.width + i]);

                                    map.heightmap.height_data[j * map.width + i] = 0;
                                    map.heightmap.tile_data[j * map.width + i] = 0;
                                    update_texture = true;
                                }
                                else
                                {
                                    map.heightmap.height_data[j * map.width + i] -= (ushort)(Value * cell_strength);
                                }

                                pixels.Add(coord);
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
                                SFCoord coord = new SFCoord(i, j);

                                float cell_strength = GetStrengthAt(coord);
                                if (map.lake_manager.GetLakeIndexAt(coord) != SFEngine.Utility.NO_INDEX)
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
                                SFCoord coord = new SFCoord(i, j);

                                float cell_strength = GetStrengthAt(coord);
                                if (map.lake_manager.GetLakeIndexAt(coord) != SFEngine.Utility.NO_INDEX)
                                    continue;

                                if (!op_height.PreOperatorHeights.ContainsKey(coord))
                                    op_height.PreOperatorHeights.Add(coord, map.heightmap.height_data[j * map.width + i]);

                                int v = (int)((terrain_sum - map.heightmap.height_data[j * map.width + i]) * cell_strength * rough_str);
                                if (v > map.heightmap.height_data[j * map.width + i])
                                {
                                    if (!op_tex_correction.PreOperatorTextures.ContainsKey(coord))
                                        op_tex_correction.PreOperatorTextures.Add(coord, map.heightmap.tile_data[j * map.width + i]);

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

                                pixels.Add(coord);
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
            // submit operators
            if (op_height != null)
            {
                if (op_height.PreOperatorHeights.Count != 0)
                {
                    op_height.Finish(map);

                    if (op_tex_correction != null)
                    {
                        if (op_tex_correction.PreOperatorTextures.Count != 0)
                        {
                            op_tex_correction.Finish(map);

                            map_operators.MapOperatorCluster op_cluster = new map_operators.MapOperatorCluster();
                            op_cluster.SubOperators.Add(op_height);
                            op_cluster.SubOperators.Add(op_tex_correction);

                            op_cluster.Finish(map);
                            MainForm.mapedittool.op_queue.Push(op_cluster);
                        }
                        else
                            MainForm.mapedittool.op_queue.Push(op_height);
                    }
                    else
                        MainForm.mapedittool.op_queue.Push(op_height);
                }
            }
            op_tex_correction = null;
            op_height = null;

            MainForm.mapedittool.ui.RedrawMinimap(pixels);
            pixels.Clear();
            MainForm.mapedittool.update_render = true;
            base.OnMouseUp(b);
        }
    }
}
