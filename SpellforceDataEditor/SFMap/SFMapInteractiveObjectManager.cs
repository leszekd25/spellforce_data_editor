using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapInteractiveObject
    {
        static int max_id = 0;

        public SFCoord grid_position = new SFCoord(0, 0);
        public int id = -1;
        public int game_id = -1;
        public int angle = 0;
        public int unk_byte = 0;

        public string GetObjectName()
        {
            return "INT_OBJECT_" + id.ToString();
        }

        public SFMapInteractiveObject()
        {
            id = max_id;
            max_id += 1;
        }

        public override string ToString()
        {
            return GetObjectName();
        }
    }

    public class SFMapInteractiveObjectManager
    {
        public List<SFMapInteractiveObject> int_objects { get; private set; } = new List<SFMapInteractiveObject>();
        public SFMap map = null;

        public SFMapInteractiveObject AddInteractiveObject(int id, SFCoord position, int angle, int unk_byte)
        {
            SFMapInteractiveObject obj = new SFMapInteractiveObject();
            obj.grid_position = position;
            obj.game_id = id;
            obj.angle = angle;
            obj.unk_byte = unk_byte;
            int_objects.Add(obj);

            string obj_name = obj.GetObjectName();
            SF3D.SFRender.SFRenderEngine.scene_manager.AddObjectObject(id, obj_name);
            return obj;
        }

        public void RemoveInteractiveObject(SFMapInteractiveObject int_obj)
        {
            int_objects.Remove(int_obj);

            SF3D.SFRender.SFRenderEngine.scene_manager.DeleteObject(int_obj.GetObjectName());

            map.heightmap.GetChunk(int_obj.grid_position).RemoveInteractiveObject(int_obj);
        }
    }
}
