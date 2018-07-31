using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "employees")]
    public class EmployeeEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("company")]
        public int companyid { get; set; }

        [ForeignKey("person")]
        public int personid { get; set; }
        public string position { get; set; }
        #endregion


        #region Foreign Objects
        public virtual CompanyEF company { get; set; }
        public virtual PersonEF person { get; set; }
        #endregion
    }
}
