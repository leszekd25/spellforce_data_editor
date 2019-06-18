using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SpellforceDataEditor.SFMap
{
    public struct SFCoord : IComparable
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

        public static SFCoord operator*(SFCoord c, int q)
        {
            return new SFCoord(c.x * q, c.y * q);
        }

        public static bool operator ==(SFCoord c1, SFCoord c2)
        {
            return ((c1.x == c2.x) && (c1.y == c2.y));
        }

        public static bool operator !=(SFCoord c1, SFCoord c2)
        {
            return !(c1 == c2);
        }

        public static bool operator <(SFCoord c1, SFCoord c2)
        {
            if (c1.x < c2.x)
                return true;
            else if (c1.x == c2.x)
                return (c1.y < c2.y);
            else
                return false;
        }

        public static bool operator >(SFCoord c1, SFCoord c2)
        {
            if (c1.x > c2.x)
                return true;
            else if (c1.x == c2.x)
                return (c1.y > c2.y);
            else
                return false;
        }

        public int CompareTo(object o)
        {
            SFCoord pos = (SFCoord)o;
            if (this < pos)
                return -1;
            if (this == pos)
                return 0;
            return 1;
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

        public static int DistanceManhattan(SFCoord p1, SFCoord p2)
        {
            return Math.Max(Math.Abs(p1.x - p2.x), Math.Abs(p1.y - p2.y));
        }

        public static int DistanceDiamond(SFCoord p1, SFCoord p2)
        {
            return Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
        }

        public static float Distance(SFCoord p1, SFCoord p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
        }

        public static int DistanceSquared(SFCoord p1, SFCoord p2)
        {
            return ((p1.x - p2.x) * (p1.x - p2.x)) + ((p1.y - p2.y) * (p1.y * -p2.y));  
        }

        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ")";
        }
    }
}
