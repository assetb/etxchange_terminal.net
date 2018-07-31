using AltaBO;
using AltaOffice;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentFormation
{
    public class TechSpecService
    {
        #region Variables
        #endregion

        #region Methods
        public static bool CreateDocument(string fileNameInfo, List<LotsExtended> lotsExtendedInfo, string lotCode = "", decimal minimalPrice = 0, decimal startPrice = 0, bool withPDF = false)
        {
            try {
                return FillDocument(fileNameInfo, lotsExtendedInfo, lotCode, minimalPrice, startPrice, withPDF);
            } catch (Exception) { return false; }
        }


        public static bool ParseDocument(string fileNameInfo, List<LotsExtended> lotExtendedInfo, out List<LotsExtended> lotExtendedResult)
        {
            // Open document
            IExcelService excel = new ExcelService(fileNameInfo);

            // Parse document
            int rCount = excel.GetRowsCount();

            for (var iCount = 2; iCount < rCount + 1; iCount++) {
                if (lotExtendedInfo[iCount - 2].serialnumber == Convert.ToInt32(excel.GetCell(iCount, "A")) && lotExtendedInfo[iCount - 2].name == excel.GetCell(iCount, "B")) {
                    /*lotExtendedInfo[iCount - 2].serialnumber = Convert.ToInt32(excel.GetCell(iCount, "A"));
                    lotExtendedInfo[iCount - 2].name = excel.GetCell(iCount, "B");
                    lotExtendedInfo[iCount - 2].unit = excel.GetCell(iCount, "C");
                    lotExtendedInfo[iCount - 2].quantity = Convert.ToDecimal(excel.GetCell(iCount, "D"));
                    lotExtendedInfo[iCount - 2].price = Convert.ToDecimal(excel.GetCell(iCount, "E"));
                    lotExtendedInfo[iCount - 2].sum = Convert.ToDecimal(excel.GetCell(iCount, "F"));
                    lotExtendedInfo[iCount - 2].country = excel.GetCell(iCount, "G");
                    lotExtendedInfo[iCount - 2].techspec = excel.GetCell(iCount, "H");
                    lotExtendedInfo[iCount - 2].terms = excel.GetCell(iCount, "I");
                    lotExtendedInfo[iCount - 2].paymentterm = excel.GetCell(iCount, "J");
                    lotExtendedInfo[iCount - 2].dks = Convert.ToInt32(excel.GetCell(iCount, "K"));
                    lotExtendedInfo[iCount - 2].contractnumber = excel.GetCell(iCount, "L");*/
                    lotExtendedInfo[iCount - 2].endprice = Convert.ToDecimal(excel.GetCell(iCount, "M"));
                    lotExtendedInfo[iCount - 2].endsum = Convert.ToDecimal(excel.GetCell(iCount, "N"));
                } else {
                    lotExtendedResult = new List<LotsExtended>();
                    return false;
                }
            }

            // Close document
            excel.CloseWorkbook(true);
            excel.CloseExcel();

            // Return
            lotExtendedResult = new List<LotsExtended>(lotExtendedInfo);
            return true;
        }


        private static bool FillDocument(string fileName, List<LotsExtended> lotsExtended, string lotCode = "", decimal minimalPrice = 0, decimal startPrice = 0, bool withPDF = false)
        {
            IExcelService excel = new ExcelService(fileName);

            if (minimalPrice > 0) {
                decimal difference = 100 - ((startPrice - minimalPrice) / (startPrice / 100));
                foreach (var subItem in lotsExtended) {
                    subItem.endprice = subItem.price / 100 * difference;
                    subItem.endsum = subItem.endprice * subItem.quantity;
                }
            }

            int iRow = 4;

            excel.SetCells(1, "A", "Техническая спецификация по лоту №" + lotCode);

            try {
                foreach (var item in lotsExtended) {
                    excel.SetCells(iRow, "A", item.serialnumber);
                    excel.SetCells(iRow, "B", item.name);
                    excel.SetCells(iRow, "C", item.unit);
                    excel.SetCells(iRow, "D", item.quantity);
                    excel.SetCells(iRow, "E", item.price);
                    excel.SetCells(iRow, "F", item.sum);
                    excel.SetCells(iRow, "G", item.country);
                    excel.SetCells(iRow, "H", item.techspec);
                    excel.SetCells(iRow, "I", item.terms);
                    excel.SetCells(iRow, "J", item.paymentterm);
                    excel.SetCells(iRow, "K", item.dks);
                    excel.SetCells(iRow, "L", item.contractnumber);
                    excel.SetCells(iRow, "M", item.endprice);
                    excel.SetCells(iRow, "N", item.endsum);

                    for (var i = 1; i < 15; i++) excel.SetCellBorder(iRow, i);

                    iRow++;
                }
            } catch (Exception) { }

            excel.SetCells(iRow, "F", lotsExtended.Sum(l => l.sum));
            excel.SetCells(iRow, "N", lotsExtended.Sum(l => l.endsum));

            excel.SetPagesFit();

            if (withPDF) excel.SaveAsPDF(fileName.Replace(".xlsx", ".pdf"), false);

            excel.CloseWorkbook(true);
            excel.CloseExcel();

            return true;
        }


        public static List<LotsExtended> ParseOrderWithTS(string fileName, int lotId)
        {
            WordService word = new WordService(fileName, false);

            if (word.GetTablesCount() == 0) return null;

            List<LotsExtended> lotsExtended = new List<LotsExtended>();
            int tablesCount = word.GetTablesCount();

            for (var tCount = 1; tCount <= tablesCount; tCount++) {
                int rowCount = word.GetTableRowsCount(tCount);
                string valueLeft = "", valueRight = "";

                for (var rCount = tCount == 1 ? 2 : 1; rCount < rowCount + 1; rCount++) {
                    bool getValue = true;

                    try {
                        valueLeft = word.GetCell(tCount, rCount, 1);
                        valueRight = word.GetCell(tCount, rCount, 12);
                    } catch { getValue = false; }

                    if (getValue && (string.IsNullOrEmpty(valueLeft.Replace("\r", "").Replace("\a", "")) || string.IsNullOrEmpty(valueRight.Replace("\r", "").Replace("\a", "")))) getValue = false;

                    if (getValue) {
                        lotsExtended.Add(new LotsExtended() {
                            serialnumber = Convert.ToInt32(word.GetCell(tCount, rCount, 1).Replace("\r", "").Replace("\a", "")),
                            name = word.GetCell(tCount, rCount, 2).Replace("\r", "").Replace("\a", ""),
                            unit = word.GetCell(tCount, rCount, 3).Replace("\r", "").Replace("\a", ""),
                            quantity = Convert.ToDecimal(word.GetCell(tCount, rCount, 4).Replace("\r", "").Replace("\a", "").Replace(" ", "")),
                            price = Convert.ToDecimal(word.GetCell(tCount, rCount, 5).Replace("\r", "").Replace("\a", "").Replace(" ", "")),
                            sum = Convert.ToDecimal(word.GetCell(tCount, rCount, 6).Replace("\r", "").Replace("\a", "").Replace(" ", "")),
                            country = word.GetCell(tCount, rCount, 7).Replace("\r", "").Replace("\a", ""),
                            techspec = word.GetCell(tCount, rCount, 8).Replace("\r", "").Replace("\a", ""),
                            terms = word.GetCell(tCount, rCount, 9).Replace("\r", "").Replace("\a", ""),
                            paymentterm = word.GetCell(tCount, rCount, 10).Replace("\r", "").Replace("\a", ""),
                            dks = Convert.ToInt32(Math.Round(Convert.ToDecimal(word.GetCell(tCount, rCount, 11).Replace("\r", "").Replace("\a", "").Replace("%", "")), 0)),
                            contractnumber = word.GetCell(tCount, rCount, 12).Replace("\r", "").Replace("\a", ""),
                            lotId = lotId
                        });
                    }
                }
            }

            word.CloseDocument(false);
            word.CloseWord(false);

            return lotsExtended;
        }
        #endregion
    }
}
