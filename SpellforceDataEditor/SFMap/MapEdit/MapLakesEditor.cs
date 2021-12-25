using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFMap;
using SFEngine.SFCFF;
using SFEngine.SFLua;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapLakesEditor: MapEditor
    {
        SFMapLake selected_lake = null;

        private void SelectLake(SFMapLake lake)
        {
            selected_lake = lake;
            MainForm.mapedittool.InspectorSelect(lake);
        }

        public override void OnMousePress(SFCoord pos, MouseButtons button, ref special_forms.SpecialKeysPressed specials)
        {
            if (map == null)
                return;
            int lake_index = map.lake_manager.GetLakeIndexAt(pos);

            if (button == MouseButtons.Left)
            {
                if (lake_index == SFEngine.Utility.NO_INDEX)
                {

                    List<SFMapLake> consumed_lakes = new List<SFMapLake>();
                    List<int> consumed_lakes_indices = new List<int>();
                    
                    MainForm.mapedittool.op_queue.OpenCluster();
                    SFMapLake new_lake = map.lake_manager.AddLake(pos, 0, 0, -1, consumed_lakes, consumed_lakes_indices);

                    if (new_lake != null)
                    {
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

                        int new_lake_index = map.lake_manager.lakes.Count - 1;

                        map_operators.MapOperatorLake op_lake = new map_operators.MapOperatorLake() 
                        {
                            pos = new_lake.start,
                            z_diff = new_lake.z_diff,
                            type = new_lake.type,
                            lake_index = new_lake_index,
                            change_add = true
                        };
                        MainForm.mapedittool.op_queue.Push(op_lake);

                        SelectLake(map.lake_manager.lakes[new_lake_index]);
                        MainForm.mapedittool.ui.RedrawMinimap(map.lake_manager.lakes[new_lake_index].cells);
                    }

                    MainForm.mapedittool.op_queue.CloseCluster();
                }
                else
                    SelectLake(map.lake_manager.lakes[lake_index]);
            }
            else if (button == MouseButtons.Right)
            {
                if (lake_index != SFEngine.Utility.NO_INDEX)
                {
                    if (map.lake_manager.lakes[lake_index] == selected_lake)
                        SelectLake(null);
                    HashSet<SFCoord> tmp_cells = map.lake_manager.lakes[lake_index].cells;

                    SFMapLake lake = map.lake_manager.lakes[lake_index];
                    map_operators.MapOperatorLake op_lake = new map_operators.MapOperatorLake()
                    {
                        pos = lake.start,
                        z_diff = lake.z_diff,
                        type = lake.type,
                        lake_index = lake_index,
                        change_add = false
                    };
                    MainForm.mapedittool.op_queue.Push(op_lake);

                    map.lake_manager.RemoveLake(map.lake_manager.lakes[lake_index]);
                    MainForm.mapedittool.ui.RedrawMinimap(tmp_cells);
                }
                else
                    SelectLake(null);
            }

            MainForm.mapedittool.update_render = true;
        }
    }
}
