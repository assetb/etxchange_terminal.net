using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models.TransactionAPI
{
    public class AuctionDetails
    {
        public TradeSectionEnum Section { get; set; }
        public AuctionTypeEnum Type { get; set; }
        public AuctionClassEnum Class { get; set; }
        public AuctionDirectionEnum Direction { get; set; }
        public TradeOperationEnum Operation { get; set; }
        public BrokerCo Broker { get; set; }
        public decimal Provision { get; set; }
        public bool CC { get; set; }
        public int Period { get; set; }
        public int SessionsNo { get; set; }
        public Deadlines Deadlines { get; set; }
        public List<Lot> Lots { get; set; }
        public Attachments Attachments { get; set; }
    }
}