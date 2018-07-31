using AltaMySqlDB.model;
using AltaMySqlDB.model.views;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AltaMySqlDB.Helpers
{
    public static class ContextProgedureHelper
    {
        public static List<AnaliticCountStatusView> CustCountProcedure(this EntityContext context, int customerId, DateTime startDate, DateTime endDate)
        {
            var cusID = new MySqlParameter("@cusID", customerId);
            var sDate = new MySqlParameter("@startDate", startDate.ToString("yyyy-MM-dd"));
            var eDate = new MySqlParameter("@endDate", endDate.ToString("yyyy-MM-dd"));
            var analiticCountStatusView = context.Database.SqlQuery<AnaliticCountStatusView>("call custCount(@cusID, @startDate, @endDate);", cusID, sDate, eDate).ToList();

            analiticCountStatusView.ForEach(a => a.StatusEf = context.statuses.Find(a.Status));

            return analiticCountStatusView;
        }

        public static List<AnaliticCountStatusView> SupCountProcedure(this EntityContext context, int supplierId, DateTime startDate, DateTime endDate)
        {
            var cusID = new MySqlParameter("@supID", supplierId);
            var sDate = new MySqlParameter("@startDate", startDate.ToString("yyyy-MM-dd"));
            var eDate = new MySqlParameter("@endDate", endDate.ToString("yyyy-MM-dd"));
            var analiticCountStatusView = context.Database.SqlQuery<AnaliticCountStatusView>("call supCount(@supID, @startDate, @endDate);", cusID, sDate, eDate).ToList();

            analiticCountStatusView.ForEach(a => a.StatusEf = context.statuses.Find(a.Status));

            return analiticCountStatusView;
        }
    }
}
