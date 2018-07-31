using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO
{
    public class FinalReport
    {
        public int id { get; set; }
        public int auctionId { get; set; }
        public int supplierId { get; set; }
        public int lotId { get; set; }
        public string dealNumber { get; set; }
        public decimal finalPriceOffer { get; set; }
        public int brokerId { get; set; }      

        public string aucNumber { get; set; }  
        public int customerid { get; set; }
        public int typeid { get; set; }
        public DateTime date { get; set; }
        public int siteid { get; set; }
        public int statusid { get; set; }    
        public string lotNumber { get; set; }
        public string description { get; set; }
        public decimal amount { get; set; }
        public string unit { get; set; }
        public decimal startprice { get; set; }
        public decimal finalprice { get; set; }
        public decimal unitprice { get; set; }
        public Supplier supplier { get; set; }
    }
}
