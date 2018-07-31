using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaOffice;
using AltaBO;
using AltaLog;

namespace DocumentFormation
{
    public class CompanyProfileService
    {
        #region Variables
        private static ExcelService excel;
        private static string fileName;
        private static Company company;
        private static Contract contract;
        #endregion

        #region Methods
        public static bool CreateDocument(Company companyInfo, Contract contractInfo, string templateFileName)
        {
            company = companyInfo;
            contract = contractInfo;
            fileName = templateFileName;

            try
            {
                FillDocument();
            }
            catch (Exception ex)
            {
                AppJournal.Write("Company profile formate", "Err: " + ex.ToString(), true);
                return false;
            }

            return true;
        }

        private static void FillDocument()
        {
            // Open document
            excel = new ExcelService(fileName);

            // Fill document
            excel.SetCells(5, "A", ""); // Registral number
            excel.SetCells(5, "B", company.name);
            excel.SetCells(5, "C", company.bin);
            excel.SetCells(5, "D", ""); // RNN
            excel.SetCells(5, "E", company.govregnumber);
            excel.SetCells(5, "F", company.govregdate == null ? "Не указано" : ((DateTime)company.govregdate).ToShortDateString());
            excel.SetCells(5, "G", contract.number);
            excel.SetCells(5, "H", contract.agreementdate == null ? "" : ((DateTime)contract.agreementdate).ToShortDateString());
            excel.SetCells(5, "I", ""); // City
            excel.SetCells(5, "J", company.addressActual);
            excel.SetCells(5, "K", company.addressLegal);
            excel.SetCells(5, "L", company.directorPowers);
            excel.SetCells(5, "M", company.director);
            excel.SetCells(5, "N", ""); // City
            excel.SetCells(5, "O", company.bank);
            excel.SetCells(5, "P", company.bik);
            excel.SetCells(5, "Q", company.iik.Replace(" ", ""));

            // Save & close document
            excel.CloseWorkbook(true);
            excel.CloseExcel();
        }
        #endregion
    }
}
