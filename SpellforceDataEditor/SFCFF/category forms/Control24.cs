using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control24 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2029 c2029;

        public Control24()
        {
            InitializeComponent();

            c2029 = SFCategoryManager.gamedata.c2029;
            category = c2029;

            column_dict.Add("Building ID", "BuildingID");
            column_dict.Add("Race ID", "RaceID");
            column_dict.Add("Can enter", "CanEnter");
            column_dict.Add("Slots", "Slots");
            column_dict.Add("Health", "Health");
            column_dict.Add("Name ID", "NameID");
            column_dict.Add("Center of rotation X", "RotCenterX");
            column_dict.Add("Center of rotation Y", "RotCenterY");
            column_dict.Add("Collision polygons", "NumOfPolygons");
            column_dict.Add("Worker cycle time", "WorkerCycleTime");
            column_dict.Add("Required building ID", "BuildingReqID");
            column_dict.Add("Initial angle", "InitialAngle");
            column_dict.Add("Extended description ID", "DescriptionExtID");
            column_dict.Add("Flags", "Flags");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            c2029.SetID(current_element, SFEngine.Utility.TryParseUInt16(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "RaceID", SFEngine.Utility.TryParseUInt8(textBox2.Text));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "CanEnter", (Byte)(checkBox1.Checked ? 1 : 0));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "Slots", SFEngine.Utility.TryParseUInt8(textBox3.Text));
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "Health", SFEngine.Utility.TryParseUInt16(textBox4.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "NameID", SFEngine.Utility.TryParseUInt16(textBox5.Text));
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "RotCenterX", SFEngine.Utility.TryParseInt16(textBox6.Text));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "RotCenterY", SFEngine.Utility.TryParseInt16(textBox7.Text));
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "NumOfPolygons", SFEngine.Utility.TryParseUInt8(textBox9.Text));
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "WorkerCycleTime", SFEngine.Utility.TryParseUInt16(textBox10.Text));
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "BuildingReqID", SFEngine.Utility.TryParseUInt16(textBox11.Text));
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "InitialAngle", SFEngine.Utility.TryParseUInt16(textBox8.Text));
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "DescriptionExtID", SFEngine.Utility.TryParseUInt16(textBox12.Text));
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            c2029.SetField(current_element, "Flags", SFEngine.Utility.TryParseUInt8(textBox13.Text));
        }

        public override void show_element()
        {
            textBox1.Text = c2029[current_element].BuildingID.ToString();
            textBox2.Text = c2029[current_element].RaceID.ToString();
            checkBox1.Checked = (c2029[current_element].CanEnter != 0);
            textBox3.Text = c2029[current_element].Slots.ToString();
            textBox4.Text = c2029[current_element].Health.ToString();
            textBox5.Text = c2029[current_element].NameID.ToString();
            textBox6.Text = c2029[current_element].RotCenterX.ToString();
            textBox7.Text = c2029[current_element].RotCenterY.ToString();
            textBox9.Text = c2029[current_element].NumOfPolygons.ToString();
            textBox10.Text = c2029[current_element].WorkerCycleTime.ToString();
            textBox11.Text = c2029[current_element].BuildingReqID.ToString();
            textBox8.Text = c2029[current_element].InitialAngle.ToString();
            textBox12.Text = c2029[current_element].DescriptionExtID.ToString();
            textBox13.Text = c2029[current_element].Flags.ToString();

            button_repr(ButtonGoto25, 2030);
            button_repr(ButtonGoto26, 2031);
        }


        public override string get_element_string(int index)
        {
            return $"{c2029[index].BuildingID} {SFCategoryManager.GetTextByLanguage(c2029[index].NameID, SFEngine.Settings.LanguageID)}";
        }

        public override string get_description_string(int index)
        {
            return $"Race: {SFCategoryManager.GetRaceName(c2029[index].RaceID)}\r\nRequires {SFCategoryManager.GetBuildingName(c2029[index].BuildingReqID)}";
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2022, textBox2.Text);
        }

        private void textBox5_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2016, textBox5.Text);
        }

        private void textBox11_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2029, textBox11.Text);
        }

        private void textBox12_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2059, textBox12.Text);
        }

        private void ButtonGoto25_Click(object sender, EventArgs e)
        {
            if (button_gen_elem(ButtonGoto25, 2030))
            {
                SFCategoryManager.gamedata.c2030.GetItemIndex(c2029[current_element].GetID(), out int building_index);
                c2029.SetField(current_element, "NumOfPolygons", (byte)SFCategoryManager.gamedata.c2030.GetItemSubItemNum(building_index));
            }
        }

        private void ButtonGoto26_Click(object sender, EventArgs e)
        {
            button_gen_elem(ButtonGoto26, 2031);
        }
    }
}
