using System;
using System.IO;
using AltaLog;
using AltaBO;
using AltaBO.specifics;
using System.Runtime.Serialization.Formatters.Binary;

namespace AltaTransport {
    public class FileArchiveTransport {
        private static PathSettings pathSettings = LoadConfiguration();


        private static string rootPath = @"\\192.168.11.5\Archive\";

        //private static String rootNetPath = "\\\\10.1.1.2\\Change\\Archive\\";
        //private static String rootPath = "C:\\Archive\\";
        //private static String baseReportPath = rootPath + "EDO\\Reports\\";
        //private static String baseOrderPath = rootPath + "Order\\";
        //private static String baseReportTemplatePath = baseReportPath + "Templates\\";
        //private static String baseJournalPath = rootNetPath + "JournalC01\\";
        //private static String baseFormC01TemplatePath = baseJournalPath + "Templates\\";
        //private static String baseEntryOrderPath = "EntryOrder\\";
        //private static String baseEntryOrderTemplatePath = baseEntryOrderPath + "Templates\\";


        /*public static void SaveConfiguration(PathSettings savedPathSettings)
        {
            try {
                using (Stream stream = File.Open("pathSettings.ini", FileMode.Create)) {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, savedPathSettings);
                }
            }
            catch {
                AppJournal.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,"Problems with loading settings from file.");
            }
        }*/


        public static PathSettings LoadConfiguration() {
            /*if (File.Exists("pathSettings.ini"))
                using (Stream stream = File.Open("pathSettings.ini", FileMode.Open)) {
                    var binaryFormatter = new BinaryFormatter();
                    return (PathSettings) binaryFormatter.Deserialize(stream);
                }*/

            pathSettings = new PathSettings {
                EDOPath = @"\\10.1.1.2\Change\Archive\EDO\In\",
                EDOReportsPath = @"\\10.1.1.2\Change\Archive\EDO\",
                EntryOrdersPath = "",
                JournalC01Path = @"\\10.1.1.2\Change\Munira\",
                OrdersPath = "",
                RootPath = @"\\10.1.1.2\Change\Archive",
                TemplatesPath = @"\\10.1.1.2\Change\Archive\Templates\"
            };

            //SaveConfiguration(pathSettings);
            //pathSettings = LoadConfiguration();

            return pathSettings;
        }


        public static void ReloadConfig(PathSettings pSettings) {
            pathSettings = pSettings;
        }


        public static string GetIncomingReportFileName(string fileName) {
            return GetReportPath() + fileName;
        }


        public static string GetOutcomingReportFileName(string shortFileName, string brokerCode, string lotCode = "") {
            string reportFileName = "";

            if(string.IsNullOrEmpty(lotCode)) reportFileName = GetReportPath() + shortFileName;
            else reportFileName = GetReportPathByLot(lotCode) + shortFileName;

            try {
                if(shortFileName.Contains("поставщику")) File.Copy(rootPath + "\\Templates\\ETS\\ReportToSupplier" + brokerCode.ToLower() + ".docx", reportFileName, true);
                else File.Copy(rootPath + "\\Templates\\ETS\\ReportToClient.docx", reportFileName, true);
            } catch(Exception ex) {
                AppJournal.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, "FileArhive: " + ex.Message);
            }

            return reportFileName;
        }


        public static string GetReportPathByLot(string lotCode) {
            var lot = DataBaseClient.GetLotByCode(lotCode);

            if(lot != null) {
                string folderPath = "\\\\192.168.11.5\\Archive\\Auctions\\ETS\\";
                string oldPath = folderPath + lot.auction.date.ToShortDateString() + "\\" + (lot.auction.number.Length > 4 ? lot.auction.number.Substring(lot.auction.number.Length - 4) : lot.auction.number).Replace("/", "_");
                string newPath = folderPath + lot.auction.date.ToShortDateString() + "\\" + lot.auction.number.Replace("/", "_");
                string path = Directory.Exists(oldPath) ? oldPath : newPath;

                return path + "\\";
            } else return GetReportPath();
        }


        public static string GetEDOPath() {
            return pathSettings.EDOPath;
        }


        public static string GetReportPath() {
            pathSettings.EDOReportsPath = rootPath + "\\Reports\\";

            var reportPath = pathSettings.EDOReportsPath + DateTime.Today.ToShortDateString() + "\\";
            try {
                if(!Directory.Exists(reportPath)) Directory.CreateDirectory(reportPath);
            } catch(Exception ex) {
                //Debug.WriteLine("FileArchive: GetReportPath: " + ex.Message);
                AppJournal.Write("FileArchive: GetReportPath: " + ex.Message);
                reportPath = ".\\";
            }
            return reportPath;
        }


        public static string GetCustomerOrderPath() {
            var orderPath = pathSettings.OrdersPath + DateTime.Today.ToShortDateString() + "\\";
            try {
                if(!Directory.Exists(orderPath)) Directory.CreateDirectory(orderPath);
            } catch(Exception ex) {
                //Debug.WriteLine("FileArchive: GetOrderPath: " + ex.Message);
                AppJournal.Write("FileArchive: GetOrderPath: " + ex.Message);
                orderPath = ".\\";
            }
            return orderPath;
        }

        public static string GetTemplatesPath(MarketPlaceEnum marketPlaceEnum = MarketPlaceEnum.ETS) {
            if(marketPlaceEnum == MarketPlaceEnum.ETS) return rootPath + @"\Templates\ETS\";
            else if(marketPlaceEnum == MarketPlaceEnum.UTB) return rootPath + @"\Templates\UTB\";
            else return pathSettings.TemplatesPath;
        }

        public static string GetReportTemplatePath() {
            return pathSettings.EDOReportsPath + pathSettings.TemplatesPath;
        }


        public static string GetClientReportTemplateFileName() {
            return pathSettings.TemplatesPath + "\\" + "reportToClient";
        }


        public static string GetSupplierReportTemplateFileName() {
            return pathSettings.TemplatesPath + "\\" + "reportToSupplier";
        }


        public static string GetJournalC01FileName() {
            return pathSettings.JournalC01Path + "\\" + "Журнал C01.xlsx";
        }


        public static string GetFormC01TemplateFileName() {
            return pathSettings.TemplatesPath + "\\" + "formC01.docx";
        }


        public static string GetEntryOrderTemplateFileName() {
            return pathSettings.TemplatesPath + "\\" + "order_ets_entry.docx";
        }


        public static string GetAttachToDealTemplateFileName() {
            return pathSettings.TemplatesPath + "\\" + "attachToDeal.xlsx";
        }


        public static string GetCommissionToDealTemplateFileName() {
            return pathSettings.TemplatesPath + "\\" + "commissionToDeal.docx";
        }


        public static string GetMoneyTransferTemplateFileName() {
            return pathSettings.TemplatesPath + "\\" + "moneyTransfer.docx";
        }


        public static string GetWaitingListTemplateFileName() {
            return pathSettings.TemplatesPath + "\\" + "waitingList.docx";
        }


        public static string GetOrderTemplatePath() {
            return pathSettings.TemplatesPath;
        }


        public static string GetExchangeOrderPath() {
            return pathSettings.TemplatesPath + DateTime.Today.ToShortDateString() + "\\Заявки на биржу\\";
        }
    }
}