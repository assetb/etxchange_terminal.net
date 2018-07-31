using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.views {
    [Table("listserv_view")]
    public class ListServView {
        [Key]
        public int id { get; set; }
        public string broker { get; set; }
        public int brokerid { get; set; }
        public DateTime createdate { get; set; }
        public int number { get; set; }
        public string status { get; set; }
        public int statusid { get; set; }
        public DateTime? departuredate { get; set; }
        public int envelopcount { get; set; }
    }
}
