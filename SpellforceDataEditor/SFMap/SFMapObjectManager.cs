﻿using System;
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
        public int npc_id = 0;
        public int unknown1 = 0;

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
        public List<SFMapObject> objects { get; private set; } = new List<SFMapObject>();
        public SFMap map = null;

        public SFMapObject AddObject(int id, SFCoord position, int angle, int unk1)
        {
            SFMapObject obj = new SFMapObject();
            obj.grid_position = position;
            obj.game_id = id;
            obj.angle = angle;
            obj.unknown1 = unk1;
            objects.Add(obj);

            string obj_name = obj.GetObjectName();
            map.render_engine.scene_manager.AddObjectObject(id, obj_name);
            return obj;
        }

        public bool ObjectIDIsReserved(int obj_id)
        {
            if ((obj_id == 65) || (obj_id == 66) || (obj_id == 67))   // editor flags (ignored)
                return true;
            if ((obj_id == 769)  || (obj_id == 778))                  // bindstone, world portal
                return true;
            if ((obj_id >= 771) && (obj_id <= 777))                   // monuments
                return true;
            if (obj_id == 2541)                                       // spawn point
                return true;
            return false;
        }
    }
}
