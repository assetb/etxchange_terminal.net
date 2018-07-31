using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "roles")]
    public class RoleEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [MaxLength(50)]
        public string name { get; set; }
        
        public string description { get; set; }

        [Column("access_string")]
        public string accessString { get; set; }
        #endregion
    }
}
