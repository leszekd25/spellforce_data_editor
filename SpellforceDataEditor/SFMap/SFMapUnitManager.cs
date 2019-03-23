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

        public SFMapUnit AddUnit(int id, SFCoord position)
        {
            SFMapUnit unit = new SFMapUnit();
            unit.grid_position = position;
            unit.game_id = id;

            string obj_name = unit.GetObjectName();
            map.render_engine.scene_manager.AddObjectUnit(id, obj_name, false);
            return unit;
        }
    }
}
