using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "auctions")]
    public class AuctionEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        public DateTime date { get; set; }

        [ForeignKey("section")]
        public int sectionid { get; set; }

        [ForeignKey("type")]
        public int typeid { get; set; }

        [MaxLength(50)]
        public string number { get; set; }

        [ForeignKey("status")]
        public int statusid { get; set; }

        public string comments { get; set; }

        public bool ndsincluded { get; set; }

        public bool signstatusid { get; set; }

        public bool published { get; set; }

        [ForeignKey("owner")]
        public int ownerid { get; set; }

        [ForeignKey("site")]
        public int siteid { get; set; }

        [ForeignKey("regulation")]
        public int regulationid { get; set; }

        [ForeignKey("trader")]
        public int traderid { get; set; }

        [ForeignKey("customer")]
        public int customerid { get; set; }

        [ForeignKey("broker")]
        public int brokerid { get; set; }

        [ForeignKey("fileslist")]
        public int? fileslistid { get; set; }
        #endregion

        #region Foreign objects
        public virtual SectionEF section { get; set; }
        public virtual TypeEF type { get; set; }
        public virtual StatusEF status { get; set; }
        public virtual UserEF owner { get; set; }
        public virtual SiteEF site { get; set; }
        public virtual RegulationEF regulation { get; set; }
        public virtual TraderEF trader { get; set; }
        public virtual CustomerEF customer { get; set; }
        public virtual BrokerEF broker { get; set; }
        public virtual FilesListEF fileslist { get; set; }
        #endregion

        #region Foreign Collections
        public virtual ICollection<LotEF> lots { get; set; }
        public virtual ICollection<ProcuratoryEF> procuratories { get; set; }
        public virtual ICollection<SupplierOrderEF> supplierOrders { get; set; }
        #endregion
    }
}
