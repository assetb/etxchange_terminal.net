using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "banks")]
    public class BankEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        public string name { get; set; }

        [ForeignKey("company")]
        public int? companyid { get; set; }
        #endregion

        #region Foreign Objects
        public virtual CompanyEF company { get; set; }
        #endregion
    }
}
