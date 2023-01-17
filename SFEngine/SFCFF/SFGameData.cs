/*
 * SFGameData is a structure which holds all data from a .cff file
 */

using SFEngine.SFChunk;
using System;
using System.Collections.Generic;

namespace SFEngine.SFCFF
{
    public struct CatColumn
    {
        public int category;
        public int column;
        public CatColumn(int c, int co)
        {
            category = c;
            column = co;
        }
    }

    public class SFGameData
    {
        public static Dictionary<int, CatColumn[]> ReferenceCategoryTable = new Dictionary<int, CatColumn[]>();

        public Dictionary<int, SFCategory> categories = new Dictionary<int, SFCategory>();
        public string fname = "";            // points nowhere if gamedata is not loaded, points to last know gamedata file which at some point of time had same content as this gamedata

        static SFGameData()
        {
            // -1 - spell ID in category 2002
            // -2 - unit ID in category 2002
            ReferenceCategoryTable.Add(2002, new CatColumn[] { new CatColumn(2002, -1), new CatColumn(2067, 2), new CatColumn(2014, 2), new CatColumn(2018, 1), new CatColumn(2026, 2) });  //-1 means custom columns
            ReferenceCategoryTable.Add(2054, new CatColumn[] { new CatColumn(2002, 1) });
            ReferenceCategoryTable.Add(2056, new CatColumn[] { });
            ReferenceCategoryTable.Add(2005, new CatColumn[] { new CatColumn(2006, 0), new CatColumn(2067, 0), new CatColumn(2003, 4), new CatColumn(2024, 2) });
            ReferenceCategoryTable.Add(2006, new CatColumn[] { new CatColumn(2005, 0), new CatColumn(2067, 0), new CatColumn(2003, 4), new CatColumn(2024, 2) });
            ReferenceCategoryTable.Add(2067, new CatColumn[] { new CatColumn(2005, 0), new CatColumn(2006, 0), new CatColumn(2003, 4), new CatColumn(2024, 2) });
            ReferenceCategoryTable.Add(2003, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2004, new CatColumn[] { new CatColumn(2003, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2013, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2003, 0), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2015, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2003, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2017, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2003, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2014, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2003, 0), new CatColumn(2012, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2012, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2003, 0), new CatColumn(2018, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2018, new CatColumn[] { new CatColumn(2004, 0), new CatColumn(2013, 0), new CatColumn(2013, 1), new CatColumn(2015, 0), new CatColumn(2017, 0), new CatColumn(2014, 0), new CatColumn(2012, 0), new CatColumn(2003, 0), new CatColumn(2025, 2), new CatColumn(2040, 2), new CatColumn(2040, 4), new CatColumn(2040, 6), new CatColumn(2042, 1), new CatColumn(2065, 2), new CatColumn(2065, 4), new CatColumn(2065, 6), });
            ReferenceCategoryTable.Add(2016, new CatColumn[] { new CatColumn(2054, 1), new CatColumn(2003, 3), new CatColumn(2022, 7), new CatColumn(2024, 1), new CatColumn(2029, 5), new CatColumn(2039, 2), new CatColumn(2044, 1), new CatColumn(2050, 1), new CatColumn(2051, 1), new CatColumn(2052, 3), new CatColumn(2053, 5), new CatColumn(2058, 1), new CatColumn(2059, 1), new CatColumn(2059, 2), new CatColumn(2061, 3), new CatColumn(2063, 1), new CatColumn(2064, 1), new CatColumn(2036, 2), new CatColumn(2072, 1) });
            ReferenceCategoryTable.Add(2022, new CatColumn[] { new CatColumn(2005, 2) });
            ReferenceCategoryTable.Add(2023, new CatColumn[] { new CatColumn(2022, 9) });
            ReferenceCategoryTable.Add(2024, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2025, 0), new CatColumn(2026, 0), new CatColumn(2028, 0), new CatColumn(2040, 0), new CatColumn(2001, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2025, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2024, 0), new CatColumn(2026, 0), new CatColumn(2028, 0), new CatColumn(2040, 0), new CatColumn(2001, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2026, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2025, 0), new CatColumn(2024, 0), new CatColumn(2028, 0), new CatColumn(2040, 0), new CatColumn(2001, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2028, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2025, 0), new CatColumn(2026, 0), new CatColumn(2024, 0), new CatColumn(2040, 0), new CatColumn(2001, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2040, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2025, 0), new CatColumn(2026, 0), new CatColumn(2028, 0), new CatColumn(2024, 0), new CatColumn(2001, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2001, new CatColumn[] { new CatColumn(2002, -2), new CatColumn(2003, 5), new CatColumn(2025, 0), new CatColumn(2026, 0), new CatColumn(2028, 0), new CatColumn(2040, 0), new CatColumn(2024, 0), new CatColumn(2041, 1) });
            ReferenceCategoryTable.Add(2029, new CatColumn[] { new CatColumn(2003, 6), new CatColumn(2001, 2), new CatColumn(2029, 10), new CatColumn(2030, 0), new CatColumn(2031, 0), new CatColumn(2036, 1) });
            ReferenceCategoryTable.Add(2030, new CatColumn[] { new CatColumn(2003, 6), new CatColumn(2001, 2), new CatColumn(2029, 10), new CatColumn(2029, 0), new CatColumn(2031, 0), new CatColumn(2036, 1) });
            ReferenceCategoryTable.Add(2031, new CatColumn[] { new CatColumn(2003, 6), new CatColumn(2001, 2), new CatColumn(2029, 10), new CatColumn(2030, 0), new CatColumn(2029, 0), new CatColumn(2036, 1) });
            ReferenceCategoryTable.Add(2039, new CatColumn[] { });
            ReferenceCategoryTable.Add(2062, new CatColumn[] { });
            ReferenceCategoryTable.Add(2041, new CatColumn[] { new CatColumn(2042, 0), new CatColumn(2047, 0) });
            ReferenceCategoryTable.Add(2042, new CatColumn[] { new CatColumn(2041, 0), new CatColumn(2047, 0) });
            ReferenceCategoryTable.Add(2047, new CatColumn[] { new CatColumn(2042, 0), new CatColumn(2041, 0) });
            ReferenceCategoryTable.Add(2044, new CatColumn[] { new CatColumn(2028, 1), new CatColumn(2031, 1) });
            ReferenceCategoryTable.Add(2048, new CatColumn[] { });
            ReferenceCategoryTable.Add(2050, new CatColumn[] { new CatColumn(2057, 0), new CatColumn(2065, 0) });
            ReferenceCategoryTable.Add(2057, new CatColumn[] { new CatColumn(2050, 0), new CatColumn(2065, 0) });
            ReferenceCategoryTable.Add(2065, new CatColumn[] { new CatColumn(2050, 0), new CatColumn(2057, 0) });
            ReferenceCategoryTable.Add(2051, new CatColumn[] { });
            ReferenceCategoryTable.Add(2052, new CatColumn[] { new CatColumn(2053, 1) });
            ReferenceCategoryTable.Add(2053, new CatColumn[] { });
            ReferenceCategoryTable.Add(2055, new CatColumn[] { });
            ReferenceCategoryTable.Add(2058, new CatColumn[] { new CatColumn(2054, 8), new CatColumn(2036, 3) });
            ReferenceCategoryTable.Add(2059, new CatColumn[] { });
            ReferenceCategoryTable.Add(2061, new CatColumn[] { new CatColumn(2061, 1) });
            ReferenceCategoryTable.Add(2063, new CatColumn[] { new CatColumn(2015, 6) });
            ReferenceCategoryTable.Add(2064, new CatColumn[] { new CatColumn(2015, 7) });
            ReferenceCategoryTable.Add(2032, new CatColumn[] { });
            ReferenceCategoryTable.Add(2049, new CatColumn[] { });
            ReferenceCategoryTable.Add(2036, new CatColumn[] { });
            ReferenceCategoryTable.Add(2072, new CatColumn[] { new CatColumn(2003, 10) });
        }

        public SFCategory this[int index]
        {
            get
            {
                if (categories.ContainsKey(index))
                {
                    return categories[index];
                }

                return null;
            }
        }

        public SFCategory GetPrecise(Tuple<ushort, ushort> key)
        {
            if (categories.ContainsKey(key.Item1))
            {
                if (categories[key.Item1].category_type == key.Item2)
                {
                    return categories[key.Item1];
                }
            }

            return null;
        }

        // loads gamedata from file
        public int Load(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameData.Load() called");

            fname = "";
            SFChunkFile sfcf = new SFChunkFile();
            int result = sfcf.OpenFile(filename);
            if (result != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameData.Load() failed!");
                return result;
            }

            result = Import(sfcf);
            sfcf.Close();

            if (result != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameData.Load() failed!");
                return result;
            }

            CompatibilitySortCategories();
#if DEBUG
            CustomScript();                         // only for experimental purposes
#endif


            fname = filename;

            return result;
        }

        // saves gamedata to file
        public int Save(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameData.Save() called");

            SFChunkFile sfcf = new SFChunkFile();
            int result = sfcf.CreateFile(filename, SFChunkFileType.GAMEDATA);
            if (result != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameData.Save() failed!");
                return result;
            }

            foreach (var cat in categories)
            {
                int cat_status = categories[cat.Key].Write(sfcf);
                if (cat_status != 0)
                {
                    sfcf.Close();

                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameData.Save() failed!");
                    return cat_status;
                }
            }

            sfcf.Close();
            fname = filename;

            return 0;
        }

        // saves gamedata diff to file
        public int SaveDiff(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameData.SaveDiff() called");

            SFChunkFile sfcf = new SFChunkFile();
            int result = sfcf.CreateFile(filename, SFChunkFileType.GAMEDATA);
            if (result != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameData.SaveDiff() failed!");
                return result;
            }

            foreach (var cat in categories)
            {
                int cat_status = categories[cat.Key].WriteDiff(sfcf);
                if (cat_status != 0)
                {
                    sfcf.Close();

                    LogUtils.Log.Error(LogUtils.LogSource.SFCFF, "SFGameData.SaveDiff() failed!");
                    return cat_status;
                }
            }

            sfcf.Close();
            fname = filename;

            return 0;
        }


        // loads gamedata from in-memory chunkfile
        public int Import(SFChunkFile sfcf)
        {
            foreach (var chunk in sfcf.GetAllChunks())
            {
                int cat_status = ImportChunk(chunk);
                if (cat_status < 0)
                {
                    sfcf.Close();
                    return cat_status;
                }
            }

            // fix up cat 2016
            SFCategory cat_text = this[2016];
            if (cat_text != null)
            {
                cat_text.special_cat2016_DetermineLanguageIDs();
            }

            return 0;
        }

        // adds chunk to gamedata
        // if chunk already exists, and is of same type, chunks are merged (existing chunk entries are replaced if need be); if merge fails, the function fails
        // if chunk already exists, but is of a different type, the function fails
        public int ImportChunk(SFChunkFileChunk sfcfc)
        {
            Tuple<ushort, ushort> cat_id = Tuple.Create((ushort)sfcfc.header.ChunkID, (ushort)sfcfc.header.ChunkDataType);
            SFCategory existing_cat = GetPrecise(cat_id);
            if (existing_cat == null)
            {
                existing_cat = this[sfcfc.header.ChunkID];
                if (existing_cat != null)
                {
                    return -4;                                // chunk with same id but other version already exists, can't merge
                }
            }

            SFCategory cat = new SFCategory(cat_id);
            int cat_status = cat.Read(sfcfc);
            if (cat_status != 0)
            {
                return cat_status;
            }

            if (existing_cat == null)
            {
                categories.Add(sfcfc.header.ChunkID, cat);
                return 0;
            }
            else
            {
                SFCategory merged_cat;
                if (SFCategory.Merge(existing_cat, cat, out merged_cat) != 0)
                {
                    return -5;
                }

                categories[sfcfc.header.ChunkID] = merged_cat;
                return 1;
            }
        }

        public static bool Merge(SFGameData GD1, SFGameData GD2, out SFGameData ret)
        {
            ret = new SFGameData();
            int result;

            foreach (var cat in GD1.categories)
            {
                SFCategory cat1 = cat.Value;
                SFCategory cat2 = GD2[cat.Key];

                if (cat2 == null)
                {
                    ret.categories.Add(cat.Key, cat1);
                }
                else if (cat1.category_type != cat2.category_type)
                {
                    return false;
                }
                else
                {
                    SFCategory merge_cat;
                    result = SFCategory.Merge(cat1, cat2, out merge_cat);
                    if (result != 0)
                    {
                        return false;
                    }

                    ret.categories.Add(cat.Key, merge_cat);
                }
            }
            foreach (var cat in GD2.categories)
            {
                SFCategory cat1 = GD1[cat.Key];
                SFCategory cat2 = cat.Value;

                if (cat1 == null)
                {
                    ret.categories.Add(cat.Key, cat2);
                }
            }

            return true;
        }

        public static bool CalculateStatus(SFGameData GD1, SFGameData GD2, ref SFGameData GDref)
        {
            foreach (var cat in GDref.categories)
            {
                cat.Value.element_status.Clear();
            }

            if ((GD1 == null) && (GD2 == null))
            {
                foreach (var cat in GDref.categories)
                {
                    for (int i = 0; i < cat.Value.GetElementCount(); i++)
                    {
                        cat.Value.element_status.Add(SFCategoryElementStatus.UNCHANGED);
                    }
                }

                return true;
            }
            foreach (var cat in GDref.categories)
            {
                SFCategory cat1 = GD1[cat.Key];
                SFCategory cat2 = GD2[cat.Key];

                if (cat1 == null)
                {
                    if (cat2 == null)
                    {
                        return false;
                    }

                    for (int i = 0; i < cat2.GetElementCount(); i++)
                    {
                        cat.Value.element_status.Add(SFCategoryElementStatus.ADDED);
                    }
                }
                else if (cat2 == null)
                {
                    for (int i = 0; i < cat1.GetElementCount(); i++)
                    {
                        cat.Value.element_status.Add(SFCategoryElementStatus.REMOVED);
                    }
                }
                else
                {
                    SFCategory cat_status = cat.Value;
                    SFCategory.CalculateStatus(cat1, cat2, ref cat_status);
                }
            }

            return true;
        }



        public void CompatibilitySortCategories()
        {
            int[] categories_proper_order = new int[]
            {
                2002, 2054, 2056, 2005, 2006, 2067, 2003, 2004, 2013, 2015, 2017, 2014, 2012,
                2018, 2016, 2022, 2023, 2024, 2025, 2026, 2028, 2040, 2001, 2029, 2030, 2031,
                2039, 2062, 2041, 2042, 2047, 2044, 2048, 2050, 2057, 2065, 2051, 2052, 2053,
                2055, 2058, 2059, 2061, 2063, 2064, 2032, 2049, 2036, 2072
            };

            Dictionary<int, SFCategory> new_categories = new Dictionary<int, SFCategory>();
            foreach (int i in categories_proper_order)
            {
                if (categories.ContainsKey(i))
                {
                    new_categories.Add(i, categories[i]);
                }
            }

            categories = new_categories;
        }

        public void CustomScript()
        {
            // shift text IDs from a given region by a certain amount
            /*ushort start_id = 48294;
            ushort end_id = 49928;
            ushort new_id = 50000;

            if (categories[2016] == null)
                return;

            foreach(var elem_list in categories[2016].element_lists)
            {
                foreach(var elem in elem_list.Elements)
                {
                    if (((ushort)(elem[0]) >= start_id) && ((ushort)(elem[0]) <= end_id)) 
                        elem[0] = (ushort)((ushort)(elem[0]) + (new_id - start_id));
                }
            }

            foreach(var ref_cat in ReferenceCategoryTable[2016])
            {
                if (categories[ref_cat.category] == null)
                    continue;

                if(categories[ref_cat.category].category_allow_multiple)
                {
                    foreach(var elem_list in categories[ref_cat.category].element_lists)
                    {
                        foreach(var elem in elem_list.Elements)
                        {
                            if (((ushort)(elem[ref_cat.column]) >= start_id) && ((ushort)(elem[ref_cat.column]) <= end_id))
                                elem[ref_cat.column] = (ushort)((ushort)(elem[ref_cat.column]) + (new_id - start_id));
                        }
                    }
                }
                else
                {
                    foreach (var elem in categories[ref_cat.category].elements)
                    {
                        if (((ushort)(elem[ref_cat.column]) >= start_id) && ((ushort)(elem[ref_cat.column]) <= end_id))
                            elem[ref_cat.column] = (ushort)((ushort)(elem[ref_cat.column]) + (new_id - start_id));
                    }
                }
            }*/

            // extract ID texts from languages 0, 2-6
            /*if (categories[2016] == null)
                return;

            HashSet<int> removed_cat = new HashSet<int>();
            foreach (var kv in categories)
            { 
                if (kv.Key != 2016)
                    removed_cat.Add(kv.Key);
            }
            foreach (var cat in removed_cat)
                categories.Remove(cat);

            foreach(var elem_list in categories[2016].element_lists)
            {
                for(int i = 0; i < elem_list.Elements.Count; i++)
                {
                    if((byte)(elem_list[i][1]) == 1)
                    {
                        elem_list.Elements.RemoveAt(i);
                        i -= 1;
                    }
                }
            }*/

            // fix category 2012
            /*if (categories[2012] == null)
                return;

            foreach(var elem_list in categories[2012].element_lists)
            {
                int min = (byte)(elem_list.Elements[0][1]);
                for(int i = 1; i < elem_list.Elements.Count; i++)
                {
                    min = Math.Min(min, (int)(byte)(elem_list.Elements[i][1]));
                }

                if(min == 0)
                {
                    foreach(var elem in elem_list.Elements)
                    {
                        elem[1] = (byte)((byte)(elem[1]) + 1);
                    }
                }
            }

            // fix category 2026
            if (categories[2026] == null)
                return;

            foreach(var elem_list in categories[2026].element_lists)
            {
                int min = (byte)(elem_list.Elements[0][1]);
                for(int i = 1; i < elem_list.Elements.Count; i++)
                {
                    min = Math.Min(min, (int)(byte)(elem_list.Elements[i][1]));
                }

                if(min == 0)
                {
                    foreach(var elem in elem_list.Elements)
                    {
                        elem[1] = (byte)((byte)(elem[1]) + 1);
                    }
                }
            }

            // fix category 2014
            if (categories[2014] == null)
                return;

            foreach(var elem_list in categories[2014].element_lists)
            {
                int min = (byte)(elem_list.Elements[0][1]);
                for(int i = 1; i < elem_list.Elements.Count; i++)
                {
                    min = Math.Min(min, (int)(byte)(elem_list.Elements[i][1]));
                }

                if(min == 0)
                {
                    foreach(var elem in elem_list.Elements)
                    {
                        elem[1] = (byte)((byte)(elem[1]) + 1);
                    }
                }
            }*/
        }

        // unloads all stored data
        // returns if succeeded
        public int Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameData.Unload() called");


            foreach (var cat in categories)
            {
                cat.Value.Unload();
            }

            categories.Clear();

            fname = "";
            return 0;
        }
    }
}
