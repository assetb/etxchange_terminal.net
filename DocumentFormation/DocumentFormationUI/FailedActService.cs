using System;
using System.Diagnostics;
using System.IO;
using AltaBO;
using AltaOffice;
using AltaTransport;

namespace DocumentFormation
{
    public class FailedActService
    {
        private string fPath = "", fName = "";
        private ExcelService excel;
        private WordService word;
        private FailedAct failedAct = new FailedAct();
        

        public FailedActService() { }


        public void StartFormation()
        {
            // Show path to order file
            ShowPath();

            // Parse order file to get needed info
            ParseOrder();

            // Copy template file to needed directory
            CopyTemplate();

            // Put info to template
            FillAct();

            // Open needed folder
            Process.Start("explorer", fPath);
        }


        private void ShowPath()
        {
            fPath = Service.GetFile("Выберите файл заявки", "(*.xlsx)|*.xlsx").FullName;
        }


        private void ParseOrder()
        {
            excel = new ExcelService(fPath);

            failedAct.sourceNumber = "Введите пожалуйста исходящий номер";
            failedAct.fromDate = DateTime.Now.ToShortDateString();
            failedAct.auctionDate = excel.GetCell("9", "A");
            failedAct.orderNumber = excel.GetCell("7", "A");

            failedAct.orderNumber = failedAct.orderNumber.Substring(failedAct.orderNumber.IndexOf("№"), failedAct.orderNumber.Length - failedAct.orderNumber.IndexOf("№"));
            failedAct.orderNumber = failedAct.orderNumber.Substring(1, failedAct.orderNumber.LastIndexOf(" "));

            fName = failedAct.orderNumber.Substring(0, failedAct.orderNumber.IndexOf("от") - 1);

            if (fName.Length > 4) {
                fName = fName.Substring(fName.Length - 4, 4);
            }

            excel.CloseWorkbook(false);
            excel.CloseExcel();
        }


        private void CopyTemplate()
        {
            var wherePath = Service.GetDirectory();
            if (wherePath != null) {

                fPath = wherePath.FullName;
                fName = fPath + "\\Акт о несостоявшемся торге под №" + fName + ".docx";

                try {
                    System.IO.File.Copy(FileArchiveTransport.GetTemplatesPath() + "\\failedAuction.docx", fName);
                } catch (Exception ex) {
                    Debug.WriteLine(ex.Message);
                }
            }
        }


        private void FillAct()
        {
            word = new WordService(fName, false);

            word.FindReplace("[sourceNumber]", failedAct.sourceNumber);
            word.FindReplace("[fromDate]", failedAct.fromDate);
            word.FindReplace("[auctionDate]", failedAct.auctionDate);
            word.FindReplace("[orderNumber]", failedAct.orderNumber);

            word.CloseDocument(true);
            word.CloseWord(true);
        }
    }
}
