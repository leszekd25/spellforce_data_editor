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
    public partial class MapInspectorLakeControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        SFMapLake selected_lake = null;

        public MapInspectorLakeControl()
        {
            InitializeComponent();
        }

        private void SelectLake(SFMapLake lake)
        {
            selected_lake = lake;
            if (selected_lake == null)
                SelectedLakePanel.Enabled = false;
            else
            {
                SelectedLakePanel.Enabled = true;
                SelectedLakeInternalDepth.Text = selected_lake.z_diff.ToString();
                SelectedLakeType.SelectedIndex = selected_lake.type;
            }
        }
        
        public override void OnMouseDown(SFCoord clicked_pos, MouseButtons button)
        {
            if (map == null)
                return;
            SFCoord fixed_pos = new SFCoord(clicked_pos.x, map.height - clicked_pos.y - 1);
            byte lake_index = map.heightmap.lake_data[fixed_pos.y * map.width + fixed_pos.x];

            if (button == MouseButtons.Left)
            {
                if (lake_index == 0)
                {
                    if(map.lake_manager.AddLake(fixed_pos, 0, 0) != null);
                    SelectLake(map.lake_manager.lakes[map.lake_manager.lakes.Count - 1]);
                }
                else
                    SelectLake(map.lake_manager.lakes[lake_index - 1]);
            }
            else if(button == MouseButtons.Right)
            {
                if(lake_index != 0)
                {
                    if (map.lake_manager.lakes[lake_index - 1] == selected_lake)
                        SelectLake(null);
                    map.lake_manager.RemoveLake(map.lake_manager.lakes[lake_index - 1]);

                }
                else
                    SelectLake(null);
            }
            
            MainForm.mapedittool.update_render = true;
        }

        private void SelectedLakeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            selected_lake.type = SelectedLakeType.SelectedIndex;
            map.lake_manager.UpdateLake(selected_lake);
        }
    }
}
