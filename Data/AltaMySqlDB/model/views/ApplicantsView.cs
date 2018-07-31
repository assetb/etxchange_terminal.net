using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.views
{
    [Table("applicantsview")]
    public class ApplicantsView
    {
        public DateTime orderdate { get; set; }
        public string auctionnumber { get; set; }
        public string lotnumber { get; set; }
        public DateTime auctiondate { get; set; }
        [Key]
        public int supplier { get; set; }
    }
}
