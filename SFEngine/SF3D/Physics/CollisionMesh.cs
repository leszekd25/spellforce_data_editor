/*
 * CollisionMesh is a geometrical object composed of a set of triangles
 * It's used to quickly calculate intersection point between a ray and an arbitrary geometrical shape
 * 
 * Currently unused...
 * */

using OpenTK;
using System;

namespace SFEngine.SF3D.Physics
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
                {
                    triangles[i] = new Triangle(vertices[i * 3 + 0],
                                                vertices[i * 3 + 1],
                                                vertices[i * 3 + 2]);
                }
            }
            else
            {
                int triangle_count = indices.Length / 3;

                triangles = new Triangle[triangle_count];

                for (int i = 0; i < triangle_count; i++)
                {
                    triangles[i] = new Triangle(vertices[indices[i * 3 + 0]],
                                                vertices[indices[i * 3 + 1]],
                                                vertices[indices[i * 3 + 2]]);
                }
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
            aabb = new BoundingBox(new Vector3(x1, y1, z1) + offset, new Vector3(x2, y2, z2) + offset);
        }
    }
}
