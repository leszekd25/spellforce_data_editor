/*
 * ScriptPlatform is a container which stores ScriptGenerator objects
 * ScriptPlatformHeader contains data about the platform and script count
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpellforceDataEditor.SFLua.lua_filesystem
{
    public struct ScriptPlatformHeader
    {
        public string name;
        public bool is_npc_platform;
        public int scripts_count;
        public long scripts_total_size;

        //loads header from a stream
        static public ScriptPlatformHeader Load(BinaryReader br)
        {
            System.Diagnostics.Debug.WriteLine("ScriptPlatformHeader.Load()");
            ScriptPlatformHeader h = new ScriptPlatformHeader();
            h.name = br.ReadString();
            h.is_npc_platform = br.ReadBoolean();
            h.scripts_count = br.ReadInt32();
            h.scripts_total_size = br.ReadInt64();
            System.Diagnostics.Debug.WriteLine(h.ToString());
            return h;
        }

        //saves header to a stream
        public void Save(BinaryWriter br)
        {
            System.Diagnostics.Debug.WriteLine("ScriptPlatformHeader.Save()");
            br.Write(name);
            br.Write(is_npc_platform);
            br.Write(scripts_count);
            br.Write(scripts_total_size);
            System.Diagnostics.Debug.WriteLine(ToString());
        }

        public override string ToString()
        {
            return name + ": " + ((is_npc_platform) ? "TRUE" : "FALSE")+"; "+scripts_count.ToString()+" scripts";
        }
    }

    public class ScriptPlatform
    {
        public string Name { get; set; } = "sample_platform";
        public bool is_npc_platform = true;
        public Dictionary<string, ScriptGenerator> Scripts { get; } = new Dictionary<string, ScriptGenerator>();

        //loads platform from a stream
        public int Load(BinaryReader br)
        {
            ScriptPlatformHeader platHeader = ScriptPlatformHeader.Load(br);
            Name = platHeader.name;
            is_npc_platform = platHeader.is_npc_platform;

            Scripts.Clear();
            for(int i = 0; i < platHeader.scripts_count; i++)
            {
                ScriptGenerator script = new ScriptGenerator();
                script.Platform = this;
                int result = script.Load(br);
                if(result != 0)
                {
                    Scripts.Clear();
                    return result;
                }
                Scripts.Add(script.Name, script);
            }

            return 0;
        }

        //saves platform to a stream
        public int Save(BinaryWriter br)
        {
            ScriptPlatformHeader platHeader = new ScriptPlatformHeader();

            platHeader.name = Name;
            platHeader.is_npc_platform = is_npc_platform;
            platHeader.scripts_count = Scripts.Count;
            platHeader.scripts_total_size = 0;    //calculated on the fly
            platHeader.Save(br);

            long scripts_ts_position = br.BaseStream.Position - sizeof(long);   //return here later
            
            List<string> names = Scripts.Keys.ToList();
            names.Sort();
            for (int i = 0; i < Scripts.Count; i++)
            {
                int result = Scripts[names[i]].Save(br);
                if (result != 0)
                    return result;
            }

            platHeader.scripts_total_size = (br.BaseStream.Position - scripts_ts_position) - sizeof(long);
            br.BaseStream.Seek(scripts_ts_position, SeekOrigin.Begin);
            br.Write(platHeader.scripts_total_size);
            br.BaseStream.Seek(0, SeekOrigin.End);

            return 0;
        }

        //creates a new script
        public int ScriptCreate(string name)
        {
            //check if platform exists
            if (ScriptExists(name))
                return -1;

            //create new platform
            ScriptGenerator script = new ScriptGenerator();
            script.Platform = this;
            script.Name = name;
            script.SType = ScriptType.SCRIPT_PLATFORM;
            script.code_nodes.Add(ScriptNode.CreateEmpty());
            Scripts.Add(name, script);

            return 0;
        }

        //see ScriptProject for similar functions...
        public bool ScriptExists(string name)
        {
            return Scripts.ContainsKey(name);
        }

        public List<string> ScriptGetNames()
        {
            return Scripts.Keys.ToList();
        }

        public ScriptGenerator ScriptGet(string name)
        {
            if (!ScriptExists(name))
                return null;
            return Scripts[name];
        }

        public void ScriptRename(string old_name, string new_name)
        {
            ScriptGenerator script = ScriptGet(old_name);
            if (script == null)
                return;
            if (ScriptExists(new_name))
                return;

            Scripts.Remove(old_name);
            Scripts[new_name] = script;
            Scripts[new_name].Name = new_name;
        }
    }
}
