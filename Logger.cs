using System;
using System.IO;

namespace F5Tools
{
    public class Logger
    {
        private static readonly string _appLocation = AppDomain.CurrentDomain.BaseDirectory ?? @"C:\Temp\A1LBWatch";
        private static string GetLogFileLocation(string fileNamePrefix) => Path.Combine(_appLocation, $"{fileNamePrefix}app-log.txt");

        public Logger()
        {
            if (!Directory.Exists(_appLocation))
                Directory.CreateDirectory(_appLocation);
        }

        public void Log(string logMessage, string logName = "app-log")
        {
            var fileNamePrefix = $"[{DateTime.Today.ToString("MMdd")}]-";
            File.AppendAllText(
                GetLogFileLocation(fileNamePrefix),
                $"[{DateTime.Now.ToString("MM/dd-HH:mm:ss")}]-[{logName.PadBoth(20)}]-{logMessage}{Environment.NewLine}");
        }
    }
}
