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
    public partial class MapImportHeightmapDialog : Form
    {
        public SFMap map;
        ushort[] previous_map;
        byte scale;
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
                        map.heightmap.ImportHeights((Bitmap)PreviewPic.BackgroundImage, scale, offset);
                        map.heightmap.RebuildGeometry(new SFCoord(0, 0), new SFCoord(map.width - 1, map.height - 1));
                        MainForm.mapedittool.update_render = true;
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
                    if(PreviewPic.BackgroundImage != null)
                    {
                        map.heightmap.ImportHeights((Bitmap)PreviewPic.BackgroundImage, scale, offset);
                        map.heightmap.RebuildGeometry(new SFCoord(0, 0), new SFCoord(map.width - 1, map.height - 1));
                        MainForm.mapedittool.update_render = true;
                    }
                }
            }
        }
        bool restore_height = true;

        public MapImportHeightmapDialog()
        {
            InitializeComponent();
        }

        private void MapImportHeightmapDialog_Load(object sender, EventArgs e)
        {
            previous_map = new ushort[map.heightmap.height_data.Length];
            Array.Copy(map.heightmap.height_data, previous_map, map.heightmap.height_data.Length);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(OpenHMapBitMap.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PreviewPic.BackgroundImage = (Bitmap)Bitmap.FromFile(OpenHMapBitMap.FileName);
                    h_Scale = (byte)HMapScale.Value;
                }
                catch(Exception)
                {
                    Close();
                }
            }
        }

        private void MapImportHeightmapDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(restore_height)
            {
                map.heightmap.height_data = previous_map;
                map.heightmap.RebuildGeometry(new SFCoord(0, 0), new SFCoord(map.width-1, map.height-1));
                MainForm.mapedittool.update_render = true;
            }
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
            if(PreviewPic.BackgroundImage != null)
            {
                HMapScale.ValueChanged -= new EventHandler(HMapScale_ValueChanged);
                h_Scale = Utility.TryParseUInt8(HMapScaleText.Text, h_Scale);
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
                h_Offset = Utility.TryParseUInt8(HMapOffsetText.Text, h_Scale);
                HMapOffset.Value = Math.Min(HMapOffset.Maximum, Math.Max((byte)0, h_Offset));
                HMapOffset.ValueChanged += new EventHandler(HMapOffset_ValueChanged);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            restore_height = false;
            Close();
        }
    }
}
