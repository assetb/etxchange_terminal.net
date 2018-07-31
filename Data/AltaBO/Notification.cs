using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaBO {
    public class Notification {
        public int id { get; set; }
        public int auctionId { get; set; }
        public int supplierId { get; set; }
        public int eventId { get; set; }
        public string eventDescription { get; set; }
        public string description { get; set; }
    }
}
