using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AltaTransport
{
    public class EDOTransport
    {
        private static string path = "C:\\EDIMail\\";


        internal static string GetEDODirectory()
        {
            return path;
        }


        internal static List<FileInfo> GetNewReports()
        {
            string[] fileNames = GetNewReportsWithoutCopy();
            return CopyReports(fileNames);
        }


        //private static string[] GetNewReportsWithoutCopy()
        //{
        //    if (Directory.Exists(path)) {
        //        string[] filenames = Directory.GetFiles(path, "IPO_RPT_*.xml");
        //        return filenames;
        //    }
        //    return new string[0];
        //}


        private static string[] GetNewReportsWithoutCopy()
        {
            if (Directory.Exists(path)) {
                string[] filenames = Directory.GetFiles(path, "IPO_RPT_2");
                return filenames;
            }
            return new string[0];
        }


        private static List<FileInfo> CopyReports(string[] filenames)
        {
            List<FileInfo> reportFileInfos = new List<FileInfo>();
            foreach(string fileName in filenames) {
                FileInfo reportFileInfo = CopyReport(fileName);
                if(reportFileInfo!=null) reportFileInfos.Add(reportFileInfo);
            }
            return reportFileInfos;
        }


        private static FileInfo CopyReport(string filename)
        {
            string outFileName = FileArchiveTransport.GetIncomingReportFileName(filename.Substring(filename.LastIndexOf("\\") + 1));
            try {
                if (File.Exists(outFileName)) File.Delete(outFileName);
                File.Move(filename, outFileName);
            } catch (Exception ex) {
                Debug.WriteLine("EDOTransport: CopyReport: " + ex.Message);
                return null;
            }
            FileInfo info = new FileInfo(outFileName);
            return info;
        }

    }
}
