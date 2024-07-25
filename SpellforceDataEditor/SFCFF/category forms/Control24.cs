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

            column_dict.Add("Building ID", new int[1] { 0 });
            column_dict.Add("Race ID", new int[1] { 1 });
            column_dict.Add("Can enter", new int[1] { 2 });
            column_dict.Add("Slots", new int[1] { 3 });
            column_dict.Add("Health", new int[1] { 4 });
            column_dict.Add("Name ID", new int[1] { 5 });
            column_dict.Add("Center of rotation X", new int[1] { 6 });
            column_dict.Add("Center of rotation Y", new int[1] { 7 });
            column_dict.Add("Collision polygons", new int[1] { 8 });
            column_dict.Add("Worker cycle time", new int[1] { 9 });
            column_dict.Add("Required building ID", new int[1] { 10 });
            column_dict.Add("Initial angle", new int[1] { 11 });
            column_dict.Add("Extended description ID", new int[1] { 12 });
            column_dict.Add("Flags", new int[1] { 13 });
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

            button_repr(ButtonGoto25, SFCategoryManager.gamedata.c2030, "Collision data", "Building");
            button_repr(ButtonGoto26, SFCategoryManager.gamedata.c2031, "Requirements", "Building");
        }


        public override string get_element_string(int index)
        {
            return $"{c2029[index].BuildingID} {SFCategoryManager.GetTextByLanguage(c2029[index].NameID, 1)}";
        }

        public override string get_description_string(int index)
        {
            return $"Race: {SFCategoryManager.GetRaceName(c2029[index].RaceID)}\r\nRequires {SFCategoryManager.GetBuildingName(c2029[index].BuildingReqID)}";
        }
    }
}
