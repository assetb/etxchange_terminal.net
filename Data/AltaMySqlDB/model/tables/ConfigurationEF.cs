using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "configurations")]
    public class ConfigurationEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [MaxLength(45)]
        public string name { get; set; }

        [MaxLength(45)]
        public string url { get; set; }

        [MaxLength(45)]
        public string user { get; set; }

        [MaxLength(45)]
        public string pass { get; set; }
        #endregion
    }
}
