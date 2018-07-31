using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaBO;

namespace AltaTradingSystemApp.Services
{
    public static class LotService
    {
        public static List<Lot> ReadLots(int auctionId)
        {
            return DataManagerService.TradingInstance().ReadLots(auctionId);
        }


        public static Lot ReadLot(int lotId)
        {
            return DataManagerService.TradingInstance().ReadLot(lotId);
        }


        public static List<LotsExtended> ReadLotExtended(int lotId)
        {
            return DataManagerService.TradingInstance().ReadLotExtended(lotId);
        }


        public static int CreateLot(Lot lot)
        {
            return DataManagerService.TradingInstance().CreateLot(lot);
        }


        public static void UpdateLot(Lot lot)
        {
            DataManagerService.TradingInstance().UpdateLot(lot);
        }


        public static void DeleteLot(int lotId)
        {
            DataManagerService.TradingInstance().DeleteLot(lotId);
        }
    }
}
