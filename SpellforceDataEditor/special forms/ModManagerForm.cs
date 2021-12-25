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

using SFEngine.SFCFF;
using SFEngine.SFUnPak;

namespace SpellforceDataEditor.special_forms
{
    public partial class ModManagerForm : Form
    {
        public const int ModToolVersion = 1;
        public static string GameVersion = "";
        bool ready = false;
        SFMod.SFModTemplate last_used = new SFMod.SFModTemplate();
        SFMod.SFMod current_mod = new SFMod.SFMod();


        public ModManagerForm()
        {
            InitializeComponent();
            // assumes game directory is specified
            Prepare();
        }

        // prepares the manager for use
        private void Prepare()
        {
            int status = 0;
            // if no files in backup folder, add them from the game dir
            if (!Directory.Exists("backup"))
            {
                SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMod, "ModManagerForm.Prepare(): Creating backup of important game files");
                Directory.CreateDirectory("backup");
            }
            if (!File.Exists("backup\\GameData.cff"))
                status += SFEngine.Utility.CopyFile(SFUnPak.game_directory_name + "\\data\\GameData.cff", "backup\\GameData.cff");
            if (!File.Exists("backup\\SpellForce.exe"))
                status += SFEngine.Utility.CopyFile(SFUnPak.game_directory_name + "\\SpellForce.exe", "backup\\SpellForce.exe");
            if(!Directory.Exists("backup\\map"))
                status += SFEngine.Utility.CopyDirectory(SFUnPak.game_directory_name + "\\map", "backup\\map");
            if(status != 0)
            {
                SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.SFMod, "ModManagerForm.Prepare(): Can't access game files!");
                StatusText.Text = "Can't access game files. Try running in administrator mode and make sure game files aren't missing.";
                ready = false;
                return;
            }

            // find game version
            GameVersion = "1.54.75000";   // temporary

            // load template
            if (!Directory.Exists("modtemplates"))
                Directory.CreateDirectory("modtemplates");
            if (!File.Exists("modtemplates\\last_used.tmpl"))
            {
                FileStream fs = File.Create("modtemplates\\last_used.tmpl");
                fs.Close();
            }

            last_used.Load("last_used");

            RefreshModList();

            SelectedModsRefresh();

            ready = true;
        }

        // reloads mod list
        private void RefreshModList()
        {
            ModList.Items.Clear();

            // load mod names
            if (!Directory.Exists("mods"))
            {
                SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMod, "ModManagerForm.RefreshModList(): Creating directory mods");
                Directory.CreateDirectory("mods");
            }
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
            for(int i = 0; i < last_used.mod_names.Count; i++)
            {
                string name = last_used.mod_names[i];
                if (ModList.Items.Contains(name))
                    ModList.SetItemChecked(ModList.Items.IndexOf(name), true);
                else
                {
                    SFEngine.LogUtils.Log.Warning(SFEngine.LogUtils.LogSource.SFMod, "ModManagerForm.SelectedModsRefresh(): Mod " + name + " not found!");
                    last_used.mod_names.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        private bool IsGameRunning()
        {
            return System.Diagnostics.Process.GetProcessesByName("spellforce").Length != 0;
        }

        // tries to restore game files to original state
        private void RestoreGameFiles(bool ask_confirm)
        {
            if (!ready)
                return;
            if (IsGameRunning())
                return;
            if (ask_confirm)
            {
                DialogResult result = MessageBox.Show("Doing this will restore your game files to their original state. Press OK to proceed.", "Confirm to restore", MessageBoxButtons.OKCancel);
                if (result != DialogResult.OK)
                    return;
            }
            SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMod, "ModManagerForm.RestoreGameFiles() called");
            StatusText.Text = "Restoring game to original state...";
            statusStrip1.Refresh();
            foreach(string s in ModList.Items)
            {
                SFMod.SFMod mod = new SFMod.SFMod();
                mod.Load("mods\\"+s+".sfmd", SFMod.ModLoadOption.ASSETS);
                foreach(string f in mod.assets.GetFileNames())
                {
                    string fname = SFUnPak.game_directory_name + "\\" + f;
                    if (File.Exists(fname))
                    {
                        SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMod, "ModManagerForm.RestoreGameFiles(): Removing file "+fname);
                        File.Delete(fname);
                    }
                }

                mod.Unload();
            }
            
            SFEngine.Utility.CopyFile("backup\\GameData.cff", SFUnPak.game_directory_name + "\\data\\GameData.cff");
            SFEngine.Utility.CopyFile("backup\\SpellForce.exe", SFUnPak.game_directory_name + "\\SpellForce.exe");
            SFEngine.Utility.CopyDirectory("backup\\map", SFUnPak.game_directory_name + "\\map");

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

            GC.Collect();
        }

        private void UpdateUsedMods(SFMod.SFModTemplate tmpl)
        {
            tmpl.mod_names.Clear();
            for (int i = 0; i < ModList.CheckedItems.Count; i++)
                tmpl.mod_names.Add(ModList.CheckedItems[i].ToString());
        }

        private bool ApplyMods()
        {
            if (!ready)
                return false;
            if (IsGameRunning())
                return false;
            // prompt to confirm
            DialogResult result = MessageBox.Show("Doing this will modify your game files. You can undo the process with the Restore option in the Mods menu. Press OK to proceed.", "Confirm to apply mods", MessageBoxButtons.OKCancel);
            if (result != DialogResult.OK)
                return false;
            // restore to original
            SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMod, "ModManagerForm.ApplyMods() called");
            RestoreGameFiles(false);
            // modify files according to selected mods and save template as last used
            SFGameData gd_to_modify = new SFGameData();
            StatusText.Text = "Loading gamedata to modify...";
            statusStrip1.Refresh();
            gd_to_modify.Load(SFUnPak.game_directory_name + "\\data\\GameData.cff");
            List<string> failed_mods = new List<string>();
            for (int i = 0; i < ModList.Items.Count; i++)
            {
                if (ModList.GetItemChecked(i))
                {
                    SFMod.SFMod mod = new SFMod.SFMod();
                    string mod_name = ModList.Items[i].ToString();
                    SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.SFMod, "ModManagerForm.ApplyMods(): Applying mod " + mod_name);
                    StatusText.Text = "Loading mod "+mod_name+"...";
                    statusStrip1.Refresh();
                    int result_load = mod.Load("mods\\" + mod_name + ".sfmd", SFMod.ModLoadOption.DATA | SFMod.ModLoadOption.ASSETS);
                    if (result_load != 0)
                    {
                        failed_mods.Add(mod_name);
                        continue;
                    }
                    StatusProgreessBar.Value = 0;
                    StatusProgreessBar.Maximum = mod.assets.assets.Count;

                    StatusText.Text = "Modifying gamedata...";
                    statusStrip1.Refresh();
                    mod.assets.update_event += new SFMod.SFModAssetUpdate(OnAssetUpdateEvent);
                    result_load = mod.Apply(gd_to_modify);
                    mod.assets.update_event -= new SFMod.SFModAssetUpdate(OnAssetUpdateEvent);
                    StatusProgreessBar.Visible = false;

                    if (result_load != 0)
                    {
                        failed_mods.Add(mod_name);
                        continue;
                    }
                }
            }

            StatusText.Text = "Saving modified gamedata...";
            statusStrip1.Refresh();
            gd_to_modify.Save(SFUnPak.game_directory_name + "\\data\\GameData.cff");

            // if some mods were not applied, show a box with all failed mods
            if (failed_mods.Count != 0)
            {
                string mod_list = "";
                foreach (string m in failed_mods)
                    mod_list += m + "\r\n";
                result = MessageBox.Show("The following mods could not be applied:\r\n" + mod_list + "Press OK to restore to original, or Cancel to continue normally.", "Could not apply mods", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                    RestoreGameFiles(false);
                else
                    StatusText.Text = "Mods applied";
            }
            else
            {
                // save used mods to file
                UpdateUsedMods(last_used);
                last_used.name = "last_used";
                last_used.Save();
                StatusText.Text = "Mods applied";
            }
            gd_to_modify.Unload();

            GC.Collect();

            return true;
        }

        // applies selected mods to a game
        private void ButtonApplyMods_Click(object sender, EventArgs e)
        {
            ApplyMods();
        }

        private void ModList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModList.SelectedIndex == -1)
                return;
            // load mod into memory (excluding assets ofc), display info
            current_mod.Unload();
            string mod_fname = "mods\\"+ModList.SelectedItem.ToString()+".sfmd";
            int result = current_mod.Load(mod_fname, SFMod.ModLoadOption.INFO);
            if(result != 0)
            {
                if(!ready)
                    StatusText.Text = "Error: Can't load mod " + ModList.SelectedItem.ToString();
                return;
            }

            LabelModInfo.Text = current_mod.info.ToString();
        }

        private void reloadModListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshModList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RestoreGameFiles(true);
        }
        
        // it doesn't matter if game can't be run, we just want to attempt to run it
        private void RunGame()
        {
            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(
                   SFUnPak.game_directory_name + "\\SpellForce.exe");

                psi.Arguments = SFEngine.Settings.GameRunArguments;
                psi.WorkingDirectory = SFUnPak.game_directory_name + "\\";
                System.Diagnostics.Process.Start(psi);
            }
            catch(Exception)
            {
                return;
            }
        }

        private void ButtonApplyAndRun_Click(object sender, EventArgs e)
        {
            if(ApplyMods())
                RunGame();
        }

        private void specifySpellforceexeArgumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SFEngine.Settings.GameRunArguments = WinFormsUtility.GetString("Input SpellForce.exe arguments", 
                                                          "Input arguments to run the game with:", 
                                                          SFEngine.Settings.GameRunArguments);
        }

        private void OnAssetUpdateEvent(string fname)
        {
            StatusProgreessBar.Visible = true;
            if (fname.Length > 39)
                fname = "..." + fname.Substring(fname.Length - 37, 37);
            StatusText.Text = "Extracting " + fname + "...";
            StatusProgreessBar.Value += 10;
            statusStrip1.Refresh();
        }
    }
}
