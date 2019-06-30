using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SF3D.Physics
{
    // a triangle consists of three vertices, and has a normal defined as a normal of the plane this triangle belongs to
    public class Triangle
    {
        public Vector3 v1, v2, v3;
        public Vector3 normal { get; private set; }

        public Triangle(Vector3 u1, Vector3 u2, Vector3 u3)
        {
            v1 = u1; v2 = u2; v3 = u3;
            normal = Vector3.Cross(v3 - v1, v2 - v1).Normalized();
            if (normal == Vector3.Zero)
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "Triangle(): Malformed triangle, normal is zero length!");
        }

        public float GetArea()
        {
            return (Vector3.Cross(v2 - v1, v3 - v1).Length) / 2;
        }

        public static float GetArea(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return (Vector3.Cross(v2 - v1, v3 - v1).Length) / 2;
        }
    }
}
