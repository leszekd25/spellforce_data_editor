using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        const int MAX_TEXTURES = 63;
        const int MAX_REINDEX = 255;
        const int TEXTURES_AVAILABLE = 119;

        public SFTexture[] base_texture_bank { get; private set; } = new SFTexture[MAX_TEXTURES];
        public byte[] texture_id { get; private set; } = new byte[MAX_TEXTURES];
        public SFTexture[] texture_array { get; private set; } = new SFTexture[MAX_REINDEX];
        public SFMapTerrainTextureTileData[] texture_tiledata { get; private set; } = new SFMapTerrainTextureTileData[MAX_REINDEX];
        List<string> texture_filenames;
        public int terrain_texture { get; private set; } = -1;     //texture GL ID


        public void LoadTextureNames()
        {
            if (SFUnPak.SFUnPak.game_directory_name == "")
                return;

            texture_filenames = SFUnPak.SFUnPak.ListAllWithFilename("texture", "landscape_island_", new string[] { "sf1.pak"});
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
            LoadTextureNames();
            // load base textures
            for (int i = 0; i < MAX_TEXTURES; i++)
            {
                string filename = GetTextureNameByID(texture_id[i]);
                base_texture_bank[i] = new SFTexture();

                MemoryStream ms = SFUnPak.SFUnPak.LoadFileFrom("sf1.pak", "texture\\" + filename);
                if (ms == null)
                    throw new Exception("SFMapTerrainTextureManager.Init(): Can't find texture!");
                int res_code = base_texture_bank[i].Load(ms);
                if (res_code != 0)
                    throw new Exception("SFMapTerrainTextureManager.Init(): Can't load texture!");
                ms.Close();

                base_texture_bank[i].Uncompress();
                base_texture_bank[i].Init();
            }
            // generate inbetween textures
            for(int i = 0; i < 32; i++)
            {
                texture_array[i] = base_texture_bank[i];
            }
            for(int i = 224; i < 255; i++)
            {
                texture_array[i] = base_texture_bank[i - 192];
            }
            for(int i = 32; i < 224; i++)
            {
                if ((texture_tiledata[i].ind1 != 0) && (texture_tiledata[i].ind2 != 0))
                {
                    texture_array[i] = SFTexture.MixUncompressed(base_texture_bank[texture_tiledata[i].ind1], texture_tiledata[i].weight1,
                                                                 base_texture_bank[texture_tiledata[i].ind2], texture_tiledata[i].weight2);
                    texture_array[i].Init();
                }
                else
                    texture_array[i] = base_texture_bank[0];
            }

            int mipmap_divisor = 1;
            int min_allowed_level = 0;
            int _size = 256;
            while(_size > SFTexture.MaximumAllowedTextureSize) { min_allowed_level += 1; _size /= 2; }

            min_allowed_level = (min_allowed_level > SFTexture.IgnoredMipMapsCount ? min_allowed_level : SFTexture.IgnoredMipMapsCount);

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
            if ((tex_id <= 0) || (tex_id > 237))
                return false;
            texture_id[base_index] = (byte)tex_id;
            // unload existing texture
            base_texture_bank[base_index].Dispose();
            // load new texture
            string filename = GetTextureNameByID(tex_id);

            MemoryStream ms = SFUnPak.SFUnPak.LoadFileFrom("sf1.pak", "texture\\" + filename);
            if (ms == null)
                throw new Exception("SFMapTerrainTextureManager.SetBaseTexture(): Can't find texture!");
            int res_code = base_texture_bank[base_index].Load(ms);
            if (res_code != 0)
                throw new Exception("SFMapTerrainTextureManager.SetBaseTexture(): Can't load texture!");
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
                    texture_array[i] = SFTexture.MixUncompressed(base_texture_bank[texture_tiledata[i].ind1], texture_tiledata[i].weight1,
                                                                 base_texture_bank[texture_tiledata[i].ind2], texture_tiledata[i].weight2);
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
                base_texture_bank[texture_tiledata[tile_id].ind1], texture_tiledata[tile_id].weight1,
                base_texture_bank[texture_tiledata[tile_id].ind2], texture_tiledata[tile_id].weight2);
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

        public void FreeTileMemory(int tile_id)
        {
            if ((tile_id >= 32) && (tile_id < 224))
                texture_array[tile_id].FreeMemory();
        }

        public void Unload()
        {
            GL.DeleteTexture(terrain_texture);
            for(int i = 0; i < MAX_REINDEX; i++)
            {
                texture_array[i].Dispose();
            }
        }
    }
}
