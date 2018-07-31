using AltaBO.reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaReportsApp.services
{
    public static class CalculatingService
    {
        public static List<DebtorDetails> GetDebtorDetails(DebtorsList debtorsList, int statusId)
        {
            var debtorDetailsList = ReportManager.ReadDebtorDetails(debtorsList.supplierId, statusId);

            if (debtorDetailsList == null) return null;

            foreach(var item in debtorDetailsList) {
                decimal percent = ReportManager.GetRatePercent(item);

                if (percent == 0) item.statusName = "Без тарифа";
                else item.debt = item.debt / 100 * percent;                
            }

            return debtorDetailsList;
        }
    }
}
