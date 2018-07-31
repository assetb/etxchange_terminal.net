using System;
using System.Globalization;
using AltaBO;
using AltaBO.specifics;
using AltaOffice;
using AltaTransport;
using AltaArchiveApp;
using AltaMySqlDB.service;

namespace DocumentFormation {

    public class UTBOrderService {
        private static Order orderInfo;
        private static string orderFileName;
        private static WordService word;
        private static ExcelService excel;
        private static ArchiveManager archiveManager;


        public static Order ParseOrder(string orderFileName, Order order) {
            int rowCount = 0;
            bool isEnd = false;

            orderInfo = order;

            // Open file
            excel = new ExcelService(orderFileName);

            excel.SetActiveSheet("ЗАЯВКА №");

            if(excel.IsSheetOpened()) {
                // Get broker
                rowCount = excel.FindRow("Альта и К");
                orderInfo.Auction.Broker = new Broker();

                if(rowCount != 0) orderInfo.Auction.Broker.Code = "ALTK";
                else {
                    rowCount = excel.FindRow("Альтаир-Нур");

                    if(rowCount != 0) orderInfo.Auction.Broker.Code = "ALTA";
                    else {
                        rowCount = excel.FindRow("Корунд");

                        if(rowCount != 0) orderInfo.Auction.Broker.Code = "KORD";
                    }
                }

                // Get lots
                rowCount = excel.FindRow("№ п/п") + 1;

                orderInfo.Auction.Lots = new System.Collections.ObjectModel.ObservableCollection<Lot>();

                while(isEnd) {
                    if(!string.IsNullOrEmpty(excel.GetCell(rowCount, "A"))) {
                        orderInfo.Auction.Lots.Add(new Lot() {
                            Number = excel.GetCell(rowCount, "A"),
                            Name = excel.GetCell(rowCount, "B"),
                            Quantity = Convert.ToDecimal(excel.GetCell(rowCount, "C")),
                            Unit = excel.GetCell(rowCount, "D"),
                            Price = Convert.ToDecimal(excel.GetCell(rowCount, "E")),
                            Sum = Convert.ToDecimal(excel.GetCell(rowCount, "F")),
                            Step = Convert.ToDecimal(excel.GetCell(rowCount, "G")),
                            DeliveryPlace = excel.GetCell(rowCount, "H"),
                            PaymentTerm = excel.GetCell(rowCount, "I"),
                            Warranty = Convert.ToDecimal(excel.GetCell(rowCount, "J").Replace(",", ".")),
                            LocalContent = Convert.ToDecimal(excel.GetCell(rowCount, "K").Replace("%", ""))
                        });
                    } else isEnd = true;
                }
                return orderInfo;
            }
            return null;
        }


        public static void GenerateOrder(Order order) {
            archiveManager = new ArchiveManager();

            orderInfo = order;

            var docReq = new DocumentRequisite() {
                fileName = "Заявка №" + order.Auction.Number.Replace("/", "_") + ".docx",
                date = order.Auction.Date,
                market = MarketPlaceEnum.UTB,
                number = order.Auction.Number,
                type = DocumentTypeEnum.Order,
                section = DocumentSectionEnum.Auction
            };

            orderFileName = archiveManager.GetTemplate(docReq, DocumentTemplateEnum.Order);

            if(!string.IsNullOrEmpty(orderFileName)) {
                orderFileName = FillTemplateFile();

                if(orderFileName != null) {
                    docReq.fileName.Replace(".docx", ".pdf");
                    archiveManager.SaveFile(docReq, order.Auction.FilesListId);
                }
            }
        }


        public static void GenerateOrder(Order order, string fileName) {
            if(order == null) return;

            orderInfo = order;
            orderFileName = fileName;

            FillTemplateFile();
        }


        private static string FillTemplateFile() {
            word = new WordService(orderFileName, false);

            try {
                // Header
                word.FindReplace("[orderNumber]", orderInfo.Auction.Number);
                word.FindReplace("[customer]", orderInfo.Auction.Customer);
                word.FindReplace("[broker]", (orderInfo.Auction.Broker.Code == "ALTK" ? "Альта и К" : orderInfo.Auction.Broker.Code == "ALTA" ? "Альтаир-Нур" : "Корунд-777"));
                word.FindReplace("[type]", orderInfo.Auction.Type);
                word.FindReplace("[auctionDate]", orderInfo.Auction.Date.ToShortDateString());

                // Table
                int rowCount = 2;
                decimal fullSum = 0;

                foreach(var item in orderInfo.Auction.Lots) {
                    if(rowCount > 2) word.AddTableRow(1);

                    word.SetCell(1, rowCount, 1, "1");
                    word.SetCell(1, rowCount, 2, item.Name);
                    word.SetCell(1, rowCount, 3, Math.Round(item.Quantity, 2).ToString(CultureInfo.InvariantCulture));
                    word.SetCell(1, rowCount, 4, item.Unit);
                    word.SetCell(1, rowCount, 5, Math.Round(item.Price, 2).ToString(CultureInfo.InvariantCulture));
                    word.SetCell(1, rowCount, 6, Math.Round((item.Quantity * item.Price), 2).ToString(CultureInfo.InvariantCulture));
                    word.SetCell(1, rowCount, 7, Math.Round(item.Step, 2).ToString(CultureInfo.InvariantCulture));
                    word.SetCell(1, rowCount, 8, item.DeliveryPlace);
                    word.SetCell(1, rowCount, 9, item.PaymentTerm);
                    word.SetCell(1, rowCount, 10, Math.Round(item.Warranty, 1).ToString(CultureInfo.InvariantCulture));
                    word.SetCell(1, rowCount, 11, Math.Round(item.LocalContent, 0).ToString(CultureInfo.InvariantCulture));

                    fullSum += Math.Round((item.Quantity * item.Price), 2);
                    rowCount++;
                }

                word.SetCell(1, rowCount + 1, 6, fullSum.ToString());

                // Footer (requisites)
                word.FindReplace("[executorName]", orderInfo.Initiator);


                word.SaveAsPDF(orderFileName);

                word.CloseDocument(true);
                word.CloseWord(true);

                Service.DeleteFile(orderFileName);
                return orderFileName.Replace(".docx", ".pdf");
            } catch(Exception) { return null; }
        }
    }
}