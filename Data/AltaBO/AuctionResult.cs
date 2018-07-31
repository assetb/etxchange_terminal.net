using System;

namespace AltaBO {
    public class AuctionResult {
        public int id { get; set; }
        public DateTime date { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string bin { get; set; }
        public decimal startprice { get; set; }
        public decimal minimalprice { get; set; }
        public decimal reward { get; set; }
    }
}
