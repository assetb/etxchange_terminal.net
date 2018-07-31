using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "types")]
    public class TypeEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [MaxLength(100)]
        public string name { get; set; }
        #endregion
    }
}
