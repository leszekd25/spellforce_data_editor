/*
 * SFVisualLink is a connection between game data element and its 3D representation in game
 * SFVisualLink is a simple object, being just an entry's ID and a set of asset names
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SF3D.SceneSynchro
{
    public class SFVisualLink
    {
        public int ID { get; private set; }
        public List<String> lines { get; } = new List<string>();

        public SFVisualLink()
        {

        }

        //assumes correct data
        public bool Read(string data)
        {
            String[] arr = data.Split(' ');
            int tmp_id;    //ugh...
            bool success = int.TryParse(arr[0], out tmp_id);
            if (!success)
                return false;
            ID = tmp_id;
            for (int i = 1; i < arr.Length; i++)
                lines.Add(arr[i]);
            return true;
        }
    }
}
