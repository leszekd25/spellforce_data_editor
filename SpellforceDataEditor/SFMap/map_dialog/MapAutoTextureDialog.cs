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
    public partial class MapAutoTextureDialog : Form
    {
        public class AutotextureTileData
        {
            public byte tile_id;
            public byte tile_weight;
            public TextBox tile_id_textbox;
            public Button tile_image;
            public TextBox tile_weight_textbox;

            public AutotextureTileData(byte id, byte  w)
            {
                tile_id = id;
                tile_weight = w;

            }
        }

        public SFMap map = null;
        public List<AutotextureTileData> tex_below = new List<AutotextureTileData>();
        int wsum_below = 0;
        public List<AutotextureTileData> tex_above = new List<AutotextureTileData>();
        int wsum_above = 0;
        public bool ready = false;

        public MapAutoTextureDialog()
        {
            InitializeComponent();
        }

        public void AddTexAbove()
        {
            AddTex(tex_above, PanelAboveThreshold, ButtonAddAbove);
        }

        public void AddTexBelow()
        {
            AddTex(tex_below, PanelBelowThreshold, ButtonAddBelow);
        }

        public void AddTex(List<AutotextureTileData> list, Panel panel_tex, Button add_button)
        {
            list.Add(new AutotextureTileData(0, 0));
            int cur_tex = list.Count - 1;
            // add button
            TextBox ti_tb = new TextBox();
            ti_tb.Location = new Point(add_button.Location.X, add_button.Location.Y - 23);
            ti_tb.Size = new Size(70, 20);
            ti_tb.Text = "0";
            ti_tb.Tag = cur_tex;
            ti_tb.Validated += new EventHandler(textID_Validated);
            Button tim_b = new Button();
            tim_b.Location = add_button.Location;
            tim_b.Size = add_button.Size;
            tim_b.Tag = cur_tex;
            tim_b.Image = map.heightmap.texture_manager.texture_tile_image[0];//CreateBitmapFromTexture(
                                       //map.heightmap.texture_manager.tile_texture_atlas[0]);
            tim_b.MouseClick += new MouseEventHandler(buttonTex_MouseClick);
            TextBox tw_tb = new TextBox();
            tw_tb.Location = new Point(add_button.Location.X, add_button.Location.Y + 73);
            tw_tb.Size = new Size(70, 20);
            tw_tb.Text = "0";
            tw_tb.Tag = cur_tex;
            tw_tb.Validated += new EventHandler(textWeight_Validated);
            list[cur_tex].tile_id_textbox = ti_tb;
            list[cur_tex].tile_image = tim_b;
            list[cur_tex].tile_weight_textbox = tw_tb;
            add_button.Location = new Point(add_button.Location.X + 73, add_button.Location.Y);
            panel_tex.Controls.Add(ti_tb);
            panel_tex.Controls.Add(tim_b);
            panel_tex.Controls.Add(tw_tb);
        }

        public void RemoveTex(int tex_index, List<AutotextureTileData> list, Panel panel_tex, Button add_button)
        {
            AutotextureTileData td = list[tex_index];
            td.tile_id_textbox.Validated -= new EventHandler(textID_Validated);
            panel_tex.Controls.Remove(td.tile_id_textbox);
            td.tile_image.MouseClick -= new MouseEventHandler(buttonTex_MouseClick);
            panel_tex.Controls.Remove(td.tile_image);
            td.tile_weight_textbox.Validated -= new EventHandler(textWeight_Validated);
            panel_tex.Controls.Remove(td.tile_weight_textbox);
            list.Remove(td);
            for(int i=tex_index;i<list.Count;i++)
            {
                list[i].tile_id_textbox.Location = new Point(list[i].tile_id_textbox.Location.X - 73, list[i].tile_id_textbox.Location.Y);
                list[i].tile_image.Tag = ((int)list[i].tile_image.Tag) - 1;
                list[i].tile_image.Location = new Point(list[i].tile_image.Location.X - 73, list[i].tile_image.Location.Y);
                list[i].tile_weight_textbox.Location = new Point(list[i].tile_weight_textbox.Location.X - 73, list[i].tile_weight_textbox.Location.Y);
            }
            add_button.Location = new Point(add_button.Location.X - 73, add_button.Location.Y);
        }

        private void ButtonAddAbove_Click(object sender, EventArgs e)
        {
            AddTexAbove();
        }

        private void ButtonAddBelow_Click(object sender, EventArgs e)
        {
            AddTexBelow();
        }

        private void buttonTex_MouseClick(object sender, MouseEventArgs e)
        {
            Button b_pressed = (Button)sender;
            if(e.Button == MouseButtons.Left)
            {
                if (b_pressed.Parent == PanelAboveThreshold)
                    RemoveTex((int)b_pressed.Tag, tex_above, PanelAboveThreshold, ButtonAddAbove);
                else
                    RemoveTex((int)b_pressed.Tag, tex_below, PanelBelowThreshold, ButtonAddBelow);
            }
        }

        private void textID_Validated(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int tex_index = (int)tb.Tag;
            AutotextureTileData td = null;
            if (tb.Parent == PanelAboveThreshold)
                td = tex_above[tex_index];
            else
                td = tex_below[tex_index];

            td.tile_id = Utility.TryParseUInt8(tb.Text);
            if (td.tile_id >= 224)
                td.tile_id = 0;
            tb.Text = td.tile_id.ToString();

            td.tile_image.Image = map.heightmap.texture_manager.texture_tile_image[td.tile_id];//map.heightmap.texture_manager.CreateBitmapFromTexture(
                                      //map.heightmap.texture_manager.tile_texture_atlas[td.tile_id]);

        }

        private void textWeight_Validated(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int tex_index = (int)tb.Tag;
            AutotextureTileData td = null;
            if (tb.Parent == PanelAboveThreshold)
                td = tex_above[tex_index];
            else
                td = tex_below[tex_index];

            td.tile_weight = Utility.TryParseUInt8(tb.Text);
            tb.Text = td.tile_weight.ToString();

        }

        private void SlopeValue_Validated(object sender, EventArgs e)
        {
            SlopeValueTrackbar.Value = (int)Math.Max(0, Math.Min(90, Utility.TryParseUInt32(SlopeValue.Text)));
        }

        private void SlopeValueTrackbar_ValueChanged(object sender, EventArgs e)
        {
            SlopeValue.Text = SlopeValueTrackbar.Value.ToString();
        }

        private byte ChooseTile(List<AutotextureTileData> list, int w_sum)
        {
            int r = MathUtils.Rand() % w_sum;
            int cur = 0;
            for (int i = 0; i < list.Count - 1; i++)
            {
                cur += list[i].tile_weight;
                if (cur > r)
                    return list[i].tile_id;
            }
            return list[list.Count - 1].tile_id;
        }

        public void GenerateMapTextures()
        {
            float slope_limit = (float)Math.Cos(SlopeValueTrackbar.Value * Math.PI / 180);

            for(int y=0;y<map.height;y++)
                for(int x=0;x<map.width; x++)
                {
                    if (map.heightmap.height_data[y * map.width + x] == 0)
                        continue;

                    OpenTK.Vector3 normal = map.heightmap.GetVertexNormal(x, y);
                    if (normal.Y < slope_limit)   // above
                        map.heightmap.tile_data[y * map.width + x] = ChooseTile(tex_above, wsum_above);
                    else                          // below
                        map.heightmap.tile_data[y * map.width + x] = ChooseTile(tex_below, wsum_below);
                }
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            int sum_weights = 0;
            for (int i = 0; i < tex_below.Count; i++)
            {
                sum_weights += tex_below[i].tile_weight;
                if (tex_below[i].tile_id == 0)
                {
                    MessageBox.Show("One of tiles below slope threshold is 0! Assign another tile ID to that tile.");
                    return;
                }
            }
            if (sum_weights == 0)
            {
                MessageBox.Show("Sum of weights for tiles below slope threshold is 0! Assign weight to at least one tile.");
                return;
            }
            wsum_below = sum_weights;

            sum_weights = 0;
            for (int i = 0; i < tex_above.Count; i++)
            {
                sum_weights += tex_above[i].tile_weight;
                if (tex_above[i].tile_id == 0)
                {
                    MessageBox.Show("One of tiles above slope threshold is 0! Assign another tile ID to that tile.");
                    return;
                }
            }
            if (sum_weights == 0)
            {
                MessageBox.Show("Sum of weights for tiles above slope threshold is 0! Assign weight to at least one tile.");
                return;
            }
            wsum_above = sum_weights;

            GenerateMapTextures();
            map.heightmap.RebuildTerrainTexture(new SFCoord(0, 0), new SFCoord(map.width - 1, map.height - 1));
            MainForm.mapedittool.ui.RedrawMinimap();
            MainForm.mapedittool.update_render = true;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
