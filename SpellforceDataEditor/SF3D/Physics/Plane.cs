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

        public Plane(Vector3 p, Vector3 n)
        {
            point = p;
            normal = n;
        }

        public Plane(Triangle t)
        {
            point = t.v1;
            normal = t.normal;
        }

        // NOTE: positive result is outside of the plane, negative result is inside of the plane
        public float DistanceTo(Vector3 v)
        {
            float d = -normal.X * point.X - normal.Y * point.Y - normal.Z * point.Z;
            return (normal.X * v.X + normal.Y * v.Y + normal.Z * v.Z + d)
                 / (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
        }

        public bool SideOf(Vector3 v)
        {
            float d = -normal.X * point.X - normal.Y * point.Y - normal.Z * point.Z;
            return (normal.X * v.X + normal.Y * v.Y  + normal.Z * v.Z + d) > 0;
        }
    }
}
