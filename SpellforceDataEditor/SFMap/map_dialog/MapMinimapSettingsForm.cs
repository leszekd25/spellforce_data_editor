using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_dialog
{
    public partial class MapMinimapSettingsForm : Form
    {
        public SFMap map;
        public SFMapMinimap new_minimap;
        public SFMapMinimapSource new_source;

        public MapMinimapSettingsForm()
        {
            InitializeComponent();
        }
        private void MapMinimapSettingsForm_Shown(object sender, EventArgs e)
        {
            SetSource(map.metadata.minimap_source);
        }

        private void SetSource(SFMapMinimapSource mms)
        {
            new_source = mms;

            switch(mms)
            {
                case SFMapMinimapSource.ORIGINAL:
                    UseOriginalMap.Checked = true;
                    LabelOptionDescription.Text = "Exported minimap image will be the same as imported minimap image.";

                    ButtonImportCustomMinimap.Enabled = true;

                    new_minimap = map.metadata.original_minimap;
                    break;
                case SFMapMinimapSource.EDITOR:
                    UseEditorMap.Checked = true;
                    LabelOptionDescription.Text = "Editor minimap will be exported. Note that it will be downscaled to reduce map file size.";

                    ButtonImportCustomMinimap.Enabled = true;

                    new_minimap = new SFMapMinimap();
                    new_minimap.FromBitmap(
                        MainForm.mapedittool.ui.minimap_tex.ToBitmap(
                            new SF3D.SFTexture.SFTextureToBitmapArgs() 
                            { 
                                ConversionType = SF3D.SFTexture.SFTextureToBitmapArgType.DIMENSION, 
                                DimWidth = 128, 
                                DimHeight = 128 }));
                    break;
                case SFMapMinimapSource.CUSTOM:
                    UseCustomMap.Checked = true;
                    LabelOptionDescription.Text = "You can select a custom image to embed in the map file. Low quality downscaling will be performed. It is advised that you perform downscaling yourself, before importing the image here. Image size should be 128x128.";

                    ButtonImportCustomMinimap.Enabled = true;

                    new_minimap = map.metadata.custom_minimap;
                    break;
            }

            ShowMinimap();
        }

        private void ShowMinimap()
        {
            if(new_minimap == null)
            {
                MinimapPicture.Image = MinimapPicture.InitialImage;
                return;
            }

            MinimapPicture.Image = new_minimap.ToBitmap();
        }

        private void UseOriginalMap_Click(object sender, EventArgs e)
        {
            SetSource(SFMapMinimapSource.ORIGINAL);
        }

        private void UseEditorMap_Click(object sender, EventArgs e)
        {
            SetSource(SFMapMinimapSource.EDITOR);

        }

        private void CustomMap_Click(object sender, EventArgs e)
        {
            SetSource(SFMapMinimapSource.CUSTOM);
        }

        private void ButtonImportCustomMinimap_Click(object sender, EventArgs e)
        {
            if (ImportMinimapDialog.ShowDialog() != DialogResult.OK)
                return;

            System.Drawing.Bitmap bmp = null;
            try
            {
                bmp = new Bitmap(ImportMinimapDialog.FileName);
                bmp = new Bitmap(bmp, new Size(128, 128));
            }
            catch(Exception ex)
            {
                return;
            }

            if (bmp == null)
                return;

            new_minimap = new SFMapMinimap();
            new_minimap.FromBitmap(bmp);

            map.metadata.custom_minimap = new_minimap;

            SetSource(SFMapMinimapSource.CUSTOM);
        }
    }
}
