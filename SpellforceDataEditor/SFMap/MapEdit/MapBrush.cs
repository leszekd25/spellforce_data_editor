using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public enum BrushInterpolationMode { CONSTANT = 0, LINEAR, QUADRATIC, SINUSOIDAL };

    public enum BrushShape { SQUARE = 0, CIRCLE, DIAMOND };

    public class MapBrush
    {
        public float size;
        public BrushInterpolationMode interpolation_mode;
        public BrushShape shape;
        public SFCoord center; 

        public float GetStrengthAt(SFCoord pos)
        {
            float distance;
            switch (shape)
            {
                case BrushShape.SQUARE:
                    distance = SFCoord.DistanceManhattan(center, pos);
                    break;
                case BrushShape.CIRCLE:
                    distance = SFCoord.Distance(center, pos);
                    break;
                case BrushShape.DIAMOND:
                    distance = SFCoord.DistanceDiamond(center, pos);
                    break;
                default:
                    distance = 0;
                    break;
            }
            if (distance >= size)
                return 0;
            float k = distance / size;
            switch (interpolation_mode)
            {
                case BrushInterpolationMode.CONSTANT:
                    return 1;
                case BrushInterpolationMode.LINEAR:
                    return 1 - k;
                case BrushInterpolationMode.QUADRATIC:
                    return 1 - (float)Math.Pow(k, 2);
                case BrushInterpolationMode.SINUSOIDAL:
                    return (float)(Math.Sin(Math.PI * (0.5 - k))+1)/2;
            }
            return 0;
        }
    }
}
