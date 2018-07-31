using System;

namespace AltaBO
{
    public class TechSpecReportBO
    {
        public int? id { get; set; }
        public string auctionNumber { get; set; }
        public DateTime orderDate { get; set; }
        public string lotCode { get; set; }
        public string productName { get; set; }
        public string unit { get; set; }
        public decimal? quantity { get; set; }
        public decimal? price { get; set; }
        public decimal? productSum { get; set; }
        public decimal? startPriceOffer { get; set; }
        public string dealNumber { get; set; }
        public string name { get; set; }
        public DateTime auctionDate { get; set; }
        public decimal? productFinalPrice { get; set; }
        public decimal? productFinalSum { get; set; }
        public decimal? finalPriceOffer { get; set; }
        public decimal? differenceBetweenSum { get; set; }
        public decimal? differencePercent { get; set; }
        public int customerId { get; set; }
        public int siteId { get; set; }
        public int statusId { get; set; }
        public int auctionId { get; set; }
        public string contractNumber { get; set; }
    }
}
