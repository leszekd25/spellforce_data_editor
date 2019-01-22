using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class SFMapUnitManager
    {
        public List<SFMapUnit> units { get; private set; } = new List<SFMapUnit>();
        public SF3D.SceneSynchro.SFSceneManager scene { get; private set; } = null;

        public void AssignScene(SF3D.SceneSynchro.SFSceneManager s)
        {
            scene = s;
        }

        public SFMapUnit AddUnit(int id, SFCoord position)
        {
            SFMapUnit unit = new SFMapUnit();
            unit.grid_position = position;
            unit.game_id = id;

            string obj_name = unit.GetObjectName();
            scene.AddObjectUnit(id, obj_name, false);
            return unit;
        }
    }
}
