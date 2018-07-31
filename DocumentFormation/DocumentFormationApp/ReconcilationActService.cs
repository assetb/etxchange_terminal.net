using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO;
using AltaOffice;

namespace DocumentFormation {
    public class ReconcilationActService {
        #region Variables
        private static WordService word;
        private static DateTime startDate, endDate;
        private static Broker broker;
        private static Supplier supplier;
        private static List<ReconcilationReport> reconcilationReport;
        private static string fileName;
        #endregion

        #region Methods
        public static void FormateAct(DateTime startDateInfo, DateTime endDateInfo, Broker brokerInfo, Supplier supplierInfo, List<ReconcilationReport> reconcilationReportInfo, string templateFileName) {
            // Set variables
            startDate = startDateInfo;
            endDate = endDateInfo;
            broker = brokerInfo;
            supplier = supplierInfo;
            reconcilationReport = reconcilationReportInfo;
            fileName = templateFileName;

            // Fill document
            FillDocument();
        }

        private static void FillDocument() {
            // Variables
            decimal fullDebet = reconcilationReport.Sum(r => r.debit);
            decimal fullCredit = reconcilationReport.Sum(r => r.credit);

            // Open office
            word = new WordService(fileName, false);

            // Fill document
            word.FindReplace("[startDate]", startDate.ToShortDateString());
            word.FindReplace("[endDate]", endDate.ToShortDateString());
            word.FindReplace("[broker]", broker.Name);
            word.FindReplace("[client]", supplier.Name);
            word.FindReplace("[broker]", broker.Name);
            word.FindReplace("[client]", supplier.Name);
            word.FindReplace("[broker]", broker.Name);
            word.FindReplace("[client]", supplier.Name);

            word.FindReplace("[startDate]", startDate.ToShortDateString());
            word.FindReplace("[startDate]", startDate.ToShortDateString());

            int iRow = 4;
            string debit = "", credit = "", fDebit = "", fCredit = "";

            foreach(var item in reconcilationReport) {
                word.AddTableRow(1, iRow);

                debit = $"{item.debit:C}";
                credit = $"{item.credit:C}";

                word.SetCell(1, iRow, 1, item.docDate);
                word.SetCell(1, iRow, 2, item.docName);
                word.SetCell(1, iRow, 3, debit.Substring(0, debit.Length - 2));
                word.SetCell(1, iRow, 4, credit.Substring(0, credit.Length - 2));

                iRow++;
            }

            iRow += 3;

            if(fullDebet > fullCredit) {
                string fullSum = $"{(fullDebet - fullCredit):C}";

                fullSum = fullSum.Substring(0, fullSum.Length - 2);

                word.SetCell(1, iRow, 2, fullSum);
                word.FindReplace("[status]", "в пользу " + broker.Name + " " + fullSum);
            } else {
                string fullSum = $"{(fullCredit - fullDebet):C}";

                fullSum = fullSum.Substring(0, fullSum.Length - 2);

                word.SetCell(1, iRow, 3, fullSum);
                word.FindReplace("[status]", "в пользу " + supplier.Name + " " + fullSum);
            }

            fDebit = $"{fullDebet:C}";
            fCredit = $"{fullCredit:C}";

            word.FindReplace("[fullDebet]", fDebit.Substring(0, fDebit.Length - 2));
            word.FindReplace("[fullCredit]", fCredit.Substring(0, fCredit.Length - 2));

            word.FindReplace("[endDate]", endDate.ToShortDateString());
            word.FindReplace("[endDate]", endDate.ToShortDateString());
            word.FindReplace("[broker]", broker.Name);
            word.FindReplace("[endDate]", endDate.ToShortDateString());
            word.FindReplace("[broker]", broker.Name);
            word.FindReplace("[client]", supplier.Name);
            word.FindReplace("[brokerBin]", broker.Requisites);
            word.FindReplace("[clientBin]", supplier.BIN);

            // Close office
            word.CloseDocument(true);
            word.CloseWord(true);
        }
        #endregion
    }
}
