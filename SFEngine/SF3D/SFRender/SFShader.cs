/*
 * SFShader is a structure which takes care of shader parameters and shader compilation
 */

using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace SFEngine.SF3D.SFRender
{
    public struct ShaderInfo
    {
        public ShaderType type;
        public string data;
    }

    public class SFShader
    {
        public int ProgramID { get; private set; } = Utility.NO_INDEX;            // shader id as assigned by OpenGL
        private Dictionary<string, int> parameters = new Dictionary<string, int>();   // shader parameters IDs
        private HashSet<string> defines = new HashSet<string>();                  // shader defines

        // returns shader parameter ID given parameter name
        public int this[string name]
        {
            get { return parameters[name]; }
        }

        // compiles shader (typically only called once)
        // call AFTER you set up defines
        public void CompileShader(ShaderInfo[] shaders)
        {
            // delete existing program and clear program data
            parameters.Clear();
            if (ProgramID != Utility.NO_INDEX)
            {
                GL.DeleteProgram(ProgramID);
            }

            // generate define preamble
            string define_preamble = "#version 420\r\n";
            foreach (string s in defines)
            {
                define_preamble += "#define " + s + " 1\r\n";
            }

            // generate shaders and compile them
            int[] shader_ids = new int[shaders.Length];
            string info;

            for (int i = 0; i < shader_ids.Length; i++)
            {
                shader_ids[i] = GL.CreateShader(shaders[i].type);
                GL.ShaderSource(shader_ids[i], define_preamble + shaders[i].data);

                GL.CompileShader(shader_ids[i]);
                info = GL.GetShaderInfoLog(shader_ids[i]);
                System.Diagnostics.Debug.WriteLine(shaders[i].type.ToString() + " compile info: " + info);
            }

            // generate program
            ProgramID = GL.CreateProgram();
            System.Diagnostics.Debug.WriteLine("ShaderCompiler.Compile(): Creating shader id " + ProgramID.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "ShaderCompiler.Compile(): Creating shader id " + ProgramID.ToString());

            // link program
            for (int i = 0; i < shader_ids.Length; i++)
            {
                GL.AttachShader(ProgramID, shader_ids[i]);
            }

            GL.LinkProgram(ProgramID);
            info = GL.GetProgramInfoLog(ProgramID);
            System.Diagnostics.Debug.WriteLine("Link info: " + info);

            for (int i = 0; i < shader_ids.Length; i++)
            {
                GL.DetachShader(ProgramID, shader_ids[i]);
            }

            for (int i = 0; i < shader_ids.Length; i++)
            {
                GL.DeleteShader(shader_ids[i]);
            }

            // done
        }

        // adds shader parameter (one that exists as a uniform variable in shader code)
        public void AddParameter(string name)
        {
            parameters[name] = GL.GetUniformLocation(ProgramID, name);
        }

        // DEBUG FUNCTION
        public void ExtractBinary(string path)
        {
            byte[] data = new byte[100000];
            int ret_len = 0;
            BinaryFormat ret_fmt = (BinaryFormat)0;
            GL.GetProgramBinary(ProgramID, 100000, out ret_len, out ret_fmt, data);
            byte[] data2 = new byte[ret_len];
            System.Array.Copy(data, data2, ret_len);
            System.IO.File.WriteAllBytes(path, data2);
        }

        // sets shader #define constant for conditional compilation
        public void SetDefine(string name, bool exists)
        {
            if (defines.Contains(name))
            {
                if (!exists)
                {
                    defines.Remove(name);
                }
            }
            else
            {
                if (exists)
                {
                    defines.Add(name);
                }
            }
        }
    }
}
