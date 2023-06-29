/*
 * (axis-aligned) BoundingBox is a minimal in volume axis-algned box a given geometric shape can fit into
 * Operations for checking if a point lies inside the box, if a plane intersects the box,
 * and if the box is outside of a convex hull are provided
 * assumes upvector = (0, 0, 1)
 * */

using OpenTK;
using System;

namespace SFEngine.SF3D.Physics
{
    public struct BoundingBox
    {
        public Vector3 a;             // lesser of X, Y and Z coordinates are stored here
        public Vector3 b;             // greater of X, Y and Z coordinates are stored here
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

        public static BoundingBox operator +(BoundingBox ab1, BoundingBox ab2)
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

        public bool IsOutsideOfFrustum(Frustum frustum)
        {
            for(int i = 0; i < 6; i++)
            {
                Plane pl = frustum.frustum_planes[i];
                Vector3 v_near = a;

                if (pl.normal.X < 0)
                {
                    v_near.X = b.X;
                }

                if (pl.normal.Y < 0)
                {
                    v_near.Y = b.Y;
                }

                if (pl.normal.Z < 0)
                {
                    v_near.Z = b.Z;
                }

                // bounding box is outside this plane - it's outside of all frustum planes
                if (pl.DistanceTo(v_near) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // limit on point coordinates is (-100000, 100000)
        public static BoundingBox FromPoints(Vector3[] vs)
        {
            Vector3 aa, bb;
            aa = bb = vs[0];

            for (int i = 1; i < vs.Length; i++)
            {
                MathUtils.Expand(vs[i].X, ref aa.X, ref bb.X);
                MathUtils.Expand(vs[i].Y, ref aa.Y, ref bb.Y);
                MathUtils.Expand(vs[i].Z, ref aa.Z, ref bb.Z);
            }

            return new BoundingBox(aa, bb);
        }

        // rotate bounding box along XY plane and return new rotated box
        public BoundingBox RotatedByAzimuthAltitude(float azimuth, float altitude)
        {
            azimuth *= (float)(Math.PI / 180);
            altitude *= (float)(Math.PI / 180);
            // rotate all 8 points along the respective XY planes by azimuth, and create new bounding box from min and max of those points
            Vector3[] vs = new Vector3[8];
            vs[0] = new Vector3(a);
            vs[1] = new Vector3(a.X, a.Y, b.Z); vs[2] = new Vector3(a.X, b.Y, a.Z); vs[3] = new Vector3(a.X, b.Y, b.Z);
            vs[4] = new Vector3(b.X, a.Y, a.Z); vs[5] = new Vector3(b.X, a.Y, b.Z); vs[6] = new Vector3(b.X, b.Y, a.Z);
            vs[7] = new Vector3(b);

            MathUtils.RotateVec3Array(vs, center, azimuth, altitude);

            BoundingBox bb = BoundingBox.FromPoints(vs);
            return Union(bb);
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

        public static bool operator ==(BoundingBox b1, BoundingBox b2)
        {
            return (b1.a == b2.a) && (b1.b == b2.b);
        }

        public static bool operator !=(BoundingBox b1, BoundingBox b2)
        {
            return (b1.a != b2.a) || (b1.b != b2.b);
        }


        public static BoundingBox Zero = new BoundingBox(Vector3.Zero, Vector3.Zero);
    }
}
