using SFEngine.SFUnPak;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using OpenTK.Mathematics;
using System.Runtime.Intrinsics.Arm;
using SFEngine.SFLua.LuaTokenizer;
using System.IO;

namespace SpellforceDataEditor
{
    public partial class MainForm : Form
    {
        // app updater
        static HttpClient client = new HttpClient();
        Thread checknewversion_thread = null;
        bool update_finished = false;
        bool update_available = false;

        // form control
        public static special_forms.SpelllforceCFFEditor data = null;
        public static special_forms.SFAssetManagerForm viewer = null;
        public static special_forms.SQLModifierForm sqlmodify = null;
        public static special_forms.MapEditorForm mapedittool = null;
        public static special_forms.AboutForm applicationinfo = null;
        public static special_forms.SaveDataEditorForm svdata = null;

        public MainForm()
        {
            SFEngine.SFLua.LuaTokenizer.Parser parser = new(
                File.ReadAllText("C:\\Users\\lesze\\Documents\\Visual Studio 2015\\Projects\\SpellForce1\\main\\redist\\script\\P63\\n0.lua"));

            SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.Main, "MainForm() called");
            InitializeComponent();
            linkEditor.Links.Add(0, linkEditor.Text.Length, "https://github.com/leszekd25/spellforce_data_editor/tree/with_viewer/bin");
            linkEditor.Visible = false;
            checknewversion_thread = new Thread(CheckNewVersionAvailable);
            checknewversion_thread.Start();
            TimerCheckUpdateStatus.Start();

            // check if data loaded from settings
            if (SFUnPak.game_directory_specified)
            {
                SFEngine.SFResources.SFResourceManager.ListAllFilesystemResources();
                LabelIsSpecifiedGameDir.Text = "Game directory:\r\nSpecified";
            }
            else
            {
                LabelIsSpecifiedGameDir.Text = "Game directory:\r\nNOT specified";
            }

            SFEngine.LogUtils.Log.TotalMemoryUsage();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
#if DEBUG
            bSaveData.Visible = true;
#endif
        }

        async void CheckNewVersionAvailable()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                string response = await client.GetStringAsync("https://raw.githubusercontent.com/leszekd25/spellforce_data_editor/with_viewer/bin/README.md");
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                int i = response.IndexOf("Latest version:");
                if (i == SFEngine.Utility.NO_INDEX)
                {
                    SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.Main, "MainForm.getVersion_completed(): Invalid update info");
                    return;
                }
                string newest_version = response.Substring(i + "Latest version:".Length).Trim();
                if (labelVersion.Text.IndexOf(newest_version) == SFEngine.Utility.NO_INDEX)
                {
                    SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.Main, "MainForm.getVersion_completed(): New editor version available");
                    update_available = true;
                }
                else
                {
                    SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.Main, "MainForm.getVersion_completed(): Editor is up-to-date");
                }
            }
            catch (HttpRequestException e)
            {
                SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.Main, $"MainForm.getVersion_completed(): Error while retrieving update info (error code {e.HttpRequestError}");
                return;
            }
            finally
            {
                update_finished = true;
            }
        }

        private void ButtonSpecifyGameDirectory_Click(object sender, EventArgs e)
        {
            if (GameDirDialog.ShowDialog() == DialogResult.OK)
            {
                int result = SFUnPak.SpecifyGameDirectory(GameDirDialog.SelectedPath);
                if (result == 0)
                {
                    SFEngine.SFResources.SFResourceManager.ListAllFilesystemResources();
                    SFEngine.Settings.GameDirectory = GameDirDialog.SelectedPath;
                    LabelIsSpecifiedGameDir.Text = "Game directory:\r\nSpecified";
                    SFEngine.Settings.Save();
                }
                else
                {
                    LabelIsSpecifiedGameDir.Text = "Game directory:\r\nFailed to specify!";
                }
            }
        }

        private void bGDEditor_Click(object sender, EventArgs e)
        {
            if (data != null)
            {
                return;
            }

            data = new special_forms.SpelllforceCFFEditor();
            data.FormClosed += new FormClosedEventHandler(data_FormClosed);
            data.Show();
        }

        private void data_FormClosed(object sender, FormClosedEventArgs e)
        {
            data.FormClosed -= new FormClosedEventHandler(data_FormClosed);
            data = null;
            GC.Collect();
        }

        private void bAssets_Click(object sender, EventArgs e)
        {
            if (viewer != null)
            {
                return;
            }

            if (mapedittool != null)
            {
                MessageBox.Show("Can't run both Map Editor and Asset Viewer simultaneously! Fix coming soon :^)");
                return;
            }
            if (!SFUnPak.game_directory_specified)
            {
                MessageBox.Show("Game directory is not specified!");
                return;
            }
            viewer = new special_forms.SFAssetManagerForm();
            viewer.FormClosed += new FormClosedEventHandler(viewer_FormClosed);
            viewer.Show();
        }

        private void viewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            viewer.FormClosed -= new FormClosedEventHandler(viewer_FormClosed);
            viewer = null;
            GC.Collect();
        }

        private void linkEditor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void TimerCheckUpdateStatus_Tick(object sender, EventArgs e)
        {
            if (!update_finished)
            {
                TimerCheckUpdateStatus.Start();
            }
            else
            {
                if (update_available)
                {
                    linkEditor.Visible = true;
                }
            }
        }

        private void bMap_Click(object sender, EventArgs e)
        {
            if (mapedittool != null)
            {
                return;
            }

            if (viewer != null)
            {
                MessageBox.Show("Can't run both Map Editor and Asset Viewer simultaneously! Fix coming soon :^)");
                return;
            }
            if (!SFUnPak.game_directory_specified)
            {
                MessageBox.Show("Game directory is not specified!");
                return;
            }
            try
            {
                mapedittool = new special_forms.MapEditorForm();
                mapedittool.FormClosed += new FormClosedEventHandler(mapedittool_FormClosed);
                mapedittool.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Error while starting Map Editor!");
            }
        }

        private void mapedittool_FormClosed(object sender, FormClosedEventArgs e)
        {
            mapedittool.FormClosed -= new FormClosedEventHandler(mapedittool_FormClosed);
            mapedittool = null;
            GC.Collect();
        }

        private void bSQLEdit_Click(object sender, EventArgs e)
        {
            if (sqlmodify != null)
            {
                return;
            }

            if (!SFUnPak.game_directory_specified)
            {
                MessageBox.Show("Game directory is not specified!");
                return;
            }
            sqlmodify = new special_forms.SQLModifierForm();
            sqlmodify.FormClosed += new FormClosedEventHandler(sqlmodify_FormClosed);
            sqlmodify.Show();
        }

        private void sqlmodify_FormClosed(object sender, FormClosedEventArgs e)
        {
            sqlmodify.FormClosed -= new FormClosedEventHandler(sqlmodify_FormClosed);
            sqlmodify = null;
            GC.Collect();
        }

        private void ButtonAbout_Click(object sender, EventArgs e)
        {
            if (applicationinfo != null)
            {
                return;
            }

            applicationinfo = new special_forms.AboutForm();
            applicationinfo.FormClosed += new FormClosedEventHandler(applicationinfo_FormClosed);
            applicationinfo.Show();
        }

        private void applicationinfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            applicationinfo.FormClosed -= new FormClosedEventHandler(applicationinfo_FormClosed);
            applicationinfo = null;
            GC.Collect();
        }

        private void bSaveData_Click(object sender, EventArgs e)
        {
            if (svdata != null)
            {
                return;
            }

            svdata = new special_forms.SaveDataEditorForm();
            svdata.FormClosed += new FormClosedEventHandler(svdata_FormClosed);
            svdata.Show();
        }

        private void svdata_FormClosed(object sender, FormClosedEventArgs e)
        {
            svdata.FormClosed -= new FormClosedEventHandler(svdata_FormClosed);
            svdata = null;
            GC.Collect();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void labelVersion_Click(object sender, EventArgs e)
        {

        }
    }
}
