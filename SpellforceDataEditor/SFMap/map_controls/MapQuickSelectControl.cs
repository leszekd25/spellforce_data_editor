using SFEngine.SFCFF;
using SFEngine.SFMap;
using System;
using System.Data;
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
                {
                    Visible = false;
                }
                else
                {
                    Visible = true;
                }

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
            {
                return;
            }

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

        private void textBox9_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                if(MainForm.data != null)
                {
                    MainForm.data.trace_id(internal_qs_ref.cat_id, SFEngine.Utility.TryParseUInt16(((TextBox)sender).Text));
                }
            }
        }

        private void textBox9_Validated_1(object sender, EventArgs e)
        {
            int index = SFEngine.Utility.TryParseInt32((string)((TextBox)sender).Tag);
            ushort id = SFEngine.Utility.TryParseUInt16(textBox1.Text);
            QsRef.ID[index] = id;
        }
    }
}
