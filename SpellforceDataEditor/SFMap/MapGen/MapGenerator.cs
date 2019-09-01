using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SFMap.MapGen
{
    public class MapGenerator
    {
        public int Width;
        public int Height;

        public int GradientCellSizeX;
        public int GradientOffsetX;
        public float GradientErosionMeanX;
        public float GradientErosionSigmaX;
        public int GradientCellSizeY;
        public int GradientOffsetY;
        public float GradientErosionMeanY;
        public float GradientErosionSigmaY;
        public int GradientBlurSize;
        public float GradientBlurSigma;

        public ushort BaseZ;

        private float ApplySigmoid(float m)
        {
            float k = (float)MathUtils.Sigmoid(m * 10);
            return (k-0.5f)*2;
        }

        public ushort[] ProduceHeightmap()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "MapGenerator.ProduceHeightmap() called");
            GradientMap base_map = new GradientMap(Width, Height);
            base_map.SetAll(1);
            int gradient_width = Width / GradientCellSizeX;
            int gradient_height = Height / GradientCellSizeY;
            GradientMap gradient_map = new GradientMap(gradient_width + 1, gradient_height + 1);
            gradient_map.SetAll(1);

            // apply offset
            for (int y = 0; y < gradient_map.Height; y++)
            {
                for (int x = 0; x < GradientOffsetX; x++)
                {
                    gradient_map.Set(x, y, 0);
                    gradient_map.Set(gradient_width - x, y, 0);
                }
            }

            for (int y = 0; y < GradientOffsetY; y++)
            {
                for (int x = 0; x < gradient_map.Width; x++)
                {
                    gradient_map.Set(x, y, 0);
                    gradient_map.Set(x, gradient_height - y, 0);
                }
            }

            // generate erosion

            for(int  y = 0;  y  <  gradient_map.Height;  y++)
            {
                Vector2 horizontal_erosion = MathUtils.GaussRand(GradientErosionMeanX, GradientErosionSigmaX);
                SFCoord fixed_he = new SFCoord((int)horizontal_erosion.X, (int)horizontal_erosion.Y);
                if (fixed_he.x > 0)
                    for (int x = 0; x < fixed_he.x; x++)
                        gradient_map.Set(GradientOffsetX+x, y, 0);
                if (fixed_he.y > 0)
                    for (int x = 0; x < fixed_he.y; x++)
                        gradient_map.Set(gradient_width - GradientOffsetX - x, y, 0);
            }

            for(int x=0;x<gradient_map.Width;x++)
            {
                Vector2 vertical_erosion = MathUtils.GaussRand(GradientErosionMeanY, GradientErosionSigmaY);
                SFCoord fixed_ve = new SFCoord((int)vertical_erosion.X, (int)vertical_erosion.Y);
                if (fixed_ve.x > 0)
                    for (int y = 0; y < fixed_ve.x; y++)
                        gradient_map.Set(x, GradientOffsetY + y, 0);
                if (fixed_ve.y > 0)
                    for (int y = 0; y < fixed_ve.y; y++)
                        gradient_map.Set(x, gradient_height - GradientOffsetY - y, 0);
            }

            // blur gradient map  using  gaussian  kernel
            LatticeKernel kernel = new GaussKernel(GradientBlurSize, GradientBlurSigma);
            gradient_map.ApplyKernel(kernel);

            // apply gradient map to  height map
            base_map.MultiplyMap(gradient_map, FilteringType.BICUBIC);

            // clamp map
            //base_map.AddAll(-0.5f);
            //base_map.MultiplyAll(2);
            //base_map.ClampAll(0, 1.5f);
            base_map.ClampAll(0, 1.5f);
            base_map.ApplyFunction(ApplySigmoid);
            base_map.AddAll(-0.5f);
            base_map.MultiplyAll(2);
            base_map.ClampAll(0, 1.5f);
            base_map.ApplyFunction(ApplySigmoid);

            // generate  height  map  from the  map
            ushort[] ret = new ushort[Width * Height];
            ParallelOptions loop_options = new ParallelOptions();
            loop_options.MaxDegreeOfParallelism = 4;
            int height_per_task = Height / 4;
            Parallel.For(0, 4, (i) =>
            {
                int end = height_per_task * (i + 1);
                for (int y = height_per_task * i; y < end; y++)
                    for (int x = 0; x < Width; x++)
                        ret[y * Width + x] = (ushort)(BaseZ * base_map.Get(x, y));
            });
            //for (int y = 0; y < Height; y++)
            //    for (int x = 0; x < Width; x++)
            //        ret[y * Width + x] = (ushort)(BaseZ*base_map.Get(x, y));

            return ret;
        }
    }
}
