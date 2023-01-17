using System;

namespace MapViewerNetNative
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MapViewerWindow mew = new MapViewerWindow();

            mew.Run();
        }

    }
}
