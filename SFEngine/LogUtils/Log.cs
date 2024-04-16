/*
 * Log is a singleton which provides methods to provide information, issue warnings and signalize errors
 * AFTER the application closes, Log saves results to a UserLog.txt
 * */

using System;
using System.Collections.Generic;
using System.IO;

namespace SFEngine.LogUtils
{
    // types of messages
    [Flags]
    public enum LogOption { NONE = 0, ERROR = 1, WARNING = 2, INFO = 4, ALL = 7 }

    // sources of messages
    public enum LogSource { SFCFF = 0, SF3D, SFChunkFile, SFLua, SFMap, SFMod, SFResources, SFSound, SFUnPak, Utility, Main, _UNK }

    static public class Log
    {
        // a single log entry type
        struct LogData(LogOption o, LogSource s, string d)
        {
            public LogOption option = o;
            public LogSource source = s;
            public string data = d;
        }

        static LogOption option = LogOption.NONE;                 // message types which will be written to a file
        static readonly List<LogData> log_list = [];      // messages stored here

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

            Info(LogSource.Main, "TotalMemoryUsage(): Logging resource usage");
            SFResources.SFResourceManager.LogMemoryUsage();
            if (SF3D.SFSubModel3D.Cache != null)
            {
                Info(LogSource.Main, "TotalMemoryUsage(): Logging static mesh cache usage");
                SF3D.SFSubModel3D.Cache.LogMemoryUsage();
            }
            if (SF3D.SFModelSkinChunk.Cache != null)
            {
                Info(LogSource.Main, "TotalMemoryUsage(): Logging animated mesh cache usage");
                SF3D.SFModelSkinChunk.Cache.LogMemoryUsage();
            }
        }

        // allows messages of selected type to show in log file
        static public void SetOption(LogOption o)
        {
            option |= o;
        }

        // saves all messages to the file
        static public void SaveLog(string filename)
        {
            using FileStream fs = new(filename, FileMode.Create, FileAccess.Write);
            using StreamWriter sw = new(fs);
            foreach (LogData ld in log_list)
            {
                if ((ld.option & option) != 0)
                {
                    sw.WriteLine("[" + ld.option.ToString() + "] " + ld.source.ToString() + ": " + ld.data);
                }
            }
        }
    }
}
