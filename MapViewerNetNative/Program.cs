using System;

namespace MapViewerNetNative
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            MapViewerWindow mew = new MapViewerWindow();

            mew.Run();
        }

    }
}
