using SFEngine.SFMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapLakesEditor : MapEditor
    {
        public enum EditMode { FLOOD = 0, SELECT = 1 }

        SFMapLake selected_lake = null;
        public EditMode edit_mode = EditMode.FLOOD;
        bool first_click = false;

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

            if (button == MouseButtons.Left)
            {
                if(first_click)
                {
                    return;
                }
                first_click = true;
                if (edit_mode == EditMode.FLOOD)
                {
                    ushort lake_level = map.heightmap.GetZ(pos);

                    List<SFMapLake> consumed_lakes = new List<SFMapLake>();
                    List<int> consumed_lakes_indices = new List<int>();
                    SFMapLake new_lake = map.lake_manager.AddLake(pos, lake_level, 0, -1, consumed_lakes, consumed_lakes_indices);
                    if(new_lake == null)
                    {
                        return;
                    }
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
                        MainForm.mapedittool.ui.RedrawMinimapLake(l, false, false);

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
                    MainForm.mapedittool.op_queue.Push(op_lake);
                    MainForm.mapedittool.op_queue.CloseCluster();

                    map.heightmap.RefreshOverlay();
                    MainForm.mapedittool.ui.RedrawMinimapLake(new_lake, true, true);
                }
                else
                {
                    int lake_index = map.lake_manager.GetLakeIndexAt(pos);
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
                if (first_click)
                {
                    return;
                }
                first_click = true;
                if (edit_mode == EditMode.FLOOD)
                {
                    int lake_index = map.lake_manager.GetLakeIndexAt(pos);
                    if(lake_index != SFEngine.Utility.NO_INDEX)
                    {
                        SFMapLake lake = map.lake_manager.lakes[lake_index];
                        map_operators.MapOperatorLake op_lake2 = new map_operators.MapOperatorLake()
                        {
                            pos = lake.start,
                            z_diff = lake.z_diff,
                            type = lake.type,
                            lake_index = lake_index,
                            change_add = false,
                            ApplyOnPush = true,
                        };
                        MainForm.mapedittool.op_queue.Push(op_lake2);
                    }
                }
                else
                {
                    SelectLake(null);
                }
            }

            MainForm.mapedittool.update_render = true;
        }

        public override void OnMouseUp(MouseButtons b)
        {
            first_click = false;
        }
    }
}
