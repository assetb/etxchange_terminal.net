using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.views {
    [Table("auctionsresultview")]
    public class AuctionsResultView {
        [Key]
        public int id { get; set; }
        public DateTime date { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string bin { get; set; }
        public decimal startprice { get; set; }
        public decimal minimalprice { get; set; }
        public decimal reward { get; set; }
    }
}
