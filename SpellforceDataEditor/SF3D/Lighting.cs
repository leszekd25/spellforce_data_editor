using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SF3D
{
    public class LightingAmbient
    {
        public float Strength = 1.0f;
        public Vector4 Color = new Vector4(1.0f);
    }

    public class LightingSun
    {
        public float Strength = 1.0f;
        public Vector4 Color = new Vector4(1.0f);
        public Vector3 Direction = new Vector3(0.0f, -1.0f, 0.0f);
        public Matrix4 LightProjection = Matrix4.CreateOrthographic(20, 20, 1, 7.5f);
        public Matrix4 LightMatrix { get; private set; }

        public void SetupLightView(Vector3 camera_pos)
        {
            LightMatrix = LightProjection * Matrix4.LookAt(camera_pos, camera_pos + Direction, new Vector3(0, -1, 0));
        }
    }
}
