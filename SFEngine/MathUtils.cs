using OpenTK;
using System;
using SFEngine.SFMap;

namespace SFEngine
{
    public static class MathUtils
    {
        static Random r = new Random();

        public static float Lerp(Vector2 v, float t)
        {
            return v[0] + t * (v[0] - v[1]);
        }

        public static float Cerp(Vector4 v, float t)
        {
            return (2 * v[1] + t * (
                    v[2] - v[0] + t * (
                    2 * v[0] - 5 * v[1] + 4 * v[2] - v[3] + t * (
                    -v[0] + 3 * v[1] - 3 * v[2] + v[3])))) * 0.5f;
        }

        public static float Bilinear(Matrix2 v, Vector2 t)
        {
            return Lerp(new Vector2(Lerp(v.Row0, t[0]),
                                    Lerp(v.Row1, t[0])), t[1]);
        }

        public static float Bicubic(Matrix4 v, Vector2 t)
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

        public static void Clamp<T>(ref T t, T x1, T x2) where T : IComparable<T>, IEquatable<T>
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

        public static void Expand<T>(T t, ref T x1, ref T x2) where T : IComparable<T>, IEquatable<T>
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

        public static void Clamp(ref Vector2 t, Vector2 x1, Vector2 x2)
        {
            if (t.X < x1.X)
            {
                t.X = x1.X;
            }
            else if (t.X > x2.X)
            {
                t.X = x2.X;
            }
            if (t.Y < x1.Y)
            {
                t.Y = x1.Y;
            }
            else if (t.Y > x2.Y)
            {
                t.Y = x2.Y;
            }
        }

        public static void Clamp(ref SFCoord t, SFCoord x1, SFCoord x2)
        {
            if (t.x < x1.x)
            {
                t.x = x1.x;
            }
            else if (t.x > x2.x)
            {
                t.x = x2.x;
            }
            if (t.y < x1.y)
            {
                t.y = x1.y;
            }
            else if (t.y > x2.y)
            {
                t.y = x2.y;
            }
        }

        public static void Expand(Vector2 t, ref Vector2 x1, ref Vector2 x2)
        {
            if (t.X < x1.X)
            {
                x1.X = t.X;
            }
            else if (t.X > x2.X)
            {
                x2.X = t.X;
            }
            if (t.Y < x1.Y)
            {
                x1.Y = t.Y;
            }
            else if (t.Y > x2.Y)
            {
                x2.Y = t.Y;
            }
        }

        public static void Expand(SFCoord t, ref SFCoord x1, ref SFCoord x2)
        {
            if (t.x < x1.x)
            {
                x1.x = t.x;
            }
            else if (t.x > x2.x)
            {
                x2.x = t.x;
            }
            if (t.y < x1.y)
            {
                x1.y = t.y;
            }
            else if (t.y > x2.y)
            {
                x2.y = t.y;
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
        public static Vector2 RotateVec2(Vector2 v, float angle)
        {
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            return new Vector2(v.X * c - v.Y * s, v.X * s + v.Y * c);
        }

        // clockwise
        public static Vector2 RotateVec2Mirrored(Vector2 v, float angle)
        {
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            return new Vector2(v.X * c - v.Y * s, -v.X * s - v.Y * c);
        } 

        public static Vector2 RotateVec2PivotSinCos(Vector2 v, Vector2 p, float s, float c)
        {
            Vector2 tmp = v - p;
            return new Vector2(tmp.X * c - tmp.Y * s, tmp.X * s + tmp.Y * c) + p;
        }

        public static void RotateVec3Array(Vector3[] vs, Vector3 offset, float azimuth, float altitude)
        {
            float asin = (float)Math.Sin(azimuth);
            float acos = (float)Math.Cos(azimuth);
            float lsin = (float)Math.Sin(altitude);
            float lcos = (float)Math.Cos(altitude);

            for (int i = 0; i < vs.Length; i++)
            {
                vs[i] -= offset;

                vs[i] = new Vector3(
                    vs[i].X * lcos + vs[i].Z * lsin * asin + vs[i].Y * lsin * acos,
                    vs[i].X * lsin + vs[i].Z * lcos * asin + vs[i].Y * lcos * acos,
                    vs[i].Z * acos - vs[i].Y * asin);

                vs[i] += offset;
            }
        }
    }
}
