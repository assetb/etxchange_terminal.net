using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables {

    [Table(name: "serialnumbers")]
    public class SerialNumberEF {
        [Key]
        public int id { get; set; }
        public int number { get; set; }
        public string description { get; set; }
    }
}
