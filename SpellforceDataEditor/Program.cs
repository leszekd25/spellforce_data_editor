using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
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
                SFLua.SFLuaEnvironment.UnloadSQL();
                SFUnPak.SFUnPak.CloseAllPaks();
                SFEngine.LogUtils.Log.Info(SFEngine.LogUtils.LogSource.Main, "Program.Main(): session finish time: " + DateTime.Now.ToLongTimeString());
                SFEngine.LogUtils.Log.SaveLog("UserLog.txt");
            }
#endif
        }
    }
}
