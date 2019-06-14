using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    enum EditorMode { IDLE = 0, ADDING, REMOVING };

    public partial class MapInspectorLakeControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        EditorMode edit_mode = EditorMode.IDLE;
        HashSet<SFCoord> selection = new HashSet<SFCoord>();

        public MapInspectorLakeControl()
        {
            InitializeComponent();
        }

        // changes terrain in preparation of creating lake in the area
        private void PrepareLakeGround(SFCoord p, short lake_level, int slope_strength, int base_depth, int dist_to_boundary)
        {
            map.heightmap.height_data[p.y * map.width + p.x] =
                (short)(lake_level - slope_strength * dist_to_boundary);
        }

        private void PrepareUnlakeGround(SFCoord p, short lake_level)
        {
            map.heightmap.height_data[p.y * map.width + p.x] = (short)(lake_level + 5);
        }

        public override void OnMouseDown(SFCoord clicked_pos, MouseButtons button)
        {
            if (edit_mode == EditorMode.IDLE)
            {
                if (button == MouseButtons.Left)
                    edit_mode = EditorMode.ADDING;
                else if (button == MouseButtons.Right)
                    edit_mode = EditorMode.REMOVING;
            }

            int size = (int)Math.Ceiling(BrushControl.brush.size);
            BrushControl.brush.center = clicked_pos;
            SFCoord topleft = new SFCoord(clicked_pos.x - size, clicked_pos.y - size);
            SFCoord bottomright = new SFCoord(clicked_pos.x + size, clicked_pos.y + size);
            if (topleft.x < 0)
                topleft.x = 0;
            if (topleft.y < 0)
                topleft.y = 0;
            if (bottomright.x >= map.width)
                bottomright.x = map.width - 1;
            if (bottomright.y >= map.height)
                bottomright.y = map.height - 1;

            for (int i = topleft.x; i <= bottomright.x; i++)
            {
                for (int j = topleft.y; j <= bottomright.y; j++)
                {
                    SFCoord p = new SFCoord(i, j);
                    if (BrushControl.brush.GetStrengthAt(p) == 0)
                        continue;
                        
                    selection.Add(p);
                    map.heightmap.OverlayAdd("ManualLakeTile", new SFCoord(p.x, map.height - p.y - 1));
                }
            }

            map.heightmap.RebuildOverlay(topleft, bottomright, "ManualLakeTile");
        }

        public override void OnMouseUp()
        {
            HashSet<SFCoord> inverted_selection = new HashSet<SFCoord>();
            foreach (SFCoord p in selection)
                inverted_selection.Add(new SFCoord(p.x, map.height - p.y - 1));

            short base_depth = Utility.TryParseInt16(DepthTextBox.Text);
            short slope_strength = Utility.TryParseInt16(SlopeTextBox.Text);
            if ((base_depth == 0) || (slope_strength == 0))
            {
                LabelError.Text = "Parameter error - base depth or\r\nslope strength set to 0";
                edit_mode = EditorMode.IDLE;
                selection.Clear();
                map.heightmap.OverlayClear("ManualLakeTile");
                return;
            }

            SFCoord topleft = new SFCoord(30000, 30000);
            SFCoord bottomright = new SFCoord(-1, -1);
            foreach (SFCoord p in selection)
            {
                if (p.x < topleft.x)
                    topleft.x = p.x;
                if (p.y < topleft.y)
                    topleft.y = p.y;
                if (p.x > bottomright.x)
                    bottomright.x = p.x;
                if (p.y > bottomright.y)
                    bottomright.y = p.y;
            }

            // 1) find all separate lakes generated from selection alone, make sure it's only one lake
            List<HashSet<SFCoord>> selection_lakes_cells = map.heightmap.GetSeparateIslands(selection);
            if (selection_lakes_cells.Count != 1)
            {
                LabelError.Text = "Selection error - selection split between/r/nmultiple areas";
                edit_mode = EditorMode.IDLE;
                selection.Clear();
                map.heightmap.OverlayClear("ManualLakeTile");
                return;
            }
            else
                LabelError.Text = "";

            // 2) generate temporary lake
            SFMapLake tmp_lake = new SFMapLake();
            tmp_lake.cells = inverted_selection;
            tmp_lake.RecalculateBoundary();

            // 3) find all lakes this lake intersects or neighbors
            List<SFMapLake> affected_lakes = new List<SFMapLake>();
            foreach(SFCoord p in tmp_lake.boundary)
            {
                byte lake_i = map.heightmap.lake_data[p.y * map.width + p.x];
                if(lake_i != 0)
                {
                    SFMapLake l = map.lake_manager.lakes[lake_i - 1];
                    if (!affected_lakes.Contains(l))
                        affected_lakes.Add(l);
                }
            }

            // 4) check all possible cases
            if (edit_mode == EditorMode.ADDING)
            {
                // 4a) no lakes affected: create one new lake
                if (affected_lakes.Count == 0)
                {
                    // I. find lowest point on the boundary, set lake level to that -1
                    short min_h = 30000;   // theoretical max val is 32767
                    foreach (SFCoord p in tmp_lake.boundary)
                        min_h = (map.heightmap.height_data[p.y * map.width + p.x] < min_h ?
                            map.heightmap.height_data[p.y * map.width + p.x] : 
                            min_h);
                    min_h -= 1;     // water level

                    // II. gradually change terrain level of lake cells according to parameters
                    // also find lowest lake point and set it as a starting point
                    SFCoord start_pos = tmp_lake.cells.First();
                    foreach (SFCoord p in tmp_lake.cells)
                    {
                        PrepareLakeGround(p, min_h, slope_strength, base_depth, tmp_lake.GetDistanceFromBoundary(p));
                        if (map.heightmap.height_data[p.y * map.width + p.x] < 
                            map.heightmap.height_data[start_pos.y * map.width + start_pos.x])
                            start_pos = p;
                    }
                    // III. add lake based on determined parameters

                    short depth = (short)(min_h - map.heightmap.height_data[start_pos.y * map.width + start_pos.x]);
                    map.lake_manager.AddLake(start_pos, depth, TypeCombo.SelectedIndex);
                }

                // 4b) one lake affected: if selection is entirely in affected lake, only change lake type, otherwise merge lakes
                else if(affected_lakes.Count == 1)
                {
                    // I. check if new lake is entirely contained in affected lake
                    if(affected_lakes[0].cells.IsSupersetOf(inverted_selection))
                    {
                        string tex_name = map.lake_manager.GetLakeTextureName(TypeCombo.SelectedIndex);
                        if (SFResources.SFResourceManager.Textures.Load(tex_name) >= -1)
                        {
                            // simply change texture
                            map.render_engine.scene_manager.objects_static[affected_lakes[0].GetObjectName()].Mesh.materials[0].texture =
                                SFResources.SFResourceManager.Textures.Get(tex_name);
                        }
                    }
                    // II. if not, prepare lake ground on cells not belonging to the affected lake
                    else
                    {
                        // I. find lowest point on the boundary, set lake level to that -1
                        SFCoord af_lake_pos = affected_lakes[0].start;
                        short min_h = (short)(map.heightmap.height_data[af_lake_pos.y * map.width + af_lake_pos.x]
                                              + affected_lakes[0].z_diff);
                        short depth = affected_lakes[0].z_diff;

                        tmp_lake.cells.UnionWith(affected_lakes[0].cells);
                        tmp_lake.RecalculateBoundary();

                        // II. gradually change terrain level of lake cells according to parameters
                        // also find lowest lake point and set it as a starting po  int   
                        SFCoord start_pos = inverted_selection.First();
                        foreach (SFCoord p in inverted_selection)
                        {
                            if (affected_lakes[0].cells.Contains(p))
                                continue;
                            PrepareLakeGround(p, min_h, slope_strength, depth, tmp_lake.GetDistanceFromBoundary(p));
                        }

                        map.lake_manager.UpdateLake(affected_lakes[0]);
                    }
                }

                // 4c) multiple lakes affected: remove all lakes, create a new lake which encompasses selection and all removed lakes
                else
                {
                    // I. if all lakes are of the same level, you're good to go
                    SFCoord af_lake_pos = affected_lakes[0].start;
                    short min_h = (short)(map.heightmap.height_data[af_lake_pos.y * map.width + af_lake_pos.x]
                                          + affected_lakes[0].z_diff);
                    short depth = affected_lakes[0].z_diff;
                    for(int i = 1; i < affected_lakes.Count; i++)
                    {
                        SFCoord next_af_lake_pos = affected_lakes[i].start;
                        short next_min_h = (short)(map.heightmap.height_data[next_af_lake_pos.y * map.width + next_af_lake_pos.x]
                                              + affected_lakes[i].z_diff);
                        short next_depth = affected_lakes[i].z_diff;
                        if(next_min_h != min_h)
                        {
                            LabelError.Text = "Selection error - lakes are not\r\non the same level";
                            edit_mode = EditorMode.IDLE;
                            selection.Clear();
                            map.heightmap.OverlayClear("ManualLakeTile");
                            return;
                        }
                        else
                        {
                            if(next_depth > depth)
                            {
                                depth = next_depth;
                                af_lake_pos = next_af_lake_pos;
                            }
                        }
                    }
                    // II. prepare terrain for the lake at the selection
                    for(int i = 0; i < affected_lakes.Count; i++)
                        tmp_lake.cells.UnionWith(affected_lakes[i].cells);
                    tmp_lake.RecalculateBoundary();
                    
                    foreach (SFCoord p in inverted_selection)
                    {
                        bool is_lake = false;
                        for (int i = 0; i < affected_lakes.Count; i++)
                            if (affected_lakes[0].cells.Contains(p))
                            {
                                is_lake = true;
                                break;
                            }
                        if (is_lake)
                            continue;
                        PrepareLakeGround(p, min_h, slope_strength, depth, tmp_lake.GetDistanceFromBoundary(p));
                    }

                    // III. remove affected lakes
                    foreach(SFMapLake lake in affected_lakes)
                        map.lake_manager.RemoveLake(lake);

                    // IV. create new lake
                    map.lake_manager.AddLake(af_lake_pos, depth, TypeCombo.SelectedIndex);
                }
            }
            // 4) check all possible cases
            else if(edit_mode == EditorMode.REMOVING)
            {
                // 4a) no lakes affected: do nothing
                if(affected_lakes.Count == 0)
                {
                    LabelError.Text = "Selection error - no lakes selected";
                    edit_mode = EditorMode.IDLE;
                    selection.Clear();
                    map.heightmap.OverlayClear("ManualLakeTile");
                    return;
                }
                // 4b) one or more lakes affected: remove cells from each lake, determine if they were split into multiple lakes,
                // create lakes if this is the case
                foreach(SFMapLake lake in affected_lakes)
                {
                    if(lake.cells.IsSubsetOf(inverted_selection))
                    {
                        SFCoord af_lake_pos = lake.start;
                        short lake_lvl = (short)(map.heightmap.height_data[af_lake_pos.y * map.width + af_lake_pos.x]
                                              + lake.z_diff);
                        foreach (SFCoord p in lake.cells)
                            PrepareUnlakeGround(p, lake_lvl);
                        map.lake_manager.RemoveLake(lake);
                    }
                    else
                    {
                        short lake_lvl = (short)(map.heightmap.height_data[lake.start.y * map.width + lake.start.x]
                                              + lake.z_diff);

                        HashSet<SFCoord> intersection = new HashSet<SFCoord>();
                        foreach (SFCoord p in lake.cells)
                            intersection.Add(p);
                        intersection.IntersectWith(inverted_selection);

                        HashSet<SFCoord> new_lake_area = new HashSet<SFCoord>();
                        foreach (SFCoord p in lake.cells)
                            new_lake_area.Add(p);
                        foreach (SFCoord p in intersection)
                            new_lake_area.Remove(p);

                        List<HashSet<SFCoord>> new_lakes = map.heightmap.GetSeparateIslands(new_lake_area);

                        // unlake the ground
                        foreach (SFCoord p in intersection)
                            PrepareUnlakeGround(p, lake_lvl);

                        if(new_lakes.Count == 1)    // this means the lake was not split, only modified - simply modify existing lake
                        {
                            // modify lake cells
                            lake.cells = new_lake_area;
                            lake.RecalculateBoundary();
                            // modify lake terrain
                            foreach (SFCoord p in lake.cells)
                                PrepareLakeGround(p, lake_lvl, slope_strength, base_depth, lake.GetDistanceFromBoundary(p));
                            // find a new start and z_diff

                            SFCoord start_pos = lake.cells.First();
                            foreach (SFCoord p in lake.cells)
                            {
                                if (map.heightmap.height_data[p.y * map.width + p.x] <
                                    map.heightmap.height_data[start_pos.y * map.width + start_pos.x])
                                    start_pos = p;
                            }

                            // update lake
                            short depth = (short)(lake_lvl - map.heightmap.height_data[start_pos.y * map.width + start_pos.x] + 1);
                            lake.z_diff = depth;
                            lake.start = start_pos;
                            map.lake_manager.UpdateLake(lake);
                        }
                        else
                        {
                            // remove existing lake
                            map.lake_manager.RemoveLake(lake);
                            // generate new lake for each island
                            foreach(HashSet<SFCoord> island in new_lakes)
                            {
                                // find deepest point in each lake
                                short min_h = 30000;   // theoretical max val is 32767
                                SFCoord start_pos = island.First();
                                foreach (SFCoord p in island)
                                {
                                    if (map.heightmap.height_data[p.y * map.width + p.x] < min_h)
                                    {
                                        min_h = map.heightmap.height_data[p.y * map.width + p.x];
                                        start_pos = p;
                                    }
                                }
                                // add lake
                                short depth = (short)(lake_lvl - min_h);
                                SFMapLake generated_lake = map.lake_manager.AddLake(start_pos, depth, lake.type);
                                // prepare ground
                                foreach(SFCoord p in generated_lake.cells)
                                    PrepareLakeGround(p, lake_lvl, slope_strength, base_depth, generated_lake.GetDistanceFromBoundary(p));

                            }
                        }
                        // 1) generate temporary lake with affected lake's borders
                        // 2) subtract selection from temporary lake
                        // 3) generate temporary lake for each island created
                        // 4) unlake the ground
                        // 5) generate lake
                    }
                }
            }

            MainForm.mapedittool.update_render = true;
            edit_mode = EditorMode.IDLE;
            selection.Clear();
            map.heightmap.OverlayClear("ManualLakeTile");
            map.heightmap.RebuildGeometry(topleft, bottomright);
        }
    }
}
