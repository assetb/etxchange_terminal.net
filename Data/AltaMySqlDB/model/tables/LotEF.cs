using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "lots")]
    public class LotEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("auction")]
        public int auctionid { get; set; }

        [MaxLength(45)]
        public string number { get; set; }

        public string description { get; set; }

        [ForeignKey("unit")]
        public int unitid { get; set; }

        public decimal amount { get; set; }

        public decimal price { get; set; }

        public decimal sum { get; set; }

        public string paymentterm { get; set; }

        public string deliverytime { get; set; }

        public string deliveryplace { get; set; }

        public int dks { get; set; }

        [MaxLength(30)]
        public string contractnumber { get; set; }

        public double step { get; set; }

        public double warranty { get; set; }

        public int localcontent { get; set; }

        [ForeignKey("attachment")]
        public int? attachmentid { get; set; }

        [ForeignKey("filelist")]
        public int? filelistid { get; set; }
        #endregion

        #region Foreign objects
        public virtual AuctionEF auction { get; set; }
        public virtual UnitEF unit { get; set; }
        public virtual ScanEF attachment { get; set; }
        public virtual FilesListEF filelist { get; set; }
        #endregion

        #region Foreign collections
        public virtual ICollection<LotsExtendedEF> lotsextended { get; set; }
        #endregion
    }
}
