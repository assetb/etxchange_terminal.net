using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "customers")]
    public class CustomerEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [ForeignKey("company")]
        public int companiesid { get; set; }
        #endregion

        #region Foreign Objects
        public virtual CompanyEF company { get; set; }
        #endregion
    }
}
