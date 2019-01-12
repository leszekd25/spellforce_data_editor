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
        public int ProgramID { get; private set; } = -1;            // shader id as assigned by OpenGL
        private Dictionary<string, int> parameters = new Dictionary<string, int>();   // shader parameters IDs

        // returns shader parameter ID given parameter name
        public int this[string name]
        {
            get { return parameters[name]; }
        }

        // compiles shader (typically only called once)
        public void CompileShader(string vshader, string fshader)
        {
            ProgramID = ShaderCompiler.Compile(vshader, fshader);
        }

        // adds shader parameter
        public void AddParameter(string name)
        {
            parameters[name] = GL.GetUniformLocation(ProgramID, name);
        }
    }
}
