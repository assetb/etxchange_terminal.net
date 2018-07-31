using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AltaBO;
using KarazhiraCabinet.Models;

namespace KarazhiraCabinet.Services {
    public class OrderToExcelModelConverter {
        public static List<ActiveAuctionsModel> ConvertActiveAuctions(List<Order> orders) {
            List<ActiveAuctionsModel> activeAuctionsModel = new List<ActiveAuctionsModel>();

            if(orders.Count == 1) {
                activeAuctionsModel.Add(new ActiveAuctionsModel() {
                    date = orders[0].Auction.Date,
                    number = orders[0].Auction.Number,
                    status = orders[0].Auction.Status,
                    lotName = orders[0].Auction.Lots[0].Name,
                    sum = orders[0].Auction.Lots[0].Sum,
                    orderDate = orders[0].Date
                });
            } else {
                foreach(var item in orders) {
                    activeAuctionsModel.Add(new ActiveAuctionsModel() {
                        date = item.Auction.Date,
                        number = item.Auction.Number,
                        status = item.Auction.Status,
                        lotName = item.Auction.Lots[0].Name,
                        sum = item.Auction.Lots[0].Sum,
                        orderDate = item.Date
                    });
                }
            }

            return activeAuctionsModel;
        }


        public static List<EndedAuctionsModel> ConvertEndedAuctions(List<Order> orders) {
            List<EndedAuctionsModel> endedAuctionsModel = new List<EndedAuctionsModel>();

            if(orders.Count == 1) {
                endedAuctionsModel.Add(new EndedAuctionsModel() {
                    date = orders[0].Auction.Date,
                    number = orders[0].Auction.Number,
                    status = orders[0].Auction.Status,
                    lotName = orders[0].Auction.Lots[0].Name,
                    sum = orders[0].Auction.Lots[0].Sum,
                    orderDate = orders[0].Date
                });
            } else {
                foreach(var item in orders) {
                    endedAuctionsModel.Add(new EndedAuctionsModel() {
                        date = item.Auction.Date,
                        number = item.Auction.Number,
                        status = item.Auction.Status,
                        lotName = item.Auction.Lots[0].Name,
                        sum = item.Auction.Lots[0].Sum,
                        winningSum = item.Auction.Procuratories == null ? 0 : item.Auction.Procuratories[0].MinimalPrice,
                        economy = item.Auction.Procuratories == null ? 0 : item.Auction.Lots[0].Sum - item.Auction.Procuratories[0].MinimalPrice,
                        supplier = item.Auction.Procuratories == null ? "" : item.Auction.Procuratories[0].SupplierName,
                        orderDate = item.Date
                    });
                }
            }

            return endedAuctionsModel;
        }
    }
}