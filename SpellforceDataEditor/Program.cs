using System;
using System.Windows.Forms;

namespace SpellforceDataEditor
{
    static class Program
    {
        /*// this is to force the program to use nvidia gpu when available
        // https://stackoverflow.com/questions/17270429/forcing-hardware-accelerated-rendering
        [System.Runtime.InteropServices.DllImport("nvapi64.dll", EntryPoint = "fake")]
        static extern int LoadNvApi64();

        [System.Runtime.InteropServices.DllImport("nvapi.dll", EntryPoint = "fake")]
        static extern int LoadNvApi32();

        static void InitializeDedicatedGraphics()
        {
            try
            {
                if (Environment.Is64BitProcess)
                    LoadNvApi64();
                else
                    LoadNvApi32();
            }
            catch { } // will always fail since 'fake' entry point doesn't exists
        }*/

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //InitializeDedicatedGraphics();

            SFEngine.LogUtils.Log.SetOption(SFEngine.LogUtils.LogOption.ALL);
            SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.Main, "Program.Main(): session start time: " + DateTime.Now.ToLongTimeString());
            SFEngine.Settings.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
            Application.Run(new MainForm());
#else
            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception e)
            {
                SFEngine.LogUtils.Log.Error(SFEngine.LogUtils.LogSource.Main, "Program.Main() terminated due to error! Exception data: " + e.ToString() + " # " + e.Message);
            }
            finally
            {
                SFEngine.SFLua.SFLuaEnvironment.UnloadSQL();
                SFEngine.SFUnPak.SFUnPak.CloseAllPaks();
                SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.Main, "Program.Main(): session finish time: " + DateTime.Now.ToLongTimeString());
                SFEngine.LogUtils.Log.SaveLog("UserLog.txt");
            }
#endif
        }
    }
}
