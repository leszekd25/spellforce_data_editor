using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

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
                    return false;
            }

            return true;
        }

        public bool ContainsPointIgnoreZFar(Vector3 p)
        {
            for(int i = 0; i < 5; i++)
            {
                if (frustum_planes[i].SideOf(p))
                    return false;
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
    }
}
