using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor
{
    public partial class MainForm : Form
    {
        // form control
        public static special_forms.SpelllforceCFFEditor data = null;
        public static special_forms.SFAssetManagerForm viewer = null;
        public static special_forms.ModManagerForm modmanager = null;
        public static special_forms.ScriptBuilderForm scripts = null;

        public MainForm()
        {
            InitializeComponent();
            linkEditor.Links.Add(0, linkEditor.Text.Length, "https://github.com/leszekd25/spellforce_data_editor/tree/with_viewer/bin");
            linkEditor.Visible = false;
            SFCFF.SFCategoryManager.init();
            CheckNewVersionAvailable();
        }

        void CheckNewVersionAvailable()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(getVersion_completed);

            Uri dw_string = new Uri("https://raw.githubusercontent.com/leszekd25/spellforce_data_editor/with_viewer/bin/README.md");
            wc.DownloadStringAsync(dw_string);
        }

        void getVersion_completed(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            string str = e.Result;
            int i = str.IndexOf("Latest version:");
            if (i == -1)
                return;
            string newest_version = str.Substring(i + "Latest version:".Length).Trim();
            if (labelVersion.Text.IndexOf(newest_version) == -1)
                linkEditor.Visible = true;
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
            data = null;
            GC.Collect();
        }

        private void bAssets_Click(object sender, EventArgs e)
        {
            if (viewer != null)
                return;
            viewer = new special_forms.SFAssetManagerForm();
            viewer.FormClosed += new FormClosedEventHandler(this.viewer_FormClosed);
            viewer.Show();
        }

        private void viewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            viewer = null;
            GC.Collect();
        }

        private void bScripting_Click(object sender, EventArgs e)
        {
            if (scripts != null)
                return;
            scripts = new special_forms.ScriptBuilderForm();
            scripts.FormClosed += new FormClosedEventHandler(this.scripts_FormClosed);
            scripts.Link(data);
            scripts.Show();
        }

        private void scripts_FormClosed(object sender, FormClosedEventArgs e)
        {
            scripts = null;
            GC.Collect();
        }

        private void bMods_Click(object sender, EventArgs e)
        {
            if (modmanager != null)
                return;
            modmanager = new special_forms.ModManagerForm();
            modmanager.FormClosed += new FormClosedEventHandler(this.modmanager_FormClosed);
            modmanager.Show();
        }

        private void modmanager_FormClosed(object sender, FormClosedEventArgs e)
        {
            modmanager = null;
            GC.Collect();
        }

        private void linkEditor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}
