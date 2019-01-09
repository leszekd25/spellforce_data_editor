/*
 * ModManagerForm is the form which allows for selecting mods to be loaded into the game, and loading them
 * */

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

using SpellforceDataEditor.SFUnPak;

namespace SpellforceDataEditor.special_forms
{
    public partial class ModManagerForm : Form
    {
        public const int ModToolVersion = 1;
        public static string GameVersion = "";
        bool ready = false;
        SFMod.SFModTemplate last_used = new SFMod.SFModTemplate();
        SFMod.SFModTemplate cur_used = new SFMod.SFModTemplate();
        SFMod.SFMod current_mod = new SFMod.SFMod();


        public ModManagerForm()
        {
            InitializeComponent();
            // check if game drectory is specified
            if(SFUnPak.SFUnPak.game_directory_name == "")
            {
                if (File.Exists("game_directory.txt"))
                {
                    string gamedir = File.ReadAllText("game_directory.txt");
                    int result = SFUnPak.SFUnPak.SpecifyGameDirectory(gamedir);
                    if (result == 0)
                    {
                        StatusText.Text = "Ready";
                        ready = true;
                    }
                    else
                    {
                        StatusText.Text = "Something went wrong! Try specifying game directory again";
                        ready = false;
                    }
                }
                else
                {
                    StatusText.Text = "Specify game directory before using this tool";
                    ready = false;
                }
            }
            if(ready)
            {
                Prepare();
            }
        }

        // prepares the manager for use
        private void Prepare()
        {
            // if no files in backup folder, add them from the game dir
            if (!Directory.Exists("backup"))
                Directory.CreateDirectory("backup");
            try
            {
                if (!File.Exists("backup\\GameData.cff"))
                {
                    // assumes file exists in game files
                    File.Copy(SFUnPak.SFUnPak.game_directory_name + "\\data\\GameData.cff", "backup\\GameData.cff");
                }
                if (!File.Exists("backup\\SpellForce.exe"))
                {
                    // assumes file exits in game files
                    File.Copy(SFUnPak.SFUnPak.game_directory_name + "\\SpellForce.exe", "backup\\SpellForce.exe");
                }
            }
            catch(UnauthorizedAccessException)
            {
                StatusText.Text = "Can't access game files. Try running in administrator mode";
                ready = false;
                return;
            }
            catch(FileNotFoundException)
            {
                StatusText.Text = "Can't find game files. Make sure you selected a valid game directory";
                ready = false;
                return;
            }

            // find game version
            GameVersion = "1.54.75000";   // temporary

            // load template
            if (!Directory.Exists("modtemplates"))
                Directory.CreateDirectory("modtemplates");
            if (!File.Exists("modtemplates\\last_used.tmpl"))
                File.Create("modtemplates\\last_used.tmpl");
            last_used.Load("last_used");
            cur_used.Load("last_used");
            cur_used.name = "";

            RefreshModList();

            SelectedModsRefresh();
        }

        // reloads mod list
        private void RefreshModList()
        {
            ModList.Items.Clear();

            // load mod names
            if (!Directory.Exists("mods"))
                Directory.CreateDirectory("mods");
            else
            {
                List<string> modfiles = Directory.EnumerateFiles("mods").ToList();
                foreach (string s in modfiles)
                {
                    string s2 = s.Replace("mods\\", "").Replace(".sfmd", "");
                    ModList.Items.Add(s2, false);
                }
            }
        }

        // refreshes mod status on the list
        private void SelectedModsRefresh()
        {
            for (int i = 0; i < ModList.Items.Count; i++)
                ModList.SetItemChecked(i, false);
            foreach(string name in cur_used.mod_names)
            {
                ModList.SetItemChecked(ModList.Items.IndexOf(name), true);
            }
        }

        // tries to restore game files to original state
        private void RestoreGameFiles(bool ask_confirm)
        {
            if (ask_confirm)
            {
                DialogResult result = MessageBox.Show("Doing this will restore your game files to their original state. Press OK to proceed.", "Confirm to restore", MessageBoxButtons.OKCancel);
                if (result != DialogResult.OK)
                    return;
            }
            StatusText.Text = "Processing...";
            statusStrip1.Refresh();
            foreach(string s in ModList.Items)
            {
                SFMod.SFMod mod = new SFMod.SFMod();
                mod.Load("mods\\"+s+".sfmd");
                foreach(string f in mod.assets.GetFileNames())
                {
                    string fname = SFUnPak.SFUnPak.game_directory_name + "\\" + f;
                    if (File.Exists(fname))
                        File.Delete(fname);
                }

                mod.Unload();
            }

            File.Copy("backup\\GameData.cff", SFUnPak.SFUnPak.game_directory_name + "\\data\\GameData.cff", true);
            File.Copy("backup\\SpellForce.exe", SFUnPak.SFUnPak.game_directory_name + "\\SpellForce.exe", true);
            StatusText.Text = "Done";
        }

        private void restoreToOriginalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreGameFiles(true);
        }

        private void makeYourOwnModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModCreatorForm creator = new ModCreatorForm();
            creator.ShowDialog();

            RefreshModList();
            SelectedModsRefresh();

        }

        // applies selected mods to a game
        private void ButtonApplyMods_Click(object sender, EventArgs e)
        {
            if (!ready)
                return;
            // prompt to confirm
            DialogResult result = MessageBox.Show("Doing this will modify your game files. You can undo the process with the Restore option in the Mods menu. Press OK to proceed.", "Confirm to apply mods", MessageBoxButtons.OKCancel);
            if (result != DialogResult.OK)
                return;
            // restore to original
            StatusText.Text = "Applying changes...";
            RestoreGameFiles(false);
            // modify files according to selected mods and save template as last used
            SFCFF.SFGameData gd_to_modify = new SFCFF.SFGameData();
            gd_to_modify.Load(SFUnPak.SFUnPak.game_directory_name + "\\data\\GameData.cff");
            List<string> failed_mods = new List<string>();
            for(int i = 0; i < ModList.Items.Count; i++)
            {
                if(ModList.GetItemChecked(i))
                {
                    SFMod.SFMod mod = new SFMod.SFMod();
                    string mod_name = ModList.Items[i].ToString();
                    int result_load = mod.Load("mods\\"+mod_name+".sfmd");
                    if (result_load != 0)
                    {
                        failed_mods.Add(mod_name);
                        continue;
                    }
                    result_load = mod.Apply(gd_to_modify);
                    if (result_load != 0)
                    {
                        failed_mods.Add(mod_name);
                        continue;
                    }
                }
            }
            gd_to_modify.Save(SFUnPak.SFUnPak.game_directory_name + "\\data\\GameData.cff");
            // if some mods were not applied, show a box with all failed mods
            if (failed_mods.Count != 0)
            {
                string mod_list = "";
                foreach (string m in failed_mods)
                    mod_list += m + "\r\n";
                result = MessageBox.Show("The following mods could not be applied:\r\n" + mod_list + "Press OK to restore to original, or Cancel to continue normally.", "Could not apply mods", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    RestoreGameFiles(false);
                }
            }
            else
            {
                last_used.CopyFrom(cur_used);
                last_used.name = "last_used";
                last_used.Save();
            }
            gd_to_modify.Unload();

            GC.Collect();
            StatusText.Text = "Changes saved";
        }

        private void ModList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModList.SelectedIndex == -1)
                return;
            // load mod into memory (excluding assets ofc), display info
            current_mod.Unload();
            string mod_fname = "mods\\"+ModList.SelectedItem.ToString()+".sfmd";
            int result = current_mod.Load(mod_fname);
            if(result != 0)
            {
                StatusText.Text = "Error: Can't load mod " + ModList.SelectedItem.ToString();
                return;
            }

            LabelModInfo.Text = current_mod.info.ToString();
            StatusText.Text = "Mod info loaded";
        }

        private void loadTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SFMod.mod_controls.TemplateSelectForm select_tmpl = new SFMod.mod_controls.TemplateSelectForm();
            DialogResult result = select_tmpl.ShowDialog();
            if (result != DialogResult.OK)
                return;

            cur_used.Load(select_tmpl.result);

            SelectedModsRefresh();

            StatusText.Text = "Template loaded";
        }

        private void saveTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = Utility.GetString("Choose a name", "Enter the name of your template:");
            cur_used.name = name;
            cur_used.Save();
            StatusText.Text = "Template saved";
        }

        private void specifyGameDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool prev_ready = ready;
            if (SelectGameDirectory.ShowDialog() == DialogResult.OK)
            {
                StatusText.Text = "Processing...";
                int result = SFUnPak.SFUnPak.SpecifyGameDirectory(SelectGameDirectory.SelectedPath);
                if (result == 0)
                {
                    File.WriteAllText("game_directory.txt", SelectGameDirectory.SelectedPath);
                    StatusText.Text = "Ready";
                    ready = true;
                }
                else
                {
                    StatusText.Text = "Failed to specify directory";
                }
            }
            if((ready)&&(!prev_ready))
                Prepare();
        }

        private void specifyOriginalGamedatacffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectOrigCFF.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(SelectOrigCFF.FileName, "backup\\GameData.cff");
                    StatusText.Text = "Done";
                }
                catch (Exception)
                {
                    StatusText.Text = "Could not choose this file!";
                }
            }
        }

        private void specifyOriginalSpellforceexeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectOrigEXE.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(SelectOrigEXE.FileName, "backup\\SpellForce.exe");
                    StatusText.Text = "Done";
                }
                catch (Exception)
                {
                    StatusText.Text = "Could not choose this file!";
                }
            }
        }

        private void reloadModListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshModList();
        }
    }
}
