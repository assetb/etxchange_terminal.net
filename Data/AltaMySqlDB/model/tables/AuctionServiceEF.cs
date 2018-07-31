using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "auctionservice")]
    public class AuctionServiceEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("lot")]
        public int lotid { get; set; }

        [ForeignKey("status")]
        public int statusid { get; set; }

        [ForeignKey("supplier")]
        public int supplierid { get; set; }
        public decimal sum { get; set; }
        public string comments { get; set; }
        #endregion


        #region Foreign
        public virtual LotEF lot { get; set; }
        public virtual StatusEF status { get; set; }
        public virtual SupplierEF supplier { get; set; }
        #endregion
    }
}
