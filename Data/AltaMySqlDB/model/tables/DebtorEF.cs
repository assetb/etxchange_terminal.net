using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.tables {
    [Table(name: "accounting")]
    public class DebtorEF {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("supplier")]
        public int supplierid { get; set; }

        [ForeignKey("auction")]
        public int auctionid { get; set; }

        public decimal debt { get; set; }
        public DateTime debtdate { get; set; }
        public bool paymentstatus { get; set; }
        #endregion

        #region ForeignKeys
        public SupplierEF supplier { get; set; }
        public AuctionEF auction { get; set; }
        #endregion
    }
}
