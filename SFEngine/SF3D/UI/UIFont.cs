/* UIFont is a wrapper around a texture which extracts characters as quad data, used in UI manager
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFEngine.SFResources;

using OpenTK;

namespace SFEngine.SF3D.UI
{
    public class UIFont
    {
        public SFTexture font_texture { get; set; } = null;
        public Vector2[] character_uvs_start { get; private set; }
        public Vector2[] character_uvs_end { get; private set; }
        public Vector2[] character_vertex_sizes { get; private set; }
        public Vector2[] character_offsets { get; private set; }
        public Vector2[] character_sizes { get; private set; }
        public int space_between_letters = 0;

        public int Load(string fname)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "UIFont.Load() called, font texture name: " + fname);
            // 1. load font texture
            string tex_name = fname;
            int tex_code = SFResourceManager.Textures.Load(tex_name);
            if ((tex_code != 0) && (tex_code != -1))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "UIFont.Load(): Could not load texture (texture name = " + tex_name + ")");
                return tex_code;
            }
            font_texture = SFResourceManager.Textures.Get(tex_name);

            // 2. characters in the font go from ascii codes 32-255, but character_uvs contain all characters from 0 to 255
            character_uvs_start = new Vector2[256];
            character_uvs_end = new Vector2[256];
            character_vertex_sizes = new Vector2[256];
            character_sizes = new Vector2[256];
            character_offsets = new Vector2[256];

            // 2.1. find texture cell size based on texture size
            int cell_size = font_texture.width / 16;    // each font character sits entirely in a cell with bounds [0, 0, cell_size, cell_size]

            // 2.2. find baseline for text heights
            int base_y = -1;
            for(int y = 0; y < cell_size; y++)
            {
                if(font_texture.GetUncompressedAlpha(font_texture.width-cell_size, font_texture.height-cell_size+y) != 0)
                {
                    base_y = y;
                    break;
                }
            }

            // 3. calculate quad data for each character
            for(int i = 32; i < 256; i++)
            {
                int c_x = (i-32) % 16;
                int c_y = (i-32) / 16;

                // 3.1. find character width in relation to cell
                int c_wstart = -1;
                int c_wend = cell_size-1;
                for(int x = 0; x < cell_size; x++)
                {
                    var alpha = font_texture.GetUncompressedAlpha(x + c_x * cell_size, c_y * cell_size + cell_size - 1);
                    if (c_wstart == -1)
                    {
                        if (alpha != 0)
                            c_wstart = x;
                    }
                    else if(alpha == 0)
                    {
                        c_wend = x-1;
                        break;
                    }
                }
                if (c_wstart == -1)        // empty character -> ignore
                    continue;

                // 3.2. find character extent in Y axis
                int c_rhstart = cell_size - 2;
                int c_rhend = 0;
                for(int x = c_wstart; x <= c_wend; x++)
                {
                    for (int y = 0; y <= c_rhstart; y++)
                    {
                        if (font_texture.GetUncompressedAlpha(x + c_x * cell_size, y + c_y * cell_size) != 0)
                        {
                            c_rhstart = Math.Min(c_rhstart, y);
                            break;
                        }
                    }

                    for (int y = cell_size-2; y >= c_rhend; y--)
                    {
                        if (font_texture.GetUncompressedAlpha(x + c_x * cell_size, y + c_y * cell_size) != 0)
                        {
                            c_rhend = Math.Max(c_rhend, y);
                            break;
                        }
                    }
                }

                // 3.3. find character extent in X axis
                int c_rwstart = c_wstart;
                int c_rwend = c_wend;
                for(int y = c_rhstart; y <= c_rhend; y++)
                {
                    for(int x = 0; x <= c_rwstart; x++)
                    {
                        if (font_texture.GetUncompressedAlpha(x + c_x * cell_size, y + c_y * cell_size) != 0)
                        {
                            c_rwstart = Math.Min(c_rwstart, x);
                            break;
                        }
                    }

                    for(int x = cell_size - 1; x >= c_rwend; x--)
                    {
                        if (font_texture.GetUncompressedAlpha(x + c_x * cell_size, y + c_y * cell_size) != 0)
                        {
                            c_rwend = Math.Max(c_rwend, x);
                            break;
                        }
                    }
                }

                // 3.5. we have character bounds and character code width, now we can start generating quad data
                character_sizes[i] = new Vector2(c_wend - c_wstart + 1, c_rhend - c_rhstart + 1);
                character_vertex_sizes[i] = new Vector2(c_rwend - c_rwstart + 1, c_rhend - c_rhstart + 1);
                character_offsets[i] = new Vector2(c_wstart - c_rwstart, -c_rhstart + base_y);
                character_uvs_start[i] = new Vector2((c_x * cell_size + c_rwstart)/(float)font_texture.width, (c_y * cell_size + c_rhstart)/(float)font_texture.height);
                character_uvs_end[i] = new Vector2((c_x * cell_size + c_rwend + 1) / (float)font_texture.width, (c_y * cell_size + c_rhend + 1) / (float)font_texture.height);
            }

            font_texture.FreeMemory();

            return 0;
        }

        public void Dispose()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "UIFont.Dispose() called, font texture: " + font_texture == null ? Utility.S_MISSING : font_texture.GetName());
            if (font_texture != null)
                SFResourceManager.Textures.Dispose(font_texture.GetName());
        }
    }
}
