using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name: "persons")]
    public class PersonEF
    {
        #region Columns
        [Key]
        public int id { get; set; }

        [MaxLength(45)]
        public string name { get; set; }

        [MaxLength(45)]
        public string tel { get; set; }
        #endregion

        #region Foreign Objects
        #endregion
    }
}
