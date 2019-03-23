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
    public struct SFMapTerrainTexture
    {
        public int id;
        public string filename;
        public SFTexture tex;

        public void LoadTexture()
        {
            tex = new SFTexture();

            //System.Diagnostics.Debug.WriteLine("TEXTURE " + filename + "...");
            MemoryStream ms = SFUnPak.SFUnPak.LoadFileFrom("sf1.pak", "texture\\" + filename);
            if (ms == null)
            {
                tex = null;
                return;
            }
            //resource loading stack
            int res_code = tex.Load(ms);
            if(res_code != 0)
            {
                tex = null;
                return;
            }
            //System.Diagnostics.Debug.WriteLine(tex.ToString());
            ms.Close();
        }
    }

    public struct SFMapTerrainTextureReindexer
    {
        public int index;
        public int real_index;
    }

    public class SFMapTerrainTextureManager
    {
        const int MAX_TEXTURES = 63;
        const int MAX_REINDEX = 255;
        const int TEXTURES_AVAILABLE = 119;

        public SFMapTerrainTexture[] texture_array { get; private set; } = new SFMapTerrainTexture[MAX_TEXTURES];
        public SFMapTerrainTextureReindexer[] texture_reindex { get; private set; } = new SFMapTerrainTextureReindexer[MAX_REINDEX];
        List<string> texture_filenames;
        public int terrain_texture { get; private set; } = -1;     //texture GL ID


        public void LoadTextureNames()
        {
            if (SFUnPak.SFUnPak.game_directory_name == "")
                return;

            texture_filenames = SFUnPak.SFUnPak.ListAllWithFilename("texture", "landscape_island_", new string[] { "sf1.pak"});
        }

        public void SetTextureReindexRaw(byte[] data)
        {
            // set first reindex to zeros
            texture_reindex[0].index = 0;
            texture_reindex[0].real_index = 0;
            for(int i = 1; i < MAX_REINDEX; i++)
            {
                texture_reindex[i].real_index = data[i * 14 + 0];
                texture_reindex[i].index = i;
            }
        }

        public void SetTextureIDsRaw(byte[] data)
        {
            for (int i = 0; i < MAX_TEXTURES; i++)
                texture_array[i].id = data[i];
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
            for (int i = 0; i < MAX_TEXTURES; i++)
            {
                texture_array[i].filename = GetTextureNameByID(texture_array[i].id);
                texture_array[i].LoadTexture();
                texture_array[i].tex.Uncompress();
            }

            // todo: add mipmaps : )
            terrain_texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, terrain_texture);
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 8, SizedInternalFormat.Rgba8, 256, 256, 63);
            for (int i = 0; i < 63; i++)
            {
                int offset = 0;

                int w = 256;
                int h = 256;

                for (int level = 0; level < 8 && (w != 0 || h != 0); ++level)
                {
                    int size = ((w + 3) / 4) * ((h + 3) / 4) * 64;

                    GL.TexSubImage3D(TextureTarget.Texture2DArray, level, 0, 0, i, w, h, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref texture_array[i].tex.data[offset]);

                    offset += size;
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
        }

        public void Unload()
        {
            GL.DeleteTexture(terrain_texture);
        }
    }
}
