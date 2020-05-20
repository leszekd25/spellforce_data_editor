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
        public int unknown_flags = 0;
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

        public SFMapUnit AddUnit(int id, SFCoord position, int flags)
        {
            SFMapUnit unit = new SFMapUnit();
            unit.grid_position = position;
            unit.unknown_flags = flags;
            unit.game_id = id;
            units.Add(unit);

            string obj_name = unit.GetObjectName();
            SF3D.SceneSynchro.SceneNode node = SF3D.SFRender.SFRenderEngine.scene.AddSceneUnit(id, obj_name);
            node.SetParent(map.heightmap.GetChunkNode(position));
            
            // 3. add new unit in respective chunk
            map.heightmap.GetChunk(position).AddUnit(unit);

            return unit;
        }

        public void RemoveUnit(SFMapUnit u)
        {
            units.Remove(u);
            
            SF3D.SceneSynchro.SceneNode chunk_node = map.heightmap.GetChunkNode(u.grid_position);
            SF3D.SceneSynchro.SceneNode unit_node = chunk_node.FindNode<SF3D.SceneSynchro.SceneNode>(u.GetObjectName());
            if (unit_node != null)
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(unit_node);

            map.heightmap.GetChunk(u.grid_position).RemoveUnit(u);
        }

        public int GetHighestGroup()
        {
            int m_group = 0;
            for (int i = 0; i < units.Count; i++)
                m_group = Math.Max(m_group, units[i].group);
            return m_group;
        }
    }
}
