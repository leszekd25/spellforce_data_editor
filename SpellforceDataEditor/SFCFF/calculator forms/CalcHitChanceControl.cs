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
            int index = SFCategoryManager.gamedata[2024].GetElementIndex(unit_id);
            if (index == Utility.NO_INDEX)
                return;
            ushort stats_id = (ushort)SFCategoryManager.gamedata[2024][index][2];

            // unit stats
            int index2 = SFCategoryManager.gamedata[2005].GetElementIndex((int)stats_id);
            if (index2 == Utility.NO_INDEX)
                return;
            SFCategoryElement stats = SFCategoryManager.gamedata[2005][index2];
            
            int lvl = (int)(ushort)stats[1];
            int dex = (int)(ushort)stats[4];
            int agi = (int)(ushort)stats[3];

            // get unit eq
            int index3 = SFCategoryManager.gamedata[2025].GetElementIndex(unit_id);
            if (index3 != Utility.NO_INDEX)
            {
                SFCategoryElementList items = SFCategoryManager.gamedata[2025].element_lists[index];
                for(int i = 0; i < items.Elements.Count; i++)
                {
                    //item stats
                    ushort item_id = (ushort)items[i][2];
                    int index4 = SFCategoryManager.gamedata[2004].GetElementIndex((int)item_id);
                    if (index4 == Utility.NO_INDEX)
                        continue;

                    SFCategoryElement item = SFCategoryManager.gamedata[2004][index4];
                    dex += (short)item[4];
                    agi += (short)item[3];
                }
            }

            if(set_index == 0)
            {
                textBox3.Text = lvl.ToString();
                textBox4.Text = dex.ToString();
                textBox5.Text = agi.ToString();
                label10.Text = SFCategoryManager.GetUnitName((ushort)unit_id);
            }
            else
            {
                textBox6.Text = lvl.ToString();
                textBox7.Text = dex.ToString();
                textBox8.Text = agi.ToString();
                label11.Text = SFCategoryManager.GetUnitName((ushort)unit_id);
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
                StepInto(textBox1, 2024);
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                StepInto(textBox2, 2024);
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
