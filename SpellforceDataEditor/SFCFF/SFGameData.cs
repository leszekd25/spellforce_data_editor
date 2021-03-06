﻿/*
 * SFGameData is a structure which holds all data from a .cff file
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.IO;
using SpellforceDataEditor.SFChunk;

namespace SpellforceDataEditor.SFCFF
{
    public class SFGameData
    {
        public Dictionary<int, SFCategory> categories = new Dictionary<int, SFCategory>();

        public string fname = "";            // points nowhere if gamedata is not loaded, points to last know gamedata file which at some point of time had same content as this gamedata

        public SFCategory this[int index]
        {
            get
            {
                if(categories.ContainsKey(index))
                    return categories[index];

                return null;
            }
        }

        public SFCategory GetPrecise(Tuple<ushort, ushort> key)
        {
            if(categories.ContainsKey(key.Item1))
            {
                if (categories[key.Item1].category_type == key.Item2)
                    return categories[key.Item1];
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
            if(result != 0)
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

            return 0;
        }

        // adds chunk to gamedata
        // if chunk already exists, and is of same type, chunks are merged (existing chunk entries are replaced if need be); if merge fails, the function fails
        // if chunk already exists, but is of a different type, the function fails
        public int ImportChunk(SFChunkFileChunk sfcfc)
        {
            SFCategory existing_cat = GetPrecise(Tuple.Create((ushort)sfcfc.header.ChunkID, (ushort)sfcfc.header.ChunkDataType));
            if(existing_cat == null)
            {
                existing_cat = this[sfcfc.header.ChunkID];
                if(existing_cat != null)
                {
                    return -4;                                // chunk with same id but other version already exists, can't merge
                }
            }

            SFCategory cat = new SFCategory();
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
                    return -5;

                categories[sfcfc.header.ChunkID] = merged_cat;
                return 1;
            }
        }

        public static bool Merge(SFGameData GD1, SFGameData GD2, out SFGameData ret)
        {
            ret = new SFGameData();
            int result;

            foreach(var cat in GD1.categories)
            {
                SFCategory cat1 = cat.Value;
                SFCategory cat2 = GD2[cat.Key];

                if(cat2 == null)
                {
                    ret.categories.Add(cat.Key, cat1);
                }
                else if(cat1.category_type != cat2.category_type)
                {
                    return false;
                }    
                else
                {
                    SFCategory merge_cat;
                    result = SFCategory.Merge(cat1, cat2, out merge_cat);
                    if (result != 0)
                        return false;

                    ret.categories.Add(cat.Key, merge_cat);
                }
            }
            foreach(var cat in GD2.categories)
            {
                SFCategory cat1 = GD1[cat.Key];
                SFCategory cat2 = cat.Value;

                if(cat1 == null)
                {
                    ret.categories.Add(cat.Key, cat2);
                }
            }

            return true;
        }

        public static bool CalculateStatus(SFGameData GD1, SFGameData GD2, ref SFGameData GDref)
        {
            foreach (var cat in GDref.categories)
                cat.Value.element_status.Clear();

            if ((GD1 == null)&&(GD2 == null))
            {
                foreach (var cat in GDref.categories)
                    for (int i = 0; i < cat.Value.GetElementCount(); i++)
                        cat.Value.element_status.Add(SFCategoryElementStatus.UNCHANGED);

                return true;
            }
            foreach(var cat in GDref.categories)
            {
                SFCategory cat1 = GD1[cat.Key];
                SFCategory cat2 = GD2[cat.Key];

                if (cat1 == null)
                {
                    if (cat2 == null)
                        return false;

                    for (int i = 0; i < cat2.GetElementCount(); i++)
                        cat.Value.element_status.Add(SFCategoryElementStatus.ADDED);
                }
                else if (cat2 == null)
                {
                    for (int i = 0; i < cat1.GetElementCount(); i++)
                        cat.Value.element_status.Add(SFCategoryElementStatus.REMOVED);
                }
                else
                {
                    SFCategory cat_status = cat.Value;
                    SFCategory.CalculateStatus(cat1, cat2, ref cat_status);
                }
            }

            return true;
        }



        // unloads all stored data
        // returns if succeeded
        public int Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFCFF, "SFGameData.Unload() called");


            foreach (var cat in categories)
                cat.Value.Unload();
            categories.Clear();

            fname = "";
            return 0;
        }
    }
}
