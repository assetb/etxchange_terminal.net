using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO.reports
{
    public class DebtorsList
    {
        public int supplierId { get; set; }
        public string companyName { get; set; }
        public decimal debt { get; set; }
        public string companyBin { get; set; }
        public string telephones { get; set; }
        public string address { get; set; }
        public string customerName { get; set; }
        public int customerId { get; set; }
    }
}
