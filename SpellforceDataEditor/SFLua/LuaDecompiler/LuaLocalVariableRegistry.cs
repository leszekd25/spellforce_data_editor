using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFLua.LuaDecompiler
{
    public class LuaLocalVariableRegistry
    {
        public Dictionary<int, string> registry = new Dictionary<int, string>();
        public Dictionary<int, int> last_updated = new Dictionary<int, int>();

        // locals can be registered once, otherwise exception is thrown
        public void RegisterLocal(int local_id, string local_name)
        {
            if (!registry.ContainsKey(local_id))
                registry.Add(local_id, local_name);
            else
                throw new InvalidOperationException("Could not register local id " + local_id.ToString() + ": Local already registered!");
        }

        // does nothing if local was not registered
        public void UnregisterLocal(int local_id)
        {
            if (registry.ContainsKey(local_id))
                registry.Remove(local_id);
        }

        // throws exception if local is not registered
        public string GetLocal(int local_id)
        {
            if (registry.ContainsKey(local_id))
                return registry[local_id];

            else
                throw new InvalidOperationException("Could not get local id " + local_id.ToString() + ": Local not registered!");
        }

        public bool IsLocalRegistered(int local_id)
        {
            return registry.ContainsKey(local_id);
        }

        public void UpdateLocal(int local_id, int instr_id)
        {
            if (!registry.ContainsKey(local_id))
            {
                if (last_updated.ContainsKey(local_id))
                    last_updated[local_id] = instr_id;
                else
                    last_updated.Add(local_id, instr_id);
            }
        }

        public int GetLastUpdated(int local_id)
        {
            if(!registry.ContainsKey(local_id))
            {
                if (last_updated.ContainsKey(local_id))
                    return last_updated[local_id];
                else
                    return Utility.NO_INDEX;
            }
            else
                throw new InvalidOperationException("Could not get last update of local id " + local_id.ToString() + ": Local already registered!");
        }
    }
}
