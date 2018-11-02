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
        public int ProgramID { get; private set; } = -1;
        private Dictionary<string, int> parameters = new Dictionary<string, int>();

        public int this[string name]
        {
            get { return parameters[name]; }
        }

        public void CompileShader(string vshader, string fshader)
        {
            ProgramID = ShaderCompiler.Compile(vshader, fshader);
        }

        public void AddParameter(string name)
        {
            parameters[name] = GL.GetUniformLocation(ProgramID, name);
        }
    }
}
