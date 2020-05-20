using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace SpellforceDataEditor.SFCFF
{
    static public class CustomScript
    {
        static private float get_dmg(int min_dmg, int max_dmg, int sp)
        {
            Single mean = ((Single)min_dmg + (Single)max_dmg) / 2;
            Single ratio = ((Single)sp) / 100;
            return mean * ratio;
        }

        static public void WeaponsToFile(bool ignore_no_ui_items)
        {
            if (!SFCategoryManager.ready)
                return;

            StringBuilder sb = new StringBuilder(); 
            CultureInfo ci = new CultureInfo("en-us");

            foreach (var item in SFCategoryManager.gamedata[6].elements)
            {
                ushort item_id = (ushort)item[0];
                var weapon = SFCategoryManager.gamedata[9].FindElementBinary<ushort>(0, item_id);
                if (weapon == null)
                    continue;

                if (ignore_no_ui_items)
                {
                    var weapon_ui = SFCategoryManager.gamedata[12].FindElementBinary<ushort>(0, item_id);
                    if (weapon_ui == null)
                        continue;
                }

                SFCategoryElement type_elem = SFCategoryManager.gamedata[43].FindElementBinary<UInt16>(0, (ushort)weapon[6]);
                string type_name = SFCategoryManager.GetTextFromElement(type_elem, 1);
                SFCategoryElement material_elem = SFCategoryManager.gamedata[44].FindElementBinary<UInt16>(0, (ushort)weapon[7]);
                string material_name = SFCategoryManager.GetTextFromElement(material_elem, 1);

                int avg_tier = 0;
                var req = SFCategoryManager.gamedata[10].FindElementBinary<ushort>(0, item_id);
                if(req != null)
                {
                    int req_count = req.variants.Count / 5;
                    if (((byte)req[2]) == 0)
                        avg_tier = SFCategoryManager.GetMaxSkillLevel((byte)req[4]);
                    else
                        avg_tier = ((byte)req[4]);

                    for(int i = 1; i < req_count; i++)
                    {
                        int next_tier = 0;
                        if (((byte)req[i*5+2]) == 0)
                            next_tier = SFCategoryManager.GetMaxSkillLevel((byte)req[4]);
                        else
                            next_tier = ((byte)req[i*5+4]);

                        if (next_tier < avg_tier)
                            avg_tier = next_tier;
                    }

                }

                sb.Append(item_id.ToString().PadRight(5));
                sb.Append(SFCategoryManager.GetItemName(item_id).PadRight(64));
                sb.Append(weapon[1].ToString().PadRight(3));
                sb.Append(weapon[2].ToString().PadRight(3));
                sb.Append(weapon[5].ToString().PadRight(4));
                sb.Append(get_dmg((ushort)weapon[1], (ushort)weapon[2], (ushort)weapon[5]).ToString("G04", ci).PadRight(7));
                sb.Append(weapon[3].ToString().PadRight(3));
                sb.Append(weapon[4].ToString().PadRight(3));
                sb.Append(type_name.PadRight(40));
                sb.Append(material_name.PadRight(40));
                sb.Append(avg_tier.ToString().PadRight(3));
                sb.AppendLine();
            }

            File.WriteAllText("weapon_data.txt", sb.ToString());
        }
    }
}
