using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_dialog
{
    public partial class MapExportHeightmapDialog : Form
    {
        public SFEngine.SFMap.SFMap map;
        byte scale = 0;
        byte offset = 55;
        byte h_Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    if (PreviewPic.BackgroundImage != null)
                    {
                        Bitmap bmp = (Bitmap)PreviewPic.BackgroundImage;
                        map.heightmap.ExportHeights(bmp, scale, offset);
                        PreviewPic.Invalidate();
                    }
                }
            }
        }
        byte h_Offset
        {
            get { return offset; }
            set
            {
                if (offset != value)
                {
                    offset = value;
                    if (PreviewPic.BackgroundImage != null)
                    {
                        Bitmap bmp = (Bitmap)PreviewPic.BackgroundImage;
                        map.heightmap.ExportHeights(bmp, scale, offset);
                        PreviewPic.Invalidate();
                    }
                }
            }
        }

        public MapExportHeightmapDialog()
        {
            InitializeComponent();
        }

        private void MapExportHeightmapDialog_Load(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(map.width, map.height);
            PreviewPic.BackgroundImage = bmp;
            h_Scale = 50;
        }

        private void HMapScale_ValueChanged(object sender, EventArgs e)
        {
            if (PreviewPic.BackgroundImage != null)
            {
                h_Scale = (byte)HMapScale.Value;
                HMapScaleText.Text = h_Scale.ToString();
            }
        }

        private void HMapScaleText_Validated(object sender, EventArgs e)
        {
            if (PreviewPic.BackgroundImage != null)
            {
                HMapScale.ValueChanged -= new EventHandler(HMapScale_ValueChanged);
                h_Scale = SFEngine.Utility.TryParseUInt8(HMapScaleText.Text, h_Scale);
                HMapScale.Value = Math.Min(HMapScale.Maximum, Math.Max((byte)1, h_Scale));
                HMapScale.ValueChanged += new EventHandler(HMapScale_ValueChanged);
            }
        }

        private void HMapOffset_ValueChanged(object sender, EventArgs e)
        {
            if (PreviewPic.BackgroundImage != null)
            {
                h_Offset = (byte)HMapOffset.Value;
                HMapOffsetText.Text = h_Offset.ToString();
            }
        }

        private void HMapOffsetText_Validated(object sender, EventArgs e)
        {
            if (PreviewPic.BackgroundImage != null)
            {
                HMapOffset.ValueChanged -= new EventHandler(HMapOffset_ValueChanged);
                h_Offset = SFEngine.Utility.TryParseUInt8(HMapOffsetText.Text, h_Scale);
                HMapOffset.Value = Math.Min(HMapOffset.Maximum, Math.Max((byte)0, h_Offset));
                HMapOffset.ValueChanged += new EventHandler(HMapOffset_ValueChanged);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ((Bitmap)PreviewPic.BackgroundImage).Save("heightmap.png", System.Drawing.Imaging.ImageFormat.Png);
            Close();
        }
    }
}
