using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using System.IO;
using SpellforceDataEditor.SF3D;
using SpellforceDataEditor.SFUnPak;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SFMap
{
    public struct SFMapTerrainTextureReindexer
    {
        public int index;
        public int real_index;
    }

    public struct SFMapTerrainTextureTileData
    {
        public byte ind1, ind2, ind3;
        public byte weight1, weight2, weight3;
        public byte reindex_data, reindex_index;
        public byte material_property;
        public bool blocks_movement, blocks_vision;
    }

    public class SFMapTerrainTextureManager
    {
        public const int MAX_TEXTURES = 63;
        public const int MAX_REINDEX = 255;
        public const int TEXTURES_AVAILABLE = 119;

        public SFTexture[] base_texture_bank { get; private set; } = new SFTexture[MAX_TEXTURES];
        public byte[] texture_id { get; private set; } = new byte[MAX_TEXTURES];
        public SFTexture[] texture_array { get; private set; } = new SFTexture[MAX_REINDEX];
        public SFMapTerrainTextureTileData[] texture_tiledata { get; private set; } = new SFMapTerrainTextureTileData[MAX_REINDEX];
        List<string> texture_filenames;
        public int terrain_texture { get; private set; } = -1;     //texture GL ID


        public void LoadTextureNames()
        {
            if (SFUnPak.SFUnPak.game_directory_name == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.LoadTextureNames(): Unspecified game directory!");
                return;
            }

            texture_filenames = SFUnPak.SFUnPak.ListAllWithFilename("texture", "landscape_island_", new string[] { "sf1.pak"});
            // bugfix for sf1.pak texture list
            texture_filenames.Remove("landscape_island_100_mud.dds");
            texture_filenames.Remove("landscape_island_100_mudd.dds");

        }

        public void SetTextureIDsRaw(byte[] data)
        {
            for (int i = 0; i < MAX_TEXTURES; i++)
                texture_id[i] = data[i];
        }

        public string GetTextureNameByID(int id)
        {
            if (id == 0)
                return "landscape_island_worldd.dds";
            int d_offset = 0;
            if(id > TEXTURES_AVAILABLE)
            {
                id -= TEXTURES_AVAILABLE;
                d_offset = 1;
            }
            id -= 1;
            return texture_filenames[id * 2 + d_offset];
        }

        // generate opengl array texture for heightmap
        public void Init()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.Init() called");

            LoadTextureNames();
            // load base textures
            for (int i = 0; i < MAX_TEXTURES; i++)
            {
                string filename = GetTextureNameByID(texture_id[i]);
                base_texture_bank[i] = new SFTexture();

                MemoryStream ms    = SFUnPak.SFUnPak.LoadFileFrom("sf32.pak", "texture\\" + filename);
                if (ms == null) ms = SFUnPak.SFUnPak.LoadFileFrom("sf22.pak", "texture\\" + filename);
                if (ms == null) ms = SFUnPak.SFUnPak.LoadFileFrom("sf1.pak", "texture\\" + filename);
                if (ms == null)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.Init(): Could not find texture " + filename);
                    throw new Exception("SFMapTerrainTextureManager.Init(): Can't find texture!");
                }
                int res_code = base_texture_bank[i].Load(ms);
                if (res_code != 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.Init(): Could not load texture " + filename);
                    throw new Exception("SFMapTerrainTextureManager.Init(): Can't load texture!");
                }
                ms.Close();

                base_texture_bank[i].Uncompress();
                base_texture_bank[i].Init();
            }
            // generate inbetween textures
            texture_array[0] = base_texture_bank[0];
            for(int i = 1; i < 32; i++)
            {
                texture_array[i] = base_texture_bank[i+31];
            }
            for(int i = 224; i < 255; i++)
            {
                texture_array[i] = base_texture_bank[i - 192];
            }
            for(int i = 32; i < 224; i++)
            {
                if ((texture_tiledata[i].ind1 != 0) && (texture_tiledata[i].ind2 != 0))
                {
                    texture_array[i] = SFTexture.MixUncompressed(base_texture_bank[texture_tiledata[i].ind1+31], texture_tiledata[i].weight1,
                                                                 base_texture_bank[texture_tiledata[i].ind2+31], texture_tiledata[i].weight2);
                    texture_array[i].Init();
                }
                else
                    texture_array[i] = base_texture_bank[0];
            }

            int mipmap_divisor = 1;
            int min_allowed_level = 0;
            int _size = 256;
            while(_size > Settings.MaximumAllowedTextureSize) { min_allowed_level += 1; _size /= 2; }

            min_allowed_level = (min_allowed_level > Settings.IgnoredMipMapsCount ? min_allowed_level : Settings.IgnoredMipMapsCount);

            for (int i = 0; i < min_allowed_level; i++)
                mipmap_divisor *= 2;

            // todo: add mipmaps : )
            terrain_texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, terrain_texture);
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 8 - min_allowed_level, SizedInternalFormat.Rgba8, 256/mipmap_divisor, 256/mipmap_divisor, 255);
            for (int i = 0; i < 255; i++)
            {
                int offset = 0;

                int w = 256;
                int h = 256;
                min_allowed_level = 1000;

                for (int level = 0; level < 8 && (w != 0 || h != 0); ++level)
                {
                    int size = ((w + 3) / 4) * ((h + 3) / 4) * 64;

                    if (texture_array[i].IsValidMipMapLevel(level))
                    {
                        if (min_allowed_level > level) min_allowed_level = level;

                        GL.TexSubImage3D(TextureTarget.Texture2DArray, level - min_allowed_level, 0, 0, i, w, h, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref texture_array[i].data[offset]);

                        offset += size;
                    }
                    w /= 2;
                    h /= 2;
                }

                while (true)
                {
                    ErrorCode ec = GL.GetError();
                    if (ec == ErrorCode.NoError)
                        break;
                    LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.Init(): OpenGL error '" + ec.ToString() + "' for terrain texture id " + i.ToString());
                    System.Diagnostics.Debug.WriteLine("TTM.Init() " + ec+" "+i.ToString());
                }
            }
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)All.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)All.Repeat);
            GL.BindTexture(TextureTarget.Texture2DArray, 0);

            for (int i = 32; i < 224; i++)
                if(!base_texture_bank.Contains(texture_array[i]))
                    texture_array[i].FreeMemory();
        }

        public bool SetBaseTexture(int base_index, int tex_id)
        {
            if((base_index < 0)||(base_index >= MAX_TEXTURES))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.SetBaseTexture(): Invalid base index " + base_index.ToString());
            }
            if ((tex_id <= 0) || (tex_id > 238))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.SetBaseTexture(): Invalid terrain texture ID "+tex_id.ToString());

                return false;
            }
            texture_id[base_index] = (byte)tex_id;
            // unload existing texture
            base_texture_bank[base_index].Dispose();
            // load new texture
            string filename = GetTextureNameByID(tex_id);

            MemoryStream ms = SFUnPak.SFUnPak.LoadFileFrom("sf1.pak", "texture\\" + filename);
            if (ms == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.SetBaseTexture(): Could not find texture "+filename);
                throw new Exception("SFMapTerrainTextureManager.SetBaseTexture(): Can't find texture!");
            }
            int res_code = base_texture_bank[base_index].Load(ms);
            if (res_code != 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.SetBaseTexture(): Could not find texture " + filename);
                throw new Exception("SFMapTerrainTextureManager.SetBaseTexture(): Can't load texture!");
            }
            ms.Close();

            base_texture_bank[base_index].Uncompress();
            base_texture_bank[base_index].Init();
            // insert texture data to the atlas
            int atlas_index = base_index;
            if (base_index >= 32)
                atlas_index = 192 + base_index;

            GL.BindTexture(TextureTarget.Texture2DArray, terrain_texture);

            int offset = 0;

            int w = 256;
            int h = 256;
            int min_allowed_level = 1000;

            for (int level = 0; level < 8 && (w != 0 || h != 0); ++level)
            {
                int size = ((w + 3) / 4) * ((h + 3) / 4) * 64;
                if (texture_array[atlas_index].IsValidMipMapLevel(level))
                {
                    if (min_allowed_level > level) min_allowed_level = level;

                    GL.TexSubImage3D(TextureTarget.Texture2DArray, level - min_allowed_level, 0, 0, atlas_index, w, h, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref texture_array[atlas_index].data[offset]);

                    offset += size;
                }
                w /= 2;
                h /= 2;
            }

            GL.BindTexture(TextureTarget.Texture2DArray, 0);
            // mix all textures...
            for(int i = 32; i < 224; i++)
            {
                if((texture_tiledata[i].ind1 == base_index)||(texture_tiledata[i].ind2 == base_index))
                {
                    texture_array[i].Dispose();
                    texture_array[i] = SFTexture.MixUncompressed(base_texture_bank[texture_tiledata[i].ind1+31], texture_tiledata[i].weight1,
                                                                 base_texture_bank[texture_tiledata[i].ind2+31], texture_tiledata[i].weight2);
                    texture_array[i].Init();

                    offset = 0;

                    w = 256;
                    h = 256;
                    min_allowed_level = 1000;

                    for (int level = 0; level < 8 && (w != 0 || h != 0); ++level)
                    {
                        int size = ((w + 3) / 4) * ((h + 3) / 4) * 64;
                        if (texture_array[i].IsValidMipMapLevel(level))
                        {
                            if (min_allowed_level > level) min_allowed_level = level;

                            GL.TexSubImage3D(TextureTarget.Texture2DArray, level - min_allowed_level, 0, 0, i, w, h, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref texture_array[i].data[offset]);

                            offset += size;
                        }
                        w /= 2;
                        h /= 2;
                    }

                    texture_array[i].FreeMemory();
                }
            }
            return true;
        }

        public void UpdateTileTexture(int tile_id)
        {
            // unload existing texture
            texture_array[tile_id].Dispose();
            // load new texture

            texture_array[tile_id] = SFTexture.MixUncompressed(
                base_texture_bank[texture_tiledata[tile_id].ind1+31], texture_tiledata[tile_id].weight1,
                base_texture_bank[texture_tiledata[tile_id].ind2+31], texture_tiledata[tile_id].weight2);
            texture_array[tile_id].Init();
            // insert texture data to the atlas

            GL.BindTexture(TextureTarget.Texture2DArray, terrain_texture);

            int offset = 0;

            int w = 256;
            int h = 256;
            int min_allowed_level = 1000;

            for (int level = 0; level < 8 && (w != 0 || h != 0); ++level)
            {
                int size = ((w + 3) / 4) * ((h + 3) / 4) * 64;
                if (texture_array[tile_id].IsValidMipMapLevel(level))
                {
                    if (min_allowed_level > level) min_allowed_level = level;

                    GL.TexSubImage3D(TextureTarget.Texture2DArray, level - min_allowed_level, 0, 0, tile_id, w, h, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref texture_array[tile_id].data[offset]);

                    offset += size;
                }
                w /= 2;
                h /= 2;
            }

            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        // operates on a 64x64 mip map level ground texture, hardcoded for now...
        public Bitmap CreateBitmapFromTexture(SFTexture tex)
        {
            Bitmap b = new Bitmap(tex.width / 4, tex.height / 4);
            int ignored_level = 0;
            while (!tex.IsValidMipMapLevel(ignored_level))
                ignored_level += 1;

            if (ignored_level > 2)
                return b;

            int offset = (tex.width * tex.height * 4) * (ignored_level > 0 ? 0 : 1)
                       + (tex.width * tex.height) * (ignored_level > 1 ? 0 : 1);


            for (int i = 0; i < 64; i++)
                for (int j = 0; j < 64; j++)
                    b.SetPixel(i, j, Color.FromArgb(
                        255,
                        tex.data[offset + 4 * (i * 64 + j) + 0],
                        tex.data[offset + 4 * (i * 64 + j) + 1],
                        tex.data[offset + 4 * (i * 64 + j) + 2]));
            return b;
        }


        public void FreeTileMemory(int tile_id)
        {
            if ((tile_id >= 32) && (tile_id < 224))
                texture_array[tile_id].FreeMemory();
        }

        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.Unload() called");
            GL.DeleteTexture(terrain_texture);
            for(int i = 0; i < MAX_REINDEX; i++)
            {
                texture_array[i].Dispose();
            }
        }
    }
}
