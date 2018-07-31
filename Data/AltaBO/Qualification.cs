using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO {
    public class Qualification {
        public int id { get; set; }
        public int auctionId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string note { get; set; }
        public bool file { get; set; }
    }
}
