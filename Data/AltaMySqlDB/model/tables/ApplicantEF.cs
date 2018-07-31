using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "applicants")]
    public class ApplicantEF
    {
        #region Columns
        [Key]
        public int? id { get; set; }

        [ForeignKey("auction")]
        public int? auctionid { get; set; }

        [ForeignKey("supplierorder")]
        public int? supplierorderid { get; set; }

        [ForeignKey("lot")]
        public int? lotid { get; set; }
        #endregion

        #region Foreigen Objects
        public virtual AuctionEF auction { get; set; }
        public virtual SupplierOrderEF supplierorder { get; set; }
        public virtual LotEF lot { get; set; }
        #endregion
    }
}
