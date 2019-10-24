/*
 * Triangle is the simplest two-dimensional bounded shape
 * Operations for retrieving triangle area and checking is a point lies inside of the triangle are provided
 * */

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
        public Vector3 v12, v13;    // cached for calculations
        float d00, d01, d11;        // cached cor collision calc
        float denom;                // cached for collision calc
        public Vector3 normal;
        public float area2;

        public Triangle(Vector3 u1, Vector3 u2, Vector3 u3)
        {
            v1 = u1; v2 = u2; v3 = u3;
            v12 = Vector3.Subtract(v2, v1); v13 = Vector3.Subtract(v3, v1);
            d00 = Vector3.Dot(v12, v12);
            d01 = Vector3.Dot(v12, v13);
            d11 = Vector3.Dot(v13, v13);
            denom = 1.0f / (d00 * d11 - d01 * d01);
            Vector3 cross = Vector3.Cross(v13, v12);
            normal = cross.Normalized();
            area2 = cross.LengthSquared / 4;
            if (normal == Vector3.Zero)
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "Triangle(): Malformed triangle, normal is zero length!");
        }

        public float GetArea()
        {
            return (float)Math.Sqrt(area2);
        }

        // returns true if a point lies inside of the triangle
        //  assuming p belongs to the same plane as triangle
        public bool ContainsPoint(Vector3 p)
        {
            Vector3 v = Vector3.Subtract(p, v1);
            float d20 = Vector3.Dot(v, v12);
            float d21 = Vector3.Dot(v, v13);
            float alpha = (d11 * d20 - d01 * d21) * denom;
            float beta = (d00 * d21 - d01 * d20) * denom;
            //float gamma = 1.0f - alpha - beta;
            return ((alpha >= 0) && (alpha <= 1)
                    && (beta >= 0) && (beta <= 1)
					&& (alpha + beta <= 1));   // <- a small improvement
                    //&& (gamma >= 0) && (gamma <= 1));
        }
    }
}
