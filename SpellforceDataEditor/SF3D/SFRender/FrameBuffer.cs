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
    public struct FrameBufferColorAttachmentInfo
    {
        public PixelInternalFormat internal_format;
        public PixelFormat format;
        public PixelType pixeltype;
    }

    public class FrameBuffer
    {
        public enum RenderBufferType { NONE = 0, COLOR = 1, DEPTH = 2, STENCIL = 3, DEPTHSTENCIL = 4 }
        [Flags]
        public enum TextureType { NONE = 0, COLOR = 1, DEPTH = 2, STENCIL = 4 }

        static float[] vertices = new float[] { -1, -3, -1, 1, 3, 1 };
        static float[] uvs = new float[] { 0, -1, 0, 1, 2, 1 };

        public static int screen_vao { get; private set; } = -1;

        static int vertices_vbo = Utility.NO_INDEX;
        static int uvs_vbo = Utility.NO_INDEX;
        static int ref_count = 0;

        public int width, height;
        public int fbo = Utility.NO_INDEX;
        public FrameBufferColorAttachmentInfo[] attachments;
        public int[] texture_colors = null;
        public bool use_depth_component;
        public int texture_depth = Utility.NO_INDEX;
        public int rbo = Utility.NO_INDEX;
        public int sample_count = 1;

        public FrameBuffer(int w, int h, FrameBufferColorAttachmentInfo[] _attachments, int s_count, bool use_depth)
        {
            if (ref_count == 0)
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

            use_depth_component = use_depth;
            attachments = _attachments;
            sample_count = s_count;

            Resize(w, h);
        }

        public void Resize(int w, int h)
        {
            width = w;
            height = h;

            if (fbo != Utility.NO_INDEX)
            {
                if (rbo != Utility.NO_INDEX)
                    GL.DeleteRenderbuffer(rbo);

                if (texture_colors != null)
                    foreach (int tc in texture_colors)
                        GL.DeleteTexture(tc);
                texture_colors = null;

                if (texture_depth != Utility.NO_INDEX)
                    GL.DeleteTexture(texture_depth);

                GL.DeleteFramebuffer(fbo);
            }

            if (attachments != null)
                texture_colors = new int[attachments.Length];

            fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            if (attachments != null)
                for (int i = 0; i < attachments.Length; i++)
                {
                    texture_colors[i] = GL.GenTexture();
                    if (sample_count > 1)
                    {
                        GL.BindTexture(TextureTarget.Texture2DMultisample, texture_colors[i]);
                        GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, sample_count, attachments[i].internal_format, width, height, true);
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, (FramebufferAttachment)((int)FramebufferAttachment.ColorAttachment0 + i), TextureTarget.Texture2DMultisample, texture_colors[i], 0);
                    }
                    else
                    {
                        GL.BindTexture(TextureTarget.Texture2D, texture_colors[i]);
                        GL.TexImage2D(TextureTarget.Texture2D, 0, attachments[i].internal_format, width, height, 0, attachments[i].format, attachments[i].pixeltype, new IntPtr(0));
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, (FramebufferAttachment)((int)FramebufferAttachment.ColorAttachment0 + i), TextureTarget.Texture2D, texture_colors[i], 0);
                    }
                }

            if (use_depth_component)
            {
                texture_depth = GL.GenTexture();
                if (sample_count > 1)
                {
                    GL.BindTexture(TextureTarget.Texture2DMultisample, texture_depth);
                    GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, sample_count, PixelInternalFormat.DepthComponent32, width, height, true);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2DMultisample, texture_depth, 0);
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, texture_depth);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, new IntPtr(0));
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, texture_depth, 0);
                }
            }

            SetUpDrawBuffers();

            FramebufferErrorCode e = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (e != FramebufferErrorCode.FramebufferComplete)
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "Framebuffer.Resize(): Error generating framebuffer! Error type " + e.ToString());

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void SetUpDrawBuffers()
        {
            if (attachments == null)
            {
                GL.DrawBuffer(DrawBufferMode.None);
                GL.ReadBuffer(ReadBufferMode.None);
            }
            else
            {
                DrawBuffersEnum[] atts = new DrawBuffersEnum[attachments.Length];
                for (int i = 0; i < attachments.Length; i++)
                    atts[i] = (DrawBuffersEnum)((int)DrawBuffersEnum.ColorAttachment0 + i);
                GL.DrawBuffers(atts.Length, atts);
            }
        }

        public void Dispose()
        {
            if (fbo != Utility.NO_INDEX)
            {
                if (rbo != Utility.NO_INDEX)
                    GL.DeleteRenderbuffer(rbo);

                if(texture_depth != Utility.NO_INDEX)
                    GL.DeleteTexture(texture_depth);
                texture_depth = Utility.NO_INDEX;

                if (texture_colors != null)
                    foreach (int tc in texture_colors)
                        GL.DeleteTexture(tc);

                texture_colors = null;
                GL.DeleteFramebuffer(fbo);
            }

            ref_count -= 1;
            if (ref_count == 0)
            {
                GL.DeleteBuffer(uvs_vbo);
                GL.DeleteBuffer(vertices_vbo);
                GL.DeleteVertexArray(screen_vao);
            }
            rbo = Utility.NO_INDEX;
            fbo = Utility.NO_INDEX;
        }
    }
}
