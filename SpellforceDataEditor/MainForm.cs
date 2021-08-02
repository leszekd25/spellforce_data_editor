using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace SpellforceDataEditor
{
    public partial class MainForm : Form
    {
        // app updater
        Thread checknewversion_thread = null;
        bool update_finished = false;
        bool update_available = false;

        // form control
        public static special_forms.SpelllforceCFFEditor data = null;
        public static special_forms.SFAssetManagerForm viewer = null;
        public static special_forms.SQLModifierForm sqlmodify = null;
        public static special_forms.ModManagerForm modmanager = null;
        public static special_forms.MapEditorForm mapedittool = null;
        public static special_forms.AboutForm applicationinfo = null;
        public static special_forms.SaveDataEditorForm svdata = null;

        public MainForm()
        {
            // script decompilation and disassembly
            /*MemoryStream ms = SFUnPak.SFUnPak.LoadFileFrom("sf34.pak", "script\\p23\\clanrtsspawnp23.lua");
            BinaryReader br = new BinaryReader(ms);
            SFLua.LuaDecompiler.LuaBinaryScript scr = new SFLua.LuaDecompiler.LuaBinaryScript(br);
            scr.func.DumpAll();
            br.Close();
            SFLua.LuaDecompiler.Decompiler dec = new SFLua.LuaDecompiler.Decompiler();
            var chunk = dec.Decompile(scr.func);
            StringWriter sw = new StringWriter();
            chunk.WriteLuaString(sw);
            File.WriteAllText("func_dec2.txt", sw.ToString());*/

            // open sf0.pak, extract fonttables
            /*SFUnPak.SFPakFileSystem sf0 = new SFUnPak.SFPakFileSystem();
            sf0.Init("sf0.pak");
            List<string> files = sf0.ListAllWithFilename("texture", "font_fonttable");
            sf0.Open();
            foreach (var f in files)
            {
                MemoryStream ms = sf0.GetFileBuffer("texture\\" + f);
                FileStream fs = new FileStream(f, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(ms.ToArray(), 0, (int)ms.Length);
                ms.Close();
                fs.Close();
            }
            sf0.Close();*/

            LogUtils.Log.Info(LogUtils.LogSource.Main, "MainForm() called");
            InitializeComponent();
            linkEditor.Links.Add(0, linkEditor.Text.Length, "https://github.com/leszekd25/spellforce_data_editor/tree/with_viewer/bin");
            linkEditor.Visible = false;
            //CheckNewVersionAvailable();
            checknewversion_thread = new Thread(CheckNewVersionAvailable);
            checknewversion_thread.Start();
            TimerCheckUpdateStatus.Start();

            // check if data loaded from settings
            if (SFUnPak.SFUnPak.game_directory_specified)
                LabelIsSpecifiedGameDir.Text = "Game directory:\r\nSpecified";
            else
                LabelIsSpecifiedGameDir.Text = "Game directory:\r\nNOT specified";

            LogUtils.Log.TotalMemoryUsage();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
#if DEBUG
            bSaveData.Visible = true;
#endif
        }

        void CheckNewVersionAvailable()
        {           
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(getVersion_completed);

            // explicitly setup security protocol, github changes some things on their end
            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls
                   | System.Net.SecurityProtocolType.Tls11
                   | System.Net.SecurityProtocolType.Tls12
                   | System.Net.SecurityProtocolType.Ssl3;

            Uri dw_string = new Uri("https://raw.githubusercontent.com/leszekd25/spellforce_data_editor/with_viewer/bin/README.md");
            wc.DownloadStringAsync(dw_string);
        }

        void getVersion_completed(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            update_finished = true;
            if (e.Cancelled)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.Main, "MainForm.getVersion_completed(): Could not retrieve update info");
                return;
            }
            if (e.Error != null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.Main, "MainForm.getVersion_completed(): Error while retrieving update info");
                return;
            }
            string str = e.Result;
            int i = str.IndexOf("Latest version:");
            if (i == Utility.NO_INDEX)
            {
                LogUtils.Log.Error(LogUtils.LogSource.Main, "MainForm.getVersion_completed(): Invalid update info");
                return;
            }
            string newest_version = str.Substring(i + "Latest version:".Length).Trim();
            if (labelVersion.Text.IndexOf(newest_version) == Utility.NO_INDEX)
            {
                LogUtils.Log.Info(LogUtils.LogSource.Main, "MainForm.getVersion_completed(): New editor version available");
                update_available = true;
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.Main, "MainForm.getVersion_completed(): Editor is up-to-date");
            }
        }

        private void ButtonSpecifyGameDirectory_Click(object sender, EventArgs e)
        {
            if (GameDirDialog.ShowDialog() == DialogResult.OK)
            {
                int result = SFUnPak.SFUnPak.SpecifyGameDirectory(GameDirDialog.SelectedPath);
                if (result == 0)
                {
                    Settings.GameDirectory = GameDirDialog.SelectedPath;
                    LabelIsSpecifiedGameDir.Text = "Game directory:\r\nSpecified";
                    Settings.Save();
                }
                else
                    LabelIsSpecifiedGameDir.Text = "Game directory:\r\nFailed to specify!";
            }
        }

        private void bGDEditor_Click(object sender, EventArgs e)
        {
            if (data != null)
                return;
            data = new special_forms.SpelllforceCFFEditor();
            data.FormClosed += new FormClosedEventHandler(this.data_FormClosed);
            data.Show();
        }

        private void data_FormClosed(object sender, FormClosedEventArgs e)
        {
            data.FormClosed -= new FormClosedEventHandler(this.data_FormClosed);
            data = null;
            GC.Collect();
        }

        private void bAssets_Click(object sender, EventArgs e)
        {
            if (viewer != null)
                return;
            if(mapedittool != null)
            {
                MessageBox.Show("Can't run both Map Editor and Asset Viewer simultaneously! Fix coming soon :^)");
                return;
            }
            if (!SFUnPak.SFUnPak.game_directory_specified)
            {
                MessageBox.Show("Game directory is not specified!");
                return;
            }
            viewer = new special_forms.SFAssetManagerForm();
            viewer.FormClosed += new FormClosedEventHandler(this.viewer_FormClosed);
            viewer.Show();
        }

        private void viewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            viewer.FormClosed -= new FormClosedEventHandler(this.viewer_FormClosed);
            viewer = null;
            GC.Collect();
        }


        private void bMods_Click(object sender, EventArgs e)
        {
            if (modmanager != null)
                return;
            if (!SFUnPak.SFUnPak.game_directory_specified)
            {
                MessageBox.Show("Game directory is not specified!");
                return;
            }
            modmanager = new special_forms.ModManagerForm();
            modmanager.FormClosed += new FormClosedEventHandler(this.modmanager_FormClosed);
            modmanager.Show();
        }

        private void modmanager_FormClosed(object sender, FormClosedEventArgs e)
        {
            modmanager.FormClosed -= new FormClosedEventHandler(this.modmanager_FormClosed);
            modmanager = null;
            GC.Collect();
        }

        private void linkEditor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void TimerCheckUpdateStatus_Tick(object sender, EventArgs e)
        {
            if (!update_finished)
                TimerCheckUpdateStatus.Start();
            else
            {
                if (update_available)
                    linkEditor.Visible = true;
            }
        }

        private void bMap_Click(object sender, EventArgs e)
        {
            if (mapedittool != null)
                return;
            if (viewer != null)
            {
                MessageBox.Show("Can't run both Map Editor and Asset Viewer simultaneously! Fix coming soon :^)");
                return;
            }
            if (!SFUnPak.SFUnPak.game_directory_specified)
            {
                MessageBox.Show("Game directory is not specified!");
                return;
            }
            try
            {
                mapedittool = new special_forms.MapEditorForm();
                mapedittool.FormClosed += new FormClosedEventHandler(this.mapedittool_FormClosed);
                mapedittool.Show();
            }
            catch(Exception)
            {
                MessageBox.Show("Error while starting Map Editor!");
            }
        }

        private void mapedittool_FormClosed(object sender, FormClosedEventArgs e)
        {
            mapedittool.FormClosed -= new FormClosedEventHandler(this.mapedittool_FormClosed);
            mapedittool = null;
            GC.Collect();
        }

        private void bSQLEdit_Click(object sender, EventArgs e)
        {
            if (sqlmodify != null)
                return;
            if (!SFUnPak.SFUnPak.game_directory_specified)
            {
                MessageBox.Show("Game directory is not specified!");
                return;
            }
            sqlmodify = new special_forms.SQLModifierForm();
            sqlmodify.FormClosed += new FormClosedEventHandler(this.sqlmodify_FormClosed);
            sqlmodify.Show();
        }

        private void sqlmodify_FormClosed(object sender, FormClosedEventArgs e)
        {
            sqlmodify.FormClosed -= new FormClosedEventHandler(this.sqlmodify_FormClosed);
            sqlmodify = null;
            GC.Collect();
        }

        private void ButtonAbout_Click(object sender, EventArgs e)
        {
            if (applicationinfo != null)
                return;

            applicationinfo = new special_forms.AboutForm();
            applicationinfo.FormClosed += new FormClosedEventHandler(this.applicationinfo_FormClosed);
            applicationinfo.Show();
        }

        private void applicationinfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            applicationinfo.FormClosed -= new FormClosedEventHandler(this.applicationinfo_FormClosed);
            applicationinfo = null;
            GC.Collect();
        }

        private void bSaveData_Click(object sender, EventArgs e)
        {
            if (svdata != null)
                return;

            svdata = new special_forms.SaveDataEditorForm();
            svdata.FormClosed += new FormClosedEventHandler(this.svdata_FormClosed);
            svdata.Show();
        }

        private void svdata_FormClosed(object sender, FormClosedEventArgs e)
        {
            applicationinfo.FormClosed -= new FormClosedEventHandler(this.svdata_FormClosed);
            applicationinfo = null;
            GC.Collect();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
