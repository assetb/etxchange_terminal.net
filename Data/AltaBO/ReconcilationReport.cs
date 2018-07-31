using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO {
    public class ReconcilationReport {
        public string clientName { get; set; }
        public string contractNum { get; set; }
        public decimal credit { get; set; }
        public decimal debit { get; set; }
        public string docDate { get; set; }
        public string docName { get; set; }
        public string currency { get; set; }
    }
}
