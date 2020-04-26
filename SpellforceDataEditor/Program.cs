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
            LogUtils.Log.SetOption(LogUtils.LogOption.ALL);
            LogUtils.Log.Info(LogUtils.LogSource.Main, "Program.Main(): session start time: " + DateTime.Now.ToLongTimeString());
            Settings.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //try
            //{
                Application.Run(new MainForm());
            /*}
            catch (Exception e)
            {
                LogUtils.Log.Error(LogUtils.LogSource.Main, "Program.Main() terminated due to error! Exception data: " + e.ToString() + " # " + e.Message);
            }
            finally
            {
                SFLua.SFLuaEnvironment.UnloadSQL();
                SFUnPak.SFUnPak.CloseAllPaks();
                Settings.Save();
                LogUtils.Log.Info(LogUtils.LogSource.Main, "Program.Main(): session finish time: " + DateTime.Now.ToLongTimeString());
                LogUtils.Log.SaveLog("UserLog.txt");
            }*/
        }
    }
}
