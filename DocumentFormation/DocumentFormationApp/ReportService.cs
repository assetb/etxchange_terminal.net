using AltaArchiveApp;
using AltaBO;
using AltaMySqlDB.model;
using AltaMySqlDB.model.tables;
using AltaMySqlDB.service;
using AltaOffice;
using AltaTransport;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentFormation
{
    /// <summary>
    /// Business functions over report BO conserning to document formation.
    /// </summary>
    public class ReportService
    {
        #region ETS reports
        private static IDataManager dataManager = new EntityContext();
        private static ArchiveManager archiveManager;
        /// <summary>
        /// Insert the report BO data into a file template of a report for suppliers.
        /// Вставка данных в шаблон.
        /// </summary>
        /// <param name="report">report BO to insert</param>
        /// <param name="service">file template of report for supplier</param>
        /// <returns>resulting file template with data</returns>
        private static WordService InsertIntoSupplierReport(Report report, WordService service)
        {
            switch (report.Id.ToLower()) {
                case "altk":
                    report.BrokerName = "ТОО Альта и К";
                    break;
                case "kord":
                    report.BrokerName = "ТОО Корунд-777";
                    break;
                case "alta":
                    report.BrokerName = "ТОО Альтаир-Нур";
                    break;
                case "akal":
                    report.BrokerName = "ТОО Ак Алтын Ко";
                    break;
                default:
                    report.BrokerName = "";
                    break;
            }

            service.FindReplace("[firmBroker]", report.BrokerName);
            service.FindReplace("[number]", report.Number);

            var cName = report.ClientName;
            var lotInfo = DataBaseClient.GetLotByCode(report.Code);
            var supplierInfo = DataBaseClient.GetSupplierByBroker_Auction(cName.Substring(cName.Length - 4), lotInfo == null ? 0 : lotInfo.auctionid, lotInfo == null ? 0 : lotInfo.id);

            if (supplierInfo != null) {
                cName = supplierInfo[1];

                // Change statuses of supplier orders to lose
                ChangeStatusesToSO(lotInfo.auctionid, Convert.ToInt32(supplierInfo[4]));
            }

            service.FindReplace("[supplierName]", cName);
            service.FindReplace("[auctionDate]", report.DateTo);
            service.FindReplace("[dealRegDate]", report.Moment);
            service.FindReplace("[productName]", report.ProductName);

            decimal startPrice = DataBaseClient.GetPriceFromLot(report.Code);
            string sPrice = $"{startPrice:C}";
            sPrice = sPrice.Substring(0, sPrice.Length - 2);

            service.FindReplace("[startPrice]", startPrice == 0 ? "Введите стартовую цену" : sPrice + " тенге, с учетом НДС");
            service.FindReplace("[lotCode]", report.Code);
            service.FindReplace("[qty]", report.Qty);

            try {
                service.SetCell(1, 11, 2, lotInfo.auction.customer.company.name);
            } catch { }

            service.FindReplace("[lastPrice]", report.Price);
            service.FindReplace("[amountSum]", report.Amt);
            service.FindReplace("[clientBrokerCode]", report.ContrCode);
            service.FindReplace("[directorName]", report.Director);

            // Make record in final report table
            try {
                if (lotInfo != null && supplierInfo != null) {
                    var finalReport = DataBaseClient.ReadFinalReport(lotInfo.auctionid, lotInfo.id);
                    decimal lastPrice = Convert.ToDecimal(report.Amt.Substring(0, report.Amt.IndexOf(" т") - 2).Replace(" ", ""));

                    if (finalReport != null) DataBaseClient.DeleteFinalReport(finalReport);

                    FinalReportEF finalReportItem = new FinalReportEF() {
                        auctionId = lotInfo.auctionid,
                        dealNumber = report.Number,
                        supplierId = Convert.ToInt32(supplierInfo[2]),
                        lotId = lotInfo.id,
                        finalPriceOffer = lastPrice,
                        brokerId = Convert.ToInt32(supplierInfo[3])
                    };

                    DataBaseClient.CreateFinalReport(finalReportItem);

                    // Change procuratory
                    try {
                        DataBaseClient.UpdateProcuratory(Convert.ToInt32(supplierInfo[2]), lotInfo.id, lastPrice);
                    } catch (Exception) { }

                    DataBaseClient.SetAuctionStatus(lotInfo.auctionid, 2);
                }
            } catch (Exception) { }

            return service;
        }


        /// <summary>
        /// Insert the report BO data into a file template of a report for customers.
        /// Вставка данных в шаблон отчета для заказчиков.
        /// </summary>
        /// <param name="report">report BO to insert</param>
        /// <param name="service">file template of report for customer</param>
        /// <returns>resulting file template with data</returns>
        private static WordService InsertIntoClientReport(Report report, WordService service)
        {
            var lotInfo = DataBaseClient.GetLotByCode(report.Code);

            try {
                service.FindReplace("[orderNumber]", lotInfo == null ? "Номер аукциона" : lotInfo.auction.number);
                service.FindReplace("[dealNumber]", report.Number);

                try {
                    service.SetCell(1, 6, 2, lotInfo.auction.customer.company.name);
                    var customerCode = DataBaseClient.GetSupplierCodeByCompany(lotInfo.auction.customer.company.id);

                    if (customerCode != null) service.SetCell(1, 7, 2, customerCode);
                } catch { }

                service.FindReplace("[auctionDate]", report.Moment.Substring(0, report.Moment.IndexOf(" ", StringComparison.Ordinal)));
                service.FindReplace("[dealRegDate]", report.Moment);
                service.FindReplace("[lotCode]", report.Code);
                service.FindReplace("[productName]", report.ProductName);
            } catch (Exception) { }

            string lotSum = lotInfo != null ? $"{lotInfo.sum:C}" : "0,00";
            lotSum = lotSum.Substring(0, lotSum.Length - 2);

            service.FindReplace("[startPrice]", lotInfo == null ? "Введите стартовую цену" : lotSum + " тенге, с учетом НДС");
            service.FindReplace("[amountSum]", report.Amt);

            var supplierInfo = DataBaseClient.GetSupplierByBroker_Auction(report.ContrCode, lotInfo == null ? 0 : lotInfo.auctionid, lotInfo == null ? 0 : lotInfo.id);

            service.FindReplace("[supplierCode]", supplierInfo == null ? "Код поставщика" : supplierInfo[0]);
            service.FindReplace("[supplierName]", supplierInfo == null ? "Наименование поставщика" : supplierInfo[1]);
            service.FindReplace("[brokerCode]", report.ContrCode);

            switch (report.ContrCode.ToLower()) {
                case "altk":
                    report.BrokerName = "ТОО Альта и К";
                    break;
                case "kord":
                    report.BrokerName = "ТОО Корунд-777";
                    break;
                case "alta":
                    report.BrokerName = "ТОО Альтаир Нур";
                    break;
                case "akal":
                    report.BrokerName = "ТОО Ак Алтын Ко";
                    break;
                default:
                    report.BrokerName = "";
                    break;
            }

            service.FindReplace("[brokerName]", report.BrokerName);

            // Change auction status
            try {
                if (lotInfo != null) DataBaseClient.SetAuctionStatus(lotInfo.auctionid, 2);
            } catch (Exception) { }

            return service;
        }


        public static void CreateReport(string template, Report report, int type)
        {
            archiveManager = new ArchiveManager(dataManager);
            var service = new WordService(template, false);
            service = type == 2 ? InsertIntoSupplierReport(report, service) : InsertIntoClientReport(report, service);
            var fileListId = DataBaseClient.GetFileListAuctionByLot(report.Code);

            if (fileListId != null) {
                DocumentRequisite docReq = new DocumentRequisite() {
                    date = fileListId.date,
                    fileName = template.Substring(template.LastIndexOf("\\") + 1),
                    market = AltaBO.specifics.MarketPlaceEnum.ETS,
                    number = fileListId.number,
                    section = AltaBO.specifics.DocumentSectionEnum.Auction,
                    type = type == 2 ? AltaBO.specifics.DocumentTypeEnum.SupplierReport : AltaBO.specifics.DocumentTypeEnum.CustomerReport
                };

                archiveManager.SaveFile(docReq, (int)fileListId.fileslistid);
                docReq.fileName = docReq.fileName.Replace(".docx", ".pdf");
                archiveManager.SaveFile(docReq, (int)fileListId.fileslistid);
            }

            service.SaveAsPDF(template);
            service.CloseDocument(true);
            service.CloseWord(true);
        }
        #endregion

        #region UTB Report
        private static Order order = new Order();
        private static string[] reportFileNames = new string[2];

        public static void CreateReports(Order orderInfo)
        {
            order = orderInfo;

            reportFileNames = ArchiveTransport.PutUTBReports(orderInfo);

            FillTemplate();

            // Put file to base
            var fileListId = DataBaseClient.ReadAuction(order.Auction.Id);

            if (fileListId != null) {
                DocumentRequisite docReq = new DocumentRequisite() {
                    date = order.Auction.Date,
                    fileName = reportFileNames[0].Substring(reportFileNames[0].LastIndexOf("\\")),
                    market = AltaBO.specifics.MarketPlaceEnum.UTB,
                    number = order.Auction.Number,
                    section = AltaBO.specifics.DocumentSectionEnum.Auction,
                    type = AltaBO.specifics.DocumentTypeEnum.CustomerReport
                };

                // Customer file
                // Check for exist
                if (DataBaseClient.ReadDocument((int)fileListId.fileslistid, (int)AltaBO.specifics.DocumentTypeEnum.CustomerReport) == null) {
                    archiveManager.SaveFile(docReq, (int)fileListId.fileslistid);

                    docReq.fileName = reportFileNames[1].Substring(reportFileNames[1].LastIndexOf("\\"));
                }

                // Supplier file
                // Check for exist
                if (DataBaseClient.ReadDocument((int)fileListId.fileslistid, (int)AltaBO.specifics.DocumentTypeEnum.SupplierReport) == null) {
                    docReq.type = AltaBO.specifics.DocumentTypeEnum.SupplierReport;

                    archiveManager.SaveFile(docReq, (int)fileListId.fileslistid);
                }
            }
            //
        }


        public static void CreateReports(Order orderInfo, string clientFileName, string supplierfileName)
        {
            order = orderInfo;
            reportFileNames[0] = clientFileName;
            reportFileNames[1] = supplierfileName;

            FillTemplate();
        }


        private static WordService word;
        private static int winApplicant = 0, winBroker = 0, winProcuratoryNum;
        private static decimal minimalPrice = 0;

        private static void FillTemplate()
        {
            // TODO make one function for all
            minimalPrice = order.Auction.Procuratories.Min(p => p.MinimalPrice);

            winApplicant = 0;

            foreach (var item in order.Auction.Procuratories) {
                if (order.Auction.Procuratories[winApplicant].MinimalPrice == minimalPrice) break;
                winApplicant++;
            }

            winBroker = 0;

            foreach (var item in order.Auction.SupplierOrders) {
                if (order.Auction.SupplierOrders[winBroker].SupplierId == order.Auction.Procuratories[winApplicant].SupplierId) break;
                winBroker++;
            }

            // Customer report
            word = new WordService(reportFileNames[0], false);

            word.FindReplace("[auctionNumber]", order.Auction.Number);

            word.SetCell(1, 1, 2, order.Auction.Customer);
            word.SetCell(1, 3, 2, order.Auction.Date.ToShortDateString() + " " + order.Auction.Type);
            word.SetCell(1, 5, 2, order.Auction.SupplierOrders[winBroker].Name);
            word.SetCell(1, 6, 2, order.Auction.ProtocolNumber);

            // Table
            try {
                word.SetCell(1, 7, 2, order.Auction.Lots[0].Name);
                word.SetCell(1, 8, 2, order.Auction.Lots[0].UnitId == 11 ? "По приложению" : order.Auction.Lots[0].Quantity.ToString());
                word.SetCell(1, 9, 2, order.Auction.Lots[0].Unit);
                word.SetCell(1, 10, 2, order.Auction.Lots[0].UnitId == 11 ? "По приложению" : (minimalPrice / order.Auction.Lots[0].Quantity).ToString("C").Replace(" ₽", ""));
                word.SetCell(1, 11, 2, minimalPrice.ToString("C").Replace(" ₽", ""));
                word.SetCell(1, 12, 2, order.Auction.Lots[0].Sum.ToString("C").Replace(" ₽", ""));
                word.SetCell(1, 13, 2, (order.Auction.Lots[0].Sum - minimalPrice).ToString("C").Replace(" ₽", ""));
                word.SetCell(1, 14, 2, order.Auction.Lots[0].PaymentTerm);
                word.SetCell(1, 15, 2, order.Auction.Lots[0].DeliveryTime);
                word.SetCell(1, 16, 2, order.Auction.Lots[0].DeliveryPlace);
            } catch (Exception) { }

            word.SaveAsPDF(reportFileNames[0]);

            word.CloseDocument(true);
            word.CloseWord(true);

            // Supplier report
            word = new WordService(reportFileNames[1], false);

            word.FindReplace("[auctionNumber]", order.Auction.Number);

            word.SetCell(1, 1, 2, order.Auction.SupplierOrders[winBroker].Name);
            word.SetCell(1, 2, 2, order.Auction.Customer);
            word.SetCell(1, 4, 2, order.Auction.Date.ToShortDateString() + " " + order.Auction.Type);
            word.SetCell(1, 6, 2, order.Auction.ProtocolNumber);
            word.SetCell(1, 7, 2, order.Auction.Lots[0].Name);
            word.SetCell(1, 8, 2, order.Auction.Lots[0].UnitId == 11 ? "По приложению" : order.Auction.Lots[0].Quantity.ToString());
            word.SetCell(1, 9, 2, order.Auction.Lots[0].Unit);
            word.SetCell(1, 10, 2, order.Auction.Lots[0].UnitId == 11 ? "По приложению" : (minimalPrice / order.Auction.Lots[0].Quantity).ToString("C").Replace(" ₽", ""));
            word.SetCell(1, 11, 2, minimalPrice.ToString("C").Replace(" ₽", ""));
            word.SetCell(1, 12, 2, order.Auction.Lots[0].Sum.ToString("C").Replace(" ₽", ""));
            word.SetCell(1, 14, 2, order.Auction.Lots[0].PaymentTerm);
            word.SetCell(1, 15, 2, order.Auction.Lots[0].DeliveryTime);
            word.SetCell(1, 16, 2, order.Auction.Lots[0].DeliveryPlace);

            word.SaveAsPDF(reportFileNames[1]);

            word.CloseDocument(true);
            word.CloseWord(true);
        }
        #endregion

        #region Utilits
        private static void ChangeStatusesToSO(int auctionId, int supplierOrderId)
        {
            foreach (var item in DataBaseClient.ReadSupplierOrders(auctionId)) {
                if (item.statusid != 16 && item.statusid != 23) {
                    item.statusid = 24;

                    try {
                        DataBaseClient.UpdateSupplierOrder(item);
                    } catch { }
                }
            }

            var soItem = DataBaseClient.ReadSupplierOrder(supplierOrderId);

            if (soItem != null) {
                soItem.statusid = 23;

                try {
                    DataBaseClient.UpdateSupplierOrder(soItem);
                } catch { }
            }
        }
        #endregion

        #region Kaspi report
        public bool CreateKaspiReport(string templateFileName, Order order, List<PriceOffer> priceOffers, string accountNumber, string finalPriceOffer)
        {
            if (string.IsNullOrEmpty(templateFileName) || order == null || order.Auction == null || order.Auction.Lots == null || order.Auction.Lots.Count < 1 ||
                order.Auction.Lots[0].LotsExtended == null || order.Auction.Lots[0].LotsExtended.Count < 1 || order.Auction.SupplierOrders == null || order.Auction.SupplierOrders.Count < 1 ||
                priceOffers == null || priceOffers.Count < 1) return false;

            var word = new WordService(templateFileName, false);

            if (word == null) return false;

            try {
                word.FindReplace("[accountNumber]", accountNumber != null ? accountNumber : "");
                word.FindReplace("[auctionType]", order.Auction.Type != null ? order.Auction.Type : "");
                word.FindReplace("[brokerCustomer]", order.Organizer != null ? order.Organizer : "");
                word.FindReplace("[auctionDate]", order.Auction.Date != null ? order.Auction.Date.ToString() : "");
                word.FindReplace("[productName]", order.Auction.Lots[0].Name != null ? order.Auction.Lots[0].Name : "");
                word.FindReplace("[lotName]", order.Auction.Lots[0].Name != null ? order.Auction.Lots[0].Name : "");

                var supplierOrderWinner = order.Auction.SupplierOrders.FirstOrDefault(s => s.status.Id == 23);

                word.FindReplace("[brokerSupplier]", supplierOrderWinner != null ? supplierOrderWinner.BrokerName != null ? supplierOrderWinner.BrokerName : "" : "");
                word.FindReplace("[startPrice]", order.Auction.Lots[0].Price != null ? order.Auction.Lots[0].Price.ToString() : "");
                word.FindReplace("[finalPriceOffer]", finalPriceOffer != null ? finalPriceOffer : "");
                word.FindReplace("[productsCount]", order.Auction.Lots[0].LotsExtended.Count > 1 ? "Согласно спецификации" : order.Auction.Lots[0].Quantity != null ? order.Auction.Lots[0].Quantity.ToString() : "");

                int rowCount = 2;

                foreach (var item in order.Auction.SupplierOrders) {
                    if (rowCount > 2) word.AddTableRow(1);

                    word.SetCell(1, rowCount, 1, item.Name != null ? item.Name : "");
                    word.SetCell(1, rowCount, 2, item.date != null ? item.date.ToString() : "");

                    rowCount += 1;
                }

                rowCount = 2;

                foreach (var item in priceOffers) {
                    if (rowCount > 2) word.AddTableRow(2);

                    word.SetCell(2, rowCount, 1, (rowCount - 1).ToString());
                    word.SetCell(2, rowCount, 1, item.firmName != null ? item.firmName : "");
                    word.SetCell(2, rowCount, 1, item.lotPriceOffer != null ? item.lotPriceOffer : "");
                    word.SetCell(2, rowCount, 1, item.offerTime != null ? item.offerTime.ToString() : "");

                    rowCount += 1;
                }

                word.CloseDocument(true);
                word.CloseWord(true);
            } catch {
                if (word.IsOpenDocument()) word.CloseDocument(false);
                if (word.IsOpenWord()) word.CloseWord(false);

                return false;
            }

            return true;
        }
        #endregion
    }
}