﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapPortal: SFMapEntity
    {
        static int max_id = 0;

        public override string GetName()
        {
            return "PORTAL_" + id.ToString();
        }

        public SFMapPortal()
        {
            id = max_id;
            max_id += 1;
        }
    }

    public class SFMapPortalManager
    {
        public List<SFMapPortal> portals { get; private set; } = new List<SFMapPortal>();
        public SFMap map = null;

        public SFMapPortal AddPortal(int id, SFCoord position, int angle, int index)
        {
            SFMapPortal ptl = new SFMapPortal();
            ptl.grid_position = position;
            ptl.game_id = id;
            ptl.angle = angle;

            if (index == -1)
                index = portals.Count;
            portals.Insert(index, ptl);

            string ptl_name = ptl.GetName();

            ptl.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(778, ptl_name, true, true, true);
            ptl.node.SetParent(map.heightmap.GetChunkNode(position));
            return ptl;
        }

        public void RemovePortal(SFMapPortal portal)
        {
            portals.Remove(portal);
            
            if (portal.node != null)
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(portal.node);
            portal.node = null;

            map.heightmap.GetChunk(portal.grid_position).RemovePortal(portal);
        }
    }
}
