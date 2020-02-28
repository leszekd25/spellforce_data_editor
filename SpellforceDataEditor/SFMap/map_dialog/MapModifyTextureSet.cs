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
    public partial class MapModifyTextureSet : Form
    {
        SFMap map = null;
        int selected_base = -1;

        public MapModifyTextureSet()
        {
            InitializeComponent();
        }

        public MapModifyTextureSet(SFMap m)
        {
            map = m;
            InitializeComponent();
            LoadTextures();
        }

        private void LoadTextures()
        {
            for (int i = 1; i < 32; i++)
            {
                map_controls.MapTerrainTextureControl mttc = new map_controls.MapTerrainTextureControl();
                mttc.ID = i;
                mttc.delegate_onpress = OnTextureSetPress;
                mttc.SetImage(map.heightmap.texture_manager.texture_tile_image[i], i);
                PanelTextureSet.Controls.Add(mttc);
                mttc.ResizeWidth(54);
                mttc.Location = new Point(((i - 1) % 7) * 54, ((i - 1) / 7) * 70);
            }

            for (int i = 1; i <= SFMapTerrainTextureManager.TEXTURES_AVAILABLE; i++)
            {
                map_controls.MapTerrainTextureControl mttc = new map_controls.MapTerrainTextureControl();
                mttc.ID = i;
                mttc.delegate_onpress = OnTextureAvailablePress;
                mttc.SetImage(map.heightmap.texture_manager.texture_base_image[i], i);
                PanelAllTextures.Controls.Add(mttc);
                mttc.ResizeWidth(54);
                mttc.Location = new Point(((i - 1) % 7) * 54, ((i - 1) / 7) * 70);
            }
        }

        private void OnTextureSetPress(int ID)
        {
            selected_base = ID;
        }

        private void OnTextureAvailablePress(int ID)
        {
            if (selected_base == -1)
                return;

            map.heightmap.texture_manager.SetBaseTexture(selected_base, ID);
            foreach (map_controls.MapTerrainTextureControl c in PanelTextureSet.Controls)
            {
                if (c.ID == selected_base)
                {
                    c.SetImage(map.heightmap.texture_manager.texture_tile_image[selected_base], selected_base);
                    break;
                }
            }
            MainForm.mapedittool.ui.RedrawMinimap(map);
            MainForm.mapedittool.update_render = true;
        }
    }
}
