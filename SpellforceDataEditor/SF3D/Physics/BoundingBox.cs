/*
 * (axis-aligned) BoundingBox is a minimal in volume axis-algned box a given geometric shape can fit into
 * Operations for checking if a point lies inside the box, if a plane intersects the box,
 * and if the box is outside of a convex hull are provided
 * assumes upvector = (0, 0, 1)
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
        public Vector3 center;

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
                if (outside == 255)       // equivalent to "each vertex is outside of at least one plane of the convex hull"
                    return true;
            }
            return false;
        }

        // limit on point coordinates is (-100000, 100000)
        public static BoundingBox FromPoints(Vector3[] vs)
        {
            float xmin, xmax, ymin, ymax, zmin, zmax;
            xmin = ymin = zmin = 99999;
            xmax = ymax = zmax = -99999;

            for (int i = 0; i < vs.Length; i++)
            {
                if (xmin > vs[i].X)
                    xmin = vs[i].X;
                if (xmax < vs[i].X)
                    xmax = vs[i].X;
                if (ymin > vs[i].Y)
                    ymin = vs[i].Y;
                if (ymax < vs[i].Y)
                    ymax = vs[i].Y;
                if (zmin > vs[i].Z)
                    zmin = vs[i].Z;
                if (zmax < vs[i].Z)
                    zmax = vs[i].Z;
            }

            return new BoundingBox(new Vector3(xmin, ymin, zmin), new Vector3(xmax, ymax, zmax));
        }

        // rotate bounding box along XY plane and return new rotated box
        public BoundingBox RotatedByAzimuthAltitude(float azimuth, float altitude)
        {
            azimuth *= (float)(Math.PI / 180);
            altitude *= (float)(Math.PI / 180);
            // rotate all 8 points along the respective XY planes by azimuth, and create new bounding box from min and max of those points
            Vector3[] vs = new Vector3[8];
            for (int i = 0; i < 8; i++)
                vs[i] = vertices[i];

            MathUtils.RotateVec3Array(vs, center, azimuth, altitude);

            BoundingBox bb = BoundingBox.FromPoints(vs);
            return this.Union(bb);
        }

        public BoundingBox Intersection(BoundingBox _aabb)
        {
            float dxmin, dxmax, dymin, dymax, dzmin, dzmax, dx, dy, dz;

            dxmin = Math.Max(a.X, _aabb.a.X);
            dxmax = Math.Min(b.X, _aabb.b.X);
            dymin = Math.Max(a.Y, _aabb.a.Y);
            dymax = Math.Min(b.Y, _aabb.b.Y);
            dzmin = Math.Max(a.Z, _aabb.a.Z);
            dzmax = Math.Min(b.Z, _aabb.b.Z);

            dx = Math.Max(0, dxmax - dxmin);
            dy = Math.Max(0, dymax - dymin);
            dz = Math.Max(0, dzmax - dzmin);

            return new BoundingBox(new Vector3(dxmin, dymin, dzmin), new Vector3(dxmin + dx, dymin + dy, dzmin + dz));
        }

        public BoundingBox Union(BoundingBox _aabb)
        {
            float dxmin, dxmax, dymin, dymax, dzmin, dzmax;

            dxmin = Math.Min(a.X, _aabb.a.X);
            dxmax = Math.Max(b.X, _aabb.b.X);
            dymin = Math.Min(a.Y, _aabb.a.Y);
            dymax = Math.Max(b.Y, _aabb.b.Y);
            dzmin = Math.Min(a.Z, _aabb.a.Z);
            dzmax = Math.Max(b.Z, _aabb.b.Z);

            return new BoundingBox(new Vector3(dxmin, dymin, dzmin), new Vector3(dxmax, dymax, dzmax));
        }

        public void CropMinY(float minz)
        {
            a.Y = minz;
            vertices[0] = new Vector3(a);
            vertices[1] = new Vector3(a.X, a.Y, b.Z);
            vertices[4] = new Vector3(b.X, a.Y, a.Z);
            vertices[5] = new Vector3(b.X, a.Y, b.Z);

            center = (a + b) / 2;
        }

        public double GetVolume()
        {
            return (b.X - a.X) * (b.Y - a.Y) * (b.Z - a.Z);
        }
    }
}
