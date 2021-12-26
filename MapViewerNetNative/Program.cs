using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

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
