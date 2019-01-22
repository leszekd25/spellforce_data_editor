using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SF3D.Physics
{
    public class BoundingBox
    {
        public Vector3 a;
        public Vector3 b;

        // makes sure that a is minimum and b is maximum
        public BoundingBox(Vector3 _a, Vector3 _b)
        {
            a = _a;
            b = _b;
            float tmp;
            if (a.X > b.X)
            {
                tmp = a.X;
                a.X = b.X;
                b.X = tmp;
            }
            if (a.Y > b.Y)
            {
                tmp = a.Y;
                a.Y = b.Y;
                b.Y = tmp;
            }
            if (a.Z > b.Z)
            {
                tmp = a.Z;
                a.Z = b.Z;
                b.Z = tmp;
            }
        }

        public static BoundingBox operator +(BoundingBox ab, Vector3 c)
        {
            return new BoundingBox(ab.a + c, ab.b + c);
        }

        public static BoundingBox operator -(BoundingBox ab, Vector3 c)
        {
            return new BoundingBox(ab.a - c, ab.b - c);
        }

        // negative if bb contains p
        public float DistanceIsotropic(Vector3 p)
        {
            float dx = Math.Max(a.X - p.X, p.X - b.X);
            float dy = Math.Max(a.Y - p.Y, p.Y - b.Y);
            float dz = Math.Max(a.Z - p.Z, p.Z - b.Z);
            return Math.Max(dx, Math.Max(dy, dz));
        }
    }
}
