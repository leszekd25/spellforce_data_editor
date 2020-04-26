/*
 * CompressedMatrix is an intermediate structure which holds rotation matrix (3x3) using quaternion (4) and translation vector (3)
 * Holding data in this structure simplifies operations that would otherwise have to be done on 4x4 matrices
 * Note that currently it's only used to reduce data bandwidth
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SF3D
{
    public struct CompressedMatrix
    {
        Vector3 position;
        Quaternion rotation;

        public CompressedMatrix(Vector3 v, Quaternion q)
        {
            position = v;
            rotation = q;
        }

        public Matrix4 to_mat4()
        {
            Matrix4 mat = Matrix4.CreateFromQuaternion(rotation);
            mat.Row3 = new Vector4(position, 1);
            return mat;
        }
    }
}
