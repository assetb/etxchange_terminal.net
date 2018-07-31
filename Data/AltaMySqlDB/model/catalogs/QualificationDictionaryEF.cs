using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaMySqlDB.model.tables;

namespace AltaMySqlDB.model.catalogs {
    public class QualificationDictionaryEF {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int siteid { get; set; }

        public virtual SiteEF site { get; set; }
    }
}
