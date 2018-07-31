using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "currencies")]
    public class CurrencyEF
    {
        #region Columns
        [Key]
        public int id { get; set; }
        
        [MaxLength(45)]
        public string name { get; set; }

        [MaxLength(45)]
        public string code { get; set; }
        #endregion
    }
}
