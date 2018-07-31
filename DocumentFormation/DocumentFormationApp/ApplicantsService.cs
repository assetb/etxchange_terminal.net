using System.Collections.Generic;
using AltaOffice;
using AltaBO;
using AltaBO.specifics;
using System;
using System.Linq;

namespace DocumentFormation
{
    public class ApplicantsService
    {
        #region ETS & UTB
        private static WordService word;
        private static Order order;
        private static List<SupplierOrder> notAllowed;
        private static DocumentRequisite docReq;
        private static bool forCustomer;
        private static string fileName;


        public static void CreateApplicants(Order orderInfo, List<SupplierOrder> notAllowedSuppliers, DocumentRequisite docReqInfo, string fileNameInfo, bool forCustomerInfo = false)
        {
            order = orderInfo;
            notAllowed = new List<SupplierOrder>(notAllowedSuppliers);
            docReq = docReqInfo;
            fileName = fileNameInfo;
            forCustomer = forCustomerInfo;

            FillTemplate(order.Auction.SiteId == 4 ? 2 : 1);
        }


        private static void FillTemplate(int mode)
        {
            word = new WordService(fileName, false);

            if (mode == 1) { // UTB
                var iCount = 1;

                word.FindReplace("[auctionDate]", order.Auction.Date.ToShortDateString());
                word.FindReplace("[auctionNumber]", order.Auction.Number);

                foreach (var item in order.Auction.SupplierOrders) {
                    word.SetCell(1, iCount, 1, iCount + ". " + item.Name);
                    word.SetCell(2, iCount, 1, iCount + ". " + item.Name);
                    iCount++;
                    word.AddTableRow(1);
                    word.AddTableRow(2);
                }
            } else { // ETS
                word.FindReplace("[orderDate]", DateTime.Now.ToShortDateString());
                word.FindReplace("[lotCode]", order.Auction.Lots[0].Number);
                word.FindReplace("[orderNumber]", order.Auction.Number);
                word.FindReplace("[lotCode]", order.Auction.Lots[0].Number);

                int iCount = 1;

                foreach (var item in order.Auction.SupplierOrders) {
                    if (iCount != 1) {
                        word.AddTableRow(1);
                    }

                    word.SetCell(1, iCount + 1, 1, iCount.ToString());
                    word.SetCell(1, iCount + 1, 2, "Код клиента: " + item.Code + " -\n" + ConvertToShortName(item.Name) + "\n\nКод брокера: " + item.BrokerCode + " -\n" + ConvertToShortName(item.BrokerName));
                    word.SetCell(1, iCount + 1, 3, "БИН " + item.BIN + "\n" + item.BankName + "\nБИК " + item.BIK + " Кбе " + item.Kbe + "\n" + item.IIK);
                    word.SetCell(1, iCount + 1, 4, item.Address);

                    iCount++;
                }

                if (!forCustomer) {
                    iCount = 1;

                    foreach (var item in notAllowed) {
                        if (iCount != 1) {
                            word.AddTableRow(2);
                        }

                        word.SetCell(2, iCount + 1, 1, iCount.ToString());
                        word.SetCell(2, iCount + 1, 2, "Код брокера: " + item.BrokerCode + " -\n" + ConvertToShortName(item.BrokerName));
                        word.SetCell(2, iCount + 1, 3, item.Name + "\nБИН: " + item.BIN + "\nАдрес: " + item.Address + "\nТелефоны: " + item.Phones);

                        iCount++;
                    }
                }
            }

            word.SaveAsPDF(fileName);

            word.CloseDocument(true);
            word.CloseWord(true);
        }


        private static string ConvertToShortName(string fullName)
        {
            if (fullName.ToUpper().Contains("ТОВАРИЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ")) fullName = fullName.ToUpper().Replace("ТОВАРИЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ", "ТОО");
            else if (fullName.ToUpper().Contains("АКЦИОНЕРНОЕ ОБЩЕСТВО")) fullName = fullName.ToUpper().Replace("АКЦИОНЕРНОЕ ОБЩЕСТВО", "АО");
            else if (fullName.ToUpper().Contains("ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ")) fullName = fullName.ToUpper().Replace("ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ", "ИП");
            else if (fullName.ToUpper().Contains("ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ")) fullName = fullName.ToUpper().Replace("ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ", "ООО");

            return fullName;
        }
        #endregion

        #region Kaspi
        public static bool FillApplicants(string templateFileName, List<SupplierOrder> applicants, DateTime auctionDate, string orderNumber, string customerCompany)
        {
            if (string.IsNullOrEmpty(templateFileName) || applicants == null || applicants.Count < 1) return false;

            var word = new WordService(templateFileName, false);

            if (word == null) return false;

            try {
                word.FindReplace("[auctionDate]", auctionDate != null ? auctionDate.ToShortDateString() : "");
                word.FindReplace("[orderNumber]", orderNumber != null ? orderNumber : "");
                word.FindReplace("[customerCompany]", customerCompany != null ? customerCompany : "");

                FillApplicantsTable(word, applicants, 1);

                var accessPlayers = applicants.Where(a => a.status != null && a.status.Id != null && a.status.Id != 16);

                if (accessPlayers != null && accessPlayers.ToList().Count > 0) FillApplicantsTable(word, accessPlayers.ToList(), 2);

                var deniedPlayers = applicants.Where(a => a.status != null && a.status.Id != null && a.status.Id == 16);

                if (deniedPlayers != null && deniedPlayers.ToList().Count > 0) FillApplicantsTable(word, deniedPlayers.ToList(), 3);
            } catch {
                return false;
            } finally {
                if (word.IsOpenDocument()) word.CloseDocument(true);
                if (word.IsOpenWord()) word.CloseWord(true);
            }

            return true;
        }


        private static void FillApplicantsTable(WordService word, List<SupplierOrder> applicants, int tableNumber)
        {
            int iCount = 1;
            int rowCount = 2;

            foreach (var item in applicants) {
                if (iCount > 2) word.AddTableRow(tableNumber);

                word.SetCell(tableNumber, rowCount, 1, iCount.ToString());
                word.SetCell(tableNumber, rowCount, 2, item.Name != null ? item.Name : "");

                iCount += 1;
                rowCount += 1;
            }
        }


        public static List<SupplierOrder> ParseApplicants(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var word = new WordService(fileName, false);

            if (word == null) return null;

            List<SupplierOrder> supplierOrders = new List<SupplierOrder>();

            try {
                int rowQuantity = word.GetTableRowsCount(1);
                SupplierOrder applicant = new SupplierOrder();

                for (var rowCount = 2; rowCount <= rowQuantity + 1; rowCount++) {
                    var supplierName = word.GetCell(1, rowCount, 2);
                    applicant.Name = supplierName != null ? supplierName : "";

                    var brokerName = word.GetCell(1, rowCount, 3);
                    applicant.BrokerName = brokerName != null ? brokerName : "";

                    supplierOrders.Add(applicant);
                }
            } catch {
                return null;
            } finally {
                if (word.IsOpenDocument()) word.CloseDocument();
                if (word.IsOpenWord()) word.CloseWord(false);
            }

            return supplierOrders;
        }


        public static bool FillMembers(string templateFileName, List<SupplierOrder> members)
        {
            if (string.IsNullOrEmpty(templateFileName) || members == null || members.Count < 1) return false;

            var word = new WordService(templateFileName, false);

            if (word == null) return false;

            try {
                int rowCount = 2;

                foreach (var item in members) {
                    if (rowCount > 2) word.AddTableRow(1);

                    word.SetCell(1, rowCount, 1, (rowCount - 1).ToString());
                    word.SetCell(1, rowCount, 2, item.Name != null ? item.Name : "");
                    word.SetCell(1, rowCount, 3, item.BrokerName != null ? item.BrokerName : "");
                }
            } catch {
                return false;
            } finally {
                if (word.IsOpenDocument()) word.CloseDocument();
                if (word.IsOpenWord()) word.CloseWord(false);
            }

            return true;
        }
        #endregion
    }
}