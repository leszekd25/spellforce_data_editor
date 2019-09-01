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
    public partial class MapPromptNewMap : Form
    {
        public ushort MapSize = 256;
        public MapGen.MapGenerator generator = new MapGen.MapGenerator();
        public bool use_generator = false;

        public MapPromptNewMap()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MapSize = Utility.TryParseUInt16(comboBox1.Text);
        }

        private void CheckGenerateTerrain_CheckedChanged(object sender, EventArgs e)
        {
            use_generator = CheckGenerateTerrain.Checked;
            PanelTerrainGenerator.Enabled = CheckGenerateTerrain.Checked;
        }

        public void UpdateGenerator()
        {
            generator.Width = MapSize;
            generator.Height = MapSize;
            generator.GradientCellSizeX = (int)Utility.TryParseUInt32(ErosionCellSizeX.Text);
            generator.GradientOffsetX = (int)Utility.TryParseUInt32(ErosionOffsetX.Text);
            generator.GradientErosionMeanX = Utility.TryParseFloat(ErosionStrengthX.Text);
            generator.GradientErosionSigmaX = Utility.TryParseFloat(ErosionVarianceX.Text);
            generator.GradientCellSizeY = (int)Utility.TryParseUInt32(ErosionCellSizeY.Text);
            generator.GradientOffsetY = (int)Utility.TryParseUInt32(ErosionOffsetY.Text);
            generator.GradientErosionMeanY = Utility.TryParseFloat(ErosionStrengthY.Text);
            generator.GradientErosionSigmaY = Utility.TryParseFloat(ErosionVarianceY.Text);
            generator.GradientBlurSize = (int)Utility.TryParseUInt32(ErosionBlurSize.Text);
            generator.GradientBlurSigma = Utility.TryParseFloat(ErosionBlurStrength.Text);
            generator.BaseZ = Utility.TryParseUInt16(BaseTerrainHeight.Text);
        }
    }
}
