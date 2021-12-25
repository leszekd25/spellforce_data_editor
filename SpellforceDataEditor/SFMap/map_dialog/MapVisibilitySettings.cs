using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SF3D.SFRender;


namespace SpellforceDataEditor.SFMap.map_dialog
{
    public partial class MapVisibilitySettings : Form
    {
        public SFEngine.SFMap.SFMap map;
        bool ready = false;

        public MapVisibilitySettings()
        {
            InitializeComponent();
            checkBox1.Checked = SFEngine.Settings.UnitsVisible;
            checkBox2.Checked = SFEngine.Settings.BuildingsVisible;
            checkBox3.Checked = SFEngine.Settings.ObjectsVisible;
            checkBox4.Checked = SFEngine.Settings.DecorationsVisible;
            checkBox5.Checked = SFEngine.Settings.LakesVisible;
            checkBox6.Checked = SFEngine.Settings.VisualizeHeight;
            checkBox7.Checked = SFEngine.Settings.OverlaysVisible;
            checkBox8.Checked = SFEngine.Settings.DisplayGrid;
            checkBox9.Checked = MainForm.mapedittool.ui.GetMinimapVisible();
            checkBox10.Checked = MainForm.mapedittool.ui.GetMinimapIconsVisible();
            checkBox11.Checked = SFEngine.Settings.DynamicMap;

            button1.BackColor = Color.FromArgb(((byte)SFEngine.Settings.GridColor.X*255), 
                                               ((byte)SFEngine.Settings.GridColor.Y*255), 
                                               ((byte)SFEngine.Settings.GridColor.Z*255));

            ready = true;
        }

        public void UpdateVisibility()
        {
            map.heightmap.SetVisibilitySettings();
            MainForm.mapedittool.update_render = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SFEngine.Settings.UnitsVisible = checkBox1.Checked;
            if (ready)
                UpdateVisibility();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            SFEngine.Settings.BuildingsVisible = checkBox2.Checked;
            if (ready)
                UpdateVisibility();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            SFEngine.Settings.ObjectsVisible = checkBox3.Checked;
            if (ready)
                UpdateVisibility();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            SFEngine.Settings.DecorationsVisible = checkBox4.Checked;
            if (ready)
                UpdateVisibility();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            SFEngine.Settings.LakesVisible = checkBox5.Checked;
            MainForm.mapedittool.update_render = true;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            SFEngine.Settings.VisualizeHeight = checkBox6.Checked;
            SFRenderEngine.RecompileMainShaders();
            MainForm.mapedittool.update_render = true;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            SFEngine.Settings.OverlaysVisible = checkBox7.Checked;
            MainForm.mapedittool.update_render = true;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            SFEngine.Settings.DisplayGrid = checkBox8.Checked;
            SFRenderEngine.RecompileMainShaders();
            MainForm.mapedittool.update_render = true;
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.mapedittool.ui.SetMinimapVisible(checkBox9.Checked);
            checkBox10.Enabled = checkBox9.Checked;
            MainForm.mapedittool.update_render = true;
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.mapedittool.ui.SetMinimapIconsVisible(checkBox10.Checked);
            MainForm.mapedittool.update_render = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(GridColorPicker.ShowDialog() == DialogResult.OK)
            {
                SFEngine.Settings.GridColor = new OpenTK.Vector4(
                    GridColorPicker.Color.R,
                    GridColorPicker.Color.G,
                    GridColorPicker.Color.B,
                    255) / 255f;
                button1.BackColor = GridColorPicker.Color;
                MainForm.mapedittool.update_render = true;
            }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            SFEngine.Settings.DynamicMap = checkBox11.Checked;
            if (SFEngine.Settings.DynamicMap)
                MainForm.mapedittool.EnableAnimation(true);
            else
                MainForm.mapedittool.DisableAnimation(true);
            MainForm.mapedittool.update_render = true;
        }
    }
}
