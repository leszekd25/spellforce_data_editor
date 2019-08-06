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

namespace SpellforceDataEditor.SFMap.map_dialog
{
    public partial class MapSelectPlatformScriptDialog : Form
    {
        public uint PlatformID = 6666;
        public string result { get; private set; } = null;

        public MapSelectPlatformScriptDialog()
        {
            InitializeComponent();
        }

        private void MapSelectPlatformScriptDialog_Load(object sender, EventArgs e)
        {
            FindAllScripts();
        }

        private void FindAllScripts()
        {
            // 1) find all scripts in sf34.pak:script\\p%ID 
            string dname = "script\\p" + PlatformID.ToString();

            List<string> scripts_found = SFUnPak.SFUnPak.ListAllInDirectory(dname, new string[] { "sf34.pak" });

            // 2) find all files in game_directory\\script\\p%ID
            string real_dname = SFUnPak.SFUnPak.game_directory_name + "\\" + dname;
            if (Directory.Exists(real_dname))
            {
                string[] real_files = Directory.GetFiles(real_dname, "*.lua");
                foreach (string s in real_files)
                {
                    string s2 = Path.GetFileName(s);
                    if (!scripts_found.Contains(s2))
                        scripts_found.Add(s2);
                }
            }
            scripts_found.Sort();

            ListScripts.Items.Clear();
            foreach (string s in scripts_found)
                ListScripts.Items.Add(s);
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (ListScripts.SelectedIndex != -1)
                result = ListScripts.Items[ListScripts.SelectedIndex].ToString();
            Close();
        }
    }
}
