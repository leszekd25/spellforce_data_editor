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
    public partial class LuaDecompilerForm : Form
    {
        public LuaDecompilerForm()
        {
            InitializeComponent();
        }
        

        // assumes all files exist
        private void DecompileFiles(string[] fnames)
        {
            ButtonDecSingle.Enabled = false;
            ButtonDecDirectory.Enabled = false;
            ButtonOK.Enabled = false;

            ScriptsFound.Text = fnames.Length.ToString();
            Progress.Maximum = fnames.Length;
            Progress.Value = 0;
            ScriptsFound.Refresh();
            Progress.Refresh();

            int decompiled_scripts = 0;
            int failed_scripts = 0;

            for (int i = 0; i < fnames.Length; i++)
            {
                string fname = fnames[i];
                FileInfo fo = new FileInfo(fname);
                string file_dir = fo.DirectoryName;
                string new_fname = Path.GetFileNameWithoutExtension(fo.Name) + "_d.lua";

                FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                try
                {
                    LuaDecompiler.LuaBinaryScript scr = new LuaDecompiler.LuaBinaryScript(br);
                    LuaDecompiler.Decompiler dec = new LuaDecompiler.Decompiler();
                    LuaDecompiler.Node n = dec.Decompile(scr.func);

                    StringWriter sw = new StringWriter();
                    n.WriteLuaString(sw);
                    File.WriteAllText(file_dir + "\\" + new_fname, sw.ToString());

                    decompiled_scripts += 1;
                }
                catch (Exception)
                {
                    failed_scripts += 1;
                    LogUtils.Log.Error(LogUtils.LogSource.SFLua, "LuaDecompilerForm.DecompileFiles(): Failed to decompile script "
                        +fo.Name+"!");
                }
                finally
                {
                    br.Close();
                }
                if (i % 5 == 0)
                {
                    ScriptsDecompiled.Text = decompiled_scripts.ToString();
                    ScriptsFailed.Text = failed_scripts.ToString();
                    Progress.Value = i;
                    Application.DoEvents();
                }
            }

            ScriptsDecompiled.Text = decompiled_scripts.ToString();
            ScriptsFailed.Text = failed_scripts.ToString();
            Progress.Value = Progress.Maximum;

            ButtonDecSingle.Enabled = true;
            ButtonDecDirectory.Enabled = true;
            ButtonOK.Enabled = true;

            LogUtils.Log.Info(LogUtils.LogSource.SFLua, "LuaDecompilerForm.DecompileFiles(): Decompiled files: "
                +decompiled_scripts.ToString()+", failed to decompile: "+failed_scripts.ToString());
        }

        private void DecompileDirectory(string dname)
        {
            string[] list_items = Directory.GetFiles(dname, "*.lua", SearchOption.AllDirectories);
            DecompileFiles(list_items);
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonDecSingle_Click(object sender, EventArgs e)
        {
            if (DecFileDialog.ShowDialog() == DialogResult.OK)
                DecompileFiles(DecFileDialog.FileNames);
        }

        private void ButtonDecDirectory_Click(object sender, EventArgs e)
        {
            if (DecDirectoryDialog.ShowDialog() == DialogResult.OK)
                DecompileDirectory(DecDirectoryDialog.SelectedPath);
        }
    }
}
