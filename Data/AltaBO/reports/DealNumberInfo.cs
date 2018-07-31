using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using altaik.baseapp.vm;

namespace AltaBO.reports
{
    public class DealNumberInfo : BaseViewModel
    {
        public string dealNumber { get; set; }
        public DateTime auctionDate { get; set; }
        public string auctionNumber { get; set; }
        public string customerName { get; set; }
        public string lotCode { get; set; }
        public string supplierName { get; set; }
        public decimal finalPriceOffer { get; set; }
        public string traderName { get; set; }
        public string brokerName { get; set; }
        public int brokerId { get; set; }
        public int auctionId { get; set; }
        public int supplierId { get; set; }
        public int exchangeId { get; set; }

        private decimal _debt;
        public decimal debt {
            get { return _debt; }
            set { _debt = value; RaisePropertyChangedEvent("debt"); }
        }
    }
}
