using OpenTK;
using System;
using System.Collections.Generic;

namespace SFEngine.SFMap
{
    public class SFMapCollisionPolygon2D
    {
        public Vector2[] vertices { get; private set; }
        Vector2 offset = Vector2.Zero;
        int current_angle = -1;
        Vector2[] rotated_vertices;
        SFCoord rotated_bbox_topleft;
        SFCoord rotated_bbox_bottomright;

        public SFMapCollisionPolygon2D(Vector2[] v, Vector2 o)
        {
            vertices = v;
            offset = o;
            rotated_vertices = new Vector2[vertices.Length];
        }

        public void Rebuild(int angle)
        {
            if (angle == current_angle)
            {
                return;
            }

            current_angle = angle;
            float angle_rad = (float)(angle * Math.PI / 180);

            float s = (float)Math.Sin(angle_rad);
            float c = (float)Math.Cos(angle_rad);

            for (int i = 0; i < vertices.Length; i++)
            {
                rotated_vertices[i] = MathUtils.RotateVec2PivotSinCos(vertices[i], offset, s, c) - offset;
            }

            Vector2 topleft, bottomright;
            topleft = rotated_vertices[0]; bottomright = rotated_vertices[0];
            foreach (Vector2 v in rotated_vertices)
            {
                MathUtils.Expand(v, ref topleft, ref bottomright);
            }
            rotated_bbox_topleft = new SFCoord((int)(topleft.X - 1), (int)(topleft.Y - 1));
            rotated_bbox_bottomright = new SFCoord((int)(bottomright.X + 1), (int)(bottomright.Y + 1));
        }

        // http://geomalgorithms.com/a03-_inclusion.html

        private float PointIsLeftSide(Vector2 v, int s_ind, int s_ind2)
        {
            return ((rotated_vertices[s_ind2].X - rotated_vertices[s_ind].X) * (v.Y - rotated_vertices[s_ind].Y)
            - (v.X - rotated_vertices[s_ind].X) * (rotated_vertices[s_ind2].Y - rotated_vertices[s_ind].Y));
        }

        // calculate winding number of a polygon at point _p; winding number is 0 only if _p is outside of polygon
        public bool PointIsInside(SFCoord _p)
        {
            Vector2 p = new Vector2(_p.x, _p.y);
            int result = 0;        // winding number of the polygon

            for (int i = 0; i < vertices.Length; i++)
            {
                int j = i + 1;
                if (j == vertices.Length)
                {
                    j = 0;
                }

                if (rotated_vertices[i].Y <= p.Y)
                {
                    if (rotated_vertices[j].Y > p.Y)
                    {
                        if (PointIsLeftSide(p, i, j) > 0)
                        {
                            result += 1;
                        }
                    }
                }
                else
                {
                    if (rotated_vertices[j].Y <= p.Y)
                    {
                        if (PointIsLeftSide(p, i, j) < 0)
                        {
                            result -= 1;
                        }
                    }
                }
            }

            return (result != 0);
        }

        public IEnumerable<SFCoord> GetAllPointsInside()
        {
            for (int i = rotated_bbox_topleft.x; i <= rotated_bbox_bottomright.x; i++)
            {
                for (int j = rotated_bbox_topleft.y; j <= rotated_bbox_bottomright.y; j++)
                {
                    SFCoord point = new SFCoord(i, j);
                    if (PointIsInside(point))
                    {
                        yield return point;
                    }
                }
            }
        }
    }

    public class SFMapCollisionBoundary
    {
        public List<SFMapCollisionPolygon2D> polygons = new List<SFMapCollisionPolygon2D>();
        public Vector2 origin = new Vector2(0, 0);

        public SF3D.SFModel3D b_outline;
        int id = 0;
        static int max_id = 0;

        public SFMapCollisionBoundary()
        {
            id = max_id;
            max_id++;
        }

        public IEnumerable<SFCoord> GetCells(SFCoord pos, int angle)
        {
            foreach (SFMapCollisionPolygon2D poly in polygons)
            {
                poly.Rebuild(angle);
                foreach (SFCoord p in poly.GetAllPointsInside())
                {
                    yield return p + pos;
                }
            }
        }

        public void RebuildModel3D()
        {
            b_outline = new SF3D.SFModel3D();

            int seg_count = 0;
            foreach (SFMapCollisionPolygon2D s in polygons)
            {
                seg_count += s.vertices.Length;
            }

            Vector3[] vertices = new Vector3[seg_count * 4];
            Vector2[] uvs = new Vector2[seg_count * 4];
            byte[] colors = new byte[seg_count * 16];
            Vector3[] normals = new Vector3[seg_count * 4];
            uint[] indices = new uint[seg_count * 6];

            seg_count = 0;
            float line_width = 0.03f;
            foreach (SFMapCollisionPolygon2D s in polygons)
            {
                for (int i = 0; i < s.vertices.Length; i++)
                {
                    Vector2 v1 = s.vertices[i];
                    Vector2 v2 = s.vertices[(i + 1) % s.vertices.Length];
                    Vector2 n = ((v2 - v1).Normalized().PerpendicularLeft) * line_width;

                    vertices[(seg_count + i) * 4 + 0] = new Vector3((v1 + n).X, (v1 + n).Y, 1);
                    vertices[(seg_count + i) * 4 + 1] = new Vector3((v1 - n).X, (v1 - n).Y, 1);
                    vertices[(seg_count + i) * 4 + 2] = new Vector3((v2 + n).X, (v2 + n).Y, 1);
                    vertices[(seg_count + i) * 4 + 3] = new Vector3((v2 - n).X, (v2 - n).Y, 1);

                    indices[(seg_count + i) * 6 + 0] = (uint)((seg_count + i) * 4 + 0);
                    indices[(seg_count + i) * 6 + 1] = (uint)((seg_count + i) * 4 + 1);
                    indices[(seg_count + i) * 6 + 2] = (uint)((seg_count + i) * 4 + 2);
                    indices[(seg_count + i) * 6 + 3] = (uint)((seg_count + i) * 4 + 1);
                    indices[(seg_count + i) * 6 + 4] = (uint)((seg_count + i) * 4 + 2);
                    indices[(seg_count + i) * 6 + 5] = (uint)((seg_count + i) * 4 + 3);
                }
                seg_count += s.vertices.Length;
            }
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(0, 0);
            }
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = 0xFF;
            }

            SF3D.SFSubModel3D sbm = new SF3D.SFSubModel3D();
            sbm.CreateRaw(vertices, uvs, colors, normals, indices, null);

            b_outline.CreateRaw(new SF3D.SFSubModel3D[] { sbm });

            SFResources.SFResourceManager.Models.AddManually(b_outline, GetName());
        }

        public string GetName()
        {
            return "_OUTLINE_" + id.ToString();
        }
    }
}
