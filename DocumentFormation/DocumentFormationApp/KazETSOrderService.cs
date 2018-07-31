using AltaBO;
using AltaOffice;
using System;
using System.Text.RegularExpressions;

namespace DocumentFormation {
    public class KazETSOrderService {
        #region Variables
        private static WordService word;
        private static ExcelService excel;
        private static Order order;
        private static string[] orderFiles;
        #endregion

        #region Methods
        public static void FormateOrder(Order orderInfo, string[] orderFilesInfo) {
            order = orderInfo;
            orderFiles = orderFilesInfo;

            FillOrder();
            FillCoverLetter();
            FillSpecification();
        }


        private static void FillOrder() {
            // Fill order
            excel = new ExcelService(orderFiles[0]);

            excel.SetCells(4, "B", "Заявка №" + order.Auction.Number); // Auction number

            if(order.customerid == 1) {
                excel.SetCells(7, "C", "ТОО 'Востокцветмет', Восточно - Казахстанская область, г Усть - Каменогорск, улица имени Александра Протозанова, 121, БИН: 140740012829, ИИК: KZ666010151000205076, АО 'Народный Банк Казахстан', БИК: HSBKKZKX"); // Customer data for VCM
                excel.SetCells(30, "B", "Начальник отдела закупок "); // Head trader
                excel.DeleteAllPictures();
            } else if(order.customerid == 7) {
                excel.SetCells(7, "C", "ТОО 'Полиметалл' Адрес: РК, ВКО, Жарминский район, поселок Ауэзов, Промышленная зона, БИН 930340000251, ИИК KZ23914102203KZ000A3 – KZT, Банковские реквизиты:, БИК SABRKZKA"); // Customer data for Polymetall
                excel.SetCells(30, "B", "Директор службы снабжения  ТОО 'Полиметалл'"); // Head trader
                excel.DeleteAllPictures();
            } else if(order.customerid == 11) {
                excel.SetCells(7, "C", "ТОО 'Vertex Holding' Адрес: РК, г.Алматы, ул. Чайковского, 170, БИН 041240005077, ИИК KZ57722S000000127438"); // Customer data for V
                excel.SetCells(30, "B", "Директор службы снабжения  ТОО 'Vertex Holding'"); // Head trader
                excel.DeleteAllPictures();
            } else if(order.customerid == 12) {
                excel.SetCells(7, "C", "ТОО 'Kerem Equipment LTD (Керем Иквипмент ЛТД)', Адрес: РК, г. Семей, ул. Интернациональная, 52, БИН 090640017369, ИИК KZ428560000003939474 - KZT"); // Customer data for Kerem
                excel.SetCells(30, "B", "Директор службы снабжения  ТОО 'Kerem Equipment LTD (Керем Иквипмент ЛТД)'"); // Head trader
                excel.DeleteAllPictures();
            } else if (order.customerid == 14) {
                excel.SetCells(7, "C", "ТОО 'MADOT OIL', Адрес: РК, г. Алматы, ул. Кастеева 106 'В', БИН 161240022071, ИИК KZ619261802192594000 - KZT"); // Customer data for Kerem
                excel.SetCells(30, "B", "Директор службы снабжения  ТОО 'MADOT OIL'"); // Head trader
                excel.DeleteAllPictures();
            } else if (order.customerid == 8) {
                excel.SetCells(7, "C", "ТОО 'M-Ali Petrol', Адрес: РК, г. Алматы, ул. Кастеева 106 В, БИН 161040025306, ИИК KZ559261802191065000 - KZT"); // Customer data for Kerem
                excel.SetCells(30, "B", "Директор службы снабжения  ТОО 'M-Ali Petrol'"); // Head trader
                excel.DeleteAllPictures();
            } else if (order.customerid == 22) {
                excel.SetCells(7, "C", "ТОО 'Фэлкон Ойл энд Гэс ЛТД', Адрес: г. Алматы, Алмалинский район, ул. Шевченко 90, Бизнес-центр «Каратал», 9 этаж., БИН 000940000676, ИИК KZ268560000000523422 - KZT"); // Customer data for Kerem
                excel.SetCells(30, "B", "Директор службы снабжения  ТОО 'Фэлкон Ойл энд Гэс ЛТД'"); // Head trader
                excel.DeleteAllPictures();
            }

            excel.SetCells(8, "C", order.Auction.Lots[0].Name); // Lot name
            excel.SetCells(9, "C", order.Auction.Lots[0].CodeTRFEA); // TR FEA
            excel.SetCells(10, "C", order.Auction.Lots[0].Name); // Lot description
            excel.SetCells(11, "C", order.Auction.Lots[0].UnitId != 11 ? order.Auction.Lots[0].Price.ToString() : order.Auction.Lots[0].Unit); // Start price
            excel.SetCells(12, "C", order.Auction.Lots[0].UnitId != 11 ? order.Auction.Lots[0].Quantity.ToString() : order.Auction.Lots[0].Unit); // Quantity
            excel.SetCells(13, "C", order.Auction.Lots[0].Unit); // Unit of size
            excel.SetCells(14, "C", order.Auction.Lots[0].Sum); // Amount sum
            excel.SetCells(15, "C", order.Auction.Lots[0].Step); // Step
            excel.SetCells(16, "C", order.Auction.Lots[0].PaymentTerm); // Payment
            excel.SetCells(17, "C", order.Auction.Lots[0].DeliveryPlace); // Delivery term
            excel.SetCells(18, "C", order.Auction.Lots[0].DeliveryTime); // Delivery time
            excel.SetCells(20, "C", order.Auction.Lots[0].LocalContent); // Local
            excel.SetCells(21, "C", order.Auction.Date.ToShortDateString()); // Auction date
            excel.SetCells(23, "C", order.Auction.Lots[0].Warranty); // Warranty
            excel.SetCells(25, "C", CountWorkDays(order.Auction.Date, 10)); // Order ellapse date
            excel.SetCells(28, "B", "Дата " + order.Date.ToShortDateString()); // Order date

            // Set cursor on first position
            excel.SetRange(1, "A");

            // Convert order to pdf
            excel.SaveAsPDF(orderFiles[0]);

            // Close order
            excel.CloseWorkbook(true);
            excel.CloseExcel();
        }


        private static void FillCoverLetter() {
            // Fill cover letter
            excel = new ExcelService(orderFiles[1]);

            excel.SetCells(3, "A", "ЗАЯВКА №" + order.Auction.Number); // Order number
            excel.SetCells(7, "E", order.Auction.Customer); // Customer name
            excel.SetCells(11, "B", order.Auction.Lots[0].Name); // Lot name
            excel.SetCells(11, "D", order.Auction.Lots[0].Unit); // Unit of size
            excel.SetCells(11, "E", order.Auction.Lots[0].UnitId != 11 ? order.Auction.Lots[0].Quantity.ToString() : order.Auction.Lots[0].Unit); // Quantity
            excel.SetCells(11, "F", (order.Auction.Lots[0].UnitId == 11 ? "По приложению" : order.Auction.Lots[0].Price.ToString())); // Price
            excel.SetCells(11, "H", order.Auction.Lots[0].Sum); // Sum
            excel.SetCells(12, "H", order.Auction.Lots[0].Sum); // Amount sum
            excel.SetCells(18, "A", "\"" + order.Date.Day + "\" " + GetMonthName(order.Date.Month) + " " + order.Date.Year + " г."); // Order date

            // Convert cover letter to pdf
            excel.SaveAsPDF(orderFiles[1], false);

            // Close cover letter
            excel.CloseWorkbook(true);
            excel.CloseExcel();
        }


        private static void FillSpecification() {
            // Fill specification
            word = new WordService(orderFiles[2], false);

            word.FindReplace("[auctionNumber]", order.Auction.Number); // Auction number

            word.SetCell(1, 2, 1, "1"); // Number
            word.SetCell(1, 2, 2, order.Auction.Lots[0].Name); // Name
            word.SetCell(1, 2, 3, order.Auction.Lots[0].Unit); // Unit of size
            word.SetCell(1, 2, 4, order.Auction.Lots[0].Quantity.ToString()); // Quantity
            word.SetCell(1, 2, 5, order.Auction.Lots[0].Price.ToString()); // Price
            word.SetCell(1, 2, 6, order.Auction.Lots[0].Sum.ToString()); // Sum
            word.SetCell(1, 2, 7, order.Auction.Lots[0].Step.ToString()); // Step
            word.SetCell(1, 2, 8, order.Auction.Lots[0].DeliveryPlace + " | " + order.Auction.Lots[0].DeliveryTime); // Terms
            word.SetCell(1, 2, 9, order.Auction.Lots[0].PaymentTerm); // Payment
            word.SetCell(1, 2, 10, order.Auction.Lots[0].Warranty.ToString()); // Warranty
            word.SetCell(1, 2, 11, order.Auction.Lots[0].LocalContent.ToString()); // Local

            // Close specification
            word.CloseDocument(true);
            word.CloseWord(true);
        }


        private static string GetMonthName(int month) {
            string[] monthName = new string[12] { "января", "февраля", "марта", "апреля", "мая", "июня", "июля", "августа", "сентября", "октября", "ноября", "декабря" };

            return monthName[month - 1];
        }


        private static string CountWorkDays(DateTime startDate, int daysCount) {
            DateTime iDate = startDate;

            for(var i = 1; i <= daysCount; i++) {
                iDate = iDate.AddDays(1);

                if(iDate.DayOfWeek == DayOfWeek.Saturday) iDate = iDate.AddDays(2);
                if(iDate.DayOfWeek == DayOfWeek.Sunday) iDate = iDate.AddDays(1);
            }

            return iDate.ToShortDateString();
        }


        public static Lot GetLotData(string orderFileName) {
            Lot lot = new Lot();

            // Open file
            excel = new ExcelService(orderFileName);

            // Check for sheets count
            if(excel.GetSheetsCount(false) < 3) excel.AddSheet("");

            // Parse file and get data
            excel.SetSheetByIndex(1);

            // Check for space between hat and broker
            if(string.IsNullOrEmpty(excel.GetCell(4, "B"))) excel.InsertRow(4);

            lot.Number = "1";
            lot.Name = excel.GetCell(9, "C");

            var quantityCell = excel.GetCell(13, "C");

            try {
                if(quantityCell != null) lot.Quantity = string.IsNullOrEmpty(quantityCell) ? 0 : quantityCell.ToLower().Contains("прилож") ? 0 : Convert.ToDecimal(quantityCell);
                else lot.Quantity = 0;
            } catch { lot.Quantity = 0; }

            Regex regex = new Regex(@"[\d]*[,]*[\d]*");

            lot.Price = lot.Quantity == 0 ? 0 : Convert.ToDecimal(regex.Match(excel.GetCell(12, "C").Replace(" ", "")).Value);
            lot.Sum = lot.Quantity == 0 ? Convert.ToDecimal(regex.Match(excel.GetCell(15, "C").Replace(" ", "")).Value) : lot.Quantity * lot.Price;

            try {
                lot.Step = Convert.ToDecimal(regex.Match(excel.GetCell(16, "C")).Value);
            } catch { lot.Step = 0; }

            lot.PaymentTerm = excel.GetCell(17, "C");
            lot.DeliveryPlace = excel.GetCell(18, "C");
            lot.DeliveryTime = excel.GetCell(19, "C");
            lot.LocalContent = Convert.ToDecimal(regex.Match(excel.GetCell(21, "C")).Value);
            lot.Warranty = Convert.ToDecimal(regex.Match(excel.GetCell(24, "C")).Value);

            // Parse tech spec part for lot ex
            excel.SetSheetByIndex(2);

            lot.LotsExtended = new System.Collections.ObjectModel.ObservableCollection<LotsExtended>();
            int startRow = excel.FindRow("Наименование");

            if(startRow != 0) {
                int iRow = startRow + 1;
                int iCount = 1;

                while(!string.IsNullOrEmpty(excel.GetCell(iRow, "A")) || !string.IsNullOrEmpty(excel.GetCell(iRow, "B"))) {
                    lot.LotsExtended.Add(new LotsExtended() {
                        serialnumber = iCount,
                        name = excel.GetCell(iRow, "B"),
                        unit = excel.GetCell(iRow, "C"),
                        quantity = Convert.ToDecimal(excel.GetCell(iRow, "D")),
                        price = Convert.ToDecimal(excel.GetCell(iRow, "E")),
                        sum = Convert.ToDecimal(excel.GetCell(iRow, "F")),
                        country = string.IsNullOrEmpty(excel.GetCell(iRow, "G")) ? "" : excel.GetCell(iRow, "G"),
                        techspec = excel.GetCell(iRow, "H"),
                        terms = excel.GetCell(iRow, "I"),
                        paymentterm = excel.GetCell(iRow, "J"),
                        dks = Convert.ToInt32(Math.Round(Convert.ToDecimal(regex.Match(excel.GetCell(iRow, "K")).Value), 0)),
                        contractnumber = string.IsNullOrEmpty(excel.GetCell(iRow, "L")) ? "" : excel.GetCell(iRow, "L")
                    });

                    iCount++;
                    iRow++;
                }

                lot.Dks = lot.LotsExtended[0].dks;
            }

            // close excel
            excel.CloseWorkbook(false);
            excel.CloseExcel();

            // Return collected data
            return lot;
        }
        #endregion
    }
}