using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspectorFlagControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        public MapInspectorFlagControl()
        {
            InitializeComponent();
        }

        private void MapInspectorFlagControl_VisibleChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (Visible == false)
            {
                map.heightmap.OverlayClear("TileMovementBlock");
                map.heightmap.OverlaySetVisible("TileMovementBlock", false);

                map.heightmap.OverlayClear("ManualMovementBlock");
                map.heightmap.OverlaySetVisible("ManualMovementBlock", false);

                map.heightmap.OverlayClear("ManualVisionBlock");
                map.heightmap.OverlaySetVisible("ManualVisionBlock", false);
                foreach (SFMapHeightMapChunk chunk in map.heightmap.chunks)
                {
                    chunk.OverlayUpdate("TileMovementBlock");
                    chunk.OverlayUpdate("ManualMovementBlock");
                    chunk.OverlayUpdate("ManualVisionBlock");
                }
            }
            else
            {
                for (int i = 0; i < map.height; i++)
                    for (int j = 0; j < map.width; j++)
                        if (map.heightmap.texture_manager.texture_tiledata[map.heightmap.tile_data[i * map.width + j]].blocks_movement)
                            map.heightmap.OverlayAdd("TileMovementBlock", new SFCoord(j, i));

                foreach (SFCoord p in map.heightmap.chunk42_data)
                    map.heightmap.OverlayAdd("ManualMovementBlock", p);

                foreach (SFCoord p in map.heightmap.chunk56_data)
                    map.heightmap.OverlayAdd("ManualVisionBlock", p);

                if (CheckMovement.Checked)
                {
                    map.heightmap.OverlaySetVisible("TileMovementBlock", true);
                    map.heightmap.OverlaySetVisible("ManualMovementBlock", true);
                    foreach (SFMapHeightMapChunk chunk in map.heightmap.chunks)
                    {
                        chunk.OverlayUpdate("TileMovementBlock");
                        chunk.OverlayUpdate("ManualMovementBlock");
                    }
                }
                if (CheckVision.Checked)
                {
                    map.heightmap.OverlaySetVisible("ManualVisionBlock", true);
                    foreach (SFMapHeightMapChunk chunk in map.heightmap.chunks)
                        chunk.OverlayUpdate("ManualVisionBlock");
                }
            }
            MainForm.mapedittool.update_render = true;
        }

        private void CheckMovement_CheckedChanged(object sender, EventArgs e)
        {
            map.heightmap.OverlaySetVisible("TileMovementBlock", CheckMovement.Checked);
            map.heightmap.OverlaySetVisible("ManualMovementBlock", CheckMovement.Checked);
            if(CheckMovement.Checked)
                foreach (SFMapHeightMapChunk chunk in map.heightmap.chunks)
                {
                    chunk.OverlayUpdate("TileMovementBlock");
                    chunk.OverlayUpdate("ManualMovementBlock");
                }

            MainForm.mapedittool.update_render = true;
        }

        private void CheckVision_CheckedChanged(object sender, EventArgs e)
        {
            map.heightmap.OverlaySetVisible("ManualVisionBlock", CheckVision.Checked);
            if(CheckVision.Checked)
                foreach (SFMapHeightMapChunk chunk in map.heightmap.chunks)
                    chunk.OverlayUpdate("ManualVisionBlock");

            MainForm.mapedittool.update_render = true;
        }

        public override void OnMouseDown(SFCoord clicked_pos, MouseButtons button)
        {
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

            List<SFCoord> data_list;
            string o_name;
            if (ComboFlagMode.SelectedIndex == 0)
            {
                data_list = map.heightmap.chunk42_data;
                o_name = "ManualMovementBlock";
            }
            else
            {
                data_list = map.heightmap.chunk56_data;
                o_name = "ManualVisionBlock";
            }

            if (button == MouseButtons.Left)
            {
                for (int i = topleft.x; i <= bottomright.x; i++)
                {
                    for (int j = topleft.y; j <= bottomright.y; j++)
                    {
                        SFCoord p = new SFCoord(i, j);
                        if (BrushControl.brush.GetStrengthAt(p) == 0)
                            continue;
                        if (!data_list.Contains(p))
                        {
                            data_list.Add(p);
                            map.heightmap.OverlayAdd(o_name, new SFCoord(p.x, map.height - p.y - 1));
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
                        if (BrushControl.brush.GetStrengthAt(p) == 0)
                            continue;
                        if (data_list.Contains(p))
                        {
                            data_list.Remove(p);
                            map.heightmap.OverlayRemove(o_name, new SFCoord(p.x, map.height - p.y - 1));
                        }
                    }
                }
            }

            map.heightmap.RebuildOverlay(topleft, bottomright, o_name);
        }
    }
}
