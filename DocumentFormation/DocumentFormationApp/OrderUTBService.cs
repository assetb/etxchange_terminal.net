using System;
using System.Diagnostics;
using System.Globalization;
using AltaBO;
using AltaMySqlDB.model.tables;
using AltaOffice;
using AltaTransport;

namespace DocumentFormation
{
    public class OrderUTBService
    {
        private static Order orderInfo;
        private static string orderFileName;
        private static WordService word;
        private static LotEF lotInfo;


        public static int CheckNewOrders()
        {
            return DataBaseClient.ReadNewKarazhiraOrders();
        }


        public static void CreateOrder(Order order, int auctionId, int traderId)
        {
            orderInfo = order;

            orderFileName = ArchiveTransport.PutUTBOrder(order);

            lotInfo = DataBaseClient.UpdateUTBNewOrder(order, auctionId, traderId);

            FillTemplateFile();

            Process.Start("explorer", @"\\192.168.11.5\Archive\Auctions\UTB\" + order.Auction.Date.ToShortDateString() + "\\" + order.Title.Replace("/", "_"));
        }


        private static void FillTemplateFile()
        {
            word = new WordService(orderFileName, false);

            try {
                word.FindReplace("[orderNumber]", orderInfo.Title);
                word.FindReplace("[auctionDate]", orderInfo.Auction.Date.ToShortDateString());

                word.SetCell(1, 2, 1, "1");
                word.SetCell(1, 2, 2, lotInfo.description);
                word.SetCell(1, 2, 3, lotInfo.amount.ToString(CultureInfo.InvariantCulture));
                word.SetCell(1, 2, 4, DataBaseClient.ReadUnitInfo(lotInfo.unitid).name);
                word.SetCell(1, 2, 5, lotInfo.price.ToString(CultureInfo.InvariantCulture));
                word.SetCell(1, 2, 6, lotInfo.sum.ToString(CultureInfo.InvariantCulture));
                word.SetCell(1, 2, 7, lotInfo.step.ToString(CultureInfo.InvariantCulture));
                word.SetCell(1, 2, 8, lotInfo.deliveryplace);
                word.SetCell(1, 2, 9, lotInfo.paymentterm);
                word.SetCell(1, 2, 10, lotInfo.warranty.ToString(CultureInfo.InvariantCulture));
                word.SetCell(1, 2, 11, lotInfo.localcontent.ToString());
                word.SetCell(1, 3, 6, lotInfo.sum.ToString(CultureInfo.InvariantCulture));
            } catch (Exception) {
                // ignored
            }

            word.CloseDocument(true);
            word.CloseWord(true);
        }
    }
}
