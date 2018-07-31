using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO {
    public class Supplier {
        public int Id { get; set; }
        public int companyId { get; set; }
        public string Name { get; set; }
        public string BIN { get; set; }
        public string Contacts { get; set; }
        public string Emails { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
    }
}
