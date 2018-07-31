using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO {
    public class MailDeliveryException {
        public int id { get; set; }
        public string lot_Name { get; set; }
        public DateTime date { get; set; }
        public string customer { get; set; }
    }
}
