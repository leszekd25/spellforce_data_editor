/*
 * ScriptGenerator is an object which generates Lua code from Lua block structure
 * Block structure is saved using nodes, and regenerated upon loading a script
 * ScriptGeneratorHeader contains data about nodes which generate block structure
 * ScriptNode is a structure which holds data describing each control block, it is an intermediary
 *     between byte stream and graphical interpretation of control blocks
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using SpellforceDataEditor.SFLua.lua_controls;

namespace SpellforceDataEditor.SFLua.lua_filesystem
{
    //available script types
    public enum ScriptType
    {
        SCRIPT_CAMERA = 0,
        SCRIPT_AI, SCRIPT_RTSSPAWN, SCRIPT_EFFECT, SCRIPT_PLATFORM,
        SCRIPT_NPC, SCRIPT_PATCH, SCRIPT_CUTSCENE, SCRIPT_CUSTOM
    }

    //available node types
    public enum ScriptNodeValueType
    {
        BOOL = 0,
        COLOR, COMPLEX, CUSTOM, DOUBLE,
        ENUM, INT, STRING, GD, PARAMETER,
        HIDDEN_PARAMETER
    }

    public struct ScriptHeader
    {
        public string name;
        public ScriptType type;
        public int node_count;
        public bool has_dialog;
        public int size;

        //saves header to a stream
        public void Save(BinaryWriter br)
        {
            System.Diagnostics.Debug.WriteLine("ScriptHeader.Save()");
            br.Write(name);
            br.Write((int)type);
            br.Write(node_count);
            br.Write(has_dialog);
            br.Write(size);
            System.Diagnostics.Debug.WriteLine(ToString());
        }

        //loads header from a stream
        static public ScriptHeader Load(BinaryReader br)
        {
            System.Diagnostics.Debug.WriteLine("ScriptHeader.Load()");
            ScriptHeader h = new ScriptHeader();
            h.name = br.ReadString();
            h.type = (ScriptType)br.ReadInt32();
            h.node_count = br.ReadInt32();
            h.has_dialog = br.ReadBoolean();
            h.size = br.ReadInt32();
            System.Diagnostics.Debug.WriteLine(h.ToString());
            return h;
        }

        public override string ToString()
        {
            return name + ": " + type.ToString("F") + "; " + node_count.ToString() + " nodes";
        }
    }

    [Serializable]
    public struct ScriptNode
    {
        public int id;
        public int parent_id;
        public string name;
        public ScriptNodeValueType type;
        public LuaParseFlag parse_flags;
        public string pschar;
        public object value;
        public object def;
        public bool important;

        public void Save(BinaryWriter br)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(br.BaseStream, this);
        }

        static public ScriptNode Load(BinaryReader br)
        {
            BinaryFormatter bf = new BinaryFormatter();
            ScriptNode r = (ScriptNode)bf.Deserialize(br.BaseStream);
            return r;
            //return (ScriptNode)bf.Deserialize(br.BaseStream);
        }

        //creates an empty node
        static public ScriptNode CreateEmpty()
        {
            ScriptNode s = new ScriptNode();
            s.id = 0;
            s.parent_id = -1;
            s.name = "root";
            s.type = ScriptNodeValueType.COMPLEX;
            s.parse_flags = 0;
            s.value = null;
            s.def = null;
            s.pschar = "";
            s.important = false;
            return s;
        }

        public override string ToString()
        {
            return "[" + id.ToString() + ": " + parent_id.ToString() + "] " + name + ": " + type.ToString("F");
        }
    }

    public class ScriptGenerator
    {
        public ScriptPlatform Platform { get; set; } = null;
        public ScriptType SType { get; set; } = ScriptType.SCRIPT_NPC;
        public string Name { get; set; } = "sample text";
        public LuaValueComplexControl root_control = null;
        public List<ScriptNode> code_nodes = new List<ScriptNode>();
        public List<LuaValueControl> code_blocks = new List<LuaValueControl>();
        public lua_dialog.LuaDialogManager dialog = new lua_dialog.LuaDialogManager();

        //BuildCode[]() functions are templates for BuildCode() function which generates code
        //      based on script type; [] is replaced with type name
        private string BuildCodeCUSTOM()
        {
            return root_control.ToCode(true);
        }

        private string BuildCodeRTSSPAWN()
        {
            return BuildCodeCUSTOM();
        }

        private string BuildCodeCUTSCENE()
        {
            return BuildCodeCUSTOM();
        }

        private string BuildCodeAI()
        {
            return BuildCodeCUSTOM();
        }

        private string BuildCodeEFFECT()
        {
            return BuildCodeCUSTOM();
        }

        private string BuildCodeCAMERA()
        {
            return BuildCodeCUSTOM();
        }
        //never called when root_control == null
        private string BuildCodeNPC()
        {
            string base_string = "";

            base_string += "function CreateStateMachine(_Type,_PlatformId,_NpcId,_X,_Y)\r\n\r\nBeginDefinition(_Type, _PlatformId, _NpcId, _X, _Y)\r\n";
            base_string += root_control.ToCode(true);
            if(dialog != null)
            {
                base_string += "\r\n\r\n";
                base_string += dialog.GenerateCode();
                base_string += "\r\n";
            }
            base_string += "\r\nEndDefinition()\r\nend";

            return base_string;
        }

        //never called when root_control == null
        private string BuildCodePLATFORM()
        {
            return BuildCodeNPC();
        }

        //never called when root_control == null
        private string BuildCodePATCH()
        {
            return BuildCodeNPC();
        }

        //chooses a method to build code with and returns generated code
        public string BuildCode()
        {
            if (root_control == null)
                throw new NullReferenceException("ScriptGenerator.BuildCode(): Root control is null!");

            string t = Enum.GetName(typeof(ScriptType), SType);
            t = t.Substring(7);

            MethodInfo generator = typeof(ScriptGenerator).GetMethod("BuildCode" + t, BindingFlags.NonPublic | BindingFlags.Instance);
            string code = (string)generator.Invoke(this, null);

            return code;
        }

        //generates block structure from stored nodes, starting from supplied control
        public void NodesToBlocks(LuaValueComplexControl root)
        {
            root_control = root ?? throw new ArgumentNullException("ScriptGenerator.NodesToBlocks(): Argument control is null!");

            LuaUtility.NodesToBlocks(root_control, code_blocks, code_nodes);
        }

        //generates nodes from stored root structure
        public void BlocksToNodes()
        {
            LuaUtility.BlocksToNodes(root_control, code_blocks, code_nodes);
        }

        //loads script from stream
        public int Load(BinaryReader br)
        {
            ScriptHeader scrHeader = ScriptHeader.Load(br);
            SType = scrHeader.type;
            Name = scrHeader.name;

            root_control = null;
            code_nodes.Clear();

            code_nodes.Add(ScriptNode.CreateEmpty());

            System.Diagnostics.Debug.WriteLine("SCRIPT LOAD! " + Name);
            for (int i = 1; i < scrHeader.node_count; i++)
            {
                ScriptNode node = ScriptNode.Load(br);
                code_nodes.Add(node);
                System.Diagnostics.Debug.WriteLine("NODE LOAD! " + node.ToString());
            }

            if (scrHeader.has_dialog)
            {
                dialog.Load(br);
            }

            return 0;
        }

        //saves script to a stream
        public int Save(BinaryWriter br)
        {
            /*//if no root control, no need to actually save anything
            if (root_control == null)
            {
                if (code_nodes.Count != 0)
                    return 0;
                return -4;
            }*/

            ScriptHeader scrHeader = new ScriptHeader();

            scrHeader.name = Name;
            scrHeader.type = SType;
            
            //assumes nodes are updated

            scrHeader.node_count = code_nodes.Count;
            scrHeader.has_dialog = ((dialog != null) && (SType == ScriptType.SCRIPT_NPC));
            scrHeader.size = 0; //updated later
            scrHeader.Save(br);

            long scr_size_position = br.BaseStream.Position - sizeof(int);

            System.Diagnostics.Debug.WriteLine("SAVE SCRIPT! " + scrHeader.name);
            for (int i = 1; i < code_nodes.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine("SAVE NODE! " + code_nodes[i].ToString());
                code_nodes[i].Save(br);
            }

            if(scrHeader.has_dialog)
            {
                dialog.Save(br);
            }

            scrHeader.size = (int)(br.BaseStream.Position - scr_size_position) - sizeof(int);
            br.BaseStream.Seek(scr_size_position, SeekOrigin.Begin);
            br.Write(scrHeader.size);
            br.BaseStream.Seek(0, SeekOrigin.End);

            return 0;
        }
    }
}