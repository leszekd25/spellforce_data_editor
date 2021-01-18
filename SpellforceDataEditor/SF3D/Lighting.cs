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
        public float Azimuth;    // horizontal angle with respect to 0* corresponding to (1, 0, 0), and 90* corresponding to (0, 0, -1)
        public float Altitude;   // vertical angle with respect to 0( corresponding to (1, 0, 0), and 90* corresponding to (0, 1, 0)
        public Vector3 Direction { get; private set; } = new Vector3(0.0f, -1.0f, 0.0f);
        public float ShadowSize = 40;
        public Matrix4 LightProjection = Matrix4.CreateOrthographic(20, 20, 1, 100.0f);
        public Matrix4 LightMatrix { get; private set; }
        public float ZNear;
        public float ZFar;
        public float ShadowDepth;

        public void SetupLightView(Vector3 camerapos)
        {
            SF3D.Physics.BoundingBox aabb = new Physics.BoundingBox(
                new Vector3(camerapos.X - ShadowSize, 0, camerapos.Z - ShadowSize),
                new Vector3(camerapos.X + ShadowSize, camerapos.Y + 100, camerapos.Z + ShadowSize));
            SetupLightView(aabb);
        }

        public void SetLightDirection(Vector3 dir)
        {
            Direction = dir;
            // calculate azimuth and altitude
            Vector3 d2 = dir;
            d2.Z = 0;
            if (d2.LengthSquared == 0)
                Azimuth = 0;
            else
            {
                d2.Normalize();
                Azimuth = (float)(Vector3.CalculateAngle(Vector3.UnitX, d2)*180/Math.PI);
            }

            d2 = dir;
            d2.Y = 0;
            if(d2.LengthSquared == 0)
            {
                if (dir.Z > 0)
                    Altitude = 90;
                else
                    Altitude = 90;
            }
            else
            {
                d2.Normalize();
                if (dir.X > 0)
                    Altitude = (float)(Vector3.CalculateAngle(Vector3.UnitX, d2)*180/Math.PI);
                else
                    Altitude = (float)(Vector3.CalculateAngle(-Vector3.UnitX, d2)*180/Math.PI);
            }
        }

        public void SetAzimuthAltitude(float az, float al)
        {
            Azimuth = az;
            Altitude = al;

            // construct direction from azimuth and altitude

            if(al >= 0)
                Direction = -new Vector3((float)Math.Cos(-az * Math.PI / 180) * (float)Math.Cos(-al * Math.PI / 180),
                                (float)Math.Sin(-al * Math.PI / 180),
                                (float)Math.Sin(-az * Math.PI / 180) * (float)Math.Cos(-al * Math.PI / 180)).Normalized();
            else
                Direction = new Vector3((float)Math.Cos(-az * Math.PI / 180) * (float)Math.Cos(-al * Math.PI / 180),
                 (float)Math.Sin(-al * Math.PI / 180),
                 (float)Math.Sin(-az * Math.PI / 180) * (float)Math.Cos(-al * Math.PI / 180)).Normalized();
        }
        
        public void SetupLightView(Physics.BoundingBox aabb)
        {
            Physics.BoundingBox rotated_aabb = aabb.RotatedByAzimuthAltitude(Azimuth, Altitude);
            ZNear = 0.1f;
            ZFar = (rotated_aabb.a - rotated_aabb.b).Length;

            LightProjection = Matrix4.CreateOrthographic(rotated_aabb.b.X - rotated_aabb.a.X, rotated_aabb.b.Z - rotated_aabb.a.Z, ZNear, ZFar);

            Vector3 camera_pos = rotated_aabb.center;
            camera_pos += Direction * (ZFar / 2);

            LightMatrix = Matrix4.LookAt(camera_pos, camera_pos-Direction, new Vector3(0, 1, 0)) * LightProjection; //camera_pos-Direction+new Vector3(0, 0, 0.05f)
        }
    }

    public class Atmosphere
    {
        public Vector4 FogColor = new Vector4(100, 100, 100, 255) / 255f;
        public float FogStart = 100.0f;
        public float FogEnd = 200.0f;
        public LightingAmbient ambient_light { get; } = new LightingAmbient();
        public LightingSun sun_light { get; } = new LightingSun();
        public InterpolatedColor altitude_sun_color { get; set; }
        public InterpolatedColor altitude_ambient_color { get; set; }
        public InterpolatedColor altitude_fog_color { get; set; }

        // azimuth from 0 to 359, altitude from -90 to 90
        public void SetSunLocation(float azimuth, float altitude)
        {
            sun_light.SetAzimuthAltitude(azimuth, altitude);

            sun_light.Color = altitude_sun_color.Get(altitude + 90f);
            ambient_light.Color = altitude_ambient_color.Get(altitude + 90f);
            FogColor = altitude_fog_color.Get(altitude + 90f);
        }
    }
}
