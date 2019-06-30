using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapCollisionPolygon2D
    {
        public Vector2[] vertices { get; private set; }
        int current_angle = -1;
        Vector2[] rotated_vertices;
        SFCoord rotated_bbox_topleft;
        SFCoord rotated_bbox_bottomright;

        public SFMapCollisionPolygon2D(Vector2[] v, Vector2 offset)
        {
            Rebuild(v, offset);
        }

        public void Rebuild(Vector2[] v, Vector2 offset)
        {
            vertices = v;
            for (int i = 0; i < v.Length; i++)
                vertices[i] = new Vector2(-v[i].X, v[i].Y);
            rotated_vertices = new Vector2[vertices.Length];
            SetRotation(offset, 0);
        }

        // counter-clockwise rotation around the origin: 0 is hour 3, 90 is hour 0, 180 is hour 9, 270 is hour 6
        public void SetRotation(Vector2 offset, int angle)
        {
            if (angle == current_angle)
                return;
            current_angle = angle;
            float angle_rad = (float)(angle * Math.PI / 180);

            Vector2 fixed_offset = new Vector2(offset.X , offset.Y );

            Vector2 rotated_offset = new Vector2(fixed_offset.X, fixed_offset.Y);
            if (angle != 0)
            {
                rotated_offset.X = (float)((Math.Cos(angle_rad) * fixed_offset.X) - (Math.Sin(angle_rad) * fixed_offset.Y));
                rotated_offset.Y = (float)((Math.Sin(angle_rad) * fixed_offset.X) + (Math.Cos(angle_rad) * fixed_offset.Y));
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                if (angle == 0)
                    rotated_vertices[i] = vertices[i] + fixed_offset;
                else
                {
                    rotated_vertices[i].X = (float)((Math.Cos(angle_rad) * vertices[i].X) - (Math.Sin(angle_rad) * vertices[i].Y));
                    rotated_vertices[i].Y = (float)((Math.Sin(angle_rad) * vertices[i].X) + (Math.Cos(angle_rad) * vertices[i].Y));
                    rotated_vertices[i] += rotated_offset;
                }
            }

            Vector2 topleft, bottomright;
            topleft = rotated_vertices[0]; bottomright = rotated_vertices[0];
            foreach (Vector2 v in rotated_vertices)
            {
                if (v.X < topleft.X)
                    topleft.X = v.X;
                if (v.X > bottomright.X)
                    bottomright.X = v.X;
                if (v.Y < topleft.Y)
                    topleft.Y = v.Y;
                if (v.Y > bottomright.Y)
                    bottomright.Y = v.Y;
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

            for(int i = 0; i < vertices.Length; i++)
            {
                int j = i + 1;
                if (j == vertices.Length)
                    j = 0;

                if(rotated_vertices[i].Y <= p.Y)
                {
                    if (rotated_vertices[j].Y > p.Y)
                        if (PointIsLeftSide(p, i, j) > 0)
                            result += 1;
                }
                else
                {
                    if (rotated_vertices[j].Y <= p.Y)
                        if (PointIsLeftSide(p, i, j) < 0)
                            result -= 1;
                }
            }

            return (result != 0);
        }

        // no offset!! offset is given later
        public HashSet<SFCoord> GetAllPointsInside()
        {
            HashSet<SFCoord> result = new HashSet<SFCoord>();
            SFCoord point;

            for (int i = rotated_bbox_topleft.x; i <= rotated_bbox_bottomright.x; i++)
                for (int j = rotated_bbox_topleft.y; j <= rotated_bbox_bottomright.y; j++)
                {
                    point = new SFCoord(i, j);
                    if (PointIsInside(point))
                        result.Add(point);
                }

            return result;
        }
    }

    public class SFMapCollisionBoundary
    {
        public List<SFMapCollisionPolygon2D> polygons { get; private set; } = new List<SFMapCollisionPolygon2D>();
        public Vector2 origin { get; private set; } = new Vector2(0, 0);
        public HashSet<SFCoord> interior_cells { get; private set; } = new HashSet<SFCoord>();

        SF3D.SFModel3D b_outline = new SF3D.SFModel3D();

        public void AddPolygon(SFMapCollisionPolygon2D poly)
        {
            polygons.Add(poly);
            // update with SetRotation
        }

        public void ClearPolygons()
        {
            polygons.Clear();
            interior_cells.Clear();
        }

        public void SetOffset(Vector2 v)
        {
            origin = v;
        }

        // after this call, new polygons will still have original rotation of 0 degrees...
        // after this call, interior_cells is updated, but cells inside still need to be offset
        public void SetRotation(int angle)
        {
            interior_cells.Clear();
            foreach (SFMapCollisionPolygon2D poly in polygons)
            {
                poly.SetRotation(origin, angle);
                interior_cells.UnionWith(poly.GetAllPointsInside());
            }
        }
    }
}
