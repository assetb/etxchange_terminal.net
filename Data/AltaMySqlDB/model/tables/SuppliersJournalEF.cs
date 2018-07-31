using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AltaMySqlDB.model.tables
{
    [Table(name:"suppliersjournal")]
    public class SuppliersJournalEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("supplier")]
        public int supplierid { get; set; }

        [ForeignKey("broker")]
        public int brokerid { get; set; }

        public string code { get; set; }
        public DateTime? regdate { get; set; }
        public int serialnumber { get; set; }
        #endregion

        #region Foreign keys
        public virtual SupplierEF supplier { get; set; }
        public virtual BrokerEF broker { get; set; }
        #endregion
    }
}
