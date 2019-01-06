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
using SpellforceDataEditor.SFMod;

namespace SpellforceDataEditor.special_forms
{
    public partial class ModCreatorForm : Form
    {
        SFMod.SFMod mod = new SFMod.SFMod();

        public ModCreatorForm()
        {
            InitializeComponent();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            LabelAssetsInfo.Text = "";
            LabelBytecodeInfo.Text = "";
            LabelDataInfo.Text = "";
            LabelModInfo.Text = "";
            UpdateDataInfo();
            UpdateAssetsInfo();
            UpdateModInfo();
        }

        private void UpdateDataInfo()
        {
            LabelDataInfo.Text = "CFF info:\r\n"+mod.data.ToString();
        }

        private void UpdateAssetsInfo()
        {
            LabelAssetsInfo.Text = "Assets info:\r\n" + mod.assets.ToString();
        }

        private void UpdateModInfo()
        {
            LabelModInfo.Text = "Mod info as it appears on mod selection screen:\r\n" + mod.info.ToString();
        }

        private void ButtonChooseData_Click(object sender, EventArgs e)
        {
            if (OpenCFFFile.ShowDialog() == DialogResult.OK)
            {
                StatusText.Text = "Processing... Please wait";
                StatusBar.Refresh();
                mod.data.GenerateDiff("backup\\GameData.cff", OpenCFFFile.FileName);
                UpdateDataInfo();
                StatusText.Text = "Ready";
            }
        }

        private void ButtonChooseAssets_Click(object sender, EventArgs e)
        {
            if(OpenAssetDirectory.ShowDialog() == DialogResult.OK)
            {
                mod.assets.unpacked_asset_directory = OpenAssetDirectory.SelectedPath;
                mod.assets.GenerateAssetInfo();
                UpdateAssetsInfo();
            }
        }

        private void ButtonInfo_Click(object sender, EventArgs e)
        {
            SFMod.mod_controls.ModInfoForm info = new SFMod.mod_controls.ModInfoForm();
            info.ShowDialog();
            if (info.DialogResult == DialogResult.OK)
                mod.info = info.mod_info;
            UpdateModInfo();
        }

        private void ButtonFinish_Click(object sender, EventArgs e)
        {
            string mod_name = TextBoxFileName.Text;
            if (mod_name == "")
            {
                StatusText.Text = "Choose a filename first";
                return;
            }
            // ask if replace first!
            if(File.Exists("mods\\" + mod_name + ".sfmd"))
            {
                SFMod.SFMod prev_mod = new SFMod.SFMod();
                prev_mod.LoadOnlyInfo("mods\\" + mod_name + ".sfmd");
                DialogResult result = MessageBox.Show("There already exists a mod by this name. Press Yes to save changes and increase revision number by 1, No to save changes AND revision number, or Cancel to abort.", "Confirm to replace", MessageBoxButtons.YesNoCancel);
                if(result == DialogResult.Cancel)
                {
                    StatusText.Text = "Aborted";
                    return;
                }
                if (result == DialogResult.Yes)
                    mod.info.Revision = prev_mod.info.Revision + 1;
            }

            StatusText.Text = "Processing...";
            StatusBar.Refresh();
            mod.Save("mods\\"+mod_name+".sfmd");
            StatusText.Text = "Successfully created mod " + mod_name;
        }

        // NEEDED TO CREATE MOD:
        // 1. load both orig and new cff files and perform simple diff on them, then save changes
        // (separate logic from category manager...)
        // 2. load files and write them to the mod file
        // do this in 16 MB chunks
        // 3. write patches to file (this one is easy) (LOL)
    }
}
