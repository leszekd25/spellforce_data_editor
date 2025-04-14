/*
 * SFTexture is a resource which contains texture image data
 * It loads data from provided DDS/TGA file, and feeds it to the GPU
 * It has to be disposed upon removal, to remove any data from the GPU
 * */

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SFEngine.SFResources;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;

namespace SFEngine.SF3D
{
    public class SFTexture : SFResource
    {
        static Dictionary<SizedInternalFormat, int> InternalFormatSizeBits = new Dictionary<SizedInternalFormat, int>();

        public byte[] data = null;
        public int data_offset = 0;
        public int width;
        public int height;
        public int depth;
        public int tex_id = Utility.NO_INDEX;
        public uint mipMapStart;
        public uint mipMapCount;
        public uint sampleCount;

        public TextureTarget texture_target;
        public SizedInternalFormat internal_format;
        public PixelFormat pixel_format;
        public PixelType pixel_type;
        public int min_filter;
        public int mag_filter;
        public int wrap_s;
        public int wrap_t;
        public Vector4 wrap_border_col;
        public int anisotropy;

        public bool generate_mipmap = false;                  // for minimap
        public bool free_on_init = true;                      // for textures that will be updated (like heightmap texture)
        public bool ignore_mipmap_settings_on_load = false;   // for terrain texture and minimap icons

        static SFTexture()
        {
            InternalFormatSizeBits.Add(SizedInternalFormat.CompressedRgbaS3tcDxt1Ext, 4);
            InternalFormatSizeBits.Add(SizedInternalFormat.CompressedRgbaS3tcDxt3Ext, 8);
            InternalFormatSizeBits.Add(SizedInternalFormat.CompressedRgbaS3tcDxt5Ext, 8);
            InternalFormatSizeBits.Add(SizedInternalFormat.Depth24Stencil8, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.Depth32fStencil8, 40);
            InternalFormatSizeBits.Add(SizedInternalFormat.DepthComponent16, 16);
            InternalFormatSizeBits.Add(SizedInternalFormat.DepthComponent32f, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.DepthComponent32Arb, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.R16, 16);
            InternalFormatSizeBits.Add(SizedInternalFormat.R16f, 16);
            InternalFormatSizeBits.Add(SizedInternalFormat.R16ui, 16);
            InternalFormatSizeBits.Add(SizedInternalFormat.R16i, 16);
            InternalFormatSizeBits.Add(SizedInternalFormat.R32f, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.R32i, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.R8, 8);
            InternalFormatSizeBits.Add(SizedInternalFormat.R8i, 8);
            InternalFormatSizeBits.Add(SizedInternalFormat.R8ui, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rg16, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rg16f, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rg16i, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rg16ui, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rg32i, 64);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rg32ui, 64);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rg32f, 64);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb8, 24);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb8i, 24);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb8ui, 24);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb8Snorm, 24);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb16, 48);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb16i, 48);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb16ui, 48);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb16f, 48);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb16Snorm, 48);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb32i, 96);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgb32ui, 96);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba8, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba8i, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba8ui, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba8Snorm, 32);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba16, 64);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba16i, 64);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba16ui, 64);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba16f, 64);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba32f, 128);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba32i, 128);
            InternalFormatSizeBits.Add(SizedInternalFormat.Rgba32ui, 128);
            InternalFormatSizeBits.Add(SizedInternalFormat.Srgb8, 32);
        }

        public SFTexture()
        {

        }

        public void CalculateMipmapLevel(int start_w, int start_h)
        {
            if(ignore_mipmap_settings_on_load)
            {
                return;
            }

            int w = start_w;
            int h = start_h;

            int skip1 = 0;
            for(; skip1 < Settings.IgnoredMipMapsCount; skip1++)
            {
                if((start_w == 0)||(start_h == 0))
                {
                    break;
                }

                start_w /= 2;
                start_h /= 2;
            }

            int skip2 = 0;
            while(true)
            {
                if ((w == 0) || (h == 0))
                {
                    break;
                }
                if ((w <= Settings.MaximumAllowedTextureSize) && (h <= Settings.MaximumAllowedTextureSize))
                {
                    break;
                }

                skip2++;
                w /= 2;
                h /= 2;
            }

            if(skip1 < skip2)
            {
                skip1 = skip2;
                start_w = w;
                start_h = h;
            }

            mipMapStart = (uint)skip1;
            mipMapCount = 0;
            while(true)
            {
                if ((start_w == 0) || (start_h == 0))
                {
                    if(mipMapCount == 0)
                    {
                        if(mipMapStart == 0)
                        {
                            throw new Exception("SFTexture.CalculateMipMapSettings: Texture size is 0");
                        }
                        mipMapStart -= 1;
                        mipMapCount = 1;
                    }
                    break;
                }
                mipMapCount += 1;

                start_w /= 2;
                start_h /= 2;
            }
        }

        public override void Init()
        {
            DeviceSize = 0;
            tex_id = GL.CreateTexture(texture_target);

            int blockSize = (internal_format == SizedInternalFormat.CompressedRgbaS3tcDxt1Ext) ? 8 : 16;

            int offset = 0;
            int w = width;
            int h = height;
            int size;

            // multisampled textures are handled separately
            if (sampleCount > 1)
            {
                GL.TextureStorage2DMultisample(tex_id, (int)sampleCount, internal_format, width, height, true);
            }
            else
            {
                // texture arrays are handled separately
                if (texture_target == TextureTarget.Texture2dArray)
                {
                    if(mipMapStart != 0)
                    {
                        LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.Init(): Mip starts at " + mipMapStart.ToString() + ", which is invalid for 2D texture array");
                        throw new Exception("SFTexture.Init(): Mip starts at " + mipMapStart.ToString() + ", which is invalid for 2D texture array");
                    }
                    GL.TextureStorage3D(tex_id, (int)mipMapCount, (SizedInternalFormat)internal_format, width, height, depth);
                    for(int level = 0; level < mipMapCount; ++level)
                    {
                        size = w * h * InternalFormatSizeBits[internal_format] / 8;
                        DeviceSize += size;

                        w /= 2;
                        h /= 2;
                    }
                }
                else
                {
                    for (int level = 0; level < mipMapStart; level++)
                    {
                        w /= 2;
                        h /= 2;
                    }
                    GL.TextureStorage2D(tex_id, (int)mipMapCount, internal_format, w, h);
                    for (int level = 0; level < mipMapCount; ++level)
                    {
                        // compressed textures (DXT1/3/5) are handled separately)
                        if ((internal_format == SizedInternalFormat.CompressedRgbaS3tcDxt1Ext) 
                            || (internal_format == SizedInternalFormat.CompressedRgbaS3tcDxt3Ext) 
                            || (internal_format == SizedInternalFormat.CompressedRgbaS3tcDxt5Ext))
                        {
                            size = ((w + 3) / 4) * ((h + 3) / 4) * blockSize;
                            GL.CompressedTextureSubImage2D(tex_id, level, 0, 0, w, h, (InternalFormat)internal_format, size, in data[data_offset + offset]);
                        }
                        // all other textures are handled separately
                        else
                        {
                            size = w * h * InternalFormatSizeBits[internal_format] / 8;
                            if ((data != null) && (data.Length >= data_offset + offset + size))
                            {
                                GL.TextureSubImage2D(tex_id, level, 0, 0, w, h, pixel_format, pixel_type, in data[data_offset + offset]);
                            }
                        }
                        DeviceSize += size;
                        offset += size;

                        w /= 2;
                        h /= 2;
                    }

                    if (generate_mipmap)
                    {
                        GL.GenerateTextureMipmap(tex_id);
                    }
                }


                GL.TextureParameteri(tex_id, TextureParameterName.TextureMinFilter, min_filter);//(generate_mipmap || (mipMapCount > 1)) ? (int)All.LinearMipmapLinear : (int)All.Linear);
                GL.TextureParameteri(tex_id, TextureParameterName.TextureMagFilter, mag_filter);//(int)All.Linear);
                GL.TextureParameteri(tex_id, TextureParameterName.TextureWrapS, wrap_s);//(int)All.ClampToEdge);
                GL.TextureParameteri(tex_id, TextureParameterName.TextureWrapT, wrap_t);//(int)All.ClampToEdge);
                if (wrap_s == (int)All.ClampToBorder)
                {
                    float[] col = [wrap_border_col.X, wrap_border_col.Y, wrap_border_col.Z, wrap_border_col.W];
                    unsafe
                    {
                        fixed(float* ptr = col)
                        {
                            GL.TextureParameterfv(tex_id, TextureParameterName.TextureBorderColor, ptr);
                        }
                    }
                }
                if (Settings.AnisotropicFiltering)
                {
                    GL.TextureParameterf(tex_id, (TextureParameterName)All.TextureMaxAnisotropy, Math.Max(1, Math.Min((float)Settings.MaxAnisotropy, anisotropy)));
                }
            }

            if(free_on_init)
            {
                FreeMemory();
            }
        }

        public void SetWrapMode(int mode)
        {
            GL.TextureParameteri(tex_id, TextureParameterName.TextureWrapS, mode);
            GL.TextureParameteri(tex_id, TextureParameterName.TextureWrapT, mode);
        }

        public struct SFTextureLoadArgs
        {
            public bool FreeOnInit;
            public bool IgnoreMipmapSettings;
        }

        public override int Load(byte[] data, int offset, object custom_data)
        {
            /*
            MemoryStream ms = new MemoryStream(data, offset, data.Length - offset);
            BinaryReader br = new BinaryReader(ms);*/

            if (custom_data != null)
            {
                ignore_mipmap_settings_on_load = ((SFTextureLoadArgs)custom_data).IgnoreMipmapSettings;
            }

            if (LoadDDS(data, offset) != 0)
            {
                if (LoadTGA(data, offset) != 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.Load(): Could not deduce texture data type!");
                    return -201;
                }
            }

            if(custom_data != null)
            {
                free_on_init = ((SFTextureLoadArgs)custom_data).FreeOnInit;
            }

            return 0;
        }

        public int LoadDDS(byte[] fdata, int foffset)
        {
            MemoryStream ms = new MemoryStream(fdata, foffset, fdata.Length - foffset);
            BinaryReader br = new BinaryReader(ms);
            br.BaseStream.Position = 0;

            uint[] header = new uint[31];
            uint filecode = br.ReadUInt32();
            if (filecode != 0x20534444)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadDDS(): Invalid header guard");
                return -502;
            }

            for (int i = 0; i < 31; i++)
            {
                header[i] = br.ReadUInt32();
            }

            width = (int)header[2];
            height = (int)header[3];
            depth = 1;
            uint linearSize;
            uint blockSize;
            uint mipMapC = header[6];
            uint fourCC = header[20];
            if (mipMapC == 0)
                mipMapC = 1;
            mipMapStart = 0;
            mipMapCount = mipMapC;
            CalculateMipmapLevel(width, height);
            min_filter = (int)All.LinearMipmapLinear;
            mag_filter = (int)All.Linear;
            wrap_s = (int)All.ClampToEdge;
            wrap_t = (int)All.ClampToEdge;
            anisotropy = Settings.MaxAnisotropy;
            generate_mipmap = false;
            free_on_init = true;

            texture_target = TextureTarget.Texture2d;
            // no pixel format or pixel type; texture internal format is compressed, as seen below

            switch (fourCC)
            {
                case 0x31545844:
                    internal_format = SizedInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    blockSize = 8;
                    break;
                case 0x33545844:
                    internal_format = SizedInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    blockSize = 16;
                    break;
                case 0x35545844:
                    internal_format = SizedInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    blockSize = 16;
                    break;
                default:
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadDDS(): Invalid texture format");
                    return -503;  //wrong fourcc
            }
            linearSize = (((uint)width + 3) / 4) * blockSize;
            if (linearSize < 1)
            {
                linearSize = 1;
            }

            uint size = (((uint)width + 3) / 4) * (((uint)height + 3) / 4) * blockSize;
            uint skip_size = 0;

            uint buf_size = size;


            if (mipMapC > 1)
            {
                uint w, h;
                w = (uint)width;
                h = (uint)height;
                buf_size = 0;
                for (uint level = 0; level < mipMapC && (w != 0 || h != 0); ++level)
                {
                    size = ((w + 3) / 4) * ((h + 3) / 4) * blockSize;
                    if (level >= mipMapStart)
                    {
                        buf_size += size;
                    }
                    else
                    {
                        skip_size += size;
                    }
                    w /= 2;
                    h /= 2;
                }
            }

            data = fdata;
            data_offset = (int)(foffset + 128 + skip_size);

            //br.BaseStream.Position += skip_size;
            //data = br.ReadBytes((int)buf_size);
            RAMSize = data.Length - data_offset;//data.Length;
            return 0;
        }

        // http://www.paulbourke.net/dataformats/tga/tgatest.c
        public int LoadTGA(byte[] fdata, int foffset)   // tga contains no mipmaps
        {
            MemoryStream ms = new MemoryStream(fdata, foffset, fdata.Length - foffset);
            BinaryReader br = new BinaryReader(ms);
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

            if ((image_type != 2) && (image_type != 10))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadTGA(): Invalid image type");
                return -512;
            }

            if ((isp_bpp / 8 < 2) && (isp_bpp / 8 > 4))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadTGA(): Invalid number of bits per pixel");
                return -513;
            }

            if (is_color_map > 1)
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
            for (int i = 0; i < isp_w; i++)
            {
                for (int j = 0; j < isp_h / 2; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        byte tmp_px = pixels[(i + j * isp_w) * 4 + k];
                        pixels[(i + j * isp_w) * 4 + k] = pixels[(i + (isp_h - j - 1) * isp_w) * 4 + k];
                        pixels[(i + (isp_h - j - 1) * isp_w) * 4 + k] = tmp_px;
                    }
                }
            }

            data = pixels;
            data_offset = 0;
            width = isp_w; 
            height = isp_h;
            depth = 1;
            mipMapStart = 0;
            mipMapCount = 1;
            CalculateMipmapLevel(width, height);
            texture_target = TextureTarget.Texture2d;
            internal_format = SizedInternalFormat.Rgba8;
            pixel_format = PixelFormat.Rgba;
            pixel_type = PixelType.UnsignedByte;
            min_filter = (int)All.LinearMipmapLinear;
            mag_filter = (int)All.Linear;
            wrap_s = (int)All.ClampToEdge;
            wrap_t = (int)All.ClampToEdge;
            anisotropy = Settings.MaxAnisotropy;
            generate_mipmap = true;
            free_on_init = true;
            RAMSize = data.Length;

            return 0;
        }

        public void TGAFixPixel(byte[] pixels, int pixeloffset, int bpp)
        {
            if (bpp == 4)
            {
                byte tmp_r = pixels[pixeloffset * 4 + 0];
                pixels[pixeloffset * 4 + 0] = pixels[pixeloffset * 4 + 2];
                pixels[pixeloffset * 4 + 2] = tmp_r;
            }
            else if (bpp == 3)
            {
                byte tmp_r = pixels[pixeloffset * 4 + 0];
                pixels[pixeloffset * 4 + 0] = pixels[pixeloffset * 4 + 2];
                pixels[pixeloffset * 4 + 2] = tmp_r;
                pixels[pixeloffset * 4 + 3] = 255;
            }
            else if (bpp == 2)
            {
                byte b1 = pixels[pixeloffset * 4 + 0];
                byte b2 = pixels[pixeloffset * 4 + 1];
                pixels[pixeloffset * 4 + 0] = (byte)((b2 & 0x7c) << 1);
                pixels[pixeloffset * 4 + 1] = (byte)(((b2 & 0x03) << 6) | ((b1 & 0xe0) >> 2));
                pixels[pixeloffset * 4 + 2] = (byte)((b1 & 0x1f) << 3);
                pixels[pixeloffset * 4 + 3] = (byte)(b1 & 0x80);
            }
        }

        public int LoadUncompressedRGBA(BinaryReader br, ushort w, ushort h)
        {
            br.BaseStream.Position = 0;

            int expected_size = w * h * 4;
            int read_size = 0;

            if (br.BaseStream.Length != expected_size)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadUncompressedRGBA(): Data length is not valid");
                return -612;
            }

            data = br.ReadBytes(expected_size);
            data_offset = 0;
            read_size = data.Length;
            if(read_size != expected_size)
            {
                data = null;
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFTexture.LoadUncompressedRGBA(): Data length is not valid");
                return -612;
            }
            width = w; 
            height = h;
            depth = 1;
            mipMapStart = 0;
            mipMapCount = 1;
            texture_target = TextureTarget.Texture2d;
            internal_format = SizedInternalFormat.Rgba8;
            pixel_format = PixelFormat.Rgba;
            pixel_type = PixelType.UnsignedByte;
            min_filter = (int)All.Linear;
            mag_filter = (int)All.Linear;
            wrap_s = (int)All.ClampToEdge;
            wrap_t = (int)All.ClampToEdge;
            anisotropy = Settings.MaxAnisotropy;
            RAMSize = data.Length;
            free_on_init = false;
            generate_mipmap = false;

            return 0;
        }

        static public SFTexture DynamicTexture(ushort w, ushort h, ushort d, TextureTarget target, SizedInternalFormat ifmt, PixelFormat pfmt, PixelType ptp, int minf, int magf, int ws, int wt, Vector4 wbcol, int an, bool gen_mip, bool own_memory)
        {
            SFTexture tex = new SFTexture()
            {
                data = (own_memory ? new byte[w * h * d * InternalFormatSizeBits[ifmt] / 8] : null),
                data_offset = 0,
                width = w,
                height = h,
                depth = d,
                texture_target = target,
                internal_format = ifmt,
                pixel_format = pfmt,
                pixel_type = ptp,
                free_on_init = !own_memory,
                generate_mipmap = gen_mip,
                ignore_mipmap_settings_on_load = false,
                mipMapStart = 0,
                mipMapCount = 1,
                min_filter = minf,
                mag_filter = magf,
                wrap_s = ws,
                wrap_t = wt,
                wrap_border_col = wbcol,
                anisotropy = Settings.MaxAnisotropy,
                sampleCount = 0,
            };
            if (tex.generate_mipmap)
            {
                tex.CalculateMipmapLevel(w, h);
            }
            tex.RAMSize = (own_memory? tex.data.Length: 0);
            return tex;
        }

        static public SFTexture FrameBufferAttachment(ushort w, ushort h, uint sample_count, uint mipstart, uint mipcount, SizedInternalFormat ifmt, PixelFormat pfmt, PixelType ptp, int minf, int magf, int ws, int wt, Vector4 wbcol, int an)
        {
            if(sample_count > 1)
            {
                mipstart = 0;
                mipcount = 1;
            }

            SFTexture tex = new SFTexture()
            {
                data_offset = 0,
                width = w,
                height = h,
                depth = 1,
                sampleCount = sample_count,
                mipMapStart = mipstart,
                mipMapCount = mipcount,
                internal_format = ifmt,
                pixel_format = pfmt,
                pixel_type = ptp,
                min_filter = minf,
                mag_filter = magf,
                wrap_s = ws,
                wrap_t = wt,
                wrap_border_col = wbcol,
                anisotropy = an,
            };
            tex.texture_target = (sample_count > 1 ? TextureTarget.Texture2dMultisample : TextureTarget.Texture2d);
            tex.free_on_init = true;
            tex.generate_mipmap = false;
            tex.ignore_mipmap_settings_on_load = true;

            return tex;
        }

        public void UpdateImage<T>(T[] idata, int idata_offset, int lvl, int d)
        {
            GCHandle idata_handle = GCHandle.Alloc(idata, GCHandleType.Pinned);
            IntPtr idata_ptr = idata_handle.AddrOfPinnedObject();
            if (texture_target == TextureTarget.Texture2d)
            {
                GL.TextureSubImage2D(tex_id, lvl, 0, 0, width, height, pixel_format, pixel_type, new IntPtr(idata_ptr.ToInt64() + idata_offset));
            }
            else if(texture_target == TextureTarget.Texture2dArray)
            {
                GL.TextureSubImage3D(tex_id, lvl, 0, 0, d, width, height, 1, pixel_format, pixel_type, new IntPtr(idata_ptr.ToInt64() + idata_offset));
            }
            idata_handle.Free();

            if (generate_mipmap)
            {
                GL.GenerateTextureMipmap(tex_id);
            }
        }

        // uncompress utility (used in heightmap array texture to have all textures use the same uncompressed format
        // assumes texture is already initialized
        public void Uncompress()
        {
            if(tex_id == SFEngine.Utility.NO_INDEX)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFTexture.Uncompress(): Texture is not initialized!");
                return;
            }
            if (internal_format == SizedInternalFormat.Rgba8)
            {
                return;
            }

            // 2. Get image
            int blockSize = (internal_format == SizedInternalFormat.CompressedRgbaS3tcDxt1Ext) ? 8 : 16;
            byte[] pixels = new byte[data.Length * 64 / blockSize];
            int offset = 0;
            int w = width;
            int h = height;
            for (int level = 0; level < (mipMapStart + mipMapCount) && (w != 0 || h != 0); ++level)
            {
                int size = ((w + 3) / 4) * ((h + 3) / 4) * 64;
                if (level >= mipMapStart)
                {
                    GL.GetTextureImage(tex_id, (int)(level - mipMapStart), PixelFormat.Rgba, PixelType.UnsignedByte, size, ref pixels[offset]);

                    offset += size;
                }
                w /= 2;
                h /= 2;
            }

            // 3. Deinit
            GL.DeleteTexture(tex_id);
            DeviceSize = 0;
            tex_id = Utility.NO_INDEX;

            data = pixels;
            data_offset = 0;
            internal_format = SizedInternalFormat.Rgba8;
            pixel_format = PixelFormat.Rgba;
            pixel_type = PixelType.UnsignedByte;
            RAMSize = data.Length;
        }

        public byte GetUncompressedAlpha(int x, int y)
        {
            return data[(y * width + x) * 4 + 3];
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

            if (internal_format != SizedInternalFormat.Rgba8)
            {
                return b;
            }

            int cur_w = width;
            int cur_h = height;
            int cur_mip = 0;

            if (args.ConversionType == SFTextureToBitmapArgType.DIMENSION)
            {
                int default_width = width;
                int default_height = height;
                for (uint level = 0; level < mipMapStart && (cur_w != 0 || cur_h != 0); ++level)
                {
                    default_width /= 2;
                    default_height /= 2;
                }

                while ((cur_h != 0) && (cur_w != 0))
                {
                    if ((cur_h == args.DimHeight) && (cur_w == args.DimWidth))
                    {
                        break;
                    }

                    cur_h /= 2;
                    cur_w /= 2;
                    cur_mip += 1;
                }

                if ((cur_h == 0) || (cur_w == 0))
                {
                    return b;
                }

                int skip = 1 << (cur_mip);
                int div = skip * skip;

                b = new Bitmap(cur_w, cur_h);
                Rectangle rect = new Rectangle(0, 0, cur_w, cur_h);
                System.Drawing.Imaging.BitmapData bd = b.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, b.PixelFormat);

                IntPtr ptr = bd.Scan0;
                int bytes = Math.Abs(bd.Stride) * b.Height;
                byte[] rgbValues = new byte[bytes];
                Marshal.Copy(ptr, rgbValues, 0, bytes);

                for (int j = 0; j < default_height; j += skip)
                {
                    for (int i = 0; i < default_width; i += skip)
                    {
                        // sum colors and average
                        int red = 0;
                        int green = 0;
                        int blue = 0;
                        for(int y = 0; y < skip; y++)
                        {
                            for(int x = 0; x < skip; x++)
                            {
                                red += data[4 * ((j + y) * default_width + i + x) + 0];
                                green += data[4 * ((j + y) * default_width + i + x) + 1];
                                blue += data[4 * ((j + y) * default_width + i + x) + 2];
                            }
                        }
                        red /= div;
                        green /= div;
                        blue /= div;

                        int k = (i / skip) + cur_w * (cur_h - 1 - (j / skip));

                        rgbValues[k * 4 + 3] = 255;
                        rgbValues[k * 4 + 2] = (byte)red;
                        rgbValues[k * 4 + 1] = (byte)green;
                        rgbValues[k * 4 + 0] = (byte)blue;
                    }
                }

                // Copy the RGB values back to the bitmap
                Marshal.Copy(rgbValues, 0, ptr, bytes);

                // Unlock the bits.
                b.UnlockBits(bd);

                return b;
            }
            else
            {
                return b;
            }
        }

        // texture format: bmp
        public void Export(string fname)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFTexture.Export() called, filename: " + fname + ".bmp");

            if (internal_format != SizedInternalFormat.Rgba8)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFTexture.Export(): Invalid internal format for texture export!");
                return;
            }
            if (data == null)
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

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bw.Write(data[(i * height + j) * 4 + 2]);
                    bw.Write(data[(i * height + j) * 4 + 1]);
                    bw.Write(data[(i * height + j) * 4 + 0]);
                }
            }

            bw.Close();
        }

        // used after loading textures that are no longer needed in memory, i.e, won't be reloaded anymore
        public void FreeMemory()
        {
            data = null;
            data_offset = 0;
            RAMSize = 0;
        }

        public override void Dispose()
        {
            if (tex_id != Utility.NO_INDEX)
            {
                GL.DeleteTexture(tex_id);
                tex_id = Utility.NO_INDEX;
                DeviceSize = 0;
            }
            FreeMemory();
        }

        public override string ToString()
        {
            return Name;
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

            if ((tex1.internal_format != SizedInternalFormat.Rgba8) 
                || (tex2.internal_format != SizedInternalFormat.Rgba8)
                || (tex3.internal_format != SizedInternalFormat.Rgba8))
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
            new_tex.mipMapStart = tex1.mipMapStart;
            new_tex.mipMapCount = tex1.mipMapCount;
            new_tex.texture_target = tex1.texture_target;
            new_tex.internal_format = tex1.internal_format;
            new_tex.pixel_format = tex1.pixel_format;
            new_tex.pixel_type = tex1.pixel_type;
            new_tex.wrap_s = tex1.wrap_s;
            new_tex.wrap_t = tex1.wrap_t;
            new_tex.min_filter = tex1.min_filter;
            new_tex.mag_filter = tex1.mag_filter;
            new_tex.anisotropy = tex1.anisotropy;
            new_tex.wrap_border_col = tex1.wrap_border_col;
            new_tex.free_on_init = false;
            new_tex.generate_mipmap = false;
            if (new_tex.data == null)
            {
                new_tex.data = new byte[tex1.data.Length];
                new_tex.RAMSize = new_tex.data.Length;
            }

            for (int i = 0; i < tex1.data.Length; i++)
            {
                new_tex.data[i] = (byte)((w1 * tex1.data[i] + w2 * tex2.data[i] + w3 * tex3.data[i]) / 255);
            }
        }
    }
}
