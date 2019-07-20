using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SFMap.MapGen
{
    public enum FilteringType { NEAREST, BILINEAR, BICUBIC }

    public class GradientMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float[] map { get; private set; } = null;

        public GradientMap(int  w,  int  h)
        {
            Width = w;
            Height = h;
            map = new float[Width * Height];
            map.Initialize();
        }

        public GradientMap(GradientMap m)
        {
            Width = m.Width;
            Height = m.Height;
            map = new float[Width * Height];
            m.map.CopyTo(map, 0);
        }

        // respects boundaries
        public float Get(int x, int  y)
        {
            if ((x < 0) || (x >= Width) || (y < 0) || (y >= Height))
                return 0;
            return map[y * Width + x];
        }

        // doesnt respect boundaries!
        public void Set(int x, int  y,  float  f)
        {
            map[y * Width + x] = f;
        }

        public float GetNearest(float x, float y)
        {
            return Get((int)x, (int)y);
        }

        public float GetBilinear(float x, float y)
        {
            int _x = (int)x;
            int _y = (int)y;
            Vector2 d = new Vector2(x - _x, y - _y);

            Matrix2 bilinear_matrix = new Matrix2(new Vector2(Get(_x, _y),     Get(_x + 1, _y)),
                                                  new Vector2(Get(_x, _y + 1), Get(_x + 1, _y + 1)));

            return MathUtils.Bilinear(bilinear_matrix, d);
        }

        public float GetBicubic(float x, float y)
        {
            int _x = (int)x;
            int _y = (int)y;
            Vector2 d = new Vector2(x - _x, y - _y);

            Matrix4 bicubic_matrix = new Matrix4(new Vector4(Get(_x - 1, _y - 1), Get(_x, _y - 1), Get(_x + 1, _y - 1), Get(_x + 2, _y - 1)),
                                                 new Vector4(Get(_x - 1, _y),     Get(_x, _y),     Get(_x + 1, _y),     Get(_x + 2, _y)),
                                                 new Vector4(Get(_x - 1, _y + 1), Get(_x, _y + 1), Get(_x + 1, _y + 1), Get(_x + 2, _y + 1)),
                                                 new Vector4(Get(_x - 1, _y + 2), Get(_x, _y + 2), Get(_x + 1, _y + 2), Get(_x + 2, _y + 2)));

            return MathUtils.Bicubic(bicubic_matrix, d);
        }

        public float GetFiltered(float x, float y, FilteringType f_type)
        {
            if (f_type == FilteringType.BICUBIC)
                return GetBicubic(x, y);
            if (f_type == FilteringType.BILINEAR)
                return GetBilinear(x, y);
            return GetNearest(x, y);
        }

        public void SetAll(float f)
        {
            for (int i = 0; i < map.Length; i++)
                map[i] = f;
        }

        public void AddAll(float f)
        {
            for (int i = 0; i < map.Length; i++)
                map[i] += f;
        }

        public void MultiplyAll(float f)
        {
            for (int i = 0; i < map.Length; i++)
                map[i] *= f;
        }

        public void ClampAll(float min, float max)
        {
            for (int i = 0; i < map.Length; i++)
                map[i] = Math.Max(Math.Min(max, map[i]), min);
        }

        public void Normalize()
        {
            float min = map[0];
            float max = map[0];
            for(int i=1;i<map.Length;i++)
            {
                if (map[i] < min)
                    min = map[i];
                if (map[i] > max)
                    max = map[i];
            }
            float d = max - min;
            if (d == 0)
                return;
            for (int i = 0; i < map.Length; i++)
                map[i] = (map[i] - min) / d;
        }

        public void ApplyFunction(Func<float, float> f)
        {
            for (int i = 0; i < map.Length; i++)
                map[i] = f(map[i]);
        }

        public void SetMap(GradientMap m, FilteringType f_type)
        {
            float rx = m.Width / Width;
            float ry = m.Height / Height;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Set(x, y, m.GetFiltered(x * rx, y * ry, f_type));
        }

        public void AddMap(GradientMap m, FilteringType f_type)
        {
            float rx = m.Width / Width;
            float ry = m.Height / Height;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Set(x, y, Get(x,y)+m.GetFiltered(x * rx, y * ry, f_type));
        }

        public void MultiplyMap(GradientMap m, FilteringType f_type)
        {
            float rx = (float)m.Width / Width;
            float ry = (float)m.Height / Height;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Set(x, y, Get(x,y)*m.GetFiltered(x * rx, y * ry, f_type));
        }

        public void ApplyKernel(LatticeKernel k)
        {
            float[] new_map = new float[Width * Height];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    new_map[y * Width + x] = k.Get(this, x, y);
            map = new_map;
        }
    }
}
