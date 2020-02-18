using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor
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
            return new Vector2(v.X * c - v.Y * s, v.X * s + v.Y * c);
        }

        public static float MapRange(float v, float s1, float s2, float d1, float d2)
        {
            return d1 + ((v - s1) / (s2 - s1)) * (d2 - d1);
        }
    }
}
