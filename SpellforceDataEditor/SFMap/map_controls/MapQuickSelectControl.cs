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
    public partial class MapQuickSelectControl : UserControl
    {
        SFMapQuickSelectHelper internal_qs_ref = null;
        public SFMapQuickSelectHelper QsRef
        {
            get
            { 
                return internal_qs_ref;
            }
            set
            {
                if (value == null)
                    Visible = false;
                else
                    Visible = true;
                internal_qs_ref = value;

                UpdateIDs();
            }
        }
        public MapQuickSelectControl()
        {
            InitializeComponent();
        }

        public void UpdateIDs()
        {
            if (QsRef == null)
                return;

            textBox1.Text = QsRef.ID[0].ToString();
            textBox2.Text = QsRef.ID[1].ToString();
            textBox3.Text = QsRef.ID[2].ToString();
            textBox4.Text = QsRef.ID[3].ToString();
            textBox5.Text = QsRef.ID[4].ToString();
            textBox6.Text = QsRef.ID[5].ToString();
            textBox7.Text = QsRef.ID[6].ToString();
            textBox8.Text = QsRef.ID[7].ToString();
            textBox9.Text = QsRef.ID[8].ToString();
            textBox10.Text = QsRef.ID[9].ToString();
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            MainForm.mapedittool.external_QuickSelect_OnSet(0, Utility.TryParseUInt16(textBox1.Text, 0));
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {
            MainForm.mapedittool.external_QuickSelect_OnSet(1, Utility.TryParseUInt16(textBox2.Text, 0));
        }

        private void textBox3_Validated(object sender, EventArgs e)
        {
            MainForm.mapedittool.external_QuickSelect_OnSet(2, Utility.TryParseUInt16(textBox3.Text, 0));
        }

        private void textBox4_Validated(object sender, EventArgs e)
        {
            MainForm.mapedittool.external_QuickSelect_OnSet(3, Utility.TryParseUInt16(textBox4.Text, 0));
        }

        private void textBox5_Validated(object sender, EventArgs e)
        {
            MainForm.mapedittool.external_QuickSelect_OnSet(4, Utility.TryParseUInt16(textBox5.Text, 0));
        }

        private void textBox6_Validated(object sender, EventArgs e)
        {
            MainForm.mapedittool.external_QuickSelect_OnSet(5, Utility.TryParseUInt16(textBox6.Text, 0));
        }

        private void textBox7_Validated(object sender, EventArgs e)
        {
            MainForm.mapedittool.external_QuickSelect_OnSet(6, Utility.TryParseUInt16(textBox7.Text, 0));
        }

        private void textBox8_Validated(object sender, EventArgs e)
        {
            MainForm.mapedittool.external_QuickSelect_OnSet(7, Utility.TryParseUInt16(textBox8.Text, 0));
        }

        private void textBox9_Validated(object sender, EventArgs e)
        {
            MainForm.mapedittool.external_QuickSelect_OnSet(8, Utility.TryParseUInt16(textBox9.Text, 0));
        }

        private void textBox10_Validated(object sender, EventArgs e)
        {
            MainForm.mapedittool.external_QuickSelect_OnSet(9, Utility.TryParseUInt16(textBox10.Text, 0));
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = MainForm.mapedittool.external_QuickSelect_DetermineCategory();
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(textBox1.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = MainForm.mapedittool.external_QuickSelect_DetermineCategory();
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(textBox2.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = MainForm.mapedittool.external_QuickSelect_DetermineCategory();
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(textBox3.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        private void textBox4_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = MainForm.mapedittool.external_QuickSelect_DetermineCategory();
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(textBox4.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        private void textBox5_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = MainForm.mapedittool.external_QuickSelect_DetermineCategory();
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(textBox5.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        private void textBox6_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = MainForm.mapedittool.external_QuickSelect_DetermineCategory();
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(textBox6.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        private void textBox7_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = MainForm.mapedittool.external_QuickSelect_DetermineCategory();
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(textBox7.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        private void textBox8_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = MainForm.mapedittool.external_QuickSelect_DetermineCategory();
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(textBox8.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        private void textBox9_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = MainForm.mapedittool.external_QuickSelect_DetermineCategory();
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(textBox9.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }

        private void textBox10_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            int cat_id = MainForm.mapedittool.external_QuickSelect_DetermineCategory();
            if (cat_id == -1)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(textBox10.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[cat_id].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(cat_id, real_elem_id);
            }
        }
    }
}
