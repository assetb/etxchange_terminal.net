using System.Diagnostics;
using System.IO;
using AltaBO;
using AltaOffice;
using AltaTransport;

namespace DocumentFormation
{
    public class WaitingListService
    {
        private WaitingList waitingList;
        private string FILE_NAME = "waitingList.docx";
        private string fPath;
        private WordService word;


        // Check for type: for client or for stock
        public WaitingListService(WaitingList waitingList)
        {
            this.waitingList = waitingList;
        }


        public void StartFillList()
        {
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
            System.IO.File.Copy(FileArchiveTransport.GetWaitingListTemplateFileName(), fPath + "\\" + FILE_NAME, true);
        }


        private void OpenTemplate()
        {
            word = new WordService(fPath + "\\" + FILE_NAME, false);

            word.FindReplace("[orderNumber]", waitingList.orderNumber);
            word.FindReplace("[orderDate]", waitingList.orderDate);
            word.FindReplace("[lotCode]", waitingList.lotCode + " (" + waitingList.sourceNumber + ")");
            word.FindReplace("[lotCode]", waitingList.lotCode);

            // Fill table
            var iCount = 1;
            string[] sCompany;

            foreach (var item in waitingList.waitingListTable) {
                if (iCount != 1) {
                    word.AddTableRow(1);
                    word.AddTableRow(2);
                }

                word.SetCell(1, iCount + 1, 1, iCount.ToString()); // Number
                word.SetCell(1, iCount + 1, 2, item.company); // Companies
                word.SetCell(1, iCount + 1, 3, item.bankReq); // Requisites
                word.SetCell(1, iCount + 1, 4, item.location); // Location

                sCompany = item.company.Split('\n');
                word.SetCell(2, iCount, 1, iCount + ") " + sCompany[1]);
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
            System.IO.File.Move(fPath + "\\" + FILE_NAME, fPath + "\\Список претендентов по лоту " + waitingList.lotCode + ".docx");
        }
    }
}
