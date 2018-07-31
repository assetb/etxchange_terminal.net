using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.views {
    [Table("companies_with_contract_view")]
    public class CompaniesWithContractView {
        [Key]
        public int companyId { get; set; }
        public string companyName { get; set; }
        public string companyBin { get; set; }
        public string contractNumber { get; set; }
        public int brokerId { get; set; }
    }
}
