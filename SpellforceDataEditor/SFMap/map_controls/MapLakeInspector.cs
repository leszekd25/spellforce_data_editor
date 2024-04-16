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

                    selection_helper.SelectLake(selected_lake);
                }
                else
                {
                    Enabled = false;
                }
            }
            if (!Enabled)
            {
                selection_helper.CancelSelection();
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
    }
}
