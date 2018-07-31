using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaMySqlDB.model.catalogs {
    public class EventEF {
        public int id { get; set; }
        public string description { get; set; }
        public int belongs { get; set; }
    }
}
