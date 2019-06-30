using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapPortal
    {
        static int max_id = 0;

        public SFCoord grid_position = new SFCoord(0, 0);
        public int id = -1;
        public int game_id = -1;
        public int angle = 0;

        public string GetObjectName()
        {
            return "PORTAL_" + id.ToString();
        }

        public SFMapPortal()
        {
            id = max_id;
            max_id += 1;
        }

        public override string ToString()
        {
            return GetObjectName();
        }
    }

    public class SFMapPortalManager
    {
        public List<SFMapPortal> portals { get; private set; } = new List<SFMapPortal>();
        public SFMap map = null;

        public SFMapPortal AddPortal(int id, SFCoord position, int angle)
        {
            SFMapPortal ptl = new SFMapPortal();
            ptl.grid_position = position;
            ptl.game_id = id;
            ptl.angle = angle;
            portals.Add(ptl);

            string ptl_name = ptl.GetObjectName();
            SF3D.SFRender.SFRenderEngine.scene_manager.AddObjectObject(778, ptl_name);   // portal id
            return ptl;
        }

        public void RemovePortal(SFMapPortal portal)
        {
            portals.Remove(portal);

            SF3D.SFRender.SFRenderEngine.scene_manager.DeleteObject(portal.GetObjectName());

            map.heightmap.GetChunk(portal.grid_position).RemovePortal(portal);
        }
    }
}
