using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using System.IO;
using SFEngine.SF3D;
using SFEngine.SFUnPak;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SFEngine.SFMap
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
        // game loads 63 base textures, and that much are loaded, but for the purpose of the editor only 32 textures are used
        // texture 0 is always base world texture (ID 0), texures 1-31 are terrain textures seen from far away
        // textures 32-62 are the actual used textures, together with texture 0
        public const int MAX_TEXTURES = 63;
        public const int MAX_USED_TEXTURES = 32;
        public const int MAX_TILES = 255;
        public const int TEXTURES_AVAILABLE = 119;

        // map info contains texture IDs for all textures - both far textures and proper textures
        // in game, proper texture = far texture + TEXTURES_AVAILABLE
        // exception: texture 0, proper texture = far texture
        public byte[] texture_id { get; private set; } = new byte[MAX_TEXTURES];

        // all available texture names are here
        List<string> texture_filenames;

        // all 32 used textures - only the last 31 of those are modifiable in the editor
        // 1st texture is used as bump map for far terrain :)
        public SFTexture[] base_texture_bank { get; private set; } = new SFTexture[MAX_USED_TEXTURES];

        public Image[] texture_base_image = new Image[TEXTURES_AVAILABLE + 1]; // all base textures in the game
        public bool base_images_loaded { get; private set; } = false;

        SFTexture texture_tile_mixer = null;

        // each tile contains data which is saved in map file
        public SFMapTerrainTextureTileData[] texture_tiledata { get; private set; } = new SFMapTerrainTextureTileData[MAX_TILES];
        public Image[] texture_tile_image = new Image[MAX_TILES];         // tiles loaded - first 31 are loaded bases
        // helper array to check if a mixed tile is defined
        public bool[] tile_defined { get; private set; } = new bool[MAX_TILES];
        /// <summary>
        /// Contains the average color for each tile texture (updated in RefreshTilePreview)
        /// </summary>
        public Color[] tile_average_color = new Color[MAX_TILES];
        public Color tile_ocean_color;

        // map consists of tiles: tiles 0-31 are textures from texture_id, 32-223 - mixed custom tiles
        // tiles 224-255 are tiles with proper texture
        // mixed tiles use IDs from 0 to 31...

        public int terrain_texture { get; private set; } = -1;     //texture GL ID

        // tilemap texture buffer
        int uniformTileData_buffer;
        int[] uniformTileData;
        // tile color buffer
        int uniformTileColor_buffer;
        Vector4[] uniformTileColor;


        public void LoadTextureNames()
        {
            if (SFUnPak.SFUnPak.game_directory_name == "")
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.LoadTextureNames(): Unspecified game directory!");
                return;
            }

            List<string> tmp_terrain_list = SFUnPak.SFUnPak.ListAllWithFilename("texture", "landscape_island_", new string[] { "sf1.pak" });
            tmp_terrain_list.Remove("landscape_island_100_mud.dds");
            tmp_terrain_list.Remove("landscape_island_100_mudd.dds");

            texture_filenames = new List<string>();
            texture_filenames.Add("landscape_island_worldd.dds");

            List<string> tmp_helper_list = new List<string>();
            for(int i = 1; i <= TEXTURES_AVAILABLE; i++)
            {
                string s = String.Format("{0,3:000}", i);

                for(int j = 0; j < tmp_terrain_list.Count; j++)
                {
                    if(tmp_terrain_list[j].Contains(s))
                    {
                        tmp_helper_list.Add(tmp_terrain_list[j]);
                    }
                }
                if(tmp_helper_list.Count == 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.LoadTextureNames(): Could not find texture with ID " + i.ToString());
                    continue;
                }

                // add longer of the filenames
                int max_ind = 0;
                int max_len = tmp_helper_list[0].Length;
                for (int j = 1; j < tmp_helper_list.Count; j++)
                {
                    if(tmp_helper_list[j].Length > max_len)
                    {
                        max_ind = j;
                        max_len = tmp_helper_list[j].Length;
                    }
                }

                texture_filenames.Add(tmp_helper_list[max_ind]);

                tmp_helper_list.Clear();
            }

            // texture_filenames = SFUnPak.SFUnPak.ListAllWithFilename("texture", "landscape_island_", new string[] { "sf1.pak" });
            // bugfix for sf1.pak texture list
            /*texture_filenames.Remove("landscape_island_100_mud.dds");
            texture_filenames.Remove("landscape_island_100_mudd.dds");*/

        }

        public void SetTextureIDsRaw(byte[] data)
        {
            for (int i = 0; i < MAX_TEXTURES; i++)
                texture_id[i] = data[i];
        }

        public string GetTextureNameByID(int id)
        {
            if(id > TEXTURES_AVAILABLE)
            {
                id -= TEXTURES_AVAILABLE;
            }
            return texture_filenames[id];
        }

        // loads terrain texture with a given id
        public SFTexture LoadTerrainTexture(int tex_id)
        {
            string filename = GetTextureNameByID(tex_id);
            SFTexture tex = new SFTexture();

            MemoryStream ms = SFUnPak.SFUnPak.LoadFileFrom("sf32.pak", "texture\\" + filename);
            if (ms == null) ms = SFUnPak.SFUnPak.LoadFileFrom("sf22.pak", "texture\\" + filename);
            if (ms == null) ms = SFUnPak.SFUnPak.LoadFileFrom("sf1.pak", "texture\\" + filename);
            if (ms == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.LoadTerrainTexture(): Could not find texture " + filename);
                throw new Exception("SFMapTerrainTextureManager.Init(): Can't find texture!");
            }
            int res_code = tex.Load(ms, 1);
            if (res_code != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.LoadTerrainTexture(): Could not load texture " + filename);
                throw new Exception("SFMapTerrainTextureManager.Init(): Can't load texture!");
            }
            ms.Close();

            tex.Uncompress();     // needed for this to work in texture atlas (?)
            tex.Init();
            return tex;
        }

        // generate opengl array texture for heightmap
        public void Init()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.Init() called");

            LoadTextureNames();
            // load base textures
            for (int i = 0; i < MAX_USED_TEXTURES; i++)
            {
                if (i == 0)
                    base_texture_bank[i] = LoadTerrainTexture(texture_id[i]);
                else
                    base_texture_bank[i] = LoadTerrainTexture(texture_id[i + 31]);
            }

            // generate inbetween textures
            texture_tile_mixer = new SFTexture();
            tile_defined[0] = true;
            for (int i = 1; i < 32; i++)
                tile_defined[i] = true;
            for(int i = 224; i < 255; i++)
                tile_defined[i] = true;

            for (int i = 32; i < 224; i++)
            {
                if ((texture_tiledata[i].ind1 != 0) || (texture_tiledata[i].ind2 != 0) || (texture_tiledata[i].ind3 != 0))
                    tile_defined[i] = true;
                else
                    tile_defined[i] = false;
            }

            /*int mipmap_divisor = 1;
            int min_allowed_level = 0;
            int _size = 256;
            while(_size > Settings.MaximumAllowedTextureSize) { min_allowed_level += 1; _size /= 2; }

            min_allowed_level = (min_allowed_level > Settings.IgnoredMipMapsCount ? min_allowed_level : Settings.IgnoredMipMapsCount);

            for (int i = 0; i < min_allowed_level; i++)
                mipmap_divisor *= 2;*/

            terrain_texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, terrain_texture);

            //GL.TexStorage3D(TextureTarget3d.Texture2DArray, 8 - min_allowed_level, SizedInternalFormat.Rgba8, 256/mipmap_divisor, 256/mipmap_divisor, 32);
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 8, SizedInternalFormat.Rgba8, 256, 256, 32);
            for (int i = 0; i < 32; i++)
            {
                //int offset = 0;

                //int w = 256;
                //int h = 256;
                //min_allowed_level = 1000;

                GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, 256, 256, 1, PixelFormat.Rgba, PixelType.UnsignedByte, base_texture_bank[i].data);
                /*for (int level = 0; level < 8 && (w != 0 || h != 0); ++level)
                {
                    int size = ((w + 3) / 4) * ((h + 3) / 4) * 64;

                    if (base_texture_bank[i].IsValidMipMapLevel(level))
                    {
                        if (min_allowed_level > level) min_allowed_level = level;

                        GL.TexSubImage3D(TextureTarget.Texture2DArray, level - min_allowed_level, 0, 0, i, w, h, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref base_texture_bank[i].data[offset]);

                        offset += size;
                    }
                    w /= 2;
                    h /= 2;
                }*/
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)All.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)All.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)All.Linear);
            if (Settings.AnisotropicFiltering)
                GL.TexParameter(TextureTarget.Texture2DArray, (TextureParameterName)All.TextureMaxAnisotropy, (float)Settings.MaxAnisotropy);

            GL.BindTexture(TextureTarget.Texture2DArray, 0);

            GenerateTileImages();
            
            // create uniform buffer object for tiledata
            uniformTileData_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, uniformTileData_buffer);
            GL.BufferData(BufferTarget.UniformBuffer, 2 * 4 * 4 * MAX_TILES, new IntPtr(0), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            uniformTileData = new int[2 * 4 * MAX_TILES];
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, uniformTileData_buffer, new IntPtr(0), 2 * 4 * 4 * MAX_TILES);

            UpdateUniformTileData(0, MAX_TILES - 1);

            // create uniform buffer object for tile color
            uniformTileColor_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, uniformTileColor_buffer);
            GL.BufferData(BufferTarget.UniformBuffer, 4 * 4 * MAX_TILES, new IntPtr(0), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            uniformTileColor = new Vector4[MAX_TILES];
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 2, uniformTileColor_buffer, new IntPtr(0), 4 * 4 * MAX_TILES);  // 2, not 1, 1 is reserved for overlays (see heightmap)

            UpdateUniformTileColor(0, MAX_TILES - 1);

            tile_ocean_color = Color.FromArgb(100, 100, 255);
        }

        public void UpdateUniformTileData(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                if (i < 224)
                    uniformTileData[i * 4 + 0] = texture_tiledata[i].ind1;
                else
                    uniformTileData[i * 4 + 0] = texture_tiledata[i].ind1 - 31;

                uniformTileData[i * 4 + 1] = texture_tiledata[i].ind2;
                uniformTileData[i * 4 + 2] = texture_tiledata[i].ind3;
                uniformTileData[i * 4 + 3] = 0;
            }
            for (int i = start; i <= end; i++)
            {
                uniformTileData[MAX_TILES * 4 + i * 4 + 0] = texture_tiledata[i].weight1;
                uniformTileData[MAX_TILES * 4 + i * 4 + 1] = texture_tiledata[i].weight2;
                uniformTileData[MAX_TILES * 4 + i * 4 + 2] = texture_tiledata[i].weight3;
                uniformTileData[MAX_TILES * 4 + i * 4 + 3] = 0;
            }
            // exceptional case...
            uniformTileData[0] = 0;

            GL.BindBuffer(BufferTarget.UniformBuffer, uniformTileData_buffer);
            GL.BufferSubData(BufferTarget.UniformBuffer, new IntPtr(4 * 4 * start), 4 * 4 * (end - start) + 4 * 4, ref uniformTileData[4 * start]);
            GL.BufferSubData(BufferTarget.UniformBuffer, new IntPtr(4 * 4 * (start + MAX_TILES)), 4 * 4 * (end - start) + 4 * 4, ref uniformTileData[4 * (start + MAX_TILES)]);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public Vector4 VecFromCol(Color c)
        {
            return new Vector4(c.R/255.0f, c.G / 255.0f, c.B / 255.0f, c.A / 255.0f);
        }

        public void UpdateUniformTileColor(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                if(i < 224)
                    uniformTileColor[i] = VecFromCol(tile_average_color[i]);
                else
                    uniformTileColor[i] = VecFromCol(tile_average_color[i- 223]);
            }

            GL.BindBuffer(BufferTarget.UniformBuffer, uniformTileColor_buffer);
            GL.BufferSubData(BufferTarget.UniformBuffer, new IntPtr(4 * 4 * start), 4 * 4 * (end - start) + 4 * 4, ref uniformTileColor[start]);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public bool SetBaseTexture(int base_index, int tex_id)
        {
            // only 1-31 allowed
            if((base_index <= 0)||(base_index >= MAX_USED_TEXTURES))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.SetBaseTexture(): Invalid base index " + base_index.ToString());

                return false;
            }
            // only 1-119 allowed
            if ((tex_id <= 0) || (tex_id > TEXTURES_AVAILABLE))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.SetBaseTexture(): Invalid terrain texture ID "+tex_id.ToString());

                return false;
            }
            texture_id[base_index] = (byte)tex_id;
            texture_id[base_index + 31] = (byte)(tex_id + TEXTURES_AVAILABLE);
            // unload existing texture
            base_texture_bank[base_index].Dispose();
            // load new texture
            base_texture_bank[base_index] = LoadTerrainTexture(tex_id + TEXTURES_AVAILABLE);

            // insert texture data to the atlas
            GL.BindTexture(TextureTarget.Texture2DArray, terrain_texture);

            GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, base_index, 256, 256, 1, PixelFormat.Rgba, PixelType.UnsignedByte, base_texture_bank[base_index].data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray); 
            
            GL.BindTexture(TextureTarget.Texture2DArray, 0);
            /*int offset = 0;

            int w = 256;
            int h = 256;
            int min_allowed_level = 1000;

            for (int level = 0; level < 8 && (w != 0 || h != 0); ++level)
            {
                int size = ((w + 3) / 4) * ((h + 3) / 4) * 64;
                if (base_texture_bank[base_index].IsValidMipMapLevel(level))
                {
                    if (min_allowed_level > level)
                        min_allowed_level = level;

                    GL.TexSubImage3D(TextureTarget.Texture2DArray, level - min_allowed_level, 0, 0, base_index, w, h, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref base_texture_bank[base_index].data[offset]);
                    
                    offset += size;
                }
                w /= 2;
                h /= 2;
            }*/
            RefreshTilePreview(base_index);
            UpdateUniformTileColor(base_index, base_index);
            UpdateUniformTileColor(base_index+223, base_index+223);

            //GL.BindTexture(TextureTarget.Texture2DArray, 0);
            return true;
        }

        // operates on a 64x64 mip map level ground texture
        public Bitmap CreateBitmapFromTexture(SFTexture tex)
        {
            return tex.ToBitmap(new SFTexture.SFTextureToBitmapArgs() { ConversionType = SFTexture.SFTextureToBitmapArgType.DIMENSION, DimWidth = 64, DimHeight = 64 });
        }

        // generates previews for all loaded tiles
        public void GenerateTileImages()
        {
            for (int i = 0; i < MAX_TILES - 31; i++)
                RefreshTilePreview(i);
        }

        // generates previews for all existing terrain textures in game
        public void GenerateBaseImages()
        {
            if (base_images_loaded)
                return;

            for(int i = 1; i <= TEXTURES_AVAILABLE; i++)
            {
                SFTexture tex = LoadTerrainTexture(i+TEXTURES_AVAILABLE);
                texture_base_image[i] = CreateBitmapFromTexture(tex);
                tex.Dispose();
            }
            base_images_loaded = true;
        }

        // operates on 64x64 mipmap, hardcoded for now...
        public void GenerateAverageTileColor(int tile_id, SFTexture tex)
        {
            int ignored_level = 2;//0;
            /*while (!tex.IsValidMipMapLevel(ignored_level))
                ignored_level += 1;

            if (ignored_level > 2)
                return;*/

            int offset = (tex.width * tex.height * 4) * (ignored_level > 0 ? 0 : 1)
                       + (tex.width * tex.height) * (ignored_level > 1 ? 0 : 1);

            int r = 0, g = 0, b = 0;

            for (int i = 0; i < 64; i++)
                for (int j = 0; j < 64; j++)
                {
                    r += tex.data[offset + 4 * (i * 64 + j) + 0];
                    g += tex.data[offset + 4 * (i * 64 + j) + 1];
                    b += tex.data[offset + 4 * (i * 64 + j) + 2];
                }

            r /= 4096;
            g /= 4096;
            b /= 4096;

            r = Math.Min(255, (int)(r*1.6f));
            g = Math.Min(255, (int)(g*1.6f));
            b = Math.Min(255, (int)(b*1.6f));

            tile_average_color[tile_id] = Color.FromArgb(r, g, b);
        }

        public void RefreshTilePreview(int tile_id)
        {
            if (tile_id < 0)
                return;

            if (tile_id < 32)
            {
                texture_tile_image[tile_id] = CreateBitmapFromTexture(base_texture_bank[tile_id]);

                GenerateAverageTileColor(tile_id, base_texture_bank[tile_id]);
            }
            else if (tile_defined[tile_id])
            {
                SFTexture.MixUncompressed(
                    base_texture_bank[texture_tiledata[tile_id].ind1], texture_tiledata[tile_id].weight1,
                    base_texture_bank[texture_tiledata[tile_id].ind2], texture_tiledata[tile_id].weight2,
                    base_texture_bank[texture_tiledata[tile_id].ind3], texture_tiledata[tile_id].weight3,
                    ref texture_tile_mixer);
                texture_tile_image[tile_id] = CreateBitmapFromTexture(texture_tile_mixer);

                GenerateAverageTileColor(tile_id, texture_tile_mixer);
            }
        }

        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.Unload() called");
            GL.DeleteTexture(terrain_texture);
            for (int i = 0; i < 32; i++)
                base_texture_bank[i].Dispose();
            GL.DeleteBuffer(uniformTileData_buffer);
        }
    }
}
