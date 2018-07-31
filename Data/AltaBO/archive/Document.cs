using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO.archive {
    public class Document {
        public int id { get; set; }
        public String number { get; set; }
        public String name { get; set; }
        public string description { get; set; }
        public int type { get; set; }
        public String author { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime uploadDate { get; set; }
        public string year { get; set; }
        public string broker { get; set; }
        public string case_ {get;set;}
        public string volume { get; set; }
        public int? serialNumber { get; set; }
        public string exchange { get; set; }
        public string customer { get; set; }
        public string auctionDate { get; set; }
        public string auctionNumber { get; set; }
        public string company { get; set; }
        public int? linkId { get; set; }
    }
}
