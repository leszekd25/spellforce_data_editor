using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SFEngine
{
    public static class MathUtils
    {
        static Random r = new Random();

        public static Quaternion Slerp(Quaternion q1, Quaternion q2, float t)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared == 0.0f)
            {
                if (q2.LengthSquared == 0.0f)
                {
                    return Quaternion.Identity;
                }
                return q2;
            }
            else if (q2.LengthSquared == 0.0f)
            {
                return q1;
            }


            float cosHalfAngle = q1.W * q2.W + Vector3.Dot(q1.Xyz, q2.Xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return q1;
            }
            else if (cosHalfAngle < 0.0f)
            {
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)System.Math.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * t) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - t;
                blendB = t;
            }

            Quaternion result = new Quaternion(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
            if (result.LengthSquared > 0.0f)
            {
                return Quaternion.Normalize(result);
            }
            else
            {
                return Quaternion.Identity;
            }
        }

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
        
        public static double GaussianDensity(float x, float  sigma)
        {
            return (1 / (sigma * Math.Sqrt(2 * Math.PI)) * Math.Exp(-(x * x) / (2 * sigma * sigma)));
        }

        public static int Rand()
        {
            return r.Next();
        }

        public static float Randf(float start=0,  float end=1)
        {
            float v = (float)r.Next() / Int32.MaxValue;
            return start + (end - start) * v;
        }


        public static Vector2 GaussRand(float x, float sigma)
        {
            float S = 0;
            float v1 = 0;
            float v2 = 0;
            while(true)
            {
                v1 = Randf(-1, 1);
                v2 = Randf(-1, 1);
                S = v1 * v1 + v2 * v2;
                if (S == 0)
                    continue;
                if (S < 1)
                    break;
            }
            float t = (float)Math.Sqrt(((-2 * Math.Log(S)) / S));
            return new Vector2(v1 * t  * sigma + x, v2 * t * sigma + x);
        }

        public static double Sigmoid(double t, double b=Math.E)
        {
            return 1 / (1 + Math.Pow(b, -t));
        }

        public static float DegToRad(int deg)
        {
            return 3.141526f * deg / 180.0f;
        }

        public static int RadToDeg(float rad)
        {
            return (int)(rad * 180 / 3.141526f);
        }

        // counterclockwise
        public static Vector2 RotateVec2(Vector2 v, float angle)
        {
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            return new Vector2(v.X * c - v.Y * s, - v.X * s - v.Y * c);
        }

        // counterclockwise, upvector = (0, 0, 1)
        public static Vector3 RotateVec3(Vector3 v, float azimuth, float altitude)
        {
            float asin = (float)Math.Sin(azimuth);
            float acos = (float)Math.Cos(azimuth);
            float lsin = (float)Math.Sin(altitude);
            float lcos = (float)Math.Cos(altitude);

            return new Vector3(
                v.X * lcos + v.Y * lsin * asin + v.Z * lsin * acos,
                v.Y * acos - v.Z * asin,
                v.X * lsin + v.Y * lcos * asin + v.Z * lcos * acos);
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

        public static float MapRange(float v, float s1, float s2, float d1, float d2)
        {
            return d1 + ((v - s1) / (s2 - s1)) * (d2 - d1);
        }

        public static float DistanceManhattan(Vector3 v1, Vector3 v2)
        {
            return Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y) + Math.Abs(v1.Z - v2.Z);
        }
    }
}
