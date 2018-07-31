using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name:"suppliers")]
    public class SupplierEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("company")]
        public int companyid { get; set; }
        #endregion

        #region Foreign keys
        public virtual CompanyEF company { get; set; }
        #endregion

        #region Foreign Collections
        public virtual ICollection<SuppliersJournalEF> supplierJournals { get; set; }
        #endregion
    }
}
