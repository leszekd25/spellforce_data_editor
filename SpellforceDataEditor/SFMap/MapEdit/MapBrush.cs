using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public enum BrushShape { SQUARE = 0, CIRCLE, DIAMOND };

    public class MapBrush
    {
        public float size;
        public BrushShape shape;
        public SFCoord center;

        // 0 at center, 1 outside of brush
        public float GetInvertedDistanceNormalized(SFCoord pos)
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
            if (distance > (size - 1))
                return 1;
            float k = distance / (size - 1);
            return k;
        }
    }
}
