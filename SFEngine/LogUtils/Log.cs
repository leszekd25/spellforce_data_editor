/*
 * Log is a singleton which provides methods to provide information, issue warnings and signalize errors
 * AFTER the application closes, Log saves results to a UserLog.txt
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.LogUtils
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
        static List<LogData> log_list = new List<LogData>();      // messages stored here

        // these three are utility functions

        // logs message of type INFO
        static public void Info(LogSource s, string d)
        {
            log_list.Add(new LogData(LogOption.INFO, s, d));
        }

        // logs message of type WARNING
        static public void Warning(LogSource s, string d)
        {
            log_list.Add(new LogData(LogOption.WARNING, s, d));
        }

        // logs message of type ERROR
        static public void Error(LogSource s, string d)
        {
            log_list.Add(new LogData(LogOption.ERROR, s, d));
        }

        // this is slow! only use sporadically
        static public void TotalMemoryUsage()
        {
            log_list.Add(new LogData(LogOption.INFO, LogSource.Main, "TotalMemoryUsage(): \r\n    Current total memory used: "
                + System.Diagnostics.Process.GetCurrentProcess().WorkingSet64.ToString() + " bytes \r\n    Current managed memory used: "
                + GC.GetTotalMemory(true).ToString() + " bytes"));
        }

        // allows messages of selected type to show in log file
        static public void SetOption(LogOption o)
        {
            option |= o;
        }

        // saves all messages to the file
        static public void SaveLog(string filename)
        {
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
