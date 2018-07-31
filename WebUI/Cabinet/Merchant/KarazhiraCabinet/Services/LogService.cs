using System;
using System.IO;

namespace KarazhiraCabinet.Services
{
    public static class LogService
    {
        public static void LogInfo(string msg)
        {
            var sw = File.AppendText("C:\\temp\\" + DateTime.Now.ToShortDateString() + "_log.log");

            try {
                var logLine = string.Format("{0:G}: {1}.", DateTime.Now, msg);
                sw.WriteLine(logLine);
            } finally { sw.Close(); }
        }
    }
}