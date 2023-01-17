using SFEngine.SFMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapLakesEditor : MapEditor
    {
        SFMapLake selected_lake = null;
        public MapBrush Brush { get; set; } = null;
        public int Depth = 50;
        bool is_adding = false;
        SFCoord lake_start_pos = new SFCoord(0, 0);
        const int BORDER_HEIGHT_DIFF = 50;
        public HashSet<SFCoord> selection = new HashSet<SFCoord>();
        public bool is_selecting = false;

        public void SelectLake(SFMapLake lake)
        {
            selected_lake = lake;
            MainForm.mapedittool.InspectorSelect(lake);
        }

        public override void OnMousePress(SFCoord pos, MouseButtons button, ref special_forms.SpecialKeysPressed specials)
        {
            if (!((button == MouseButtons.Left) || (button == MouseButtons.Right)))
            {
                return;
            }

            if (map == null)
            {
                return;
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

            if (button == MouseButtons.Left)
            {
                if ((is_selecting) && (is_adding))
                {
                    return;
                }

                if (!is_adding)
                {
                    lake_start_pos = pos;
                }

                is_adding = true;

                if (!is_selecting)
                {
                    SFMapHeightMapFlag flag = (Depth < SFMapLakeManager.LAKE_SHALLOW_DEPTH ? SFMapHeightMapFlag.LAKE_SHALLOW : SFMapHeightMapFlag.LAKE_DEEP);

                    for (int i = topleft.x; i <= bottomright.x; i++)
                    {
                        for (int j = topleft.y; j <= bottomright.y; j++)
                        {
                            SFCoord p = new SFCoord(i, j);
                            if (Brush.GetInvertedDistanceNormalized(p) == 1)
                            {
                                continue;
                            }

                            if (map.heightmap.IsFlagSet(p, SFMapHeightMapFlag.EDITOR_MASK))
                            {
                                continue;
                            }

                            selection.Add(p);
                            map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_SHALLOW | SFMapHeightMapFlag.LAKE_DEEP | SFMapHeightMapFlag.LAKE_SHORE, false);
                            map.heightmap.SetFlag(p, flag, true);
                        }
                    }

                    map.heightmap.RefreshOverlay();
                }
                else
                {
                    int lake_index = map.lake_manager.GetLakeIndexAt(lake_start_pos);
                    if (lake_index != SFEngine.Utility.NO_INDEX)
                    {
                        SelectLake(map.lake_manager.lakes[lake_index]);
                    }
                    else
                    {
                        SelectLake(null);
                    }
                }
            }
            else if (button == MouseButtons.Right)
            {
                if (!is_adding)
                {
                    lake_start_pos = pos;
                }

                is_adding = true;

                if (!is_selecting)
                {
                    for (int i = topleft.x; i <= bottomright.x; i++)
                    {
                        for (int j = topleft.y; j <= bottomright.y; j++)
                        {
                            SFCoord p = new SFCoord(i, j);
                            if (Brush.GetInvertedDistanceNormalized(p) == 1)
                            {
                                continue;
                            }

                            if (map.heightmap.IsFlagSet(p, SFMapHeightMapFlag.EDITOR_MASK))
                            {
                                continue;
                            }

                            selection.Add(p);
                            map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_SHALLOW | SFMapHeightMapFlag.LAKE_DEEP | SFMapHeightMapFlag.LAKE_SHORE, false);
                        }
                    }

                    map.heightmap.RefreshOverlay();
                }
                else
                {
                    // retrieve depth
                    int lake_index = map.lake_manager.GetLakeIndexAt(lake_start_pos);
                    if (lake_index != SFEngine.Utility.NO_INDEX)
                    {
                        MainForm.mapedittool.external_UpdateLakeDepth(map.lake_manager.lakes[lake_index].z_diff);
                    }
                }
            }

            MainForm.mapedittool.update_render = true;
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if (is_selecting)
            {
                is_adding = false;
                return;
            }

            if (is_adding)
            {
                SFCoord topleft = lake_start_pos;
                SFCoord bottomright = lake_start_pos;

                foreach (SFCoord p in selection)
                {
                    if (p.x < topleft.x)
                    {
                        topleft.x = p.x;
                    }

                    if (p.x > bottomright.x)
                    {
                        bottomright.x = p.x;
                    }

                    if (p.y < topleft.y)
                    {
                        topleft.y = p.y;
                    }

                    if (p.y > bottomright.y)
                    {
                        bottomright.y = p.y;
                    }
                }

                // fill with water
                if (b == MouseButtons.Left)
                {
                    // 1. find all lakes that touch or neighbor the selection
                    List<SFMapLake> lakes = new List<SFMapLake>();
                    foreach (var lake in map.lake_manager.lakes)
                    {
                        if (selection.Overlaps(lake.cells))
                        {
                            lakes.Add(lake);
                        }
                        else if (selection.Overlaps(lake.shore))
                        {
                            lakes.Add(lake);
                        }
                    }

                    // fix flags
                    foreach (SFCoord p in selection)
                    {
                        map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_SHALLOW | SFMapHeightMapFlag.LAKE_DEEP | SFMapHeightMapFlag.LAKE_SHORE, false);
                    }

                    foreach (var lake in lakes)
                    {
                        ushort lake_z = (ushort)(map.heightmap.GetZ(lake.start) + lake.z_diff);
                        foreach (SFCoord p in lake.cells)
                        {
                            short lake_cell_z_diff = (short)(lake_z - map.heightmap.GetZ(p));
                            map.heightmap.SetFlag(p, (lake_cell_z_diff < SFMapLakeManager.LAKE_SHALLOW_DEPTH ? SFMapHeightMapFlag.LAKE_SHALLOW : SFMapHeightMapFlag.LAKE_DEEP), true);
                        }
                        foreach (SFCoord p in lake.shore)
                        {
                            map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_SHORE, true);
                        }
                    }

                    // 2. exit early if existing lakes stuff doesnt match, or if selection is bad
                    HashSet<SFCoord> tmp_selection = map.heightmap.ExtractFirstSubisland(selection);
                    if (selection.Count != 0)
                    {
                        map.heightmap.RefreshOverlay();
                        selection.Clear();
                        is_adding = false;
                        MainForm.mapedittool.update_render = true;
                        return;
                    }
                    selection = tmp_selection;

                    HashSet<SFCoord> lakes_cells = new HashSet<SFCoord>();
                    int lakes_level = map.heightmap.GetZ(lake_start_pos);
                    int lakes_type = 0;
                    if (lakes.Count > 0)
                    {
                        lake_start_pos = lakes[0].start;
                        lakes_level = lakes[0].z_diff + map.heightmap.GetZ(lakes[0].start);
                        lakes_type = lakes[0].type;
                        lakes_cells.UnionWith(lakes[0].cells);

                        for (int i = 1; i < lakes.Count; i++)
                        {
                            if (lakes[i].z_diff + map.heightmap.GetZ(lakes[i].start) != lakes_level)
                            {
                                map.heightmap.RefreshOverlay();
                                selection.Clear();
                                is_adding = false;
                                MainForm.mapedittool.update_render = true;
                                return;
                            }
                            if (lakes[i].type != lakes_type)
                            {
                                map.heightmap.RefreshOverlay();
                                selection.Clear();
                                is_adding = false;
                                MainForm.mapedittool.update_render = true;
                                return;
                            }
                            lakes_cells.UnionWith(lakes[i].cells);
                        }
                    }
                    // do not allow adding lake if lake's starting pos is masked - no reference point to place the lake
                    else if (map.heightmap.IsFlagSet(lake_start_pos, SFMapHeightMapFlag.EDITOR_MASK))
                    {
                        map.heightmap.RefreshOverlay();
                        selection.Clear();
                        is_adding = false;
                        MainForm.mapedittool.update_render = true;
                        return;
                    }

                    // 3. level the terrain 
                    int lake_start_terrain_level = lakes_level - Depth;
                    if (lake_start_terrain_level <= 0)
                    {
                        map.heightmap.RefreshOverlay();
                        selection.Clear();
                        is_adding = false;
                        MainForm.mapedittool.update_render = true;
                        return;
                    }

                    map_operators.MapOperatorTerrainHeight op_height = new map_operators.MapOperatorTerrainHeight();
                    foreach (SFCoord p in selection)
                    {
                        op_height.PreOperatorHeights.Add(p, map.heightmap.height_data[p.y * map.width + p.x]);
                        map.heightmap.height_data[p.y * map.width + p.x] = (ushort)(lake_start_terrain_level);
                        op_height.PostOperatorHeights.Add(p, map.heightmap.height_data[p.y * map.width + p.x]);
                    }

                    // 4. add borders
                    int lake_border_terrain_level = lakes_level + BORDER_HEIGHT_DIFF;

                    HashSet<SFCoord> selection_border = map.heightmap.GetBorder(selection);
                    foreach (SFCoord p in selection_border)
                    {
                        if (map.heightmap.height_data[p.y * map.width + p.x] < lake_border_terrain_level)
                        {
                            if (!lakes_cells.Contains(p))
                            {
                                op_height.PreOperatorHeights.Add(p, map.heightmap.height_data[p.y * map.width + p.x]);
                                map.heightmap.height_data[p.y * map.width + p.x] = (ushort)(lake_border_terrain_level);
                                op_height.PostOperatorHeights.Add(p, map.heightmap.height_data[p.y * map.width + p.x]);
                            }
                        }
                    }

                    // 5. remove lakes and add new lake
                    List<SFMapLake> consumed_lakes = new List<SFMapLake>();
                    List<int> consumed_lakes_indices = new List<int>();

                    SFMapLake new_lake = map.lake_manager.AddLake(lake_start_pos, (short)Depth, lakes_type, -1, consumed_lakes, consumed_lakes_indices);
                    int new_lake_index = map.lake_manager.lakes.Count - 1;

                    map_operators.MapOperatorLake op_lake = new map_operators.MapOperatorLake()
                    {
                        pos = new_lake.start,
                        z_diff = new_lake.z_diff,
                        type = new_lake.type,
                        lake_index = new_lake_index,
                        change_add = true
                    };

                    MainForm.mapedittool.op_queue.OpenCluster();
                    for (int i = 0; i < consumed_lakes.Count; i++)
                    {
                        SFMapLake l = consumed_lakes[i];
                        int l_index = consumed_lakes_indices[i];

                        map_operators.MapOperatorLake op_lake2 = new map_operators.MapOperatorLake()
                        {
                            pos = l.start,
                            z_diff = l.z_diff,
                            type = l.type,
                            lake_index = l_index,
                            change_add = false
                        };
                        MainForm.mapedittool.op_queue.Push(op_lake2);
                    }
                    MainForm.mapedittool.op_queue.Push(op_height);
                    MainForm.mapedittool.op_queue.Push(op_lake);
                    MainForm.mapedittool.op_queue.CloseCluster();

                    map.heightmap.RebuildGeometry(topleft, bottomright);
                    map.heightmap.RefreshOverlay();
                    MainForm.mapedittool.ui.RedrawMinimap(map.lake_manager.lakes[new_lake_index].cells);
                }
                // dry water
                else if (b == MouseButtons.Right)
                {
                    List<SFMapLake> lakes = new List<SFMapLake>();
                    foreach (var lake in map.lake_manager.lakes)
                    {
                        if (selection.Overlaps(lake.cells))
                        {
                            lakes.Add(lake);
                        }
                    }

                    foreach (var lake in lakes)
                    {
                        ushort lake_z = (ushort)(map.heightmap.GetZ(lake.start) + lake.z_diff);
                        foreach (SFCoord p in lake.cells)
                        {
                            short lake_cell_z_diff = (short)(lake_z - map.heightmap.GetZ(p));
                            map.heightmap.SetFlag(p, (lake_cell_z_diff < SFMapLakeManager.LAKE_SHALLOW_DEPTH ? SFMapHeightMapFlag.LAKE_SHALLOW : SFMapHeightMapFlag.LAKE_DEEP), true);
                        }
                        foreach (SFCoord p in lake.shore)
                        {
                            map.heightmap.SetFlag(p, SFMapHeightMapFlag.LAKE_SHORE, true);
                        }
                    }

                    // 2. exit early if existing lakes stuff doesnt match, or if selection is bad
                    HashSet<SFCoord> tmp_selection = map.heightmap.ExtractFirstSubisland(selection);
                    if (selection.Count != 0)
                    {
                        map.heightmap.RefreshOverlay();
                        selection.Clear();
                        is_adding = false;
                        MainForm.mapedittool.update_render = true;
                        return;
                    }
                    selection = tmp_selection;

                    map_operators.MapOperatorTerrainHeight op_height = new map_operators.MapOperatorTerrainHeight();
                    List<int> removed_lakes_indices = new List<int>();
                    List<map_operators.MapOperatorLake> op_lakes = new List<map_operators.MapOperatorLake>();

                    foreach (var lake in lakes)
                    {
                        int lake_index = map.lake_manager.lakes.IndexOf(lake);
                        int lake_level = lake.z_diff + map.heightmap.GetZ(lake.start);
                        int land_level = lake_level + BORDER_HEIGHT_DIFF;

                        HashSet<SFCoord> intersection = new HashSet<SFCoord>(selection);
                        intersection.IntersectWith(lake.cells);
                        HashSet<SFCoord> remainder = new HashSet<SFCoord>(lake.cells);
                        remainder.ExceptWith(intersection);

                        removed_lakes_indices.Add(lake_index);

                        foreach (SFCoord p in intersection)
                        {
                            op_height.PreOperatorHeights.Add(p, map.heightmap.height_data[p.y * map.width + p.x]);
                            map.heightmap.height_data[p.y * map.width + p.x] = (ushort)(land_level);
                            op_height.PostOperatorHeights.Add(p, (ushort)(land_level));
                        }

                        if (remainder.Count() > 0)
                        {
                            List<HashSet<SFCoord>> new_lakes_cells = map.heightmap.SplitIsland(remainder);
                            foreach (HashSet<SFCoord> new_lake_cells in new_lakes_cells)
                            {
                                int new_lake_deepest_terrain_level = int.MaxValue;
                                SFCoord new_lake_deepest_terrain_point = new SFCoord(0, 0);
                                foreach (SFCoord p in new_lake_cells)
                                {
                                    int cur_z = map.heightmap.GetZ(p);
                                    if (cur_z < new_lake_deepest_terrain_level)
                                    {
                                        new_lake_deepest_terrain_level = cur_z;
                                        new_lake_deepest_terrain_point = p;
                                    }
                                }

                                map_operators.MapOperatorLake op_lake_add = new map_operators.MapOperatorLake()
                                {
                                    pos = new_lake_deepest_terrain_point,
                                    z_diff = (short)(lake_level - new_lake_deepest_terrain_level),
                                    type = lake.type,
                                    lake_index = 0,
                                    change_add = true,
                                };
                                op_lakes.Add(op_lake_add);

                                MainForm.mapedittool.ui.RedrawMinimap(new_lake_cells);
                            }
                        }

                        foreach (SFCoord p in op_height.PreOperatorHeights.Keys)
                        {
                            map.heightmap.height_data[p.y * map.width + p.x] = op_height.PreOperatorHeights[p];
                        }
                    }

                    // if any lakes were modified, then this list will not be empty
                    // otherwise, nothing will have happened
                    if (op_lakes.Count + removed_lakes_indices.Count != 0)
                    {
                        MainForm.mapedittool.op_queue.OpenCluster(true);
                        removed_lakes_indices.Sort();
                        for (int i = removed_lakes_indices.Count - 1; i >= 0; i--)
                        {
                            SFMapLake l = map.lake_manager.lakes[removed_lakes_indices[i]];
                            map_operators.MapOperatorLake op_lake2 = new map_operators.MapOperatorLake()
                            {
                                pos = l.start,
                                z_diff = l.z_diff,
                                type = l.type,
                                lake_index = removed_lakes_indices[i],
                                change_add = false,
                            };

                            MainForm.mapedittool.op_queue.Push(op_lake2);
                        }
                        MainForm.mapedittool.op_queue.Push(op_height);
                        for (int i = 0; i < op_lakes.Count; i++)
                        {
                            op_lakes[i].lake_index = map.lake_manager.lakes.Count - removed_lakes_indices.Count + i;
                            MainForm.mapedittool.op_queue.Push(op_lakes[i]);
                        }
                        MainForm.mapedittool.op_queue.CloseCluster();

                        map.heightmap.RebuildGeometry(topleft, bottomright);
                    }

                }

                map.heightmap.RefreshOverlay();
                selection.Clear();
                is_adding = false;
                MainForm.mapedittool.update_render = true;
            }
        }
    }
}
