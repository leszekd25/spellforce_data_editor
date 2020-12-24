using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            MainForm.mapedittool.op_queue.OpenCluster();
            if (button == MouseButtons.Left)
            {
                if (lake_index == Utility.NO_INDEX)
                {
                    if (map.lake_manager.AddLake(pos, 0, 0) != null)
                    {
                        SelectLake(map.lake_manager.lakes[map.lake_manager.lakes.Count - 1]);
                        MainForm.mapedittool.ui.RedrawMinimap(map.lake_manager.lakes[map.lake_manager.lakes.Count - 1].cells);
                    }
                }
                else
                    SelectLake(map.lake_manager.lakes[lake_index]);
            }
            else if (button == MouseButtons.Right)
            {
                if (lake_index != Utility.NO_INDEX)
                {
                    if (map.lake_manager.lakes[lake_index] == selected_lake)
                        SelectLake(null);
                    HashSet<SFCoord> tmp_cells = map.lake_manager.lakes[lake_index].cells;
                    map.lake_manager.RemoveLake(map.lake_manager.lakes[lake_index]);
                    MainForm.mapedittool.ui.RedrawMinimap(tmp_cells);
                }
                else
                    SelectLake(null);
            }
            MainForm.mapedittool.op_queue.CloseCluster();

            MainForm.mapedittool.update_render = true;
        }
    }
}
