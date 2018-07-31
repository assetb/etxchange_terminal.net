using AltaBO;
using AltaBO.specifics;
using AltaOffice;
using System;
using System.Globalization;
using System.Text;

namespace DocumentFormation
{
    public class SupplierOrderService
    {
        //private static WordService word;
        //private static IExcelService excel;
        ////private static decimal minimalPrice;
        //private static Order order = new Order();
        ////private static DocumentRequisite docReq;
        //private static string fileName, coverLetterFileName, techSpecFile;
        //private static bool withPDF = false;

        public static void CreateSupplierOrder(Order orderInfo, string supplierOrderFile, string supplierCoverLetterFile, string supplierTechSpecFile, bool withPDFInfo = false)
        {
            FillTemplate(orderInfo, supplierOrderFile, withPDFInfo, coverLetterFileName: supplierCoverLetterFile, techSpecFile: supplierTechSpecFile);
        }

        /*public static void CreateSupplierOrder(Order orderInfo, DocumentRequisite docReqInfo, bool withPDFInfo = false) {
            order = orderInfo;
            docReq = docReqInfo;
            fileName = supplierOrderFile;
            withPDF = withPDFInfo;

            string folderPath = "\\\\192.168.11.5\\Archive\\Auctions\\";
            string oldPath = folderPath + docReq.market + "\\" + docReq.date.ToShortDateString() + "\\" + (docReq.market == MarketPlaceEnum.ETS ? (docReq.number.Length > 4 ? docReq.number.Substring(docReq.number.Length - 4).Replace("/", "_") : docReq.number) : docReq.number.Replace("/", "_")) + "\\" + docReq.fileName;
            string newPath = folderPath + docReq.market + "\\" + docReq.date.ToShortDateString() + "\\" + docReq.number.Replace("/", "_") + "\\" + docReq.fileName;
            fileName = File.Exists(oldPath) ? oldPath : newPath;

            if(order.Auction.SiteId != 4 && order.Auction.Customer.ToLower().Contains("караж")) {
                if(orderInfo.Auction.SupplierOrders[0].Name == orderInfo.Auction.SupplierOrders[0].BrokerName) minimalPrice = orderInfo.Auction.Lots[0].Price * orderInfo.Auction.Lots[0].Quantity;
                else minimalPrice = orderInfo.Auction.Procuratories[0].MinimalPrice;
            }

            FillTemplate();
        }*/

        public static void CreateSupplierOrder(Order orderInfo, string supplierOrderTemplate, bool withPDFInfo = false)
        {
            FillTemplate(orderInfo, supplierOrderTemplate, withPDFInfo);
        }

        private static void FillTemplate(Order order, string fileName, bool withPDF = false, string coverLetterFileName = "", string techSpecFile = "")
        {
            switch (order.Auction.SiteId)
            {
                case ((int)MarketPlaceEnum.UTB):
                    {
                        FillTemplateForUTB(order, fileName, withPDF);
                    }
                    break;
                case ((int)MarketPlaceEnum.ETS):
                    {
                        FillTemplateForETS(order, fileName, withPDF);
                    }
                    break;
                case ((int)MarketPlaceEnum.KazETS):
                    {
                        FillTemplateForKazETS(order, fileName, coverLetterFileName, techSpecFile, withPDF);
                    }
                    break;
            }
        }

        private static void FillTemplateForUTB(Order order, string fileName, bool withPDF = false)
        {
            WordService word = new WordService(fileName, false);
            word.FindReplace("[orderNumber]", order.Auction.Number + " от " + order.Date.ToShortDateString());

            word.SetCell(1, 1, 2, order.Auction.SupplierOrders[0].Name);
            word.SetCell(1, 3, 2, order.Auction.Date.ToString("dd.MM.yyyy"));
            word.SetCell(1, 4, 2, order.Auction.Customer);

            word.FindReplace("[clientName]", order.Auction.SupplierOrders[0].Name);
            word.FindReplace("[clientName]", order.Auction.SupplierOrders[0].Name);
            word.FindReplace("[companyDirector]", string.IsNullOrEmpty(order.Auction.SupplierOrders[0].CompanyDirector) ? "[Впишите ФИО Директора компании]" : order.Auction.SupplierOrders[0].CompanyDirector);
            word.FindReplace("[brokerName]", order.Auction.SupplierOrders[0].BrokerName);
            word.FindReplace("[clientName]", order.Auction.SupplierOrders[0].Name);
            word.FindReplace("[traderName]", order.Auction.SupplierOrders[0].Trader);

            int iCount = 2;
            decimal sum = 0;
            decimal minimalPrice = 0; // Early getted from inside

            foreach (var item in order.Auction.Lots)
            {
                if (iCount > 2) word.AddTableRow(2);

                word.SetCell(2, iCount, 1, (iCount - 1).ToString());
                word.SetCell(2, iCount, 2, item.Number);
                word.SetCell(2, iCount, 3, item.Name);
                word.SetCell(2, iCount, 4, item.Unit);
                word.SetCell(2, iCount, 5, item.Quantity.ToString(CultureInfo.InvariantCulture));

                if (order.Auction.SiteId != 4 && order.Auction.Customer.ToLower().Contains("караж"))
                {
                    word.SetCell(2, iCount, 6, Math.Round(minimalPrice / item.Quantity, 2).ToString(CultureInfo.InvariantCulture));
                    word.SetCell(2, iCount, 7, Math.Round(minimalPrice, 2).ToString(CultureInfo.InvariantCulture));
                    sum += minimalPrice;
                }
                else
                {
                    word.SetCell(2, iCount, 6, Math.Round(item.Sum / item.Quantity, 2).ToString(CultureInfo.InvariantCulture));
                    word.SetCell(2, iCount, 7, Math.Round(item.Sum * item.Quantity, 2).ToString(CultureInfo.InvariantCulture));
                    sum += item.Sum;
                }

                word.SetCell(2, iCount, 8, item.PaymentTerm);
                word.SetCell(2, iCount, 9, item.DeliveryPlace.Replace("|", ", ") + ", " + item.DeliveryTime);
                iCount++;
            }

            word.FindReplace("[sum]", Math.Round(sum, 2).ToString(CultureInfo.InvariantCulture));
            word.FindReplace("[brokerDirector]", order.Auction.SupplierOrders[0].Trader);

            if (withPDF) word.SaveAsPDF(fileName);

            word.CloseDocument(true);
            word.CloseWord(true);
        }

        private static void FillTemplateForETS(Order order, string fileName, bool withPDF = false)
        {
            WordService word = new WordService(fileName, false);
            word.FindReplace("[fromDate]", order.Date.ToShortDateString());

            if (order.customerid == 9) word.FindReplace("[clientBroker]", "ТОО 'Golden Gate Products'");
            else word.FindReplace("[clientBroker]", "ТОО 'Альта и К'");

            word.FindReplace("[orderInitiator]", order.Auction.Customer.Replace("Товарищество с ограниченной ответственностью", "ТОО").Replace("Акционерное общество", "АО"));

            string lots = "";
            int lCount = 1;

            foreach (var item in order.Auction.Lots)
            {
                lots += "№" + lCount + " - " + item.Number + (lCount == order.Auction.Lots.Count ? "" : ", ");
                lCount++;
            }

            word.FindReplace("[lotNumber]", lots);

            word.FindReplace("[memberCode]", order.Auction.SupplierOrders[0].BrokerCode);
            word.FindReplace("[fullMemberName]", order.Auction.SupplierOrders[0].BrokerName);
            word.FindReplace("[clientCode]", order.Auction.SupplierOrders[0].Code);
            word.FindReplace("[clientFullName]", order.Auction.SupplierOrders[0].Name);
            word.FindReplace("[clientAddress]", order.Auction.SupplierOrders[0].Address);
            word.FindReplace("[clientBIN]", order.Auction.SupplierOrders[0].BIN);
            //word.FindReplace("[clientPhones]", order.Auction.SupplierOrders[0].Phones);
            word.FindReplace("[clientBankIIK]", order.Auction.SupplierOrders[0].IIK);
            word.FindReplace("[clientBankBIK]", order.Auction.SupplierOrders[0].BIK);
            word.FindReplace("[clientBankName]", order.Auction.SupplierOrders[0].BankName);

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Список запрашиваемых документов").Append("\n");
            stringBuilder.Append("1. Заявка на участие").Append("\n");

            var iCount = 2;
            if (order.Auction.SupplierOrders[0].RequestedDocs != null)
            {
                foreach (var rDoc in order.Auction.SupplierOrders[0].RequestedDocs)
                {
                    stringBuilder.Append(iCount + ". " + rDoc.name).Append("\n");
                    iCount++;
                }
            }

            word.SetCell(2, 3, 2, stringBuilder.ToString());

            if (withPDF) word.SaveAsPDF(fileName);

            word.CloseDocument(true);
            word.CloseWord(true);
        }

        private static void FillTemplateForKazETS(Order order, string fileName, string coverLetterFileName, string techSpecFile, bool withPDF = false)
        {
            // Fill order
            //excel = new ExcelService(fileName);
            IExcelService excel = new ExcelService(fileName);

            string supplierData = order.Auction.SupplierOrders[0].Name + "\n" + order.Auction.SupplierOrders[0].Address + "\nБИН " + order.Auction.SupplierOrders[0].BIN + "\nИИК " + order.Auction.SupplierOrders[0].IIK + "\n" + order.Auction.SupplierOrders[0].BankName + "\nБИК " + order.Auction.SupplierOrders[0].BIK;

            excel.SetCells(4, "B", "Заявка №" + order.Auction.Number); // Auction number
            excel.SetCells(7, "C", supplierData); // Supplier data
            excel.SetCells(8, "C", order.Auction.Lots[0].Name); // Lot name
            excel.SetCells(9, "C", order.Auction.Lots[0].CodeTRFEA); // TR FEA
            excel.SetCells(10, "C", order.Auction.Lots[0].Name); // Lot description
            excel.SetCells(11, "C", order.Auction.Lots[0].UnitId != 11 ? Math.Round(order.Auction.Procuratories[0].MinimalPrice / order.Auction.Lots[0].Quantity, 2).ToString() : order.Auction.Lots[0].Unit); // Start price
            excel.SetCells(12, "C", order.Auction.Lots[0].UnitId != 11 ? order.Auction.Lots[0].Quantity.ToString() : order.Auction.Lots[0].Unit); // Quantity
            excel.SetCells(13, "C", order.Auction.Lots[0].Unit); // Unit of size
            excel.SetCells(14, "C", order.Auction.Procuratories[0].MinimalPrice); // Amount sum
            excel.SetCells(15, "C", order.Auction.Lots[0].Step); // Step
            excel.SetCells(16, "C", order.Auction.Lots[0].PaymentTerm); // Payment term
            excel.SetCells(17, "C", order.Auction.Lots[0].DeliveryPlace); // Delivery terms
            excel.SetCells(18, "C", order.Auction.Lots[0].DeliveryTime); // Delivery time
            excel.SetCells(20, "C", order.Auction.Lots[0].LocalContent); // Local content
            excel.SetCells(21, "C", order.Auction.Date.ToShortDateString()); // Auction date
            excel.SetCells(23, "C", order.Auction.Lots[0].Warranty); // Warranty
            excel.SetCells(25, "C", CountWorkDays(order.Auction.Date, 10)); // Order ellapse date

            // If supplier is alternative then need make one day later than private supplier
            DateTime tmpDate = order.Date;

            if (order.Auction.SupplierOrders[0].Name == order.Auction.SupplierOrders[0].BrokerName) tmpDate = order.Deadline;
            else tmpDate = order.Deadline.AddDays(-1);

            if (tmpDate.DayOfWeek == DayOfWeek.Sunday) tmpDate = tmpDate.AddDays(-2);
            else if (tmpDate.DayOfWeek == DayOfWeek.Saturday) tmpDate = tmpDate.AddDays(-1);

            //excel.SetCells(28, "B", "Дата \"" + tmpDate.Day + "\" " + GetMonthName(tmpDate.Month) + " " + tmpDate.Year + " г."); // Supplier order date

            excel.CloseWorkbook(true);
            excel.CloseExcel();

            // Fill cover letter
            //excel = new ExcelService(coverLetterFileName);
            excel = new ExcelService(coverLetterFileName);

            excel.SetCells(3, "A", "ЗАЯВКА №" + order.Auction.Number); // Order number
            excel.SetCells(7, "E", order.Auction.SupplierOrders[0].Name); // Supplier name
            excel.SetCells(11, "B", order.Auction.Lots[0].Name); // Lot name
            excel.SetCells(11, "D", order.Auction.Lots[0].Unit); // Unit of size
            excel.SetCells(11, "E", order.Auction.Lots[0].UnitId != 11 ? order.Auction.Lots[0].Quantity.ToString() : order.Auction.Lots[0].Unit); // Quantity
            excel.SetCells(11, "F", (order.Auction.Lots[0].UnitId == 11 ? "По приложению" : (order.Auction.Procuratories[0].MinimalPrice / order.Auction.Lots[0].Quantity).ToString())); // Price
            excel.SetCells(11, "H", order.Auction.Procuratories[0].MinimalPrice); // Sum
            excel.SetCells(12, "H", order.Auction.Procuratories[0].MinimalPrice); // Amount sum
            excel.SetCells(18, "A", "\"" + tmpDate.Day + "\" " + GetMonthName(tmpDate.Month) + " " + tmpDate.Year + " г."); // Order date

            // Convert cover letter to pdf
            if (withPDF) excel.SaveAsPDF(coverLetterFileName, false);

            // Close cover letter
            excel.CloseWorkbook(true);
            excel.CloseExcel();

            // Fill tech spec
            WordService word = new WordService(techSpecFile, false);

            word.FindReplace("[auctionNumber]", order.Auction.Number); // Auction number

            word.SetCell(1, 2, 1, "1"); // Number
            word.SetCell(1, 2, 2, order.Auction.Lots[0].Name); // Name
            word.SetCell(1, 2, 3, order.Auction.Lots[0].Unit); // Unit of size
            word.SetCell(1, 2, 4, order.Auction.Lots[0].Quantity.ToString()); // Quantity
            word.SetCell(1, 2, 5, (order.Auction.Procuratories[0].MinimalPrice/ order.Auction.Lots[0].Quantity).ToString()); // Price
            word.SetCell(1, 2, 6, order.Auction.Procuratories[0].MinimalPrice.ToString()); // Sum
            word.SetCell(1, 2, 7, order.Auction.Lots[0].Step.ToString()); // Step
            word.SetCell(1, 2, 8, order.Auction.Lots[0].DeliveryPlace + " | " + order.Auction.Lots[0].DeliveryTime); // Terms
            word.SetCell(1, 2, 9, order.Auction.Lots[0].PaymentTerm); // Payment
            word.SetCell(1, 2, 10, order.Auction.Lots[0].Warranty.ToString()); // Warranty
            word.SetCell(1, 2, 11, order.Auction.Lots[0].LocalContent.ToString()); // Local

            // Close specification
            word.CloseDocument(true);
            word.CloseWord(true);
        }

        private static string GetMonthName(int month)
        {
            string[] monthName = new string[12] { "января", "февраля", "марта", "апреля", "мая", "июня", "июля", "августа", "сентября", "октября", "ноября", "декабря" };

            return monthName[month - 1];
        }

        private static string CountWorkDays(DateTime startDate, int daysCount)
        {
            DateTime iDate = startDate;

            for (var i = 1; i <= daysCount; i++)
            {
                iDate = iDate.AddDays(1);

                if (iDate.DayOfWeek == DayOfWeek.Saturday) iDate = iDate.AddDays(2);
                if (iDate.DayOfWeek == DayOfWeek.Sunday) iDate = iDate.AddDays(1);
            }

            return iDate.ToShortDateString();
        }
    }
}