using System;
using System.Collections.Generic;
using AltaTransport;
using AltaOffice;
using AltaMySqlDB.model.tables;
using System.Diagnostics;
using AltaBO;

namespace DocumentFormation {
    public class ProtocolsService {
        private static string protocolsFileName;
        private static WordService word;
        private static Order order;


        public static void CreateProtocols(Order orderInfo) {
            order = orderInfo;

            protocolsFileName = ArchiveTransport.PutUTBProtocol(order);

            FillTemplate();
        }


        private static void FillTemplate() {
            word = new WordService(protocolsFileName, false);

            var winApplicant = order.Auction.SupplierOrders[0].Name.ToUpper().Contains("АЛТЫН") ? 1 : 0; 

            decimal minimalPrice = order.Auction.Procuratories[0].MinimalPrice < order.Auction.Procuratories[1].MinimalPrice ? order.Auction.Procuratories[0].MinimalPrice : order.Auction.Procuratories[1].MinimalPrice;

            word.FindReplace("[auctionDate]", order.Auction.Date.ToShortDateString());
            word.FindReplace("[amount]", Math.Round(order.Auction.Lots[0].Sum, 2).ToString());
            word.FindReplace("[supplier1]", order.Auction.SupplierOrders[0].Name);
            word.FindReplace("[supplier2]", order.Auction.SupplierOrders[1].Name);
            word.FindReplace("[winningPrice]", Math.Round(minimalPrice, 2).ToString());
            word.SetCell(1, 17, 1, "1");
            word.SetCell(1, 17, 2, order.Auction.Lots[0].Number);
            word.SetCell(1, 17, 3, order.Auction.Lots[0].Name);
            word.SetCell(1, 17, 4, order.Auction.Lots[0].Unit);
            word.SetCell(1, 17, 5, Math.Round(order.Auction.Lots[0].Quantity, 2).ToString());
            word.SetCell(1, 17, 6, Math.Round(minimalPrice / order.Auction.Lots[0].Quantity, 2).ToString());
            word.SetCell(1, 17, 7, Math.Round(minimalPrice, 2).ToString());
            word.SetCell(1, 17, 8, order.Auction.Lots[0].PaymentTerm);
            word.SetCell(1, 17, 9, order.Auction.Lots[0].DeliveryTime + ", " + order.Auction.Lots[0].DeliveryPlace.Replace("|", ", "));
            word.SetCell(1, 18, 7, Math.Round(minimalPrice, 2).ToString());
            word.FindReplace("[supplier]", order.Auction.SupplierOrders[winApplicant].Name);
            word.FindReplace("[supplierBroker]", order.Auction.SupplierOrders[winApplicant].BrokerName);
            word.FindReplace("[broker]", order.Auction.SupplierOrders[winApplicant].BrokerName.Replace("Товарищество с ограниченной ответственностью", "ТОО"));

            word.CloseDocument(true);
            word.CloseWord(true);
        }
    }
}
