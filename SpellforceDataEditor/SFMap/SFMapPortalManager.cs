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

            //SF3D.SceneSynchro.SceneNode node = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(778, bld_name, true);
            //node.SetParent(map.heightmap.GetChunkNode(position));
            SF3D.SFRender.SFRenderEngine.scene.AddObjectObject(778, ptl_name, true);   // portal id
            return ptl;
        }

        public void RemovePortal(SFMapPortal portal)
        {
            portals.Remove(portal);
            
            //SF3D.SceneSynchro.SceneNode chunk_node = map.heightmap.GetChunkNode(portal.grid_position);
            //SF3D.SceneSynchro.SceneNode ptl_node = chunk_node.FindNode<SF3D.SceneSynchro.SceneNode>(portal.GetObjectName());
            //if (ptl_node != null)
            //    SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(ptl_node);
            SF3D.SFRender.SFRenderEngine.scene.DeleteObject(portal.GetObjectName());

            map.heightmap.GetChunk(portal.grid_position).RemovePortal(portal);
        }
    }
}
