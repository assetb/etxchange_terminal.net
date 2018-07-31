using System.IO;
using AltaBO;
using AltaBO.specifics;

namespace AltaTransport {
    public class ArchiveTransport {
        #region Variables
        /*
                private static string auctionNumber;
        */
        private const string archivePath = @"\\192.168.11.5\Archive\";

        private const string utbOrderTemplateFileName = archivePath + @"\Templates\UTB\Order.docx";
        private const string utbSupplierOrderTemplateFileName = archivePath + @"\Templates\UTB\SupplierOrder.docx";
        private const string utbApplicantsTemplateFileName = archivePath + @"\Templates\UTB\Applicants.docx";
        private const string utbProtocolTemplateFileName = archivePath + @"\Templates\UTB\Protocol.docx";
        private const string utbCustomerReportTemplateFileName = archivePath + @"\Templates\UTB\CustomerReport.docx";
        private const string utbSupplierReportTemplateFileName = archivePath + @"\Templates\UTB\SupplierReport.docx";
        private const string utbBillTemplateFileName = archivePath + @"\Templates\UTB\Invoice";

        private const string etsOrderTemplateFileName = archivePath + @"\Templates\ETS\Order.xlsx";
        private const string etsOrderAttachTemplateFileName = archivePath + @"\Templates\ETS\OrderAttach.docx";
        private const string etsJournalC01TemplateFileName = archivePath + @"\Templates\ETS\FormC01.docx";
        private const string etsSupplierOrderTemplateFileName = archivePath + @"\Templates\ETS\SupplierOrder.docx";
        private const string etsApplicantsTemplateFileName = archivePath + @"\Templates\ETS\Applicants.docx";
        private const string etsProcuratoryTemplateFileName = archivePath + @"\Templates\ETS\Procuratory.docx";
        #endregion


        #region ETS
        /*public static void PutOrderToArchive(int source, string[] files, object info) {
            switch(source) {
                case 1: // ETS - Vostok                 
                    var order = (Order)info;

                    auctionNumber = "";
                    auctionNumber = order.Title.Substring(0, order.Title.IndexOf("от") - 1);
                    auctionNumber = auctionNumber.Replace("/", "_");

                    if(auctionNumber.Length > 4) auctionNumber = auctionNumber.Substring(auctionNumber.Length - 4, 4);

                    var dateFolderName = archivePath + @"Auctions\ETS\" + order.Auction.Date.ToShortDateString();
                    var endFolderName = archivePath + @"Auctions\ETS\" + order.Auction.Date.ToShortDateString() + @"\" + auctionNumber;

                    if(!Directory.Exists(dateFolderName))
                        Directory.CreateDirectory(dateFolderName);

                    if(!Directory.Exists(endFolderName))
                        Directory.CreateDirectory(endFolderName);

                    var tmp = "";

                    foreach(var item in files) {
                        tmp = item.Substring(item.LastIndexOf(@"\") + 1, item.Length - item.LastIndexOf(@"\") - 1);

                        File.Move(item, endFolderName + @"\" + tmp);
                    }
                    break;
            }
        }*/

        public static string[] PutETSOrders(Order order, string orderFile, string treatyFile = null) {
            var dateFolderName = archivePath + @"Auctions\ETS\" + order.Auction.Date.ToShortDateString();
            var endFolderName = archivePath + @"Auctions\ETS\" + order.Auction.Date.ToShortDateString() + @"\" + order.Auction.Number.Substring(order.Auction.Number.Length - 4) + @"\";
            string[] fileNames = new string[4];

            if(!Directory.Exists(dateFolderName))
                Directory.CreateDirectory(dateFolderName);

            if(!Directory.Exists(endFolderName))
                Directory.CreateDirectory(endFolderName);

            File.Copy(etsOrderTemplateFileName, endFolderName + "Заявка №" + order.Auction.Number.Substring(order.Auction.Number.Length - 4) + ".xlsx", true);
            fileNames[0] = endFolderName + "Заявка №" + order.Auction.Number.Substring(order.Auction.Number.Length - 4) + ".xlsx";
            File.Copy(etsOrderAttachTemplateFileName, endFolderName + "Приложение к заявке №" + order.Auction.Number.Substring(order.Auction.Number.Length - 4) + ".docx", true);
            fileNames[1] = endFolderName + "Приложение к заявке №" + order.Auction.Number.Substring(order.Auction.Number.Length - 4) + ".docx";

            if(orderFile.Contains(".xlsx")) {
                File.Copy(orderFile, endFolderName + "Заявка (original).xlsx", true);
                fileNames[2] = endFolderName + "Заявка (original).xlsx";
            } else {
                File.Copy(orderFile, endFolderName + "Заявка (original).xls", true);
                fileNames[2] = endFolderName + "Заявка (original).xls";
            }

            if(!string.IsNullOrEmpty(treatyFile)) {
                if(treatyFile.Contains(".docx")) {
                    File.Copy(treatyFile, endFolderName + "Приложение к заявке (original).docx", true);
                    fileNames[3] = endFolderName + "Приложение к заявке (original).docx";
                } else {
                    File.Copy(treatyFile, endFolderName + "Приложение к заявке (original).doc", true);
                    fileNames[3] = endFolderName + "Приложение к заявке (original).doc";
                }
            }

            return fileNames;
        }

        public static string PutETSJournalC01(string bin, string code) {
            var endFolderName = archivePath + @"Companies\" + bin + @"\";

            if(!Directory.Exists(endFolderName))
                Directory.CreateDirectory(endFolderName);

            File.Copy(etsJournalC01TemplateFileName, endFolderName + "Форма С01 для " + code + ".docx", true);

            return endFolderName + "Форма С01 для " + code + ".docx";
        }

        public static string PutETSSupplierOrder(Order order) {
            var endFolderName = archivePath + @"Auctions\ETS\" + order.Auction.Date.ToShortDateString() + @"\" + order.Auction.Number.Substring(order.Auction.Number.Length - 4) + @"\";

            if(!Directory.Exists(endFolderName)) Directory.CreateDirectory(endFolderName);

            System.IO.File.Copy(etsSupplierOrderTemplateFileName, endFolderName + "Заявка на участие от " + order.Auction.SupplierOrders[0].Name.Replace("\"", "'") + ".docx", true);

            return endFolderName + "Заявка на участие от " + order.Auction.SupplierOrders[0].Name.Replace("\"", "'") + ".docx";
        }

        public static string PutETSApplicants(Order order) {
            var endFolderName = archivePath + @"Auctions\ETS\" + order.Auction.Date.ToShortDateString() + @"\" + order.Auction.Number.Substring(order.Auction.Number.Length - 4) + @"\";

            if(!Directory.Exists(endFolderName)) Directory.CreateDirectory(endFolderName);

            System.IO.File.Copy(etsApplicantsTemplateFileName, endFolderName + "Список претендентов на аукцион №" + order.Auction.Number.Replace("/", "_") + ".docx", true);

            //DataBaseClient.CreateApplicants(order);

            return endFolderName + "Список претендентов на аукцион №" + order.Auction.Number.Replace("/", "_") + ".docx";
        }

        public static string PutETSProcuratory(Order order) {
            var endFolderName = archivePath + @"Auctions\ETS\" + order.Auction.Date.ToShortDateString() + @"\" + order.Auction.Number.Substring(order.Auction.Number.Length - 4).Replace("/", "_") + @"\";

            if(!Directory.Exists(endFolderName)) Directory.CreateDirectory(endFolderName);

            string fName = endFolderName + "Поручение на сделку от " + order.Auction.SupplierOrders[0].Name.Replace("\"", "'").Replace("Товарищество с ограниченной ответственностью", "ТОО") + " №" + (order.Auction.Number.Substring(order.Auction.Number.Length - 4)).Replace("/", "_") + " по лоту " + order.Auction.Lots[0].Number + ".docx";

            File.Copy(etsProcuratoryTemplateFileName, fName, true);

            return fName;
        }
        #endregion


        #region UTB
        public static string PutUTBOrder(Order order) {
            var dateFolderName = archivePath + @"Auctions\UTB\" + order.Auction.Date.ToShortDateString();
            var endFolderName = archivePath + @"Auctions\UTB\" + order.Auction.Date.ToShortDateString() + @"\" + order.Auction.Number.Replace("/", "_") + @"\";

            if(!Directory.Exists(dateFolderName))
                Directory.CreateDirectory(dateFolderName);

            if(!Directory.Exists(endFolderName))
                Directory.CreateDirectory(endFolderName);

            endFolderName += "Заявка №" + order.Auction.Number.Replace("/", "_") + ".docx";

            System.IO.File.Copy(utbOrderTemplateFileName, endFolderName, true);

            return endFolderName;
        }


        public static string PutUTBSupplierOrder(Order order, bool isAlternative, decimal minimalPrice) {
            var endFolderName = archivePath + @"Auctions\UTB\" + order.Auction.Date.ToShortDateString() + @"\" + order.Auction.Number.Replace("/", "_") + @"\";

            if(!Directory.Exists(endFolderName)) Directory.CreateDirectory(endFolderName);

            System.IO.File.Copy(utbSupplierOrderTemplateFileName, endFolderName + "Заявка на участие от " + order.Auction.SupplierOrders[0].Name.Replace("\"", "'") + ".docx", true);

            DataBaseClient.CreateProcuratory(order, minimalPrice);

            return endFolderName + "Заявка на участие от " + order.Auction.SupplierOrders[0].Name.Replace("\"", "'") + ".docx";
        }


        public static string PutUTBApplicants(Order order) {
            var endFolderName = archivePath + @"Auctions\UTB\" + order.Auction.Date.ToShortDateString() + @"\" + order.Auction.Number.Replace("/", "_") + @"\";

            if(!Directory.Exists(endFolderName)) Directory.CreateDirectory(endFolderName);

            System.IO.File.Copy(utbApplicantsTemplateFileName, endFolderName + "Список претендентов на аукцион №" + order.Auction.Number.Replace("/", "_") + ".docx", true);

            //DataBaseClient.CreateApplicants(order);

            return endFolderName + "Список претендентов на аукцион №" + order.Auction.Number.Replace("/", "_") + ".docx";
        }


        public static string PutUTBBill(int brokerType, Order order, bool isWarranty, string lotNumber) {
            var endFolderName = archivePath + @"Auctions\" + (order.Auction.SiteId == 4 ? "ETS" : order.Auction.SiteId == 5 ? "KazETS" : "UTB") + @"\" + order.Auction.Date.ToShortDateString() + @"\" + order.Auction.Number.Replace("/", "_") + @"\";

            string oldPath = archivePath + @"Auctions\" + (order.Auction.SiteId == 4 ? "ETS" : order.Auction.SiteId == 5 ? "KazETS" : "UTB") + @"\" + order.Auction.Date.ToShortDateString() + @"\" + (order.Auction.SiteId == 4 ? order.Auction.Number.Length > 4 ? order.Auction.Number.Substring(order.Auction.Number.Length - 4) : order.Auction.Number : order.Auction.Number).Replace("/", "_") + @"\";
            string newPath = endFolderName;

            endFolderName = Directory.Exists(oldPath) ? oldPath : newPath;

            var brokerName = "";

            if(brokerType == 1) brokerName = "Altaik";
            else if(brokerType == 2) brokerName = "Korund";
            else if(brokerType == 3) brokerName = "AkAltyn";

            File.Copy(utbBillTemplateFileName + brokerName + ".docx", endFolderName + "Счет " + (isWarranty ? "ГО " : "") + "для компании " + order.Auction.SupplierOrders[0].Name.Replace("\"", "'").Replace("Товарищество с ограниченной ответственностью", "ТОО") + " №" + order.Auction.Number.Replace("/", "_") + " по лоту " + lotNumber + ".docx", true);

            return endFolderName + "Счет " + (isWarranty ? "ГО " : "") + "для компании " + order.Auction.SupplierOrders[0].Name.Replace("\"", "'").Replace("Товарищество с ограниченной ответственностью", "ТОО") + " №" + order.Auction.Number.Replace("/", "_") + " по лоту " + lotNumber + ".docx";
        }


        public static string PutUTBProtocol(Order order) {
            var endFolderName = archivePath + @"Auctions\UTB\" + order.Auction.Date.ToShortDateString() + @"\" + order.Auction.Number.Replace("/", "_") + @"\";

            if(!Directory.Exists(endFolderName))
                Directory.CreateDirectory(endFolderName);

            System.IO.File.Copy(utbProtocolTemplateFileName, endFolderName + "Шаблон протокола по аукциону №" + order.Auction.Number.Replace("/", "_") + ".docx", true);

            return endFolderName + "Шаблон протокола по аукциону №" + order.Auction.Number.Replace("/", "_") + ".docx";
        }


        public static string[] PutUTBReports(Order order) {
            var fileNames = new string[2];

            var endFolderName = archivePath + @"Auctions\UTB\" + order.Auction.Date.ToShortDateString() + @"\" + order.Auction.Number.Replace("/", "_") + @"\";

            if(!Directory.Exists(endFolderName))
                Directory.CreateDirectory(endFolderName);

            fileNames[0] = endFolderName + "Отчет заказчику по лоту №" + order.Auction.Lots[0].Number + ".docx";
            fileNames[1] = endFolderName + "Отчет поставщику по лоту №" + order.Auction.Lots[0].Number + ".docx";

            File.Copy(utbCustomerReportTemplateFileName.Replace(".docx", (order.Auction.Broker.Code == "ALTK" ? ".docx" : "AltairNur.docx")), fileNames[0], true);

            string supName = order.Auction.SupplierOrders[0].Name.ToUpper();
            int winApplicant = (supName.Contains("АК АЛТЫН") || supName.Contains("КОРУНД")) ? 1 : 0;
            int winBroker = order.Auction.SupplierOrders[winApplicant].Name == order.Auction.SupplierOrders[0].Name ? 0 : 1;
            string brokerSyfix = order.Auction.SupplierOrders[winBroker].BrokerName.ToUpper().Contains("АЛТЫН") ? "AkAltyn.docx" : "Korund.docx";

            File.Copy(utbSupplierReportTemplateFileName.Replace(".docx", brokerSyfix), fileNames[1], true);

            return fileNames;
        }
        #endregion


        #region
        public static void CreateOrder() {
            // Input order number
            // Upload order file
            // Upload treaty file, if customer is Vostok
            // Push 'Create new order'

            // Create record in fileslist table and get id
            // Put order file in specific folder (order\[site]\number)
            // Create record in file table with order file and fileslist id
            // Put treaty file in specific folder (order\[site]\number)
            // Create record in file table with treaty order file and filelist id
            // Create record in order table with status new and fileslistid
        }
        #endregion
    }
}