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
        public Matrix4 LightProjection = Matrix4.CreateOrthographic(20, 20, 1, 100.0f);
        public Matrix4 LightMatrix { get; private set; }

        public void SetupLightView(Physics.BoundingBox aabb)
        {
            LightProjection = Matrix4.CreateOrthographic(aabb.b.X - aabb.a.X, aabb.b.Z - aabb.a.Z, 1, 1 + aabb.b.Y - aabb.a.Y);
            Vector3 camera_pos = new Vector3((aabb.b.X + aabb.a.X) / 2, 1 + aabb.b.Y, (aabb.b.Z + aabb.a.Z) / 2);
            LightMatrix = Matrix4.LookAt(camera_pos, camera_pos-Direction+new Vector3(0, 0, 0.05f), new Vector3(0, 1, 0)) * LightProjection;
        }
    }

    public class Atmosphere
    {
        public Vector4 FogColor = new Vector4(100, 100, 100, 255) / 255f;
        public float FogStart = 100.0f;
        public float FogEnd = 200.0f;
    }
}
