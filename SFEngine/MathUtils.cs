using OpenTK.Mathematics;
using System;
using System.Windows.Forms.ComponentModel.Com2Interop;

namespace SFEngine
{
    public static class MathUtils
    {
        static Random r = new Random();

        public static float Lerp(in Vector2 v, float t)
        {
            return v[0] + t * (v[0] - v[1]);
        }

        public static float Cerp(in Vector4 v, float t)
        {
            return (2 * v[1] + t * (
                    v[2] - v[0] + t * (
                    2 * v[0] - 5 * v[1] + 4 * v[2] - v[3] + t * (
                    -v[0] + 3 * v[1] - 3 * v[2] + v[3])))) * 0.5f;
        }

        public static float Bilinear(in Matrix2 v, in Vector2 t)
        {
            return Lerp(new Vector2(Lerp(v.Row0, t[0]),
                                    Lerp(v.Row1, t[0])), t[1]);
        }

        public static float Bicubic(in Matrix4 v, in Vector2 t)
        {
            return Cerp(new Vector4(Cerp(v.Row0, t[0]),
                                    Cerp(v.Row1, t[0]),
                                    Cerp(v.Row2, t[0]),
                                    Cerp(v.Row3, t[0])), t[1]);
        }

        public static double GaussianDensity(float x, float sigma)
        {
            return (1 / (sigma * Math.Sqrt(2 * Math.PI)) * Math.Exp(-(x * x) / (2 * sigma * sigma)));
        }

        public static void Clamp<T>(ref T t, in T x1, in T x2) where T : IComparable<T>, IEquatable<T>
        {
            if (t.CompareTo(x1) < 0)
            {
                t = x1;
            }
            else if (t.CompareTo(x2) > 0)
            {
                t = x2;
            }
        }

        public static void Expand<T>(in T t, ref T x1, ref T x2) where T : IComparable<T>, IEquatable<T>
        {
            if (t.CompareTo(x1) < 0)
            {
                x1 = t;
            }
            else if (t.CompareTo(x2) > 0)
            {
                x2 = t;
            }
        }

        public static void RandSetSeed(int seed)
        {
            r = new Random(seed);
        }

        public static int Rand()
        {
            return r.Next();
        }

        public static float Randf(float start = 0, float end = 1)
        {
            float v = (float)r.Next() / Int32.MaxValue;
            return start + (end - start) * v;
        }


        public static Vector2 GaussRand(float x, float sigma)
        {
            float S = 0;
            float v1 = 0;
            float v2 = 0;
            while (true)
            {
                v1 = Randf(-1, 1);
                v2 = Randf(-1, 1);
                S = v1 * v1 + v2 * v2;
                if (S == 0)
                {
                    continue;
                }

                if (S < 1)
                {
                    break;
                }
            }
            float t = (float)Math.Sqrt(((-2 * Math.Log(S)) / S));
            return new Vector2(v1 * t * sigma + x, v2 * t * sigma + x);
        }

        public static double Sigmoid(double t, double b = Math.E)
        {
            return 1 / (1 + Math.Pow(b, -t));
        }

        // counterclockwise (the correct version)
        public static void RotateVec2(in Vector2 v, float angle, out Vector2 v2)
        {
            Vector2 tv = v;
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            v2.X = tv.X * c - tv.Y * s;
            v2.Y = tv.X * s + tv.Y * c;
        }

        // clockwise
        public static void RotateVec2Mirrored(in Vector2 v, float angle, out Vector2 v2)
        {
            Vector2 tv = v;
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            v2.X = tv.X * c - tv.Y * s;
            v2.Y = -tv.X * s - tv.Y * c;
        } 

        public static void RotateVec2PivotSinCos(in Vector2 v, in Vector2 p, float s, float c, out Vector2 v2)
        {
            Vector2.Subtract(in v, in p, out Vector2 tmp);
            v2.X = tmp.X * c - tmp.Y * s + p.X;
            v2.Y = tmp.X * s + tmp.Y * c + p.Y;
        }

        public static void RotateVec3Array(Vector3[] vs, in Vector3 offset, float azimuth, float altitude)
        {
            float asin = (float)Math.Sin(azimuth);
            float acos = (float)Math.Cos(azimuth);
            float lsin = (float)Math.Sin(altitude);
            float lcos = (float)Math.Cos(altitude);

            for (int i = 0; i < vs.Length; i++)
            {
                Vector3 v = vs[i];
                Vector3.Subtract(in v, in offset, out v);
                Vector3 v2;
                v2.X = v.X * lcos + v.Z * lsin * asin + v.Y * lsin * acos;
                v2.Y = v.X * lsin + v.Z * lcos * asin + v.Y * lcos * acos;
                v2.Z = v.Z * acos - v.Y * asin;
                Vector3.Add(in v2, in offset, out vs[i]);
            }
        }
    }
}
