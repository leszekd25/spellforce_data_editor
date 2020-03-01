/*
 * (axis-aligned) BoundingBox is a minimal in volume axis-algned box a given geometric shape can fit into
 * Operations for checking if a point lies inside the box, if a plane intersects the box,
 * and if the box is outside of a convex hull are provided
 * */

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
        public Vector3 a;             // lesser of X, Y and Z coordinates are stored here
        public Vector3 b;             // greater of X, Y and Z coordinates are stored here
        private Vector3[] vertices;
        public Vector3 center { get; private set; }

        // automatically sets a and b to fit the definition
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

            center = (a + b) / 2;
        }

        public static BoundingBox operator +(BoundingBox ab, Vector3 c)
        {
            return new BoundingBox(ab.a + c, ab.b + c);
        }

        public static BoundingBox operator -(BoundingBox ab, Vector3 c)
        {
            return new BoundingBox(ab.a - c, ab.b - c);
        }

        public static BoundingBox operator+(BoundingBox ab1, BoundingBox ab2)
        {
            return new BoundingBox(
                new Vector3(
                    Math.Min(ab1.a.X, ab2.a.X),
                    Math.Min(ab1.a.Y, ab2.a.Y),
                    Math.Min(ab1.a.Z, ab2.a.Z)),
                new Vector3(
                    Math.Max(ab1.b.X, ab2.b.X),
                    Math.Max(ab1.b.Y, ab2.b.Y),
                    Math.Max(ab1.b.Z, ab2.b.Z)));
        }

        public BoundingBox OffsetBy(Vector3 c)
        {
            return this + (c - center);
        }

        // returns manhattan distance between a point and the box
        // if point is inside of the box, returned value is less than 0
        public float DistanceIsotropic(Vector3 p)
        {
            float dx = Math.Max(a.X - p.X, p.X - b.X);
            float dy = Math.Max(a.Y - p.Y, p.Y - b.Y);
            float dz = Math.Max(a.Z - p.Z, p.Z - b.Z);
            return Math.Max(dx, Math.Max(dy, dz));
        }

        // returns -1 if the box is outside of the plane, 1 if inside, 0 if plane intersects the box
        public int IntersectPlane(Plane pl)
        {
            bool is_outside = pl.SideOf(a);
            bool side_changed = is_outside;
            bool is_intersecting = false;

            is_outside = pl.SideOf(new Vector3(b.X, a.Y, a.Z));  is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(b.X, b.Y, a.Z));  is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(b.X, a.Y, b.Z));  is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(b.X, b.Y, b.Z));  is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(a.X, b.Y, a.Z));  is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(a.X, a.Y, b.Z));  is_intersecting |= is_outside ^ side_changed;
            is_outside = pl.SideOf(new Vector3(a.X, b.Y, b.Z));  is_intersecting |= is_outside ^ side_changed;

            return is_intersecting ? 0 : (is_outside ? 1 : -1);
        }

        // returns true if the box is not contained within the convex hull described by a set of bounding planes
        // useful for checking if a bounding box is visible in camera frustum (which is just that, a convex hull)
        public bool IsOutsideOfConvexHull(Plane[] planes)
        {
            byte outside = 0;

            foreach(Plane pl in planes)
            {
                for (int i = 0; i < 8; i++)
                {
                    byte vertex_test_result = (byte)((pl.SideOf(vertices[i]) ? 1 : 0) << i);
                    if (vertex_test_result == 0)
                        break;
                    outside |= vertex_test_result;
                }
                if (outside == 255)
                    return true;
            }
            return false;
        }
    }
}
