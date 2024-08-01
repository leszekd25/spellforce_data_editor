using SFEngine.SFCFF;
using SFEngine.SFCFF.CTG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.category_forms
{
    public partial class Control1 : SpellforceDataEditor.SFCFF.category_forms.SFControl
    {
        Category2002 c2002;

        static int[] tracetable;
        public Control1()
        {
            InitializeComponent();

            c2002 = SFCategoryManager.gamedata.c2002;
            category = c2002;

            tracetable = new int[10];
            for (int i = 0; i < 10; i++)
            {
                tracetable[i] = 0;
            }

            column_dict.Add("Spell effect ID", "SpellID");
            column_dict.Add("Spell type ID", "SpellLineID");
            column_dict.Add("Requirement 1 1", "SkillReq[0]");
            column_dict.Add("Requirement 1 2", "SkillReq[1]");
            column_dict.Add("Requirement 1 3", "SkillReq[2]");
            column_dict.Add("Requirement 2 1", "SkillReq[3]"); 
            column_dict.Add("Requirement 2 2", "SkillReq[4]");
            column_dict.Add("Requirement 2 3", "SkillReq[5]");
            column_dict.Add("Requirement 3 1", "SkillReq[6]");
            column_dict.Add("Requirement 3 2", "SkillReq[7]");
            column_dict.Add("Requirement 3 3", "SkillReq[8]");
            column_dict.Add("Requirement 4 1", "SkillReq[9]");
            column_dict.Add("Requirement 4 2", "SkillReq[10]"); 
            column_dict.Add("Requirement 4 3", "SkillReq[11]");
            column_dict.Add("Mana cost", "ManaCost");
            column_dict.Add("Cast time", "CastTime");
            column_dict.Add("Recast time", "RecastTime");
            column_dict.Add("Minimum range", "MinRange");
            column_dict.Add("Maximum range", "MaxRange");
            column_dict.Add("Casting type 1", "CastType1");
            column_dict.Add("Casting type 2", "CastType2");
            column_dict.Add("Spell data 1", "Params[0]");
            column_dict.Add("Spell data 2", "Params[1]");
            column_dict.Add("Spell data 3", "Params[2]");
            column_dict.Add("Spell data 4", "Params[3]");
            column_dict.Add("Spell data 5", "Params[4]");
            column_dict.Add("Spell data 6", "Params[5]");
            column_dict.Add("Spell data 7", "Params[6]");
            column_dict.Add("Spell data 8", "Params[7]");
            column_dict.Add("Spell data 9", "Params[8]");
            column_dict.Add("Spell data 10", "Params[9]");
            column_dict.Add("Effect power", "EffectPower");
            column_dict.Add("Effect range", "EffectRange");
        }

        private void tb_effID_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SpellID", SFEngine.Utility.TryParseUInt16(tb_effID.Text));
        }

        private void tb_typeID_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SpellLineID", SFEngine.Utility.TryParseUInt16(tb_typeID.Text));
        }

        private void tb_req1_1_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[0]", SFEngine.Utility.TryParseUInt8(tb_req1_1.Text));
        }

        private void tb_req1_2_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[1]", SFEngine.Utility.TryParseUInt8(tb_req1_2.Text));
        }

        private void tb_req1_3_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[2]", SFEngine.Utility.TryParseUInt8(tb_req1_3.Text));
        }

        private void tb_req2_1_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[3]", SFEngine.Utility.TryParseUInt8(tb_req2_1.Text));
        }

        private void tb_req2_2_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[4]", SFEngine.Utility.TryParseUInt8(tb_req2_2.Text));
        }

        private void tb_req2_3_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[5]", SFEngine.Utility.TryParseUInt8(tb_req2_3.Text));
        }

        private void tb_req3_1_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[6]", SFEngine.Utility.TryParseUInt8(tb_req3_1.Text));
        }

        private void tb_req3_2_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[7]", SFEngine.Utility.TryParseUInt8(tb_req3_2.Text));
        }

        private void tb_req3_3_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[8]", SFEngine.Utility.TryParseUInt8(tb_req3_3.Text));
        }

        private void tb_req4_1_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[9]", SFEngine.Utility.TryParseUInt8(tb_req4_1.Text));
        }

        private void tb_req4_2_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[10]", SFEngine.Utility.TryParseUInt8(tb_req4_2.Text));
        }

        private void tb_req4_3_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "SkillReq[11]", SFEngine.Utility.TryParseUInt8(tb_req4_3.Text));
        }

        private void tb_mnc_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "ManaCost", SFEngine.Utility.TryParseUInt16(tb_mnc.Text));
        }

        private void tb_ctm_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "CastTime", SFEngine.Utility.TryParseUInt32(tb_ctm.Text));
        }

        private void tb_rtm_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "RecastTime", SFEngine.Utility.TryParseUInt32(tb_rtm.Text));
        }

        private void tb_rng_min_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "MinRange", SFEngine.Utility.TryParseUInt16(tb_rng_min.Text));
        }

        private void tb_rng_max_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "MaxRange", SFEngine.Utility.TryParseUInt16(tb_rng_max.Text));
        }

        private void tb_ct1_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "CastType1", SFEngine.Utility.TryParseUInt8(tb_ct1.Text));
        }

        private void tb_ct2_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "CastType2", SFEngine.Utility.TryParseUInt8(tb_ct2.Text));
        }

        private void tb_sd1_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "Params[0]", SFEngine.Utility.TryParseUInt32(tb_sd1.Text));
        }

        private void tb_sd2_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "Params[1]", SFEngine.Utility.TryParseUInt32(tb_sd2.Text));
        }

        private void tb_sd3_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "Params[2]", SFEngine.Utility.TryParseUInt32(tb_sd3.Text));
        }

        private void tb_sd4_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "Params[3]", SFEngine.Utility.TryParseUInt32(tb_sd4.Text));
        }

        private void tb_sd5_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "Params[4]", SFEngine.Utility.TryParseUInt32(tb_sd5.Text));
        }

        private void tb_sd6_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "Params[5]", SFEngine.Utility.TryParseUInt32(tb_sd6.Text));
        }

        private void tb_sd7_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "Params[6]", SFEngine.Utility.TryParseUInt32(tb_sd7.Text));
        }

        private void tb_sd8_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "Params[7]", SFEngine.Utility.TryParseUInt32(tb_sd8.Text));
        }

        private void tb_sd9_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "Params[8]", SFEngine.Utility.TryParseUInt32(tb_sd9.Text));
        }

        private void tb_sd10_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "Params[9]", SFEngine.Utility.TryParseUInt32(tb_sd10.Text));
        }

        private void tb_effpow_TextChanged(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "EffectPower", SFEngine.Utility.TryParseUInt16(tb_effpow.Text));
        }

        private void tb_effrng_Validated(object sender, EventArgs e)
        {
            c2002.SetField(current_element, "EffectRange", SFEngine.Utility.TryParseUInt16(tb_effrng.Text));
        }

        private void set_data_labels(string[] p)
        {
            lb_sd1.Text = p[0];
            lb_sd2.Text = p[1];
            lb_sd3.Text = p[2];
            lb_sd4.Text = p[3];
            lb_sd5.Text = p[4];
            lb_sd6.Text = p[5];
            lb_sd7.Text = p[6];
            lb_sd8.Text = p[7];
            lb_sd9.Text = p[8];
            lb_sd10.Text = p[9];
            for (int i = 0; i < 10; i++)
            {
                TextBox t = Controls.Find("tb_sd" + (i + 1).ToString(), true)[0] as TextBox;
                char c = p[10][i];
                if (c == '0')
                {
                    t.BackColor = SystemColors.Window;
                    tracetable[i] = SFEngine.Utility.NO_INDEX;
                }
                else
                {
                    t.BackColor = Color.DarkOrange;
                    if (c == '1')
                    {
                        tracetable[i] = 2002;
                    }
                    else
                    {
                        tracetable[i] = 2024;
                    }
                }
            }
        }

        public override void show_element()
        {
            Category2002Item item = c2002.Items[current_element];
            tb_effID.Text = item.SpellID.ToString();
            tb_typeID.Text = item.SpellLineID.ToString();
            tb_req1_1.Text = item.GetSkillReq(0).ToString();
            tb_req1_2.Text = item.GetSkillReq(1).ToString();
            tb_req1_3.Text = item.GetSkillReq(2).ToString();
            tb_req2_1.Text = item.GetSkillReq(3).ToString();
            tb_req2_2.Text = item.GetSkillReq(4).ToString();
            tb_req2_3.Text = item.GetSkillReq(5).ToString();
            tb_req3_1.Text = item.GetSkillReq(6).ToString();
            tb_req3_2.Text = item.GetSkillReq(7).ToString();
            tb_req3_3.Text = item.GetSkillReq(8).ToString();
            tb_req4_1.Text = item.GetSkillReq(9).ToString();
            tb_req4_2.Text = item.GetSkillReq(10).ToString();
            tb_req4_3.Text = item.GetSkillReq(11).ToString();
            tb_mnc.Text = item.ManaCost.ToString();
            tb_ctm.Text = item.CastTime.ToString();
            tb_rtm.Text = item.RecastTime.ToString();
            tb_rng_min.Text = item.MinRange.ToString();
            tb_rng_max.Text = item.MaxRange.ToString();
            tb_ct1.Text = item.CastType1.ToString();
            tb_ct2.Text = item.CastType2.ToString();
            tb_sd1.Text = item.GetParam(0).ToString();
            tb_sd2.Text = item.GetParam(1).ToString();
            tb_sd3.Text = item.GetParam(2).ToString();
            tb_sd4.Text = item.GetParam(3).ToString();
            tb_sd5.Text = item.GetParam(4).ToString();
            tb_sd6.Text = item.GetParam(5).ToString();
            tb_sd7.Text = item.GetParam(6).ToString();
            tb_sd8.Text = item.GetParam(7).ToString();
            tb_sd9.Text = item.GetParam(8).ToString();
            tb_sd10.Text = item.GetParam(9).ToString();
            tb_effpow.Text = item.EffectPower.ToString();
            tb_effrng.Text = item.EffectRange.ToString();
            set_data_labels(SFSpellDescriptor.get(SFEngine.Utility.TryParseUInt16(tb_typeID.Text)));
        }

        private string get_target_mode(Byte tm)
        {
            switch (tm)
            {
                case 1:
                    return "Figure";
                case 2:
                    return "Building";
                case 3:
                    return "Object";
                case 4:
                    return "in World";
                case 5:
                    return "in Area";
                default:
                    return SFEngine.Utility.S_NONAME;
            }
        }

        private string get_target_type(Byte tm)
        {
            switch (tm)
            {
                case 1:
                    return "Enemy";
                case 2:
                    return "Ally";
                case 3:
                    return "Other";
                default:
                    return SFEngine.Utility.S_NONAME;
            }
        }

        public override string get_element_string(int index)
        {
            ushort type_id = c2002[index].SpellLineID;
            bool spellline_found = SFCategoryManager.gamedata.c2054.GetItemIndex(type_id, out int spellline_index);
            if (spellline_found)
            {
                return $"{c2002[index].SpellID} {SFCategoryManager.GetTextByLanguage(SFCategoryManager.gamedata.c2054[spellline_index].TextID, SFEngine.Settings.LanguageID)} level {c2002[index].GetSpellLevel()}";
            }
            else
            {
                return $"{c2002[index].SpellID} {SFEngine.Utility.S_ITEM_MISSING} level {c2002[index].GetSpellLevel()}";
            }
        }

        public override string get_description_string(int index)
        {
            Category2002Item item = c2002[index];

            StringWriter sw = new StringWriter();
            sw.WriteLine("Requirements: ");
            for (int i = 0; i < 4; i++)
            {
                if (item.GetSkillReq(i * 3 + 0) + item.GetSkillReq(i * 3 + 1) + item.GetSkillReq(i * 3 + 2) != 0)
                {
                    sw.WriteLine(SFCategoryManager.GetSkillName(item.GetSkillReq(i * 3 + 0), item.GetSkillReq(i * 3 + 1), item.GetSkillReq(i * 3 + 2)));
                }
            }
            sw.WriteLine($"Target: {get_target_type(item.CastType1)} {get_target_mode(item.CastType2)}");

            return sw.ToString();
        }

        // trace
        private void tb_typeID_MouseDown(object sender, MouseEventArgs e)
        {
            textbox_trace(e, 2054, tb_typeID.Text);
        }

        private void tb_sd1_MouseDown(object sender, MouseEventArgs e)
        {
            int i = SFEngine.Utility.TryParseInt32((string)((TextBox)sender).Tag);
            if (tracetable[i] != SFEngine.Utility.NO_INDEX)
            {
                textbox_trace(e, tracetable[i], ((TextBox)sender).Text);
            }
        }
    }
}
