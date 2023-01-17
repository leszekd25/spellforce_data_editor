
using OpenTK;

namespace SFEngine.SF3D
{
    static public class ColorUtility
    {
        // converts R5G6B5 to RGB8
        static public Vector4 ConvertColor16toV32(ushort col)
        {
            byte r = (byte)((col >> 11) << 3);
            byte g = (byte)((col >> 5) << 2);
            byte b = (byte)(col << 3);
            return new Vector4(r, g, b, 255) / 255;
        }

        static public ushort ConvertColorV32To16(Vector4 color)
        {
            byte r = (byte)((uint)(color.X * 255) / 8);
            byte g = (byte)((uint)(color.Y * 255) / 4);
            byte b = (byte)((uint)(color.Z * 255) / 8);
            return (ushort)(r << 11 + g << 5 + b);
        }
    }
}
