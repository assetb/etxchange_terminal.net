using AltaBO;
using AltaBO.reports;
using AltaMySqlDB.model;
using AltaMySqlDB.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaReportsApp.services
{
    public static class ReportManager
    {
        public static IDataManager dbManager = new EntityContext();


        public static List<AltaBO.reports.AuctionResult> ReadAuctionsResult()
        {
            return dbManager.ReadAuctionsResultReport();
        }


        public static List<DealNumberInfo> ReadDealNumbersInfo()
        {
            return dbManager.ReadDealNumbersInfo();
        }


        public static List<DebtorsList> ReadDebtorsList()
        {
            return dbManager.ReadDebtorsList();
        }


        public static List<DebtorsList> ReadDebtorsList(string customerName, string supplierName)
        {
            return dbManager.ReadDebtorsList(customerName, supplierName);
        }

        public static List<DebtorDetails> ReadDebtorDetails(int supplierId, int statusId)
        {
            return dbManager.ReadDebtorDetails(supplierId, statusId);
        }


        public static decimal GetRatePercent(DebtorDetails debtorDetails)
        {
            return GetRatePercent(debtorDetails.supplierId, debtorDetails.auctionId, debtorDetails.exchangeId, debtorDetails.debt, debtorDetails.brokerId);
        }


        public static decimal GetRatePercent(int supplierId, int auctionId, int exchangeId, decimal debt, int brokerId)
        {
            /*var supplierOrder = dbManager.ReadSupplierOrder(supplierId, auctionId);

            if (supplierOrder == null || supplierOrder.contractId == null) return 0;*/

            //
            var company = dbManager.GetCompanySupplier(supplierId);

            if (company == null) return 0;

            var contracts = dbManager.ReadContracts(company.id, brokerId);

            if (contracts == null || contracts.Count < 1) return 0;

            RatesList ratesList = new RatesList() { id = 0 };

            foreach (var contract in contracts) {
                var ratesLst = dbManager.ReadRatesList(contract.id, exchangeId);

                if (ratesLst != null) ratesList = ratesLst;
            }

            if (ratesList.id == 0) return 0;
            //

            //var ratesList = dbManager.ReadRatesList((int)supplierOrder.contractId, exchangeId);

            if (ratesList == null) return 0;

            var rates = dbManager.ReadRates(ratesList.id);

            if (rates == null) return 0;

            var rateItem = rates.OrderByDescending(r => r.transaction).FirstOrDefault(r => r.transaction < debt);

            try {
                if (rateItem != null) return rates.FirstOrDefault(r => r.id == rateItem.id).percent;
                else return rates.FirstOrDefault(r => r.id == rates.OrderBy(ra => ra.transaction).FirstOrDefault().id).percent;
            } catch { return 0; }
        }


        public static void UpdateDebtorStatus(DebtorDetails debtorDetails, bool isDebtor)
        {
            dbManager.UpdateFinalReport(debtorDetails.id, isDebtor ? 9 : 10);
        }


        public static List<ContractsReportView> ReadContractsReport(DateTime fromDate, DateTime toDate, string searchQuery)
        {
            return dbManager.ReadContractsReport(fromDate, toDate, searchQuery);
        }
    }
}
