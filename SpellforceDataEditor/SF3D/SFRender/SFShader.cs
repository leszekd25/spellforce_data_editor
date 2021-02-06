/*
 * SFShader is a structure which takes care of shader parameters and shader compilation
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SF3D.SFRender
{
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
        public void CompileShader(string vshader, string fshader)
        {
            parameters.Clear();
            if (ProgramID != Utility.NO_INDEX)
                GL.DeleteProgram(ProgramID);
            ProgramID = ShaderCompiler.Compile(defines, vshader, fshader);
        }

        // adds shader parameter (one that exists as a uniform variable in shader code)
        public void AddParameter(string name)
        {
            parameters[name] = GL.GetUniformLocation(ProgramID, name);
        }

        // sets shader #define constant for conditional compilation
        public void SetDefine(string name, bool exists)
        {
            if (defines.Contains(name))
            {
                if (!exists)
                    defines.Remove(name);
            }
            else
            {
                if (exists)
                    defines.Add(name);
            }
        }
    }
}
