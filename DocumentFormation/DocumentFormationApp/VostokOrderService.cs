using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using altaik.baseapp.ext;
using AltaBO;
using AltaOffice;
using DocumentFormation.model;
using System;

namespace DocumentFormation {
    /// <summary>
    /// Class to operate with VostokCvetMet customer incoming file to get Order.
    /// It uses windows buffer to copy attachments to output files.
    /// </summary>
    public class VostokOrderService : CustomerOrderService {
        private const string WARRANTY_LETER = "Настоящим подтверждаю действительность письма Гарантийного письма ТОО \"Востокцветмет\"   № 07-2/6 от «5» января 2017 года и в случае его недействительности либо отказа Инициатора от его исполнения, принимаю на себя все указанные в Гарантийном письме обязательства.";

        private readonly ExcelService excel;
        private readonly WordService word;

        public VostokOrderService(string orderFileName, string treatyFileName) {
            excel = new ExcelService(orderFileName);
            word = new WordService(treatyFileName, true);
        }


        private string lotNumber, lotName, lotMinRequerments;
        private string lotStartPrice;

        public override Order UpdateOrder(Order order) {
            if(order == null) return null;

            excel.SetActiveSheet(DocumentTypeEnum.Application.GetName());

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

                lotNumber = "1";
                lotName = Regex.Replace(excel.GetLabelValue("Наименование товара"), @"[Лл]от\s*№\d*:\s*", "");
                lotMinRequerments = "0";
            }

            excel.SetActiveSheet(DocumentTypeEnum.TechSpecs.GetName() + " к Ло");

            if(excel.IsSheetOpened()) {
                // Change title
                var titleTechRowWord = excel.FindRow(DocumentTypeEnum.TechSpecs.GetName() + " к Ло");

                if(titleTechRowWord != 0) excel.SetCells(titleTechRowWord, "A", "Техническая спецификация к Лоту № 1");

                // Delete not needed header rows
                if(string.IsNullOrEmpty(excel.GetCell(1, 1))) {
                    for(var i = 1; i < 5; i++) {
                        excel.DeleteRow(1);
                    }
                }

                // Delete not needed footer rows and find sum
                var rowsCount = excel.GetRowsCount();
                var row = 0;

                if(string.IsNullOrEmpty(excel.GetCell(excel.GetRowsCount(), "F"))) {
                    for(var i = rowsCount; i > 0; i--) {
                        if(!string.IsNullOrEmpty(excel.GetCell(i, "F"))) {
                            row = i;
                            lotStartPrice = excel.GetCell(i, "F");

                            try {
                                if(!string.IsNullOrEmpty(excel.GetCell(i - 1, "K"))) lotMinRequerments = excel.GetCell(i - 1, "K");
                                else lotMinRequerments = excel.GetCell(i - 2, "K");
                            } catch { lotMinRequerments = excel.GetCell(i - 2, "K"); }
                            break;
                        }
                    }

                    for(var i = row + 1; i < rowsCount + 1; i++) {
                        excel.DeleteRow(row + 1);
                    }
                } else lotStartPrice = excel.GetCell(excel.GetRowsCount(), "F");
            }

            var lot = new Lot {
                Number = lotNumber,
                Name = lotName,
                MinRequerments = lotMinRequerments,
                StartPrice = lotStartPrice
            };

            if(order.Auction.Lots == null) order.Auction.Lots = new ObservableCollection<Lot>();
            order.Auction.Lots.Add(lot);

            // For Creating LotsExtended
            var iRow = excel.FindRow("Наименование");
            bool isBusy = true;
            int iCount = 1;

            if(order.Auction.Lots[0].LotsExtended == null) order.Auction.Lots[0].LotsExtended = new ObservableCollection<LotsExtended>();

            while(isBusy) {
                if(!string.IsNullOrEmpty(excel.GetCell(iRow + iCount, 1))) {
                    LotsExtended lotEx = new LotsExtended();

                    lotEx.serialnumber = iCount;
                    lotEx.name = excel.GetCell(iRow + iCount, 2);
                    lotEx.unit = excel.GetCell(iRow + iCount, 3);
                    lotEx.quantity = Convert.ToDecimal(excel.GetCell(iRow + iCount, 4).Replace(".", ","));
                    lotEx.price = Convert.ToDecimal(excel.GetCell(iRow + iCount, 5).Replace(".", ","));
                    lotEx.sum = Convert.ToDecimal(excel.GetCell(iRow + iCount, 6).Replace(".", ","));
                    lotEx.country = excel.GetCell(iRow + iCount, 7);
                    lotEx.techspec = excel.GetCell(iRow + iCount, 8);
                    lotEx.terms = excel.GetCell(iRow + iCount, 9);
                    lotEx.paymentterm = excel.GetCell(iRow + iCount, 10);

                    try {
                        lotEx.dks = Convert.ToInt32(Regex.Replace(excel.GetCell(iRow + iCount, 11).Replace("%", ""), "[^0-9]", ""));
                    } catch(Exception) { lotEx.dks = 0; }

                    lotEx.contractnumber = excel.GetCell(iRow + iCount, 12);
                    order.Auction.Lots[0].LotsExtended.Add(lotEx);
                } else isBusy = false;

                iCount++;
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


        public override void Close() {
            excel.CloseWorkbook(false);
            excel.CloseExcel();
            word.CloseDocument(true);
            word.CloseWord(true);
        }

        public override bool CopyTechSpecs(int sheetIndex) {
            throw new NotImplementedException();
        }
    }
}