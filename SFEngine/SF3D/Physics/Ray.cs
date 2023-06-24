/*
 * Ray is a high-level object which is used to calculate intersection between a line and other 3D primitives
 * Ray is described similarly to a line (see Line), but stores additional info
 * Ray has maximum length provided, if the intersection happens further than the length, it will not be registered
 * */

using OpenTK;
using System;

namespace SFEngine.SF3D.Physics
{
    // ray consists of a starting point and a direction vector
    public class Ray
    {
        public Vector3 start;
        public Vector3 vector;
        public Vector3 nvector;    // vector but normalized
        public Vector3 nvector_inverted;     // inverted normal

        float length;
        float length2;
        public float Length { get { return length; } set { length = value; length2 = value * value; } }

        public Ray(Vector3 s, Vector3 v)
        {
            start = s;
            vector = v;
            Length = v.Length;
            length2 = v.LengthSquared;
            nvector = v / length;
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

        // version of Intersect(Triangle) for use in IntersectGeomPool() to avoid GC allocation in the form of new Triangle()
        // https://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/raytri_tam.pdf (culling version)
        public bool IntersectMollerTrumbore(Vector3 v1, Vector3 v2, Vector3 v3, out Vector3 point)
        {
            Vector3 e1 = Vector3.Subtract(v3, v1);
            Vector3 e2 = Vector3.Subtract(v2, v1);
            Vector3 pvec = Vector3.Cross(nvector, e2);
            float determinant = Vector3.Dot(e1, pvec);
            if(determinant < 0.0000001f)
            {
                point = Vector3.Zero;
                return false;
            }
            Vector3 tvec = Vector3.Subtract(start, v1);
            float u = Vector3.Dot(tvec, pvec);
            if((u < 0.0f)||(u > determinant))
            {
                point = Vector3.Zero;
                return false;
            }
            Vector3 qvec = Vector3.Cross(tvec, e1);
            float v = Vector3.Dot(nvector, qvec);
            if((v < 0.0f)||(u+v > determinant))
            {
                point = Vector3.Zero;
                return false;
            }
            float inv_determinant = 1.0f / determinant;
            float t = inv_determinant * Vector3.Dot(e2, qvec);
            point = Vector3.Add(Vector3.Multiply(nvector, t), start);
            return true;
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
            {
                return false;
            }

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
                        {
                            if (ray_grad_xz.X < 0)
                            {
                                cur_tile_x -= 1;
                            }
                        }

                        if (vertical_boundary)
                        {
                            if (ray_grad_xz.Y < 0)
                            {
                                cur_tile_y -= 1;
                            }
                        }

                        Vector3 v1, v2, v3;
                        while (true)
                        {
                            if (!((cur_tile_x >= xoffset) && (cur_tile_x <= xoffset + chunk_size) && (cur_tile_y >= yoffset) && (cur_tile_y <= yoffset + chunk_size)))
                            {
                                break;
                            }

                            int fixed_tile_y = hmap.height - cur_tile_y;
                            // check intersection with tile geometry (2 triangles)
                            v1 = new Vector3(cur_tile_x, hmap.GetZ(new SFMap.SFCoord(cur_tile_x, fixed_tile_y - 1)) / 100.0f, cur_tile_y);// hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x];
                            v2 = new Vector3(cur_tile_x + 1, hmap.GetZ(new SFMap.SFCoord(cur_tile_x + 1, fixed_tile_y - 1)) / 100.0f, cur_tile_y);// hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x + 1];
                            v3 = new Vector3(cur_tile_x, hmap.GetZ(new SFMap.SFCoord(cur_tile_x, fixed_tile_y - 2)) / 100.0f, cur_tile_y + 1);// hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x + - map_w1];
                            if (IntersectMollerTrumbore(v1, v2, v3, out point))
                            {
                                return true;
                            }

                            v1 = new Vector3(cur_tile_x + 1, hmap.GetZ(new SFMap.SFCoord(cur_tile_x + 1, fixed_tile_y - 1)) / 100.0f, cur_tile_y);// hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x + 1];
                            v2 = new Vector3(cur_tile_x + 1, hmap.GetZ(new SFMap.SFCoord(cur_tile_x + 1, fixed_tile_y - 2)) / 100.0f, cur_tile_y + 1);// hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x + - map_w1 + 1];
                            v3 = new Vector3(cur_tile_x, hmap.GetZ(new SFMap.SFCoord(cur_tile_x, fixed_tile_y - 2)) / 100.0f, cur_tile_y + 1); //hmap.mesh.vertices[fixed_tile_y * map_w1 + cur_tile_x + - map_w1];
                            if (IntersectMollerTrumbore(v1, v2, v3, out point))
                            {
                                return true;
                            }

                            // move ray to the next tile
                            int new_tile_x = (ray_grad_xz.X > 0 ? cur_tile_x + 1 : cur_tile_x - 1);
                            int new_tile_y = (ray_grad_xz.Y > 0 ? cur_tile_y + 1 : cur_tile_y - 1);
                            float new_tile_x_dist_coeff = (Math.Abs(ray_xz.X - ((new_tile_x + 0.5f) + 0.5f * (ray_grad_xz.X < 0 ? 1 : -1)))) / ray_grad_abs_xz.X;
                            float new_tile_y_dist_coeff = (Math.Abs(ray_xz.Y - ((new_tile_y + 0.5f) + 0.5f * (ray_grad_xz.Y < 0 ? 1 : -1)))) / ray_grad_abs_xz.Y;

                            if (ray_grad_abs_xz.X == 0)
                            {
                                cur_tile_y = new_tile_y;
                            }
                            else if (ray_grad_abs_xz.Y == 0)
                            {
                                cur_tile_x = new_tile_x;
                            }
                            else if (new_tile_x_dist_coeff < new_tile_y_dist_coeff)
                            {
                                cur_tile_x = new_tile_x;
                            }
                            else
                            {
                                cur_tile_y = new_tile_y;
                            }

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
                {
                    cur_chunk_y = new_chunk_y;
                }
                else if (ray_grad_abs_xz.Y == 0)
                {
                    cur_chunk_x = new_chunk_x;
                }
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
                {
                    break;
                }
            }

            return false;
        }
    }
}
