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

        // calculates point of intersection between a heightmap and the ray
        // this is the preferred way of checking ray collision with the heightmap
        // NOT WORKING YET
        public bool Intersect(SFMap.SFMapHeightMap hmap, out Vector3 point)
        {
            point = new Vector3(0, 0, 0);
            Vector2 ray_grid_start = new Vector2(start.X, start.Z);
            Vector2 ray_grid_grad = new Vector2(vector.X, vector.Z).Normalized();
            int chunk_size = SFMap.SFMapHeightMapGeometryPool.CHUNK_SIZE;


            int next_grid_x = (int)Math.Floor(ray_grid_start.X / chunk_size) * chunk_size;
            int next_grid_y = (int)Math.Floor(ray_grid_start.Y / chunk_size) * chunk_size;
            if (ray_grid_grad.X > 0)
                next_grid_x += chunk_size;
            if (ray_grid_grad.Y > 0)
                next_grid_y += chunk_size;

            SFMap.SFCoord chunk_center = new SFMap.SFCoord(
                chunk_size / 2 + (int)Math.Floor(ray_grid_start.X / chunk_size) * chunk_size,
                chunk_size / 2 + (int)Math.Floor(ray_grid_start.Y / chunk_size) * chunk_size);

            float distance_travelled = 0;
            float lgt = Length;
            while (distance_travelled < lgt)
            {
                // check if current chunk center is in bounds
                if(!((hmap.width < chunk_center.x) || (0 > chunk_center.x) || (hmap.height < chunk_center.y) || (0 > chunk_center.y)))
                {
                    // check if collision happens on the map chunk right here
                    SFMap.SFMapHeightMapChunk hmap_chunk = hmap.GetChunk(new SFMap.SFCoord(chunk_center.x, hmap.map.width-chunk_center.y-1));
                    if ((hmap_chunk.collision_cache != null)&&(hmap_chunk.collision_cache.triangles != null))
                    {
                        if (Intersect(hmap_chunk.collision_cache, out point))
                            return true;
                    }
                }

                // move ray forward
                if (ray_grid_grad.X == 0)
                {
                    float coeff_y = Math.Abs((ray_grid_start.Y - next_grid_y) / ray_grid_grad.Y);
                    ray_grid_start += ray_grid_grad * coeff_y;
                    distance_travelled += coeff_y;
                    chunk_center = new SFMap.SFCoord(chunk_center.x, chunk_center.y+(ray_grid_grad.Y > 0 ? chunk_size : -chunk_size));
                    next_grid_y += (ray_grid_grad.Y > 0 ? chunk_size : -chunk_size);
                }
                else if (ray_grid_grad.Y == 0)
                {
                    float coeff_x = Math.Abs((ray_grid_start.X - next_grid_x) / ray_grid_grad.X);
                    ray_grid_start += ray_grid_grad * coeff_x;
                    distance_travelled += coeff_x;
                    chunk_center = new SFMap.SFCoord(chunk_center.x + (ray_grid_grad.X > 0 ? chunk_size : -chunk_size), chunk_center.y);
                    next_grid_x += (ray_grid_grad.X > 0 ? chunk_size : -chunk_size);
                }
                else
                {
                    float coeff_x = Math.Abs((ray_grid_start.X - next_grid_x) / ray_grid_grad.X);
                    float coeff_y = Math.Abs((ray_grid_start.Y - next_grid_y) / ray_grid_grad.Y);
                    if (coeff_x > coeff_y)           // will move 1 up/down on the grid
                    {
                        ray_grid_start += ray_grid_grad * coeff_y; 
                        distance_travelled += coeff_y;
                        chunk_center = new SFMap.SFCoord(chunk_center.x, chunk_center.y + (ray_grid_grad.Y > 0 ? chunk_size : -chunk_size));
                        next_grid_y += (ray_grid_grad.Y > 0 ? chunk_size : -chunk_size);
                    }
                    else
                    {
                        ray_grid_start += ray_grid_grad * coeff_x; 
                        distance_travelled += coeff_x;
                        chunk_center = new SFMap.SFCoord(chunk_center.x + (ray_grid_grad.X > 0 ? chunk_size : -chunk_size), chunk_center.y);
                        next_grid_x += (ray_grid_grad.X > 0 ? chunk_size : -chunk_size);
                    }
                }

                // check if ray out of bounds
                if ((hmap.width < chunk_center.x) && (ray_grid_grad.X > 0))
                    break;
                if ((0 > chunk_center.x) && (ray_grid_grad.X < 0))
                    break;
                if ((hmap.height < chunk_center.y) && (ray_grid_grad.Y > 0))
                    break;
                if ((0 > chunk_center.y) && (ray_grid_grad.Y < 0))
                    break;
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
