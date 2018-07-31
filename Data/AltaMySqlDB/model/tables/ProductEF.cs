using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.tables
{
    [Table("goods")]
    public class ProductEF
    {
        #region Columns
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        [ForeignKey("unit")]
        public int? unitid { get; set; }
        #endregion

        #region Foreign Objects
        public virtual UnitEF unit { get; set; }
        #endregion

        public virtual ICollection<CompanyEF> companies { get; set; }
    }
}
