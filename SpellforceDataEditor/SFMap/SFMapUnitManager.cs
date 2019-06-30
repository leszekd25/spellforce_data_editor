using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpellforceDataEditor.SF3D;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapUnit
    {
        static int max_id = 0;

        public SFCoord grid_position = new SFCoord(0, 0);
        public int id = -1;
        public int game_id = -1;
        public int angle = 0;
        public int npc_id = 0;
        public int unknown = 0;
        public int group = 0;
        public int unknown2 = 0;

        public string GetObjectName()
        {
            return "UNIT_" + id.ToString();
        }

        public SFMapUnit()
        {
            id = max_id;
            max_id += 1;
        }

        public override string ToString()
        {
            return GetObjectName();
        }
    }

    public class SFMapUnitManager
    {
        public List<SFMapUnit> units { get; private set; } = new List<SFMapUnit>();
        public SFMap map = null;

        public SFMapUnit AddUnit(int id, SFCoord position, int angle)
        {
            SFMapUnit unit = new SFMapUnit();
            unit.grid_position = position;
            unit.angle = angle;
            unit.game_id = id;
            units.Add(unit);

            string obj_name = unit.GetObjectName();
            SF3D.SFRender.SFRenderEngine.scene_manager.AddObjectUnit(id, obj_name, false);
            
            // 3. add new unit in respective chunk
            map.heightmap.GetChunk(position).AddUnit(unit);
            SF3D.SFRender.SFRenderEngine.scene_manager.objects_static[unit.GetObjectName()].Visible = map.heightmap.GetChunk(position).Visible;

            return unit;
        }

        public void RemoveUnit(SFMapUnit u)
        {
            units.Remove(u);

            SF3D.SFRender.SFRenderEngine.scene_manager.DeleteObject(u.GetObjectName());

            map.heightmap.GetChunk(u.grid_position).RemoveUnit(u);
        }
    }
}
