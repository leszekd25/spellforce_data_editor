using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor
{
    public static class Settings
    {
        public static int IgnoredMipMapsCount { get; private set; } = 0;
        public static int MaximumAllowedTextureSize { get; private set; } = 2048;

        public static string GameDirectory { get; set; } = "";
        public static int LanguageID { get; private set; } = 1;

        public static bool LogInfo { get; private set; } = true;
        public static bool LogWarning { get; private set; } = true;
        public static bool LogError { get; private set; } = true;

        public static bool AllowLua { get; private set; } = true;
        public static int EnableLuaRunWarning { get; private set; } = 3;  // 3- YES, 2 - ONCE, 1 - ONCE (warned), 0 - NO

        public static int Load()
        {
            LogUtils.Log.Info(LogUtils.LogSource.Main, "Settings.Load() called");

            try
            {
                string[] settings = File.ReadAllLines("config.txt");
                bool ignore_rest = false;
                foreach(string s in settings)
                {
                    if (s.Length == 0)
                        continue;
                    if (s[0] == '#')
                        continue;

                    string[] words = s.Split(' ');
                    if (words.Length < 2)
                        continue;

                    switch(words[0])
                    {
                        case "!":
                            if (words[1] == "Ignore")
                                ignore_rest = true;
                            break;
                        case "IgnoredMipMapsCount":
                            IgnoredMipMapsCount = Utility.TryParseUInt8(words[1], (byte)IgnoredMipMapsCount);
                            if (IgnoredMipMapsCount > 2)
                                IgnoredMipMapsCount = 2;
                            break;
                        case "MaximumAllowedTextureSize":
                            MaximumAllowedTextureSize = Utility.TryParseUInt16(words[1], (ushort)MaximumAllowedTextureSize);
                            if (MaximumAllowedTextureSize < 64)
                                MaximumAllowedTextureSize = 64;
                            break;
                        case "GameDirectory":
                            string s2 = words[1];
                            for (int i = 2; i < words.Length; i++)
                                s2 += " " + words[i];
                            GameDirectory = s2;
                            SFUnPak.SFUnPak.SpecifyGameDirectory(GameDirectory);
                            break;
                        case "LogInfo":
                            LogInfo = (words[1] == "YES");
                            if (LogInfo)
                                LogUtils.Log.SetOption(LogUtils.LogOption.INFO);
                            break;
                        case "LogWarning":
                            LogWarning = (words[1] == "YES");
                            if (LogWarning)
                                LogUtils.Log.SetOption(LogUtils.LogOption.WARNING);
                            break;
                        case "LogError":
                            LogError = (words[1] == "YES");
                            if (LogError)
                                LogUtils.Log.SetOption(LogUtils.LogOption.ERROR);
                            break;
                        case "AllowLua":
                            AllowLua = (words[1] == "YES");
                            break;
                        case "EnableLuaRunWarning":
                            if (words[1] == "YES")
                                EnableLuaRunWarning = 3;
                            else if (words[1] == "ONCE")
                                EnableLuaRunWarning = 2;
                            else
                                EnableLuaRunWarning = 0;
                            break;
                        case "LanguageID":
                            LanguageID = Utility.TryParseUInt8(words[1], (byte)LanguageID);
                            if (LanguageID > 4)
                                LanguageID = 1;
                            break;
                    }
                    if (ignore_rest)
                        break;
                }
            }
            catch(Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.Main, "Settings.Load(): failed to load settings from config.txt");
                return -1;
            }

            return 0;
        }

        public static void Save()
        {
            LogUtils.Log.Info(LogUtils.LogSource.Main, "Settings.Save() called");

            try
            {
                string[] settings = File.ReadAllLines("config.txt");
                List<string> new_settings = new List<string>();
                foreach (string s in settings)
                {
                    if(s.Length == 0)
                    {
                        new_settings.Add("");
                        continue;
                    }
                    string[] words = s.Split(' ');
                    if ((words.Length == 0) || (words[0].Length == 0))
                    {
                        new_settings.Add("");
                        continue;
                    }
                    if((s[0] == '#')||(s[0] == '!'))
                    {
                        new_settings.Add(s);
                        continue;
                    }

                    bool skipped_line = false;
                    switch (words[0])
                    {
                        case "IgnoredMipMapsCount":
                            words = new string[] { words[0], IgnoredMipMapsCount.ToString() };
                            break;
                        case "MaximumAllowedTextureSize":
                            words = new string[] { words[0], MaximumAllowedTextureSize.ToString() };
                            break;
                        case "GameDirectory":
                            words = new string[] { words[0], GameDirectory };
                            break;
                        case "LogInfo":
                            words = new string[] { words[0], LogInfo ? "YES" : "NO" };
                            break;
                        case "LogWarning":
                            words = new string[] { words[0], LogWarning ? "YES" : "NO" };
                            break;
                        case "LogError":
                            words = new string[] { words[0], LogError ? "YES" : "NO" };
                            break;
                        case "AllowLua":
                            words = new string[] { words[0], AllowLua ? "YES" : "NO" };
                            break;
                        case "EnableLuaRunWarning":
                            words = new string[] { words[0], (EnableLuaRunWarning == 3 ? "YES" : (EnableLuaRunWarning == 0 ? "NO" : "ONCE")) };
                            break;
                        case "LanguageID":
                            words = new string[] { words[0], LanguageID.ToString() };
                            break;
                        default:
                            new_settings.Add(s);
                            skipped_line = true;
                            break;
                    }
                    if (skipped_line)
                        continue;
                    string s2 = words[0] + " " + words[1];
                    new_settings.Add(s2);
                }
                File.WriteAllLines("config.txt", new_settings.ToArray());
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.Main, "Settings.Save(): failed to save settings to config.txt");
            }
        }

        public static bool ConfirmRunLua(string fname)
        {
            if (EnableLuaRunWarning < 2)
                return true;
            if (EnableLuaRunWarning == 2)
                EnableLuaRunWarning -= 1;
            DialogResult dr = MessageBox.Show("Editor will now run Lua script '" + fname + "'. Press OK to continue, Cancel to abort.", "Allow running Lua script", MessageBoxButtons.OKCancel);
            return dr == DialogResult.OK;
        }
    }
}
