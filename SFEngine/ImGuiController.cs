using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ImGuiNET;
using OpenTK.Platform;
using System.Runtime.InteropServices;

namespace SFEngine
{
    public class ImGuiController
    {
        int width;
        int height;
        Vector2 scale_factor = Vector2.One;

        int vao;
        int vbo;
        int vbo_size;
        int ebo;
        int ebo_size;
        Matrix4 proj_matrix;
        int vshader;
        int fshader;
        int program;
        int proj_uniform;
        int font_texture_uniform;
        int font_tex_id;

        public ImGuiController(Vector2 size)
        {
            width = (int)size.X;
            height = (int)size.Y;

            ImGui.CreateContext();
            ImGuiIOPtr io = ImGui.GetIO();
            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard | ImGuiConfigFlags.DockingEnable;
            io.Fonts.Flags |= ImFontAtlasFlags.NoBakedLines;

            Init();
            io.DisplaySize = new System.Numerics.Vector2(size.X, size.Y);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(scale_factor.X, scale_factor.Y);
            proj_matrix = Matrix4.CreateOrthographicOffCenter(0, io.DisplaySize.X, io.DisplaySize.Y, 0, -1, 1);

            ImGui.NewFrame();
        }

        public void OnResize(Vector2 new_size)
        {
            width = (int)new_size.X;
            height = (int)new_size.Y;

            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(new_size.X, new_size.Y);
            proj_matrix = Matrix4.CreateOrthographicOffCenter(0, io.DisplaySize.X, io.DisplaySize.Y, 0, -1, 1);
        }

        void Init()
        {
            // get previous pipeline
            int prevVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int prevArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);
            int prevElementArrayBuffer = GL.GetInteger(GetPName.ElementArrayBufferBinding);
            int prevProgram = GL.GetInteger(GetPName.CurrentProgram);
            int prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
            GL.ActiveTexture(TextureUnit.Texture0);
            int prevTexture2D = GL.GetInteger(GetPName.TextureBinding2d);

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
            vbo_size = 8192;
            ebo_size = 32768;

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 20 * vbo_size, new IntPtr(0), BufferUsage.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 20, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 20, 8);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, 20, 16);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 2 * ebo_size, new IntPtr(0), BufferUsage.DynamicDraw);
            GL.VertexArrayElementBuffer(vao, ebo);
            GL.BindVertexArray(0);

            proj_matrix = Matrix4.Identity;

            // create imgui shader
            string vshader_text = @"#version 430 core

uniform mat4 projection_matrix;

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_texCoord;
layout(location = 2) in vec4 in_color;

out vec4 color;
out vec2 texCoord;

void main()
{
    gl_Position = projection_matrix * vec4(in_position, 0, 1);
    color = in_color;
	texCoord = in_texCoord;
}";
            string fshader_text = @"#version 430 core

uniform sampler2D FontTexture;

in vec4 color;
in vec2 texCoord;

out vec4 outputColor;

void main()
{
    outputColor = color * texture(FontTexture, texCoord);
}";
            vshader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vshader, vshader_text);
            GL.CompileShader(vshader);
            fshader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fshader, fshader_text);
            GL.CompileShader(fshader);

            program = GL.CreateProgram();
            GL.AttachShader(program, vshader);
            GL.AttachShader(program, fshader);

            GL.LinkProgram(program);

            GL.DetachShader(program, vshader);
            GL.DetachShader(program, fshader);

            GL.DeleteShader(vshader);
            GL.DeleteShader(fshader);

            proj_uniform = GL.GetUniformLocation(program, "projection_matrix");
            font_texture_uniform = GL.GetUniformLocation(program, "FontTexture");

            // recreate device texture
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int w, out int h, out int bytes_per_pixel);

            font_tex_id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2d, font_tex_id);
            GL.TexStorage2D(TextureTarget.Texture2d, 1, SizedInternalFormat.Rgba8, w, h);
            GL.TexSubImage2D(TextureTarget.Texture2d, 0, 0, 0, w, h, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            io.Fonts.SetTexID(font_tex_id);
            io.Fonts.ClearTexData();

            // reset pipeline
            GL.BindTexture(TextureTarget.Texture2d, prevTexture2D);
            GL.ActiveTexture((TextureUnit)prevActiveTexture);
            GL.UseProgram(prevProgram);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, prevArrayBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, prevElementArrayBuffer);
            GL.BindVertexArray(prevVAO);
        }

        public void Render()
        {
            ImGui.Render();

            // get previous pipeline
            int prevVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int prevArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);
            int prevProgram = GL.GetInteger(GetPName.CurrentProgram);
            bool prevBlendEnabled = GL.GetBoolean(GetPName.Blend);
            bool prevScissorTestEnabled = GL.GetBoolean(GetPName.ScissorTest);
            int prevBlendEquationRgb = GL.GetInteger(GetPName.BlendEquationRgb);
            int prevBlendEquationAlpha = GL.GetInteger(GetPName.BlendEquationAlpha);
            int prevBlendFuncSrcRgb = GL.GetInteger(GetPName.BlendSrcRgb);
            int prevBlendFuncSrcAlpha = GL.GetInteger(GetPName.BlendSrcAlpha);
            int prevBlendFuncDstRgb = GL.GetInteger(GetPName.BlendDstRgb);
            int prevBlendFuncDstAlpha = GL.GetInteger(GetPName.BlendDstAlpha);
            bool prevCullFaceEnabled = GL.GetBoolean(GetPName.CullFace);
            bool prevDepthTestEnabled = GL.GetBoolean(GetPName.DepthTest);
            int prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
            GL.ActiveTexture(TextureUnit.Texture0);
            int prevTexture2D = GL.GetInteger(GetPName.TextureBinding2d);
            Span<int> prevScissorBox = stackalloc int[4];
            GL.GetInteger(GetPName.ScissorBox, prevScissorBox);

            // bind buffers
            int v_offset = 0;
            int e_offset = 0;
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            // resize buffers if needed
            ImDrawDataPtr draw_data = ImGui.GetDrawData();
            for (int i = 0; i < draw_data.CmdListsCount; i++)
            {
                ImDrawListPtr draw_list = draw_data.CmdLists[i];

                v_offset += draw_list.VtxBuffer.Size;
                e_offset += draw_list.IdxBuffer.Size;
            }
            if (vbo_size < v_offset)
            {
                vbo_size = (int)(v_offset * 1.5f);
                GL.BufferData(BufferTarget.ArrayBuffer, 20 * vbo_size, new IntPtr(0), BufferUsage.DynamicDraw);
            }
            if(ebo_size < e_offset)
            {
                ebo_size = (int)(v_offset * 1.5f);
                GL.BufferData(BufferTarget.ElementArrayBuffer, 2 * ebo_size, new IntPtr(0), BufferUsage.DynamicDraw);
            }

            // buffer data from draw list
            v_offset = 0;
            e_offset = 0;
            for (int i = 0; i < draw_data.CmdListsCount; i++)
            {
                ImDrawListPtr draw_list = draw_data.CmdLists[i];

                GL.BufferSubData(BufferTarget.ArrayBuffer, v_offset * Marshal.SizeOf<ImDrawVert>(), draw_list.VtxBuffer.Size * Marshal.SizeOf<ImDrawVert>(), draw_list.VtxBuffer.Data);
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, e_offset * Marshal.SizeOf<ushort>(), draw_list.IdxBuffer.Size * Marshal.SizeOf<ushort>(), draw_list.IdxBuffer.Data);

                v_offset += draw_list.VtxBuffer.Size;
                e_offset += draw_list.IdxBuffer.Size;
            }

            // setup pipeline
            ImGuiIOPtr io = ImGui.GetIO();
            GL.UseProgram(program);
            GL.UniformMatrix4f(proj_uniform, 1, false, proj_matrix);
            GL.Uniform1i(font_texture_uniform, 0);
            GL.BindVertexArray(vao);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            draw_data.ScaleClipRects(io.DisplayFramebufferScale);

            // render
            for (int n = 0; n < draw_data.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdLists[n];

                for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2d, (int)pcmd.TextureId);

                        // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, height - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

                        if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                        {
                            GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(pcmd.IdxOffset * sizeof(ushort)), unchecked((int)pcmd.VtxOffset));
                        }
                        else
                        {
                            GL.DrawElements(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                        }
                    }
                }
            }

            // reset pipeline
            GL.BindTexture(TextureTarget.Texture2d, prevTexture2D);
            GL.ActiveTexture((TextureUnit)prevActiveTexture);
            GL.UseProgram(prevProgram);
            GL.BindVertexArray(prevVAO);
            GL.Scissor(prevScissorBox[0], prevScissorBox[1], prevScissorBox[2], prevScissorBox[3]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, prevArrayBuffer);
            GL.BlendEquationSeparate((BlendEquationMode)prevBlendEquationRgb, (BlendEquationMode)prevBlendEquationAlpha);
            GL.BlendFuncSeparate(
                (BlendingFactor)prevBlendFuncSrcRgb,
                (BlendingFactor)prevBlendFuncDstRgb,
                (BlendingFactor)prevBlendFuncSrcAlpha,
                (BlendingFactor)prevBlendFuncDstAlpha);
            if (prevBlendEnabled) GL.Enable(EnableCap.Blend); else GL.Disable(EnableCap.Blend);
            if (prevDepthTestEnabled) GL.Enable(EnableCap.DepthTest); else GL.Disable(EnableCap.DepthTest);
            if (prevCullFaceEnabled) GL.Enable(EnableCap.CullFace); else GL.Disable(EnableCap.CullFace);
            if (prevScissorTestEnabled) GL.Enable(EnableCap.ScissorTest); else GL.Disable(EnableCap.ScissorTest);

            ImGui.NewFrame();
        }

        public void UpdateDeltaTime(float dt)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.DeltaTime = dt;
        }

        public void UpdateMousePos(Vector2 pos)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.AddMousePosEvent(pos.X, pos.Y);
        }

        public void UpdateMouseState(MouseButton button, bool is_down)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.AddMouseButtonEvent((int)button, is_down);
        }

        public void UpdateMouseWheel(Vector2 delta)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.AddMouseWheelEvent(delta.X, delta.Y);
        }

        public void AddCharPress(char c)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.AddInputCharacter(c);
        }

        public void UpdateKeyState(Key key, bool is_down)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            if (KeyRemap(key, out ImGuiKey res))
            {
                io.AddKeyEvent(res, is_down);
            }
        }

        private bool KeyRemap(Key key, out ImGuiKey result)
        {
            ImGuiKey KeyToImGuiKeyShortcut(Key keyToConvert, Key startKey1, ImGuiKey startKey2)
            {
                int changeFromStart1 = (int)keyToConvert - (int)startKey1;
                return startKey2 + changeFromStart1;
            }

            result = key switch
            {
                >= Key.F1 and <= Key.F24 => KeyToImGuiKeyShortcut(key, Key.F1, ImGuiKey.F1),
                >= Key.Keypad0 and <= Key.Keypad9 => KeyToImGuiKeyShortcut(key, Key.Keypad0, ImGuiKey.Keypad0),
                >= Key.A and <= Key.Z => KeyToImGuiKeyShortcut(key, Key.A, ImGuiKey.A),
                >= Key.D0 and <= Key.D9 => KeyToImGuiKeyShortcut(key, Key.D0, ImGuiKey._0),
                Key.LeftShift or Key.RightShift => ImGuiKey.ModShift,
                Key.LeftControl or Key.RightControl => ImGuiKey.ModCtrl,
                Key.LeftAlt or Key.RightAlt => ImGuiKey.ModAlt,
                Key.LeftGUI or Key.RightGUI => ImGuiKey.ModSuper,
                Key.Application => ImGuiKey.Menu,
                Key.UpArrow => ImGuiKey.UpArrow,
                Key.DownArrow => ImGuiKey.DownArrow,
                Key.LeftArrow => ImGuiKey.LeftArrow,
                Key.RightArrow => ImGuiKey.RightArrow,
                Key.Return => ImGuiKey.Enter,
                Key.Escape => ImGuiKey.Escape,
                Key.Space => ImGuiKey.Space,
                Key.Tab => ImGuiKey.Tab,
                Key.Backspace => ImGuiKey.Backspace,
                Key.Insert => ImGuiKey.Insert,
                Key.Delete => ImGuiKey.Delete,
                Key.PageUp => ImGuiKey.PageUp,
                Key.PageDown => ImGuiKey.PageDown,
                Key.Home => ImGuiKey.Home,
                Key.End => ImGuiKey.End,
                Key.CapsLock => ImGuiKey.CapsLock,
                Key.ScrollLock => ImGuiKey.ScrollLock,
                Key.PrintScreen => ImGuiKey.PrintScreen,
                Key.PauseBreak => ImGuiKey.Pause,
                Key.NumLock => ImGuiKey.NumLock,
                Key.KeypadDivide => ImGuiKey.KeypadDivide,
                Key.KeypadMultiply => ImGuiKey.KeypadMultiply,
                Key.KeypadSubtract => ImGuiKey.KeypadSubtract,
                Key.KeypadAdd => ImGuiKey.KeypadAdd,
                Key.KeypadDecimal => ImGuiKey.KeypadDecimal,
                Key.KeypadEnter => ImGuiKey.KeypadEnter,
                Key.OEM3 => ImGuiKey.GraveAccent,
                Key.Minus => ImGuiKey.Minus,
                Key.Plus => ImGuiKey.Equal,
                Key.OEM4 => ImGuiKey.LeftBracket,
                Key.OEM6 => ImGuiKey.RightBracket,
                Key.OEM1 => ImGuiKey.Semicolon,
                Key.OEM7 => ImGuiKey.Apostrophe,
                Key.Comma => ImGuiKey.Comma,
                Key.Period => ImGuiKey.Period,
                Key.OEM2 => ImGuiKey.Slash,
                Key.OEM102 => ImGuiKey.Backslash,
                _ => ImGuiKey.None
            };

            return result != ImGuiKey.None;
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteProgram(program);
            GL.DeleteTexture(font_tex_id);
        }
    }
}
