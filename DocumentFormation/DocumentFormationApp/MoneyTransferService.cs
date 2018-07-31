using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO;
using AltaOffice;
using AltaLog;

namespace DocumentFormation {
    public class MoneyTransferService {
        #region Variables
        private static Order order;
        private static string fileName;
        private static decimal sum;
        private static WordService word;
        private static string broker;
        private static List<MoneyTransferList> moneyTransferList;
        #endregion

        #region Methods
        public static bool CreateDocument(string templateFileName, string brokerInfo, List<MoneyTransferList> moneyTransferListInfo) {
            fileName = templateFileName;
            broker = brokerInfo;
            moneyTransferList = moneyTransferListInfo;

            try {
                FillTemplate(2);
            } catch(Exception ex) {
                AppJournal.Write("Money transfer formation", "Err: " + ex.ToString(), true);
                return false;
            }

            return true;
        }

        public static bool CreateDocument(Order orderInfo, string templateFileName, decimal transactionSum) {
            order = orderInfo;
            fileName = templateFileName;
            sum = transactionSum;

            try {
                FillTemplate(1);
            } catch(Exception ex) {
                AppJournal.Write("Money transfer formation", "Err: " + ex.ToString(), true);
                return false;
            }

            return true;
        }

        private static void FillTemplate(int mode) {
            // Open template file for filling
            word = new WordService(fileName, false);

            switch(mode) {
                case 1: // Single
                        // Fill file with data
                    word.FindReplace("[brokerName]", order.Auction.SupplierOrders[0].BrokerName);
                    word.SetCell(1, 2, 1, order.Auction.SupplierOrders[0].BrokerCode);
                    word.SetCell(1, 2, 2, order.Auction.SupplierOrders[0].Code);
                    word.SetCell(1, 2, 3, order.Auction.Lots[0].Number);
                    word.SetCell(1, 2, 4, (Math.Round(sum, 2)).ToString());
                    word.FindReplace("[curDate]", DateTime.Now.ToShortDateString());
                    break;
                case 2: // Multi
                    word.FindReplace("[brokerName]", broker);

                    int iRow = 2;

                    foreach(var item in moneyTransferList) {
                        word.SetCell(1, iRow, 1, item.fromCompany);
                        word.SetCell(1, iRow, 2, item.toCompany);
                        word.SetCell(1, iRow, 3, item.lotNumber);
                        word.SetCell(1, iRow, 4, (Math.Round(sum, 2)).ToString());

                        word.AddTableRow(1);
                        iRow++;
                    }

                    word.FindReplace("[curDate]", DateTime.Now.ToShortDateString());
                    break;
            }
            // Close file & save
            word.CloseDocument(true);
            word.CloseWord(true);
        }
        #endregion
    }
}
