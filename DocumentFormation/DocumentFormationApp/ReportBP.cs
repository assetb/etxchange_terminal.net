using System;
using System.Linq;
using AltaTransport;
using AltaTransport.model;
using DocumentFormationServices;

namespace DocumentFormation
{
    /// <summary>
    /// API functions working over reports
    /// </summary>
    public static class ReportBP
    {
        /// <summary>
        /// Makes and Returns new reports from single file
        /// </summary>
        /// <returns></returns>
        public static int MakeNewReport(string reportFile)
        {
            var reportCount = 0;
            string targetFileName = "";

            foreach (var report in ETSReportConverter.GetReports(reportFile)) {
                targetFileName = FileArchiveTransport.GetOutcomingReportFileName((string.Equals(report.Id.ToLower(), "altk", StringComparison.OrdinalIgnoreCase) ?
                    DFConfig.ClientReportFileName : DFConfig.SupplierReportFileName +
                    report.ClientName.Replace("'", " ").Trim().Replace("\"", "") + " по лоту ") + report.Code + ".docx", report.Id, report.Code);

                //ReportService.CreateReport(targetFileName, report);

                reportCount++;
            }

            return reportCount;
        }


        public static int MakeNewReport(string reportFile, int type)
        {
            var reportCount = 0;
            string targetFileName = "";

            foreach (var report in ETSReportConverter.GetReports(reportFile)) {
                targetFileName = FileArchiveTransport.GetOutcomingReportFileName((type == 1 ? DFConfig.ClientReportFileName : DFConfig.SupplierReportFileName +
                    report.ClientName.Replace("'", " ").Trim().Replace("\"", "") + " по лоту ") + report.Code + ".docx", report.Id, report.Code);

                ReportService.CreateReport(targetFileName, report, type);

                reportCount++;
            }

            return reportCount;
        }


        /// <summary>
        /// Makes and Returns new reports from report document
        /// </summary>
        /// <returns></returns>
        public static int MakeNewReports(ReportDocument reportDocument)
        {
            return reportDocument.ReportFileNames.Sum(reportDocumentReportFileName => MakeNewReport(reportDocumentReportFileName.FullName));
        }


        /// <summary>
        /// Makes and Returns new reports from single stage of reading external system giving reports
        /// </summary>
        /// <returns></returns>
        public static int MakeNewReports()
        {
            return ReportTransport.GetNew().Sum(MakeNewReports);
        }


        public static string GetReportsPath()
        {
            return FileArchiveTransport.GetReportPath();
        }

    }
}