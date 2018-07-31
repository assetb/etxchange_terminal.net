using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltaMySqlDB.model.catalogs;

namespace AltaMySqlDB.model.tables
{
    public class NotificationEF
    {
        public int id { get; set; }
        public int auctionId { get; set; }
        public int supplierId { get; set; }
        public int eventId { get; set; }
        public string description { get; set; }

        public virtual AuctionEF auction { get; set; }
        public virtual SupplierEF supplier { get; set; }
        public virtual EventEF event_ { get; set; }
    }
}
