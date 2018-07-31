using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "sites")]
    public class SiteEF
    {
        #region Columns
        [Key]
        public int id { get; set; }
        
        public string name { get; set; }

        public string link { get; set; }

        [ForeignKey("company")]
        public int? companyId { get; set; }
        #endregion

        #region Foreign Objects
        public virtual CompanyEF company { get; set; }
        #endregion
    }
}
