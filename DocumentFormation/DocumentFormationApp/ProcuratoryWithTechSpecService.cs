using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO;
using AltaOffice;
using System.Collections.ObjectModel;

namespace DocumentFormation
{
    public static class ProcuratoryWithTechSpecService
    {
        public static bool FillKaspiOrder(string templateFileName, Order order, string direction)
        {
            if (string.IsNullOrEmpty(templateFileName) || order == null || order.Auction == null || order.Auction.Lots == null || order.Auction.Lots.Count < 1 ||
                order.Auction.Lots[0].LotsExtended == null || order.Auction.Lots[0].LotsExtended.Count < 1) return false;

            var excel = new ExcelService(templateFileName);

            if (excel == null) return false;

            try {
                excel.SetCells("4", "D", direction != null ? direction : "Продажа/покупка");
                excel.SetCells("5", "D", order.Organizer != null ? order.Organizer.Replace("Товарищество с ограниченной ответственностью", "ТОО") : "-");
                excel.SetCells("6", "D", order.Initiator != null ? order.Initiator.Replace("Товарищество с ограниченной ответственностью", "ТОО") : "-");
                excel.SetCells("7", "D", order.Auction.Lots[0].Name != null ? order.Auction.Lots[0].Name : "-");
                excel.SetCells("8", "D", order.Auction.Type != null ? order.Auction.Type : "-");
                excel.SetCells("9", "D", order.Auction.Lots[0].LotsExtended[0].marka != null ? order.Auction.Lots[0].LotsExtended[0].marka : "-");
                excel.SetCells("10", "D", order.Auction.Lots[0].LotsExtended[0].gost != null ? order.Auction.Lots[0].LotsExtended[0].gost : "-");
                excel.SetCells("11", "D", order.Auction.Lots[0].CodeTRFEA != null ? order.Auction.Lots[0].CodeTRFEA : "-");
                excel.SetCells("12", "D", order.Auction.Lots[0].LotsExtended[0].country != null ? order.Auction.Lots[0].LotsExtended[0].country : "-");
                excel.SetCells("13", "D", order.Auction.Lots[0].LotsExtended[0].factory != null ? order.Auction.Lots[0].LotsExtended[0].factory : "-");
                excel.SetCells("14", "D", order.Auction.Lots[0].LotsExtended.Count > 1 ? "Согласно спецификации" : (order.Auction.Lots[0].Quantity != null ? order.Auction.Lots[0].Quantity : 0).ToString());
                excel.SetCells("15", "D", order.Auction.Lots[0].LotsExtended.Count > 1 ? "Согласно спецификации" : (order.Auction.Lots[0].Unit != null ? order.Auction.Lots[0].Unit : "-"));
                excel.SetCells("16", "D", order.Auction.Lots[0].LotsExtended.Count > 1 ? "Согласно спецификации" : (order.Auction.Lots[0].Price != null ? order.Auction.Lots[0].Price : 0).ToString());
                excel.SetCells("17", "D", order.Auction.Comments != null ? order.Auction.Comments : ""); // currency
                excel.SetCells("18", "D", (order.Auction.Lots[0].Step != null ? order.Auction.Lots[0].Step : 0) + " " + excel.GetCell("17", "D"));
                excel.SetCells("19", "D", order.Auction.Lots[0].DeliveryPlace != null ? order.Auction.Lots[0].DeliveryPlace : "");
                excel.SetCells("20", "D", order.Auction.Lots[0].LotsExtended[0].terms != null ? order.Auction.Lots[0].LotsExtended[0].terms : "-");
                excel.SetCells("21", "D", order.Auction.Lots[0].PaymentTerm != null ? order.Auction.Lots[0].PaymentTerm : "-");
                excel.SetCells("22", "D", order.Auction.Lots[0].DeliveryTime != null ? order.Auction.Lots[0].DeliveryTime : "-");
                excel.SetCells("23", "D", order.Auction.Date != null ? order.Auction.Date.ToShortDateString() : "-");
                excel.SetCells("25", "D", order.Auction.Lots[0].LotsExtended[0].sum != null ? order.Auction.Lots[0].LotsExtended.Sum(l => l.sum) : 0);
                excel.SetCells("26", "D", (order.Auction.InvoicePercent != null ? order.Auction.InvoicePercent : 0) + "%");

                int rowCount = 31;
                int iCount = 1;

                foreach (var item in order.Auction.Lots[0].LotsExtended) {
                    if (iCount > 1) excel.InsertRow(rowCount);

                    excel.SetCells(rowCount, "B", iCount);
                    excel.SetCells(rowCount, "C", item.name != null ? item.name : "-");
                    excel.SetCells(rowCount, "D", item.unit != null ? item.unit : "-");
                    excel.SetCells(rowCount, "E", item.quantity != null ? item.quantity : 0);
                    excel.SetCells(rowCount, "F", item.price != null ? item.price : 0);
                    excel.SetCells(rowCount, "G", (item.quantity != null && item.price != null) ? (item.quantity * item.price) : 0);

                    iCount += 1;
                    rowCount += 1;
                }

                excel.SetCells(rowCount, "G", order.Auction.Lots[0].LotsExtended.Sum(l => l.sum));
            } catch {
                return false;
            } finally {
                if (excel.IsWorkbookOpened()) excel.CloseWorkbook(true);
                if (excel.IsExcelOpened()) excel.CloseExcel();
            }

            return true;
        }


        public static Order ParseKaspiOrder(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var excel = new ExcelService(fileName);

            if (excel == null) return null;

            Order order = new Order();
            order.Auction = new Auction();
            order.Auction.Lots = new System.Collections.ObjectModel.ObservableCollection<Lot>();
            order.Auction.Lots.Add(new Lot() { LotsExtended = new System.Collections.ObjectModel.ObservableCollection<LotsExtended>() });

            bool result;
            DateTime dateTime;
            decimal decimalNumber;
            double doubleNumber;
            int intNumber;

            try {
                order.Organizer = excel.GetCell("5", "D");
                order.Initiator = excel.GetCell("6", "D");
                order.Auction.Lots[0].Name = excel.GetCell("7", "D");
                order.Auction.Type = excel.GetCell("8", "D");

                result = decimal.TryParse(excel.GetCell("14", "D"), out decimalNumber);
                order.Auction.Lots[0].Quantity = result ? decimalNumber : 0;
                order.Auction.Lots[0].Unit = excel.GetCell("15", "D");

                result = decimal.TryParse(excel.GetCell("16", "D"), out decimalNumber);
                order.Auction.Lots[0].Price = result ? decimalNumber : 0;
                order.Auction.Comments = excel.GetCell("17", "D"); // currency

                result = decimal.TryParse(excel.GetCell("18", "D"), out decimalNumber);
                order.Auction.Lots[0].Step = result ? decimalNumber : 0;
                order.Auction.Lots[0].DeliveryPlace = excel.GetCell("19", "D");
                order.Auction.Lots[0].PaymentTerm = excel.GetCell("21", "D");
                order.Auction.Lots[0].DeliveryTime = excel.GetCell("22", "D");

                result = DateTime.TryParse(excel.GetCell("23", "D"), out dateTime);
                order.Auction.Date = result ? dateTime : DateTime.Now;

                result = decimal.TryParse(excel.GetCell("25", "D"), out decimalNumber);
                order.Auction.Lots[0].Sum = result ? decimalNumber : 0;

                result = double.TryParse(excel.GetCell("26", "D"), out doubleNumber);
                order.Auction.InvoicePercent = result ? doubleNumber : 0;

                int rowCount = 31;

                while (!string.IsNullOrEmpty(excel.GetCell(rowCount, "B")) && !string.IsNullOrEmpty(excel.GetCell(rowCount, "D"))) {
                    LotsExtended lotsExtended = new LotsExtended();

                    result = Int32.TryParse(excel.GetCell(rowCount, "B"), out intNumber);
                    lotsExtended.serialnumber = result ? intNumber : 0;
                    lotsExtended.name = excel.GetCell(rowCount, "C");
                    lotsExtended.unit = excel.GetCell(rowCount, "D");

                    result = decimal.TryParse(excel.GetCell(rowCount, "E"), out decimalNumber);
                    lotsExtended.quantity = result ? decimalNumber : 0;

                    result = decimal.TryParse(excel.GetCell(rowCount, "F"), out decimalNumber);
                    lotsExtended.price = result ? decimalNumber : 0;
                    lotsExtended.sum = lotsExtended.quantity * lotsExtended.price;
                    lotsExtended.marka = excel.GetCell("9", "D");
                    lotsExtended.gost = excel.GetCell("10", "D");
                    lotsExtended.codeTNVD = excel.GetCell("11", "D");
                    lotsExtended.country = excel.GetCell("12", "D");
                    lotsExtended.factory = excel.GetCell("13", "D");
                    lotsExtended.terms = excel.GetCell("20", "D");

                    order.Auction.Lots[0].LotsExtended.Add(lotsExtended);
                }
            } catch {
                return null;
            } finally {
                if (excel.IsWorkbookOpened()) excel.CloseWorkbook();
                if (excel.IsExcelOpened()) excel.CloseExcel();
            }

            return order;
        }


        public static bool FillKaspiProcuratory(string templateFileName, Order order, string direction)
        {
            if (string.IsNullOrEmpty(templateFileName) || order == null || order.Auction == null || order.Auction.Lots == null || order.Auction.Lots.Count < 1 ||
                order.Auction.Lots[0].LotsExtended == null || order.Auction.Lots[0].LotsExtended.Count < 1) return false;

            var word = new WordService(templateFileName, false);

            if (word == null) return false;

            try {
                word.FindReplace("[brokerCompany]", "");
                word.FindReplace("[brokerDirector]", "");

                word.SetCell(1, 6, 3, direction != null ? direction : "Продажа/покупка");
                word.SetCell(1, 7, 3, order.Organizer != null ? order.Organizer : "");
                word.SetCell(1, 8, 3, order.Initiator != null ? order.Initiator : "");
                word.SetCell(1, 9, 3, order.Auction.Lots[0].Name != null ? order.Auction.Lots[0].Name : "");
                word.SetCell(1, 10, 3, order.Auction.Type != null ? order.Auction.Type : "");
                word.SetCell(1, 11, 3, order.Auction.Lots[0].LotsExtended[0].marka != null ? order.Auction.Lots[0].LotsExtended[0].marka : "");
                word.SetCell(1, 12, 3, order.Auction.Lots[0].LotsExtended[0].gost != null ? order.Auction.Lots[0].LotsExtended[0].gost : "");
                word.SetCell(1, 13, 3, order.Auction.Lots[0].CodeTRFEA != null ? order.Auction.Lots[0].CodeTRFEA : "");
                word.SetCell(1, 14, 3, order.Auction.Lots[0].LotsExtended[0].country != null ? order.Auction.Lots[0].LotsExtended[0].country : "");
                word.SetCell(1, 15, 3, order.Auction.Lots[0].LotsExtended[0].factory != null ? order.Auction.Lots[0].LotsExtended[0].factory : "");
                word.SetCell(1, 16, 3, order.Auction.Lots[0].LotsExtended.Count > 1 ? "Согласно спецификации" : (order.Auction.Lots[0].Quantity != null ? order.Auction.Lots[0].Quantity : 0).ToString());
                word.SetCell(1, 17, 3, order.Auction.Lots[0].LotsExtended.Count > 1 ? "Согласно спецификации" : (order.Auction.Lots[0].Unit != null ? order.Auction.Lots[0].Unit : ""));
                word.SetCell(1, 18, 3, order.Auction.Lots[0].LotsExtended.Count > 1 ? "Согласно спецификации" : (order.Auction.Lots[0].Price != null ? order.Auction.Lots[0].Price : 0).ToString());
                word.SetCell(1, 19, 3, order.Auction.Comments != null ? order.Auction.Comments : ""); // currency
                word.SetCell(1, 20, 3, (order.Auction.Lots[0].Step != null ? order.Auction.Lots[0].Step : 0) + " " + word.GetCell(1, 19, 3));
                word.SetCell(1, 21, 3, order.Auction.Lots[0].DeliveryPlace != null ? order.Auction.Lots[0].DeliveryPlace : "");
                word.SetCell(1, 22, 3, order.Auction.Lots[0].LotsExtended[0].terms != null ? order.Auction.Lots[0].LotsExtended[0].terms : "");
                word.SetCell(1, 23, 3, order.Auction.Lots[0].PaymentTerm != null ? order.Auction.Lots[0].PaymentTerm : "");
                word.SetCell(1, 24, 3, order.Auction.Lots[0].DeliveryTime != null ? order.Auction.Lots[0].DeliveryTime : "");
                word.SetCell(1, 25, 3, order.Auction.Date != null ? order.Auction.Date.ToShortDateString() : "");
                word.SetCell(1, 26, 3, "Техническая спецификация");
                word.SetCell(1, 27, 3, order.Auction.Lots[0].LotsExtended[0].sum != null ? order.Auction.Lots[0].LotsExtended.Sum(l => l.sum).ToString() : "");
            } catch {
                return false;
            } finally {
                if (word.IsOpenDocument()) word.CloseDocument(true);
                if (word.IsOpenWord()) word.CloseWord(true);
            }

            return true;
        }


        public static bool FillKaspiTechSpec(string templateFileName, List<LotsExtended> lotsExtended)
        {
            if (string.IsNullOrEmpty(templateFileName)) return false;

            var excel = new ExcelService(templateFileName);

            if (excel == null) return false;

            int rowCount = 15;
            int iCount = 1;

            try {
                foreach (var item in lotsExtended) {
                    if (iCount > 1) excel.InsertRow(rowCount + 1);

                    excel.SetCells(rowCount, "C", iCount);
                    excel.SetCells(rowCount, "D", item.name != null ? item.name : "");
                    excel.SetCells(rowCount, "E", item.unit != null ? item.unit : "");
                    excel.SetCells(rowCount, "F", item.quantity != null ? item.quantity : 0);
                    excel.SetCells(rowCount, "H", item.price != null ? item.price : 0);
                    excel.SetCells(rowCount, "I", (item.quantity != null && item.price != null) ? (item.quantity * item.price) : 0);
                    excel.SetCells(rowCount, "K", item.terms != null ? item.terms : "");

                    iCount += 1;
                    rowCount += 1;
                }

                excel.SetCells(rowCount, "F", lotsExtended.Sum(l => l.quantity));
                excel.SetCells(rowCount, "I", lotsExtended.Sum(l => l.sum));
            } catch {
                return false;
            } finally {
                if (excel.IsWorkbookOpened()) excel.CloseWorkbook(true);
                if (excel.IsExcelOpened()) excel.CloseExcel();
            }

            return true;
        }


        public static Order ParseKaspiProcuratory(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var word = new WordService(fileName, false);

            if (word == null) return null;

            Order order = new Order();
            order.Auction = new Auction();
            order.Auction.Lots = new System.Collections.ObjectModel.ObservableCollection<Lot>();
            order.Auction.Lots.Add(new Lot() { LotsExtended = new System.Collections.ObjectModel.ObservableCollection<LotsExtended>() });

            bool result;
            DateTime dateTime;
            decimal decimalNumber;
            double doubleNumber;
            int intNumber;

            try {
                order.Organizer = word.GetCell(1, 6, 3).Replace("\r\a", "").Trim();
                order.Initiator = word.GetCell(1, 7, 3).Replace("\r\a", "").Trim();
                order.Auction.Lots[0].Name = word.GetCell(1, 8, 3).Replace("\r\a", "").Trim();
                order.Auction.Type = word.GetCell(1, 9, 3).Replace("\r\a", "").Trim();

                result = decimal.TryParse(word.GetCell(1, 15, 3).Replace("\r\a", "").Trim(), out decimalNumber);
                order.Auction.Lots[0].Quantity = result ? decimalNumber : 0;
                order.Auction.Lots[0].Unit = word.GetCell(1, 16, 3).Replace("\r\a", "").Trim();

                result = decimal.TryParse(word.GetCell(1, 17, 3).Replace("\r\a", "").Trim(), out decimalNumber);
                order.Auction.Lots[0].Price = result ? decimalNumber : 0;
                order.Auction.Comments = word.GetCell(1, 18, 3).Replace("\r\a", "").Trim(); // currency

                result = decimal.TryParse(word.GetCell(1, 19, 3).Replace("\r\a", "").Trim(), out decimalNumber);
                order.Auction.Lots[0].Step = result ? decimalNumber : 0;
                order.Auction.Lots[0].DeliveryPlace = word.GetCell(1, 20, 3).Replace("\r\a", "").Trim();
                order.Auction.Lots[0].PaymentTerm = word.GetCell(1, 21, 3).Replace("\r\a", "").Trim();
                order.Auction.Lots[0].DeliveryTime = word.GetCell(1, 22, 3).Replace("\r\a", "").Trim();

                result = DateTime.TryParse(word.GetCell(1, 24, 3).Replace("\r\a", "").Trim(), out dateTime);
                order.Auction.Date = result ? dateTime : DateTime.Now;

                result = decimal.TryParse(word.GetCell(1, 26, 3).Replace("\r\a", "").Trim(), out decimalNumber);
                order.Auction.Lots[0].Sum = result ? decimalNumber : 0;
            } catch {
                return null;
            } finally {
                if (word.IsOpenDocument()) word.CloseDocument(false);
                if (word.IsOpenWord()) word.CloseWord(false);
            }


            return order;
        }


        public static ObservableCollection<LotsExtended> ParseKaspiTechSpec(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var excel = new ExcelService(fileName);

            if (excel == null) return null;

            ObservableCollection<LotsExtended> lotsExtendedList = new ObservableCollection<LotsExtended>();
            bool result;
            decimal decimalNumber;
            int intNumber;

            try {
                var rowCount = excel.FindRow("Наименование товара");

                if (rowCount == null || rowCount == 0) throw new Exception();

                rowCount += 2;

                while (!string.IsNullOrEmpty(excel.GetCell(rowCount, "C")) && !string.IsNullOrEmpty(excel.GetCell(rowCount, "E"))) {
                    LotsExtended lotsExtended = new LotsExtended();

                    result = Int32.TryParse(excel.GetCell(rowCount, "C"), out intNumber);
                    lotsExtended.serialnumber = result ? intNumber : 0;
                    lotsExtended.name = excel.GetCell(rowCount, "D");
                    lotsExtended.unit = excel.GetCell(rowCount, "E");

                    result = decimal.TryParse(excel.GetCell(rowCount, "F"), out decimalNumber);
                    lotsExtended.quantity = result ? decimalNumber : 0;

                    result = decimal.TryParse(excel.GetCell(rowCount, "H"), out decimalNumber);
                    lotsExtended.price = result ? decimalNumber : 0;
                    lotsExtended.sum = lotsExtended.quantity * lotsExtended.price;
                    lotsExtended.terms = excel.GetCell(rowCount, "K");

                    lotsExtendedList.Add(lotsExtended);

                    rowCount += 1;
                }
            } catch {
                return null;
            } finally {
                if (excel.IsWorkbookOpened()) excel.CloseWorkbook();
                if (excel.IsExcelOpened()) excel.CloseExcel();
            }

            return lotsExtendedList;
        }
    }
}