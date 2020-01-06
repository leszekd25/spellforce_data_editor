/*
 * Ray is a high-level object which is used to calculate intersection between a line and other 3D primitives
 * Ray is described similarly to a line (see Line), but stores additional info
 * Ray has maximum length provided, if the intersection happens further than the length, it will not be registered
 * */

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

        float length2;
        public float Length { get { return (float)Math.Sqrt(length2); } set { length2 = value * value; } }

        public Ray(Vector3 s, Vector3 v)
        {
            start = s;
            vector = v;
            length2 = v.LengthSquared;
            nvector = v.Normalized();
        }

        // calculates point of intersection between the ray and a plane
        // returns true if intersection happened, and writes point of intersection to a given out parameter
        public bool Intersect(Plane pl, out Vector3 point)
        {
            float ln_prod = Vector3.Dot(vector, pl.normal);
            if (ln_prod != 0)
            {
                // intersection of ray and plane
                float ray_d = Vector3.Dot(pl.point - start, pl.normal) / ln_prod;
                point = new Vector3(ray_d * vector + start);
                if ((point - start).LengthSquared > length2)
                    return false;
                return true;
            }
            else
            {
                point = new Vector3(0, 0, 0);
                return false;
            }
        }
        
        // calculates point of intersection between the ray and a triangle
        // similar to plane intersection, but also must check if point lies inside of the given triangle
        public bool Intersect(Triangle tr, out Vector3 point)
        {
            float ln_prod = Vector3.Dot(vector, tr.normal);
            if(ln_prod > 0)
            {
                // intersection of ray and plane the triangle belongs to
                float ray_d = Vector3.Dot(Vector3.Subtract(tr.v1, start), tr.normal) / ln_prod;
                point = Vector3.Add(ray_d * vector, start);
                if (Vector3.Subtract(point, start).LengthSquared > length2)
                    return false;
                // check if point is in triangle using barycentric coordinates
                return tr.ContainsPoint(point);
            }
            else
            {
                point = new Vector3(0, 0, 0);
                return false;
            }
        }

        // calculates whether the ray intersects a bounding  box, does NOT calculate the point of intersection
        public bool Intersect(BoundingBox ab)
        {
            // binary sampling of distance to the box
            float current_start = 0;
            float current_end = (float)Math.Sqrt(length2);
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

        // calculates point of intersection between a 3D model and the ray
        // slow!
        public bool Intersect(SFModel3D mesh, Vector3 offset, out Vector3 point)
        {
            point = new Vector3(0, 0, 0);
            if (mesh == null)
                return false;

            BoundingBox ab = mesh.aabb + offset;
            if (!Intersect(ab))
                return false;

            Ray r = this - offset;
            foreach (SFSubModel3D sbm in mesh.submodels)
            {
                int triangle_count = sbm.face_indices.Length / 3;
                for (int i = 0; i < triangle_count; i++)
                {
                    Triangle t = new Triangle(sbm.vertices[sbm.face_indices[i * 3 + 2]],
                        sbm.vertices[sbm.face_indices[i * 3 + 0]],
                        sbm.vertices[sbm.face_indices[i * 3 + 1]]);
                    if (r.Intersect(t, out point))
                        return true;
                }
            }

            return false;
        }

        // calculates point of intersection between a collision mesh and the ray
        // since the triangles are pre-cached, this is much faster than with the 3d model
        public bool Intersect(CollisionMesh mesh, out Vector3 point)
        {
            point = new Vector3(0, 0, 0);
            if (mesh == null)
                return false;
            
            if (!Intersect(mesh.aabb))
                return false;
            
            Ray r = this - mesh.offset;

            for (int i = 0; i < mesh.triangles.Length; i++)
            {
                if (r.Intersect(mesh.triangles[i], out point))
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
