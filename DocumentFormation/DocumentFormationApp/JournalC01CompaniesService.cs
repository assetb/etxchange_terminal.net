using System.Collections.Generic;
using AltaBO;
using AltaOffice;
using AltaTransport;

namespace DocumentFormation
{
    public class JournalC01CompaniesService
    {
        public static List<JournalC01Company> GetCompanies(string brokerCode)
        {
            var journalC01Companies = new List<JournalC01Company>();
            var excel = new ExcelService(FileArchiveTransport.GetJournalC01FileName());

            excel.SetActiveSheetByName(brokerCode);

            for (var iRow = excel.GetRowsCount(); iRow > 0; iRow--)
            {
                var name = excel.GetCell(iRow, "C");
                if(!string.IsNullOrEmpty(name)) journalC01Companies.Add(new JournalC01Company { name = name, bin = excel.GetCell(iRow, "D"), code = excel.GetCell(iRow, "E") });
            }

            excel.CloseWorkbook(false);
            excel.CloseExcel();

            return journalC01Companies;
        }

    }
}
