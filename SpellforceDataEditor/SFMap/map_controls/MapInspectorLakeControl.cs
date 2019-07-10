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

        private void MapInspectorLakeControl_VisibleChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (Visible == true)
            {
                for (int i = 0; i < map.heightmap.lake_data.Length; i++)
                {
                    SFCoord p = new SFCoord(i % map.width, i / map.width);
                    if (map.heightmap.lake_data[i] > 0)
                        map.heightmap.OverlayAdd("LakeTile", p);

                    map.heightmap.OverlaySetVisible("LakeTile", true);
                    map.heightmap.OverlaySetVisible("ManualLakeTile", true);
                }
            }
            else
            {
                map.heightmap.OverlayClear("LakeTile");
                map.heightmap.OverlaySetVisible("LakeTile", false);
                map.heightmap.OverlaySetVisible("ManualLakeTile", false);

            }

            foreach (SF3D.SceneSynchro.SceneNodeMapChunk chunk_node in map.heightmap.chunk_nodes)
                chunk_node.MapChunk.OverlayUpdate("LakeTile");

            MainForm.mapedittool.update_render = true;
        }

        private void PrepareLakeBoundary(SFMapLake lake)
        {
            short lake_lowest = map.heightmap.height_data[lake.start.y * map.width + lake.start.x];
            foreach (SFCoord p in lake.boundary)
                map.heightmap.height_data[p.y * map.width + p.x] = (short)(lake_lowest + lake.z_diff + 1);
        }

        // select cells to create lake over
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
                bottomright.x = (short)(map.width - 1);
            if (bottomright.y >= map.height)
                bottomright.y = (short)(map.height - 1);

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

        // create lake using selected cells
        public override void OnMouseUp()
        {
            HashSet<SFCoord> inverted_selection = new HashSet<SFCoord>();
            foreach (SFCoord p in selection)
                inverted_selection.Add(new SFCoord(p.x, map.height - p.y - 1));

            short base_depth = Utility.TryParseInt16(DepthTextBox.Text);
            short slope_strength = Utility.TryParseInt16(SlopeTextBox.Text);
            int lake_type = TypeCombo.SelectedIndex;
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
            
            if (edit_mode == EditorMode.ADDING)
            {
                // find lowest possible level
                short lowest_lvl = 32767;
                SFCoord new_start = new SFCoord(0, 0);
                foreach (SFMapLake lake in affected_lakes)
                {
                    SFCoord tmp_p = lake.start;
                    if (lowest_lvl > map.heightmap.height_data[tmp_p.y * map.width + tmp_p.x])
                    {
                        new_start = tmp_p;
                        lowest_lvl = map.heightmap.height_data[tmp_p.y * map.width + tmp_p.x];
                    }
                }
                if(lowest_lvl == 32767)
                {
                    foreach (SFCoord p in tmp_lake.cells)
                    {
                        if (lowest_lvl > map.heightmap.height_data[p.y * map.width + p.x] - base_depth)
                        {
                            new_start = p;
                            lowest_lvl = (short)(map.heightmap.height_data[p.y * map.width + p.x] - base_depth);
                        }
                    }
                }

                foreach (SFCoord p in tmp_lake.cells)
                {
                    map.heightmap.height_data[p.y * map.width + p.x] = lowest_lvl;
                    map.heightmap.OverlayAdd("LakeTile", p);
                }

                foreach (SFMapLake lake in affected_lakes)
                {
                    tmp_lake.cells.UnionWith(lake.cells);
                    map.lake_manager.RemoveLake(lake);
                }
                tmp_lake.RecalculateBoundary();
                tmp_lake.start = new_start;
                tmp_lake.z_diff = base_depth;
                PrepareLakeBoundary(tmp_lake);
                map.lake_manager.AddLake(new_start, tmp_lake.z_diff, lake_type);

            }
            else if(edit_mode == EditorMode.REMOVING)
            {
                foreach(SFMapLake lake in affected_lakes)
                {
                    HashSet<SFCoord> lake_overlap = new HashSet<SFCoord>(lake.cells);
                    HashSet<SFCoord> new_lake_area = new HashSet<SFCoord>(lake.cells);
                    // calculate overlap area of lake and selection
                    lake_overlap.IntersectWith(tmp_lake.cells);
                    if (lake_overlap.Count == 0)
                        continue;

                    // calculate current lowest point of the lake
                    short lowest_level = map.heightmap.height_data[lake.start.y * map.width + lake.start.x];
                    SFCoord start = lake.start;

                    // calculate area of lake without overlapping region, if its 0, remove lake
                    new_lake_area.ExceptWith(lake_overlap);

                    // modify overlap area to be above current lake level
                    foreach (SFCoord p in lake_overlap)
                    {
                        map.heightmap.OverlayRemove("LakeTile", p);
                        map.heightmap.height_data[p.y * map.width + p.x] = (short)(lowest_level + lake.z_diff + 1);
                    }

                    int new_lake_type = lake.type;
                    short lake_zdiff = lake.z_diff;

                    map.lake_manager.RemoveLake(lake);

                    if (new_lake_area.Count == 0)
                        continue;

                    // if new area consists of several islands, so be it
                    List<HashSet<SFCoord>> new_lakes = map.heightmap.GetSeparateIslands(new_lake_area);
                    foreach (HashSet<SFCoord> new_lake in new_lakes)
                    {
                        // find new lowest point of the new lake
                        short new_lowest_level = 32767;
                        SFCoord new_start = new SFCoord(0, 0);
                        foreach (SFCoord p in new_lake)
                        {
                            if (map.heightmap.height_data[p.y * map.width + p.x] < new_lowest_level)
                            {
                                new_start = p;
                                new_lowest_level = map.heightmap.height_data[p.y * map.width + p.x];
                            }
                        }

                        map.lake_manager.AddLake(new_start, (short)(lake_zdiff - (new_lowest_level - lowest_level)), new_lake_type);
                    }
                }
            }

            MainForm.mapedittool.update_render = true;
            edit_mode = EditorMode.IDLE;
            selection.Clear();
            map.heightmap.OverlayClear("ManualLakeTile");
            map.heightmap.RebuildGeometry(topleft, bottomright);
            map.heightmap.RebuildOverlay(topleft, bottomright, "LakeTile");
        }
    }
}
