using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "order")]
    public class OrderEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        public int? initiatorid { get; set; }

        [ForeignKey("auction")]
        public int auctionid { get; set; }

        [ForeignKey("filesList")]
        public int? fileslistid { get; set; }

        [ForeignKey("status")]
        public int statusid { get; set; }
        public string number { get; set; }

        [ForeignKey("site")]
        public int siteid { get; set; }
        public DateTime date { get; set; }

        [ForeignKey("customer")]
        public int? customerid { get; set; }
        #endregion

        #region Foreign Objects
        public virtual CustomerEF customer { get; set; }
        public virtual AuctionEF auction { get; set; }
        public virtual FilesListEF filesList { get; set; }
        public virtual StatusEF status { get; set; }
        public virtual SiteEF site { get; set; }
        #endregion
    }
}
