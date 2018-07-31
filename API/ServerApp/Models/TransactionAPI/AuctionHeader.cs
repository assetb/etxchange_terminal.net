using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models.TransactionAPI
{
    public class AuctionHeader
    {
        public int No { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }
}