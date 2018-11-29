/*
 * ScriptProject is the main container for lua data
 * Script hierarchy goes as follows:
 * ScriptProject -> multiple ScriptPlatform -> multiple ScriptGenerator
 * ScriptProject allows reading/writing projects to .luap file, and platform manipulation
 * ScriptProjectHeader is a file header containing project version, name, and how many platforms the project contains
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpellforceDataEditor.SFLua.lua_filesystem
{
    public struct ScriptProjectHeader
    {
        public int version;
        public string name;
        public int platform_count;

        //load header from the stream
        static public ScriptProjectHeader Load(BinaryReader br)
        {
            System.Diagnostics.Debug.WriteLine("ScriptProjectHeader.Load()");
            ScriptProjectHeader h = new ScriptProjectHeader();
            h.version = br.ReadInt32();
            h.name = br.ReadString();
            h.platform_count = br.ReadInt32();
            System.Diagnostics.Debug.WriteLine(h.ToString());
            return h;
        }

        //save header to the stream
        public void Save(BinaryWriter br)
        {
            System.Diagnostics.Debug.WriteLine("ScriptProjectHeader.Save()");
            br.Write(version);
            br.Write(name);
            br.Write(platform_count);
            System.Diagnostics.Debug.WriteLine(ToString());
        }

        public override string ToString()
        {
            return name + ": " + version.ToString() + "; " + platform_count.ToString() + " platforms";
        }
    }

    public class ScriptProject
    {
        public const int Version = 0x0100;
        public string Name { get; set; } = "sample_project";
        Dictionary<string, ScriptPlatform> Platforms { get; } = new Dictionary<string, ScriptPlatform>();

        //load project from given file
        public int Load(string filename)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                return -2;
            }

            BinaryReader br = new BinaryReader(fs);

            ScriptProjectHeader mainHeader = ScriptProjectHeader.Load(br);
            //check data integrity
            if(mainHeader.version != Version)
            {
                fs.Close();
                return -3;
            }
            Name = mainHeader.name;

            Platforms.Clear();
            for(int i = 0; i < mainHeader.platform_count; i++)
            {
                ScriptPlatform platform = new ScriptPlatform();
                int result = platform.Load(br);
                if(result != 0)
                {
                    fs.Close();
                    Platforms.Clear();
                    return result;
                }
                Platforms.Add(platform.Name, platform);
            }

            fs.Close();
            return 0;
        }
        
        //save project under given filename
        public int Save(string filename)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            }
            catch (Exception e)
            {
                return -2;
            }

            BinaryWriter br = new BinaryWriter(fs);

            ScriptProjectHeader mainHeader = new ScriptProjectHeader();
            mainHeader.version = Version;
            mainHeader.name = Name;
            mainHeader.platform_count = Platforms.Count;
            mainHeader.Save(br);

            List<string> names = Platforms.Keys.ToList();
            names.Sort();
            for (int i = 0; i < Platforms.Count; i++)
            {
                int result = Platforms[names[i]].Save(br);
                if (result != 0)
                {
                    fs.Close();
                    return result;
                }
            }

            fs.Close();

            return 0;
        }

        //creates a new platform
        public int PlatformCreate(string name)
        {
            //check if platform exists
            if (PlatformExists(name))
                return -1;

            //create new platform
            ScriptPlatform platform = new ScriptPlatform();
            platform.Name = name;
            platform.is_npc_platform = true;
            platform.ScriptCreate("n0");
            Platforms.Add(name, platform);

            return 0;
        }

        //returns whether a platform with a given name exists
        public bool PlatformExists(string name)
        {
            return Platforms.ContainsKey(name);
        }

        //returns list of all platform names
        public List<string> PlatformGetNames()
        {
            return Platforms.Keys.ToList();
        }

        //returns a platform given its name
        public ScriptPlatform PlatformGet(string name)
        {
            if (!PlatformExists(name))
                return null;
            return Platforms[name];
        }

        //renames existing platform
        public void PlatformRename(string old_name, string new_name)
        {
            ScriptPlatform platform = PlatformGet(old_name);
            if (platform == null)
                return;
            if (PlatformExists(new_name))
                return;

            Platforms.Remove(old_name);
            Platforms[new_name] = platform;
            Platforms[new_name].Name = new_name;
        }
    }
}
