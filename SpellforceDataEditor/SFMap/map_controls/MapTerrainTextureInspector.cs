using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public enum TerrainTileType { NONE, BASE, CUSTOM};

    public partial class MapTerrainTextureInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        TerrainTileType inspectortype = TerrainTileType.NONE;

        public MapTerrainTextureInspector()
        {
            InitializeComponent();
            SelectedCustomTileMixImage1.ID = 0;
            SelectedCustomTileMixImage1.delegate_onpress = OnCustomTileMixPress;
            SelectedCustomTileMixImage2.ID = 1;
            SelectedCustomTileMixImage2.delegate_onpress = OnCustomTileMixPress;
            SelectedCustomTileMixImage3.ID = 2;
            SelectedCustomTileMixImage3.delegate_onpress = OnCustomTileMixPress;
        }

        public void OnCustomTileMixPress(int ID)
        {
            map_dialog.MapSelectTile tileselectdialog = new map_dialog.MapSelectTile(map, map_dialog.MapTileSelectType.BASE);
            tileselectdialog.ShowDialog();

            if(tileselectdialog.SelectedTile != Utility.NO_INDEX)
            {
                byte selected_tile = (byte)((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile;
                if (ID == 0)
                {
                    map.heightmap.texture_manager.texture_tiledata[selected_tile].ind1 = (byte)tileselectdialog.SelectedTile;
                    SelectedCustomTileMixImage1.SetImage(
                        map.heightmap.texture_manager.texture_tile_image[map.heightmap.texture_manager.texture_tiledata[selected_tile].ind1],
                        map.heightmap.texture_manager.texture_tiledata[selected_tile].ind1);
                }
                else if (ID == 1)
                {
                    map.heightmap.texture_manager.texture_tiledata[selected_tile].ind2 = (byte)tileselectdialog.SelectedTile;
                    SelectedCustomTileMixImage2.SetImage(
                         map.heightmap.texture_manager.texture_tile_image[map.heightmap.texture_manager.texture_tiledata[selected_tile].ind2],
                         map.heightmap.texture_manager.texture_tiledata[selected_tile].ind2);
                }
                else if (ID == 2)
                {
                    map.heightmap.texture_manager.texture_tiledata[selected_tile].ind3 = (byte)tileselectdialog.SelectedTile;
                    SelectedCustomTileMixImage3.SetImage(
                         map.heightmap.texture_manager.texture_tile_image[map.heightmap.texture_manager.texture_tiledata[selected_tile].ind3],
                         map.heightmap.texture_manager.texture_tiledata[selected_tile].ind3);
                }
                else
                    throw new Exception("MapTerrainTextureInspector.OnCustomTileMixPress(): Invalid button ID!");

                map.heightmap.texture_manager.UpdateUniformTileData(selected_tile, selected_tile);
                map.heightmap.texture_manager.RefreshTilePreview(selected_tile);
                SelectedCustomTileTex.SetImage(map.heightmap.texture_manager.texture_tile_image[selected_tile], 0);

                foreach (MapTerrainTextureControl c in PanelTiles.Controls)
                {
                    if (c.ID == (int)selected_tile)
                    {
                        c.SetImage(map.heightmap.texture_manager.texture_tile_image[selected_tile], selected_tile);
                        break;
                    }
                }
            }
        }

        public void OnBaseTexturePress(int ID)
        {
            if(ID == 0)
            {
                ((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile = 0;
                PanelTileProperties.Enabled = false;
                PanelTileMixer.Enabled = false;
                PanelButtons.Enabled = false;
                return;
            }
            ((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile = ID+223;
            PanelTileProperties.Enabled = true;
            TileBlocksMovement.Checked = map.heightmap.texture_manager.texture_tiledata[ID + 223].blocks_movement;
            TileBlocksVision.Checked = map.heightmap.texture_manager.texture_tiledata[ID + 223].blocks_vision;
        }

        public void OnCustomTexturePress(int ID)
        {
            ((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile = ID;
            PanelTileProperties.Enabled = true;
            PanelTileMixer.Enabled = true;
            TileBlocksMovement.Checked = map.heightmap.texture_manager.texture_tiledata[ID].blocks_movement;
            TileBlocksVision.Checked = map.heightmap.texture_manager.texture_tiledata[ID].blocks_vision;

            SelectedCustomTileMixImage1.SetImage(
                map.heightmap.texture_manager.texture_tile_image[map.heightmap.texture_manager.texture_tiledata[ID].ind1],
                map.heightmap.texture_manager.texture_tiledata[ID].ind1);
            SelectedCustomTileMixImage2.SetImage(
                 map.heightmap.texture_manager.texture_tile_image[map.heightmap.texture_manager.texture_tiledata[ID].ind2],
                 map.heightmap.texture_manager.texture_tiledata[ID].ind2);
            SelectedCustomTileMixImage3.SetImage(
                 map.heightmap.texture_manager.texture_tile_image[map.heightmap.texture_manager.texture_tiledata[ID].ind3],
                 map.heightmap.texture_manager.texture_tiledata[ID].ind3);
            TexWeight1.Text = map.heightmap.texture_manager.texture_tiledata[ID].weight1.ToString();
            TexWeight2.Text = map.heightmap.texture_manager.texture_tiledata[ID].weight2.ToString();
            TexWeight3.Text = map.heightmap.texture_manager.texture_tiledata[ID].weight3.ToString();

            SelectedCustomTileTex.SetImage(map.heightmap.texture_manager.texture_tile_image[ID], ID);
        }

        public void LoadBaseTextures()
        {
            PanelTileMixer.Visible = false;
            PanelButtons.Visible = false;
            PanelTiles.Size = new Size(280, 379);
            PanelTiles.Controls.Clear();

            for(int i=1;i<32; i++)
            {
                MapTerrainTextureControl mttc = new MapTerrainTextureControl();
                mttc.ID = i;
                mttc.delegate_onpress = OnBaseTexturePress;
                mttc.SetImage(map.heightmap.texture_manager.texture_tile_image[i], i);
                PanelTiles.Controls.Add(mttc);
                mttc.ResizeWidth(46);
                mttc.Location = new Point(((i - 1) % 6) * 46, ((i - 1) / 6) * 62);
            }

            PanelTileProperties.Enabled = false;
        }

        public void LoadCustomTextures()
        {
            PanelTileMixer.Visible = true;
            PanelButtons.Visible = true;
            PanelTiles.Size = new Size(280, 277);
            PanelTiles.Controls.Clear();
            int cur_i = 0;
            for(int i = 32; i < 224; i++)
            {
                if(map.heightmap.texture_manager.tile_defined[i])
                {
                    MapTerrainTextureControl mttc = new MapTerrainTextureControl();
                    mttc.ID = i;
                    mttc.delegate_onpress = OnCustomTexturePress;
                    mttc.SetImage(map.heightmap.texture_manager.texture_tile_image[i], i);
                    PanelTiles.Controls.Add(mttc);
                    mttc.ResizeWidth(46);
                    mttc.Location = new Point((cur_i % 6) * 46, (cur_i / 6) * 62);
                    cur_i += 1;
                }
            }

            PanelTileMixer.Enabled = false;
            PanelTileProperties.Enabled = false;
        }

        public void SetInspectorType(TerrainTileType type)
        {
            if (type == TerrainTileType.NONE)
                throw new Exception("MapTerrainTextureInspector.SetInspectorType(): Invalid inspector type!");
            if (type == inspectortype)
                return;

            inspectortype = type;
            if (type == TerrainTileType.BASE)
                LoadBaseTextures();
            else
                LoadCustomTextures();
        }

        public void SelectTileType(byte ttype)
        {
            if (ttype > 223)
                ttype = (byte)(ttype - 223);

            if(ttype == 0)
            {
                OnBaseTexturePress(0);
                return;
            }
            if(ttype < 32)
            {
                SetInspectorType(TerrainTileType.BASE);
                OnBaseTexturePress(ttype);
                PanelTiles.Controls[ttype - 1].Focus();
            }
            else
            {
                SetInspectorType(TerrainTileType.CUSTOM);
                OnCustomTexturePress(ttype);
                foreach(MapTerrainTextureControl c in PanelTiles.Controls)
                    if(c.ID == ttype)
                    {
                        c.Focus();
                        break;
                    }
            }
        }

        private void TexWeight1_Validated(object sender, EventArgs e)
        {
            byte selected_tile = (byte)((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile;
            TexWeight1.Text = Utility.TryParseUInt8(TexWeight1.Text).ToString();
            map.heightmap.texture_manager.texture_tiledata[selected_tile].weight1 = Utility.TryParseUInt8(TexWeight1.Text);

            map.heightmap.texture_manager.UpdateUniformTileData(selected_tile, selected_tile);
            map.heightmap.texture_manager.RefreshTilePreview(selected_tile);
            SelectedCustomTileTex.SetImage(map.heightmap.texture_manager.texture_tile_image[selected_tile], 0);

            foreach(MapTerrainTextureControl c in PanelTiles.Controls)
            {
                if(c.ID == (int)selected_tile)
                {
                    c.SetImage(map.heightmap.texture_manager.texture_tile_image[selected_tile], selected_tile);
                    break;
                }
            }
        }

        private void TexWeight2_Validated(object sender, EventArgs e)
        {
            byte selected_tile = (byte)((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile;
            TexWeight2.Text = Utility.TryParseUInt8(TexWeight2.Text).ToString();
            map.heightmap.texture_manager.texture_tiledata[selected_tile].weight2 = Utility.TryParseUInt8(TexWeight2.Text);

            map.heightmap.texture_manager.UpdateUniformTileData(selected_tile, selected_tile);
            map.heightmap.texture_manager.RefreshTilePreview(selected_tile);
            SelectedCustomTileTex.SetImage(map.heightmap.texture_manager.texture_tile_image[selected_tile], 0);

            foreach (MapTerrainTextureControl c in PanelTiles.Controls)
            {
                if (c.ID == (int)selected_tile)
                {
                    c.SetImage(map.heightmap.texture_manager.texture_tile_image[selected_tile], selected_tile);
                    break;
                }
            }
        }

        private void TexWeight3_Validated(object sender, EventArgs e)
        {
            byte selected_tile = (byte)((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile;
            TexWeight3.Text = Utility.TryParseUInt8(TexWeight3.Text).ToString();
            map.heightmap.texture_manager.texture_tiledata[selected_tile].weight3 = Utility.TryParseUInt8(TexWeight3.Text);

            map.heightmap.texture_manager.UpdateUniformTileData(selected_tile, selected_tile);
            map.heightmap.texture_manager.RefreshTilePreview(selected_tile);
            SelectedCustomTileTex.SetImage(map.heightmap.texture_manager.texture_tile_image[selected_tile], 0);

            foreach (MapTerrainTextureControl c in PanelTiles.Controls)
            {
                if (c.ID == (int)selected_tile)
                {
                    c.SetImage(map.heightmap.texture_manager.texture_tile_image[selected_tile], selected_tile);
                    break;
                }
            }
        }

        private void ButtonAddCustomTile_Click(object sender, EventArgs e)
        {
            int new_tile = 32;
            while(true)
            {
                if (map.heightmap.texture_manager.tile_defined[new_tile] == false)
                    break;
                new_tile += 1;
            }
            map.heightmap.texture_manager.tile_defined[new_tile] = true;
            LoadCustomTextures();
        }

        private void ButtonRemoveCustomTile_Click(object sender, EventArgs e)
        {
            byte selected_tile = (byte)((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile;
            if (selected_tile == 0)
                return;

            if(map.heightmap.texture_manager.tile_defined[selected_tile] == true)
            {
                ((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile = 0;
                map.heightmap.texture_manager.tile_defined[selected_tile] = false;
                LoadCustomTextures();
            }
        }

        private void TileBlocksMovement_CheckedChanged(object sender, EventArgs e)
        {
            byte selected_tile = (byte)((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile;
            map.heightmap.texture_manager.texture_tiledata[selected_tile].blocks_movement = TileBlocksMovement.Checked;
        }

        private void TileBlocksVision_CheckedChanged(object sender, EventArgs e)
        {
            byte selected_tile = (byte)((MapEdit.MapTerrainTextureEditor)MainForm.mapedittool.selected_editor).SelectedTile;
            map.heightmap.texture_manager.texture_tiledata[selected_tile].blocks_vision = TileBlocksVision.Checked;
        }
    }
}
