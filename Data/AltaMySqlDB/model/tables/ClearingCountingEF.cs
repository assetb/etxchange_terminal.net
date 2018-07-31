using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables {
    [Table(name: "clearingcountings")]
    public class ClearingCountingEF {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("broker")]
        public int brokerid { get; set; }

        [ForeignKey("fromsupplier")]
        public int fromsupplierid { get; set; }

        [ForeignKey("tosupplier")]
        public int tosupplierid { get; set; }

        [ForeignKey("lot")]
        public int lotid { get; set; }

        public decimal transaction { get; set; }
        public DateTime createdate { get; set; }

        [ForeignKey("status")]
        public int statusid { get; set; }
        #endregion

        #region Foreign
        public virtual BrokerEF broker { get; set; }
        public virtual SupplierEF fromsupplier { get; set; }
        public virtual SupplierEF tosupplier { get; set; }
        public virtual LotEF lot { get; set; }
        public virtual StatusEF status { get; set; }
        #endregion
    }
}
