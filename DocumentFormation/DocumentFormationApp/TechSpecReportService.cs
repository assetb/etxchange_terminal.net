using AltaBO;
using AltaOffice;
using System.Collections.Generic;

namespace DocumentFormation
{
    public class TechSpecReportService
    {
        #region Methods
        public static void CreateDocument(List<TechSpecReportBO> techSpecReportItem, string fileNamePath)
        {
            FillDocument(fileNamePath, techSpecReportItem);
        }

        public static void CreateDocument(List<FinalReportPlmtl> finalReportPlmtlInfo, string fileNamePath)
        {
            FillDocument(fileNamePath, finalReportPlmtlInfo);
        }

        private static void FillDocument(string fileName, List<FinalReportPlmtl> finalReportPlmtl)
        {
            IExcelService excel = new ExcelService(fileName);

            int iRow = 2, startRow = 0;

            string curLotCode = "", curAuctionNum = "";
            bool IsNew = true;
            decimal oldFinalPrice = 0;

            foreach (var item in finalReportPlmtl)
            {
                if (curLotCode == item.lotCode && curAuctionNum == item.auctionNumber) IsNew = false;
                else
                {
                    excel.SetCells(iRow, "A", item.dealNumber);
                    excel.SetCells(iRow, "B", item.auctionDate);
                    excel.SetCells(iRow, "J", item.name);
                    excel.SetCells(iRow, "K", item.bin);
                    excel.SetCells(iRow, "M", item.address);
                    excel.SetCells(iRow, "N", item.email);
                    excel.SetCells(iRow, "P", item.telephone);

                    if (iRow != 2)
                    {
                        CountLastStroke(excel, startRow, iRow, oldFinalPrice);
                        excel.InsertRow(iRow + 1);
                        iRow++;
                    }

                    startRow = iRow;
                    curLotCode = item.lotCode;
                    curAuctionNum = item.auctionNumber;
                    IsNew = true;
                }

                try
                {
                    excel.SetCells(iRow, "C", item.productName);
                    excel.SetCells(iRow, "E", item.unit);
                    excel.SetCells(iRow, "F", item.quantity);
                    excel.SetCells(iRow, "G", item.productSum);
                }
                catch { }

                excel.InsertRow(iRow + 1);
                iRow++;
            }


            excel.CloseWorkbook(true);
            excel.CloseExcel();
            excel = null;
        }

        private static void FillDocument(string fileName, List<TechSpecReportBO> techSpecReport)
        {
            IExcelService excel = new ExcelService(fileName);

            int iRow = 2, iCount = 1, startRow = 0;

            string curLotCode = "", curAuctionNum = "";
            bool IsNew = true;
            decimal oldStartPrice = 0, oldFinalPrice = 0;

            foreach (var item in techSpecReport)
            {
                try
                {
                    if (curLotCode == item.lotCode && curAuctionNum == item.auctionNumber) IsNew = false;
                    else
                    {
                        if (iRow != 2)
                        {
                            CountLastStroke(excel, startRow, iRow, oldFinalPrice);
                            excel.InsertRow(iRow + 1);
                            iRow++;
                        }

                        startRow = iRow;
                        curLotCode = item.lotCode;
                        curAuctionNum = item.auctionNumber;
                        IsNew = true;
                    }

                    if (iCount == techSpecReport.Count) CountLastStroke(excel, startRow, iRow, oldFinalPrice); // Last line               

                    if (IsNew)
                    {
                        excel.SetCells(iRow, "A", item.auctionNumber + " от " + item.orderDate.ToShortDateString());
                        excel.SetCells(iRow, "B", item.lotCode);
                        excel.SetCells(iRow, "I", item.dealNumber == null ? "" : item.dealNumber);
                        excel.SetCells(iRow, "J", item.name == null ? "" : item.name);
                        excel.SetCells(iRow, "K", item.auctionDate.ToShortDateString());
                        // TODO: Почему здесь изменяется дата?
                        excel.SetCells(iRow, "L", item.auctionDate.AddDays(1).ToShortDateString());
                    }

                    excel.SetCells(iRow, "C", item.productName);
                    excel.SetCells(iRow, "D", item.unit);
                    excel.SetCells(iRow, "E", item.quantity);
                    excel.SetCells(iRow, "F", item.price);
                    excel.SetCells(iRow, "G", string.Format("=F{0}*E{1}", iRow, iRow));
                    excel.SetCells(iRow, "H", "-");

                    excel.SetCells(iRow, "M", item.productFinalPrice == null ? 0 : item.productFinalPrice);
                    excel.SetCells(iRow, "N", string.Format("=M{0}*E{1}", iRow, iRow));
                    excel.SetCells(iRow, "O", "-");

                    excel.SetCells(iRow, "P", string.Format("=G{0}-N{1}", iRow, iRow));
                    excel.SetCells(iRow, "Q", string.Format("=P{0}/G{1}", iRow, iRow));
                    excel.SetCells(iRow, "R", item.contractNumber);
                }
                catch { }

                excel.InsertRow(iRow + 1);
                iRow++;

                try
                {
                    oldStartPrice = (decimal)item.startPriceOffer;
                    oldFinalPrice = (decimal)item.finalPriceOffer;
                }
                catch
                {
                    oldStartPrice = 0;
                    oldFinalPrice = 0;
                }

                iCount++;
            }

            excel.CloseWorkbook(true);
            excel.CloseExcel();
            excel = null;

        }


        private static void CountLastStroke(IExcelService excel, int startRow, int iRow, decimal oldFinalPrice)
        {
            excel.SetCells(iRow, "G", string.Format("=SUM(G{0}:G{1})", startRow, (iRow - 1)));
            excel.SetCells(iRow, "H", string.Format("=G{0}-F{1}*E{2}", iRow, iRow, iRow));
            excel.SetCells(iRow, "N", string.Format("=SUM(N{0}:N{1})", startRow, iRow - 1));
            excel.SetCells(iRow, "O", string.Format("=N{0}-F{1}*E{2}", iRow, iRow, iRow));
            excel.SetCells(iRow, "P", string.Format("=G{0}-N{1}", iRow, iRow));
            excel.SetCells(iRow, "Q", string.Format("=P{0}/G{1}", iRow, iRow));

            if (excel.GetCell(iRow, "N") == "0" || excel.GetCell(iRow, "N") == "-") excel.SetCells(iRow, "N", oldFinalPrice);
        }
        #endregion
    }
}
