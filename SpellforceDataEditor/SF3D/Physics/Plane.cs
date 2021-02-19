/*
 * Plane describes a 2D plane in a 3D space using a point and a normal to the plane
 * Normal also describes sides of the plane, which is important for certain operations
 * Operations for calculating distance from a point to the plane and determining the side a point is on in relation to the plane
 * are provided
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SF3D.Physics
{
    // plane is described using a point belonging to it and a normal vector to the plane
    public class Plane
    {
        public Vector3 point;
        public Vector3 normal;
        public float d;

        public Plane(Vector3 p, Vector3 n)
        {
            point = p;
            normal = n.Normalized();
            d = -normal.X * point.X - normal.Y * point.Y - normal.Z * point.Z;
            if (normal == Vector3.Zero)
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "Plane(): Normal is zero length (point: "+p.ToString()+")");
        }

        // creates a plane to which a given triangle belongs
        public Plane(Triangle t)
        {
            point = t.v1;
            normal = t.normal;
            d = -normal.X * point.X - normal.Y * point.Y - normal.Z * point.Z;
        }

        // creates a plane  to which given  3  points  belong
        public Plane(Vector3 p1, Vector3 p2, Vector3  p3)
        {
            point = p1;
            normal = Vector3.Cross(p3 - p1, p2 - p1).Normalized();
            d = -normal.X * point.X - normal.Y * point.Y - normal.Z * point.Z;

            if (normal == Vector3.Zero)
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "Plane(): Malformed plane, normal is zero length!");
        }

        // returns distance from a given point to the plane
        // depending on the side of the plane the point is on, result will be positive or negative
        public float DistanceTo(Vector3 v)
        {
            return (normal.X * v.X + normal.Y * v.Y + normal.Z * v.Z + d);
        }

        // returns true if a point lies on positive side of the plane, false otherwise
        public bool SideOf(Vector3 v)
        {
            return (normal.X * v.X + normal.Y * v.Y  + normal.Z * v.Z + d) > 0;
        }
    }
}
