using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaMySqlDB.model.tables;

namespace AltaMySqlDB.model.views {
    [Table("suppliers_with_contract_view")]
    public class SuppliersWithContractsView {
        [Key]
        public int id { get; set; }

        [ForeignKey("company")]
        public int companyid { get; set; }
        public string name { get; set; }

        #region Foreign 
        public virtual CompanyEF company { get; set; }
        #endregion
    }
}
