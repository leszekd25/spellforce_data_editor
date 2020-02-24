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
    public partial class MapMinimapSettings : Form
    {
        public MapMinimapSettings()
        {
            InitializeComponent();
            this.checkBox1.Checked = MainForm.mapedittool.Minimap.Visible;
        }

        public bool IsMinimapVisible()
        {
            return this.checkBox1.Checked;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(this.checkBox1.CheckState == CheckState.Checked)
            {
                MainForm.mapedittool.Minimap.ShowMinimap();
            }
            else
            {
                MainForm.mapedittool.Minimap.HideMinimap();
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int size = int.Parse(this.textBox1.Text);
            size = size <= 64 ? 64 : (size >= 1024 ? 1024 : size);
            MainForm.mapedittool.Minimap.ResizeMinimap(size);
        }
    }
}
