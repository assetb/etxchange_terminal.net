using AltaBO.archive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaOffice;

namespace DocumentFormation
{
    public class ArchiveVolumeRegistryService
    {
        public static void FormateDocument(string fileName, List<Document> documentsList)
        {
            FillDocument(fileName, documentsList);
        }


        private static void FillDocument(string fileName, List<Document> documentsList)
        {
            ExcelService excel = new ExcelService(fileName, true);

            // Make title
            string docType = documentsList[0].name.ToLower();
            string titleEnd = docType.Contains("догов") ? "договоров" : docType.Contains("счет") ? "счет-фактур" : "документов";

            excel.MergeCells(2, 1, 2, 5);
            excel.SetCells(2, 1, "РЕЕСТР " + titleEnd.ToUpper());
            excel.SetCellFontName(2, 1, "Times New Roman");
            excel.SetCellBoldStyle(2, 1, true);
            excel.SetCellFontHAlignment(2, 1, 2);

            // Make Headers
            excel.SetCells(4, 1, "Архивный номер");
            excel.SetCellWrapText(4, 1, true);

            excel.SetCells(4, 2, "Компания (контрагент)");
            excel.SetCells(4, 3, "Номер документа");
            excel.SetCells(4, 4, "Дата создания");
            excel.SetCells(4, 5, "Тип документа");

            for (var i = 1; i < 6; i++)
            {
                excel.SetCellBoldStyle(4, i, true);
                excel.SetCellBorder(4, i);
                excel.SetCellFontHAlignment(4, i, 2);
                excel.SetCellFontVAlignment(4, i, 2);
                excel.SetCellBackgroundColor(4, i, System.Drawing.Color.FromArgb(248, 203, 173));
            }

            // Fill content
            int iCount = 5;

            foreach (var item in documentsList)
            {
                excel.SetCells(iCount, 1, item.serialNumber);
                excel.SetCellFontHAlignment(iCount, 1, 2);

                excel.SetCells(iCount, 2, item.company);
                excel.SetCellWrapText(iCount, 2, true);

                excel.SetCells(iCount, 3, item.number);
                excel.SetCellFontHAlignment(iCount, 3, 2);

                excel.SetCells(iCount, 4, item.createdDate.ToShortDateString());
                excel.SetCells(iCount, 5, item.name);
                excel.SetCellWrapText(iCount, 5, true);

                for (var i = 1; i < 6; i++)
                {
                    excel.SetCellBorder(iCount, i);
                    excel.SetCellFontVAlignment(iCount, i, 2);
                }

                excel.SetRowAutoFit(iCount, 1);

                iCount++;
            }

            // Last configurations
            excel.SetCellFontName(0, 0, "Times New Roman", true);
            excel.SetCellFontSize(0, 0, 9, true);
            excel.SetCellFontSize(2, 1, 12);
            excel.SetColumnWidth(1, 1, 8);
            excel.SetColumnWidth(1, 2, 34);
            excel.SetColumnWidth(1, 3, 16);
            excel.SetColumnWidth(1, 4, 13);
            excel.SetColumnWidth(1, 5, 21);

            // Close excel
            excel.CloseWorkbook(true);
            excel.CloseExcel();
        }
    }
}
