/*
 * Camera3D inherits from Object3D, and is simply a view matrix and a few functions to manipulate openGL view
 * It is used explicitly in render engine
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SF3D
{
    public class Camera3D: Object3D
    {   
        private Vector3 lookat = new Vector3(0, 1, 0);
        private Vector2 direction = new Vector2(0, 0);
        private Matrix4 proj_matrix = new Matrix4();
        private Matrix4 viewproj_matrix = new Matrix4();
        private Vector3[] frustrum_vertices;// = camera.get_frustrum_vertices();
        // construct frustrum planes
        private Physics.Plane[] frustrum_planes;// = new Physics.Plane[6];

        public Vector3 Lookat
        {
            get
            {
                return lookat;
            }
            set
            {
                lookat = value;

                direction.X = (float)Math.Atan2(-(lookat.Z - Position.Z), lookat.X - Position.X) + 2*1.570796f;
                if (Math.Abs(lookat.X - Position.Z) > 0.0001)
                {
                    direction.Y = (float)Math.Atan2(lookat.Y - Position.Y, new Vector3(lookat.X - Position.X, -(lookat.Z - Position.Z), 0).Length);
                    direction.Y = (direction.Y > 1.5 ? 1.5f : (direction.Y < -1.5 ? -1.5f : direction.Y));
                }
                //System.Diagnostics.Debug.WriteLine(direction.ToString());
                modified = true;
            }
        }
        public Vector2 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
                direction.Y = (direction.Y > 1.5 ? 1.5f : (direction.Y < -1.5 ? -1.5f : direction.Y));
                //calculate rotation vector
                lookat = Position + new Vector3((float)Math.Cos(direction.X) * (float)Math.Cos(direction.Y),
                                                (float)Math.Sin(direction.Y),
                                                (float)Math.Sin(direction.X) * (float)Math.Cos(direction.Y));
                modified = true;
            }
        }

        // view matrix: modelmatrix
        // proj matrix: projmatrix
        public Matrix4 ViewProjMatrix { get { return viewproj_matrix; } }
        public Matrix4 ProjMatrix { get { return proj_matrix; } set { proj_matrix = value; viewproj_matrix = proj_matrix; } }
        public Physics.Plane[] FrustrumPlanes { get { return frustrum_planes; } }
        public Vector3[] FrustrumVertices { get { return frustrum_vertices; } }

        public Camera3D()
        {
            frustrum_vertices = new Vector3[8];
            frustrum_planes = new Physics.Plane[6];
            update_modelMatrix();
        }

        new public void update_modelMatrix()
        {
            modelMatrix = Matrix4.LookAt(Position, lookat, new Vector3(0, 1, 0));
            viewproj_matrix = ModelMatrix * ProjMatrix;

            // calculate frustrum geometry
            calculate_frustrum_vertices();
            frustrum_planes[0] = new Physics.Plane(new Physics.Triangle(frustrum_vertices[0], frustrum_vertices[4], frustrum_vertices[1]));  // top plane
            frustrum_planes[1] = new Physics.Plane(new Physics.Triangle(frustrum_vertices[2], frustrum_vertices[3], frustrum_vertices[6]));  // bottom plane
            frustrum_planes[2] = new Physics.Plane(new Physics.Triangle(frustrum_vertices[0], frustrum_vertices[2], frustrum_vertices[4]));  // left plane
            frustrum_planes[3] = new Physics.Plane(new Physics.Triangle(frustrum_vertices[1], frustrum_vertices[5], frustrum_vertices[3]));  // right plane
            frustrum_planes[4] = new Physics.Plane(new Physics.Triangle(frustrum_vertices[0], frustrum_vertices[1], frustrum_vertices[2]));  // near plane
            frustrum_planes[5] = new Physics.Plane(new Physics.Triangle(frustrum_vertices[4], frustrum_vertices[6], frustrum_vertices[5]));  // far plane

            modified = false;
        }

        public void translate(Vector3 tr)
        {
            position += tr;
            lookat += tr;
            modified = true;
        }

        // todo: this is wrong at certain angles?
        private void calculate_frustrum_vertices()
        {
            // get forward, up, right direction
            // mirrored in XZ plane...
            Vector3 forward = modelMatrix.Row2.Xyz;
            Vector3 up = modelMatrix.Row1.Xyz;
            Vector3 right = modelMatrix.Row0.Xyz;

            // 100f, Math.Pi/4 are magic for now.....
            float deviation = (float)Math.Tan(Math.PI / 4)/2;
            Vector3 center  = position + forward * 0.1f;
            Vector3 center2 = position + forward * 100f;
            frustrum_vertices[0] = center + (-right + up) * deviation * 0.1f;
            frustrum_vertices[1] = center + (right + up) * deviation * 0.1f;
            frustrum_vertices[2] = center + (-right - up) * deviation * 0.1f;
            frustrum_vertices[3] = center + (right - up) * deviation * 0.1f;
            frustrum_vertices[4] = center2 + (-right + up) * deviation * 100f;
            frustrum_vertices[5] = center2 + (right + up) * deviation * 100f;
            frustrum_vertices[6] = center2 + (-right - up) * deviation * 100f;
            frustrum_vertices[7] = center2 + (right - up) * deviation * 100f;
            // reflection along XY plane at z = position.Z
            for(int i = 0; i < 8; i++)
                frustrum_vertices[i].Z = 2 * position.Z - frustrum_vertices[i].Z;
        }
    }
}
