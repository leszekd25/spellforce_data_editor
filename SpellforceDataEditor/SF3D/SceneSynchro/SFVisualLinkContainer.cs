/*
 * SFVisualLinkContainer is a manager for SFVisualLink objects
 * It can differentiate between asset types and allows asset name extraction based on its type
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SF3D.SceneSynchro
{
    public class SFVisualLinkContainer
    {
        List<SFVisualLink> items = new List<SFVisualLink>();
        List<SFVisualLink> buildings = new List<SFVisualLink>();
        List<SFVisualLink> objects = new List<SFVisualLink>();
        List<SFVisualLink> heads = new List<SFVisualLink>();
        public bool is_ready { get; private set; } = false;

        public SFVisualLinkContainer()
        {
            //read data from .sdb files
            //System.Diagnostics.Debug.WriteLine("LOADING DATA");
            LoadSDB("buildings.sdb", buildings);
            LoadSDB("items.sdb", items);
            LoadSDB("objects.sdb", objects);
            LoadSDB("heads.sdb", heads);
            is_ready = true;
        }

        private int LoadSDB(string fname, List<SFVisualLink> list)
        {
            //System.Diagnostics.Debug.WriteLine("FILE " + fname);
            list.Clear();
            FileStream fs;
            try
            {
                fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                return -2;
            }

            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                SFVisualLink vl = new SFVisualLink();
                if (vl.Read(sr.ReadLine()))
                    list.Add(vl);
            }
            //System.Diagnostics.Debug.WriteLine("COUNT " + list.Count.ToString());

            sr.Close();
            return 0;
        }

        //binary search
        private SFVisualLink FindVisualLink(List<SFVisualLink> list, int id)
        {
            int current_start = 0;
            int current_end = list.Count - 1;
            int current_center;
            int val;
            while (current_start <= current_end)
            {
                current_center = (current_start + current_end) / 2;    //care about overflow
                val = list[current_center].ID;
                if (val.CompareTo(id) == 0)
                    return list[current_center];
                if (val.CompareTo(id) < 0)
                    current_start = current_center + 1;
                else
                    current_end = current_center - 1;
            }
            return null;
        }

        public List<String> GetBuildingMeshes(int building_id)
        {
            SFVisualLink building = FindVisualLink(buildings, building_id);
            if (building == null)
                return null;

            List<String> meshes = building.lines.ToList();
            for (int i = 0; i < meshes.Count; i++)
            {
                string str = meshes[i];
                if (str.Contains("frame"))
                {
                    meshes.RemoveAt(i);
                    i--;
                }
            }

            return meshes;
        }

        public List<String> GetObjectMeshes(int object_id)
        {
            SFVisualLink obj = FindVisualLink(objects, object_id);
            if (obj == null)
                return null;

            return obj.lines;
        }

        public String GetItemMesh(int item_id, bool is_female)
        {
            SFVisualLink item = FindVisualLink(items, item_id);
            if (item == null)
                return "";

            if (item.lines.Count == 0)
                return "";
            if (item.lines.Count == 1)
                return item.lines[0];
            if (item.lines.Count >= 2)
            {
                for(int i = 0; i < item.lines.Count-1; i++)
                {
                    string str = item.lines[i];
                    if ((is_female) && (str.Contains("_female")))
                        return str;
                    if ((!is_female) && (str.Contains("_male")))
                        return str;
                }
                return item.lines[0];
            }

            return "";
        }

        public String GetItemAnimation(int item_id, bool is_female)
        {
            SFVisualLink item = FindVisualLink(items, item_id);
            if (item == null)
                return null;

            if (item.lines.Count < 2)
                return "";
            if(item.lines[item.lines.Count-1] == "figure_hero")
            {
                if (is_female)
                    return "figure_hero_female";
                return "figure_hero_male";
            }
            if (!(item.lines[item.lines.Count - 1].Contains("male")))
                return item.lines[item.lines.Count - 1];
            
            return "";
        }

        public String GetHeadMesh(int head_id, bool is_female)
        {
            SFVisualLink head = FindVisualLink(heads, head_id);
            if (head == null)
                return "";

            return (is_female ? head.lines[1] : head.lines[0]);
        }
    }
}
