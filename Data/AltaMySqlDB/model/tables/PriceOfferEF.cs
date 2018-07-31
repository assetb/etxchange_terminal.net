using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "priceoffers")]
    public class PriceOfferEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("supplyoffer")]
        public int supplyofferid { get; set; }

        [ForeignKey("lot")]
        public int lotid { get; set; }

        public double price { get; set; }
        #endregion

        #region Foreign Objects
        public virtual SupplierOrderEF supplyoffer { get; set; }
        public virtual LotEF lot { get; set; }
        #endregion
    }
}
