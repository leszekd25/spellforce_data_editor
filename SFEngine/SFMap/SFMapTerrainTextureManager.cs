using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFEngine.SF3D;
using SFEngine.SFResources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

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

        public override string ToString()
        {
            return "Tiles: [" + ind1.ToString() + ", " + ind2.ToString() + ", " + ind3.ToString() 
                + "], weights: [" + weight1.ToString() + ", " + weight2.ToString() + ", " + weight3.ToString() + "]";
        }
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
        public SFTexture[] tile_texture_bank = new SFTexture[MAX_TILES];
        // helper array to check if a mixed tile is defined
        public bool[] tile_defined { get; private set; } = new bool[MAX_TILES];
        /// <summary>
        /// Contains the average color for each tile texture (updated in RefreshTilePreview)
        /// </summary>
        public Color[] tile_average_color = new Color[MAX_TILES];
        public Color tile_ocean_color;

        // map consists of tiles: tiles 0-31 are textures from texture_id, 32-223 - mixed custom tiles
        // tiles 224-255 are tiles with proper texture (1-31)
        // mixed tiles use IDs from 0 to 31...

        public int terrain_texture { get; private set; } = -1;     //texture GL ID

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

            // bugfix for sf1.pak texture list
            List<string> tmp_terrain_list = SFUnPak.SFUnPak.ListAllWithFilename("texture", "landscape_island_", new string[] { "sf1.pak" });
            tmp_terrain_list.Remove("landscape_island_100_mud.dds");
            tmp_terrain_list.Remove("landscape_island_100_mudd.dds");

            texture_filenames = new List<string>();
            texture_filenames.Add("landscape_island_worldd.dds");

            List<string> tmp_helper_list = new List<string>();
            for (int i = 1; i <= TEXTURES_AVAILABLE; i++)
            {
                string s = String.Format("{0,3:000}", i);

                for (int j = 0; j < tmp_terrain_list.Count; j++)
                {
                    if (tmp_terrain_list[j].Contains(s))
                    {
                        tmp_helper_list.Add(tmp_terrain_list[j]);
                    }
                }
                if (tmp_helper_list.Count == 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.LoadTextureNames(): Could not find texture with ID " + i.ToString());
                    continue;
                }

                // add longer of the filenames
                int max_ind = 0;
                int max_len = tmp_helper_list[0].Length;
                for (int j = 1; j < tmp_helper_list.Count; j++)
                {
                    if (tmp_helper_list[j].Length > max_len)
                    {
                        max_ind = j;
                        max_len = tmp_helper_list[j].Length;
                    }
                }

                texture_filenames.Add(tmp_helper_list[max_ind]);

                tmp_helper_list.Clear();
            }
        }

        public void SetTextureIDsRaw(byte[] data)
        {
            for (int i = 0; i < MAX_TEXTURES; i++)
            {
                texture_id[i] = data[i];
            }
        }

        public string GetTextureNameByID(int id)
        {
            if (id > TEXTURES_AVAILABLE)
            {
                id -= TEXTURES_AVAILABLE;
            }
            return texture_filenames[id];
        }

        // loads terrain texture with a given id
        public SFTexture LoadTerrainTexture(int tex_id)
        {
            string filename = GetTextureNameByID(tex_id);

            SFTexture tex = null;
            int tex_code = SFResourceManager.Textures.Load(filename, SFUnPak.FileSource.PAK, new SFTexture.SFTextureLoadArgs() { FreeOnInit = false, IgnoreMipmapSettings = true });
            if ((tex_code != 0) && (tex_code != -1))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFMapTerrainTextureManager.LoadTerrainTexture(): Could not load texture (texture name = " + filename + ")");
                throw new Exception("SFMapTerrainTextureManager.Init(): Can't load texture!");
            }

            tex = SFResourceManager.Textures.Get(filename);
            tex.Uncompress();     // needed for this to work in texture atlas (?)

            byte[] data = tex.data;
            SFResourceManager.Textures.Dispose(filename);
            tex.data = data;

            return tex;
        }

        public void GenerateTileTextures()
        {
            for(int i = 0; i < 32; i++)
            {
                tile_texture_bank[i] = base_texture_bank[i];
                GenerateAverageTileColor(i, base_texture_bank[i]);
            }
            for(int i = 32; i < 224; i++)
            {
                tile_texture_bank[i] = new SFTexture();
                if (tile_defined[i])
                {
                    SFTexture.MixUncompressed(
                    base_texture_bank[texture_tiledata[i].ind1], texture_tiledata[i].weight1,
                    base_texture_bank[texture_tiledata[i].ind2], texture_tiledata[i].weight2,
                    base_texture_bank[texture_tiledata[i].ind3], texture_tiledata[i].weight3,
                    ref tile_texture_bank[i]);
                    GenerateAverageTileColor(i, tile_texture_bank[i]);
                }
            }
            for(int i = 1; i < 32; i++)
            {
                tile_texture_bank[223 + i] = base_texture_bank[i];
                tile_average_color[223 + i] = tile_average_color[i];
            }
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
                {
                    base_texture_bank[i] = LoadTerrainTexture(texture_id[i]);
                }
                else
                {
                    base_texture_bank[i] = LoadTerrainTexture(texture_id[i + 31]);
                }
            }

            // generate inbetween textures
            texture_tile_mixer = new SFTexture();
            tile_defined[0] = true;
            for (int i = 1; i < 32; i++)
            {
                tile_defined[i] = true;
            }

            for (int i = 224; i < 255; i++)
            {
                tile_defined[i] = true;
            }

            for (int i = 32; i < 224; i++)
            {
                tile_defined[i] = ((texture_tiledata[i].ind1 != 0) || (texture_tiledata[i].ind2 != 0) || (texture_tiledata[i].ind3 != 0));
            }

            GenerateTileTextures();

            terrain_texture = GL.GenTexture();
            SF3D.SFRender.SFRenderEngine.SetTexture(0, TextureTarget.Texture2DArray, terrain_texture);
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 8, SizedInternalFormat.Rgba8, 256, 256, MAX_TILES);
            for (int i = 0; i < 32; i++)
            {
                GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, 256, 256, 1, PixelFormat.Rgba, PixelType.UnsignedByte, base_texture_bank[i].data);
            }
            for(int i = 32; i < 224; i++)
            {
                if(tile_defined[i])
                {
                    GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, 256, 256, 1, PixelFormat.Rgba, PixelType.UnsignedByte, tile_texture_bank[i].data);
                }
            }
            for(int i = 1; i < 32; i++)
            {
                GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, 223 + i, 256, 256, 1, PixelFormat.Rgba, PixelType.UnsignedByte, base_texture_bank[i].data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)All.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)All.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)All.Linear);
            if (Settings.AnisotropicFiltering)
            {
                GL.TexParameter(TextureTarget.Texture2DArray, (TextureParameterName)All.TextureMaxAnisotropy, (float)Settings.MaxAnisotropy);
            }

            GenerateTileImages();

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

        public Vector4 VecFromCol(Color c)
        {
            return new Vector4(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, c.A / 255.0f);
        }

        public void UpdateUniformTileColor(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                if (i < 224)
                {
                    uniformTileColor[i] = VecFromCol(tile_average_color[i]);
                }
                else
                {
                    uniformTileColor[i] = VecFromCol(tile_average_color[i - 223]);
                }
            }

            GL.BindBuffer(BufferTarget.UniformBuffer, uniformTileColor_buffer);
            GL.BufferSubData(BufferTarget.UniformBuffer, new IntPtr(4 * 4 * start), 4 * 4 * (end - start) + 4 * 4, ref uniformTileColor[start]);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public bool SetBaseTexture(int base_index, int tex_id)
        {
            // only 1-31 allowed
            if ((base_index <= 0) || (base_index >= MAX_USED_TEXTURES))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.SetBaseTexture(): Invalid base index " + base_index.ToString());

                return false;
            }
            // only 1-119 allowed
            if ((tex_id <= 0) || (tex_id > TEXTURES_AVAILABLE))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.SetBaseTexture(): Invalid terrain texture ID " + tex_id.ToString());

                return false;
            }
            texture_id[base_index] = (byte)tex_id;
            texture_id[base_index + 31] = (byte)(tex_id + TEXTURES_AVAILABLE);
            // load new texture
            base_texture_bank[base_index] = LoadTerrainTexture(tex_id + TEXTURES_AVAILABLE);

            // insert texture data to the atlas
            RefreshBaseTexture(base_index);
            RefreshTilePreview(base_index);
            for (int i = 32; i < 224; i++)
            {
                if ((tile_defined[i]) && ((texture_tiledata[i].ind1 == base_index) || (texture_tiledata[i].ind2 == base_index) || (texture_tiledata[i].ind3 == base_index))) 
                {
                    RefreshTilePreview(i);
                }
            }
            UpdateUniformTileColor(0, MAX_TILES - 1);

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
            {
                RefreshTilePreview(i);
            }
        }

        // generates previews for all existing terrain textures in game
        public void GenerateBaseImages()
        {
            if(!Settings.EditorMode)
            {
                return;
            }

            if (base_images_loaded)
            {
                return;
            }

            for (int i = 1; i <= TEXTURES_AVAILABLE; i++)
            {
                SFTexture tex = LoadTerrainTexture(i + TEXTURES_AVAILABLE);
                texture_base_image[i] = CreateBitmapFromTexture(tex);
            }
            base_images_loaded = true;
        }

        // operates on 64x64 mipmap, hardcoded for now...
        public void GenerateAverageTileColor(int tile_id, SFTexture tex)
        {
            int ignored_level = 2;

            int offset = (tex.width * tex.height * 4) * (ignored_level > 0 ? 0 : 1)
                       + (tex.width * tex.height) * (ignored_level > 1 ? 0 : 1);

            int r = 0, g = 0, b = 0;

            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    r += tex.data[offset + 4 * (i * 64 + j) + 0];
                    g += tex.data[offset + 4 * (i * 64 + j) + 1];
                    b += tex.data[offset + 4 * (i * 64 + j) + 2];
                }
            }

            r /= 4096;
            g /= 4096;
            b /= 4096;

            r = Math.Min(255, (int)(r * 1.6f));
            g = Math.Min(255, (int)(g * 1.6f));
            b = Math.Min(255, (int)(b * 1.6f));

            tile_average_color[tile_id] = Color.FromArgb(r, g, b);
        }

        public void RefreshBaseTexture(int base_index)
        {
            SF3D.SFRender.SFRenderEngine.SetTexture(0, TextureTarget.Texture2DArray, terrain_texture);

            tile_texture_bank[base_index] = base_texture_bank[base_index];
            GenerateAverageTileColor(base_index, base_texture_bank[base_index]);
            GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, base_index, 256, 256, 1, PixelFormat.Rgba, PixelType.UnsignedByte, tile_texture_bank[base_index].data);

            for (int i = 32; i < 224; i++)
            {
                if(tile_defined[i])
                {
                    if((texture_tiledata[i].ind1 == base_index)|| (texture_tiledata[i].ind1 == base_index)|| (texture_tiledata[i].ind1 == base_index))
                    {
                        SFTexture.MixUncompressed(
                        base_texture_bank[texture_tiledata[i].ind1], texture_tiledata[i].weight1,
                        base_texture_bank[texture_tiledata[i].ind2], texture_tiledata[i].weight2,
                        base_texture_bank[texture_tiledata[i].ind3], texture_tiledata[i].weight3,
                        ref tile_texture_bank[i]);
                        GenerateAverageTileColor(i, tile_texture_bank[i]);
                        GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, 256, 256, 1, PixelFormat.Rgba, PixelType.UnsignedByte, tile_texture_bank[i].data);
                    }
                }
            }

            tile_texture_bank[223 + base_index] = tile_texture_bank[base_index];
            tile_average_color[223 + base_index] = tile_average_color[base_index];
            GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, 223 + base_index, 256, 256, 1, PixelFormat.Rgba, PixelType.UnsignedByte, tile_texture_bank[223 + base_index].data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
        }

        public void RefreshTileTexture(int tile_id)
        {
            if (tile_id < 32)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.RefreshTileTexture: Invalid tile ID " + tile_id.ToString());
                throw new Exception("SFMapTerrainTextureManager.RefreshTileTexture: Invalid tile ID " + tile_id.ToString());
            }

            SF3D.SFRender.SFRenderEngine.SetTexture(0, TextureTarget.Texture2DArray, terrain_texture);
            if (tile_defined[tile_id])
            {
                SFTexture.MixUncompressed(
                base_texture_bank[texture_tiledata[tile_id].ind1], texture_tiledata[tile_id].weight1,
                base_texture_bank[texture_tiledata[tile_id].ind2], texture_tiledata[tile_id].weight2,
                base_texture_bank[texture_tiledata[tile_id].ind3], texture_tiledata[tile_id].weight3,
                ref tile_texture_bank[tile_id]);
                GenerateAverageTileColor(tile_id, tile_texture_bank[tile_id]);
                GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, tile_id, 256, 256, 1, PixelFormat.Rgba, PixelType.UnsignedByte, tile_texture_bank[tile_id].data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
        }

        public void RefreshTilePreview(int tile_id)
        {
            if (!Settings.EditorMode)
            {
                return;
            }

            if (tile_id < 0)
            {
                return;
            }

            if (tile_id < 32)
            {
                texture_tile_image[tile_id] = CreateBitmapFromTexture(base_texture_bank[tile_id]);
            }
            else if (tile_defined[tile_id])
            {
                texture_tile_image[tile_id] = CreateBitmapFromTexture(tile_texture_bank[tile_id]);
            }
        }

        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapTerrainTextureManager.Unload() called");
            GL.DeleteTexture(terrain_texture);
        }
    }
}
