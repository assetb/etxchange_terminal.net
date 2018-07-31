using System;
using System.IO;
using AltaBO;
using AltaOffice;
using AltaTransport;

namespace DocumentFormation
{
    public class InstructionAttachToDealService
    {
        private string path;
        private string FILE_NAME_COMMISSION = "commissionToDeal.docx";
        private StockDealInfo stockDealInfo = new StockDealInfo();
        private ExcelService excel;
        private WordService word;


        public InstructionAttachToDealService(string path, StockDealInfo stockDealInfo)
        {
            this.path = path;
            this.stockDealInfo = stockDealInfo;
        }


        public void CopyTemplates()
        {
            System.IO.File.Copy(FileArchiveTransport.GetCommissionToDealTemplateFileName(), path + "\\" + FILE_NAME_COMMISSION, true);
        }


        public void GetInfoForAttachDocument()
        {
            // Show path to client order attach and open them
            word = new WordService(GetPathWithOrderAttach(), false);

            // Find table with technic specification
            var tCount = word.GetTablesCount();

            for (var iCount = 1; iCount <= tCount; iCount++) {
                if (word.GetCell(iCount, 1, 1).Contains("Техническая") || word.GetCell(iCount, 2, 1).Contains("Техническая")) {
                    // Delete first two rows
                    word.DeleteTableRows(iCount, new int[2] { 1, 1 });

                    // Copy left table
                    word.CopyTable(iCount);
                    break;
                }
            }

            // Close order attach
            word.CloseDocument(false);
            word.CloseWord(false);
        }


        private string auctionNumber;
        private string auctionDate;

        public void GetInfoForCommissionDocument()
        {
            // Show path to client order and open them
            excel = new ExcelService(GetPathWithOrder());

            // Get order number and date
            if (excel.GetCell(7, "A").Contains("ВЦМ"))
                auctionNumber = excel.GetCell(7, "A").Substring(excel.GetCell(7, "A").IndexOf("ВЦМ") - 1);
            else auctionNumber = "";

            auctionDate = excel.GetCell(9, "A");

            // Close order
            excel.CloseWorkbook(false);
            excel.CloseExcel();

            // Paste to commission doc
            PasteInfoToCommissionDoc();
        }


        private void PasteInfoToCommissionDoc()
        {
            // Open commission template to fill
            word = new WordService(path + "\\" + FILE_NAME_COMMISSION, false);

            // Fill commisiion
            word.SetCell(1, 2, 2, stockDealInfo.ComDateOut.ToShortDateString());
            word.SetCell(1, 4, 2, auctionDate);
            word.SetCell(1, 5, 2, "Заявка № " + auctionNumber);
            word.SetCell(1, 6, 2, stockDealInfo.MemberName);
            word.SetCell(1, 7, 2, stockDealInfo.MemberCode);
            word.SetCell(1, 8, 2, stockDealInfo.SupplierName);
            word.SetCell(1, 9, 2, stockDealInfo.SupplierCode);
            word.SetCell(1, 12, 1, stockDealInfo.LotCode);
            word.SetCell(1, 25, 1, stockDealInfo.ComDateIn.ToShortDateString());
            word.SetCell(1, 25, 2, stockDealInfo.ComDateIn.ToLongTimeString());
            word.SetCell(1, 25, 3, stockDealInfo.Employe);

            // Paste table
            word.PasteBookmark("table");

            // Fit tables in document
            word.AutoFitTables();

            // Fill lefted info
            word.FindReplace("[startPrice]", word.GetCell(2, word.FindTextInRow(2, "ИТОГО", 2), 6));

            // Close and save commission
            word.CloseDocument(true);
            word.CloseWord(true);

            System.IO.File.Move(path + "\\" + FILE_NAME_COMMISSION, path + "\\" + "Поручение на сделку по лоту - " + stockDealInfo.LotCode + " - у клиента - " + stockDealInfo.SupplierName + " " + DateTime.Now.ToString().Replace(":","_") + ".docx");
        }


        private string GetPathWithOrderAttach()
        {
            return Service.GetFile("Выберите приложение к заявке для приложения к сделке", "Приложение к заявке (*.docx, *.doc)|*.docx;*.doc").FullName;
        }


        private string GetPathWithOrder()
        {
            return Service.GetFile("Выберите заявку для поручения на сделку", "Заявка (*.xlsx,*.xls)|*.xlsx;*.xls").FullName;
        }
    }
}
