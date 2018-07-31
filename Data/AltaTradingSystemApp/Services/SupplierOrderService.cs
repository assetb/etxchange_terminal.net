using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO;
using AltaBO.views;

namespace AltaTradingSystemApp.Services
{
    public static class SupplierOrderService
    {
        public static List<SupplierOrderView> ReadSupplierOrders(int auctionId)
        {
            return DataManagerService.TradingInstance().ReadSupplierOrders(auctionId);
        }


        public static SupplierOrderView ReadSupplierOrder(int supplierOrderId)
        {
            return DataManagerService.TradingInstance().ReadSupplierOrder(supplierOrderId);
        }


        public static List<Procuratory> ReadProcuratories(int auctionId, int supplierId)
        {
            return DataManagerService.TradingInstance().ReadProcuratories(auctionId, supplierId);
        }


        public static Procuratory ReadProcuratory(int procuratoryId)
        {
            return DataManagerService.TradingInstance().ReadProcuratory(procuratoryId);
        }
    }
}
