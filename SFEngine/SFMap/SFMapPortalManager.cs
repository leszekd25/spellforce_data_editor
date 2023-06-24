using System.Collections.Generic;

namespace SFEngine.SFMap
{
    public class SFMapPortal : SFMapEntity
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

        public int AddPortal(int id, SFCoord position, int angle, int index = -1)
        {
            map.object_manager.AddObjectCollisionBoundary(778);

            SFMapPortal ptl = new SFMapPortal();
            ptl.grid_position = position;
            ptl.game_id = id;
            ptl.angle = angle;

            if (index == -1)
            {
                index = portals.Count;
            }

            portals.Insert(index, ptl);

            string ptl_name = ptl.GetName();

            ptl.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(778, ptl_name, true, true);
            ptl.node.SetParent(map.heightmap.GetChunkNode(position));

            map.heightmap.SetFlag(ptl.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, true);
            map.object_manager.ApplyObjectBlockFlags(ptl.grid_position, ptl.angle, 778, true);

            ptl.node.Position = map.heightmap.GetFixedPosition(position);
            ptl.node.Scale = new OpenTK.Vector3(100 / 128f);
            ptl.node.SetAnglePlane(angle);
            map.UpdateNodeDecal(ptl.node, new OpenTK.Vector2(position.x, position.y), OpenTK.Vector2.Zero, angle);

            map.heightmap.GetChunk(position).portals.Add(ptl);
            return index;
        }

        public void RemovePortal(int portal_index)
        {
            SFMapPortal portal = portals[portal_index];
            portals.Remove(portal);

            if (portal.node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(portal.node);
            }

            portal.node = null;

            map.heightmap.GetChunk(portal.grid_position).portals.Remove(portal);

            map.heightmap.SetFlag(portal.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, false);
            map.object_manager.ApplyObjectBlockFlags(portal.grid_position, portal.angle, 778, false);
        }

        public void MovePortal(int portal_map_index, SFCoord new_pos)
        {
            SFMapPortal portal = portals[portal_map_index];

            // move unit and set chunk dependency
            map.heightmap.GetChunkNode(portal.grid_position).MapChunk.portals.Remove(portal);
            map.object_manager.ApplyObjectBlockFlags(portal.grid_position, portal.angle, 778, false);
            map.heightmap.SetFlag(portal.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, false);
            portal.grid_position = new_pos;
            map.heightmap.SetFlag(portal.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, true);
            map.object_manager.ApplyObjectBlockFlags(portal.grid_position, portal.angle, 778, true);
            map.heightmap.GetChunkNode(portal.grid_position).MapChunk.portals.Add(portal);
            portal.node.SetParent(map.heightmap.GetChunkNode(portal.grid_position));

            // change visual transform
            portal.node.Position = map.heightmap.GetFixedPosition(new_pos);
            map.UpdateNodeDecal(portal.node, new OpenTK.Vector2(portal.grid_position.x, portal.grid_position.y), OpenTK.Vector2.Zero, portal.angle);
        }

        // todo: probably account for offset?
        public void RotatePortal(int portal_map_index, int angle)
        {
            SFMapPortal portal = portals[portal_map_index];

            map.object_manager.ApplyObjectBlockFlags(portal.grid_position, portal.angle, 778, false);
            portal.angle = angle;
            map.object_manager.ApplyObjectBlockFlags(portal.grid_position, portal.angle, 778, true);

            portal.node.SetAnglePlane(angle);
            map.UpdateNodeDecal(portal.node, new OpenTK.Vector2(portal.grid_position.x, portal.grid_position.y), OpenTK.Vector2.Zero, portal.angle);
        }
    }
}
