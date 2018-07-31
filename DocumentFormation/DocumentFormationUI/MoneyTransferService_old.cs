using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using AltaBO;
using AltaOffice;
using AltaTransport;

namespace DocumentFormation
{
    public class MoneyTransferService_old
    {
        private ObservableCollection<MoneyTransferList> moneyTransferList;
        private string FILE_NAME = "moneyTransfer.docx";
        private string fPath;
        private WordService word;

        public MoneyTransferService_old() { }


        public void StartFill(ObservableCollection<MoneyTransferList> moneyTransferList)
        {
            this.moneyTransferList = moneyTransferList;

            // Show end path
            var wherePath = Service.GetDirectory();
            if (wherePath != null) {

                fPath = wherePath.FullName;

                // Copy templates to end path
                CopyTemplate();

                // Open template and fill
                OpenTemplate();

                // Close template
                CloseTemplate();

                Process.Start("explorer", wherePath.FullName);
            }
        }


        private void CopyTemplate()
        {
            System.IO.File.Copy(FileArchiveTransport.GetMoneyTransferTemplateFileName(), fPath + "\\" + FILE_NAME, true);
        }


        private void OpenTemplate()
        {
            word = new WordService(fPath + "\\" + FILE_NAME, false);

            word.FindReplace("[brokerName]", moneyTransferList[0].brokerName);

            var iCount = 1;

            foreach (var item in moneyTransferList) {
                if (iCount != 1) {
                    word.AddTableRow(1);
                }

                word.SetCell(1, iCount + 1, 1, item.fromCompany);
                word.SetCell(1, iCount + 1, 2, item.toCompany);
                word.SetCell(1, iCount + 1, 3, item.lotNumber);
                word.SetCell(1, iCount + 1, 4, item.sum);

                iCount++;
            }
        }


        private void CloseTemplate()
        {
            word.CloseDocument(true);
            word.CloseWord(true);

            RenameFile();
        }


        private void RenameFile()
        {
            System.IO.File.Move(fPath + "\\" + FILE_NAME, fPath + "\\Заявление об изменении учета денег (до торгов) на " + DateTime.Now.ToString().Replace(":", "_") + ".docx");
        }


        public void ChangeMoneyDirection()
        {
            // Choose file that need correct
            fPath = Service.GetFile("Выберите файл заявления об изменении учета денег (до торгов)", "Заявление об изменении учета денег (*.docx)|*.docx").FullName;

            // Choose path to save
            var wherePath = Service.GetDirectory();
            if (wherePath != null) {

                // Copy this file
                var tmpName = wherePath.FullName + "\\Заявление об изменении учета денег (после торгов) " + DateTime.Now.ToString().Replace(":", "_") + ".docx";

                System.IO.File.Copy(fPath, tmpName, true);

                // Change data in copied file
                word = new WordService(tmpName, false);

                var rCount = word.GetTableRowsCount(1);
                string lSide, rSide;

                for(var iRow = 2; iRow <= rCount; iRow++) {
                    lSide = word.GetCell(1, iRow, 1);
                    rSide = word.GetCell(1, iRow, 2);
                    word.SetCell(1, iRow, 1, rSide);
                    word.SetCell(1, iRow, 2, lSide);
                }

                // Close
                word.CloseDocument(true);
                word.CloseWord(true);

                // Open prepared dir
                Process.Start("explorer", wherePath.FullName);
            }
        }
    }
}
