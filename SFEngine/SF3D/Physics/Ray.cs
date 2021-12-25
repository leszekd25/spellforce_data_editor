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

namespace SFEngine.SF3D.Physics
{
    // ray consists of a starting point and a direction vector
    public class Ray
    {
        public Vector3 start;
        public Vector3 vector;
        public Vector3 nvector;    // vector but normalized
        public Vector3 nvector_inverted;     // inverted normal

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
                point = Vector3.Zero;
                return false;
            }
        }

        // calculates point of intersection between the ray and a triangle
        // similar to plane intersection, but also must check if point lies inside of the given triangle
        public bool Intersect(Triangle tr, out Vector3 point)
        {
            float ln_prod = Vector3.Dot(vector, tr.normal);
            if (ln_prod > 0)
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
                point = Vector3.Zero;
                return false;
            }
        }

        // calculates whether the ray intersects a bounding  box, does NOT calculate the point of intersection
        public bool IntersectNoPoint(BoundingBox ab)
        {
            float min_dist = 0.1f;
            // binary sampling of distance to the box
            float current_start = 0;
            float current_end = (float)Math.Sqrt(length2);
            float current_center;
            Vector3 current_point;
            float dist;
            while (current_end - current_start >= min_dist)
            {
                current_center = (current_start + current_end) / 2;    //care about overflow
                current_point = start + nvector * current_center;
                dist = ab.DistanceIsotropic(current_point);
                if (dist <= min_dist)
                    return true;
                if (ab.DistanceIsotropic(start + nvector * current_start) < ab.DistanceIsotropic(start + nvector * current_end))
                    current_end = current_center;
                else
                    current_start = current_center;
            }
            return false;
        }

        // https://tavianator.com/2015/ray_box_nan.html
        // fast ray-box intersection
        // plane contact counts as intersetcion
        public bool Intersect(BoundingBox b, out Vector3 point)
        {
            double t1, t2;
            double tmin = double.MinValue;
            double tmax = double.MaxValue;

            // X clip
            if (nvector.X != 0)
            {
                t1 = (b.a.X - start.X) / nvector.X;
                t2 = (b.b.X - start.X) / nvector.X;

                tmin = Math.Max(tmin, Math.Min(t1, t2));
                tmax = Math.Min(tmax, Math.Max(t1, t2));
            }

            // Y clip
            if (nvector.Y != 0)
            {
                t1 = (b.a.Y - start.Y) / nvector.Y;
                t2 = (b.b.Y - start.Y) / nvector.Y;

                tmin = Math.Max(tmin, Math.Min(t1, t2));
                tmax = Math.Min(tmax, Math.Max(t1, t2));
            }

            // Z clip
            if (nvector.Z != 0)
            {
                t1 = (b.a.Z - start.Z) / nvector.Z;
                t2 = (b.b.Z - start.Z) / nvector.Z;

                tmin = Math.Max(tmin, Math.Min(t1, t2));
                tmax = Math.Min(tmax, Math.Max(t1, t2));
            }

            point = start + nvector * (float)tmin;

            return (tmin < Length) & (tmax >= Math.Max(tmin, 0.0));
        }

        // calculates point of intersection between a collision mesh and the ray
        // since the triangles are pre-cached, this is much faster than with the 3d model
        public bool Intersect(CollisionMesh mesh, out Vector3 point)
        {
            point = Vector3.Zero;
            if (mesh == null)
                return false;

            if (!IntersectNoPoint(mesh.aabb))
                return false;

            Ray r = this - mesh.offset;

            for (int i = 0; i < mesh.triangles.Length; i++)
            {
                if (r.Intersect(mesh.triangles[i], out point))
                    return true;
            }

            return false;
        }

        // version of Intersect(Triangle) for use in IntersectGeomPool() to avoid GC allocation in the form of new Triangle()
        public bool Intersect(Vector3 v1, Vector3 v2, Vector3 v3, out Vector3 point)
        {
            Vector3 v12 = Vector3.Subtract(v2, v1);
            Vector3 v13 = Vector3.Subtract(v3, v1);
            Vector3 normal = Vector3.Cross(v12, v13).Normalized();

            float ln_prod = Vector3.Dot(vector, normal);
            if (ln_prod > 0)
            {
                // intersection of ray and plane the triangle belongs to
                float ray_d = Vector3.Dot(Vector3.Subtract(v1, start), normal) / ln_prod;
                point = Vector3.Add(ray_d * vector, start);
                if (Vector3.Subtract(point, start).LengthSquared > length2)
                    return false;
                // check if point is in triangle using barycentric coordinates
                Vector3 v = Vector3.Subtract(point, v1); 
                float d00 = Vector3.Dot(v12, v12);
                float d01 = Vector3.Dot(v12, v13);
                float d11 = Vector3.Dot(v13, v13);
                float denom = 1.0f / (d00 * d11 - d01 * d01);

                float d20 = Vector3.Dot(v, v12);
                float d21 = Vector3.Dot(v, v13);
                float alpha = (d11 * d20 - d01 * d21) * denom;
                if (alpha < 0)
                    return false;

                float beta = (d00 * d21 - d01 * d20) * denom;
                return ((beta >= 0) && (alpha + beta <= 1));
            }
            else
            {
                point = Vector3.Zero;
                return false;
            }
        }

        // calculates point of intersection between a heightmap and the ray
        // this is the preferred way of checking ray collision with the heightmap
        public bool Intersect(SFMap.SFMapHeightMap hmap, out Vector3 point)
        {
            point = Vector3.Zero;

            int chunk_size = SFMap.SFMapHeightMapMesh.CHUNK_SIZE;
            int chunk_count = hmap.width / chunk_size;

            Vector2 ray_start_xz = new Vector2(start.X, start.Z);
            Vector2 ray_xz = new Vector2(start.X, start.Z);
            Vector2 ray_grad_xz = new Vector2(nvector.X, nvector.Z);
            if (ray_grad_xz.Length == 0)
                return false;

            float projection_coefficient = 1 / ray_grad_xz.Length;
            ray_grad_xz = ray_grad_xz.Normalized();
            Vector2 ray_grad_abs_xz = new Vector2(Math.Abs(ray_grad_xz.X), Math.Abs(ray_grad_xz.Y));

            int cur_chunk_x = (int)(ray_xz.X / chunk_size);
            int cur_chunk_y = (int)(ray_xz.Y / chunk_size);

            int map_w1 = hmap.width + 1;
            double tmin;

            bool horizontal_boundary = false;
            bool vertical_boundary = false;

            while (true)
            {
                if ((cur_chunk_x >= 0) && (cur_chunk_x < chunk_count) && (cur_chunk_y >= 0) && (cur_chunk_y < chunk_count))
                {
                    // chunk exists, check if chunk aabb intersects the ray
                    SFMap.SFMapHeightMapChunk chunk = hmap.chunk_nodes[cur_chunk_y * chunk_count + cur_chunk_x].MapChunk;
                    Vector3 col_point;
                    if (Intersect(chunk.aabb, out col_point))
                    {
                        // chunk potentially intersects the ray, now check chunk triangles
                        int xoffset = chunk.ix * chunk_size;
                        int yoffset = chunk.iy * chunk_size;

                        int cur_tile_x = (int)(ray_xz.X);
                        int cur_tile_y = (int)(ray_xz.Y);

                        if (horizontal_boundary)
                            if (ray_grad_xz.X < 0)
                                cur_tile_x -= 1;
                        if (vertical_boundary)
                            if (ray_grad_xz.Y < 0)
                                cur_tile_y -= 1;

                        Vector3 v1, v2, v3;
                        while (true)
                        {
                            if (!((cur_tile_x >= xoffset) && (cur_tile_x < xoffset + chunk_size) && (cur_tile_y >= yoffset) && (cur_tile_y < yoffset + chunk_size)))
                                break;

                            int fixed_tile_y = hmap.height - cur_tile_y;
                            // check intersection with tile geometry (2 triangles)
                            v1 = new Vector3(cur_tile_x, hmap.GetZ(new SFMap.SFCoord(cur_tile_x, fixed_tile_y - 1)) / 100.0f, cur_tile_y);// hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x];
                            v2 = new Vector3(cur_tile_x + 1, hmap.GetZ(new SFMap.SFCoord(cur_tile_x + 1, fixed_tile_y - 1)) / 100.0f, cur_tile_y);// hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x + 1];
                            v3 = new Vector3(cur_tile_x, hmap.GetZ(new SFMap.SFCoord(cur_tile_x, fixed_tile_y - 2)) / 100.0f, cur_tile_y + 1);// hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x + - map_w1];
                            if (Intersect(v1, v2, v3, out point))
                                return true;

                            v1 = new Vector3(cur_tile_x + 1, hmap.GetZ(new SFMap.SFCoord(cur_tile_x + 1, fixed_tile_y - 1)) / 100.0f, cur_tile_y);// hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x + 1];
                            v2 = new Vector3(cur_tile_x + 1, hmap.GetZ(new SFMap.SFCoord(cur_tile_x + 1, fixed_tile_y - 2)) / 100.0f, cur_tile_y + 1);// hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x + - map_w1 + 1];
                            v3 = new Vector3(cur_tile_x, hmap.GetZ(new SFMap.SFCoord(cur_tile_x, fixed_tile_y - 2)) / 100.0f, cur_tile_y + 1); //hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x + - map_w1];
                            if (Intersect(v1, v2, v3, out point))
                                return true;

                            // move ray to the next tile
                            int new_tile_x = (ray_grad_xz.X > 0 ? cur_tile_x + 1 : cur_tile_x - 1);
                            int new_tile_y = (ray_grad_xz.Y > 0 ? cur_tile_y + 1 : cur_tile_y - 1);
                            float new_tile_x_dist_coeff = (Math.Abs(ray_xz.X - ((new_tile_x + 0.5f) + 0.5f * (ray_grad_xz.X < 0 ? 1 : -1)))) / ray_grad_abs_xz.X;
                            float new_tile_y_dist_coeff = (Math.Abs(ray_xz.Y - ((new_tile_y + 0.5f) + 0.5f * (ray_grad_xz.Y < 0 ? 1 : -1)))) / ray_grad_abs_xz.Y;

                            if (ray_grad_abs_xz.X == 0)
                                cur_tile_y = new_tile_y;
                            else if (ray_grad_abs_xz.Y == 0)
                                cur_tile_x = new_tile_x;
                            else if (new_tile_x_dist_coeff < new_tile_y_dist_coeff)
                                cur_tile_x = new_tile_x;
                            else
                                cur_tile_y = new_tile_y;

                            // calculate collision of ray with the tile xz (we know that it collides)
                            tmin = Math.Min(new_tile_x_dist_coeff, new_tile_y_dist_coeff);

                            ray_xz = ray_xz + ray_grad_xz * (float)tmin;
                        }
                    }
                }

                // move ray to the next chunk
                int new_chunk_x = (ray_grad_xz.X > 0 ? cur_chunk_x + 1 : cur_chunk_x - 1);
                int new_chunk_y = (ray_grad_xz.Y > 0 ? cur_chunk_y + 1 : cur_chunk_y - 1);
                float new_chunk_x_dist_coeff = (Math.Abs(ray_xz.X - ((new_chunk_x * chunk_size + chunk_size / 2) + (chunk_size / 2) * (ray_grad_xz.X < 0 ? 1 : -1)))) / ray_grad_abs_xz.X;
                float new_chunk_y_dist_coeff = (Math.Abs(ray_xz.Y - ((new_chunk_y * chunk_size + chunk_size / 2) + (chunk_size / 2) * (ray_grad_xz.Y < 0 ? 1 : -1)))) / ray_grad_abs_xz.Y;

                if (ray_grad_abs_xz.X == 0)
                    cur_chunk_y = new_chunk_y;
                else if (ray_grad_abs_xz.Y == 0)
                    cur_chunk_x = new_chunk_x;
                else if (new_chunk_x_dist_coeff < new_chunk_y_dist_coeff)
                {
                    horizontal_boundary = true;
                    vertical_boundary = false;
                    cur_chunk_x = new_chunk_x;
                }
                else
                {
                    horizontal_boundary = false;
                    vertical_boundary = true;
                    cur_chunk_y = new_chunk_y;
                }

                // calculate collision of ray with the chunk xz (we know that it collides)
                tmin = Math.Min(new_chunk_x_dist_coeff, new_chunk_y_dist_coeff);

                ray_xz = ray_xz + ray_grad_xz * (float)tmin;

                if ((ray_xz - ray_start_xz).Length * projection_coefficient > Length)
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
