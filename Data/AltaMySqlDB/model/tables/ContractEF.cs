using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "contracts")]
    public class ContractEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("company")]
        public int? companyid { get; set; }

        public string number { get; set; }

        public DateTime? agreementdate { get; set; }

        public DateTime? terminationdate { get; set; }

        [ForeignKey("bank")]
        public int? bankid { get; set; }

        [ForeignKey("document")]
        public int? documentId { get; set; }

        public DateTime? updatedate { get; set; }

        public DateTime? createdate { get; set; }

        [ForeignKey("contracttype")]
        public int? contracttypeid { get; set; }

        [ForeignKey("currency")]
        public int? currencyid { get; set; }

        [ForeignKey("broker")]
        public int? brokerid { get; set; }

        [ForeignKey("site")]
        public int? siteid { get; set; }

        [ForeignKey("author")]
        public int? authorid { get; set; }

        public int? scantype { get; set; }
        #endregion

        #region Foreign Objects
        public virtual CompanyEF company { get; set; }
        public virtual BankEF bank { get; set; }
        public virtual DocumentEF document { get; set; }
        public virtual ContractTypeEF contracttype { get; set; }
        public virtual CurrencyEF currency { get; set; }
        public virtual BrokerEF broker { get; set; }
        public virtual SiteEF site { get; set; }
        public virtual TraderEF author { get; set; }
        #endregion
    }
}
