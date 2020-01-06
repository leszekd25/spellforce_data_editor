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

    // for some reason shadows dont want to work, need to investigate
    public class LightingSun
    {
        public float Strength = 1.0f;
        public Vector4 Color = new Vector4(1.0f);
        public Vector3 Direction = new Vector3(0.0f, -1.0f, 0.0f);
        public float ShadowSize = 40;
        public Matrix4 LightProjection = Matrix4.CreateOrthographic(20, 20, 1, 100.0f);
        public Matrix4 LightMatrix { get; private set; }

        public void SetupLightView(Vector3 camerapos)
        {
            // 1. get light direction angle to the level
            //Vector3 DirProjToLevel = new Vector3(Direction.X, 0, Direction.Z);
            //float AngleBeta = Vector3.CalculateAngle(Direction, DirProjToLevel);
            //float AngleAlpha = Vector3.CalculateAngle(DirProjToLevel, new Vector3(0, 0, 1));

            // 2. get shadowmap aspect ratio (dependent on angle)
            /*float ShadowFactorX = (float)(Math.Min
                 (Math.Abs(Math.Min(Math.Abs(1 / Math.Cos(AngleAlpha)), 10) / Math.Sin(AngleBeta)), 10));
             float ShadowFactorY = (float)(Math.Min
                 (Math.Abs(Math.Min(Math.Abs(1 / -Math.Sin(AngleAlpha)), 10) / Math.Sin(AngleBeta)), 10));*/

            float ShadowFactor = 1;// (float)Math.Min(Math.Abs(1 / Math.Sin(AngleBeta)), 10);


            SF3D.Physics.BoundingBox aabb = new Physics.BoundingBox(
                new Vector3(camerapos.X - ShadowSize * ShadowFactor, 0, camerapos.Z - ShadowSize * ShadowFactor),
                new Vector3(camerapos.X + ShadowSize * ShadowFactor, camerapos.Y + 100, camerapos.Z + ShadowSize * ShadowFactor));
            SetupLightView(aabb);
        }
        
        public void SetupLightView(Physics.BoundingBox aabb)
        {
            LightProjection = Matrix4.CreateOrthographic(aabb.b.X - aabb.a.X, aabb.b.Z - aabb.a.Z, 1, 1 + aabb.b.Y - aabb.a.Y);

            Vector3 camera_pos = new Vector3((aabb.b.X + aabb.a.X) / 2, aabb.b.Y, (aabb.b.Z + aabb.a.Z) / 2);
            /*Physics.Plane levelplane = new Physics.Plane(new Vector3(camera_pos.X, 0, camera_pos.Z), new Vector3(0, 1, 0));
            Physics.Ray r = new Physics.Ray(camera_pos, Direction) { length = 1000 };
            Vector3 col_pos = Vector3.Zero;
            Vector3 shift_amount = Vector3.Zero;
            if (r.Intersect(levelplane, out col_pos))
                shift_amount = levelplane.point - col_pos;
            camera_pos += shift_amount;*/

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
