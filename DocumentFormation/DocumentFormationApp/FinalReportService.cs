using AltaBO.reports;
using AltaOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentFormation
{
    public static class FinalReportService
    {
        public static void FormateDocument(string fileName, List<DealNumberInfo> reportList)
        {
            ExcelService excel = new ExcelService(fileName);

            int iRow = 2;

            foreach (var item in reportList) {
                excel.SetCells(iRow, "A", item.dealNumber);
                excel.SetCells(iRow, "B", item.auctionDate);
                excel.SetCells(iRow, "C", item.auctionNumber);
                excel.SetCells(iRow, "D", item.customerName);
                excel.SetCells(iRow, "E", item.lotCode);
                excel.SetCells(iRow, "F", item.supplierName);
                excel.SetCells(iRow, "G", item.finalPriceOffer);
                excel.SetCells(iRow, "H", item.debt);
                excel.SetCells(iRow, "I", item.traderName);
                excel.SetCells(iRow, "J", item.brokerName);

                iRow++;
            }

            excel.CloseWorkbook(true);
            excel.CloseExcel();
        }
    }
}
