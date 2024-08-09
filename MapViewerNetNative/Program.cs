using System;

namespace MapViewerNetNative
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            MapViewerWindow mew2 = new MapViewerWindow();
            mew2.Run();

            //MapViewerWindow mew = new MapViewerWindow();

            //mew.Run();
        }

    }
}
