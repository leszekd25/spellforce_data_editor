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
        public SFEngine.SFMap.MapGen.MapGenerator generator = new SFEngine.SFMap.MapGen.MapGenerator();
        public bool use_generator = false;

        public MapPromptNewMap()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MapSize = SFEngine.Utility.TryParseUInt16(comboBox1.Text);
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
            generator.GradientCellSizeX = (int)SFEngine.Utility.TryParseUInt32(ErosionCellSizeX.Text);
            generator.GradientOffsetX = (int)SFEngine.Utility.TryParseUInt32(ErosionOffsetX.Text);
            generator.GradientErosionMeanX = SFEngine.Utility.TryParseFloat(ErosionStrengthX.Text);
            generator.GradientErosionSigmaX = SFEngine.Utility.TryParseFloat(ErosionVarianceX.Text);
            generator.GradientCellSizeY = (int)SFEngine.Utility.TryParseUInt32(ErosionCellSizeY.Text);
            generator.GradientOffsetY = (int)SFEngine.Utility.TryParseUInt32(ErosionOffsetY.Text);
            generator.GradientErosionMeanY = SFEngine.Utility.TryParseFloat(ErosionStrengthY.Text);
            generator.GradientErosionSigmaY = SFEngine.Utility.TryParseFloat(ErosionVarianceY.Text);
            generator.GradientBlurSize = (int)SFEngine.Utility.TryParseUInt32(ErosionBlurSize.Text);
            generator.GradientBlurSigma = SFEngine.Utility.TryParseFloat(ErosionBlurStrength.Text);
            generator.BaseZ = SFEngine.Utility.TryParseUInt16(BaseTerrainHeight.Text);
        }
    }
}
