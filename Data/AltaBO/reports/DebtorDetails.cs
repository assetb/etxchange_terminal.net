using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO.reports
{
    public class DebtorDetails
    {
        public int id { get; set; }
        public int supplierId { get; set; }
        public int auctionId { get; set; }
        public DateTime auctionDate { get; set; }
        public string auctionNumber { get; set; }
        public int exchangeId { get; set; }
        public string exchangeName { get; set; }
        public string customerName { get; set; }
        public string dealNumber { get; set; }
        public int lotId { get; set; }
        public string lotCode { get; set; }
        public string lotDescription { get; set; }
        public decimal debt { get; set; }
        public string brokerName { get; set; }
        public int brokerId { get; set; }
        public string statusName { get; set; }
        public string traderName { get; set; }
        public int statusId { get; set; }
    }
}
