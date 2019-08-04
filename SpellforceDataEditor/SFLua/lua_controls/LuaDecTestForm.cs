using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SpellforceDataEditor.SFLua.lua_controls
{
    public partial class LuaDecTestForm : Form
    {
        public LuaDecTestForm()
        {
            InitializeComponent();
        }

        public void DecompilerTest()
        {
            button1.Enabled = false;
            ButtonOK.Enabled = false;

            int found_scripts = 0;
            int decompiled_scripts = 0;
            int failed_scripts = 0;
            int unparsed_scripts = 0;

            List<string> scripts = SFUnPak.SFUnPak.ListAllWithExtension("", ".lua", new string[] { "sf34.pak" });
            List<string> fs_names = new List<string>();
            found_scripts = scripts.Count;
            ScriptsFound.Text = found_scripts.ToString();
            ScriptsFound.Refresh();
            Progress.Maximum = found_scripts;

            // decompile all scripts
            SFUnPak.SFPakFileSystem fs = SFUnPak.SFUnPak.GetPak("sf34.pak");
            for(int i = 0; i < found_scripts; i++)
            {
                string s = scripts[i];
                MemoryStream ms = fs.GetFileBuffer(s);
                BinaryReader br = new BinaryReader(ms);
                try
                {
                    LuaDecompiler.LuaBinaryScript scr = new LuaDecompiler.LuaBinaryScript(br);
                    LuaDecompiler.LuaStack stack = new LuaDecompiler.LuaStack();
                    LuaDecompiler.DecompileNode n = scr.func.Decompile(stack);
                    if (n == null)
                    {
                        fs_names.Add(s + " FAILED (SIZE "+br.BaseStream.Length.ToString()+")");
                        failed_scripts += 1;
                    }
                    else
                    {
                        decompiled_scripts += 1;
                    }
                }
                catch(Exception e)
                {
                    unparsed_scripts += 1;
                    fs_names.Add(s + " UNPARSED (SIZE " + br.BaseStream.Length.ToString() + ")");
                }
                finally
                {
                    br.Close();
                }
                if(i%10 == 0)
                {
                    ScriptsFailed.Text = (unparsed_scripts + failed_scripts).ToString();
                    ScriptsDecompiled.Text = decompiled_scripts.ToString();
                    Progress.Value = i;
                    Application.DoEvents();
                }
            }
            // write failed scripts to a file
            FileStream scr_dump = new FileStream("scripts_dump.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(scr_dump);
            foreach (string s in fs_names)
                sw.WriteLine(s);
            sw.Close();

            button1.Enabled = true;
            ButtonOK.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DecompilerTest();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
