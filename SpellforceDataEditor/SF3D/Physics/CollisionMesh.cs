/*
 * CollisionMesh is a geometrical object composed of a set of triangles
 * It's used to quickly calculate intersection point between a ray and an arbitrary geometrical shape
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SF3D.Physics
{
    public class CollisionMesh
    {
        public Triangle[] triangles;       
        public BoundingBox aabb;          // used to quickly check if ray can even hit any of the triangles
        public Vector3 offset;

        // generates a collision mesh using provided geometry and an offset (faster to give an offset to a ray
        // than to whole array of triangles)
        public void Generate(Vector3 off, Vector3[] vertices, uint[] indices = null)
        {
            offset = off;
            if (indices == null)
            {
                int triangle_count = vertices.Length / 3;

                triangles = new Triangle[triangle_count];

                for (int i = 0; i < triangle_count; i++)
                    triangles[i] = new Triangle(vertices[i * 3 + 0],
                                                vertices[i * 3 + 1],
                                                vertices[i * 3 + 2]);
            }
            else
            {
                int triangle_count = indices.Length / 3;

                triangles = new Triangle[triangle_count];

                for (int i = 0; i < triangle_count; i++)
                    triangles[i] = new Triangle(vertices[indices[i * 3 + 0]],
                                                vertices[indices[i * 3 + 1]],
                                                vertices[indices[i * 3 + 2]]);
            }

            float x1, x2, y1, y2, z1, z2;
            x1 = 10000;
            x2 = -10000;
            y1 = 10000;
            y2 = -10000;
            z1 = 10000;
            z2 = -10000;
            foreach (Vector3 v in vertices)
            {
                x1 = Math.Min(x1, v.X);
                x2 = Math.Max(x2, v.X);
                y1 = Math.Min(y1, v.Y);
                y2 = Math.Max(y2, v.Y);
                z1 = Math.Min(z1, v.Z);
                z2 = Math.Max(z2, v.Z);
            }
            aabb = new BoundingBox(new Vector3(x1, y1, z1), new Vector3(x2, y2, z2));
            aabb += offset;
        }

        // same as above, but vectors are stored in a float array now, and indices are always used
        // used for heightmap speedup
        public void GenerateFromHeightmap(Vector3 off, SFMap.SFMapHeightMapGeometryPool pool, int chunk_id)
        {
            float[] vertices = pool.vertices_pool;
            uint[] indices = pool.indices_pool;

            offset = off;

            int triangle_count = 512;

            triangles = new Triangle[triangle_count];

            int v_off = chunk_id * 3 * 17 * 17;
            int f_off = chunk_id * 6 * 16 * 16;

            for (int i = 0; i < triangle_count; i++)
            {
                Vector3 v1 = new Vector3(vertices[v_off + 3 * indices[f_off + 3 * i + 0] + 0],
                    vertices[v_off + 3 * indices[f_off + 3 * i + 0] + 1],
                    vertices[v_off + 3 * indices[f_off + 3 * i + 0] + 2]);
                Vector3 v2 = new Vector3(vertices[v_off + 3 * indices[f_off + 3 * i + 1] + 0],
                    vertices[v_off + 3 * indices[f_off + 3 * i + 1] + 1],
                    vertices[v_off + 3 * indices[f_off + 3 * i + 1] + 2]);
                Vector3 v3 = new Vector3(vertices[v_off + 3 * indices[f_off + 3 * i + 2] + 0],
                    vertices[v_off + 3 * indices[f_off + 3 * i + 2] + 1],
                    vertices[v_off + 3 * indices[f_off + 3 * i + 2] + 2]);
                triangles[i] = new Triangle(v1, v2, v3);
            }

            float x1, x2, y1, y2, z1, z2;
            x1 = 10000;
            x2 = -10000;
            y1 = 10000;
            y2 = -10000;
            z1 = 10000;
            z2 = -10000;
            for(int i = 0; i < 17*17; i++)
            {
                x1 = Math.Min(x1, vertices[v_off + 3 * i + 0]);
                x2 = Math.Max(x2, vertices[v_off + 3 * i + 0]);
                y1 = Math.Min(y1, vertices[v_off + 3 * i + 1]);
                y2 = Math.Max(y2, vertices[v_off + 3 * i + 1]);
                z1 = Math.Min(z1, vertices[v_off + 3 * i + 2]);
                z2 = Math.Max(z2, vertices[v_off + 3 * i + 2]);
            }
            aabb = new BoundingBox(new Vector3(x1, y1, z1), new Vector3(x2, y2, z2));
            aabb += offset;
        }
    }
}
