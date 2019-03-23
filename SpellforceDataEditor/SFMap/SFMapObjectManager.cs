using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapObject
    {
        static int max_id = 0;

        public SFCoord grid_position = new SFCoord(0, 0);
        public int id = -1;
        public int game_id = -1;
        public int angle = 0;

        public string GetObjectName()
        {
            return "OBJECT_" + id.ToString();
        }

        public SFMapObject()
        {
            id = max_id;
            max_id += 1;
        }

        public override string ToString()
        {
            return GetObjectName();
        }
    }

    public class SFMapObjectManager
    {
        public List<SFMapObject> units { get; private set; } = new List<SFMapObject>();
        public SFMap map = null;

        public SFMapObject AddObject(int id, SFCoord position, int angle)
        {
            SFMapObject obj = new SFMapObject();
            obj.grid_position = position;
            obj.game_id = id;
            obj.angle = angle;

            string obj_name = obj.GetObjectName();
            map.render_engine.scene_manager.AddObjectObject(id, obj_name);
            return obj;
        }
    }
}
