using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables {
    [Table(name: "countries")]
    public class CountryEF {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public int? len_bik { get; set; }
        public int? len_iik { get; set; }
        public int? len_bin { get; set; }
        public int? world_code { get; set; }
        public int? code_1c { get; set; }
    }
}
