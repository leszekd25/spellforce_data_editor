using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SFMap
{
    public struct SFCoord
    {
        public int x;
        public int y;

        public SFCoord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static SFCoord operator +(SFCoord c1, SFCoord c2)
        {
            return new SFCoord(c1.x + c2.x, c1.y + c2.y);
        }

        public static SFCoord operator -(SFCoord c1, SFCoord c2)
        {
            return new SFCoord(c1.x - c2.x, c1.y - c2.y);
        }

        public static bool operator==(SFCoord c1, SFCoord c2)
        {
            return ((c1.x == c2.x) && (c1.y == c2.y));
        }

        public static bool operator!=(SFCoord c1, SFCoord c2)
        {
            return !(c1 == c2);
        }

        public static explicit operator SFCoord(Vector2 v)
        {
            return new SFCoord((int)v.X, (int)v.Y);
        }

        public static explicit operator Vector2(SFCoord c)
        {
            return new Vector2((float)c.x, (float)c.y);
        }

        public bool InRect(int x1, int y1, int x2, int y2)
        {
            return ((x >= x1) && (x <= x2) && (y >= y1) && (y <= y2));
        }

        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ")";
        }
    }
}
