/*
 * LuaUtility functions similarly to Utility, it contains helper functions to Lua tools
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpellforceDataEditor.SFLua.lua_controls;
using SpellforceDataEditor.SFLua.lua_filesystem;

namespace SpellforceDataEditor.SFLua
{
    static public class LuaUtility
    {
        //a dictionary which pairs node type with corresponding lua control type
        public static Dictionary<ScriptNodeValueType, Type> nodevalue_to_luacontrol = new Dictionary<ScriptNodeValueType, Type>
        {
            {ScriptNodeValueType.BOOL, typeof(LuaValueBoolControl) },
            {ScriptNodeValueType.COLOR, typeof(LuaValueColorControl) },
            {ScriptNodeValueType.COMPLEX, typeof(LuaValueComplexControl) },
            {ScriptNodeValueType.CUSTOM, typeof(LuaValueCustomControl) },
            {ScriptNodeValueType.DOUBLE, typeof(LuaValueDoubleControl) },
            {ScriptNodeValueType.ENUM, typeof(LuaValueEnumControl) },
            {ScriptNodeValueType.INT, typeof(LuaValueIntControl) },
            {ScriptNodeValueType.STRING, typeof(LuaValueStringControl) },
            {ScriptNodeValueType.GD, typeof(LuaValueGDControl) },
            {ScriptNodeValueType.PARAMETER, typeof(LuaValueParameterControl) },
            {ScriptNodeValueType.HIDDEN_PARAMETER, typeof(_LuaValueHiddenParameterControl) }
        };
        
        //sorts all controls starting from cur_control depth-first, assigning IDs as it sorts
        static private void SortBlocks(LuaValueControl cur_control, List<LuaValueControl> code_blocks, ref int current_id)
        {
            cur_control.ID = current_id;
            code_blocks.Add(cur_control);
            current_id += 1;

            LuaValueComplexControl next_branch = null;
            if (cur_control is LuaValueComplexControl)
                next_branch = (LuaValueComplexControl)cur_control;
            else if (cur_control is LuaValueCustomControl)
                SortBlocks(((LuaValueCustomControl)cur_control).GetParamsControl(), code_blocks, ref current_id);
            if (next_branch != null)
                for (int i = 0; i < next_branch.GetValuesPanel().Controls.Count; i++)
                    SortBlocks((LuaValueControl)next_branch.GetValuesPanel().Controls[i], code_blocks, ref current_id);
        }

        //given the (sorted) lua controls starting from cur_control, create node tree describing the control structure
        //todo: i think you don't need to do this recursively, as code_blocks array is already generated
        static public void GenerateNodes(LuaValueControl cur_control, List<ScriptNode> code_nodes)
        {
            ScriptNode n = new ScriptNode();
            n.id = cur_control.ID;
            if (cur_control.ParentControl != null)
                n.parent_id = cur_control.ParentControl.ID;
            else
                n.parent_id = -1;
            n.name = cur_control.Value.Name;
            n.type = nodevalue_to_luacontrol.FirstOrDefault(x => x.Value == cur_control.GetType()).Key;
            n.value = cur_control.Value.Value;
            n.def = cur_control.Default.Value;
            if (cur_control is LuaValueComplexControl)
                n.parse_flags = ((LuaValueComplexControl)cur_control).ParseFlags;
            else
                n.parse_flags = 0;
            n.pschar = cur_control.PSChar;
            n.important = cur_control.Important;
            /* if (n.value != null)
                 System.Diagnostics.Debug.WriteLine(n.ToString());
             else
                 System.Diagnostics.Debug.WriteLine("NULL");*/
            code_nodes.Add(n);

            LuaValueComplexControl next_branch = null;
            if (cur_control is LuaValueComplexControl)
                next_branch = (LuaValueComplexControl)cur_control;
            else if (cur_control is LuaValueCustomControl)
                GenerateNodes(((LuaValueCustomControl)cur_control).GetParamsControl(), code_nodes);
            if (next_branch != null)
                for (int i = 0; i < next_branch.GetValuesPanel().Controls.Count; i++)
                    GenerateNodes((LuaValueControl)next_branch.GetValuesPanel().Controls[i], code_nodes);
        }
        
        //generate nodes from given control structure, starting at root_control
        static public void BlocksToNodes(LuaValueComplexControl root_control, List<LuaValueControl> code_blocks, List<ScriptNode> code_nodes)
        {
            //System.Diagnostics.Debug.WriteLine("BLOCKS TO NODES! BLOCK = " + root_control.Value.Name);
            if (root_control == null)
                return;

            int cur_id = 0;
            code_blocks.Clear();
            SortBlocks(root_control, code_blocks, ref cur_id);

            code_nodes.Clear();
            GenerateNodes(root_control, code_nodes);
        }

        //generate control structure given node tree, starting at root_control
        static public void NodesToBlocks(LuaValueComplexControl root_control, List<LuaValueControl> code_blocks, List<ScriptNode> code_nodes)
        {
            //read root node - only set name
            root_control.Value.Name = code_nodes[0].name;
            //System.Diagnostics.Debug.WriteLine("NODES TO BLOCKS! BLOCK = " + root_control.Value.Name);

            //generate nodes, using lookup tables
            code_blocks.Clear();
            code_blocks.Add(root_control);
            root_control.RefreshName();
            for (int i = 1; i < code_nodes.Count; i++)
            {
                LuaValueControl val;

                Type control_type = nodevalue_to_luacontrol[code_nodes[i].type];
                if (control_type == typeof(LuaValueComplexControl))
                    val = new LuaValueComplexControl(code_nodes[i].parse_flags, code_nodes[i].pschar);
                else
                    val = (LuaValueControl)Activator.CreateInstance(control_type, code_nodes[i].value);
                val.Default = new LuaValue(code_nodes[i].def);
                val.Value.Name = code_nodes[i].name;
                val.PSChar = code_nodes[i].pschar;
                val.Important = code_nodes[i].important;
                val.RefreshName();
                LuaValueControl par = code_blocks[code_nodes[i].parent_id];
                if (par is LuaValueComplexControl)
                {
                    ((LuaValueComplexControl)par).AddLuaControl(val);
                }
                code_blocks.Add(val);
                if (val is LuaValueCustomControl)
                {
                    code_blocks.Add(((LuaValueCustomControl)val).GetParamsControl());
                    i++;
                }
            }
            //resize nodes with children
            bool[] checked_blocks = new bool[code_blocks.Count];
            for (int i = 0; i < checked_blocks.Length; i++)
                checked_blocks[i] = false;
            for (int i = code_nodes.Count - 1; i > 0; i--)
            {
                if (!checked_blocks[code_nodes[i].parent_id])
                {
                    code_blocks[i].ResetSize();
                    checked_blocks[code_nodes[i].parent_id] = true;
                }
            }
        }
    }
}
