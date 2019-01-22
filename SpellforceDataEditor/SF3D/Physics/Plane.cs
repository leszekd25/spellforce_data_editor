using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SF3D.Physics
{
    // plane is described using a point belonging to it and a normal of the plane
    public class Plane
    {
        public Vector3 point;
        public Vector3 normal;

        public Plane(Triangle t)
        {
            point = t.v1;
            normal = t.normal;
        }

        public float DistanceTo(Vector3 v)
        {
            float d = -normal.X * point.X - normal.Y * point.Y - normal.Z * point.Z;
            return Math.Abs(normal.X * v.X + normal.Y * v.Y + normal.Z * v.Z + d)
                 / (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
        }
    }
}
