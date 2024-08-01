using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SFEngine.SFCFF.CTG;
using SFEngine.SFChunk;
using System.Xml.Linq;
using OpenTK.Graphics.OpenGL;

namespace SFEngine.SFCFF
{
    public class SFGameDataNew
    {
        public string fname;
        public Category2002 c2002 = new();
        public Category2054 c2054 = new();
        public Category2056 c2056 = new();
        public Category2005 c2005 = new();
        public Category2006 c2006 = new();
        public Category2067 c2067 = new();
        public Category2003 c2003 = new();
        public Category2004 c2004 = new();
        public Category2013 c2013 = new();
        public Category2015 c2015 = new();
        public Category2017 c2017 = new();
        public Category2014 c2014 = new();
        public Category2012 c2012 = new();
        public Category2018 c2018 = new();
        public Category2016 c2016 = new();
        public Category2022 c2022 = new();
        public Category2023 c2023 = new();
        public Category2024 c2024 = new();
        public Category2025 c2025 = new();
        public Category2026 c2026 = new();
        public Category2028 c2028 = new();
        public Category2040 c2040 = new();
        public Category2001 c2001 = new();
        public Category2029 c2029 = new();
        public Category2030 c2030 = new();
        public Category2031 c2031 = new();
        public Category2039 c2039 = new();
        public Category2062 c2062 = new();
        public Category2041 c2041 = new();
        public Category2042 c2042 = new();
        public Category2047 c2047 = new();
        public Category2044 c2044 = new();
        public Category2048 c2048 = new();
        public Category2050 c2050 = new();
        public Category2057 c2057 = new();
        public Category2065 c2065 = new();
        public Category2051 c2051 = new();
        public Category2052 c2052 = new();
        public Category2053 c2053 = new();
        public Category2055 c2055 = new();
        public Category2058 c2058 = new();
        public Category2059 c2059 = new();
        public Category2061 c2061 = new();
        public Category2063 c2063 = new();
        public Category2064 c2064 = new();
        public Category2032 c2032 = new();
        public Category2049 c2049 = new();
        public Category2036 c2036 = new();
        public Category2072 c2072 = new();

        public IEnumerable<ICategory> GetCategories()
        {
            yield return c2002;
            yield return c2054;
            yield return c2056;
            yield return c2005;
            yield return c2006;
            yield return c2067;
            yield return c2003;
            yield return c2004;
            yield return c2013;
            yield return c2015;
            yield return c2017;
            yield return c2014;
            yield return c2012;
            yield return c2018;
            yield return c2016;
            yield return c2022;
            yield return c2023;
            yield return c2024;
            yield return c2025;
            yield return c2026;
            yield return c2028;
            yield return c2040;
            yield return c2001;
            yield return c2029;
            yield return c2030;
            yield return c2031;
            yield return c2039;
            yield return c2062;
            yield return c2041;
            yield return c2042;
            yield return c2047;
            yield return c2044;
            yield return c2048;
            yield return c2050;
            yield return c2057;
            yield return c2065;
            yield return c2051;
            yield return c2052;
            yield return c2053;
            yield return c2055;
            yield return c2058;
            yield return c2059;
            yield return c2061;
            yield return c2063;
            yield return c2064;
            yield return c2032;
            yield return c2049;
            yield return c2036;
            yield return c2072;
            yield break;
        }

        // loads gamedata from file
        public int Load(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameDataNew.Load() called");

            fname = "";
            SFChunkFile sfcf = new SFChunkFile();
            int result = sfcf.OpenFile(filename);
            if (result != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameDataNew.Load() failed: could not open file");
                return result;
            }

            result = Import(sfcf);
            sfcf.Close();

            if (result != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameDataNew.Load() failed: error while loading data");
                return result;
            }

            fname = filename;

            return result;
        }

        // saves gamedata to file
        public int Save(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameDataNew.Save() called");

            SFChunkFile sfcf = new SFChunkFile();
            int result = sfcf.CreateFile(filename, SFChunkFileType.GAMEDATA);
            if (result != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameDataNew.Save() failed: Could not get file to save to");
                return result;
            }

            foreach (var cat in GetCategories())
            {
                bool write_result = cat.Save(sfcf);
                if(!write_result)
                {
                    sfcf.Close();

                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameDataNew.Save() failed: failed to write data to buffer");
                    return -118;
                }
            }

            sfcf.Close();
            fname = filename;

            return 0;
        }

        // merges from two provided gamedatas
        public int Merge(SFGameDataNew gd1, SFGameDataNew gd2)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameDataNew.Merge() called");
            // category map
            Dictionary<int, ICategory> cats1 = new();
            foreach (var c in gd1.GetCategories())
            {
                cats1[c.GetCategoryID()] = c;
            }
            Dictionary<int, ICategory> cats2 = new();
            foreach (var c in gd2.GetCategories())
            {
                cats2[c.GetCategoryID()] = c;
            }
            Dictionary<int, ICategory> cats3 = new();
            foreach (var c in GetCategories())
            {
                if (!c.MergeFrom(cats1[c.GetCategoryID()], cats2[c.GetCategoryID()]))
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"SFGameDataNew.Merge() failed: failed to merge category {c.GetCategoryID()}");
                    return -118;
                }
            }

            return 0;
        }

        // diffs from two provided gamedatas
        public int Diff(SFGameDataNew gd1, SFGameDataNew gd2)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameDataNew.Diff() called");
            // category map
            Dictionary<int, ICategory> cats1 = new();
            foreach (var c in gd1.GetCategories())
            {
                cats1[c.GetCategoryID()] = c;
            }
            Dictionary<int, ICategory> cats2 = new();
            foreach (var c in gd2.GetCategories())
            {
                cats2[c.GetCategoryID()] = c;
            }
            Dictionary<int, ICategory> cats3 = new();
            foreach (var c in GetCategories())
            {
                if (!c.DiffFrom(cats1[c.GetCategoryID()], cats2[c.GetCategoryID()]))
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, $"SFGameDataNew.Diff() failed: failed to diff category {c.GetCategoryID()}");
                    return -118;
                }
            }

            return 0;
        }

        // loads gamedata from in-memory chunkfile
        public int Import(SFChunkFile sfcf)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameDataNew.Import() called");
            foreach (var cat in GetCategories())
            {
                bool read_result = cat.Load(sfcf);
                if(!read_result)
                {
                    LogUtils.Log.Info(LogUtils.LogSource.SFCFF, $"SFGameDataNew.Import(): failed to load chunk [{cat.GetCategoryID()}, {cat.GetCategoryType()}]");
                    sfcf.Close();
                    return -118;
                }
            }

            return 0;
        }

        // unloads all stored data
        public int Clear()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameDataNew.Unload() called");

            foreach (var cat in GetCategories())
            {
                cat.Clear();
            }

            fname = "";
            return 0;
        }
    }
}
