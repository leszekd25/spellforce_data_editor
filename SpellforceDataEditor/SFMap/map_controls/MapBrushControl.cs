using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapBrushControl : UserControl
    {
        public MapEdit.MapBrush brush { get; private set; } = new MapEdit.MapBrush();

        public MapBrushControl()
        {
            InitializeComponent();
            brush.size = 1;
            brush.shape = MapEdit.BrushShape.SQUARE;
            brush.interpolation_mode = MapEdit.BrushInterpolationMode.CONSTANT;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            brush.size = Utility.TryParseFloat(textBox1.Text, 1f);
            trackBar1.Value = (int)(brush.size * 100);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            brush.size = trackBar1.Value/100;
            textBox1.Text = brush.size.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
                brush.shape = (MapEdit.BrushShape)comboBox1.SelectedIndex;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
                brush.interpolation_mode = (MapEdit.BrushInterpolationMode)comboBox2.SelectedIndex;

        }
    }
}
