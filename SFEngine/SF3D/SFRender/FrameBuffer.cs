/*
 * OpenGL renders everything to a framebuffer
 * While device context already contains a framebuffer, users can create their own framebuffers to
 * render temporary stuff on it to be used for subsequent render passes
 * FrameBuffer provides simple interface for creating and destroying framebuffers of various types
 * */

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace SFEngine.SF3D.SFRender
{
    public struct FramebufferAttachmentInfo
    {
        public FramebufferAttachment attachment_type;
        public PixelInternalFormat internal_format;
        public PixelFormat format;
        public PixelType pixel_type;
        public int sample_count;
        public int min_filter;
        public int mag_filter;
        public int wrap_s;
        public int wrap_t;
        public Vector4 wrap_border_col;
        public int anisotropy;
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
        public FramebufferAttachmentInfo[] attachments;
        public SFTexture[] textures = null;
        //public int[] textures = null;

        public FrameBuffer(int w, int h, FramebufferAttachmentInfo[] _attachments)
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

            attachments = _attachments;

            Resize(w, h);
        }

        public void Resize(int w, int h)
        {
            width = w;
            height = h;

            if (fbo != Utility.NO_INDEX)
            {
                if (textures != null)
                {
                    for (int i = 0; i < textures.Length; i++)//foreach(int t in textures)
                    {
                        SFResources.SFResourceManager.Textures.Dispose("_FRAMEBUFFER_" + fbo.ToString() + "_ATTACHMENT_" + i.ToString());
                    }
                }

                textures = null;

                GL.DeleteFramebuffer(fbo);
            }

            if (attachments != null)
            {
                textures = new SFTexture[attachments.Length];//new int[attachments.Length];
            }

            fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            List<DrawBuffersEnum> col_attachments = new List<DrawBuffersEnum>();

            if (attachments != null)
            {
                for (int i = 0; i < attachments.Length; i++)
                {
                    //textures[i] = GL.GenTexture();
                    int mipcount = 1;
                    if (attachments[i].min_filter == (int)All.LinearMipmapLinear)
                    {
                        int cur_w = width / 2;
                        int cur_h = width / 2;
                        while ((cur_w != 0) && (cur_h != 0))
                        {
                            mipcount++;
                            cur_w /= 2;
                            cur_h /= 2;
                        }
                    }

                    textures[i] = SFTexture.FrameBufferAttachment((ushort)width, (ushort)height, (uint)attachments[i].sample_count, 0, (uint)mipcount, 
                        (InternalFormat)attachments[i].internal_format, attachments[i].format, attachments[i].pixel_type,
                        attachments[i].min_filter, attachments[i].mag_filter, attachments[i].wrap_s, attachments[i].wrap_t, attachments[i].wrap_border_col, attachments[i].anisotropy);
                    SFResources.SFResourceManager.Textures.AddManually(textures[i], "_FRAMEBUFFER_" + fbo.ToString() + "_ATTACHMENT_" + i.ToString());
                    /*TextureTarget tex_target;
                    if (attachments[i].sample_count > 1)
                    {
                        tex_target = TextureTarget.Texture2DMultisample;
                        SFRenderEngine.SetTexture(0, tex_target, textures[i]);
                        GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, attachments[i].sample_count, attachments[i].internal_format, width, height, true);
                    }
                    else
                    {
                        tex_target = TextureTarget.Texture2D;
                        SFRenderEngine.SetTexture(0, tex_target, textures[i]);
                        GL.TexImage2D(tex_target, 0, attachments[i].internal_format, width, height, 0, attachments[i].format, attachments[i].pixel_type, new IntPtr(0));
                        if (attachments[i].min_filter == (int)All.LinearMipmapLinear)
                        {
                            int cur_w = width / 2;
                            int cur_h = width / 2;
                            int lvl = 1;
                            while ((cur_w != 0) && (cur_h != 0))
                            {
                                GL.TexImage2D(tex_target, lvl, attachments[i].internal_format, cur_w, cur_h, 0, attachments[i].format, attachments[i].pixel_type, new IntPtr(0));
                                lvl++;
                                cur_w /= 2;
                                cur_h /= 2;
                            }
                        }

                        GL.TexParameter(tex_target, TextureParameterName.TextureMinFilter, attachments[i].min_filter);
                        GL.TexParameter(tex_target, TextureParameterName.TextureMagFilter, attachments[i].mag_filter);
                        GL.TexParameter(tex_target, TextureParameterName.TextureWrapS, attachments[i].wrap_s);
                        GL.TexParameter(tex_target, TextureParameterName.TextureWrapT, attachments[i].wrap_t);
                        if (attachments[i].wrap_s == (int)All.ClampToBorder)
                        {
                            float[] col = new float[] { attachments[i].wrap_border_col.X, attachments[i].wrap_border_col.Y, attachments[i].wrap_border_col.Z, attachments[i].wrap_border_col.W };
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, col);
                        }
                        if ((Settings.AnisotropicFiltering) && (attachments[i].anisotropy != 0))
                        {
                            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)All.TextureMaxAnisotropy, (float)attachments[i].anisotropy);
                        }
                    }*/
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachments[i].attachment_type, textures[i].texture_target, textures[i].tex_id, 0);
                    //GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachments[i].attachment_type, tex_target, textures[i], 0);


                    if ((attachments[i].attachment_type >= FramebufferAttachment.ColorAttachment0) && (attachments[i].attachment_type <= FramebufferAttachment.ColorAttachment31))
                    {
                        col_attachments.Add((DrawBuffersEnum)attachments[i].attachment_type);
                    }
                }
            }

            if (col_attachments.Count == 0)
            {
                GL.DrawBuffer(DrawBufferMode.None);
                GL.ReadBuffer(ReadBufferMode.None);
            }
            else
            {
                GL.DrawBuffers(col_attachments.Count, col_attachments.ToArray());
            }

            FramebufferErrorCode e = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (e != FramebufferErrorCode.FramebufferComplete)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "Framebuffer.Resize(): Error generating framebuffer! Error type " + e.ToString());
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            if (fbo != Utility.NO_INDEX)
            {
                if (textures != null)
                {
                    for (int i = 0; i < textures.Length; i++)//foreach(int t in textures)
                    {
                        SFResources.SFResourceManager.Textures.Dispose("_FRAMEBUFFER_" + fbo.ToString() + "_ATTACHMENT_" + i.ToString());
                    }
                }

                textures = null;

                GL.DeleteFramebuffer(fbo);
            }

            ref_count -= 1;
            if (ref_count == 0)
            {
                GL.DeleteBuffer(uvs_vbo);
                GL.DeleteBuffer(vertices_vbo);
                GL.DeleteVertexArray(screen_vao);
            }
            fbo = Utility.NO_INDEX;
        }
    }
}
