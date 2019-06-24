/*
 * SFTexture is a resource which contains texture image data
 * It loads data from provided DDS file, and feeds it to the GPU
 * It has to be disposed upon removal, to remove any data from the GPU
 * */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SpellforceDataEditor.SFResources;

namespace SpellforceDataEditor.SF3D
{
    public class SFTexture: SFResource
    {
        static public int IgnoredMipMapsCount = 2;
        static public int MaximumAllowedTextureSize = 256;

        public byte[] data { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }
        public int tex_id { get; private set; } = -1;
        public uint mipMapCount { get; private set; }
        public InternalFormat format { get; private set; }
        string name = "";

        public SFTexture()
        {
            data = null;
        }

        public SFTexture(MemoryStream ms)
        {
            Load(ms);
            Init();
        }

        public bool IsValidMipMapLevel(int level)
        {
            bool ret = ((level >= IgnoredMipMapsCount) || (level == mipMapCount - 1));
            int size_skip = 0; int _w = width; int _h = height;
            while((_w > MaximumAllowedTextureSize)||(_h > MaximumAllowedTextureSize)) { size_skip += 1; _w /= 2; _h /= 2; }
            return (ret) && (size_skip <= level);
        }

        public void Init()
        {
            tex_id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex_id);

            int blockSize = (format == InternalFormat.CompressedRgbaS3tcDxt1Ext) ? 8 : 16;

            int offset = 0;
            int w = width;
            int h = height;
            int min_allowed_level = 1000;


            /* load the mipmaps */
            if (format != InternalFormat.Rgba)
            {
                for (int level = 0; level < mipMapCount && (w != 0 || h != 0); ++level)
                {
                    int size = ((w + 3) / 4) * ((h + 3) / 4) * blockSize;
                    if (IsValidMipMapLevel(level))
                    {
                        if (min_allowed_level > level) min_allowed_level = level;

                        byte[] mipMapData = data.Skip(offset).Take(size).ToArray();
                        GL.CompressedTexImage2D(TextureTarget.Texture2D, level - min_allowed_level, format, w, h,
                            0, size, mipMapData);
                        offset += size;
                    }

                    w /= 2;
                    h /= 2;
                }
            }
            else
            {
                for (int level =  0; level < mipMapCount && (w != 0 || h != 0); ++level)
                {
                    int size = ((w + 3) / 4) * ((h + 3) / 4) * 64; if (IsValidMipMapLevel(level))
                    {
                        if (min_allowed_level > level) min_allowed_level = level;

                        byte[] mipMapData = data.Skip(offset).Take(size).ToArray();
                        GL.TexImage2D(TextureTarget.Texture2D, level - min_allowed_level, PixelInternalFormat.Rgba, w, h,
                            0, PixelFormat.Rgba, PixelType.UnsignedByte, mipMapData);
                        offset += size;
                    }

                    w /= 2;
                    h /= 2;
                }
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public int Load(MemoryStream ms)
        {
            BinaryReader br = new BinaryReader(ms);

            uint[] header = new uint[31];
            uint filecode = br.ReadUInt32();
            if(filecode != 0x20534444)
            {
                br.Close();
                return -502;
            }

            for (int i = 0; i < 31; i++)
                header[i] = br.ReadUInt32();

            width = (int)header[2];
            height = (int)header[3];
            uint linearSize;
            uint blockSize;
            mipMapCount = header[6];
            uint fourCC = header[20];

            uint components = (fourCC == 0x31545844) ? 3u : 4u;

            switch (fourCC)
            {
                case 0x31545844:
                    format = InternalFormat.CompressedRgbaS3tcDxt1Ext;
                    blockSize = 8;
                    break;
                case 0x33545844:
                    format = InternalFormat.CompressedRgbaS3tcDxt3Ext;
                    blockSize = 16;
                    break;
                case 0x35545844:
                    format = InternalFormat.CompressedRgbaS3tcDxt5Ext;
                    blockSize = 16;
                    break;
                default:
                    br.Close();
                    return -503;  //wrong fourcc
            }
            linearSize = (((uint)width + 3) / 4) * blockSize;
            if (linearSize < 1)
                linearSize = 1;
            uint size = (((uint)width + 3) / 4) * (((uint)height + 3) / 4) * blockSize;
            uint skip_size = 0;

            uint buf_size;
            if (mipMapCount > 1)
            {
                uint w, h;
                w = (uint)width;
                h = (uint)height;
                buf_size = 0;
                for (uint level = 0; level < mipMapCount && (w!=0 || h!=0); ++level)
                {
                    size = ((w + 3) / 4) * ((h + 3) / 4) * blockSize;
                    if (IsValidMipMapLevel((int)level))
                        buf_size += size;
                    else
                        skip_size += size;
                    w /= 2;
                    h /= 2;
                }
            }
            else
                buf_size = size;

            data = br.ReadBytes((int)(buf_size+skip_size)).Skip((int)skip_size).ToArray();
            return 0;
        }

        // uncompress utility (used in heightmap array texture to have all textures use the same uncompressed format
        public void Uncompress()
        {
            if (format == InternalFormat.Rgba)
                return;

            // 1. Init
            Init();

            // 2. Get image
            GL.BindTexture(TextureTarget.Texture2D, tex_id);
            int blockSize = (format == InternalFormat.CompressedRgbaS3tcDxt1Ext) ? 8 : 16;
            byte[] pixels = new byte[data.Length * 64 / blockSize];
            int offset = 0;
            int w = width;
            int h = height;
            int min_allowed_level = 1000;
            for (int level = 0; level < mipMapCount && (w != 0 || h != 0); ++level)
            {
                int size = ((w + 3) / 4) * ((h + 3) / 4) * 64;
                if (IsValidMipMapLevel((int)level))
                {
                    if (min_allowed_level > level) min_allowed_level = level;

                    GL.GetTexImage(TextureTarget.Texture2D, level - min_allowed_level, PixelFormat.Rgba, PixelType.UnsignedByte, ref pixels[offset]);

                    offset += size;
                }
                w /= 2;
                h /= 2;
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // 3. Deinit
            GL.DeleteTexture(tex_id);
            tex_id = -1;
            data = pixels;
            format = InternalFormat.Rgba;
        }

        public void SetName(string s)
        {
            name = s;
        }

        public string GetName()
        {
            return name;
        }

        // used after loading textures that are no longer needed in memory, i.e, won't be reloaded anymore
        public void FreeMemory()
        {
            data = null;
        }

        public void Dispose()
        {
            if (tex_id != -1)
            {
                GL.DeleteTexture(tex_id);
                tex_id = -1;
            }
        }

        new public string ToString()
        {
            return "TEX SIZE#"+width.ToString()+" "+height.ToString()
                + "\nTEX FORMAT#" + format.ToString()
                +"\nTEX MIPMAPS#" + mipMapCount.ToString();
        }

        public static SFTexture MixUncompressed(SFTexture tex1, byte w1, SFTexture tex2, byte w2)
        {
            if ((tex1.width != tex2.width) || (tex1.height != tex2.height))
                throw new Exception("SFTexture.MixUncompressed(): Texture dimensions do not match!");
            if ((tex1.format != InternalFormat.Rgba) || (tex2.format != InternalFormat.Rgba))
                throw new Exception("SFTexture.MixUncompressed(): Texture(s) are not uncompressed!");
            if (w1 + w2 == 0)
                throw new Exception("SFTexture.MixUncompressed(): Texture weights invalid!");

            SFTexture new_tex = new SFTexture();
            new_tex.width = tex1.width;
            new_tex.height = tex1.height;
            new_tex.mipMapCount = tex1.mipMapCount;
            new_tex.format = tex1.format;
            new_tex.data = new byte[tex1.data.Length];
            for(int i = 0; i < tex1.data.Length; i++)
            {
                new_tex.data[i] = (byte)((w1 * tex1.data[i] + w2 * tex2.data[i])/255);
            }

            return new_tex;
        }
    }
}
