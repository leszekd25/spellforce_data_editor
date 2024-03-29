﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace SFEngine.SF3D
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

        public Matrix4[] ShadowCascadeLightProjection;
        public Matrix4[] ShadowCascadeLightMatrix;
        public Physics.Frustum[] ShadowCascadeFrustum;

        public LightingSun()
        {

        }

        public void SetAzimuthAltitude(float az, float al)
        {
            Azimuth = az;
            Altitude = al;

            // construct direction from azimuth and altitude

            if (al >= 0)
            {
                Direction = -new Vector3((float)Math.Cos(-az * Math.PI / 180) * (float)Math.Cos(-al * Math.PI / 180),
                                (float)Math.Sin(-al * Math.PI / 180),
                                (float)Math.Sin(-az * Math.PI / 180) * (float)Math.Cos(-al * Math.PI / 180)).Normalized();
            }
            else
            {
                Direction = new Vector3((float)Math.Cos(-az * Math.PI / 180) * (float)Math.Cos(-al * Math.PI / 180),
                 (float)Math.Sin(-al * Math.PI / 180),
                 (float)Math.Sin(-az * Math.PI / 180) * (float)Math.Cos(-al * Math.PI / 180)).Normalized();
            }
        }

        public void SetupLightView(Physics.BoundingBox aabb)
        {
            Physics.BoundingBox rotated_aabb = aabb.RotatedByAzimuthAltitude(Azimuth, Altitude);
            ZNear = 0.1f;
            ZFar = (rotated_aabb.a - rotated_aabb.b).Length;

            LightProjection = Matrix4.CreateOrthographic(rotated_aabb.b.X - rotated_aabb.a.X, rotated_aabb.b.Z - rotated_aabb.a.Z, ZNear, ZFar);

            Vector3 camera_pos = rotated_aabb.center;
            camera_pos += Direction * (ZFar / 2);

            LightMatrix = Matrix4.LookAt(camera_pos, camera_pos - Direction, new Vector3(0, 1, 0)) * LightProjection; //camera_pos-Direction+new Vector3(0, 0, 0.05f)
        }
    }

    public class Atmosphere
    {
        /*// vao used to render sky
        static float[] vertices = new float[] {
            -1, -1,
            -1, 1,
            1, -1,
            -1, 1,
            1, -1,
            1, 1 };
        static float[] uvs = new float[] {
            0, 0,
            0, 1,
            1, 0,
            0, 1,
            1, 0,
            1, 1 };

        public static int sky_vao { get; private set; } = -1;

        static int vertices_vbo = Utility.NO_INDEX;
        static int uvs_vbo = Utility.NO_INDEX;

        static int ref_count = 0;*/


        public Vector4 FogColor = new Vector4(100, 100, 100, 255) / 255f;
        public float FogStrength = 1.0f;
        public float FogStart = 0.0f;
        public float FogEnd = 200.0f;
        public float FogExponent = 1.5f;
        public LightingAmbient ambient_light { get; } = new LightingAmbient();
        public LightingSun sun_light { get; } = new LightingSun();
        public InterpolatedFloat altitude_sun_strength { get; set; }
        public InterpolatedColor altitude_sun_color { get; set; }
        public InterpolatedFloat altitude_ambient_strength { get; set; }
        public InterpolatedColor altitude_ambient_color { get; set; }
        public InterpolatedFloat altitude_fog_strength { get; set; }
        public InterpolatedColor altitude_fog_color { get; set; }

        public Atmosphere()
        {
            /*if (ref_count == 0)
            {
                sky_vao = GL.GenVertexArray();
                GL.BindVertexArray(sky_vao);

                vertices_vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertices_vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 4, vertices, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

                uvs_vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, uvs_vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * 4, uvs, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

                GL.BindVertexArray(0);
            }
            ref_count += 1;*/
        }

        // azimuth from 0 to 359, altitude from -90 to 90
        public void SetSunLocation(float azimuth, float altitude)
        {
            MathUtils.Clamp(ref azimuth, 0, 359);
            MathUtils.Clamp(ref altitude, -90, 90);
            sun_light.SetAzimuthAltitude(azimuth, altitude);

            sun_light.Color = altitude_sun_color.Get(altitude + 90f);
            sun_light.Strength = altitude_sun_strength.Get(altitude + 90f);
            ambient_light.Color = altitude_ambient_color.Get(altitude + 90f);
            ambient_light.Strength = altitude_ambient_strength.Get(altitude + 90f);
            FogColor = altitude_fog_color.Get(altitude + 90f);
            FogStrength = altitude_fog_strength.Get(altitude + 90f);

            SFRender.SFRenderEngine.ApplyLight();
        }

        public void Dispose()
        {
            /*if (ref_count == 0)
            {
                return;
            }

            ref_count -= 1;
            if (ref_count == 0)
            {
                GL.DeleteBuffer(uvs_vbo);
                GL.DeleteBuffer(vertices_vbo);
                GL.DeleteVertexArray(sky_vao);
            }*/
        }
    }
}
