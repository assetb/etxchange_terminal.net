using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name:"procuratories")]
    public class ProcuratoryEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("supplier")]
        public int? supplierid { get; set; }

        [ForeignKey("auction")]
        public int? auctionid { get; set; }

        [ForeignKey("lot")]
        public int? lotid { get; set; }

        public decimal minimalprice { get; set; }

        [Column("scanid")]
        public int? fileId { get; set; }

        [ForeignKey("filelist")]
        public int? filelistid { get; set; }
        #endregion

        #region Foreign keys
        public virtual SupplierEF supplier { get; set; }
        public virtual AuctionEF auction { get; set; }
        public virtual LotEF lot { get; set; }
        public virtual FilesListEF filelist { get; set; }
        #endregion
    }
}
