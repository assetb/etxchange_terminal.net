using System.IO;
using AltaBO;
using AltaOffice;
using AltaTransport;

namespace DocumentFormation
{
    public class FormC01Service
    {
        private string path;


        public FormC01Service(string path)
        {
            this.path = path;
        }


        public FormC01 InsertCompany(FormC01 form)
        {
            var excel = new ExcelService(path);
            excel.SetActiveSheetByName(form.code.ToUpper());

            var rowsCount = excel.GetRowsCount();
            string number = null;

            for (var iRow = rowsCount; iRow > 0; iRow--) {
                if (excel.GetCell(iRow, "B") != "") number = excel.GetCell(iRow, "B");
            }

            if (string.IsNullOrEmpty(number)) return null;

            var iNumber = 0;
            iNumber = int.Parse(number);
            number = (iNumber + 1).ToString();

            if (number.Length == 1) number = number.Insert(0, "00");
            else if (number.Length == 2) number = number.Insert(0, "0");
            else if (number.Length > 3) number = "1";

            form.number = number;
            form.codeG = form.broker.code.ToLower() + "g" + form.number;
            form.codeP = form.broker.code.ToLower() + "p" + form.number;
            form.codeS = form.broker.code.ToLower() + "s" + form.number;

            for (var iRow = rowsCount; iRow > 0; iRow--) {
                if (!string.IsNullOrEmpty(excel.GetCell(iRow, "B"))) {
                    excel.SetCells(iRow + 1, "A", form.date);
                    excel.SetCells(iRow + 1, "B", form.number);
                    excel.SetCells(iRow + 1, "C", form.name);
                    excel.SetCells(iRow + 1, "D", form.bin);
                    excel.SetCells(iRow + 1, "E", form.code);
                    excel.SetCells(iRow + 1, "F", form.codeG + " " + form.codeS);
                    excel.SetCells(iRow + 1, "G", form.codeP);
                    break;
                }
            }

            excel.CloseWorkbook(true);
            excel.CloseExcel();

            return form;
        }


        public void CreateTemplate(FormC01 form, string saveTo)
        {
            var FILE_NAME = "formC01.docx";

            System.IO.File.Copy(FileArchiveTransport.GetFormC01TemplateFileName(), saveTo + "\\" + FILE_NAME, true);

            var word = new WordService(saveTo + "\\" + FILE_NAME, false);

            word.SetCell(1, 1, 2, form.broker.name);
            word.SetCell(1, 2, 2, form.broker.code);
            word.SetCell(2, 2, 1, form.code);
            word.SetCell(2, 2, 2, form.bin);
            word.SetCell(2, 2, 3, form.name);
            word.SetCell(2, 2, 5, form.codeG + " " + form.codeS);
            word.SetCell(2, 2, 6, form.codeP);

            word.CloseDocument(true);
            word.CloseWord(true);
        }

    }
}
