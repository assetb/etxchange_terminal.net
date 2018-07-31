using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AltaMySqlDB.model.tables
{
    [Table(name:"traders")]
    public class TraderEF
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("person")]
        public int personid { get; set; }

        [ForeignKey("broker")]
        public int? brokerid { get; set; }                

        public virtual PersonEF person { get; set; }
        public virtual BrokerEF broker { get; set; }
    }
}
