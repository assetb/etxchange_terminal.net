using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name:"units")]
    public class UnitEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [MaxLength(45)]
        public string name { get; set; }

        [MaxLength(150)]
        public string description { get; set; }
        #endregion
    }
}
