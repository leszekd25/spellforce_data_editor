using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFMap.MapGen
{
    public class LatticeKernel
    {
        public int Size { get; protected set; } = 0;

        public LatticeKernel(int size)
        {
            Size = size;
        }

        public virtual void Generate()
        {

        }

        public virtual float Get(GradientMap m, int x, int y)
        {
            return m.Get(x, y);
        }
    }

    public class SeparableKernel: LatticeKernel
    {
        protected float[] k_vector;

        public SeparableKernel(int size):  base(size)
        {
            k_vector = new float[size * 2 + 1];
        }

        public override float Get(GradientMap m, int  x, int y)
        {
            float sum = 0;
            int diameter = Size * 2 + 1;
            for (int i = 0; i < diameter; i++)
                for (int j = 0; j < diameter; j++)
                    sum += m.Get(x + i - Size, y + j - Size) * k_vector[i] * k_vector[j];
            return sum;
        }
    }

    public class GaussKernel: SeparableKernel
    {
        private float sigma = 1;

        public GaussKernel(int size, float s): base(size)
        {
            sigma = s;
            Generate();
        }

        public override void Generate()
        {
            if(Size == 0)
            {
                k_vector = new float[1] { 1.0f };
                return;
            }
            k_vector = new float[Size * 2 + 1];
            float sum = 0;
            for (int i = -Size; i <= Size; i++)
            {
                k_vector[i + Size] = (float)MathUtils.GaussianDensity(i, sigma);
                sum += k_vector[i + Size];
            }
            for (int i = -Size; i <= Size; i++)
                k_vector[i + Size] /= sum;
        }
    }
}
