using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspectorHeightMapControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        public MapInspectorHeightMapControl()
        {
            InitializeComponent();
        }

        public override void OnMouseDown(SFMap map, SFCoord clicked_pos)
        {
            float strength = Utility.TryParseFloat(StrengthTextBox.Text, 0f);
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

            switch(ComboDrawMode.SelectedIndex)
            {
                case 0:  // add
                    for (int i = topleft.x; i <= bottomright.x; i++)
                    {
                        for (int j = topleft.y; j <= bottomright.y; j++)
                        {
                            float cell_strength = BrushControl.brush.GetStrengthAt(new SFCoord(i, j));
                            int fixed_j = map.height - j - 1;
                            map.heightmap.height_data[fixed_j * map.width + i] += (short)(strength * cell_strength);
                            if (map.heightmap.height_data[fixed_j * map.width + i] > 30000)
                                map.heightmap.height_data[fixed_j * map.width + i] = 30000;
                        }
                    }
                    break;
                case 1:  // sub
                    for (int i = topleft.x; i <= bottomright.x; i++)
                    {
                        for (int j = topleft.y; j <= bottomright.y; j++)
                        {
                            float cell_strength = BrushControl.brush.GetStrengthAt(new SFCoord(i, j));
                            int fixed_j = map.height - j - 1;
                            map.heightmap.height_data[fixed_j * map.width + i] -= (short)(strength * cell_strength);
                            if (map.heightmap.height_data[fixed_j * map.width + i] < 0)
                                map.heightmap.height_data[fixed_j * map.width + i] = 0;
                        }
                    }
                    break;
                case 2:  // set
                    for (int i = topleft.x; i <= bottomright.x; i++)
                    {
                        for (int j = topleft.y; j <= bottomright.y; j++)
                        {
                            int fixed_j = map.height - j - 1;
                            map.heightmap.height_data[fixed_j * map.width + i] = (short)(strength);
                        }
                    }
                    break;
                default:
                    break;
            }

            map.heightmap.RebuildGeometry(topleft, bottomright);
        }

        private void ComboDrawMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboDrawMode.SelectedIndex == -1)
                return;
            if (ComboDrawMode.SelectedIndex == 2)
                LabelStrength.Text = "Value";
            else
                LabelStrength.Text = "Strength";
        }
    }
}
