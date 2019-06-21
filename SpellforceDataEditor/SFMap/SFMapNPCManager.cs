using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public enum NPCType { UNIT = 0, BUILDING, OBJECT }

    public struct SFMapNPCInfo
    {
        public NPCType npc_type;
        public object npc_ref;    // must be SFMapUnit, SFMapBuilding or SFMapObject

        public SFMapNPCInfo( object _ref)
        {
            npc_ref = _ref;
            if (_ref.GetType() == typeof(SFMapUnit))
                npc_type = NPCType.UNIT;
            else if (_ref.GetType() == typeof(SFMapBuilding))
                npc_type = NPCType.BUILDING;
            else if (_ref.GetType() == typeof(SFMapObject))
                npc_type = NPCType.OBJECT;
            else
                throw new ArgumentException("SFMapNPCInfo(): Invalid object type!");
        }
    }

    public class SFMapNPCManager
    {
        public Dictionary<int, SFMapNPCInfo> npc_info { get; private set; } = new Dictionary<int, SFMapNPCInfo>();
        public SFMap map = null;

        public int AddNPCRef(int npc_id, object npc_ref)
        {
            if (npc_info.ContainsKey(npc_id))
                return -1;

            npc_info.Add(npc_id, new SFMapNPCInfo(npc_ref));
            return 0;
        }

        public int RemoveNPCRef(int npc_id)
        {
            if (!npc_info.ContainsKey(npc_id))
                return -1;

            npc_info.Remove(npc_id);
            return 0;
        }
    }
}
