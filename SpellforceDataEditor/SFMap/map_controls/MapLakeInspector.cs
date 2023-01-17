using SFEngine.SFMap;
using System;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapLakeInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        public SFMapLake selected_lake { get; private set; } = null;

        public MapLakeInspector()
        {
            InitializeComponent();
        }

        public override void OnSelect(object o)
        {
            if (o == null)
            {
                Enabled = false;
            }
            else
            {
                if (o.GetType() == typeof(SFMapLake))
                {
                    selected_lake = (SFMapLake)o;
                    Enabled = true;
                    if (selected_lake.type == 0)
                    {
                        LakeTypeWater.Checked = true;
                    }
                    else if (selected_lake.type == 1)
                    {
                        LakeTypeSwamp.Checked = true;
                    }
                    else if (selected_lake.type == 2)
                    {
                        LakeTypeLava.Checked = true;
                    }
                    else if (selected_lake.type == 3)
                    {
                        LakeTypeIce.Checked = true;
                    }
                    else
                    {
                        throw new Exception("MapLakeInspector.OnSelect(): Invalid lake type!");
                    }

                    SelectedLakeDepth.Text = selected_lake.z_diff.ToString();
                    SelectedLakeLevel.Text = (selected_lake.z_diff + map.heightmap.GetZ(selected_lake.start)).ToString();

                    map.selection_helper.SelectLake(selected_lake);
                }
                else
                {
                    Enabled = false;
                }
            }
            if (!Enabled)
            {
                map.selection_helper.CancelSelection();
                selected_lake = null;
            }
        }

        private void LakeTypeWater_Click(object sender, EventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            if (selected_lake.type == 0)
            {
                return;
            }

            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorLakeType()
            {
                lake_index = map.lake_manager.GetLakeIndexAt(selected_lake.start),
                PreOperatorType = selected_lake.type,
                PostOperatorType = 0,
                ApplyOnPush = true
            });
        }

        private void LakeTypeSwamp_Click(object sender, EventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            if (selected_lake.type == 1)
            {
                return;
            }

            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorLakeType()
            {
                lake_index = map.lake_manager.GetLakeIndexAt(selected_lake.start),
                PreOperatorType = selected_lake.type,
                PostOperatorType = 1,
                ApplyOnPush = true
            });
        }

        private void LakeTypeLava_Click(object sender, EventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            if (selected_lake.type == 2)
            {
                return;
            }

            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorLakeType()
            {
                lake_index = map.lake_manager.GetLakeIndexAt(selected_lake.start),
                PreOperatorType = selected_lake.type,
                PostOperatorType = 2,
                ApplyOnPush = true
            });
        }

        private void LakeTypeIce_Click(object sender, EventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            if (selected_lake.type == 3)
            {
                return;
            }

            MainForm.mapedittool.op_queue.Push(new map_operators.MapOperatorLakeType()
            {
                lake_index = map.lake_manager.GetLakeIndexAt(selected_lake.start),
                PreOperatorType = selected_lake.type,
                PostOperatorType = 3,
                ApplyOnPush = true
            });
        }

        private void SelectedLakeDepth_Leave(object sender, EventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            short depth = (short)SFEngine.Utility.TryParseUInt16(SelectedLakeDepth.Text, (ushort)selected_lake.z_diff);
            if (depth == selected_lake.z_diff)
            {
                return;
            }

            ushort level = (ushort)(selected_lake.z_diff + map.heightmap.GetZ(selected_lake.start));
            if (level - depth <= 0)
            {
                SelectedLakeDepth.Text = selected_lake.z_diff.ToString();
                return;
            }

            ushort terrain_level = (ushort)(level - depth);

            int selected_lake_index = map.lake_manager.lakes.IndexOf(selected_lake);
            // set terrain
            map_operators.MapOperatorLake op_lake_remove = new map_operators.MapOperatorLake()
            {
                pos = selected_lake.start,
                z_diff = selected_lake.z_diff,
                type = selected_lake.type,
                lake_index = selected_lake_index,
                change_add = false
            };

            map_operators.MapOperatorTerrainHeight op_height = new map_operators.MapOperatorTerrainHeight();
            foreach (SFCoord p in selected_lake.cells)
            {
                op_height.PreOperatorHeights.Add(p, map.heightmap.height_data[p.y * map.width + p.x]);
                op_height.PostOperatorHeights.Add(p, terrain_level);
            }

            map_operators.MapOperatorLake op_lake = new map_operators.MapOperatorLake()
            {
                pos = selected_lake.start,
                z_diff = depth,
                type = selected_lake.type,
                lake_index = map.lake_manager.lakes.Count - 1,
                change_add = true
            };


            OnSelect(null);
            MainForm.mapedittool.op_queue.OpenCluster(true);
            MainForm.mapedittool.op_queue.Push(op_lake_remove);
            MainForm.mapedittool.op_queue.Push(op_height);
            MainForm.mapedittool.op_queue.Push(op_lake);
            MainForm.mapedittool.op_queue.CloseCluster();
            OnSelect(map.lake_manager.lakes[map.lake_manager.lakes.Count - 1]);
        }

        private void SelectedLakeLevel_Leave(object sender, EventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            ushort current_level = (ushort)(selected_lake.z_diff + map.heightmap.GetZ(selected_lake.start));
            ushort new_level = SFEngine.Utility.TryParseUInt16(SelectedLakeLevel.Text, current_level);
            if (new_level == current_level)
            {
                return;
            }

            short level_diff = (short)(current_level - new_level);

            ushort border_terrain_level = (ushort)(new_level + 50);

            int selected_lake_index = map.lake_manager.lakes.IndexOf(selected_lake);
            // set terrain
            map_operators.MapOperatorLake op_lake_remove = new map_operators.MapOperatorLake()
            {
                pos = selected_lake.start,
                z_diff = selected_lake.z_diff,
                type = selected_lake.type,
                lake_index = selected_lake_index,
                change_add = false
            };

            map_operators.MapOperatorTerrainHeight op_height = new map_operators.MapOperatorTerrainHeight();
            foreach (SFCoord p in selected_lake.cells)
            {
                op_height.PreOperatorHeights.Add(p, map.heightmap.height_data[p.y * map.width + p.x]);
                op_height.PostOperatorHeights.Add(p, (ushort)(map.heightmap.height_data[p.y * map.width + p.x] - level_diff));
            }
            foreach (SFCoord p in selected_lake.shore)
            {
                if (map.heightmap.height_data[p.y * map.width + p.x] < border_terrain_level)
                {
                    op_height.PreOperatorHeights.Add(p, map.heightmap.height_data[p.y * map.width + p.x]);
                    op_height.PostOperatorHeights.Add(p, border_terrain_level);
                }
            }

            map_operators.MapOperatorLake op_lake = new map_operators.MapOperatorLake()
            {
                pos = selected_lake.start,
                z_diff = selected_lake.z_diff,
                type = selected_lake.type,
                lake_index = map.lake_manager.lakes.Count - 1,
                change_add = true
            };


            OnSelect(null);
            MainForm.mapedittool.op_queue.OpenCluster(true);
            MainForm.mapedittool.op_queue.Push(op_lake_remove);
            MainForm.mapedittool.op_queue.Push(op_height);
            MainForm.mapedittool.op_queue.Push(op_lake);
            MainForm.mapedittool.op_queue.CloseCluster();
            OnSelect(map.lake_manager.lakes[map.lake_manager.lakes.Count - 1]);
        }
    }
}
