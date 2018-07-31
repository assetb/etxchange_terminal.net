using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO {
    public class Acl {
        public int id { get; set; }
        public int caseId { get; set; }
        public bool read { get; set; }
        public bool write { get; set; }
        public bool readWrite { get; set; }
    }
}
