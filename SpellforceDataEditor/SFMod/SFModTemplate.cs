/*
 * SFModTemplate is a file which describes mods to be loaded into the game
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace SpellforceDataEditor.SFMod
{
    public class SFModTemplate
    {
        public string name;
        public List<string> mod_names { get; private set; } = new List<string>();

        // load
        public void Load(string tmpl_name)
        {
            mod_names.Clear();

            FileStream fs = File.Open("modtemplates\\" + tmpl_name + ".tmpl", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
                mod_names.Add(sr.ReadLine());
            sr.Close();

            name = tmpl_name;
        }

        // save
        public void Save()
        {
            FileStream fs = File.Open("modtemplates\\" + name + ".tmpl", FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            foreach (string s in mod_names)
                sw.WriteLine(s);
            sw.Close();
        }

        public void CopyFrom(SFModTemplate tmpl)
        {
            name = tmpl.name;
            mod_names.Clear();
            foreach (string s in tmpl.mod_names)
                mod_names.Add(s);
        }
    }
}
