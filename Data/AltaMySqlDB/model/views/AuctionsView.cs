using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.views {
    [Table("auctionsview")]
    public class AuctionsView {
        [Key]
        public int id { get; set; }
        public DateTime? date { get; set; }
        public string number { get; set; }
        public int? customerid { get; set; }
        public string customer { get; set; }
        public int? siteid { get; set; }
        public string site { get; set; }
        public int? statusid { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public decimal? sum { get; set; }
        public DateTime? applicantsdeadline { get; set; }
    }
}
