using System.Drawing;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_dialog
{
    public enum MapTileSelectType { BASE, BASECUSTOM, AVAILABLETEX }

    public partial class MapSelectTile : Form
    {
        SFEngine.SFMap.SFMap map = null;
        MapTileSelectType SelectionType { get; set; } = MapTileSelectType.BASE;
        public int SelectedTile { get; private set; } = -1;

        public MapSelectTile()
        {
            InitializeComponent();
        }

        public MapSelectTile(SFEngine.SFMap.SFMap m, MapTileSelectType t)
        {
            map = m;
            SelectionType = t;
            InitializeComponent();

            if (SelectionType == MapTileSelectType.BASE)
            {
                GenerateTilesBase();
            }
        }

        public void OnTileSelectBase(int ID)
        {
            SelectedTile = ID;
            Close();
        }

        public void GenerateTilesBase()
        {
            for (int i = 0; i < 32; i++)
            {
                map_controls.MapTerrainTextureControl mttc = new map_controls.MapTerrainTextureControl();
                mttc.ID = i;
                mttc.delegate_onpress = OnTileSelectBase;
                mttc.SetImage(map.heightmap.texture_manager.texture_tile_image[i], i);
                PanelTiles.Controls.Add(mttc);
                mttc.ResizeWidth(64);
                mttc.Location = new Point((i % 10) * 70, (i / 10) * 84);
            }
        }
    }
}
