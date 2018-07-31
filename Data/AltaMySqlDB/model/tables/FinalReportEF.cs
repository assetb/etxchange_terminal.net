using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "finalreport")]
    public class FinalReportEF {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("auction")]
        public int auctionId { get; set; }
        public string dealNumber { get; set; }

        [ForeignKey("supplier")]
        public int supplierId { get; set; }

        [ForeignKey("lot")]
        public int lotId { get; set; }
        public decimal finalPriceOffer { get; set; }

        [ForeignKey("broker")]
        public int brokerId { get; set; }

        [ForeignKey("status")]
        public int statusId { get; set; }
        #endregion

        #region ForeignKeys
        public virtual AuctionEF auction { get; set; }
        public virtual SupplierEF supplier { get; set; }
        public virtual LotEF lot { get; set; }
        public virtual BrokerEF broker { get; set; }
        public virtual StatusEF status { get; set; }
        #endregion
    }
}
