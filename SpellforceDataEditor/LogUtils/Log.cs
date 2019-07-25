/*
 * Log is a singleton which provides methods to provide information, issue warnings and signalize errors
 * AFTER every program execution, Log saves results to a UserLog.txt
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.LogUtils
{
    // types of messages
    [Flags]
    public enum LogOption { NONE = 0, ERROR = 1, WARNING = 2, INFO = 4, ALL = 7 }

    // sources of messages
    public enum LogSource { SFCFF = 0, SF3D, SFChunkFile, SFLua, SFMap, SFMod, SFResources, SFSound, SFUnPak, Utility, Main, _UNK}

    static public class Log
    {
        // a single log entry type
        struct LogData
        {
            public LogOption option;
            public LogSource source;
            public string data;

            public LogData(LogOption o, LogSource s, string d) { option = o; source = s; data = d; }
        }

        static LogOption option = LogOption.NONE;                 // message types which will be written to a file
        static List<LogData> log_list = new List<LogData>();

        static public void Info(LogSource s, string d)
        {
            log_list.Add(new LogData(LogOption.INFO, s, d));
        }

        static public void Warning(LogSource s, string d)
        {
            log_list.Add(new LogData(LogOption.WARNING, s, d));
        }

        static public void Error(LogSource s, string d)
        {
            log_list.Add(new LogData(LogOption.ERROR, s, d));
        }

        // this is slow! only use sporadically
        static public void MemoryUsage()
        {
            log_list.Add(new LogData(LogOption.INFO, LogSource.Main, "MemoryUsage(): \r\n    Current   total memory used: "
                + System.Diagnostics.Process.GetCurrentProcess().WorkingSet64.ToString() + " bytes \r\n    Current managed memory used: "
                + GC.GetTotalMemory(true).ToString() + " bytes"));
        }

        static public void SetOption(LogOption o)
        {
            option |= o;
        }

        static public void SaveLog(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                foreach (LogData ld in log_list)
                {
                    if((ld.option & option) != 0)
                        sw.WriteLine("["+ld.option.ToString()+"] "+ld.source.ToString() + ": " + ld.data);
                }
            }
            fs.Close();
        }
    }
}
