using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace AltaLog {
    public class AppJournal {
        public static string Source { get; } = "AltaVisionService";
        public static string Name { get; } = "Приложение Alta Служба Надзора";
        public static bool HasWinJournal { get; set; } = false;
        public static bool HasLogFile { get; set; } = true;

        private static bool _isWinJournalExist = true;
        private static bool _isLogFileExist = false;
        public static string logFileName = "serverApp.log";

        public static void Write(string message) {
            if(HasWinJournal)
                if(!_isWinJournalExist)
                    try {
                        if(!EventLog.SourceExists(Source, ".")) {
                            EventLog.CreateEventSource(Source, Name);
                            _isWinJournalExist = true;
                        }
                    } catch(SecurityException sex) {
                        Debug.WriteLine("Security Excepton when creating Log." + sex.Message);
                        try {
                            EventLog.CreateEventSource(Source, Name);
                            _isWinJournalExist = true;
                        } catch(Exception ex) {
                            Debug.WriteLine(ex.Message);
                        }
                    } catch(Exception ex) {
                        Debug.WriteLine(ex.Message);
                    } else
                    try {
                        EventLog.WriteEntry(Source, message);
                    } catch(Exception ex) {
                        Debug.WriteLine(ex.Message);
                    }

            if(HasLogFile) {
                if(!_isLogFileExist) {
                    File.Create(logFileName);
                    _isLogFileExist = true;
                } else {
                    File.AppendAllLines(logFileName, new string[1] { message });
                }
            } else {
                Debug.WriteLine(message);
            }
        }


        public static void Write(string sender, string message, bool withDateTime = false) {
            if(!Directory.Exists("C:\\temp")) Directory.CreateDirectory("C:\\temp");

            string fileName = "C:\\temp\\serverApp_" + DateTime.Now.ToShortDateString().Replace(".", "_") + ".log";

            File.AppendAllLines(fileName, new string[1] { DateTime.Now.ToString() + "| " + sender + ": " + message});
        }
    }
}
