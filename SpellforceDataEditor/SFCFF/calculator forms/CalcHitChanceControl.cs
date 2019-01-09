using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.calculator_forms
{
    public partial class CalcHitChanceControl : SpellforceDataEditor.SFCFF.calculator_forms.CalcControl
    {
        public CalcHitChanceControl()
        {
            InitializeComponent();
        }

        private float GetHitChance(int attacker_lvl, int dex, int target_lvl, int agi, int ill, bool charmed, bool human, bool undead, bool fog)
        {
            int chance = 60;
            if (charmed)
                chance = 90;
            if(attacker_lvl > target_lvl)
                chance += 10 * (((attacker_lvl * 100) / target_lvl - 100)/100);
            else if(attacker_lvl < target_lvl)
                chance -= 20*((100-(attacker_lvl*100)/target_lvl)/100);
            chance = Math.Max(Math.Min(100, chance), 0);
            if (dex < agi)
                chance = ((((dex * 100) / agi) * 6 + 400) * chance) / 1000;
            else
                chance = 100 - (agi * (100 - chance)) / dex;
            if ((!undead) && (fog))
                chance /= 2;
            if (human)
                chance = (700 * chance + 3 * (ill * 100) / 256) / 1000;
            return chance;

        }

        private void SetUnitStats(int set_index, int unit_id)
        {
            if (!SFCategoryManager.ready)
                return;

            // unit stuff
            int index = SFCategoryManager.get_category(17).get_element_index(unit_id);
            if (index == -1)
                return;
            ushort stats_id = (ushort)SFCategoryManager.get_category(17).get_element(index).get_single_variant(2).value;

            // unit stats
            int index2 = SFCategoryManager.get_category(3).get_element_index((int)stats_id);
            if (index2 == -1)
                return;
            SFCategoryElement stats = SFCategoryManager.get_category(3).get_element(index2);
            
            int lvl = (int)(ushort)stats.get_single_variant(1).value;
            int dex = (int)(ushort)stats.get_single_variant(4).value;
            int agi = (int)(ushort)stats.get_single_variant(3).value;

            // get unit eq
            int index3 = SFCategoryManager.get_category(18).get_element_index(unit_id);
            if (index3 != -1)
            {
                SFCategoryElement items = SFCategoryManager.get_category(18).get_element(index);
                int item_count = items.get().Count / 3;
                for(int i = 0; i < item_count; i++)
                {
                    //item stats
                    ushort item_id = (ushort)items.get_single_variant(3 * i + 2).value;
                    int index4 = SFCategoryManager.get_category(7).get_element_index((int)item_id);
                    if (index4 == -1)
                        continue;

                    SFCategoryElement item = SFCategoryManager.get_category(7).get_element(index4);
                    dex += (short)item.get_single_variant(4).value;
                    agi += (short)item.get_single_variant(3).value;
                }
            }

            if(set_index == 0)
            {
                textBox3.Text = lvl.ToString();
                textBox4.Text = dex.ToString();
                textBox5.Text = agi.ToString();
                label10.Text = SFCategoryManager.get_unit_name((ushort)unit_id);
            }
            else
            {
                textBox6.Text = lvl.ToString();
                textBox7.Text = dex.ToString();
                textBox8.Text = agi.ToString();
                label11.Text = SFCategoryManager.get_unit_name((ushort)unit_id);
            }
        }

        public override float[] GetCalcResult()
        {
            int level1 = (int)(double)GetValue(textBox3);
            int level2 = (int)(double)GetValue(textBox6);
            int dex1 = (int)(double)GetValue(textBox4);
            int dex2 = (int)(double)GetValue(textBox7);
            int agi1 = (int)(double)GetValue(textBox5);
            int agi2 = (int)(double)GetValue(textBox8);
            int ill1 = (int)(double)GetValue(textBox9);
            int ill2 = (int)(double)GetValue(textBox10);

            bool charmed1 = (bool)GetValue(checkBox7);
            bool charmed2 = (bool)GetValue(checkBox8);
            bool human1 = (bool)GetValue(checkBox1);
            bool human2 = (bool)GetValue(checkBox4);
            bool undead1 = (bool)GetValue(checkBox2);
            bool undead2 = (bool)GetValue(checkBox5);
            bool fog1 = (bool)GetValue(checkBox3);
            bool fog2 = (bool)GetValue(checkBox6);

            return new float[] {GetHitChance(level1, dex1, level2, agi2, ill1, charmed1, human1, undead1, fog1),
                                GetHitChance(level2, dex2, level1, agi1, ill2, charmed2, human2, undead2, fog2)};
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
                StepInto(textBox1, 17);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                StepInto(textBox2, 17);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SetUnitStats(0, Utility.TryParseInt32(textBox1.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            SetUnitStats(1, Utility.TryParseInt32(textBox2.Text));
        }



        public override string GetCalcText()
        {
            float[] data = GetCalcResult();
            return "Unit 1 hit chance against unit 2: " + data[0].ToString() + "\r\nUnit 2 hit chance against unit 1: " + data[1].ToString();
        }
    }
}
