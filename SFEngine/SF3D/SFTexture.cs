/*
 * SFTexture is a resource which contains texture image data
 * It loads data from provided DDS/TGA file, and feeds it to the GPU
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
using SFEngine.SFResources;

namespace SFEngine.SF3D
{
    public class SFTexture: SFResource
    {
        public byte[] data { get; private set; }
        private int data_length_before_free = 0;                // used for GetSizeBytes
        public int width { get; private set; }
        public int height { get; private set; }
        public int tex_id { get; private set; } = Utility.NO_INDEX;
        public uint mipMapCount { get; private set; }

        public bool ignore_mipmap_level_settings { get; set; } = false;
        public InternalFormat format { get; private set; }
        string name = "";

        public SFTexture()
        {
            data = null;
        }

        public SFTexture(MemoryStream ms)
        {
            Load(ms, null);
            Init();
        }

        // returns whether provided mip map level is valid, considering user settings
        public bool IsValidMipMapLevel(int level)
        {
            if (ignore_mipmap_level_settings)
                return true;

            bool ret = ((level >= Settings.IgnoredMipMapsCount) || (level == mipMapCount - 1) || (mipMapCount <= Settings.IgnoredMipMapsCount));

            int size_skip = 0; int _w = width; int _h = height;
            while((_w > Settings.MaximumAllowedTextureSize)||(_h > Settings.MaximumAllowedTextureSize))
            { size_skip += 1; _w /= 2; _h /= 2; }

            return (ret) && ((size_skip <= level)||(mipMapCount == 1));
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

                        GL.CompressedTexImage2D(TextureTarget.Texture2D, level - min_allowed_level, format, w, h,
                            0, size, ref data[offset]);
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
                    int size = w * h * 4;
                    if (IsValidMipMapLevel(level))
                    {
                        if (min_allowed_level > level) min_allowed_level = level;

                        GL.TexImage2D(TextureTarget.Texture2D, level - min_allowed_level, PixelInternalFormat.Rgba, w, h,
                            0, PixelFormat.Rgba, PixelType.UnsignedByte, ref data[offset]);
                        offset += size;
                    }

                    w /= 2;
                    h /= 2;
                }
            }
            if (mipMapCount == 1)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            if (Settings.AnisotropicFiltering)
                GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)All.TextureMaxAnisotropy, (float)Settings.MaxAnisotropy);
            

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public int Load(MemoryStream ms, object custom_data)
        {
            if (custom_data != null)
                ignore_mipmap_level_settings = (((int)custom_data) == 1);

            BinaryReader br = new BinaryReader(ms);

            if (LoadDDS(br) != 0)
                if (LoadTGA(br) != 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.Load(): Could not deduce texture data type!");
                    return -201;
                }

            return 0;
        }

        public int LoadDDS(BinaryReader br)
        {
            br.BaseStream.Position = 0;

            uint[] header = new uint[31];
            uint filecode = br.ReadUInt32();
            if (filecode != 0x20534444)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadDDS(): Invalid header guard");
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
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadDDS(): Invalid texture format");
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
                for (uint level = 0; level < mipMapCount && (w != 0 || h != 0); ++level)
                {
                    size = ((w + 3) / 4) * ((h + 3) / 4) * blockSize;
                    if (IsValidMipMapLevel((int)level))
                        buf_size += size;
                    else
                    {
                        skip_size += size;
                    }
                    w /= 2;
                    h /= 2;
                }
            }
            else
            {
                mipMapCount = 1;
                buf_size = size;
            }

            br.BaseStream.Position += skip_size;
            data = br.ReadBytes((int)buf_size);
            return 0;
        }

        // http://www.paulbourke.net/dataformats/tga/tgatest.c
        public int LoadTGA(BinaryReader br)   // tga contains no mipmaps
        {
            br.BaseStream.Position = 0;

            byte[] header = br.ReadBytes(18);
            byte id_length = header[0];
            byte is_color_map = header[1];
            byte image_type = header[2];
            ushort cmap_first_index = BitConverter.ToUInt16(header, 3);
            ushort cmap_length = BitConverter.ToUInt16(header, 5);
            byte cmap_bpp = header[7];
            ushort isp_x = BitConverter.ToUInt16(header, 8);
            ushort isp_y = BitConverter.ToUInt16(header, 10);
            ushort isp_w = BitConverter.ToUInt16(header, 12);
            ushort isp_h = BitConverter.ToUInt16(header, 14);
            byte isp_bpp = header[16];
            byte isp_desc = header[17];

            if((image_type != 2)&&(image_type != 10))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadTGA(): Invalid image type");
                return -512;
            }

            if((isp_bpp/8 < 2)&&(isp_bpp/8 > 4))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadTGA(): Invalid number of bits per pixel");
                return -513;
            }

            if(is_color_map > 1)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadTGA(): Unknown color map type");
                return -514;
            }

            br.BaseStream.Position += id_length;
            br.BaseStream.Position += is_color_map * cmap_length;

            // read image
            int readpixels = 0;
            int bytes_per_pixel = isp_bpp / 8;
            byte[] pixels = new byte[isp_w * isp_h * 4];
            byte[] pixel;
            if (image_type == 2)
            {
                while (readpixels < isp_w * isp_h)
                {
                    br.Read(pixels, readpixels * 4, bytes_per_pixel);
                    TGAFixPixel(pixels, readpixels, bytes_per_pixel);
                    readpixels += 1;
                }
            }
            else if (image_type == 10)
            {
                while (readpixels < isp_w * isp_h)
                {
                    byte c_data = br.ReadByte();
                    pixel = br.ReadBytes(bytes_per_pixel);
                    System.Buffer.BlockCopy(pixel, 0, pixels, readpixels * 4, bytes_per_pixel);
                    TGAFixPixel(pixels, readpixels, bytes_per_pixel);
                    readpixels += 1;
                    if (c_data >= 128)
                    {
                        int source_pixel = readpixels - 1;
                        c_data -= 128;
                        for (int i = 0; i < c_data; i++)
                        {
                            System.Buffer.BlockCopy(pixel, source_pixel * 4, pixels, readpixels * 4, 4);
                            readpixels += 1;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < c_data; i++)
                        {
                            pixel = br.ReadBytes(bytes_per_pixel);
                            System.Buffer.BlockCopy(pixel, 0, pixels, readpixels * 4, bytes_per_pixel);
                            TGAFixPixel(pixels, readpixels, bytes_per_pixel);
                            readpixels += 1;
                        }
                    }
                }
            }

            // flip texture Y
            for(int i = 0; i < isp_w; i++)
                for(int j = 0; j < isp_h/2; j++)
                    for (int k = 0; k < 4; k++)
                    {
                        byte tmp_px = pixels[(i + j * isp_w) * 4 + k];
                        pixels[(i + j * isp_w) * 4 + k] = pixels[(i + (isp_h - j - 1) * isp_w) * 4 + k];
                        pixels[(i + (isp_h - j - 1) * isp_w) * 4 + k] = tmp_px;
                    }

            data = pixels;
            width = isp_w; height = isp_h; mipMapCount = 1; format = InternalFormat.Rgba;

            return 0;
        }

        public void TGAFixPixel(byte[] pixels, int pixeloffset, int bpp)
        {
            if(bpp==4)
            {
                byte tmp_r = pixels[pixeloffset * 4+0];
                pixels[pixeloffset * 4 + 0] = pixels[pixeloffset * 4 + 2];
                pixels[pixeloffset * 4 + 2] = tmp_r;
            }
            else if(bpp==3)
            {
                byte tmp_r = pixels[pixeloffset * 4 + 0];
                pixels[pixeloffset * 4 + 0] = pixels[pixeloffset * 4 + 2];
                pixels[pixeloffset * 4 + 2] = tmp_r;
                pixels[pixeloffset * 4 + 3] = 255;
            }
            else if(bpp==2)
            {
                byte b1 = pixels[pixeloffset * 4 + 0];
                byte b2 = pixels[pixeloffset * 4 + 1];
                pixels[pixeloffset * 4 + 0] = (byte)((b2 & 0x7c) << 1);
                pixels[pixeloffset * 4 + 1] = (byte)(((b2 & 0x03) << 6) | ((b1 & 0xe0) >> 2));
                pixels[pixeloffset * 4 + 2] = (byte)((b1 & 0x1f) << 3);
                pixels[pixeloffset * 4 + 3] = (byte)(b1 & 0x80);
            }
        }

        public int LoadUncompressedRGBA(BinaryReader br, ushort w, ushort h, byte mipmaps)
        {
            br.BaseStream.Position = 0;

            ushort _w = w;
            ushort _h = h;

            int expected_size = 0;
            int read_size = 0;

            if (mipmaps == 0)
                mipmaps = 1;

            for (int level = 0; level < mipmaps && (_w != 0 || _h != 0); ++level)
            {
                int size = w*h*4;
                expected_size += size;
                if (IsValidMipMapLevel(level))
                    read_size += size;

                _w /= 2;
                _h /= 2;
            }

            if (br.BaseStream.Length != expected_size)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadUncompressedRGBA(): Data length is not valid");
                return -612;
            }

            data = br.ReadBytes(read_size);
            width = w; height = h; mipMapCount = mipmaps; format = InternalFormat.Rgba;

            return 0;
        }

        static public SFTexture RGBAImage(ushort w, ushort h)
        {
            SFTexture tex = new SFTexture() { data = new byte[w * h * 4], width = w, height = h, mipMapCount = 1, format = InternalFormat.Rgba };
            return tex;
        }

        public void UpdateImage()
        {
            if(data == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.UpdateImage(): Texture data was freed, can not update");
                return;
            }

            GL.BindTexture(TextureTarget.Texture2D, tex_id);

            /* load the mipmaps */
            if (format != InternalFormat.Rgba)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.UpdateImage(): Invalid texture format!");
                return;
            }
            if(mipMapCount != 1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.UpdateImage(): Invalid mipmap count!");
                return;
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height,
                        0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);
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
            tex_id = Utility.NO_INDEX;

            data = pixels;

            format = InternalFormat.Rgba;
        }

        public byte GetUncompressedAlpha(int x, int y)
        {
            return data[(y * width + x) * 4 + 3];
        }

        public void custom_WhiteToAlpha()
        {
            if(format != InternalFormat.Rgba)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.custom_WhiteToAlpha(): Format is not RGBA!");
                return;
            }

            for(int i = 0; i < data.Length; i += 4)
            {
                data[i + 3] = (byte)(255 - data[i + 0]);
                data[i + 0] = 0;
                data[i + 1] = 0;
                data[i + 2] = 0;
            }
        }

        public enum SFTextureToBitmapArgType { DIMENSION = 0, MIPMAP = 1 }    // mipmap not supported for now...

        public struct SFTextureToBitmapArgs
        {
            // DIMENSION => look for mipmap with this dimension and get pixels from those, MIPMAP => get pixels from given mipmap
            public SFTextureToBitmapArgType ConversionType;
            public int DimWidth;
            public int DimHeight;
            public int MipmapLevel;
        }

        // create a bitmap from texture
        // only works for uncompressed textures for now
        public System.Drawing.Bitmap ToBitmap(SFTextureToBitmapArgs args)
        {
            System.Drawing.Bitmap b = null;

            if (format != InternalFormat.Rgba)
                return b;

            int cur_w = width;
            int cur_h = height;
            int cur_mip = 0;

            if (args.ConversionType == SFTextureToBitmapArgType.DIMENSION)
            {
                int default_width = width;
                int default_height = height;
                for (uint level = 0; level < mipMapCount && (cur_w != 0 || cur_h != 0); ++level)
                {
                    if (IsValidMipMapLevel((int)level))
                        break;

                    default_width /= 2;
                    default_height /= 2;
                }

                while ((cur_h != 0)&&(cur_w != 0))
                {
                    if ((cur_h == args.DimHeight) && (cur_w == args.DimWidth))
                        break;

                    cur_h /= 2;
                    cur_w /= 2;
                    cur_mip += 1;
                }

                if((cur_h == 0)||(cur_w == 0))
                        return b;

                int skip = 1 << (cur_mip);

                b = new System.Drawing.Bitmap(cur_w, cur_h);
                for (int j = 0; j < default_height; j += skip)
                    for (int i = 0; i < default_width; i+=skip)
                        b.SetPixel(i/skip, cur_h - 1 - j/skip, System.Drawing.Color.FromArgb(
                            255,
                            data[4 * (j * default_width + i) + 0],
                            data[4 * (j * default_width + i) + 1],
                            data[4 * (j * default_width + i) + 2]));
                return b;
            }
            else
                return b;
        }

        // texture format: bmp
        public void Export(string fname)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFTexture.Export() called, filename: " + fname + ".bmp");

            if(format != InternalFormat.Rgba)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.Export(): Invalid internal format for texture export!");
                return;
            }
            if(data == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.Export(): Data was freed before export!");
                return;
            }

            FileStream fs = new FileStream(fname + ".bmp", FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write((byte)0x42);
            bw.Write((byte)0x4D);
            bw.Write((uint)(54 + width * height * 3));
            bw.Write((uint)0);
            bw.Write((uint)54);
            bw.Write((uint)40);
            bw.Write((uint)width);
            bw.Write((uint)height);
            bw.Write((ushort)1);
            bw.Write((ushort)24);
            bw.Write((uint)0);
            bw.Write((uint)0);
            bw.Write((uint)0);
            bw.Write((uint)0);
            bw.Write((uint)0);
            bw.Write((uint)0);

            for(int i = 0; i < width; i++)
                for(int j = 0; j < height; j++)
                {
                    bw.Write(data[(i * height + j) * 4 + 2]);
                    bw.Write(data[(i * height + j) * 4 + 1]); 
                    bw.Write(data[(i * height + j) * 4 + 0]);
                }

            bw.Close();
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
            if(data != null)
                data_length_before_free = data.Length;
            data = null;
        }

        public void Dispose()
        {
            if (tex_id != Utility.NO_INDEX)
            {                
                GL.DeleteTexture(tex_id);
                FreeMemory();
                tex_id = Utility.NO_INDEX;
            }
        }

        new public string ToString()
        {
            return GetName();
        }

        public int GetSizeBytes()
        {
            if (data == null)
                return data_length_before_free;
            return data.Length;
        }

        public static SFTexture MixUncompressed(SFTexture tex1, byte w1, SFTexture tex2, byte w2)
        {
            if ((tex1.width != tex2.width) || (tex1.height != tex2.height))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.MixUncompressed(): Texture dimensions do not match!");
                throw new Exception("SFTexture.MixUncompressed(): Texture dimensions do not match!");
            }
            if ((tex1.format != InternalFormat.Rgba) || (tex2.format != InternalFormat.Rgba))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.MixUncompressed(): Texture(s) are not uncompressed!");
                throw new Exception("SFTexture.MixUncompressed(): Texture(s) are not uncompressed!");
            }
            if (w1 + w2 == 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.MixUncompressed(): Texture weights are both 0! Using weight 127 for both weights");
                w1 = 127; w2 = 127;
            }

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

        // used only for terrain texture preview for now...
        public static void MixUncompressed(SFTexture tex1, byte w1, SFTexture tex2, byte w2, SFTexture tex3, byte w3, ref SFTexture new_tex)
        {
            if ((tex1.width != tex2.width) || (tex1.height != tex2.height))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.MixUncompressed(): Texture1 and texture2 dimensions do not match!");
                throw new Exception("SFTexture.MixUncompressed(): Texture1 and texture2 dimensions do not match!");
            }
            if ((tex1.width != tex3.width) || (tex1.height != tex3.height))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.MixUncompressed(): Texture1 and texture3 dimensions do not match!");
                throw new Exception("SFTexture.MixUncompressed(): Texture1 and texture3 dimensions do not match!");
            }

            if ((tex1.format != InternalFormat.Rgba) || (tex2.format != InternalFormat.Rgba) || (tex3.format != InternalFormat.Rgba))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.MixUncompressed(): Texture(s) are not uncompressed!");
                throw new Exception("SFTexture.MixUncompressed(): Texture(s) are not uncompressed!");
            }
            if (w1 + w2 + w3 == 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.MixUncompressed(): Texture weights are both 0! Using weight 85 for all weights");
                w1 = 85; w2 = 85; w3 = 85;
            }
            
            new_tex.width = tex1.width;
            new_tex.height = tex1.height;
            new_tex.mipMapCount = tex1.mipMapCount;
            new_tex.format = tex1.format;
            if(new_tex.data == null)
                new_tex.data = new byte[tex1.data.Length];
            for (int i = 0; i < tex1.data.Length; i++)
            {
                new_tex.data[i] = (byte)((w1 * tex1.data[i] + w2 * tex2.data[i] + w3 * tex3.data[i]) / 255);
            }
        }
    }
}
