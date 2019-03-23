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
        private Vector3[] vertices;

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

            vertices = new Vector3[8];
            vertices[0] = new Vector3(a);
            vertices[1] = new Vector3(a.X, a.Y, b.Z); vertices[2] = new Vector3(a.X, b.Y, a.Z); vertices[3] = new Vector3(a.X, b.Y, b.Z);
            vertices[4] = new Vector3(b.X, a.Y, a.Z); vertices[5] = new Vector3(b.X, a.Y, b.Z); vertices[6] = new Vector3(b.X, b.Y, a.Z);
            vertices[7] = new Vector3(b);
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

        // on which side of the plane this aabb is (1 if outside, -1 if inside, 0 if intersects)
        public int IntersectPlane(Plane pl)
        {
            bool is_outside = pl.SideOf(a);
            bool side_changed = is_outside;
            bool is_intersecting = false;

            is_outside = pl.SideOf(new Vector3(b.X, a.Y, a.Z)); is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(b.X, b.Y, a.Z)); is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(b.X, a.Y, b.Z)); is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(b.X, b.Y, b.Z)); is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(a.X, b.Y, a.Z)); is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(a.X, a.Y, b.Z)); is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(a.X, b.Y, b.Z)); is_intersecting |= is_outside ^ side_changed;

            return is_intersecting ? 0 : (is_outside ? 1 : -1);
        }

        // does aabb intersect a convex hull
        public bool IsOutsideOfConvexHull(Plane[] planes)
        {
            byte outside = 0;

            foreach(Plane pl in planes)
            {
                for (int i = 0; i < 8; i++)
                {
                    byte vertex_test_result = (byte)((pl.SideOf(vertices[i]) ? 1 : 0) << i);    // is this branch really necessary?
                    outside |= vertex_test_result;
                }
                if (outside == 255)
                    return true;
            }
            return false;


        }
    }
}
