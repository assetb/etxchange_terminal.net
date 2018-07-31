using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using altaik.baseapp.ext;
using AltaBO;
using AltaOffice;
using DocumentFormation.model;
using System;

namespace DocumentFormation {
    public class VostokOrderService_ref : CustomerOrderService {
        #region Variables
        private const string WARRANTY_LETER = "Настоящим подтверждаю действительность письма Гарантийного письма ТОО \"Востокцветмет\"   № 07-2/6 от «5» января 2017 года и в случае его недействительности либо отказа Инициатора от его исполнения, принимаю на себя все указанные в Гарантийном письме обязательства.";

        private readonly ExcelService excel;
        private readonly WordService word;

        private string lotName, lotMinRequerments;
        private string lotStartPrice;
        #endregion

        #region Functions
        public VostokOrderService_ref(string orderFileName, string treatyFileName) {
            excel = new ExcelService(orderFileName);
            word = new WordService(treatyFileName, true);
        }


        public override Order UpdateOrder(Order order) {
            if(order == null) return null;

            // Get start params about excel file
            int sheetsAllCount = excel.GetSheetsCount();
            int sheetsVisCount = excel.GetSheetsCount(false);

            // Open sheet with order & parse them into orderBO
            int orderIndex = 0;

            for(var i = 1; i < sheetsAllCount + 1; i++) {
                excel.SetSheetByIndex(i);

                if(excel.CheckSheetVisibility()) {
                    orderIndex = i;
                    break;
                }
            }

            if(excel.IsSheetOpened()) {
                order.Initiator = excel.GetLabelValue("Полное наименование Заказчика");

                var brokerCode = excel.GetLabelValue("Код Брокера Заказчика");
                brokerCode = brokerCode.ToUpper().Replace("К", "K").Replace("А", "A");

                switch(brokerCode.ToLower().Substring(0, 4)) {
                    case "altk": order.Auction.Broker = new Broker(brokerCode, "ТОО «Альта и К», Андреев В.И., Директор Казахстан, 050064, г.Алматы, мкрн.Думан - 2, дом 18, кв. 55 тел.: +7(727) 390 - 43 - 02 e - mail: info @altaik.kz"); break;
                    case "alta": order.Auction.Broker = new Broker("ALTA", "ТОО «Альтаир Нур»"); break;
                }

                order.Auction.ExchangeProvisionSize = excel.GetLabelValue("Размер Гарантийного обеспечения");
                var regex = new Regex(@"[^\d,\.]*");
                order.Auction.ExchangeProvisionSize = regex.Replace(order.Auction.ExchangeProvisionSize, "").Replace(".", ",");

                lotName = Regex.Replace(excel.GetLabelValue("Наименование товара"), @"[Лл]от\s*№\d*:\s*", "");
            }

            // Open sheet(-s) with tech spec & parse them into orderBO
            int lotCount = sheetsVisCount - 2;
            int lotTechSpec = orderIndex + 1;

            for(var i = 1; i < lotCount + 1; i++) {
                for(var iLot = lotTechSpec; iLot < sheetsAllCount + 1; iLot++) {
                    excel.SetSheetByIndex(iLot);

                    if(excel.CheckSheetVisibility()) {
                        lotTechSpec = iLot + 1;
                        break;
                    }
                }

                if(excel.IsSheetOpened()) {
                    // Change title
                    var titleTechRowWord = excel.FindRow(DocumentTypeEnum.TechSpecs.GetName() + " к Ло");

                    if(titleTechRowWord != 0) excel.SetCells(titleTechRowWord, "A", "Техническая спецификация к Лоту №" + i);

                    // Delete not needed header rows
                    if(string.IsNullOrEmpty(excel.GetCell(1, 1))) {
                        for(var iRow = 1; iRow < 5; iRow++) {
                            excel.DeleteRow(1);
                        }
                    }

                    // Delete not needed footer rows and find sum
                    var rowsCount = excel.GetRowsCount();
                    var row = 0;

                    if(string.IsNullOrEmpty(excel.GetCell(excel.GetRowsCount(), "F"))) {
                        for(var iRow = rowsCount; iRow > 0; iRow--) {
                            if(!string.IsNullOrEmpty(excel.GetCell(iRow, "F"))) {
                                row = iRow;
                                lotStartPrice = excel.GetCell(iRow, "F");

                                try {
                                    if(!string.IsNullOrEmpty(excel.GetCell(iRow - 1, "K"))) lotMinRequerments = excel.GetCell(iRow - 1, "K");
                                    else lotMinRequerments = excel.GetCell(iRow - 2, "K");
                                } catch { lotMinRequerments = excel.GetCell(iRow - 2, "K"); }
                                break;
                            }
                        }

                        for(var iRow = row + 1; iRow < rowsCount + 1; iRow++) {
                            excel.DeleteRow(row + 1);
                        }
                    } else lotStartPrice = excel.GetCell(excel.GetRowsCount(), "F");

                    // Create lot item
                    var lot = new Lot {
                        Number = i.ToString(),
                        Name = lotName,
                        MinRequerments = lotMinRequerments,
                        StartPrice = lotStartPrice
                    };

                    if(order.Auction.Lots == null) order.Auction.Lots = new ObservableCollection<Lot>();
                    order.Auction.Lots.Add(lot);

                    // For Creating LotsExtended
                    var iRowNum = excel.FindRow("Наименование");
                    bool isBusy = true;
                    int iCount = 1;

                    if(order.Auction.Lots[i - 1].LotsExtended == null) order.Auction.Lots[i - 1].LotsExtended = new ObservableCollection<LotsExtended>();

                    while(isBusy) {
                        if(!string.IsNullOrEmpty(excel.GetCell(iRowNum + iCount, 1))) {
                            LotsExtended lotEx = new LotsExtended();

                            lotEx.serialnumber = iCount;
                            lotEx.name = excel.GetCell(iRowNum + iCount, 2);
                            lotEx.unit = excel.GetCell(iRowNum + iCount, 3);
                            lotEx.quantity = Convert.ToDecimal(excel.GetCell(iRowNum + iCount, 4).Replace(".", ","));
                            lotEx.price = Convert.ToDecimal(excel.GetCell(iRowNum + iCount, 5).Replace(".", ","));
                            lotEx.sum = Convert.ToDecimal(excel.GetCell(iRowNum + iCount, 6).Replace(".", ","));
                            lotEx.country = excel.GetCell(iRowNum + iCount, 7);
                            lotEx.techspec = excel.GetCell(iRowNum + iCount, 8);
                            lotEx.terms = excel.GetCell(iRowNum + iCount, 9);
                            lotEx.paymentterm = excel.GetCell(iRowNum + iCount, 10);

                            try {
                                lotEx.dks = Convert.ToInt32(Regex.Replace(excel.GetCell(iRowNum + iCount, 11).Replace("%", ""), "[^0-9]", ""));
                            } catch(Exception) { lotEx.dks = 0; }

                            lotEx.contractnumber = excel.GetCell(iRowNum + iCount, 12);
                            order.Auction.Lots[i - 1].LotsExtended.Add(lotEx);
                        } else isBusy = false;

                        iCount++;
                    }
                }
            }

            order.Warranty = WARRANTY_LETER;

            return order;
        }


        public override bool CopyQualificationsToBuffer() {
            excel.SetActiveSheet(DocumentTypeEnum.Qualifications.GetName() + ",");

            if(excel.IsSheetOpened()) {
                // Delete not needed header rows
                if(string.IsNullOrEmpty(excel.GetCell(1, "A"))) {
                    var titleRow = excel.FindRow("квалификационные тре");

                    if(titleRow != 0) excel.SetCells(titleRow, "A", "Квалификационные требования, которым должен соответствовать Претендент на участие в Аукционе");

                    for(var i = 1; i < titleRow; i++) {
                        excel.DeleteRow(1);
                    }
                }

                // Delete not needed footer rows
                var footerTop = 0;
                var footerStart = 0;

                for(var i = excel.GetRowsCount(); i > 0; i--) {
                    if(excel.GetCell(i, "A").ToLower().Contains("уполномоченное лицо")) footerTop = i;
                    else if(excel.GetCell(i, "A").ToLower().Contains("подпись лица")) { footerStart = i; break; }

                }

                if(footerTop != 0) {
                    for(var i = footerStart; i < footerTop + 1; i++) {
                        excel.DeleteRow(footerStart);
                    }
                }

                // Delete some words
                var competitionRowWord = excel.FindRow("конкурсной");

                if(competitionRowWord != 0) excel.SetCells(competitionRowWord, "A", excel.GetCell(competitionRowWord, "A").Replace("конкурсной ", ""));

                // Append with original
                var originalRowWord = excel.FindRow("в прошитом");

                if(originalRowWord != 0) excel.SetCells(originalRowWord, "A", excel.GetCell(originalRowWord, "A").Replace("в прошитом", "в оригинале и прошитом"));
            }

            excel.FindQualificationForVostok(DocumentTypeEnum.Qualifications.GetName(), DocumentTypeEnum.TechSpecs.GetName(), DocumentTypeEnum.Application.GetName());

            return true;
        }


        public override bool CopyAgreement() {
            // TODO Delete all after delivery place
            word.CopyAll();
            return true;
        }


        public override bool CopyTechSpecs() {
            excel.FindTechSpecForVostok(DocumentTypeEnum.TechSpecs.GetName(), DocumentTypeEnum.Application.GetName());
            return true;
        }


        public override bool CopyTechSpecs(int sheetIndex) {
            int sheetsAllCount = excel.GetSheetsCount();
            int sheetsVisCount = excel.GetSheetsCount(false);
            int iCount = 1, sCount = 1;
            bool inSearch = true;

            while(inSearch) {
                excel.SetSheetByIndex(iCount);

                if(excel.CheckSheetVisibility()) {
                    if(sCount == sheetIndex) {
                        excel.CopySheetToBuffer(sheetIndex);
                        return true;
                    } else sCount++;
                }

                iCount++;

                if(iCount > sheetsAllCount) return false;
            }

            return false;
        }


        public override void Close() {
            excel.CloseWorkbook(false);
            excel.CloseExcel();
            word.CloseDocument(true);
            word.CloseWord(true);
        }
        #endregion
    }
}