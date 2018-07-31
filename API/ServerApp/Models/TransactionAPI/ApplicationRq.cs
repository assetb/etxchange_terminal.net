using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models.TransactionAPI
{
    public class ApplicationRq
    {
        public RequestHeader Header { get; set; }
        public AuctionHeader AuctionHeader { get; set; }
        public ActionEnum Action { get; set; }
        public ExchangeEnum Exchange { get; set; }
        public string Representative { get; set; }
        public string Warranty { get; set; }
        public Contract Contract { get; set; }
        public AuctionDetails AuctionDetails { get; set; }
        public Attachments Attachments { get; set; }
    }
}