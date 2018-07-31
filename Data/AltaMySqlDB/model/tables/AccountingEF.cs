using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "accounting")]
    public class AccountingEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [MaxLength(45)]
        public string code { get; set; }
        
        [ForeignKey("company")]
        public int companyid { get; set; }

        [ForeignKey("configuration")]
        public int configurationid { get; set; }
        #endregion

        #region Foreign Objects
        public CompanyEF company { get; set; }
        #endregion
    }
}
