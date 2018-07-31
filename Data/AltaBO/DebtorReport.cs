using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO {
    public class DebtorReport {
        public string clientName { get; set; }
        public string clientBIN { get; set; }
        public decimal credit { get; set; }
        public decimal debit { get; set; }
        public decimal result { get; set; }
        public string brokerName { get; set; }
        public bool balance { get; set; }
    }
}
