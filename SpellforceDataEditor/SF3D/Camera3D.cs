/*
 * Camera3D inherits from Object3D, and is simply a view matrix and a few functions to manipulate openGL view
 * It is used explicitly in render engine
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SF3D
{
    public class Camera3D: Object3D
    {
        public Vector3 lookat { get; private set; } = new Vector3(0, 0, 1);

        public Camera3D()
        {
            update_modelMatrix();
        }

        new public void update_modelMatrix()
        {
            modelMatrix = Matrix4.LookAt(Position, lookat, new Vector3(0, 1, 0));
            modified = false;
        }

        public void look_at(Vector3 to)
        {
            lookat = to;

            modified = true;
        }

        public void translate(Vector3 tr)
        {
            position += tr;
            lookat += tr;
            modified = true;
        }
    }
}
