 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SpellforceDataEditor.SF3D;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspectorTerrainTextureControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        enum TTMode { IDLE = 0, EDIT_BASE_TEXTURE, CHANGE_TILE_TEXTURE}

        TTMode editor_mode;
        int focused_base_texture = -1;
        int previously_focused_base_texture = -1;
        int previously_focused_tiletex = -1;


        public MapInspectorTerrainTextureControl()
        {
            InitializeComponent();
        }


        // 64x64 data
        public void GenerateBaseTexturePreviews()
        {
            PanelTexturePreview.Controls.Clear();
            SFMapTerrainTextureManager manager = map.heightmap.texture_manager;
            for(int i =0; i < 32; i++)
            {
                Button b = new Button();
                b.Size = new Size(70, 70);
                b.Location = new Point(3+(i%4)*74, 3+(i/4)*74);
                if(i==0)
                    b.Image = manager.CreateBitmapFromTexture(manager.base_texture_bank[0]);
                else
                    b.Image = manager.CreateBitmapFromTexture(manager.base_texture_bank[i+31]);
                PanelTexturePreview.Controls.Add(b);
            }
        }

        public void GenerateTileListEntries()
        {
            ListTiles.Items.Clear();
            for(int i = 0; i < 32; i++)
            {
                ListTiles.Items.Add(i.ToString() + ". " + map.heightmap.texture_manager.texture_tiledata[i].ind1.ToString());
            }
            for(int i = 32; i < 224; i++)
            {
                ListTiles.Items.Add(i.ToString()+". "
                    + map.heightmap.texture_manager.texture_tiledata[i].ind1.ToString() + ", "
                    + map.heightmap.texture_manager.texture_tiledata[i].ind2.ToString() + ", "
                    + map.heightmap.texture_manager.texture_tiledata[i].ind3.ToString() + ", ");
            }
        }

        private int GetCurrentFocusedTextureButton()
        {
            for (int i = 0; i < 32; i++)
                if (PanelTexturePreview.Controls[i].Focused)
                    return i;
            return -1;
        }

        private int GetCurrentFocusedTileTextureButton()
        {
            if (Tex1Button.Focused)
                return 0;
            if (Tex2Button.Focused)
                return 1;
            if (Tex3Button.Focused)
                return 2;
            return -1;
        }

        private void TimerControl_Tick(object sender, EventArgs e)
        {
            if (map == null)
                return;
            // control logic here
            int cur_focused = GetCurrentFocusedTextureButton();
            int tiletex_focused = GetCurrentFocusedTileTextureButton();

            switch (editor_mode)
            {
                case TTMode.IDLE:
                    if(tiletex_focused != -1)
                    {
                        if (ListTiles.SelectedIndex < 32)
                            break;
                        previously_focused_tiletex = tiletex_focused;
                        editor_mode = TTMode.CHANGE_TILE_TEXTURE;
                    }
                    else if((cur_focused == -1)&&(focused_base_texture != -1))
                    {
                        if (TexIDTextBox.Focused)
                            editor_mode = TTMode.EDIT_BASE_TEXTURE;
                        previously_focused_base_texture = focused_base_texture;
                        focused_base_texture = -1;
                    }
                    else if(cur_focused != -1)
                    {
                        focused_base_texture = cur_focused;
                        previously_focused_base_texture = cur_focused;
                        TexIDTextBox.Text = map.heightmap.texture_manager.texture_id[cur_focused].ToString();
                    }
                    break;
                case TTMode.EDIT_BASE_TEXTURE:
                    if (TexIDTextBox.Focused == false)
                        editor_mode = TTMode.IDLE;

                    break;
                case TTMode.CHANGE_TILE_TEXTURE:
                    if(tiletex_focused == -1)
                    {
                        if (cur_focused != -1)
                        {
                            if (previously_focused_tiletex == 0)
                            {
                                map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].ind1 = (byte)cur_focused;
                                Tex1Button.Image = ((Button)(PanelTexturePreview.Controls[cur_focused])).Image;
                                Tex1Button.Refresh();
                            }
                            else if (previously_focused_tiletex == 1)
                            {
                                map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].ind2 = (byte)cur_focused;
                                Tex2Button.Image = ((Button)(PanelTexturePreview.Controls[cur_focused])).Image;
                                Tex2Button.Refresh();
                            }
                            else if (previously_focused_tiletex == 2)
                            {
                                map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].ind3 = (byte)cur_focused;
                                Tex3Button.Image = ((Button)(PanelTexturePreview.Controls[cur_focused])).Image;
                                Tex3Button.Refresh();
                            }
                            map.heightmap.texture_manager.UpdateTileTexture(ListTiles.SelectedIndex);
                            TexPreview.Image = map.heightmap.texture_manager.CreateBitmapFromTexture(map.heightmap.texture_manager.texture_array[ListTiles.SelectedIndex]);
                            map.heightmap.texture_manager.FreeTileMemory(ListTiles.SelectedIndex);
                            TexPreview.Refresh();
                            MainForm.mapedittool.update_render = true;
                            focused_base_texture = cur_focused;
                            previously_focused_base_texture = cur_focused;

                            ListTiles.Items[ListTiles.SelectedIndex] = (ListTiles.SelectedIndex.ToString() + ". "
                                + map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].ind1.ToString() + ", "
                                + map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].ind2.ToString() + ", "
                                + map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].ind3.ToString() + ", ");
                        }
                        editor_mode = TTMode.IDLE;

                        previously_focused_tiletex = -1;
                    }
                    break;
                default:
                    break;
            }
        }

        private void MapInspectorTerrainTextureControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == false)
                TimerControl.Stop();
            else
                TimerControl.Start();
        }

        private void TexIDTextBox_Validated(object sender, EventArgs e)
        {
            if (map == null)
                return;

            int tex_id = 0;
            if ((previously_focused_base_texture != 0) && (previously_focused_base_texture != -1))
                tex_id = previously_focused_base_texture + 31;

            if (!(map.heightmap.texture_manager.SetBaseTexture(tex_id, Utility.TryParseInt32(TexIDTextBox.Text)+119)))
                TexIDTextBox.BackColor = Color.Red;
            else
            {
                map.heightmap.texture_manager.SetBaseTexture(tex_id - 31, Utility.TryParseInt32(TexIDTextBox.Text));
                TexIDTextBox.BackColor = Color.White;
                ((Button)(PanelTexturePreview.Controls[previously_focused_base_texture])).Image =
                    map.heightmap.texture_manager.CreateBitmapFromTexture(map.heightmap.texture_manager.base_texture_bank[tex_id]);


                PanelTexturePreview.Controls[previously_focused_base_texture].Refresh();
                MainForm.mapedittool.update_render = true;
            }

            editor_mode = TTMode.IDLE;
        }

        private void ListTiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectTexture(ListTiles.SelectedIndex);
        }

        private void SelectTexture(int tex_i)
        {
            if (map == null)
                return;

            if (tex_i == -1)
                return;
            if (tex_i < 32)
            {
                Tex1Button.Image = null;
                Tex2Button.Image = null;
                Tex3Button.Image = null;
                Tex1Weight.Text = "";
                Tex2Weight.Text = "";
                Tex3Weight.Text = "";
                TexPreview.Image = ((Button)(PanelTexturePreview.Controls[tex_i])).Image;
            }
            else if (tex_i < 224)
            {
                Tex1Button.Image = ((Button)(PanelTexturePreview.Controls[map.heightmap.texture_manager.texture_tiledata[tex_i].ind1])).Image;
                Tex2Button.Image = ((Button)(PanelTexturePreview.Controls[map.heightmap.texture_manager.texture_tiledata[tex_i].ind2])).Image;
                Tex3Button.Image = ((Button)(PanelTexturePreview.Controls[map.heightmap.texture_manager.texture_tiledata[tex_i].ind3])).Image;
                Tex1Weight.Text = map.heightmap.texture_manager.texture_tiledata[tex_i].weight1.ToString();
                Tex2Weight.Text = map.heightmap.texture_manager.texture_tiledata[tex_i].weight2.ToString();
                Tex3Weight.Text = map.heightmap.texture_manager.texture_tiledata[tex_i].weight3.ToString();
                map.heightmap.texture_manager.UpdateTileTexture(tex_i);
                TexPreview.Image = map.heightmap.texture_manager.CreateBitmapFromTexture(map.heightmap.texture_manager.texture_array[tex_i]);
                map.heightmap.texture_manager.FreeTileMemory(tex_i);
            }
            MovementCheck.Checked = map.heightmap.texture_manager.texture_tiledata[tex_i].blocks_movement;
            VisionCheck.Checked = map.heightmap.texture_manager.texture_tiledata[tex_i].blocks_vision;
        }

        private void Tex1Weight_TextChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (ListTiles.SelectedIndex < 32)
                return;
            map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].weight1 = Utility.TryParseUInt8(Tex1Weight.Text);
            map.heightmap.texture_manager.UpdateTileTexture(ListTiles.SelectedIndex);
            TexPreview.Image = map.heightmap.texture_manager.CreateBitmapFromTexture(map.heightmap.texture_manager.texture_array[ListTiles.SelectedIndex]);
            map.heightmap.texture_manager.FreeTileMemory(ListTiles.SelectedIndex);
            TexPreview.Refresh();
            MainForm.mapedittool.update_render = true;
        }

        private void Tex2Weight_TextChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (ListTiles.SelectedIndex < 32)
                return;
            map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].weight2 = Utility.TryParseUInt8(Tex2Weight.Text);
            map.heightmap.texture_manager.UpdateTileTexture(ListTiles.SelectedIndex);
            TexPreview.Image = map.heightmap.texture_manager.CreateBitmapFromTexture(map.heightmap.texture_manager.texture_array[ListTiles.SelectedIndex]);
            map.heightmap.texture_manager.FreeTileMemory(ListTiles.SelectedIndex);
            TexPreview.Refresh();
            MainForm.mapedittool.update_render = true;
        }

        private void Tex3Weight_TextChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (ListTiles.SelectedIndex < 32)
                return;
            map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].weight3 = Utility.TryParseUInt8(Tex3Weight.Text);
            map.heightmap.texture_manager.UpdateTileTexture(ListTiles.SelectedIndex);
            TexPreview.Image = map.heightmap.texture_manager.CreateBitmapFromTexture(map.heightmap.texture_manager.texture_array[ListTiles.SelectedIndex]);
            map.heightmap.texture_manager.FreeTileMemory(ListTiles.SelectedIndex);
            TexPreview.Refresh();
            MainForm.mapedittool.update_render = true;
        }

        private void MovementCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (ListTiles.SelectedIndex == -1)
                return;
            map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].blocks_movement = MovementCheck.Checked;
            if((ListTiles.SelectedIndex!=0)&&(ListTiles.SelectedIndex < 32))
                map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex+223].blocks_movement = MovementCheck.Checked;
        }

        private void VisionCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (ListTiles.SelectedIndex == -1)
                return;
            map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex].blocks_vision = VisionCheck.Checked;
            if ((ListTiles.SelectedIndex != 0) && (ListTiles.SelectedIndex < 32))
                map.heightmap.texture_manager.texture_tiledata[ListTiles.SelectedIndex+223].blocks_vision = VisionCheck.Checked;
        }

        public override void OnMouseDown(SFCoord clicked_pos, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                if (ListTiles.SelectedIndex == -1)
                    return;
                if (ListTiles.SelectedIndex == 0)
                    return;

                int tex_id = ListTiles.SelectedIndex;
                if (tex_id < 32)
                    tex_id += 223;

                int size = (int)Math.Ceiling(BrushControl.brush.size);
                BrushControl.brush.center = clicked_pos;
                SFCoord topleft = new SFCoord(clicked_pos.x - size, clicked_pos.y - size);
                SFCoord bottomright = new SFCoord(clicked_pos.x + size, clicked_pos.y + size);
                if (topleft.x < 0)
                    topleft.x = 0;
                if (topleft.y < 0)
                    topleft.y = 0;
                if (bottomright.x >= map.width)
                    bottomright.x = (short)(map.width - 1);
                if (bottomright.y >= map.height)
                    bottomright.y = (short)(map.height - 1);

                for (int i = topleft.x; i <= bottomright.x; i++)
                {
                    for (int j = topleft.y; j <= bottomright.y; j++)
                    {
                        if (BrushControl.brush.GetStrengthAt(new SFCoord(i, j)) == 0)
                            continue;
                        int fixed_j = map.height - j - 1;
                        if (map.heightmap.height_data[fixed_j * map.width + i] == 0)
                            continue;
                        if(CheckEditSimilar.Checked)
                        {
                            bool b_mov = map.heightmap.texture_manager.texture_tiledata[tex_id].blocks_movement ^
                                         map.heightmap.texture_manager.texture_tiledata[map.heightmap.tile_data[fixed_j * map.width + i]].blocks_movement;
                            if (b_mov)
                                continue;
                        }
                        map.heightmap.tile_data[fixed_j * map.width + i] = (byte)(tex_id);
                    }
                }

                map.heightmap.RebuildTerrainTexture(topleft, bottomright);
            }
            else if(button == MouseButtons.Right)
            {
                if ((clicked_pos.x < 0) || (clicked_pos.x >= map.width))
                    return;
                if ((clicked_pos.y <=0 ) || (clicked_pos.y > map.height))
                    return;
                int tex_id = map.heightmap.tile_data[(map.height - clicked_pos.y - 1) * map.width + clicked_pos.x];
                if (tex_id > 223)
                    tex_id -= 223;
                ListTiles.SelectedIndex = tex_id;
            }
        }
    }
}
