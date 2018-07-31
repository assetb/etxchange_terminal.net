using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO.reports
{
    public class AuctionResult
    {
        public DateTime auctionDate { get; set; }
        public string exchange { get; set; }
        public string customer { get; set; }
        public string auctionNumber { get; set; }
        public string lotNumber { get; set; }
        public string startPrice { get; set; }
        public string endPrice { get; set; }
        public string supplier { get; set; }
        public string broker { get; set; }
    }
}
