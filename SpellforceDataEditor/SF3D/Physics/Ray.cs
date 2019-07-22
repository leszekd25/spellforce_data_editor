using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SF3D.Physics
{
    // ray consists of a starting point and a direction vector
    public class Ray
    {
        public Vector3 start;
        public Vector3 vector;
        public Vector3 nvector;    // vector but normalized
        public float length;

        public Ray(Vector3 s, Vector3 v)
        {
            start = s;
            vector = v;
            length = v.Length;
            nvector = v.Normalized();
        }

        public bool Intersect(Plane pl, out Vector3 point)
        {
            float ln_prod = Vector3.Dot(vector, pl.normal);
            if (ln_prod != 0)
            {
                // intersection of ray and plane the triangle belongs to
                float ray_d = Vector3.Dot(pl.point - start, pl.normal) / ln_prod;
                point = new Vector3(ray_d * vector + start);
                if ((point - start).Length > length)
                    return false;
                return true;
            }
            else
            {
                point = new Vector3(0, 0, 0);
                return false;
            }
        }
        
        public bool Intersect(Triangle tr, out Vector3 point)
        {
            float ln_prod = Vector3.Dot(vector, tr.normal);
            if(ln_prod != 0)
            {
                // intersection of ray and plane the triangle belongs to
                float ray_d = Vector3.Dot(Vector3.Subtract(tr.v1, start), tr.normal) / ln_prod;
                point = Vector3.Add(ray_d * vector, start);
                if (Vector3.Subtract(point, start).Length > length)
                    return false;
                // check if point is in triangle
                // barycentric coordinates https://math.stackexchange.com/questions/4322/check-whether-a-point-is-within-a-3d-triangle
                /*Line l1 = new Line(tr.v2, tr.v3);
                Line l2 = new Line(tr.v3, tr.v1);
                float alpha = (float)Math.Sqrt(l1.Distance2(point) / l1.Distance2(tr.v1));
                float beta = (float)Math.Sqrt(l2.Distance2(point) / l2.Distance2(tr.v2));*/

                /*float area = tr.area2;
                float alpha = (float)Math.Sqrt(Triangle.GetArea2(point, tr.v2, tr.v3) / area);
                float beta = (float)Math.Sqrt(Triangle.GetArea2(point, tr.v3, tr.v1) / area);*/
                return tr.ContainsPoint(point);

                /*float gamma = 1 - alpha - beta;
                return ((alpha >= 0) && (alpha <= 1)
                    && (beta >= 0) && (beta <= 1)
                    && (gamma >= 0) && (gamma <= 1));*/
            }
            else
            {
                point = new Vector3(0, 0, 0);
                return false;
            }
        }

        public bool Intersect(BoundingBox ab)
        {
            // binary sampling of distance to the box
            float current_start = 0;
            float current_end = length;
            float current_center;
            Vector3 current_point;
            float dist;
            while(current_end - current_start >= 0.1f)
            {
                current_center = (current_start + current_end) / 2;    //care about overflow
                current_point = start + nvector * current_center;
                dist = ab.DistanceIsotropic(current_point);
                if (dist <= 0.1f)
                    return true;
                if (ab.DistanceIsotropic(start + nvector * current_start) < ab.DistanceIsotropic(start + nvector * current_end))
                    current_end = current_center;
                else
                    current_start = current_center;
            }
            return false;
        }

        public bool Intersect(SFModel3D mesh, Vector3 offset, out Vector3 point)
        {
            point = new Vector3(0, 0, 0);
            if (mesh == null)
                return false;

            BoundingBox ab = mesh.aabb + offset;
            if (!Intersect(ab))
                return false;

            Ray r = this - offset;
            int triangle_count = mesh.face_indices.Length / 3;
            for(int i = 0; i < triangle_count; i++)
            {
                // todo: use precalculated triangle normals
                Triangle t = new Triangle(mesh.vertices[mesh.face_indices[i * 3 + 2]],
                    mesh.vertices[mesh.face_indices[i * 3 + 0]],
                    mesh.vertices[mesh.face_indices[i * 3 + 1]]);
                if (r.Intersect(t, out point))
                    return true;
            }

            return false;
        }

        // sequence of triangles
        public bool Intersect(Triangle[] triangles, Vector3 offset, out Vector3 point)
        {
            point = new Vector3(0, 0, 0);

            Ray r = this - offset;
            for (int i = 0; i < triangles.Length; i++)
            {
                // todo: use precalculated triangle normals
                if (r.Intersect(triangles[i], out point))
                    return true;
            }

            return false;
        }

        public static Ray operator +(Ray r, Vector3 c)
        {
            return new Ray(r.start + c, r.vector);
        }

        public static Ray operator -(Ray r, Vector3 c)
        {
            return new Ray(r.start - c, r.vector);
        }
    }
}
