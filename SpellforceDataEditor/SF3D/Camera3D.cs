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
        Vector3 lookat = new Vector3(0, 0, 1);

        public Camera3D()
        {
            update_modelMatrix();
        }

        new public void update_modelMatrix()
        {
            modelMatrix = Matrix4.LookAt(Position, lookat, new Vector3(0, 1, 0));
            System.Diagnostics.Debug.WriteLine("LOOKAT"+lookat.X.ToString()+" "+lookat.Y.ToString()+" "+lookat.Z.ToString());
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
