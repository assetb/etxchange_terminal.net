using AltaBO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaTradingSystemApp.Services
{
    public static class AuctionService
    {
        public static int CreateOrder(Order order)
        {
            return DataManagerService.TradingInstance().CreateOrder(order);
        }


        public static List<Order> ReadOrders(int statusId = 1)
        {
            return DataManagerService.TradingInstance().ReadOrders(statusId);
        }


        public static Order ReadOrder(int auctionId)
        {
            return DataManagerService.TradingInstance().ReadOrder(auctionId); 
        }


        public static List<Auction> ReadAuctions(DateTime fromDate, DateTime toDate, int statusId)
        {
            return DataManagerService.TradingInstance().ReadAuctions(fromDate, toDate, statusId);
        }


        public static int CreateAuction(Auction auction)
        {
            return DataManagerService.TradingInstance().CreateAuction(auction);
        }


        public static void UpdateAuction(Auction auction)
        {
            DataManagerService.TradingInstance().UpdateAuction(auction);
        }


        public static Regulation ReadRegulation(int regulationId)
        {
            return DataManagerService.TradingInstance().ReadRegulation(regulationId);
        }


        public static int CreateRegulation(Regulation regulation)
        {
            return DataManagerService.TradingInstance().CreateRegulation(regulation);
        }


        public static void UpdateRegulation(Regulation regulation, int regulationId)
        {
            DataManagerService.TradingInstance().UpdateRegulation(regulation, regulationId);
        }
    }
}
