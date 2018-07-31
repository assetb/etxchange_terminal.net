using System;

namespace AltaBO
{
    public class StockDealInfo
    {
        public DateTime DealRegDate { get; set; }
        public string LotCode { get; set; }
        public string MemberName { get; set; }
        public string MemberCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public DateTime ComDateIn { get; set; }
        public DateTime ComDateOut { get; set; }
        public string Employe { get; set; }
    }
}
