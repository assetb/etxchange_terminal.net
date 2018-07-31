using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.views {
    [Table("envelops_view")]
    public class EnvelopsView {
        [Key]
        public int id { get; set; }
        [NotMapped]
        public int serialnumber { get; set; }
        public string company { get; set; }
        public string index { get; set; }
        public string address { get; set; }
        public string phones { get; set; }
        public int companyid { get; set; }
        public string code { get; set; }
        public int listservid { get; set; }
        public int envelopcontentcount { get; set; }
    }
}
