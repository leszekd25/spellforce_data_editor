using OpenTK;
using System;
using System.Collections.Generic;

namespace SFEngine.SF3D.Physics
{
    public class Frustum
    {
        public Vector3 start;
        public Vector3 direction;
        public float ZNear;
        public float ZFar;
        public float aspect_ratio;

        public Vector3[] frustum_vertices = new Vector3[8];// = camera.get_frustrum_vertices();
        // construct frustrum planes
        public Plane[] frustum_planes = new Plane[6];

        public Frustum(Vector3 _start, Vector3 _direction, float _znear, float _zfar, float _aspect_ratio)
        {
            start = _start;
            direction = _direction.Normalized();
            ZNear = _znear;
            ZFar = _zfar;
            aspect_ratio = _aspect_ratio;

            Calculate();
        }

        public bool ContainsPoint(Vector3 p)
        {
            foreach (Plane pl in frustum_planes)
            {
                if (pl.SideOf(p))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ContainsPointIgnoreZFar(Vector3 p)
        {
            for (int i = 0; i < 5; i++)
            {
                if (frustum_planes[i].SideOf(p))
                {
                    return false;
                }
            }

            return true;
        }

        public void Calculate()
        {
            // get forward, up, right direction
            Vector3 forward = direction.Normalized();
            Vector3 right = Vector3.Cross(forward, new Vector3(0, 1, 0)).Normalized();
            Vector3 up = Vector3.Cross(forward, right);
            right *= aspect_ratio;

            // 200f, Math.Pi/4 are magic for now.....
            float deviation = (float)Math.Tan(Math.PI / 4) / 2;
            Vector3 center = start + forward * ZNear;
            Vector3 center2 = start + forward * ZFar;
            frustum_vertices[0] = center + (-right - up) * deviation * ZNear;
            frustum_vertices[1] = center + (right - up) * deviation * ZNear;
            frustum_vertices[2] = center + (-right + up) * deviation * ZNear;
            frustum_vertices[3] = center + (right + up) * deviation * ZNear;
            frustum_vertices[4] = center2 + (-right - up) * deviation * ZFar;
            frustum_vertices[5] = center2 + (right - up) * deviation * ZFar;
            frustum_vertices[6] = center2 + (-right + up) * deviation * ZFar;
            frustum_vertices[7] = center2 + (right + up) * deviation * ZFar;

            frustum_planes[0] = new Plane(frustum_vertices[0], frustum_vertices[4], frustum_vertices[1]);  // top plane
            frustum_planes[1] = new Plane(frustum_vertices[2], frustum_vertices[3], frustum_vertices[6]);  // bottom plane
            frustum_planes[2] = new Plane(frustum_vertices[0], frustum_vertices[2], frustum_vertices[4]);  // left plane
            frustum_planes[3] = new Plane(frustum_vertices[1], frustum_vertices[5], frustum_vertices[3]);  // right plane
            frustum_planes[4] = new Plane(frustum_vertices[0], frustum_vertices[1], frustum_vertices[2]);  // near plane
            frustum_planes[5] = new Plane(frustum_vertices[4], frustum_vertices[6], frustum_vertices[5]);  // far plane
        }

        public List<Vector3> GetConvexHullCutByYZero()
        {
            // the following assumptions are made:
            // only needs to cut side triangles - frustum is never rolled, only pitch/yaw changes
            // if one side triangle is cut, so is the other
            // top vertices are always higher up than bottom ones
            // ignore near plane completely, it's influence is negliglble
            // start vertex Y is always above 0
            List<Vector3> ret = new List<Vector3>();
            ret.Add(start);

            // check if any cutting is needed
            float bottom_y = Math.Min(start.Y, frustum_vertices[6].Y);

            if(bottom_y >= 0)
            {
                ret.Add(start);
                ret.Add(frustum_vertices[4]);
                ret.Add(frustum_vertices[5]);
                ret.Add(frustum_vertices[6]);
                ret.Add(frustum_vertices[7]);
                return ret;
            }

            // Y=0 cuts the frustum; figure out which vertices belong to the result, and add generated vertices to it
            float top_edge_t = -start.Y / (frustum_vertices[4].Y - start.Y);
            float bot_edge_t = -start.Y / (frustum_vertices[6].Y - start.Y);
            float far_edge_t = -frustum_vertices[6].Y / (frustum_vertices[4].Y - frustum_vertices[6].Y);

            // far edge is cut - new vertices are generated
            if((far_edge_t > 0)&&(far_edge_t < 1))
            {
                ret.Add(frustum_vertices[6] + (frustum_vertices[4] - frustum_vertices[6]) * far_edge_t);
                ret.Add(frustum_vertices[7] + (frustum_vertices[5] - frustum_vertices[7]) * far_edge_t);
            }

            // top edge is cut - new vertices are generated
            if ((top_edge_t > 0) && (top_edge_t < 1))
            {
                ret.Add(start + (frustum_vertices[4] - start) * top_edge_t);
                ret.Add(start + (frustum_vertices[5] - start) * top_edge_t);
            }
            else
            {
                ret.Add(frustum_vertices[4]);
                ret.Add(frustum_vertices[5]);
            }

            // bottom edge is cut - new vertices are generated
            if ((bot_edge_t > 0) && (bot_edge_t < 1))
            {
                ret.Add(start + (frustum_vertices[6] - start) * bot_edge_t);
                ret.Add(start + (frustum_vertices[7] - start) * bot_edge_t);
            }
            else
            {
                ret.Add(frustum_vertices[6]);
                ret.Add(frustum_vertices[7]);
            }

            return ret;
        }
    }
}
