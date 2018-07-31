using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AltaLog;
using AltaTransport.model;

namespace AltaTransport
{
    /// <summary>
    /// Functions to transport reports to different locations.
    /// Current version uses EDO client to get report from ETS exchange. EDO clients can be more than one.
    /// </summary>
    public static class ReportTransport
    {
        private static List<EDOClient> _edoClients;


        //public ReportTransport()
        //{
        //    _edoClients = new List<EDOClient>();
        //    foreach (var path in EDOConfig.Pathes)
        //    {
        //        var edoClient = new EDOClient(path);
        //        _edoClients.Add(edoClient);
        //    }
        //}


        public static bool HasNew()
        {
            if (_edoClients == null) {
                _edoClients = new List<EDOClient>();
                foreach (var path in EDOConfig.Pathes) {
                    var edoClient = new EDOClient(path);
                    _edoClients.Add(edoClient);
                }
            }

            return _edoClients.Any(edoClient => edoClient.HasNew());
        }
        
        
        /// <summary>
        /// Returns new reports come from Exchange. Reports are converted to Report BO.
        /// Current version gets reports from EDO which is method to connect with ETS exchange.
        /// </summary>
        /// <returns>List of new reports as BO</returns>
        public static List<ReportDocument> GetNew() {
            return _edoClients?.Select(edoClient => new ReportDocument() {ReportFileNames = edoClient.GetNew()}).ToList();
        }


        /// <summary>
        /// Puts incoming report files to archive.
        /// </summary>
        public static void Save()
        {
            //var reportFileInfos = new List<FileInfo>();
            //foreach(string fileName in _files) {
            //    FileInfo reportFileInfo = CopyReport(fileName);
            //    if(reportFileInfo!=null) reportFileInfos.Add(reportFileInfo);
            //}
            //return reportFileInfos;

            if (_edoClients == null) return;

            foreach (var edoClient in _edoClients)
            {
                CopyReport(edoClient.Files);
            }
        }


        /// <summary>
        /// Copies income report files to archive.
        /// </summary>
        /// <param name="files"></param>
        public static void CopyReport(string[] files)
        {
            foreach (var file in files)
            {
                CopyReport(file);
            }

        }


        /// <summary>
        /// Returns new reports come from Exchange.
        /// Current vertion gets reports from EDO which is method to connect with ETS exchange.
        /// </summary>
        /// <returns>files of ETS reports</returns>
        public static List<FileInfo> GetNewFiles()
        {
            var reports = new List<FileInfo>();
            foreach (var path in EDOConfig.Pathes)
            {
                var edoClient = new EDOClient(path);
                reports.AddRange(edoClient.GetNew());
            }
            return reports;
        }


        /// <summary>
        /// Copies single income report file to archive.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>file to which copied</returns>
        public static FileInfo CopyReport(string filename)
        {
            var outFileName = FileArchiveTransport.GetIncomingReportFileName(filename.Substring(filename.LastIndexOf(@"\", StringComparison.Ordinal) + 1));
            try {
                if (File.Exists(outFileName)) File.Delete(outFileName);
                File.Move(filename, outFileName);
            } catch (Exception ex) {
                AppJournal.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,"ReportTransport: CopyReport: " + ex.Message);
                return null;
            }
            var info = new FileInfo(outFileName);
            return info;
        }
    }
}
