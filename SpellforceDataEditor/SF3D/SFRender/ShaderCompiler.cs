/*
 * Shader compiler helper class
 * More at http://www.opengl-tutorial.org/beginners-tutorials/tutorial-2-the-first-triangle/
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SF3D.SFRender
{
    static public class ShaderCompiler
    {
        static public int Compile(string vertex_shader, string fragment_shader)
        {
            int VertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            int FragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

            //read vertex shader
            string VertexShaderCode = vertex_shader;
            //read fragment shader
            string FragmentShaderCode = fragment_shader;

            int result = 0;
            int InfoLogLength;

            System.Diagnostics.Debug.WriteLine("Compiling " + vertex_shader);
            GL.ShaderSource(VertexShaderID, VertexShaderCode);
            GL.CompileShader(VertexShaderID);
            string info = GL.GetShaderInfoLog(VertexShaderID);
            System.Diagnostics.Debug.WriteLine(info);

            System.Diagnostics.Debug.WriteLine("Compiling " + fragment_shader);
            GL.ShaderSource(FragmentShaderID, FragmentShaderCode);
            GL.CompileShader(FragmentShaderID);
            info = GL.GetShaderInfoLog(FragmentShaderID);
            System.Diagnostics.Debug.WriteLine(info);

            System.Diagnostics.Debug.WriteLine("LINKING PROGRAM");
            int programID = GL.CreateProgram();
            GL.AttachShader(programID, VertexShaderID);
            GL.AttachShader(programID, FragmentShaderID);
            GL.LinkProgram(programID);

            info = GL.GetProgramInfoLog(programID);
            System.Diagnostics.Debug.WriteLine(info);

            GL.DetachShader(programID, VertexShaderID);
            GL.DetachShader(programID, FragmentShaderID);

            GL.DeleteShader(VertexShaderID);
            GL.DeleteShader(FragmentShaderID);
            
            return programID;
        }
    }
}
