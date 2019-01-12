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
        public byte[] data { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }
        public int tex_id { get; private set; } = -1;
        public uint mipMapCount { get; private set; }
        InternalFormat format;

        public SFTexture()
        {
            data = null;
        }

        public SFTexture(MemoryStream ms)
        {
            Load(ms);
            Init();
        }

        public void Init()
        {
            tex_id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex_id);

            int blockSize = (format == InternalFormat.CompressedRgbaS3tcDxt1Ext) ? 8 : 16;
            int offset = 0;
            int w = width;
            int h = height;
            

            /* load the mipmaps */
            for (int level = 0; level < mipMapCount && (w!=0 || h!=0); ++level)
            {
                int size = ((w + 3) / 4) * ((h + 3) / 4) * blockSize;
                byte[] mipMapData = data.Skip(offset).Take(size).ToArray();
                GL.CompressedTexImage2D(TextureTarget.Texture2D, level, format, w, h,
                    0, size, mipMapData);

                offset += size;
                w /= 2;
                h /= 2;
            }
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
                    buf_size += size;
                    w /= 2;
                    h /= 2;
                }
            }
            else
                buf_size = size;

            data = br.ReadBytes((int)buf_size);
            return 0;
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
                +"\nTEX FORMAT#"+format.ToString();
        }
    }
}
