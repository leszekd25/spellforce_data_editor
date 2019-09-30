/*
 * OpenGL renders everything to a framebuffer
 * While device context already contains a framebuffer, users can create their own framebuffers to
 * render temporary stuff on it to be used for subsequent render passes
 * FrameBuffer provides simple interface for creating and destroying framebuffers of various types
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SF3D.SFRender
{
    public class FrameBuffer
    {
        public enum RenderBufferType { NONE = 0, COLOR = 1, DEPTH = 2, STENCIL = 3, DEPTHSTENCIL = 4}
        [Flags]
        public enum TextureType { NONE = 0, COLOR = 1, DEPTH = 2, STENCIL = 4}

        static float[] vertices = new float[] { -1, -3, -1, 1, 3, 1 };
        static float[] uvs = new float[] { 0, -1, 0, 1, 2, 1 };

        public static int screen_vao { get; private set; } = -1;

        static int vertices_vbo = -1;
        static int uvs_vbo = -1;
        static int ref_count = 0;

        public int width, height;
        public RenderBufferType buff_type;
        public TextureType tex_type;
        public int fbo = -1;
        public int texture_color = -1;
        public int texture_depth = -1;
        public int texture_stencil = -1;
        public int rbo = -1;

        public int sample_count = 0;

        public FrameBuffer(int w, int h, int sc, TextureType tex = TextureType.COLOR, RenderBufferType buf = RenderBufferType.DEPTHSTENCIL)
        {
            if(ref_count == 0)
            {
                screen_vao = GL.GenVertexArray();
                GL.BindVertexArray(screen_vao);

                vertices_vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertices_vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 4, vertices, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

                uvs_vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, uvs_vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * 4, uvs, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindVertexArray(0);
            }
            ref_count += 1;

            buff_type = buf;
            tex_type = tex;
            sample_count = sc;

            Resize(w, h);
        }

        public void Resize(int w, int h)
        {
            width = w;
            height = h;

            if (fbo != -1)
            {
                if (rbo != -1)
                    GL.DeleteRenderbuffer(rbo);
                if (texture_color != -1)
                    GL.DeleteTexture(texture_color);
                if (texture_depth != -1)
                    GL.DeleteTexture(texture_depth);
                if (texture_stencil != -1)
                    GL.DeleteTexture(texture_stencil);
                GL.DeleteFramebuffer(fbo);
            }

            // determine renderbuffer type
            FramebufferAttachment buff_fba_type;
            RenderbufferStorage buff_st_type;
            switch((int)buff_type)
            {
                case 1:
                    buff_fba_type = FramebufferAttachment.ColorAttachment0;
                    buff_st_type = RenderbufferStorage.Rgb8;
                    break;
                case 2:
                    buff_fba_type = FramebufferAttachment.DepthAttachment;
                    buff_st_type = RenderbufferStorage.DepthComponent24;
                    break;
                case 3:
                    buff_fba_type = FramebufferAttachment.StencilAttachment;
                    buff_st_type = RenderbufferStorage.StencilIndex8;
                    break;
                case 4:
                    buff_fba_type = FramebufferAttachment.DepthStencilAttachment;
                    buff_st_type = RenderbufferStorage.Depth24Stencil8;
                    break;
                default:
                    buff_fba_type = FramebufferAttachment.DepthStencilAttachment;
                    buff_st_type = RenderbufferStorage.Depth24Stencil8;
                    break;
            }

            fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            if (sample_count != 0)
            {
                if ((tex_type & TextureType.COLOR) != 0)
                {
                    texture_color = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2DMultisample, texture_color);
                    GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, sample_count, PixelInternalFormat.Rgb, width, height, true);
                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMinFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMagFilter, (int)All.Linear);
                    GL.BindTexture(TextureTarget.Texture2DMultisample, 0);

                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture_color, 0);
                }

                if ((tex_type & TextureType.DEPTH) != 0)
                {
                    texture_depth = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2DMultisample, texture_depth);
                    GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, sample_count, PixelInternalFormat.DepthComponent24, width, height, true);
                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
                    float[] col = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureBorderColor, col);
                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                    GL.BindTexture(TextureTarget.Texture2DMultisample, 0);

                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, texture_depth, 0);
                }

                if ((tex_type & TextureType.STENCIL) != 0)
                {
                    texture_stencil = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2DMultisample, texture_stencil);
                    GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, sample_count, PixelInternalFormat.R8, width, height, true);
                    GL.BindTexture(TextureTarget.Texture2DMultisample, 0);

                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.StencilAttachment, texture_stencil, 0);
                }

                if (buff_type != RenderBufferType.NONE)
                {
                    rbo = GL.GenRenderbuffer();
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
                    GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, sample_count, buff_st_type, width, height);
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                    GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, buff_fba_type, RenderbufferTarget.Renderbuffer, rbo);
                }
            }
            else
            {
                if ((tex_type & TextureType.COLOR) != 0)
                {
                    texture_color = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, texture_color);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, new IntPtr(0));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                    GL.BindTexture(TextureTarget.Texture2D, 0);

                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture_color, 0);
                }

                if ((tex_type & TextureType.DEPTH) != 0)
                {
                    texture_depth = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, texture_depth);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, new IntPtr(0));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
                    float[] col = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, col);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                    GL.BindTexture(TextureTarget.Texture2D, 0);

                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, texture_depth, 0);
                }

                if ((tex_type & TextureType.STENCIL) != 0)
                {
                    texture_stencil = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, texture_stencil);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, width, height, 0, PixelFormat.StencilIndex, PixelType.UnsignedByte, new IntPtr(0));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
                    float[] col = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, col);
                    GL.BindTexture(TextureTarget.Texture2D, 0);

                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.StencilAttachment, texture_stencil, 0);
                }

                if (buff_type != RenderBufferType.NONE)
                {
                    rbo = GL.GenRenderbuffer();
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
                    GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, buff_st_type, width, height);
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                    GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, buff_fba_type, RenderbufferTarget.Renderbuffer, rbo);
                }
            }

            if ((buff_type != RenderBufferType.COLOR) && ((tex_type & TextureType.COLOR) == 0))
            {
                GL.DrawBuffer(DrawBufferMode.None);
                GL.ReadBuffer(ReadBufferMode.None);
            }
            FramebufferErrorCode e = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (e != FramebufferErrorCode.FramebufferComplete)
            {
                while (true)
                {
                    ErrorCode ec = GL.GetError();
                    if (ec == ErrorCode.NoError)
                        break;
                    LogUtils.Log.Error(LogUtils.LogSource.SFMap, "Framebuffer.Resize(): OpenGL error '" + ec.ToString());
                    System.Diagnostics.Debug.WriteLine("Framebuffer.Resize() " + ec);
                }
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "Framebuffer.Resize(): Error generating framebuffer! Error type "+e.ToString());
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            if(fbo != -1)
            {
                if(rbo != -1)
                    GL.DeleteRenderbuffer(rbo);
                if (texture_color != -1)
                    GL.DeleteTexture(texture_color);
                if (texture_depth != -1)
                    GL.DeleteTexture(texture_depth);
                if (texture_stencil != -1)
                    GL.DeleteTexture(texture_stencil);
                GL.DeleteFramebuffer(fbo);
            }
            ref_count -= 1;
            if(ref_count == 0)
            {
                GL.DeleteBuffer(uvs_vbo);
                GL.DeleteBuffer(vertices_vbo);
                GL.DeleteVertexArray(screen_vao);
            }
        }
    }
}
