using SFEngine.SFMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public enum HMapBrushInterpolationMode { CONSTANT = 0, LINEAR, SQUARE, SINUSOIDAL }
    public enum HMapEditMode { RAISE = 0, SET, SMOOTH }

    public class MapHeightMapEditor : MapEditor
    {
        public MapBrush Brush { get; set; }
        public int Value { get; set; }
        public HMapBrushInterpolationMode Interpolation { get; set; }
        public HMapEditMode EditMode { get; set; }

        HashSet<SFCoord> pixels = new HashSet<SFCoord>();

        map_operators.MapOperatorTerrainHeight op_height = null;
        map_operators.MapOperatorTerrainTexture op_tex_correction = null;
        bool first_clicked = false;
        bool only_first_click = false;

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
            {
                return;
            }
            if(only_first_click)
            {
                return;
            }
            if(specials.Ctrl)
            {
                only_first_click = true;
            }

            int size = (int)Math.Ceiling(Brush.size);
            Brush.center = clicked_pos;
            SFCoord topleft = new SFCoord(clicked_pos.x - size, clicked_pos.y - size);
            SFCoord bottomright = new SFCoord(clicked_pos.x + size, clicked_pos.y + size);
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
            HMapBrushInterpolationMode tmp_interp = Interpolation;
            if (specials.Shift)
            {
                if (first_clicked)
                {
                    return;
                }

                Brush.size = 2048.0f;
                Interpolation = HMapBrushInterpolationMode.CONSTANT;
                topleft = new SFCoord(0, 0);
                bottomright = new SFCoord(map.width - 1, map.height - 1);
            }
            double terrain_sum = 0;
            double terrain_weight = 0;
            bool update_texture = false;

            if (op_height == null)
            {
                op_height = new map_operators.MapOperatorTerrainHeight();
            }

            if (op_tex_correction == null)
            {
                op_tex_correction = new map_operators.MapOperatorTerrainTexture();
            }

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

                                if (map.heightmap.IsAnyFlagSet(coord, SFMapHeightMapFlag.EDITOR_MASK))
                                {
                                    continue;
                                }

                                float cell_strength = GetStrengthAt(coord);
                                if (cell_strength == 0)
                                {
                                    continue;
                                }

                                if (!op_height.PreOperatorHeights.ContainsKey(coord))
                                {
                                    op_height.PreOperatorHeights.Add(coord, map.heightmap.height_data[j * map.width + i]);
                                }

                                map.heightmap.height_data[j * map.width + i] += (ushort)(Value * cell_strength);
                                if (map.heightmap.height_data[j * map.width + i] > 65535)
                                {
                                    map.heightmap.height_data[j * map.width + i] = 65535;
                                }

                                pixels.Add(coord);
                            }
                        }
                        break;

                    case HMapEditMode.SET:  // set
                        if (Value == 0)
                        {
                            update_texture = true;
                        }

                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                SFCoord coord = new SFCoord(i, j);

                                if (map.heightmap.IsAnyFlagSet(coord, SFMapHeightMapFlag.EDITOR_MASK))
                                {
                                    continue;
                                }

                                float cell_strength = GetStrengthAt(coord);
                                if (cell_strength == 0)
                                {
                                    continue;
                                }

                                if (!op_height.PreOperatorHeights.ContainsKey(coord))
                                {
                                    op_height.PreOperatorHeights.Add(coord, map.heightmap.height_data[j * map.width + i]);
                                }

                                map.heightmap.height_data[j * map.width + i] = (ushort)(Value);

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

                                if (map.heightmap.IsAnyFlagSet(coord, SFMapHeightMapFlag.EDITOR_MASK))
                                {
                                    continue;
                                }

                                float cell_strength = GetStrengthAt(coord);
                                if (cell_strength == 0)
                                {
                                    continue;
                                }

                                terrain_sum += map.heightmap.height_data[j * map.width + i] * cell_strength;
                                terrain_weight += cell_strength;
                            }
                        }
                        if (terrain_weight == 0)
                        {
                            break;
                        }

                        terrain_sum /= terrain_weight;

                        float smooth_str = Value / 100.0f;
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                SFCoord coord = new SFCoord(i, j);

                                if (map.heightmap.IsAnyFlagSet(coord, SFMapHeightMapFlag.EDITOR_MASK))
                                {
                                    continue;
                                }

                                float cell_strength = GetStrengthAt(coord);
                                if (cell_strength == 0)
                                {
                                    continue;
                                }

                                ushort new_level = (ushort)(map.heightmap.height_data[j * map.width + i]
                                    + ((terrain_sum - map.heightmap.height_data[j * map.width + i]) * cell_strength * smooth_str));

                                if (!op_height.PreOperatorHeights.ContainsKey(coord))
                                {
                                    op_height.PreOperatorHeights.Add(coord, map.heightmap.height_data[j * map.width + i]);
                                }

                                map.heightmap.height_data[j * map.width + i] = new_level;

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

                                if (map.heightmap.IsAnyFlagSet(coord, SFMapHeightMapFlag.EDITOR_MASK))
                                {
                                    continue;
                                }

                                float cell_strength = GetStrengthAt(coord);
                                if (cell_strength == 0)
                                {
                                    continue;
                                }

                                ushort new_level = (ushort)(Math.Max(0, map.heightmap.height_data[j * map.width + i] - Value * cell_strength));

                                if (!op_height.PreOperatorHeights.ContainsKey(coord))
                                {
                                    op_height.PreOperatorHeights.Add(coord, map.heightmap.height_data[j * map.width + i]);
                                }

                                if (new_level == 0)
                                {
                                    if (!op_tex_correction.PreOperatorTextures.ContainsKey(coord))
                                    {
                                        op_tex_correction.PreOperatorTextures.Add(coord, map.heightmap.GetTile(coord));
                                    }

                                    map.heightmap.SetTile(coord, 0);
                                    update_texture = true;
                                }
                                map.heightmap.height_data[j * map.width + i] = new_level;

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

                                if (map.heightmap.IsAnyFlagSet(coord, SFMapHeightMapFlag.EDITOR_MASK))
                                {
                                    continue;
                                }

                                float cell_strength = GetStrengthAt(coord);
                                if (cell_strength == 0)
                                {
                                    continue;
                                }

                                terrain_sum += map.heightmap.height_data[j * map.width + i] * cell_strength;
                                terrain_weight += cell_strength;
                            }
                        }
                        if (terrain_weight == 0)
                        {
                            break;
                        }

                        terrain_sum /= terrain_weight;

                        float rough_str = Value / 100.0f;
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                SFCoord coord = new SFCoord(i, j);

                                if (map.heightmap.IsAnyFlagSet(coord, SFMapHeightMapFlag.EDITOR_MASK))
                                {
                                    continue;
                                }

                                float cell_strength = GetStrengthAt(coord);
                                if (cell_strength == 0)
                                {
                                    continue;
                                }

                                int v = (int)((terrain_sum - map.heightmap.height_data[j * map.width + i]) * cell_strength * rough_str);
                                ushort new_level = (ushort)(Math.Min(65535, Math.Max(0, map.heightmap.height_data[j * map.width + i] - v)));

                                if (!op_height.PreOperatorHeights.ContainsKey(coord))
                                {
                                    op_height.PreOperatorHeights.Add(coord, map.heightmap.height_data[j * map.width + i]);
                                }

                                if (new_level == 0)
                                {
                                    if (!op_tex_correction.PreOperatorTextures.ContainsKey(coord))
                                    {
                                        op_tex_correction.PreOperatorTextures.Add(coord, map.heightmap.GetTile(coord));
                                    }

                                    map.heightmap.SetTile(coord, 0);
                                    update_texture = true;
                                }
                                map.heightmap.height_data[j * map.width + i] = new_level;

                                pixels.Add(coord);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            first_clicked = true;

            map.heightmap.RebuildGeometry(topleft, bottomright);
            if (update_texture)
            {
                map.heightmap.RebuildTerrainTexture(topleft, bottomright);
            }

            Brush.size = tmp_size;
            Interpolation = tmp_interp;
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (!((b == MouseButtons.Left) || (b == MouseButtons.Right)))
            {
                return;
            }

            // submit operators
            if (op_height != null)
            {
                if (op_height.PreOperatorHeights.Count != 0)
                {
                    op_height.Finish(map);
                    map_operators.MapOperatorCluster op_cluster = new map_operators.MapOperatorCluster() { Revert1stSubOperatorOnRevert = true };
                    op_cluster.SubOperators.Add(op_height);

                    if (op_tex_correction != null)
                    {
                        if (op_tex_correction.PreOperatorTextures.Count != 0)
                        {
                            op_tex_correction.Finish(map);

                            op_cluster.SubOperators.Add(op_tex_correction);

                        }
                    }

                    // modify lakes if need be
                    // 1. get all lakes touched by terrain operator
                    HashSet<SFMapLake> lakes = new HashSet<SFMapLake>();
                    foreach(SFCoord p in op_height.PreOperatorHeights.Keys)
                    {
                        int lake_index = map.lake_manager.GetLakeIndexAt(p);
                        if (lake_index != SFEngine.Utility.NO_INDEX)
                        {
                            lakes.Add(map.lake_manager.lakes[lake_index]);
                        }
                    }
                    if(lakes.Count > 0)
                    {
                        HashSet<SFMapLake> removed_lakes = new HashSet<SFMapLake>();
                        // 2. for each lake, determine cells inside lake that rose above lake level
                        foreach(SFMapLake lake in lakes)
                        {
                            // lake might have been already removed
                            if(removed_lakes.Contains(lake))
                            {
                                continue;
                            }

                            int lake_type = lake.type;
                            HashSet<SFCoord> landfill_cells = new HashSet<SFCoord>();
                            ushort lake_level;
                            if (op_height.PreOperatorHeights.ContainsKey(lake.start))
                            {
                                lake_level = (ushort)(op_height.PreOperatorHeights[lake.start] + lake.z_diff);
                            }
                            else
                            {
                                lake_level = lake.GetWaterLevel(map.heightmap);
                            }
                            foreach(SFCoord p in lake.cells)
                            {
                                if(map.heightmap.GetZ(p) > lake_level)
                                {
                                    landfill_cells.Add(p);
                                }
                            }

                            List<HashSet<SFCoord>> new_lake_areas;
                            List<SFCoord> lowest_points = new List<SFCoord>();
                            int lake_index = map.lake_manager.lakes.IndexOf(lake);
                            bool lakes_modified_so_far = false;
                            if (landfill_cells.Count > 0)
                            {
                                // split lake
                                HashSet<SFCoord> remaining_cells = new HashSet<SFCoord>(lake.cells);
                                remaining_cells.ExceptWith(landfill_cells);
                                new_lake_areas = map.heightmap.SplitIsland(remaining_cells);
                                // delete the lake
                                map.lake_manager.RemoveLake(lake);
                                removed_lakes.Add(lake);
                                map_operators.MapOperatorLake op_lake_remove = new map_operators.MapOperatorLake()
                                {
                                    lake_index = lake_index,
                                    type = lake_type,
                                    pos = lake.start,
                                    z_diff = lake.z_diff,
                                    change_add = false
                                };
                                op_cluster.SubOperators.Add(op_lake_remove);
                                MainForm.mapedittool.ui.RedrawMinimapLake(lake, false, false);
                                lakes_modified_so_far = true;
                            }
                            else
                            {
                                new_lake_areas = [lake.cells];
                            }

                            // find lowest place in each lake area
                            foreach (HashSet<SFCoord> area in new_lake_areas)
                            {
                                if(area.Count == 0)
                                {
                                    lowest_points.Add(new SFCoord(-1, -1));
                                    continue;
                                }
                                SFCoord lowest_point = area.First();
                                ushort lowest_z = map.heightmap.GetZ(lowest_point);
                                foreach(SFCoord p in area)
                                {
                                    ushort cur_z = map.heightmap.GetZ(p);
                                    if(cur_z < lowest_z)
                                    {
                                        lowest_z = cur_z;
                                        lowest_point = p;
                                    }
                                }
                                lowest_points.Add(lowest_point);
                            }
                            // now, calculate lakes if they were added at the given point
                            for(int i = 0; i < new_lake_areas.Count; i++)
                            {
                                // area is empty, skip
                                if (new_lake_areas[i].Count == 0)
                                {
                                    continue;
                                }
                                // lake was already made here due to another lake flood
                                if (lakes_modified_so_far)
                                {
                                    if (map.heightmap.IsAnyFlagSet(lowest_points[i], SFMapHeightMapFlag.LAKE_SHALLOW | SFMapHeightMapFlag.LAKE_DEEP))
                                    {
                                        continue;
                                    }
                                }

                                // make new lake
                                List<SFMapLake> consumed_lakes = new List<SFMapLake>();
                                List<int> consumed_lakes_indices = new List<int>();
                                SFMapLake new_lake = map.lake_manager.AddLake(lowest_points[i], lake_level, lake_type, -1, consumed_lakes, consumed_lakes_indices);
                                if(new_lake == null)
                                {
                                    continue;
                                }
                                int new_lake_index = map.lake_manager.lakes.Count - 1;

                                map_operators.MapOperatorLake op_lake_add = new map_operators.MapOperatorLake()
                                {
                                    pos = new_lake.start,
                                    z_diff = new_lake.z_diff,
                                    type = new_lake.type,
                                    lake_index = new_lake_index,
                                    change_add = true
                                };

                                for (int j = 0; j < consumed_lakes.Count; j++)
                                {
                                    SFMapLake l = consumed_lakes[j];
                                    int l_index = consumed_lakes_indices[j];
                                    MainForm.mapedittool.ui.RedrawMinimapLake(l, false, false);

                                    map_operators.MapOperatorLake op_lake2 = new map_operators.MapOperatorLake()
                                    {
                                        pos = l.start,
                                        z_diff = l.z_diff,
                                        type = l.type,
                                        lake_index = l_index,
                                        change_add = false
                                    };
                                    op_cluster.SubOperators.Add(op_lake2);
                                    removed_lakes.Add(l);
                                }
                                op_cluster.SubOperators.Add(op_lake_add);
                                MainForm.mapedittool.ui.RedrawMinimapLake(new_lake, true, false);
                                lakes_modified_so_far = true;
                            }
                        }
                    }

                    op_cluster.Finish(map);
                    MainForm.mapedittool.op_queue.Push(op_cluster);
                }
            }
            op_tex_correction = null;
            op_height = null;
            first_clicked = false;
            only_first_click = false;

            MainForm.mapedittool.ui.RedrawMinimapCells(pixels);
            pixels.Clear();
            MainForm.mapedittool.update_render = true;
            base.OnMouseUp(b);
        }
    }
}
