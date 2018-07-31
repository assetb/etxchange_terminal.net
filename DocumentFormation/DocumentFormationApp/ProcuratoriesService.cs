using AltaBO;
using AltaOffice;
using AltaTransport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DocumentFormation
{
    public class ProcuratoriesService
    {
        //private static string procuratoryFileName;
        //private static WordService word, techSpecWord;
        //private static Order order = new Order();
        //private static int[] tableNumbers;
        //private static List<Lot> prices;
        //private static string orderApplicant;


        public static void CreateProcuratory(string templateFileName, Order orderInfo, List<Lot> pricesLots = null, int[] tableNumbersInfo = null, string pathToOrderApplicant = null)
        {
            FillTemplate(templateFileName, orderInfo, pricesLots, tableNumbersInfo, pathToOrderApplicant);
        }


        public static void CreateProcuratory(Order orderInfo, string templateFileName, int[] tableNumbersInfo = null)
        {
            string orderApplicant = null;
            //order = orderInfo;
            //tableNumbers = tableNumbersInfo == null ? null : tableNumbersInfo;
            //procuratoryFileName = templateFileName;

            string[] result;
            var path = templateFileName.Substring(0, templateFileName.LastIndexOf("\\") + 1);

            try {
                result = Directory.GetFiles(path, "Приложение к заявке*.docx");
            } catch (Exception) {
                result = null;
            }

            if (result != null && result.Length > 0 && !string.IsNullOrEmpty(result[0])) {
                orderApplicant = result[0];
            }

            FillTemplate(templateFileName, orderInfo, tableNumbers: tableNumbersInfo, orderApplicant: orderApplicant);
        }


        private static void FillTemplate(string procuratoryFileName, Order order, List<Lot> prices = null, int[] tableNumbers = null, string orderApplicant = null)
        {
            WordService word = new WordService(procuratoryFileName, false);

            try {
                word.SetCell(1, 2, 2, DateTime.Now.ToShortDateString());
                word.SetCell(1, 4, 2, order.Auction.Date.ToShortDateString());
                word.SetCell(1, 5, 2, "Заявка № " + order.Auction.Number);
                word.SetCell(1, 6, 2, order.Auction.SupplierOrders[0].BrokerName.Replace("Товарищество с ограниченной ответственностью", "ТОО"));
                word.SetCell(1, 7, 2, order.Auction.SupplierOrders[0].BrokerCode);
                word.SetCell(1, 8, 2, order.Auction.SupplierOrders[0].Name);
                word.SetCell(1, 9, 2, order.Auction.SupplierOrders[0].Code);

                int rCount = 0, iCount = 1;

                foreach (var item in order.Auction.Lots) {

                    word.SetCell(1, 11 + rCount, 3, "Цена лота (стартовая - " + Math.Round(item.Sum, 2).ToString() + ")");
                    word.SetCell(1, 12 + rCount, 1, item.Number);
                    word.SetCell(1, 12 + rCount, 2, "Лот №" + iCount);

                    if (prices != null && prices.Any(p => p.Id == item.Id)) {
                        word.SetCell(1, 12 + rCount, 3, prices.First(p => p.Id == item.Id).Sum.ToString());
                        word.SetItalicInCell(1, 12 + rCount, 3, 0);
                        word.SetTextColorInCell(1, 12 + rCount, 3);
                    } else {
                        word.SetItalicInCell(1, 12 + rCount, 3, 1);
                        word.SetCell(1, 12 + rCount, 3, "Введите пожалуйста сумму с учетом понижения");
                    }

                    iCount++;
                    rCount += 2;

                    if (rCount > 16) {
                        word.AddTableRow(1, rCount - 1);
                        word.AddTableRow(1, rCount - 1);
                    }
                }
            } catch (Exception) { }

            try {
                word.FindReplace("[commissionDateIn]", DateTime.Now.ToShortDateString());
                word.FindReplace("[commissionTimeIn]", "11:00");
                word.FindReplace("[brokerPerson]", order.Auction.Trader);
            } catch (Exception) { }

            // Paste tech spec table
            if (tableNumbers != null) {
                foreach (var item in tableNumbers) {
                    try {
                        GetTechSpec(item, orderApplicant);
                    } catch (Exception) { }

                    Thread.Sleep(4000);

                    try {
                        word.PasteInTheBookmark("table");
                    } catch (Exception) { }
                }
            }
            word.AutoFitTables();

            word.CloseDocument(true);
            word.CloseWord(true);
        }


        private static void GetTechSpec(int tableNumber, string orderApplicant)
        {
            if (!String.IsNullOrEmpty(orderApplicant) && File.Exists(orderApplicant)) {
                WordService techSpecWord = new WordService(orderApplicant, false);

                techSpecWord.CopyTable(tableNumber);

                Thread.Sleep(4000);

                techSpecWord.CloseDocument(false);
                techSpecWord.CloseWord(false);
            }
        }


        public static void FormateProcuratory(string templateFileName, Order orderItem, bool autoCounting = false)
        {
            //IExcelService excel = new ExcelService(templateFileName);
            IExcelService excel = new ExcelService(templateFileName);

            // First page
            excel.SetSheetByIndex(1);

            // Main data
            excel.SetCells(5, "D", DateTime.Now.ToShortDateString());
            excel.SetCells(7, "D", orderItem.Auction.Date.ToShortDateString());
            excel.SetCells(8, "D", orderItem.Auction.Number + " от " + orderItem.Date.ToShortDateString());
            excel.SetCells(9, "D", orderItem.Auction.SupplierOrders[0].BrokerName);
            excel.SetCells(10, "D", orderItem.Auction.SupplierOrders[0].BrokerCode);
            excel.SetCells(11, "D", orderItem.Auction.SupplierOrders[0].Name);
            excel.SetCells(12, "D", orderItem.Auction.SupplierOrders[0].Code);
            excel.SetCells(37, "A", orderItem.Auction.Date.ToShortDateString());
            excel.SetCells(37, "D", "11:00");
            excel.SetCells(37, "H", orderItem.Auction.SupplierOrders[0].Trader);

            // Lots data
            int rowCount = 17;
            int lotCount = 1;
            decimal endSum = 0;

            foreach (var item in orderItem.Auction.SupplierOrders[0].lots) {
                excel.SetCells(rowCount, "D", string.Format("Цена лота (стартовая - {0} с НДС)", item.Sum));
                excel.SetCells(rowCount + 1, "A", item.Number);
                excel.SetCells(rowCount + 1, "C", string.Format("Лот №{0}", lotCount));

                endSum = 0;

                if (orderItem.Auction.Procuratories != null && orderItem.Auction.Procuratories.Count > 0) {
                    var procSum = orderItem.Auction.Procuratories.First(p => p.lotId == item.Id);

                    if (procSum != null) endSum = procSum.MinimalPrice;
                }

                excel.SetCells(rowCount + 1, "D", endSum == 0 ? "Пожалуйста введите сумму понижения (с НДС)" : endSum.ToString());
                excel.SetCellItalicStyle(rowCount + 1, "D", true);
                excel.SetCellForegroundColor(rowCount + 1, "D", System.Drawing.Color.FromArgb(128, 128, 128));

                rowCount += 2;
                lotCount++;
            }

            // Tech spec pages
            if (orderItem.Auction.SupplierOrders[0].lots[0].LotsExtended != null && orderItem.Auction.SupplierOrders[0].lots[0].LotsExtended.Count > 0) FillTechSpecSheet(excel, orderItem, autoCounting);

            // Close excel
            excel.CloseWorkbook(true);
            excel.CloseExcel();
        }


        private static void FillTechSpecSheet(IExcelService excel, Order orderItem, bool autoCounting)
        {
            int lotCount = 1;

            foreach (var item in orderItem.Auction.SupplierOrders[0].lots) {
                // Create new sheet and copy header for more then one lot
                if (lotCount > 1) {
                    excel.AddSheet(string.Format("Тех. спец. по лоту №{0}", item.Number));
                    excel.SetSheetByIndex(lotCount);
                    excel.SetSheetByIndex(lotCount + 1);
                    excel.PasteRangeTo(1, "A", 3, "N", lotCount);
                } else excel.SetSheetByIndex(lotCount + 1);

                excel.ChangeSheetName(lotCount + 1, string.Format("Тех. спец. по лоту №{0}", item.Number));

                // Header
                excel.SetCells(1, "A", string.Format("Техническая спецификация к лоту №{0} - {1}", lotCount, item.Number));

                // Formula variables
                decimal endSum = 0;

                if (orderItem.Auction.Procuratories != null && orderItem.Auction.Procuratories.Count > 0) {
                    var procSum = orderItem.Auction.Procuratories.First(p => p.lotId == item.Id);

                    if (procSum != null) endSum = procSum.MinimalPrice;
                }

                //autoCouting
                if (autoCounting) {
                    // Old KV
                    //decimal difference = item.Sum - endSum;
                    //decimal factor = difference / item.LotsExtended.Count;
                    //foreach (var subItem in item.LotsExtended) {
                    //    subItem.endprice = subItem.price - factor / item.Quantity;
                    //    subItem.endsum = subItem.sum - factor;
                    //}

                    // New

                    decimal difference = 100 - ((item.Sum - endSum) / (item.Sum / 100));
                    foreach (var subItem in item.LotsExtended) {
                        subItem.endprice = subItem.price / 100 * difference;
                        subItem.endsum = subItem.endprice * subItem.quantity;
                    }
                }

                // Full sum
                excel.SetCells(5, "F", item.LotsExtended.Sum(l => l.sum));
                excel.SetCells(5, "N", item.LotsExtended.Sum(l => l.endsum));

                // Content
                int rowCount = 4;

                foreach (var subItem in item.LotsExtended) {
                    if (rowCount != 4) excel.InsertRow(rowCount);

                    excel.SetCells(rowCount, "A", subItem.serialnumber);

                    excel.SetCells(rowCount, "B", subItem.name);
                    excel.SetCellWrapText(rowCount, "B", true);

                    excel.SetCells(rowCount, "C", subItem.unit);
                    excel.SetCells(rowCount, "D", subItem.quantity);
                    excel.SetCells(rowCount, "E", subItem.price);
                    excel.SetCells(rowCount, "F", subItem.sum);
                    excel.SetCells(rowCount, "G", subItem.country);
                    excel.SetCellWrapText(rowCount, "G", true);

                    excel.SetCells(rowCount, "H", subItem.techspec);
                    excel.SetCellWrapText(rowCount, "H", true);

                    excel.SetCells(rowCount, "I", subItem.terms);
                    excel.SetCellWrapText(rowCount, "I", true);

                    excel.SetCells(rowCount, "J", subItem.paymentterm);
                    excel.SetCellWrapText(rowCount, "J", true);

                    excel.SetCells(rowCount, "K", subItem.dks);
                    excel.SetCells(rowCount, "L", subItem.contractnumber);
                    excel.SetCells(rowCount, "M", subItem.endprice);
                    excel.SetCells(rowCount, "N", subItem.endsum);

                    for (var i = 1; i < 15; i++) {
                        excel.SetCellFontVAlignment(rowCount, i, 2);
                        excel.SetCellBorder(rowCount, i);
                    }

                    rowCount++;
                }

                lotCount++;
            }
        }


        public static List<LotsExtended> ParseTechSpec(string sourceFileName, string lotCode)
        {
            // Open file in excel
            IExcelService excel = new ExcelService(sourceFileName);

            // Check for right data structure
            excel.SetSheetByIndex(1);
            var procTest = excel.FindRow("Поручение");

            if (procTest == 0) return null;

            // Get sheets count
            int sheetsCount = excel.GetSheetsCount();
            List<LotsExtended> lotExList = new List<LotsExtended>();

            // Parse tables for getting data
            for (var curSheet = 2; curSheet == sheetsCount; curSheet++) {
                excel.SetSheetByIndex(curSheet);

                var lotTest = excel.FindRow(lotCode);

                if (lotTest != 0) {
                    int rowCount = 4;

                    while (!string.IsNullOrEmpty(excel.GetCell(rowCount, "A"))) {
                        LotsExtended lotExItem = new LotsExtended() {
                            serialnumber = Convert.ToInt32(excel.GetCell(rowCount, "A")),
                            name = excel.GetCell(rowCount, "B"),
                            unit = excel.GetCell(rowCount, "C"),
                            quantity = Convert.ToDecimal(excel.GetCell(rowCount, "D")),
                            price = Convert.ToDecimal(excel.GetCell(rowCount, "E")),
                            sum = Convert.ToDecimal(excel.GetCell(rowCount, "F")),
                            country = excel.GetCell(rowCount, "G"),
                            techspec = excel.GetCell(rowCount, "H"),
                            terms = excel.GetCell(rowCount, "I"),
                            paymentterm = excel.GetCell(rowCount, "J"),
                            dks = Convert.ToInt32(excel.GetCell(rowCount, "K")),
                            contractnumber = excel.GetCell(rowCount, "L"),
                            endprice = Convert.ToDecimal(excel.GetCell(rowCount, "M")),
                            endsum = Convert.ToDecimal(excel.GetCell(rowCount, "N"))
                        };

                        lotExList.Add(lotExItem);
                        rowCount++;
                    }
                }
            }

            // Close excel
            excel.CloseWorkbook();
            excel.CloseExcel();

            return lotExList;
        }
    }
}